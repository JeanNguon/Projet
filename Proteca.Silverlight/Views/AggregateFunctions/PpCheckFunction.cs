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
    public class PpCheckFunction : AggregateFunction<PpTmp, Nullable<Boolean>>
    {
        public PpCheckFunction()
        {
            this.AggregationExpression = items => IsAllCkecked(items);
        }

        public Nullable<Boolean> IsAllCkecked(IEnumerable<PpTmp> source)
        {
            
            Nullable<Boolean> estValide = source.All(e => e.Valider);

            if (estValide.HasValue && !estValide.Value && !source.All(e => !e.Valider))
            {
                estValide = null;
            }
            return estValide;
        }
    }
}
