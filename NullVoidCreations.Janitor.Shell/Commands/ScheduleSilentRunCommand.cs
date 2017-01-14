using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class ScheduleSilentRunCommand: AsyncDelegateCommand
    {
        internal const string SilentRunKey = "JanitorSilentRun";
        CommandBase _skipUac;

        public ScheduleSilentRunCommand(ViewModelBase viewModel)
            : base(viewModel)
        {
            _skipUac = new SkipUacCommand(viewModel);
            _skipUac.IsEnabled = true;
        }

        protected override object ExecuteOverride(object parameter)
        {
            if ((bool)parameter)
            {
                SettingsManager.Instance.SkipUac = true;
                _skipUac.Execute(SettingsManager.Instance.SkipUac);

                var command = string.Format("\"{0}\" /run /TN \"{1}\"", KnownPaths.Instance.TaskScheduler, SkipUacCommand.SkipUacTask);
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
