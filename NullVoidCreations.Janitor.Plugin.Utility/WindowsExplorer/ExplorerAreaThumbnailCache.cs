using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.System.WindowsExplorer
{
    public class ExplorerAreaThumbnailCache: ScanAreaBase
    {
        public ExplorerAreaThumbnailCache(ScanTargetBase target)
            : base("Thumbnail Cache", target)
        {

        }

        public override List<Issue> Analyse()
        {
            var path = Path.Combine(KnownPaths.Instance.AppDataLocal, @"Microsoft\Windows\Explorer");

            Issues.Clear();
            foreach (var file in new DirectoryWalker(path, IncludeFile))
                Issues.Add(new Issue(Target, this, file));

            return Issues;
        }

        public override List<Issue> Fix()
        {
            return null;
        }

        bool IncludeFile(string path)
        {
            var fileInfo = new FileInfo(path);
            return fileInfo.Name.StartsWith("thumbcache_") && fileInfo.Extension.Equals(".db");
        }
    }
}
