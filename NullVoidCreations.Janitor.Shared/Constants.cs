using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shared
{
    public class Constants
    {
        public static readonly string ProductName, InternalName, ExecutableFile, PluginsDirectory, PluginsSearchFilter;
        public static readonly Uri UpdatesMetadataUrl, WebLinksUrl;
        public static readonly Version ProductVersion;

        static Constants()
        {
            ProductName = "PC MECHANIC PRO™";
            ProductVersion = new Version("1.8.0.0");
            InternalName = "Janitor";

            PluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";
            PluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            ExecutableFile = DesignMode ? string.Empty : Assembly.GetEntryAssembly().Location;

            UpdatesMetadataUrl = new Uri("https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/Updates.dat");
            WebLinksUrl = new Uri("https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/WebLinks.dat");
        }

        public static bool DesignMode
        {
            get { return (bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue); }
        }
    }
}
