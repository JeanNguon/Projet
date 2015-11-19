using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Silverlight.Views.Windows;
using Proteca.Web.Models;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for PortionIntegrite entity
    /// </summary>
    [ExportAsViewModel("PortionIntegrite")]
    public class PortionIntegriteViewModel : OuvrageViewModel<PortionIntegrite>, IEventSink<ObservableCollection<GeoSecteur>>
    {
        #region Private Members

        /// <summary>
        /// Commentaire initiale avant les éventuelles modifications du commentaire
        /// </summary>
        private string CommentaireBeforeUpdate { get; set; }

        /// <summary>
        /// Déclaration de la liste des portions
        /// </summary>
        private List<AnAction> _listActions;

        /// <summary>
        /// Déclaration de variable de code département
        /// </summary>
        private string _filtreCodeDepartement;

        /// <summary>
        /// Déclaration de variable de code département1
        /// </summary>
        private string _filtreCodeDepartement1;

        /// <summary>
        /// TileView des Equipements
        /// </summary>
        private TileViewItemState _equipementTileItemState;

        /// <summary>
        /// Déclaration de l'état du tileview des actions
        /// </summary>
        private TileViewItemState _actionsTileItemState = TileViewItemState.Minimized;

        /// <summary>
        /// Déclaration de variable FiltreCleEnsEle
        /// </summary>
        private int? _filtreCleEnsEle;

        EventHandler OnMesModeleMesureLoaded;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        ///  Gère l'affichage du panel d'info
        /// </summary>
        public bool IsInfoAffiche
        {
            get
            {
                return this.SelectedEntity != null && !IsNewMode && !String.IsNullOrEmpty(this.SelectedEntity.InfosPortion);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<AnAction> ListActions
        {
            get
            {
                if (_listActions != null)
                {
                    return _listActions.OrderBy(ee => ee.Libelle).ToList();
                }
                return null;
            }
            set
            {
                _listActions = value;
                RaisePropertyChanged(() => this.ListActions);
            }
        }

        /// <summary>
        /// Déclaration de la variable incluant les portions supprimées
        /// </summary>
        public Boolean IsDelete { get; set; }

        /// <summary>
        /// Déclaration de la variable filtrant sur les postes gaz
        /// </summary>
        bool _isPosteGaz;
        public bool IsPosteGaz
        {
            get
            {
                return _isPosteGaz;
            }
            set
            {
                _isPosteGaz = value;
                RaisePropertyChanged(() => this.IsPosteGaz);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            }
        }
        /// <summary>
        /// Déclaration de la variable filtrant sur les stations
        /// </summary>
        bool _isStation;
        public bool IsStation
        {
            get
            {
                return _isStation;
            }
            set
            {
                _isStation = value;
                RaisePropertyChanged(() => this.IsStation);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            }
        }

        /// <summary>
        /// Variable définissant la visiblité du tableau des ListPpEquipements si aucune donnée et si mode lecture
        /// </summary>
        public bool ShowEquipements
        {
            get
            {
                return (ListPpEquipements != null && ListPpEquipements.Count > 0);
            }
        }

        /// <summary>
        /// Variable définissant la visiblité du tableau des niveaux associés si aucune donnée et si mode lecture
        /// </summary>
        public bool ShowNiveauAssocie
        {
            get
            {
                if (IsEditMode)
                {
                    return true;
                }
                else
                {
                    if (NiveauProtectionAssocies != null && NiveauProtectionAssocies.Count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Retourne les niveaux de protection associés
        /// </summary>
        public ObservableCollection<MesNiveauProtection> NiveauProtectionAssocies
        {
            get
            {
                if (this.SelectedEntity != null && this.SelectedEntity.MesNiveauProtection != null && this.SelectedEntity.MesNiveauProtection.Any())
                {
                    return new ObservableCollection<MesNiveauProtection>(this.SelectedEntity.MesNiveauProtection.Where(n => n.IsNew()).Reverse().Union(this.SelectedEntity.MesNiveauProtection.Where(n => !n.IsNew()).OrderBy(i => i.TypeEquipement.NumeroOrdre)));
                }
                return new ObservableCollection<MesNiveauProtection>();
            }
        }

        /// <summary>
        /// Retourne les secteurs associés
        /// </summary>
        public ObservableCollection<GeoSecteur> SecteursAssocies
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    return new ObservableCollection<GeoSecteur>(this.SelectedEntity.PiSecteurs.Select(r => r.GeoSecteur));
                }
                return null;
            }
        }

        /// <summary>
        /// Définit l'état du TileView des actions et permet de charger les actions
        /// </summary>
        public TileViewItemState ActionsTileItemState
        {
            get { return _actionsTileItemState; }
            set
            {
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null && this.ListActions == null)
                {
                    IsBusy = true;
                    ((PortionIntegriteService)service).GetListActionsByPortionIntegrite(this.SelectedEntity.ClePortion, GetListActionsDone);
                }
                _actionsTileItemState = value;
                RaisePropertyChanged(() => ActionsTileItemState);
            }
        }

        /// <summary>
        /// Etat du tileview des équipements
        /// </summary>
        public TileViewItemState EquipementTileItemState
        {
            get { return _equipementTileItemState; }
            set
            {
                // Force le rechargement
                //if (value == TileViewItemState.Maximized && this.SelectedEntity != null && (this.ListPpEquipements == null || this.ListPpEquipements.Count == 0))
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null)
                {
                    IsBusy = true;
                    ((PpEquipementService)servicePpEquipement).GetEntitiesByClePortion(this.SelectedEntity.ClePortion, PpEquipementLoaded);
                }

                _equipementTileItemState = value;
                RaisePropertyChanged(() => this.EquipementTileItemState);
            }
        }

        /// <summary>
        /// Retourne la liste des PpEquipement lié à la PI
        /// </summary>
        public ObservableCollection<PpEquipement> ListPpEquipements
        {
            get
            {
                if (servicePpEquipement.Entities != null && servicePpEquipement.Entities.Any() && this.SelectedEntity != null)
                    return new ObservableCollection<PpEquipement>(servicePpEquipement.Entities
                        .OrderBy(pp => pp.LibellePp).ThenBy(pp => pp.LibelleEquipement));
                else
                    return null;
            }
        }

        /// <summary>
        /// Retourne le code département
        /// </summary>
        public string FiltreCodeDepartement
        {
            get
            {
                if (_filtreCodeDepartement == null && SelectedEntity != null && SelectedEntity.RefCommune != null)
                {
                    _filtreCodeDepartement = SelectedEntity.RefCommune.CodeDepartement;
                }
                return _filtreCodeDepartement;
            }
            set
            {
                _filtreCodeDepartement = value;
                RaisePropertyChanged(() => this.FiltreCodeDepartement);
                RaisePropertyChanged(() => this.RefCommunes);
            }
        }

        /// <summary>
        /// Retourne le code département1
        /// </summary>
        public string FiltreCodeDepartement1
        {
            get
            {
                if (_filtreCodeDepartement1 == null && SelectedEntity != null && SelectedEntity.RefCommune1 != null)
                {
                    _filtreCodeDepartement1 = SelectedEntity.RefCommune1.CodeDepartement;
                }
                return _filtreCodeDepartement1;
            }
            set
            {
                _filtreCodeDepartement1 = value;
                RaisePropertyChanged(() => this.FiltreCodeDepartement1);
                RaisePropertyChanged(() => this.RefCommunes1);
            }
        }

        /// <summary>
        /// Retourne la clé EnsembleElectrique sélectionner
        /// </summary>
        public int? FiltreCleEnsElec
        {
            get { return _filtreCleEnsEle; }
            set
            {
                _filtreCleEnsEle = value;
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
            }
        }

        /// <summary>
        /// Retourne les régions du service région
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return new ObservableCollection<GeoRegion>(serviceRegion.Entities.OrderBy(r => r.LibelleRegion)); }
        }

        /// <summary>
        /// Retourne les GEO ensembles électrique du service EnsElec
        /// </summary>
        public List<GeoEnsembleElectrique> GeoEnsemblesElectrique
        {
            get
            {
                List<GeoEnsembleElectrique> GeoEnsElecs = null;
                if (FiltreCleRegion != null)
                {
                    if (FiltreCleAgence != null)
                    {
                        if (FiltreCleSecteur != null)
                        {
                            GeoEnsElecs = serviceGeoEnsElec.Entities.Where(i => i.CleSecteur == FiltreCleSecteur).Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                {
                                    return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                })).ToList();
                        }
                        else
                        {
                            GeoEnsElecs = serviceGeoEnsElec.Entities.Where(i => i.CleAgence == FiltreCleAgence).Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                {
                                    return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                })).ToList();
                        }
                    }
                    else
                    {
                        GeoEnsElecs = serviceGeoEnsElec.Entities.Where(i => i.CleRegion == FiltreCleRegion).Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                {
                                    return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                })).ToList();
                    }
                }
                else
                {
                    GeoEnsElecs = serviceGeoEnsElec.Entities.Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                            {
                                return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                            })).ToList();
                }
                if (IsStation || IsPosteGaz)
                {
                    GeoEnsElecs = GeoEnsElecs.Where(ee =>
                        (IsStation && ee.EnumStructureCplx.HasValue && ee.EnumStructureCplx.Value == 23)
                        || (IsPosteGaz && ee.EnumStructureCplx.HasValue && ee.EnumStructureCplx.Value == 24)).ToList();
                }

                return GeoEnsElecs;
            }
        }

        /// <summary>
        /// Retourne tout les GEO ensembles électrique du service EnsElec
        /// </summary>
        public List<GeoEnsembleElectrique> AllGeoEnsemblesElectrique
        {
            get
            {
                return serviceGeoEnsElec.Entities.Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                {
                    return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                })).ToList();
            }
        }

        /// <summary>
        /// Retourne les entités du service RefDiametre
        /// </summary>
        public ObservableCollection<RefDiametre> RefDiametres
        {
            get { return serviceRefDiametre.Entities; }
        }

        /// <summary>
        /// Retourne les entités du service RefRevetement
        /// </summary>
        public ObservableCollection<RefRevetement> RefRevetements
        {
            get { return serviceRefRevetement.Entities; }
        }

        /// <summary>
        /// Retourne les entités du service type équipement
        /// </summary>
        public ObservableCollection<TypeEquipement> TypeEq
        {
            get { return serviceTypeEquipement.Entities; }
        }

        /// <summary>
        /// Retourne les entités du service modèle mesure
        /// </summary>
        public ObservableCollection<MesModeleMesure> MesModeleMesures
        {
            get { return serviceMesModeleMesure.Entities; }
        }

        /// <summary>
        /// Retourne les entités du service RefCommune
        /// </summary>
        public ObservableCollection<RefCommune> RefCommunes
        {
            get
            {
                if (this.FiltreCodeDepartement != null)
                {
                    if (this.FiltreCodeDepartement.ToString().Length == 1)
                    {
                        return null;
                    }
                    return new ObservableCollection<RefCommune>(serviceCommune.Entities.Where(r => r.CodeDepartement.ToLower() == FiltreCodeDepartement.ToString().ToLower()).OrderBy(r => r.Libelle));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retourne les entités du service RefCommune
        /// </summary>
        public ObservableCollection<RefCommune> RefCommunes1
        {
            get
            {
                if (this.FiltreCodeDepartement1 != null)
                {
                    if (this.FiltreCodeDepartement1.ToString().Length == 1)
                    {
                        return null;
                    }
                    return new ObservableCollection<RefCommune>(serviceCommune.Entities.Where(r => r.CodeDepartement.ToLower() == FiltreCodeDepartement1.ToString().ToLower()).OrderBy(r => r.Libelle));
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion Public Properties

        #region Services
        
        /// <summary>
        /// Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;

        /// <summary>
        /// Service utilisé pour gérer l'entité de type équipement
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> serviceTypeEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de modèle mesure
        /// </summary>
        [Import]
        public IEntityService<MesModeleMesure> serviceMesModeleMesure { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type Commune
        /// </summary>
        [Import]
        public IEntityService<RefCommune> serviceCommune { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les GEO ensembles électriques
        /// </summary>
        [Import]
        public IEntityService<GeoEnsembleElectrique> serviceGeoEnsElec { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les diamètres
        /// </summary>
        [Import]
        public IEntityService<RefDiametre> serviceRefDiametre { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les revêtements
        /// </summary>
        [Import]
        public IEntityService<RefRevetement> serviceRefRevetement { get; set; }

        /// <summary>
        /// Service utilisé pour géré les PpEquipements associés
        /// </summary>
        [Import]
        public IEntityService<PpEquipement> servicePpEquipement { get; set; }

        #endregion

        #region Constructor

        public PortionIntegriteViewModel()
            : base()
        {

            this.OnRegionSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            };

            this.OnAgenceSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            };

            this.OnSecteurSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            };

            this.OnImportsSatisfiedEvent += (o, e) =>
            {
                EventAggregator.Subscribe<ObservableCollection<GeoSecteur>>(this);
            };

            this.OnViewActivated += (o, e) =>
            {
                
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Recherche des Portions Intégrités"));
                    EventAggregator.Publish("PortionIntegrite_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }

                // MAJ de la vue
                this.FiltreCodeDepartement = null;
                this.FiltreCodeDepartement1 = null;
                this.ListActions = null;
                this.IsDelete = false;
                this.IsStation = false;
                this.IsPosteGaz = false;
                RaisePropertyChanged(() => this.SecteursAssocies);
                RaisePropertyChanged(() => this.IsDelete);
                RaisePropertyChanged(() => this.ShowNiveauAssocie);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.SelectedEntity.PiSecteurs);
            };

            this.OnEntitiesLoaded += (o, e) =>
            {
                // MAJ des services
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.RefCommunes);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.AllGeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.RefRevetements);
                RaisePropertyChanged(() => this.RefDiametres);
                RaisePropertyChanged(() => this.TypeEq);

                // Chargement supplémentaire des modèles de mesure
                IsBusy = true;
                ((MesModeleMesureService)serviceMesModeleMesure).GetAllEntity((err) => MesModeleMesureLoaded(err));
            };

            this.OnViewModeChanged += (o, e) =>
            {
                this.FiltreCodeDepartement = null;
                this.FiltreCodeDepartement1 = null;
                RaisePropertyChanged(() => this.ShowNiveauAssocie);
                RaisePropertyChanged(() => SecteursAssocies);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.IsInfoAffiche);
            };

            this.OnDetailLoaded += (o, e) =>
            {
                // initialisation du code département
                this.FiltreCodeDepartement = null;
                this.FiltreCodeDepartement1 = null;
                this.RaisePropertyChanged(() => this.SelectedEntity);
                ListActions = null;

                // Chargement des modèles de mesure
                if (serviceMesModeleMesure != null && serviceMesModeleMesure.Entities != null && serviceMesModeleMesure.Entities.Any())
                {
                    FinChargement();
                }
                else
                {
                    EventHandler onMesModeleMesureLoaded = null;
                    onMesModeleMesureLoaded = (oo, ee) =>
                    {
                        FinChargement();
                        OnMesModeleMesureLoaded -= onMesModeleMesureLoaded;
                    };
                    OnMesModeleMesureLoaded += onMesModeleMesureLoaded;
                }

                RaisePropertyChanged(() => this.ListPpEquipements);
                if (EquipementTileItemState == TileViewItemState.Maximized && this.SelectedEntity != null)
                {
                    IsBusy = true;
                    ((PpEquipementService)servicePpEquipement).GetEntitiesByClePortion(this.SelectedEntity.ClePortion, PpEquipementLoaded);
                }

                //Au changement de portionIntegrite on charge les actions si l'utilisateur se trouve sur l'onglet des actions
                if (this.ActionsTileItemState == TileViewItemState.Maximized)
                {
                    IsBusy = true;
                    ((PortionIntegriteService)service).GetListActionsByPortionIntegrite(this.SelectedEntity.ClePortion, GetListActionsDone);
                }

                this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                this.RaisePropertyChanged(() => this.IsInfoAffiche);
            };

            this.OnCanceled += (o, e) =>
            {
                // Mise à jour du champs d'erreur

                // Correction mantis 20065 : supression de toutes les erreus de validation qui restent 
                //  (au cas ou aucun champ n'a été modifié mais qu'il y avait quand même des erreurs de validation)
                if (this.SelectedEntity != null && this.SelectedEntity.ValidationErrors.FirstOrDefault() != null)
                {

                    this.SelectedEntity.ValidationErrors.Clear();
                }


                // Mise à jour de la vue
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
                RaisePropertyChanged(() => this.SelectedEntity.PiSecteurs);
                RaisePropertyChanged(() => this.SecteursAssocies);

                // initialisation du code département
                this.FiltreCodeDepartement = null;
                this.FiltreCodeDepartement1 = null;
                this.RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.ListPpEquipements);
                this.CommentaireBeforeUpdate = this.SelectedEntity != null ? this.SelectedEntity.Commentaire : string.Empty;
            };

            this.OnAddedEntity += (o, e) =>
            {
                // MAJ de la vue
                this.SelectedEntity.GeoEnsElec = null;
                RaisePropertyChanged(() => this.SelectedEntity.GeoEnsElec);
                this.RaisePropertyChanged(() => this.SecteursAssocies);
                this.RaisePropertyChanged(() => this.NiveauProtectionAssocies);
                this.RaisePropertyChanged(() => this.AllGeoEnsemblesElectrique);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                // MAJ des GeoEnsemblesElectrique
                ((GeoEnsembleElectriqueService)serviceGeoEnsElec).GetEntities((err) => GeoEnsElecLoaded(err));
                RaisePropertyChanged(() => this.ListPpEquipements);
                // initialisation du code département
                this.FiltreCodeDepartement = null;
                this.FiltreCodeDepartement1 = null;
                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.IsInfoAffiche);
            };

            // Define commands
            AddNiveauProtectionCommand = new ActionCommand<object>(
                obj => AddNiveauProtection(), obj => true);
            NavigateEquipementCommand = new ActionCommand<object>(
                obj => NavigateToEquipement(obj), obj => true);
            GetDialogSecteurCommand = new ActionCommand<object>(
                obj => ShowDialog(), obj => true);
            RemoveSecteurCommand = new ActionCommand<object>(
                obj => DeletePiSecteur(obj), obj => true);
            DeleteNiveauProtectionCommand = new ActionCommand<object>(
                obj => DeleteNiveauProtection(obj), obj => true);
            IntegratePortionCommand = new ActionCommand<object>(
                obj => IntegratePortion(), obj => true);
            NavigatePPCommand = new ActionCommand<object>(
                obj => NavigateToPP(obj), obj => true);
        }

        #endregion

        #region Command

        /// <summary>
        /// Déclaration de l'objet de command de réintégration de l'utilisateur
        /// </summary>
        public IActionCommand IntegratePortionCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DeleteNiveauProtectionCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command d'ajout d'un niveau de protection
        /// </summary>
        public IActionCommand AddNiveauProtectionCommand { get; set; }

        /// <summary>
        /// Déclaration de l'objet de command de navigation vers un équipement
        /// </summary>
        public IActionCommand NavigateEquipementCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de navigation vers un équipement
        /// </summary>
        public IActionCommand GetDialogSecteurCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression d'un secteur associé
        /// </summary>
        public IActionCommand RemoveSecteurCommand { get; set; }

        /// <summary>
        /// Déclaration de la commande pour naviguer vers la PP
        /// </summary>
        public IActionCommand NavigatePPCommand { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialisation des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void initGeoPreferences()
        {
            base.initGeoPreferences();

            if (userService.CurrentUser != null)
                this.FiltreCleEnsElec = userService.CurrentUser.PreferenceCleEnsembleElectrique;
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

                if (this.FiltreCleEnsElec != userService.CurrentUser.PreferenceCleEnsembleElectrique)
                    userService.CurrentUser.SetPreferenceClePortion(null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        protected override void DeactivateView(string viewName)
        {
            Router.DeactivateView("Selected_Secteur");
            base.DeactivateView(viewName);
        }

        /// <summary>
        /// 
        /// </summary>
        private void FinChargement()
        {
            // Recherche du type équipement pour chaqye modèle de mesure à chaque ligne
            foreach (var niveauProtection in NiveauProtectionAssocies)
            {
                niveauProtection.ListeModeleMesure = serviceMesModeleMesure.Entities;
                if (niveauProtection.MesModeleMesure != null)
                {
                    niveauProtection.TypeEquipement = niveauProtection.MesModeleMesure.TypeEquipement;
                }
            }

            // MAJ de la vue
            RaisePropertyChanged(() => this.SelectedEntity);
            RaisePropertyChanged(() => this.FiltreCodeDepartement);
            RaisePropertyChanged(() => this.FiltreCodeDepartement1);
            RaisePropertyChanged(() => this.SecteursAssocies);
            RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            RaisePropertyChanged(() => this.ShowNiveauAssocie);
        }

        /// <summary>
        /// Surcharge de la méthode Delete du service
        /// </summary>
        /// <param name="skipNavigation"></param>
        protected override void Delete()
        {
            var result = System.Windows.MessageBox.Show(Resource.PortionIntegrite_DeleteConfirmation, "", System.Windows.MessageBoxButton.OKCancel);
            if (result == System.Windows.MessageBoxResult.OK)
            {
                IsBusy = true;

                // Récupération des équipements
                ((PortionIntegriteService)this.service).DeletePortionIntegriteAndCascade(this.SelectedEntity.ClePortion, this.CurrentUser.CleUtilisateur, (error, retour) =>
                    {
                        IsBusy = false;
                        if (error != null || retour == 0)
                        {
                            Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                            ErrorWindow.CreateNew(Resource.PortionIntegrite_ErrorOnDelete);
                        }
                        else
                        {
                            Find();
                        }
                    });
            }
            else
            {
                IsBusy = false;
            }

        }

        /// <summary>
        /// La suppression de la portion vient d'être réalisée
        /// </summary>
        /// <param name="ex"></param>
        private void DeletePIDone(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IsBusy = false;
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(PortionIntegrite).Name));
            }
            else
            {
                Find();
            }
        }

        /// <summary>
        /// Fonction de réintégration de la portion
        /// </summary>
        protected virtual void IntegratePortion()
        {
            // Réintégration logique
            this.SelectedEntity.Supprime = false;
            this.RaisePropertyChanged(() => this.IsInfoAffiche);
            if (this.SelectedEntity.EnsembleElectrique != null && this.SelectedEntity.EnsembleElectrique.Supprime)
            {
                this.NotifyError = true;
                this.IsEditMode = true;
                // Passage de l'ensemble elec à null après le mode Edition (Pour la gestiond de l'historique)
                this.SelectedEntity.CleEnsElectrique = 0;
            }
            else if (!this.NotifyError)
            {
                this.Save(true);
            }
        }

        /// <summary>
        /// Fonction d'ajout d'un niveau de protection
        /// </summary>
        protected void AddNiveauProtection()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (!UserCanAdd)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else
            {
                var niveauProtection = new MesNiveauProtection();
                niveauProtection.ListeModeleMesure = serviceMesModeleMesure.Entities;
                this.SelectedEntity.MesNiveauProtection.Add(niveauProtection);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            }
        }

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        protected virtual void DeleteNiveauProtection(object Obj)
        {
            var result = MessageBox.Show(Resource.NiveauProtectionAssocies_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                ((PortionIntegriteService)service).DeleteAssociateEntity((MesNiveauProtection)Obj);
                this.SelectedEntity.MesNiveauProtection.Remove((MesNiveauProtection)Obj);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            }

        }

        /// <summary>
        /// Affichage de la popup de sélection d'un secteur
        /// </summary>
        private void ShowDialog()
        {
            ChildWindow.Title = "Sélection de secteur(s)";
            ChildWindow.Show();
            EventAggregator.Publish("Selected_Secteur".AsViewNavigationArgs());
        }

        /// <summary>
        /// Supprime un secteur lié à la portion intégrité
        /// </summary>
        /// <param name="obj"></param>
        private void DeletePiSecteur(object obj)
        {
            PiSecteurs PiSecteurToDelete = this.SelectedEntity.PiSecteurs.Where(p => p.GeoSecteur.CleSecteur == (int)obj).FirstOrDefault();
            this.SelectedEntity.PiSecteurs.Remove(PiSecteurToDelete);
            ((PortionIntegriteService)service).DeleteAssociateEntity(PiSecteurToDelete);
            RaisePropertyChanged(() => this.SecteursAssocies);
        }

        /// <summary>
        /// Surcharge de la méthode save pour mettre a jour les valeurs conditionnées
        /// </summary>
        protected override void Save()
        {
            this.Save(false);
        }

        /// <summary>
        /// Surcharge de la méthode save pour mettre a jour les valeurs conditionnées
        /// </summary>
        protected override void Save(bool forceSave)
        {
            this.Save(forceSave, false);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Save(bool forceSave, bool withHisto)
        {
            if (this.CommentaireBeforeUpdate != this.SelectedEntity.Commentaire)
            {
                if ((string.IsNullOrEmpty(this.CommentaireBeforeUpdate) ^ string.IsNullOrEmpty(this.SelectedEntity.Commentaire)) ||
                    !string.IsNullOrEmpty(this.CommentaireBeforeUpdate) && !string.IsNullOrEmpty(this.SelectedEntity.Commentaire))
                {
                    this.SelectedEntity.DateMajCommentaire = DateTime.Now;
                    this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                }
            }

            base.Save(forceSave, withHisto);
        }

        /// <summary>
        /// Méthode de recherche appellé par la commande FindCommand
        /// cette méthode appelle la méthode Find du service
        /// pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            IsBusy = true;

            this.saveGeoPreferences();

            ((PortionIntegriteService)this.service).FindPortionIntegritesByCriterias(
                this.FiltreCleRegion, this.FiltreCleAgence, this.FiltreCleSecteur, this.FiltreCleEnsElec, this.IsDelete, this.IsPosteGaz, this.IsStation, SearchDone);
        }

        /// <summary>
        /// La recherche des ensemble électrique vient être terminée
        /// </summary>
        /// <param name="error"></param>
        private void SearchDone(Exception error)
        {
            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => EntitiesCount);
            RaisePropertyChanged(() => ResultIndicator);

            if (IsAutoNavigateToFirst && this.Entities != null && this.Entities.Any())
            {
                int _clePortion = (int)Entities.First().GetCustomIdentity();
                if (this.SelectedEntity != null && this.SelectedEntity.ClePortion == _clePortion)
                {
                    this.IsBusy = false;
                }
                else
                {
                    NavigationService.Navigate(_clePortion);
                }
            }
            else if (this.Entities == null || !this.Entities.Any())
            {
                ListActions = null;
                this.SelectedEntity = null;
                RaisePropertyChanged(() => this.ListPpEquipements);
                NavigationService.NavigateRootUrl();
            }

            this.IsBusy = false;
            if (OnFindLoaded != null)
            {
                OnFindLoaded(this, null);
            }
        }

        /// <summary>
        /// La liste des actions de l'ensemble électrique vient d'être chargée
        /// </summary>
        /// <param name="error"></param>
        private void GetListActionsDone(Exception error, List<AnAction> listActions)
        {
            IsBusy = false;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                ListActions = listActions;
            }
        }

        /// <summary>
        /// Méthode utilisé pour charger l'entité de type GEOEnsembleElectrique
        /// </summary>
        private void GeoEnsElecLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoEnsembleElectrique).Name));
            }
            else
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.AllGeoEnsemblesElectrique);
            }
        }

        /// <summary>
        /// Méthode utilisé pour charger l'entité de type MesModeleMesure
        /// </summary>
        private void MesModeleMesureLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(MesModeleMesure).Name));
            }
            else
            {
                if (OnMesModeleMesureLoaded != null)
                {
                    OnMesModeleMesureLoaded(this, null);
                }
                RaisePropertyChanged(() => this.MesModeleMesures);

                // We're done
                IsBusy = false;
            }
        }

        /// <summary>
        /// Méthode utilisé pour charger l'entité de type RefRevetement
        /// </summary>
        private void PpEquipementLoaded(Exception error)
        {
            IsBusy = false;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(PpEquipement).Name));
            }
            else
            {
                RaisePropertyChanged(() => this.ListPpEquipements);
            }
        }

        /// <summary>
        /// Permet de naviguer vers un équipement
        /// </summary>
        /// <param name="cleEquipement"></param>
        public void NavigateToEquipement(object obj)
        {
            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.Equipement.GetStringValue(),
                   ((PpEquipement)obj).CodeEquipement.ToString(),
                   ((PpEquipement)obj).CleEquipement.ToString()),
                   UriKind.Relative));
        }

        /// <summary>
        /// Permet de naviguer vers une PP
        /// </summary>
        /// <param name="obj"></param>
        public void NavigateToPP(object obj)
        {
            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/PP/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.Equipement.GetStringValue(),
                   ((PpEquipement)obj).ClePp.ToString()),
                   UriKind.Relative));
        }

        public void HandleEvent(ObservableCollection<GeoSecteur> publishedEvent)
        {
            if (IsActive)
            {
                // Récupération de la liste des secteurs ajouter
                ObservableCollection<GeoSecteur> mesSecteurs = publishedEvent;

                if (this.SelectedEntity != null)
                {
                    // Pour chaque secteur, on ajoute un PISecteur
                    foreach (var unSecteur in mesSecteurs)
                    {
                        if (this.SecteursAssocies == null)
                        {
                            this.SelectedEntity.PiSecteurs.Add(new PiSecteurs() { GeoSecteur = unSecteur });
                        }
                        else if (this.SecteursAssocies.Where(s => s.CleSecteur == unSecteur.CleSecteur).Count() == 0)
                        {
                            this.SelectedEntity.PiSecteurs.Add(new PiSecteurs() { CleSecteur = unSecteur.CleSecteur });
                        }
                    }


                    // Mise à jour du champs d'erreur
                    if (mesSecteurs != null && this.SelectedEntity.ValidationErrors.Where(v => v.MemberNames.Contains("PiSecteurs")).FirstOrDefault() != null)
                    {
                        this.SelectedEntity.ValidationErrors.Remove(this.SelectedEntity.ValidationErrors.Where(v => v.MemberNames.Contains("PiSecteurs")).FirstOrDefault());
                        RaisePropertyChanged(() => this.SelectedEntity.PiSecteurs);
                    }
                }

                // MAJ de la vue
                RaisePropertyChanged(() => this.SecteursAssocies);
            }
        }

        #endregion

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
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_PI_NIV);
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
            return this.SelectedEntity != null && (this.SelectedEntity.IsNew() || GetAutorisation());
        }

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            if (this.CurrentUser != null && this.SelectedEntity != null)
            {

                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_PI_NIV);
                string codePortee = role.RefUsrPortee.CodePortee;

                if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                {
                    return this.SelectedEntity.PiSecteurs.Any(s => s.GeoSecteur.CleAgence == CurrentUser.CleAgence);
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                    codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                {
                    return true;
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    return false;
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                {
                    return this.SelectedEntity.PiSecteurs.Any(s => s.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion);
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                {
                    return this.SelectedEntity.PiSecteurs.Any(s => s.GeoSecteur.CleSecteur == CurrentUser.CleSecteur);
                }
            }
            return false;
        }

        #endregion Autorisations

    }
}