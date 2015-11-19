using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Equipement entity
    /// </summary>
    public class EquipementViewModel : OuvrageViewModel<EqEquipement>
    {
        #region Private Members

        /// <summary>
        /// Déclaration de la variable FiltreCleEnsElec
        /// </summary>
        private int? _filtreCleEnsElec;

        /// <summary>
        /// Déclaration de la variable FiltreClePortion
        /// </summary>
        private int? _filtreClePortion;

        /// <summary>
        /// Liste des tournées associé aux équipements
        /// </summary>
        private List<Tournee> _listTournees;

        /// <summary>
        /// TileView des tournées
        /// </summary>
        private TileViewItemState _tourneesTileItemState;

        /// <summary>
        /// Liste des pp principale
        /// </summary>
        private ObservableCollection<Pp> _ppList = new ObservableCollection<Pp>();

        /// <summary>
        /// Liste des pp secondaire
        /// </summary>
        private ObservableCollection<Pp> _pp2List = new ObservableCollection<Pp>();

        /// <summary>
        /// Commentaire initiale avant les éventuelles modifications du commentaire
        /// </summary>
        private string CommentaireBeforeUpdate { get; set; }

        private bool _isDeleted = false;
        private bool _isReintegrated = false;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        ///  Gère l'affichage du panel d'info
        /// </summary>
        public bool IsInfoAffiche
        {
            get
            {
                return this.SelectedEntity != null && !IsNewMode && !String.IsNullOrEmpty(this.SelectedEntity.InfosEquipment);
            }
        }

        /// <summary>
        /// Perme d'afficher ou non le lien vers l' equipement origine ou dépacé
        /// </summary>
        public bool AfficheNavigate
        {
            get
            {
                return this.SelectedEntity != null && this.SelectedEntity.NavigateToEquipement != null;
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
        /// Indique si l'on inclue les équipement supprimés dans la recherche
        /// </summary>
        public bool IncludeDeletedEquipment { get; set; }

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
        /// Liste des portions permettant de filtrer les PP pour l'association avce l'équipement
        /// </summary>
        public List<GeoEnsElecPortion> ListPortions
        {
            get
            {
                Func<GeoEnsElecPortion, bool> filter = null;
                if (IsEditMode)
                {
                    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                    if (role != null && role.RefUsrPortee != null)
                    {
                        string codePortee = role.RefUsrPortee.CodePortee;
                        if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                        {
                            filter = i => i.CleAgence == CurrentUser.CleAgence.Value && i.NbPp > 0 && i.PortionSupprime == false;
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                        {
                            filter = i => i.NbPp > 0 && i.PortionSupprime == false;
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                        {
                            filter = i => i.CleRegion == CurrentUser.GeoAgence.CleRegion && i.NbPp > 0 && i.PortionSupprime == false;
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                        {
                            if (CurrentUser.CleSecteur.HasValue)
                            {
                                filter = i => CurrentUser.CleSecteur.HasValue && i.CleSecteur == CurrentUser.CleSecteur.Value && i.NbPp > 0 && i.PortionSupprime == false;
                            }
                        }
                    }
                }
                else
                {
                    filter = i => i.NbPp > 0 && i.PortionSupprime == false;
                }

                if (filter != null)
                    return ServiceGeoEnsElecPortion.Entities
                        .Where(filter)
                        .Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) => a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion)))
                        .OrderBy(pi => pi.LibellePortion)
                        .ToList();
                else
                    return new List<GeoEnsElecPortion>();
            }
        }



        /// <summary>
        /// Liste des Pp filtrées par la portion sélectionnée
        /// </summary>
        public ObservableCollection<Pp> PpList
        {
            get
            {
                if (_ppList != null)
                {
                    return new ObservableCollection<Pp>(_ppList.OrderBy(p => p.Pk).ToList());
                }
                else
                {
                    return new ObservableCollection<Pp>();
                }
            }
            set
            {
                _ppList = value;
                RaisePropertyChanged(() => this.PpList);
            }
        }

        /// <summary>
        /// Liste des Pp secondaires filtrées par la portion sélectionnée
        /// </summary>
        public ObservableCollection<Pp> Pp2List
        {
            get
            {
                if (_pp2List != null)
                {
                    return new ObservableCollection<Pp>(_pp2List.OrderBy(p => p.Pk).ToList());
                }
                else
                {
                    return new ObservableCollection<Pp>();
                }
            }
            set
            {
                _pp2List = value;
                RaisePropertyChanged(() => this.Pp2List);
            }
        }

        /// <summary>
        /// Indique si c'est un nouvel équipement
        /// </summary>
        public bool IsNewEquipement
        {
            get
            {
                if (this.SelectedEntity != null)
                    return this.SelectedEntity.IsNew();
                else
                    return true;
            }
        }

        /// <summary>
        /// Liste des tournées de l'équipement en cours
        /// </summary>
        public List<Tournee> ListTournees
        {
            get { return _listTournees; }
            set
            {
                _listTournees = value;
                RaisePropertyChanged(() => this.ListTournees);
            }
        }

        /// <summary>
        /// Etat du tileview des tournées pour les équipements
        /// </summary>
        public TileViewItemState TourneesTileItemState
        {
            get { return _tourneesTileItemState; }
            set
            {
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null && this.ListTournees == null)
                {
                    IsBusy = true;
                    ((EqEquipementService)service).GetListTournnees(this.SelectedEntity.CleEquipement, GetListTourneesDone);
                }

                _tourneesTileItemState = value;
                RaisePropertyChanged(() => TourneesTileItemState);
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
                    return new ObservableCollection<MesNiveauProtection>(this.SelectedEntity.MesNiveauProtection.Reverse());
                }
                return new ObservableCollection<MesNiveauProtection>();
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

        EventHandler OnMesModeleMesureLoaded;

        #endregion Public Properties

        #region Commandes

        /// <summary>
        /// Commande de réintégration
        /// </summary>
        public IActionCommand ReintegrateEquipementCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DeleteNiveauProtectionCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command d'ajout d'un niveau de protection
        /// </summary>
        public IActionCommand AddNiveauProtectionCommand { get; set; }

        #endregion Commandes

        #region Services

        ///// <summary>
        ///// Service utilisé pour gérer les logs
        ///// </summary>
        //[Import]
        //public IEntityService<LogOuvrage> ServiceLogOuvrage { get; set; }

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

        /// <summary>
        /// Service utilisé pour gérer les Pp
        /// </summary>
        [Import]
        public IEntityService<Pp> ServicePp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de modèle mesure
        /// </summary>
        [Import]
        public IEntityService<MesModeleMesure> ServiceMesModeleMesure { get; set; }

        /// <summary>
        /// Service utilisé pour pouvoir associé les type d'équipement sur l'équipement en ajout
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> ServiceTypeEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'ajout d'un EqEquipement depuis un EqEquipementTmp
        /// </summary>
        [Import]
        public IEntityService<EqEquipementTmp> ServiceEqEquipementTmp { get; set; }

        /// <summary>
        /// Service utilisé pour supprimer les liaisons de la table DrainageLiaisonsext
        /// </summary>
        [Import]
        public IEntityService<EqDrainageLiaisonsext> ServiceDrainageLiaisonsext { get; set; }

        /// <summary>
        /// Service utilisé pour supprimer les liaisons de la table SoutirageLiaisonsext
        /// </summary>
        [Import]
        public IEntityService<EqSoutirageLiaisonsext> ServiceSoutirageLiaisonsext { get; set; }

        #endregion Services

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public EquipementViewModel()
            : base()
        {
            this.ReintegrateEquipementCommand = new ActionCommand<object>(
                obj => ReintegrateEquipement(), obj => CanEdit);

            this.AddNiveauProtectionCommand = new ActionCommand<object>(
                obj => AddNiveauProtection(), obj => true);

            this.DeleteNiveauProtectionCommand = new ActionCommand<object>(
                obj => DeleteNiveauProtection(obj), obj => true);

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

            this.OnEntitiesLoaded += (o, e) =>
            {
                // Chargement supplémentaire des modèles de mesure
                IsBusy = true;
                ((MesModeleMesureService)ServiceMesModeleMesure).GetAllEntity((err) => MesModeleMesureLoaded(err));
            };

            this.OnViewActivated += (o, e) =>
            {
                //On vérifie si on est en compléter d'un equipement secondaire
                object idTmp;
                if (e.ViewParameter.TryGetValue(Global.Constants.PARM_ID_TMP, out idTmp) && idTmp != null && idTmp is int)
                {
                    if (AllServicesLoaded)
                    {
                        this.ServiceEqEquipementTmp.GetEntityByCle((int)idTmp, EquipementTmpLoaded);
                    }
                    else
                    {
                        EventHandler onAllServicesLoaded = null;
                        onAllServicesLoaded = (oo, ee) =>
                        {
                            this.ServiceEqEquipementTmp.GetEntityByCle((int)idTmp, EquipementTmpLoaded);
                            OnAllServicesLoaded -= onAllServicesLoaded;
                        };
                        OnAllServicesLoaded += onAllServicesLoaded;
                    }
                }
                else
                {
                    this.IsAutoNavigateToFirst = true;
                    RaisePropertyChanged(() => this.IsNewEquipement);
                    RaisePropertyChanged(() => this.AfficheNavigate);
                    RaisePropertyChanged(() => this.ShowNiveauAssocie);
                    ListTournees = null;
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
                RaisePropertyChanged(() => this.FiltreClePortion);
                RaisePropertyChanged(() => this.ListPortions);
            };

            this.OnDetailLoaded += (o, e) =>
            {
                this.IsAutoNavigateToFirst = true;
                registerPropertyChanged();
                refreshPortions();
                RaisePropertyChanged(() => IsNewEquipement);
                RaisePropertyChanged(() => this.AfficheNavigate);
                this.ReintegrateEquipementCommand.RaiseCanExecuteChanged();
                this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                ListTournees = null;

                // Chargement des modèles de mesure
                if (ServiceMesModeleMesure != null && ServiceMesModeleMesure.Entities != null && ServiceMesModeleMesure.Entities.Any())
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

                //Au changement de la pp on charge les équipements si l'utilisateur se trouve sur l'onglet des équipements
                if (this.TourneesTileItemState == TileViewItemState.Maximized)
                {
                    IsBusy = true;
                    ((EqEquipementService)service).GetListTournnees(this.SelectedEntity.CleEquipement, GetListTourneesDone);
                }
                // Portion Selected
                if (this.SelectedEntity.PortionSelected != null && this.SelectedEntity.PortionSelected.ClePortion > 0)
                {
                    ((PpService)ServicePp).GetPpsByClePortion(this.SelectedEntity.PortionSelected.ClePortion, GetPpsLoaded);
                    //SelectedEntity.LibellePortion = this.SelectedEntity.PortionSelected.LibellePortion;
                }

                // Portion Selected 2
                if (this.SelectedEntity.Portion2Selected != null && this.SelectedEntity.Portion2Selected.ClePortion > 0)
                {
                    ((PpService)ServicePp).GetPpsByClePortion(this.SelectedEntity.Portion2Selected.ClePortion, GetPps2Loaded);
                    //SelectedEntity.LibellePortion2 = this.SelectedEntity.Portion2Selected.LibellePortion;
                }

                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.SelectedEntity.InfosEquipment);
                RaisePropertyChanged(() => this.IsInfoAffiche);
            };

            this.OnCanceled += (o, e) =>
            {
                if (this.ServiceEqEquipementTmp.DetailEntity != null)
                {
                    this.ServiceEqEquipementTmp.Entities.Remove(this.ServiceEqEquipementTmp.DetailEntity);
                    this.ServiceEqEquipementTmp.DetailEntity = null;
                    this.SelectedEntity = null;
                    NavigationService.NavigateUri(new Uri(CurrentNavigation.Current.BaseUrl, UriKind.Relative));

                    RaisePropertyChanged(() => this.SelectedEntity);

                    this.IsEditMode = false;
                    this.IsAutoNavigateToFirst = true;
                }

                refreshPortions();
                RaisePropertyChanged(() => IsNewEquipement);
                RaisePropertyChanged(() => this.AfficheNavigate);
                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            };

            this.OnAddedEntity += (o, e) =>
            {
                registerPropertyChanged();
                refreshPortions();
                if (CurrentNavigation.Current.Filtre != null)
                    this.SelectedEntity.TypeEquipement = ServiceTypeEquipement.Entities.First(t => t.CodeEquipement == CurrentNavigation.Current.Filtre.ToString());
                CommentaireBeforeUpdate = String.Empty;
                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => IsNewEquipement);
                RaisePropertyChanged(() => this.AfficheNavigate);
                PpList = null;
                Pp2List = null;
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                // EP : Message géré dans le ouvrage view model
                //if (_isDeleted)
                //    MessageBox.Show(Resource.DeleteEquipement_LogicalDeleteSucces, string.Empty, MessageBoxButton.OK);

                this.ServiceEqEquipementTmp.DetailEntity = null;
                this.IsAutoNavigateToFirst = true;
                this.CommentaireBeforeUpdate = String.Empty;

                _isReintegrated = false;
                _isDeleted = false;
            };

            this.OnSaveError += (o, e) =>
            {
                if (_isReintegrated || _isDeleted)
                {
                    this.IsEditMode = true;
                    _isReintegrated = false;
                    _isDeleted = false;
                }
            };

            this.OnViewModeChanged += (o, e) =>
            {
                this.IsAutoNavigateToFirst = true;
                RaisePropertyChanged(() => this.ListPortions);
                RaisePropertyChanged(() => this.IsInfoAffiche);
                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.ShowNiveauAssocie);
            };
        }

        #endregion Constructor

        #region Override Methods

        /// <summary>
        /// Initialisation des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void initGeoPreferences()
        {
            base.initGeoPreferences();
            if (this.CurrentUser != null)
            {
                this.FiltreCleEnsElec = this.CurrentUser.PreferenceCleEnsembleElectrique;
                this.FiltreClePortion = this.CurrentUser.PreferenceClePortion;
            }
        }

        /// <summary>
        ///Enregistrement des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void saveGeoPreferences()
        {
            base.saveGeoPreferences();
            if (this.CurrentUser != null)
            {
                this.CurrentUser.SetPreferenceCleEnsembleElectrique(this.FiltreCleEnsElec);
                this.CurrentUser.SetPreferenceClePortion(this.FiltreClePortion);
            }
        }

        /// <summary>
        /// Lancement de la recherche
        /// </summary>
        protected override void Find()
        {
            IsBusy = true;

            saveGeoPreferences();

            ((EqEquipementService)this.service).FindEquipementByCriterias(this.FiltreCleRegion, this.FiltreCleAgence, this.FiltreCleSecteur,
                this.FiltreCleEnsElec, this.FiltreClePortion, this.IncludeDeletedEquipment, CurrentNavigation.Current.Filtre.ToString(), SearchDone);
        }

        /// <summary>
        /// On met à jour la date de la mise à jour avant la sauvegarde si le commentaire actuel est différent
        /// de l'ancien commentaire
        /// </summary>
        protected override void Save()
        {
            this.Save(false);
        }

        protected override void Save(bool forceSave)
        {
            this.Save(forceSave, false);
        }

        /// <summary>
        /// On met à jour la date de la mise à jour avant la sauvegarde si le commentaire actuel est différent
        /// de l'ancien commentaire
        /// </summary>
        protected override void Save(bool forceSave, bool withHisto)
        {
            if (SelectedEntity != null)
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

                //On initialise l'utilisateur lors de la création de la PP
                if (SelectedEntity.IsNew())
                    SelectedEntity.CleUtilisateur = CurrentUser.CleUtilisateur;

                if (SelectedEntity.HasChanges || SelectedEntity.IsNew())
                {
                    SelectedEntity.DateMajEquipement = DateTime.Now;
                }

                // Gestion de la suppression des références entre Raccord isolent et liaison lors du changement de PP de rattachement
                if ((SelectedEntity is EqLiaisonInterne || SelectedEntity is EqLiaisonExterne) && SelectedEntity.HasChanges && !SelectedEntity.IsNew())
                {
                    EqEquipement origin = (EqEquipement)SelectedEntity.GetOriginal();
                    if ((origin.ClePp != SelectedEntity.ClePp) || SelectedEntity.Supprime)
                    {
                        foreach (EqRaccordIsolant ri in SelectedEntity.EqRaccordIsolant1)
                        {
                            ri.CleLiaison = null;
                        }
                    }
                }

                //Gestion de la suppression des liaisons soutiragesliaisonsest lors de la suppression logique d'un soutirage
                else if (SelectedEntity is EqSoutirage && SelectedEntity.HasChanges && !SelectedEntity.IsNew())
                {
                    EqSoutirage origin = (SelectedEntity.GetOriginal()) as EqSoutirage;
                    if (SelectedEntity.Supprime && !origin.Supprime)
                    {
                        for (int i = (SelectedEntity as EqSoutirage).EqSoutirageLiaisonsext.Count - 1; i > -1; i--)
                        {
                            this.ServiceSoutirageLiaisonsext.Delete((SelectedEntity as EqSoutirage).EqSoutirageLiaisonsext.ElementAt(i));
                        }
                    }
                }

                //Gestion de la suppression des Liaisons drainagesliaisonsext lorrs de la suppression logique d'un drainage
                else if (SelectedEntity is EqDrainage && SelectedEntity.HasChanges && !SelectedEntity.IsNew())
                {
                    EqDrainage origin = (SelectedEntity.GetOriginal()) as EqDrainage;
                    if (SelectedEntity.Supprime && !origin.Supprime)
                    {
                        for (int i = (SelectedEntity as EqDrainage).EqDrainageLiaisonsext.Count - 1; i > -1; i--)
                        {
                            this.ServiceDrainageLiaisonsext.Delete((SelectedEntity as EqDrainage).EqDrainageLiaisonsext.ElementAt(i));
                        }
                    }
                }

                //Suppression de l'eqtmp si on est en dans le cas
                if (this.ServiceEqEquipementTmp.DetailEntity != null)
                {
                    int size = (this.ServiceEqEquipementTmp.DetailEntity.Visites != null) ? this.ServiceEqEquipementTmp.DetailEntity.Visites.Count : 0;
                    for (int i = size - 1; i > -1; i--)
                    {
                        Visite v = this.ServiceEqEquipementTmp.DetailEntity.Visites.ElementAt(i);
                        this.SelectedEntity.Visites.Add(v);
                        this.ServiceEqEquipementTmp.DetailEntity.Visites.Remove(v);
                    }
                    this.ServiceEqEquipementTmp.Delete(this.ServiceEqEquipementTmp.DetailEntity);
                    this.ServiceEqEquipementTmp.DetailEntity = null;
                }
            }

            ListTournees = null;
            MessageBoxResult result = MessageBoxResult.OK;

            if (this.SelectedEntity.ClePp != 0)
            {
                if ((this.SelectedEntity is EqLiaisonInterne && !this.SelectedEntity.Supprime && this.SelectedEntity.ClePp == ((EqLiaisonInterne)this.SelectedEntity).ClePp2) ||
                  (this.SelectedEntity is EqFourreauMetallique && !this.SelectedEntity.Supprime && this.SelectedEntity.ClePp == ((EqFourreauMetallique)this.SelectedEntity).ClePp2) ||
                  (this.SelectedEntity is EqRaccordIsolant && !this.SelectedEntity.Supprime && this.SelectedEntity.ClePp == ((EqRaccordIsolant)this.SelectedEntity).ClePp2))
                {
                    result = MessageBox.Show(Resource.EqEquipement_SamePP,
                        string.Empty, MessageBoxButton.OKCancel);
                }
            }

            refreshPortions();

            if (result == MessageBoxResult.OK)
            {
                base.Save(forceSave, withHisto);

                //Ajout de la validation sur le LibellePortion dans le cas où l'erreur est remonté par la PP supprime et que la portion est supprime
                if (!IsNewMode && IsEditMode && SelectedEntity.ValidationErrors.Any(ve => ve.MemberNames.Contains("Pp")) && this.SelectedEntity.PortionSelected.PortionSupprime)
                {
                    this.SelectedEntity.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(Resource.DeleteStateError, new List<string>() { "PortionSelected" }));
                }
            }
        }

        protected virtual void OnDeleting()
        {
        }

        protected override void Cancel()
        {
            this.SelectedEntity.ValidationErrors.Clear();

            base.Cancel();
        }

        /// <summary>
        /// Suppression d'un équipement
        /// </summary>
        /// <param name="skipNavigation"></param>
        protected override void Delete()
        {
            MessageBoxResult result = MessageBox.Show(Resource.EqEquipement_DeleteConfirmation,
               Resource.EqEquipement_DeleteCaption, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                IsBusy = true;
                ListTournees = null;
                CanPhysicalDeleteByEquipement();
                IsBusy = false;
            }
        }

        protected virtual void CanPhysicalDeleteByEquipement()
        {
            ((EqEquipementService)service).CanPhysicalDeleteByEquipement(this.SelectedEntity.CleEquipement, CanPhysicalDeletedEquipementDone);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// L'équipement temporaire a été chergé et est donc vidé dans un nouvel équipement
        /// </summary>
        /// <param name="error"></param>
        private void EquipementTmpLoaded(Exception error)
        {
            if (this.ServiceEqEquipementTmp.DetailEntity != null && this.ServiceEqEquipementTmp.DetailEntity.EstValide)
            {
                //Ajout de l'équipement et copie des propriétés disponibles
                this.Add();
                this.SelectedEntity.Libelle = this.ServiceEqEquipementTmp.DetailEntity.Libelle;
                this.SelectedEntity.TypeEquipement = this.ServiceEqEquipementTmp.DetailEntity.TypeEquipement;
                this.SelectedEntity.Pp = this.ServiceEqEquipementTmp.DetailEntity.Pp2;

                this.IsAutoNavigateToFirst = false;

                this.IsEditMode = true;

                //Imitation du DetailLoaded
                registerPropertyChanged();
                refreshPortions();
                RaisePropertyChanged(() => IsNewEquipement);
                RaisePropertyChanged(() => this.AfficheNavigate);
                this.ReintegrateEquipementCommand.RaiseCanExecuteChanged();
                this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                ListTournees = null;

                // Portion Selected
                if (this.SelectedEntity.PortionSelected != null && this.SelectedEntity.PortionSelected.ClePortion > 0)
                {
                    ((PpService)ServicePp).GetPpsByClePortion(this.SelectedEntity.PortionSelected.ClePortion, GetPpsLoaded);
                    //SelectedEntity.LibellePortion = this.SelectedEntity.PortionSelected.LibellePortion;
                }

                // Portion Selected 2
                if (this.SelectedEntity.Portion2Selected != null && this.SelectedEntity.Portion2Selected.ClePortion > 0)
                {
                    ((PpService)ServicePp).GetPpsByClePortion(this.SelectedEntity.Portion2Selected.ClePortion, GetPps2Loaded);
                    //SelectedEntity.LibellePortion2 = this.SelectedEntity.Portion2Selected.LibellePortion;
                }

                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.ShowNiveauAssocie);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            }
            else
            {
                RaisePropertyChanged(() => this.IsNewEquipement);
                RaisePropertyChanged(() => this.AfficheNavigate);
                RaisePropertyChanged(() => this.ShowNiveauAssocie);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
                ListTournees = null;
            }
        }

        ///// <summary>
        ///// Abonnement à propertyChanged pour la gestion des coordonnées GPS et du niveau de sensibilité
        ///// </summary>
        private void registerPropertyChanged()
        {
            if (SelectedEntity != null)
            {
                PropertyChangedEventHandler anyPropertyChanged = null;
                anyPropertyChanged = (oo, ee) =>
                {
                    if (ee.PropertyName == "PortionSelected")
                    {
                        if (SelectedEntity.PortionSelected != null && SelectedEntity.PortionSelected.ClePortion > 0)
                        {
                            ((PpService)ServicePp).GetPpsByClePortion(SelectedEntity.PortionSelected.ClePortion, GetPpsLoaded);
                        }
                    }

                    if (ee.PropertyName == "Portion2Selected")
                    {
                        if (SelectedEntity.Portion2Selected != null && SelectedEntity.Portion2Selected.ClePortion > 0)
                        {
                            ((PpService)ServicePp).GetPpsByClePortion(SelectedEntity.Portion2Selected.ClePortion, GetPps2Loaded);
                        }
                    }
                };
                SelectedEntity.PropertyChanged -= anyPropertyChanged;
                SelectedEntity.PropertyChanged += anyPropertyChanged;
            }
        }


        /// <summary>
        /// Récupération de l'information si l'équipement doit être supprimé logiquement ou physiquement
        /// </summary>
        /// <param name="error">exception lors de la récupération de l'information</param>
        /// <param name="canPhysicalDelete"></param>
        private void CanPhysicalDeletedEquipementDone(Exception error, bool canPhysicalDelete)
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
                // Suppression des références entre raccord isolant
                foreach (EqRaccordIsolant ri in SelectedEntity.EqRaccordIsolant1)
                {
                    ri.CleLiaison = null;
                    this.LogOuvrage("M", ri);
                }

                // Lors de la suppression logique ou physique d'un soutirage ou d'un soutirage, 
                // il faut ajouter un Log sur la ou les liaisons Externes concernées.
                if (SelectedEntity is EqSoutirage)
                {
                    EqSoutirage currentSO = SelectedEntity as EqSoutirage;
                    foreach (EqSoutirageLiaisonsext SO_LE in currentSO.EqSoutirageLiaisonsext)
                    {
                        this.LogOuvrage("M", SO_LE.EqLiaisonExterne, Proteca.Web.Resources.ResourceHisto.EqSoutirageLiaisonsext);
                    }
                }
                else if (SelectedEntity is EqDrainage)
                {
                    EqDrainage currentDR = SelectedEntity as EqDrainage;
                    foreach (EqDrainageLiaisonsext DR_LE in currentDR.EqDrainageLiaisonsext)
                    {
                        this.LogOuvrage("M", DR_LE.EqLiaisonExterne, Proteca.Web.Resources.ResourceHisto.EqDrainageLiaisonsext);
                    }
                }

                //// Lors de la suppression logique ou physique d'une Liaison, 
                //// il faut ajouter un Log dans le ou les raccords associés.
                //if (SelectedEntity is EqLiaisonExterne)
                //{
                //    EqLiaisonExterne currentLE = SelectedEntity as EqLiaisonExterne;
                //    foreach (EqDrainageLiaisonsext DR_LE in currentLE.EqDrainageLiaisonsext)
                //    {
                //        this.LogOuvrage("M", DR_LE.EqLiaisonExterne);
                //    }
                //}

                // Lors de la suppression logique ou physique d'une Liaison, 
                // il faut ajouter un Log dans le ou les raccords associés.
                /* if (SelectedEntity is EqLiaisonInterne)
                 {
                     EqLiaisonInterne currentLI = SelectedEntity as EqLiaisonInterne;
                     foreach (EqDrainageLiaisonsext DR_LE in currentLI.EqDrainageLiaisonsext)
                     {
                         this.LogOuvrage("M", DR_LE.EqLiaisonExterne);
                     }
                 }
                 */
                // Réalisation de la suppression physique ou logique
                if (canPhysicalDelete)
                {
                    SelectedEntity.DateMajCommentaire = DateTime.MaxValue; //valide la supression des compostions avant la suppression physique pour le domainservice validatechangeset
                    base.Delete(false, true);
                }
                else
                {

                    if (SelectedEntity.DateMajCommentaire == DateTime.MaxValue) SelectedEntity.DateMajCommentaire = null; //assure la non suppression des compostions avant la suppression physique pour le domainservice validatechangeset

                    this.SelectedEntity.Supprime = true;
                    _isDeleted = true;
                    this.Save(true);
                    RaisePropertyChanged(() => this.SelectedEntity.InfosEquipment);
                    RaisePropertyChanged(() => this.IsInfoAffiche);
                }
            }
        }

        /// <summary>
        /// Réintégration de l'équipement
        /// </summary>
        protected virtual void ReintegrateEquipement()
        {
            this.SelectedEntity.Supprime = false;
            RaisePropertyChanged(() => this.IsInfoAffiche);
            RaisePropertyChanged(() => this.SelectedEntity.InfosEquipment);

            if (this.SelectedEntity.Pp != null && this.SelectedEntity.Pp.Supprime)
            {
                this.IsEditMode = true;
                this.NotifyError = true;

                // Passage de la PP à null après le mode Edition (Pour la gestiond de l'historique)
                this.SelectedEntity.ClePp = 0;
            }
            else if (!this.NotifyError)
            {
                _isReintegrated = true;
                this.Save(true);
            }
        }

        /// <summary>
        /// Récupération de la iste des tournées de l'équipements
        /// </summary>
        /// <param name="error"></param>
        /// <param name="listTournee"></param>
        private void GetListTourneesDone(Exception error, List<Tournee> listTournees)
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
                this.ListTournees = listTournees;
            }
        }

        /// <summary>
        /// Surcharge de la méthode pour permettre le chargement des Portions associée (et donc des libellé de protion)
        /// </summary>
        protected override void GetHistorisation()
        {
            //refreshPortions();
            base.GetHistorisation();
        }

        /// <summary>
        /// Met à jour les Portions
        /// </summary>
        protected virtual void refreshPortions()
        {
            if (this.SelectedEntity != null)
            {
                this.SelectedEntity.PortionSelected = null;
            }

            if (this.SelectedEntity != null && this.SelectedEntity.Pp != null)
            {
                this.SelectedEntity.PortionSelected = this.ListPortions.FirstOrDefault(p => p.ClePortion == this.SelectedEntity.Pp.ClePortion);
            }

            RaisePropertyChanged(() => this.SelectedEntity);
        }

        /// <summary>
        /// La recherche des equipement vient d'être effectuée.
        /// </summary>
        /// <param name="ex"></param>
        private void SearchDone(Exception ex)
        {
            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => EntitiesCount);
            RaisePropertyChanged(() => ResultIndicator);
            if (IsAutoNavigateToFirst && this.Entities != null && this.Entities.Any())
            {
                int cleEquipement = (int)Entities.First().GetCustomIdentity();
                if (this.SelectedEntity != null && this.SelectedEntity.CleEquipement == cleEquipement)
                {
                    this.IsBusy = false;
                }
                else
                {
                    NavigationService.Navigate(cleEquipement);
                }
            }
            else if (this.Entities == null || !this.Entities.Any())
            {
                this.SelectedEntity = null;
                NavigationService.NavigateRootUrl();
                this.IsBusy = false;
            }
            else
            {
                this.IsBusy = false;
            }
            if (OnFindLoaded != null)
            {
                OnFindLoaded(this, null);
            }
        }

        /// <summary>
        /// La liste des Pps en fonction de la cle portion vien dd'être chargée
        /// </summary>
        /// <param name="error"></param>
        private void GetPpsLoaded(Exception error)
        {
            ObservableCollection<Pp> result = new ObservableCollection<Pp>();
            if (this.CurrentUser != null && this.SelectedEntity != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                if (role != null && role.RefUsrPortee != null)
                {
                    string codePortee = role.RefUsrPortee.CodePortee;

                    if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue() && CurrentUser.CleAgence.HasValue)
                    {
                        result = new ObservableCollection<Pp>(ServicePp.Entities.Where(p => !p.Supprime && p.GeoSecteur != null && p.GeoSecteur.CleAgence == CurrentUser.CleAgence.Value).OrderBy(p => p.Libelle));
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                        codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                    {
                        result = new ObservableCollection<Pp>(ServicePp.Entities.Where(p => !p.Supprime).OrderBy(p => p.Libelle));
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue() && CurrentUser.GeoAgence != null)
                    {
                        result = new ObservableCollection<Pp>(ServicePp.Entities.Where(p => !p.Supprime && p.GeoSecteur != null && p.GeoSecteur.GeoAgence != null && p.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion).OrderBy(p => p.Libelle));
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue() && CurrentUser.CleSecteur.HasValue)
                    {
                        result = new ObservableCollection<Pp>(ServicePp.Entities.Where(p => !p.Supprime && p.CleSecteur == CurrentUser.CleSecteur.Value).OrderBy(p => p.Libelle));
                    }
                }
            }
            PpList = result;
            RaisePropertyChanged(() => this.SelectedEntity);
        }

        /// <summary>
        /// La liste des Pps secondaire en fonction de la cle portion vien dd'être chargée
        /// </summary>
        /// <param name="error"></param>
        private void GetPps2Loaded(Exception error)
        {
            ObservableCollection<Pp> result = new ObservableCollection<Pp>();
            if (this.CurrentUser != null && this.SelectedEntity != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                if (role != null && role.RefUsrPortee != null)
                {
                    string codePortee = role.RefUsrPortee.CodePortee;

                    if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue() && CurrentUser.CleAgence.HasValue)
                    {
                        result = new ObservableCollection<Pp>(ServicePp.Entities.Where(p => !p.Supprime && p.GeoSecteur != null && p.GeoSecteur.CleAgence == CurrentUser.CleAgence.Value).OrderBy(p => p.Libelle));
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                        codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                    {
                        result = new ObservableCollection<Pp>(ServicePp.Entities.Where(p => !p.Supprime).OrderBy(p => p.Libelle));
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue() && CurrentUser.GeoAgence != null)
                    {
                        result = new ObservableCollection<Pp>(ServicePp.Entities.Where(p => !p.Supprime && p.GeoSecteur != null && p.GeoSecteur.GeoAgence != null && p.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion).OrderBy(p => p.Libelle));
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue() && CurrentUser.CleSecteur.HasValue)
                    {
                        result = new ObservableCollection<Pp>(ServicePp.Entities.Where(p => !p.Supprime && p.CleSecteur == CurrentUser.CleSecteur.Value).OrderBy(p => p.Libelle));
                    }
                }
            }
            Pp2List = result;
            //Dans le cas ou l'utilisateur a selectionné deux fois la même portion on s'assusre que la PP primaire n'est pas présente dans la liste des PP secondaire
            //if (this.PortionSelected != null && this.PortionSelected.ClePortion == this.Portion2Selected.ClePortion)
            //{
            //    Pp2List = new ObservableCollection<Pp>(Pp2List.Where(p => p.ClePp != this.SelectedEntity.ClePp));
            //}
            RaisePropertyChanged(() => this.SelectedEntity);
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

                // We're done
                IsBusy = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FinChargement()
        {
            // Recherche du type équipement pour chaqye modèle de mesure à chaque ligne
            foreach (var niveauProtection in NiveauProtectionAssocies)
            {
                niveauProtection.ListeModeleMesure = ServiceMesModeleMesure.Entities;
                if (niveauProtection.MesModeleMesure != null)
                {
                    niveauProtection.TypeEquipement = ServiceTypeEquipement.Entities.FirstOrDefault(ty => ty.CodeEquipement == "PP");
                }
            }

            // MAJ de la vue
            RaisePropertyChanged(() => this.SelectedEntity);
            RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            RaisePropertyChanged(() => this.ShowNiveauAssocie);
        }

        #endregion Private Functions

        #region Public Functions

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="completed"></param>
        public void AddEquipement<T>() where T : EqEquipement
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (!UserCanAdd)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else
            {
                T entity = Activator.CreateInstance<T>();

                this.SelectedEntity = entity;

                this.service.Add(entity);
                this.Entities.Add(entity);
                this.IsEditMode = true;
            }
            if (OnAddedEntity != null)
            {
                OnAddedEntity(this, null);
            }
        }

        /// <summary>
        /// Fonction d'ajout d'un niveau de protection
        /// </summary>
        public void AddNiveauProtection()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (!UserCanAdd)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else
            {
                var niveauProtection = new MesNiveauProtection();
                niveauProtection.ListeModeleMesure = ServiceMesModeleMesure.Entities;
                niveauProtection.TypeEquipement = this.SelectedEntity.TypeEquipement;
                this.SelectedEntity.MesNiveauProtection.Add(niveauProtection);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            }
        }

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        public virtual void DeleteNiveauProtection(object Obj)
        {
            var result = MessageBox.Show(Resource.NiveauProtectionAssocies_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                ((EqEquipementService)service).DeleteAssociateEntity((MesNiveauProtection)Obj);
                this.SelectedEntity.MesNiveauProtection.Remove((MesNiveauProtection)Obj);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            }
        }

        #endregion Public Functions

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
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
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
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            if (this.CurrentUser != null && this.SelectedEntity != null && this.SelectedEntity.Pp != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                if (role != null && role.RefUsrPortee != null)
                {
                    string codePortee = role.RefUsrPortee.CodePortee;

                    if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                    {
                        return this.SelectedEntity.Pp.GeoSecteur != null && this.SelectedEntity.Pp.GeoSecteur.CleAgence == CurrentUser.CleAgence;
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
                        return this.SelectedEntity.Pp.GeoSecteur != null && this.SelectedEntity.Pp.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion;
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                    {
                        return this.SelectedEntity.Pp.CleSecteur == CurrentUser.CleSecteur;
                    }
                }
            }
            return false;
        }

        #endregion

    }
}
