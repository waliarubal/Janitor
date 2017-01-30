using System.IO;
using NullVoidCreations.Janitor.Shared;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
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

        }

        protected override object ExecuteOverride(object parameter)
        {
            var lnkPath = Path.Combine(Win32Helper.Instance.GetPublicDesktopDirectory(), string.Format("{0}.lnk", Constants.ProductName));
            var lnk1Path = Path.Combine(Win32Helper.Instance.GetStartMenuDirectory(), string.Format("PC Mechanic Pro\\{0}.lnk", Constants.ProductName));
            var icon = string.Format("{0}, 0", Constants.ExecutableFile);

            var task = new TaskModel();
            task.Name = SkipUacTask;
            task.ExecutablePath = Constants.ExecutableFile;

            bool result;
            if ((bool)parameter)
            {
                result = task.CreateOrUpdate();
                if (result)
                {
                    var arguments = string.Format("/run /TN \"{0}\"", task.Name);

                    if (File.Exists(lnkPath))
                        Win32Helper.Instance.CreateShortcut(lnkPath, KnownPaths.Instance.TaskScheduler, arguments, KnownPaths.Instance.ApplicationDirectory, icon, true);

                    if (File.Exists(lnk1Path))
                        Win32Helper.Instance.CreateShortcut(lnk1Path, KnownPaths.Instance.TaskScheduler, arguments, KnownPaths.Instance.ApplicationDirectory, icon, true);
                }
            }
            else
            {
                result = task.Delete();
                if (result)
                {
                    if (File.Exists(lnkPath))
                        Win32Helper.Instance.CreateShortcut(lnkPath, Constants.ExecutableFile, null, KnownPaths.Instance.ApplicationDirectory, icon, false);

                    if (File.Exists(lnk1Path))
                        Win32Helper.Instance.CreateShortcut(lnk1Path, Constants.ExecutableFile, null, KnownPaths.Instance.ApplicationDirectory, icon, false);
                }
            }

            return result;
        }

        protected override void ExecuteSuccessOverride(object result)
        {
            
        }
    }
}
