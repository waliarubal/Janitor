using System;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Properties;
using NullVoidCreations.Janitor.Core.Models;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class SettingsManager
    {
        static volatile SettingsManager _instance;
        string _codeName, _appDirectory, _pluginsDirectory, _pluginsSearchFilter;

        private SettingsManager()
        {
            _codeName = "Janitor";
            _appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), CodeName);
            _pluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            _pluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";

            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
        }

        ~SettingsManager()
        {
            Settings.Default.Save();
        }

        #region properties

        public string CodeName
        {
            get { return _codeName; }
        }

        public string PluginsDirectory
        {
            get { return _pluginsDirectory; }
        }

        public string PluginsSearchFilter
        {
            get { return _pluginsSearchFilter; }
        }

        public ScanType LastScan
        {
            get { return (ScanType)Settings.Default.LastScanType; }
            set { Settings.Default.LastScanType = (byte) value; }
        }

        public DateTime LastScanTime
        {
            get { return Settings.Default.LastScanTime; }
            set { Settings.Default.LastScanTime = value; }
        }

        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingsManager();

                return _instance;
            }
        }

        public string LicenseKey
        {
            get { return Settings.Default.LicenseKey; }
            set
            {
                if (value == Settings.Default.LicenseKey)
                    return;

                Settings.Default.LicenseKey = value;
            }
        }

        #endregion
    }
}
