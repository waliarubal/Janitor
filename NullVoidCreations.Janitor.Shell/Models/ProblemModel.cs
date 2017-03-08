using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public enum ProblemType: byte
    {
        IsUnlicensed,
        IsHavingIssues,
        IsHavingPluginUpdatesAvailable,
        IsHavingUpdatesAvailable
    }

    public class ProblemModel: NotificationBase
    {
        public ProblemModel(ProblemType type)
        {
            Type = type;
            switch (Type)
            {
                case ProblemType.IsUnlicensed:
                    Title = "Product Not Activated";
                    Message = "Maximum cleaning is not ensured as the product is not activated. An activated product finds and fixes even the newest issues with improved speed.";
                    break;

                case ProblemType.IsHavingIssues:
                    Title = "Issues Found";
                    Message = "One or more issues are found during a computer scan which need your attention. Issues hamper system performance so they must be fixed as soon as possible.";
                    break;

                case ProblemType.IsHavingPluginUpdatesAvailable:
                    Title = "Scan Target plugins are not up to date";
                    Message = "Their are updates available for scan targets plugins. Scan targets look for issues in specific computer areas and fix them, update them to ensure maximum cleaning.";
                    break;

                case ProblemType.IsHavingUpdatesAvailable:
                    Title = "Program is not up to date";
                    Message = "A new version of program is available. Program update contain patches for discovered issues, always keep the mechanic updated to ensure maximum system performance.";
                    break;
            }
        }

        public ProblemModel(ProblemType type, string title, string message)
        {
            Type = type;
            Title = title;
            Message = message;
        }

        #region properties

        public string Title
        {
            get { return GetValue<string>("Title"); }
            set { this["Title"] = value; }
        }

        public string Message
        {
            get { return GetValue<string>("Message"); }
            set { this["Message"] = value; }
        }

        public ProblemType Type
        {
            get { return GetValue<ProblemType>("Type"); }
            private set { this["Type"] = value; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}: {1}", Type, Title);
        }
    }
}
