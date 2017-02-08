using System.IO;
using System.Diagnostics;

namespace NullVoidCreations.Janitor.Shared.Helpers
{
    public class FileSystemHelper
    {
        volatile static FileSystemHelper _instance;

        private FileSystemHelper()
        {

        }

        #region properties

        public static FileSystemHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FileSystemHelper();

                return _instance;
            }
        }

        #endregion

        public bool DeleteFile(string path)
        {
            var isDeleted = true;
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                isDeleted = false;
            }

            return isDeleted;
        }

        public bool RunProgram(string executable, string arguments, bool runAsAdministrator, bool hideUi = false)
        {
            if (string.IsNullOrEmpty(executable) || !File.Exists(executable))
                return false;
            if (string.IsNullOrEmpty(arguments))
                arguments = string.Empty;

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = executable;
            startInfo.Arguments = arguments;
            if (hideUi)
            {
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
            }
            
            if (runAsAdministrator)
            {
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
            }

            return Process.Start(startInfo) != null;
        }

        public bool RunScheduledTask(string taskName)
        {
            var arguments = string.Format("/run /TN \"{0}\"", taskName);
            return FileSystemHelper.Instance.RunProgram(KnownPaths.Instance.TaskScheduler, arguments, false, true);
        }
    }
}
