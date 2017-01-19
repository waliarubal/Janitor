using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class ScanStatusModel: NotificationBase
    {
        public ScanStatusModel(ScanTargetBase target, ScanAreaBase area, bool isAnalysing, bool isFixing, bool isExecuting)
        {
            Target = target;
            Area = area;
            IsAnalysing = isAnalysing;
            IsFixing = isFixing;
            IsExecuting = isExecuting;
            ProgressMax = 1;
        }

        #region properties

        public bool IsExecuting
        {
            get;
            private set;
        }

        public bool IsAnalysing
        {
            get;
            private set;
        }

        public bool IsFixing
        {
            get;
            private set;
        }

        public int TargetScanned
        {
            get;
            set;
        }

        public int AreaScanned
        {
            get;
            set;
        }

        public long IssueCount
        {
            get;
            set;
        }

        public bool IsHavingIssues
        {
            get { return IsAnalysing && IssueCount > 0; }
        }

        public int ProgressMax
        {
            get;
            set;
        }

        public int ProgressMin
        {
            get;
            set;
        }

        public int ProgressCurrent
        {
            get;
            set;
        }

        public ScanTargetBase Target
        {
            get;
            private set;
        }

        public ScanAreaBase Area
        {
            get;
            private set;
        }

        #endregion
    }
}
