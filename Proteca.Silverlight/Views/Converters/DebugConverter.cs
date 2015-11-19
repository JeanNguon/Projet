using System;
using System.Windows;
using System.Windows.Data;

namespace Proteca.Silverlight.Views.Converters
{
    public class DebugConverter : DependencyObject, IValueConverter
    {


        public object Item1 { get { return (object)GetValue(Item1Property); } set { SetValue(Item1Property, value); } }
        public static readonly DependencyProperty Item1Property = DependencyProperty.Register("Item1", typeof(object), typeof(DebugConverter), new PropertyMetadata(null));

        public object Item2 { get { return (object)GetValue(Item2Property); } set { SetValue(Item2Property, value); } }
        public static readonly DependencyProperty Item2Property = DependencyProperty.Register("Item2", typeof(object), typeof(DebugConverter), new PropertyMetadata(null));

        public object Item3 { get { return (object)GetValue(Item3Property); } set { SetValue(Item3Property, value); } }
        public static readonly DependencyProperty Item3Property = DependencyProperty.Register("Item3", typeof(object), typeof(DebugConverter), new PropertyMetadata(null));

        public object Item4 { get { return (object)GetValue(Item4Property); } set { SetValue(Item4Property, value); } }
        public static readonly DependencyProperty Item4Property = DependencyProperty.Register("Item4", typeof(object), typeof(DebugConverter), new PropertyMetadata(null));

        public object Item5 { get { return (object)GetValue(Item5Property); } set { SetValue(Item5Property, value); } }
        public static readonly DependencyProperty Item5Property = DependencyProperty.Register("Item5", typeof(object), typeof(DebugConverter), new PropertyMetadata(null));




        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
