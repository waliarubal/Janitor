using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class SettingsBase : NotificationBase, IDisposable
    {
        readonly Dictionary<string, object> _settings;
        string _fileName;
        volatile bool _isLoaded;

        #region constructor / destructor

        protected SettingsBase()
        {
            _settings = new Dictionary<string, object>();
        }

        ~SettingsBase()
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

        #endregion

        #region properties

        public string FileName
        {
            get { return _fileName; }
        }

        public IEnumerable<string> Keys
        {
            get { return _settings.Keys; }
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

        #endregion

        protected T GetSetting<T>(string key)
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
            client.Proxy = null;
            try
            {
                var xml = client.DownloadString(url);

                var document = new XmlDocument();
                document.LoadXml(xml);
                Load(document);
            }
            catch
            {
                
            }
            client.Dispose();
        }

        public void Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                _isLoaded = true;
                _fileName = fileName;
                return;
            }

            var reader = XmlTextReader.Create(fileName);
            var document = new XmlDocument();
            document.Load(reader);
            Load(document);
            reader.Close();

            _fileName = fileName;
        }

        void Load(XmlDocument document)
        {
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

            _isLoaded = true;
        }

        void Save(string fileName = "")
        {
            if (string.IsNullOrEmpty(_fileName) && !string.IsNullOrEmpty(fileName))
                _fileName = fileName;

            FileSystemHelper.Instance.DeleteFile(_fileName);

            var writer = new XmlTextWriter(_fileName, Encoding.Default);
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
        }
    }
}
