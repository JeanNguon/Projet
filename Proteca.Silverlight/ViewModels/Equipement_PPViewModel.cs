using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;
using Jounce.Core.Application;
using Proteca.Silverlight.Resources;
using Telerik.Windows.Controls;
using System.Reflection;
using System.Windows;
using Proteca.Web.Resources;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for PP entity
    /// </summary>
    [ExportAsViewModel("Equipement_PP")]
    public class Equipement_PPViewModel : OuvrageViewModel<Pp>
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
        /// Commentaire initiale avant les éventuelles modifications du commentaire
        /// </summary>
        private string CommentaireBeforeUpdate { get; set; }

        /// <summary>
        /// Déclaration de la variable ListSecteurs
        /// </summary>
        private ObservableCollection<GeoSecteur> _listSecteurs;

        /// <summary>
        /// Déclaration de la variable EquipementssTileItemState
        /// </summary>
        private TileViewItemState _equipementssTileItemState = TileViewItemState.Minimized;

        /// <summary>
        /// Déclaration de la liste d'équipements liés à la PP
        /// </summary>
        private List<EqEquipement> _listEquipement;

        /// <summary>
        /// Liste des tournées associé aux équipements
        /// </summary>
        private List<Tournee> _listTournees;

        /// <summary>
        /// TileView des tournées
        /// </summary>
        private TileViewItemState _tourneesTileItemState;

        /// <summary>
        /// Indique si l'élément vient d'être supprimé
        /// </summary>
        private bool _isDeleted = false;

        /// <summary>
        /// Indique si l'élément vient d'être réintégré
        /// </summary>
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
        /// Perme d'afficher ou non le lien vers la pp origine ou dépacé
        /// </summary>
        public bool AfficheNavigate
        {
            get
            {
                return this.SelectedEntity != null && this.SelectedEntity.NavigateToPP != null;
            }
        }
        // Active / désactive les listes déroulante TME / TMS si la case ) cocher courant alternatif est cochée
        public bool IsTmeTmsEnabled
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    return IsEditMode && !this.SelectedEntity.CourantsAlternatifsInduits;
                }
                return IsEditMode;
            }
        }

        // Force la validation avec historisation sur certaines conditions
        public bool PPCanSave
        {
            get
            {
                if (this.SelectedEntity != null && this.ObjetToDuplicate != null)
                {
                    if (
                           ((HistoPp)this.ObjetToDuplicate).CourantsAlternatifsInduits != ((Pp)this.SelectedEntity).CourantsAlternatifsInduits 
                        || ((HistoPp)this.ObjetToDuplicate).CourantsVagabonds          != ((Pp)this.SelectedEntity).CourantsVagabonds
                        || ((HistoPp)this.ObjetToDuplicate).ElectrodeEnterreeAmovible  != ((Pp)this.SelectedEntity).ElectrodeEnterreeAmovible 
                        || ((HistoPp)this.ObjetToDuplicate).TemoinEnterreAmovible      != ((Pp)this.SelectedEntity).TemoinEnterreAmovible
                        || ((HistoPp)this.ObjetToDuplicate).TemoinMetalliqueDeSurface  != ((Pp)this.SelectedEntity).TemoinMetalliqueDeSurface 
                        || ((HistoPp)this.ObjetToDuplicate).PresenceTelemesure         != ((Pp)this.SelectedEntity).PresenceDUneTelemesure)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        #region Expander

        /// <summary>
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return serviceRegion.Entities; }
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
                        return ServiceGeoEnsElecPortion.Entities.Where(i=> i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                }
            }
        }

        #endregion Expander

        /// <summary>
        /// Liste des secteurs
        /// </summary>
        public ObservableCollection<GeoSecteur> ListSecteurs
        {
            get
            {
                return _listSecteurs;
            }
            set
            {
                _listSecteurs = value;
                RaisePropertyChanged(() => this.ListSecteurs);
            }
        }

        /// <summary>
        /// Liste des portions permettant de filtrer les PP pour l'association avce l'équipement
        /// </summary>
        public List<GeoEnsElecPortion> ListPortions
        {
            get
            {
                List<GeoEnsElecPortion> result = new List<GeoEnsElecPortion>();
                if (IsEditMode)
                {
                    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                    if (role != null && role.RefUsrPortee != null)
                    {
                        switch (role.RefUsrPortee.GetPorteesEnum())
                        {
                            case RefUsrPortee.ListPorteesEnum.Agence :
                                result = ServiceGeoEnsElecPortion.Entities.Where(i => CurrentUser.CleAgence.HasValue && i.CleAgence == CurrentUser.CleAgence.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                                break;
                            case RefUsrPortee.ListPorteesEnum.National :
                            case RefUsrPortee.ListPorteesEnum.Autorisee :
                                result = ServiceGeoEnsElecPortion.Entities.Where(i => i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                                break;
                            case RefUsrPortee.ListPorteesEnum.Region :
                                result = ServiceGeoEnsElecPortion.Entities.Where(i => CurrentUser.GeoAgence != null && i.CleRegion == CurrentUser.GeoAgence.CleRegion && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                                break;
                            case RefUsrPortee.ListPorteesEnum.Secteur :
                                result = ServiceGeoEnsElecPortion.Entities.Where(i => CurrentUser.CleSecteur.HasValue && i.CleSecteur == CurrentUser.CleSecteur.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    result = ServiceGeoEnsElecPortion.Entities.Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                    })).OrderBy(pi => pi.LibellePortion).ToList();
                }

                return result;
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
        /// Définit l'état de du TileView des portions permet de charger les portions
        /// </summary>
        public TileViewItemState EquipementssTileItemState
        {
            get { return _equipementssTileItemState; }
            set
            {
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null && this.ListEquipement == null)
                {
                    IsBusy = true;
                    ((PpService)service).GetListEquipement(this.SelectedEntity.ClePp, GetListEquipementsDone);
                }

                _equipementssTileItemState = value;
                RaisePropertyChanged(() => EquipementssTileItemState);
            }
        }

        /// <summary>
        /// Liste des équipements de la PP en cours
        /// </summary>
        public List<EqEquipement> ListEquipement
        {
            get { return _listEquipement; }
            set
            {
                _listEquipement = value;
                RaisePropertyChanged(() => this.ListEquipement);
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
                    ((PpService)service).GetListTournnees(this.SelectedEntity.ClePp, GetListTourneesDone);
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

        #region Commands
        /// <summary>
        /// Commande utilisé pour réintégrer une PP
        /// </summary>
        public IActionCommand ReintegratePPCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DeleteNiveauProtectionCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command d'ajout d'un niveau de protection
        /// </summary>
        public IActionCommand AddNiveauProtectionCommand { get; set; }

        #endregion Commands

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public Equipement_PPViewModel()
            : base()
        {
            this.SaveCommand = new ActionCommand<object>(
               obj => Save(), obj => PPCanSave);

            this.ReintegratePPCommand = new ActionCommand<object>(
                obj => ReintegratePP(), obj => CanEdit);

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

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(r => r.Key == "IsTopContainerLoaded"))
                {
                    EventAggregator.Publish("CustomTopContainer".AsViewNavigationArgs().AddNamedParameter("HideContainer", false));
                    EventAggregator.Publish("TypeEquipement".AsViewNavigationArgs().AddNamedParameter("IsTopContainerLoaded", true));
                }

                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("PP_DetailPP".AsViewNavigationArgs());
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.EquipementExpanderTitle));
                    EventAggregator.Publish("PP_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }

                RaisePropertyChanged(() => this.IsNewEquipement);
                RaisePropertyChanged(() => this.AfficheNavigate);
                RaisePropertyChanged(() => this.ShowNiveauAssocie);
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
                RaisePropertyChanged(() => this.ListPortions);
            };

            this.OnEntitiesLoaded += (o, e) =>
            {
                // Chargement supplémentaire des modèles de mesure
                IsBusy = true;
                ((MesModeleMesureService)ServiceMesModeleMesure).GetAllEntity((err) => MesModeleMesureLoaded(err));
            };

            this.OnDetailLoaded += (o, e) =>
            {
                if (SelectedEntity != null)
                {
                    EventAggregator.Publish("PP_DetailPP".AsViewNavigationArgs().AddNamedParameter("SelectedEntity", this.SelectedEntity));

                    if (this.SelectedEntity.ValidationErrors.Any() && !IsEditMode)
                    {
                        this.SelectedEntity.ValidationErrors.Clear();
                    }

                    RaisePropertyChanged(() => this.IsNewEquipement);
                    RaisePropertyChanged(() => this.SelectedEntity);
                    RaisePropertyChanged(() => this.AfficheNavigate);

                    this.ReintegratePPCommand.RaiseCanExecuteChanged();

                    ListEquipement = null;
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
                    if (this.EquipementssTileItemState == TileViewItemState.Maximized)
                    {
                        IsBusy = true;
                        ((PpService)service).GetListEquipement(this.SelectedEntity.ClePp, GetListEquipementsDone);
                    }

                    //Au changement de la pp on charge les équipements si l'utilisateur se trouve sur l'onglet des équipements
                    if (this.TourneesTileItemState == TileViewItemState.Maximized)
                    {
                        IsBusy = true;
                        ((PpService)service).GetListTournnees(this.SelectedEntity.ClePp, GetListTourneesDone);
                    }

                    RaisePropertyChanged(() => this.SelectedEntity.InfosEquipment);
                    RaisePropertyChanged(() => this.IsInfoAffiche);

                    registerPropertyChanged();
                }
            };

            this.OnAddedEntity += (o, e) =>
            {
                EventAggregator.Publish("PP_DetailPP".AsViewNavigationArgs().AddNamedParameter("SelectedEntity", this.SelectedEntity));

                RaisePropertyChanged(() => this.IsNewEquipement);

                RaisePropertyChanged(() => this.AfficheNavigate);
                registerPropertyChanged();
                RaisePropertyChanged(() => this.ListPortions);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            };

            this.OnCanceled += (o, e) =>
            {
                if (this.SelectedEntity != null)
                {
                    this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                }
                RaisePropertyChanged(() => this.IsNewEquipement);
                RaisePropertyChanged(() => this.AfficheNavigate);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                _isReintegrated = false;
                _isDeleted = false;
                RaisePropertyChanged(() => this.AfficheNavigate);
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
                RaisePropertyChanged(() => this.IsInfoAffiche);
                RaisePropertyChanged(() => this.ShowNiveauAssocie);
            };
        }

        #endregion Constructor

        #region Services
        
        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les GEO ensembles électriques / portions
        /// </summary>
        [Import]
        public IEntityService<GeoEnsElecPortion> ServiceGeoEnsElecPortion { get; set; }

        /// <summary>
        /// Service utilisé pour pouvoir associer les niveaux de protections sur l'équipement PP
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> ServiceTypeEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les PP Jumelé
        /// </summary>
        [Import]
        public IEntityService<PpJumelee> ServicePpJumelee { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de modèle mesure
        /// </summary>
        [Import]
        public IEntityService<MesModeleMesure> ServiceMesModeleMesure { get; set; }

        #endregion Services

        #region Override Methods
        
        protected override void DeactivateView(string viewName)
        {
            // désactivation de la vue de détail PP
            Router.DeactivateView("Visite_DetailPP");

            base.DeactivateView(viewName);
        }

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
            IsBusy = true;

            saveGeoPreferences();

            ((PpService)this.service).FindPpByCriterias(this.FiltreCleRegion, this.FiltreCleAgence, this.FiltreCleSecteur,
                this.FiltreCleEnsElec, this.FiltreClePortion, this.IncludeDeletedEquipment, SearchDone);
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
        /// Surcharge de la méthode save pour mettre a jour les valeurs conditionnées
        /// </summary>
        protected override void Save(bool forceSave, bool withHisto)
        {
            if (SelectedEntity != null)
            {
                //Si la PP est issue d'un déplacement son secteur peu ne pas être contenu dans la liste et donc rien n'est visible à l'écran
                if (this.SelectedEntity.PortionIntegrite != null && !this.SelectedEntity.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur == this.SelectedEntity.GeoSecteur))
                {
                    this.SelectedEntity.GeoSecteur = null;
                }

                if (this.CommentaireBeforeUpdate != this.SelectedEntity.Commentaire)
                {
                    if ((string.IsNullOrEmpty(this.CommentaireBeforeUpdate) ^ string.IsNullOrEmpty(this.SelectedEntity.Commentaire)) ||
                        !string.IsNullOrEmpty(this.CommentaireBeforeUpdate) && !string.IsNullOrEmpty(this.SelectedEntity.Commentaire))
                    {
                        this.SelectedEntity.DateMajCommentaire = DateTime.Now;
                        this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                    }
                }

                SelectedEntity.DateMajPp = DateTime.Now;

                //On initialise l'utilisateur lors de la création de la PP
                if (SelectedEntity.IsNew())
                    SelectedEntity.CleUtilisateur = CurrentUser.CleUtilisateur;

                if (!SelectedEntity.PresenceDUneTelemesure)
                {
                    SelectedEntity.DateMiseEnServiceTelemesure = null;
                }
                if (!SelectedEntity.TemoinEnterreAmovible)
                {
                    SelectedEntity.EnumSurfaceTme = null;
                }
                if (!SelectedEntity.TemoinMetalliqueDeSurface)
                {
                    SelectedEntity.EnumSurfaceTms = null;
                }

                this.SelectedEntity.DdeDeverrouillageCoordGps = (this.SelectedEntity.CoordonneeGpsFiabilisee == false && this.SelectedEntity.DdeDeverrouillageCoordGps) ? false : this.SelectedEntity.DdeDeverrouillageCoordGps;

                // Si la fiabilisation n'est pas active, on supprime les champs liés
                if (!this.SelectedEntity.DdeDeverrouillageCoordGps)
                {
                    this.SelectedEntity.UsrUtilisateur1 = null;
                    this.SelectedEntity.DateDdeDeverrouillageCoordGps = null;
                }
            }

            ListEquipement = null;
            ListTournees = null;

            base.Save(forceSave, withHisto);
        }

        /// <summary>
        /// Surcharge de la méthode Delete
        /// </summary>
        protected override void Delete()
        {
            MessageBoxResult result = MessageBox.Show(Resource.DeletePP_Text, Resource.DeletePPCaption, MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                IsBusy = true;
                ListEquipement = null;
                ListTournees = null;

                if (SelectedEntity.PpJumelee.Count > 0 || SelectedEntity.PpJumelee1.Count > 0)
                {
                    MessageBoxResult resultPPJumele = MessageBox.Show(Resource.Equipement_PP_ErrorPPJumele, Resource.DeletePPCaption, MessageBoxButton.OK);
                    if (resultPPJumele == MessageBoxResult.OK)
                    {
                        IsEditMode = false;
                        IsBusy = false;
                    }
                }
                else
                {
                    ((PpService)service).GetEquipementsToDeleteByPP(this.SelectedEntity.ClePp, PPDeletedDone);
                }
            }
        }

        protected override void Cancel()
        {
            if (this.SelectedEntity.IsNew())
            {
                this.SelectedEntity.ValidationErrors.Clear();
                RaisePropertyChanged(() => this.SelectedEntity);
            }
            base.Cancel();
        }

        #endregion Protected Methods

        #region Private Functions

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
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Tournee).Name));
            }
            else
            {
                this.ListTournees = listTournees;
            }
        }

        /// <summary>
        /// Récupération des équipements de la PP
        /// </summary>
        /// <param name="error"></param>
        /// <param name="listEquipement"></param>
        private void GetListEquipementsDone(Exception error, List<EqEquipement> listEquipement)
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
                this.ListEquipement = listEquipement;
            }
        }

        /// <summary>
        /// La liste des équipements qui doivent être supprimé logiquement en cascade est chargée
        /// </summary>
        /// <param name="error"></param>
        /// <param name="code">un code pour la suppression: 
        ///     - 1=>suppression physique 
        ///     - 2=> suppression logique
        ///     - 3=> suppression impossible</param>
        private void PPDeletedDone(Exception error, int code)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(error);
            }
            else
            {
                //s'il y a présence d'équipements ayant cette PP en PP secondaire non obligatoire et que la Pp est supprimable
                if (code > 4)
                {
                    foreach (EqFourreauMetallique eq in (this.service as PpService).EquipementsSecondaires.OfType<EqFourreauMetallique>())
                    {
                        eq.ClePp2 = null;
                        this.LogOuvrage("M", eq);
                    }
                    foreach (EqRaccordIsolant eq in (this.service as PpService).EquipementsSecondaires.OfType<EqRaccordIsolant>())
                    {
                        eq.ClePp2 = null;
                        this.LogOuvrage("M", eq);
                    }                    
                }

                // on reprend le schéma de suppresion historique
                if (code > 3)
                {
                    code -= 3;
                }

                // Suppresion physique ou logique => gestion des PPJumelee
                if (code > 1)
                {
                    foreach (PpJumelee PJ in this.SelectedEntity.PpJumelee.Union(this.SelectedEntity.PpJumelee1))
                    {
                        if (PJ.ClePp == this.SelectedEntity.ClePp)
                        {
                            this.LogOuvrage("M", PJ.Pp1, ResourceHisto.PpJumelee);
                            ServicePpJumelee.Delete(PJ);
                        }
                        else if (PJ.PpClePp == this.SelectedEntity.ClePp)
                        {
                            this.LogOuvrage("M", PJ.Pp, ResourceHisto.PpJumelee);
                            ServicePpJumelee.Delete(PJ);
                        }
                    }
                }

                // suppression physique
                if (code == 3)
                {
                    base.Delete(false, true);
                }
                //suppression logique
                else if (code == 2)
                {
                    this.SelectedEntity.Supprime = true;
                    _isDeleted = true;
                    this.Save(true);
                    RaisePropertyChanged(() => this.SelectedEntity.InfosEquipment);
                    RaisePropertyChanged(() => this.IsInfoAffiche);
                }
                // suppression impossible des éléments liés non supprimé logiquement sont rattachés à la PP
                else
                {
                    MessageBox.Show(Resource.BaseProtecaEntityViewModel_DeleteImpossible, string.Empty, MessageBoxButton.OK);
                }
            }

            IsBusy = false;
        }

        /// <summary>
        /// Réintégration d'une PP
        /// </summary>
        private void ReintegratePP()
        {
            this.SelectedEntity.Supprime = false;
            RaisePropertyChanged(() => this.SelectedEntity.InfosEquipment);
            RaisePropertyChanged(() => this.IsInfoAffiche);
            if (this.SelectedEntity.PortionIntegrite != null && this.SelectedEntity.PortionIntegrite.Supprime)
            {
                this.NotifyError = true;
                this.IsEditMode = true;
                // Passage de la portion à null après le mode Edition (Pour la gestiond de l'historique)
                this.SelectedEntity.PortionIntegrite = null;
                RaisePropertyChanged(() => this.SelectedEntity.PortionIntegrite);

            }
            else if (!this.NotifyError)
            {
                _isReintegrated = true;
                this.Save(true);
            }
        }

        /// <summary>
        /// La recherche des PP est terminée
        /// </summary>
        private void SearchDone(Exception error)
        {
            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => EntitiesCount);
            RaisePropertyChanged(() => ResultIndicator);
            if (IsAutoNavigateToFirst && this.Entities != null && this.Entities.Any())
            {
                int _clePp = (int)Entities.First().GetCustomIdentity();
                if (this.SelectedEntity != null && this.SelectedEntity.ClePp == _clePp)
                {
                    this.IsBusy = false;
                }
                else
                {
                    NavigationService.Navigate(_clePp);
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
        /// Abonnement à propertyChanged pour la gestion des coordonnées GPS et du niveau de sensibilité
        /// </summary>
        private void registerPropertyChanged()
        {
            if (SelectedEntity != null)
            {
                SelectedEntity.PropertyChanged += (oo, ee) =>
                {
                    if (ee.PropertyName == "CourantsAlternatifsInduits" || ee.PropertyName == "TemoinEnterreAmovible" || ee.PropertyName == "TemoinMetalliqueDeSurface" || ee.PropertyName == "ElectrodeEnterreeAmovible" || ee.PropertyName == "PresenceDUneTelemesure" || ee.PropertyName == "CourantsVagabonds")
                    {
                        this.SaveCommand.RaiseCanExecuteChanged();
                    }
                };
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

        #region Public Methods

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
                niveauProtection.TypeEquipement = ServiceTypeEquipement.Entities.FirstOrDefault(ty => ty.CodeEquipement == "PP");
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
                ((PpService)service).DeleteAssociateEntity((MesNiveauProtection)Obj);
                this.SelectedEntity.MesNiveauProtection.Remove((MesNiveauProtection)Obj);
                RaisePropertyChanged(() => this.NiveauProtectionAssocies);
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
            if (this.CurrentUser != null && this.SelectedEntity != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                if (role != null && role.RefUsrPortee != null)
                {
                    string codePortee = role.RefUsrPortee.CodePortee;

                    if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                    {
                        return this.SelectedEntity.GeoSecteur != null && this.SelectedEntity.GeoSecteur.CleAgence == CurrentUser.CleAgence;
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
                        return this.SelectedEntity.GeoSecteur != null && this.SelectedEntity.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion;
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                    {
                        return this.SelectedEntity.CleSecteur == CurrentUser.CleSecteur;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
