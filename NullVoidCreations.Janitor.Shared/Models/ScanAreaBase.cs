using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public abstract class ScanAreaBase : NotificationBase
    {
        string _name;
        ScanTargetBase _target;
        readonly List<IssueBase> _issues, _issuesFixed;
        bool _isSelected;

        #region constructor / destructor

        public ScanAreaBase(string name, ScanTargetBase target)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (target == null)
                throw new ArgumentNullException("target");

            _name = name;
            _target = target;
            _issues = new List<IssueBase>();
            _issuesFixed = new List<IssueBase>();
        }

        ~ScanAreaBase()
        {
            _issues.Clear();
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

        protected ScanTargetBase Target
        {
            get { return _target; }
        }

        public List<IssueBase> Issues
        {
            get { return _issues; }
        }

        public List<IssueBase> IssuesFixed
        {
            get { return _issuesFixed; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;

                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        #endregion

        public abstract List<IssueBase> Analyse();

        public List<IssueBase> Fix()
        {
            IssuesFixed.Clear();
            for (var index = Issues.Count - 1; index >= 0; index--)
            {
                var issue = Issues[index];
                if (issue.Fix())
                {
                    Issues.RemoveAt(index);
                    IssuesFixed.Add(issue);
                }
            }

            return IssuesFixed;
        }

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
