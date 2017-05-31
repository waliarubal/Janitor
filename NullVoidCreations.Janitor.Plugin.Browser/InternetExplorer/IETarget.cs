using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Plugin.Browser.InternetExplorer
{
    public class IETarget: ScanTargetBase
    {
        public IETarget()
            : base("Internet Explorer", new Version("16.12.31.0"), new DateTime(2016, 12, 31))
        {
            IconSource = "pack://application:,,,/plugin_browser;component/Resources/InternetExplorer.png";

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
