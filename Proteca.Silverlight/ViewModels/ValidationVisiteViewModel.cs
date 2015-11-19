using System.Collections.ObjectModel;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using Proteca.Silverlight.Models;
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

namespace Proteca.Silverlight.ViewModels
{

    /// <summary>
    /// ViewModel for ValidationVisite entity
    /// </summary>
    [ExportAsViewModel("ValidationVisite")]
    public class ValidationVisiteViewModel : BaseProtecaEntityViewModel<Visite>
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
        /// Definition de la liste pour les Visites créées pour les Pp Jumelées
        /// </summary>
        private List<Visite> _visitesForPpJumelee;

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
                return new ObservableCollection<string> { Resource.Rejets_LibellePortion, Resource.Rejets_VisiteContent};
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
                return _listRejets.Where(r => !Entities.Any(v => v.CleVisite == r.CleVisite)).ToList();
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
                _rejetsTileItemState = value;
                RaisePropertyChanged(() => RejetsTileItemState);
                if (value == TileViewItemState.Maximized)
                {
                    this.RefreshListRejet();
                }
            }
        }

        /// <summary>
        /// Liste permettant de stocker toutes les Visites créées pour les Pp Jumelées
        /// </summary>
        public List<Visite> VisitesForPpJumelee
        {
            get
            {
                if (_visitesForPpJumelee == null)
                {
                    _visitesForPpJumelee = new List<Visite>();
                }
                return _visitesForPpJumelee;
            }
            set { _visitesForPpJumelee = value; }
        }

        public int CountVisitesRejetees
        {
            get
            {
                return (this.ListRejets == null) ? 0 : this.ListRejets.Count();
            }
        }

        public int CountVisitesAValider
        {
            get
            {
                return (this.Entities == null) ? 0 : this.Entities.Count();
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
        /// Service pour récupérer les MesClassementMesure
        /// </summary>
        [Import]
        public IEntityService<MesClassementMesure> ServiceMesClassementMesure { get; set; }

        /// <summary>
        /// Service pour récupérer les RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceRefEnumValeur { get; set; }

        /// <summary>
        /// Service pour récupérer les TypeEquipement
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> ServiceTypeEquipement { get; set; }

        /// <summary>
        /// Dervice pour supprimer les Alertes
        /// </summary>
        [Import]
        public IEntityService<Alerte> ServiceAlerte { get; set; }

        /// <summary>
        /// Service pour supprimer des MesMesure
        /// </summary>
        [Import]
        public IEntityService<MesMesure> ServiceMesMesure { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les instruments
        /// </summary>
        [Import]
        public IEntityService<InsInstrument> ServiceInstrument { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les alertes
        /// </summary>
        [Import]
        public IEntityService<AnAnalyseSerieMesure> ServiceAnAnalyseSerieMesure { get; set; }

        #endregion Services

        #region Constructor

        public ValidationVisiteViewModel()
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

            this.OnEntitiesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);

                IsBusy = true;
                ((MesClassementMesureService)this.ServiceMesClassementMesure).GetMesClassementMesureWithMesNiveauProtection(LoadMesNiveauProtectionDone);
            };

            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.ValidationVisite_ExpanderTitle));
                    EventAggregator.Publish("ValidationVisite_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
                ClearListRejet();
            };

            this.OnCanceled += (o, e) =>
            {
                this.ClearListRejet();
                this.RefreshList();
            };

            this.OnSaveSuccess += (o, e) =>
            {
                int count = this.Entities.Count;
                for (int i = count - 1; i > -1; i--)
                {
                    if (this.Entities[i].EstValidee)
                    {
                        this.Entities.RemoveAt(i);
                    }
                }

                this.RefreshList();
            };

            this.OnSaveError += (o, e) =>
            {
                this.ClearListRejet();
            };

            //Commandes
            DeleteLineCommand = new ActionCommand<object>(
                obj => DeleteLine(obj), obj => true);

            CheckGroupCommand = new ActionCommand<object>(
                obj => CheckGroup(obj), obj => true);

            CheckColumnCommand = new ActionCommand<object>(
                obj => CheckColumn(obj), obj => true);
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
            RaisePropertyChanged(() => this.CountVisitesRejetees);
        }

        #endregion Constructor

        #region Commands

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DeleteLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de check des lignes par groupe
        /// </summary>
        public IActionCommand CheckGroupCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de check des lignes par groupe
        /// </summary>
        public IActionCommand CheckColumnCommand { get; private set; }

        #endregion Commands

        #region Override Methods

        protected override void DeactivateView(string viewName)
        {
            VisitesForPpJumelee.Clear();

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
            if (DateMin.HasValue && DateMax.HasValue && DateMin.Value > DateMax.Value)
            {
                ErrorWindow.CreateNew(Resource.SaisieVisite_SearchErrorDate.ToString());
            }
            else
            {
                IsBusy = true;

                saveGeoPreferences();

                ((VisiteService)this.service).FindVisitesNonValideesByCriterias(FiltreCleRegion, FiltreCleAgence, FiltreCleSecteur,
                                                                     FiltreCleEnsElec, FiltreClePortion,
                                                                     DateMin, DateMax, SearchDone);
            }
        }

        /// <summary>
        /// Méthode surchargée pour permettre de recharger les tableaux visites mesures 
        /// à l'annulation de la suppression d'une visite
        /// </summary>
        protected override void Cancel()
        {
            this.IsBusy = true;

            base.Cancel();

            List<Visite> elementsRecharges = this.Entities.Where(v => !v.HasVisiteMesure).ToList();

            if (elementsRecharges.Any())
            {
                foreach (Visite item in elementsRecharges)
                {
                    item.LoadVisiteMesures(this.ServiceMesClassementMesure.Entities, null, false, true);
                }
                RaisePropertyChanged(() => this.Entities);
            }

            RaisePropertyChanged(() => this.CountVisitesAValider);

            this.IsBusy = false;
        }

        /// <summary>
        /// Le chargement des niveaux de protection vient d'être effectué.
        /// </summary>
        /// <param name="ex"></param>
        private void LoadMesNiveauProtectionDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(MesClassementMesure).Name));
            }

            IsBusy = false;
        }

        /// <summary>
        /// La recherche des alertes vient d'être effectuée.
        /// </summary>
        /// <param name="ex"></param>
        private void SearchDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Visite).Name));
            }
            else
            {
                foreach (Visite visite in Entities)
                {
                    visite.LoadVisiteMesures(this.ServiceMesClassementMesure.Entities, null, false, true);
                }

                this.RefreshList();
            }

            IsBusy = false;
        }

        protected override void Save()
        {
            //Recherche des visites passées à validée dans l'écran et création des alertes sur seuils si des mesures sont en alerte
            IEnumerable<Visite> visites = this.Entities.Where(v => v.EstValidee);
            foreach (Visite v in visites)
            {
                v.CleUtilisateurValidation = this.CurrentUser.CleUtilisateur;
                v.DateValidation = DateTime.Now;
                IEnumerable<MesMesure> mesures = v.MesMesure.Where(m => m.IsDepassementSeuil && m.Alerte == null);
                foreach (MesMesure mes in mesures)
                {
                    Alerte alerte = new Alerte()
                    {
                        Supprime = false,
                        Date = v.DateVisite != null ? (DateTime)v.DateVisite : DateTime.Now,
                        CleVisite = v.CleVisite,
                        RefEnumValeur = ServiceRefEnumValeur.Entities.FirstOrDefault(en => en.CodeGroupe == RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue() && en.Valeur == "S")
                    };

                    mes.Alertes.Add(alerte);
                }
                mesures = v.MesMesure.Where(m => !m.IsDepassementSeuil && m.Alerte != null);
                foreach (MesMesure mes in mesures)
                {
                    ServiceAlerte.Delete(mes.Alerte);
                }
            }

            visites = this.Entities.Where(v => !v.EstValidee);
            foreach (Visite v in visites)
            {
                int counter = v.MesMesure.Count();
                foreach (MesMesure m in v.MesMesure)
                {
                    m.RejectChanges();
                }
                v.RejectChanges();
            }

            base.Save();
        }

        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        protected virtual void DeleteLine(object Obj)
        {
            var result = MessageBox.Show(Resource.ValidationVisite_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK && Obj is Visite && this.Entities.Any(v => v.CleVisite == (Obj as Visite).CleVisite))
            {
                Visite visiteToDelete = this.Entities.FirstOrDefault(v => v.CleVisite == (Obj as Visite).CleVisite);

                //crée le rejet qui correspond à la visite en cours de suppression
                this.AddToListRejet(new Rejet()
                {
                    CleVisite = visiteToDelete.CleVisite,

                    LibellePortion = visiteToDelete.LibellePortion,
                    CodeEquipement = visiteToDelete.Ouvrage.CodeEquipement,
                    LibelleOuvrage = visiteToDelete.LibelleOuvrage,
                    DateVisite = visiteToDelete.DateVisite,
                    TypeEval = visiteToDelete.RefEnumValeur.LibelleCourt,

                    VisiteContent = visiteToDelete.VisiteSerialized
                });

                this.service.Delete(visiteToDelete);

                RaisePropertyChanged(() => this.CountVisitesAValider);
            }
        }

        /// <summary>
        /// Fonction de check des lignes par groupe
        /// </summary>
        /// <param name="Obj">Object CheckBox du groupe</param>
        protected virtual void CheckGroup(object Obj)
        {
            //On vérifie que l'object est bien une checkbox et qu'il contient bien son datacontext
            if (Obj != null && Obj is CheckBox && (Obj as CheckBox).DataContext != null)
            {
                //Récupération du GroupViewModel contenant la checkbox
                GroupViewModel gvm = (Obj as CheckBox).DataContext as GroupViewModel;
                //Récupération de l'état dans lequel doit être la checkbox
                Nullable<Boolean> estValidee = (Nullable<Boolean>)gvm.AggregateResults[1].Value;
                //Si la checkbox est cochée ou non on prends l'inverse de son état sinon on la met à cochée
                estValidee = estValidee.HasValue ? !estValidee.Value : true;

                //Récupération de la premièreVisite pour selectionner un élément plus tard
                // + Set des items contenus dans le groupe à la nouvelle valeur
                Visite tmpVisite = null;
                foreach (Visite item in gvm.Group.Items)
                {
                    if (tmpVisite == null)
                    {
                        tmpVisite = item;
                    }
                    item.EstValidee = estValidee.Value && item.CanEditGeo;
                }

                //Reset du binding après avoir setté la valeur de la checkbox 
                Binding binding = new Binding("AggregateResults[1].Value");
                (Obj as CheckBox).SetBinding(CheckBox.IsCheckedProperty, binding);

                //Mise à jour des aggregatesResults pour le tableau
                RadGridView grid = (Obj as CheckBox).ParentOfType<RadGridView>();
                if (grid != null && tmpVisite != null)
                {
                    grid.Columns["Valider"].IsReadOnly = false;
                    grid.Items.EditItem(tmpVisite);
                    grid.Items.CommitEdit();
                    grid.CancelEdit();
                    grid.Columns["Valider"].IsReadOnly = true;
                }
            }
        }

        /// <summary>
        /// Fonction de check des lignes pour toutes les entitées
        /// </summary>
        /// <param name="Obj"></param>
        protected virtual void CheckColumn(object Obj)
        {
            //On vérifie que l'objet est bien une checkbox
            if (Obj != null && Obj is CheckBox)
            {
                //On vérifie l'état dans lequel doit se trouver le bouton et on calcule comment il doit modifier les items
                Boolean estValidee = Entities.Any(v => !v.EstValidee && v.CanEditGeo);

                //Récupération d'une visite par groupe dans une liste
                // + mise à jour de la propriété EstValidee
                List<Visite> tmpVisites = new List<Visite>();
                foreach (Visite item in Entities.Where(v => v.CanEditGeo))
                {
                    //Séléction d'une Visite par libelle portion
                    if (!tmpVisites.Any(v => v.CanEditGeo && v.LibellePortion == item.LibellePortion))
                    {
                        tmpVisites.Add(item);
                    }
                    item.EstValidee = estValidee && item.CanEditGeo;
                }

                //Reset du binding après avoir setté la valeur de la checkbox 
                Binding binding = new Binding("AggregateResults[0].Value");
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
                binding.RelativeSource.AncestorType = typeof(Proteca.Silverlight.Views.UserContols.CustomGridView);
                (Obj as CheckBox).SetBinding(CheckBox.IsCheckedProperty, binding);

                //Mise à jour des aggregatesResults pour le tableau
                RadGridView grid = (Obj as CheckBox).ParentOfType<RadGridView>();
                if (grid != null && tmpVisites.Count > 0)
                {
                    grid.Columns["Valider"].IsReadOnly = false;
                    //Mise à jour des aggregatesResults de chaque groupe
                    foreach (Visite item in tmpVisites)
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
        /// Rafraichissement de la liste (tout est raffraichit)
        /// </summary>
        private void RefreshList()
        {
            this.CheckCanEditByGeo();

            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => this.EntitiesCount);
            RaisePropertyChanged(() => this.ResultIndicator);
            RaisePropertyChanged(() => this.CountVisitesAValider);

            EditCommand.RaiseCanExecuteChanged();
        }

        #endregion Private Methods

        #region Autorisations

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'ajout d'une visite
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanAdd()
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.IMPORT_VALIDATION);
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
        /// sur la suppression d'une visite
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanDelete()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'édition d'une visite
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanEdit()
        {
            return GetAutorisation() && this.Entities.Any(v => v.CanEditGeo);
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur la sauvegarde d'une visite
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
                string codePortee = role.RefUsrPortee.CodePortee;

                if (codePortee != RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parcours les visites pour setter les canEditGeo en fonction des droits de l'utilisateur
        /// </summary>
        private void CheckCanEditByGeo()
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.IMPORT_VALIDATION);

                RefUsrPortee.ListPorteesEnum niveau = role.RefUsrPortee.GetPorteesEnum();

                foreach (Visite v in this.Entities)
                {
                    switch (niveau)
                    {
                        case RefUsrPortee.ListPorteesEnum.National:
                            v.CanEditGeo = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Region:
                            v.CanEditGeo = this.CurrentUser.GeoAgence != null && this.CurrentUser.GeoAgence.CleRegion == v.Ouvrage.PpAttachee.GeoSecteur.GeoAgence.CleRegion;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            v.CanEditGeo = this.CurrentUser.CleAgence.HasValue && this.CurrentUser.CleAgence.Value == v.Ouvrage.PpAttachee.GeoSecteur.CleAgence;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                            v.CanEditGeo = this.CurrentUser.CleSecteur.HasValue && this.CurrentUser.CleSecteur.Value == v.Ouvrage.PpAttachee.CleSecteur;
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
