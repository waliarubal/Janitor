using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class ScanProgressEventArgs: NotificationBase
    {
        public ScanProgressEventArgs(ScanTargetBase target, ScanAreaBase area, bool isRunning)
        {
            Target = target;
            Area = area;
            IsRunning = isRunning;
        }

        #region properties

        public bool IsRunning
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
