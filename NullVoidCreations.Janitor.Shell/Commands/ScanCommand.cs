using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;
using NullVoidCreations.Janitor.Shell.ViewModels;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class ScanCommand : CommandBase, IObserver
    {
        ComputerScanViewModel _viewModel;
        BackgroundWorker _worker;
        LicenseModel _license;

        #region constructor / destructor

        public ScanCommand(ComputerScanViewModel viewModel)
            : base(viewModel)
        {
            _viewModel = ViewModel as ComputerScanViewModel;
            IsRecallAllowed = true;

            Subject.Instance.AddObserver(this);
        }

        ~ScanCommand()
        {
            Subject.Instance.RemoveObserver(this);

            if (_worker == null)
                return;

            _worker.DoWork -= new DoWorkEventHandler(Worker_DoWork);
            _worker.ProgressChanged -= new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.Dispose();
        }

        #endregion

        public override void Execute(object parameter)
        {
            IsEnabled = true;
            switch (parameter as string)
            {
                case "Smart":
                    if (_viewModel.IsExecuting)
                        break;

                    _viewModel.Scan = new ScanModel(ScanType.SmartScan);
                    StartScan(_viewModel.Scan);
                    break;

                case "Custom":
                    if (_viewModel.IsExecuting)
                        break;

                    _viewModel.Scan = new ScanModel(ScanType.CustomScan);

                    var scanParametersView = new CustomScanView(_viewModel.Scan.Targets);
                    scanParametersView.Owner = Application.Current.MainWindow;
                    var result = scanParametersView.ShowDialog();
                    if (result.HasValue && result.Value == true)
                        StartScan(_viewModel.Scan);
                    break;

                case "Repeat":
                    if (_viewModel.IsExecuting)
                        break;

                    _viewModel.Scan = GetSavedScanDetails();
                    StartScan(_viewModel.Scan);
                    break;

                case "Cancel":
                    CancelScan();
                    break;

                case "Fix":
                    if (_viewModel.IsExecuting)
                        break;

                    if (_license.IsTrial)
                    {
                        _viewModel.Activate.Execute(null);
                        break;
                    }

                    // start scan if it wasen't done earlier
                    if (_viewModel.Scan == null)
                    {
                        _viewModel.Scan = new ScanModel(ScanType.SmartScan);
                        StartScan(_viewModel.Scan);
                        break;
                    }

                    StartFix(_viewModel.Scan);
                    break;
            }
        }

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            switch (code)
            {
                case MessageCode.ScanStatusChanged:
                    if (_worker.IsBusy)
                        _worker.ReportProgress(-1, data[0]);
                    break;

                case MessageCode.LicenseChanged:
                    _license = NullVoidCreations.Janitor.Shell.Core.LicenseManager.Instance.License;
                    break;
            }
        }

        void StartScan(ScanModel scan)
        {
            _viewModel.IsExecuting = IsExecuting = true;

            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);

            _worker.RunWorkerAsync(new object[] { true, _viewModel.Scan });
        }

        void StartFix(ScanModel scan)
        {
            _viewModel.IsExecuting = IsExecuting = true;

            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);

            _worker.RunWorkerAsync(new object[] { false, _viewModel.Scan });
        }

        void RaiseProgessChanged(
            ScanTargetBase target,
            ScanAreaBase area,
            bool isScanning,
            bool isFixing,
            int targetsScanned,
            int areasScanned,
            long issueCount,
            int progressMax,
            int progressMin,
            int progressCurrent,
            bool isExecuting)
        {
            var status = new ScanStatusModel(target, area, isScanning, isFixing, isExecuting);
            status.TargetScanned = targetsScanned;
            status.AreaScanned = areasScanned;
            status.IssueCount = issueCount;
            status.ProgressMax = progressMax;
            status.ProgressMin = progressMin;
            status.ProgressCurrent = progressCurrent;
            Subject.Instance.NotifyAllObservers(this, MessageCode.ScanStatusChanged, status);
        }

        ScanModel Analyse(ScanModel scan)
        {
            Subject.Instance.NotifyAllObservers(this, MessageCode.AnalysisStarted, false);

            var issues = new List<IssueBase>();
            var targets = 0;
            var areas = 0;

            var progressCurrent = 0;
            var progressMax = 0;
            foreach (var target in scan.Targets)
                foreach (var area in target.Areas)
                    if (area.IsSelected)
                        progressMax++;

            if (_worker.CancellationPending)
                goto EXIT_SCAN;

            RaiseProgessChanged(null, null, true, false, targets, areas, issues.Count, progressMax, 0, progressCurrent, true);
            foreach (var target in scan.Targets)
            {
                if (_worker.CancellationPending)
                    goto EXIT_SCAN;

                targets++;
                RaiseProgessChanged(target, null, true, false, targets, areas, issues.Count, progressMax, 0, progressCurrent, true);
                foreach (var area in target.Areas)
                {
                    if (area.IsSelected)
                    {
                        if (_worker.CancellationPending)
                            goto EXIT_SCAN;

                        progressCurrent++;
                        areas++;
                        RaiseProgessChanged(target, area, true, false, targets, areas, issues.Count, progressMax, 0, progressCurrent, true);
                        foreach (var issue in area.Analyse())
                        {
                            if (_worker.CancellationPending)
                                goto EXIT_SCAN;

                            RaiseProgessChanged(target, area, true, false, targets, areas, issues.Count, progressMax, 0, progressCurrent, true);
                            issues.Add(issue);
                            Thread.Sleep(2);
                        }
                    }
                }
            }

        EXIT_SCAN:
            RaiseProgessChanged(null, null, true, false, targets, areas, issues.Count, progressMax, 0, progressCurrent, false);
            scan.Issues = issues;

            SaveScanDetails(scan);
            Subject.Instance.NotifyAllObservers(this, MessageCode.AnalysisStopped, issues.Count);

            return scan;
        }

        ScanModel Fix(ScanModel scan)
        {
            Subject.Instance.NotifyAllObservers(this, MessageCode.FixingStarted);

            var issues = new List<IssueBase>();
            var targets = 0;
            var areas = 0;

            var progressCurrent = 0;
            var progressMax = 0;
            foreach (var target in scan.Targets)
                foreach (var area in target.Areas)
                    if (area.IsSelected)
                        progressMax++;


            if (_worker.CancellationPending)
                goto EXIT_SCAN;

            RaiseProgessChanged(null, null, false, true, targets, areas, issues.Count, progressMax, 0, progressCurrent, true);
            foreach (var target in scan.Targets)
            {
                if (_worker.CancellationPending)
                    goto EXIT_SCAN;

                targets++;
                RaiseProgessChanged(target, null, false, true, targets, areas, issues.Count, progressMax, 0, progressCurrent, true);
                foreach (var area in target.Areas)
                {
                    if (area.IsSelected)
                    {
                        if (_worker.CancellationPending)
                            goto EXIT_SCAN;

                        progressCurrent++;
                        areas++;
                        RaiseProgessChanged(target, area, false, true, targets, areas, issues.Count, progressMax, 0, progressCurrent, true);
                        foreach (var issue in area.Fix())
                        {
                            if (_worker.CancellationPending)
                                goto EXIT_SCAN;

                            RaiseProgessChanged(target, area, false, true, targets, areas, issues.Count, progressMax, 0, progressCurrent, true);
                            issues.Add(issue);
                            Thread.Sleep(2);
                        }
                    }
                }
            }

        EXIT_SCAN:
            RaiseProgessChanged(null, null, false, true, targets, areas, issues.Count, progressMax, 0, progressCurrent, false);
            scan.Issues = issues;

            SaveScanDetails(scan);
            Subject.Instance.NotifyAllObservers(this, MessageCode.FixingStopped, issues.Count);

            return scan;
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _viewModel.ScanStatus = e.UserState as ScanStatusModel;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            var parameters = e.Argument as object[];
            var isScanOperation = (bool)parameters[0];
            var activeScan = parameters[1] as ScanModel;

            e.Result = isScanOperation ? Analyse(activeScan) : Fix(activeScan);
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _worker.DoWork -= new DoWorkEventHandler(Worker_DoWork);
            _worker.ProgressChanged -= new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.Dispose();
            _worker = null;

            _viewModel.Scan = e.Result as ScanModel;
            _viewModel.IsExecuting = IsExecuting = false;
        }

        ScanModel GetSavedScanDetails()
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

        void SaveScanDetails(ScanModel scan)
        {
            if (scan == null)
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

        void CancelScan()
        {
            if (_worker != null && _worker.IsBusy)
                _worker.CancelAsync();
        }
    }
}
