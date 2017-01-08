using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Models;
using System;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class LicenseExManager: ISignalObserver
    {
        internal const string EncryptionKey = "QFByb3Blcl9QYXRvbGEhMjAxNQ==";

        static volatile LicenseExManager _instance;
        LicenseModel _license;

        #region constructor / destructor

        private LicenseExManager()
        {
            _license = new LicenseModel();
        }

        ~LicenseExManager()
        {
            
        }

        #endregion

        #region properties

        public static LicenseExManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LicenseExManager();

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
            SignalHost.Instance.RaiseSignal(this, Signal.LicenseChanged);
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
            SignalHost.Instance.RaiseSignal(this, Signal.LicenseChanged, errorMessage);

            return errorMessage;
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            
        }
    }
}
