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
            IssuesFixed.Clear();
            for (var index = Issues.Count - 1; index >= 0; index--)
            {
                if (FileSystemHelper.Instance.DeleteFile(Issues[index].Details))
                {
                    var issue = Issues[index];
                    Issues.RemoveAt(index);
                    IssuesFixed.Add(issue);
                }
            }

            return IssuesFixed;
        }

        bool IncludeFile(string path)
        {
            return path.EndsWith(".lnk", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
