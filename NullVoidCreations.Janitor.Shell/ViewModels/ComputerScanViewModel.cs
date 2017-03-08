using System;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class ComputerScanViewModel: ViewModelBase, ISignalObserver
    {
        CommandBase _doScan;

        public ComputerScanViewModel()
        {
            ScanStatus = new ScanStatusModel(null, null, true, false, false);
            _doScan = new ScanCommand(this);

            SignalHost.Instance.AddObserver(this);
            GetLastScan();
        }

        ~ComputerScanViewModel()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #region properties

        public ScanModel Scan
        {
            get { return GetValue<ScanModel>("Scan"); }
            set { this["Scan"] = value; }
        }

        public ScanStatusModel ScanStatus
        {
            get { return GetValue<ScanStatusModel>("ScanStatus"); }
            set { this["ScanStatus"] = value; }
        }

        public bool IsScannedInPast
        {
            get { return GetValue<bool>("IsScannedInPast"); }
            private set { this["IsScannedInPast"] = value; }
        }

        public string LastScanName
        {
            get { return GetValue<string>("LastScanName"); }
            private set { this["LastScanName"] = value; }
        }

        public DateTime LastScanTime
        {
            get { return GetValue<DateTime>("LastScanTime"); }
            private set { this["LastScanTime"] = value; }
        }

        #endregion

        #region commands

        public CommandBase DoScan
        {
            get { return _doScan; }
        }

        #endregion

        void GetLastScan()
        {
            IsScannedInPast = SettingsManager.Instance.LastScan != ScanType.Unknown;
            if (IsScannedInPast)
            {
                LastScanName = SettingsManager.Instance.LastScan == ScanType.SmartScan ? "Smart Scan" : "Custom Scan";
                LastScanTime = SettingsManager.Instance.LastScanTime;
            }
        }

        public void SignalReceived(Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Initialized:
                case Signal.AnalysisStopped:
                    GetLastScan();
                    break;

                case Signal.ScanTrigerred:
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
