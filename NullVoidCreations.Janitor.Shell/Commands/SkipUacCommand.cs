using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
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
            var lnkPath = Path.Combine(KnownPaths.Instance.DesktopDirectory, string.Format("{0}.lnk", App.ProductName));
            var lnk1Path = Path.Combine(NativeApiHelper.Instance.GetStartMenuDirectory(), string.Format("{0}.lnk", App.ProductName));
            var icon = string.Format("{0}, 0", SettingsManager.Instance.ExecutablePath);

            var task = new TaskModel();
            task.Name = string.Format("{0} - Skip UAC", App.ProductName);
            task.ExecutablePath = SettingsManager.Instance.ExecutablePath;

            bool result;
            if ((bool)parameter)
            {
                result = task.Create();
                if (result)
                {
                    var taskSchedulerPath = Path.Combine(KnownPaths.Instance.System32Directory, "schtasks.exe");
                    var arguments = string.Format("/run /TN \"{0}\"", task.Name);

                    if (File.Exists(lnkPath))
                        NativeApiHelper.Instance.CreateShortcut(lnkPath, taskSchedulerPath, arguments, KnownPaths.Instance.ApplicationDirectory, icon, true);

                    if (File.Exists(lnk1Path))
                        NativeApiHelper.Instance.CreateShortcut(lnk1Path, taskSchedulerPath, arguments, KnownPaths.Instance.ApplicationDirectory, icon, true);
                }
            }
            else
            {
                result = task.Delete();
                if (result)
                {
                    if (File.Exists(lnkPath))
                        NativeApiHelper.Instance.CreateShortcut(lnkPath, SettingsManager.Instance.ExecutablePath, null, KnownPaths.Instance.ApplicationDirectory, icon, true);

                    if (File.Exists(lnk1Path))
                        NativeApiHelper.Instance.CreateShortcut(lnk1Path, SettingsManager.Instance.ExecutablePath, null, KnownPaths.Instance.ApplicationDirectory, icon, true);
                }
            }

            return result;
        }

        protected override void ExecuteSuccessOverride(object result)
        {
            
        }
    }
}
