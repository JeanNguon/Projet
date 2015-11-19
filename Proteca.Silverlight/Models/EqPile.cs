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
    public partial class EqPile
    {
        #region Override Methods

        /// <summary>
        /// Retourne une instance d'historisation de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqPile monhisto = (HistoEqPile)base.GetHisto();

            // Champs spécifiques
            monhisto.DatePrevisionRenouvellementPile = this.DatePrevisionRenouvellementPile;
            monhisto.DateRenouvellementDeversoir = this.DateRenouvellementDeversoir;
            monhisto.NombrePiles = this.NombrePiles;

            // Champs de référence
            monhisto.RefEnumValeur = this.RefEnumValeur != null ? this.RefEnumValeur : null;
            monhisto.CleTypeDeversoir = this.CleTypeDeversoir;

            return monhisto;
        }

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqPile moneq = (EqPile)base.DuplicateEq();

            // Champs spécifiques
            moneq.DatePrevisionRenouvellementPile = this.DatePrevisionRenouvellementPile;
            moneq.DateRenouvellementDeversoir = this.DateRenouvellementDeversoir;
            moneq.NombrePiles = this.NombrePiles;

            // Champs de référence
            moneq.RefEnumValeur = this.RefEnumValeur != null ? this.RefEnumValeur : null;
            moneq.CleTypeDeversoir = this.CleTypeDeversoir;

            return moneq;
        }

        #endregion
    }
}
