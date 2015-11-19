using System;
using System.Collections.ObjectModel;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Helpers;
using Proteca.Web.Models;
using System.Windows;
using System.Linq;
using Proteca.Silverlight.Services.EntityServices;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using System.ComponentModel.Composition;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for GeoSecteur entity
    /// </summary>
    [ExportAsViewModel("GeoSecteur")]
    public class GeoSecteurViewModel : BaseProtecaEntityViewModel<GeoSecteur>
    {
        #region Properties

        private bool IsDeleting = false;

        #endregion Properties

        #region Services

        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public GeoSecteurViewModel(): base()
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
                if (SelectedEntity!=null && !SelectedEntity.IsNew() && !IsDeleting)
                {
                    EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("SelectedItem", this.SelectedEntity));
                }
            };

            this.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "SelectedEntity" )
                {
                    EventAggregator.Publish<ObservableCollection<GeoSecteur>>(this.Entities);
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("RegionLoaded", true));
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
                GeoRegionViewModel regionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
                regionVM.AllServicesLoaded = false;
                GeoAgenceViewModel agenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
                agenceVM.AllServicesLoaded = false;
                DecoupageGeoViewModel DecoupageGeoVM = Router.ResolveViewModel<DecoupageGeoViewModel>(false, "DecoupageGeo");
                DecoupageGeoVM.Deactivate("DecoupageGeo");
                TreeViewGeoViewModel TreeViewVM = Router.ResolveViewModel<TreeViewGeoViewModel>(false, "TreeViewGeo");
                TreeViewVM.Deactivate("TreeViewGeo");
            }
        }

        /// <summary>
        /// Suppression d'une agence
        /// </summary>
        protected override void Delete()
        {
            MessageBoxResult result = MessageBox.Show(Resource.DecoupageGeo_DeleteSecteurConfirmation,
                Resource.DecoupageGeo_DeleteSecteurCaptionMsgError, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                IsBusy = true;
                ((GeoSecteurService)this.service).CheckAndDeleteEntityByCle(this.SelectedEntity.CleSecteur, CheckAndDeleteDone);
            }
        }

        /// <summary>
        /// Ajout du nouveau secteur et association avec l'agence sélectionnée dans le treeview
        /// </summary>
        protected override void Add()
        {
            TreeViewGeoViewModel treeViewGeoVM = Router.ResolveViewModel<TreeViewGeoViewModel>(false, "TreeViewGeo");
            GeoAgence agence = ((GeoAgence)treeViewGeoVM.SelectedItem);

            base.Add();

            agence.GeoSecteur.Add(this.SelectedEntity);
        }

        /// <summary>
        /// Supprime la dernière vue visitée si il s'agissait d'un ajout
        /// </summary>
        protected override void Cancel()
        {
            int cleAgence = 0;
            if (this.SelectedEntity.IsNew())
            {
                cleAgence = this.SelectedEntity.CleAgence;
            }            
            base.Cancel();

            if (cleAgence > 0)
            {
                NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                   MainNavigation.Administration.GetStringValue(),
                   AdministrationNavigation.DecoupageGeo.GetStringValue(),
                   Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue(), cleAgence),
                   UriKind.Relative));
            }
        }

        #endregion Protected Method

        #region Private Functions

        /// <summary>
        /// Retour de la validation et la suppression d'un secteur
        /// </summary>
        /// <param name="error">Message d'erreur</param>
        /// <param name="text">Liste des conditions non respecter pour supprimer le secteur</param>
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
                //Les règles de gestion doivent être validées avant de supprimer le secteur
                MessageBox.Show(text, Resource.DecoupageGeo_DeleteSecteurCaptionMsgError, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(Resource.BaseProtecaEntityViewModel_DeleteSuccess, string.Empty, MessageBoxButton.OK);

                GeoAgence agence = SelectedEntity.GeoAgence;

                this.service.Clear();

                // Rechargement des régions
                GeoRegionViewModel regionVM = Router.ResolveViewModel<GeoRegionViewModel>(false, "GeoRegion");
                regionVM.service.GetEntities(e=>
                    {
                        // Rechargement des Agences
                        GeoAgenceViewModel agenceVM = Router.ResolveViewModel<GeoAgenceViewModel>(false, "GeoAgence");
                        agenceVM.service.GetEntities(null);

                        // Rechargement des secteurs
                        this.service.GetEntities(c =>
                        {
                            EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("RefreshTree", true));
                            EventAggregator.Publish("TreeViewGeo".AsViewNavigationArgs().AddNamedParameter("SelectedItem", agence));

                            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/{3}={4}",
                          MainNavigation.Administration.GetStringValue(),
                          AdministrationNavigation.DecoupageGeo.GetStringValue(),
                          Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue(),
                          Global.Constants.PARM_ID,
                          ((GeoAgence)agence).CleAgence), UriKind.Relative));
                        });
                    }
                );
            }
        }

        #endregion Private Functions

    }
}
