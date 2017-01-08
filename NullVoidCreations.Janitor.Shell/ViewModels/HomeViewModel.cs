using System;
using System.Collections.Generic;
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
        readonly CommandBase _activate, _purchaseLicense, _doScan, _showPopup;
        string _computerName, _operatingSyetem, _processor, _model;
        decimal _memory;
        int _issueCount;
        bool _isLicensed, _isHavingIssues, _isHavingPluginUpdatesAvailable, _isHavingUpdatesAvailable;
        LicenseModel _license;
        readonly Queue<CommandBase> _startupActions;

        #region constructor / destructor

        public HomeViewModel()
        {
            SignalHost.Instance.AddObserver(this);

            _startupActions = new Queue<CommandBase>();
            _license = new LicenseModel();
            _computerName = _operatingSyetem = _processor = _model = "Analysing...";

            _showPopup = new BalloonCommand(this);
            _purchaseLicense = new PurchaseLicenseCommand(this);
            _doScan = new DelegateCommand(this, ExecuteDoScan);
            _activate = new ActivateLicenseCommand(this);
            _activate.IsEnabled = _doScan.IsEnabled = true;
        }

        ~HomeViewModel()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        public LicenseModel License
        {
            get { return _license; }
            private set
            {
                if (value == _license)
                    return;

                _license = value;
                RaisePropertyChanged("License");
            }
        }

        public bool IsLicensed
        {
            get { return _isLicensed; }
            private set
            {
                if (value == _isLicensed)
                    return;

                _isLicensed = value;
                RaisePropertyChanged("IsLicensed");
            }
        }

        public bool IsHavingIssues
        {
            get { return _isHavingIssues; }
            private set
            {
                if (value == _isHavingIssues)
                    return;

                _isHavingIssues = value;
                RaisePropertyChanged("IsHavingIssues");
            }
        }

        public int IssueCount
        {
            get { return _issueCount; }
            private set
            {
                if (value == _issueCount)
                    return;

                _issueCount = value;
                RaisePropertyChanged("IssueCount");
            }
        }

        public bool IsHavingPluginUpdatesAvailable
        {
            get { return _isHavingPluginUpdatesAvailable; }
            private set
            {
                if (value == _isHavingPluginUpdatesAvailable)
                    return;

                _isHavingPluginUpdatesAvailable = value;
                RaisePropertyChanged("IsHavingPluginUpdatesAvailable");
            }
        }

        public bool IsHavingUpdatesAvailable
        {
            get { return _isHavingUpdatesAvailable; }
            private set
            {
                if (value == _isHavingUpdatesAvailable)
                    return;

                _isHavingUpdatesAvailable = value;
                RaisePropertyChanged("IsHavingUpdatesAvailable");
            }
        }

        public string ComputerName
        {
            get { return _computerName; }
            private set
            {
                if (value == _computerName)
                    return;

                _computerName = value;
                RaisePropertyChanged("ComputerName");
            }
        }

        public string Model
        {
            get { return _model; }
            private set
            {
                if (value == _model)
                    return;

                _model = value;
                RaisePropertyChanged("Model");
            }
        }

        public string OperatingSystem
        {
            get { return _operatingSyetem; }
            private set
            {
                if (value == _operatingSyetem)
                    return;

                _operatingSyetem = value;
                RaisePropertyChanged("OperatingSystem");
            }
        }

        public decimal Memory
        {
            get { return _memory; }
            private set
            {
                if (value == _memory)
                    return;

                _memory = value;
                RaisePropertyChanged("Memory");
            }
        }

        public string Processor
        {
            get { return _processor; }
            private set
            {
                if (value == _processor)
                    return;

                _processor = value;
                RaisePropertyChanged("Processor");
            }
        }

        #endregion

        #region commands

        public CommandBase Activate
        {
            get { return _activate; }
        }

        public CommandBase PurchaseLicense
        {
            get { return _purchaseLicense; }
        }

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
            
            SignalHost.Instance.RaiseSignal(this, Signal.ScanTrigerred, type);
        }

        void WeHaveProblems()
        {
            byte problems = 0;
            if (!IsLicensed)
                problems++;
            if (IsHavingIssues)
                problems++;
            if (IsHavingPluginUpdatesAvailable)
                problems++;
            if (IsHavingUpdatesAvailable)
                problems++;

            SignalHost.Instance.RaiseSignal(this, Signal.ProblemsAppeared, problems);
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Initialized:
                    SignalReceived(sender, Signal.SystemInformationLoaded);
                    SignalReceived(sender, Signal.LicenseChanged);
                    break;

                case Signal.SystemInformationLoaded:
                    ComputerName = SysInformation.Instance[SysInformation.ManagementClassNames.ComputerSystem, "Name"] as string;
                    Model = SysInformation.Instance[SysInformation.ManagementClassNames.ComputerSystem, "Model"] as string;
                    Memory = Convert.ToDecimal(SysInformation.Instance[SysInformation.ManagementClassNames.ComputerSystem, "TotalPhysicalMemory"]) / 1024 / 1024 / 1024;
                    OperatingSystem = string.Format("{0} ({1})", SysInformation.Instance[SysInformation.ManagementClassNames.OperatingSystem, "Caption"], SysInformation.Instance[SysInformation.ManagementClassNames.OperatingSystem, "OSArchitecture"]);
                    Processor = SysInformation.Instance[SysInformation.ManagementClassNames.Processor, "Name"] as string;
                    break;

                case Signal.LicenseChanged:
                    License = LicenseExManager.Instance.License;
                    IsLicensed = License != null && !License.IsTrial;
                    WeHaveProblems();
                    break;

                case Signal.FixingStarted:
                case Signal.AnalysisStarted:
                    IssueCount = 0;
                    IsHavingIssues = false;
                    WeHaveProblems();
                    break;

                case Signal.AnalysisStopped:
                    IssueCount = (int)data[0];
                    IsHavingIssues = IssueCount > 0;
                    WeHaveProblems();
                    break;

                case Signal.FixingStopped:
                    IssueCount = (int)data[0];
                    IsHavingIssues = false;
                    WeHaveProblems();
                    break;

                case Signal.UpdateStarted:
                    switch ((UpdateCommand.UpdateType)data[0])
                    {
                        case UpdateCommand.UpdateType.Plugin:
                            IsHavingPluginUpdatesAvailable = true;
                            break;

                        case UpdateCommand.UpdateType.Program:
                            IsHavingUpdatesAvailable = true;
                            break;
                    }
                    WeHaveProblems();
                    break;

                case Signal.UpdateStopped:
                    var wasUpdateSuccessful = (bool)data[1];
                    switch ((UpdateCommand.UpdateType)data[0])
                    {
                        case UpdateCommand.UpdateType.Plugin:
                            IsHavingPluginUpdatesAvailable = !wasUpdateSuccessful;
                            break;

                        case UpdateCommand.UpdateType.Program:
                            IsHavingUpdatesAvailable = !wasUpdateSuccessful;
                            break;
                    }
                    WeHaveProblems();
                    break;
            }
        }
    }
}
