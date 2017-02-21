using System;
using NullVoidCreations.Janitor.Licensing;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class CustomerRegistrationViewModel: ViewModelBase
    {
        Customer _customer;
        string _email, _password, _errorMessage;
        bool _isCustomerAgrementAgreed, _isTrialKeyRequested;

        CommandBase _createAccount;

        public CustomerRegistrationViewModel()
        {
            _customer = new Customer();

            _createAccount = new AsyncDelegateCommand(this, null, ExecuteCreateAccount, CreateAccountExecuted);
        }

        #region properties

        public Customer Customer
        {
            get { return _customer; }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (value == _email)
                    return;

                _email = value;
                RaisePropertyChanged("Email");
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password)
                    return;

                _password = value;
                RaisePropertyChanged("Password");
            }
        }

        public bool IsCustomerAgreementAgreed
        {
            get { return _isCustomerAgrementAgreed; }
            set
            {
                if (value == _isCustomerAgrementAgreed)
                    return;

                _isCustomerAgrementAgreed = value;
                CreateAccount.IsEnabled = value;
                RaisePropertyChanged("IsCustomerAgreementAgreed");
            }
        }

        public bool IsTrialKeyRequested
        {
            get { return _isTrialKeyRequested; }
            set
            {
                if (value == _isTrialKeyRequested)
                    return;

                _isTrialKeyRequested = value;
                RaisePropertyChanged("IsTrialKeyRequested");
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
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
            ErrorMessage = null;
            try
            {
                Customer.Register(Email, Password, IsTrialKeyRequested);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return parameter;
        }

        void CreateAccountExecuted(object result)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
                (result as CustomWindow).Close();
        }
    }
}
