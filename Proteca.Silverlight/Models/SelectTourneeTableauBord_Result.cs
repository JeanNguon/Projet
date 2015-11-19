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
    public partial class SelectTourneeTableauBord_Result
    {
        public bool IsVisite
        {
            get
            {
                return this.DATE_VISITE != null;
            }
        }

        public String IsVisiteText
        {
            get
            {
                return (this.IsVisite) ? Resource.SelectTourneeTableauBord_Result_Visite : Resource.SelectTourneeTableauBord_Result_NonVisite;
            }
        }
    }
}
