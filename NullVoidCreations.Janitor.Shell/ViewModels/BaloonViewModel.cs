using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class BaloonViewModel: ViewModelBase
    {
        string _html;
        readonly CommandBase _hide;

        public BaloonViewModel()
        {
            _hide = new HideBaloonCommand(this);
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
