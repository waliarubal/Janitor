using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Browser.InternetExplorer
{
    public class IEAreaHistory : ScanAreaBase
    {
        public IEAreaHistory(ScanTargetBase target)
            : base("History", target)
        {

        }

        public override List<Issue> Analyse()
        {
            var paths = new string[]
            {
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\Windows\History\History.IE5"),
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\Windows\History\Low\History.IE5"),
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\Internet Explorer\Recovery\Last Active"),
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\Internet Explorer\Recovery\High\Last Active"),
                Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\Internet Explorer\Recovery\High\Active")
            };

            Issues.Clear();
            foreach (var directory in paths)
                foreach (var file in new DirectoryWalker(directory))
                    Issues.Add(new Issue(Target, this, file));

            return Issues;
        }

        public override List<Issue> Fix()
        {
            return null;
        }
    }
}
