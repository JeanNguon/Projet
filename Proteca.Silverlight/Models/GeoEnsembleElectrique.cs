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
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;

namespace Proteca.Web.Models
{
    public partial class GeoEnsembleElectrique
    {
        #region public Properties

        /// <summary>
        /// Url de l'élément courant
        /// </summary>
        public string NaviagtionUrl
        {
            get
            {
                return string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.EnsembleElectrique.GetStringValue(),
                   CleEnsElectrique);
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Retourne le libelle de l'ensemble électrique
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Libelle;
        }

        #endregion

    }
}
