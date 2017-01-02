using Hardcodet.Wpf.TaskbarNotification;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class HideBalloonCommand: DelegateCommand
    {
        TaskbarIcon _notificationIcon;

        public HideBalloonCommand(ViewModelBase viewModel)
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
