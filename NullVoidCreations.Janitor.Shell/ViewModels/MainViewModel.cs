using System.Diagnostics;
using System.Windows;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class MainViewModel: ViewModelBase, ISignalObserver
    {
        bool _isWorking, _isUpdating, _isOk, _isHavingIssues, _isAnalysing, _isFixing, _isLoadingStartupEntries;
        byte _problemsCount;
        int _selectedViewIndex, _issueCount;
        readonly CommandBase _close, _open, _minimize;

        BalloonView _taskbarBalloon;

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
            _isOk = false;

            _close = new DelegateCommand(this, ExecuteClose);
            _open = new DelegateCommand(this, ExecuteOpen);
            _minimize = new DelegateCommand(this, ExecuteMinimize);
            _close.IsEnabled = _open.IsEnabled = true;

            SignalHost.Instance.AddObserver(this);
        }

        ~MainViewModel()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        internal MainView View
        {
            get;
            set;
        }

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

        public CommandBase Minimize
        {
            get { return _minimize; }
        }

        public CommandBase Open
        {
            get { return _open; }
        }

        #endregion

        void ExecuteMinimize(object parameter)
        {
            SignalReceived(this, Signal.Minimize);
        }

        void ExecuteClose(object parameter)
        {
            SignalReceived(this, Signal.Close);
        }

        void ExecuteOpen(object parameter)
        {
            SignalReceived(this, Signal.ShowUi);
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Shutdown:
                    var shutdown = new ProcessStartInfo();
                    shutdown.FileName = "shutdown";
                    shutdown.Arguments = "/s /t 2";
                    shutdown.UseShellExecute = false;
                    shutdown.CreateNoWindow = true;
                    Process.Start(shutdown);
                    break;

                case Signal.ShowHome:
                    SelectedViewIndex = (int)SelectedView.Home;
                    SignalHost.Instance.RaiseSignal(this, Signal.StopWork);
                    break;

                case Signal.ShowUi:
                    NativeApiHelper.Instance.Show(View.Handle);
                    break;

                case Signal.Minimize:
                    NativeApiHelper.Instance.Minimize(View.Handle);
                    break;

                case Signal.Close:
                    if (SettingsManager.Instance.ExitOnClose)
                        App.Current.Shutdown(0);
                    else
                        SignalReceived(sender, Signal.CloseToTray);
                    break;

                case Signal.CloseToTray:
                    NativeApiHelper.Instance.Hide(View.Handle);
                    break;

                case Signal.CloseAndStart:
                    UiHelper.Instance.ExecuteOnUiThread(() =>
                    {
                        App.Current.Shutdown(0);
                        Process.Start(SharedConstants.ExecutableFile);
                    });
                    break;

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
                    IsHavingIssues = IsAnalysing && IssueCount > 0 && !(bool)data[1];
                    IsAnalysing = signal == Signal.AnalysisStopped;
                    IsFixing = signal == Signal.FixingStopped;
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
                    SelectedViewIndex = (int)SelectedView.Update;
                    IsUpdating = true;
                    break;

                case Signal.UpdateStopped:
                    IsUpdating = false;
                    break;

                case Signal.Initialized:
                    View.NotificationIcon.Visibility = Visibility.Visible;
                    break;

                case Signal.ShowBaloon:
                    if (_taskbarBalloon == null)
                        _taskbarBalloon = new BalloonView();
                    
                    (_taskbarBalloon.DataContext as BalloonViewModel).Html = data[0] as string;
                    View.NotificationIcon.ShowCustomBalloon(_taskbarBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 20000);
                    break;

                case Signal.HideBaloon:
                    View.NotificationIcon.CloseBalloon();
                    break;
            }
        }
    }
}
