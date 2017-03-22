using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Miscellaneous.Utilities
{
    public class UtilitiesAreaAvastAntivirus: ScanAreaBase
    {
        public UtilitiesAreaAvastAntivirus(ScanTargetBase target)
            : base("Avast! Antivirus", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var paths = new string[]
            {
                Path.Combine(KnownPaths.Instance.ProgramDataDirectory, @"AVAST Software\Avast\log"),
                Path.Combine(KnownPaths.Instance.AppDataRoaming, @"AVAST Software\Avast\Cache")
            };

            Issues.Clear();
            foreach (var directory in paths)
            {
                foreach (var file in new DirectoryWalker(directory))
                {
                    var issue = new FileIssueModel(Target, this, file);
                    Issues.Add(issue);
                    yield return issue;
                }
            }
        }

        bool IncludeFile(string path)
        {
            return path.EndsWith(".log", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
