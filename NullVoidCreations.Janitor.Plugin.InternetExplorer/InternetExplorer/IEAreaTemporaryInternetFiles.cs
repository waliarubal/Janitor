using NullVoidCreations.Janitor.Shared.Models;
using System;
using NullVoidCreations.Janitor.Shared.Helpers;
using System.Collections.Generic;

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

        protected override long Analyse()
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            foreach (var file in new DirectoryWalker(directory))
                _files.Add(file);

            return _files.Count;
        }

        protected override long Fix()
        {
            return 0;
        }
    }
}
