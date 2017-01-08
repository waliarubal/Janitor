using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class MessageViewModel: ViewModelBase
    {
        string _title, _message, _button1Text, _button2Text, _iconSource;
        readonly CommandBase _ok, _cancel;

        public MessageViewModel()
        {
            _button1Text = "OK";
            _button2Text = "Cancel";
            _iconSource = "/NullVoidCreations.Janitor.Shell;component/Resources/Info48.png";

            _ok = new DelegateCommand(this, ExecuteOk);
            _cancel = new DelegateCommand(this, ExecuteCancel);
            _ok.IsEnabled = _cancel.IsEnabled = true;
        }

        #region properties

        public string Title
        {
            get { return _title; }
            internal set
            {
                if (value == _title)
                    return;

                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        public string Message
        {
            get { return _message; }
            internal set
            {
                if (value == _message)
                    return;

                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        public string Button1Text
        {
            get { return _button1Text; }
            internal set
            {
                if (value == _button1Text)
                    return;

                _button1Text = value;
                RaisePropertyChanged("Button1Text");
            }
        }

        public string Button2Text
        {
            get { return _button2Text; }
            internal set
            {
                if (value == _button2Text)
                    return;

                _button2Text = value;
                RaisePropertyChanged("Button2Text");
            }
        }

        public string Icon
        {
            get { return _iconSource; }
            internal set
            {
                if (value == _iconSource)
                    return;

                _iconSource = value;
                RaisePropertyChanged("Icon");
            }
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
