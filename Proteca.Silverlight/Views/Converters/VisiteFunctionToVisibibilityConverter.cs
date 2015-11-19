using System;
using System.Windows.Data;
using Telerik.Windows.Data;

namespace Proteca.Silverlight.Views.Converters
{
    public class VisiteFunctionToVisibibilityConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value != null && value is AggregateResultCollection && ((AggregateResultCollection)value).Count > 0 && ((bool)((AggregateResultCollection)value)[0].Value)) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Not Supported");
        }
    }
}
