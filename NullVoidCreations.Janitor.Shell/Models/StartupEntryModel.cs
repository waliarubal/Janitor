using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.Models
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

        string _registryKey;

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

        internal bool RemoveFromStartup()
        {
            var removed = false;
            switch(Area)
            {
                case StartupArea.Registry:
                    try
                    {
                        var key = Registry.LocalMachine.OpenSubKey(_registryKey);
                        if (key != null)
                        {
                            key.DeleteValue(Name, false);
                            key.Close();
                            removed = true;
                        }
                    }
                    catch
                    {
                        
                    }
                    break;

                case StartupArea.RegistryUser:
                    try
                    {
                        var key = Registry.LocalMachine.OpenSubKey(_registryKey);
                        if (key != null)
                        {
                            key.DeleteValue(Name, false);
                            key.Close();
                            removed = true;
                        }
                    }
                    catch
                    {
                        
                    }
                    break;

                case StartupArea.StartupDirectory:
                case StartupArea.StartupDirectoryUser:
                    removed = FileSystemHelper.Instance.DeleteFile(Program.FullName);
                    break;
            }

            return removed;
        }

        internal static IEnumerable<StartupEntryModel> GetStartupEntries()
        {
            RegistryKey key;

            // load startup entries from registry
            var subKeyNames = new string[] 
            {
                @"Software\Microsoft\Windows\CurrentVersion\RunOnce",
                @"Software\Microsoft\Windows\CurrentVersion\Run",
                @"Software\Microsoft\Windows\CurrentVersion\RunOnceEx"
            };
            foreach (var subKeyName in subKeyNames)
            {
                key = Registry.LocalMachine.OpenSubKey(subKeyName, false);
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                    {
                        var entry = new StartupEntryModel(key.GetValue(name, string.Empty) as string, StartupEntryModel.StartupArea.Registry);
                        entry.Name = name;
                        entry._registryKey = subKeyName;
                        yield return entry;
                    }

                    key.Close();
                }

                key = Registry.CurrentUser.OpenSubKey(subKeyName, false);
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                    {
                        var entry = new StartupEntryModel(key.GetValue(name, string.Empty) as string, StartupEntryModel.StartupArea.RegistryUser);
                        entry.Name = name;
                        entry._registryKey = subKeyName;
                        yield return entry;
                    }

                    key.Close();
                }
            }

            Func<string, bool> fileInclusionFilter = (path) =>
            {
                return !path.EndsWith("desktop.ini", StringComparison.InvariantCultureIgnoreCase);
            };

            // load startup entries from startup directory
            const string ShellFoldersKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders";
            key = Registry.LocalMachine.OpenSubKey(ShellFoldersKey, false);
            if (key != null)
            {
                var startupDirectory = key.GetValue("Common Startup") as string;
                key.Close();

                if (!string.IsNullOrEmpty(startupDirectory))
                {
                    foreach (var file in new DirectoryWalker(startupDirectory, fileInclusionFilter))
                    {
                        var entry = new StartupEntryModel(file, StartupEntryModel.StartupArea.StartupDirectory);
                        yield return entry;
                    }
                }
            }

            key = Registry.CurrentUser.OpenSubKey(ShellFoldersKey, false);
            if (key != null)
            {
                var startupDirectory = key.GetValue("Startup") as string;
                key.Close();

                if (!string.IsNullOrEmpty(startupDirectory))
                {
                    foreach (var file in new DirectoryWalker(startupDirectory, fileInclusionFilter))
                    {
                        var entry = new StartupEntryModel(file, StartupEntryModel.StartupArea.StartupDirectoryUser);
                        yield return entry;
                    }
                }
            }
        }
    }
}
