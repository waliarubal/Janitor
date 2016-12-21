using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Browser.InternetExplorer
{
    public class IEAreaCookies : ScanAreaBase
    {
        public IEAreaCookies(ScanTargetBase target)
            : base("Cookies", target)
        {

        }

        public override List<Issue> Analyse()
        {
            var paths = new string[]
            {
                Path.Combine(KnownPaths.Instance.AppDataRoaming, @"Microsoft\Windows\Cookies"),
                Path.Combine(KnownPaths.Instance.AppDataLocalLow, @"Microsoft\Internet Explorer\DOMStore")
            };

            Issues.Clear();
            foreach (var directory in paths)
                foreach (var file in new DirectoryWalker(directory))
                    Issues.Add(new Issue(Target, this, file));

            return Issues;
        }

        public override List<Issue> Fix()
        {
            IssuesFixed.Clear();
            for (var index = Issues.Count - 1; index >= 0; index--)
            {
                if (FileSystemHelper.Instance.DeleteFile(Issues[index].Details))
                {
                    var issue = Issues[index];
                    Issues.RemoveAt(index);
                    IssuesFixed.Add(issue);
                }
            }

            return IssuesFixed;
        }
    }
}
