using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;

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

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            
        }
    }
}
