using System;
using System.Windows;
using NullVoidCreations.Janitor.Shell.Views;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shell.Core;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SettingsManager.Instance.LoadArguments(e.Args);

            // process command line arguments for key generation
            if (SettingsManager.Instance.CommandLineArguments.ContainsKey("GenerateKey") &&
                SettingsManager.Instance.CommandLineArguments.ContainsKey("Email") &&
                SettingsManager.Instance.CommandLineArguments.ContainsKey("Days") &&
                SettingsManager.Instance.CommandLineArguments.ContainsKey("Path"))
            {
                var email = SettingsManager.Instance.CommandLineArguments["Email"];
                var path = SettingsManager.Instance.CommandLineArguments["Path"];
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(path))
                    goto CLOSE_APP;

                int days;
                if (!Int32.TryParse(SettingsManager.Instance.CommandLineArguments["Days"], out days))
                    goto CLOSE_APP;

                var licenseKey = LicenseManager.Instance.GenerateLicenseKey(email, days);
                if (File.Exists(path) && !FileSystemHelper.Instance.DeleteFile(path))
                    goto CLOSE_APP;

                File.WriteAllText(path, licenseKey);

            CLOSE_APP:
                Shutdown(0);
                return;
            }
            
            var mainWindow = new MainView();
            MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
