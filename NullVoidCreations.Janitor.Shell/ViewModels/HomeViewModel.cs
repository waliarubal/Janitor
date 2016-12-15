using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using System;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class HomeViewModel: ViewModelBase
    {
        readonly CommandBase _getSystemInformation;
        string _computerName, _operatingSyetem, _processor, _model;
        decimal _memory;

        #region constructor / destructor

        public HomeViewModel()
        {
            _computerName = _operatingSyetem = _processor = _model = "Analysing...";
            _getSystemInformation = new AsyncDelegateCommand(this, null, ExecuteGetSystemInformation, SystemInformationReceived);
        }

        #endregion

        #region properties

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

        public CommandBase GetSystemInformation
        {
            get { return _getSystemInformation; }
        }

        #endregion

        object ExecuteGetSystemInformation(object parameter)
        {
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.ComputerSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.OperatingSystem);
            SysInformation.Instance.Fill(SysInformation.ManagementClassNames.Processor);
            
            var sysInfo = new object[] 
            {
                SysInformation.Instance[SysInformation.ManagementClassNames.ComputerSystem, "Name"],
                SysInformation.Instance[SysInformation.ManagementClassNames.ComputerSystem, "Model"],
                SysInformation.Instance[SysInformation.ManagementClassNames.ComputerSystem, "TotalPhysicalMemory"],
                SysInformation.Instance[SysInformation.ManagementClassNames.OperatingSystem, "Caption"],
                SysInformation.Instance[SysInformation.ManagementClassNames.OperatingSystem, "OSArchitecture"],
                SysInformation.Instance[SysInformation.ManagementClassNames.Processor, "Name"]
            };
            return sysInfo;
        }

        void SystemInformationReceived(object result)
        {
            var sysInfo = result as object[];

            ComputerName = sysInfo[0] as string;
            Model = sysInfo[1] as string;
            Memory = Convert.ToDecimal(sysInfo[2]) / 1024 / 1024 / 1024;
            OperatingSystem = string.Format("{0} ({1})", sysInfo[3], sysInfo[4]);
            Processor = sysInfo[5] as string;
        }
    }
}
