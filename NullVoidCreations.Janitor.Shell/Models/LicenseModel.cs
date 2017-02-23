using System;
using NullVoidCreations.Janitor.Licensing;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class LicenseModel: NotificationBase, IDisposable
    {
        License _license;
        Customer _customer;

        public LicenseModel()
        {
            
        }

        ~LicenseModel()
        {
            Dispose();
        }

        #region properties

        public Customer Customer
        {
            get { return _customer; }
        }

        public string SerialKey
        {
            get { return _license == null ? string.Empty : _license.SerialKey; }
        }

        public string Email
        {
            get { return _customer == null ? string.Empty : _customer.Email; }
        }

        public string Name
        {
            get { return _customer == null ? string.Empty : _customer.Name; }
        }

        public DateTime IssueDate
        {
            get { return _license == null ? DateTime.MinValue : _license.IssueDate; }
        }

        public DateTime ExpirationDate
        {
            get { return _license == null ? DateTime.MinValue : _license.ExpirationDate; }
        }

        public bool IsTrial
        {
            get { return _license == null ? true : !_license.IsActivated; }
        }

        #endregion

        #region private methods

        void RaisePropertyChanged()
        {
            RaisePropertyChanged("SerialKey");
            RaisePropertyChanged("Email");
            RaisePropertyChanged("Name");
            RaisePropertyChanged("IssueDate");
            RaisePropertyChanged("ExpirationDate");
            RaisePropertyChanged("IsTrial");
        }

        #endregion

        internal void Load(string licenseFile)
        {
            if (_customer == null)
            {
                var email = SettingsManager.Instance["CustomerEmail"] as string;
                if (email == null)
                    return;

                _customer = new Customer();
                _customer.Email = email;
                _customer.Name = SettingsManager.Instance["CustomerName"] as string;
            }

            _license = _customer.LoadLicense(licenseFile);
            RaisePropertyChanged();
        }

        internal void Activate(string serialKey)
        {
            var customer = new Customer();
            _license = customer.ActivateLicense(serialKey, LicenseManager.Instance.LicenseFile);
            if (_license == null)
                return;

            _customer = customer;
            SettingsManager.Instance["CustomerName"] = _customer.Name;
            SettingsManager.Instance["CustomerEmail"] = _customer.Email;

            RaisePropertyChanged();
        }

        public void Dispose()
        {
            if (_license != null)
                _license.Dispose();
        }
    }
}
