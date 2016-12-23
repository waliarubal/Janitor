using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.System.System
{
    public class SystemAreaWindowsLogs : ScanAreaBase
    {
        public SystemAreaWindowsLogs(ScanTargetBase target)
            : base("Windows Log Files", target)
        {

        }

        public override List<IssueBase> Analyse()
        {
            object[][] paths = new object[][]
            {
                new object[]{ KnownPaths.Instance.WindowsDirectory, false },
                new object[]{ Path.Combine(KnownPaths.Instance.WindowsDirectory, @"SoftwareDistribution"), true },
                new object[]{ Path.Combine(KnownPaths.Instance.WindowsDirectory, @"Logs"), true },
                new object[]{ Path.Combine(KnownPaths.Instance.WindowsDirectory, @"Microsoft.NET"), true },
                new object[]{ Path.Combine(KnownPaths.Instance.System32Directory, @"config\systemprofile\AppData"), true },
                new object[]{ Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft"), true }
            };

            Issues.Clear();
            foreach (var path in paths)
                foreach (var file in new DirectoryWalker(path[0] as string, IncludeFile, (bool)path[1]))
                    Issues.Add(new FileIssue(Target, this, file));

            return Issues;
        }

        bool IncludeFile(string path)
        {
            return new FileInfo(path).Extension.Equals(".log");
        }
    }
}
