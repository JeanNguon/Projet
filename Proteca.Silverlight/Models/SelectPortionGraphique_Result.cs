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
using Proteca.Silverlight.Resources;

namespace Proteca.Web.Models
{
    public partial class SelectPortionGraphique_Result
    {
        public decimal PkTruncated
        {
            get
            {
                return Decimal.Round(this.PK, 1);
            }
        }
    }
}
