using System;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class IssueBase: NotificationBase
    {
        string _details;
        bool _isFixed;
        ScanTargetBase _target;
        ScanAreaBase _area;

        #region constructor / destructor

        public IssueBase(ScanTargetBase target, ScanAreaBase area)
            : this(target, area, null)
        {

        }

        public IssueBase(ScanTargetBase target, ScanAreaBase area, string details)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (area == null)
                throw new ArgumentNullException("area");

            _target = target;
            _area = area;
            _details = details;
        }

        ~IssueBase()
        {
            _target = null;
            _area = null;
        }

        #endregion

        #region properties

        public ScanTargetBase Target
        {
            get { return _target; }
        }

        public ScanAreaBase Area
        {
            get { return _area; }
        }

        public string Details
        {
            get { return _details; }
            protected set
            {
                if (value == _details)
                    return;

                _details = value;
                RaisePropertyChanged("Details");
            }
        }

        public bool IsFixed
        {
            get { return _isFixed; }
            protected set
            {
                if (value == _isFixed)
                    return;

                _isFixed = value;
                RaisePropertyChanged("IsFixed");
            }
        }

        #endregion

        public abstract bool Fix();
    }
}
