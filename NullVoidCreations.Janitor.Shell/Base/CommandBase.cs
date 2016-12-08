using System;
using System.Windows.Input;

namespace NullVoidCreations.Janitor.Shell.Base
{
    public abstract class CommandBase: NotificationBase, ICommand
    {
        bool _isExecuting, _isEnabled;
        string _title;
        Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        protected CommandBase()
        {
            IsEnabled = true;
        }

        protected CommandBase(Func<object, bool> canExecute)
        {
            _canExecute = canExecute;
        }

        #region properties

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value == _isEnabled)
                    return;

                _isEnabled = value;
                RaisePropertyChanged("IsEnabled");
                RaiseCanExecuteChanged();
            }
        }

        public bool IsExecuting
        {
            get { return _isExecuting; }
            protected set
            {
                if (value == _isExecuting)
                    return;

                _isExecuting = value;
                RaisePropertyChanged("IsExecuting");
                RaiseCanExecuteChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title)
                    return;

                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        #endregion

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter) && !IsExecuting;

            return IsEnabled && !IsExecuting;
        }

        public abstract void Execute(object parameter);

        #region methods

        void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged == null)
                return;

            CanExecuteChanged(this, EventArgs.Empty);
        }

        #endregion
    }
}
