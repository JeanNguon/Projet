using System;
using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using System.Reflection;
using System.Xml.Linq;

namespace Proteca.Web.Models
{
    public partial class EqEquipementTmp : IOuvrage
    {
        #region Override
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);
            CanValid = !EstValide;
        }

        public override string ToString()
        {
            return this.Libelle;
        }
        #endregion

        #region Proprietes

        private bool _canValidGeo;
        public bool CanValidGeo
        {
            get
            {
                return _canValidGeo && CanValid;
            }
            set
            {
                _canValidGeo = value;
                this.RaisePropertyChanged("CanValidGeo");
            }
        }

        private bool _canCompleteGeo;
        public bool CanCompleteGeo
        {
            get
            {
                return _canCompleteGeo && !CanValid;
            }
            set
            {
                _canCompleteGeo = value;
                this.RaisePropertyChanged("CanCompleteGeo");
                this.RaisePropertyChanged("NavigateToTmp");
            }
        }

        public Pp Pp
        {
            get
            {
                return this.Pp2;
            }
        }

        public String LibellePortion
        {
            get
            {
                if (this.Pp2 != null && this.Pp2.PortionIntegrite != null)
                {
                    return this.Pp2.PortionIntegrite.Libelle;
                }
                return String.Empty;
            }
        }

        private bool _canValid;
        public bool CanValid
        {
            get
            {
                return _canValid;
            }
            set
            {
                _canValid = value;
                this.RaisePropertyChanged("CanValid");
                this.RaisePropertyChanged("Etat");
                this.RaisePropertyChanged("CanValidGeo");
                this.RaisePropertyChanged("CanCompleteGeo");
                this.RaisePropertyChanged("NavigateToTmp");
            }
        }

        public Uri NavigateToTmp
        {
            get
            {
                return (this.CanCompleteGeo) ? new Uri(string.Format("/{0}/{1}/{2}/IdTmp={3}",
                              MainNavigation.GestionOuvrages.GetStringValue(),
                              OuvrageNavigation.Equipement.GetStringValue(),
                              this.TypeEquipement.CodeEquipement,
                              this.CleEqTmp), UriKind.Relative) : null ;
            }
        }

        public String Etat
        {
            get
            {
                return (CanValid) ? Resource.ValidationEquipement_EtatAValider : Resource.ValidationEquipement_EtatACompleter;
            }
        }
        #endregion

        #region IOuvrage
        public void ForceRaisePropertyChanged(String propertyName)
        {
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Retourne la Pp liée à l'ouvrage
        /// </summary>
        public String CodeEquipement
        {
            get
            {
                return (this.TypeEquipement != null) ? this.TypeEquipement.CodeEquipement : String.Empty;
            }
        }

        /// <summary>
        /// Retourne la Pp liée à l'ouvrage
        /// </summary>
        public Pp PpAttachee
        {
            get
            {
                return this.Pp2;
            }
        }

        /// <summary>
        /// Retourne la première composition liée à sa Pp
        /// </summary>
        public Composition Composition
        {
            get
            {
                return this.Compositions.FirstOrDefault();
            }
        }

        /// <summary>
        /// Retourne la dernière visite
        /// </summary>
        public Visite LastVisite
        {
            get
            {
                return this.Visites.Where(v => v.IsNew() || (v.DateVisite.HasValue && v.DateVisite >= VisitePeriodeDebut && v.DateVisite <= VisitePeriodeFin)).OrderBy(v => v.IsNew()).ThenBy(v => v.IsNewInOfflineMode).ThenBy(v => v.DateVisite).LastOrDefault();
            }
        }

        public DateTime? VisitePeriodeDebut { get; set; }
        public DateTime? VisitePeriodeFin { get; set; }

        /// <summary>
        /// L'EqEquipementTmp n'a pas d'histo => pour le besoin de l'implementation de IOuvrage on retourne ici null
        /// </summary>
        /// <returns></returns>
        public Entity GetHisto()
        {
            return null;
        }

        /// <summary>
        /// Retourne le libelle avec le code de l'équipement
        /// </summary>
        public String LibelleExtended
        {
            get
            {
                string result = string.Empty;
                if (this.TypeEquipement != null)
                {
                    result = this.TypeEquipement.CodeEquipement + " - " + this.Libelle;
                }
                return result;
            }
        }

        /// <summary>
        /// Propriété d'affichage du chemin géographique de l'élément
        /// Region / Agence / Secteur / EE / Portion
        /// MANTIS 11962 FSI 25/06/2014 : Bloquage de l'export d'une tournée
        /// </summary>
        public String LibelleCheminGeo
        {
            get
            {
                string result = string.Empty;
                if (this.Pp2 != null)
                {
                    result = this.Pp2.LibelleCheminGeo;
                }
                return result;
            }
        }

        #endregion

        #region Event
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }
        #endregion

        public XElement CreateXElement()
        {
            XElement xEqEquipement = new XElement("EQ");

            xEqEquipement.Add(new XElement("CleTypeEvaluation", Composition.EnumTypeEval));
            xEqEquipement.Add(new XElement("TypeEvaluation", Composition.RefEnumValeur.LibelleCourt));

            xEqEquipement.Add(new XElement("CleEquipement", 0));
            xEqEquipement.Add(new XElement("ClePp", ClePp));
            xEqEquipement.Add(new XElement("CleTypeEq", CleTypeEq));
            xEqEquipement.Add(new XElement("TypeEquipement", TypeEquipement.CodeEquipement));
            xEqEquipement.Add(new XElement("Libelle", Libelle));

            XElement xVisites = new XElement("Visites");
            foreach (Visite visite in Visites)
            {
                xVisites.Add(visite.IsNew() ? visite.CreateXVisite() : new XElement("Visite"));
            }
            xEqEquipement.Add(xVisites);

            return xEqEquipement;
        }
    }
}
