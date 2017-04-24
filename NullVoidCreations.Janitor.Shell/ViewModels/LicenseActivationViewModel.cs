using System;
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

        public LicenseActivationViewModel()
        {
            _purchaseLicense = new RunProgramCommand(this);
            _activate = new AsyncDelegateCommand(this, null, ExecuteActivate, ActivationComplete);
            _copyFromClipboard = new DelegateCommand(this, ExecuteCopyFromClipboard);
            _copyFromClipboard.IsEnabled = true;
        }

        #region properties

        public string BuyNowUrl
        {
            get { return TemporarySettingsManager.Instance["BuyNowUrl"] as string; }
        }

        public string SerialKey
        {
            get { return GetValue<string>("SerialKey"); }
            set
            {
                this["SerialKey"] = value;
                Activate.IsEnabled = !string.IsNullOrEmpty(value);
            }
        }

        public string ErrorMessage
        {
            get { return GetValue<string>("ErrorMessage"); }
            private set { this["ErrorMessage"] = value; }
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
            var text = Clipboard.GetText();
            if (string.IsNullOrEmpty(text))
                return;
            if (text.Length >= 23)
                text = text.Substring(0, 23);

            SerialKey = text;
        }

        object ExecuteActivate(object window)
        {
            ErrorMessage = null;
            try
            {
                LicenseManager.Instance.Activate(SerialKey);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                SignalHost.Instance.RaiseSignal(Signal.LicenseChanged, ErrorMessage);
            }

            return window;
        }

        void ActivationComplete(object win)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                var window = win as CustomWindow;
                window.DialogResult = true;
                window.Close();
            }
        }
    }
}
