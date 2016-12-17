using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class ScanStatusModel: NotificationBase
    {
        public ScanStatusModel(ScanTargetBase target, ScanAreaBase area, bool isRunning)
        {
            Target = target;
            Area = area;
            IsRunning = isRunning;
            ProgressMax = 1;
        }

        #region properties

        public bool IsRunning
        {
            get;
            private set;
        }

        public bool IsHavingIssues
        {
            get { return IssueCount > 0; }
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
