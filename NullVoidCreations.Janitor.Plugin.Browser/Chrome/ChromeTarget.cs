using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Plugin.Browser.Chrome
{
    public class ChromeTarget: ScanTargetBase
    {
        public ChromeTarget()
            : base("Google Chrome", new Version("16.12.31.0"), new DateTime(2016, 12, 31))
        {
            IconSource = "/plugin_browser;component/Resources/Chrome.png";

            var areas = new List<ScanAreaBase>()
            {
                new ChromeAreaCache(this)
            };
            Areas = areas;
        }
    }
}
