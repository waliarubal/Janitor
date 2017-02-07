using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shared
{
    public class Constants
    {
        public static readonly string ProductName, ProductTagLine, InternalName, ExecutableFile, PluginsDirectory, PluginsSearchFilter, SupportPhone, SupportEmail;
        public static readonly Uri UpdatesMetadataUrl, WebLinksUrl;
        public static readonly Version ProductVersion;

        static Constants()
        {
            InternalName = "Janitor";
            ProductName = "WinDoc";
            ProductTagLine = "Your Computer's Family Doctor";
            ProductVersion = new Version("1.8.0.0");
            SupportPhone = "+91 99288 93416";
            SupportEmail = "walia.rubal@gmail.com";

            PluginsSearchFilter = "NullVoidCreations.Janitor.Plugin.*.dll";
            PluginsDirectory = KnownPaths.Instance.ApplicationDirectory;
            ExecutableFile = IsInDesignMode ? string.Empty : Assembly.GetEntryAssembly().Location;

            UpdatesMetadataUrl = new Uri("https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/Updates.dat");
            WebLinksUrl = new Uri("https://raw.githubusercontent.com/waliarubal/JanitorUpdates/master/WebLinks.dat");
        }

        #region properties

        public static bool IsInDesignMode
        {
            get { return (bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue); }
        }

        public static bool IsAdministrator
        {
            get
            {
                var identity = WindowsIdentity.GetCurrent();
                if (identity != null)
                {
                    var principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }

                return false;
            }
        }

        public bool IsUacSupported
        {
            get { return Environment.OSVersion.Version.Major >= 6; }
        }

        #endregion
    }
}
