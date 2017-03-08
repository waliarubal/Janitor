using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class TemporarySettingsManager: SettingsBase
    {
        static TemporarySettingsManager _instance;

        private TemporarySettingsManager()
        {

        }

        #region properties

        public static TemporarySettingsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TemporarySettingsManager();

                return _instance;
            }
        }

        #endregion
    }
}
