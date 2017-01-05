using System;
using System.Net;
using System.Windows;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.ViewModels;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.Commands
{
    public class BalloonCommand: DelegateCommand
    {
        TaskbarIcon _notificationIcon;
        WebClient _client;
        BalloonView _content;

        public BalloonCommand(ViewModelBase viewModel)
            : base(viewModel)
        {
            IsEnabled = true;

            _client = new WebClient();
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

            if (_notificationIcon == null)
                _notificationIcon = (TaskbarIcon)App.Current.Resources["NotificationIcon"];

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (_content == null)
                    _content = new BalloonView();
                (_content.DataContext as BalloonViewModel).Html = e.Result;
                _notificationIcon.ShowCustomBalloon(_content, PopupAnimation.Slide, 60000);
            });
        }

        protected override void ExecuteOverride(object parameter)
        {
            if (parameter == null)
                return;
            
            var uri = new Uri(parameter as string);
            _client.DownloadStringAsync(uri);
        }
    }
}
