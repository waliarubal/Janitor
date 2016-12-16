using System;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class Settings
    {
        static volatile Settings _instance;
        string _codeName, _appDirectory, _pluginsDirectory, _pluginsSearchFilter;

        private Settings()
        {
            _codeName = "Janitor";
            _appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), CodeName);
            _pluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            _pluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";

            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
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

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();

                return _instance;
            }
        }

        #endregion
    }
}
