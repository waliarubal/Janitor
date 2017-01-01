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
        public enum UpdateType : byte
        {
            Plugin,
            Program
        }

        const string CencelMessage = "Update cancelled by user.";
        const string DownloadErrorMessage = "An error occured while downloading update. Please try again later.";
        const string UpdateErrorMessage = "An error occured while installing the update.";
        const string UpdatingMessage = "Updating from version {0} to version {1}.";
        const string UpToDateMessage = "Installed version {0} is up to date.";
        const char Separator = '|';
        readonly Uri  MetadataUrl = new Uri(@"https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/Update.txt");

        readonly UpdateType _type;
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
                Description = string.Format(UpToDateMessage, App.Current.Resources["ProductVersion"]);
            else
                Description = string.Format(UpToDateMessage, string.Empty);

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

        #endregion

        protected override void ExecuteOverride(object parameter)
        {
            SignalHost.Instance.NotifyAllObservers(this, Signal.UpdateStarted, _type);
            Progress = 0;
            Title = "Updating...";

            string[] metaData;

            // fetch metadata
            try
            {
                metaData = _client.DownloadString(MetadataUrl).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                Description = DownloadErrorMessage;
                SignalHost.Instance.NotifyAllObservers(this, Signal.UpdateStopped, _type, true);
                return;
            }

            if (metaData.Length != 2)
            {
                Description = DownloadErrorMessage;
                SignalHost.Instance.NotifyAllObservers(this, Signal.UpdateStopped, _type, true);
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
            var availableVersion = new Version(metaData[1]);
            var currentVersion = _type == UpdateType.Program ?
                new Version(App.Current.Resources["ProductVersion"] as string) : 
                PluginManager.Instance.Version;
            if (availableVersion <= currentVersion)
            {
                Title = "Check for Updates";
                Description = string.Format(UpToDateMessage, currentVersion);
                SignalHost.Instance.NotifyAllObservers(this, Signal.UpdateStopped, _type, true);
                return;
            }

            // download update
            Description = string.Format(UpdatingMessage, currentVersion, availableVersion);
            UpdateUrl = new Uri(metaData[2]);
            var updateFile = _type == UpdateType.Program ?
                Path.Combine(KnownPaths.Instance.ApplicationDirectory, "Setup.exe") :
                Path.Combine(SettingsManager.Instance.PluginsDirectory, "Update.zip");
            FileSystemHelper.Instance.DeleteFile(updateFile);
            try
            {
                _client.DownloadFileAsync(UpdateUrl, updateFile, updateFile);
            }
            catch
            {
                Description = DownloadErrorMessage;
                SignalHost.Instance.NotifyAllObservers(this, Signal.UpdateStopped, _type, false);
                return;
            }

            return;
        }

        void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var updateFile = e.UserState as string;

            // update failed
            if (string.IsNullOrEmpty(updateFile) || !File.Exists(updateFile))
            {
                SignalHost.Instance.NotifyAllObservers(this, Signal.UpdateStopped, _type, false);
                ViewModel.IsExecuting = IsExecuting = false;
                return;
            }

            // intall update
            var isUpdateInstalled = true;
            var message = string.Empty;
            if (_type == UpdateType.Program)
            {
                var startInfo = new ProcessStartInfo(updateFile);
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = KnownPaths.Instance.ApplicationDirectory;
                Process.Start(startInfo);

                App.Current.Shutdown(0);
            }
            else
            {
                if (!PluginManager.Instance.UpdatePlugins(updateFile))
                {
                    message = UpdateErrorMessage;
                    isUpdateInstalled = false;
                }
                else
                    message = string.Format(UpToDateMessage, PluginManager.Instance.Version);

                FileSystemHelper.Instance.DeleteFile(updateFile);
            }

            Description = message;
            Title = "Check for Updates";
            ViewModel.IsExecuting = IsExecuting = false;
            SignalHost.Instance.NotifyAllObservers(this, Signal.UpdateStopped, _type, isUpdateInstalled);
        }

        void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // set execution status as its disabled by command's base
            if (!IsExecuting)
                ViewModel.IsExecuting = IsExecuting = true;

            Progress = e.ProgressPercentage;
        }

        public void Update(ISignalObserver sender, Signal code, params object[] data)
        {
            if (code != Signal.Initialized || _type != UpdateType.Plugin)
                return;

            Description = string.Format(UpToDateMessage, PluginManager.Instance.Version);
        }
    }
}
