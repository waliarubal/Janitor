using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shell.Core
{
    public enum MessageCode : byte
    {
        ScanStatusChanged,
        ScanStarted,
        ScanStopped,
        PluginsLoaded,
        PluginsUnloaded,
        SystemInformationLoaded,
        LicenseChanged
    }

    class Subject
    {
        List<IObserver> _observers;
        volatile static Subject _instance;

        #region constructor / destructor

        private Subject()
        {
            _observers = new List<IObserver>();
        }

        ~Subject()
        {
            _observers.Clear();
        }

        #endregion

        #region properties

        public static Subject Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Subject();

                return _instance;
            }
        }

        #endregion

        public void AddObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public bool RemoveObserver(IObserver observer)
        {
            return _observers.Remove(observer);
        }

        public void NotifyAllObservers(IObserver sender, MessageCode code, params object[] data)
        {
            foreach (var observer in _observers)
                observer.Update(sender, code, data);
        }
    }
}
