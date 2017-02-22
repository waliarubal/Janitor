using NullVoidCreations.Janitor.Shared;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    class SkipUacCommand: AsyncDelegateCommand
    {
        public static string SkipUacTask;

        static SkipUacCommand()
        {
            SkipUacTask = string.Format("{0}SkipUAC", Constants.InternalName); ;
        }

        public SkipUacCommand(ViewModelBase viewMode)
            : base(viewMode)
        {
            IsEnabled = Constants.IsUacSupported;
        }

        protected override object ExecuteOverride(object parameter)
        {
            var task = new TaskModel();
            task.Name = SkipUacTask;
            task.ExecutablePath = Constants.ExecutableFile;
            task.CommandLineArguments = "$(Arg0)";

            bool result;
            if ((bool)parameter)
                result = task.CreateOrUpdate();
            else
                result = task.Delete();

            return result;
        }

        protected override void ExecuteSuccessOverride(object result)
        {
            
        }
    }
}
