using System.ComponentModel;
using System.Threading;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class SmartScanCommand: CommandBase, IObserver
    {
        ComputerScanViewModel _viewModel;
        BackgroundWorker _worker;

        #region constructor / destructor
       
        public SmartScanCommand(ComputerScanViewModel viewModel)
            : base(viewModel)
        {
            Title = "Smart Scan";
            Description = "Perform a full system scan looking all the scan targets for issues.";

            _viewModel = ViewModel as ComputerScanViewModel;

            Subject.Instance.AddObserver(this);
        }

        ~SmartScanCommand()
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
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            var activeScan = e.Argument as Scan;
            if (activeScan != null)
                activeScan.Analyse();

            e.Result = activeScan;
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _viewModel.Scan = e.Result as Scan;
            _viewModel.IsExecuting = IsExecuting = false;
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
    }
}
