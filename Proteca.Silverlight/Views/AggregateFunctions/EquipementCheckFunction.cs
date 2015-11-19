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
    public class EquipementCheckFunction : AggregateFunction<EqEquipementTmp, Nullable<Boolean>>
    {
        public EquipementCheckFunction()
        {
            this.AggregationExpression = items => IsAllCkecked(items.Where(e => e.CanValid));
        }

        public Nullable<Boolean> IsAllCkecked(IEnumerable<EqEquipementTmp> source)
        {
            
            Nullable<Boolean> estValide = source.All(e => e.EstValide);

            if (estValide.HasValue && !estValide.Value && !source.All(e => !e.EstValide))
            {
                estValide = null;
            }
            return estValide;
        }
    }
}
