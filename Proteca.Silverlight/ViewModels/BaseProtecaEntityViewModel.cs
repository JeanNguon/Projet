using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Jounce.Framework.Workflow;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Exception Error { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ActivateViewEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, object> ViewParameter { get; set; }
    }

    public class ProtecaActionCommand<T> : ActionCommand<T>
    {
        public override void RaiseCanExecuteChanged()
        {
            JounceHelper.ExecuteOnUI(() => base.RaiseCanExecuteChanged());
        }


        /// <summary>
        /// Default constructor - do nothing
        /// </summary>
        public ProtecaActionCommand() : base() { }

        /// <summary>
        ///     Constructor with action to perform
        /// </summary>
        /// <param name="execute">The action to execute</param>
        public ProtecaActionCommand(Action<T> execute) : base(execute) { }

        /// <summary>
        ///     Constructor with action and condition
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">A function to determine whether execution is allowed</param>
        public ProtecaActionCommand(Action<T> execute, Func<T, bool> canExecute) : base(execute, canExecute) { }
    }
    public class BaseAsyncProtecaEntityViewModel<T> : BaseProtecaEntityViewModel<T> where T : Entity
    {
        protected override void RaisePropertyChanged(string propertyName)
        {
            JounceHelper.ExecuteOnUI(() => base.RaisePropertyChanged(propertyName));
        }
    }

    /// <summary>
    /// Implémentation de base d'un ViewModel permettant de gérer l'affichage et l'édition d'un type d'entité T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseProtecaEntityViewModel<T> : BaseViewModel, IEventSink<ViewMode>, IEventSink<ExpanderMode>, IPartImportsSatisfiedNotification where T : Entity
    {



        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type T
        /// </summary>
        [Import]
        public IEntityService<T> service { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'utilisateur connecté
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> userService { get; set; }

        /// <summary>
        /// Service utilisé pour naviguer dans l'application
        /// </summary>
        [Import(typeof(INavigationService))]
        public INavigationService NavigationService { get; set; }

        /// <summary>
        /// Service utilisé pour l'impression
        /// </summary>
        [Import(typeof(IPrintingService))]
        public IPrintingService PrintService { get; set; }

        /// <summary>
        /// Service utilisé pour l'export PDF et Excel
        /// </summary>
        [Import(typeof(IExportService))]
        public IExportService ExportService { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceRefEnumValeur { get; set; }

        #endregion

        #region Events

        #region Région/Agence/Secteur

        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnRegionSelected;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnAgenceSelected;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnSecteurSelected;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public EventHandler<ActivateViewEventArgs> OnViewActivated;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnSaveSuccess;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnDeleteSuccess;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnCanceled;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnImportsSatisfiedEvent;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler<ErrorEventArgs> OnSaveError;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler<ErrorEventArgs> OnDeleteError;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnViewModeChanged;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnViewModeChanging;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnDetailLoaded;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnEntitiesLoaded;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnAddedEntity;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnFindLoaded;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnAllServicesLoaded;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnAllServicesLoadedSync;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructeur par défaut de l'ensemble des ViewModels
        /// </summary>
        public BaseProtecaEntityViewModel()
        {
            // Initialisation par défaut
            IsAutoNavigateToFirst = true;
            AllowEditEmptyEntities = true;
            IsAutoAddOnEditMode = true;

            // Define commands
            FindCommand = new ActionCommand<object>(
                obj => Find(), obj => CanFind);
            SaveCommand = new ActionCommand<object>(
               obj => Save(), obj => CanSave);
            CancelCommand = new ActionCommand<object>(
               obj => Cancel(), obj => CanCancel);
            AddCommand = new ActionCommand<object>(
                obj => Add(), obj => CanAdd);
            DeleteCommand = new ActionCommand<object>(
               obj => Delete(), obj => CanDelete);
            EditCommand = new ActionCommand<object>(
                obj => Edit(), obj => CanEdit);
            SelectCommand = new ActionCommand<object>(
                obj => Select(), obj => CanSelect);
            PrintCommand = new ActionCommand<object>(
                obj => PrintGrid(obj), obj => CanPrintGrid);
            ExportPDFCommand = new ActionCommand<object>(
                obj => ExportPDF(obj), obj => CanExportPDF);
            ExportExcelCommand = new ActionCommand<object>(
                obj => ExportExcel(obj), obj => CanExportExcel);
        }

        #endregion

        #region Commands

        public IActionCommand FindCommand { get; protected set; }
        public IActionCommand SaveCommand { get; protected set; }
        public IActionCommand CancelCommand { get; protected set; }
        public IActionCommand DeleteCommand { get; protected set; }
        public IActionCommand AddCommand { get; protected set; }
        public IActionCommand EditCommand { get; protected set; }
        public IActionCommand SelectCommand { get; protected set; }
        public IActionCommand PrintCommand { get; protected set; }
        public IActionCommand ExportPDFCommand { get; protected set; }
        public IActionCommand ExportExcelCommand { get; protected set; }

        #endregion

        #region Properties

        #region Région/Agence/Secteur

        /// <summary>
        /// Déclaration de la variable FiltreCleRegion
        /// </summary>
        private int? _filtreCleRegion;

        /// <summary>
        /// Déclaration de la variable FiltreCleAgence
        /// </summary>
        private int? _filtreCleAgence;

        /// <summary>
        /// Déclaration de la variable FiltreCleSecteur
        /// </summary>
        private int? _filtreCleSecteur;

        /// <summary>
        /// Retourne la clé de région filtré
        /// </summary>
        public int? FiltreCleRegion
        {
            get
            {
                return _filtreCleRegion;
            }
            set
            {
                _filtreCleRegion = value;

                if (OnRegionSelected != null)
                {
                    OnRegionSelected(this, null);
                }

                RaisePropertyChanged(() => this.FiltreCleRegion);
            }
        }

        /// <summary>
        /// Retourne la clé d'agence filtré
        /// </summary>
        public int? FiltreCleAgence
        {
            get
            {
                return _filtreCleAgence;
            }
            set
            {
                _filtreCleAgence = value;

                if (OnAgenceSelected != null)
                {
                    OnAgenceSelected(this, null);
                }

                RaisePropertyChanged(() => this.FiltreCleAgence);
            }
        }

        /// <summary>
        /// Retourne la clé de secteur filtré
        /// </summary>
        public int? FiltreCleSecteur
        {
            get
            {
                return _filtreCleSecteur;
            }
            set
            {
                _filtreCleSecteur = value;

                if (OnSecteurSelected != null)
                {
                    OnSecteurSelected(this, null);
                }

                RaisePropertyChanged(() => this.FiltreCleSecteur);
            }
        }

        #endregion

        #region Expander

        private Boolean _isLeftBarExpanded = true;
        public Boolean IsLeftBarExpanded
        {
            get
            {
                return true;
                // Désactivation des tileview à droite si la zone de recherche est replié
                //return _isLeftBarExpanded; 
            }
            set
            {
                _isLeftBarExpanded = value;
                RaisePropertyChanged(() => this.IsLeftBarExpanded);
            }
        }

        #endregion

        #region TileView

        private Telerik.Windows.Controls.TileViewItemState _mainTileItemState = Telerik.Windows.Controls.TileViewItemState.Maximized;
        public Telerik.Windows.Controls.TileViewItemState MainTileItemState
        {
            get { return _mainTileItemState; }
            set
            {
                _mainTileItemState = value;
                RaisePropertyChanged(() => this.MainTileItemState);
                RaisePropertyChanged(() => this.IsMainTileItemState);
            }
        }

        public bool IsMainTileItemState
        {
            get
            {
                return MainTileItemState == Telerik.Windows.Controls.TileViewItemState.Maximized;
            }
        }

        #endregion

        public bool AllServicesLoaded { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Utilisateur connecté
        /// </summary>
        public UsrUtilisateur CurrentUser
        {
            get
            {
                return this.userService.CurrentUser;
            }
        }

        private T entity;
        /// <summary>
        /// Element de type T sélectionné
        /// </summary>
        public T SelectedEntity
        {
            get { return entity; }
            set
            {
                var previous = entity;
                if (previous != value)
                {
                    entity = value;
                    SetCanProperties();
                    this.SelectedEntity.ActivateChangePropagation();
                    if (this.SelectedEntity != null && this.SelectedEntity.EntityState != EntityState.Detached)
                    {
                        // Lors de chaque mise à jour d'une propriété de l'élément en cours d'édition
                        this.SelectedEntity.PropertyChanged += (o, e) =>
                        {
                            if (this.SelectedEntity != null)
                            {
                                // Mise à jour des propriétés Can* en fonction des changements et des erreurs identifiés
                                //this.CanSave = !this.SelectedEntity.HasValidationErrors && (this.SelectedEntity.HasChanges || this.SelectedEntity.HasChildChanges() || this.SelectedEntity.IsNew());
                                //this.CanCancel = true;
                                if (this.NotifyError)
                                {
                                    this.NotifyError = this.SelectedEntity.HasValidationErrors;
                                }
                            }
                            SetCanProperties();
                        };
                    }

                    RaisePropertyChanged(() => this.SelectedEntity);
                    RaisePropertyChanged(() => this.PreviousEntity);
                    RaisePropertyChanged(() => this.PreviousUri);
                    RaisePropertyChanged(() => this.NextEntity);
                    RaisePropertyChanged(() => this.NextUri);
                }
            }
        }

        /// <summary>
        /// Element de type T précédent
        /// </summary>
        public virtual T PreviousEntity
        {
            get
            {
                if (SelectedEntity != null)// && !SelectedEntity.IsNew()) //EPI : En mode édition, les liens doivent être grisé donc pas besoin de vérifier si l'entité est nouvelle
                {
                    int index = Entities.IndexOf(SelectedEntity);
                    if (index > 0)
                    {
                        return Entities.ElementAt(index - 1);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Uri vers l'élément précédent
        /// </summary>
        public virtual Uri PreviousUri
        {
            get
            {
                if (PreviousEntity != null)
                {
                    return NavigationService.getUriById((int)PreviousEntity.GetCustomIdentity());
                }
                return null;
            }
        }

        /// <summary>
        /// Element de type T suivant
        /// </summary>
        public virtual T NextEntity
        {
            get
            {
                if (SelectedEntity != null)// && !SelectedEntity.IsNew()) //EPI : En mode édition, les liens doivent être grisé donc pas besoin de vérifier si l'entité est nouvelle
                {
                    int index = Entities.IndexOf(SelectedEntity);
                    if (index >= 0 && index < Entities.Count - 1)
                    {
                        return Entities.ElementAt(index + 1);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Uri vers l'élément suivant
        /// </summary>
        public virtual Uri NextUri
        {
            get
            {
                if (NextEntity != null)
                {
                    return NavigationService.getUriById((int)NextEntity.GetCustomIdentity());
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual ObservableCollection<T> Entities
        {
            get
            {
                return service.Entities;
            }
        }

        /// <summary>
        /// Autorise la navigation automatique vers le premier élément (sélection d'une seule entité dans l'écran)
        /// </summary>
        public bool IsAutoNavigateToFirst { get; set; }

        /// <summary>
        /// Autorise l'ajout ou l'édition sans avoir effectué de recherche préalable (ex : écran de type tableau sans expander autorisant l'ajout)
        /// </summary>
        public bool AllowEditEmptyEntities { get; set; }

        /// <summary>
        /// Autorise l'ajout automatique lors de la navigation en édition
        /// </summary>
        public bool IsAutoAddOnEditMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private bool isBusy;
        /// <summary>
        /// 
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                SetCanProperties();
                RaisePropertyChanged(() => this.IsBusy);
                RaisePropertyChanged(() => this.IsNotBusy);
                EventAggregator.Publish(isBusy ? IsBusyEnum.IsBusy : IsBusyEnum.IsNotBusy);
            }
        }

        public bool IsNotBusy
        {
            get
            {
                return !isBusy;
            }
        }

        private bool _notifyError;

        public bool NotifyError
        {
            get
            {
                return _notifyError;
            }
            set
            {
                _notifyError = value;
                RaisePropertyChanged(() => this.NotifyError);
            }
        }

        private bool _formatValidationError = false;

        public bool HasFormatValidationError
        {
            get
            {
                return _formatValidationError;
            }
            set
            {
                _formatValidationError = value;
                RaisePropertyChanged(() => this.HasFormatValidationError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool canAdd;
        /// <summary>
        /// 
        /// </summary>
        public bool CanAdd
        {
            get { return canAdd && UserCanAdd; }
            set
            {
                canAdd = value;
                RaisePropertyChanged(() => this.CanAdd);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool canSave;
        /// <summary>
        /// 
        /// </summary>
        public bool CanSave
        {
            get { return true; } // Par défaut l'utilisateur peut toujours cliquer sur le bouton sauvegarder en mode édition
            set
            {
                canSave = value;
                RaisePropertyChanged(() => this.CanSave);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool canDelete;
        /// <summary>
        /// 
        /// </summary>
        public bool CanDelete
        {
            get { return canDelete && UserCanDelete; }
            set
            {
                canDelete = value;
                RaisePropertyChanged(() => this.CanDelete);
            }
        }

        public bool CanSearch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool canEdit;
        /// <summary>
        /// 
        /// </summary>
        public virtual bool CanEdit
        {
            get { return canEdit && UserCanEdit && (SelectedEntity != null || (Entities != null && (Entities.Any() || AllowEditEmptyEntities) && !IsAutoNavigateToFirst)); }
            set
            {
                canEdit = value;
                RaisePropertyChanged(() => this.CanEdit);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool canSelect;
        /// <summary>
        /// 
        /// </summary>
        public bool CanSelect
        {
            get { return canSelect; }
            set
            {
                canSelect = value;
                RaisePropertyChanged(() => this.CanSelect);
            }
        }

        private bool canCancel;
        private bool canFind;
        private bool canExportExcel;
        private bool canExportPDF;
        private bool canPrintGrid;

        /// <summary>
        /// 
        /// </summary>
        public bool CanCancel
        {
            get { return true; }// TODO : gérer les modifications sur liste déroulantecanCancel; }
            set
            {
                canCancel = value;
                RaisePropertyChanged(() => this.CanCancel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanFind
        {
            get { return canFind; }
            set
            {
                canFind = value;
                RaisePropertyChanged(() => this.CanFind);
            }
        }

        /// <summary>
        /// Autorise l'export Excel
        /// </summary>
        public bool CanExportExcel
        {
            get { return canExportExcel; }
            set
            {
                canExportExcel = value;
                RaisePropertyChanged(() => this.CanExportExcel);
            }
        }

        /// <summary>
        /// Autorise l'export PDF
        /// </summary>
        public bool CanExportPDF
        {
            get { return canExportPDF; }
            set
            {
                canExportPDF = value;
                RaisePropertyChanged(() => this.CanExportPDF);
            }
        }

        /// <summary>
        /// Autorise l'impression d'un tableau
        /// </summary>
        public bool CanPrintGrid
        {
            get { return canPrintGrid; }
            set
            {
                canPrintGrid = value;
                RaisePropertyChanged(() => this.CanPrintGrid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int EntitiesCount
        {
            get
            {
                return Entities != null ? Entities.Count() : 0;
            }
        }

        /// <summary>
        /// Retourne le nombre de résultat formaté
        /// </summary>
        public string ResultIndicator
        {
            get
            {
                return (EntitiesCount > 0) ? EntitiesCount.ToString() + ((EntitiesCount == 1) ? " résultat trouvé" : " résultats trouvés") : "Aucun résultat trouvé";
            }
        }

        /// <summary>
        /// Surcharger les propriétées suivantes pour contrôler les droits sur l'écran concerné
        /// </summary>

        public bool UserCanAdd
        {
            get
            {
                return GetUserCanAdd();
            }
        }

        public bool UserCanSave
        {
            get
            {
                return GetUserCanSave();
            }
        }

        public bool UserCanDelete
        {
            get
            {
                return GetUserCanDelete();
            }
        }

        public bool UserCanEdit
        {
            get
            {
                return GetUserCanEdit();
            }
        }

        public bool UserCanRead
        {
            get
            {
                return GetUserCanRead();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _isEditMode;
        /// <summary>
        /// 
        /// </summary>
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                bool lastEditMode = _isEditMode;
                if (value != _isEditMode)
                {
                    _isEditMode = value;

                    if (OnViewModeChanging != null)
                    {
                        OnViewModeChanging(this, null);
                    }

                    // Publish automatique si la valeur change
                    if (value != lastEditMode)
                    {
                        EventAggregator.Publish(value ? ViewMode.EditMode : ViewMode.NavigationMode);
                    }

                    if (OnViewModeChanged != null)
                    {
                        OnViewModeChanged(this, null);
                    }
                }

                RaisePropertyChanged(() => this.IsEditMode);
                RaisePropertyChanged(() => this.IsNewMode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNewMode
        {
            get { return SelectedEntity != null ? IsEditMode && SelectedEntity.IsNew() : false; }
        }

        /// <summary>
        /// 
        /// </summary>
        private int? _selectedId;
        public int? SelectedId
        {
            get
            {
                return _selectedId;
            }
            set
            {
                int? previous = _selectedId;
                if (value != previous)
                {
                    _selectedId = value;
                    if (_selectedId.HasValue)
                    {
                        IsBusy = true;
                        NavigationService.Navigate(_selectedId.Value);
                    }
                }
                RaisePropertyChanged(() => this.SelectedId);
            }
        }

        /// <summary>
        /// Déclaration de la variable de filtre de l'entité
        /// </summary>
        protected List<Expression<Func<T, bool>>> Filtres { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            EventAggregator.Subscribe<ViewMode>(this);
            EventAggregator.Subscribe<ExpanderMode>(this);

            // Load all entities
            //this.LoadAllServices();
            if (this.OnImportsSatisfiedEvent != null)
            {
                OnImportsSatisfiedEvent(this, null);
            }
            this.SetCanProperties();
        }

        /// <summary>
        /// Chargement de toutes les entitées liées aux ViewModel
        /// </summary>
        private void LoadAllServices()
        {
            EventHandler onAllServicesLoaded = null;
            onAllServicesLoaded = (o, e) =>
            {
                EntitiesLoaded(null);
                OnAllServicesLoaded -= onAllServicesLoaded;
            };
            OnAllServicesLoaded += onAllServicesLoaded;
            this.EnsureAllNeededServicesLoaded();
        }

        /// <summary>
        /// Rechargement de toutes les entitées liées aux ViewModel (mode déporté)
        /// </summary>
        private void ReloadAllServices()
        {
            AllServicesLoaded = false;
            this.EnsureAllNeededServicesLoadedSync(false);

            if (SelectedEntity != null)
            {
                SelectedEntity = Entities.ElementAtOrDefault((int)SelectedEntity.GetCustomIdentity());
            }
        }

        #region Default Methods

        protected virtual bool GetUserCanAdd()
        {
            return this.CurrentUser != null && this.CurrentUser.IsAdministrateur;
        }
        protected virtual bool GetUserCanDelete()
        {
            return this.CurrentUser != null && this.CurrentUser.IsAdministrateur;
        }
        protected virtual bool GetUserCanSave()
        {
            return this.CurrentUser != null && this.CurrentUser.IsAdministrateur;
        }
        protected virtual bool GetUserCanEdit()
        {
            return this.CurrentUser != null && this.CurrentUser.IsAdministrateur;
        }
        protected virtual bool GetUserCanRead()
        {
            return true;
        }

        /// <summary>
        /// Lancement de l'impression de la grid
        /// </summary>
        /// <param name="parameter"></param>
        private void PrintGrid(object parameter)
        {
            //TODO : Mettre un IsBusy car pas de retour pour l'utilisateur de l'avancement de l'export
            //TODO : Gérer l'exception OutOfMemoryException pour les tableaux trop longs
            try
            {
                this.PrintService.PrintGrid(parameter);
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, ex);
                if (ex is OutOfMemoryException)
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_OutOfMemory);
                }
                else
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_PrintError);
                }
            }
            IsBusy = false;
        }

        /// <summary>
        /// Lancement de l'export PDF
        /// </summary>
        /// <param name="parameter"></param>
        private void ExportPDF(object parameter)
        {
            IsBusy = true;
            try
            {
                this.ExportService.ExportPDF(parameter);
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, ex);
                if (ex is System.IO.IOException)
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_FileInUseError);
                }
                else if (ex is OutOfMemoryException)
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_OutOfMemory);
                }
                else
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ExportPDFError);
                }
            }
            IsBusy = false;
        }

        /// <summary>
        /// Lancement de l'export Excel
        /// </summary>
        /// <param name="parameter"></param>
        private void ExportExcel(object parameter)
        {
            IsBusy = true;
            try
            {
                this.ExportService.ExportExcel(parameter);
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, ex);
                if (ex is System.IO.IOException)
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_FileInUseError);
                }
                else if (ex is OutOfMemoryException)
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_OutOfMemory);
                }
                else
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ExportXLSError);
                }
            }

            IsBusy = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedEvent"></param>
        public void HandleEvent(ViewMode publishedEvent)
        {
            if (IsActive)
            {
                IsEditMode = publishedEvent == ViewMode.EditMode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedEvent"></param>
        public void HandleEvent(ExpanderMode publishedEvent)
        {
            if (IsActive)
            {
                IsLeftBarExpanded = publishedEvent == ExpanderMode.Expanded;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Save()
        {
            Save(false);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Save(bool forceSave)
        {
            IsBusy = true;

            if (IsEditMode || forceSave)
            {
                Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

                Collection<ValidationResult> errors = new Collection<ValidationResult>();
                bool isValid = true;
                if (SelectedEntity != null)
                {
                    SelectedEntity.ValidationErrors.Clear();
                    isValid = Validator.TryValidateObject(SelectedEntity, new ValidationContext(SelectedEntity, null, null), errors, true);
                }
                if (isValid && !HasFormatValidationError)
                {
                    if (!UserCanSave)
                    {
                        ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
                    }
                    else
                    {
                        bool isNew = this.SelectedEntity.IsNew();

                        try
                        {
                            service.SaveChanges((error) =>
                            {
                                IsBusy = false;
                                if (error == null)
                                {
                                    this.IsEditMode = false;

                                    if (this.OnSaveSuccess != null)
                                    {
                                        this.OnSaveSuccess(this, null);
                                    }

                                    if (IsAutoNavigateToFirst && isNew && this.SelectedEntity.GetCustomIdentity() != null)
                                    {
                                        NavigateToId((int)this.SelectedEntity.GetCustomIdentity());
                                    }

                                    RaisePropertyChanged(() => Entities);
                                    RaisePropertyChanged(() => ResultIndicator);
                                    RaisePropertyChanged(() => EntitiesCount);

                                    RaisePropertyChanged(() => SelectedEntity);
                                    RaisePropertyChanged(() => this.PreviousEntity);
                                    RaisePropertyChanged(() => this.PreviousUri);
                                    RaisePropertyChanged(() => this.NextEntity);
                                    RaisePropertyChanged(() => this.NextUri);

                                    this.NotifyError = false;
                                }
                                else
                                {
                                    isBusy = false;
                                    if (this.SelectedEntity == null || this.SelectedEntity.HasValidationErrors || this.SelectedEntity.HasChildErrors())
                                    {
                                        this.NotifyError = true;
                                    }
                                    if (this.OnSaveError != null)
                                    {
                                        this.OnSaveError(this, new ErrorEventArgs() { Error = error });
                                    }
                                }

                            });
                        }
                        catch (ValidationException)
                        {
                            //ErrorWindow.CreateNew(ve.Message);
                            this.NotifyError = true;
                            IsBusy = false;
                        }
                    }
                }
                else
                {
                    IsBusy = false;

                    if (forceSave)
                    {
                        if (this.SelectedEntity == null || this.SelectedEntity.HasValidationErrors || this.SelectedEntity.HasChildErrors())
                            this.NotifyError = true;

                        if (this.OnSaveError != null)
                            this.OnSaveError(this, new ErrorEventArgs());
                    }
                    foreach (ValidationResult err in errors)
                    {
                        SelectedEntity.ValidationErrors.Add(err);
                    }
                    this.NotifyError = errors.Any() || HasFormatValidationError;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Cancel()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            bool navigateToDefaultPage = false;
            if (this.SelectedEntity != null && this.SelectedEntity.IsNew() && !Application.Current.IsRunningOutOfBrowser)
            {
                this.service.Delete(SelectedEntity);
                this.SelectedEntity = null;
                navigateToDefaultPage = IsAutoNavigateToFirst;

                CanDelete = false;
                CanEdit = false;

                this.DeleteCommand.RaiseCanExecuteChanged();
                this.EditCommand.RaiseCanExecuteChanged();

            }

            service.RejectChanges();
            if (Application.Current.IsRunningOutOfBrowser)
            {
                ReloadAllServices();
            }

            this.IsEditMode = false;
            if (navigateToDefaultPage)
            {
                // accéder à une page par défaut
                NavigationService.NavigateToPreviousView();
            }

            RaisePropertyChanged(() => SelectedEntity);
            RaisePropertyChanged(() => Entities);

            if (OnCanceled != null)
            {
                OnCanceled(this, null);
            }

            NotifyError = false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Delete()
        {
            Delete(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skipNavigation"></param>
        protected virtual void Delete(bool skipNavigation)
        {
            Delete(skipNavigation, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skipNavigation"></param>
        protected virtual void Delete(bool skipNavigation, bool skipConfirmation)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (!UserCanDelete)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else
            {
                if (SelectedEntity != null)
                {

                    var result = MessageBoxResult.None;

                    if (!skipConfirmation)
                    {
                        String ConfirmMsg = Resource.ResourceManager.GetString(typeof(T).Name + "_DeleteConfirmation");

                        if (ConfirmMsg == null)
                        {
                            ConfirmMsg = Resource.BaseProtecaEntityViewModel_DeleteConfirmation;
                        }

                        result = MessageBox.Show(ConfirmMsg, "", MessageBoxButton.OKCancel);
                    }

                    if (skipConfirmation || result == MessageBoxResult.OK)
                    {
                        IsBusy = true;
                        int previousId = 0;
                        if (PreviousEntity != null && !skipNavigation)
                        {
                            previousId = (int)PreviousEntity.GetCustomIdentity();
                        }

                        this.service.Delete(SelectedEntity);
                        this.service.SaveChanges((error) =>
                        {
                            IsBusy = false;
                            if (error == null)
                            {
                                MessageBox.Show(Resource.BaseProtecaEntityViewModel_DeleteSuccess, "", MessageBoxButton.OK);

                                if (this.OnDeleteSuccess != null)
                                {
                                    this.OnDeleteSuccess(this, null);
                                }

                                RaisePropertyChanged(() => Entities);
                                RaisePropertyChanged(() => SelectedEntity);
                                RaisePropertyChanged(() => ResultIndicator);

                                if (!skipNavigation)
                                {
                                    if (Entities != null && Entities.Any())
                                    {
                                        if (previousId != 0)
                                        {
                                            NavigationService.Navigate(previousId);
                                        }
                                        else
                                        {
                                            NavigationService.Navigate((int)Entities.First().GetCustomIdentity());
                                        }
                                    }
                                    else
                                    {
                                        SelectedEntity = null;
                                        RaisePropertyChanged(() => SelectedEntity);
                                        NavigationService.NavigateRootUrl();
                                    }
                                }
                            }
                            else
                            {
                                if (this.OnDeleteError != null)
                                {
                                    this.OnDeleteError(this, new ErrorEventArgs() { Error = error });
                                }
                                else
                                {
                                    ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_DeleteError, typeof(T).Name));
                                }
                            }

                        });
                    }
                }
                else
                {
                    ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_DeleteError, typeof(T).Name));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Add()
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
                if (!this.Entities.Contains(entity))
                {
                    this.Entities.Add(entity);
                }
                this.IsEditMode = true;
            }

            if (OnAddedEntity != null)
            {
                OnAddedEntity(this, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Edit()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (!UserCanEdit)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else
            {
                this.IsEditMode = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Select()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ((INavigationService)service).Navigate((int)SelectedEntity.GetCustomIdentity());
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Find()
        {
            saveGeoPreferences();
            service.FindEntities(Filtres, (error) => FindEntitiesLoaded(error));
        }

        /// <summary>
        /// Initialise les critères géographiques
        /// </summary>
        protected virtual void initGeoPreferences()
        {
            if (userService.CurrentUser != null)
            {
                FiltreCleRegion = userService.CurrentUser.PreferenceCleRegion;
                FiltreCleAgence = userService.CurrentUser.PreferenceCleAgence;
                FiltreCleSecteur = userService.CurrentUser.PreferenceCleSecteur;
            }
        }

        /// <summary>
        /// Sauvegarde les critères géographiques
        /// </summary>
        protected virtual void saveGeoPreferences()
        {
            if (userService.CurrentUser != null)
            {
                userService.CurrentUser.SetPreferenceCleRegion(FiltreCleRegion);
                userService.CurrentUser.SetPreferenceCleAgence(FiltreCleAgence);
                userService.CurrentUser.SetPreferenceCleSecteur(FiltreCleSecteur);
                userService.CurrentUser.SetPreferenceCleEnsembleElectrique(null);
                userService.CurrentUser.SetPreferenceClePortion(null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void LoadEntities()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // Flip busy flag
            IsBusy = true;

            // Load Entities
            service.GetEntities((error) => EntitiesLoaded(error));
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void LoadDetailEntity()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // Flip busy flag
            IsBusy = true;

            // Load Detail Entity
            service.GetEntityByCle(SelectedId.Value, (error) => DetailEntityLoaded(error));
        }

        /// <summary>
        /// Affecte les droits de l'utilisateurs
        /// </summary>
        protected virtual void SetCanProperties()
        {
            CanCancel = CanCancel && !IsBusy;
            CanSave = CanSave && !IsBusy;
            CanAdd = !IsBusy;
            CanDelete = !IsBusy && SelectedEntity != null;
            CanEdit = true;
            CanFind = true;
            CanExportExcel = true;
            CanExportPDF = true;
            CanPrintGrid = true;
            this.AddCommand.RaiseCanExecuteChanged();
            this.DeleteCommand.RaiseCanExecuteChanged();
            this.SaveCommand.RaiseCanExecuteChanged();
            this.EditCommand.RaiseCanExecuteChanged();
            this.CancelCommand.RaiseCanExecuteChanged();
            this.FindCommand.RaiseCanExecuteChanged();
        }

        protected override void DeactivateView(string viewName)
        {
            IsActive = false;
            base.DeactivateView(viewName);

            //if (NavigationService.CurrentView != viewName)
            {
                AllServicesLoaded = false;
                this.SelectedEntity = null;
                MainTileItemState = Telerik.Windows.Controls.TileViewItemState.Maximized;

                var properties = this.GetType().GetProperties();
                // Pour chaque service de type IEntityService<>
                foreach (var prop in properties.Where(p => p.PropertyType.IsGenericType && (p.PropertyType.GetGenericTypeDefinition() == typeof(IEntityService<>))))
                {
                    var serv = prop.GetValue(this, null);
                    if (serv != null)
                    {
                        var clearMethod = serv.GetType().GetMethod("Clear");
                        if (clearMethod != null)
                        {
                            clearMethod.Invoke(serv, null);
                        }
                    }
                }

                RaisePropertyChanged(() => this.ResultIndicator);
                RaisePropertyChanged(() => this.EntitiesCount);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName,
          IDictionary<string, object> viewParameters)
        {
            ((UserService)userService).GetEntities((completed) =>
            {
                continueLoad(viewName, viewParameters);
            });
        }


        void continueLoad(string viewName,
          IDictionary<string, object> viewParameters)
        {

            IsActive = true;

            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (!this.UserCanRead)
            {
                //ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_NotAllowed);
                // Naviguer vers la page d'erreur
                EventAggregator.Publish(new Exception("Vous ne possédez pas les droits nécessaires pour accéder à cette page."));
            }
            else
            {
                if (!viewParameters.Any(p => p.Key == "IsExpanderLoaded") && !AllServicesLoaded) // Evite de chager deux fois tous les services pour lorsqu'il y a un expander associé à un même viewmodel
                {
                    if (Application.Current.IsRunningOutOfBrowser)
                    {
                        ReloadAllServices();
                    }
                    else
                        LoadAllServices();
                }

                if (viewParameters.ContainsKey(Global.Constants.PARM_ID))
                {
                    // Lorsque l'on change d'entité sélectionnée,
                    // on annule les changements qui ont pu être fait par ailleurs
                    // Cela assure que lorsque l'on sauvegarde,
                    // on ne sauvegarde que les modifications de la page courante
                    service.RejectChanges();
                    if (Application.Current.IsRunningOutOfBrowser)
                    {
                        ReloadAllServices();
                    }


                    _selectedId = viewParameters.ParameterValue<int>(Global.Constants.PARM_ID);
                    RaisePropertyChanged(() => this.SelectedId);

                    if (SelectedId.HasValue)
                    {
                        if (AllServicesLoaded)
                        {
                            this.LoadDetailEntity();
                        }
                        else
                        {
                            int storeSelectedId = SelectedId.Value;
                            EventHandler onAllServicesLoaded = null;
                            onAllServicesLoaded = (o, e) =>
                            {
                                if (!SelectedId.HasValue)
                                {
                                    _selectedId = storeSelectedId;
                                }
                                this.LoadDetailEntity();
                                OnAllServicesLoaded -= onAllServicesLoaded;
                            };
                            OnAllServicesLoaded += onAllServicesLoaded;
                        }
                    }
                }
                else if (viewParameters.ContainsKey(Global.Constants.PARM_STATE) && viewParameters[Global.Constants.PARM_STATE].ToString() == Global.Constants.STATE_NEW)
                {
                    // Lorsque l'on change d'entité sélectionnée,
                    // on annule les changements qui ont pu être fait par ailleurs
                    // Cela assure que lorsque l'on sauvegarde,
                    // on ne sauvegarde que les modifications de la page courante
                    service.RejectChanges();
                    if (Application.Current.IsRunningOutOfBrowser)
                    {
                        ReloadAllServices();
                    }

                    this.Add();
                }
                else if (IsEditMode && IsAutoAddOnEditMode)
                {
                    // Lorsque l'on change d'entité sélectionnée,
                    // on annule les changements qui ont pu être fait par ailleurs
                    // Cela assure que lorsque l'on sauvegarde,
                    // on ne sauvegarde que les modifications de la page courante
                    service.RejectChanges();
                    if (Application.Current.IsRunningOutOfBrowser)
                    {
                        ReloadAllServices();
                    }

                    this.Add();
                }
                else if (IsAutoNavigateToFirst && viewParameters.Count == 0 && Entities != null && Entities.Any())
                {
                    NavigateToId((int)Entities.First().GetCustomIdentity());
                }

                if (OnViewActivated != null)
                {
                    this.OnViewActivated(this, new ActivateViewEventArgs() { ViewParameter = viewParameters });
                }
            }
        }

        protected void RaiseOnAllServicesLoadedEvent()
        {
            if (this.OnAllServicesLoaded != null)
            {
                OnAllServicesLoaded(this, null);
            }
        }

        //private int servicesLoadedCount = 0;
        //private String lockObject = String.Empty;

        /// <summary>
        ///  Charge la liste de toutes les entitées
        /// </summary>
        /// <returns></returns>
        private void EnsureAllNeededServicesLoaded(Boolean isReloadMode = true)
        {
            IsBusy = true;
            userService.GetEntities((err) =>
            {
                if (err != null)
                {
                    Logger.Log(LogSeverity.Error, GetType().FullName, err);
                    ErrorWindow.CreateNew(Resource.Error_UserNotFound);
                }
                else
                {
                    EntityServiceHelper.LoadAllServicesAsync(
                        this,
                        (svc, error) =>
                        {
                            if (error != null)
                                Logger.Log(LogSeverity.Error, GetType().FullName, error);
                        }, () =>
                        {
                            AllServicesLoaded = true;
                            IsBusy = false;
                            if (OnAllServicesLoaded != null)
                                OnAllServicesLoaded(this, null);
                            if (isReloadMode)
                                initGeoPreferences();
                            else
                                saveGeoPreferences();
                        });
                }
            });
        }

        /// <summary>
        ///  Charge la liste de toutes les entitées de manière synchrone pour le module déporté
        /// </summary>
        /// <returns></returns>
        private void EnsureAllNeededServicesLoadedSync(Boolean isReloadMode = true)
        {
            IsBusy = true;
            userService.GetEntities((err) =>
            {
                if (err != null)
                {
                    Logger.Log(LogSeverity.Error, GetType().FullName, err);
                    ErrorWindow.CreateNew(Resource.Error_UserNotFound);
                }
                else
                {
                    EntityServiceHelper.LoadAllServicesAsync(
                        this,
                        (svc, error) =>
                        {
                            if (error != null)
                                Logger.Log(LogSeverity.Error, GetType().FullName, error);
                        }, () =>
                        {
                            AllServicesLoaded = true;
                            IsBusy = false;
                            if (OnAllServicesLoadedSync != null)
                                OnAllServicesLoadedSync(this, null);
                            if (isReloadMode)
                                initGeoPreferences();
                            else
                                saveGeoPreferences();
                        });
                }
            });
        }

        #endregion

        #endregion

        #region Completion Callbacks

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        private void EntitiesLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(T).Name));
            }
            // If no error is returned, set the model to entities
            else
            {
                if (this.Entities != null && this.Entities.Any())
                {
                    if (SelectedId.HasValue && this.Entities.Any(a => (int)a.GetCustomIdentity() == SelectedId.Value))
                    {
                        this.SelectedEntity = this.Entities.Where(a => (int)a.GetCustomIdentity() == SelectedId.Value).First();
                    }
                    else if (IsAutoNavigateToFirst)
                    {
                        NavigateToId((int)Entities.First().GetCustomIdentity());
                    }
                }
            }

            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => this.ResultIndicator);
            RaisePropertyChanged(() => this.EntitiesCount);
            RaisePropertyChanged(() => this.PreviousEntity);
            RaisePropertyChanged(() => this.PreviousUri);
            RaisePropertyChanged(() => this.NextEntity);
            RaisePropertyChanged(() => this.NextUri);

            // We're done
            IsBusy = false;

            if (OnEntitiesLoaded != null)
            {
                OnEntitiesLoaded(this, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        protected void DetailEntityLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(T).Name));
            }
            else
            {
                if (service.DetailEntity != null)
                {
                    SelectedEntity = service.DetailEntity;
                    if (IsEditMode)
                    {
                        this.Edit();
                    }
                }
            }

            // We're done
            IsBusy = false;

            if (OnDetailLoaded != null)
            {
                OnDetailLoaded(this, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        private void FindEntitiesLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(T).Name));
            }
            // If no error is returned, set the model to entities
            else
            {
                if (IsAutoNavigateToFirst && this.Entities != null && this.Entities.Any())
                {
                    NavigateToId((int)Entities.First().GetCustomIdentity());
                }
                else if (this.Entities == null || !this.Entities.Any())
                {
                    this.SelectedEntity = null;
                    NavigationService.NavigateRootUrl();
                }
            }

            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => this.ResultIndicator);
            RaisePropertyChanged(() => this.EntitiesCount);

            if (OnFindLoaded != null)
            {
                OnFindLoaded(this, null);
            }

            // We're done
            IsBusy = false;
        }

        protected virtual void NavigateToId(int id, bool forceReload = false)
        {
            NavigationService.Navigate(id, forceReload);
        }

        #endregion

    }
}
