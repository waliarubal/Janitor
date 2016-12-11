using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Models;
using NullVoidCreations.Janitor.Shell.Commands;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class ComputerScanViewModel: ViewModelBase
    {
        volatile Scan _scan;
        CommandBase _smartScan, _customScan;

        public ComputerScanViewModel()
        {
            _smartScan = new SmartScanCommand(this);
        }

        #region properties

        public Scan ActiveScan
        {
            get { return _scan; }
            set
            {
                if (value == _scan)
                    return;

                _scan = value;
                RaisePropertyChanged("ActiveScan");
            }
        }

        #endregion

        #region commands

        public CommandBase SmartScan
        {
            get { return _smartScan; }
        }

        #endregion
    }
}
