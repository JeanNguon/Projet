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
    public class VisiteCheckFunction : AggregateFunction<Visite, Nullable<Boolean>>
    {
        public VisiteCheckFunction()
        {
            this.AggregationExpression = items => IsAllCkecked(items);
        }

        public Nullable<Boolean> IsAllCkecked(IEnumerable<Visite> source)
        {
            if (source == null || !source.Any())
                return false;
            var toutesValidees = source.All(v => v.EstValidee);
            var anyValidee = source.Any(v => v.EstValidee);
            if (!toutesValidees && anyValidee)
            {
                return null;
            }
            return toutesValidees;
        }
    }
}
