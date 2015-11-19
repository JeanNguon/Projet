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
    public partial class EqAnodeGalvanique
    {

        #region Override Methods

        /// <summary>
        /// Retourne une instance de l'historique de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqAnodeGalvanique monhisto = (HistoEqAnodeGalvanique)base.GetHisto();

            // Champs spécifiques
            monhisto.PileAssociee = this.PileAssociee;

            // Champs de référence
            monhisto.CleTypeAnode = this.CleTypeAnode;

            return monhisto;
        }

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqAnodeGalvanique moneq = (EqAnodeGalvanique)base.DuplicateEq();

            // Champs spécifiques
            moneq.PileAssociee = this.PileAssociee;

            // Champs de référence
            moneq.CleTypeAnode = this.CleTypeAnode;

            return moneq;
        }
        
        #endregion
    }
}
