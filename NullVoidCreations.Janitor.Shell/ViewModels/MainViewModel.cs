using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class MainViewModel: ViewModelBase, IObserver
    {
        bool _isScanning, _isUpdating;

        #region constructor / destructor

        public MainViewModel()
        {
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

        #endregion

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            switch(code)
            {
                case MessageCode.ScanStarted:
                    IsScanning = true;
                    break;

                case MessageCode.ScanStopped:
                    IsScanning = false;
                    break;
            }
        }
    }
}
