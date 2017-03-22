using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.DataStructures;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Miscellaneous.Multimedia
{
    public class MultimediaAreaWindowsMediaPlayer : ScanAreaBase
    {
        public  MultimediaAreaWindowsMediaPlayer(ScanTargetBase target)
            : base("Windows Media Player", target)
        {

        }

        public override IEnumerable<IssueBase> Analyse()
        {
            Func<string, bool> selectAllFiles = (path) => true;
            var paths = new Doublet<string, Func<string, bool>>[]
            {
                Doublet<string, Func<string, bool>>.Create(Path.Combine(KnownPaths.Instance.MyDocumentsDirectory, @"NetworkService\Local Settings\Application Data\Microsoft\Media Player"), selectAllFiles),
                Doublet<string, Func<string, bool>>.Create(Path.Combine(KnownPaths.Instance.MyDocumentsDirectory, @"NetworkService\Local Settings\Microsoft\Media Player"), selectAllFiles)
            };

            Issues.Clear();
            foreach (var pattern in paths)
            {
                foreach (var file in new DirectoryWalker(pattern.First, pattern.Second, false))
                {
                    var issue = new FileIssueModel(Target, this, file);
                    Issues.Add(issue);
                    yield return issue;
                }
            }
        }
    }
}
