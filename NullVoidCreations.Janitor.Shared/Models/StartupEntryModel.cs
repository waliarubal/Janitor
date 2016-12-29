using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shared.Models
{
    public sealed class StartupEntryModel
    {
        public enum StartupArea: byte
        {
            Registry,
            RegistryUser,
            StartupDirectory,
            StartupDirectoryUser
        }

        internal StartupEntryModel(string command, StartupArea area)
        {
            Command = command;
            Area = area;

            string path = null;
            switch(area)
            {
                case StartupArea.RegistryUser:
                case StartupArea.Registry:
                    var startIndex = command.IndexOf('"', 0);
                    var endIndex = command.IndexOf('"', startIndex + 1);
                    if (endIndex > startIndex)
                        path = command.Substring(startIndex + 1, endIndex - startIndex - 1);
                    Program = new FileInfo(path);
                    break;

                case StartupArea.StartupDirectoryUser:
                case StartupArea.StartupDirectory:
                    path = command;
                    Program = new FileInfo(path);
                    Name = Program.Name.Substring(0, Program.Name.Length - Program.Extension.Length);
                    break;

                default:
                    return;
            }

            Size = Program.Length / 1024;
        }

        #region properties

        public string Command
        {
            get;
            private set;
        }

        public StartupArea Area
        {
            get;
            private set;
        }

        public FileInfo Program
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            internal set;
        }

        public decimal Size
        {
            get;
            internal set;
        }

        #endregion

        public override string ToString()
        {
            return Program == null ? string.Empty : Program.Name;
        }

        public override int GetHashCode()
        {
            return Program == null ? -1 : Program.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as StartupEntryModel;
            if (compareTo == null)
                return false;

            return GetHashCode() == compareTo.GetHashCode();
        }

        public void RemoveFromStartup()
        {
            switch(Area)
            {
                case StartupArea.Registry:
                    break;

                case StartupArea.StartupDirectory:
                case StartupArea.StartupDirectoryUser:
                    FileSystemHelper.Instance.DeleteFile(Program.FullName);
                    break;
            }
        }
    }
}
