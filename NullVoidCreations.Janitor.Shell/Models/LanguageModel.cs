using NullVoidCreations.Janitor.Shared.Base;
using System.IO;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class LanguageModel: NotificationBase
    {
        public LanguageModel(string fileName)
        {
            Name = Path.GetFileNameWithoutExtension(fileName);
            FileName = fileName;
            IconFileName = Path.Combine(Path.GetDirectoryName(fileName), string.Format("{0}.png", Name));
        }

        #region properties

        public string Name
        {
            get { return GetValue<string>("Name"); }
            private set { this["Name"] = value; }
        }

        public string FileName
        {
            get { return GetValue<string>("FileName"); }
            private set { this["FileName"] = value; }
        }

        public string IconFileName
        {
            get { return GetValue<string>("IconFileName"); }
            private set { this["IconFileName"] = value; }
        }

        #endregion
    }
}
