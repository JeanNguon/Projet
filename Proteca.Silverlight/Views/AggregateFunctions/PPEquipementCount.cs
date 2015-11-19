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
using System.Linq;
using System.Collections.Generic;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Views.AggregateFunctions
{
    public static class PPEquipementCount
    {
        public static int Count<TSource>(IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            int itemCount = source.Count();
            if (itemCount == 1 && !selector(source.First()).HasValue)
            {
                return 0;
            }

            return itemCount;
        }
    }
}
