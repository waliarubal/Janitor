using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Core.Models
{
    public enum ScanType: byte
    {
        Unknown,
        SmartScan,
        CustomScan
    }

    public class ScanModel: NotificationBase, IDisposable
    {
        ScanType _type;
        string _name;
        readonly ObservableCollection<ScanTargetBase> _targets;
        List<IssueBase> _issues;

        #region constructor / destructor

        public ScanModel(ScanType type)
        {
            _type = type;
            _name = _type == ScanType.SmartScan ? "Smart Scan" : "Custom Scan";
            _targets = new ObservableCollection<ScanTargetBase>();
            _issues = new List<IssueBase>();

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
            UiHelper.Instance.ExecuteOnUiThread(new Action(() => _targets.Clear()));
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

        public bool IsFixed
        {
            get;
            internal set;
        }

        public bool IsCancelled
        {
            get;
            internal set;
        }

        public List<IssueBase> Issues
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

        internal static ScanModel GetSavedScanDetails()
        {
            var scan = new ScanModel(SettingsManager.Instance.LastScan);
            if (scan.Type == ScanType.CustomScan)
            {
                var selectedAreaKeys = new HashSet<string>(SettingsManager.Instance.LastScanSelectedAreas.Split(new char[] { 'Ӫ' }, StringSplitOptions.RemoveEmptyEntries));
                if (selectedAreaKeys.Count > 0)
                {
                    for (var index = scan.Targets.Count - 1; index >= 0; index--)
                    {
                        var target = scan.Targets[index];
                        var hasSelectedArea = false;
                        foreach (var area in target.Areas)
                        {
                            if (selectedAreaKeys.Contains(string.Format("{0}{2}{1}", target.Name, area.Name, 'ӝ')))
                            {
                                area.IsSelected = true;
                                hasSelectedArea = true;
                            }
                        }

                        if (!hasSelectedArea)
                            scan.Targets.RemoveAt(index);
                    }
                }
            }

            return scan;
        }

        internal static void SaveScanDetails(ScanModel scan)
        {
            if (scan == null || scan.IsCancelled)
                return;

            if (scan.Type == ScanType.CustomScan)
            {
                var selectedAreaKeys = new StringBuilder();
                foreach (var target in scan.Targets)
                {
                    foreach (var area in target.Areas)
                    {
                        if (area.IsSelected)
                        {
                            selectedAreaKeys.AppendFormat("{0}{2}{1}{3}", target.Name, area.Name, 'ӝ', 'Ӫ');
                        }
                    }
                }

                SettingsManager.Instance.LastScanSelectedAreas = selectedAreaKeys.ToString();
            }
            else
                SettingsManager.Instance.LastScanSelectedAreas = string.Empty;

            SettingsManager.Instance.LastScan = scan.Type;
            SettingsManager.Instance.LastScanTime = DateTime.Now;
        }
    }
}
