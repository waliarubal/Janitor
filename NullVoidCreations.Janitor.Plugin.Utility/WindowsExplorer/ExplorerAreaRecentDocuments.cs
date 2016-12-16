using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Models;
using NullVoidCreations.Janitor.Shared.Helpers;
using System.IO;

namespace NullVoidCreations.Janitor.Plugin.System.WindowsExplorer
{
    public class ExplorerAreaRecentDocuments: ScanAreaBase
    {
        public ExplorerAreaRecentDocuments(ScanTargetBase target)
            : base("Recent Documents", target)
        {

        }

        public override List<Issue> Analyse()
        {
            var path = Path.Combine(KnownPaths.Instance.AppDataRoaming, @"Microsoft\Windows\Recent");

            Issues.Clear();
            foreach (var file in new DirectoryWalker(path, IncludeFile))
                Issues.Add(new Issue(Target, this, file));

            return Issues;
        }

        public override List<Issue> Fix()
        {
            return null;
        }

        bool IncludeFile(string path)
        {
            return path.EndsWith(".lnk", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
