using System;
using System.Windows.Threading;
using NullVoidCreations.Janitor.Shell.ViewModels;
using NullVoidCreations.Janitor.Shell.Views;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class UiHelper
    {
        static UiHelper _instance;

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

        #endregion

        public void ExecuteOnUiThread(Action action, params object[] arguments)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(action, arguments);
        }

        public bool Question(string messageFormat, params object[] messageParts)
        {
            return ShowMessage(App.ProductName, "/NullVoidCreations.Janitor.Shell;component/Resources/Question48.png", "Yes", "No", messageFormat, messageParts);
        }

        public void Error(string messageFormat, params object[] messageParts)
        {
            ShowMessage(App.ProductName, "/NullVoidCreations.Janitor.Shell;component/Resources/Error48.png", "OK", "Cancel", messageFormat, messageParts);
        }

        public void Alert(string messageFormat, params object[] messageParts)
        {
            ShowMessage(App.ProductName, "/NullVoidCreations.Janitor.Shell;component/Resources/Info48.png", "OK", "Cancel", messageFormat, messageParts);
        }

        bool ShowMessage(
            string title, 
            string icon, 
            string button1Text, 
            string button2Text, 
            string messageFormat, 
            params object[] messageParts)
        {
            var message = string.Format(messageFormat, messageParts);

            var view = new MessageView();
            view.Owner = App.Current.MainWindow;
            var viewModel = view.DataContext as MessageViewModel;
            viewModel.Title = title;
            viewModel.Button1Text = button1Text;
            viewModel.Button2Text = button2Text;
            viewModel.Icon = icon;
            viewModel.Message = message;
            if (view.ShowDialog() == true)
                return true;

            return false;
        }

    }
}
