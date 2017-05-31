using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Plugin.System.System
{
    public class SystemTarget: ScanTargetBase
    {
        public SystemTarget()
            : base("System", new Version("16.12.31.0"), new DateTime(2016, 12, 31))
        {
            IconSource = "pack://application:,,,/plugin_system;component/Resources/System.png";

            var areas = new List<ScanAreaBase>()
            {
                new SystemAreaRecycleBin(this),
                new SystemAreaTemporaryFiles(this),
                new SystemAreaMemoryDump(this),
                new SystemAreaWindowsLogs(this),
                new SystemAreaErrorReporting(this)
            };
            Areas = areas;
        }
    }
}
