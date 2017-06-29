using System;
using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class ScanAreaBase : NotificationBase
    {
        #region constructor / destructor

        protected ScanAreaBase(string name, ScanTargetBase target)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (target == null)
                throw new ArgumentNullException("target");

            Name = name;
            Target = target;
            Issues = new List<IssueBase>();
            IssuesFixed = new List<IssueBase>();
        }

        ~ScanAreaBase()
        {
            Issues.Clear();
        }

        #endregion

        #region properties

        public string Name
        {
            get { return GetValue<string>("Name"); }
            protected set { this["Name"] = value; }
        }

        protected ScanTargetBase Target
        {
            get { return GetValue<ScanTargetBase>("Target"); }
            private set { this["Target"] = value; }
        }

        public List<IssueBase> Issues
        {
            get { return GetValue<List<IssueBase>>("Issues"); }
            private set { this["Issues"] = value; }
        }

        public List<IssueBase> IssuesFixed
        {
            get { return GetValue<List<IssueBase>>("IssuesFixed"); }
            private set { this["IssuesFixed"] = value; }
        }

        public bool IsSelected
        {
            get { return GetValue<bool>("IsSelected"); }
            set { this["IsSelected"] = value; }
        }

        #endregion

        public abstract IEnumerable<IssueBase> Analyse();

        public IEnumerable<IssueBase> Fix()
        {
            IssuesFixed.Clear();
            for (var index = Issues.Count - 1; index >= 0; index--)
            {
                var issue = Issues[index];
                if (issue.Fix())
                {
                    Issues.RemoveAt(index);
                    IssuesFixed.Add(issue);

                    yield return issue;
                }
            }
        }

        public override int GetHashCode()
        {
            return string.Format("{0}{1}", Target.Name, Name).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Target: {0}, Area: {1}", Target.Name, Name);
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
