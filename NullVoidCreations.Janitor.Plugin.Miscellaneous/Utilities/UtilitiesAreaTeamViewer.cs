using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Miscellaneous.Utilities
{
    public class UtilitiesAreaTeamViewer: ScanAreaBase
    {
        public UtilitiesAreaTeamViewer(ScanTargetBase target)
            : base("TeamViewer", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var path = Path.Combine(KnownPaths.Instance.AppDataRoaming, @"TeamViewer");

            Issues.Clear();
            foreach (var file in new DirectoryWalker(path, IncludeFile))
            {
                var issue = new FileIssueModel(Target, this, file);
                Issues.Add(issue);
                yield return issue;
            }
        }

        bool IncludeFile(string path)
        {
            return path.EndsWith("connections.txt", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
