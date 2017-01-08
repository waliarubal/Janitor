using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Views;
using NullVoidCreations.Janitor.Core.Models;

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

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SignalHost.Instance.RaiseSignal(this, Signal.Initialized);
            SignalHost.Instance.RaiseSignal(this, Signal.SystemInformationLoaded);

            if (SettingsManager.Instance.RunScanAtLaunch)
                SignalHost.Instance.RaiseSignal(this, Signal.ScanTrigerred, ScanType.SmartScan);

            _worker.DoWork -= new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.Dispose();
            _worker = null;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            FirstTimeExecution();

            // configure to run at startup
            if (SettingsManager.Instance.RunAtBoot)
                new ScheduleSilentRunCommand(null).Execute(SettingsManager.Instance.RunAtBoot);

            // program update
            var update = new UpdateCommand(null, UpdateCommand.UpdateType.Program, true);
            update.IsEnabled = true;
            update.Execute(null);
            while (update.IsExecuting)
                Thread.Sleep(1000);

            // plugins update
            update = new UpdateCommand(null, UpdateCommand.UpdateType.Plugin, true);
            update.IsEnabled = true;
            update.Execute(null);
            while (update.IsExecuting)
                Thread.Sleep(1000);

            // load license
            LicenseExManager.Instance.LoadLicense();

            // load plugins
            PluginManager.Instance.LoadPlugins();

            // load system information
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.ComputerSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.OperatingSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.Processor);
        }

        /// <summary>
        /// This method takes care of first time initialization.
        /// </summary>
        void FirstTimeExecution()
        {
            var firstExecutionDate = SettingsManager.Instance.FirstExecutionDate;
            if (default(DateTime) != firstExecutionDate)
                return;

            SettingsManager.Instance.FirstExecutionDate = DateTime.Now;

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
