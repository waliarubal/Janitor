﻿using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Core
{
    sealed class PluginManager: ISignalObserver
    {
        AppDomain _container;
        readonly Dictionary<string, ScanTargetBase> _targets;
        static PluginManager _instance;

        #region constructor/destructor

        private PluginManager()
        {
            _targets = new Dictionary<string, ScanTargetBase>();

            SignalHost.Instance.AddObserver(this);

            // create plugins directory if missing
            if (!Directory.Exists(SettingsManager.Instance.PluginsDirectory))
                Directory.CreateDirectory(SettingsManager.Instance.PluginsDirectory);

            LoadPlugins();
        }

        ~PluginManager()
        {
            SignalHost.Instance.RemoveObserver(this);
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
            private set
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

        public bool UpdatePlugins(string archiveFile)
        {
            if (string.IsNullOrEmpty(archiveFile))
                return false;
            if (!File.Exists(archiveFile))
                return false;

            var isUpdated = true;
            UnloadPlugins();
            using (var zip = ZipFile.Read(archiveFile))
            {
                var path = SettingsManager.Instance.PluginsDirectory;
                foreach (var entry in zip)
                {
                    try
                    {
                        entry.Extract(path, ExtractExistingFileAction.OverwriteSilently);
                    }
                    catch (Exception ex)
                    {
                        isUpdated = false;
                    }
                }
            }
            LoadPlugins();

            return isUpdated;
        }

        public void LoadPlugins()
        {
            UnloadPlugins();

            var scanTargetType = typeof(ScanTargetBase);
            var proxyType = typeof(Proxy);
            var proxy = (Proxy)_container.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName);

            var pluginFiles = Directory.GetFiles(SettingsManager.Instance.PluginsDirectory, SettingsManager.Instance.PluginsSearchFilter);
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

            SignalHost.Instance.NotifyAllObservers(this, Signal.PluginsLoaded, Targets);
        }

        public void UnloadPlugins()
        {
            _targets.Clear();
            DestroyContainer();
            CreateContainer();

            SignalHost.Instance.NotifyAllObservers(this, Signal.PluginsUnloaded);
        }

        void CreateContainer()
        {
            var evidence = AppDomain.CurrentDomain.Evidence;

            var setupInfo = new AppDomainSetup();
            setupInfo.ApplicationBase = SettingsManager.Instance.PluginsDirectory;

            _container = AppDomain.CreateDomain("ScanTargets", evidence, setupInfo);
        }

        void DestroyContainer()
        {
            if (_container != null)
                AppDomain.Unload(_container);

            _container = null;
        }


        public void Update(ISignalObserver sender, Signal code, params object[] data)
        {
            
        }
    }
}
