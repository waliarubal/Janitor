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

        [BsonIgnoreIfDefault]
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

        Customer GetCustomer(string email, string password)
        {
            var query = Query.And(
                Query<Customer>.EQ(e => e.Email, email), 
                Query<Customer>.EQ(e => e.PasswordHash, StringCipher.Instance.MD5Hash(password)));
            var entity = Customers.FindOneAs<Customer>(query);
            return entity;
        }

        public void Refresh()
        {
            var customer = GetCustomer(Email);
            Name = customer.Name;
            PasswordHash = customer.PasswordHash;
            Licenses = customer.Licenses;
        }

        public Customer Login(string email, string password)
        {
            if (Customers == null)
                throw new InvalidOperationException("Could not connect to internet.");
            if (string.IsNullOrEmpty(email))
                throw new InvalidOperationException("Email address not entered.");
            if (!Regex.IsMatch(email, "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$"))
                throw new InvalidOperationException("Email address is not in a valid format.");
            if (string.IsNullOrEmpty(password))
                throw new InvalidOperationException("Password not entered.");

            var customer = GetCustomer(email, password);
            if (customer == null)
                throw new Exception("Email/password pair is incorrect.");

            return customer;
        }

        public void Register(string confirmEmail, string confirmPassword)
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
        }

        public bool RemoveLicense(string serialKey, string fileName)
        {
            if (Customers == null)
                throw new InvalidOperationException("Could not connect to internet license removal.");

            var pull = Update<Customer>.Pull(customer => customer.Licenses, license => license.EQ(q => q.SerialKey, serialKey));
            var query = Query.And(Query.EQ("_id", new BsonString(Email)), Query.EQ("lics._id", new BsonString(serialKey)));
            var result = Customers.Update(query, pull).Ok;

            License.DeleteLicenseFile(serialKey, fileName);
            for (var index = Licenses.Count - 1; index >= 0; index--)
            {
                if (Licenses[index].SerialKey.Equals(serialKey))
                {
                    Licenses.RemoveAt(index);
                    break;
                }
            }

            return result;
        }

        public bool AddLicense(DateTime issueDate, DateTime expirationDate, out License license)
        {
            license = License.Generate(issueDate, expirationDate, Email);

            var query = Query<Customer>.EQ(e => e.Email, Email);
            var update = Update<Customer>.Push<License>(l => l.Licenses, license);
            var result = Customers.Update(query, update).Ok;

            // licenses list is empty in case of new customer
            if (Licenses == null)
                Licenses = new List<License>();

            Licenses.Add(license);

            return result;
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
