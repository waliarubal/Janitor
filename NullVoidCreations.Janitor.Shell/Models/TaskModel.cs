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

        public bool CreateOrUpdate()
        {
            var existingTask = TaskService.Instance.GetTask(Name);
            if (existingTask != null)
            {
                existingTask.Definition.RegistrationInfo.Description = Description;
                existingTask.Definition.Actions.Clear();
                existingTask.Definition.Actions.Add(ExecutablePath, CommandLineArguments, new FileInfo(ExecutablePath).DirectoryName);
                existingTask.Definition.Principal.LogonType = TaskLogonType.InteractiveToken;
                existingTask.Definition.Principal.RunLevel = TaskRunLevel.Highest;
                existingTask.Definition.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;
                existingTask.Definition.Settings.StopIfGoingOnBatteries = false;
                existingTask.Definition.Triggers.Clear();
                if (Schedule != null)
                    existingTask.Definition.Triggers.Add(Schedule);

                existingTask.RegisterChanges();

                return true;
            }

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
