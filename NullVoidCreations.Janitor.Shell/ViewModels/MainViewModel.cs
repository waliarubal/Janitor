using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using System;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class MainViewModel: ViewModelBase, IObserver
    {
        bool _isScanning, _isUpdating, _isOk, _isHavingIssues;
        byte _problemsCount;

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

        #endregion

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            switch(code)
            {
                case MessageCode.ScanStarted:
                    IsScanning = true;
                    IsHavingIssues = (bool)data[0];
                    break;

                case MessageCode.ScanStopped:
                    IsScanning = false;
                    IsHavingIssues = (int)data[0] > 0;
                    break;

                case MessageCode.ProblemsAppeared:
                    ProblemsCount = (byte)data[0];
                    IsOk = ProblemsCount == 0;
                    break;
            }

            
        }
    }
}
