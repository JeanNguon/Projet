using System.Collections.ObjectModel;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System.Linq;
using System;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.EntityServices;
using System.Windows;
using System.Collections.Generic;
using Proteca.Silverlight.Helpers;
using Jounce.Framework.Command;
using Jounce.Core.Command;
using System.ComponentModel.DataAnnotations;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Alerte entity
    /// </summary>
    [ExportAsViewModel("Alerte")]
    public class AlerteViewModel : BaseProtecaEntityViewModel<Alerte>
    {
        #region Private Members

        /// <summary>
        /// Déclaration de l'énum permettant d'afficher les types en base
        /// </summary>
        private string enumTypeAlerte = RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue();

        /// <summary>
        /// Déclaration de la variable FiltreCleEnsElec
        /// </summary>
        private int? _filtreCleEnsElec;

        /// <summary>
        /// Déclaration de la variable FiltreClePortion
        /// </summary>
        private int? _filtreClePortion;

        public ObservableCollection<Alerte> trash = new ObservableCollection<Alerte>();

        #endregion Private Members

        #region Public Members

        public ObservableCollection<AlerteDetail> DetailEntities
        {
            get
            {
                return ((AlerteService)service).DetailEntities;
            }
        }

        /// <summary>
        /// Retourne les liste des polarisations du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListTypeAlerte
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return new ObservableCollection<RefEnumValeur>(ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumTypeAlerte).OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return ServiceRegion.Entities; }
        }

        /// <summary>
        /// Indique si l'on inclue les équipements supprimés dans la recherche
        /// </summary>
        public bool IncludeDisabledAlerte { get; set; }

        /// <summary>
        /// Retourne la clé de l'ensemble électrique
        /// </summary>
        public int? FiltreCleEnsElec
        {
            get { return _filtreCleEnsElec; }
            set
            {
                _filtreCleEnsElec = value;
                RaisePropertyChanged(() => FiltreCleEnsElec);
                RaisePropertyChanged(() => GeoEnsElecPortions);
            }
        }

        /// <summary>
        /// Retourne la clé de la portion intégrité
        /// </summary>
        public int? FiltreClePortion
        {
            get { return _filtreClePortion; }
            set
            {
                _filtreClePortion = value;
                RaisePropertyChanged(() => this.FiltreClePortion);
            }
        }

        /// <summary>
        /// Retourne les GEO ensembles électrique 
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsemblesElectrique
        {
            get
            {
                if (FiltreCleRegion.HasValue)
                {
                    if (FiltreCleAgence.HasValue)
                    {
                        if (FiltreCleSecteur.HasValue)
                        {
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteur.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                            })).ToList();
                        }
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgence.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                        })).ToList();
                    }
                    return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegion.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                    })).ToList();
                }
                else
                {
                    return ServiceGeoEnsElecPortion.Entities.Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                    })).ToList();
                }
            }
        }

        /// <summary>
        /// Retourne les GEO ensembles électrique / portions 
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsElecPortions
        {
            get
            {
                if (FiltreCleEnsElec.HasValue)
                {
                    if (FiltreCleRegion.HasValue)
                    {
                        if (FiltreCleAgence.HasValue)
                        {
                            if (FiltreCleSecteur.HasValue)
                            {
                                return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteur.Value && i.CleEnsElectrique == FiltreCleEnsElec.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                            }
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgence.Value && i.CleEnsElectrique == FiltreCleEnsElec.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                            })).OrderBy(pi => pi.LibellePortion).ToList();
                        }
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegion.Value && i.CleEnsElectrique == FiltreCleEnsElec.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else
                    {
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElec.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                }
                else
                {
                    if (FiltreCleRegion.HasValue)
                    {
                        if (FiltreCleAgence.HasValue)
                        {
                            if (FiltreCleSecteur.HasValue)
                            {
                                return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteur.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                            }
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgence.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                            })).OrderBy(pi => pi.LibellePortion).ToList();
                        }
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegion.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else
                    {
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                }
            }
        }

        /// <summary>
        /// Retourne la date de debut du filtre
        /// </summary>
        [RequiredCustom]
        public Nullable<DateTime> DateMin { get; set; }

        /// <summary>
        /// Retourne la date de fin du filtre
        /// </summary>
        [RequiredCustom]
        public Nullable<DateTime> DateMax { get; set; }

        /// <summary>
        /// Retourne le point kilométrique de début du filtre
        /// </summary>
        public decimal? PkMin { get; set; }

        /// <summary>
        /// Retourne le point kilométrique de fin du filtre
        /// </summary>
        public decimal? PkMax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private bool editEnable;
        /// <summary>
        /// 
        /// </summary>
        public bool EditEnable
        {
            get { return editEnable && UserCanEdit && EntitiesCount > 0; }
            set
            {
                editEnable = value;
                RaisePropertyChanged(() => this.EditEnable);
            }
        }

        #endregion Public Members

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceRefEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> ServiceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les GEO ensembles électriques / portions
        /// </summary>
        [Import]
        public IEntityService<GeoEnsElecPortion> ServiceGeoEnsElecPortion { get; set; }

        #endregion Services

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public AlerteViewModel()
            : base()
        {
            IsAutoNavigateToFirst = false;
            EditEnable = true;

            this.OnRegionSelected += (o, e) =>
            {
                this.FiltreCleEnsElec = null;
                this.FiltreClePortion = null;
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
                RaisePropertyChanged(() => this.FiltreClePortion);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

            this.OnAgenceSelected += (o, e) =>
            {
                this.FiltreCleEnsElec = null;
                this.FiltreClePortion = null;
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
                RaisePropertyChanged(() => this.FiltreClePortion);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

            this.OnCanceled += (o, e) =>
            {
                RefreshList();
            };

            this.OnSecteurSelected += (o, e) =>
            {
                this.FiltreCleEnsElec = null;
                this.FiltreClePortion = null;
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
                RaisePropertyChanged(() => this.FiltreClePortion);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
                RaisePropertyChanged(() => this.ListTypeAlerte);

                RaisePropertyChanged(() => this.DetailEntities);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                IEnumerable<AlerteDetail> desactivatedLines = this.DetailEntities.Where(dv => dv.Alerte != null && dv.Alerte.Supprime && dv.CanDisable);
                foreach (AlerteDetail item in desactivatedLines)
                {
                    item.CanDisable = false;
                }
                RaisePropertyChanged(() => this.DetailEntities);
            };

            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.AlerteExpanderTitle));
                    EventAggregator.Publish("Alerte_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };

            this.DateMax = DateTime.Now.Date;
            this.DateMin = DateTime.Now.Date;

        }
        #endregion Constructor

        #region Override Methods

        /// <summary>
        /// Initialisation des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void initGeoPreferences()
        {
            base.initGeoPreferences();
            if (userService.CurrentUser != null)
            {
                this.FiltreCleEnsElec = userService.CurrentUser.PreferenceCleEnsembleElectrique;
                this.FiltreClePortion = userService.CurrentUser.PreferenceClePortion;
            }
        }

        /// <summary>
        ///Enregistrement des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void saveGeoPreferences()
        {
            base.saveGeoPreferences();
            if (userService.CurrentUser != null)
            {
                userService.CurrentUser.SetPreferenceCleEnsembleElectrique(this.FiltreCleEnsElec);
                userService.CurrentUser.SetPreferenceClePortion(this.FiltreClePortion);
            }
        }

        /// <summary>
        /// Méthode de recherche appellé par la commande FindCommand
        /// cette méthode appelle la méthode Find du service
        /// pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            if (!DateMin.HasValue || !DateMax.HasValue)
            {
                ErrorWindow.CreateNew(Resource.SaisieVisite_SearchEmpty.ToString());
            }
            else if (DateMin.Value.Date > DateMax.Value.Date)
            {
                ErrorWindow.CreateNew(Resource.SaisieVisite_SearchErrorDate.ToString());
            }
            else if (PkMin.HasValue && PkMax.HasValue && PkMin.Value > PkMax.Value)
            {
                ErrorWindow.CreateNew(Resource.Visite_PkIntervalleError.ToString());
            }
            else
            {
                IsBusy = true;

                saveGeoPreferences();

                ((AlerteService)this.service).FindAlerteDetailsByCriterias(FiltreCleRegion, FiltreCleAgence, FiltreCleSecteur, FiltreCleEnsElec, FiltreClePortion,
                                                                            PkMin, PkMax, DateMin.Value, DateMax.Value, IncludeDisabledAlerte, ListTypeAlerte, SearchDone);
            }


        }

        protected override void Save()
        {
            base.Save();
        }

        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// La recherche des alertes vient d'être effectuée.
        /// </summary>
        /// <param name="ex"></param>
        private void SearchDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Alerte).Name));
            }
            else
            {
                RefreshList();
            }

            IsBusy = false;
        }

        /// <summary>
        /// Rafraichissement de la liste (tout est raffraichit)
        /// </summary>
        private void RefreshList()
        {
            foreach (AlerteDetail item in this.DetailEntities)
            {
                //item.LoadedDisable = item.Supprime;
                item.Alerte = this.Entities.FirstOrDefault(a => a.CleAlerte == item.CleAlerte);
                if (item.Type == "S")
                {
                    item.CanDisable = false;
                }
            }

            this.CheckCanDisableByGeo();

            RaisePropertyChanged(() => this.DetailEntities);
            RaisePropertyChanged(() => this.Entities);

            RaisePropertyChanged(() => EntitiesCount);
            RaisePropertyChanged(() => ResultIndicator);
            RaisePropertyChanged(() => this.EditEnable);
        }

        #endregion Private Methods

        #region Autorisations

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'ajout d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanAdd()
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GESTION_ALERTES_NIV);
                string codePortee = role.RefUsrPortee.CodePortee;

                if (codePortee != RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Détermine les droist de l'utilisateur courant 
        /// sur la suppression d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanDelete()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'édition d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanEdit()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur la sauvegarde d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanSave()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// MANTIS 10815, 06/05/14, FSI : Droits spécifiques pour les utilisateurs externes
        /// Retourne si true l'utilisateur n'est pas prestataire
        /// </summary>
        /// <returns></returns>
        protected override bool GetUserCanRead()
        {
         
         
                return this.CurrentUser != null && !this.CurrentUser.EstPresta;
         
        }

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            return this.DetailEntities.Any(ad => ad.CanDisableGeo);
        }

        /// <summary>
        /// Parcours les alerteDetail pour setter les canDisable Geo en fonction des droits de l'utilisateur
        /// </summary>
        private void CheckCanDisableByGeo()
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GESTION_ALERTES_NIV);

                RefUsrPortee.ListPorteesEnum niveau = role.RefUsrPortee.GetPorteesEnum();

                foreach (AlerteDetail ad in this.DetailEntities.Where(a => a.CanDisable))
                {
                    switch (niveau)
                    {
                        case RefUsrPortee.ListPorteesEnum.National:
                            ad.CanDisableGeo = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Region:
                            ad.CanDisableGeo = ad.CleRegion.HasValue && this.CurrentUser.GeoAgence.CleRegion == ad.CleRegion.Value;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            ad.CanDisableGeo = ad.CleAgence.HasValue && this.CurrentUser.CleAgence == ad.CleAgence.Value;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                            ad.CanDisableGeo = ad.CleSecteur.HasValue && this.CurrentUser.CleSecteur == ad.CleSecteur.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion Autorisations
    }
}
