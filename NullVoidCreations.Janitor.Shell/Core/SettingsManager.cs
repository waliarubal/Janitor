using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class SettingsManager : ISignalObserver, IDisposable
    {
        static volatile SettingsManager _instance;
        readonly string _settingsFile;
        readonly Dictionary<string, object> _settings;
        volatile bool _isLoaded;

        private SettingsManager()
        {
            _settings = new Dictionary<string, object>();
            _settingsFile = Path.Combine(KnownPaths.Instance.ApplicationDirectory, "Settings.dat");

            if (!UiHelper.Instance.DesignMode)
                Load(_settingsFile);
        }

        ~SettingsManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
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
            if (string.IsNullOrEmpty(key) || !_settings.ContainsKey(key))
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

        public void Load(Uri url)
        {
            var client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Setings_DownloadStringCompleted);
            client.Proxy = null;
            client.DownloadStringAsync(url, client);
        }

        void Setings_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var client = e.UserState as WebClient;
            client.DownloadStringCompleted -= new DownloadStringCompletedEventHandler(Setings_DownloadStringCompleted);
            client.Dispose();

            if (e.Error != null)
                return;

            var document = new XmlDocument();
            document.LoadXml(e.Result);
            Load(document);
        }

        public void Load(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            var reader = XmlTextReader.Create(fileName);
            var document = new XmlDocument();
            document.Load(reader);
            Load(document);
            reader.Close();
        }

        void Load(XmlDocument document)
        {
            if (UiHelper.Instance.DesignMode)
                goto LOADED;

            var nodes = document.SelectNodes("/Settings/Setting");
            foreach (XmlNode node in nodes)
            {
                var key = node.Attributes["Key"].Value;
                var value = node.Attributes["Value"].Value;
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
            var writer = new XmlTextWriter(_settingsFile, Encoding.Default);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            var xmlDocument = new XmlDocument();
            var rootNode = xmlDocument.CreateElement("Settings");
            foreach (var key in _settings.Keys)
            {
                var node = xmlDocument.CreateElement("Setting");

                var attribute = xmlDocument.CreateAttribute("Key");
                attribute.Value = key;
                node.Attributes.Append(attribute);

                attribute = xmlDocument.CreateAttribute("Value");
                attribute.Value = _settings[key].ToString();
                node.Attributes.Append(attribute);

                rootNode.AppendChild(node);
            }
            xmlDocument.AppendChild(rootNode);
            xmlDocument.Save(writer);

            SignalHost.Instance.RaiseSignal(this, Signal.SettingsSaved);
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {

        }
    }
}
