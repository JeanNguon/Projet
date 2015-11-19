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
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using Proteca.Silverlight.Models;

namespace Proteca.Silverlight.ViewModels
{

    /// <summary>
    /// ViewModel for ValidationEquipement entity
    /// </summary>
    [ExportAsViewModel("ValidationEquipement")]
    public class ValidationEquipementViewModel : BaseOuvrageViewModel<EqEquipementTmp>
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

        private TileViewItemState _rejetsTileItemState = TileViewItemState.Minimized;

        private List<Rejet> _listRejets;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Liste des colonnes dont la visibilité doit être inersée pour l'export
        /// </summary>
        public ObservableCollection<string> ColumnsHiddenToExport
        {
            get
            {
                return new ObservableCollection<string> { Resource.Rejets_LibellePortion, Resource.Rejets_VisiteContent, Resource.Rejets_PpTmpContent};
            }
        }

        /// <summary>
        /// Liste des rejets de visites
        /// </summary>
        public List<Rejet> ListRejets
        {
            get
            {
                if (_listRejets == null)
                {
                    _listRejets = new List<Rejet>();
                }
                return _listRejets.Where(r => r.CleEqTmp.HasValue && !Entities.Any(e => e.CleEqTmp == r.CleEqTmp.Value) || r.ClePpTmp.HasValue && !PpEntities.Any(p => p.ClePpTmp == r.ClePpTmp.Value)).ToList();
            }
        }

        /// <summary>
        /// Etat du tileview des rejets de visites
        /// </summary>
        public TileViewItemState RejetsTileItemState
        {
            get { return _rejetsTileItemState; }
            set
            {
                if (value == TileViewItemState.Maximized)
                {
                    this.RefreshListRejet();
                }
                _rejetsTileItemState = value;
                RaisePropertyChanged(() => RejetsTileItemState);
            }
        }

        public int CountTmpRejetes
        {
            get
            {
                return (this.ListRejets == null) ? 0 : this.ListRejets.Count();
            }
        }

        public override bool CanEdit
        {
            get
            {
                return canEdit && UserCanEdit;
            }
            set
            {
                base.CanEdit = value;
            }
        }

        /// <summary>
        /// Collection des PpTmp
        /// </summary>
        public ObservableCollection<PpTmp> PpEntities
        {
            get
            {
                return this.ServicePpTmp.Entities;
            }
        }

        /// <summary>
        /// Nombre de PpTmp retournées par la query
        /// </summary>
        public int PpEntitiesCount
        {
            get
            {
                return PpEntities != null ? PpEntities.Count() : 0;
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
        public DateTime? DateMin { get; set; }

        /// <summary>
        /// Retourne la date de fin du filtre
        /// </summary>
        public DateTime? DateMax { get; set; }

        /// <summary>
        /// Retourne la valeur de la case à cocher A valider.
        /// </summary>
        public bool IsAValider { get; set; }

        /// <summary>
        /// Retourne la valeur de la case à cocher A compléter.
        /// </summary>
        public bool IsACompleter { get; set; }

        public bool? FiltreValider
        {
            get
            {
                if ((this.IsAValider ^ this.IsACompleter))
                {
                    return IsACompleter;
                }
                else
                {
                    return null;
                }
            }
        }

        private bool _isEqTmpExpanded;
        /// <summary>
        /// Retourne si l'expander EqTmp est développé
        /// </summary>
        public bool IsEqTmpExpanded
        {
            get
            {
                return _isEqTmpExpanded;
            }
            set
            {
                _isEqTmpExpanded = value;
                if (_isEqTmpExpanded == true)
                {
                    IsPpTmpExpanded = false;
                }

                RaisePropertyChanged(() => this.IsEqTmpExpanded);
                RaisePropertyChanged(() => this.IsPpTmpExpanded);
            }
        }

        private bool _isPpTmpExpanded;
        /// <summary>
        /// Retourne si l'expander PpTmp est développé
        /// </summary>
        public bool IsPpTmpExpanded
        {
            get
            {
                return _isPpTmpExpanded;
            }
            set
            {
                _isPpTmpExpanded = value;
                if (_isPpTmpExpanded == true)
                {
                    IsEqTmpExpanded = false;
                }

                RaisePropertyChanged(() => this.IsPpTmpExpanded);
                RaisePropertyChanged(() => this.IsEqTmpExpanded);
            }
        }

        #endregion Public Members

        #region Services

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
        /// Service utilisé pour gérer les Pp temporaires
        /// </summary>
        [Import]
        public IEntityService<PpTmp> ServicePpTmp { get; set; }

        /// <summary>
        /// Service utilisé pour disposer des types equipement
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> ServiceTypeEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour disposer des types equipement
        /// </summary>
        [Import]
        public IEntityService<CategoriePp> ServiceCategoriePp { get; set; }

        /// <summary>
        /// Service utilisé pour disposer des types equipement
        /// </summary>
        [Import]
        public IEntityService<RefNiveauSensibilitePp> ServiceRefNiveauSensibilitePp { get; set; }

        #endregion Services

        #region Constructor

        public ValidationEquipementViewModel()
            : base()
        {
            IsAutoNavigateToFirst = false;

            AllowEditEmptyEntities = false;

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
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };
            
            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.ValidationEquipementTmp_ExpanderTitle));
                    EventAggregator.Publish("ValidationEquipement_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
                this.ClearListRejet();
                this.IsPpTmpExpanded = false;
                this.IsEqTmpExpanded = false;
                RaisePropertyChanged(() => this.PpEntities);
                RaisePropertyChanged(() => this.PpEntitiesCount);
            };

