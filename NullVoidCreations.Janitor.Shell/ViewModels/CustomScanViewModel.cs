using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class CustomScanViewModel: ViewModelBase
    {
        readonly CommandBase _scan;

        public CustomScanViewModel()
        {
            Targets = new ObservableCollection<ScanTargetBase>();
            _scan = new DelegateCommand(this, ExecuteScan);
            _scan.IsEnabled = true;
        }

        #region properties

        public ObservableCollection<ScanTargetBase> Targets
        {
            get { return GetValue<ObservableCollection<ScanTargetBase>>("Targets"); }
            set { this["Targets"] = value; }
        }

        public string ErrorMessage
        {
            get { return GetValue<string>("ErrorMessage"); }
            private set { this["ErrorMessage"] = value; }
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

            if (!AreScanAreasSelected())
            {
                ErrorMessage = "No scan area selected.";
                return;
            }

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

        bool AreScanAreasSelected()
        {
            foreach (var target in Targets)
                foreach (var area in target.Areas)
                    if (area.IsSelected)
                        return true;

            return false;
        }
    }
}
