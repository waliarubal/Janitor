using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Plugin.System.WindowsExplorer
{
    public class ExplorerTarget: ScanTargetBase
    {
        public ExplorerTarget()
            : base("Windows Explorer", new Version("1.0.0.0"), new DateTime(2016, 12, 31))
        {
            IconSource = "/plugin_system;component/Resources/Explorer.png";

            var areas = new List<ScanAreaBase>()
            {
                new ExplorerAreaRecentDocuments(this),
                new ExplorerAreaThumbnailCache(this)
            };
            Areas = areas;
        }
    }
}
