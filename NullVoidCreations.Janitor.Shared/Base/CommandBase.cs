using System;
using System.Windows.Input;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class CommandBase: NotificationBase, ICommand
    {
        Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        #region constructor / destructor

        protected CommandBase(ViewModelBase viewModel, bool isEnabled = true)
        {
            IsEnabled = isEnabled;
            ViewModel = viewModel;
        }

        protected CommandBase(ViewModelBase viewModel, Func<object, bool> canExecute)
        {
            _canExecute = canExecute;
            ViewModel = viewModel;
        }

        #endregion

        #region properties

        public bool IsEnabled
        {
            get { return GetValue<bool>("IsEnabled"); }
            set
            {
                this["IsEnabled"] = value;
                RaiseCanExecuteChanged();
            }
        }

        public bool IsRecallAllowed
        {
            get { return GetValue<bool>("IsRecallAllowed"); }
            set
            {
                this["IsRecallAllowed"] = value;
                RaiseCanExecuteChanged();
            }
        }

        public bool IsExecuting
        {
            get { return GetValue<bool>("IsExecuting"); }
            protected set
            {
                this["IsExecuting"] = value;
                RaiseCanExecuteChanged();
            }
        }

        public string Title
        {
            get { return GetValue<string>("Title"); }
            set { this["Title"] = value; }
        }

        public string Description
        {
            get { return GetValue<string>("Description"); }
            set { this["Description"] = value; }
        }

        protected ViewModelBase ViewModel
        {
            get { return GetValue<ViewModelBase>("ViewModel"); }
            private set { this["ViewModel"] = value; }
        }

        #endregion

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter) && !IsExecuting;

            if (IsRecallAllowed)
                return IsEnabled;
            else
                return IsEnabled && !IsExecuting;
        }

        public abstract void Execute(object parameter);

        #region methods

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged == null)
                return;

            CanExecuteChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
