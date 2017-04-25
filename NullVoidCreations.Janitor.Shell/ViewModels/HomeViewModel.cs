using System.Collections.ObjectModel;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class HomeViewModel: ViewModelBase, ISignalObserver
    {
        readonly CommandBase _doScan;

        #region constructor / destructor

        public HomeViewModel()
        {
            SignalHost.Instance.AddObserver(this);

            Problems = new ObservableCollection<ProblemModel>();
            ComputerName = OperatingSystem = Processor = Model = "Analysing...";
           
            _doScan = new DelegateCommand(this, ExecuteDoScan);
            _doScan.IsEnabled = true;
        }

        ~HomeViewModel()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        public ObservableCollection<ProblemModel> Problems
        {
            get { return GetValue<ObservableCollection<ProblemModel>>("Problems"); }
            private set { this["Problems"] = value; }
        }

        public string BuyNowUrl
        {
            get { return TemporarySettingsManager.Instance["BuyNowUrl"] as string; }
        }

        public LicenseModel License
        {
            get { return GetValue<LicenseModel>("License"); }
            private set { this["License"] = value; }
        }

        public bool IsLicensed
        {
            get { return GetValue<bool>("IsLicensed"); }
            private set { this["IsLicensed"] = value; }
        }

        public string ComputerName
        {
            get { return GetValue<string>("ComputerName"); }
            private set { this["ComputerName"] = value; }
        }

        public string Model
        {
            get { return GetValue<string>("Model"); }
            private set { this["Model"] = value; }
        }

        public string OperatingSystem
        {
            get { return GetValue<string>("OperatingSystem"); }
            private set { this["OperatingSystem"] = value; }
        }

        public decimal Memory
        {
            get { return GetValue<decimal>("Memory"); }
            private set { this["Memory"] = value; }
        }

        public string Processor
        {
            get { return GetValue<string>("Processor"); }
            private set { this["Processor"] = value; }
        }

        #endregion

        #region commands

        public CommandBase DoScan
        {
            get { return _doScan; }
        }

        #endregion

        void ExecuteDoScan(object scanType)
        {
            var type = ScanType.Unknown;
            if ("Smart".Equals(scanType))
                type = ScanType.SmartScan;
            else if ("Custom".Equals(scanType))
                type = ScanType.CustomScan;
            
            SignalHost.Instance.RaiseSignal(Signal.ScanTrigerred, type);
        }

        void WeHaveProblem(ProblemType type, bool isResolved = false)
        {
            ProblemModel problem = null;
            if (isResolved)
            {
                for (var index = 0; index < Problems.Count; index++)
                {
                    problem = Problems[index];
                    if (problem.Type == type)
                    {
                        Problems.RemoveAt(index);
                        break;
                    }
                }
                problem = null;
            }
            else
            {
                var found = false;
                for (var index = 0; index < Problems.Count; index++)
                {
                    problem = Problems[index];
                    if (problem.Type == type)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    problem = new ProblemModel(type);
                    Problems.Add(problem);
                }
            }

            SignalHost.Instance.RaiseSignal(Signal.ProblemAppeared, Problems.Count, problem);
        }

        public void SignalReceived(Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Initialized:
                    SignalReceived(Signal.SystemInformationLoaded);
                    SignalReceived(Signal.LicenseChanged);
                    break;

                case Signal.SystemInformationLoaded:
                    ComputerName = SysInformation.Instance.ComputerName;
                    Model = SysInformation.Instance.Model;
                    Memory = SysInformation.Instance.Memory;
                    OperatingSystem = string.Format("{0} ({1})", SysInformation.Instance.OperatingSystem, SysInformation.Instance.OperatingSystemArchitecture);
                    Processor = SysInformation.Instance.Processor;
                    break;

                case Signal.LicenseChanged:
                    License = LicenseManager.Instance.License;
                    IsLicensed = License != null && !License.IsTrial;
                    WeHaveProblem(ProblemType.IsUnlicensed, IsLicensed);
                    break;

                case Signal.FixingStarted:
                case Signal.AnalysisStarted:
                    WeHaveProblem(ProblemType.IsHavingIssues, true);
                    break;

                case Signal.AnalysisStopped:
                    WeHaveProblem(ProblemType.IsHavingIssues, (int)data[0] == 0);
                    break;

                case Signal.FixingStopped:
                    WeHaveProblem(ProblemType.IsHavingIssues, true);
                    break;

                case Signal.UpdateStopped:
                    var wasUpdateSuccessful = (bool)data[1];
                    switch ((UpdateCommand.UpdateType)data[0])
                    {
                        case UpdateCommand.UpdateType.Plugin:
                            WeHaveProblem(ProblemType.IsHavingPluginUpdatesAvailable, wasUpdateSuccessful);
                            break;

                        case UpdateCommand.UpdateType.Program:
                            WeHaveProblem(ProblemType.IsHavingUpdatesAvailable, wasUpdateSuccessful);
                            break;
                    }
                    break;
            }
        }
    }
}
