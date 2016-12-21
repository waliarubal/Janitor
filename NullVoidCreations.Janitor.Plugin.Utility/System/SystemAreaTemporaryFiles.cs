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

        public override List<Issue> Analyse()
        {
            var paths = new string[]
            {
                KnownPaths.Instance.SystemTempDirectory,
                KnownPaths.Instance.UserTempDirectory
            };

            Issues.Clear();
            foreach (var path in paths)
                foreach (var file in new DirectoryWalker(path))
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
    }
}
