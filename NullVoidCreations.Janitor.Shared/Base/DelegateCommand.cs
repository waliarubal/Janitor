using System;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public sealed class DelegateCommand : CommandBase
    {
        Action<object> _method;

        #region constructor / destructor

        public DelegateCommand(ViewModelBase viewModel, Action<object> method)
            : this(viewModel, null, method)
        {

        }

        public DelegateCommand(ViewModelBase viewModel, Func<object, bool> canExecute, Action<object> method)
            : base(viewModel, canExecute)
        {
            _method = method;
        }

        #endregion

        public override void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            ViewModel.IsExecuting = IsExecuting = true;
            _method.Invoke(parameter);
            ViewModel.IsExecuting = IsExecuting = false;
        }
        
    }
}
