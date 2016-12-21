using System;
using System.Windows.Data;
using System.Globalization;

namespace NullVoidCreations.Janitor.Shell.Converters
{
    public class InverseBoolConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
