using System.Windows;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class ShowPopupCommand: CommandBase
    {
        Window _popup;

        public ShowPopupCommand()
            : base(null)
        {
            IsEnabled = true;
        }

        public override void Execute(object parameter)
        {
            var windowToOpen = parameter as string;
            if (windowToOpen == null)
                return;

            if (_popup != null)
                _popup.Close();

            switch(windowToOpen)
            {
                case "LicenseActivation":
                    _popup = new LicenseActivationView();
                    break;

                case "CustomerRegistration":
                    _popup = new CustomerRegistrationView();
                    break;

                default:
                    return;
            }
            UiHelper.Instance.ShowPopup(_popup);
        }
    }
}
