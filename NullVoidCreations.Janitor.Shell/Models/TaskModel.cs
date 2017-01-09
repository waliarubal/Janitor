using System.IO;
using Microsoft.Win32.TaskScheduler;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class TaskModel
    {
        #region properties

        public string Name { get; set; }

        public string Description { get; set; }

        public string ExecutablePath { get; set; }

        public string CommandLineArguments { get; set; }

        #endregion

        public bool Create()
        {
            if (TaskService.Instance.GetTask(Name) != null)
                return false;

            var taskDefinition = TaskService.Instance.NewTask();
            taskDefinition.RegistrationInfo.Description = Description;
            taskDefinition.Actions.Add(ExecutablePath, CommandLineArguments, new FileInfo(ExecutablePath).DirectoryName);
            taskDefinition.Principal.LogonType = TaskLogonType.InteractiveToken;
            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;

            return TaskService.Instance.RootFolder.RegisterTaskDefinition(Name, taskDefinition) != null;
        }

        public bool Delete()
        {
            var isDeleted = true;
            try
            {
                TaskService.Instance.RootFolder.DeleteTask(Name);
            }
            catch
            {
                isDeleted = false;
            }

            return isDeleted;
        }
    }
}
