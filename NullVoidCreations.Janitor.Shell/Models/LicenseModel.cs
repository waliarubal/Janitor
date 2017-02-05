using System;
using NullVoidCreations.Janitor.Licensing;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class LicenseModel: NotificationBase, IDisposable
    {
        LicenseEx _license;

        public LicenseModel()
        {
            _license = new LicenseEx(null);
        }

        ~LicenseModel()
        {
            Dispose();
        }

        #region properties

        public string SerialKey
        {
            get { return _license.SerialKey; }
        }

        public string Email
        {
            get { return _license.Email; }
        }

        public DateTime IssueDate
        {
            get { return _license.IssueDate; }
        }

        public DateTime ExpirationDate
        {
            get { return _license.ExpirationDate; }
        }

        public bool IsTrial
        {
            get { return !_license.IsActivated; }
        }

        #endregion

        #region private methods

        void RaisePropertyChanged()
        {
            RaisePropertyChanged("SerialKey");
            RaisePropertyChanged("Email");
            RaisePropertyChanged("IssueDate");
            RaisePropertyChanged("ExpirationDate");
            RaisePropertyChanged("IsTrial");
        }

        #endregion

        internal void Load(string licenseFile)
        {
            _license = new LicenseEx(licenseFile);
            RaisePropertyChanged();
        }

        internal void Activate(string serialKey, string licenseFile)
        {
            _license.Activate(serialKey, licenseFile);
            RaisePropertyChanged();
        }

        public void Dispose()
        {
            _license.Dispose();
        }
    }
}
