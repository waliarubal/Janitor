using System;
using System.Windows;
using NullVoidCreations.Janitor.Shell.Views;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shell.Core;

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
            
            var mainWindow = new MainView();
            MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
