using System;
using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class ScanTargetBase : NotificationBase
    {
        #region constructor / destructor

        protected ScanTargetBase(string name, Version version, DateTime date)
            : this(name, version, date, null, null)
        {

        }

        protected ScanTargetBase(string name, Version version, DateTime date, string description)
            : this(name, version, date, description, null)
        {

        }

        protected ScanTargetBase(string name, Version version, DateTime date, string description, string iconSource)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (version == null)
                throw new ArgumentNullException("version");
            if (date == null)
                throw new ArgumentNullException("date");

            Name = name;
            Version = version;
            Date = date;
            Description = description;
            IconSource = iconSource;
            Areas = new List<ScanAreaBase>();
        }

        #endregion

        #region properties

        public string Name
        {
            get { return GetValue<string>("Name"); }
            private set { this["Name"] = value; }
        }

        public Version Version
        {
            get { return GetValue<Version>("Version"); }
            private set { this["Version"] = value; }
        }

        public DateTime Date
        {
            get { return GetValue<DateTime>("Date"); }
            private set { this["Date"] = value; }
        }

        public string Description
        {
            get { return GetValue<string>("Description"); }
            set { this["Description"] = value; }
        }

        /// <summary>
        /// This should point to 22 x 22 image resource.
        /// </summary>
        public string IconSource
        {
            get { return GetValue<string>("IconSource"); }
            set { this["IconSource"] = value; }
        }

        public List<ScanAreaBase> Areas
        {
            get { return GetValue<List<ScanAreaBase>>("Areas"); }
            protected set { this["Areas"] = value; }
        }

        #endregion

        /// <summary>
        /// This method sets selected state of areas previously selected by user and clears list of issues from previous scan.
        /// It is necessary because targets are shared among various scan operations.
        /// </summary>
        public void Reset(bool selectAllAreas)
        {
            if (Areas != null && Areas.Count > 0)
            {
                foreach (var area in Areas)
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
