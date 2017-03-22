using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Miscellaneous.Multimedia
{
    public class MultimediaAreaVlcMediaPlayer: ScanAreaBase
    {
        public MultimediaAreaVlcMediaPlayer(ScanTargetBase target)
            : base("VLC Media Player", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            var path = Path.Combine(KnownPaths.Instance.AppDataRoaming, @"vlc\art\artistalbum");

            Issues.Clear();
            foreach (var file in new DirectoryWalker(path, IncludeFile))
            {
                var issue = new FileIssueModel(Target, this, file);
                Issues.Add(issue);
                yield return issue;
            }
        }

        bool IncludeFile(string path)
        {
            return Path.GetFileNameWithoutExtension(path).Equals("art", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
