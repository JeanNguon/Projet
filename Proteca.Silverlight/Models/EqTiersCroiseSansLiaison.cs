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
using System.ServiceModel.DomainServices.Client;

namespace Proteca.Web.Models
{
    public partial class EqTiersCroiseSansLiaison
    {
        #region Override Methods

        /// <summary>
        /// Retourne une instance de l'historique de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqTiersCroiseSansLiaison monhisto = (HistoEqTiersCroiseSansLiaison)base.GetHisto();

            // Champs spécifiques
            monhisto.NomTiersAssocie = this.NomTiersAssocie;
            monhisto.PresencePcSurOuvrageTiers = this.PresencePcSurOuvrageTiers;
            monhisto.TypeFluide = this.TypeFluide;

            return monhisto;
        }

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqTiersCroiseSansLiaison moneq = (EqTiersCroiseSansLiaison)base.DuplicateEq();
            
            // Champs spécifiques
            moneq.NomTiersAssocie = this.NomTiersAssocie;
            moneq.PresencePcSurOuvrageTiers = this.PresencePcSurOuvrageTiers;
            moneq.TypeFluide = this.TypeFluide;

            return moneq;
        }

        #endregion
    }
}
