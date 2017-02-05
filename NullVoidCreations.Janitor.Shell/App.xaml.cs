using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NullVoidCreations.Janitor.Shared;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex _mutex;

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //TODO: add exception handeler
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //TODO: add exception handeler
        }

        protected override void OnExit(ExitEventArgs e)
        {
            WorkQueueManager.Instance.Dispose();
            SettingsManager.Instance.Dispose();
            Core.LicenseManager.Instance.Dispose();
            SignalHost.Instance.Dispose();
            App.Current.DispatcherUnhandledException -= new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            base.OnExit(e);
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            App.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);

            // ensure single instance
            bool createdNew;
            _mutex = new Mutex(true, Constants.ProductName, out createdNew);
            if (!createdNew)
            {
                Win32Helper.Instance.ActivateOtherWindow(Constants.ProductName);
                Shutdown(0);
                return;
            }

            base.OnStartup(e);

            UiHelper.Instance.Resources["ProductName"] = Constants.ProductName;
            UiHelper.Instance.Resources["ProductTagLine"] = Constants.ProductTagLine;
            UiHelper.Instance.Resources["ProductVersion"] = Constants.ProductVersion.ToString();
            UiHelper.Instance.Resources["SupportPhone"] = Constants.SupportPhone;
            UiHelper.Instance.Resources["SupportEmail"] = Constants.SupportEmail;

            LanguageManager.Instance.GetLanguageFiles();

            // initialization
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            worker.RunWorkerAsync(worker);

            SettingsManager.Instance.Load(Constants.UpdatesMetadataUrl);
            SettingsManager.Instance.Load(Constants.WebLinksUrl);
            
            MainWindow = new MainView();
            MainWindow.Show();

            CommandLineManager.Instance.LoadArguments(e.Args);
            CommandLineManager.Instance.ProcessArguments();
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            FirstTimeExecution();

            // skip UAC
            var skipUac = new SkipUacCommand(null);
            skipUac.Execute(SettingsManager.Instance.SkipUac);

            // run at startup, disable if UAC skipping is not enabled
            SettingsManager.Instance.RunAtBoot = SettingsManager.Instance.RunAtBoot && SettingsManager.Instance.SkipUac;
            var runAtBoot = new ScheduleSilentRunCommand(null);
            runAtBoot.Execute(SettingsManager.Instance.RunAtBoot);

            // load license
            Core.LicenseManager.Instance.LoadLicense();

            // load plugins
            PluginManager.Instance.LoadPlugins();

            // load system information
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.ComputerSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.OperatingSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.Processor);

            e.Result = e.Argument;
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SignalHost.Instance.RaiseSignal(Signal.Initialized);

            // trigger work pipeline
            if (SettingsManager.Instance.RunProgramUpdateAtLaunch)
                WorkQueueManager.Instance.AddWork(WorkSignal.ProgramUpdate);
            if (SettingsManager.Instance.RunPluginUpdateAtLaunch)
            {
                WorkQueueManager.Instance.AddWork(WorkSignal.PluginUpdate);
                WorkQueueManager.Instance.AddWork(WorkSignal.ShowHome);
            }
            if (SettingsManager.Instance.RunScanAtLaunch)
                WorkQueueManager.Instance.AddWork(WorkSignal.SmartScan);
                
            WorkQueueManager.Instance.AddWork(WorkSignal.ShowTrialWarning);
            WorkQueueManager.Instance.DoWork();

            var worker = e.Result as BackgroundWorker;
            worker.DoWork -= new DoWorkEventHandler(Worker_DoWork);
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            worker.Dispose();
            worker = null;
        }

        /// <summary>
        /// This method takes care of first time initialization.
        /// </summary>
        void FirstTimeExecution()
        {
            var firstExecutionDate = SettingsManager.Instance.FirstExecution;
            if (default(DateTime) != firstExecutionDate)
                return;

            SettingsManager.Instance.FirstExecution = DateTime.Now;

            SettingsManager.Instance.RunProgramUpdateAtLaunch = true;
            SettingsManager.Instance.RunPluginUpdateAtLaunch = true;
            SettingsManager.Instance.RunScanAtLaunch = true;
            SettingsManager.Instance.SkipUac = true;
            SettingsManager.Instance.RunAtBoot = true;
        }
    }
}
