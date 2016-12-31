using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class UpdateCommand : CommandBase, IObserver
    {
        public enum UpdateType : byte
        {
            Plugin,
            Program
        }

        const string Name = "Check for Updates";
        const string CencelMessage = "Update cancelled by user.";
        const string DownloadErrorMessage = "An error occured while downloading update. Please try again later.";
        const string UpdatingMessage = "Updating from version {0} to version {1}.";
        const string UpToDateMessage = "Installed version {0} is up to date.";
        const char Separator = '|';
        readonly Uri  MetadataUrl = new Uri(@"https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/Update.txt");

        readonly UpdateType _type;
        readonly string _updateFilePath;
        bool _isDownloading;
        WebClient _client;
        string _message;
        int _progress;
        Uri _updateUrl;
        
        #region constructor/destructor

        public UpdateCommand(ViewModelBase viewModel, UpdateType type)
            : base(viewModel)
        {
            Subject.Instance.AddObserver(this);

            _updateFilePath = Path.Combine(SettingsManager.Instance.PluginsDirectory, "Update.zip");

            _type = type;
            Title = Name;
            IsRecallAllowed = true;

            _client = new WebClient();
            _client.Proxy = null;
            _client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            _client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Client_DownloadFileCompleted);
            _client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Client_DownloadStringCompleted);
        }

        ~UpdateCommand()
        {
            _client.Dispose();
            Subject.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        public bool IsDownloading
        {
            get { return _isDownloading; }
            private set
            {
                if (value == _isDownloading)
                    return;

                _isDownloading = value;
                RaisePropertyChanged("IsDownloading");
            }
        }

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

        public string Message
        {
            get { return _message; }
            private set
            {
                if (value == _message)
                    return;

                _message = value;
                RaisePropertyChanged("Message");
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

        void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            IsDownloading = false;
            if (e.Cancelled)
            {
                Title = Name;
                Message = CencelMessage;
                return;
            }
            if (e.Error != null)
            {
                Title = Name;
                Message = DownloadErrorMessage;
                return;
            }

            var metaData = e.Result.Split(new char[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            if (metaData.Length != 2)
                goto EXIT;

            if (_type == UpdateType.Program)
            {
                var programMetaData = metaData[1].Split(new char[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
                if (programMetaData.Length != 3)
                    goto EXIT;

                var availableVersion = new Version(programMetaData[1]);
                UpdateUrl = new Uri(programMetaData[2]);
            }
            else
            {
                var pluginsMetaData = metaData[0].Split(new char[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
                if (pluginsMetaData.Length != 3)
                    return;

                var availableVersion = new Version(pluginsMetaData[1]);
                if (availableVersion <= PluginManager.Instance.Version)
                    goto EXIT;

                Message = string.Format(UpdatingMessage, PluginManager.Instance.Version, availableVersion);
                UpdateUrl = new Uri(pluginsMetaData[2]);
                _client.DownloadFileAsync(_updateUrl, _updateFilePath, availableVersion);
            }

        EXIT:
            Title = Name;
            Message = string.Format(UpToDateMessage, PluginManager.Instance.Version);
        }

        void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            IsDownloading = false;
            if (e.Cancelled)
            {
                Title = Name;
                Message = CencelMessage;
                return;
            }
            if (e.Error != null)
            {
                Title = Name;
                Message = DownloadErrorMessage;
                return;
            }

            if (_type == UpdateType.Program)
            {

            }
            else
            {
                PluginManager.Instance.UpdatePlugins(e.UserState as Version, _updateFilePath);
                FileSystemHelper.Instance.DeleteFile(_updateFilePath);
            }

            Title = Name;
            Message = string.Format(UpToDateMessage, PluginManager.Instance.Version);
        }

        void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        public override void Execute(object parameter)
        {
            Title = "Cancel Update";
            IsDownloading = true;
            Progress = 0;

            _client.DownloadStringAsync(MetadataUrl);
        }

        public void Update(IObserver sender, MessageCode code, params object[] data)
        {
            if (code != MessageCode.Initialized)
                return;

            Message = string.Format(UpToDateMessage, PluginManager.Instance.Version);
        }
    }
}
