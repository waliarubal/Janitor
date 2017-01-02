using Hardcodet.Wpf.TaskbarNotification;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class HideBaloonCommand: DelegateCommand
    {
        TaskbarIcon _notificationIcon;

        public HideBaloonCommand(ViewModelBase viewModel)
            : base(viewModel)
        {
            IsEnabled = true;
        }

        protected override void ExecuteOverride(object parameter)
        {
            if (_notificationIcon == null)
                _notificationIcon = (TaskbarIcon)App.Current.Resources["NotificationIcon"];

            _notificationIcon.CloseBalloon();
        }
    }
}
