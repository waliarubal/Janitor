using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;
using System;

namespace NullVoidCreations.Janitor.Plugin.Browser.Chrome
{
    public class ChromeAreaCache: ScanAreaBase
    {
        public ChromeAreaCache(ScanTargetBase target)
            : base("Internet Cache", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var paths = new string[]
            {
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Google\Chrome\User Data\Default\Cache"),
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Google\Chrome\User Data\Default\GPUCache"),
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Google\Chrome\User Data\Default\Service Worker\Database"),
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Google\Chrome\User Data\Default\Service Worker\ScriptCache")
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

            var path = Path.Combine(KnownPaths.Instance.AppDataLocal, @"Google\Chrome\User Data\Default");
            foreach (var file in new DirectoryWalker(path, IncludeFile, false))
            {
                var issue = new FileIssue(Target, this, file);
                Issues.Add(issue);
                yield return issue;
            }
        }

        bool IncludeFile(string path)
        {
            return path.EndsWith("-journal", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
