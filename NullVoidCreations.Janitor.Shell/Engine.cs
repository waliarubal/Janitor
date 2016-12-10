using System;

namespace NullVoidCreations.Janitor.Shell
{
    sealed class Engine
    {
        static Engine _instance;
        AppDomain _pluginsContainer;

        #region constructor / destructor

        private Engine()
        {
            
        }

        #endregion

        #region properties

        public static Engine Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Engine();

                return _instance;
            }
        }

        #endregion

        
    }
}
