using System.Collections.Generic;
using System.Xml;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public enum PathType: byte
    {
        Directory,
        File
    }

    public class ScanPathModel: NotificationBase
    {
        public ScanPathModel(XmlNode pathXml)
        {
            FullName = pathXml.SelectSingleNode("//Path/Property[@Name='FullName']/@Value").Value;
            Type = (PathType)byte.Parse(pathXml.SelectSingleNode("//Path/Property[@Name='Type']/@Value").Value);
            Recursive = bool.Parse(pathXml.SelectSingleNode("//Path/Property[@Name='Recursive']/@Value").Value);

            Filters = new List<string>();
            foreach (XmlNode node in pathXml.SelectNodes("//Path/Filters/Filter"))
                Filters.Add(node.InnerText);
        }

        #region properties

        public string FullName
        {
            get { return GetValue<string>("FullName"); }
            private set { this["FullName"] = value; }
        }

        public string ExpandedFullName
        {
            get { return KnownPaths.Instance.ExpandPath(FullName); }
        }

        public PathType Type
        {
            get { return GetValue<PathType>("Type"); }
            private set { this["Type"] = value; }
        }

        public bool Recursive
        {
            get { return GetValue<bool>("Recursive"); }
            private set { this["Recursive"] = value; }
        }

        public List<string> Filters
        {
            get { return GetValue<List<string>>("Filters"); }
            private set { this["Filters"] = value; }
        }

        #endregion
    }
}
