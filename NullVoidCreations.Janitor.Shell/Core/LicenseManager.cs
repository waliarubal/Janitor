using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Models;
using System;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class LicenseManager: IObserver
    {
        internal const string EncryptionKey = "QFByb3Blcl9QYXRvbGEhMjAxNQ==";

        static volatile LicenseManager _instance;
        LicenseModel _license;

        #region constructor / destructor

        private LicenseManager()
        {
            _license = new LicenseModel();

            Subject.Instance.AddObserver(this);
        }

        ~LicenseManager()
        {
            Subject.Instance.RemoveObserver(this);
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

        #endregion

        public string GenerateLicenseKey(string email, int days)
        {
            return new LicenseModel().Generate(DateTime.Now, DateTime.Now.AddDays(days), email);
        }

        /// <summary>
        /// This method loads and validates license key saved in settings.
        /// </summary>
        public void LoadLicense()
        {
            _license.Validate(SettingsManager.Instance.LicenseKey);
            Subject.Instance.NotifyAllObservers(this, MessageCode.LicenseChanged);
        }

        /// <summary>
        /// This method validates license key entered by user using UI and returns any error that occured.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ValidateLicense(string key)
        {
            var errorMessage = "License key not entered.";
            if (!string.IsNullOrEmpty(key))
                errorMessage = _license.Validate(key);

            SettingsManager.Instance.LicenseKey = key;
            Subject.Instance.NotifyAllObservers(this, MessageCode.LicenseChanged, errorMessage);

            return errorMessage;
        }

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            
        }
    }
}
