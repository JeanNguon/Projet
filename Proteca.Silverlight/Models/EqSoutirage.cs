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
using Proteca.Silverlight.Helpers;

namespace Proteca.Web.Models
{
    public partial class EqSoutirage
    {
        #region Override Methods

        /// <summary>
        /// Retourne une instance d'historisation de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity GetHisto()
        {
            HistoEqSoutirage monhisto = (HistoEqSoutirage)base.GetHisto();

            // Champs spécifiques
            monhisto.Autoregule = this.Autoregule;
            monhisto.CoordDebDeversoirLat = this.CoordDebDeversoirLat;
            monhisto.CoordDebDeversoirLong = this.CoordDebDeversoirLong;
            monhisto.CoordFinDeversoirLat = this.CoordFinDeversoirLat;
            monhisto.CoordFinDeversoirLong = this.CoordFinDeversoirLong;
            monhisto.DateControle = this.DateControle;
            monhisto.DateMiseEnServiceRedresseur = this.DateMiseEnServiceRedresseur;
            monhisto.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            monhisto.DatePoseDeversoir = this.DatePoseDeversoir;
            monhisto.DateRenouvellementDeversoir = this.DateRenouvellementDeversoir;
            monhisto.IntensiteReglage = this.IntensiteReglage;
            monhisto.LongueurDeversoir = this.LongueurDeversoir;
            monhisto.MasseAuMetreLineaire = this.MasseAuMetreLineaire;
            monhisto.PresenceReenclencheur = this.PresenceReenclencheur;
            monhisto.PresenceTelemesure = this.PresenceTelemesure;
            monhisto.TensionReglage = this.TensionReglage;

            // Champs de référence
            monhisto.TypeDeversoir = this.CleDeversoir;
            monhisto.TypeRedresseur = this.RefSousTypeOuvrage1 != null ? this.RefSousTypeOuvrage1.Libelle : null;

            return monhisto;
        }

