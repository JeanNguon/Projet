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
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using System.Collections.Generic;
using System.Linq;

namespace Proteca.Web.Models
{
    public partial class PortionIntegrite
    {
        /// <summary>
        /// Si la portion intégrité a été supprimée on affiche un message sinon rien
        /// </summary>
        public string InfosPortion
        {
            get
            {
                if (this.Supprime)
                {
                    return Resource.PortionIntegrite_Supprimee;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Propriété pour le binding afin de déterminer si l'entité est nouvelle
        /// </summary>
        public Boolean IsNew
        {
            get
            {
                return this.IsNew();
            }
        }

        /// <summary>
        /// Propriété permettant d'identifier les portions sélectionnées (utilisé par l'écran de création d'action hors analyse)
        /// </summary>
        public bool IsSelected 
        {
            get;
            set;
        }

        #region Gestion de la propagation des modifications des EntityCollection

        public Boolean HasChildChanges
        {
            get { return ChildChanges.Any(); }
        }

        private List<String> _childChanges;
        public List<String> ChildChanges
        {
            get
            {
                if (_childChanges == null)
                {
                    _childChanges = new List<String>();
                }
                return _childChanges;
            }
            set { _childChanges = value; }
        }

        public void RaiseAnyDataMemberChanged(string prop)
        {
            if (prop == "PiSecteurs" || prop == "MesNiveauProtection")
            {
                if (!ChildChanges.Contains(prop))
                {
                    ChildChanges.Add(prop);
                    //this.RaiseDataMemberChanged(prop);
                }
            }
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "EntityState" && this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
            {
                ChildChanges.Clear();
            }
        }

        #endregion

        public void RaiseAnyPropertyChanged(string prop)
        {
            this.RaisePropertyChanged(prop);
        }

        /// <summary>
        /// Url de l'ensemble électrique de rattachement
        /// </summary>
        public string NaviagtionEEUrl
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
        /// Url de l'élément courant
        /// </summary>
        public string NaviagtionUrl 
        { 
            get 
            {
                return string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.PortionIntegrite.GetStringValue(),
                   ClePortion);
            } 
        }

        public override string ToString()
        {
            return Libelle;
        }

        private GeoEnsembleElectrique _geoEnsElec = null;
        public GeoEnsembleElectrique GeoEnsElec
        {
            get
            {
                if (this.EnsembleElectrique != null)
                {
                    _geoEnsElec = new GeoEnsembleElectrique() { CleEnsElectrique = this.CleEnsElectrique, Libelle = this.EnsembleElectrique.Libelle };
                }      
                return _geoEnsElec;
            }
            set
            {
                _geoEnsElec = value;
                if (value != null)
                {
                    this.CleEnsElectrique = value.CleEnsElectrique;
                }
                else
                {
                    this.CleEnsElectrique = 0;
                }
                this.RaisePropertyChanged("GeoEnsElec");
            }
        }

        private Nullable<decimal> _pointKiloDecouper;
        /// <summary>
        /// Point kilo at which to perform the "decoupage"
        /// </summary>
        public Nullable<decimal> PointKiloDecouper
        {
            get
            {
                return _pointKiloDecouper;
            }
            set
            {
                _pointKiloDecouper = value;
                this.RaisePropertyChanged("PointKiloDecouper");           
            }
        }

        private PortionIntegrite _portionCibleItem;
        /// <summary>
        /// Entity Id of the buddy portion to be assembled with.
        /// </summary>
        public PortionIntegrite PortionCibleItem
        {
            get
            {
                return _portionCibleItem;

            }
            set
            {
                _portionCibleItem = value;
                this.RaisePropertyChanged("PortionCibleItem");
            }
        }

    }
}
