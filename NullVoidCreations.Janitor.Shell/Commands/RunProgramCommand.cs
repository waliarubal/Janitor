using System.Collections.Generic;
using System.Diagnostics;
using NullVoidCreations.Janitor.Shared.Base;

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
            string fileName = null, arguments = null;

            var multiParams = parameter as List<object>;
            if (multiParams == null)
                fileName = parameter as string;
            else
            {
                fileName = multiParams[0] as string;
                arguments = multiParams[1] as string;
            }

            if (string.IsNullOrEmpty(fileName))
                return;
            if (string.IsNullOrEmpty(arguments))
                arguments = string.Empty;

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;
            Process.Start(startInfo);
        }
    }
}
