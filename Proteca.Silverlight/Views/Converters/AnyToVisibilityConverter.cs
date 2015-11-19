using System;
using System.Windows.Data;

namespace Proteca.Silverlight.Views.Converters
{
    public class AnyToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int count = 0;
            if (value != null && !String.IsNullOrEmpty(value.ToString()) && int.TryParse(value.ToString(), out count))
            {
                if(count > 0)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Not Supported");
        }
    }
}
