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
    public class BinaryTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //Reverse the template for the next call
            DataTemplate tmp = this.OddTemplate;
            this.OddTemplate = this.EvenTemplate;
            this.EvenTemplate = tmp;
            return tmp;

        }
        public DataTemplate OddTemplate { get; set; }
        public DataTemplate EvenTemplate { get; set; }
    }
}
