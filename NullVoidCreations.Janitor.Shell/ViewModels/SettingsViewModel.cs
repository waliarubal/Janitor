using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
        #region properties

        public bool RunAtBoot
        {
            get { return SettingsManager.Instance.RunAtBoot; }
            set
            {
                if (value == SettingsManager.Instance.RunAtBoot)
                    return;

                SettingsManager.Instance.RunAtBoot = value;
                RaisePropertyChanged("RunAtBoot");
            }
        }

        public bool RunScanAtLaunch
        {
            get { return SettingsManager.Instance.RunScanAtLaunch; }
            set
            {
                if (value == SettingsManager.Instance.RunScanAtLaunch)
                    return;

                SettingsManager.Instance.RunScanAtLaunch = value;
                RaisePropertyChanged("RunScanAtLaunch");
            }
        }

        public bool RunPluginUpdateAtLaunch
        {
            get { return SettingsManager.Instance.RunPluginUpdateAtLaunch; }
            set
            {
                if (value == SettingsManager.Instance.RunPluginUpdateAtLaunch)
                    return;

                SettingsManager.Instance.RunPluginUpdateAtLaunch = value;
                RaisePropertyChanged("RunPluginUpdateAtLaunch");
            }
        }

        public bool RunProgramUpdateAtLaunch
        {
            get { return SettingsManager.Instance.RunProgramUpdateAtLaunch; }
            set
            {
                if (value == SettingsManager.Instance.RunProgramUpdateAtLaunch)
                    return;

                SettingsManager.Instance.RunProgramUpdateAtLaunch = value;
                RaisePropertyChanged("RunProgramUpdateAtLaunch");
            }
        }

        public bool SkipUac
        {
            get { return SettingsManager.Instance.SkipUac; }
            set
            {
                if (value == SettingsManager.Instance.SkipUac)
                    return;

                SettingsManager.Instance.SkipUac = value;
                RaisePropertyChanged("SkipUac");
            }
        }

        public bool ExitOnClose
        {
            get { return SettingsManager.Instance.ExitOnClose; }
            set
            {
                if (value == SettingsManager.Instance.ExitOnClose)
                    return;

                SettingsManager.Instance.ExitOnClose = value;
                RaisePropertyChanged("ExitOnClose");
            }
        }

        public bool CloseAfterFixing
        {
            get { return SettingsManager.Instance.CloseAfterFixing; }
            set
            {
                if (value == SettingsManager.Instance.CloseAfterFixing)
                    return;

                SettingsManager.Instance.CloseAfterFixing = value;
                RaisePropertyChanged("CloseAfterFixing");
            }
        }

        public bool ShutdownAfterFixing
        {
            get { return SettingsManager.Instance.ShutdownAfterFixing; }
            set
            {
                if (value == SettingsManager.Instance.ShutdownAfterFixing)
                    return;

                SettingsManager.Instance.ShutdownAfterFixing = value;
                RaisePropertyChanged("ShutdownAfterFixing");
            }
        }

        #endregion
    }
}
