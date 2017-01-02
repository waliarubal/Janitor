using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class BalloonViewModel: ViewModelBase
    {
        string _html;
        readonly CommandBase _hide;

        public BalloonViewModel()
        {
            _hide = new HideBalloonCommand(this);
        }

        #region properties

        public string Html
        {
            get { return _html; }
            internal set
            {
                if (value == _html)
                    return;

                _html = value;
                RaisePropertyChanged("Html");
            }
        }

        #endregion

        #region commands

        public CommandBase Hide
        {
            get { return _hide; }
        }

        #endregion
    }
}
