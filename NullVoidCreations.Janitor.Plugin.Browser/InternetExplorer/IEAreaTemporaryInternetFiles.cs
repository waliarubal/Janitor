using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Browser.InternetExplorer
{
    public class IEAreaTemporaryInternetFiles: ScanAreaBase
    {

        public IEAreaTemporaryInternetFiles(ScanTargetBase target)
            : base("Temporary Internet Files", target)
        {
            
        }

        public override List<IssueBase> Analyse()
        {
            var paths = new string[]
            {
                KnownPaths.Instance.InternetCache,
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\FeedsCache")
            };

            Issues.Clear();
            foreach(var path in paths)
                foreach (var file in new DirectoryWalker(path))
                    Issues.Add(new FileIssue(Target, this, file));

            return Issues;
        }
    }
}
