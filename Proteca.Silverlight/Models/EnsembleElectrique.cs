using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;

namespace Proteca.Web.Models
{
    public partial class EnsembleElectrique
    {        
        #region Gestion des valeurs par défaut des champs obligatoires

        private Nullable<int> _enumPeriodiciteNullable;
        [RequiredCustom()]
        public Nullable<int> EnumPeriodiciteNullable
        {
            get
            {
                return _enumPeriodiciteNullable;
            }
            set
            {
                if (_enumPeriodiciteNullable != value)
                {
                    this.ValidateProperty("EnumPeriodiciteNullable", value);

                    if (value.HasValue)
                    {
                        this.EnumPeriodicite = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("EnumPeriodicite");
                    }
                    _enumPeriodiciteNullable = value;
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Initialisation des données au chargement des écrans
        /// </summary>
        /// <param name="isInitialLoad"></param>
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew())
            {
                _enumPeriodiciteNullable = this.EnumPeriodicite;
            }
        }

        /// <summary>
        /// MAJ de des propriétés
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "EntityState")
            {
                if (this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
                {
                    if (this.IsNew())
                    {
                        _enumPeriodiciteNullable = null;
                    }
                    else
                    {
                        _enumPeriodiciteNullable = this.EnumPeriodicite;
                    }
                    this.RaisePropertyChanged("EnumPeriodiciteNullable");
                }
            }
        }

        #endregion

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

        /// <summary>
        /// retourne True si il existe au moins une analyse sur l'ensemble électrique
        /// </summary>
        public Boolean AnyAnalyse
        {
            get { return (AnAnalyseEe != null && AnAnalyseEe.Any()); }
        }

        /// <summary>
        /// Date de la dernière analyse
        /// </summary>
        public String DateDeLAnalyse
        {
            get
            {
                if (AnyAnalyse)
                {
                    return AnAnalyseEe.OrderByDescending(a => a.DateAnalyse).First().DateAnalyse.Value.ToString("dd/MM/yyyy");
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Retourne la source de l'image à afficher suivant l'état de l'analyse
        /// </summary>
        public string EtatSourceImage
        {
            get
            {
                string result = String.Empty;
                if (AnAnalyseEe.Any())
                {
                    result = AnAnalyseEe.OrderByDescending(a => a.DateAnalyse).FirstOrDefault().EtatSourceImage;
                }
                return result;
            }
        }

        #endregion Public Properties

        /// <summary>
        /// Retourne le libelle de l'ensemble électrique
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Libelle;
        }
    }
}
