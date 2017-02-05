using System;
using System.Net;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class BalloonCommand: DelegateCommand
    {
        WebClient _client;

        public BalloonCommand(ViewModelBase viewModel)
            : base(viewModel)
        {
            IsEnabled = true;

            _client = new WebClient();
            _client.Proxy = null;
            _client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Client_DownloadStringCompleted);
        }

        ~BalloonCommand()
        {
            _client.DownloadStringCompleted -= new DownloadStringCompletedEventHandler(Client_DownloadStringCompleted);
            _client.Dispose();
        }

        void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            SignalHost.Instance.RaiseSignal(Signal.ShowBaloon, e.Result);
        }

        protected override void ExecuteOverride(object parameter)
        {
            if (parameter == null)
                SignalHost.Instance.RaiseSignal(Signal.HideBaloon);
            else
                _client.DownloadStringAsync(new Uri(parameter as string));
        }
    }
}
