using System.IO;
using System.Diagnostics;
using System.Text;

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

        public string GetArgumentsString(params string[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                return string.Empty;

            var args = new StringBuilder(arguments[0]);
            for (var index = 1; index < arguments.Length; index++)
                args.AppendFormat(" \"{0}\"", arguments[index]);

            return args.ToString();
        }

        public bool RunProgram(string executable, string arguments, bool runAsAdministrator, bool hideUi = false)
        {
            if (string.IsNullOrEmpty(executable))
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

        public bool RunScheduledTask(string taskName, params string[] arguments)
        {
            var commandLine = string.Format("/run /TN \"{0}\"", taskName);

            var args = FileSystemHelper.Instance.GetArgumentsString(arguments);
            if (args.Length > 0)
                commandLine = string.Format("{0} {1}", commandLine, args);

            return FileSystemHelper.Instance.RunProgram(KnownPaths.Instance.TaskScheduler, commandLine, false, true);
        }
    }
}
