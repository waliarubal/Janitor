using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class LanguageManager: SettingsBase
    {
        static LanguageManager _instance;
        string _language;
        readonly string _path;
        readonly Dictionary<string, string> _languages;

        private LanguageManager()
        {
            _languages = new Dictionary<string, string>();

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

        public IEnumerable<string> Languages
        {
            get { return _languages.Keys; }
        }

        public string LoadedLanguage
        {
            get { return _language; }
        }

        #endregion

        public void GetLanguageFiles()
        {
            _languages.Clear();
            foreach (var fileName in new DirectoryWalker(_path, false))
                _languages.Add(Path.GetFileNameWithoutExtension(fileName), fileName);
        }

        public void LoadLanguage(string language)
        {
            _language = language;
            Load(_languages[language]);

            var resources = UiHelper.Instance.Resources;
            foreach (var key in Keys)
                resources[key] = GetSetting<string>(key);
        }
    }
}
