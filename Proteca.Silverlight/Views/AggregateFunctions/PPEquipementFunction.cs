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

namespace Proteca.Silverlight.Views.AggregateFunctions
{
    public class PPEquipementFunction : EnumerableSelectorAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get
            {
                return "Count";
            }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(PPEquipementCount);
            }
        }
    }
}
