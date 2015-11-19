using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Models;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using System.Collections.Generic;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Services.EntityServices;
using System.ServiceModel.DomainServices.Client;

namespace Proteca.Silverlight.ViewModels
{
    [ExportAsViewModel("TreeViewGeo")]
    public class TreeViewGeoViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        #region Private Member

        private bool IsFirstLoad = false;

        #endregion Private Member

        #region Services

        [Import]
        public IEntityService<GeoRegion> service { get; set; }

        [Import(typeof(INavigationService))]
        public INavigationService NavService { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'utilisateur connecté
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> userService { get; set; }

        #endregion

        #region Event

        public EventHandler OnAllServicesLoaded;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public TreeViewGeoViewModel()
        {
            AddCommand = new ActionCommand<object>(
                obj => AddTreeItem(), obj => CanAdd);
            DeleteCommand = new ActionCommand<object>(
                obj => DeleteTreeItem(), obj => CanDelete);
            EditCommand = new ActionCommand<object>(
                obj => UpdateTreeItem(), obj => CanUpdate);
        }

        #endregion

        #region Properties

        public Boolean AllServicesLoaded { get; set; }

        private bool canAdd = true;
        public bool CanAdd
        {
            get { return canAdd; }
            set
            {
                canAdd = value;
                RaisePropertyChanged(() => this.CanAdd);
            }
        }

        private bool canDelete = true;
        public bool CanDelete
        {
            get { return canDelete; }
            set
            {
                canDelete = value;
                RaisePropertyChanged(() => this.CanDelete);
            }
        }

        private bool canUpdate = true;
        public bool CanUpdate
        {
            get { return canUpdate; }
            set
            {
                canUpdate = value;
                RaisePropertyChanged(() => this.CanUpdate);
            }
        }

        public List<GeoRegion> Regions
        {
            get { return this.service.Entities.OrderBy(r => r.LibelleRegion).ToList(); }
        }

        private ObservableCollection<Entitee> treeViewGeo = null;
        public ObservableCollection<Entitee> TreeViewGeo
        {
            set
            {
                treeViewGeo = value;
                RaisePropertyChanged(() => this.TreeViewGeo);
            }
            get
            {
                if (treeViewGeo == null)
                {
                    Entitee ent = new Entitee();
                    ent.Libelle = "GRTgaz";
                    ent.Regions = Regions;
                    treeViewGeo = new ObservableCollection<Entitee>() { ent };
                }
                else if (treeViewGeo[0].Regions == null || !treeViewGeo[0].Regions.Any())
                {
                    Entitee ent = treeViewGeo[0];
                    ent.Regions = Regions;
                    treeViewGeo[0] = ent;
                    selectItem(_selectedItem);
                }

                return treeViewGeo;
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

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value && value != null && _selectedItem != null && IsFirstLoad == false)
                {
                    if (value is GeoRegion)
                    {
                        NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                            MainNavigation.Administration.GetStringValue(),
                            AdministrationNavigation.DecoupageGeo.GetStringValue(),
                            Adm_DecoupageGeoNavigation.GeoRegion.GetStringValue(),
                            Global.Constants.PARM_ID,
                            ((GeoRegion)value).CleRegion), UriKind.Relative));
                    }
                    else if (value is GeoAgence)
                    {
                        NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                           MainNavigation.Administration.GetStringValue(),
                           AdministrationNavigation.DecoupageGeo.GetStringValue(),
                           Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue(),
                           Global.Constants.PARM_ID,
                           ((GeoAgence)value).CleAgence), UriKind.Relative));
                    }
                    else if (value is GeoSecteur)
                    {
                        NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                           MainNavigation.Administration.GetStringValue(),
                           AdministrationNavigation.DecoupageGeo.GetStringValue(),
                           Adm_DecoupageGeoNavigation.GeoSecteur.GetStringValue(),
                           Global.Constants.PARM_ID,
                           ((GeoSecteur)value).CleSecteur), UriKind.Relative));
                    }
                    else if (value is Entitee)
                    {
                        NavService.NavigateUri(new Uri(string.Format("/{0}/{1}",
                           MainNavigation.Administration.GetStringValue(),
                           AdministrationNavigation.DecoupageGeo.GetStringValue()), UriKind.Relative));
                    }
                }

                if (_selectedItem != null && _selectedItem is IGeoCommun)
                {
                    ((IGeoCommun)_selectedItem).IsSelected = false;
                }
                _selectedItem = value;

                if (_selectedItem != null && _selectedItem is IGeoCommun)
                {
                    ((IGeoCommun)_selectedItem).IsSelected = true;
                }

                if (treeViewGeo != null)
                {
                    treeViewGeo[0].IsExpanded = true;
                }
                if (_selectedItem is GeoSecteur && ((GeoSecteur)_selectedItem).EntityState != EntityState.Detached)
                {
                    ((GeoSecteur)_selectedItem).GeoAgence.GeoRegion.IsExpanded = true;
                    ((GeoSecteur)_selectedItem).GeoAgence.IsExpanded = true;
                }
                else if (_selectedItem is GeoAgence && ((GeoAgence)_selectedItem).EntityState != EntityState.Detached)
                {
                    ((GeoAgence)_selectedItem).GeoRegion.IsExpanded = true;
                }
                if (_selectedItem != null)
                    ((IGeoCommun)_selectedItem).IsExpanded = true;

                RaisePropertyChanged("SelectedItem");
            }
        }

        #endregion

        #region Commands

        public IActionCommand DeleteCommand { get; private set; }
        public IActionCommand EditCommand { get; private set; }
        public IActionCommand AddCommand { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            // Load all entities
            //this.LoadRegions();
            //EventAggregator.Subscribe<ObservableCollection<Region>>(this);

            // Chargement des ViewModels Géo non créé
            Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
            Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
            Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
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

        /// <summary>
        /// Ajout d'un éléments à partir du treeview à l'aide du menu contextuel
        /// </summary>
        public void AddTreeItem()
        {
            if (SelectedItem is Entitee)
            {
                GeoRegionViewModel GeoRegionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
                if (GeoRegionVM.IsActive)
                {
                    EventAggregator.Publish("GeoRegion".AsViewNavigationArgs().AddNamedParameter(Global.Constants.PARM_STATE, Global.Constants.STATE_NEW));
                }
                else
                {
                    GeoRegionVM.IsEditMode = true;
                    NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}",
                       MainNavigation.Administration.GetStringValue(),
                       AdministrationNavigation.DecoupageGeo.GetStringValue(),
                       Adm_DecoupageGeoNavigation.GeoRegion.GetStringValue()),
                       UriKind.Relative));
                }
            }
            else if (SelectedItem is GeoRegion)
            {
                GeoAgenceViewModel GeoAgenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
                if (GeoAgenceVM.IsActive)
                {
                    EventAggregator.Publish("GeoAgence".AsViewNavigationArgs().AddNamedParameter(Global.Constants.PARM_STATE, Global.Constants.STATE_NEW));
                }
                else
                {
                    GeoAgenceVM.IsEditMode = true;
                    NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}",
                       MainNavigation.Administration.GetStringValue(),
                       AdministrationNavigation.DecoupageGeo.GetStringValue(),
                       Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue()),
                       UriKind.Relative));
                }
            }
            else if (SelectedItem is GeoAgence)
            {
                GeoSecteurViewModel GeoSecteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
                if (GeoSecteurVM.IsActive)
                {
                    EventAggregator.Publish("GeoSecteur".AsViewNavigationArgs().AddNamedParameter(Global.Constants.PARM_STATE, Global.Constants.STATE_NEW));
                }
                else
                {
                    GeoSecteurVM.IsEditMode = true;
                    NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}",
                       MainNavigation.Administration.GetStringValue(),
                       AdministrationNavigation.DecoupageGeo.GetStringValue(),
                       Adm_DecoupageGeoNavigation.GeoSecteur.GetStringValue()),
                       UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// Ajout d'un éléments à partir du treeview à l'aide du menu contextuel
        /// </summary>
        public void DeleteTreeItem()
        {
            if (SelectedItem is GeoRegion)
            {
                GeoRegionViewModel regionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
                regionVM.SelectedEntity = this.SelectedItem as GeoRegion;

                regionVM.DeleteCommand.Execute(null);
            }
            else if (SelectedItem is GeoAgence)
            {
                GeoAgenceViewModel agenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
                agenceVM.SelectedEntity = this.SelectedItem as GeoAgence;

                agenceVM.DeleteCommand.Execute(null);
            }
            else if (SelectedItem is GeoSecteur)
            {
                GeoSecteurViewModel secteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
                secteurVM.SelectedEntity = this.SelectedItem as GeoSecteur;

                secteurVM.DeleteCommand.Execute(null);
            }
        }

        /// <summary>
        /// Modifier un éléments à partir du treeview à l'aide du menu contextuel
        /// </summary>
        public void UpdateTreeItem()
        {
            if (SelectedItem is GeoRegion)
            {
                GeoRegionViewModel GeoRegionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
                GeoRegionVM.IsEditMode = true;

                NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                    MainNavigation.Administration.GetStringValue(),
                    AdministrationNavigation.DecoupageGeo.GetStringValue(),
                    Adm_DecoupageGeoNavigation.GeoRegion.GetStringValue(),
                    Global.Constants.PARM_ID,
                    ((GeoRegion)SelectedItem).CleRegion), UriKind.Relative));
            }
            else if (SelectedItem is GeoAgence)
            {
                GeoAgenceViewModel GeoAgenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
                GeoAgenceVM.IsEditMode = true;

                NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                   MainNavigation.Administration.GetStringValue(),
                   AdministrationNavigation.DecoupageGeo.GetStringValue(),
                   Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue(),
                   Global.Constants.PARM_ID,
                   ((GeoAgence)SelectedItem).CleAgence), UriKind.Relative));
            }
            else if (SelectedItem is GeoSecteur)
            {
                GeoSecteurViewModel GeoSecteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
                GeoSecteurVM.IsEditMode = true;

                NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                   MainNavigation.Administration.GetStringValue(),
                   AdministrationNavigation.DecoupageGeo.GetStringValue(),
                   Adm_DecoupageGeoNavigation.GeoSecteur.GetStringValue(),
                   Global.Constants.PARM_ID,
                   ((GeoSecteur)SelectedItem).CleSecteur), UriKind.Relative));
            }
        }

        protected override void DeactivateView(string viewName)
        {
            AllServicesLoaded = false;
            this._selectedItem = null;
            ((GeoRegionService)this.service).Clear();
            TreeViewGeo = null;            
            base.DeactivateView(viewName);
            //AllServicesLoaded = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);
            object selectedItemParameter = viewParameters.ContainsKey("SelectedItem") ? viewParameters["SelectedItem"] : null;
            Boolean isFromEntitee = viewParameters.ContainsKey("isFromEntitee") ? (Boolean)viewParameters["isFromEntitee"] : false;
            Boolean isRefreshTree = viewParameters.ContainsKey("RefreshTree") ? (Boolean)viewParameters["RefreshTree"] : false;
            Boolean isRegionLoaded = viewParameters.ContainsKey("RegionLoaded") ? (Boolean)viewParameters["RegionLoaded"] : false;
            

            if (isRefreshTree)
            {
                if (TreeViewGeo[0].Regions != null && TreeViewGeo[0].Regions.Any())
                {
                    TreeViewGeo[0].Regions.Clear();
                }
                this.RaisePropertyChanged(() => this.TreeViewGeo);
            }

            if (isRegionLoaded)
            {
                this.RaisePropertyChanged(() => this.TreeViewGeo);
            }

            if (this.Regions != null && this.Regions.Any(r=>r.EntityState != EntityState.Detached))
            {
                if (!isRefreshTree && !isRegionLoaded && (!isFromEntitee || SelectedItem == null))
                {
                    this.activateView(selectedItemParameter);
                }                
            }
            //else if (isFromEntitee && isRefreshTree)
            //{
            //    EventHandler onAllServicesLoaded = null;
            //    onAllServicesLoaded = (o, e) =>
            //    {
            //        this.activateView(selectedItemParameter);
            //        onAllServicesLoaded -= onAllServicesLoaded;
            //    };
            //    this.OnAllServicesLoaded += onAllServicesLoaded;

            //    this.LoadRegions();
            //}
        }

        private void activateView(object selectedItemParameter)
        {
            IsFirstLoad = true;
            if (TreeViewGeo != null && TreeViewGeo[0].Regions != null && TreeViewGeo[0].Regions.Any() && selectedItemParameter != null)
            {
                selectItem(selectedItemParameter);
            }
            else if (selectedItemParameter != null)
            {
                // Save Selected Item in private Value
                _selectedItem = selectedItemParameter;
            }
            IsFirstLoad = false;

            if (selectedItemParameter == null)
            {
                if (TreeViewGeo != null && TreeViewGeo[0].Regions != null && TreeViewGeo[0].Regions.Any())
                {
                    SelectDefaultUserItem();
                }
            }

            RaisePropertyChanged(() => this.TreeViewGeo);
        }

        /// <summary>
        /// Select TreeView Item from external object
        /// </summary>
        /// <param name="item"></param>
        private void selectItem(object item)
        {
            treeViewGeo[0].Regions = Regions;
            if (item is GeoRegion)
            {                
                if (((GeoRegion)item).CleRegion == 0)
                {
                    SelectedItem = treeViewGeo[0].Regions.First();
                }
                else
                {
                    SelectedItem = treeViewGeo[0].Regions.FirstOrDefault(r => r.CleRegion == ((GeoRegion)item).CleRegion);
                }
            }
            else if (item is GeoAgence)
            {
                GeoRegion region = treeViewGeo[0].Regions.FirstOrDefault(r=> r.CleRegion == ((GeoAgence)item).CleRegion);
                if (region != null)
	            {
                    SelectedItem = region.GeoAgence.FirstOrDefault(a => a.CleAgence == ((GeoAgence)item).CleAgence);
	            }
                    
                //SelectedItem = treeViewGeo[0].Regions.First(r => r.CleRegion == ((GeoAgence)item).CleRegion)
                //    .GeoAgence.First(a => a.CleAgence == ((GeoAgence)item).CleAgence);
            }
            else if (item is GeoSecteur)
            {
                GeoRegion region = treeViewGeo[0].Regions.FirstOrDefault(r => r.GeoAgence.Any(a => a.CleAgence == ((GeoSecteur)item).CleAgence));
                if (region != null)
                {
                    SelectedItem = region.GeoAgence.First(a => a.CleAgence == ((GeoSecteur)item).CleAgence).GeoSecteur.FirstOrDefault(s => s.CleSecteur == ((GeoSecteur)item).CleSecteur);
                }

                //SelectedItem = treeViewGeo[0].Regions.First(r => r.CleRegion == ((GeoSecteur)item).GeoAgence.CleRegion)
                //    .GeoAgence.First(a => a.CleAgence == ((GeoSecteur)item).CleAgence)
                //    .GeoSecteur.First(s => s.CleSecteur == ((GeoSecteur)item).CleSecteur);
            }
            else if (item is Entitee)
            {
                SelectedItem = treeViewGeo[0];
            }
        }

        /// <summary>
        /// Sélectionne par défaut la région, l'agence ou le secteur préféré de l'utilisateur (dernière recherche géographique ou rattachement de l'utilisateur actuellement connecté)
        /// </summary>
        private void SelectDefaultUserItem()
        {
            if (this.userService.CurrentUser != null)
            {
                var region = this.Regions.FirstOrDefault(r => r.CleRegion == this.userService.CurrentUser.PreferenceCleRegion);
                if (region != null)
                {
                    var agence = region.GeoAgence.FirstOrDefault(a => a.CleAgence == this.userService.CurrentUser.PreferenceCleAgence);
                    if (agence != null && this.userService.CurrentUser.PreferenceCleAgence.HasValue)
                    {
                        var secteur = agence.GeoSecteur.FirstOrDefault(s => s.CleSecteur == this.userService.CurrentUser.PreferenceCleSecteur);
                        if (secteur != null)
                        {
                            NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                                MainNavigation.Administration.GetStringValue(),
                                AdministrationNavigation.DecoupageGeo.GetStringValue(),
                                Adm_DecoupageGeoNavigation.GeoSecteur.GetStringValue(),
                                Global.Constants.PARM_ID, secteur.CleSecteur), UriKind.Relative));
                        }
                        else
                        {
                            NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                                MainNavigation.Administration.GetStringValue(),
                                AdministrationNavigation.DecoupageGeo.GetStringValue(),
                                Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue(),
                                Global.Constants.PARM_ID, agence.CleAgence), UriKind.Relative));
                        }
                    }
                    else if (agence != null)
                    {
                        NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                               MainNavigation.Administration.GetStringValue(),
                               AdministrationNavigation.DecoupageGeo.GetStringValue(),
                               Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue(),
                               Global.Constants.PARM_ID, agence.CleAgence), UriKind.Relative));
                    }
                    else
                    {
                        NavService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                               MainNavigation.Administration.GetStringValue(),
                               AdministrationNavigation.DecoupageGeo.GetStringValue(),
                               Adm_DecoupageGeoNavigation.GeoRegion.GetStringValue(),
                               Global.Constants.PARM_ID, region.CleRegion), UriKind.Relative));
                    }
                }
            }
        }

        #endregion

        #region Completion Callbacks

        private void RegionsLoaded(Exception error)
        {
            if (error != null)
            {
                ErrorWindow.CreateNew(error);
            }
            else
            {
                if (this.service != null && this.service.Entities != null)
                {
                    this.service.Entities.CollectionChanged += (o, e) =>
                    {
                        this.RaisePropertyChanged(() => this.TreeViewGeo);
                    };
                }
            }
            AllServicesLoaded = true;
            this.RaisePropertyChanged(() => this.TreeViewGeo);
            if (OnAllServicesLoaded != null)
            {
                OnAllServicesLoaded(this, null);
            }
            // We're done
            IsBusy = false;
        }

        #endregion

        #region Helpers
        #endregion
        
    }
}
