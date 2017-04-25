using System;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class HelpViewModel : ViewModelBase, ISignalObserver
    {
        CommandBase _logOut;

        #region constructor / destructor

        public HelpViewModel()
        {
            SignalHost.Instance.AddObserver(this);
        }

        ~HelpViewModel()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        public string RegisteredTo
        {
            get { return GetValue<string>("RegisteredTo"); }
            private set { this["RegisteredTo"] = value; }
        }

        public string SerialKey
        {
            get { return GetValue<string>("SerialKey"); }
            private set { this["SerialKey"] = value; }
        }

        public DateTime ExpiryDate
        {
            get { return GetValue<DateTime>("ExpiryDate"); }
            private set { this["ExpiryDate"] = value; }
        }

        public bool IsTrial
        {
            get { return GetValue<bool>("IsTrial"); }
            private set { this["IsTrial"] = value; }
        }

        public bool IsAuthenticated
        {
            get { return GetValue<bool>("IsAuthenticated"); }
            private set { this["IsAuthenticated"] = value; }
        }

        #endregion

        #region commands

        public CommandBase LogOut
        {
            get
            {
                if (_logOut == null)
                    _logOut = new DelegateCommand(this, ExecuteLogOut) { IsEnabled = true };

                return _logOut;
            }
        }

        #endregion

        void ExecuteLogOut(object argument)
        {
            LicenseManager.Instance.LogOut();
            SignalHost.Instance.RaiseSignal(Signal.Authentication, LicenseManager.Instance.IsAuthenticated);
        }

        public void SignalReceived(Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Initialized:
                    SignalReceived(Signal.LicenseChanged, data);
                    break;

                case Signal.LicenseChanged:
                    RegisteredTo = LicenseManager.Instance.License.Name;
                    ExpiryDate = LicenseManager.Instance.License.ExpirationDate;
                    SerialKey = LicenseManager.Instance.License.SerialKey;
                    IsTrial = LicenseManager.Instance.License.IsTrial;
                    break;

                case Signal.Authentication:
                    IsAuthenticated = (bool)data[0];
                    break;
            }
        }

    }
}
