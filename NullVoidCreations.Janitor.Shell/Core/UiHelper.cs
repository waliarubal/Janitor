using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NullVoidCreations.Janitor.Shared;
using NullVoidCreations.Janitor.Shell.ViewModels;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class UiHelper
    {
        static UiHelper _instance;
        MainView _mainWindow;

        private UiHelper()
        {

        }

        #region properties

        public static UiHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UiHelper();

                return _instance;
            }
        }

        public ResourceDictionary Resources
        {
            get { return App.Current.Resources; }
        }

        public MainView MainWindow
        {
            get 
            {
                if (_mainWindow == null)
                    _mainWindow = App.Current.MainWindow as MainView;

                return _mainWindow; 
            }
        }

        #endregion

        public void DoBackgroundWork(Action work)
        {
            var threadStart = new ThreadStart(work);

            var thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        public bool? ShowPopup(Window window)
        {
            window.Owner = MainWindow;
            return window.ShowDialog();
        }

        public void ExecuteOnUiThread(Action action, params object[] arguments)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(action, arguments);
        }

        public bool Question(string messageFormat, params object[] messageParts)
        {
            return ShowMessage(Constants.ProductName, "/program_shell;component/Resources/Question48.png", "Yes", "No", true, messageFormat, messageParts);
        }

        public void Error(string messageFormat, params object[] messageParts)
        {
            ShowMessage(Constants.ProductName, "/program_shell;component/Resources/Error48.png", "OK", "Cancel", false, messageFormat, messageParts);
        }

        public void Alert(string messageFormat, params object[] messageParts)
        {
            ShowMessage(Constants.ProductName, "/program_shell;component/Resources/Info48.png", "OK", "Cancel", false, messageFormat, messageParts);
        }

        bool ShowMessage(
            string title, 
            string icon, 
            string button1Text, 
            string button2Text,
            bool isButton2Visible,
            string messageFormat,
            params object[] messageParts)
        {
            var message = string.Format(messageFormat, messageParts);

            var view = new MessageView();
            var viewModel = view.DataContext as MessageViewModel;
            viewModel.Title = title;
            viewModel.Button1Text = button1Text;
            viewModel.Button2Text = button2Text;
            viewModel.IsButton2Visible = isButton2Visible;
            viewModel.Icon = icon;
            viewModel.Message = message;
            if (ShowPopup(view) == true)
                return true;

            return false;
        }

    }
}
