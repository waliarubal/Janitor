using System;
using System.Windows.Controls;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(AboutView_Loaded);
        }

        void AboutView_Loaded(object sender, EventArgs e)
        {
            Loaded -= new System.Windows.RoutedEventHandler(AboutView_Loaded);

            var viewModel = DataContext as AboutViewModel;
            if (viewModel == null)
                return;

            viewModel.GetTargets.Execute(null);
        }
    }
}
