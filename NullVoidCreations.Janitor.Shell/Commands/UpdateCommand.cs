using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class UpdateCommand : CommandBase
    {
        public enum UpdateType : byte
        {
            Plugin,
            Program
        }

        readonly UpdateType _type;
        readonly char _separator;
        readonly Uri _metadataUrl;
        readonly string _title, _cencelMessage, _downloadErrorMessage, _updatingMessage, _upToDateMessage, _updateFilePath;
        bool _isDownloading;
        WebClient _client;
        string _message;
        int _progress;
        Uri _updateUrl;
        
        #region constructor/destructor

        public UpdateCommand(ViewModelBase viewModel, UpdateType type)
            : base(viewModel)
        {
            // constants
            _title = "Check for Updates";
            _cencelMessage = "Update cancelled by user.";
            _downloadErrorMessage = "An error occured while downloading update. Please try again later.";
            _updatingMessage = "Updating from version {0} to version {1}.";
            _upToDateMessage = "Installed version {0} is up to date.";
            _separator = '|';
            _metadataUrl = new Uri(@"https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/Update.txt");
            _updateFilePath = Path.Combine(SettingsManager.Instance.PluginsDirectory, "Update.zip");
            
            _type = type;
            Title = _title;
            Message = string.Format(_upToDateMessage, new Version());
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
                Title = _title;
                Message = _cencelMessage;
                return;
            }
            if (e.Error != null)
            {
                Title = _title;
                Message = _downloadErrorMessage;
                return;
            }

            var metaData = e.Result.Split(new char[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            if (metaData.Length != 2)
                return;

            Version availableVersion;
            if (_type == UpdateType.Program)
            {
                var programMetaData = metaData[1].Split(new char[] { _separator }, StringSplitOptions.RemoveEmptyEntries);
                if (programMetaData.Length != 3)
                    return;

                availableVersion = new Version(programMetaData[1]);
                _updateUrl = new Uri(programMetaData[2]);
            }
            else
            {
                var pluginsMetaData = metaData[0].Split(new char[] { _separator }, StringSplitOptions.RemoveEmptyEntries);
                if (pluginsMetaData.Length != 3)
                    return;

                availableVersion = new Version(pluginsMetaData[1]);
                Message = string.Format(_updatingMessage, new Version(), availableVersion);
                _updateUrl = new Uri(pluginsMetaData[2]);
                _client.DownloadFileAsync(_updateUrl, _updateFilePath);
            }
        }

        void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            IsDownloading = false;
            if (e.Cancelled)
            {
                Title = _title;
                Message = _cencelMessage;
                return;
            }
            if (e.Error != null)
            {
                Title = _title;
                Message = _downloadErrorMessage;
                return;
            }

            if (_type == UpdateType.Program)
            {

            }
            else
            {
                PluginManager.Instance.UpdatePlugins(_updateFilePath);
                FileSystemHelper.Instance.DeleteFile(_updateFilePath);
            }

            Title = _title;
            Message = string.Format(_upToDateMessage, new Version());
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

            _client.DownloadStringAsync(_metadataUrl);
        }
    }
}
