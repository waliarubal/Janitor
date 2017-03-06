using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class ProblemModel: NotificationBase
    {
        string _title, _message;

        #region properties

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title)
                    return;

                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (value == _message)
                    return;

                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        #endregion
    }
}
