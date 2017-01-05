using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {

            InitializeComponent();
            (DataContext as MainViewModel).View = this;
        }
    }
}
