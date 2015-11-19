using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Proteca.Silverlight.Views.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime result;
            if (value != null && DateTime.TryParse(value.ToString(), out result))
            {
                return result.ToString("d");
            }
            else
            {
                return string.Empty;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            else
            {
                return value;
            }
        }
    }
}
