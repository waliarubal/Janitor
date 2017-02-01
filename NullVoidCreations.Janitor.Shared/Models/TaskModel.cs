using System;
using System.IO;
using Microsoft.Win32.TaskScheduler;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public class TaskModel
    {
        public enum ScheduleType : byte
        {
            None,
            Once,
            Daily,
            Weekly
        }

        public TaskModel()
        {
            Schedule = ScheduleType.None;
            WeekDays = new bool[7];
            Start = DateTime.MaxValue;
            End = DateTime.MaxValue;
        }

        #region properties

        public string Name { get; set; }

        public string Description { get; set; }

        public string ExecutablePath { get; set; }

        public string CommandLineArguments { get; set; }

        public ScheduleType Schedule { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool[] WeekDays { get; private set; }

        #endregion

        public bool CreateOrUpdate()
        {
            if (Schedule == ScheduleType.None)
                return Delete();

            // schedule
            Trigger trigger = null;
            if (Schedule == ScheduleType.Once)
            {
                trigger = new TimeTrigger(Start);
                trigger.EndBoundary = DateTime.MaxValue;
            }
            else if (Schedule == ScheduleType.Daily)
            {
                trigger = new DailyTrigger();
                trigger.StartBoundary = Start;
                trigger.EndBoundary = DateTime.MaxValue;
            }
            else if (Schedule == ScheduleType.Weekly)
            {
                DaysOfTheWeek days = DaysOfTheWeek.AllDays;
                for (short index = 0; index < WeekDays.Length; index++)
                {
                    if (!WeekDays[index])
                        continue;

                    DaysOfTheWeek day = DaysOfTheWeek.AllDays;
                    switch (index)
                    {
                        case 0:
                            day = DaysOfTheWeek.Sunday;
                            break;

                        case 1:
                            day = DaysOfTheWeek.Monday;
                            break;

                        case 2:
                            day = DaysOfTheWeek.Tuesday;
                            break;

                        case 3:
                            day = DaysOfTheWeek.Wednesday;
                            break;

                        case 4:
                            day = DaysOfTheWeek.Thursday;
                            break;

                        case 5:
                            day = DaysOfTheWeek.Friday;
                            break;

                        case 6:
                            day = DaysOfTheWeek.Saturday;
                            break;
                    }

                    if (days == DaysOfTheWeek.AllDays)
                        days = day;
                    else
                        days |= day;
                }
                trigger = new WeeklyTrigger(days);
                trigger.StartBoundary = Start;
                trigger.EndBoundary = DateTime.MaxValue;
            }

            // enclose path with spaces in double quotes
            var executable = ExecutablePath;
            if (executable.Contains(" "))
                executable = string.Format("\"{0}\"", ExecutablePath);

            // task
            var existingTask = TaskService.Instance.GetTask(Name);
            if (existingTask != null)
            {
                existingTask.Definition.RegistrationInfo.Description = Description;
                existingTask.Definition.Actions.Clear();
                existingTask.Definition.Actions.Add(executable, CommandLineArguments, new FileInfo(ExecutablePath).DirectoryName);
                existingTask.Definition.Principal.LogonType = TaskLogonType.InteractiveToken;
                existingTask.Definition.Principal.RunLevel = TaskRunLevel.Highest;
                existingTask.Definition.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;
                existingTask.Definition.Settings.StopIfGoingOnBatteries = false;
                existingTask.Definition.Triggers.Clear();
                if (Schedule != ScheduleType.None)
                    existingTask.Definition.Triggers.Add(trigger);

                existingTask.RegisterChanges();

                return true;
            }

            var taskDefinition = TaskService.Instance.NewTask();
            taskDefinition.RegistrationInfo.Description = Description;
            taskDefinition.Actions.Add(executable, CommandLineArguments, new FileInfo(ExecutablePath).DirectoryName);
            taskDefinition.Principal.LogonType = TaskLogonType.InteractiveToken;
            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
            taskDefinition.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;
            taskDefinition.Settings.StopIfGoingOnBatteries = false;
            if (Schedule != ScheduleType.None)
                taskDefinition.Triggers.Add(trigger);

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
