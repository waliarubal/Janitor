using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public class FileIssue: IssueBase
    {
        public FileIssue(ScanTargetBase target, ScanAreaBase area, string path)
            : base(target, area, path)
        {

        }

        public override bool Fix()
        {
            IsFixed = FileSystemHelper.Instance.DeleteFile(Details);
            return IsFixed;
        }
    }
}
