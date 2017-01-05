using System;
using System.ComponentModel;
using System.Threading;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public class AsyncDelegateCommand : CommandBase
    {
        Func<object, object> _method;
        Action<object> _callback;
        BackgroundWorker _worker;

        private AsyncDelegateCommand(ViewModelBase viewModel, Func<object, bool> canExecute)
            : base(viewModel, canExecute)
        {
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler(DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkCompleted);
        }

        public AsyncDelegateCommand(ViewModelBase viewModel, Func<object, bool> canExecute, Func<object, object> method, Action<object> callback)
            : this(viewModel, canExecute)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
            _callback = callback;
        }

        public AsyncDelegateCommand(ViewModelBase viewModel) : this(viewModel, null)
        {
            _method = ExecuteOverride;
            _callback = ExecuteSuccessOverride;
        }

        ~AsyncDelegateCommand()
        {
            if (_worker.IsBusy)
                _worker.CancelAsync();

            _worker.DoWork += new DoWorkEventHandler(DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkCompleted);
            _worker.Dispose();
        }

        protected virtual object ExecuteOverride(object parameter)
        {
            throw new NotImplementedException();
        }

        protected virtual void ExecuteSuccessOverride(object result)
        {
            throw new NotImplementedException();
        }

        void DoWork(object sender, DoWorkEventArgs target)
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            target.Result = _method.Invoke(target.Argument);   
        }

        void WorkCompleted(object sender, RunWorkerCompletedEventArgs target)
        {
            if (target.Error == null && _callback != null)
                _callback.Invoke(target.Result);

            IsExecuting = false;
            if (ViewModel != null)
                ViewModel.IsExecuting = IsExecuting;
        }

        public override void Execute(object parameter)
        {
            if (_worker.IsBusy)
                return;

            IsExecuting = true;
            if (ViewModel != null)
                ViewModel.IsExecuting = IsExecuting;
            
            _worker.RunWorkerAsync(parameter);
        }
    }
}
