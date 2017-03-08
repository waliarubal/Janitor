using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class AboutViewModel: ViewModelBase, ISignalObserver
    {
        #region constructor / destructor

        public AboutViewModel()
        {
            SignalHost.Instance.AddObserver(this);
        }

        ~AboutViewModel()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        public List<ScanTargetBase> Targets
        {
            get { return GetValue<List<ScanTargetBase>>("Targets"); }
            private set { this["Targets"] = value; }
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

        public string RegisteredTo
        {
            get { return GetValue<string>("RegisteredTo"); }
            private set { this["RegisteredTo"] = value; }
        }

        public string SerialKey
        {
            get { return GetValue<string>("SerialKey"); }
            private set { this["SerialKey"] = value; }
        }

        public DateTime ExpiryDate
        {
            get { return GetValue<DateTime>("ExpiryDate"); }
            private set { this["ExpiryDate"] = value; }
        }

        public bool IsTrial
        {
            get { return GetValue<bool>("IsTrial"); }
            private set { this["IsTrial"] = value; }
        }

        #endregion

        #region commands

        

        #endregion

        public void SignalReceived(Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Initialized:
                    SignalReceived(Signal.PluginsLoaded, PluginManager.Instance.Targets);
                    SignalReceived(Signal.SystemInformationLoaded, data);
                    SignalReceived(Signal.LicenseChanged, data);
                    break;

                case Signal.SystemInformationLoaded:
                    Memory = SysInformation.Instance.Memory;
                    OperatingSystem = string.Format("{0} ({1})", SysInformation.Instance.OperatingSystem, SysInformation.Instance.OperatingSystemArchitecture);
                    Processor = SysInformation.Instance.Processor;
                    break;

                case Signal.PluginsLoaded:
                    var plugins = new List<ScanTargetBase>();
                    foreach (var plugin in data[0] as IEnumerable<ScanTargetBase>)
                        plugins.Add(plugin);

                    Targets = plugins;
                    break;

                case Signal.PluginsUnloaded:
                    Targets = null;
                    break;

                case Signal.LicenseChanged:
                    RegisteredTo = LicenseManager.Instance.License.Name;
                    ExpiryDate = LicenseManager.Instance.License.ExpirationDate;
                    SerialKey = LicenseManager.Instance.License.SerialKey;
                    IsTrial = LicenseManager.Instance.License.IsTrial;
                    break;
            }
        }
    }
}