        /// <summary>
        /// Duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public override Entity DuplicateEq()
        {
            EqSoutirage moneq = (EqSoutirage)base.DuplicateEq();

            // Champs spécifiques
            moneq.Autoregule = this.Autoregule;
            moneq.CoordDebDeversoirLat = this.CoordDebDeversoirLat;
            moneq.CoordDebDeversoirLong = this.CoordDebDeversoirLong;
            moneq.CoordFinDeversoirLat = this.CoordFinDeversoirLat;
            moneq.CoordFinDeversoirLong = this.CoordFinDeversoirLong;
            moneq.DateControleNullable = this.DateControleNullable;
            moneq.DateMiseEnServiceRedresseurNullable = this.DateMiseEnServiceRedresseurNullable;
            moneq.DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure;
            moneq.DatePoseDeversoir = this.DatePoseDeversoir;
            moneq.DateRenouvellementDeversoir = this.DateRenouvellementDeversoir;
            moneq.IntensiteReglageNullable = this.IntensiteReglageNullable;
            moneq.LongueurDeversoirNullable = this.LongueurDeversoirNullable;
            moneq.MasseAuMetreLineaire = this.MasseAuMetreLineaire;
            moneq.PresenceReenclencheur = this.PresenceReenclencheur;
            moneq.PresenceTelemesure = this.PresenceTelemesure;
            moneq.TensionReglageNullable = this.TensionReglageNullable;

            // Champs de référence
            moneq.CleDeversoir = this.CleDeversoir;
            //moneq.RefSousTypeOuvrage1 = this.RefSousTypeOuvrage1;
            moneq.CleRedresseur = this.CleRedresseur;

            return moneq;
        }

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
                        _dateMiseEnServiceRedresseurNullable = null;
                        _dateControleNullable = null;
                        _tensionReglageNullable = null;
                        _intensiteReglageNullable = null;
                        _longueurDeversoirNullable = null;                     
                    }
                    else
                    {
                        _dateMiseEnServiceRedresseurNullable = this.DateMiseEnServiceRedresseur;
                        _dateControleNullable = this.DateControle;
                        _tensionReglageNullable = this.TensionReglage;
                        _intensiteReglageNullable = this.IntensiteReglage;
                        _longueurDeversoirNullable = this.LongueurDeversoir;
                    }
                    this.RaisePropertyChanged("DateMiseEnServiceRedresseurNullable");
                    this.RaisePropertyChanged("DateControleNullable");
                    this.RaisePropertyChanged("TensionReglageNullable");
                    this.RaisePropertyChanged("IntensiteReglageNullable");
                    this.RaisePropertyChanged("LongueurDeversoirNullable");
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
            if (!this.IsNew)
            {
                _tensionReglageNullable = this.TensionReglage;
                _dateMiseEnServiceRedresseurNullable = this.DateMiseEnServiceRedresseur;
                _dateControleNullable = this.DateControle;
                _intensiteReglageNullable = this.IntensiteReglage;
                _longueurDeversoirNullable = this.LongueurDeversoir;
            }
        }

        #endregion

        #region Gestion des valeurs par défaut des champs obligatoires

        private Nullable<DateTime> _dateMiseEnServiceRedresseurNullable;
        [RequiredCustom()]
        public Nullable<DateTime> DateMiseEnServiceRedresseurNullable
        {
            get
            {
                return _dateMiseEnServiceRedresseurNullable;
            }
            set
            {
                if (_dateMiseEnServiceRedresseurNullable != value)
                {
                    this.ValidateProperty("DateMiseEnServiceRedresseurNullable", value);

                    if (value.HasValue)
                    {
                        this.DateMiseEnServiceRedresseur = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("DateMiseEnServiceRedresseur");
                    }
                    _dateMiseEnServiceRedresseurNullable = value;
                }
            }
        }

        private Nullable<DateTime> _dateControleNullable;
        [RequiredCustom()]
        public Nullable<DateTime> DateControleNullable
        {
            get
            {
                return _dateControleNullable;
            }
            set
            {
                if (_dateControleNullable != value)
                {
                    this.ValidateProperty("DateControleNullable", value);

                    if (value.HasValue)
                    {
                        this.DateControle = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("DateControle");
                    }
                    _dateControleNullable = value;
                }
            }
        }

        private Nullable<int> _tensionReglageNullable;
        [RequiredCustom()]
        public Nullable<int> TensionReglageNullable
        {
            get
            {
                return _tensionReglageNullable;
            }
            set
            {
                if (_tensionReglageNullable != value)
                {
                    this.ValidateProperty("TensionReglageNullable", value);

                    if (value.HasValue)
                    {
                        this.TensionReglage = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("TensionReglage");
                    }
                    _tensionReglageNullable = value;
                }
            }
        }

        private Nullable<decimal> _intensiteReglageNullable;
        [RequiredCustom()]
        public Nullable<decimal> IntensiteReglageNullable
        {
            get
            {
                return _intensiteReglageNullable;
            }
            set
            {
                if (_intensiteReglageNullable != value)
                {
                    this.ValidateProperty("IntensiteReglageNullable", value);

                    if (value.HasValue)
                    {
                        this.IntensiteReglage = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("IntensiteReglage");
                    }
                    _intensiteReglageNullable = value;
                }
            }
        }

        private Nullable<decimal> _longueurDeversoirNullable;
        [RequiredCustom()]
        public Nullable<decimal> LongueurDeversoirNullable
        {
            get
            {
                return _longueurDeversoirNullable;
            }
            set
            {
                if (_longueurDeversoirNullable != value)
                {
                    this.ValidateProperty("LongueurDeversoirNullable", value);

                    if (value.HasValue)
                    {
                        this.LongueurDeversoir = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("LongueurDeversoir");
                    }
                    _longueurDeversoirNullable = value;
                }
            }
        }
        #endregion
    }
}
