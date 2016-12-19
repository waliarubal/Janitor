using System;
using System.Windows.Controls;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(HomeView_Loaded);
        }

        void HomeView_Loaded(object sender, EventArgs e)
        {
            Loaded -= new System.Windows.RoutedEventHandler(HomeView_Loaded);

            var viewModel = DataContext as HomeViewModel;
            if (viewModel == null)
                return;

            viewModel.LoadData.Execute(null);
        }
    }
}
