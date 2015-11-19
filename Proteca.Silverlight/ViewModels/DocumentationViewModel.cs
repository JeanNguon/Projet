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
using Jounce.Core.Command;
using Jounce.Framework.Command;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Services;
using Proteca.Silverlight.Models;
using System.Collections;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Documentation entity
    /// </summary>
    [ExportAsViewModel("Documentation")]
    public class DocumentationViewModel : BaseProtecaEntityViewModel<Document>
    {
        #region public properties

        /// <summary>
        /// Inclut les postes gaz
        /// </summary>
        private bool _includePostegaz;
        public bool IncludePosteGaz
        {
            get
            {
                return _includePostegaz;
            }
            set
            {
                _includePostegaz = value;
                RaisePropertyChanged(() => GeoEnsemblesElectrique);
                RaisePropertyChanged(() => GeoEnsElecPortions);
            }
        }

        /// <summary>
        /// Inclut les stations
        /// </summary>
        private bool _includestation;
        public bool IncludeStation
        {
            get
            {
                return _includestation;
            }
            set
            {
                _includestation = value;
                RaisePropertyChanged(() => GeoEnsemblesElectrique);
                RaisePropertyChanged(() => GeoEnsElecPortions);
            }
        }

        /// <summary>
        /// filtre code ouvrage
        /// </summary>
        public string CodeOuvrage { get; set; }


        /// <summary>
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return ServiceRegion.Entities; }
        }

        /// <summary>
        /// Liste des ouvrages
        /// </summary>
        public ObservableCollection<TypeDocument> Ouvrages
        {
            get
            {
                return new ObservableCollection<TypeDocument>(((TypeDocumentService)serviceTypeDocument).Entities.Where(e => !this.IsFiltreOuvrage || e.Libelle != Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.EQUIPEMENTS.GetStringValue()));
            }
        }


        TypeDocument _ouvrage;
        /// <summary>
        /// Ouvrage sélectionné
        /// </summary>
        public TypeDocument Ouvrage
        {
            get
            {
                return _ouvrage;
            }
            set
            {
                _ouvrage = value;
                this.RaisePropertyChanged(() => this.Ouvrage);
                this.RaisePropertyChanged(() => this.Dossiers);

            }
        }

        private TypeDocument _dossier;
        /// <summary>
        /// Dossier sélectionné
        /// </summary>
        public TypeDocument Dossier
        {
            get
            {
                return _dossier;
            }
            set
            {
                _dossier = value;
                this.RaisePropertyChanged(() => this.Dossier);
                this.RaisePropertyChanged(() => this.Designations);
            }
        }

        /// <summary>
        /// Liste des dossiers de l'ouvrage sélectionné
        /// </summary>
        public List<TypeDocument> Dossiers
        {
            get
            {
                if (Ouvrage != null)
                    return Ouvrage.Entities;

                return null;
            }

        }


        TypeDocument _designation;
        /// <summary>
        /// Désignation sélectionnée
        /// </summary>
        public TypeDocument Designation
        {
            get
            {
                return _designation;
            }
            set
            {
                _designation = value;
                this.RaisePropertyChanged(() => this.Designation);
            }
        }

        /// <summary>
        /// Liste des désignation du type de dossier sélectionné
        /// </summary>
        public List<TypeDocument> Designations
        {
            get
            {
                if (this.Dossier != null)
                {
                    return Dossier.Entities;
                }

                return null;
            }
        }

        /// <summary>
        /// Liste des toutes les désignations correspondant aux filtres sélectionnés
        /// </summary>
        public List<TypeDocument> AllDesignations
        {
            get
            {
                return this.Ouvrages.Where(o => this.Ouvrage == null || o.Cle == this.Ouvrage.Cle)
                    .SelectMany(o => o.Entities.Where(d => this.Dossier == null || d.Cle == this.Dossier.Cle))
                    .SelectMany(d => d.Entities.Where(de => this.Designation == null || this.Designation.Cle == de.Cle)).ToList();
            }
        }

        /// <summary>
        /// Liste des types equipements
        /// </summary>
        public ObservableCollection<TypeEquipement> TypeEquipement
        {
            get { return serviceTypeEquipement.Entities; }
        }

        /// <summary>
        /// Retourne le code type equipement
        /// </summary>
        private string _filtreTypeEquipement;
        public string FiltreTypeEquipement
        {
            get { return _filtreTypeEquipement; }
            set
            {
                _filtreTypeEquipement = value;
                RaisePropertyChanged(() => FiltreTypeEquipement);
            }
        }

        /// <summary>
        /// Retourne la clé de l'ensemble électrique
        /// </summary>
        private int? _filtreCleEnsElec;
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
        private int? _filtreClePortion;
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
        /// Filtre ouvrage
        /// </summary>
        private bool _isFiltreOuvrage = false;
        public bool IsFiltreOuvrage
        {
            get { return _isFiltreOuvrage; }
            set
            {
                _isFiltreOuvrage = value;
                if (!IsFiltreOuvrage && this.Ouvrages != null && this.Ouvrages.Any())
                {
                    Ouvrage = this.Ouvrages.FirstOrDefault(t => t.Libelle == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.EQUIPEMENTS.GetStringValue());
                }
                else if (_ouvrage != null && _ouvrage.Libelle == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.EQUIPEMENTS.GetStringValue())
                {
                    Ouvrage = null;
                }
                RaisePropertyChanged(() => this.IsFiltreOuvrage);
                RaisePropertyChanged(() => this.IsFiltreEquipement);
                RaisePropertyChanged(() => this.Ouvrages);
                RaisePropertyChanged(() => this.Ouvrage);
            }
        }

        private bool _isResultatEquipement;
        public bool IsResultatEquipement
        {
            get { return _isResultatEquipement; }
            set
            {
                _isResultatEquipement = value;
                RaisePropertyChanged(() => this.IsResultatEquipement);
            }
        }


        /// <summary>
        /// Filtre Equipement
        /// </summary>
        public bool IsFiltreEquipement
        {
            get { return !IsFiltreOuvrage; }
        }

        /// <summary>
        /// Retourne les GEO ensembles électrique du service EnsElec
        /// </summary>
        public List<GeoEnsembleElectrique> GeoEnsemblesElectrique
        {
            get
            {

                if (FiltreCleRegion != null)
                {
                    if (FiltreCleAgence != null)
                    {
                        if (FiltreCleSecteur != null)
                        {
                            return serviceGeoEnsElec.Entities.Where(i => i.CleSecteur == FiltreCleSecteur && (!i.EnumStructureCplx.HasValue
                                                                    || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                    || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                             .ToList();
                        }
                        return serviceGeoEnsElec.Entities.Where(i => i.CleAgence == FiltreCleAgence && (!i.EnumStructureCplx.HasValue
                                                                    || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                    || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                         .Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                                             {
                                                                 return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                                             }))
                                                         .ToList();
                    }
                    return serviceGeoEnsElec.Entities.Where(i => i.CleRegion == FiltreCleRegion && (!i.EnumStructureCplx.HasValue
                                                                || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                     .Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                                         {
                                                             return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                                         }))
                                                     .ToList();
                }
                else
                {
                    return serviceGeoEnsElec.Entities.Where(i => (!i.EnumStructureCplx.HasValue
                                                                || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                     .Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                                         {
                                                             return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                                         }))
                                                     .ToList();
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
                                return serviceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElec.Value && i.CleSecteur == FiltreCleSecteur.Value && (!i.EnumStructureCplx.HasValue
                                                                                || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                                || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                                        .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                                                        {
                                                                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                                                        }))
                                                                        .OrderBy(pi => pi.LibellePortion)
                                                                        .ToList();
                            }
                            return serviceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElec.Value && i.CleAgence == FiltreCleAgence.Value && (!i.EnumStructureCplx.HasValue
                                                                            || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                            || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                                    .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                                                    {
                                                                        return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                                                    }))
                                                                    .OrderBy(pi => pi.LibellePortion)
                                                                    .ToList();
                        }
                        return serviceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElec.Value && i.CleRegion == FiltreCleRegion.Value && (!i.EnumStructureCplx.HasValue
                                                                        || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                        || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                                .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                                                {
                                                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                                                }))
                                                                .OrderBy(pi => pi.LibellePortion)
                                                                .ToList();
                    }
                    else
                    {
                        return serviceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElec.Value && (!i.EnumStructureCplx.HasValue
                                                                        || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                        || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                                .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                                                {
                                                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                                                }))
                                                                .OrderBy(pi => pi.LibellePortion)
                                                                .ToList();
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
                                return serviceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteur.Value && (!i.EnumStructureCplx.HasValue
                                                                                || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                                || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                                        .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                                                            {
                                                                                return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                                                            }))
                                                                        .OrderBy(pi => pi.LibellePortion)
                                                                        .ToList();
                            }
                            return serviceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgence.Value && (!i.EnumStructureCplx.HasValue
                                                                            || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                            || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                                    .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                                                        {
                                                                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                                                        }))
                                                                    .OrderBy(pi => pi.LibellePortion)
                                                                    .ToList();
                        }
                        return serviceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegion.Value && (!i.EnumStructureCplx.HasValue
                                                                        || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                        || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                                .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                                                    {
                                                                        return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                                                    }))
                                                                .OrderBy(pi => pi.LibellePortion)
                                                                .ToList();
                    }
                    else
                    {
                        return serviceGeoEnsElecPortion.Entities.Where(i => (!i.EnumStructureCplx.HasValue
                                                                        || (IncludeStation && i.EnumStructureCplx.Value == 23)
                                                                        || (IncludePosteGaz && i.EnumStructureCplx.Value == 24)))
                                                                .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                                                    {
                                                                        return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                                                    }))
                                                                .OrderBy(pi => pi.LibellePortion)
                                                                .ToList();
                    }
                }
            }
        }

        private ObservableCollection<Documentation> _documents;
        public ObservableCollection<Documentation> Documents
        {
            get
            {
                return _documents;
            }
            set
            {
                _documents = value;
                RaisePropertyChanged("Documents");
                RaisePropertyChanged("HasDocuments");
            }
        }

        public bool HasDocuments
        {
            get
            {
                return Documents != null && Documents.Any();
            }
        }

        private ObservableCollection<GeoEnsElecPortionEqPp> _geoEnsElecPortionEqPpEntities;
        #endregion

        #region constructor

        public DocumentationViewModel()
            : base()
        {
            this.OnAllDocumentationServicesLoaded += new EventHandler(AllDocumentationServicesLoaded);

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
                // MAJ des services
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
                RaisePropertyChanged(() => this.TypeEquipement);
                RaisePropertyChanged(() => this.Ouvrages);
                this.Documents = null;
                this.IsFiltreOuvrage = false;
            };
            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resource.Documentation_TitreExpander));
                    EventAggregator.Publish("Documentation_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };
        }
        #endregion

        #region Events
        public EventHandler OnAllDocumentationServicesLoaded;
        #endregion

        #region Override methods

        /// <summary>
        /// Suite à la modification de la matrice des droits => ce droit est universel
        /// </summary>
        /// <returns></returns>
        protected override bool GetUserCanRead()
        {
            //bool res = true;
            //if (this.CurrentUser!=null && this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.ACCES_DOC).RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
            //{
            //    res = false;
            //}
            //return res;
            return true;
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
        /// Initialisation des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void initGeoPreferences()
        {
            base.initGeoPreferences();
            if (this.CurrentUser != null)
            {
                this.FiltreCleEnsElec =
                    (this.CurrentUser.PreferenceCleEnsembleElectrique != null && this.GeoEnsemblesElectrique.Any(ee => ee.CleEnsElectrique == this.CurrentUser.PreferenceCleEnsembleElectrique)) ?
                    this.CurrentUser.PreferenceCleEnsembleElectrique : null;
                this.FiltreClePortion =
                    (this.CurrentUser.PreferenceClePortion != null && this.GeoEnsElecPortions.Any(p => p.ClePortion == this.CurrentUser.PreferenceClePortion)) ?
                    this.CurrentUser.PreferenceClePortion : null;
            }
        }

        /// <summary>
        /// Méthode de recherche appellé par la commande FindCommand
        /// cette méthode appelle la méthode Find du service
        /// pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            IsBusy = true;

            saveGeoPreferences();

            List<System.Linq.Expressions.Expression<Func<GeoEnsElecPortionEqPp, bool>>> filtres = new List<System.Linq.Expressions.Expression<Func<GeoEnsElecPortionEqPp, bool>>>();

            if (this.FiltreCleRegion != null)
            {
                filtres.Add(u => u.CleRegion == this.FiltreCleRegion);
            }
            if (this.FiltreCleAgence != null)
            {
                filtres.Add(u => u.CleAgence == this.FiltreCleAgence);
            }
            if (this.FiltreCleSecteur != null)
            {
                filtres.Add(u => u.CleSecteur == this.FiltreCleSecteur);
            }
            if (this.FiltreCleEnsElec != null)
            {
                filtres.Add(u => u.CleEnsElectrique == this.FiltreCleEnsElec);
            }
            if (this.FiltreClePortion != null)
            {
                filtres.Add(u => u.ClePortion == this.FiltreClePortion);
            }
            if (this.FiltreTypeEquipement != null && this.FiltreTypeEquipement != "PP")
            {
                filtres.Add(u => u.CodeEquipement == this.FiltreTypeEquipement);
            }
            if (this.CodeOuvrage != null)
            {
                filtres.Add(u => u.Code == this.CodeOuvrage);
            }


            CleOuvrage? typeOuvrage = null;
            int? cleOuvrage = null;
            string relativePath = null;

            if (this.Designation != null)
            {
                relativePath = this.Designation.ServerRelativeUrl + "/" + this.Designation.Libelle;
            }
            else if (this.Dossier != null)
            {
                relativePath = this.Dossier.ServerRelativeUrl + "/" + this.Dossier.Libelle;
            }
            else if (this.Ouvrage != null)
            {
                relativePath = this.Ouvrage.ServerRelativeUrl + "/" + this.Ouvrage.Libelle;
            }

            int callbackServiceCount = 2;
            object lockObj = new object();

            if (this.IsFiltreOuvrage)
            {
                callbackServiceCount = 1;
            }

            else if (this.IsFiltreEquipement)
            {
                // Recherche de tous les documents correspondant
                serviceGeoEnsElecPortionEqPp.FindEntities(filtres, ex =>
                {
                    if (ex != null)
                    {
                        // erreur de récupération des données
                        Logger.Log(LogSeverity.Error, this.GetType().FullName, ex.ToString());
                        ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Documentation).Name));
                    }
                    lock (lockObj)
                    {
                        callbackServiceCount--;
                        if (callbackServiceCount == 0)
                        {
                            if (OnAllDocumentationServicesLoaded != null)
                            {
                                OnAllDocumentationServicesLoaded(this, null);
                            }
                        }
                    }
                });
            }

            // Recherche de toutes les nomenclatures correspondantes
            ((DocumentService)this.service).GetEntitiesByCleOuvrage(exception =>
            {
                if (exception == null)
                {
                    _geoEnsElecPortionEqPpEntities = this.serviceGeoEnsElecPortionEqPp.Entities;
                }
                else
                {
                    // erreur de récupération des données dans sharepoint
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, exception.ToString());
                    ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Documentation).Name));
                }
                lock (lockObj)
                {
                    callbackServiceCount--;
                    if (callbackServiceCount == 0)
                    {
                        if (OnAllDocumentationServicesLoaded != null)
                        {
                            OnAllDocumentationServicesLoaded(this, null);
                        }
                    }
                }

            }, typeOuvrage, cleOuvrage, relativePath);

        }



        private void AllDocumentationServicesLoaded(object sender, EventArgs args)
        {
            //List de la doc a produire
            //  List<Documentation> listAvecDocument = new List<Documentation>();

            List<Documentation> listComplete = new List<Documentation>();
            List<Documentation> listDocumentationSansDoc = new List<Documentation>();

            if ((this.IsFiltreEquipement && this.serviceGeoEnsElecPortionEqPp.Entities.Any() || (this.IsFiltreOuvrage && (serviceGeoEnsElecPortion.Entities.Any() || serviceGeoEnsElec.Entities.Any()))))
            {

                #region New Code

                //si on est en mode PP eq
                if (this.IsFiltreEquipement)
                {
                    TypeEquipement TypePP = serviceTypeEquipement.Entities.Where(teq => teq.CodeEquipement == "PP").FirstOrDefault();
                    
                    //list filtrant les recherches par eq/pp/portion/EE
                    var listEq=serviceGeoEnsElecPortionEqPp.Entities.Where(eqPp => eqPp.CleEquipement.HasValue).ToList();
                    var listPp = serviceGeoEnsElecPortionEqPp.Entities.Where(eqPp => eqPp.ClePp.HasValue).GroupBy(eqPp => eqPp.ClePp.Value).Select(grp => grp.First()).ToList();

                    // List de tous les chemins (Designation . EqPp)
                    foreach (TypeDocument d in AllDesignations.Where(d => d.TypeOuvrage == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.EQUIPEMENTS.GetStringValue()))
                    {
                        if (FiltreTypeEquipement != "PP")
                        {
                            foreach (var eq in listEq)
                            {
                                Documentation docEq = new Documentation(eq, d, CleOuvrage.CleEquipement);
                                docEq.LibelleTypeEquipement = eq.CodeEquipement != null ? this.TypeEquipement.First(t => t.CodeEquipement == eq.CodeEquipement).Libelle : null;
                                docEq.NumeroOrdre = eq.CodeEquipement != null ? this.TypeEquipement.First(t => t.CodeEquipement == eq.CodeEquipement).NumeroOrdre : (int?)null;
                                docEq.Region = this.Regions.First(r => r.CleRegion == eq.CleRegion).LibelleRegion;
                                listDocumentationSansDoc.Add(docEq);
                            }
                        }
                        if (string.IsNullOrEmpty(FiltreTypeEquipement) || (FiltreTypeEquipement == "PP"))
                        {
                            foreach (var pp in listPp)
                            {
                                Documentation docPp = new Documentation(pp, d, CleOuvrage.ClePP);
                                docPp.LibelleTypeEquipement = TypePP.CodeEquipement != null ? TypePP.Libelle : null;
                                docPp.NumeroOrdre = TypePP.CodeEquipement != null ? TypePP.NumeroOrdre : (int?)null;
                                docPp.Region = this.Regions.First(r => r.CleRegion == pp.CleRegion).LibelleRegion;
                                listDocumentationSansDoc.Add(docPp);
                            }
                        }
                    }

                    foreach (Documentation docVide in listDocumentationSansDoc.Where(doc => doc.cleOuvrage == CleOuvrage.CleEquipement))
                    {
                        listComplete.AddRange(this.Entities
                            .Where(e => e.TypeOuvrage == docVide.cleOuvrage && docVide.CleEquipement == e.CleOuvrage && docVide.CleEquipement != null && docVide.Designation.Libelle.ToUpper() == e.Designation.Libelle.ToUpper())
                            .Select(match => Documentation.AddDocToDocumentation(docVide, match))
                            .DefaultIfEmpty(docVide));
                    }

                    foreach (Documentation docVide in listDocumentationSansDoc.Where(doc => doc.cleOuvrage == CleOuvrage.ClePP))
                    {
                        listComplete.AddRange(this.Entities
                            .Where(e => e.TypeOuvrage == CleOuvrage.ClePP && docVide.ClePp == e.CleOuvrage && docVide.Designation.Libelle.ToUpper() == e.Designation.Libelle.ToUpper())
                            .Select(match=>Documentation.AddDocToDocumentation(docVide, match))
                            .DefaultIfEmpty(docVide));
                    }

                }

                else if (this.IsFiltreOuvrage) // Mode Portion EqElec
                {
                    //list filtrant les recherches par eq/pp/portion/EE
                    List<GeoEnsElecPortion> listPortion = new List<GeoEnsElecPortion>();
                    //List<GeoEnsElecPortion> listEE = new List<GeoEnsElecPortion>();

                    List<GeoEnsElecPortion> listGeoEnsElecPortion = GeoEnsElecPortions;
                    if (this.FiltreClePortion.HasValue)
                    {
                        listPortion = GeoEnsElecPortions.Where(gep => gep.ClePortion == this.FiltreClePortion.Value).ToList();
                    }
                    var listEE = GeoEnsElecPortions.GroupBy(a=>a.CleEnsElectrique).Select(grp=>grp.First()).ToList();
                    if (this.FiltreCleEnsElec.HasValue)
                    {
                        listEE = listEE.Where(ge => ge.CleEnsElectrique == this.FiltreCleEnsElec).ToList();
                    }


                    // List de tous les chemins (Designation . Portions/EE)
                    foreach (TypeDocument d in AllDesignations.Where(d => d.TypeOuvrage == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.PORTIONS.GetStringValue()))
                    {
                        foreach (GeoEnsElecPortion portion in listPortion)
                        {
                            Documentation docPortion = new Documentation(portion, d, CleOuvrage.ClePortion);                 
                            docPortion.NumeroOrdre = null;
                            docPortion.Region = this.Regions.First(r => r.CleRegion == portion.CleRegion).LibelleRegion;
                            listDocumentationSansDoc.Add(docPortion);
                        }
                    }

                    foreach (TypeDocument d in AllDesignations.Where(d => d.TypeOuvrage == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.ENSEMBLES_ELECTRIQUES.GetStringValue()))
                    {
                        foreach (GeoEnsElecPortion EE in listEE)
                        {
                            Documentation docEe = new Documentation(EE, d, CleOuvrage.CleEnsembleElectrique);
                            docEe.ClePortion = null;
                            docEe.LibellePortion = "";
                            docEe.NumeroOrdre = null;
                            docEe.Region = this.Regions.First(r => r.CleRegion == EE.CleRegion).LibelleRegion;
                            listDocumentationSansDoc.Add(docEe);
                        }
                    }

                    foreach (Documentation docVide in listDocumentationSansDoc.Where(doc => doc.cleOuvrage == CleOuvrage.ClePortion))
                    {
                        listComplete.AddRange(this.Entities
                            .Where(e => e.TypeOuvrage == docVide.cleOuvrage 
                                && docVide.CleEquipement == e.CleOuvrage 
                                && docVide.Designation.Libelle.ToUpper() == e.Designation.Libelle.ToUpper())
                            .Select(match=>Documentation.AddDocToDocumentation(docVide, match))
                            .DefaultIfEmpty(docVide));
                    }

                    foreach (Documentation docVide in listDocumentationSansDoc.Where(doc => doc.cleOuvrage == CleOuvrage.CleEnsembleElectrique))
                    {
                        listComplete.AddRange(this.Entities
                            .Where(e => e.TypeOuvrage == CleOuvrage.CleEnsembleElectrique 
                                && docVide.CleEnsElectrique == e.CleOuvrage 
                                && docVide.Designation.Libelle.ToUpper() == e.Designation.Libelle.ToUpper())
                            .Select(match=>Documentation.AddDocToDocumentation(docVide, match))
                            .DefaultIfEmpty(docVide));
                    }
                }

                #endregion
                #region old code

                //if (this.IsFiltreEquipement)
                //{
                //    TypeEquipement TypePP = serviceTypeEquipement.Entities.Where(teq => teq.CodeEquipement == "PP").FirstOrDefault();

                //    if (this.FiltreTypeEquipement != "PP")
                //    {
                //        listAvecDocument = (from d in listEqPp.Distinct(new InlineEqualityComparer<GeoEnsElecPortionEqPp>((a, b) =>
                //                                {
                //                                    return a.CleEquipement.Equals(b.CleEquipement);
                //                                }))
                //                            from e in this.Entities
                //                            where (e.TypeOuvrage == CleOuvrage.CleEquipement && d.CleEquipement == e.CleOuvrage) && d.CleEquipement != null
                //                            select new Documentation()
                //                            {
                //                                Cle = d.Id,
                //                                CodeEquipement = d.CodeEquipement,
                //                                Designation = e.Designation,
                //                                Dossier = e.Designation.TypeDossier,
                //                                TypeOuvrage = e.TypeOuvrage,
                //                                NumeroVersion = e.NumeroVersion,
                //                                Libelle = e.Libelle,
                //                                DateEnregistrement = e.DateEnregistrement,
                //                                DocumentUrl = e.DocumentUrl,
                //                                ClePp = d.ClePp,
                //                                CleEnsElectrique = d.CleEnsElectrique,
                //                                ClePortion = d.ClePortion,
                //                                CleEquipement = d.CleEquipement,
                //                                LibelleEe = d.LibelleEe,
                //                                LibellePortion = d.LibellePortion,
                //                                LibelleEquipement = d.LibelleEquipement,
                //                                LibellePp = d.LibellePp,
                //                                LibelleTypeEquipement = d.CodeEquipement != null ? this.TypeEquipement.First(t => t.CodeEquipement == d.CodeEquipement).Libelle : null,
                //                                NumeroOrdre = d.CodeEquipement != null ? this.TypeEquipement.First(t => t.CodeEquipement == d.CodeEquipement).NumeroOrdre : (int?)null,
                //                                Code = d.Code,
                //                                Region = this.Regions.First(r => r.CleRegion == d.CleRegion).LibelleRegion
                //                            }).ToList();

                //        listComplete = (from e in listEqPp.Distinct(new InlineEqualityComparer<GeoEnsElecPortionEqPp>((a, b) =>
                //                            {
                //                                return a.CleEquipement.Equals(b.CleEquipement);
                //                            }))
                //                        from d in this.AllDesignations
                //                        where !listAvecDocument.Any(l => l.CleEquipement == e.CleEquipement && d.Cle == l.Designation.Cle) && e.CleEquipement != null
                //                        select new Documentation()
                //                        {
                //                            Cle = e.Id,
                //                            CodeEquipement = e.CodeEquipement,
                //                            Designation = d,
                //                            Dossier = d.Parent.Libelle,
                //                            TypeOuvrage = null,
                //                            NumeroVersion = null,
                //                            Libelle = null,
                //                            DateEnregistrement = null,
                //                            DocumentUrl = null,
                //                            ClePp = e.ClePp,
                //                            CleEnsElectrique = e.CleEnsElectrique,
                //                            ClePortion = e.ClePortion,
                //                            CleEquipement = e.CleEquipement,
                //                            LibelleEe = e.LibelleEe,
                //                            LibellePortion = e.LibellePortion,
                //                            LibelleEquipement = e.LibelleEquipement,
                //                            LibellePp = e.LibellePp,
                //                            LibelleTypeEquipement = e.CodeEquipement != null ? this.TypeEquipement.First(t => t.CodeEquipement == e.CodeEquipement).Libelle : null,
                //                            NumeroOrdre = e.CodeEquipement != null ? this.TypeEquipement.First(t => t.CodeEquipement == e.CodeEquipement).NumeroOrdre : (int?)null,
                //                            Code = e.Code,
                //                            Region = this.Regions.First(r => r.CleRegion == e.CleRegion).LibelleRegion
                //                        }).ToList();
                //    }
                //    if (String.IsNullOrEmpty(this.FiltreTypeEquipement) || this.FiltreTypeEquipement == "PP")
                //    {
                //        listAvecDocument = listAvecDocument.Union((from d in listEqPp.Distinct(new InlineEqualityComparer<GeoEnsElecPortionEqPp>((a, b) =>
                //                                                                                            {
                //                                                                                                return a.ClePp.Equals(b.ClePp);
                //                                                                                            }))
                //                                                   from e in this.Entities
                //                                                   where (e.TypeOuvrage == CleOuvrage.ClePP && d.ClePp == e.CleOuvrage)
                //                                                   select new Documentation()
                //                                                   {
                //                                                       Cle = d.Id,
                //                                                       CodeEquipement = "PP",
                //                                                       Designation = e.Designation,
                //                                                       Dossier = e.Designation.TypeDossier,
                //                                                       TypeOuvrage = e.TypeOuvrage,
                //                                                       NumeroVersion = e.NumeroVersion,
                //                                                       Libelle = e.Libelle,
                //                                                       DateEnregistrement = e.DateEnregistrement,
                //                                                       DocumentUrl = e.DocumentUrl,
                //                                                       ClePp = d.ClePp,
                //                                                       CleEnsElectrique = d.CleEnsElectrique,
                //                                                       ClePortion = d.ClePortion,
                //                                                       CleEquipement = d.ClePp,
                //                                                       LibelleEe = d.LibelleEe,
                //                                                       LibellePortion = d.LibellePortion,
                //                                                       LibelleEquipement = d.LibellePp,
                //                                                       LibellePp = d.LibellePp,
                //                                                       LibelleTypeEquipement = TypePP.CodeEquipement != null ? TypePP.Libelle : null,
                //                                                       NumeroOrdre = TypePP.CodeEquipement != null ? TypePP.NumeroOrdre : (int?)null,
                //                                                       Code = d.Code,
                //                                                       Region = this.Regions.First(r => r.CleRegion == d.CleRegion).LibelleRegion
                //                                                   }).ToList()).ToList();

                //        listComplete = listComplete.Union((from e in listEqPp.Distinct(new InlineEqualityComparer<GeoEnsElecPortionEqPp>((a, b) =>
                //                                                {
                //                                                    return a.ClePp.Equals(b.ClePp);
                //                                                }))
                //                                           from d in this.AllDesignations
                //                                           where !listAvecDocument.Any(l => l.ClePp == e.ClePp && d.Cle == l.Designation.Cle)
                //                                           select new Documentation()
                //                                           {
                //                                               Cle = e.Id,
                //                                               CodeEquipement = "PP",
                //                                               Designation = d,
                //                                               Dossier = d.Parent.Libelle,
                //                                               TypeOuvrage = null,
                //                                               NumeroVersion = null,
                //                                               Libelle = null,
                //                                               DateEnregistrement = null,
                //                                               DocumentUrl = null,
                //                                               ClePp = e.ClePp,
                //                                               CleEnsElectrique = e.CleEnsElectrique,
                //                                               ClePortion = e.ClePortion,
                //                                               CleEquipement = e.ClePp,
                //                                               LibelleEe = e.LibelleEe,
                //                                               LibellePortion = e.LibellePortion,
                //                                               LibelleEquipement = e.LibellePp,
                //                                               LibellePp = e.LibellePp,
                //                                               LibelleTypeEquipement = TypePP.CodeEquipement != null ? TypePP.Libelle : null,
                //                                               NumeroOrdre = TypePP.CodeEquipement != null ? TypePP.NumeroOrdre : (int?)null,
                //                                               Code = e.Code,
                //                                               Region = this.Regions.First(r => r.CleRegion == e.CleRegion).LibelleRegion
                //                                           }).ToList()).ToList();
                //    }
                //}
                //else if (this.IsFiltreOuvrage)
                //{
                //    List<GeoEnsElecPortion> listGeoEnsElecPortion = GeoEnsElecPortions;
                //    if (this.FiltreClePortion.HasValue)
                //    {
                //        listGeoEnsElecPortion = GeoEnsElecPortions.Where(gep => gep.ClePortion == this.FiltreClePortion.Value).ToList();
                //    }
                //    List<GeoEnsElecPortion> listGeoEnsElec = GeoEnsElecPortions.Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                //                                                                                        {
                //                                                                                            return a.CleEnsElectrique.Equals(b.CleEnsElectrique);
                //                                                                                        })).ToList();
                //    if (this.FiltreCleEnsElec.HasValue)
                //    {
                //        listGeoEnsElec = listGeoEnsElec.Where(ge => ge.CleEnsElectrique == this.FiltreCleEnsElec).ToList();
                //    }

                //    if (this.Ouvrage == null || this.Ouvrage.Libelle == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.PORTIONS.GetStringValue())
                //    {
                //        listAvecDocument = (from d in listGeoEnsElecPortion
                //                            from e in this.Entities
                //                            where (e.TypeOuvrage == CleOuvrage.ClePortion && d.ClePortion == e.CleOuvrage)
                //                            select new Documentation()
                //                            {
                //                                Cle = d.Id,
                //                                CodeEquipement = null,
                //                                Designation = e.Designation,
                //                                Dossier = e.Designation.TypeDossier,
                //                                TypeOuvrage = e.TypeOuvrage,
                //                                NumeroVersion = e.NumeroVersion,
                //                                Libelle = e.Libelle,
                //                                DateEnregistrement = e.DateEnregistrement,
                //                                DocumentUrl = e.DocumentUrl,
                //                                ClePp = null,
                //                                CleEnsElectrique = d.CleEnsElectrique,
                //                                ClePortion = d.ClePortion,
                //                                CleEquipement = null,
                //                                LibelleEe = d.LibelleEe,
                //                                LibellePortion = d.LibellePortion,
                //                                LibelleEquipement = null,
                //                                LibellePp = null,
                //                                LibelleTypeEquipement = null,
                //                                NumeroOrdre = null,
                //                                Code = d.Code,
                //                                Region = this.Regions.First(r => r.CleRegion == d.CleRegion).LibelleRegion
                //                            }).ToList();

                //        listComplete = (from e in listGeoEnsElecPortion
                //                        from d in this.AllDesignations.Where(o => o.TypeOuvrage == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.PORTIONS.GetStringValue())
                //                        where !listAvecDocument.Any(l => l.ClePortion == e.ClePortion && d.Cle == l.Designation.Cle)
                //                        select new Documentation()
                //                        {
                //                            Cle = e.Id,
                //                            CodeEquipement = null,
                //                            Designation = d,
                //                            Dossier = d.Parent.Libelle,
                //                            TypeOuvrage = null,
                //                            NumeroVersion = null,
                //                            Libelle = null,
                //                            DateEnregistrement = null,
                //                            DocumentUrl = null,
                //                            ClePp = null,
                //                            CleEnsElectrique = e.CleEnsElectrique,
                //                            ClePortion = e.ClePortion,
                //                            CleEquipement = null,
                //                            LibelleEe = e.LibelleEe,
                //                            LibellePortion = e.LibellePortion,
                //                            LibelleEquipement = null,
                //                            LibellePp = null,
                //                            LibelleTypeEquipement = null,
                //                            NumeroOrdre = null,
                //                            Code = e.Code,
                //                            Region = this.Regions.First(r => r.CleRegion == e.CleRegion).LibelleRegion
                //                        }).ToList();
                //    }

                //    if (this.Ouvrage == null || this.Ouvrage.Libelle == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.ENSEMBLES_ELECTRIQUES.GetStringValue())
                //    {
                //        listAvecDocument = listAvecDocument.Union((from d in listGeoEnsElec
                //                                                   from e in this.Entities
                //                                                   where (e.TypeOuvrage == CleOuvrage.CleEnsembleElectrique && d.CleEnsElectrique == e.CleOuvrage)
                //                                                   select new Documentation()
                //                                                   {
                //                                                       Cle = d.Id,
                //                                                       CodeEquipement = null,
                //                                                       Designation = e.Designation,
                //                                                       Dossier = e.Designation.TypeDossier,
                //                                                       TypeOuvrage = e.TypeOuvrage,
                //                                                       NumeroVersion = e.NumeroVersion,
                //                                                       Libelle = e.Libelle,
                //                                                       DateEnregistrement = e.DateEnregistrement,
                //                                                       DocumentUrl = e.DocumentUrl,
                //                                                       ClePp = null,
                //                                                       CleEnsElectrique = d.CleEnsElectrique,
                //                                                       ClePortion = null,
                //                                                       CleEquipement = null,
                //                                                       LibelleEe = d.LibelleEe,
                //                                                       LibellePortion = "",
                //                                                       LibelleEquipement = null,
                //                                                       LibellePp = null,
                //                                                       LibelleTypeEquipement = null,
                //                                                       NumeroOrdre = null,
                //                                                       Code = d.Code,
                //                                                       Region = this.Regions.First(r => r.CleRegion == d.CleRegion).LibelleRegion
                //                                                   }).ToList()).ToList();

                //        listComplete = listComplete.Union((from e in listGeoEnsElec
                //                                           from d in this.AllDesignations.Where(o => o.TypeOuvrage == Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.ENSEMBLES_ELECTRIQUES.GetStringValue())
                //                                           where !listAvecDocument.Any(l => !l.ClePortion.HasValue && l.CleEnsElectrique == e.CleEnsElectrique && d.Cle == l.Designation.Cle)
                //                                           select new Documentation()
                //                                           {
                //                                               Cle = e.Id,
                //                                               CodeEquipement = null,
                //                                               Designation = d,
                //                                               Dossier = d.Parent.Libelle,
                //                                               TypeOuvrage = null,
                //                                               NumeroVersion = null,
                //                                               Libelle = null,
                //                                               DateEnregistrement = null,
                //                                               DocumentUrl = null,
                //                                               ClePp = null,
                //                                               CleEnsElectrique = e.CleEnsElectrique,
                //                                               ClePortion = null,
                //                                               CleEquipement = null,
                //                                               LibelleEe = e.LibelleEe,
                //                                               LibellePortion = "",
                //                                               LibelleEquipement = null,
                //                                               LibellePp = null,
                //                                               LibelleTypeEquipement = null,
                //                                               NumeroOrdre = null,
                //                                               Code = e.Code,
                //                                               Region = this.Regions.First(r => r.CleRegion == e.CleRegion).LibelleRegion
                //                                           }).ToList()).ToList();
                //    }

                //}

                #endregion

              
            }

            this.IsResultatEquipement = this.IsFiltreEquipement;

            this.Documents = new ObservableCollection<Documentation>((listComplete)
                .OrderByDescending(d => String.IsNullOrEmpty(d.Libelle))
                .ThenBy(d => d.LibelleEe)
                .ThenBy(d => d.LibellePortion)
                .ThenBy(d => d.LibellePp)
                .ThenBy(d => d.NumeroOrdre)
                .ThenBy(d => d.LibelleEquipement)
                .ThenBy(d => d.Dossier)
                .ThenBy(d => d.LibelleDesignation));
            

            IsBusy = false;
        }

        #endregion

        #region services

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> ServiceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les GEO ensembles électriques
        /// </summary>
        [Import]
        public IEntityService<GeoEnsembleElectrique> serviceGeoEnsElec { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les GEO ensembles électriques / portions
        /// </summary>
        [Import]
        public IEntityService<GeoEnsElecPortion> serviceGeoEnsElecPortion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les GEO ensembles électriques / portions / Equipement / Pp
        /// </summary>
        [Import]
        public IEntityService<GeoEnsElecPortionEqPp> serviceGeoEnsElecPortionEqPp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les type equipment
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> serviceTypeEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les types de document
        /// </summary>
        [Import]
        public IEntityService<TypeDocument> serviceTypeDocument { get; set; }

        #endregion

    }
}
