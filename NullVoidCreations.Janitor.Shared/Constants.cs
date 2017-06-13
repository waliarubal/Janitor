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
            InternalName = "Janitor";
            ProductName = "PC MECHANIC PRO™";
            ProductVersion = new Version("1.8.0.0");

            PluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";
            PluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            ExecutableFile = IsInDesignMode ? string.Empty : Assembly.GetEntryAssembly().Location;

            //UpdatesMetadataUrl = new Uri("https://raw.githubusercontent.com/jainvikas1197/JanitorUpdates/master/Updates.dat");
            WebLinksUrl = new Uri("https://raw.githubusercontent.com/jainvikas1197/JanitorUpdates/master/WebLinks.dat");
        }

        public static bool IsInDesignMode
        {
            get { return (bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue); }
        }
    }
}
