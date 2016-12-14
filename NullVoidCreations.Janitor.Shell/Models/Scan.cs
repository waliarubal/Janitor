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

    public class Scan: NotificationBase, IDisposable, IObserver
    {
        ScanType _type;
        readonly ObservableCollection<ScanTargetBase> _targets;
        List<Issue> _issues;

        #region constructor / destructor

        public Scan(ScanType type)
        {
            _type = type;
            _targets = new ObservableCollection<ScanTargetBase>();
            _issues = new List<Issue>();

            foreach (var target in PluginManager.Instance.Targets)
                _targets.Add(target);

            Subject.Instance.AddObserver(this);
        }

        ~Scan()
        {
            Dispose();
        }

        public void Dispose()
        {
            Subject.Instance.RemoveObserver(this);

            _issues.Clear();
            _targets.Clear();
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

        void RaiseProgessChanged(
            ScanTargetBase target, 
            ScanAreaBase area, 
            bool isRunning, 
            int targetsScanned, 
            int areasScanned, 
            long issueCount, 
            int progressMax, 
            int progressMin, 
            int progressCurrent)
        {
            var status = new ScanStatus(target, area, isRunning);
            status.TargetScanned = targetsScanned;
            status.AreaScanned = areasScanned;
            status.IssueCount = issueCount;
            status.ProgressMax = progressMax;
            status.ProgressMin = progressMin;
            status.ProgressCurrent = progressCurrent;
            Subject.Instance.NotifyAllObservers(this, MessageCode.ScanStatusChanged, status);
        }

        internal void Analyse()
        {
            Subject.Instance.NotifyAllObservers(this, MessageCode.ScanStarted);

            var issues = new List<Issue>();
            var targets = 0;
            var areas = 0;

            var progressCurrent = 0;
            var progressMax = 0;
            foreach (var target in Targets)
                if (target.IsSelected)
                    foreach (var area in target.Areas)
                        if (area.IsSelected)
                            progressMax++;

            RaiseProgessChanged(null, null, true, targets, areas, issues.Count, progressMax, 0, progressCurrent);
            foreach (var target in Targets)
            {
                if (target.IsSelected)
                {
                    targets++;
                    RaiseProgessChanged(target, null, true, targets, areas, issues.Count, progressMax, 0, progressCurrent);
                    foreach (var area in target.Areas)
                    {
                        if (area.IsSelected)
                        {
                            progressCurrent++;
                            areas++;
                            RaiseProgessChanged(target, area, true, targets, areas, issues.Count, progressMax, 0, progressCurrent);
                            foreach (var issue in area.Analyse())
                            {
                                RaiseProgessChanged(target, area, true, targets, areas, issues.Count, progressMax, 0, progressCurrent);
                                issues.Add(issue);
                            }
                        }
                            
                    }
                }
            }
            RaiseProgessChanged(null, null, false, targets, areas, issues.Count, progressMax, 0, progressCurrent);
            Issues = issues;

            Subject.Instance.NotifyAllObservers(this, MessageCode.ScanStopped);
        }

        internal void Fix()
        {
            //foreach (var target in Targets)
            //    if (target.IsSelected)
            //        foreach (var area in target.Areas)
            //            if (area.IsSelected)
            //                foreach (var issue in area.Fix())
            //                    _issues.Remove(issue);
        }

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            
        }
    }
}
