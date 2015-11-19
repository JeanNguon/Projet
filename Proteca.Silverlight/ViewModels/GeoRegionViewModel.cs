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

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for GeoRegion entity
    /// </summary>
    [ExportAsViewModel("GeoRegion")]
    public class GeoRegionViewModel : BaseProtecaEntityViewModel<GeoRegion>
    {
        #region Command

        /// <summary>
        /// Commande pour ajouter un secteur à l'agence courante
        /// </summary>
        public IActionCommand AddAgenceCommand { get; private set; }
        
        #endregion Command

        #region Constructor

        /// <summary>
        /// Constructeur GeoRegionViewModel
        /// </summary>
        public GeoRegionViewModel(): base()
        {
            IsAutoNavigateToFirst = false;

            this.OnViewActivated += (o, e) =>
            {
                EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Sélection géographique").AddNamedParameter<Double>("MaxWidth", 500));
                if (this.SelectedEntity != null && this.SelectedEntity.IsNew())
                {
                    EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("SelectedItem", this.SelectedEntity));
                }
                else if (!e.ViewParameter.ContainsKey(Global.Constants.PARM_ID))
                {
                    EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs());
                }
            };

            this.OnDetailLoaded += (o, e) =>
            {
                if (service.DetailEntity == null)
                {
                    SelectedEntity = null;
                }
                if (this.SelectedEntity != null)
                {
                    EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("SelectedItem", this.SelectedEntity));
                }
            };

            this.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "SelectedEntity")
                {
                    EventAggregator.Publish<ObservableCollection<GeoRegion>>(this.Entities);
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("RegionLoaded", true));
            };

            this.OnCanceled += (o, e) =>
            {
                EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("RefreshTree", true));
            };
        }

        #endregion Constructor

        #region Protected Method

        /// <summary>
        /// Override de la méthode de BaseProtecaViewModel pour désactiver le fonctionnement
        /// Car le fonctionnement est géré au niveau du treeview
        /// </summary>
        /// <param name="viewName"></param>
        protected override void DeactivateView(string viewName)
        {
            if (NavigationService.CurrentView != "GeoRegion" && NavigationService.CurrentView != "GeoAgence" && NavigationService.CurrentView != "GeoSecteur" && NavigationService.CurrentView != "DecoupageGeo")
            {
                base.DeactivateView(viewName);
                GeoAgenceViewModel agenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
                agenceVM.AllServicesLoaded = false;
                GeoSecteurViewModel secteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
                secteurVM.AllServicesLoaded = false;
                DecoupageGeoViewModel DecoupageGeoVM = Router.ResolveViewModel<DecoupageGeoViewModel>(false, "DecoupageGeo");
                DecoupageGeoVM.Deactivate("DecoupageGeo");
                TreeViewGeoViewModel TreeViewVM = Router.ResolveViewModel<TreeViewGeoViewModel>(false, "TreeViewGeo");
                TreeViewVM.Deactivate("TreeViewGeo");
            }
            this.IsActive = false;
        }

        /// <summary>
        /// Récupération des droits du viewmodel Agence pour initialiser 
        /// la commande d'ajout d'agence.
        /// </summary>
        protected override void InitializeVm()
        {
            base.InitializeVm();

            GeoAgenceViewModel geoAgenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");

            geoAgenceVM.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "CanAdd")
                {
                    AddAgenceCommand.RaiseCanExecuteChanged();
                }
            };

            AddAgenceCommand = new ActionCommand<object>(
                obj => AddAgence(), obj => geoAgenceVM.CanAdd);
        }

        /// <summary>
        /// Suppression d'une region
        /// </summary>
        protected override void Delete()
        {
            MessageBoxResult result = MessageBox.Show(Resource.DecoupageGeo_DeleteRegionConfirmation,
                Resource.DecoupageGeo_DeleteRegionCaptionMsg, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                IsBusy = true;
                ((GeoRegionService)this.service).CheckAndDeleteRegionByCle(this.SelectedEntity.CleRegion, CheckAndDeleteDone);
            }
        }

        /// <summary>
        /// Supprime la dernière vue visitée si il s'agissait d'un ajout
        /// </summary>
        protected override void Cancel()
        {
            bool IsNew = this.SelectedEntity.IsNew();
                       
            base.Cancel();

            //if (IsNew)
            //{
            //    NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}",
            //       MainNavigation.Administration.GetStringValue(),
            //       AdministrationNavigation.DecoupageGeo.GetStringValue()),
            //       UriKind.Relative));
            //}
        }

        #endregion Protected Method

        #region Private Methods
        
        /// <summary>
        /// Ajout d'un secteur à l'agence courante
        /// </summary>
        private void AddAgence()
        {
            GeoAgenceViewModel GeoAgenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
            this.IsActive = false;
            GeoAgenceVM.IsEditMode = true;
            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}",
               MainNavigation.Administration.GetStringValue(),
               AdministrationNavigation.DecoupageGeo.GetStringValue(),
               Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue()),
               UriKind.Relative));
        }

        /// <summary>
        /// Retour de la validation et de la suppression d'une region
        /// </summary>
        /// <param name="error"></param>
        /// <param name="text"></param>
        private void CheckAndDeleteDone(Exception error, string text)
        {
            IsBusy = false;
            
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegionViewModel).Name));
            }
            else if (!string.IsNullOrEmpty(text))
            {
                //Les règles de gestion doivent être validées avant de supprimer la région
                MessageBox.Show(text, Resources.Resource.DecoupageGeo_DeleteRegionCaptionMsgError, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(Resource.BaseProtecaEntityViewModel_DeleteSuccess, string.Empty, MessageBoxButton.OK);

                this.service.Clear();

                service.GetEntities(e =>
                    {
                        // Rechargement des Agences
                        GeoAgenceViewModel agenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
                        agenceVM.service.GetEntities(null);

                        // Rechargement des secteurs
                        GeoSecteurViewModel secteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
                        secteurVM.service.GetEntities(null);

                        EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("RefreshTree", true));

                        NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}",
                           MainNavigation.Administration.GetStringValue(),
                           AdministrationNavigation.DecoupageGeo.GetStringValue()),
                           UriKind.Relative));
                    }
                );
            }
        }

        #endregion Private Methods

    }
}
