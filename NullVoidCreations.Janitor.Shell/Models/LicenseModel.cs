using NullVoidCreations.Janitor.Shared.Base;
using System;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class LicenseModel: NotificationBase
    {
        bool _isTrial;
        DateTime _issueDate, _expirationDate;
        string _registeredEmail, _licenseKey;

        public LicenseModel()
        {
            Rest();
        }

        #region properties

        public string LicenseKey
        {
            get { return _licenseKey; }
            set
            {
                _licenseKey = value;
                RaisePropertyChanged("LicenseKey");
            }
        }

        public string RegisteredEmail
        {
            get { return _registeredEmail; }
            private set
            {
                _registeredEmail = value;
                RaisePropertyChanged("RegisteredEmail");
            }
        }

        public DateTime IssueDate
        {
            get { return _issueDate; }
            private set
            {
                _issueDate = value;
                RaisePropertyChanged("IssueDate");
            }
        }

        public DateTime ExpirationDate
        {
            get { return _expirationDate; }
            private set
            {
                _expirationDate = value;
                RaisePropertyChanged("ExpirationDate");
            }
        }

        public bool IsTrial
        {
            get { return _isTrial; }
            private set
            {
                _isTrial = value;
                RaisePropertyChanged("IsTrial");
            }
        }

        #endregion

        internal void Rest()
        {
            IssueDate = new DateTime(1900, 1, 1);
            ExpirationDate = new DateTime(1900, 1, 1);
            IsTrial = true;
        }

        internal string Generate(DateTime isssueDate, DateTime expirationDate, string registeredEmail)
        {
            IssueDate = isssueDate;
            ExpirationDate = expirationDate;
            RegisteredEmail = registeredEmail;

            var key = string.Format("{0:0000}{1:00}{2:00}{3:0000}{4:00}{5:00}{6}",
                IssueDate.Year,
                IssueDate.Month,
                IssueDate.Day,
                ExpirationDate.Year,
                ExpirationDate.Month,
                ExpirationDate.Day,
                RegisteredEmail);
            LicenseKey = StringCipher.Instance.Encrypt(key, LicenseManager.EncryptionKey);

            return LicenseKey;
        }

        internal string Validate(string key)
        {
            LicenseKey = key;
            string errorMessage = null;
            try
            {
                var decryptedKey = StringCipher.Instance.Decrypt(LicenseKey, LicenseManager.EncryptionKey);
                IssueDate = ExtractDate(decryptedKey, 0);
                ExpirationDate = ExtractDate(decryptedKey, 8);
                RegisteredEmail = key.Substring(16, decryptedKey.Length - 16);
                IsTrial = IsExpired(this);

                if (IsTrial)
                    errorMessage = "License key expired.";
            }
            catch
            {
                errorMessage = "Invalid license key.";
                Rest();
            }

            return errorMessage;
        }

        bool IsExpired(LicenseModel license)
        {
            var currentDate = DateTime.Now;
            if (currentDate.Date < license.IssueDate.Date)
                return true;
            if (currentDate.Date > license.ExpirationDate.Date)
                return true;

            return false;
        }

        DateTime ExtractDate(string text, int startIndex)
        {
            var date = new DateTime(
                int.Parse(text.Substring(startIndex, 4)),
                int.Parse(text.Substring(startIndex + 4, 2)),
                int.Parse(text.Substring(startIndex + 6, 2)));

            return date;
        }
    }
}
