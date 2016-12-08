using System;
using NullVoidCreations.Janitor.Shell.Base;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public sealed class ScanTarget : NotificationBase
    {
        string _name, _description, _iconSource;

        #region constructor / destructor

        public ScanTarget(string name)
            : this(name, null, null)
        {

        }

        public ScanTarget(string name, string description)
            : this(name, description, null)
        {

        }

        public ScanTarget(string name, string description, string iconSource)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _name = name;
            _description = description;
            _iconSource = iconSource;
        }

        #endregion

        #region properties

        public string Name
        {
            get { return _name; }
            private set
            {
                if (value == _name)
                    return;

                _name = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description)
                    return;

                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        public string IconSource
        {
            get { return _iconSource; }
            set
            {
                if (value == _iconSource)
                    return;

                _iconSource = value;
                RaisePropertyChanged("IconSource");
            }
        }

        #endregion

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var compareWith = obj as ScanTarget;
            if (compareWith == null)
                return false;

            return GetHashCode() == compareWith.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Target: {0}", Name);
        }
    }
}
