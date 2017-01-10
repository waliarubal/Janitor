using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class UpdateCommand : DelegateCommand, ISignalObserver
    {
        internal static readonly string PluginsUpdateFile, ApplicationUpdateFile;

        static UpdateCommand()
        {
            ApplicationUpdateFile = Path.Combine(KnownPaths.Instance.ApplicationTempTirectory, "Setup.exe");
            PluginsUpdateFile = Path.Combine(KnownPaths.Instance.ApplicationTempTirectory, "Update.zip");
        }

        public enum UpdateType : byte
        {
            Plugin,
            Program
        }

        const string CencelMessage = "Update cancelled by user.";
        const string DownloadErrorMessage = "An error occured while downloading update. Please try again later.";
        const string UpdateErrorMessage = "An error occured while installing the update.";
        const string UpdatingMessage = "Updating from version {0} to version {1}.";
        const string UpToDateMessage = "Installed version {0} is up to date. Last checked for update on {1}.";
        const string RestartRequiredMessage = "Update has been downloaded. Please restart program to apply update.";
        const char Separator = '|';
        readonly Uri MetadataUrl = new Uri(@"https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/Update.txt");

        readonly UpdateType _type;
        bool _isSilent;
        volatile int _progress;
        Uri _updateUrl;
        WebClient _client;

        #region constructor/destructor

        public UpdateCommand(ViewModelBase viewModel, UpdateType type)
            : base(viewModel)
        {
            SignalHost.Instance.AddObserver(this);

            Title = "Check for Updates";
            _type = type;
            if (_type == UpdateType.Program)
                Description = string.Format(UpToDateMessage, App.Current.Resources["ProductVersion"], SettingsManager.Instance.LastProgramUpdateCheck.ToString("MM/dd/yyyy HH:mm:ss"));
            else
                Description = string.Format(UpToDateMessage, string.Empty, SettingsManager.Instance.LastPluginUpdateCheck.ToString("MM/dd/yyyy HH:mm:ss"));

            UpdateUrl = MetadataUrl;
            IsEnabled = true;
            IsRecallAllowed = true;

            _client = new WebClient();
            _client.Proxy = null;
            _client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            _client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
        }

        ~UpdateCommand()
        {
            _client.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            _client.DownloadFileCompleted -= new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
            _client.Dispose();

            SignalHost.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        public int Progress
        {
            get { return _progress; }
            private set
            {
                if (value == _progress)
                    return;

                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        public Uri UpdateUrl
        {
            get { return _updateUrl; }
            private set
            {
                if (value == _updateUrl)
                    return;

                _updateUrl = value;
                RaisePropertyChanged("UpdateUrl");
            }
        }

        Version AvailableVersion
        {
            get;
            set;

        }

        #endregion

        protected override void ExecuteOverride(object parameter)
        {
            SignalHost.Instance.RaiseSignal(this, Signal.UpdateStarted, _type);
            Progress = 0;
            Title = "Updating...";
            if (_type == UpdateType.Plugin)
                SettingsManager.Instance.LastPluginUpdateCheck = DateTime.Now;
            else
                SettingsManager.Instance.LastProgramUpdateCheck = DateTime.Now;

            string[] metaData;

            // fetch metadata
            try
            {
                metaData = _client.DownloadString(MetadataUrl).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                Description = DownloadErrorMessage;
                SignalHost.Instance.RaiseSignal(this, Signal.UpdateStopped, _type, true);
                SignalHost.Instance.RaiseSignal(this, Signal.StopWork);
                return;
            }

            if (metaData.Length != 2)
            {
                Description = DownloadErrorMessage;
                SignalHost.Instance.RaiseSignal(this, Signal.UpdateStopped, _type, true);
                SignalHost.Instance.RaiseSignal(this, Signal.StopWork);
                return;
            }

            // validate meta data
            metaData = _type == UpdateType.Program ?
                metaData[1].Split(new char[] { Separator }, StringSplitOptions.RemoveEmptyEntries) :
                metaData[0].Split(new char[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            if (metaData.Length != 3)
            {
                Description = DownloadErrorMessage;
                return;
            }

            // validate version
            AvailableVersion = new Version(metaData[1]);
            var currentVersion = _type == UpdateType.Program ?
                new Version(App.Current.Resources["ProductVersion"] as string) :
                PluginManager.Instance.Version;
            if (AvailableVersion <= currentVersion)
            {
                Title = "Check for Updates";
                Description = string.Format(UpToDateMessage, currentVersion,
                    _type == UpdateType.Plugin ?
                    SettingsManager.Instance.LastPluginUpdateCheck.ToString("MM/dd/yyyy HH:mm:ss") :
                    SettingsManager.Instance.LastProgramUpdateCheck.ToString("MM/dd/yyyy HH:mm:ss"));
                SignalHost.Instance.RaiseSignal(this, Signal.UpdateStopped, _type, true);
                SignalHost.Instance.RaiseSignal(this, Signal.StopWork);
                return;
            }

            if (!Directory.Exists(KnownPaths.Instance.ApplicationTempTirectory))
                Directory.CreateDirectory(KnownPaths.Instance.ApplicationTempTirectory);

            // download update
            Description = string.Format(UpdatingMessage, currentVersion, AvailableVersion);
            UpdateUrl = new Uri(metaData[2]);
            var updateFile = _type == UpdateType.Program ? ApplicationUpdateFile : PluginsUpdateFile;
            FileSystemHelper.Instance.DeleteFile(updateFile);
            try
            {
                _client.DownloadFileAsync(UpdateUrl, updateFile, updateFile);
            }
            catch
            {
                Description = DownloadErrorMessage;
                SignalHost.Instance.RaiseSignal(this, Signal.UpdateStopped, _type, false);
                SignalHost.Instance.RaiseSignal(this, Signal.StopWork);
                return;
            }

            return;
        }

        void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var updateFile = e.UserState as string;
            IsExecuting = false;
            if (ViewModel != null)
                ViewModel.IsExecuting = IsExecuting;

            // update failed
            if (string.IsNullOrEmpty(updateFile) || !File.Exists(updateFile))
            {
                SignalHost.Instance.RaiseSignal(this, Signal.UpdateStopped, _type, false);
                SignalHost.Instance.RaiseSignal(this, Signal.StopWork);
                return;
            }

            // intall update
            var isUpdateInstalled = true;
            var message = string.Empty;
            if (_type == UpdateType.Program)
            {
                message = RestartRequiredMessage;
                if (_isSilent || UiHelper.Instance.Question(App.ProductName, RestartRequiredMessage))
                {
                    var startInfo = new ProcessStartInfo(updateFile);
                    startInfo.UseShellExecute = true;
                    startInfo.WorkingDirectory = KnownPaths.Instance.ApplicationTempTirectory;
                    Process.Start(startInfo);

                    App.Current.Shutdown(0);
                }
            }
            else
            {
                message = RestartRequiredMessage;
                if (_isSilent || UiHelper.Instance.Question(App.ProductName, RestartRequiredMessage))
                {
                    PluginManager.Instance.Version = AvailableVersion;
                    SignalHost.Instance.RaiseSignal(this, Signal.CloseAndStart);
                }
            }

            Description = message;
            Title = "Check for Updates";
            _isSilent = false;
            SignalHost.Instance.RaiseSignal(this, Signal.UpdateStopped, _type, isUpdateInstalled);
            SignalHost.Instance.RaiseSignal(this, Signal.StopWork);
        }

        void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // set execution status as its disabled by command's base
            if (!IsExecuting)
            {
                IsExecuting = true;
                if (ViewModel != null)
                    ViewModel.IsExecuting = IsExecuting;
            }

            Progress = e.ProgressPercentage;
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.Initialized:
                    if (_type == UpdateType.Plugin)
                        Description = string.Format(UpToDateMessage, PluginManager.Instance.Version,
                    _type == UpdateType.Plugin ?
                    SettingsManager.Instance.LastPluginUpdateCheck.ToString("MM/dd/yyyy HH:mm:ss") :
                    SettingsManager.Instance.LastProgramUpdateCheck.ToString("MM/dd/yyyy HH:mm:ss"));
                    break;

                case Signal.UpdateTriggered:
                    if (IsExecuting)
                        break;

                    if (_type == (UpdateType)data[0])
                    {
                        _isSilent = true;
                        Execute(null);
                    }
                    break;
            }
        }
    }
}
