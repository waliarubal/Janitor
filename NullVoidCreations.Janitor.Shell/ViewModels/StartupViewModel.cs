﻿using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class StartupViewModel : ViewModelBase, ISignalObserver
    {
        readonly CommandBase _refresh, _delete;
        StartupEntryModel _selectedEntry;
        ObservableCollection<StartupEntryModel> _entries;

        public StartupViewModel()
        {
            _refresh = new AsyncDelegateCommand(this, null, ExecuteRefresh, RefreshCompleted);
            _delete = new AsyncDelegateCommand(this, null, ExecuteDelete, DeleteComplete);
            _refresh.IsEnabled = _delete.IsEnabled = true;

            SignalHost.Instance.AddObserver(this);
        }

        ~StartupViewModel()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #region properties

        public ObservableCollection<StartupEntryModel> Entries
        {
            get { return _entries; }
            private set
            {
                if (value == _entries)
                    return;

                _entries = value;
                RaisePropertyChanged("Entries");
            }
        }

        public StartupEntryModel SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                if (value == _selectedEntry)
                    return;

                _selectedEntry = value;
                RaisePropertyChanged("SelectedEntry");
            }
        }

        #endregion

        #region commands

        public CommandBase Refresh
        {
            get { return _refresh; }
        }

        public CommandBase Delete
        {
            get { return _delete; }
        }

        #endregion

        void DeleteComplete(object result)
        {
            if ((bool)result)
            {
                Entries.Remove(SelectedEntry);
                SelectedEntry = null;
            }
        }

        object ExecuteDelete(object parameter)
        {
            var entry = parameter as StartupEntryModel;
            if (entry == null)
                return false;

            return entry.RemoveEntry();
        }

        object ExecuteRefresh(object parameter)
        {
            SignalHost.Instance.RaiseSignal(this, Signal.StartupEntriesLoadStarted);

            var entries = new ObservableCollection<StartupEntryModel>();
            foreach (var entry in StartupEntryModel.GetStartupEntries())
                entries.Add(entry);

            SignalHost.Instance.RaiseSignal(this, Signal.StartupEntriesLoadStopped);

            return entries;
        }

        void RefreshCompleted(object startupEntries)
        {
            Entries = startupEntries as ObservableCollection<StartupEntryModel>;
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Initialized:
                    Refresh.Execute(Entries);
                    break;
            }
        }
    }
}