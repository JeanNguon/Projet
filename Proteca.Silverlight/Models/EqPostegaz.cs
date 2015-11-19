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
    public partial class EqPostegaz
    {
        #region Override Methods

        /// <summary>
        /// Retourne une instance d'historisation de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqPostegaz monhisto = (HistoEqPostegaz)base.GetHisto();

            // Champs spécifiques
            monhisto.CodePosteGaz = this.CodePosteGaz;
            monhisto.TypePoste = this.TypePoste;

            return monhisto;
        }

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqPostegaz moneq = (EqPostegaz)base.DuplicateEq();

            // Champs spécifiques
            moneq.CodePosteGaz = this.CodePosteGaz;
            moneq.TypePoste = this.TypePoste;

            return moneq;
        }

        #endregion

    }
}
