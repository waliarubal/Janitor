using System;
using NullVoidCreations.Janitor.Shared.Base;
using System.Xml;
using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public class ScanTargetModel: ScanTargetBase
    {
        public ScanTargetModel(XmlDocument targetXml): base("TargetName", new Version(1, 0, 0, 0), DateTime.Now, "TargetDescription", "IconSource")
        {
            Name = targetXml.SelectSingleNode("/ScanTarget/Property[@Name='Name']/@Value").Value;
            Version = Version.Parse(targetXml.SelectSingleNode("/ScanTarget/Property[@Name='Version']/@Value").Value);
            Date = DateTime.Parse(targetXml.SelectSingleNode("/ScanTarget/Property[@Name='Date']/@Value").Value);
            Description = targetXml.SelectSingleNode("/ScanTarget/Property[@Name='Description']/@Value").Value;
            IconSource = targetXml.SelectSingleNode("/ScanTarget/Property[@Name='IconSource']/@Value").Value;

            var areas = new List<ScanAreaBase>();
            foreach (XmlNode node in targetXml.SelectNodes("/ScanTarget/ScanAreas/ScanArea"))
                areas.Add(new ScanAreaModel(this, node));
            Areas = areas;
        }
    }
}
