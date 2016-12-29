using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Plugin.Browser.InternetExplorer
{
    public class IETarget: ScanTargetBase
    {
        public IETarget()
            : base("Internet Explorer", new Version("1.0.0.0"), new DateTime(2016, 12, 31))
        {
            IconSource = "/NullVoidCreations.Janitor.Plugin.Browser;component/Resources/InternetExplorer.png";

            var areas = new List<ScanAreaBase>()
            {
                new IEAreaTemporaryInternetFiles(this),
                new IEAreaHistory(this),
                new IEAreaCookies(this)
            };
            Areas = areas;
        }

    }
}
