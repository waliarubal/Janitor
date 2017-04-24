using System;
using System.Collections.ObjectModel;
using System.Windows;
using NullVoidCreations.Janitor.Licensing;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class LicenseManagemntViewModel: ViewModelBase
    {
        CommandBase _delete, _copy, _add, _refresh;
        ObservableCollection<License> _licenses;

        #region properties

        public License SelectedLicense
        {
            get { return GetValue<License>("SelectedLicense"); }
            set 
            { 
                this["SelectedLicense"] = value;
                Copy.RaiseCanExecuteChanged();
                Delete.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<License> Licenses
        {
            get
            {
                if (_licenses == null)
                    _licenses = new ObservableCollection<License>();

                return _licenses;
            }
        }

        #endregion

        #region commands

        public string CustomerName
        {
            get { return LicenseManager.Instance.Customer.Name; }
        }

        public CommandBase Refresh
        {
            get
            {
                if (_refresh == null)
                    _refresh = new DelegateCommand(this, ExecuteRefresh) { IsEnabled = true };

                return _refresh;
            }
        }

        public CommandBase Delete
        {
            get
            {
                if (_delete == null)
                    _delete = new AsyncDelegateCommand(this, CanExecute, ExecuteDelete, DeleteExecuted, ConfirmDelete);

                return _delete;
            }
        }

        public CommandBase Add
        {
            get
            {
                if (_add == null)
                    _add = new AsyncDelegateCommand(this, null, ExecuteAdd, AddExecuted) { IsEnabled = true };

                return _add;
            }
        }

        public CommandBase Copy
        {
            get
            {
                if (_copy == null)
                    _copy = new DelegateCommand(this, CanExecute, ExecuteCopy);

                return _copy;
            }
        }

        #endregion

        void ExecuteRefresh(object parameter)
        {
            Licenses.Clear();
            if (LicenseManager.Instance.Customer.Licenses == null)
                return;

            foreach (var license in LicenseManager.Instance.Customer.Licenses)
                Licenses.Add(license);
        }

        object ExecuteAdd(object argument)
        {
            try
            {
                var license = LicenseManager.Instance.AddTrialLicense();
                if (license != null)
                {
                    if (LicenseManager.Instance.Activate(license.SerialKey))
                    {
                        LicenseManager.Instance.Customer.Refresh();
                        return license.SerialKey;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        void AddExecuted(object result)
        {
            if (result is Exception)
                UiHelper.Instance.Error("Failed to generate license. {0}", (result as Exception).Message);
            else
            {
                Refresh.Execute(null);
                SignalHost.Instance.RaiseSignal(Signal.LicenseChanged);
                UiHelper.Instance.Alert("License with serial key {0} generated and activated for this machine.", result);
            }
        }

        object ExecuteDelete(object argument)
        {
            try
            {
                LicenseManager.Instance.RemoveLicense(SelectedLicense.SerialKey);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        void DeleteExecuted(object result)
        {
            if (result == null)
            {
                SelectedLicense = null;
                Refresh.Execute(null);
                SignalHost.Instance.RaiseSignal(Signal.LicenseChanged);
            }
            else
                UiHelper.Instance.Error("Failed to remove the license with serial key {0}.", SelectedLicense.SerialKey);
        }

        bool ConfirmDelete(object argument)
        {
            return UiHelper.Instance.Question("Are you sure you want to remove the license with serial key {0}?", SelectedLicense.SerialKey);
        }

        bool CanExecute(object argument)
        {
            return SelectedLicense != null;
        }

        void ExecuteCopy(object argument)
        {
            Clipboard.SetText(SelectedLicense.SerialKey);
            UiHelper.Instance.Alert("Serial key {0} copied to clipboard.", SelectedLicense.SerialKey);
        }
    }
}
