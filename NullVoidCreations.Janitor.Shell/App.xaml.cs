using System;
using System.ComponentModel;
using System.Diagnostics;
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

            if (!RunWithElivatedPrivilages(e.Args))
                return;

            base.OnStartup(e);

            UiHelper.Instance.Resources["ProductName"] = Constants.ProductName;
            UiHelper.Instance.Resources["ProductTagLine"] = Constants.ProductTagLine;
            UiHelper.Instance.Resources["ProductVersion"] = Constants.ProductVersion.ToString();
            UiHelper.Instance.Resources["SupportPhone"] = Constants.SupportPhone;
            UiHelper.Instance.Resources["SupportEmail"] = Constants.SupportEmail;

            FirstTimeExecution();

            LanguageManager.Instance.GetLanguageFiles();
            LanguageManager.Instance.LoadLanguage(SettingsManager.Instance.Language);

            // background bootstraping
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            var args = new object[] { worker, e.Args };
            worker.RunWorkerAsync(args);

            MainWindow = new MainView();
            MainWindow.Show();
        }

        #region background bootstraping

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            // skip UAC
            var skipUac = new SkipUacCommand(null);
            if (skipUac.IsEnabled)
                skipUac.Execute(SettingsManager.Instance.SkipUac);

            // run at startup
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

            SettingsManager.Instance.Load(Constants.UpdatesMetadataUrl);
            SettingsManager.Instance.Load(Constants.WebLinksUrl);

            e.Result = e.Argument;
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var args = e.Result as object[];

            SignalHost.Instance.RaiseSignal(Signal.Initialized);

            // process command line
            CommandLineManager.Instance.LoadArguments(args[1] as string[]);
            CommandLineManager.Instance.ProcessArguments();

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

            // dispose off worker
            var worker = args[0] as BackgroundWorker;
            worker.DoWork -= new DoWorkEventHandler(Worker_DoWork);
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            worker.Dispose();
            worker = null;
        }

        #endregion

        bool RunWithElivatedPrivilages(string[] arguments)
        {
            if (Debugger.IsAttached || Constants.IsAdministrator)
                return true;

            Shutdown(0);
            var argsString = FileSystemHelper.Instance.GetArgumentsString(arguments);
            if (SettingsManager.Instance.SkipUac)
            {
                // create UAC skipping task
                var skipUac = new SkipUacCommand(null);
                if (skipUac.IsEnabled)
                    skipUac.Execute(true);

                // execute UAC skipping task
                FileSystemHelper.Instance.RunScheduledTask(SkipUacCommand.SkipUacTask);
            }
            else
                FileSystemHelper.Instance.RunProgram(Constants.ExecutableFile, argsString, true);

            return false;
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
            SettingsManager.Instance.SkipUac = Constants.IsUacSupported;
            SettingsManager.Instance.RunAtBoot = true;
            SettingsManager.Instance.Language = "English";
        }
    }
}
