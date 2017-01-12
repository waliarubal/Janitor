using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using System.Diagnostics;
using NullVoidCreations.Janitor.Shell.Models;
using Microsoft.Win32.TaskScheduler;
using System;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
        enum ScheduleType : byte
        {
            None,
            Once,
            Daily,
            Weekly
        }

        readonly ObservableCollection<bool> _weekDays;
        readonly CommandBase _scheduleSilentRun, _skipUac, _saveSchedule;
        bool _isScheduleDisabled, _isScheduleOnce, _isScheduleDaily, _isScheduleWeekly;
        DateTime _schedule;
        ScheduleType _type;

        public SettingsViewModel()
        {
            _weekDays = new ObservableCollection<bool>();
            for (var index = 0; index < 7; index++)
                _weekDays.Add(false);

            _isScheduleDisabled = true;
            _scheduleSilentRun = new ScheduleSilentRunCommand(this);
            _skipUac = new SkipUacCommand(this);
            _saveSchedule = new AsyncDelegateCommand(this, null, ExecuteSaveSchedule, SaveScheduleExecuted);
            _scheduleSilentRun.IsEnabled = _skipUac.IsEnabled = _saveSchedule.IsEnabled = true;
        }

        #region properties

        private ScheduleType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                switch(_type)
                {
                    case ScheduleType.None:
                        IsScheduleOnce = IsScheduleDaily = IsScheduleWeekly = false;
                        break;

                    case ScheduleType.Once:
                        IsScheduleDisabled = IsScheduleDaily = IsScheduleWeekly = false;
                        break;

                    case ScheduleType.Daily:
                        IsScheduleDisabled = IsScheduleOnce = IsScheduleWeekly = false;
                        break;

                    case ScheduleType.Weekly:
                        IsScheduleDisabled = IsScheduleOnce = IsScheduleDaily = false;
                        break;
                }
            }
        }

        public DateTime Schedule
        {
            get { return _schedule; }
            set
            {
                if (value == _schedule)
                    return;

                _schedule = value;
                RaisePropertyChanged("Schedule");
            }
        }

        public bool IsScheduleDisabled
        {
            get { return _isScheduleDisabled; }
            set
            {
                if (value == IsScheduleDisabled)
                    return;

                _isScheduleDisabled = value;
                if (!_isScheduleDisabled) Type = ScheduleType.None;
                RaisePropertyChanged("IsScheduleDisabled");
            }
        }

        public bool IsScheduleOnce
        {
            get { return _isScheduleOnce; }
            set
            {
                if (value == _isScheduleOnce)
                    return;

                _isScheduleOnce = value;
                if (!_isScheduleOnce) Type = ScheduleType.Once;
                RaisePropertyChanged("IsScheduleOnce");
            }
        }

        public bool IsScheduleDaily
        {
            get { return _isScheduleDaily; }
            set
            {
                if (value == _isScheduleDaily)
                    return;

                _isScheduleDaily = value;
                if (!_isScheduleDaily) Type = ScheduleType.Daily;
                RaisePropertyChanged("IsScheduleDaily");
            }
        }

        public bool IsScheduleWeekly
        {
            get { return _isScheduleWeekly; }
            set
            {
                if (value == _isScheduleWeekly)
                    return;

                _isScheduleWeekly = value;
                if (!_isScheduleWeekly) Type = ScheduleType.Weekly;
                RaisePropertyChanged("IsScheduleWeekly");
            }
        }

        public ObservableCollection<bool> WeekDays
        {
            get { return _weekDays; }
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
            task.Name = string.Format("{0}AutomaticSmartScan", App.ProductName);
            if (IsScheduleDisabled)
                return task.Delete();

            task.ExecutablePath = SettingsManager.Instance.ExecutablePath;
            task.CommandLineArguments = string.Format("/{0} /{1}", CommandLineManager.CommandLineArgument.Silent, CommandLineManager.CommandLineArgument.FixIssues);

            switch(Type)
            {
                case ScheduleType.Once:
                    task.Schedule = new TimeTrigger(DateTime.Now);
                    break;

                case ScheduleType.Daily:
                    task.Schedule = new DailyTrigger();
                    task.Schedule.StartBoundary = DateTime.Now;
                    break;

                case ScheduleType.Weekly:
                    task.Schedule = new WeeklyTrigger(DaysOfTheWeek.Monday | DaysOfTheWeek.Saturday);
                    task.Schedule.StartBoundary = DateTime.Now;
                    break;
            }
            

            return task.Create();
        }

        void SaveScheduleExecuted(object result)
        {

        }
    }
}
