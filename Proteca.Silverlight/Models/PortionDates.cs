using System;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;

namespace Proteca.Web.Models
{
    public partial class PortionDates
    {
        /// <summary>
        /// Url de l'élément courant
        /// </summary>
        public string NaviagtionUrl
        {
            get
            {
                return string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.PortionIntegrite.GetStringValue(),
                   ClePortion);
            }
        }

        /// <summary>
        /// Date ECD formatter pour la présentation graphique
        /// </summary>
        public string DateEcdBinding
        {
            get 
            {
                if (this.DateEcd.HasValue)
                    return this.DateEcd.Value.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Date Eg formatter pour la présentation graphique
        /// </summary>
        public string DateEgBinding
        {
            get
            {
                if (this.DateEg.HasValue)
                    return this.DateEg.Value.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Date Cf formatter pour la présentation graphique
        /// </summary>
        public string DateCfBinding
        {
            get
            {
                if (this.DateCf.HasValue)
                    return this.DateCf.Value.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
    }
}
