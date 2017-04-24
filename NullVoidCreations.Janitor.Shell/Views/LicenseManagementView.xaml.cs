using System.Windows;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Views
{
    /// <summary>
    /// Interaction logic for LicenseManagementView.xaml
    /// </summary>
    public partial class LicenseManagementView
    {
        public LicenseManagementView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(LicenseManagementView_Loaded);
        }

        void LicenseManagementView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(LicenseManagementView_Loaded);
            (DataContext as LicenseManagemntViewModel).Refresh.Execute(null);
        }
    }
}
