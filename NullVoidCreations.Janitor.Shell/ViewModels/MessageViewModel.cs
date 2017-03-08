using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class MessageViewModel: ViewModelBase
    {
        readonly CommandBase _ok, _cancel;

        public MessageViewModel()
        {
            Button1Text = "OK";
            Button2Text = "Cancel";
            Icon = "/program_shell;component/Resources/Info48.png";

            _ok = new DelegateCommand(this, ExecuteOk);
            _cancel = new DelegateCommand(this, ExecuteCancel);
            _ok.IsEnabled = _cancel.IsEnabled = true;
        }

        #region properties

        public string Title
        {
            get { return GetValue<string>("Title"); }
            internal set { this["Title"] = value; }
        }

        public string Message
        {
            get { return GetValue<string>("Message"); }
            internal set { this["Message"] = value; }
        }

        public string Button1Text
        {
            get { return GetValue<string>("Button1Text"); }
            internal set { this["Button1Text"] = value; }
        }

        public string Button2Text
        {
            get { return GetValue<string>("Button2Text"); }
            internal set { this["Button2Text"] = value; }
        }

        public bool IsButton2Visible
        {
            get { return GetValue<bool>("IsButton2Visible"); }
            internal set { this["IsButton2Visible"] = value; }
        }

        public string Icon
        {
            get { return GetValue<string>("Icon"); }
            internal set { this["Icon"] = value; }
        }

        #endregion

        #region commands

        public CommandBase Ok
        {
            get { return _ok; }
        }

        public CommandBase Cancel
        {
            get { return _cancel; }
        }

        #endregion

        void ExecuteOk(object parameter)
        {
            var window = parameter as CustomWindow;
            window.DialogResult = true;
            window.Close();
        }

        void ExecuteCancel(object parameter)
        {
            var window = parameter as CustomWindow;
            window.DialogResult = false;
            window.Close();
        }
    }
}
