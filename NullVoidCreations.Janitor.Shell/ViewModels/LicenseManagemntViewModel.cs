using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Licensing;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class LicenseManagemntViewModel: ViewModelBase
    {
        #region properties

        public Customer Customer
        {
            get { return GetValue<Customer>("Customer"); }
            private set { this["Customer"] = value; }
        }

        #endregion
        
    }
}
