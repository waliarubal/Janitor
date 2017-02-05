using System.Windows;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Controls;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class LicenseActivationViewModel: ViewModelBase
    {
        readonly CommandBase _purchaseLicense, _activate, _copyFromClipboard;
        string _licenseKey, _errorMessage;

        public LicenseActivationViewModel()
        {
            _purchaseLicense = new RunProgramCommand(this);
            _activate = new AsyncDelegateCommand(this, null, ExecuteValidate, ValidationComplete);
            _copyFromClipboard = new DelegateCommand(this, ExecuteCopyFromClipboard);
            _activate.IsEnabled = _copyFromClipboard.IsEnabled = true;
        }

        #region properties

        public string BuyNowUrl
        {
            get { return SettingsManager.Instance["BuyNowUrl"] as string; }
        }

        public string LicenseKey
        {
            get { return _licenseKey; }
            set
            {
                if (value == _licenseKey)
                    return;

                _licenseKey = value;
                RaisePropertyChanged("LicenseKey");
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                if (value == _errorMessage)
                    return;

                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        #endregion

        #region commands

        public CommandBase PurchaseLicense
        {
            get { return _purchaseLicense; }
        }

        public CommandBase Activate
        {
            get { return _activate; }
        }

        public CommandBase CopyFromClipboard
        {
            get { return _copyFromClipboard; }
        }

        #endregion

        void ExecuteCopyFromClipboard(object parameter)
        {
            LicenseKey = Clipboard.GetText();
        }

        object ExecuteValidate(object window)
        {
            return new object[]
            {
                window,
                LicenseManager.Instance.ValidateLicense(LicenseKey)
            };
        }

        void ValidationComplete(object result)
        {
            var res = result as object[];

            ErrorMessage = res[1] as string;
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                var window = res[0] as CustomWindow;
                window.DialogResult = true;
                window.Close();
            }
        }
    }
}
