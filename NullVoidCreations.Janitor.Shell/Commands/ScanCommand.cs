using System.ComponentModel;
using System.Threading;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
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
            switch(parameter as string)
            {
                case "Smart":
                    StartScan(ScanType.SmartScan);
                    break;

                case "Custom":
                    var scanParameters = new CustomScanView();
                    scanParameters.ShowDialog();

                    StartScan(ScanType.CustomScan);
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

        void StartScan(ScanType type)
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

            _viewModel.Scan = new ScanModel(type);
            if (type == ScanType.SmartScan)
            {
                // select all areas for smart scan
                foreach (var target in _viewModel.Scan.Targets)
                {
                    foreach (var area in target.Areas)
                        area.IsSelected = true;
                }
            }

            _worker.RunWorkerAsync(_viewModel.Scan);
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _viewModel.ScanStatus = e.UserState as ScanStatusModel;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            // TODO: allow scan cancellation
            if (_worker.CancellationPending)
                return;

            var activeScan = e.Argument as ScanModel;
            if (activeScan != null)
                activeScan.Analyse();

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
