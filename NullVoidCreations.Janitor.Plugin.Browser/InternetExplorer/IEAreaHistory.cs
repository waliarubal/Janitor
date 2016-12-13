using System;
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
            var pathRoot = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            var paths = new string[]
            {
                Path.Combine(pathRoot, @"Local\Microsoft\Windows\History\History.IE5"),
                Path.Combine(pathRoot, @"Local\Microsoft\Internet Explorer\Recovery\Last Active")
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
