using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class LanguageManager: SettingsBase
    {
        static LanguageManager _instance;
        FileInfo _loadedLanguage;
        readonly string _path;
        readonly List<FileInfo> _languageFiles;

        private LanguageManager()
        {
            _languageFiles = new List<FileInfo>();
            _path = Path.Combine(KnownPaths.Instance.ApplicationDirectory, "Translations");
            if (Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        #region properties

        public static LanguageManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LanguageManager();

                return _instance;
            }
        }

        public List<FileInfo> LanguageFiles
        {
            get { return _languageFiles; }
        }

        #endregion

        bool SelectFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Name.StartsWith("Lang_", StringComparison.InvariantCultureIgnoreCase) && 
                fileInfo.Extension.Equals(".dat", StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        public void GetLanguageFiles()
        {
            _languageFiles.Clear();
            foreach (var fileName in new DirectoryWalker(_path, SelectFile, false))
                _languageFiles.Add(new FileInfo(fileName));
        }

        public void LoadLanguage(FileInfo languageFile)
        {
            _loadedLanguage = languageFile;
            Load(_loadedLanguage.FullName);

            var resources = App.Current.Resources;
            foreach (var key in Keys)
                resources[key] = GetSetting<string>(key);
        }
    }
}
