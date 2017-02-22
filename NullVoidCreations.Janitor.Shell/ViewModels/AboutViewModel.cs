using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class AboutViewModel: ViewModelBase, ISignalObserver
    {
        string _operatingSyetem, _processor, _registeredTo, _serialKey;
        decimal _memory;
        bool _isTrial;
        List<ScanTargetBase> _targets;
        DateTime _expiryDate;

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
            get { return _targets; }
            private set
            {
                if (value == _targets)
                    return;

                _targets = value;
                RaisePropertyChanged("Targets");
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

        public string RegisteredTo
        {
            get { return _registeredTo; }
            private set
            {
                if (value == _registeredTo)
                    return;

                _registeredTo = value;
                RaisePropertyChanged("RegisteredTo");
            }
        }

        public string SerialKey
        {
            get { return _serialKey; }
            private set
            {
                if (value == _serialKey)
                    return;

                _serialKey = value;
                RaisePropertyChanged("SerialKey");
            }
        }

        public DateTime ExpiryDate
        {
            get { return _expiryDate; }
            private set
            {
                if (value == _expiryDate)
                    return;

                _expiryDate = value;
                RaisePropertyChanged("ExpiryDate");
            }
        }

        public bool IsTrial
        {
            get { return _isTrial; }
            private set
            {
                if (value == _isTrial)
                    return;

                _isTrial = value;
                RaisePropertyChanged("IsTrial");
            }
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
