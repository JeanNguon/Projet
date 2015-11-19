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
    public partial class EqLiaisonExterne
    {
        #region Override Methods

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqLiaisonExterne moneq = (EqLiaisonExterne)base.DuplicateEq();
            
            // Champs spécifiques
            moneq.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            moneq.LiaisonTechnique = this.LiaisonTechnique;
            moneq.LibelleEquipementTiers = this.LibelleEquipementTiers;
            moneq.LibellePointCommun = this.LibellePointCommun;
            moneq.PresencePcSurOuvrageTiers = this.PresencePcSurOuvrageTiers;
            moneq.PresenceTelemesure = this.PresenceTelemesure;
            moneq.ProtectionTiersParUnite = this.ProtectionTiersParUnite;
            moneq.TypeFluide = this.TypeFluide;

            // Champs de référence
            moneq.RefSousTypeOuvrage = this.RefSousTypeOuvrage;
            moneq.CleTypeLiaison = this.CleTypeLiaison;
            moneq.CleNomTiersAss = this.CleNomTiersAss;

            return moneq;
        }

        /// <summary>
        /// Retourne une instance de l'historisation de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqLiaisonExterne monhisto = (HistoEqLiaisonExterne)base.GetHisto();

            // Champs spécifiques
            monhisto.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            monhisto.LiaisonTechnique = this.LiaisonTechnique;
            monhisto.LibelleEquipementTiers = this.LibelleEquipementTiers;
            monhisto.LibellePointCommun = this.LibellePointCommun;
            monhisto.PresencePcSurOuvrageTiers = this.PresencePcSurOuvrageTiers;
            monhisto.PresenceTelemesure = this.PresenceTelemesure;
            monhisto.ProtectionTiersParUnite = this.ProtectionTiersParUnite;
            monhisto.TypeFluide = this.TypeFluide;

            // Champs de référence
            monhisto.NomTiers = this.RefSousTypeOuvrage != null ? this.RefSousTypeOuvrage.Libelle : null;
            monhisto.CleTypeLiaison = this.CleTypeLiaison;

            return monhisto;
        }

        #endregion

    }
}
