using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class ComputerScanViewModel: ViewModelBase
    {
        volatile Scan _scan;
        volatile ScanStatus _progress;
        CommandBase _smartScan, _customScan;

        public ComputerScanViewModel()
        {
            _progress = new ScanStatus(null, null, false);
            _smartScan = new SmartScanCommand(this);
        }

        #region properties

        public Scan Scan
        {
            get { return _scan; }
            set
            {
                if (value == _scan)
                    return;

                _scan = value;
                RaisePropertyChanged("Scan");
            }
        }

        public ScanStatus ScanStatus
        {
            get { return _progress; }
            set
            {
                if (value == _progress)
                    return;

                _progress = value;
                RaisePropertyChanged("ScanStatus");
            }
        }

        #endregion

        #region commands

        public CommandBase SmartScan
        {
            get { return _smartScan; }
        }

        public CommandBase CustomScan
        {
            get { return _smartScan; }
        }

        #endregion
    }
}
