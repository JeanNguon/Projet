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
    public class InstrumentStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is InsInstrument)
            {
                InsInstrument instrument = item as InsInstrument;
                if (instrument.IsEditable)
                {
                    return EditableStyle;
                }
                else
                {
                    return NotEditableStyle;
                }
            }
            return null;
        }
        public Style EditableStyle { get; set; }
        public Style NotEditableStyle { get; set; }
    }
}
