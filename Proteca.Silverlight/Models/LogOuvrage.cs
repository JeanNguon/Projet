using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System;

namespace Proteca.Web.Models
{
    public partial class LogOuvrage
    {
        public bool isHistorisation
        {
            get
            {
                return this.RefEnumValeur != null && this.RefEnumValeur.Valeur == "H";
            }
        }
    }
}
