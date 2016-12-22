using System;
using System.IO;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Properties;
using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class SettingsManager
    {
        static volatile SettingsManager _instance;
        string _codeName, _pluginsDirectory, _pluginsSearchFilter;
        Dictionary<string, string> _arguments;

        private SettingsManager()
        {
            _codeName = "Janitor";
            _pluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            _pluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";

            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
        }

        ~SettingsManager()
        {
            Settings.Default.Save();
        }

        #region properties

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

        public ScanType LastScan
        {
            get { return (ScanType)Settings.Default.LastScanType; }
            set { Settings.Default.LastScanType = (byte) value; }
        }

        public DateTime LastScanTime
        {
            get { return Settings.Default.LastScanTime; }
            set { Settings.Default.LastScanTime = value; }
        }

        public string LastScanSelectedAreas
        {
            get { return Settings.Default.LastScanSelectedAreas; }
            set { Settings.Default.LastScanSelectedAreas = value; }
        }

        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingsManager();

                return _instance;
            }
        }

        public string LicenseKey
        {
            get { return Settings.Default.LicenseKey; }
            set
            {
                if (value == Settings.Default.LicenseKey)
                    return;

                Settings.Default.LicenseKey = value;
            }
        }

        #endregion

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
    }
}
