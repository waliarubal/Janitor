using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISignalObserver
    {
        internal const string ProductName = "PC MECHANIC PRO™";

        static Mutex _mutex;
        BackgroundWorker _worker;

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //TODO: add exception handeler
        }

        protected override void OnExit(ExitEventArgs e)
        {
            App.Current.DispatcherUnhandledException -= new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            base.OnExit(e);
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            App.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);

            // ensure single instance
            bool createdNew;
            _mutex = new Mutex(true, ProductName, out createdNew);
            if (!createdNew)
            {
                NativeApiHelper.Instance.ActivateOtherWindow(ProductName);
                Shutdown(0);
                return;
            }

            base.OnStartup(e);

            // initialization
            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.RunWorkerAsync();
            
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
            LicenseExManager.Instance.LoadLicense();

            // load plugins
            PluginManager.Instance.LoadPlugins();

            // load system information
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.ComputerSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.OperatingSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.Processor);
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SignalHost.Instance.RaiseSignal(this, Signal.Initialized);

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

            _worker.DoWork -= new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.Dispose();
            _worker = null;
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
            SettingsManager.Instance.RunAtBoot = true;
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            
        }
    }
}
