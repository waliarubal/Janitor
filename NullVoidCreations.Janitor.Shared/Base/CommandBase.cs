using System;
using System.Windows.Input;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class CommandBase: NotificationBase, ICommand
    {
        bool _isExecuting, _isEnabled;
        string _title, _description;
        Func<object, bool> _canExecute;
        ViewModelBase _viewModel;

        public event EventHandler CanExecuteChanged;

        #region constructor / destructor

        protected CommandBase(ViewModelBase viewModel)
        {
            _isEnabled = true;
            _viewModel = viewModel;
        }

        protected CommandBase(ViewModelBase viewModel, Func<object, bool> canExecute)
        {
            _canExecute = canExecute;
            _viewModel = viewModel;
        }

        #endregion

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

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description)
                    return;

                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        protected ViewModelBase ViewModel
        {
            get { return _viewModel; }
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
