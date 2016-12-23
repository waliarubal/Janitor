using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.System.System
{
    public class SystemAreaTemporaryFiles: ScanAreaBase
    {
        public SystemAreaTemporaryFiles(ScanTargetBase target)
            : base("Temporary Files", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var paths = new string[]
            {
                KnownPaths.Instance.SystemTempDirectory,
                KnownPaths.Instance.UserTempDirectory
            };

            Issues.Clear();
            foreach (var directory in paths)
            {
                foreach (var file in new DirectoryWalker(directory))
                {
                    var issue = new FileIssue(Target, this, file);
                    Issues.Add(issue);
                    yield return issue;
                }
            }
        }
    }
}
