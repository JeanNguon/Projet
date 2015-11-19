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
using Jounce.Core.Command;
using Jounce.Framework.Command;
using System.ComponentModel.DataAnnotations;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for DeplacementPp entity
    /// </summary>
    [ExportAsViewModel("DeplacementPp")]
    public class DeplacementPpViewModel : BaseOuvrageViewModel<Pp>
    {
        #region services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceRefEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type portion integrite
        /// </summary>
        [Import]
        public IEntityService<PortionIntegrite> ServicePortion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type alertedetail
        /// </summary>
        [Import]
        public IEntityService<Alerte> ServiceAlerte { get; set; }

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
        public IEntityService<GeoEnsElecPortion> ServiceGeoEnsElecPortion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les Visites
        /// </summary>
        [Import]
        public IEntityService<Visite> ServiceVisite { get; set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// Déclaration de l'énum permettant d'afficher les types en base
        /// </summary>
        private string enumTypeAlerte = RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue();

        /// <summary>
        /// Déclaration de la variable de sélection d'alerte permettant l'affichage des Mesures
        /// </summary>
        private AlerteDetail _selectedAlerteDetail;

        #endregion

        #region Public Properties

        /// <summary>
        /// Lance le chargement des Mesures au changement de selection d'alerte
        /// </summary>
        public AlerteDetail SelectedAlerteDetail
        {
            get
            {
                return _selectedAlerteDetail;
            }
            set
            {
                _selectedAlerteDetail = value;
                if (_selectedAlerteDetail != null && _selectedAlerteDetail.Alerte != null && _selectedAlerteDetail.Alerte.CleVisite.HasValue && _selectedAlerteDetail.Alerte.Visite == null)
                {
                    (this.ServiceVisite as VisiteService).GetVisiteByCleLight(_selectedAlerteDetail.Alerte.CleVisite.Value, SearchVisiteDone);
                }
                else
                {
                    RaisePropertyChanged(() => this.SelectedAlerteDetail);
                }
            }
        }

        public bool IsNonEditableTileItemState
        {
            get
            {
                return (MainTileItemState == Telerik.Windows.Controls.TileViewItemState.Minimized);
            }
        }

        /// <summary>
        /// Active le bouton déplacer
        /// </summary>
        public bool IsDeplaceEnable { get; set; }

        /// <summary>
        /// Retourne la liste des portions
        /// </summary>
        public ObservableCollection<GeoEnsElecPortion> ListPortions
        {
            get
            {
                return ServiceGeoEnsElecPortion.Entities;
            }
        }

        /// <summary>
        /// PK cible du du déplacement
        /// </summary>
        private Nullable<decimal> _pKSelected;

        [MaxDecimalValue(MaxIntegerPartSize = 9, MaxDecimalPartSize = 3)]
        public Nullable<decimal> PKSelected
        {
            get
            {
                return _pKSelected;
            }
            set
            {
                if (value != null)
                {
                    Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "PKSelected" });
                }
                _pKSelected = value;
            }
        }

        /// <summary>
        /// Portion cible du dépacement
        /// </summary>
        public GeoEnsElecPortion PortionSelected { get; set; }

        /// <summary>
        /// Liste des alertes details lié àl'ensemble électrique
        /// </summary>
        public ObservableCollection<AlerteDetail> AlerteDetail
        {
            get
            {
                if (((AlerteService)ServiceAlerte).DetailEntities != null)
                {
                    return new ObservableCollection<AlerteDetail>(((AlerteService)ServiceAlerte).DetailEntities.OrderBy(c => c.Pk).ThenBy(d => d.LibelleType).ThenBy(d => d.Commentaire));
                }
                else
                {
                    return new ObservableCollection<AlerteDetail>();
                }
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
                else
                {
                    return new ObservableCollection<RefEnumValeur>();
                }
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
        private int? _filtreClePortionDeplacer;
        public int? FiltreClePortionDeplacer
        {
            get { return _filtreClePortionDeplacer; }
            set
            {
                _filtreClePortionDeplacer = value;
                RaisePropertyChanged(() => this.FiltreClePortionDeplacer);
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
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return ServiceRegion.Entities; }
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
                            return serviceGeoEnsElec.Entities.Where(i => i.CleSecteur == FiltreCleSecteur).ToList();
                        }
                        return serviceGeoEnsElec.Entities.Where(i => i.CleAgence == FiltreCleAgence).Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                        {
                            return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                        })).ToList();
                    }
                    return serviceGeoEnsElec.Entities.Where(i => i.CleRegion == FiltreCleRegion).Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                    })).ToList();
                }
                else
                {
                    return serviceGeoEnsElec.Entities.Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
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
        /// Liste des équipements de la PP en cours
        /// </summary>
        private List<EqEquipement> _listEquipement;
        public List<EqEquipement> ListEquipement
        {
            get 
            {
                if (_listEquipement == null)
                {
                    _listEquipement = new List<EqEquipement>();
                }
                return _listEquipement;
            }
            set
            {
                _listEquipement = value;
                RaisePropertyChanged(() => this.ListEquipement);
            }
        }

        /// <summary>
        /// Définit l'état de du TileView des portions permet de charger les portions
        /// </summary>
        private TileViewItemState _equipementssTileItemState;
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

        #endregion

        #region Commmand

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DeleteLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de déplacement de la PP
        /// </summary>
        public IActionCommand DeplacerCommand { get; private set; }

        #endregion

        #region Contructor

        public DeplacementPpViewModel()
            : base()
        {
            //Commandes
            DeleteLineCommand = new ActionCommand<object>(
                obj => DeleteLine(obj), obj => true);
            DeplacerCommand = new ActionCommand<object>(
                obj => DeplacerPP(), obj => true);


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
                RaisePropertyChanged(() => this.ListPortions);
            };
            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.DeplacementPp_ExpanderTitle));
                    EventAggregator.Publish("DeplacementPp_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }

                IsDeplaceEnable = false;
                this.PortionSelected = null;
                this.PKSelected = null;
                RaisePropertyChanged(() => this.IsDeplaceEnable);
                RaisePropertyChanged(() => this.PortionSelected);
                RaisePropertyChanged(() => this.PKSelected);
                RaisePropertyChanged(() => this.AlerteDetail);
            };

            this.OnDetailLoaded += (o, e) =>
            {
                if (this.SelectedEntity != null)
                {
                    IsBusy = true;
                    ((AlerteService)ServiceAlerte).FindAlerteByClePP(this.SelectedEntity.ClePp, false, SearchAlerteDone);
                    PKSelected = null;
                    PortionSelected = null;
                    IsDeplaceEnable = true;
                    RaisePropertyChanged(() => this.IsDeplaceEnable);

                    ListEquipement = null;
                    //Au changement de la pp on charge les équipements si l'utilisateur se trouve sur l'onglet des équipements
                    if (this.EquipementssTileItemState == TileViewItemState.Maximized)
                    {
                        IsBusy = true;
                        ((PpService)service).GetListEquipement(this.SelectedEntity.ClePp, GetListEquipementsDone);
                    }
                }
            };

            this.OnSaveSuccess += (o, e) =>
            {
                this.SelectedEntity = null;
                ListEquipement = null;
                IsBusy = false;
                PKSelected = null;
                PortionSelected = null;
                RaisePropertyChanged(() => this.AlerteDetail);
                InfoWindow.CreateNew(string.Format(Resource.DeplacementPp_OK, typeof(Pp).Name));
            };

            this.OnSaveError += (o, e) =>
            {
                ListEquipement = null;
                service.RejectChanges();
                ErrorWindow.CreateNew(string.Format(Resource.DeplacementPp_ErrorGlobal, typeof(Pp).Name));
                IsBusy = false;
            };

            this.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "MainTileItemState")
                {
                    RaisePropertyChanged(() => IsNonEditableTileItemState);
                }
            };
        }

        #endregion

        #region Override Methods
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
            if (userService.CurrentUser != null)
            {
                this.FiltreCleEnsElec = userService.CurrentUser.PreferenceCleEnsembleElectrique;
                this.FiltreClePortion = userService.CurrentUser.PreferenceClePortion;
            }
        }
        /// <summary>
        /// Surcharge de la méthode de retour des entities pour enlever les PP supprimés
        /// </summary>
        public override ObservableCollection<Pp> Entities
        {
            get
            {
                ObservableCollection<Pp> res = null;
                if (base.Entities != null)
                {
                    res = new ObservableCollection<Pp>(base.Entities.Where(c => c.Supprime == false));
                }
                else
                {
                    res = new ObservableCollection<Pp>();
                }
                return res;
            }
        }

        /// <summary>
        /// Surcharge du chargement du détail de l'entité
        /// </summary>
        public override void LoadDetailEntity()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IsBusy = true;
            ((PpService)service).GetDeplacementPpEntityByCle(SelectedId.Value, (error) => DetailEntityLoaded(error));
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
                this.FiltreCleEnsElec, this.FiltreClePortion, false, SearchDone);
        }

        protected override void DeactivateView(string viewName)
        {
            this.SelectedEntity = null;
            this.SelectedId = null;
            this.Entities.Clear();
            this.SelectedAlerteDetail = null;
            RaisePropertyChanged(() => this.AlerteDetail);
            base.DeactivateView(viewName);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        protected virtual void DeleteLine(object Obj)
        {
            var result = MessageBox.Show(Resource.Alerte_DisableConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                ((AlerteDetail)Obj).Supprime = true;
                Alerte alerte = ((AlerteService)ServiceAlerte).Entities.FirstOrDefault(a => a.CleAlerte == ((AlerteDetail)Obj).CleAlerte);
                if (alerte != null)
                {
                    alerte.Supprime = true;
                }
                RaisePropertyChanged(() => this.AlerteDetail);
            }
        }

        /// <summary>
        /// Fonction pour déplacer la PP
        /// </summary>
        public void DeplacerPP()
        {
            //Si il reste des alertes active ou des actions à traiter > Erreur
            if (AlerteDetail.Count(a => a.Alerte.Supprime == false) > 0)
            {
                ErrorWindow.CreateNew(string.Format(Resource.DeplacementPp_ErrorAlerte, typeof(Pp).Name));
            }
            else
            {
                IsBusy = true;

                decimal? PkTmp = PKSelected; // gestion du cas ou la Pk n'est pas renseigné.

                if (!PkTmp.HasValue)
                { PkTmp = this.SelectedEntity.Pk; } 

                // Gestion de la PK
                if (PortionSelected != null && (PkTmp.HasValue && (PkTmp.Value < PortionSelected.LongueurPortion && PkTmp.Value >= 0)))
                {
                    // On duplique la Pp
                    Pp ppDeplacer = ((Pp)this.SelectedEntity).DeplacerPp();

                    // On affecte la cle d'origine à la nouvelle Pp
                    ppDeplacer.ClePpOrigine = this.SelectedEntity.ClePp;

                    // Suppression logique de l'ancienne PP
                    this.SelectedEntity.Supprime = true;
                  
                    // Affectation de la portion cible sur la nouvelle Pp
                    ppDeplacer.ClePortion = PortionSelected.ClePortion;

                    // Affectation de la PK si déclaré
                    if (PkTmp.HasValue)
                    {
                        ppDeplacer.Pk = PkTmp.Value;
                    }

                    // Ajout des logs
                    LogOuvrage("C", ppDeplacer);
                    LogOuvrage("M", this.SelectedEntity);

                    // Ajout de la PP aux entities
                    this.service.Add(ppDeplacer);

                    ppDeplacer.AssocieCompEtVisites(this.SelectedEntity);

                    // Sauvegarde de la nouvelle PP
                    base.Save(true);

                    // MAJ de la vue
                    RaisePropertyChanged(() => this.Entities);
                    RaisePropertyChanged(() => this.SelectedEntity);
                    RaisePropertyChanged(() => this.SelectedId);
                }
                else
                {
                    IsBusy = false;

                    if (PortionSelected == null)
                    {
                        ErrorWindow.CreateNew(string.Format(Resource.DeplacementPp_ErrorPortion, typeof(Pp).Name));
                    }
                    else
                    {
                        ErrorWindow.CreateNew(string.Format(Resource.DeplacementPp_ErrorPK, typeof(Pp).Name));
                    }
                }

            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Evenement de fin de chargement de la visite et de ses mesures
        /// Gère le remplissage des tableaux VisiteMesure
        /// </summary>
        /// <param name="error"></param>
        private void SearchVisiteDone(Exception error)
        {
            if (error != null || this.SelectedAlerteDetail.Alerte.Visite == null || this.SelectedAlerteDetail.Alerte.Visite.MesMesure == null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Visite).Name));
            }
            else
            {
                this.SelectedAlerteDetail.Alerte.Visite.LoadVisiteMesures(new ObservableCollection<MesClassementMesure>(), null);
                RaisePropertyChanged(() => this.SelectedAlerteDetail);
            }
        }

        /// <summary>
        /// Méthode de retour de chargement des alertes
        /// </summary>
        /// <param name="error"></param>
        private void SearchAlerteDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Alerte).Name));
            }
            else
            {
                foreach (AlerteDetail item in this.AlerteDetail)
                {
                    Alerte element = this.ServiceAlerte.Entities.FirstOrDefault(a => a.CleAlerte == item.CleAlerte);
                    if (element != null)
                    {
                        item.Alerte = element;
                    }
                }
                this.CheckCanDisableByGeo();
                RaisePropertyChanged(() => this.AlerteDetail);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Méthode de retour de la recherche
        /// </summary>
        private void SearchDone(Exception error)
        {
            IsBusy = false;

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Pp).Name));
            }
            else
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
                }
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
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(EqEquipement).Name));
            }
            else
            {
                this.ListEquipement = listEquipement;
            }
        }

        #endregion

        #region Autorisations

        /// <summary>
        /// Parcours les alerteDetail pour setter les canDisable Geo en fonction des droits de l'utilisateur
        /// </summary>
        private void CheckCanDisableByGeo()
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GESTION_ALERTES_NIV);

                RefUsrPortee.ListPorteesEnum niveau = role.RefUsrPortee.GetPorteesEnum();

                foreach (AlerteDetail ad in this.AlerteDetail)
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
