using System;
using NullVoidCreations.Janitor.Licensing;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class CustomerRegistrationViewModel: ViewModelBase
    {
        CommandBase _createAccount;

        public CustomerRegistrationViewModel()
        {
            Customer = new Customer();

            _createAccount = new AsyncDelegateCommand(this, null, ExecuteCreateAccount, CreateAccountExecuted);
        }

        #region properties

        public Customer Customer
        {
            get { return GetValue<Customer>("Customer"); }
            private set { this["Customer"] = value; }
        }

        public string Email
        {
            get { return GetValue<string>("Email"); }
            set { this["Email"] = value; }
        }

        public string Password
        {
            get { return GetValue<string>("Password"); }
            set { this["Password"] = value; }
        }

        public bool IsCustomerAgreementAgreed
        {
            get { return GetValue<bool>("IsCustomerAgreementAgreed"); }
            set
            {
                this["IsCustomerAgreementAgreed"] = value;
                CreateAccount.IsEnabled = value;
            }
        }

        public bool IsTrialKeyRequested
        {
            get { return GetValue<bool>("IsTrialKeyRequested"); }
            set { this["IsTrialKeyRequested"] = value; }
        }

        public string ErrorMessage
        {
            get { return GetValue<string>("ErrorMessage"); }
            private set { this["ErrorMessage"] = value; }
        }

        #endregion

        #region commands

        public CommandBase CreateAccount
        {
            get { return _createAccount; }
        }

        #endregion

        object ExecuteCreateAccount(object parameter)
        {
            ErrorMessage = null;
            var isLicenseActivated = false;
            try
            {
                var serialKey = Customer.Register(Email, Password, IsTrialKeyRequested);
                if (IsTrialKeyRequested && !string.IsNullOrEmpty(serialKey))
                    isLicenseActivated = LicenseManager.Instance.License.Activate(serialKey);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            object[] args = { parameter, isLicenseActivated };
            return args;
        }

        void CreateAccountExecuted(object result)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                var args = result as object[];
                (args[0] as CustomWindow).Close();
                if ((bool)args[1])
                    LicenseManager.Instance.LoadLicense();
            }
        }
    }
}
