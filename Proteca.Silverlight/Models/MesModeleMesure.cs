using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Proteca.Silverlight.Helpers;
using System.Collections.ObjectModel;

namespace Proteca.Web.Models
{
    public partial class MesModeleMesure
    {
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew())
            {
                _numeroOrdreNullable = this.NumeroOrdre;
            }
        }

        #region Gestion des valeurs par défaut des champs obligatoires

        private Nullable<int> _numeroOrdreNullable;
        [RequiredCustom(AllowZero = true)]
        public Nullable<int> NumeroOrdreNullable
        {
            get
            {
                return _numeroOrdreNullable;
            }
            set
            {
                if (_numeroOrdreNullable != value)
                {
                    this.ValidateProperty("NumeroOrdreNullable", value);

                    if (value.HasValue)
                    {
                        this.NumeroOrdre = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("NumeroOrdre");
                    }
                    _numeroOrdreNullable = value;
                }
            }
        }

        #endregion

        public bool _isFirstLoad { get; set; }

        /// <summary>
        /// Liste des type de mesures triés par ordre de numero d'ordre
        /// </summary>
        private bool _isLoaded = false;
        public ObservableCollection<MesTypeMesure> MesTypeMesureTriees
        {
            get
            {
                if (!_isLoaded )
                {
                    this.MesTypeMesure.EntityAdded += (o, e) =>
                    {
                        this.RaisePropertyChanged("MesTypeMesureTriees");
                    };
                    this.MesTypeMesure.EntityRemoved += (o, e) =>
                    {
                        this.RaisePropertyChanged("MesTypeMesureTriees");
                    };
                    _isLoaded = true;
                }
                return new ObservableCollection<MesTypeMesure>(this.MesTypeMesure.OrderBy(t => t.IsNew()).ThenBy(t => t.NumeroOrdre));
            } 
        }
        
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == "CleUnite" && this.MesNiveauProtection.Any(n => !n.CleEquipement.HasValue && !n.ClePp.HasValue && !n.ClePortion.HasValue))
            {
                this.MesNiveauProtection.First(n => !n.CleEquipement.HasValue && !n.ClePp.HasValue && !n.ClePortion.HasValue).ForceValidateSeuil();
            }

            if (e.PropertyName == "EntityState")
            {
                if (this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
                {
                    if (this.IsNew())
                    {
                        _numeroOrdreNullable = null;
                    }
                    else
                    {
                        _numeroOrdreNullable = this.NumeroOrdre;
                    }
                    this.RaisePropertyChanged("NumeroOrdreNullable");
                }
            }
        }

    }
}
