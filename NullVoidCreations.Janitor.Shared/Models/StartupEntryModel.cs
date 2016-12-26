using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public sealed class StartupEntryModel
    {
        readonly string _caption, _command, _description, _location, _name, _settingId, _user, _userSid;

        internal StartupEntryModel(
            string caption, 
            string command, 
            string description, 
            string location, 
            string name, 
            string settingID, 
            string user, 
            string userSID)
        {
            _caption = caption;
            _command = command;
            _description = description;
            _location = location;
            _name = name;
            _settingId = settingID;
            _user = user;
            _userSid = userSID;
        }

        #region properties

        public string Caption
        {
            get { return _caption; }
        }

        public string Command
        {
            get { return _command; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Location
        {
            get { return _location; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string SettingId
        {
            get { return _settingId; }
        }

        public string User
        {
            get { return _user; }
        }

        public string UserSid
        {
            get { return _userSid; }
        }

        #endregion
    }
}
