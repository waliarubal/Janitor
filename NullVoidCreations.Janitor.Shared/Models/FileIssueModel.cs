using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public class FileIssueModel: IssueBase
    {
        public FileIssueModel(ScanTargetBase target, ScanAreaBase area, string path)
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
