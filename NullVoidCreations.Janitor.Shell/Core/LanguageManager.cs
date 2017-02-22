using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class LanguageManager: SettingsBase
    {
        static LanguageManager _instance;
        LanguageModel _language;
        readonly string _path;
        readonly Dictionary<string, LanguageModel> _languages;

        private LanguageManager()
        {
            _languages = new Dictionary<string, LanguageModel>();

            _path = Path.Combine(KnownPaths.Instance.ApplicationDirectory, "Translations");
            if (!Directory.Exists(_path))
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

        public IEnumerable<LanguageModel> Languages
        {
            get { return _languages.Values; }
        }

        public LanguageModel LoadedLanguage
        {
            get { return _language; }
        }

        #endregion

        bool FileFilter(string fileName)
        {
            return new FileInfo(fileName).Extension.Equals(".dat");
        }

        public void GetLanguageFiles()
        {
            _languages.Clear();
            foreach (var fileName in new DirectoryWalker(_path, FileFilter , false))
            {
                var language = new LanguageModel(fileName);
                _languages.Add(language.Name, language);
            }
        }

        public void LoadLanguage(string language)
        {
            _language = _languages[language];
            Load(_languages[language].FileName);
         
            var resources = UiHelper.Instance.Resources;
            foreach (var key in Keys)
                resources[key] = GetSetting<string>(key);
        }
    }
}
