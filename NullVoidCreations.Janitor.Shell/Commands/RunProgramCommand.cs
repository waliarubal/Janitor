using System.Diagnostics;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class RunProgramCommand: DelegateCommand
    {
        public RunProgramCommand(ViewModelBase viewModel)
            : base(viewModel)
        {

        }

        protected override void ExecuteOverride(object parameter)
        {
            var program = parameter as string;
            if (string.IsNullOrEmpty(program))
                return;

            Process.Start(program);
        }
    }
}
