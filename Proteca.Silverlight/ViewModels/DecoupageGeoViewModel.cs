using System;
using System.Collections.ObjectModel;
using System.Windows;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Helpers;
using Proteca.Web.Models;
using System.Linq;
using Proteca.Silverlight.Services.EntityServices;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using Jounce.Core.Event;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for DecoupageGeo View
    /// </summary>
    [ExportAsViewModel("DecoupageGeo")]
    public class DecoupageGeoViewModel : BaseViewModel, IPartImportsSatisfiedNotification, IEventSink<ViewMode>
    {

        #region Services

        [Import]
        public IEntityService<GeoRegion> service { get; set; }

        /// <summary>
        /// Service utilisé pour naviguer dans l'application
        /// </summary>
        [Import(typeof(INavigationService))]
        public INavigationService NavigationService { get; set; }

        #endregion

        #region Event

        public EventHandler OnAllServicesLoaded;
        #endregion

        #region Properties

        public Boolean AllServicesLoaded { get; set; }

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

                    // Publish automatique si la valeur change
                    if (value != lastEditMode)
                    {
                        EventAggregator.Publish(value ? ViewMode.EditMode : ViewMode.NavigationMode);
                    }
                }

                RaisePropertyChanged(() => this.IsEditMode);
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisePropertyChanged(() => this.IsBusy);
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Commande pour ajouter un secteur à l'agence courante
        /// </summary>
        public IActionCommand AddRegionCommand { get; private set; }
        
        #endregion Command

        #region Constructor

        /// <summary>
        /// Constructeur GeoRegionViewModel
        /// </summary>
        public DecoupageGeoViewModel()
            : base()
        {
            this.OnAllServicesLoaded += (o, e) =>
            {
                EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("isFromEntitee", true));
            };
        }

        #endregion Constructor

        #region public Method

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            // Load all entities
            //this.LoadRegions();

            EventAggregator.Subscribe<ViewMode>(this);
        }

        public void HandleEvent(ViewMode publishedEvent)
        {
            switch (publishedEvent)
            {
                case ViewMode.NavigationMode:
                    IsEditMode = false;
                    break;
                case ViewMode.EditMode:
                    IsEditMode = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Chargements des regions
        /// </summary>
        public void LoadRegions()
        {
            // Load Regions
            service.GetEntities((error) => RegionsLoaded(error));

            // Flip busy flag
            IsBusy = true;
        }

        #endregion

        #region Completion Callbacks

        private void RegionsLoaded(Exception error)
        {
            if (error != null)
            {
                ErrorWindow.CreateNew(error);
            }

            AllServicesLoaded = true;
            if (OnAllServicesLoaded != null)
            {
                OnAllServicesLoaded(this, null);
            }
            // We're done
            IsBusy = false;
        }

        #endregion

        #region Protected Method

        protected override void DeactivateView(string viewName)
        {
            if (NavigationService.CurrentView != "GeoRegion" && NavigationService.CurrentView != "GeoAgence" && NavigationService.CurrentView != "GeoSecteur" && NavigationService.CurrentView != "DecoupageGeo")
            {
                base.DeactivateView(viewName);
                this.service.Clear();
                this.AllServicesLoaded = false;
                GeoRegionViewModel regionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
                regionVM.AllServicesLoaded = false;
                GeoAgenceViewModel agenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
                agenceVM.AllServicesLoaded = false;
                GeoSecteurViewModel secteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
                secteurVM.AllServicesLoaded = false;
                TreeViewGeoViewModel TreeViewVM = Router.ResolveViewModel<TreeViewGeoViewModel>(false, "TreeViewGeo");
                TreeViewVM.Deactivate("TreeViewGeo");
            }
        }

        /// <summary>
        /// Récupération des droits du viewmodel Region pour initialiser 
        /// la commande d'ajout de région.
        /// </summary>
        protected override void InitializeVm()
        {
            base.InitializeVm();

            GeoRegionViewModel geoRegionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");

            geoRegionVM.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "CanAdd")
                {
                    AddRegionCommand.RaiseCanExecuteChanged();
                }
            };

            AddRegionCommand = new ActionCommand<object>(
                obj => AddRegion(), obj => geoRegionVM.CanAdd);
        }

        protected override void ActivateView(string viewName, System.Collections.Generic.IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);

            EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Sélection géographique").AddNamedParameter<Double>("MaxWidth", 500));
            //EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("isFromEntitee", true));

            if (!AllServicesLoaded)
            {
                LoadRegions();
            }
        }
        
        #endregion Protected Method

        #region Private Methods

        /// <summary>
        /// Ajout d'une région
        /// </summary>
        private void AddRegion()
        {
            GeoRegionViewModel GeoRegionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
            GeoRegionVM.IsEditMode = true;
            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}",
               MainNavigation.Administration.GetStringValue(),
               AdministrationNavigation.DecoupageGeo.GetStringValue(),
               Adm_DecoupageGeoNavigation.GeoRegion.GetStringValue()),
               UriKind.Relative));
        }

        #endregion Private Methods

        #region Autorisations

        /// <summary>
        /// Service utilisé pour gérer l'utilisateur connecté
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> userService { get; set; }

        /// <summary>
        /// Retourne si true l'utilisateur est un administrateur
        /// </summary>
        /// <returns></returns>
        protected bool GetUserCanRead()
        {
            bool result = false;
            if (this.userService != null && this.userService.CurrentUser != null)
            {
                result = this.userService.CurrentUser.IsAdministrateur;
            }
            return result;
        }

        #endregion
    }
}
