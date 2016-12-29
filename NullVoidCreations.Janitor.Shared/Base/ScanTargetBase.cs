using System;
using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class ScanTargetBase : NotificationBase
    {
        string _name, _description, _iconSource;
        Version _version;
        DateTime _date;
        List<ScanAreaBase> _areas;

        #region constructor / destructor

        public ScanTargetBase(string name, Version version, DateTime date)
            : this(name, version, date, null, null)
        {

        }

        public ScanTargetBase(string name, Version version, DateTime date, string description)
            : this(name, version, date, description, null)
        {

        }

        public ScanTargetBase(string name, Version version, DateTime date, string description, string iconSource)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (version == null)
                throw new ArgumentNullException("version");
            if (date == null)
                throw new ArgumentNullException("date");

            _name = name;
            _version = version;
            _date = date;
            _description = description;
            _iconSource = iconSource;
            _areas = new List<ScanAreaBase>();
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

        public Version Version
        {
            get { return _version; }
            private set
            {
                if (value == _version)
                    return;

                _version = value;
                RaisePropertyChanged("Version");
            }
        }

        public DateTime Date
        {
            get { return _date; }
            private set
            {
                if (value == _date)
                    return;

                _date = value;
                RaisePropertyChanged("Date");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description)
                    return;

                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        /// <summary>
        /// This should point to 22 x 22 image resource.
        /// </summary>
        public string IconSource
        {
            get { return _iconSource; }
            set
            {
                if (value == _iconSource)
                    return;

                _iconSource = value;
                RaisePropertyChanged("IconSource");
            }
        }

        public List<ScanAreaBase> Areas
        {
            get { return _areas; }
            protected set
            {
                if (value == _areas)
                    return;

                _areas = value;
                RaisePropertyChanged("Areas");
            }
        }

        #endregion

        /// <summary>
        /// This method sets selected state of areas previously selected by user and clears list of issues from previous scan.
        /// It is necessary because targets are shared among various scan operations.
        /// </summary>
        public void Reset(bool selectAllAreas)
        {
            if (_areas != null && _areas.Count > 0)
            {
                foreach (var area in _areas)
                {
                    // if area is previously selected, clear its issue list
                    if (area.IsSelected && area.Issues != null)
                        area.Issues.Clear();

                    area.IsSelected = selectAllAreas;
                }
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var compareWith = obj as ScanTargetBase;
            if (compareWith == null)
                return false;

            return GetHashCode() == compareWith.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Target: {0}, Version: {1}.{2}.{3}.{4}, Date: {5}", 
                Name,
                Version.Major,
                Version.Minor,
                Version.Build,
                Version.Revision,
                Date.ToString("MM/dd/yyyy HH:mm:ss"));
        }
    }
}
