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
using Proteca.Web.Models;

namespace Proteca.Silverlight.Views.StyleSelectors
{
    public class CategoriePPStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is CategoriePp)
            {
                CategoriePp categorie = item as CategoriePp;
                if (categorie.NonLieAUnEquipement)
                {
                    return SpecificStyle;
                }
                else
                {
                    return MainStyle;
                }
            }
            return null;
        }
        public Style SpecificStyle { get; set; }
        public Style MainStyle { get; set; }
    }
}
