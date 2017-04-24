using System;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class AuthenticationViewModel: ViewModelBase
    {
        CommandBase _login;

        #region properties

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

        #endregion

        #region commands

        public CommandBase Login
        {
            get 
            {
                if (_login == null)
                    _login = new AsyncDelegateCommand(this, null, ExecuteLogin, LoginExecuted) { IsEnabled = true };

                return _login;
            }
        }

        #endregion

        object ExecuteLogin(object parameters)
        {
            return new object[] { parameters, LicenseManager.Instance.Login(Email, Password) };
        }

        void LoginExecuted(object result)
        {
            IsExecuting = false;

            var res = result as object[];
            if (res[1] == null)
            {
                var window = res[0] as CustomWindow;
                window.DialogResult = true;
                window.Close();
            }
            else
                UiHelper.Instance.Error((res[1] as Exception).Message);
        }
    }
}
