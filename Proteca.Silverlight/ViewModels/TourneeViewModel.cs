using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.ServiceModel.DomainServices.Client;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using Ionic.Zip;
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
using Telerik.Windows.Zip;
using Proteca.Web.Resources;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Tournee entity
    /// </summary>
    [ExportAsViewModel("Tournee")]
    public class TourneeViewModel : BaseProtecaEntityViewModel<Tournee>, IEventSink<String>
    {

        #region private members

        private string enumTYPEEVALUATION = RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue();

        #endregion

        #region Services

        /// <summary>
        /// Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;

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
        /// Service utilisé pour gérer les Pp
        /// </summary>
        [Import]
        public IEntityService<Pp> ServicePp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les Equipements
        /// </summary>
        [Import]
        public IEntityService<EqEquipement> ServiceEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> serviceRefEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les types d'équipement
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> serviceTypeEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les coûts de mesure
        /// </summary>
        [Import]
        public IEntityService<MesCoutMesure> servicecoutMesure { get; set; }

        /// <summary>
        /// Service utilisé pour gérer la liste des Catégorie
        /// </summary>
        [Import]
        public IEntityService<CategoriePp> ServiceCategoriePp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer la liste des Sensibilité de PP
        /// </summary>
        [Import]
        public IEntityService<RefNiveauSensibilitePp> ServiceNiveauSensibilitePp { get; set; }

        #endregion Services

        #region Commands

        /// <summary>
        /// Commande pour exporter une tournée
        /// </summary>
        public IActionCommand ExportTourneeCommand { get; protected set; }

        /// <summary>
        /// Commande pour trouver des equipement
        /// </summary>
        public IActionCommand FindEquipementCommand { get; protected set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DeleteLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand UpOrderLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DownOrderLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand ReOrderCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command d'impression
        /// </summary>
        public IActionCommand PrintTourneeCommand { get; private set; }

        #endregion

        #region Events

        #region RégionEqui/AgenceEqui/SecteurEqui

        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnRegionEquiSelected;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnAgenceEquiSelected;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnSecteurEquiSelected;
        /// <summary>
        /// 
        /// </summary>
        //public EventHandler OnEnsElecEquiSelected;

        #endregion
        #endregion

        #region Constructor
        public TourneeViewModel()
            : base()
        {
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
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Recherche des tournées"));
                    EventAggregator.Publish("Tournee_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };

            this.OnDetailLoaded += (o, e) =>
            {
                if (this.Entities != null && this.SelectedEntity != null)
                {
                    InitGeoEquiAutorisation();

                    if (this.ListTypeEvaluation.Any())
                    {
                        this.ListTypeEvaluation.FirstOrDefault(eval => eval.LibelleCourt == "EG").IsSelected = true;
                    }

                    this.CompoBy();
                    SelectedEntity.OnCompositionPortionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CompositionPortions_CollectionChanged);
                    SelectedEntity.OnCompositionPortionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CompositionPortions_CollectionChanged);

                    SelectedEntity.OnCompositionEqChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CompositionEqs_CollectionChanged);
                    SelectedEntity.OnCompositionEqChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CompositionEqs_CollectionChanged);
                }

                ExportTourneeCommand.RaiseCanExecuteChanged();
                PrintTourneeCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => this.IsDeleteEnable);
                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.IsInfoAffiche);
                RaisePropertyChanged(() => this.IsEntityAdmin);
                RaisePropertyChanged(() => this.Cout);
                RaisePropertyChanged(() => this.NbHeures);
                RaisePropertyChanged(() => this.IsTypeEditable);
                RaisePropertyChanged(() => this.LibelleEE);
            };

            this.OnAddedEntity += (o, e) =>
            {
                InitGeoEquiAutorisation();

                if (this.Entities != null && this.SelectedEntity != null)
                {
                    SelectedEntity.OnCompositionPortionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CompositionPortions_CollectionChanged);
                    SelectedEntity.OnCompositionPortionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CompositionPortions_CollectionChanged);

                    SelectedEntity.OnCompositionEqChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CompositionEqs_CollectionChanged);
                    SelectedEntity.OnCompositionEqChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CompositionEqs_CollectionChanged);
                }

                if (this.ListTypeEvaluation.Any())
                {
                    this.ListTypeEvaluation.FirstOrDefault(eval => eval.LibelleCourt == "EG").IsSelected = true;
                }
                RaisePropertyChanged(() => this.IsEntityAdmin);
                RaisePropertyChanged(() => this.IsTypeEditable);
            };

            // Define commands
            ExportTourneeCommand = new ActionCommand<object>(obj => ShowDialog(), obj => CanExport);
            FindEquipementCommand = new ActionCommand<object>(obj => FindEquipement());
            DeleteLineCommand = new ActionCommand<object>(obj => DeleteLine(obj), obj => true);
            UpOrderLineCommand = new ActionCommand<object>(obj => UpLineOrder(), obj => IsEditMode);
            DownOrderLineCommand = new ActionCommand<object>(obj => DownLineOrder(), obj => IsEditMode);
            ReOrderCommand = new ActionCommand<object>(obj => Reorder(), obj => IsEditMode);
            PrintTourneeCommand = new ActionCommand<object>(obj => PrintTournee(obj), obj => CanExport);

            this.OnAllServicesLoaded += (o, e) =>
            {
                // MAJ des services
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectriqueEqui);
                RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);
                RaisePropertyChanged(() => this.ListTypeEvaluation);
                RaisePropertyChanged(() => this.IsTypeEditable);

                InitGeoEquiAutorisation();
                if (this.ListTypeEvaluation.Any())
                {
                    this.ListTypeEvaluation.FirstOrDefault(eval => eval.LibelleCourt == "EG").IsSelected = true;
                }
            };

            this.OnCanceled += (o, e) =>
            {
                // Suppression de la liste des équipements
                this.Equipements = new ObservableCollection<IOuvrage>();

                //Suppression de la liste  des portions 

                InitGeoEquiAutorisation();

                RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);

                // MAJ des cases à cocher
                this.EqSo = false;
                RaisePropertyChanged(() => this.EqSo);
                this.EqDr = false;
                RaisePropertyChanged(() => this.EqDr);
                this.EqLi = false;
                RaisePropertyChanged(() => this.EqLi);
                this.EqLe = false;
                RaisePropertyChanged(() => this.EqLe);
                this.EqTc = false;
                RaisePropertyChanged(() => this.EqTc);
                this.EqFm = false;
                RaisePropertyChanged(() => this.EqFm);
                this.EqPo = false;
                RaisePropertyChanged(() => this.EqPo);
                this.EqAg = false;
                RaisePropertyChanged(() => this.EqAg);
                this.EqDe = false;
                RaisePropertyChanged(() => this.EqDe);
                this.EqRi = false;
                RaisePropertyChanged(() => this.EqRi);
                this.EqPi = false;
                RaisePropertyChanged(() => this.EqPi);
                this.EqPp = false;
                RaisePropertyChanged(() => this.EqPp);
                RaisePropertyChanged(() => this.IsTypeEditable);

                if (this.SelectedEntity != null)
                {
                    this.SelectedEntity.ResetCompositionEqsByOrder();
                }
                RaisePropertyChanged(() => this.SelectedEntity);

                ExportTourneeCommand.RaiseCanExecuteChanged();
                PrintTourneeCommand.RaiseCanExecuteChanged();
            };

            this.OnSaveSuccess += (o, e) =>
            {
                // MAJ des cases à cocher
                this.EqSo = false;
                RaisePropertyChanged(() => this.EqSo);
                this.EqDr = false;
                RaisePropertyChanged(() => this.EqDr);
                this.EqLi = false;
                RaisePropertyChanged(() => this.EqLi);
                this.EqLe = false;
                RaisePropertyChanged(() => this.EqLe);
                this.EqTc = false;
                RaisePropertyChanged(() => this.EqTc);
                this.EqFm = false;
                RaisePropertyChanged(() => this.EqFm);
                this.EqPo = false;
                RaisePropertyChanged(() => this.EqPo);
                this.EqAg = false;
                RaisePropertyChanged(() => this.EqAg);
                this.EqDe = false;
                RaisePropertyChanged(() => this.EqDe);
                this.EqRi = false;
                RaisePropertyChanged(() => this.EqRi);
                this.EqPi = false;
                RaisePropertyChanged(() => this.EqPi);
                this.EqPp = false;
                RaisePropertyChanged(() => this.EqPp);

                // MAJ du verrouillage
                RaisePropertyChanged(() => this.IsVerrouilleEnable);
                RaisePropertyChanged(() => this.IsDeleteEnable);
                RaisePropertyChanged(() => this.IsEntityAdmin);
                RaisePropertyChanged(() => this.Cout);
                this.RaisePropertyChanged(() => this.NbHeures);
                RaisePropertyChanged(() => this.IsTypeEditable);

                ExportTourneeCommand.RaiseCanExecuteChanged();
                PrintTourneeCommand.RaiseCanExecuteChanged();
            };

            this.OnViewModeChanged += (o, e) =>
            {
                RaisePropertyChanged(() => this.CanDeleteEq);
                RaisePropertyChanged(() => this.IsVerrouilleEnable);
                RaisePropertyChanged(() => this.CanVerrouilleTournee);
                RaisePropertyChanged(() => this.IsDeleteEnable);
                RaisePropertyChanged(() => this.IsInfoAffiche);

                this.Equipements = null;
                RaisePropertyChanged(() => this.Equipements);
                RaisePropertyChanged(() => this.IsEntityAdmin);

                UpOrderLineCommand.RaiseCanExecuteChanged();
                DownOrderLineCommand.RaiseCanExecuteChanged();
                ReOrderCommand.RaiseCanExecuteChanged();

                ExportTourneeCommand.RaiseCanExecuteChanged();
                PrintTourneeCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => this.IsTypeEditable);

                this.RefreshGeoEquiAutorisation();
            };

            this.OnDeleteSuccess += (o, e) =>
            {
                UpOrderLineCommand.RaiseCanExecuteChanged();
                DownOrderLineCommand.RaiseCanExecuteChanged();
                ReOrderCommand.RaiseCanExecuteChanged();

                ExportTourneeCommand.RaiseCanExecuteChanged();
                PrintTourneeCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => this.IsTypeEditable);
            };

            this.OnImportsSatisfiedEvent += (o, e) =>
            {
                EventAggregator.Subscribe<String>(this);
            };

            this.ByEqSelected = true;
        }

        private void CompositionPortions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                nbGetPortionDone = 0;

                foreach (var item in e.NewItems)
                {
                    if (item is Composition && (item as Composition).ClePortion.HasValue)
                    {
                        nbGetPortionDone++;
                        this.IsBusy = true;

                        ((TourneeService)service).GetTourneePortionIntegriteByCle((item as Composition).ClePortion.Value, getPortionDone);
                    }
                }
            }
            RaisePropertyChanged(() => this.IsTypeEditable);
        }

        private void CompositionEqs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<Composition>)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    RefEnumValeur monEval = this.ListTypeEvaluation.Where(c => c.IsSelected == true).FirstOrDefault();

                    foreach (Composition item in e.NewItems)
                    {
                        item.RefEnumValeur = monEval;
                    }
                }

                // Mise en place de l'ordre
                foreach (Composition co in sender as ObservableCollection<Composition>)
                {
                    co.NumeroOrdre = (sender as ObservableCollection<Composition>).IndexOf(co);
                }
                RaisePropertyChanged(() => this.IsTypeEditable);
            }
        }

        /// <summary>
        /// Surcharge de la méthode save pour mettre a jour les compositions
        /// </summary>
        protected override void Save()
        {
            if (SelectedEntity != null)
            {
                // Si cr&ation de tournée
                if (this.SelectedEntity.IsNew())
                {
                    this.SelectedEntity.DateCreation = DateTime.Now;
                    this.SelectedEntity.CleUtilisateur = this.CurrentUser.CleUtilisateur;

                    //Nettoyage des compositions par portion
                    if (this.SelectedEntity.CompositionPortions.Any() && !this.ByPortionSelected)
                    {
                        foreach (Composition co in this.SelectedEntity.CompositionPortions)
                        {
                            this.SelectedEntity.Compositions.Remove(co);
                        }
                    }

                    //Nettoyage des compositions par EE
                    if (this.SelectedEntity.CompositionEEs.Any() && !this.ByEnsembleElecSelected)
                    {
                        foreach (Composition co in this.SelectedEntity.CompositionEEs)
                        {
                            this.SelectedEntity.Compositions.Remove(co);
                        }
                    }

                    //// Si Création par EE => ajout de la composition
                    //if (this.ByEnsembleElecSelected)
                    //{
                    //    this.SelectedEntity.Compositions.Add(new Composition() { CleEnsElectrique = this.FiltreCleEnsElecEqui });
                    //}
                }

                // MAJ des types d'évaluation
                if (this.ByEnsembleElecSelected || this.ByPortionSelected)
                {
                    foreach (Composition co in this.SelectedEntity.Compositions)
                    {
                        co.RefEnumValeur = ListTypeEvaluation.FirstOrDefault(c => c.IsSelected);
                    }
                }

                foreach (Composition co in this.SelectedEntity.CompositionEqs)
                {
                    co.NumeroOrdre = this.SelectedEntity.CompositionEqs.IndexOf(co);
                }
            }
            base.Save();
        }

        #endregion

        #region Public Properties

        public bool EqPp { get; set; }
        public bool EqDe { get; set; }
        public bool EqAg { get; set; }
        public bool EqDr { get; set; }
        public bool EqFm { get; set; }
        public bool EqLe { get; set; }
        public bool EqLi { get; set; }
        public bool EqPi { get; set; }
        public bool EqPo { get; set; }
        public bool EqRi { get; set; }
        public bool EqSo { get; set; }
        public bool EqTc { get; set; }

        public bool SearchEqDone { get; set; }
        public bool SearchPpDone { get; set; }

        public int nbGetPortionDone { get; set; }
        public string Erreur { get; set; }

        /// <summary>
        /// Retourne si oui ou non on peut modifier le type de la tournée
        /// </summary>
        public bool IsTypeEditable
        {
            get
            {
                return this.SelectedEntity != null && (this.SelectedEntity.IsNew() || !this.SelectedEntity.Compositions.Any());
            }
        }

        /// <summary>
        /// Retourne le cout de la tournee
        /// </summary>
        public string Cout
        {
            get
            {
                var moncout = GetCoutMesures().Select(c => c.Cout.GetValueOrDefault(0)).DefaultIfEmpty(0).Sum();
                return (moncout == 0 ? " -" : moncout.ToString()) + " €";
            }
        }

        /// <summary>
        /// Retourne le nombre d'heure de la tournée
        /// </summary>
        public string NbHeures
        {
            get
            {
                var mesheures = GetCoutMesures().Select(c => c.Temps.GetValueOrDefault(0)).DefaultIfEmpty(0).Sum();
                return (mesheures == 0 ? " -" : mesheures.ToString()) + " Heure(s)";
            }
        }

        private IEnumerable<MesCoutMesure> GetCoutMesures()
        {
            if (this.SelectedEntity == null)
                return Enumerable.Empty<MesCoutMesure>();

            var ppCoutMesures = this.SelectedEntity.Compositions
                .Where(co => co.ClePp.HasValue && co.Pp.CleNiveauSensibilite != 4)
                .Join(servicecoutMesure.Entities,
                    c => Tuple.Create("PP", c.Pp.EnumPolarisation, c.Pp.EnumDureeEnrg, c.EnumTypeEval),
                    cm => Tuple.Create(cm.TypeEquipement.CodeEquipement, cm.EnumTempsPolarisation, cm.EnumDureeEnregistrement, cm.EnumTypeEval),
                    (c, cm) => cm);

            var eqCoutMesures = this.SelectedEntity.Compositions
                .Where(co => (!co.ClePp.HasValue) && co.CleEquipement.HasValue)
                .Join(servicecoutMesure.Entities,
                    c => Tuple.Create(c.EqEquipement.CleTypeEq, c.EnumTypeEval),
                    cm => Tuple.Create(cm.CleTypeEq, cm.EnumTypeEval),
                    (c, cm) => cm);

            return ppCoutMesures.Concat(eqCoutMesures);

        }




        /// <summary>
        ///  Gère l'affichage du panel d'info
        /// </summary>
        public bool IsInfoAffiche
        {
            get
            {
                return this.SelectedEntity != null && !IsNewMode && !String.IsNullOrEmpty(this.SelectedEntity.InfosTournee);
            }
        }


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
                RaisePropertyChanged(() => this.GeoEnsemblesElectriqueEqui);
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
                RaisePropertyChanged(() => this.GeoEnsemblesElectriqueEqui);
            }
        }

        public String Libelle { get; set; }

        /// <summary>
        /// Retourne l'item sélectionné dans le tableau des équipements sélectionnés
        /// </summary>
        public Composition CompoSelected { get; set; }

        /// <summary>
        /// Déclaration de la variable incluant les portions supprimées
        /// </summary>
        public Boolean IsDelete { get; set; }

        /// <summary>
        /// Indique si l'entité peut être supprimée
        /// </summary>
        public Boolean IsDeleteEnable
        {
            get
            {
                if (SelectedEntity != null)
                {
                    if (SelectedEntity.Supprime)
                    {
                        return IsEditMode;
                    }
                    return !IsEditMode;
                }
                return false;
            }
        }

        /// <summary>
        /// Indique si l'utilisateur peut exporter ou non la tournee
        /// </summary>
        public bool CanExport
        {
            get { return this.SelectedEntity != null && this.SelectedEntity.Compositions.Any() && !IsEditMode; }
        }

        /// <summary>
        /// Indique sur le verrouillage est actif
        /// </summary>
        public bool IsVerrouilleEnable
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    return this.SelectedEntity.Verrouille || !IsEditMode;
                }
                return !IsEditMode;
            }
        }

        /// <summary>
        /// Autorise la suppression
        /// </summary>
        public Boolean CanDeleteEq
        {
            get { return IsEditMode && ByEqSelected; }
        }

        /// <summary>
        /// Retourne les liste des polarisations du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListTypeEvaluation
        {
            get
            {
                if (serviceRefEnumValeur != null)
                {
                    return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities.Where( r => r.CodeGroupe == enumTYPEEVALUATION 
                                                                                                        && (r.LibelleCourt == "EG" || r.LibelleCourt == "ECD" || r.LibelleCourt == "CF")
                                                                                                    ).OrderBy(r => r.NumeroOrdre));
                }
                return new ObservableCollection<RefEnumValeur>();
            }
        }

        private bool _byEqSelected;
        public bool ByEqSelected
        {
            get { return _byEqSelected; }
            set
            {
                if (!value && _byEqSelected && this.SelectedEntity.IsNew())
                {
                    CleanComposition();
                }
                _byEqSelected = value;
                RaisePropertyChanged(() => ByEqSelected);
                RaisePropertyChanged(() => this.CanDeleteEq);
            }
        }

        private bool _byPortionSelected;
        public bool ByPortionSelected
        {
            get { return _byPortionSelected; }
            set
            {
                if (!value && _byPortionSelected && this.SelectedEntity.IsNew())
                {
                    CleanComposition();
                }
                _byPortionSelected = value;
                RaisePropertyChanged(() => ByPortionSelected);
            }
        }

        private bool _byEnsembleElecSelected;
        public bool ByEnsembleElecSelected
        {
            get { return _byEnsembleElecSelected; }
            set
            {
                if (!value && _byEnsembleElecSelected && this.SelectedEntity.IsNew())
                {
                    CleanComposition();
                }
                else if (value && !_byEnsembleElecSelected && this.SelectedEntity.IsNew())
                {
                    FiltreCleEnsElecEqui = null;
                }
                _byEnsembleElecSelected = value;
                RaisePropertyChanged(() => ByEnsembleElecSelected);
            }
        }

        /// <summary>
        /// Indique de quels types sont les compositions de la tournée selectionnée
        /// </summary>
        private void CompoBy()
        {
            Composition co = this.SelectedEntity.CompositionEEs.FirstOrDefault();
            if (co != null)
            {
                this.ByEnsembleElecSelected = true;
                this._filtreCleEnsElecEqui = co.CleEnsElectrique;

                this.FiltreClePortionEqui = null;
                RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);
                RaisePropertyChanged(() => this.FiltreCleEnsElecEqui);

                if (this.ListTypeEvaluation.Any())
                {
                    this.ListTypeEvaluation.FirstOrDefault(e => e.CleEnumValeur == co.EnumTypeEval).IsSelected = true;
                }
            }
            else
            {
                co = this.SelectedEntity.CompositionPortions.FirstOrDefault();
                if (co != null)
                {
                    if (this.ListTypeEvaluation.Any())
                    {
                        this.ListTypeEvaluation.FirstOrDefault(e => e.CleEnumValeur == co.EnumTypeEval).IsSelected = true;
                    }

                    this.ByPortionSelected = true;
                }
                else
                {
                    this.ByEqSelected = true;
                }

                this.FiltreCleEnsElecEqui = null;
            }
        }

        public string LibelleEE
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    Composition compEE = this.SelectedEntity.Compositions.FirstOrDefault(c => c.CleEnsElectrique != null);
                    if (compEE != null && compEE.EnsembleElectrique != null)
                    {
                        return compEE.EnsembleElectrique.Libelle;
                    }
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Supprime toute les compositions d'une tournée
        /// </summary>
        private void CleanComposition()
        {
            while (this.SelectedEntity.Compositions.Count > 0)
            {
                ((TourneeService)service).DeleteComposition(this.SelectedEntity.Compositions.FirstOrDefault());
                //this.SelectedEntity.Compositions.Remove(this.SelectedEntity.Compositions.FirstOrDefault());
            }
            RaisePropertyChanged(() => this.IsTypeEditable);

            this.SelectedEntity.ForceRaiseCompositionEqs();
            this.SelectedEntity.ForceRaiseCompositionPortions();
            this.SelectedEntity.ForceRaiseCompositionEEs();
        }

        #region Expander Sources

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

        #endregion

        #region Recherche d'équipements

        private ObservableCollection<IOuvrage> _equipements;
        public ObservableCollection<IOuvrage> Equipements
        {
            get
            {
                if (_equipements == null)
                {
                    _equipements = new ObservableCollection<IOuvrage>();
                }
                return new ObservableCollection<IOuvrage>(_equipements.Where(o => ((o is Pp) && !this.SelectedEntity.CompositionEqs.Select(c => c.ClePp).Contains((o as Pp).ClePp) && !(o as Pp).Supprime)
                    || ((o is EqEquipement) && !this.SelectedEntity.CompositionEqs.Select(c => c.CleEquipement).Contains((o as EqEquipement).CleEquipement) && !(o as EqEquipement).Supprime)));
            }
            set
            {
                _equipements = value;
                RaisePropertyChanged(() => this.Equipements);
            }
        }

        private int? _filtreCleRegionEqui;
        public int? FiltreCleRegionEqui
        {
            get
            {
                return _filtreCleRegionEqui;
            }
            set
            {
                _filtreCleRegionEqui = value;

                this.FiltreCleEnsElecEqui = null;
                this.FiltreClePortionEqui = null;
                RaisePropertyChanged(() => this.GeoEnsemblesElectriqueEqui);
                RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);

                RaisePropertyChanged(() => this.FiltreCleRegionEqui);
            }
        }

        private int? _filtreCleAgenceEqui;
        public int? FiltreCleAgenceEqui
        {
            get
            {
                return _filtreCleAgenceEqui;
            }
            set
            {
                _filtreCleAgenceEqui = value;

                this.FiltreCleEnsElecEqui = null;
                this.FiltreClePortionEqui = null;
                RaisePropertyChanged(() => this.GeoEnsemblesElectriqueEqui);
                RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);

                RaisePropertyChanged(() => this.FiltreCleAgenceEqui);
            }
        }

        private int? _filtreCleSecteurEqui;
        public int? FiltreCleSecteurEqui
        {
            get
            {
                return _filtreCleSecteurEqui;
            }
            set
            {
                _filtreCleSecteurEqui = value;

                this.FiltreCleEnsElecEqui = null;
                this.FiltreClePortionEqui = null;
                RaisePropertyChanged(() => this.GeoEnsemblesElectriqueEqui);
                RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);

                RaisePropertyChanged(() => this.FiltreCleSecteurEqui);
            }
        }

        private int? _filtreCleEnsElecEqui;
        public int? FiltreCleEnsElecEqui
        {
            get
            {
                return _filtreCleEnsElecEqui;
            }
            set
            {
                if (this.ByEnsembleElecSelected && value.HasValue && IsEditMode)
                {
                    var result = MessageBoxResult.OK;
                    if (_filtreCleEnsElecEqui.HasValue && value.Value != _filtreCleEnsElecEqui.Value)
                    {
                        result = MessageBox.Show(Resource.CompositionEE_ChangeConfirmation, "", MessageBoxButton.OKCancel);
                    }

                    if (result == MessageBoxResult.OK)
                    {
                        this.IsBusy = true;
                        CleanComposition();
                        ((TourneeService)service).GetTourneeEnsElecByCle(value.Value, getEnsElecDone);
                    }

                }

                _filtreCleEnsElecEqui = value;

                this.FiltreClePortionEqui = null;
                RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);
                RaisePropertyChanged(() => this.FiltreCleEnsElecEqui);
            }
        }

        private int? _filtreClePortionEqui;
        public int? FiltreClePortionEqui
        {
            get
            {
                return _filtreClePortionEqui;
            }
            set
            {
                _filtreClePortionEqui = value;

                RaisePropertyChanged(() => this.FiltreClePortionEqui);
            }
        }

        /// <summary>
        /// Retourne les GEO ensembles électrique pour le filtre par portion
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsemblesElectriqueEqui
        {
            get
            {
                List<GeoEnsElecPortion> ListEnsElec = new List<GeoEnsElecPortion>();

                if (FiltreCleSecteurEqui.HasValue)
                {
                    ListEnsElec = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteurEqui.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                    })).ToList();
                }
                else if (FiltreCleAgenceEqui.HasValue)
                {
                    ListEnsElec = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgenceEqui.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                    })).ToList();
                }
                else if (FiltreCleRegionEqui.HasValue)
                {
                    ListEnsElec = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegionEqui.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                    })).ToList();
                }
                else
                {
                    ListEnsElec = ServiceGeoEnsElecPortion.Entities.Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                    })).ToList();
                }

                if (IsStation || IsPosteGaz)
                {
                    ListEnsElec = ListEnsElec.Where(ee =>
                        (IsStation && ee.EnumStructureCplx.HasValue && ee.EnumStructureCplx.Value == 23)
                        || (IsPosteGaz && ee.EnumStructureCplx.HasValue && ee.EnumStructureCplx.Value == 24)).ToList();
                }

                return ListEnsElec;
            }
        }

        /// <summary>
        /// Retourne les GEO ensembles électrique / portions 
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsElecPortionsEqui
        {
            get
            {
                List<GeoEnsElecPortion> retour = new List<GeoEnsElecPortion>();

                if (FiltreCleEnsElecEqui.HasValue)
                {
                    if (FiltreCleSecteurEqui.HasValue)
                    {
                        retour = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElecEqui.Value && i.CleSecteur == FiltreCleSecteurEqui.Value && !i.PortionSupprime).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else if (FiltreCleAgenceEqui.HasValue)
                    {
                        retour = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElecEqui.Value && i.CleAgence == FiltreCleAgenceEqui.Value && !i.PortionSupprime).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else if (FiltreCleRegionEqui.HasValue)
                    {
                        retour = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElecEqui.Value && i.CleRegion == FiltreCleRegionEqui.Value && !i.PortionSupprime).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else
                    {
                        retour = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElecEqui.Value && !i.PortionSupprime).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                }
                else
                {
                    if (FiltreCleSecteurEqui.HasValue)
                    {
                        retour = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteurEqui.Value && !i.PortionSupprime).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else if (FiltreCleAgenceEqui.HasValue)
                    {
                        retour = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgenceEqui.Value && !i.PortionSupprime).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else if (FiltreCleRegionEqui.HasValue)
                    {
                        retour = ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegionEqui.Value && !i.PortionSupprime).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else
                    {
                        //Trop de données suppression du filtre
                    }
                }

                if (this.SelectedEntity != null)
                {
                    retour = retour.Where(p => p.NbPp > 0 && !this.SelectedEntity.Compositions.Select(c => c.ClePortion).Contains(p.ClePortion)).ToList();
                }

                return retour;
            }
        }

        #endregion

        #endregion Public Properties

        #region Override Methods

        protected override void DeactivateView(string viewName)
        {
            this.FiltreCleRegionEqui = null;

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
        /// Permet la suppression logique
        /// </summary>
        protected override void Delete()
        {
            this.SelectedEntity.Supprime = true;
            RaisePropertyChanged(() => this.SelectedEntity.InfosTournee);
            RaisePropertyChanged(() => this.IsInfoAffiche);
            Save(true);
        }

        /// <summary>
        /// Surcharge de la sauvegare permettant le log des tournées
        /// </summary>
        /// <param name="forceSave"></param>
        protected override void Save(bool forceSave)
        {
            IsBusy = true;
            if (IsNewMode)
            {
                LogTournee("C");
            }
            else
            {
                if (this.SelectedEntity.HasChanges || this.SelectedEntity.HasChildChanges())
                {
                    bool IsSupprime = false;
                    bool IsOrigineSupprime = false;

                    var type = this.SelectedEntity.GetType();

                    PropertyInfo prop = type.GetProperty("Supprime");
                    if (prop != null && !this.SelectedEntity.IsNew() && this.SelectedEntity.GetOriginal() != null)
                    {
                        IsSupprime = (bool)prop.GetValue(this.SelectedEntity, null);
                        IsOrigineSupprime = (bool)prop.GetValue(this.SelectedEntity.GetOriginal(), null);
                    }

                    if (!IsSupprime && IsOrigineSupprime)
                    {
                        LogTournee("R");
                    }
                    else if (IsSupprime && !IsOrigineSupprime)
                    {
                        LogTournee("S");
                    }
                    else
                    {
                        LogTournee("M");
                    }
                }
            }

            base.Save(forceSave);
        }

        /// <summary>
        /// Override de la fonction Delete
        /// </summary>
        protected override void Delete(bool skipNavigation, bool skipConfirmation)
        {
            LogTournee("S");
            base.Delete(skipNavigation, skipConfirmation);
        }

        #endregion

        #region Public Methods

        private void PrintTournee(object obj)
        {
            String rapportUrl = Rapports.printDocumentUrl;

            rapportUrl += String.Format(Rapports.RFTO_Composition_FileName_Extended, 0, 0, 0, 0, 0, 1, SelectedEntity.CleTournee);

            HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
        }

        /// <summary>
        /// Affichage de la popup de sélection du type d'export
        /// </summary>
        private void ShowDialog()
        {
            if ((this.service as TourneeService).ValidateTournee())
            {
                ChildWindow.Title = "Choisir le format d'export de la tournée";
                ChildWindow.Show();
                EventAggregator.Publish("ExportTournee".AsViewNavigationArgs());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Resource.Erreur_Donnees);

                if ((this.service as TourneeService).CustomErrors.Any())
                {
                    sb.AppendLine("");
                    sb.AppendLine("Liste des erreurs : ");
                    sb.AppendLine("");
                }
                foreach (String err in (this.service as TourneeService).CustomErrors)
                {
                    sb.AppendLine(err);
                }

                string Erreur = sb.ToString();

                if (!String.IsNullOrEmpty(Erreur))
                {
                    MessageBox.Show(sb.ToString(), "", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Récupération de la string du type d'export et lancement des évenements en fonction
        /// </summary>
        /// <param name="publishedEvent"></param>
        public void HandleEvent(String publishedEvent)
        {
            if (IsActive)
            {
                switch (publishedEvent)
                {
                    case "ProteIn":
                        this.ExportXmlTournee();
                        break;
                    case "ProtOn":
                        this.ExportJsonTournee();
                        break;
                    case "Excel":
                        String rapportUrl = Rapports.printDocumentUrl;

                        rapportUrl += String.Format(Rapports.RFVI_Visite_Tournee_FileName_Extended, 0, 0, 0, 0, 0, 0, SelectedEntity.CleTournee);

                        HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
                        break;
                    default:
                        break;
                }
                publishedEvent = String.Empty;
            }
        }

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        protected virtual void DeleteLine(object Obj)
        {
            if (Obj is Composition)
            {
                Composition co = Obj as Composition;

                var result = MessageBoxResult.Cancel;

                if (co.CleEquipement.HasValue)
                {
                    result = MessageBox.Show(Resource.CompositionEq_DeleteConfirmation, "", MessageBoxButton.OKCancel);
                }
                else if (co.ClePp.HasValue)
                {
                    result = MessageBox.Show(Resource.CompositionPp_DeleteConfirmation, "", MessageBoxButton.OKCancel);
                }
                else if (co.ClePortion.HasValue)
                {
                    result = MessageBox.Show(Resource.CompositionPI_DeleteConfirmation, "", MessageBoxButton.OKCancel);
                }

                if (result == MessageBoxResult.OK)
                {
                    if (co.ClePp.HasValue || co.CleEquipement.HasValue)
                    {
                        this.SelectedEntity.Compositions.Remove(co);
                        ((TourneeService)service).DeleteComposition(co);

                        this.SelectedEntity.ForceRaiseCompositionEqs();
                        RaisePropertyChanged(() => this.Equipements);
                    }
                    else if (co.ClePortion.HasValue)
                    {
                        // Suppression des compositions Equipement associées à la portion
                        IEnumerable<Composition> compoEqs = this.SelectedEntity.CompositionEqs.Where(c => (c.EqEquipement != null && c.EqEquipement.Pp.ClePortion == co.ClePortion.Value) || (c.Pp != null && c.Pp.ClePortion == co.ClePortion.Value));
                        foreach (Composition compo in compoEqs)
                        {
                            this.SelectedEntity.Compositions.Remove(compo);
                            ((TourneeService)service).DeleteComposition(compo);
                        }

                        // Suppression de la composition portion
                        this.SelectedEntity.Compositions.Remove(co);
                        ((TourneeService)service).DeleteComposition(co);

                        this.SelectedEntity.ForceRaiseCompositionEqs();
                        this.SelectedEntity.ForceRaiseCompositionPortions();
                        RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);
                    }
                    RaisePropertyChanged(() => this.IsTypeEditable);
                }
            }
        }

        /// <summary>
        /// Permet de réordonner toute les compositions sélectionnées
        /// </summary>
        private void Reorder()
        {
            ObservableCollection<Composition> CompoEqReordonner = null;

            // Initialisation de l'ordre
            CompoEqReordonner = new ObservableCollection<Composition>(SelectedEntity.CompositionEqs
                .OrderBy(c => c.LibelleEe)
                .ThenBy(d => d.LibellePortion)
                .ThenBy(e => decimal.Parse(e.Pk))
                .ThenBy(f => f.Type).ToList());

            // Modification des numéro d'ordres
            foreach (Composition MaCompo in CompoEqReordonner)
            {
                MaCompo.NumeroOrdre = CompoEqReordonner.IndexOf(MaCompo);
            }

            // Affectation de la nouvelle liste
            SelectedEntity.CompositionEqs = CompoEqReordonner;

            // MAJ de la vue
            RaisePropertyChanged(() => this.SelectedEntity);
        }

        /// <summary>
        /// Permet de diminuer le numéro d'ordre de l'item sélectionné du tableau des équipements sélectionné
        /// </summary>
        private void UpLineOrder()
        {
            if (CompoSelected != null && SelectedEntity.CompositionEqs.Count > 0)
            {
                // Récupération de l'index de l'item sélectionné
                int index = SelectedEntity.CompositionEqs.IndexOf(CompoSelected);

                Composition CompoBeforeMoving = CompoSelected;

                // Si l'item n'est pas le premier, on l'inverse avec l'item qui est avant
                if (index > 0)
                {
                    SelectedEntity.MovingItems = true;
                    Composition compoToMove = SelectedEntity.CompositionEqs.ElementAt(index - 1);
                    compoToMove.NumeroOrdre = index;
                    CompoBeforeMoving.NumeroOrdre = index - 1;

                    //SelectedEntity.CompositionEqs.RemoveAt(index - 1);
                    SelectedEntity.CompositionEqs.RemoveAt(index);
                    SelectedEntity.CompositionEqs.Insert(index - 1, CompoBeforeMoving);
                    //SelectedEntity.CompositionEqs.Insert(index - 1, CompoBeforeMoving);
                    SelectedEntity.MovingItems = false;

                }
                //SelectedEntity.ResetCompositionEqsByOrder();
                // MAJ de la vue
                //RaisePropertyChanged(() => this.SelectedEntity);
                CompoSelected = CompoBeforeMoving;
                RaisePropertyChanged(() => this.CompoSelected);
            }
        }

        /// <summary>
        /// Permet de modifier le numero d'ordre sélectionné
        /// </summary>
        private void DownLineOrder()
        {
            if (CompoSelected != null && SelectedEntity.CompositionEqs.Count > 0)
            {
                // Récupération de l'index de l'item sélectionné
                int index = SelectedEntity.CompositionEqs.IndexOf(CompoSelected);
                Composition CompoBeforeMoving = CompoSelected;

                // Si l'item n'est pas le dernier, on l'inverse avec l'item qui est avant
                if (index < SelectedEntity.CompositionEqs.Count - 1)
                {
                    SelectedEntity.MovingItems = true;
                    Composition compoToMove = SelectedEntity.CompositionEqs.ElementAt(index + 1);
                    compoToMove.NumeroOrdre = index;
                    CompoBeforeMoving.NumeroOrdre = index + 1;

                    //SelectedEntity.CompositionEqs.RemoveAt(index);
                    SelectedEntity.CompositionEqs.RemoveAt(index);
                    //SelectedEntity.CompositionEqs.Insert(index, CompoBeforeMoving);
                    SelectedEntity.CompositionEqs.Insert(index + 1, CompoBeforeMoving);
                    SelectedEntity.MovingItems = false;
                }
                //SelectedEntity.ResetCompositionEqsByOrder();
                // MAJ de la vue
                //RaisePropertyChanged(() => this.SelectedEntity);
                CompoSelected = CompoBeforeMoving;
                RaisePropertyChanged(() => this.CompoSelected);
            }
        }

        /// <summary>
        /// Ajout d'un document
        /// </summary>
        protected void ExportXmlTournee()
        {
            if (this.SelectedEntity != null)
            {
                IsBusy = true;

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = ".pro";
                dialog.Filter = string.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", "pro", "ProteIn");
                dialog.FilterIndex = 1;

                bool? dialogResult = dialog.ShowDialog();
                if (dialogResult == true)
                {
                    FileToDownload = dialog.OpenFile();
                    ((TourneeService)this.service).ExportTourneeToXml(this.SelectedEntity.CleTournee, ExportXmlDone);
                }
                else
                {
                    IsEditMode = false;
                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// Ajout d'un document
        /// </summary>
        protected void ExportJsonTournee()
        {
            if (this.SelectedEntity != null)
            {
                IsBusy = true;

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = ".pon";
                dialog.Filter = string.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", "pon", "ProtOn");
                dialog.FilterIndex = 1;

                bool? dialogResult = dialog.ShowDialog();
                if (dialogResult == true)
                {
                    FileToDownload = dialog.OpenFile();
                    ((TourneeService)this.service).ExportTourneeToJson(this.SelectedEntity.CleTournee, ExportJsonDone);
                }
                else
                {
                    IsEditMode = false;
                    IsBusy = false;
                }
            }
        }

        private Stream FileToDownload;

        /// <summary>
        /// Lancement de la recherche
        /// </summary>
        protected override void Find()
        {
            IsBusy = true;

            saveGeoPreferences();

            ((TourneeService)this.service).FindTourneeByCriterias(this.FiltreCleRegion, this.FiltreCleAgence, this.FiltreCleSecteur,
                this.FiltreCleEnsElec, this.FiltreClePortion, this.Libelle, this.IsDelete, SearchDone);
        }

        /// <summary>
        /// Lancement de la recherche
        /// </summary>
        protected void FindEquipement()
        {
            Equipements = new ObservableCollection<IOuvrage>();
            List<String> filtreEq = new List<String>();
            if (this.EqSo)
                filtreEq.Add("SO");
            if (this.EqDr)
                filtreEq.Add("DR");
            if (this.EqLi)
                filtreEq.Add("LI");
            if (this.EqLe)
                filtreEq.Add("LE");
            if (this.EqTc)
                filtreEq.Add("TC");
            if (this.EqFm)
                filtreEq.Add("FM");
            if (this.EqPo)
                filtreEq.Add("PO");
            if (this.EqAg)
                filtreEq.Add("AG");
            if (this.EqDe)
                filtreEq.Add("DE");
            if (this.EqRi)
                filtreEq.Add("RI");
            if (this.EqPi)
                filtreEq.Add("PI");

            if (filtreEq.Count > 0 || !this.EqPp)
            {
                IsBusy = true;
                SearchEqDone = false;

                ((EqEquipementService)this.ServiceEquipement).FindEquipementByCriterias(this.FiltreCleRegionEqui, this.FiltreCleAgenceEqui, this.FiltreCleSecteurEqui, this.FiltreCleEnsElecEqui, this.FiltreClePortionEqui, false, filtreEq, SearchEquipementDone);
            }
            else
            {
                SearchEqDone = true;
            }

            if (this.EqPp || filtreEq.Count == 0)
            {
                IsBusy = true;
                SearchPpDone = false;
                ((PpService)this.ServicePp).FindPpByCriterias(this.FiltreCleRegionEqui, this.FiltreCleAgenceEqui, this.FiltreCleSecteurEqui, this.FiltreCleEnsElecEqui, this.FiltreClePortionEqui, false, SearchPPDone);
            }
            else
            {
                SearchPpDone = true;
            }
        }

        /// <summary>
        /// L'export des tournees est terminée
        /// </summary>
        private void ExportXmlDone(Exception error, List<String> retour)
        {
            this.IsBusy = false;

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Tournee).Name));
            }
            else
            {
                // TODO : récupérer les autres fichiers du module déporté
                if (FileToDownload != null)
                {
                    using (ZipFile zip = new ZipFile(Encoding.UTF8))
                    //using (ZipPackage zipPackage = ZipPackage.Create(FileToDownload))
                    {
                        zip.Password = "Grt!Pr0t3c@";

                        // Fichier de donnée
                        zip.AddEntry("data.xml", retour[0]);

                        // Fichier de classement mesure
                        zip.AddEntry("classementMesure.xml", retour[1]);

                        // Fichier de donnée de référence
                        zip.AddEntry("dataRef.xml", retour[2]);

                        // Fichier de template vide
                        zip.AddEntry("emptyTemplate.xml", retour[3]);

                        zip.Save(FileToDownload);
                    }
                    FileToDownload.Close();
                }
            }
        }

        /// <summary>
        /// L'export des tournees est terminée
        /// </summary>
        private void ExportJsonDone(Exception error, String retour)
        {
            this.IsBusy = false;

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Tournee).Name));
            }
            else
            {
                // TODO : récupérer les autres fichiers du module déporté
                if (FileToDownload != null)
                {
                    using (ZipFile zip = new ZipFile(Encoding.UTF8))
                    //using (ZipPackage zipPackage = ZipPackage.Create(FileToDownload))
                    {
                        zip.Password = "Grt!Pr0t3c@";

                        // Fichier de donnée
                        zip.AddEntry("extractProteca.json", retour);

                        zip.Save(FileToDownload);
                    }
                    FileToDownload.Close();
                }
            }
        }

        /// <summary>
        /// La recherche des tournee est terminée
        /// </summary>
        private void SearchDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Tournee).Name));
            }
            else
            {
                RaisePropertyChanged(() => this.Entities);
                RaisePropertyChanged(() => EntitiesCount);
                RaisePropertyChanged(() => ResultIndicator);

                if (IsAutoNavigateToFirst && this.Entities != null && this.Entities.Any())
                {
                    int _cleTournee = (int)Entities.First().GetCustomIdentity();
                    if (this.SelectedEntity == null || this.SelectedEntity.CleTournee != _cleTournee)
                    {
                        NavigationService.Navigate(_cleTournee);
                    }
                }
                else if (this.Entities == null || !this.Entities.Any())
                {
                    this.SelectedEntity = null;

                    ExportTourneeCommand.RaiseCanExecuteChanged();
                    PrintTourneeCommand.RaiseCanExecuteChanged();

                    NavigationService.NavigateRootUrl();
                }
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// La recherche des equimenet est terminée
        /// </summary>
        private void SearchEquipementDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Tournee).Name));
            }
            else
            {
                SearchEqDone = true;
                if (SearchPpDone)
                {
                    this.Equipements = new ObservableCollection<IOuvrage>(this.Equipements.Union(this.ServiceEquipement.Entities.Where(eq => !eq.Supprime && !(SelectedEntity.Compositions.Where(c => c.CleEquipement.HasValue).Select(c => c.CleEquipement.Value).Contains(eq.CleEquipement)))));
                    IsBusy = false;
                }
                else
                {
                    this.Equipements = new ObservableCollection<IOuvrage>(this.ServiceEquipement.Entities.Where(eq => !eq.Supprime && !(SelectedEntity.Compositions.Where(c => c.CleEquipement.HasValue).Select(c => c.CleEquipement.Value).Contains(eq.CleEquipement))));
                }
            }

        }

        /// <summary>
        /// La recherche des PP est terminée
        /// </summary>
        private void SearchPPDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Tournee).Name));
            }
            else
            {
                SearchPpDone = true;
                if (SearchEqDone)
                {
                    this.Equipements = new ObservableCollection<IOuvrage>((this.ServicePp.Entities.Where(pp => !pp.Supprime && !(SelectedEntity.Compositions.Where(c => c.ClePp.HasValue).Select(c => c.ClePp.Value).Contains(pp.ClePp))).OrderBy(pp => pp.Libelle)).Union(this.Equipements));
                    IsBusy = false;
                }
                else
                {
                    this.Equipements = new ObservableCollection<IOuvrage>(this.ServicePp.Entities.Where(pp => !pp.Supprime && !(SelectedEntity.Compositions.Where(c => c.ClePp.HasValue).Select(c => c.ClePp.Value).Contains(pp.ClePp))).OrderBy(pp => pp.Libelle));
                }
            }

        }

        /// <summary>
        /// La recherche de la portion avec ses PPs et Equipements est terminée
        /// </summary>
        private void getPortionDone(Exception error, PortionIntegrite portion)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(PortionIntegrite).Name));
            }
            else if (portion != null)
            {
                RefEnumValeur monEval = this.ListTypeEvaluation.Where(c => c.IsSelected == true).FirstOrDefault();
                this.SelectedEntity.Compositions.Add(new Composition() { ClePortion = portion.ClePortion, NumeroOrdre = 0, RefEnumValeur = monEval });
                int numOrdre = this.SelectedEntity.CompositionEqs.Count;
                foreach (Pp pp in portion.Pps.Where(p => !p.Supprime).OrderBy(p => p.Pk))
                {
                    this.SelectedEntity.CompositionEqs.Add(new Composition() { ClePp = pp.ClePp, NumeroOrdre = numOrdre++, RefEnumValeur = monEval });
                    foreach (EqEquipement eq in pp.EqEquipement.Where(e => !e.Supprime).OrderBy(e => e.TypeEquipement.NumeroOrdre))
                    {
                        this.SelectedEntity.CompositionEqs.Add(new Composition() { CleEquipement = eq.CleEquipement, NumeroOrdre = numOrdre++, RefEnumValeur = monEval });
                    }
                }

                //this.SelectedEntity.ForceRaiseCompositionEqs();
                this.SelectedEntity.ForceRaiseCompositionPortions();

                RaisePropertyChanged(() => this.GeoEnsElecPortionsEqui);
            }

            nbGetPortionDone--;
            if (nbGetPortionDone == 0)
            {
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// La recherche de l'Ensemble électrique avec ses portions, ses PPs et Equipements est terminée
        /// </summary>
        private void getEnsElecDone(Exception error, EnsembleElectrique ensElec)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(EnsembleElectrique).Name));
            }
            else if (ensElec != null)
            {
                RefEnumValeur monEval = this.ListTypeEvaluation.Where(c => c.IsSelected == true).FirstOrDefault();
                this.SelectedEntity.Compositions.Add(new Composition() { CleEnsElectrique = ensElec.CleEnsElectrique, NumeroOrdre = 0, RefEnumValeur = monEval });
                int numOrdre = 0;
                foreach (PortionIntegrite portion in ensElec.PortionIntegrite.Where(pi => !pi.Supprime).OrderBy(pi => pi.Libelle))
                {
                    foreach (Pp pp in portion.Pps.Where(p => !p.Supprime).OrderBy(p => p.Pk))
                    {
                        this.SelectedEntity.Compositions.Add(new Composition() { ClePp = pp.ClePp, NumeroOrdre = numOrdre++, RefEnumValeur = monEval });
                        foreach (EqEquipement eq in pp.EqEquipement.Where(e => !e.Supprime).OrderBy(e => e.TypeEquipement.NumeroOrdre))
                        {
                            this.SelectedEntity.Compositions.Add(new Composition() { CleEquipement = eq.CleEquipement, NumeroOrdre = numOrdre++, RefEnumValeur = monEval });
                        }
                    }
                }

                this.SelectedEntity.ForceRaiseCompositionEqs();
                this.SelectedEntity.ForceRaiseCompositionEEs();
            }

            this.IsBusy = false;
        }

        #endregion

        #region LogTournee

        /// <summary>
        /// Service utilisé pour gérer les log de la tournées
        /// </summary>
        [Import]
        public IEntityService<LogTournee> serviceLogTournee { get; set; }


        /// <summary>
        /// Ajout d'un enregistrement dans logOuvrage
        /// </summary>
        public void LogTournee(string Operation)
        {
            // Instanciation des propriétés
            EntityCollection<LogTournee> LogTourneeList = null;
            LogTournee _logAajouter;

            // Instanciation du resource manager
            ResourceManager resourceManager = ResourceHisto.ResourceManager;

            // Détermination du type d'équipement
            LogTourneeList = this.SelectedEntity.LogTournee;

            // Suppression des logs existant
            if (LogTourneeList != null && LogTourneeList.Any(lo => lo.IsNew()))
            {
                foreach (LogTournee log in LogTourneeList.Where(lo => lo.IsNew()))
                {
                    LogTourneeList.Remove(log);
                    serviceLogTournee.Delete(log);
                }
                _logAajouter = null;
            }

            // Instanciation du log ouvrage
            _logAajouter = new LogTournee
            {
                CleUtilisateur = this.CurrentUser.CleUtilisateur,
                RefEnumValeur = serviceRefEnumValeur.Entities.Where(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_LOG_OUVRAGE.GetStringValue() && r.Valeur == Operation).FirstOrDefault(),
                DateHistorisation = DateTime.Now
            };

            // En cas de changement du sélected entity, on log l'enregistrement
            if (this.SelectedEntity.HasChanges || this.SelectedEntity.HasChildChanges || Operation != null)
            {
                if ((this.SelectedEntity.HasChanges || this.SelectedEntity.HasChildChanges) && !IsNewMode && Operation != "S")
                {
                    string Modifiedproperties = null;

                    Entity original = this.SelectedEntity.GetOriginal();
                    if (original == null)
                    {
                        original = this.SelectedEntity;
                    }
                    List<string> elements = new List<string>();

                    foreach (PropertyInfo p in this.SelectedEntity.GetType().GetProperties())
                    {
                        // Gestion des propriétés Nullable définies coté Silverlight
                        if (p.Name.EndsWith("Nullable"))
                        {
                            continue;
                        }

                        //récupération de la valeur à afficher. Si pas de valeurs on prend le nom de la propriété
                        string propertyName = resourceManager.GetString(p.Name) == null ? p.Name : resourceManager.GetString(p.Name);

                        if (String.IsNullOrEmpty(propertyName)
                            || p.Name == "BkpCompositionPortions"
                            || p.Name == "BkpCompositionEqs"
                            || p.Name == "CompositionEEs"
                            || p.Name == "CompositionEqs"
                            || p.Name == "CompositionPortions"
                            || p.Name == "LogTournee")
                        {
                            continue;
                        }

                        if (p.CanWrite && !(p.PropertyType.BaseType == typeof(Entity)))
                        {
                            Object originalValue = p.GetValue(original, null);
                            Object newValue = p.GetValue(this.SelectedEntity, null);
                            if ((originalValue == null && newValue == null) || (originalValue != null && originalValue.Equals(newValue)))
                            {
                                continue;
                            }
                            else
                            {
                                Modifiedproperties += Modifiedproperties == null ? propertyName : " / " + propertyName;
                            }
                        }
                    }

                    foreach (String propName in this.SelectedEntity.GetChildChangesPropertyNames())
                    {
                        //récupération de la valeur à afficher. Si pas de valeurs on prend le nom de la propriété
                        string childPropertyName = resourceManager.GetString(propName) == null ? propName : resourceManager.GetString(propName);
                        Modifiedproperties += Modifiedproperties == null ? childPropertyName : " / " + childPropertyName;
                    }

                    _logAajouter.ListeChamps = Modifiedproperties;
                }

                // On ajoute le log au contexte
                LogTourneeList.Add(_logAajouter);
            }
        }

        #endregion

        #region Autorisations

        /// <summary>
        /// Retourne si l'utilisateur courant est administrateur et si l'entité n'est vide
        /// </summary>
        public bool IsEntityAdmin
        {
            get
            {
                if (this.CurrentUser != null)
                {
                    return this.CurrentUser.IsAdministrateur && this.SelectedEntity != null && !IsEditMode;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Indique si l'utilisateur peut verrouiller une tournee
        /// </summary>
        public bool CanVerrouilleTournee
        {
            get
            {
                if (this.CurrentUser != null && this.SelectedEntity != null)
                {
                    UsrRole role = null;
                    role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.VERROU_TOURNEE);

                    if (role != null && role.RefUsrPortee != null)
                    {
                        string codePortee = role.RefUsrPortee.CodePortee;
                        if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue())
                        {
                            return IsEditMode;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'ajout d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanAdd()
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_TOURNEE_NIV);
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
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_TOURNEE_NIV);
                string codePortee = role.RefUsrPortee.CodePortee;

                bool hasNoCompo = !this.SelectedEntity.Compositions.Any();
                bool hasCompoInPortee = false;

                if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                {
                    hasCompoInPortee = this.SelectedEntity.CompositionEqs.Any(c => (c.ClePp.HasValue && c.Pp.GeoSecteur.CleAgence == CurrentUser.CleAgence) || c.CleEquipement.HasValue && c.EqEquipement.Pp.GeoSecteur.CleAgence == CurrentUser.CleAgence);
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                    codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                {
                    hasCompoInPortee = true;
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    hasCompoInPortee = false;
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                {
                    hasCompoInPortee = this.SelectedEntity.CompositionEqs.Any(c => (c.ClePp.HasValue && c.Pp.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion) || c.CleEquipement.HasValue && c.EqEquipement.Pp.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion);
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue() && CurrentUser.CleSecteur.HasValue)
                {
                    hasCompoInPortee = this.SelectedEntity.CompositionEqs.Any(c => (c.ClePp.HasValue && c.Pp.CleSecteur == CurrentUser.CleSecteur.Value) || c.CleEquipement.HasValue && c.EqEquipement.Pp.CleSecteur == CurrentUser.CleSecteur.Value);
                }
                return hasNoCompo || hasCompoInPortee;
            }
            return false;
        }

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private void InitGeoEquiAutorisation()
        {
            if (this.CurrentUser != null)
            {
                //Initialisation des filtres Equi
                if (this.SelectedEntity != null && this.SelectedEntity.Compositions.Any())
                {

                }
                else
                {
                    this.FiltreCleRegionEqui = this.CurrentUser.GeoAgence.CleRegion;
                    this.FiltreCleAgenceEqui = this.CurrentUser.CleAgence;
                    this.FiltreCleSecteurEqui = this.CurrentUser.CleSecteur;
                }

                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_TOURNEE_NIV);
                string codePortee = role.RefUsrPortee.CodePortee;

                if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                    codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                {
                    this.CanSelectRegion = true;
                    this.CanSelectAgence = true;
                    this.CanSelectSecteur = true;
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                {
                    this.CanSelectRegion = false;
                    this.CanSelectAgence = true;
                    this.CanSelectSecteur = true;
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                {
                    this.CanSelectRegion = false;
                    this.CanSelectAgence = false;
                    this.CanSelectSecteur = true;
                }
                else
                {
                    this.CanSelectRegion = false;
                    this.CanSelectAgence = false;
                    this.CanSelectSecteur = false;
                }
            }
        }

        private void RefreshGeoEquiAutorisation()
        {
            this.RaisePropertyChanged(() => this.CanSelectRegion);
            this.RaisePropertyChanged(() => this.CanSelectAgence);
            this.RaisePropertyChanged(() => this.CanSelectSecteur);
        }

        private bool _canSelectRegion;
        public bool CanSelectRegion
        {
            get { return _canSelectRegion && this.IsEditMode; }
            set
            {
                _canSelectRegion = value;
                RaisePropertyChanged(() => this.CanSelectRegion);
            }
        }

        private bool _canSelectAgence;
        public bool CanSelectAgence
        {
            get { return _canSelectAgence && this.IsEditMode; }
            set
            {
                _canSelectAgence = value;
                RaisePropertyChanged(() => this.CanSelectAgence);
            }
        }

        private bool _canSelectSecteur;
        public bool CanSelectSecteur
        {
            get { return _canSelectSecteur && this.IsEditMode; }
            set
            {
                _canSelectSecteur = value;
                RaisePropertyChanged(() => this.CanSelectSecteur);
            }
        }

        #endregion Autorisations
    }
}
