using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Browser.Firefox
{
    public class FirefoxAreaSession : ScanAreaBase
    {
        public FirefoxAreaSession(ScanTargetBase target)
            : base("Session", target)
        {

        }

        public override List<Issue> Analyse()
        {
            var profiles = (Target as FirefoxTarget).Profiles;

            var paths = new List<string>();
            foreach (var profile in profiles)
            {
                paths.Add(Path.Combine(KnownPaths.Instance.AppDataRoaming, string.Format(@"Mozilla\Firefox\Profiles\{0}", profile)));
            }

            Issues.Clear();
            foreach(var directory in paths)
                foreach (var file in new DirectoryWalker(directory, IncludeFile))
                    Issues.Add(new Issue(Target, this, file));

            return Issues;
        }

        public override List<Issue> Fix()
        {
            return null;
        }

        bool IncludeFile(string path)
        {
            return path.EndsWith("sessionCheckpoints.json", StringComparison.InvariantCultureIgnoreCase) || 
                path.Contains("sessionstore-backups");
        }
    }
}
