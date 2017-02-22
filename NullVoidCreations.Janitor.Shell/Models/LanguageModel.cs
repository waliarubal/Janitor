using NullVoidCreations.Janitor.Shared.Base;
using System.IO;

namespace NullVoidCreations.Janitor.Shell.Models
{
    public class LanguageModel: NotificationBase
    {
        string _name, _fileName, _iconFileName;

        public LanguageModel(string fileName)
        {
            Name = Path.GetFileNameWithoutExtension(fileName);
            FileName = fileName;
            IconFileName = Path.Combine(Path.GetDirectoryName(fileName), string.Format("{0}.png", Name));
        }

        #region properties

        public string Name
        {
            get { return _name; }
            private set
            {
                if (value == _name)
                    return;

                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string FileName
        {
            get { return _fileName; }
            private set
            {
                if (value == _fileName)
                    return;

                _fileName = value;
                RaisePropertyChanged("FileName");
            }
        }

        public string IconFileName
        {
            get { return _iconFileName; }
            private set
            {
                if (value == _iconFileName)
                    return;

                _iconFileName = value;
                RaisePropertyChanged("IconFileName");
            }
        }

        #endregion
    }
}
