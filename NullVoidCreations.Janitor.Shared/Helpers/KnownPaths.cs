using System;
using System.IO;

namespace NullVoidCreations.Janitor.Shared.Helpers
{
    public class KnownPaths
    {
        static volatile KnownPaths _instance;
        readonly string _appDataRoaming, _appDataLocal, _appDataLocalLow, _appData, _internetCache, _appDirectory;

        private KnownPaths()
        {
            _appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _internetCache = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            _appDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _appData = Directory.GetParent(_appDataRoaming).FullName;
            _appDataLocal = Path.Combine(_appData, "Local");
            _appDataLocalLow = Path.Combine(_appData, "LocalLow");
        }

        #region properties

        public string ApplicationDirectory
        {
            get { return _appDirectory; }
        }

        public string InternetCache
        {
            get { return _internetCache; }
        }

        public string AppData
        {
            get { return _appData; }
        }

        public string AppDataRoaming
        {
            get { return _appDataRoaming; }
        }

        public string AppDataLocal
        {
            get { return _appDataLocal; }
        }

        public string AppDataLocalLow
        {
            get { return _appDataLocalLow; }
        }

        public static KnownPaths Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new KnownPaths();

                return _instance;
            }
        }

        #endregion
    }
}
