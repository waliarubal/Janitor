using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shell.Core;

namespace NullVoidCreations.Janitor.Shell.Controls
{
    [TemplatePart(Name="PART_Type", Type=typeof(Border))]
    [TemplatePart(Name = "PART_Minimize", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    public class CustomWindow : Window
    {
        Border _title;
        Button _minimize, _close;
        IntPtr _handle;

        public static readonly DependencyProperty HeaderContentProperty, IsMinimizeAllowedProperty, IsCloseAllowedProperty, CloseCommandProperty, MinimizeCommandProperty;

        static CustomWindow()
        {
            HeaderContentProperty = DependencyProperty.Register("HeaderContent", typeof(FrameworkElement), typeof(CustomWindow));
            IsMinimizeAllowedProperty = DependencyProperty.Register("IsMinimizeAllowed", typeof(bool), typeof(CustomWindow));
            IsCloseAllowedProperty = DependencyProperty.Register("IsCloseAllowed", typeof(bool), typeof(CustomWindow));
            CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(CommandBase), typeof(CustomWindow));
            MinimizeCommandProperty = DependencyProperty.Register("MinimizeCommand", typeof(CommandBase), typeof(CustomWindow));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            IsMinimizeAllowed = true;
            IsCloseAllowed = true;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _handle = IntPtr.Zero;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            _handle = NativeApiHelper.Instance.GetWindowHandle(this);
        }

        #region properties

        public IntPtr Handle
        {
            get { return _handle; }
        }

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

        public CommandBase CloseCommand
        {
            get { return (CommandBase)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        public CommandBase MinimizeCommand
        {
            get { return (CommandBase)GetValue(MinimizeCommandProperty); }
            set { SetValue(MinimizeCommandProperty, value); }
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_title != null)
                _title.MouseDown -= new MouseButtonEventHandler(Title_MouseDown);
            if (_minimize != null)
                _minimize.Click -= new RoutedEventHandler(Minimize_Click);
            if (_close != null)
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
            if (MinimizeCommand != null && MinimizeCommand.IsEnabled)
                MinimizeCommand.Execute(this);
            else
                WindowState = WindowState.Minimized;

            e.Handled = true;
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            if (CloseCommand != null && CloseCommand.IsEnabled)
                CloseCommand.Execute(this);
            else
                Close();

            e.Handled = true;
        }
        
    }
}
