using System.Windows;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class MainViewModel: ViewModelBase, ISignalObserver
    {
        bool _isWorking, _isUpdating, _isOk, _isHavingIssues, _isAnalysing, _isFixing, _isLoadingStartupEntries;
        byte _problemsCount;
        int _selectedViewIndex, _issueCount;
        readonly CommandBase _close;

        enum SelectedView: int
        {
            Home,
            ComputerScan,
            Startup,
            Update,
            Settings,
            About
        }

        #region constructor / destructor

        public MainViewModel()
        {
            _isOk = true;
            _close = new DelegateCommand(this, ExecuteClose);

            SignalHost.Instance.AddObserver(this);
        }

        ~MainViewModel()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        public int SelectedViewIndex
        {
            get { return _selectedViewIndex; }
            set
            {
                if (value == _selectedViewIndex)
                    return;
                if (value < (int)SelectedView.Home || value > (int)SelectedView.About)
                    return;

                _selectedViewIndex = value;
                RaisePropertyChanged("SelectedViewIndex");
            }
        }

        public bool IsWorking
        {
            get { return _isWorking; }
            private set
            {
                if (value == _isWorking)
                    return;

                _isWorking = value;
                RaisePropertyChanged("IsWorking");
            }
        }

        public bool IsLoadingStartupEntries
        {
            get { return _isLoadingStartupEntries; }
            private set
            {
                if (value == _isLoadingStartupEntries)
                    return;

                _isLoadingStartupEntries = value;
                RaisePropertyChanged("IsLoadingStartupEntries");
            }
        }

        public bool IsUpdating
        {
            get { return _isUpdating; }
            private set
            {
                if (value == _isUpdating)
                    return;

                _isUpdating = value;
                RaisePropertyChanged("IsUpdating");
            }
        }

        public byte ProblemsCount
        {
            get { return _problemsCount; }
            private set
            {
                if (value == _problemsCount)
                    return;

                _problemsCount = value;
                RaisePropertyChanged("ProblemsCount");
            }
        }
        

        public bool IsOk
        {
            get { return _isOk; }
            private set
            {
                if (value == _isOk)
                    return;

                _isOk = value;
                RaisePropertyChanged("IsOk");
            }
        }

        public bool IsHavingIssues
        {
            get { return _isHavingIssues; }
            private set
            {
                if (value == _isHavingIssues)
                    return;

                _isHavingIssues = value;
                RaisePropertyChanged("IsHavingIssues");
            }
        }

        public bool IsAnalysing
        {
            get { return _isAnalysing; }
            private set
            {
                if (value == _isAnalysing)
                    return;

                _isAnalysing = value;
                RaisePropertyChanged("IsAnalysing");
            }
        }

        public bool IsFixing
        {
            get { return _isFixing; }
            private set
            {
                if (value == _isFixing)
                    return;

                _isFixing = value;
                RaisePropertyChanged("IsFixing");
            }
        }

        public int IssueCount
        {
            get { return _issueCount; }
            private set
            {
                if (value == _issueCount)
                    return;

                _issueCount = value;
                RaisePropertyChanged("IssueCount");
            }
        }

        #endregion

        #region commands

        public CommandBase Close
        {
            get { return _close; }
        }

        #endregion

        void ExecuteClose(object parameter)
        {
            var window = parameter as CustomWindow;
            if (SettingsManager.Instance.ExitOnClose)
                App.Current.Shutdown(0);
            else
                window.WindowState = WindowState.Minimized;
        }

        public void Update(ISignalObserver sender, Signal code, params object[] data)
        {
            switch(code)
            {
                case Signal.FixingStarted:
                case Signal.AnalysisStarted:
                    IsWorking = true;
                    IssueCount = 0;
                    IsHavingIssues = false;
                    IsAnalysing = false;
                    IsFixing = false;
                    break;

                case Signal.FixingStopped:
                case Signal.AnalysisStopped:
                    IsWorking = false;
                    IssueCount = (int)data[0];
                    IsHavingIssues = IsAnalysing && IssueCount > 0;
                    IsAnalysing = code == Signal.AnalysisStopped;
                    IsFixing = code == Signal.FixingStopped;
                    break;

                case Signal.ProblemsAppeared:
                    ProblemsCount = (byte)data[0];
                    IsOk = ProblemsCount == 0;
                    break;

                case Signal.ScanTrigerred:
                    SelectedViewIndex = (int)SelectedView.ComputerScan;
                    break;

                case Signal.StartupEntriesLoadStarted:
                    IsLoadingStartupEntries = true;
                    break;

                case Signal.StartupEntriesLoadStopped:
                    IsLoadingStartupEntries = false;
                    break;

                case Signal.UpdateStarted:
                    IsUpdating = true;
                    break;

                case Signal.UpdateStopped:
                    IsUpdating = false;
                    break;
            }

            
        }
    }
}
