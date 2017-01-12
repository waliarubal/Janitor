using System.IO;
using Microsoft.Win32.TaskScheduler;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class TaskModel: NotificationBase
    {
        #region properties

        public string Name { get; set; }

        public string Description { get; set; }

        public string ExecutablePath { get; set; }

        public string CommandLineArguments { get; set; }

        public Trigger Schedule { get; set; }

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
            taskDefinition.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;
            taskDefinition.Settings.StopIfGoingOnBatteries = false;
            if (Schedule != null)
                taskDefinition.Triggers.Add(Schedule);

            return TaskService.Instance.RootFolder.RegisterTaskDefinition(Name, taskDefinition) != null;
        }

        public bool Delete()
        {
            var isDeleted = true;
            try
            {
                TaskService.Instance.RootFolder.DeleteTask(Name, false);
            }
            catch
            {
                isDeleted = false;
            }

            return isDeleted;
        }
    }
}
