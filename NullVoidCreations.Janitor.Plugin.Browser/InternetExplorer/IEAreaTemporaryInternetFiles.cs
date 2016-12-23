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

        public override IEnumerable<IssueBase> Analyse()
        {
            var paths = new string[]
            {
                KnownPaths.Instance.InternetCache,
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\FeedsCache")
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
        }
    }
}
