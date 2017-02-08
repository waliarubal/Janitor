using NullVoidCreations.Janitor.Shared;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class ScheduleSilentRunCommand: AsyncDelegateCommand
    {
        internal const string SilentRunKey = "JanitorSilentRun";

        public ScheduleSilentRunCommand(ViewModelBase viewModel)
            : base(viewModel)
        {
            
        }

        protected override object ExecuteOverride(object parameter)
        {
            if ((bool)parameter)
            {
                string command;

                var skipUac = new SkipUacCommand(ViewModel);
                if (skipUac.IsEnabled)
                {
                    SettingsManager.Instance.SkipUac = true;
                    skipUac.Execute(true);
                    command = string.Format("\"{0}\" /run /TN \"{1}\"", KnownPaths.Instance.TaskScheduler, SkipUacCommand.SkipUacTask);
                }
                else
                    command = Constants.ExecutableFile;

                return StartupEntryModel.AddEntry(StartupEntryModel.StartupArea.Registry, SilentRunKey, command) != null;
            }
            else
                return StartupEntryModel.RemoveEntry(StartupEntryModel.StartupArea.Registry, SilentRunKey);
        }

        protected override void ExecuteSuccessOverride(object result)
        {
            
        }
    }
}
