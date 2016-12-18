using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Core.Models
{
    public enum ScanType: byte
    {
        SmartScan,
        CustomScan
    }

    public class ScanModel: NotificationBase, IDisposable
    {
        ScanType _type;
        string _name;
        readonly ObservableCollection<ScanTargetBase> _targets;
        List<Issue> _issues;

        #region constructor / destructor

        public ScanModel(ScanType type)
        {
            _type = type;
            _name = _type == ScanType.SmartScan ? "Smart Scan" : "Custom Scan";
            _targets = new ObservableCollection<ScanTargetBase>();
            _issues = new List<Issue>();

            foreach (var target in PluginManager.Instance.Targets)
            {
                target.Reset(type == ScanType.SmartScan);
                _targets.Add(target);
            }
        }

        ~ScanModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            _issues.Clear();

            // TODO: this throws cross thread exception, investigate this
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => _targets.Clear()));
        }

        #endregion

        #region properties

        public ScanType Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public ObservableCollection<ScanTargetBase> Targets
        {
            get { return _targets; }
        }

        public List<Issue> Issues
        {
            get { return _issues; }
            internal set
            {
                if (value == _issues)
                    return;

                _issues = value;
                RaisePropertyChanged("Issues");
            }
        }

        #endregion
    }
}
