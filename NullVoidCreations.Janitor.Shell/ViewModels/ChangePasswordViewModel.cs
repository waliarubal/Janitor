using System;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Controls;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.ViewModels
{
    public class ChangePasswordViewModel: ViewModelBase
    {
        CommandBase _changePassword;

        #region properties

        public string Password
        {
            get { return GetValue<string>("Password"); }
            set { this["Password"] = value; }
        }

        public string NewPassword
        {
            get { return GetValue<string>("NewPassword"); }
            set { this["NewPassword"] = value; }
        }

        

        public string ConfirmPassword
        {
            get { return GetValue<string>("ConfirmPassword"); }
            set { this["ConfirmPassword"] = value; }
        }

        #endregion

        #region commands

        public CommandBase ChangePassword
        {
            get 
            {
                if (_changePassword == null)
                    _changePassword = new AsyncDelegateCommand(this, null, ExecuteChangePassword, ChangePasswordExecuted) { IsEnabled = true };

                return _changePassword;
            }
        }

        #endregion

        object ExecuteChangePassword(object parameters)
        {
            return new object[] { parameters, LicenseManager.Instance.ChangePassword(Password, NewPassword, ConfirmPassword) };
        }

        void ChangePasswordExecuted(object result)
        {
            IsExecuting = false;

            var res = result as object[];
            if (res[1] == null)
            {
                UiHelper.Instance.Alert("Passowrd changed successfully.");

                var window = res[0] as CustomWindow;
                window.DialogResult = true;
                window.Close();
            }
            else
                UiHelper.Instance.Error((res[1] as Exception).Message);
        }
    }
}
