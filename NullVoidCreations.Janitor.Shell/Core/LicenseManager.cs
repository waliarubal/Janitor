using System;
using System.IO;
using NullVoidCreations.Janitor.Licensing;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class LicenseManager: IDisposable
    {
        static volatile LicenseManager _instance;
        readonly string _licenseFile;
        LicenseModel _license;
        Customer _customer;

        #region constructor / destructor

        private LicenseManager()
        {
            _licenseFile = Path.Combine(KnownPaths.Instance.MyDataDirectory, "License.dat");
            _license = new LicenseModel();
        }

        ~LicenseManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
                _license.Dispose();
        }

        #endregion

        #region properties

        public static LicenseManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LicenseManager();

                return _instance;
            }
        }

        public LicenseModel License
        {
            get { return _license; }
        }

        public string LicenseFile
        {
            get { return _licenseFile; }
        }

        public Customer Customer
        {
            get { return _customer; }
        }

        public bool IsAuthenticated
        {
            get { return _customer != null; }
        }

        #endregion

        public License AddTrialLicense()
        {
            if (File.Exists(LicenseFile))
                throw new Exception("Please purchase time extention to the license activated on this machine.");

            License license;
            if (Customer.AddLicense(DateTime.Now, DateTime.Now.AddDays(90), out license))
                return license;

            return null;
        }

        public bool Activate(string serialKey)
        {
            return License.Activate(serialKey);
        }

        public bool RemoveLicense(string serialKey)
        {
            // deactivate license form machine
            if (License != null && License.SerialKey.Equals(serialKey))
            {
                SettingsManager.Instance["CustomerName"] = null;
                SettingsManager.Instance["CustomerEmail"] = null;
                License.Dispose();
            }

            return Customer.RemoveLicense(serialKey, LicenseFile);
        }

        public Exception ChangePassword(string password, string newPassword, string confirmPassword)
        {
            //TODO: work here
            LogOut();
            return null;
        }

        public Exception Login(string email, string password)
        {
            if (IsAuthenticated)
                return null;
            
            try
            {
                _customer = new Customer().Login(email, password);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public void LogOut()
        {
            if (IsAuthenticated)
                _customer = null;
        }

        /// <summary>
        /// This method loads and validates license key saved in settings.
        /// </summary>
        public void LoadLicense()
        {
            if (File.Exists(_licenseFile))
                _license.Load(_licenseFile);
        }
    }
}
