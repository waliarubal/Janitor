using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.System.System
{
    public class SystemAreaMemoryDump : ScanAreaBase
    {
        public SystemAreaMemoryDump(ScanTargetBase target)
           : base("Memory Dumps", target)
        {

        }

        public override List<IssueBase> Analyse()
        {
            var paths = new string[]
            {
                KnownPaths.Instance.AppData
            };

            Issues.Clear();
            foreach (var path in paths)
                foreach (var file in new DirectoryWalker(path, IncludeFile))
                    Issues.Add(new FileIssue(Target, this, file));

            return Issues;
        }

        bool IncludeFile(string path)
        {
            var result = true;
            try
            {
                return new FileInfo(path).Extension.Equals(".dmp");
            }
            catch
            {
                result = false;
                // TODO: handle path too long exception
            }

            return result;
        }
    }
}
