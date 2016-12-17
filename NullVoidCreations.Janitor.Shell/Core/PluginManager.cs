using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Shell.Core
{
    sealed class PluginManager: IObserver
    {
        AppDomain _container;
        readonly Dictionary<string, ScanTargetBase> _targets;
        static PluginManager _instance;

        #region constructor/destructor

        private PluginManager()
        {
            _targets = new Dictionary<string, ScanTargetBase>();

            Subject.Instance.AddObserver(this);
            LoadPlugins();
        }

        ~PluginManager()
        {
            Subject.Instance.RemoveObserver(this);
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

            Subject.Instance.NotifyAllObservers(this, MessageCode.PluginsLoaded, Targets);
        }

        public void UnloadPlugins()
        {
            _targets.Clear();
            DestroyContainer();
            CreateContainer();

            Subject.Instance.NotifyAllObservers(this, MessageCode.PluginsUnloaded);
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


        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            
        }
    }
}
