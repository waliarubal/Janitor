using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Plugin.Miscellaneous.Utilities
{
    public class UtilitiesTarget: ScanTargetBase
    {
        public UtilitiesTarget()
            : base("Utilities", new Version("2017.3.22.0"), new DateTime(2017, 3, 22))
        {
            IconSource = "/plugin_miscellaneous;component/Resources/Utilities22.png";

            var areas = new List<ScanAreaBase>()
            {
                new UtilitiesAreaAdobeAir(this),
                new UtilitiesAreaAvastAntivirus(this),
                new UtilitiesAreaTeamViewer(this),
                new UtilitiesAreaWindowsDefender(this)
            };
            Areas = areas;
        }
    }
}
