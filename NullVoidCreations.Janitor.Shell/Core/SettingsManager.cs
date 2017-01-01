using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class SettingsManager : ISignalObserver
    {
        static volatile SettingsManager _instance;
        string _codeName, _pluginsDirectory, _pluginsSearchFilter;
        readonly string _settingsFile;
        readonly Dictionary<string, object> _settings;
        volatile bool _isLoaded;

        const char Separator1 = '♪';
        const char Separator2 = '♫';

        private SettingsManager()
        {
            SignalHost.Instance.AddObserver(this);

            _codeName = "Janitor";
            _pluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            _pluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";

            _settings = new Dictionary<string, object>();
            _settingsFile = Path.Combine(KnownPaths.Instance.ApplicationDirectory, "Settings.dat");
            Load();

        }

        ~SettingsManager()
        {
            Save();
            _isLoaded = false;
            SignalHost.Instance.RemoveObserver(this);
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

        internal Dictionary<string, string> CommandLineArguments
        {
            get;
            private set;
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

        #endregion

        /// <summary>
        /// This method takes care of first time initialization.
        /// </summary>
        void FirstTimeInitialization()
        {
            if (GetSetting<bool>("IsInitializedInPast"))
                return;

            this["IsInitializedInPast"] = true;

            RunPluginUpdateAtLaunch = true;
            RunScanAtLaunch = true;
        }

        T GetSetting<T>(string key)
        {
            T setting = default(T);

            if (!_isLoaded)
                return setting;
            if (string.IsNullOrEmpty(key))
                return setting;

            try
            {
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
            FirstTimeInitialization();
            SignalHost.Instance.NotifyAllObservers(this, Signal.SettingsLoaded);
        }

        void Save()
        {
            var data = new StringBuilder();
            foreach (var key in _settings.Keys)
                data.AppendFormat("{2}{1}{3}{0}", Separator2, Separator1, key, _settings[key]);

            File.WriteAllText(_settingsFile, data.ToString());
            SignalHost.Instance.NotifyAllObservers(this, Signal.SettingsSaved);
        }

        internal void LoadArguments(string[] arguments)
        {
            CommandLineArguments = new Dictionary<string, string>();
            foreach (var argument in arguments)
            {
                if (string.IsNullOrEmpty(argument))
                    continue;
                if (!argument.StartsWith("/"))
                    continue;

                var parts = argument.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    var argName = parts[0].Remove(0, 1);
                    var argValue = string.Empty;
                    if (parts.Length > 1)
                        argValue = parts[1];

                    if (CommandLineArguments.ContainsKey(argName))
                        CommandLineArguments[argName] = argValue;
                    else
                        CommandLineArguments.Add(argName, argValue);
                }
            }
        }

        public void Update(ISignalObserver sender, Signal code, params object[] data)
        {

        }
    }
}
