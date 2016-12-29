using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Plugin.Browser.Chrome
{
    public class ChromeTarget: ScanTargetBase
    {
        public ChromeTarget()
            : base("Google Chrome", new Version("1.0.0.0"), new DateTime(2016, 12, 31))
        {
            IconSource = "/NullVoidCreations.Janitor.Plugin.Browser;component/Resources/Chrome.png";

            var areas = new List<ScanAreaBase>()
            {
                new ChromeAreaCache(this)
            };
            Areas = areas;
        }
    }
}
