using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class ScanProgress: NotificationBase
    {
        ScanTargetBase _activeTarget;
        ScanAreaBase _activeArea;
        long _issueCount;
        int _progress, _progressMax, _progressMin;

        #region properties

        public long IssueCount
        {
            get { return _issueCount; }
            set
            {
                if (value == _issueCount)
                    return;

                _issueCount = value;
                RaisePropertyChanged("IssueCount");
            }
        }

        public ScanTargetBase ActiveTarget
        {
            get { return _activeTarget; }
            set
            {
                if (value == _activeTarget)
                    return;

                _activeTarget = value;
                RaisePropertyChanged("ActiveTarget");
            }
        }

        public ScanAreaBase ActiveArea
        {
            get { return _activeArea; }
            set
            {
                if (value == _activeArea)
                    return;

                _activeArea = value;
                RaisePropertyChanged("ActiveArea");
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (value == _progress)
                    return;

                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        public int ProgressMax
        {
            get { return _progressMax; }
            set
            {
                if (value == _progressMax)
                    return;

                _progressMax = value;
                RaisePropertyChanged("ProgressMax");
            }
        }

        public int ProgressMin
        {
            get { return _progressMin; }
            set
            {
                if (value == _progressMin)
                    return;

                _progressMin = value;
                RaisePropertyChanged("ProgressMin");
            }
        }

        #endregion
    }
}
