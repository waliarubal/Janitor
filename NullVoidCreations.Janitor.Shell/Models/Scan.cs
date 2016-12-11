using System;
using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public enum ScanType: byte
    {
        SmartScan,
        CustomScan
    }

    public class Scan: NotificationBase, IDisposable
    {
        ScanType _type;
        ObservableCollection<ScanTargetBase> _targets;
        PluginManager _pluginManager;
        ScanProgress _progress;

        #region constructor / destructor

        public Scan()
        {
            _targets = new ObservableCollection<ScanTargetBase>();
        }

        ~Scan()
        {
            Dispose();
        }

        public void Dispose()
        {
            _targets.Clear();
            if (_pluginManager != null)
                _pluginManager.UnloadPlugins();
        }

        #endregion

        #region properties

        public ScanType Type
        {
            get { return _type; }
            set
            {
                if (value == _type)
                    return;

                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        public ObservableCollection<ScanTargetBase> Targets
        {
            get { return _targets; }
        }

        public ScanProgress Progress
        {
            get { return _progress; }
            set
            {
                if (value == _progress)
                    return;

                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        #endregion

        public void Initialize()
        {
            _pluginManager = new PluginManager();
            _pluginManager.LoadPlugins();

            foreach (var target in _pluginManager.Targets)
                _targets.Add(target);
        }
    }
}
