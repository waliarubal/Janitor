using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    class SkipUacCommand: AsyncDelegateCommand
    {
        public SkipUacCommand(ViewModelBase viewMode)
            : base(viewMode)
        {

        }

        protected override object ExecuteOverride(object parameter)
        {
            var task = new TaskModel();
            task.Name = string.Format("{0} - Skip UAC", App.ProductName);
            task.ExecutablePath = SettingsManager.Instance.ExecutablePath;

            if ((bool)parameter)
            {
                return task.Create();
            }
            else
            {
                return task.Delete();
            }
        }

        protected override void ExecuteSuccessOverride(object result)
        {
            
        }
    }
}
