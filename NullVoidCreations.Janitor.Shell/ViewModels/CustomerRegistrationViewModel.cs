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

        #endregion

        #region commands

        public CommandBase CreateAccount
        {
            get { return _createAccount; }
        }

        #endregion

        object ExecuteCreateAccount(object parameter)
        {
            try
            {
                Customer.Register(Email, Password);
                LicenseManager.Instance.Login(Email, Password);
            }
            catch (Exception ex)
            {
                return ex;
            }

            return parameter;
        }

        void CreateAccountExecuted(object result)
        {
            IsExecuting = false;
            if (result is CustomWindow)
            {
                var window = result as CustomWindow;
                window.DialogResult = true;
                window.Close();
            }
            else
                UiHelper.Instance.Error((result as Exception).Message);
        }
    }
}
