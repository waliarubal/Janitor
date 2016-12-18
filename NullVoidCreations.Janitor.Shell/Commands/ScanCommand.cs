using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;
using NullVoidCreations.Janitor.Shell.ViewModels;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class ScanCommand: CommandBase, IObserver
    {
        ComputerScanViewModel _viewModel;
        BackgroundWorker _worker;

        #region constructor / destructor
       
        public ScanCommand(ComputerScanViewModel viewModel)
            : base(viewModel)
        {
            _viewModel = ViewModel as ComputerScanViewModel;
            IsRepeatedCallAllowed = true;

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
            switch(parameter as string)
            {
                case "Smart":
                    if (_viewModel.IsExecuting)
                        return;

                    _viewModel.Scan = new ScanModel(ScanType.SmartScan);
                    StartScan(_viewModel.Scan);
                    break;

                case "Custom":
                    if (_viewModel.IsExecuting)
                        return;

                    _viewModel.Scan = new ScanModel(ScanType.CustomScan);

                    var scanParameters = new CustomScanView(_viewModel.Scan.Targets);
                    var result = scanParameters.ShowDialog();
                    if (result.HasValue && result.Value == true)
                        StartScan(_viewModel.Scan);
                    break;

                case "Repeat":
                    break;

                case "Cancel":
                    CancelScan();
                    break;

                case "Fix":
                    // TODO: add cleaner starting code
                    break;
            }
        }

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            switch(code)
            {
                case MessageCode.ScanStatusChanged:
                    if (_worker.IsBusy)
                        _worker.ReportProgress(-1, data[0]);
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

            _worker.RunWorkerAsync(_viewModel.Scan);
        }

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
            var status = new ScanStatusModel(target, area, isRunning);
            status.TargetScanned = targetsScanned;
            status.AreaScanned = areasScanned;
            status.IssueCount = issueCount;
            status.ProgressMax = progressMax;
            status.ProgressMin = progressMin;
            status.ProgressCurrent = progressCurrent;
            Subject.Instance.NotifyAllObservers(this, MessageCode.ScanStatusChanged, status);
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _viewModel.ScanStatus = e.UserState as ScanStatusModel;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            var activeScan = e.Argument as ScanModel;

            Subject.Instance.NotifyAllObservers(this, MessageCode.ScanStarted, false);

            var issues = new List<Issue>();
            var targets = 0;
            var areas = 0;

            var progressCurrent = 0;
            var progressMax = 0;
            foreach (var target in activeScan.Targets)
                foreach (var area in target.Areas)
                    if (area.IsSelected)
                        progressMax++;

            if (_worker.CancellationPending)
                goto EXIT_SCAN;

            RaiseProgessChanged(null, null, true, targets, areas, issues.Count, progressMax, 0, progressCurrent);
            foreach (var target in activeScan.Targets)
            {
                if (_worker.CancellationPending)
                    goto EXIT_SCAN;

                targets++;
                RaiseProgessChanged(target, null, true, targets, areas, issues.Count, progressMax, 0, progressCurrent);
                foreach (var area in target.Areas)
                {
                    if (area.IsSelected)
                    {
                        if (_worker.CancellationPending)
                            goto EXIT_SCAN;

                        progressCurrent++;
                        areas++;
                        RaiseProgessChanged(target, area, true, targets, areas, issues.Count, progressMax, 0, progressCurrent);
                        Thread.Sleep(200);
                        foreach (var issue in area.Analyse())
                        {
                            if (_worker.CancellationPending)
                                goto EXIT_SCAN;

                            RaiseProgessChanged(target, area, true, targets, areas, issues.Count, progressMax, 0, progressCurrent);
                            issues.Add(issue);
                            Thread.Sleep(2);
                        }
                    }

                }
            }

            EXIT_SCAN:
            RaiseProgessChanged(null, null, false, targets, areas, issues.Count, progressMax, 0, progressCurrent);
            activeScan.Issues = issues;

            Subject.Instance.NotifyAllObservers(this, MessageCode.ScanStopped, issues.Count > 0);

            e.Result = activeScan;
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _viewModel.Scan = e.Result as ScanModel;
            _viewModel.IsExecuting = IsExecuting = false;
        }

        void CancelScan()
        {
            if (_worker != null && _worker.IsBusy)
                _worker.CancelAsync();
        }
    }
}
