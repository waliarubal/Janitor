using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using System;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class MainViewModel: ViewModelBase, IObserver
    {
        bool _isScanning, _isUpdating, _isOk, _isHavingIssues;
        byte _problemsCount;
        int _selectedViewIndex, _issueCount;

        enum SelectedView: int
        {
            Home,
            ComputerScan,
            Update,
            Settings,
            About
        }

        #region constructor / destructor

        public MainViewModel()
        {
            _isOk = true;

            Subject.Instance.AddObserver(this);
        }

        ~MainViewModel()
        {
            Subject.Instance.RemoveObserver(this);
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

        public bool IsScanning
        {
            get { return _isScanning; }
            private set
            {
                if (value == _isScanning)
                    return;

                _isScanning = value;
                RaisePropertyChanged("IsScanning");
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

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            switch(code)
            {
                case MessageCode.ScanStarted:
                    IsScanning = true;
                    IssueCount = 0;
                    IsHavingIssues = false;
                    break;

                case MessageCode.ScanStopped:
                    IsScanning = false;
                    IssueCount = (int)data[0];
                    IsHavingIssues = IssueCount > 0;
                    break;

                case MessageCode.ProblemsAppeared:
                    ProblemsCount = (byte)data[0];
                    IsOk = ProblemsCount == 0;
                    break;

                case MessageCode.ScanTrigerred:
                    SelectedViewIndex = (int)SelectedView.ComputerScan;
                    break;
            }

            
        }
    }
}
