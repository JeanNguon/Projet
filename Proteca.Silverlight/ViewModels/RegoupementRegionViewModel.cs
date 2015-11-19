using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using Proteca.Silverlight.Services.EntityServices;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for RegoupementRegion entity
    /// </summary>
    [ExportAsViewModel("RegoupementRegion")]
    public class RegoupementRegionViewModel : BaseProtecaEntityViewModel<GeoRegion>
    {
        #region Properties

        /// <summary>
        /// Liste des régions de regroupement
        /// </summary>
        public ObservableCollection<GeoRegion> RegionsOfRegrouping
        {
            get { return serviceRegion.Entities; }
        }

        /// <summary>
        /// Liste de régions à regrouper
        /// </summary>
        public List<GeoRegion> RegionsToRegrouping
        {
            get
            {
                if (RegionOfRegrouping != null)
                {
                    return serviceRegion.Entities.Where(r => r.CleRegion != RegionOfRegrouping.CleRegion).ToList();
                }
                else
                {
                    return new List<GeoRegion>();
                }
            }
        }

        /// <summary>
        /// Indicateur pour rendre disponible le boutton de regroupement 
        /// ainsi que la liste des regions à regrouper
        /// </summary>
        public bool CanSelectRegionToRegrouping
        {
            get { return (RegionOfRegrouping != null); }
        }

        /// <summary>
        /// Indicateur pour rendre disponible le boutton de regroupement 
        /// ainsi que la liste des regions à regrouper
        /// </summary>
        public bool CanRegroupRegion
        {
            get { return (CanSelectRegionToRegrouping && RegionToRegrouping != null); }
        }

        /// <summary>
        /// La région de regroupement
        /// </summary>
        private GeoRegion _regionOfRegrouping;
        public GeoRegion RegionOfRegrouping
        {
            get { return _regionOfRegrouping; }
            set
            {
                _regionOfRegrouping = value;
                RaisePropertyChanged(() => this.RegionOfRegrouping);
                RaisePropertyChanged(() => this.RegionsToRegrouping);
                RaisePropertyChanged(() => this.CanSelectRegionToRegrouping);
                RegroupeRegionCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// La région à regrouper
        /// </summary>
        private GeoRegion _regionToRegrouping;
        public GeoRegion RegionToRegrouping
        {
            get { return _regionToRegrouping; }
            set
            {
                _regionToRegrouping = value;
                RaisePropertyChanged(() => this.RegionToRegrouping);
                RaisePropertyChanged(() => this.CanRegroupRegion);
                RegroupeRegionCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion Properties

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        #endregion Services

        #region Commandes

        /// <summary>
        /// Commande pour le bouton de regroupement des régions
        /// </summary>
        public IActionCommand RegroupeRegionCommand { get; private set; }

        #endregion Commandes

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public RegoupementRegionViewModel(): base()
        {
            this.IsAutoNavigateToFirst = false;

            this.RegroupeRegionCommand = new ActionCommand<object>(
                obj => RegroupementRegion(), obj => CanRegroupRegion);

            this.OnAllServicesLoaded += (o, e) =>
            {
                RegionLoaded(null);
            };

            this.OnCanceled += (o, e) =>
            {
                this.RegionOfRegrouping = null;
                this.RegionToRegrouping = null;
            };
        }

        #endregion Constructor

        #region Private Functions

        /// <summary>
        /// Regroupe les deux régions sélectionnées
        /// </summary>
        private void RegroupementRegion()
        {
            if (this.RegionToRegrouping != null)
            {
                MessageBoxResult result = MessageBox.Show(Resource.RegroupementRegion_ValidationText, Resource.RegroupementRegion_ValidationCaption,
                                                            MessageBoxButton.OKCancel);
                // Après la première validation
                if (result == MessageBoxResult.OK)
                {
                    result = MessageBox.Show(Resource.RegroupementRegion_ValidationText2, Resource.RegroupementRegion_ValidationCaption,
                                                            MessageBoxButton.OKCancel);
                    // Après la seconde validation
                    if (result == MessageBoxResult.OK)
                    {
                        IsBusy = true;

                        ((GeoRegionService)this.serviceRegion).CheckAndRegoupeRegionByCle(this.RegionOfRegrouping.CleRegion, 
                            this.RegionToRegrouping.CleRegion, this.RegionOfRegrouping.LibelleRegion,
                            this.RegionOfRegrouping.LibelleAbregeRegion, (error) => RegroupingRegionSaved(error));
                    }
                }
            }
        }

        /// <summary>
        /// Les données ont été enregistré.
        /// Cette fonction permet de traiter les éventuelles erreurs rencontré lors de la mise à jour.
        /// Elle va indiquer notification à l'utilisateur que le traitement est terminé et va réinitialiser l'écran
        /// </summary>
        /// <param name="ex"></param>
        private void RegroupingRegionSaved(Exception ex)
        {
            if (ex != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, ex.ToString());
                ErrorWindow.CreateNew(ex.Message);
            }
            else
            {
                RaisePropertyChanged(() => this.RegionsOfRegrouping);
                this.RegionOfRegrouping = null;
                this.RegionToRegrouping = null;
                MessageBox.Show(Resource.RegroupeRegion_SuccesText,Resource.RegroupeRegion_SuccesCaption ,MessageBoxButton.OK);
            }

            // We're done
            IsBusy = false;
        }
        
        /// <summary>
        /// Méthode utilisé pour charger l'entité de type Region
        /// </summary>
        private void RegionLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                RaisePropertyChanged(() => this.RegionsToRegrouping);
                RaisePropertyChanged(() => this.RegionsOfRegrouping);
            }

            // We're done
            IsBusy = false;
        }

        /// <summary>
        /// Activation de la vue de regroiupement de région.
        /// Lors de l'activation on cache la partie customExpander
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);
            EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("HideExpander", true));
        }

        #endregion Private Functions

        #region Autorisations

        /// <summary>
        /// Retourne si true l'utilisateur est un administrateur
        /// </summary>
        /// <returns></returns>
        protected override bool GetUserCanRead()
        {
            bool result = false;
            // suppression de la fonctionnalité de regroupement de régions
            //if (this.userService != null && this.userService.CurrentUser != null)
            //{
            //    result = this.userService.CurrentUser.IsAdministrateur;
            //}
            return result;
        }

        #endregion
    }
}
