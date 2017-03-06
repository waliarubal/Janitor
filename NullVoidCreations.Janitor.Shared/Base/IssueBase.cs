using System;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class IssueBase: NotificationBase
    {

        #region constructor / destructor

        protected IssueBase(ScanTargetBase target, ScanAreaBase area)
            : this(target, area, null)
        {

        }

        protected IssueBase(ScanTargetBase target, ScanAreaBase area, string details)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (area == null)
                throw new ArgumentNullException("area");

            Target = target;
            Area = area;
            Details = details;
        }

        ~IssueBase()
        {
            Target = null;
            Area = null;
        }

        #endregion

        #region properties

        public ScanTargetBase Target
        {
            get { return GetValue<ScanTargetBase>("Target"); }
            private set { this["Target"] = value; }
        }

        public ScanAreaBase Area
        {
            get { return GetValue<ScanAreaBase>("Area"); }
            private set { this["Area"] = value; }
        }

        public string Details
        {
            get { return GetValue<string>("Details"); }
            protected set { this["Details"] = value; }
        }

        public bool IsFixed
        {
            get { return GetValue<bool>("IsFixed"); }
            protected set { this["IsFixed"] = value; }
        }

        #endregion

        public abstract bool Fix();
    }
}
