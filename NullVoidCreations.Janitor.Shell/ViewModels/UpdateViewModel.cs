using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Commands;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class UpdateViewModel: ViewModelBase
    {
        readonly UpdateCommand _pluginUpdate, _programUpdate;

        public UpdateViewModel()
        {
            _pluginUpdate = new UpdateCommand(this, UpdateCommand.UpdateType.Plugin);
            _programUpdate = new UpdateCommand(this, UpdateCommand.UpdateType.Program);
            _pluginUpdate.IsEnabled = true;
        }

        #region properties

        public UpdateCommand PluginUpdate
        {
            get { return _pluginUpdate; }
        }

        public UpdateCommand ProgramUpdate
        {
            get { return _programUpdate; }
        }

        #endregion
    }
}
