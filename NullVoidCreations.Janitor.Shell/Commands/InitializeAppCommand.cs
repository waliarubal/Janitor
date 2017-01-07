using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;
using System;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    /// <summary>
    /// This is a special command which must be executed once dueing app start.
    /// </summary>
    class InitializeAppCommand: AsyncDelegateCommand, ISignalObserver
    {
        public InitializeAppCommand()
            : base(null)
        {
            IsEnabled = true;
        }

        protected override object ExecuteOverride(object parameter)
        {
            FirstTimeExecution();

            // load license
            LicenseManager.Instance.LoadLicense();

            // load plugins
            PluginManager.Instance.LoadPlugins();

            // load system information
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.ComputerSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.OperatingSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.Processor);

            SignalHost.Instance.RaiseSignal(this, Signal.SystemInformationLoaded);
            return null;
        }

        protected override void ExecuteSuccessOverride(object result)
        {
            SignalHost.Instance.RaiseSignal(this, Signal.Initialized);
            IsEnabled = false;
        }

        /// <summary>
        /// This method takes care of first time initialization.
        /// TODO: move this code to app init command
        /// </summary>
        void FirstTimeExecution()
        {
            var firstExecutionDate = SettingsManager.Instance.FirstExecutionDate;
            if (default(DateTime) != firstExecutionDate)
                return;

            SettingsManager.Instance.FirstExecutionDate = DateTime.Now;

            SettingsManager.Instance.RunPluginUpdateAtLaunch = true;
            SettingsManager.Instance.RunPluginUpdateAtLaunch = true;
            SettingsManager.Instance.RunPluginUpdateAtLaunch = true;

            SettingsManager.Instance.RunPluginUpdateAtLaunch = true;
            //new RunAtStartupCommand(ViewModel).Execute(SettingsManager.Instance.RunPluginUpdateAtLaunch);
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            
        }
    }
}
