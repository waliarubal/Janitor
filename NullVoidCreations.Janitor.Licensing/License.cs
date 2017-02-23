using System;
using System.IO;
using System.Text;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NullVoidCreations.Janitor.Licensing
{
    [BsonDiscriminator("lic")]
    public class License: IDisposable
    {
        const string ValidCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        const string EncryptionKey = "QFByb3Blcl9QYXRvbGEhMjAxNQ==";
        const int KeySize = 23;

        string _serialKey, _activationKey, _fileName;

        #region constructor / destructor

        private License()
        {

        }

        [BsonConstructor]
        public License(string serialKey, string activationKey)
        {
            SerialKey = serialKey;
            ActivationKey = activationKey;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~License()
        {
            Dispose(false);
        }

        #endregion

        #region properties

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("key", Order = 1)]
        public string SerialKey
        {
            get { return _serialKey; }
            private set
            {
                Segment1 = value.Substring(0, 5);
                Segment2 = value.Substring(6, 5);
                Segment3 = value.Substring(12, 5);
                Segment4 = value.Substring(18, 5);
                _serialKey = value;
            }
        }

        [BsonIgnore]
        public string Segment1
        {
            get;
            private set;
        }

        [BsonIgnore]
        public string Segment2
        {
            get;
            private set;
        }

        [BsonIgnore]
        public string Segment3
        {
            get;
            private set;
        }

        [BsonIgnore]
        public string Segment4
        {
            get;
            private set;
        }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("act_key", Order = 2)]
        public string ActivationKey
        {
            get { return _activationKey; }
            private set
            {
                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException("Activation key not specified.");

                var decryptedKey = StringCipher.Instance.Decrypt(value, EncryptionKey);
                try
                {
                    IssueDate = ExtractDate(decryptedKey, 0);
                    ExpirationDate = ExtractDate(decryptedKey, 8);
                    Email = decryptedKey.Substring(16, decryptedKey.Length - 16);
                    IsActivated = IsValid(this);

                    _activationKey = value;
                    IsActivated = true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Invalid activation key.");
                }
            }
        }

        [BsonIgnore]
        public bool IsActivated
        {
            get;
            private set;
        }

        [BsonIgnore]
        public string Email
        {
            get;
            private set;
        }

        [BsonIgnore]
        public DateTime IssueDate
        {
            get;
            private set;
        }

        [BsonIgnore]
        public DateTime ExpirationDate
        {
            get;
            private set;
        }

        #endregion

        public static string ValidateSerial(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "Serial key not specified.";
            if (key.Length != KeySize)
                return "Invalid serial key size.";

            return null;
        }

        internal static License LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            var document = new XmlDocument();
            document.Load(fileName);

            var license = new License();
            license._fileName = fileName;
            license.SerialKey = document.SelectSingleNode("/License/SerialKey").InnerText;
            license.ActivationKey = document.SelectSingleNode("/License/ActivationKey").InnerText;
            return license;
        }

        internal static License Generate(DateTime isssueDate, DateTime expirationDate, string email)
        {
            var randomGenerator = new Random();
            var licenseBuilder = new StringBuilder(KeySize);

            for (int index = 1; index <= KeySize - 3; index++)
            {
                var serialChar = ValidCharacters[randomGenerator.Next(ValidCharacters.Length)];
                if (index % 5 == 0 && index < KeySize - 3)
                    licenseBuilder.AppendFormat("{0}-", serialChar);
                else
                    licenseBuilder.Append(serialChar);
            }

            var license = new License();
            license.SerialKey = licenseBuilder.ToString();
            license.GenerateActivationKey(isssueDate, expirationDate, email);
            return license;
        }

        internal void SaveToFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            var document = new XmlDocument();
            var root = document.CreateElement("License");

            var serialKeyNode = document.CreateElement("SerialKey");
            serialKeyNode.InnerText = SerialKey;
            root.AppendChild(serialKeyNode);

            var activationKeyNode = document.CreateElement("ActivationKey");
            activationKeyNode.InnerText = ActivationKey;
            root.AppendChild(activationKeyNode);

            document.AppendChild(root);

            document.Save(fileName);
            _fileName = fileName;
        }

        #region private methods

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(_fileName))
                    SaveToFile(_fileName);
            }

            IsActivated = false;
            _serialKey = _activationKey = _fileName = null;
            Segment1 = Segment2 = Segment3 = Segment4 = Email = null;
            ExpirationDate = IssueDate = DateTime.MinValue;
        }

        void GenerateActivationKey(DateTime isssueDate, DateTime expirationDate, string email)
        {
            IssueDate = isssueDate;
            ExpirationDate = expirationDate;
            Email = email;

            var key = string.Format("{0:0000}{1:00}{2:00}{3:0000}{4:00}{5:00}{6}",
                IssueDate.Year,
                IssueDate.Month,
                IssueDate.Day,
                ExpirationDate.Year,
                ExpirationDate.Month,
                ExpirationDate.Day,
                Email);
            _activationKey = StringCipher.Instance.Encrypt(key, EncryptionKey);
        }

        DateTime ExtractDate(string text, int startIndex)
        {
            var date = new DateTime(
                int.Parse(text.Substring(startIndex, 4)),
                int.Parse(text.Substring(startIndex + 4, 2)),
                int.Parse(text.Substring(startIndex + 6, 2)));

            return date;
        }

        bool IsValid(License license)
        {
            var currentDate = DateTime.Now;
            if (currentDate.Date < license.IssueDate.Date)
                return false;
            if (currentDate.Date > license.ExpirationDate.Date)
                return false;

            return true;
        }

        #endregion
    }
}
