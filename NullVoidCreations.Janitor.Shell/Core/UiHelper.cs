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

        public bool Question(string title, string messageFormat, params object[] messageParts)
        {
            var message = string.Format(messageFormat, messageParts);

            var view = new MessageView();
            view.Owner = App.Current.MainWindow;
            var viewModel = view.DataContext as MessageViewModel;
            viewModel.Title = title;
            viewModel.Button1Text = "Yes";
            viewModel.Button2Text = "No";
            viewModel.Icon = "/NullVoidCreations.Janitor.Shell;component/Resources/Question48.png";
            viewModel.Message = message;
            if (view.ShowDialog() == true)
                return true;

            return false;
        }

    }
}
