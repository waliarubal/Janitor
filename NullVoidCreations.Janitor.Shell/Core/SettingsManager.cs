using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class SettingsManager : ISignalObserver
    {
        static volatile SettingsManager _instance;
        string _codeName, _pluginsDirectory, _pluginsSearchFilter;
        readonly string _settingsFile, _assemblyPath;
        readonly Dictionary<string, object> _settings;
        volatile bool _isLoaded;

        const char Separator1 = '♪';
        const char Separator2 = '♫';

        private SettingsManager()
        {
            _codeName = "Janitor";
            _pluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            _pluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";
            _assemblyPath = Assembly.GetExecutingAssembly().Location;

            _settings = new Dictionary<string, object>();
            _settingsFile = Path.Combine(KnownPaths.Instance.ApplicationDirectory, "Settings.dat");
            Load();

        }

        ~SettingsManager()
        {
            Save();
            _isLoaded = false;
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

        public string ExecutablePath
        {
            get { return _assemblyPath; }
        }

        public string CodeName
        {
            get { return _codeName; }
        }

        public string PluginsDirectory
        {
            get { return _pluginsDirectory; }
        }

        public string PluginsSearchFilter
        {
            get { return _pluginsSearchFilter; }
        }

        public object this[string key]
        {
            get
            {
                return _settings.ContainsKey(key) ? _settings[key] : null;
            }
            set
            {
                if (_settings.ContainsKey(key))
                    _settings[key] = value;
                else
                    _settings.Add(key, value);
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

        public DateTime ScheduleTime
        {
            get { return GetSetting<DateTime>("ScheduleTime"); }
            set { this["ScheduleTime"] = value; }
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

        T GetSetting<T>(string key)
        {
            T setting = default(T);

            if (!_isLoaded)
                return setting;
            if (string.IsNullOrEmpty(key))
                return setting;

            var type = typeof(T);
            try
            {
                if (type.IsEnum)
                    setting = (T)Enum.Parse(type, GetSetting<string>(key));
                else
                    setting = (T)Convert.ChangeType(_settings[key], typeof(T));
            }
            catch (Exception ex)
            {

            }

            return setting;
        }

        void Load()
        {
            if (!File.Exists(_settingsFile))
                goto LOADED;

            var settings = File.ReadAllText(_settingsFile).Split(new char[] { Separator2 });
            foreach (var setting in settings)
            {
                var settingEntry = setting.Split(new char[] { Separator1 });
                if (settingEntry.Length == 0)
                    continue;

                var key = settingEntry[0];
                var value = settingEntry.Length > 1 ? settingEntry[1] : null;

                if (_settings.ContainsKey(key))
                    _settings[key] = value;
                else
                    _settings.Add(key, value);
            }

        LOADED:
            _isLoaded = true;
            SignalHost.Instance.RaiseSignal(this, Signal.SettingsLoaded);
        }

        void Save()
        {
            var data = new StringBuilder();
            foreach (var key in _settings.Keys)
                data.AppendFormat("{2}{1}{3}{0}", Separator2, Separator1, key, _settings[key]);

            File.WriteAllText(_settingsFile, data.ToString());
            SignalHost.Instance.RaiseSignal(this, Signal.SettingsSaved);
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {

        }
    }
}
