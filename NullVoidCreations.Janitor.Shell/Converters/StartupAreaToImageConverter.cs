using System;
using System.Windows.Data;
using NullVoidCreations.Janitor.Shell.Models;

namespace NullVoidCreations.Janitor.Shell.Converters
{
    public class StartupAreaToImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((StartupEntryModel.StartupArea)value)
            {
                case StartupEntryModel.StartupArea.Registry:
                case StartupEntryModel.StartupArea.RegistryUser:
                    return "/NullVoidCreations.Janitor.Shell;component/Resources/Registry16.png";

                case StartupEntryModel.StartupArea.StartupDirectory:
                case StartupEntryModel.StartupArea.StartupDirectoryUser:
                    return "/NullVoidCreations.Janitor.Shell;component/Resources/Directory16.png";

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
