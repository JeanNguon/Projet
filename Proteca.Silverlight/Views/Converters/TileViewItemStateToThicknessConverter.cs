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
using Telerik.Windows.Controls;
using System.Windows.Data;

namespace Proteca.Silverlight.Views.Converters
{
    public class TileViewItemStateToThicknessConverter : IValueConverter
    {
        public Thickness MaximizedValue { get; set; }
        public Thickness MinimizedValue { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is TileViewItemState && ((TileViewItemState)value) == TileViewItemState.Maximized) ? MaximizedValue : MinimizedValue;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Not Supported");
        }
    }
}
