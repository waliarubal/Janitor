using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class StartupViewModel: ViewModelBase, IObserver
    {
        ObservableCollection<StartupEntryModel> _entries;
        readonly CommandBase _refresh;

        public StartupViewModel()
        {
            _refresh = new AsyncDelegateCommand(this, null, ExecuteRefresh, RefreshCompleted);
            _refresh.IsEnabled = true;

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

        #endregion

        #region commands

        public CommandBase Refresh
        {
            get { return _refresh; }
        }

        #endregion

        object ExecuteRefresh(object parameter)
        {
            Subject.Instance.NotifyAllObservers(this, MessageCode.StartupEntriesLoadStarted);
            var entries = new ObservableCollection<StartupEntryModel>();
            foreach (var entry in SysInformation.Instance.GetAllStartupEntries())
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
            switch(code)
            {
                case MessageCode.Initialized:
                    Refresh.Execute(Entries);
                    break;
            }
        }
    }
}
