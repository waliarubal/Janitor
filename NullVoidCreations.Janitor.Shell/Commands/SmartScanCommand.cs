using System.ComponentModel;
using System.Threading;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Models;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class SmartScanCommand: CommandBase
    {
        ComputerScanViewModel _viewModel;
        BackgroundWorker _worker;

        public SmartScanCommand(ComputerScanViewModel viewModel)
            : base(viewModel)
        {
            Title = "Smart Scan";
            Description = "Perform a full system scan looking all the scan targets for issues.";

            _viewModel = ViewModel as ComputerScanViewModel;
        }

        ~SmartScanCommand()
        {
            if (_worker == null)
                return;

            _worker.DoWork -= new DoWorkEventHandler(Worker_DoWork);
            _worker.ProgressChanged -= new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.Dispose();
        }

        public override void Execute(object parameter)
        {
            if (_viewModel.IsExecuting)
                return;

            _viewModel.IsExecuting = IsExecuting = true;

            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);

            _viewModel.Scan = new Scan(ScanType.SmartScan);
            foreach (var target in _viewModel.Scan.Targets)
            {
                target.IsSelected = true;
                foreach (var area in target.Areas)
                    area.IsSelected = true;
            }

            _worker.RunWorkerAsync(_viewModel.Scan);
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _viewModel.ScanStatus = e.UserState as ScanStatus;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            var activeScan = e.Argument as Scan;
            if (activeScan != null)
            {
                activeScan.OnScanProgress += new Scan.ScanProgress(ActiveScan_OnScanProgress);
                activeScan.Analyse();
                activeScan.OnScanProgress -= new Scan.ScanProgress(ActiveScan_OnScanProgress);
            }

            e.Result = activeScan;
        }

        void ActiveScan_OnScanProgress(ScanStatus e)
        {
            _worker.ReportProgress(-1, e);
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _viewModel.Scan = e.Result as Scan;
            _viewModel.IsExecuting = IsExecuting = false;
        }
    }
}
