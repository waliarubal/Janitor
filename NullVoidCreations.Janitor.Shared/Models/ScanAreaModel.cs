using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public class ScanAreaModel: ScanAreaBase
    {
        List<string> _filters;

        public ScanAreaModel(ScanTargetModel target, XmlNode areaXml): base("AreaName", target)
        {
            Name = areaXml.SelectSingleNode("/ScanTarget/ScanAreas/ScanArea/Property[@Name='Name']/@Value").Value;

            var paths = new List<ScanPathModel>();
            foreach (XmlNode pathNode in areaXml.SelectNodes("/ScanTarget/ScanAreas/ScanArea/ScanPaths/Path"))
                paths.Add(new ScanPathModel(pathNode));
            Paths = paths;
        }

        #region properties

        public IEnumerable<ScanPathModel> Paths
        {
            get { return GetValue<IEnumerable<ScanPathModel>>("Paths"); }
            private set { this["Paths"] = value; }
        }

        #endregion

        bool IncludeFile(string path)
        {
            if (_filters == null || _filters.Count == 0)
                return true;

            foreach (var filter in _filters)
                if (Regex.IsMatch(path, filter))
                    return true;

            return false;
        }

        public override IEnumerable<IssueBase> Analyse()
        {
            foreach (var path in Paths)
            {
                if (path.Type == PathType.Directory)
                {
                    _filters = path.Filters;
                    foreach (var file in new DirectoryWalker(path.FullName, path.Recursive))
                    {
                        var issue = new FileIssueModel(Target, this, file);
                        Issues.Add(issue);
                        yield return issue;
                    }
                    _filters = null;
                }
            }
        }
    }
}
