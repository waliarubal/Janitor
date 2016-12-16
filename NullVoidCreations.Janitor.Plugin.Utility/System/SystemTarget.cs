using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.System.System
{
    public class SystemTarget: ScanTargetBase
    {
        public SystemTarget()
            : base("System", new Version("1.0.0.0"), DateTime.Now)
        {
            IconSource = "/NullVoidCreations.Janitor.Plugin.System;component/Resources/System.png";

            var areas = new List<ScanAreaBase>()
            {
                
            };
            Areas = areas;
        }
    }
}
