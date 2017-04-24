using System;
using System.Windows.Data;

namespace NullVoidCreations.Janitor.Shell.Converters
{
    public class LicenseStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return "/program_shell;component/Resources/Ok16.png";
            else
                return "/program_shell;component/Resources/Error16.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
