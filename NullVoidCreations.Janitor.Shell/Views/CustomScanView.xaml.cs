using System.Windows;
using NullVoidCreations.Janitor.Shell.ViewModels;
using NullVoidCreations.Janitor.Shared.Models;
using System.Collections.ObjectModel;

namespace NullVoidCreations.Janitor.Shell.Views
{
    /// <summary>
    /// Interaction logic for ScanTargetBrowserView.xaml
    /// </summary>
    public partial class CustomScanView
    {
        readonly ObservableCollection<ScanTargetBase> _targets;

        public CustomScanView()
        {
            InitializeComponent();
        }

        public CustomScanView(ObservableCollection<ScanTargetBase> targets): this()
        {
            _targets = targets;
            Loaded += new RoutedEventHandler(CustomScanView_Loaded);
        }

        void CustomScanView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(CustomScanView_Loaded);

            var dataContext = DataContext as CustomScanViewModel;
            dataContext.Targets = _targets;

            e.Handled = true;
        }
    }
}
