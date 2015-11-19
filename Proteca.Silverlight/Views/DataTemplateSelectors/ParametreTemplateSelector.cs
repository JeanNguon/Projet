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

namespace Proteca.Silverlight.Views.DataTemplateSelectors
{
    public class ParametreTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is RefParametre)
            {
                RefParametre parametre = item as RefParametre;
                switch (parametre.TypeDeDonnee)
                {
                    case ("INT"):
                        return INT;
                    case ("BOOLEAN"):
                        return BOOLEAN;
                    case ("DOUBLE"):
                        return DOUBLE;
                    case("STRING"):
                        return STRING;
                    case ("MESMODELEMESURE"):
                        return MESMODELEMESURE;
                    default:
                        return STRING;
                }
            }
            return null;
        }
        public DataTemplate INT { get; set; }
        public DataTemplate BOOLEAN { get; set; }
        public DataTemplate DOUBLE { get; set; }
        public DataTemplate STRING { get; set; }
        public DataTemplate MESMODELEMESURE { get; set; }
    }
}