            this.OnCanceled += (o, e) =>
            {
                RefreshLists();
                this.ClearListRejet();
            };

            this.OnSaveSuccess += (o, e) =>
            {
                RefreshLists();
            };

            this.OnSaveError += (o, e) =>
            {
                this.ClearListRejet();
            };

            //Commandes
            DeleteEqTmpLineCommand = new ActionCommand<object>(
                obj => DeleteEqTmpLine(obj), obj => true);

            CheckEqTmpGroupCommand = new ActionCommand<object>(
                obj => CheckEqTmpGroup(obj), obj => true);

            CheckEqTmpColumnCommand = new ActionCommand<object>(
                obj => CheckEqTmpColumn(obj), obj => true);

            DeletePpTmpLineCommand = new ActionCommand<object>(
                obj => DeletePpTmpLine(obj), obj => true);

            CheckPpTmpGroupCommand = new ActionCommand<object>(
                obj => CheckPpTmpGroup(obj), obj => true);

            CheckPpTmpColumnCommand = new ActionCommand<object>(
                obj => CheckPpTmpColumn(obj), obj => true);

            this.IsACompleter = false;
            this.IsAValider = false;
        }

        #endregion Constructor

        #region Commands

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne EqTmp
        /// </summary>
        public IActionCommand DeleteEqTmpLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de check des lignes par groupe EqTmp
        /// </summary>
        public IActionCommand CheckEqTmpGroupCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de check des lignes par groupe EqTmp
        /// </summary>
        public IActionCommand CheckEqTmpColumnCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne PpTmp
        /// </summary>
        public IActionCommand DeletePpTmpLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de check des lignes par groupe PpTmp
        /// </summary>
        public IActionCommand CheckPpTmpGroupCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de check des lignes par groupe PpTmp
        /// </summary>
        public IActionCommand CheckPpTmpColumnCommand { get; private set; }

        #endregion Commands

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
            if (DateMin.HasValue && DateMax.HasValue && DateMin.Value > DateMax.Value)
            {
                ErrorWindow.CreateNew(Resource.SaisieVisite_SearchErrorDate.ToString());
            }
            else
            {
                IsBusy = true;

                saveGeoPreferences();

                (this.service as EqEquipementTmpService).FindEquipementsTmpByCriterias(FiltreCleRegion, FiltreCleAgence, FiltreCleSecteur,
                                                                     FiltreCleEnsElec, FiltreClePortion,
                                                                     DateMin, DateMax, FiltreValider, SearchEqTmpDone);
            }     
        }

        protected override void Save()
        {
            // Mise à jour de la date de validation
            IEnumerable<EqEquipementTmp> equipements = this.Entities.Where(e => e.EstValide && !e.DateValidation.HasValue && e.CanValidGeo);
            foreach (EqEquipementTmp e in equipements)
            {
                e.DateValidation = DateTime.Now;
            }

            // Transfert des modification sur la Pp, log de ceux-ci et suppression de la pptmp
            List<PpTmp> pps = this.PpEntities.Where(p => p.Valider && p.CanValidGeo).ToList();
            for(int i = pps.Count - 1; i > -1; i--)
            {
                pps.ElementAt(i).CommitChangesToPp();
                LogOuvrage("M", pps.ElementAt(i).Pp);
                ServicePpTmp.Delete(pps.ElementAt(i));
            }

            base.Save();
        }

        protected override void DeactivateView(string viewName)
        {
            base.DeactivateView(viewName);
            this.IsPpTmpExpanded = false;
            this.IsEqTmpExpanded = false;
            RaisePropertyChanged(() => this.PpEntities);
            RaisePropertyChanged(() => this.PpEntitiesCount);
        }

        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// La recherche des eqtmp vient d'être effectuée.
        /// </summary>
        /// <param name="ex"></param>
        private void SearchEqTmpDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(EqEquipementTmp).Name));
            }
            else
            {
                (this.ServicePpTmp as PpTmpService).FindPpTmpByCriterias(FiltreCleRegion, FiltreCleAgence, FiltreCleSecteur,
                                                                 FiltreCleEnsElec, FiltreClePortion,
                                                                 DateMin, DateMax, SearchPpTmpDone);
            }
        }

        /// <summary>
        /// La recherche des pptmp vient d'être effectuée.
        /// </summary>
        /// <param name="ex"></param>
        private void SearchPpTmpDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(PpTmp).Name));
            }
            else
            {
                RefreshLists();
            }

            IsBusy = false;
        }

        /// <summary>
        /// Rafraichissement de la liste (tout est raffraichit)
        /// </summary>
        private void RefreshLists()
        {
            this.CheckCanEditByGeo();

            foreach (EqEquipementTmp item in this.Entities)
            {
                item.CanValid = !item.EstValide;
            }

            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => this.EntitiesCount);

            RaisePropertyChanged(() => this.PpEntities);
            RaisePropertyChanged(() => this.PpEntitiesCount);

            RaisePropertyChanged(() => this.ResultIndicator);

            EditCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau EqTmp
        /// </summary>
        protected virtual void DeleteEqTmpLine(object Obj)
        {
            var result = MessageBox.Show(Resource.ValidationEquipementTmp_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK && Obj is EqEquipementTmp && this.Entities.Any(e => e.CleEqTmp == (Obj as EqEquipementTmp).CleEqTmp))
            {
                EqEquipementTmp eqToDelete = this.Entities.FirstOrDefault(e => e.CleEqTmp == (Obj as EqEquipementTmp).CleEqTmp);

                //crée le rejet qui correspond à la visite en cours de suppression
                Rejet rejet = new Rejet()
                {
                    CleEqTmp = eqToDelete.CleEqTmp
                };

                Visite visite = eqToDelete.Visites.FirstOrDefault();
                
                if(visite != null)
                {
                    rejet.LibellePortion = eqToDelete.LibellePortion;
                    rejet.CodeEquipement = eqToDelete.CodeEquipement;
                    rejet.LibelleOuvrage = eqToDelete.Libelle;
                    rejet.DateVisite = visite.DateVisite;
                    rejet.TypeEval = visite.RefEnumValeur.Libelle;
                    rejet.VisiteContent = visite.VisiteSerialized;
                }

                this.AddToListRejet(rejet);

                this.service.Delete(eqToDelete);

                RaisePropertyChanged(() => this.Entities);
                RaisePropertyChanged(() => this.ResultIndicator);
                RaisePropertyChanged(() => this.EntitiesCount);
            }
        }

        /// <summary>
        /// Fonction de check des lignes par groupe EqTmp
        /// </summary>
        /// <param name="Obj">Object CheckBox du groupe</param>
        protected virtual void CheckEqTmpGroup(object Obj)
        {
            //On vérifie que l'object est bien une checkbox et qu'il contient bien son datacontext
            if (Obj != null && Obj is CheckBox && (Obj as CheckBox).DataContext != null)
            {
                //Récupération du GroupViewModel contenant la checkbox
                GroupViewModel gvm = (Obj as CheckBox).DataContext as GroupViewModel;
                //Récupération de l'état dans lequel doit être la checkbox
                Nullable<Boolean> estValide = (Nullable<Boolean>)gvm.AggregateResults[0].Value;
                //Si la checkbox est cochée ou non on prends l'inverse de son état sinon on la met à cochée
                estValide = estValide.HasValue ? !estValide.Value : true;

                //Récupération du premier Equipement pour selectionner un élément plus tard
                // + Set des items contenus dans le groupe à la nouvelle valeur
                EqEquipementTmp tmpEquipement = null;
                foreach (EqEquipementTmp item in gvm.Group.Items)
                {
                    if (tmpEquipement == null)
                    {
                        tmpEquipement = item;
                    }
                    item.EstValide = estValide.Value && item.CanValidGeo;
                }

                //Reset du binding après avoir setté la valeur de la checkbox 
                Binding binding = new Binding("AggregateResults[0].Value");
                (Obj as CheckBox).SetBinding(CheckBox.IsCheckedProperty, binding);

                //Mise à jour des aggregatesResults pour le tableau
                RadGridView grid = (Obj as CheckBox).ParentOfType<RadGridView>();
                if (grid != null && tmpEquipement != null)
                {
                    grid.Columns["Valider"].IsReadOnly = false;
                    grid.Items.EditItem(tmpEquipement);
                    grid.Items.CommitEdit();
                    grid.CancelEdit();
                    grid.Columns["Valider"].IsReadOnly = true;
                }
            }
        }

        /// <summary>
        /// Fonction de check des lignes pour toutes les entitées EqTmp
        /// </summary>
        /// <param name="Obj"></param>
        protected virtual void CheckEqTmpColumn(object Obj)
        {
            //On vérifie que l'objet est bien une checkbox
            if (Obj != null && Obj is CheckBox)
            {
                // Implémentation à revoir.

                //On vérifie l'état dans lequel doit se trouver le bouton et on calcule comment il doit modifier les items
                Boolean estValide = Entities.Any(e => !e.EstValide && e.CanValidGeo);

                //Récupération d'une Equipement par groupe dans une liste
                // + mise à jour de la propriété EstValidee
                List<EqEquipementTmp> tmpEquipements = new List<EqEquipementTmp>();

                foreach (EqEquipementTmp item in Entities.Where(e => e.CanValidGeo))
                {
                    //Séléction d'une EqEquipement par libelle portion
                    if (!tmpEquipements.Any(e => e.CanValidGeo && e.Pp.PortionIntegrite.Libelle == item.Pp.PortionIntegrite.Libelle))
                    {
                        tmpEquipements.Add(item);
                    }
                    item.EstValide = estValide && item.CanValidGeo;
                }

                (Obj as CheckBox).IsChecked = estValide;

                //Reset du binding après avoir setté la valeur de la checkbox 
                Binding binding = new Binding("AggregateResults[0].Value");
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor); 
                binding.RelativeSource.AncestorType = typeof(Proteca.Silverlight.Views.UserContols.CustomGridView);
                (Obj as CheckBox).SetBinding(CheckBox.IsCheckedProperty, binding);

                //Mise à jour des aggregatesResults pour le tableau
                RadGridView grid = (Obj as CheckBox).ParentOfType<RadGridView>();
                if (grid != null && tmpEquipements.Count > 0)
                {
                    grid.Columns["Valider"].IsReadOnly = false;
                    //Mise à jour des aggregatesResults de chaque groupe
                    foreach (EqEquipementTmp item in tmpEquipements)
                    {
                        grid.Items.EditItem(item);
                        grid.Items.CommitEdit();
                        grid.CancelEdit();
                    }
                    grid.Columns["Valider"].IsReadOnly = true;
                }
            }
        }

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau PpTmp
        /// </summary>
        protected virtual void DeletePpTmpLine(object Obj)
        {
            var result = MessageBox.Show(Resource.ValidationEquipementTmp_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK && Obj is PpTmp && this.PpEntities.Any(p => p.ClePpTmp == (Obj as PpTmp).ClePpTmp))
            {
                PpTmp ppToDelete = this.PpEntities.FirstOrDefault(p => p.ClePpTmp == (Obj as PpTmp).ClePpTmp);

                //crée le rejet qui correspond à la visite en cours de suppression
                Rejet rejet = new Rejet()
                {
                    CleEqTmp = ppToDelete.ClePpTmp,

                    PpTmpContent = String.Join("\n", ppToDelete.PpTmpToText())
                };

                Visite visite = ppToDelete.Visites.FirstOrDefault();

                if (visite != null)
                {
                    rejet.LibellePortion = ppToDelete.LibellePortion;
                    rejet.CodeEquipement = "PP";
                    rejet.LibelleOuvrage = ppToDelete.Pp.Libelle;
                    rejet.DateVisite = visite.DateVisite;
                    rejet.TypeEval = visite.RefEnumValeur.Libelle;
                    rejet.VisiteContent = visite.VisiteSerialized;
                }

                this.AddToListRejet(rejet);

                this.ServicePpTmp.Delete(ppToDelete);

                RaisePropertyChanged(() => this.PpEntities);
                RaisePropertyChanged(() => this.ResultIndicator);
                RaisePropertyChanged(() => this.PpEntitiesCount);
            }
        }

        /// <summary>
        /// Fonction de check des lignes par groupe PpTmp
        /// </summary>
        /// <param name="Obj">Object CheckBox du groupe</param>
        protected virtual void CheckPpTmpGroup(object Obj)
        {
            //On vérifie que l'object est bien une checkbox et qu'il contient bien son datacontext
            if (Obj != null && Obj is CheckBox && (Obj as CheckBox).DataContext != null)
            {
                //Récupération du GroupViewModel contenant la checkbox
                GroupViewModel gvm = (Obj as CheckBox).DataContext as GroupViewModel;
                //Récupération de l'état dans lequel doit être la checkbox
                Nullable<Boolean> estValide = (Nullable<Boolean>)gvm.AggregateResults[0].Value;
                //Si la checkbox est cochée ou non on prends l'inverse de son état sinon on la met à cochée
                estValide = estValide.HasValue ? !estValide.Value : true;

                //Récupération du premier Equipement pour selectionner un élément plus tard
                // + Set des items contenus dans le groupe à la nouvelle valeur
                PpTmp tmpPp = null;
                foreach (PpTmp item in gvm.Group.Items)
                {
                    if (tmpPp == null)
                    {
                        tmpPp = item;
                    }
                    item.Valider = estValide.Value && item.CanValidGeo;
                }

                //Reset du binding après avoir setté la valeur de la checkbox 
                Binding binding = new Binding("AggregateResults[0].Value");
                (Obj as CheckBox).SetBinding(CheckBox.IsCheckedProperty, binding);

                //Mise à jour des aggregatesResults pour le tableau
                RadGridView grid = (Obj as CheckBox).ParentOfType<RadGridView>();
                if (grid != null && tmpPp != null)
                {
                    grid.Columns["Valider"].IsReadOnly = false;
                    grid.Items.EditItem(tmpPp);
                    grid.Items.CommitEdit();
                    grid.CancelEdit();
                    grid.Columns["Valider"].IsReadOnly = true;
                }
            }
        }

        /// <summary>
        /// Fonction de check des lignes pour toutes les entitées PpTmp
        /// </summary>
        /// <param name="Obj"></param>
        protected virtual void CheckPpTmpColumn(object Obj)
        {
            //On vérifie que l'objet est bien une checkbox
            if (Obj != null && Obj is CheckBox)
            {
                // Implémentation à revoir.

                //On vérifie l'état dans lequel doit se trouver le bouton et on calcule comment il doit modifier les items
                Boolean estValide = PpEntities.Any(p => !p.Valider && p.CanValidGeo);

                //Récupération d'une Equipement par groupe dans une liste
                // + mise à jour de la propriété EstValidee
                List<PpTmp> tmpPps = new List<PpTmp>();

                foreach (PpTmp item in PpEntities.Where(e => e.CanValidGeo))
                {
                    //Séléction d'une EqEquipement par libelle portion
                    if (!tmpPps.Any(p => p.CanValidGeo && p.Pp.PortionIntegrite.Libelle == item.Pp.PortionIntegrite.Libelle))
                    {
                        tmpPps.Add(item);
                    }
                    item.Valider = estValide && item.CanValidGeo;
                }

                (Obj as CheckBox).IsChecked = estValide;

                //Reset du binding après avoir setté la valeur de la checkbox 
                Binding binding = new Binding("AggregateResults[0].Value");
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
                binding.RelativeSource.AncestorType = typeof(Proteca.Silverlight.Views.UserContols.CustomGridView);
                (Obj as CheckBox).SetBinding(CheckBox.IsCheckedProperty, binding);

                //Mise à jour des aggregatesResults pour le tableau
                RadGridView grid = (Obj as CheckBox).ParentOfType<RadGridView>();
                if (grid != null && tmpPps.Count > 0)
                {
                    grid.Columns["Valider"].IsReadOnly = false;
                    //Mise à jour des aggregatesResults de chaque groupe
                    foreach (PpTmp item in tmpPps)
                    {
                        grid.Items.EditItem(item);
                        grid.Items.CommitEdit();
                        grid.CancelEdit();
                    }
                    grid.Columns["Valider"].IsReadOnly = true;
                }
            }
        }

        private void ClearListRejet()
        {
            if (this._listRejets != null)
            {
                this._listRejets.Clear();
            }
        }

        private void AddToListRejet(Rejet rejet)
        {
            if (this._listRejets != null)
            {
                this._listRejets.Add(rejet);
            }
        }

        private void RefreshListRejet()
        {
            RaisePropertyChanged(() => this.ListRejets);
            RaisePropertyChanged(() => this.CountTmpRejetes);
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
            return GetAutorisation();
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
            return GetAutorisation() && (this.Entities.Any(e => e.CanValidGeo) || this.PpEntities.Any(p => p.CanValidGeo));
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
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.IMPORT_VALIDATION);
                RefUsrPortee.ListPorteesEnum niveau = role.RefUsrPortee.GetPorteesEnum();

                if (niveau != RefUsrPortee.ListPorteesEnum.Interdite)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parcours les alerteDetail pour setter les canDisable Geo en fonction des droits de l'utilisateur
        /// </summary>
        private void CheckCanEditByGeo()
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.IMPORT_VALIDATION);
                RefUsrPortee.ListPorteesEnum niveau = role.RefUsrPortee.GetPorteesEnum();

                UsrRole roleEq = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                RefUsrPortee.ListPorteesEnum niveauEq = roleEq.RefUsrPortee.GetPorteesEnum();

                foreach (EqEquipementTmp eq in this.Entities)
                {
                    switch (niveau)
                    {
                        case RefUsrPortee.ListPorteesEnum.Autorisee:
                        case RefUsrPortee.ListPorteesEnum.National:
                            eq.CanValidGeo = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Region:
                            eq.CanValidGeo = this.CurrentUser.CleAgence.HasValue && eq.Pp.GeoSecteur.GeoAgence.CleRegion == this.CurrentUser.GeoAgence.CleRegion;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            eq.CanValidGeo = this.CurrentUser.CleAgence.HasValue && eq.Pp.GeoSecteur.CleAgence == this.CurrentUser.CleAgence.Value;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                            eq.CanValidGeo = this.CurrentUser.CleSecteur.HasValue && eq.Pp.CleSecteur == this.CurrentUser.CleSecteur.Value;
                            break;
                        default:
                            eq.CanValidGeo = false;
                            break;
                    } 
                    switch (niveauEq)
                    {
                        case RefUsrPortee.ListPorteesEnum.Autorisee:
                        case RefUsrPortee.ListPorteesEnum.National:
                            eq.CanCompleteGeo = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Region:
                            eq.CanCompleteGeo = this.CurrentUser.CleAgence.HasValue && eq.Pp.GeoSecteur.GeoAgence.CleRegion == this.CurrentUser.GeoAgence.CleRegion;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            eq.CanCompleteGeo = this.CurrentUser.CleAgence.HasValue && eq.Pp.GeoSecteur.CleAgence == this.CurrentUser.CleAgence.Value;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                            eq.CanCompleteGeo = this.CurrentUser.CleSecteur.HasValue && eq.Pp.CleSecteur == this.CurrentUser.CleSecteur.Value;
                            break;
                        default:
                            eq.CanCompleteGeo = false;
                            break;
                    }
                }

                foreach (PpTmp pp in this.PpEntities)
                {
                    switch (niveau)
                    {
                        case RefUsrPortee.ListPorteesEnum.Autorisee:
                        case RefUsrPortee.ListPorteesEnum.National:
                            pp.CanValidGeo = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Region:
                            pp.CanValidGeo = this.CurrentUser.CleAgence.HasValue && pp.Pp.GeoSecteur.GeoAgence.CleRegion == this.CurrentUser.GeoAgence.CleRegion;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            pp.CanValidGeo = this.CurrentUser.CleAgence.HasValue && pp.Pp.GeoSecteur.CleAgence == this.CurrentUser.CleAgence.Value;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                            pp.CanValidGeo = this.CurrentUser.CleSecteur.HasValue && pp.Pp.CleSecteur == this.CurrentUser.CleSecteur.Value;
                            break;
                        default:
                            pp.CanValidGeo = false;
                            break;
                    }
                }
            }
        }

        #endregion Autorisations
    }
}
