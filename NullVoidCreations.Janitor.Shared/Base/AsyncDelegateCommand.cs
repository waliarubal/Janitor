﻿using System;
using System.ComponentModel;

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
            _worker.DoWork += new DoWorkEventHandler(DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkCompleted);
        }

        public AsyncDelegateCommand(ViewModelBase viewModel, Func<object, bool> canExecute, Func<object, object> method, Action<object> callback)
            : this(viewModel, canExecute)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (callback == null)
                throw new ArgumentNullException("callback");

            _method = method;
            _callback = callback;
        }

        ~AsyncDelegateCommand()
        {
            if (_worker.IsBusy)
                _worker.CancelAsync();

            _worker.DoWork += new DoWorkEventHandler(DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkCompleted);
            _worker.Dispose();
        }

        void DoWork(object sender, DoWorkEventArgs target)
        {
            target.Result = _method.Invoke(target.Argument);   
        }

        void WorkCompleted(object sender, RunWorkerCompletedEventArgs target)
        {
            if (target.Error == null)
                _callback.Invoke(target.Result);

            ViewModel.IsExecuting = IsExecuting = false;
        }

        public override void Execute(object parameter)
        {
            if (_worker.IsBusy)
                return;

            ViewModel.IsExecuting = IsExecuting = true;
            _worker.RunWorkerAsync(parameter);
        }
    }
}
