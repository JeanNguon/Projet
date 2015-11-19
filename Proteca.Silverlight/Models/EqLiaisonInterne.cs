using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using System.Linq;
using System;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;

namespace Proteca.Web.Models
{
    public partial class EqLiaisonInterne 
    {
        #region Private Properties

        /// <summary>
        /// Retourne les libelles modifiés
        /// </summary>
        private List<string> _listPartsOfLibelle = null;
        private List<string> ListPartsOfLibelle
        {
            get
            {
                if (_listPartsOfLibelle == null)
                    _listPartsOfLibelle = new List<string>(Libelle.Split(new string[1] { " - " }, StringSplitOptions.None));

                return _listPartsOfLibelle;
            }
            set
            {
                _listPartsOfLibelle = value;
                RaisePropertyChanged("ListPartsOfLibelle");
                RaisePropertyChanged("LibellePrincipale");
                RaisePropertyChanged("LibellePrincipaleRemove");
                RaisePropertyChanged("LibellePrefix");
                RaisePropertyChanged("LibelleSufix");
            }
        }

        #endregion Private Member

        #region Liens

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
        /// Url de la Liaison secondaire de jumelage EE
        /// </summary>
        public Uri NavigationLiaisonInterEe
        {
            get
            {
                if (this.LiaisonInterEe && this.CleLiaisonInterEe.HasValue && this.CleLiaisonInterEe.Value != 0)
                {
                    return new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                       MainNavigation.GestionOuvrages.GetStringValue(),
                       OuvrageNavigation.Equipement.GetStringValue(),
                       FiltreNavigation.LI.GetStringValue(),
                       this.CleLiaisonInterEe), UriKind.Relative);
                }
                else
                { return null; }
            }
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
            if (e.PropertyName == "LiaisonInterEe")
            {
                RaisePropertyChanged("NavigationLiaisonInterEe");
            }
            if (e.PropertyName == "Libelle")
            {
                ListPartsOfLibelle = null;
            }
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Libelle principal
        /// </summary>
        public string LibellePrincipale
        {
            get
            {
                if (this.LiaisonInterEe && ListPartsOfLibelle.Count > 2)
                    return ListPartsOfLibelle.GetRange(1, ListPartsOfLibelle.Count - 2).Aggregate((text, next) => string.Format("{0} - {1}", text, next));
                else
                    return this.Libelle;
            }
            set
            {
                if (this.LiaisonInterEe && ListPartsOfLibelle.Count > 2)
                    this.Libelle = string.Format("{0} - {1} - {2}", this.LibellePrefix, value, this.LibelleSufix);
                else
                    this.Libelle = value;
            }
        }

        /// <summary>
        /// Libelle principal
        /// </summary>
        public string LibellePrincipaleRemove
        {
            get
            {
                if (ListPartsOfLibelle.Count > 2)
                {
                    return ListPartsOfLibelle.GetRange(1, ListPartsOfLibelle.Count - 2).Aggregate((text, next) => string.Format("{0} - {1}", text, next));
                }
                else
                    return this.Libelle;
            }
        }

        /// <summary>
        /// préfixe utilisé pour les liaison inter EE
        /// </summary>
        public string LibellePrefix
        {
            get
            {
                if (this.LiaisonInterEe)
                    return ListPartsOfLibelle.First();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Suffixe utilisé pour les liaison inter EE
        /// </summary>
        public string LibelleSufix
        {
            get
            {
                if (this.LiaisonInterEe)
                    return ListPartsOfLibelle.Last();
                else
                    return string.Empty;
            }
        }

        #endregion Public Properties

        #region Override Methods

        /// <summary>
        /// Retourne une instance de l'historisation de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqLiaisonInterne monhisto = (HistoEqLiaisonInterne)base.GetHisto();

            // Champs spécifiques
            monhisto.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            monhisto.LiaisonInterEe = this.LiaisonInterEe;
            monhisto.LibellePointCommun = this.LibellePointCommun;
            monhisto.PresenceTelemesure = this.PresenceTelemesure;

            // Champs de référence
            monhisto.LibellePp2 = this.Pp2 != null ? this.Pp2.Libelle : "";
            monhisto.LibellePortionPp2 = this.LibellePortion2;
            monhisto.CleTypeLiaison = this.CleTypeLiaison;

            return monhisto;
        }

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqLiaisonInterne moneq = (EqLiaisonInterne)base.DuplicateEq();

            // Champs spécifiques
            moneq.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            moneq.LiaisonInterEe = this.LiaisonInterEe;
            moneq.LibellePointCommun = this.LibellePointCommun;
            moneq.PresenceTelemesure = this.PresenceTelemesure;

            // Champs de référence
            moneq.CleTypeLiaison = this.CleTypeLiaison;
            moneq.ClePp2 = this.ClePp2;
            moneq.ClePp = this.ClePp;

            return moneq;
        }

        #endregion
    }
}
