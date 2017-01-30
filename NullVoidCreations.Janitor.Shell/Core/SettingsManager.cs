using System;
using System.IO;
using System.Text;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class SettingsManager : SettingsBase, ISignalObserver
    {
        static volatile SettingsManager _instance;

        private SettingsManager()
        {
            if (!UiHelper.Instance.DesignMode)
            {
                var fileName = Path.Combine(KnownPaths.Instance.ApplicationDirectory, "Settings.dat");
                Load(fileName);
                SignalHost.Instance.RaiseSignal(this, Signal.SettingsLoaded);
            }
        }

        #region properties

        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingsManager();

                return _instance;
            }
        }

        public Version PluginsVersion
        {
            get
            {
                var versionString = GetSetting<string>("PluginsVersion");
                if (versionString == null)
                    versionString = "0.0.0.0";

                return new Version(versionString);
            }
            set { this["PluginsVersion"] = value.ToString(); }
        }

        public DateTime FirstExecution
        {
            get { return GetSetting<DateTime>("FirstExecutionDate"); }
            set { this["FirstExecutionDate"] = value; }
        }

        public DateTime LastPluginUpdateCheck
        {
            get { return GetSetting<DateTime>("LastPluginUpdateDate"); }
            set { this["LastPluginUpdateDate"] = value; }
        }

        public DateTime LastProgramUpdateCheck
        {
            get { return GetSetting<DateTime>("LastProgramUpdateDate"); }
            set { this["LastProgramUpdateDate"] = value; }
        }

        public bool RunAtBoot
        {
            get { return GetSetting<bool>("RunAtBoot"); }
            set { this["RunAtBoot"] = value; }
        }

        public bool RunScanAtLaunch
        {
            get { return GetSetting<bool>("RunScanAtLaunch"); }
            set { this["RunScanAtLaunch"] = value; }
        }

        public bool RunPluginUpdateAtLaunch
        {
            get { return GetSetting<bool>("RunPluginUpdateAtLaunch"); }
            set { this["RunPluginUpdateAtLaunch"] = value; }
        }

        public bool RunProgramUpdateAtLaunch
        {
            get { return GetSetting<bool>("RunProgramUpdateAtLaunch"); }
            set { this["RunProgramUpdateAtLaunch"] = value; }
        }

        public bool SkipUac
        {
            get { return GetSetting<bool>("SkipUac"); }
            set { this["SkipUac"] = value; }
        }

        public bool ExitOnClose
        {
            get { return GetSetting<bool>("ExitOnClose"); }
            set { this["ExitOnClose"] = value; }
        }

        public bool CloseAfterFixing
        {
            get { return GetSetting<bool>("CloseAfterFixing"); }
            set { this["CloseAfterFixing"] = value; }
        }

        public bool ShutdownAfterFixing
        {
            get { return GetSetting<bool>("ShutdownAfterFixing"); }
            set { this["ShutdownAfterFixing"] = value; }
        }

        public string LicenseKey
        {
            get { return GetSetting<string>("LicenseKey"); }
            set
            {
                if (value == GetSetting<string>("LicenseKey"))
                    return;

                this["LicenseKey"] = value;
            }
        }

        public ScanType LastScan
        {
            get { return (ScanType)GetSetting<byte>("LastScan"); }
            set { this["LastScan"] = (byte)value; }
        }

        public DateTime LastScanTime
        {
            get { return GetSetting<DateTime>("LastScanTime"); }
            set { this["LastScanTime"] = value; }
        }

        public string LastScanSelectedAreas
        {
            get { return GetSetting<string>("LastScanSelectedAreas"); }
            set { this["LastScanSelectedAreas"] = value; }
        }

        public DateTime ScheduleDate
        {
            get { return GetSetting<DateTime>("ScheduleDate"); }
            set { this["ScheduleDate"] = value; }
        }

        public ScheduleType ScheduleType
        {
            get { return GetSetting<ScheduleType>("ScheduleType"); }
            set { this["ScheduleType"] = value; }
        }

        public bool[] ScheduleDays
        {
            get
            {
                var days = GetSetting<string>("ScheduleDays");
                if (string.IsNullOrEmpty(days) || days.Length != 7)
                    return new bool[7];

                var daysBool = new bool[days.Length];
                for (var index = 0; index < days.Length; index++)
                    daysBool[index] = days[index] == '1';
                return daysBool;
            }
            set
            {
                var daysString = new StringBuilder(value.Length);
                for (var index = 0; index < value.Length; index++)
                    daysString.Append(value[index] ? '1' : '0');
                this["ScheduleDays"] = daysString.ToString();
            }
        }

        #endregion

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {

        }
    }
}
