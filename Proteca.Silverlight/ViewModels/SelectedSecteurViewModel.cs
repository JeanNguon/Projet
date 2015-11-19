using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reflection;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework.Command;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Views.Windows;
using Proteca.Web.Models;

namespace Proteca.Silverlight.ViewModels
{
    [ExportAsViewModel("Selected_Secteur")]
    public class SelectedSecteurViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        #region Properties

        /// <summary>
        /// Retourne les regions du service
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get
            {
                return ServiceRegions.Entities;
            }
        }

        public GeoAgence SelectedAgence { get; set; }

        public Boolean AllServicesLoaded { get; set; }

        #endregion

        #region Command

        /// <summary>
        /// Commande de validation de la sélection
        /// </summary>
        public IActionCommand ValidateCommand { get; set; }

        /// <summary>
        /// Commande d'annulation de la sélection
        /// </summary>
        public IActionCommand CancelCommand { get; set; }

        #endregion

        #region Service

        /// <summary>
        /// Retourne le service de GeoRegion
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> ServiceRegions { get; set; }

        [Import]
        public ChildWindowControl Childwindow;
        
        #endregion

        #region Constructor

        public SelectedSecteurViewModel()
            : base()
        {

            ValidateCommand = new ActionCommand<object>(
                obj => Validate(), obj => true);
            CancelCommand = new ActionCommand<object>(
                obj => Cancel(), obj => true);

        }

        #endregion

        #region Private Methods

        protected override void ActivateView(string viewName, System.Collections.Generic.IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);
            if (!AllServicesLoaded)
            {
                AllServicesLoaded = true;
                ServiceRegions.GetEntities((err) => RegionLoaded(err));
            }            
        }

        protected override void DeactivateView(string viewName)
        {
            ServiceRegions.Clear();
            base.DeactivateView(viewName);
            AllServicesLoaded = false;
        }

        private void Validate()
        {
            // Instanciation de la liste de secteur à renvoyer
            ObservableCollection<GeoSecteur> secteursAenvoyer = new ObservableCollection<GeoSecteur>();

            // On ajoute le secteur dans la liste uniquement si il est indiqué comme coché
            if (SelectedAgence != null)
            {
                foreach (var secteur in SelectedAgence.SecteursTries)
                {
                    if (secteur.IsChecked == true)
                    {
                        secteursAenvoyer.Add(secteur);
                    }

                    secteur.IsChecked = false;
                }
            }

            // Publication de la liste
            EventAggregator.Publish(secteursAenvoyer);

            // Fermeture de la popup
            Childwindow.DialogResult = true;
        }

        private void Cancel()
        {
            // MAJ des indicateurs de cochage de l'item
            if (SelectedAgence != null)
            {
                foreach (var secteur in SelectedAgence.SecteursTries)
                {
                    secteur.IsChecked = false;
                }
            }

            // Fermeture de la popup
            Childwindow.DialogResult = false;
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

                AllServicesLoaded = false;
            }
            else
            {
                AllServicesLoaded = true;
                RaisePropertyChanged(() => this.Regions);
            }
        }

        public void OnImportsSatisfied()
        {            
        }

        #endregion

    }
}
