using System.Collections.Generic;
using NullVoidCreations.Janitor.Core.Models;
using NullVoidCreations.Janitor.Shell.Commands;

namespace NullVoidCreations.Janitor.Shell.Core
{
    enum WorkSignal: byte
    {
        ProgramUpdate,
        PluginUpdate,
        SmartScan,
        ShowHome,
        ShowTrialWarning
    }

    class WorkQueueManager: ISignalObserver
    {
        readonly Queue<WorkSignal> _work;
        static WorkQueueManager _instance;

        #region constructor / destructor

        private WorkQueueManager()
        {
            _work = new Queue<WorkSignal>();
            SignalHost.Instance.AddObserver(this);
        }

        ~WorkQueueManager()
        {
            SignalHost.Instance.RemoveObserver(this);
        }

        #endregion

        #region properties

        public static WorkQueueManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WorkQueueManager();

                return _instance;
            }
        }

        #endregion

        public void AddWork(WorkSignal work)
        {
            if (_work.Contains(work))
                return;

            _work.Enqueue(work);
        }

        public void DoWork()
        {
            if (_work.Count == 0)
                return;

            var work =_work.Dequeue();
            switch(work)
            {
                case WorkSignal.ProgramUpdate:
                    SignalHost.Instance.RaiseSignal(this, Signal.UpdateTriggered, UpdateCommand.UpdateType.Program);
                    break;

                case WorkSignal.PluginUpdate:
                    SignalHost.Instance.RaiseSignal(this, Signal.UpdateTriggered, UpdateCommand.UpdateType.Plugin);
                    break;

                case WorkSignal.SmartScan:
                    SignalHost.Instance.RaiseSignal(this, Signal.ScanTrigerred, ScanType.SmartScan);
                    break;

                case WorkSignal.ShowHome:
                    SignalHost.Instance.RaiseSignal(this, Signal.ShowHome);
                    break;

                case WorkSignal.ShowTrialWarning:
                    if (LicenseExManager.Instance.License.IsTrial)
                        new BalloonCommand(null).Execute("https://www.google.com");
                    break;
            }
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            switch(signal)
            {
                case Signal.StopWork:
                    DoWork();
                    break;
            }
        }
    }
}
