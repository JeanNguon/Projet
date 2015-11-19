using System;
using System.Windows.Data;

namespace Proteca.Silverlight.Views.Converters
{
    public class NullOrEmptyToHideConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null || String.IsNullOrEmpty(value.ToString())) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Not Supported");
        }
    }
}
