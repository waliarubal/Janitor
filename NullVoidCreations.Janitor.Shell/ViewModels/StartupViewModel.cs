using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class StartupViewModel : ViewModelBase, IObserver
    {
        readonly CommandBase _refresh, _delete;
        StartupEntryModel _selectedEntry;
        ObservableCollection<StartupEntryModel> _entries;

        public StartupViewModel()
        {
            _refresh = new AsyncDelegateCommand(this, null, ExecuteRefresh, RefreshCompleted);
            _delete = new AsyncDelegateCommand(this, null, ExecuteDelete, DeleteComplete);
            _refresh.IsEnabled = _delete.IsEnabled = true;

            Subject.Instance.AddObserver(this);
        }

        ~StartupViewModel()
        {
            Subject.Instance.RemoveObserver(this);
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

            return entry.RemoveFromStartup();
        }

        object ExecuteRefresh(object parameter)
        {
            Subject.Instance.NotifyAllObservers(this, MessageCode.StartupEntriesLoadStarted);

            var entries = new ObservableCollection<StartupEntryModel>();
            foreach (var entry in StartupEntryModel.GetStartupEntries())
                entries.Add(entry);

            Subject.Instance.NotifyAllObservers(this, MessageCode.StartupEntriesLoadStopped);

            return entries;
        }

        void RefreshCompleted(object startupEntries)
        {
            Entries = startupEntries as ObservableCollection<StartupEntryModel>;
        }

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            switch (code)
            {
                case MessageCode.Initialized:
                    Refresh.Execute(Entries);
                    break;
            }
        }
    }
}
