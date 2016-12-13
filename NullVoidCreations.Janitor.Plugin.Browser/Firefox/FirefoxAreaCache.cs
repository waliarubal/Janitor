using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Browser.Firefox
{
    public class FirefoxAreaCache: ScanAreaBase
    {
        public FirefoxAreaCache(ScanTargetBase target)
            : base("Internet Cache", target)
        {

        }

        public override List<Issue> Analyse()
        {
            var appData = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            var profiles = (Target as FirefoxTarget).Profiles;

            var paths = new List<string>();
            foreach (var profile in profiles)
            {
                paths.Add(Path.Combine(appData, string.Format(@"Local\Mozilla\Firefox\Profiles\{0}\cache2", profile)));
                paths.Add(Path.Combine(appData, string.Format(@"Local\Mozilla\Firefox\Profiles\{0}\jumpListCache", profile)));
            }

            Issues.Clear();
            foreach (var directory in paths)
                foreach (var file in new DirectoryWalker(directory))
                    Issues.Add(new Issue(Target, this, file));

            foreach(var profile in profiles)
            foreach (var file in new DirectoryWalker(Path.Combine(appData, string.Format(@"Local\Mozilla\Firefox\Profiles", profile)), (fileName) => fileName.EndsWith("places.sqlite-wal", StringComparison.InvariantCultureIgnoreCase) || fileName.EndsWith("places.sqlite-shm", StringComparison.InvariantCultureIgnoreCase)))
                Issues.Add(new Issue(Target, this, file));

            return Issues;
        }

        public override List<Issue> Fix()
        {
            return null;
        }
    }
}
