using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class BalloonViewModel: ViewModelBase
    {
        readonly CommandBase _close;

        public BalloonViewModel()
        {
            _close = new DelegateCommand(this, (parameter) => UiHelper.Instance.MainWindow.NotificationIcon.CloseBalloon());
            _close.IsEnabled = true;
        }

        #region properties

        public ProblemModel Problem
        {
            get { return GetValue<ProblemModel>("Problem"); }
            set { this["Problem"] = value; }
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
