using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Commands;

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

        public string Name
        {
            get;
            private set;
        }

        #endregion

        public override string ToString()
        {
            return Name == null ? string.Empty : Name;
        }

        public override int GetHashCode()
        {
            return Name == null ? -1 : Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as StartupEntryModel;
            if (compareTo == null)
                return false;

            return GetHashCode() == compareTo.GetHashCode();
        }

        internal static StartupEntryModel AddEntry(StartupArea area, string name, string command)
        {
            RegistryKey key;
            var entry = new StartupEntryModel(command, area);
            entry.Name = name;

            switch(area)
            {
                case StartupArea.StartupDirectory:
                case StartupArea.StartupDirectoryUser:
                    throw new NotImplementedException();

                case StartupArea.Registry:
                    key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    if (key == null)
                        break;

                    key.SetValue(name, command, RegistryValueKind.String);
                    key.Close();
                    return entry;

                case StartupArea.RegistryUser:
                    key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    if (key == null)
                        break;

                    key.SetValue(name, command, RegistryValueKind.String);
                    key.Close();
                    return entry;
            }

            return null;
        }

        internal static bool RemoveEntry(StartupArea area, string name)
        {
            foreach (var entry in GetStartupEntries())
            {
                if (entry.Area == area && entry.Name == name)
                    return entry.RemoveEntry();
            }

            return false;
        }

        internal bool RemoveEntry()
        {
            var removed = false;
            switch(Area)
            {
                case StartupArea.Registry:
                    try
                    {
                        var key = Registry.LocalMachine.OpenSubKey(_registryKey, true);
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
                        var key = Registry.LocalMachine.OpenSubKey(_registryKey, true);
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
                    removed = FileSystemHelper.Instance.DeleteFile(Command);
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
                        if (string.IsNullOrEmpty(name) || ScheduleSilentRunCommand.SilentRunKey.Equals(name))
                            continue;

                        var entry = new StartupEntryModel(key.GetValue(name, string.Empty, RegistryValueOptions.None) as string, StartupEntryModel.StartupArea.Registry);
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
                        if (string.IsNullOrEmpty(name) || ScheduleSilentRunCommand.SilentRunKey.Equals(name))
                            continue;

                        var entry = new StartupEntryModel(key.GetValue(name, string.Empty, RegistryValueOptions.None) as string, StartupEntryModel.StartupArea.RegistryUser);
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
                var startupDirectory = key.GetValue("Common Startup", string.Empty, RegistryValueOptions.None) as string;
                key.Close();

                if (!string.IsNullOrEmpty(startupDirectory))
                {
                    foreach (var file in new DirectoryWalker(startupDirectory, fileInclusionFilter))
                    {
                        var program = new FileInfo(file);
                        var entry = new StartupEntryModel(file, StartupEntryModel.StartupArea.StartupDirectory);
                        entry.Name = program.Name.Substring(0, program.Name.Length - program.Extension.Length);
                        yield return entry;
                    }
                }
            }

            key = Registry.CurrentUser.OpenSubKey(ShellFoldersKey, false);
            if (key != null)
            {
                var startupDirectory = key.GetValue("Startup", string.Empty, RegistryValueOptions.None) as string;
                key.Close();

                if (!string.IsNullOrEmpty(startupDirectory))
                {
                    foreach (var file in new DirectoryWalker(startupDirectory, fileInclusionFilter))
                    {
                        var program = new FileInfo(file);
                        var entry = new StartupEntryModel(file, StartupEntryModel.StartupArea.StartupDirectoryUser);
                        entry.Name = program.Name.Substring(0, program.Name.Length - program.Extension.Length);
                        yield return entry;
                    }
                }
            }
        }
    }
}
