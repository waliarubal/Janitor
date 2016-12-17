using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shared.Helpers;
using System;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class AboutViewModel: ViewModelBase, IObserver
    {
        string _operatingSyetem, _processor;
        decimal _memory;
        List<ScanTargetBase> _targets;

        #region constructor / destructor

        public AboutViewModel()
        {
            Subject.Instance.AddObserver(this);
        }

        ~AboutViewModel()
        {
            Subject.Instance.RemoveObserver(this);
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

        #endregion

        #region commands

        

        #endregion

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            switch(code)
            {
                case MessageCode.SystemInformationLoaded:
                    Memory = Convert.ToDecimal(SysInformation.Instance[SysInformation.ManagementClassNames.ComputerSystem, "TotalPhysicalMemory"]) / 1024 / 1024 / 1024;
                    OperatingSystem = string.Format("{0} ({1})", SysInformation.Instance[SysInformation.ManagementClassNames.OperatingSystem, "Caption"], SysInformation.Instance[SysInformation.ManagementClassNames.OperatingSystem, "OSArchitecture"]);
                    Processor = SysInformation.Instance[SysInformation.ManagementClassNames.Processor, "Name"] as string;
                    break;

                case MessageCode.PluginsLoaded:
                    var plugins = new List<ScanTargetBase>();
                    foreach (var plugin in data[0] as IEnumerable<ScanTargetBase>)
                        plugins.Add(plugin);

                    Targets = plugins;
                    break;

                case MessageCode.PluginsUnloaded:
                    Targets = null;
                    break;
            }
        }
    }
}
