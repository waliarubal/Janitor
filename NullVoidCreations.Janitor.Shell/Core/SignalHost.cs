using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shell.Core
{
    public enum Signal : byte
    {
        ScanStatusChanged,
        ScanTrigerred,
        AnalysisStarted,
        AnalysisStopped,
        FixingStarted,
        FixingStopped,
        PluginsLoaded,
        PluginsUnloaded,
        SystemInformationLoaded,
        LicenseChanged,
        ProblemsAppeared,
        StartupEntriesLoadStarted,
        StartupEntriesLoadStopped,
        SettingsLoaded,
        SettingsSaved,
        UpdateStarted,
        UpdateStopped,
        Initialized,
        CloseTriggered
    }

    class SignalHost
    {
        List<ISignalObserver> _observers;
        volatile static SignalHost _instance;

        #region constructor / destructor

        private SignalHost()
        {
            _observers = new List<ISignalObserver>();
        }

        ~SignalHost()
        {
            _observers.Clear();
        }

        #endregion

        #region properties

        public static SignalHost Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SignalHost();

                return _instance;
            }
        }

        #endregion

        public void AddObserver(ISignalObserver observer)
        {
            _observers.Add(observer);
        }

        public bool RemoveObserver(ISignalObserver observer)
        {
            return _observers.Remove(observer);
        }

        public void NotifyAllObservers(ISignalObserver sender, Signal code, params object[] data)
        {
            foreach (var observer in _observers)
                observer.Update(sender, code, data);
        }
    }
}
