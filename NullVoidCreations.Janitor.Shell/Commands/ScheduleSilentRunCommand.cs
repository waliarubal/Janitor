using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class ScheduleSilentRunCommand: AsyncDelegateCommand
    {
        internal const string SilentRunKey = "JanitorSilentRun";

        public ScheduleSilentRunCommand(ViewModelBase viewMode)
            : base(viewMode)
        {

        }

        protected override object ExecuteOverride(object parameter)
        {
            if ((bool)parameter)
            {
                var command = string.Format("\"{0}\" /{1}", SettingsManager.Instance.ExecutablePath, CommandLineManager.CommandLineArgument.Silent);
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
