
namespace NullVoidCreations.Janitor.Shell.Base
{
    public abstract class ViewModelBase: NotificationBase
    {
        bool _isExecuting;

        #region properties

        public bool IsExecuting
        {
            get { return _isExecuting; }
            set
            {
                if (value == _isExecuting)
                    return;

                _isExecuting = value;
                RaisePropertyChanged("IsExecuting");
            }
        }

        #endregion
    }
}
