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
using Telerik.Windows.Data;
using Proteca.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Proteca.Silverlight.Views.AggregateFunctions
{
    public class VisiteFunction : AggregateFunction<Visite, Boolean>
    {
        public VisiteFunction()
        {
            this.AggregationExpression = items => RelevePartiel(items);
        }

        public Boolean RelevePartiel(IEnumerable<Visite> source)
        {
            if (source == null || !source.Any())
                return false;
            return source.Any(v => v.RelevePartiel);
        }
    }
}
