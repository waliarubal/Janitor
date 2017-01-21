using System;
using System.Reflection;
using NullVoidCreations.Janitor.Shared.Helpers;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell
{
    class SharedConstants
    {
        public static readonly string ProductName, InternalName, ExecutableFile, PluginsDirectory, PluginsSearchFilter;
        public static readonly Uri UpdatesMetadataUrl, WebLinksUrl;

        static SharedConstants()
        {
            ProductName = "PC MECHANIC PRO™";
            InternalName = "Janitor";

            PluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";
            PluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            ExecutableFile = Assembly.GetExecutingAssembly().Location;

            UpdatesMetadataUrl = new Uri("https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/Updates.dat");
            WebLinksUrl = new Uri("https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/WebLinks.dat");
        }

        public static string BuyNowUrl
        {
            get { return SettingsManager.Instance["BuyNowUrl"] as string; }
        }

        public static string OfferUrl
        {
            get { return SettingsManager.Instance["OfferUrl"] as string; }
        }
    }
}
