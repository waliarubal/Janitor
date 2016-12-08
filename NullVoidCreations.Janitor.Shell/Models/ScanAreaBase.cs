using NullVoidCreations.Janitor.Shell.Base;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public abstract class ScanAreaBase : NotificationBase
    {
        string _name;
        ScanTarget _target;

        #region constructor / destructor

        public ScanAreaBase(string name, ScanTarget target)
        {
            _name = name;
            _target = target;
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

        protected ScanTarget Target
        {
            get { return _target; }
        }

        #endregion

        public override int GetHashCode()
        {
            return string.Format("{0}{1}", _target.Name, Name).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Target: {0}, Area: {1}", _target.Name, Name);
        }

        public override bool Equals(object obj)
        {
            var compareWith = obj as ScanAreaBase;
            if (compareWith == null)
                return false;

            return GetHashCode() == compareWith.GetHashCode();
        }
    }
}
