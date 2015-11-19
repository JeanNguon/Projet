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
    public partial class EqDrainage
    {
        #region Events

        /// <summary>
        /// Au chargement, on initialise l'écran
        /// </summary>
        /// <param name="isInitialLoad"></param>
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew)
            {
                _intensiteMaximaleSupporteeNullable = this.IntensiteMaximaleSupportee;
            }
        }

        #endregion

        #region Gestion des valeurs par défaut des champs obligatoires

        /// <summary>
        /// Private propertie pour gérer la version nullable de intensiteMaximale
        /// </summary>
        private Nullable<int> _intensiteMaximaleSupporteeNullable;

        /// <summary>
        /// Private propertie pour gérer la version nullable de intensiteMaximale
        /// </summary>
        [RequiredCustom()]
        public Nullable<int> IntensiteMaximaleSupporteeNullable
        {
            get
            {
                return _intensiteMaximaleSupporteeNullable;
            }
            set
            {
                if (_intensiteMaximaleSupporteeNullable != value)
                {
                    this.ValidateProperty("IntensiteMaximaleSupporteeNullable", value);

                    if (value.HasValue)
                    {
                        this.IntensiteMaximaleSupportee = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("IntensiteMaximaleSupportee");
                    }
                    _intensiteMaximaleSupporteeNullable = value;
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
                        _intensiteMaximaleSupporteeNullable = null;
                    }
                    else
                    {
                        _intensiteMaximaleSupporteeNullable = this.IntensiteMaximaleSupportee;
                    }
                    this.RaisePropertyChanged("IntensiteMaximaleSupporteeNullable");
                }
            }
        }

        /// <summary>
        /// Retourne une instance de l'historique de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqDrainage monhisto = (HistoEqDrainage)base.GetHisto();

            // Champs spécifiques
            monhisto.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            monhisto.IntensiteMaximaleSupportee = this.IntensiteMaximaleSupportee;
            monhisto.PresenceTelemesure = this.PresenceTelemesure;

            // Champs de référence
            monhisto.CleTypeDrainage = this.CleTypeDrainage;

            return monhisto;
        }

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqDrainage moneq = (EqDrainage)base.DuplicateEq();

            // Champs spécifiques
            moneq.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            moneq.IntensiteMaximaleSupporteeNullable = this.IntensiteMaximaleSupporteeNullable;
            moneq.PresenceTelemesure = this.PresenceTelemesure;

            // Champs de référence
            moneq.CleTypeDrainage = this.CleTypeDrainage;

            return moneq;
        }

        #endregion
    }
}
