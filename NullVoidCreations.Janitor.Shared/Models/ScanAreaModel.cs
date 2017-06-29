using NullVoidCreations.Janitor.Shared.Base;
using System.Collections.Generic;
using System.Xml;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public class ScanAreaModel: ScanAreaBase
    {
        public ScanAreaModel(ScanTargetModel target, XmlNode areaXml): base("AreaName", target)
        {
            Name = areaXml.SelectSingleNode("/ScanTarget/ScanAreas/ScanArea/Property[@Name='Name']/@Value").Value;
        }

        public override IEnumerable<IssueBase> Analyse()
        {
            throw new System.NotImplementedException();
        }

    }
}
