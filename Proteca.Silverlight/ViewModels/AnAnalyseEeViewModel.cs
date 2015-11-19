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
using Proteca.Silverlight.Views.Windows;
using System.Windows;
using System.Collections.Generic;
using Proteca.Silverlight.Helpers;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using System.Windows.Browser;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for AnAnalyseEe entity
    /// </summary>
    [ExportAsViewModel("AnAnalyseEe")]
    public class AnAnalyseEeViewModel : BaseProtecaEntityViewModel<AnAnalyseEe>, IEventSink<AnAction>
    {
        #region Services

        /// <summary>
        /// Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;

        /// <summary>
        /// Service utilisé pour gérer l'entité de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les GEO ensembles électriques
        /// </summary>
        [Import]
        public IEntityService<GeoEnsembleElectrique> serviceGeoEnsElec { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les alertes
        /// </summary>
        [Import]
        public IEntityService<Alerte> serviceAlerte { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> serviceRefEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les actions
        /// </summary>
        [Import]
        public IEntityService<AnAction> serviceAnAction { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les Visites
        /// </summary>
        [Import]
        public IEntityService<Visite> ServiceVisite { get; set; }

        #endregion

        #region Command

        /// <summary>
        /// Déclaration de l'objet de commande pour ajouter une action
        /// </summary>
        public IActionCommand AddActionCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de commande pour générer un rapport
        /// </summary>
        public IActionCommand GenerateReportCommand { get; private set; }
        
        /// <summary>
        /// Déclaration de l'objet de commande pour générer un rapport
        /// MANTIS 12775 FSI 24/06/2014 : Ajout d'un bouton pour générer le sous rapport Tableaux de mesure ECD
        /// </summary>
        public IActionCommand GenerateReportAnnexe5Tab8EcdCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de commande pour générer un rapport
        /// </summary>
        public IActionCommand GenerateReportAnnexe6Command { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de commande pour générer une analyse
        /// </summary>
        public IActionCommand AnalyseCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de commande de navigation vers une action
        /// </summary>
        public IActionCommand NavigateActionCommand { get; private set; }


        #endregion

        #region Override Properties

        /// <summary>
        /// Surcharge des entities pour rajout d'un filtre
        /// </summary>
        public override ObservableCollection<AnAnalyseEe> Entities
        {
            get
            {
                if (this.CleEnsElectrique.Value != 0)
                {
                    if (this.service.Entities != null && this.service.Entities.Count == 0)
                    {
                        return new ObservableCollection<AnAnalyseEe>();
                    }
                    else
                    {
                        return new ObservableCollection<AnAnalyseEe>(this.service.Entities.Where(a => a.CleEnsElectrique == CleEnsElectrique.Value).OrderByDescending(c => c.DateAnalyse));
                    }
                }
                else
                {
                    return new ObservableCollection<AnAnalyseEe>();
                }
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Déclaration de l'énum permettant d'afficher les types en base
        /// </summary>
        private string enumTypeAlerte = RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue();
        private string enumTypePC = RefEnumValeurCodeGroupeEnum.AN_ETAT_PC.GetStringValue();

        private int _cleEnsElectrique;
        private bool _isAlerteExpanded;
        private bool _isActionExpanded;
        private bool _isRapportExpanded;
        private bool _isAnalyseExpanded;
        private bool _isExpanderVisible;
        private bool _isFormulaireEnable;
        private bool _activeBtnAnalyser;

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
                if (_selectedAlerteDetail != null && _selectedAlerteDetail.Alerte != null && _selectedAlerteDetail.Alerte.CleVisite.HasValue && (_selectedAlerteDetail.Alerte.Visite == null || !_selectedAlerteDetail.Alerte.Visite.MesMesure.Any()))
                {
                    (this.ServiceVisite as VisiteService).GetVisiteByCleLight(_selectedAlerteDetail.Alerte.CleVisite.Value, SearchVisiteDone);
                }
                else
                {
                    RaisePropertyChanged(() => this.SelectedAlerteDetail);
                }
            }
        }

        /// <summary>
        /// Retourne si le bouton analyser est activé
        /// </summary>
        public bool ActiveBtnAnalyser
        {
            get
            {
                return _activeBtnAnalyser;
            }
            set
            {
                _activeBtnAnalyser = value;
                RaisePropertyChanged(() => this.ActiveBtnAnalyser);
            }
        }

        /// <summary>
        /// Booléen pour option de la génération de rapport
        /// </summary>
        public bool AnAnalyseEe_LibelleCarac { get; set; }

        /// <summary>
        /// Booléen pour option de la génération de rapport
        /// </summary>
        public bool AnAnalyseEe_LibelleBilan { get; set; }

        /// <summary>
        /// Booléen pour option de la génération de rapport
        /// </summary>
        public bool AnAnalyseEe_LibelleDiag { get; set; }

        /// <summary>
        /// Booléen pour option de la génération de rapport
        /// </summary>
        public bool AnAnalyseEe_LibelleCtrl { get; set; }

        /// <summary>
        /// Booléen pour option de la génération de rapport
        /// </summary>
        public bool AnAnalyseEe_LibelleMesure { get; set; }

        /// <summary>
        /// Booléen pour option de la génération de rapport
        /// </summary>
        public bool AnAnalyseEe_LibelleRecap { get; set; }

        /// <summary>
        /// Retourne les liste des polarisations du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListTypeAlerte
        {
            get
            {
                if (serviceRefEnumValeur != null)
                {
                    return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumTypeAlerte).OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        /// Retourne les liste des états PC du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListEtatPC
        {
            get
            {
                if (serviceRefEnumValeur != null)
                {
                    return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumTypePC).OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        /// Liste des alertes details lié àl'ensemble électrique
        /// </summary>
        public ObservableCollection<AlerteDetail> AlerteDetail
        {
            get
            {
                if (((AlerteService)serviceAlerte).DetailEntities != null)
                {
                    return new ObservableCollection<AlerteDetail>(((AlerteService)serviceAlerte).DetailEntities.OrderBy(c => c.Pk).ThenBy(d => d.LibelleType).ThenBy(d => d.Commentaire));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Liste des actions lié à l'analyse
        /// </summary>
        public ObservableCollection<AnAction> ActionDetail
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    return new ObservableCollection<AnAction>(this.SelectedEntity.AnAction.OrderBy(c => c.NumActionPc).ThenBy(d => d.RefEnumValeur.Libelle));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// [Expander de recherche] inclure les stations
        /// </summary>
        public bool IncludeStation { get; set; }

        /// <summary>
        /// [Expander de recherche] inclure les poste gaz
        /// </summary>
        public bool IncludePosteGaz { get; set; }

        /// <summary>
        /// [Expander de recherche] saisi de l'ensemble électrique
        /// </summary>
        public string EnsembleElectriqueTitle { get; set; }

        /// <summary>
        /// Défini l'ensemble électrique sélectionné
        /// </summary>
        public int? CleEnsElectrique
        {
            get
            {
                return _cleEnsElectrique;
            }
            set
            {
                if (value.HasValue)
                {
                    _cleEnsElectrique = value.Value;
                }
                else
                {
                    _cleEnsElectrique = 0;
                }

                // MAJ de la vue
                RaisePropertyChanged(() => this.CleEnsElectrique);
                RaisePropertyChanged(() => this.Entities);
                RaisePropertyChanged(() => EntitiesCount);
                RaisePropertyChanged(() => ResultIndicator);

                // Navigation vers le premier élément
                if (this.Entities.Count > 0)
                {
                    if (this.SelectedEntity != null)
                    {
                        if (this.CleEnsElectrique.Value != this.SelectedEntity.CleEnsElectrique)
                        {
                            SelectedId = this.Entities.Select(a => a.CleAnalyse).FirstOrDefault();
                        }
                    }
                    else
                    {
                        SelectedId = this.Entities.Select(a => a.CleAnalyse).FirstOrDefault();
                    }
                }
                
            }
        }

        /// <summary>
        /// Retourne les régions du service région
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get
            {
                return new ObservableCollection<GeoRegion>(serviceRegion.Entities.OrderBy(r => r.LibelleRegion));
            }
        }

        /// <summary>
        /// Retourne les ensembles électriques du service ensemble électrique
        /// </summary>
        public ObservableCollection<GeoEnsembleElectrique> EnsElectriques
        {
            get
            {
                if (GetAutorisation(RefUsrAutorisation.ListAutorisationsEnum.ANALYSE_NIV))
                {
                    switch ((this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.ANALYSE_NIV)).RefUsrPortee.GetPorteesEnum())
                    {
                        case RefUsrPortee.ListPorteesEnum.National:
                            return new ObservableCollection<GeoEnsembleElectrique>(serviceGeoEnsElec.Entities.Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                {
                                    return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                })).ToList());
                        case RefUsrPortee.ListPorteesEnum.Region:
                            return new ObservableCollection<GeoEnsembleElectrique>(serviceGeoEnsElec.Entities.Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                            {
                                return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                            })).Where(c => c.CleRegion == this.userService.CurrentUser.GeoAgence.CleRegion).ToList());
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            return new ObservableCollection<GeoEnsembleElectrique>(serviceGeoEnsElec.Entities.Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                {
                                    return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                })).Where(c => c.CleAgence == this.userService.CurrentUser.CleAgence).ToList());
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                            return new ObservableCollection<GeoEnsembleElectrique>(serviceGeoEnsElec.Entities.Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                                {
                                    return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                                })).Where(c => c.CleSecteur == this.userService.CurrentUser.CleSecteur).ToList());
                        default:
                            return new ObservableCollection<GeoEnsembleElectrique>();
                    }
                }
                else
                {
                    return new ObservableCollection<GeoEnsembleElectrique>();
                }
            }
        }

        /// <summary>
        /// Liste des entitées 
        /// </summary>
        public virtual List<AnAnalyseEe> EnsElecDistinct
        {
            get
            {
                return this.service.Entities.Distinct(new InlineEqualityComparer<AnAnalyseEe>((a, b) =>
                {
                    return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.EnsembleElectrique.Libelle.Equals(b.EnsembleElectrique.Libelle);
                })).OrderBy(pi => pi.EnsembleElectrique.Libelle).ToList();
            }
        }

        /// <summary>
        /// Retourne si l'expander Alerte est développé
        /// </summary>
        public bool IsAlerteExpanded
        {
            get
            {
                return _isAlerteExpanded;
            }
            set
            {
                _isAlerteExpanded = value;
                if (_isAlerteExpanded == true)
                {
                    IsActionExpanded = false;
                    IsRapportExpanded = false;
                    IsAnalyseExpanded = false;
                }

                RaisePropertyChanged(() => this.IsActionExpanded);
                RaisePropertyChanged(() => this.IsRapportExpanded);
                RaisePropertyChanged(() => this.IsAlerteExpanded);
                RaisePropertyChanged(() => this.IsAnalyseExpanded);
            }
        }

        /// <summary>
        /// Retourne si l'expander Action est développé
        /// </summary>
        public bool IsActionExpanded
        {
            get
            {
                return _isActionExpanded;
            }
            set
            {
                _isActionExpanded = value;
                if (_isActionExpanded == true)
                {
                    IsAlerteExpanded = false;
                    IsRapportExpanded = false;
                    IsAnalyseExpanded = false;
                }

                RaisePropertyChanged(() => this.IsActionExpanded);
                RaisePropertyChanged(() => this.IsRapportExpanded);
                RaisePropertyChanged(() => this.IsAlerteExpanded);
                RaisePropertyChanged(() => this.IsAnalyseExpanded);
            }
        }

        /// <summary>
        /// Retourne si l'expander Rapport est développé
        /// </summary>
        public bool IsRapportExpanded
        {
            get
            {
                return _isRapportExpanded;
            }
            set
            {
                _isRapportExpanded = value;
                if (_isRapportExpanded == true)
                {
                    IsActionExpanded = false;
                    IsAlerteExpanded = false;
                    IsAnalyseExpanded = false;
                }

                RaisePropertyChanged(() => this.IsActionExpanded);
                RaisePropertyChanged(() => this.IsRapportExpanded);
                RaisePropertyChanged(() => this.IsAlerteExpanded);
                RaisePropertyChanged(() => this.IsAnalyseExpanded);
            }
        }

        /// <summary>
        /// Retourne si les analyses sont visibles
        /// </summary>
        public bool IsAnalyseExpanded
        {
            get
            {
                return _isAnalyseExpanded;
            }
            set
            {
                _isAnalyseExpanded = value;
                if (_isAnalyseExpanded == true)
                {
                    IsActionExpanded = false;
                    IsAlerteExpanded = false;
                    IsRapportExpanded = false;
                }

                RaisePropertyChanged(() => this.IsActionExpanded);
                RaisePropertyChanged(() => this.IsRapportExpanded);
                RaisePropertyChanged(() => this.IsAlerteExpanded);
                RaisePropertyChanged(() => this.IsAnalyseExpanded);
            }
        }

        /// <summary>
        /// Retourne si les alertes sont visible
        /// </summary>
        public bool IsExpanderVisible
        {
            get
            {
                return _isExpanderVisible;
            }
            set
            {
                _isExpanderVisible = value;
                RaisePropertyChanged(() => this.IsExpanderVisible);
                RaisePropertyChanged(() => this.IsExpanderRapportVisible);
            }
        }

        protected override void Cancel()
        {
            base.Cancel();
          
            IsAnalyseExpanded = false;
            IsActionExpanded = false;
            IsAlerteExpanded = false;
            IsRapportExpanded = false;

            RaisePropertyChanged(() => this.IsActionExpanded);
            RaisePropertyChanged(() => this.IsRapportExpanded);
            RaisePropertyChanged(() => this.IsAlerteExpanded);
            RaisePropertyChanged(() => this.IsAnalyseExpanded);

            RaisePropertyChanged("IsAnalyseExpanded");
        }

        /// <summary>
        /// Retourne si les alertes sont visible
        /// </summary>
        public bool IsExpanderRapportVisible
        {
            get
            {
                return _isExpanderVisible && !IsNewMode;
            }
        }
        /// <summary>
        /// Retourne si le formulaire est visible
        /// </summary>
        public bool IsFormulaireEnable
        {
            get
            {
                return _isFormulaireEnable;
            }
            set
            {
                _isFormulaireEnable = value;
                RaisePropertyChanged(() => this.IsFormulaireEnable);
                RaisePropertyChanged(() => this.IsLinkEnable);
                RaisePropertyChanged(() => this.IsSaveVisible);
            }
        }

        public bool IsLinkEnable
        {
            get
            {
                return !this.IsFormulaireEnable && !this.IsEditMode;
            }
        }

        public bool IsSaveVisible
        {
            get
            {
                return !this.IsFormulaireEnable && this.IsEditMode;
            }
        }

        /// <summary>
        /// Indique si l'annexe 1 doit être inclue dans le rapport d'analyse
        /// </summary>
        private Boolean _isAnnexe1;
        public Boolean IsAnnexe1
        {
            get
            {
                return _isAnnexe1;
            }
            set
            {
                _isAnnexe1 = value;
                RaisePropertyChanged(() => this.IsAnnexe1);
            }
        }

        /// <summary>
        /// Indique si l'annexe 2 doit être inclue dans le rapport d'analyse
        /// </summary>
        private Boolean _isAnnexe2;
        public Boolean IsAnnexe2
        {
            get
            {
                return _isAnnexe2;
            }
            set
            {
                _isAnnexe2 = value;
                RaisePropertyChanged(() => this.IsAnnexe2);
            }
        }
        /// <summary>
        /// Indique si l'annexe 3 doit être inclue dans le rapport d'analyse
        /// </summary>
        private Boolean _isAnnexe3;
        public Boolean IsAnnexe3
        {
            get
            {
                return _isAnnexe3;
            }
            set
            {
                _isAnnexe3 = value;
                RaisePropertyChanged(() => this.IsAnnexe3);
            }
        }
        /// <summary>
        /// Indique si l'annexe 4 doit être inclue dans le rapport d'analyse
        /// </summary>
        private Boolean _isAnnexe4;
        public Boolean IsAnnexe4
        {
            get
            {
                return _isAnnexe4;
            }
            set
            {
                _isAnnexe4 = value;
                RaisePropertyChanged(() => this.IsAnnexe4);
            }
        }
        /// <summary>
        /// Indique si l'annexe 5 doit être inclue dans le rapport d'analyse
        /// </summary>
        private Boolean _isAnnexe5;
        public Boolean IsAnnexe5
        {
            get
            {
                return _isAnnexe5;
            }
            set
            {
                _isAnnexe5 = value;
                RaisePropertyChanged(() => this.IsAnnexe5);
            }
        }
        /// <summary>
        /// Indique si l'annexe 6 doit être inclue dans le rapport d'analyse
        /// </summary>
        private Boolean _isAnnexe6;
        public Boolean IsAnnexe6
        {
            get
            {
                return _isAnnexe6;
            }
            set
            {
                _isAnnexe6 = value;
                RaisePropertyChanged(() => this.IsAnnexe6);
            }
        }

        #endregion

        #region Constructor

        public AnAnalyseEeViewModel()
            : base()
        {
            this.IsAutoAddOnEditMode = false;

            // Instanciation des commandes
            AddActionCommand = new ActionCommand<object>(
                obj => AddAction(), obj => UserCanAddAction);
            GenerateReportCommand = new ActionCommand<object>(
                obj => GenerateReport(), obj => CanGenerateRapport);
            GenerateReportAnnexe5Tab8EcdCommand = new ActionCommand<object>(
                obj => GenerateReportAnnexe5Tab8Ecd(), obj => CanGenerateRapport);
            GenerateReportAnnexe6Command = new ActionCommand<object>(
                obj => GenerateReportAnnexe6(), obj => CanGenerateRapport);
            AnalyseCommand = new ActionCommand<object>(
                obj => AnalyserEnsElec(), obj => ActiveBtnAnalyser);
            NavigateActionCommand = new ActionCommand<object>(
                obj => NavigateToAction(obj), obj => true);

            this.OnImportsSatisfiedEvent += (o, e) =>
            {
                EventAggregator.Subscribe<AnAction>(this);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                IEnumerable<AlerteDetail> desactivatedLines = this.AlerteDetail.Where(dv => dv.Alerte != null && dv.Alerte.Supprime && dv.CanDisable);
                foreach (AlerteDetail item in desactivatedLines)
                {
                    item.CanDisable = false;
                }
                RaisePropertyChanged(() => this.AlerteDetail);
            };

            this.OnCanceled += (o, e) =>
            {
                if (this.SelectedEntity.IsNew())
                {
                    this.SelectedEntity = null;
                }
                ActiveBtnAnalyser = false;
                RefreshCommands();
                RaisePropertyChanged(() => this.ActionDetail);
                RaisePropertyChanged(() => this.IsLinkEnable);
                RaisePropertyChanged(() => this.IsSaveVisible);
                this.RefreshListAlertes();
            };

            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.AnAnalyseEe_ExpanderTitle));
                    EventAggregator.Publish("AnAnalyseEe_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
                AnAction.CurrentUser = CurrentUser; 
            };

            this.OnDetailLoaded += (o, e) =>
            {
                // MAJ de la vue
                this.IsExpanderVisible = true;
                IsFormulaireEnable = false;

                IsAnnexe1 = false;
                IsAnnexe2 = false;
                IsAnnexe3 = false;
                IsAnnexe4 = false;
                IsAnnexe5 = false;
                IsAnnexe6 = false;

                RaisePropertyChanged(() => this.EnsElectriques);
                RaisePropertyChanged(() => this.ListEtatPC);
                RaisePropertyChanged(() => this.ActionDetail);

                ActiveBtnAnalyser = false;
                RefreshCommands();

                if (this.SelectedEntity != null && this.SelectedEntity.AnAnalyseEeVisite != null)
                {
                    this.IsBusy = true;

                    List<int> listCleAlerte = this.SelectedEntity.AnAnalyseEeVisite.SelectMany(ae => ae.Visite.Alertes.Select(a => a.CleAlerte)).ToList();

                    // Chargement des alertes
                    ((AlerteService)serviceAlerte).FindAlerteDetailByListCleAlerte(listCleAlerte, SearchAlerteDetailDone);
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.EnsElectriques);
            };

            this.OnAddedEntity += (o, e) =>
            {
                IsExpanderVisible = false;
                IsFormulaireEnable = true;
                ActiveBtnAnalyser = true;

                this.SelectedEntity.DateAnalyse = DateTime.Now;

                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.ActionDetail);
                RaisePropertyChanged(() => this.AlerteDetail);
                RaisePropertyChanged(() => this.IsExpanderRapportVisible);
                RefreshCommands();
            };

            this.OnViewModeChanged += (o, e) =>
            {
                RefreshCommands();
                RaisePropertyChanged(() => this.CanGenerateRapport);
                RaisePropertyChanged(() => this.IsLinkEnable);
                RaisePropertyChanged(() => this.IsSaveVisible);
            };
        }

        #endregion

        #region Override Methods

        protected override void DeactivateView(string viewName)
        {
            // Force la désactivation des fiches actions (car elles sont chargées dans une popup)
            this.Router.DeactivateView("FicheAction");
            
            this.Entities.Clear();
            this.SelectedEntity = null;
            this.EnsElecDistinct.Clear();
            this.CleEnsElectrique = null;
            this.RaisePropertyChanged(() => this.Entities);
            this.RaisePropertyChanged(() => this.EnsElecDistinct);
            this.Initialisation();

            base.DeactivateView(viewName);
        }

        protected override void Delete()
        {
            if (this.SelectedEntity != null && this.SelectedEntity.AnAction.Any())
            {
                InfoWindow.CreateNew(Resource.AnAnalyseEe_InfoSuppressionAction);
            }
            else
            {
                base.Delete();
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

            this.saveGeoPreferences();

            ((AnAnalyseEeService)this.service).FindAnalyseEeByCriterias(
                this.FiltreCleRegion, this.FiltreCleAgence, this.FiltreCleSecteur, this.EnsembleElectriqueTitle, this.IncludeStation, this.IncludePosteGaz, SearchDone);
        }

        #endregion

        #region Private Methods

        private void RefreshCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            AnalyseCommand.RaiseCanExecuteChanged();
            GenerateReportCommand.RaiseCanExecuteChanged();
            GenerateReportAnnexe5Tab8EcdCommand.RaiseCanExecuteChanged();
            GenerateReportAnnexe6Command.RaiseCanExecuteChanged();
            AddActionCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Rafraichissement de la liste (tout est raffraichit)
        /// </summary>
        private void RefreshListAlertes()
        {
            foreach (AlerteDetail item in this.AlerteDetail)
            {
                Alerte element = (this.SelectedEntity != null) ? this.SelectedEntity.AnAnalyseEeVisite.SelectMany(ae => ae.Visite.Alertes).FirstOrDefault(a => a.CleAlerte == item.CleAlerte) : null;
                if (element != null)
                {
                    item.Alerte = element;
                }
            }
            this.CheckCanDisableByGeo();
            RaisePropertyChanged(() => this.AlerteDetail);
        }

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
        /// Navigation vers une action
        /// </summary>
        /// <param name="obj">clé de l'action</param>
        private void NavigateToAction(object obj)
        {
            Uri dest = new Uri((string.Format("/Pages/proteca.aspx#/{0}/{1}/Id={2}",
               MainNavigation.Visite.GetStringValue(),
               VisiteNavigation.FicheAction.GetStringValue(),
               ((AnAction)obj).CleAction.ToString())),
               UriKind.Relative);
           

            HtmlPage.Window.Navigate(dest);

           // NavigationService.Navigate(VisiteNavigation.FicheAction.GetStringValue(), "Id", ((AnAction)obj).CleAction.ToString());
        }

        /// <summary>
        /// Initialisation de l'écran
        /// </summary>
        private void Initialisation()
        {
            if (!IsNewMode)
            {
                IsExpanderVisible = false;
                IsFormulaireEnable = false;
                IsRapportExpanded = true;
            }

            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => this.EnsElecDistinct);
        }

        /// <summary>
        /// Chargement des éléments de l'écran
        /// </summary>
        private void ChargerDetailEcran()
        {
            this.SelectedEntity.UsrUtilisateur = this.CurrentUser;

            ActiveBtnAnalyser = false;
            AnalyseCommand.RaiseCanExecuteChanged();
            GenerateReportCommand.RaiseCanExecuteChanged();
            GenerateReportAnnexe5Tab8EcdCommand.RaiseCanExecuteChanged();
            GenerateReportAnnexe6Command.RaiseCanExecuteChanged();
            AddActionCommand.RaiseCanExecuteChanged();
            IsExpanderVisible = true;
            IsFormulaireEnable = false;
        }

        /// <summary>
        /// Méthode de retour du chargement des visites
        /// </summary>
        /// <param name="error"></param>
        private void SearchVisitesDone(Exception error)
        {
            ObservableCollection<Visite> visites = (this.ServiceVisite as VisiteService).Entities;

            //Création des AnAnalyseEEVisite
            foreach (Visite v in visites)
            {
                this.SelectedEntity.AnAnalyseEeVisite.Add(new AnAnalyseEeVisite()
                {
                    CleVisite = v.CleVisite
                });
            }

            List<int> listCleAlerte = visites.SelectMany(v => v.Alertes.Select(a => a.CleAlerte)).ToList();

            // Chargement des alertes
            ((AlerteService)serviceAlerte).FindAlerteDetailByListCleAlerte(listCleAlerte, SearchAlerteDetailDone);

            ChargerDetailEcran();
        }

        /// <summary>
        /// Méthode de retour de chargement des alertes
        /// </summary>
        /// <param name="error"></param>
        private void SearchAlerteDetailDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Alerte).Name));
            }
            else
            {
                this.RefreshListAlertes();
            }

            IsBusy = false;
        }

        /// <summary>
        /// Méthode de retour de recherche
        /// </summary>
        /// <param name="Error"></param>
        private void SearchDone(Exception Error)
        {
            if (Error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, Error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(AnAnalyseEe).Name));
            }
            else
            {

                // Navigation vers le premier élément
                if (IsAutoNavigateToFirst && this.Entities != null && this.Entities.Any())
                {
                    int _cleAnalyseEe = (int)Entities.First().GetCustomIdentity();
                    if (this.SelectedEntity != null && this.SelectedEntity.CleAnalyse == _cleAnalyseEe)
                    {
                        this.IsBusy = false;
                    }
                    else
                    {
                        NavigationService.Navigate(_cleAnalyseEe);
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

             
                RaisePropertyChanged(() => this.EnsElecDistinct);
                RaisePropertyChanged(() => this.Entities);
                RaisePropertyChanged(() => EntitiesCount);
                RaisePropertyChanged(() => ResultIndicator);
                if (this.EnsElecDistinct.Any())
                {
                    CleEnsElectrique = this.EnsElecDistinct.Select(c => c.CleEnsElectrique).FirstOrDefault();
                }
                else
                {
                    RaisePropertyChanged(() => this.CleEnsElectrique);
                }
            }
        }

        /// <summary>
        /// Affiche la popup pour ajouter une action
        /// </summary>
        private void AddAction()
        {
            ChildWindow.Title = "Ajout d'une action";
            ChildWindow.Closed += (o, e) =>
            {
                RaisePropertyChanged(() => this.SelectedEntity);
                IsBusy = false;
            };

            ChildWindow.Width = 660;
            ChildWindow.MaxWidth = 660;
            ChildWindow.HorizontalAlignment = HorizontalAlignment.Center;

            ChildWindow.Show();
            EventAggregator.Publish("PopupAction".AsViewNavigationArgs().AddNamedParameter("IsPopupMode", true).AddNamedParameter("IsFirstAdd", true).AddNamedParameter("Analyse", SelectedEntity));

        }

        ///// <summary>
        ///// Méthode de génération d'un rapport
        ///// </summary>
        //private void GenerateReport()
        //{
        //    if (SelectedEntity != null)
        //    {
        //        // MANTIS-18602 - Mise à jour de la date d'édition
        //        SelectedEntity.DateEdition = DateTime.Now;
        //        this.OnSaveSuccess += GenerateReportOnSaveSuccess;
        //        this.OnSaveError += (o,e) => { 
        //              // ??
        //             this.OnSaveSuccess -= GenerateReportOnSaveSuccess;
        //        };

        //        Save(true);
                
        //    }

        //private void GenerateReportOnSaveSuccess(object o, EventArgs e)
        //{
        //    String rapportUrl = Rapports.printDocumentUrl;

        //    String urlDetail = Rapports.RANA_Rapport_Analyse_FileName;

        //    rapportUrl += String.Format(urlDetail, this.SelectedEntity.CleAnalyse, IsAnnexe1, IsAnnexe2, IsAnnexe3, IsAnnexe4, IsAnnexe5, false, this.CurrentUser.CleUtilisateur);

        //    HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");

        //    this.OnSaveSuccess -= GenerateReportOnSaveSuccess;

        //}

        //}
        /// <summary>
        /// Méthode de génération d'un rapport
        /// </summary>
        private void GenerateReport()
        {
            if (SelectedEntity != null)
            {
                // MANTIS-18602 - Mise à jour de la date d'édition
                SelectedEntity.DateEdition = DateTime.Now;
                ((AnAnalyseEeService)this.service).SaveChangesToGenerate(this.SelectedEntity.CleAnalyse);

                String rapportUrl = Rapports.printDocumentUrl;

                String urlDetail = Rapports.RANA_Rapport_Analyse_FileName;

                rapportUrl += String.Format(urlDetail, this.SelectedEntity.CleAnalyse, IsAnnexe1, IsAnnexe2, IsAnnexe3, IsAnnexe4, IsAnnexe5, false, this.CurrentUser.CleUtilisateur);

                HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
            }
        }

        

        /// <summary>
        /// Méthode de génération d'un rapport
        /// </summary>
        private void GenerateReportAnnexe6()
        {
            if (this.SelectedEntity != null)
            {
                String rapportUrl = Rapports.printDocumentUrl;

                String urlDetail = Rapports.RANA_Rapport_Analyse_Annexe6_FileName;

                rapportUrl += String.Format(urlDetail, this.SelectedEntity.CleEnsElectrique);

                HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
            }
        }

        /// <summary>
        /// Méthode de génération d'un rapport
        /// MANTIS 12775 FSI 24/06/2014 : Ajout d'un bouton pour générer le sous rapport Tableaux de mesure ECD
        /// </summary>
        private void GenerateReportAnnexe5Tab8Ecd()
        {
            if (this.SelectedEntity != null)
            {
                String rapportUrl = Rapports.printDocumentUrl;

                String urlDetail = Rapports.RANA_Rapport_Analyse_Annexe5_ECD_FileName;

                rapportUrl += String.Format(urlDetail, this.SelectedEntity.CleAnalyse);

                HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
            }
        }

        /// <summary>
        /// Méthode d'analyse de l'ensemble électrique
        /// </summary>
        private void AnalyserEnsElec()
        {
            if (this.SelectedEntity.CleEnsElectrique != 0 && this.SelectedEntity.DateDebutPeriode != null && this.SelectedEntity.DateFinPeriode != null && !(this.SelectedEntity.DateDebutPeriode > this.SelectedEntity.DateFinPeriode))
            {
                this.IsBusy = true;
                // Chargement des visites reliées à l'analyse
                ((VisiteService)ServiceVisite).FindVisitesByAnalyseEECriterias(this.SelectedEntity.CleEnsElectrique, this.SelectedEntity.DateDebutPeriode.Value, this.SelectedEntity.DateFinPeriode.Value, this.SearchVisitesDone);

            }
            else if (this.SelectedEntity.DateDebutPeriode > this.SelectedEntity.DateFinPeriode)
            {
                InfoWindow.CreateNew("La date de début ne peut pas être supérieure à la date de fin");
            }
            else
            {
                InfoWindow.CreateNew("Veuillez indiquer un ensemble électrique, une date de début ainsi qu'une date de fin afin de procéder à l'analyse");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Récupération des évènements publiés
        /// </summary>
        /// <param name="publishedEvent"></param>
        public void HandleEvent(AnAction publishedEvent)
        {
            if (IsActive)
            {
                // Récupération de l'action
                AnAction monaction = serviceAnAction.Entities.Where(c => c == publishedEvent).FirstOrDefault();

                // On rattache l'action à l'analyse
                if (this.SelectedEntity != null)
                {
                    monaction.AnAnalyse = this.SelectedEntity;
                }

                // MAJ de la vue
                //IsActionVisible = true;
                RaisePropertyChanged(() => this.ActionDetail);
            }
        }

        #endregion

        #region Autorisation

        /// <summary>
        /// Autorise la génération de rapport suivant le mode d'édition
        /// </summary>
        public Boolean CanGenerateRapport
        {
            get
            {
                return !IsEditMode && SelectedEntity != null && GetAutorisation(RefUsrAutorisation.ListAutorisationsEnum.GENERATION_RAPPORT);
            }
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'ajout d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanAdd()
        {
            return !IsEditMode && GetAutorisation(RefUsrAutorisation.ListAutorisationsEnum.ANALYSE_NIV);
        }

        /// <summary>
        /// Détermine les droist de l'utilisateur courant 
        /// sur la suppression d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanDelete()
        {
            return GetAutorisationNiv(RefUsrAutorisation.ListAutorisationsEnum.ANALYSE_NIV);
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'édition d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanEdit()
        {
            return GetAutorisationNiv(RefUsrAutorisation.ListAutorisationsEnum.ANALYSE_NIV);
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur la sauvegarde d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanSave()
        {
            return GetAutorisationNiv(RefUsrAutorisation.ListAutorisationsEnum.ANALYSE_NIV);
        }

        /// <summary>
        /// Prends en paramètre un enum autorisation et retourne le droit 
        /// en fonction de la portée du role de l'utilisateur et de l'ensemble electrique analysé
        /// </summary>
        /// <param name="autorisation">ListAutorisationEnum du droit à vérifier</param>
        /// <returns></returns>
        private bool GetAutorisationNiv(RefUsrAutorisation.ListAutorisationsEnum autorisation)
        {
            if (this.SelectedEntity != null && this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(autorisation);

                if (role != null && role.RefUsrPortee != null)
                {
                    switch (role.RefUsrPortee.GetPorteesEnum())
                    {
                        case RefUsrPortee.ListPorteesEnum.Autorisee:
                            //Même que national
                        case RefUsrPortee.ListPorteesEnum.National:
                            return true;
                        case RefUsrPortee.ListPorteesEnum.Region:
                            return serviceGeoEnsElec.Entities.Where(c => c.CleEnsElectrique == this.SelectedEntity.CleEnsElectrique
                                                                            && this.CurrentUser.GeoAgence != null
                                                                            && c.CleRegion == this.CurrentUser.GeoAgence.CleRegion).Any();
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            return serviceGeoEnsElec.Entities.Where(c => c.CleEnsElectrique == this.SelectedEntity.CleEnsElectrique
                                                                            && this.CurrentUser.CleAgence.HasValue 
                                                                            && c.CleAgence == this.CurrentUser.CleAgence).Any();
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                            return serviceGeoEnsElec.Entities.Where(c => c.CleEnsElectrique == this.SelectedEntity.CleEnsElectrique
                                                                            && this.CurrentUser.CleSecteur.HasValue 
                                                                            && c.CleSecteur == this.CurrentUser.CleSecteur).Any();
                        case RefUsrPortee.ListPorteesEnum.Interdite:
                        default:
                            return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Prends en paramètre un enum autorisation et retourne le droit 
        /// en fonction du role de l'utilisateur
        /// </summary>
        /// <param name="autorisation">ListAutorisationEnum du droit à vérifier</param>
        /// <returns></returns>
        private bool GetAutorisation(RefUsrAutorisation.ListAutorisationsEnum autorisation)
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(autorisation);

                if (role != null && role.RefUsrPortee != null)
                {
                    switch (role.RefUsrPortee.GetPorteesEnum())
                    {
                        case RefUsrPortee.ListPorteesEnum.Autorisee:
                        case RefUsrPortee.ListPorteesEnum.National:
                        case RefUsrPortee.ListPorteesEnum.Region:
                        case RefUsrPortee.ListPorteesEnum.Agence:
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                            return true;
                        case RefUsrPortee.ListPorteesEnum.Interdite:
                        default:
                            return false;
                    }
                }
            }
            return false;
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

        private bool UserCanAddAction
        {
            get
            {
                //MANTIS 10815, 07/05/2014, FSI : Ajout de la vérification que l'utilisateur n'est pas prestataire
                return !IsEditMode && SelectedEntity != null && CurrentUser != null && !CurrentUser.EstPresta && GetAutorisationNiv(RefUsrAutorisation.ListAutorisationsEnum.CRE_FICHE_ACTION_NIV);
            }
        }

        #endregion

        public object id { get; set; }
    }
}
