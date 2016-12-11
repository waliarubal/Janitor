using System;
using NullVoidCreations.Janitor.Shared.Models;
using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Plugin.Browser.InternetExplorer
{
    public class IETarget: ScanTargetBase
    {
        public IETarget()
            : base("Internet Explorer", new Version("1.0.0.0"), DateTime.Now)
        {
            var areas = new List<ScanAreaBase>();
            areas.Add(new IEAreaTemporaryInternetFiles(this));
            Areas = areas;
        }

    }
}
