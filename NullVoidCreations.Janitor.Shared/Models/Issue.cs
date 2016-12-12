using System;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public class Issue: NotificationBase
    {
        string _details;
        ScanTargetBase _target;
        ScanAreaBase _area;

        #region constructor / destructor

        public Issue(ScanTargetBase target, ScanAreaBase area)
            : this(target, area, null)
        {

        }

        public Issue(ScanTargetBase target, ScanAreaBase area, string details)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (area == null)
                throw new ArgumentNullException("area");

            _target = target;
            _area = area;
            _details = details;
        }

        ~Issue()
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
            set
            {
                if (value == _details)
                    return;

                _details = value;
                RaisePropertyChanged("Details");
            }
        }

        #endregion
    }
}
