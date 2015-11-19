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
    public partial class EqDispoEcoulementCourantsAlternatifs
    {
        #region Events

        /// <summary>
        /// Au chargment, on initialisae les champs
        /// </summary>
        /// <param name="isInitialLoad"></param>
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew)
            {
                _capaciteCondensateurNullable = this.CapaciteCondensateur;
            }
        }

        #endregion
                
        #region Gestion des valeurs par défaut des champs obligatoires

        /// <summary>
        /// Private propertie pour gérer la version nullable de la capacité d'un condensateur
        /// </summary>
        private Nullable<int> _capaciteCondensateurNullable;

        /// <summary>
        /// Public propertie pour gérer la version nullable de la capacité d'un condensateur
        /// </summary>
        [RequiredCustom()]
        public Nullable<int> CapaciteCondensateurNullable
        {
            get
            {
                return _capaciteCondensateurNullable;
            }
            set
            {                
                if (_capaciteCondensateurNullable != value)
                {
                    this.ValidateProperty("CapaciteCondensateurNullable", value);

                    if (value.HasValue)
                    {
                        this.CapaciteCondensateur = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("CapaciteCondensateur");
                    }
                    _capaciteCondensateurNullable = value;
                }
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// On force la MAJ des propriétés
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "EntityState")
            {
                if (this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
                {
                    if (IsNew)
                    {
                        _capaciteCondensateurNullable = null;
                    }
                    else
                    {
                        _capaciteCondensateurNullable = this.CapaciteCondensateur;
                    }
                    this.RaisePropertyChanged("CapaciteCondensateurNullable");
                }
            }
        }

        /// <summary>
        /// Retourne une instance de l'historique de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqDispoEcoulementCourantsAlternatifs monhisto = (HistoEqDispoEcoulementCourantsAlternatifs)base.GetHisto();

            // Champs spécifiques
            monhisto.CapaciteCondensateur = this.CapaciteCondensateur;
            monhisto.CoordDebPriseTerreLat = this.CoordDebPriseTerreLat;
            monhisto.CoordDebPriseTerreLong = this.CoordDebPriseTerreLong;
            monhisto.CoordFinPriseTerreLat = this.CoordFinPriseTerreLat;
            monhisto.CoordFinPriseTerreLong = this.CoordFinPriseTerreLong;
            monhisto.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            monhisto.DatePosePriseDeTerre = this.DatePosePriseDeTerre;
            monhisto.ResistanceInitPriseDeTerre = this.ResistanceInitPriseDeTerre;

            // Champs de référence
            monhisto.CleTypePriseDeTerre = this.CleTypePriseDeTerre;

            return monhisto;
        }

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqDispoEcoulementCourantsAlternatifs moneq = (EqDispoEcoulementCourantsAlternatifs)base.DuplicateEq();

            // Champs spécifiques
            moneq.CapaciteCondensateurNullable = this.CapaciteCondensateurNullable;
            moneq.CoordDebPriseTerreLat = this.CoordDebPriseTerreLat;
            moneq.CoordDebPriseTerreLong = this.CoordDebPriseTerreLong;
            moneq.CoordFinPriseTerreLat = this.CoordFinPriseTerreLat;
            moneq.CoordFinPriseTerreLong = this.CoordFinPriseTerreLong;
            moneq.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            moneq.DatePosePriseDeTerre = this.DatePosePriseDeTerre;
            moneq.ResistanceInitPriseDeTerre = this.ResistanceInitPriseDeTerre;

            // Champs de référence
            moneq.CleTypePriseDeTerre = this.CleTypePriseDeTerre;

            return moneq;
        }

        #endregion
    }
}
