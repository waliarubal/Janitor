using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.System.WindowsExplorer
{
    public class ExplorerAreaRecentDocuments: ScanAreaBase
    {
        public ExplorerAreaRecentDocuments(ScanTargetBase target)
            : base("Recent Documents", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var path = Path.Combine(KnownPaths.Instance.AppDataRoaming, @"Microsoft\Windows\Recent");

            Issues.Clear();
            foreach (var file in new DirectoryWalker(path, IncludeFile))
            {
                var issue = new FileIssue(Target, this, file);
                Issues.Add(issue);
                yield return issue;
            }
        }

        bool IncludeFile(string path)
        {
            return path.EndsWith(".lnk", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
