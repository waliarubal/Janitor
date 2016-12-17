using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class LicenseManager: IObserver
    {
        const string LicenseFileName = "License.key";
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

        public void LoadLicense()
        {
            var licenseKey = Path.Combine(KnownPaths.Instance.ApplicationDirectory, LicenseFileName);
            if (File.Exists(licenseKey))
            {
                licenseKey = File.ReadAllText(licenseKey);
                _license.Validate(licenseKey);
            }

            Subject.Instance.NotifyAllObservers(this, MessageCode.LicenseChanged);
        }

        public bool ValidateLicense(string key)
        {
            var result = _license.Validate(key);
            Subject.Instance.NotifyAllObservers(this, MessageCode.LicenseChanged);

            return result;
        }

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            
        }
    }
}
