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
            return null;
        }
    }
}
