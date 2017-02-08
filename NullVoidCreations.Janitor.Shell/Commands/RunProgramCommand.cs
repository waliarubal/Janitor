using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class RunProgramCommand: DelegateCommand
    {
        public RunProgramCommand(ViewModelBase viewModel)
            : base(viewModel)
        {
            IsEnabled = true;
        }

        protected override void ExecuteOverride(object parameter)
        {
            string executable = null, arguments = null;
            var runAsAdministrator = false;

            var multiParams = parameter as List<object>;
            if (multiParams == null)
                executable = parameter as string;
            else
            {
                executable = multiParams[0] as string;
                arguments = multiParams[1] as string;
                runAsAdministrator = (bool)multiParams[2];
            }

            FileSystemHelper.Instance.RunProgram(executable, arguments, runAsAdministrator);
        }
    }
}
