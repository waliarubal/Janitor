using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace NullVoidCreations.Janitor.Licensing
{
    [BsonDiscriminator("cust")]
    public class Customer
    {
        string _password;

        #region properties

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("mail", Order = 1)]
        public string Email { get; set; }

        [BsonElement("reg_date", Order = 2)]
        public DateTime RegistrationDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("name", Order = 3)]
        public string Name { get; set; }

        [BsonRequired]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("pass", Order = 4)]
        public string PasswordHash { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("lics", Order = 5)]
        public IList<License> Licenses { get; set; }

        [BsonIgnore]
        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password)
                    return;

                _password = value;
                PasswordHash = StringCipher.Instance.MD5Hash(_password);
            }
        }

        [BsonIgnore]
        MongoCollection<Customer> Customers
        {
            get 
            { 
                var connection = MongoHelper.Instance.GetConnection();
                if (connection == null)
                    return null;

                return connection.GetCollection<Customer>("cust"); 
            }
        }

        #endregion

        Customer GetCustomer(string email)
        {
            var query = Query<Customer>.EQ(e => e.Email, email);
            var entity = Customers.FindOneAs<Customer>(query);
            return entity;
        }

        public string Register(string confirmEmail, string confirmPassword, bool generateTrial = false)
        {
            if (Customers == null)
                throw new InvalidOperationException("Could not connect to internet for registration.");
            if (string.IsNullOrEmpty(Name))
                throw new InvalidOperationException("Name not entered.");
            if (string.IsNullOrEmpty(Email))
                throw new InvalidOperationException("Email address not entered.");
            if (!Regex.IsMatch(Email, "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$"))
                throw new InvalidOperationException("Email address is not in a valid format.");
            if (string.IsNullOrEmpty(confirmEmail))
                throw new InvalidOperationException("Please retype email address.");
            if (!Email.Equals(confirmEmail))
                throw new InvalidOperationException("Email address does not match.");
            if (string.IsNullOrEmpty(Password))
                throw new InvalidOperationException("Password not entered.");
            if (string.IsNullOrEmpty(confirmPassword))
                throw new InvalidOperationException("Please retype password.");
            if (!Password.Equals(confirmPassword))
                throw new InvalidOperationException("Password does not match.");

            var query = Query<Customer>.EQ(e => e.Email, Email);
            var entity = Customers.FindOne(query);
            if (entity != null)
                throw new InvalidOperationException(String.Format("Another user with email address {0} exists. Use a different email address.", Email));

            RegistrationDate = DateTime.Now;
            if (!Customers.Save(this).Ok)
                throw new Exception("An error occured while registering user.");

            
            if (generateTrial)
            {
                License license;
                if (AddLicense(DateTime.Now, DateTime.Now.AddDays(90), out license))
                    return license.SerialKey;
                else
                    throw new Exception("Failed to generate license.");
            }

            return null;
        }

        public bool AddLicense(DateTime issueDate, DateTime expirationDate, out License license)
        {
            license = License.Generate(issueDate, expirationDate, Email);

            var query = Query<Customer>.EQ(e => e.Email, Email);
            var update = Update<Customer>.Push<License>(l => l.Licenses, license);
            return Customers.Update(query, update).Ok;
        }

        public License ActivateLicense(string serialKey, string fileName)
        {
            if (Customers == null)
                throw new InvalidOperationException("Could not connect to internet for activation.");

            var errorMessage = License.ValidateSerial(serialKey);
            if (errorMessage != null)
                throw new InvalidOperationException(errorMessage);

            var query = Query<Customer>.ElemMatch(
                cust => cust.Licenses, 
                lic => lic.EQ(
                    l => l.SerialKey, serialKey));

            var customer = Customers.FindOne(query);
            if (customer == null)
                throw new InvalidOperationException("Invalid serial key.");

            Email = customer.Email;
            Name = customer.Name;
            Password = customer.Password;
            Licenses = customer.Licenses;

            foreach (var licence in customer.Licenses)
            {
                if (licence.SerialKey.Equals(serialKey))
                {
                    if (!licence.MachineKey.Equals(licence.GetMachineKey()))
                        throw new InvalidOperationException("Serial key is not valid for this machine.");

                    licence.SaveToFile(fileName);
                    return licence;
                }
            }

            return null;
        }

        public License LoadLicense(string fileName)
        {
            return License.LoadFromFile(fileName);
        }
    }
}
