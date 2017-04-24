using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class ShowPopupCommand : CommandBase
    {
        CustomWindow _popup;
        string _caller;

        public ShowPopupCommand()
            : base(null)
        {
            IsEnabled = true;
        }

        public override void Execute(object parameter)
        {
            var windowToOpen = parameter as string;
            if (string.IsNullOrEmpty(windowToOpen))
                return;

            if (_popup != null)
                _popup.Close();

        SHOW_POPUP:
            switch (windowToOpen)
            {
                case "Authentication":
                    if (LicenseManager.Instance.IsAuthenticated)
                        return;

                    _popup = new AuthenticationView();
                    break;

                case "CustomerRegistration":
                    if (LicenseManager.Instance.IsAuthenticated)
                        return;

                    _popup = new CustomerRegistrationView();
                    break;

                case "LicenseManagement":
                    _popup = new LicenseManagementView();
                    break;

                case "LicenseActivation":
                    _popup = new LicenseActivationView();
                    break;

                default:
                    return;
            }

            if (_popup.IsAuthenticationRequired && !LicenseManager.Instance.IsAuthenticated)
            {
                _caller = windowToOpen;
                _popup = new AuthenticationView();
            }

            if (UiHelper.Instance.ShowPopup(_popup) == true && _caller != null)
            {
                windowToOpen = _caller;
                _caller = null;
                goto SHOW_POPUP;
            }
        }
    }
}
