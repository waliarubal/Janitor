using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NullVoidCreations.Janitor.Shell.Controls
{
    [TemplatePart(Name = "PART_Day", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Month", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Year", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Hour", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Minute", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Second", Type = typeof(TextBox))]
    public class DateTimeBox : Control
    {
        public static readonly DependencyProperty DateProperty, IsTimeProperty;

        TextBox _day, _month, _year, _hour, _minute, _second;
        bool _isInitialized;
        readonly Regex _regex;

        static DateTimeBox()
        {
            DateProperty = DependencyProperty.Register("Date", typeof(DateTime), typeof(DateTimeBox));
            IsTimeProperty = DependencyProperty.Register("IsTime", typeof(bool), typeof(DateTimeBox));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeBox), new FrameworkPropertyMetadata(typeof(DateTimeBox)));
        }

        public DateTimeBox()
        {
            _regex = new Regex("[^0-9]+");
        }

        #region properties

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set 
            { 
                SetValue(DateProperty, value);
                SetDate(value);
            }
        }

        public bool IsTime
        {
            get { return (bool)GetValue(IsTimeProperty); }
            set { SetValue(IsTimeProperty, value); }
        }

        #endregion

        bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }

        void SetDate(DateTime date)
        {
            if (!_isInitialized)
                return;

            _day.Text = date.Day.ToString("00");
            _month.Text = date.Month.ToString("00");
            _year.Text = date.Year.ToString("0000");
            _hour.Text = date.Hour.ToString("00");
            _minute.Text = date.Minute.ToString("00");
            _second.Text = date.Second.ToString("00");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _day = Template.FindName("PART_Day", this) as TextBox;
            _month = Template.FindName("PART_Month", this) as TextBox;
            _year = Template.FindName("PART_Year", this) as TextBox;
            _hour = Template.FindName("PART_Hour", this) as TextBox;
            _minute = Template.FindName("PART_Minute", this) as TextBox;
            _second = Template.FindName("PART_Second", this) as TextBox;

            _hour.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(Text_PreviewTextInput);
            _minute.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(Text_PreviewTextInput);
            _second.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(Text_PreviewTextInput);
            _day.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(Text_PreviewTextInput);
            _month.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(Text_PreviewTextInput);
            _year.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(Text_PreviewTextInput);

            _hour.GotFocus += new RoutedEventHandler(Text_GotFocus);
            _minute.GotFocus += new RoutedEventHandler(Text_GotFocus);
            _second.GotFocus += new RoutedEventHandler(Text_GotFocus);
            _day.GotFocus += new RoutedEventHandler(Text_GotFocus);
            _month.GotFocus += new RoutedEventHandler(Text_GotFocus);
            _year.GotFocus += new RoutedEventHandler(Text_GotFocus);

            _hour.LostFocus += new RoutedEventHandler(Hour_LostFocus);
            _minute.LostFocus += new RoutedEventHandler(MinSec_LostFocus);
            _second.LostFocus += new RoutedEventHandler(MinSec_LostFocus);
            _day.LostFocus += new RoutedEventHandler(Day_LostFocus);
            _month.LostFocus += new RoutedEventHandler(Month_LostFocus);
            _year.LostFocus += new RoutedEventHandler(Year_LostFocus);

            _isInitialized = true;
        }

        void Year_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = int.Parse(textBox.Text);
            if (value < 1900)
                value = 1900;
            else if (value > 9999)
                value = 9999;
            textBox.Text = value.ToString("0000");
        }

        void Month_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = int.Parse(textBox.Text);
            if (value < 1)
                value = 1;
            else if (value > 12)
                value = 12;
            textBox.Text = value.ToString("00");
        }

        void Day_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = int.Parse(textBox.Text);
            if (value < 1)
                value = 1;
            else if (value > 31)
                value = 31;
            textBox.Text = value.ToString("00");
        }

        void Hour_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = int.Parse(textBox.Text);
            if (value < 0)
                value = 0;
            else if (value > 23)
                value = 23;
            textBox.Text = value.ToString("00");
        }

        void MinSec_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = int.Parse(textBox.Text);
            if (value < 0)
                value = 0;
            else if (value > 59)
                value = 59;
            textBox.Text = value.ToString("00");
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            ValidateData();
        }

        void ValidateData()
        {
            if (IsTime)
            {
                var hour = int.Parse(_hour.Text);
                var minute = int.Parse(_minute.Text);
                var second = int.Parse(_second.Text);

                try
                {
                    Date = new DateTime(1900, 1, 1, hour, minute, second);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Date = DateTime.MinValue;
                }
            }
            else
            {
                var year = int.Parse(_year.Text);
                var month = int.Parse(_month.Text);
                var day = int.Parse(_day.Text);

                var daysMax = DateTime.DaysInMonth(year, month);
                if (day > daysMax)
                    day = daysMax;
                
                try
                {
                    Date = new DateTime(year, month, day);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Date = DateTime.MinValue;
                }
            }
        }

        void Text_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }
    }
}
