using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Browser.Firefox
{
    public class FirefoxAreaCache : ScanAreaBase
    {
        public FirefoxAreaCache(ScanTargetBase target)
            : base("Internet Cache", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var profiles = (Target as FirefoxTarget).Profiles;

            var paths = new List<string>();
            foreach (var profile in profiles)
            {
                paths.Add(Path.Combine(KnownPaths.Instance.AppDataLocal, string.Format(@"Mozilla\Firefox\Profiles\{0}\cache2", profile)));
                paths.Add(Path.Combine(KnownPaths.Instance.AppDataLocal, string.Format(@"Mozilla\Firefox\Profiles\{0}\jumpListCache", profile)));
            }

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

            foreach (var profile in profiles)
            {
                foreach (var file in new DirectoryWalker(Path.Combine(KnownPaths.Instance.AppDataLocal, string.Format(@"Mozilla\Firefox\Profiles", profile)), IncludeFile))
                {
                    var issue = new FileIssue(Target, this, file);
                    Issues.Add(issue);
                    yield return issue;
                }
            }
        }

        bool IncludeFile(string path)
        {
            return path.EndsWith("places.sqlite-wal", StringComparison.InvariantCultureIgnoreCase) ||
                path.EndsWith("places.sqlite-shm", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
