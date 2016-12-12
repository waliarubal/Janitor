using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Core.Models
{
    public enum ScanType: byte
    {
        SmartScan,
        CustomScan
    }

    public class Scan: NotificationBase, IDisposable
    {
        ScanType _type;
        readonly ObservableCollection<ScanTargetBase> _targets;
        List<Issue> _issues;
        PluginManager _pluginManager;

        public delegate void ScanProgress(ScanProgressEventArgs e);
        public event ScanProgress OnScanProgress;

        #region constructor / destructor

        public Scan(ScanType type)
        {
            _type = type;
            _targets = new ObservableCollection<ScanTargetBase>();
            _issues = new List<Issue>();
        }

        ~Scan()
        {
            Dispose();
        }

        public void Dispose()
        {
            _issues.Clear();
            _targets.Clear();
            if (_pluginManager != null)
                _pluginManager.UnloadPlugins();
        }

        #endregion

        #region properties

        public ScanType Type
        {
            get { return _type; }
        }

        public ObservableCollection<ScanTargetBase> Targets
        {
            get { return _targets; }
        }

        public List<Issue> Issues
        {
            get { return _issues; }
            private set
            {
                if (value == _issues)
                    return;

                _issues = value;
                RaisePropertyChanged("Issues");
            }
        }

        #endregion

        void RaiseProgessChanged(ScanTargetBase target, ScanAreaBase area, bool isRunning, int targetsScanned, int areasScanned, long issueCount)
        {
            var handler = OnScanProgress;
            if (handler != null)
            {
                var progress = new ScanProgressEventArgs(target, area, isRunning);
                progress.TargetScanned = targetsScanned;
                progress.AreaScanned = areasScanned;
                progress.IssueCount = issueCount;
                handler(progress);
            }
        }

        public void Initialize()
        {
            _pluginManager = new PluginManager();
            _pluginManager.LoadPlugins();

            foreach (var target in _pluginManager.Targets)
                _targets.Add(target);
        }

        public void Analyse()
        {
            var issues = new List<Issue>();
            var targets = 0;
            var areas = 0;
            RaiseProgessChanged(null, null, true, targets, areas, issues.Count);
            foreach (var target in Targets)
            {
                if (target.IsSelected)
                {
                    targets++;
                    RaiseProgessChanged(target, null, true, targets, areas, issues.Count);
                    foreach (var area in target.Areas)
                    {
                        if (area.IsSelected)
                        {
                            areas++;
                            RaiseProgessChanged(target, area, true, targets, areas, issues.Count);
                            foreach (var issue in area.Analyse())
                            {
                                RaiseProgessChanged(target, area, true, targets, areas, issues.Count);
                                issues.Add(issue);
                            }
                        }
                            
                    }
                }
            }
            RaiseProgessChanged(null, null, false, targets, areas, issues.Count);
            Issues = issues;
        }

        public void Fix()
        {
            //foreach (var target in Targets)
            //    if (target.IsSelected)
            //        foreach (var area in target.Areas)
            //            if (area.IsSelected)
            //                foreach (var issue in area.Fix())
            //                    _issues.Remove(issue);
        }
    }
}
