using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Commands;

namespace NullVoidCreations.Janitor.Shell.Core
{
    sealed class PluginManager
    {
        AppDomain _container;
        readonly Dictionary<string, ScanTargetBase> _targets;
        static PluginManager _instance;

        #region constructor/destructor

        private PluginManager()
        {
            _targets = new Dictionary<string, ScanTargetBase>();

            // create plugins directory if missing
            if (!Directory.Exists(Constants.PluginsDirectory))
                Directory.CreateDirectory(Constants.PluginsDirectory);

            // install any pending plugins update
            if (File.Exists(UpdateCommand.PluginsUpdateFile))
                UpdatePlugins(UpdateCommand.PluginsUpdateFile);
            else
                LoadPlugins();
        }

        #endregion

        #region properties

        public static PluginManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PluginManager();

                return _instance;
            }
        }

        public Version Version
        {
            get { return SettingsManager.Instance.PluginsVersion; }
            internal set
            {
                if (value == SettingsManager.Instance.PluginsVersion)
                    return;

                SettingsManager.Instance.PluginsVersion = value;
            }
        }

        public ScanTargetBase this[string name]
        {
            get
            {
                if (_targets.ContainsKey(name))
                    return _targets[name];

                return null;
            }
        }

        public IEnumerable<ScanTargetBase> Targets
        {
            get { return _targets.Values; }
        }

        #endregion

        bool UpdatePlugins(string archiveFile)
        {
            if (string.IsNullOrEmpty(archiveFile))
                return false;
            if (!File.Exists(archiveFile))
                return false;

            var isUpdated = true;
            UnloadPlugins();
            var zip = ZipStorer.Open(archiveFile, FileAccess.Read);
            var directory = zip.ReadCentralDir();
            foreach (var entry in directory)
            {
                var fileName = Path.GetFileName(entry.FilenameInZip);
                var filePath = Path.Combine(Constants.PluginsDirectory, fileName);
                zip.ExtractFile(entry, filePath);
            }
            LoadPlugins();

            FileSystemHelper.Instance.DeleteFile(archiveFile);

            return isUpdated;
        }

        public void LoadPlugins()
        {
            UnloadPlugins();

            var scanTargetType = typeof(ScanTargetBase);
            var proxyType = typeof(Proxy);
            var proxy = (Proxy)_container.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName);

            var pluginFiles = Directory.GetFiles(Constants.PluginsDirectory, Constants.PluginsSearchFilter);
            foreach (var pluginFile in pluginFiles)
            {
                var assembly = proxy.GetAssembly(pluginFile);
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(scanTargetType))
                    {
                        var target = (ScanTargetBase) assembly.CreateInstance(type.FullName, false);
                        if (_targets.ContainsKey(target.Name))
                            continue;

                        _targets.Add(target.Name, target);
                    }
                }
            }

            SignalHost.Instance.RaiseSignal(Signal.PluginsLoaded, Targets);
        }

        public void UnloadPlugins()
        {
            _targets.Clear();
            DestroyContainer();
            CreateContainer();

            SignalHost.Instance.RaiseSignal(Signal.PluginsUnloaded);
        }

        void CreateContainer()
        {
            var evidence = AppDomain.CurrentDomain.Evidence;

            var setupInfo = new AppDomainSetup();
            setupInfo.ApplicationBase = Constants.PluginsDirectory;

            _container = AppDomain.CreateDomain("ScanTargets", evidence, setupInfo);
        }

        void DestroyContainer()
        {
            if (_container != null)
                AppDomain.Unload(_container);

            _container = null;
        }
    }
}
