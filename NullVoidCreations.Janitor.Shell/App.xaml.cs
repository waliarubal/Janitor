using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using NullVoidCreations.Janitor.Shell.Commands;
using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISignalObserver
    {

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //TODO: add exception handeler
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SignalHost.Instance.RemoveObserver(this);
            App.Current.DispatcherUnhandledException -= new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            base.OnExit(e);
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SignalHost.Instance.AddObserver(this);
            App.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);

            ((TaskbarIcon)App.Current.Resources["NotificationIcon"]).Visibility = Visibility.Visible;
            MainWindow = new MainView();
            MainWindow.Show();

            CommandLineManager.Instance.LoadArguments(e.Args);
            CommandLineManager.Instance.ProcessArguments();
            new InitializeAppCommand().Execute(null);
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            switch (signal)
            {
                case Signal.LicenseChanged:
                    if (LicenseManager.Instance.License.IsTrial)
                        new BalloonCommand(null).Execute("Https://www.google.com");
                    break;

                case Signal.CloseToTray:
                    MainWindow.WindowState = WindowState.Minimized;
                    break;

                case Signal.Initialized:
                    break;
            }
        }
    }
}
