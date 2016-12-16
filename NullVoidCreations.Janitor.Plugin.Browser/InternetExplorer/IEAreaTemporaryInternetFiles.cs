using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;
using System.IO;

namespace NullVoidCreations.Janitor.Plugin.Browser.InternetExplorer
{
    public class IEAreaTemporaryInternetFiles: ScanAreaBase
    {

        public IEAreaTemporaryInternetFiles(ScanTargetBase target)
            : base("Temporary Internet Files", target)
        {
            
        }

        public override List<Issue> Analyse()
        {
            var paths = new string[]
            {
                KnownPaths.Instance.InternetCache,
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\FeedsCache")
            };

            Issues.Clear();
            foreach(var path in paths)
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
