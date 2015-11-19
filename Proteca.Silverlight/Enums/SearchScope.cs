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

namespace Proteca.Silverlight.Enums
{
    public class SearchScope
    {
        public enum settings
        {
            [StringValue("/Pages/searchproteca.aspx?IsDlg=1")]
            UriProteca
        }

        public enum metadata
        {
            [StringValue("region")]
            region,
            [StringValue("agence")]
            agence,
            [StringValue("secteur")]
            secteur,
            [StringValue("regionint")]
            regionint,
            [StringValue("agenceint")]
            agenceint,
            [StringValue("secteurint")]
            secteurint
        }

        public enum Scopes
        {
            [StringValue("Proteca")]
            All,
            [StringValue("EnsembleElectrique")]
            EnsElec,
            [StringValue("PortionIntegrite")]
            Portion,
            [StringValue("Pp")]
            Pp,
            [StringValue("Equipements")]
            Equipements,
            [StringValue("DocProteca")]
            DocOnly
        }
    }
}
