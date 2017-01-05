﻿using System.Collections.Generic;

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
        CloseToTray,
        ShowUi
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

        public void RaiseSignal(ISignalObserver sender, Signal signal, params object[] data)
        {
            for (var index = _observers.Count - 1; index >= 0; index--)
            {
                _observers[index].SignalReceived(sender, signal, data);
            }
        }
    }
}