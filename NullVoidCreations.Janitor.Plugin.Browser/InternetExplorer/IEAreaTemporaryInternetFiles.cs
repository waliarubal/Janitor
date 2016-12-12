using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Browser.InternetExplorer
{
    public class IEAreaTemporaryInternetFiles: ScanAreaBase
    {
        readonly List<string> _files;

        public IEAreaTemporaryInternetFiles(ScanTargetBase target)
            : base("Temporary Internet Files", target)
        {
            _files = new List<string>();
        }

        public override List<Issue> Analyse()
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
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
