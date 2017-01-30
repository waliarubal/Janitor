using System;
using System.Reflection;
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
            ExecutableFile = Assembly.GetExecutingAssembly().Location;

            UpdatesMetadataUrl = new Uri("https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/Updates.dat");
            WebLinksUrl = new Uri("https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/WebLinks.dat");
        }
    }
}
