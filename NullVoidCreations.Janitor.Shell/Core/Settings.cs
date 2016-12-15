using System;
using System.IO;

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
            _pluginsDirectory = AppDirectory;
            _pluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";

            if (!Directory.Exists(AppDirectory))
                Directory.CreateDirectory(AppDirectory);
            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
        }

        #region properties

        public string CodeName
        {
            get { return _codeName; }
        }

        public string AppDirectory
        {
            get { return _appDirectory; }
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
