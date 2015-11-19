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
using Proteca.Web.Models;
using System.Linq;
using System.Collections.ObjectModel;
using Proteca.Silverlight.Views.Tools;

namespace Proteca.Silverlight.Views.Converters
{
    public class ListPorteeToFilterListConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<RefUsrPortee> portees = values[0] as ObservableCollection<RefUsrPortee>;
            string typePortee = values[1] as string;
            
            if (portees != null)
            {
                return portees.Where(p => p.TypePortee == typePortee);
            }
            return portees.Where(p => p.TypePortee == typePortee);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
