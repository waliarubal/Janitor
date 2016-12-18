using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Controls;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class CustomScanViewModel: ViewModelBase
    {
        readonly CommandBase _scan;
        ObservableCollection<ScanTargetBase> _targets;

        public CustomScanViewModel()
        {
            _targets = new ObservableCollection<ScanTargetBase>();
            _scan = new DelegateCommand(this, ExecuteScan);
            _scan.IsEnabled = true;
        }

        #region properties

        public ObservableCollection<ScanTargetBase> Targets
        {
            get { return _targets; }
            set
            {
                if (value == _targets)
                    return;

                _targets = value;
                RaisePropertyChanged("Targets");
            }
        }

        #endregion

        #region commands

        public CommandBase Scan
        {
            get { return _scan; }
        }

        #endregion

        void ExecuteScan(object parameter)
        {
            var window = parameter as CustomWindow;
            if (window == null)
                return;

            // remove unselected scan targets
            for (var index = Targets.Count - 1; index >= 0; index--)
            {
                var excludeTarget = true;
                foreach (var area in Targets[index].Areas)
                {
                    if (area.IsSelected)
                    {
                        excludeTarget = false;
                        break;
                    }
                }

                if (excludeTarget)
                    Targets.RemoveAt(index);
            }

                window.DialogResult = true;
            window.Close();
        }
    }
}
