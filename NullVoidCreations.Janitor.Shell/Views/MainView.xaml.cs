using NullVoidCreations.Janitor.Shell.Core;
using NullVoidCreations.Janitor.Shell.ViewModels;

namespace NullVoidCreations.Janitor.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView: ISignalObserver
    {
        readonly MainViewModel _viewModel;

        public MainView()
        {
            InitializeComponent();
            _viewModel = DataContext as MainViewModel;
            SignalHost.Instance.AddObserver(this);
        }

        protected override void OnClosed(System.EventArgs e)
        {
            SignalHost.Instance.RemoveObserver(this);
            base.OnClosed(e);
        }

        public void Update(ISignalObserver sender, Signal code, params object[] data)
        {
            switch(code)
            {
                case Signal.CloseTriggered:
                    _viewModel.Close.Execute(this);
                    break;
            }
        }
    }
}
