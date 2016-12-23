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

        public override IEnumerable<IssueBase> Analyse()
        {
            var paths = new string[]
            {
                KnownPaths.Instance.AppData
            };

            Issues.Clear();
            foreach (var directory in paths)
            {
                foreach (var file in new DirectoryWalker(directory, IncludeFile))
                {
                    var issue = new FileIssue(Target, this, file);
                    Issues.Add(issue);
                    yield return issue;
                }
            }
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
