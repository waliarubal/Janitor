using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NullVoidCreations.Janitor.Shell.Controls
{
    [TemplatePart(Name="PART_Type", Type=typeof(Border))]
    [TemplatePart(Name = "PART_Minimize", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    public class CustomWindow : Window
    {
        Border _title;
        Button _minimize, _close;

        public static readonly DependencyProperty HeaderContentProperty, IsMinimizeAllowedProperty, IsCloseAllowedProperty;

        static CustomWindow()
        {
            HeaderContentProperty = DependencyProperty.Register("HeaderContent", typeof(FrameworkElement), typeof(CustomWindow));
            IsMinimizeAllowedProperty = DependencyProperty.Register("IsMinimizeAllowed", typeof(bool), typeof(CustomWindow));
            IsCloseAllowedProperty = DependencyProperty.Register("IsCloseAllowed", typeof(bool), typeof(CustomWindow));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            IsMinimizeAllowed = true;
            IsCloseAllowed = true;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        #region properties

        public FrameworkElement HeaderContent
        {
            get { return (FrameworkElement)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public bool IsMinimizeAllowed
        {
            get { return (bool)GetValue(IsMinimizeAllowedProperty); }
            set { SetValue(IsMinimizeAllowedProperty, value); }
        }

        public bool IsCloseAllowed
        {
            get { return (bool)GetValue(IsCloseAllowedProperty); }
            set { SetValue(IsCloseAllowedProperty, value); }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _title = Template.FindName("PART_Title", this) as Border;
            _minimize = Template.FindName("PART_Minimize", this) as Button;
            _close = Template.FindName("PART_Close", this) as Button;

            _title.MouseDown += new MouseButtonEventHandler(Title_MouseDown);
            _minimize.Click += new RoutedEventHandler(Minimize_Click);
            _close.Click += new RoutedEventHandler(Close_Click);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _title.MouseDown -= new MouseButtonEventHandler(Title_MouseDown);
            _minimize.Click -= new RoutedEventHandler(Minimize_Click);
            _close.Click -= new RoutedEventHandler(Close_Click);

            base.OnClosing(e);
        }

        void Title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
            e.Handled = true;
        }
        
    }
}
