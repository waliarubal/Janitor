using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public enum ScheduleType : byte
    {
        None,
        Once,
        Daily,
        Weekly
    }

    public class SettingsViewModel : ViewModelBase
    {
        readonly CommandBase _scheduleSilentRun, _skipUac, _saveSchedule;

        public SettingsViewModel()
        {
            // load schedule
            WeekDays = new ObservableCollection<bool>();
            for (var index = 0; index < 7; index++)
                WeekDays.Add(SettingsManager.Instance.ScheduleDays[index]);
            Date = SettingsManager.Instance.ScheduleDate;
            IsScheduleDisabled = SettingsManager.Instance.ScheduleType == ScheduleType.None;
            IsScheduleOnce = SettingsManager.Instance.ScheduleType == ScheduleType.Once;
            IsScheduleDaily = SettingsManager.Instance.ScheduleType == ScheduleType.Daily;
            IsScheduleWeekly = SettingsManager.Instance.ScheduleType == ScheduleType.Weekly;
            if (IsScheduleDisabled)
                Date = DateTime.Now;

            // commands
            _scheduleSilentRun = new ScheduleSilentRunCommand(this);
            _skipUac = new SkipUacCommand(this);
            _saveSchedule = new AsyncDelegateCommand(this, null, ExecuteSaveSchedule, SaveScheduleExecuted);
            _scheduleSilentRun.IsEnabled = _skipUac.IsEnabled = _saveSchedule.IsEnabled = true;
        }

        #region properties

        public IEnumerable<LanguageModel> Languages
        {
            get { return LanguageManager.Instance.Languages; }
        }

        public LanguageModel Language
        {
            get { return LanguageManager.Instance.LoadedLanguage; }
            set
            {
                if (value == LanguageManager.Instance.LoadedLanguage)
                    return;

                SettingsManager.Instance.Language = value.Name;
                LanguageManager.Instance.LoadLanguage(value.Name);

                if (UiHelper.Instance.Question("{0} language will be applied when program is closed and started again. Do you want to restart the program again?", value.Name))
                {
                    SignalHost.Instance.RaiseSignal(Signal.CloseAndStart);
                }
            }
        }

        public DateTime Date
        {
            get { return GetValue<DateTime>("Date"); }
            set { this["Date"] = value; }
        }

        public bool IsScheduleDisabled
        {
            get { return GetValue<bool>("IsScheduleDisabled"); }
            set
            {
                this["IsScheduleDisabled"] = value;
                if (value)
                    IsScheduleOnce = IsScheduleDaily = IsScheduleWeekly = false;
            }
        }

        public bool IsScheduleOnce
        {
            get { return GetValue<bool>("IsScheduleOnce"); }
            set
            {
                this["IsScheduleOnce"] = value;
                if (value)
                    IsScheduleDisabled = IsScheduleDaily = IsScheduleWeekly = false;
            }
        }

        public bool IsScheduleDaily
        {
            get { return GetValue<bool>("IsScheduleDaily"); }
            set
            {
                this["IsScheduleDaily"] = value;
                if (value)
                    IsScheduleDisabled = IsScheduleOnce = IsScheduleWeekly = false;
            }
        }

        public bool IsScheduleWeekly
        {
            get { return GetValue<bool>("IsScheduleWeekly"); }
            set
            {
                this["IsScheduleWeekly"] = value;
                if (value)
                    IsScheduleDisabled = IsScheduleOnce = IsScheduleDaily = false;
            }
        }

        public ObservableCollection<bool> WeekDays
        {
            get { return GetValue<ObservableCollection<bool>>("WeekDays"); }
            private set { this["WeekDays"] = value; }
        }

        public bool RunAtBoot
        {
            get { return SettingsManager.Instance.RunAtBoot; }
            set
            {
                if (value == SettingsManager.Instance.RunAtBoot)
                    return;

                SettingsManager.Instance.RunAtBoot = value;
                RaisePropertyChanged("RunAtBoot");

                // enable UAC skipping
                if (RunAtBoot)
                    SkipUac = true;
            }
        }

        public bool RunScanAtLaunch
        {
            get { return SettingsManager.Instance.RunScanAtLaunch; }
            set
            {
                if (value == SettingsManager.Instance.RunScanAtLaunch)
                    return;

                SettingsManager.Instance.RunScanAtLaunch = value;
                RaisePropertyChanged("RunScanAtLaunch");
            }
        }

        public bool RunPluginUpdateAtLaunch
        {
            get { return SettingsManager.Instance.RunPluginUpdateAtLaunch; }
            set
            {
                if (value == SettingsManager.Instance.RunPluginUpdateAtLaunch)
                    return;

                SettingsManager.Instance.RunPluginUpdateAtLaunch = value;
                RaisePropertyChanged("RunPluginUpdateAtLaunch");
            }
        }

        public bool RunProgramUpdateAtLaunch
        {
            get { return SettingsManager.Instance.RunProgramUpdateAtLaunch; }
            set
            {
                if (value == SettingsManager.Instance.RunProgramUpdateAtLaunch)
                    return;

                SettingsManager.Instance.RunProgramUpdateAtLaunch = value;
                RaisePropertyChanged("RunProgramUpdateAtLaunch");
            }
        }

        public bool SkipUac
        {
            get { return SettingsManager.Instance.SkipUac; }
            set
            {
                if (value == SettingsManager.Instance.SkipUac)
                    return;

                SettingsManager.Instance.SkipUac = value;
                RaisePropertyChanged("SkipUac");

                // disable boot time execution when UAC is disabled
                if (!SkipUac)
                {
                    RunAtBoot = false;
                    ScheduleSilentRun.Execute(RunAtBoot);
                }
            }
        }

        public bool ExitOnClose
        {
            get { return SettingsManager.Instance.ExitOnClose; }
            set
            {
                if (value == SettingsManager.Instance.ExitOnClose)
                    return;

                SettingsManager.Instance.ExitOnClose = value;
                RaisePropertyChanged("ExitOnClose");
            }
        }

        public bool CloseAfterFixing
        {
            get { return SettingsManager.Instance.CloseAfterFixing; }
            set
            {
                if (value == SettingsManager.Instance.CloseAfterFixing)
                    return;

                SettingsManager.Instance.CloseAfterFixing = value;
                RaisePropertyChanged("CloseAfterFixing");
            }
        }

        public bool ShutdownAfterFixing
        {
            get { return SettingsManager.Instance.ShutdownAfterFixing; }
            set
            {
                if (value == SettingsManager.Instance.ShutdownAfterFixing)
                    return;

                SettingsManager.Instance.ShutdownAfterFixing = value;
                RaisePropertyChanged("ShutdownAfterFixing");
            }
        }

        #endregion

        #region commands

        public CommandBase ScheduleSilentRun
        {
            get { return _scheduleSilentRun; }
        }

        public CommandBase SkipUserAccountControl
        {
            get { return _skipUac; }
        }

        public CommandBase SaveSchedule
        {
            get { return _saveSchedule; }
        }

        #endregion

        object ExecuteSaveSchedule(object parameter)
        {
            var task = new TaskModel();
            task.Name = string.Format("{0}AutomaticSmartScan", Constants.InternalName);
            if (IsScheduleDisabled)
                return task.Delete();

            task.ExecutablePath = Constants.ExecutableFile;
            task.CommandLineArguments = string.Format("/{0} /{1}", CommandLineManager.CommandLineArgument.Silent, CommandLineManager.CommandLineArgument.SmartScan);
            if (IsScheduleOnce)
            {
                task.Schedule = TaskModel.ScheduleType.Once;
                task.Start = Date;
            }
            else if (IsScheduleDaily)
            {
                task.Schedule = TaskModel.ScheduleType.Daily;
                task.Start = Date;
            }
            else if (IsScheduleWeekly)
            {
                task.Schedule = TaskModel.ScheduleType.Weekly;
                task.Start = Date;
                for (var index = 0; index < WeekDays.Count; index++)
                    task.WeekDays[index] = WeekDays[index];
            }

            var result = task.CreateOrUpdate();
            if (result)
                Date = task.Start;

            return result;
        }

        void SaveScheduleExecuted(object result)
        {
            if ((bool)result)
            {
                // weekdays
                var d = new bool[WeekDays.Count];
                for (var i = 0; i < WeekDays.Count; i++)
                    d[i] = WeekDays[i];
                SettingsManager.Instance.ScheduleDays = d;
                SettingsManager.Instance.ScheduleDate = Date;
                if (IsScheduleOnce)
                    SettingsManager.Instance.ScheduleType = ScheduleType.Once;
                else if (IsScheduleDaily)
                    SettingsManager.Instance.ScheduleType = ScheduleType.Daily;
                else if (IsScheduleWeekly)
                    SettingsManager.Instance.ScheduleType = ScheduleType.Weekly;
                else
                    SettingsManager.Instance.ScheduleType = ScheduleType.None;

                UiHelper.Instance.Alert(UiHelper.Instance.Resources["Settings29"] as string);
            }
            else
                UiHelper.Instance.Error(UiHelper.Instance.Resources["Settings30"] as string);
        }
    }
}
