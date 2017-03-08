using System.Diagnostics;
using System.Windows;
using NullVoidCreations.Janitor.Shared;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class MainViewModel: ViewModelBase, ISignalObserver
    {
        int _selectedViewIndex;
        readonly CommandBase _close, _open, _minimize;

        enum SelectedView: int
        {
            Home,
            ComputerScan,
            Startup,
            Update,
            Settings,
            Help,
            About
        }

        #region constructor / destructor

        public MainViewModel()
        {
            IsOk = false;
            ProblemsCount = 1;

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
            get { return GetValue<bool>("IsWorking"); }
            private set { this["IsWorking"] = value; }
        }

        public bool IsLoadingStartupEntries
        {
            get { return GetValue<bool>("IsLoadingStartupEntries"); }
            private set { this["IsLoadingStartupEntries"] = value; }
        }

        public bool IsUpdating
        {
            get { return GetValue<bool>("IsUpdating"); }
            private set { this["IsUpdating"] = value; }
        }

        public int ProblemsCount
        {
            get { return GetValue<int>("ProblemsCount"); }
            private set { this["ProblemsCount"] = value; }
        }
        
        public bool IsOk
        {
            get { return GetValue<bool>("IsOk"); }
            private set { this["IsOk"] = value; }
        }

        public bool IsHavingIssues
        {
            get { return GetValue<bool>("IsHavingIssues"); }
            private set { this["IsHavingIssues"] = value; }
        }

        public bool IsAnalysing
        {
            get { return GetValue<bool>("IsAnalysing"); }
            private set { this["IsAnalysing"] = value; }
        }

        public bool IsFixing
        {
            get { return GetValue<bool>("IsFixing"); }
            private set { this["IsFixing"] = value; }
        }

        public int IssueCount
        {
            get { return GetValue<int>("IssueCount"); }
            private set { this["IssueCount"] = value; }
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
            SignalReceived(Signal.Minimize);
        }

        void ExecuteClose(object parameter)
        {
            SignalReceived(Signal.Close);
        }

        void ExecuteOpen(object parameter)
        {
            SignalReceived(Signal.ShowUi);
        }

        public void SignalReceived(Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Shutdown:
                    FileSystemHelper.Instance.RunProgram("shutdown", "/s /t 2", false, true);
                    break;

                case Signal.ShowHome:
                    SelectedViewIndex = (int)SelectedView.Home;
                    SignalHost.Instance.RaiseSignal(Signal.StopWork);
                    break;

                case Signal.ShowUi:
                    Win32Helper.Instance.Show(UiHelper.Instance.MainWindow.Handle);
                    break;

                case Signal.Minimize:
                    Win32Helper.Instance.Minimize(UiHelper.Instance.MainWindow.Handle);
                    break;

                case Signal.Close:
                    if (SettingsManager.Instance.ExitOnClose)
                        App.Current.Shutdown(0);
                    else
                        SignalReceived(Signal.CloseToTray);
                    break;

                case Signal.CloseToTray:
                    Win32Helper.Instance.Hide(UiHelper.Instance.MainWindow.Handle);
                    break;

                case Signal.CloseAndStart:
                    App.Current.Shutdown(0);
                    Process.Start(Constants.ExecutableFile);
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
                    IssueCount = (int)data[0];
                    IsWorking = false;
                    IsAnalysing = signal == Signal.AnalysisStopped;
                    IsFixing = signal == Signal.FixingStopped;
                    IsHavingIssues = IsAnalysing && IssueCount > 0 && !(bool)data[1];
                    break;

                case Signal.ProblemAppeared:
                    ProblemsCount = (int)data[0];
                    IsOk = ProblemsCount == 0;

                    if (data[1] != null)
                        UiHelper.Instance.ShowBalloon(data[1] as ProblemModel);
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
                    UiHelper.Instance.MainWindow.NotificationIcon.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
