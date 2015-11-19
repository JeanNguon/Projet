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
using Proteca.Silverlight.Models;
using Telerik.Windows.Controls;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Views.StyleSelectors
{
    public class MenuStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is MenuItem)
            {
                MenuItem menuItem = item as MenuItem;
                if (menuItem.IsSelected)
                {
                    return SelectedItemStyle;
                }
                else
                {
                    return MainStyle;
                }
            }
            else if (item is TypeEquipement)
            {
                TypeEquipement typeEqItem = item as TypeEquipement;
                if (typeEqItem.IsSelected)
                {
                    return SelectedItemStyle;
                }
                else
                {
                    return MainStyle;
                }
            }
            else if (item is TypeRessource)
            {
                TypeRessource typeResItem = item as TypeRessource;
                if (typeResItem.IsSelected)
                {
                    return SelectedItemStyle;
                }
                else
                {
                    return MainStyle;
                }
            }
            return null;
        }
        public Style MainStyle { get; set; }
        public Style SelectedItemStyle { get; set; }
    }
}
