using System.Windows;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class BalloonViewModel: ViewModelBase
    {
        string _html;
        readonly CommandBase _close;
        FrameworkElement _content;

        public BalloonViewModel()
        {
            _close = new BalloonCommand(this);
        }

        #region properties

        public FrameworkElement Content
        {
            get { return _content; }
            set
            {
                if (value == _content)
                    return;

                _content = value;
                RaisePropertyChanged("Content");
            }
        }

        #endregion

        #region commands

        public CommandBase Close
        {
            get { return _close; }
        }

        #endregion
    }
}
