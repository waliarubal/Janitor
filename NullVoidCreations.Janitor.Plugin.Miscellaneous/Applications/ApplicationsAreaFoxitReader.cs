using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Miscellaneous.Applications
{
    public class ApplicationsAreaFoxitReader: ScanAreaBase
    {
        public ApplicationsAreaFoxitReader(ScanTargetBase target)
            : base("Foxit Reader", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var path = Path.Combine(KnownPaths.Instance.AppDataRoaming, @"Foxit Software\Foxit Reader\Foxit Cloud");

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
            return path.EndsWith(".log", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
