using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class AboutViewModel: ViewModelBase
    {
        readonly CommandBase _getTargets;
        List<ScanTargetBase> _targets;

        #region constructor / destructor

        public AboutViewModel()
        {
            _getTargets = new AsyncDelegateCommand(this, null, ExecuteGetTargets, GetTargetsComplete);
        }

        #endregion

        #region properties

        public List<ScanTargetBase> Targets
        {
            get { return _targets; }
            private set
            {
                if (value == _targets)
                    return;

                _targets = value;
                RaisePropertyChanged("Targets");
            }
        }

        #endregion

        #region commands

        public CommandBase GetTargets
        {
            get { return _getTargets; }
        }

        #endregion

        object ExecuteGetTargets(object parameter)
        {
            return PluginManager.Instance.Targets;
        }

        void GetTargetsComplete(object targets)
        {
            var plugins = new List<ScanTargetBase>();
            foreach (var plugin in targets as IEnumerable<ScanTargetBase>)
                plugins.Add(plugin);

            Targets = plugins;
        }
    }
}
