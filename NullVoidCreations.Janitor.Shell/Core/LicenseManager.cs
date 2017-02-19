using System;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class LicenseManager: IDisposable
    {
        static volatile LicenseManager _instance;
        readonly string _licenseFile;
        LicenseModel _license;

        #region constructor / destructor

        private LicenseManager()
        {
            _licenseFile = Path.Combine(KnownPaths.Instance.ApplicationDirectory, "License.dat");
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

        #endregion

        /// <summary>
        /// This method loads and validates license key saved in settings.
        /// </summary>
        public void LoadLicense()
        {
            if (File.Exists(_licenseFile))
            {
                _license.Load(_licenseFile);
                SignalHost.Instance.RaiseSignal(Signal.LicenseChanged);
            }
        }
    }
}
