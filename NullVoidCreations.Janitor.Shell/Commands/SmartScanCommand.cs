using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.ViewModels;
using System.ComponentModel;
using System.Threading;
using NullVoidCreations.Janitor.Shell.Models;
using NullVoidCreations.Janitor.Shared.Models;

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

            _viewModel.ActiveScan = new Scan();
            _viewModel.ActiveScan.Type = ScanType.SmartScan;
            _worker.RunWorkerAsync(_viewModel.ActiveScan);
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _viewModel.ActiveScan.Progress = e.UserState as ScanProgress;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            var scan = e.Argument as Scan;
            scan.Initialize();

            var progress = new ScanProgress();
            progress.IssueCount = 0;
            progress.ProgressMin = 0;
            progress.ProgressMax = 0;
            foreach (var target in scan.Targets)
                progress.ProgressMax += target.Areas.Count;

            _worker.ReportProgress(-1, progress);
            foreach (var target in scan.Targets)
            {
                progress.ActiveTarget = target;
                _worker.ReportProgress(-1, progress);
                foreach (var area in target.Areas)
                {
                    progress.ActiveArea = area;
                    progress.Progress += 1;
                    progress.IssueCount += area.ScanForIssues();
                    _worker.ReportProgress(-1, progress);
                }
            }
            progress.ActiveTarget = null;
            progress.ActiveArea = null;
            _worker.ReportProgress(-1, progress);
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _viewModel.IsExecuting = IsExecuting = false;
        }
    }
}
