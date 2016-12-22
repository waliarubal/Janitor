using System;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class ComputerScanViewModel: ViewModelBase, IObserver
    {
        volatile ScanModel _scan;
        volatile ScanStatusModel _progress;
        CommandBase _doScan, _activate;
        bool _isScannedInPast;
        DateTime _lastScanTime;
        string _lastScanName;

        public ComputerScanViewModel()
        {
            _progress = new ScanStatusModel(null, null, true, false, false);
            _doScan = new ScanCommand(this);
            _activate = new ActivateLicenseCommand(this);

            Subject.Instance.AddObserver(this);
        }

        ~ComputerScanViewModel()
        {
            Subject.Instance.RemoveObserver(this);
        }

        #region properties

        public ScanModel Scan
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

        public ScanStatusModel ScanStatus
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

        public bool IsScannedInPast
        {
            get { return _isScannedInPast; }
            private set
            {
                if (value == _isScannedInPast)
                    return;

                _isScannedInPast = value;
                RaisePropertyChanged("IsScannedInPast");
            }
        }

        public string LastScanName
        {
            get { return _lastScanName; }
            private set
            {
                if (value == _lastScanName)
                    return;

                _lastScanName = value;
                RaisePropertyChanged("LastScanName");
            }
        }

        public DateTime LastScanTime
        {
            get { return _lastScanTime; }
            private set
            {
                if (value == _lastScanTime)
                    return;

                _lastScanTime = value;
                RaisePropertyChanged("LastScanTime");
            }
        }

        #endregion

        #region commands

        public CommandBase DoScan
        {
            get { return _doScan; }
        }

        public CommandBase Activate
        {
            get { return _activate; }
        }

        #endregion

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            switch (code)
            {
                case MessageCode.Initialized:
                case MessageCode.AnalysisStopped:
                    IsScannedInPast = SettingsManager.Instance.LastScan != ScanType.Unknown;
                    LastScanName = SettingsManager.Instance.LastScan == ScanType.SmartScan ? "Smart Scan" : "Custom Scan";
                    LastScanTime = SettingsManager.Instance.LastScanTime;
                    break;

                case MessageCode.ScanTrigerred:
                    switch ((ScanType)data[0])
                    {
                        case ScanType.SmartScan:
                            DoScan.Execute("Smart");
                            break;

                        case ScanType.CustomScan:
                            DoScan.Execute("Custom");
                            break;
                    }
                    break;
            }
        }
    }
}
