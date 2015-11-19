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
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;

namespace Proteca.Web.Models
{
    public partial class EqRaccordIsolant 
    {
        #region Liens

        /// <summary>
        /// Url de la portion secondaire de rattachement
        /// </summary>
        public Uri NaviagtionPortionSecUrl
        {
            get
            {
                if (this.Pp2 != null)
                {
                    return new Uri(string.Format("/{0}/{1}/Id={2}",
                       MainNavigation.GestionOuvrages.GetStringValue(),
                       OuvrageNavigation.PortionIntegrite.GetStringValue(),
                       this.Pp2.ClePortion), UriKind.Relative);
                }
                else
                { return null; }
            }
        }

        /// <summary>
        /// Url de la PP de rattachement 2
        /// </summary>
        public Uri NaviagtionPP2Url
        {
            get
            {
                return new Uri(string.Format("/{0}/{1}/PP/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.Equipement.GetStringValue(),
                   this.ClePp2), UriKind.Relative);
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Retourne une instance d'historisation de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqRaccordIsolant monhisto = (HistoEqRaccordIsolant)base.GetHisto();

            // Champs spécifiques
            monhisto.LibellePp2 = this.Pp2 != null ? this.Pp2.Libelle : "";
            monhisto.LibellePortionPp2 = this.LibellePortion2;
            monhisto.PresenceEclateur = this.PresenceEclateur;

            // Champs de références
            monhisto.LibelleLiaison = this.EqEquipement1 != null ? this.EqEquipement1.Libelle : null;
            monhisto.RefEnumValeur = this.RefEnumValeur != null ? this.RefEnumValeur : null;
            monhisto.RefEnumValeur1 = this.RefEnumValeur1 != null ? this.RefEnumValeur1 : null;
            monhisto.CleTypeLiaison = this.CleTypeLiaison;
            monhisto.CleTypeRi = this.CleTypeRi;

            return monhisto;
        }


        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqRaccordIsolant moneq = (EqRaccordIsolant)base.DuplicateEq();

            // Champs spécifiques
            moneq.PresenceEclateur = this.PresenceEclateur;

            // Champs de références
            moneq.EqEquipement1 = this.EqEquipement1;
            moneq.RefEnumValeur = this.RefEnumValeur;
            moneq.RefEnumValeur1 = this.RefEnumValeur1;
            moneq.CleTypeLiaison = this.CleTypeLiaison;
            moneq.CleTypeRi = this.CleTypeRi;
            moneq.ClePp2 = this.ClePp2;

            return moneq;
        }

        /// <summary>
        /// Déplacement de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DeplacerEq()
        {
            // On duplique la liaison interne associé
            if (this.EqEquipement1 != null)
            {
                this.EqEquipement1.DuplicateEq();
            }

            return base.DeplacerEq();
        }

        #endregion

        #region Events

        /// <summary>
        /// MAJ de navigationPP2Url à la modification de la cléPP2
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "ClePp2")
            {
                RaisePropertyChanged("NaviagtionPP2Url");
            }
        }

        #endregion
    }
}
