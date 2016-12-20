using System.Diagnostics;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class PurchaseLicenseCommand : CommandBase
    {
        public PurchaseLicenseCommand(ViewModelBase viewModel)
            : base(viewModel)
        {

        }

        public override void Execute(object parameter)
        {
            Process.Start("http://www.google.com/");
        }
    }
}
