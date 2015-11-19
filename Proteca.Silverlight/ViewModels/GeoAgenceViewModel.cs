using System;
using System.Collections.ObjectModel;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Helpers;
using Proteca.Web.Models;
using Jounce.Framework.Command;
using System.Windows;
using System.Linq;
using Proteca.Silverlight.Services.EntityServices;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for GeoAgence entity
    /// </summary>
    [ExportAsViewModel("GeoAgence")]
    public class GeoAgenceViewModel : BaseProtecaEntityViewModel<GeoAgence>
    {
        #region Properties

        private bool IsDeleting = false;

        #endregion Properties

        #region Command

        /// <summary>
        /// Commande pour ajouter un secteur à l'agence courante
        /// </summary>
        public IActionCommand AddSecteurCommand { get; private set; }

        #endregion Command

        #region Services

        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur GeoAgenceViewModel
        /// </summary>
        public GeoAgenceViewModel()
            : base()
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
                if (SelectedEntity != null && !SelectedEntity.IsNew() && !IsDeleting)
                {
                    EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("SelectedItem", this.SelectedEntity));
                }
            };

            this.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "SelectedEntity")
                {
                    EventAggregator.Publish<ObservableCollection<GeoAgence>>(this.Entities);
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("RegionLoaded", true));
            };
        }

        #endregion Constructor

        #region Protected Methods

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
                GeoRegionViewModel regionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
                regionVM.AllServicesLoaded = false;
                GeoSecteurViewModel secteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
                secteurVM.AllServicesLoaded = false;
                DecoupageGeoViewModel DecoupageGeoVM = Router.ResolveViewModel<DecoupageGeoViewModel>(false, "DecoupageGeo");
                DecoupageGeoVM.Deactivate("DecoupageGeo");
                TreeViewGeoViewModel TreeViewVM = Router.ResolveViewModel<TreeViewGeoViewModel>(false, "TreeViewGeo");
                TreeViewVM.Deactivate("TreeViewGeo");
            }
        }

        /// <summary>
        /// Ajout de la nouvelle agence et association avec la region sélectionnée dans le treeview
        /// </summary>
        protected override void Add()
        {
            TreeViewGeoViewModel treeViewGeoVM = Router.ResolveViewModel<TreeViewGeoViewModel>(false, "TreeViewGeo");
            GeoRegion region = ((GeoRegion)treeViewGeoVM.SelectedItem);

            base.Add();

            region.GeoAgence.Add(this.SelectedEntity);
        }

        /// <summary>
        /// Suppression d'une agence
        /// </summary>
        protected override void Delete()
        {
            MessageBoxResult result = MessageBox.Show(Resource.DecoupageGeo_DeleteAgenceConfirmation,
                Resource.DecoupageGeo_DeleteAgenceCaptionMsgError, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                IsBusy = true;
                ((GeoAgenceService)this.service).CheckAndDeleteAgenceByCle(this.SelectedEntity.CleAgence, CheckAndDeleteDone);
            }
        }

        /// <summary>
        /// Supprime la dernière vue visitée si il s'agissait d'un ajout
        /// </summary>
        protected override void Cancel()
        {
            int cleRegion = 0;
            if (this.SelectedEntity.IsNew())
            {
                cleRegion = this.SelectedEntity.CleRegion;
            }            
            base.Cancel();

            if (cleRegion > 0)
            {
                NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                   MainNavigation.Administration.GetStringValue(),
                   AdministrationNavigation.DecoupageGeo.GetStringValue(),
                   Adm_DecoupageGeoNavigation.GeoRegion.GetStringValue(), cleRegion),
                   UriKind.Relative));
            }
        }

        /// <summary>
        /// Récupération des droits du viewmodel Secteur pour initialiser 
        /// la commande d'ajout de secteur.
        /// </summary>
        protected override void InitializeVm()
        {
            GeoSecteurViewModel geoSecteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");

            geoSecteurVM.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "CanAdd")
                {
                    AddSecteurCommand.RaiseCanExecuteChanged();
                }
            };

            this.AddSecteurCommand = new ActionCommand<object>(
                obj => AddSecteur(), obj => geoSecteurVM.CanAdd);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Ajout d'un secteur à l'agence courante
        /// </summary>
        private void AddSecteur()
        {
            GeoSecteurViewModel GeoSecteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
            this.IsActive = false;
            GeoSecteurVM.IsEditMode = true;

            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}",
               MainNavigation.Administration.GetStringValue(),
               AdministrationNavigation.DecoupageGeo.GetStringValue(),
               Adm_DecoupageGeoNavigation.GeoSecteur.GetStringValue()),
               UriKind.Relative));
        }

        /// <summary>
        /// Retour de la validation et de la suppression d'une agence
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
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(MesModeleMesureViewModel).Name));
            }
            else if (!string.IsNullOrEmpty(text))
            {
                //Les règles de gestion doivent être validées avant de supprimer l'agence
                MessageBox.Show(text, Resources.Resource.DecoupageGeo_DeleteAgenceCaptionMsgError, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(Resource.BaseProtecaEntityViewModel_DeleteSuccess, string.Empty, MessageBoxButton.OK);

                GeoRegion region = SelectedEntity.GeoRegion;

                this.service.Clear();

                // Rechargement des régions
                GeoRegionViewModel regionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
                regionVM.service.GetEntities(e =>
                    {
                        // Rechargement des secteurs
                        GeoSecteurViewModel secteurVM = Router.ResolveViewModel<GeoSecteurViewModel>(false, "GeoSecteur");
                        secteurVM.service.GetEntities(null);

                        // Rechargement des Agences
                        this.service.GetEntities(c =>
                        {
                            EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("RefreshTree", true));
                            EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("SelectedItem", region));

                            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                          MainNavigation.Administration.GetStringValue(),
                          AdministrationNavigation.DecoupageGeo.GetStringValue(),
                          Adm_DecoupageGeoNavigation.GeoRegion.GetStringValue(),
                          Global.Constants.PARM_ID,
                          ((GeoRegion)region).CleRegion), UriKind.Relative));
                        });
                    }
                );
            }
        }

       #endregion Private Methods

    }
}
