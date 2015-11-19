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
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;

namespace Proteca.Web.Models
{
    public partial class EqFourreauMetallique 
    {
        #region public properties

        /// <summary>
        /// Url de la portion secondaire de rattachement
        /// </summary>
        public Uri NaviagtionPortionSecUrl
        {
            get
            {
                if (this.ClePp2.HasValue)
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
        #endregion

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

        #endregion

        #region Events.

        /// <summary>
        /// Initialisation des données au chargement des écrans
        /// </summary>
        /// <param name="isInitialLoad"></param>
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew)
            {
                _longueurNullable = this.Longueur;
            }
        }

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
            else if (e.PropertyName == "EntityState")
            {
                if (this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
                {
                    if (IsNew)
                    {
                        _longueurNullable = null;
                    }
                    else
                    {
                        _longueurNullable = this.Longueur;
                    }
                    this.RaisePropertyChanged("LongueurNullable");
                }
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Retourne une instance de l'historisation de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqFourreauMetallique monhisto = (HistoEqFourreauMetallique)base.GetHisto();

            // Champs spécifiques
            monhisto.Longueur = this.Longueur;

            // Champs de référence
            monhisto.LibellePp2 = this.Pp2 != null ? this.Pp2.Libelle : "";
            monhisto.LibellePortionPp2 = LibellePortion2;

            return monhisto;
        }
        
        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqFourreauMetallique moneq = (EqFourreauMetallique)base.DuplicateEq();

            // Champs spécifiques
            moneq.LongueurNullable = this.LongueurNullable;

            // Champs de référence
            moneq.Pp2 = this.Pp2;

            return moneq;
        }

        #endregion

        #region Gestion des valeurs par défaut des champs obligatoires

        private Nullable<int> _longueurNullable;
        [RequiredCustom()]
        public Nullable<int> LongueurNullable
        {
            get
            {
                return _longueurNullable;
            }
            set
            {
                if (_longueurNullable != value)
                {
                    this.ValidateProperty("LongueurNullable", value);

                    if (value.HasValue)
                    {
                        this.Longueur = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("Longueur");
                    }
                    _longueurNullable = value;
                }
            }
        }

        #endregion
    }
}
