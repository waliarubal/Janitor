using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.System.System
{
    public class SystemAreaRecycleBin: ScanAreaBase
    {
        public SystemAreaRecycleBin(ScanTargetBase target)
            : base("Recycle Bin", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var paths = new List<string>();
            foreach (var drive in Environment.GetLogicalDrives())
                paths.Add(Path.Combine(drive, "$Recycle.Bin"));

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
    }
}
