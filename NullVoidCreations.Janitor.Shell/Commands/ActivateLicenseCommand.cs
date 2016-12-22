using System.Windows;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class ActivateLicenseCommand: CommandBase
    {
        public ActivateLicenseCommand(ViewModelBase viewModel)
            : base(viewModel)
        {

        }

        public override void Execute(object parameter)
        {
            var activationView = new LicenseActivationView();
            activationView.Owner = Application.Current.MainWindow;
            activationView.ShowDialog();
        }
    }
}
