
namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class ViewModelBase: NotificationBase
    {
        #region properties

        public bool IsExecuting
        {
            get { return GetValue<bool>("IsExecuting"); }
            set { this["IsExecuting"] = value; }
        }

        #endregion
    }
}
