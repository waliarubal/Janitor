using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.DataStructures;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Miscellaneous.Multimedia
{
    public class MultimediaAreaSteam: ScanAreaBase
    {
        public MultimediaAreaSteam(ScanTargetBase target)
            : base("Steam", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var paths = new Doublet<string, Func<string, bool>>[]
            {
                Doublet<string, Func<string, bool>>.Create(Path.Combine(KnownPaths.Instance.ProgramFilesDirectory, @"Steam"), (path) => path.EndsWith(".log", StringComparison.InvariantCultureIgnoreCase)),
                Doublet<string, Func<string, bool>>.Create(Path.Combine(KnownPaths.Instance.ProgramFilesDirectory, @"Steam\Logs"), (path) => path.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
            };

            Issues.Clear();
            foreach (var pattern in paths)
            {
                foreach (var file in new DirectoryWalker(pattern.First, pattern.Second, false))
                {
                    var issue = new FileIssueModel(Target, this, file);
                    Issues.Add(issue);
                    yield return issue;
                }
            }
        }
    }
}
