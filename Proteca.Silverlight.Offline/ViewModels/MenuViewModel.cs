using System;
using System.ComponentModel.Composition;
using Jounce.Core.Command;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Models;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Helpers;
using System.Windows.Browser;
using Proteca.Silverlight.Services;
using System.Windows.Controls;
using Offline;
using System.IO;
using Ionic.Zip;
using System.Text;

namespace Proteca.Silverlight.ViewModels
{
    [ExportAsViewModel("Menu")]
    public class MenuViewModel : BaseViewModel, IEventSink<CurrentNavigation>, IEventSink<ViewMode>, IPartImportsSatisfiedNotification
    {
        #region Services

        /// <summary>
        /// Service utilisé pour la configuration du Site Action.
        /// </summary>
        [Import(typeof(ISiteActionService))]
        public ISiteActionService SiteActionService { get; set; }

        /// <summary>
        /// Service utilisé pour naviguer dans l'application
        /// </summary>
        [Import(typeof(INavigationService))]
        public INavigationService NavigationService { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'utilisateur connecté.
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> UserService { get; set; }

        /// <summary>
        /// Service utilisé pour sauvegarder et charger le domainContext
        /// </summary>
        [Import(typeof(SynchronizationService))]
        public SynchronizationService SynchronizationService { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public MenuViewModel()
        {
            // Define commands
            LoadCommand = new ActionCommand<object>(
                obj => LoadSaveHelper.LoadFromFile(this.SynchronizationService, this.EventAggregator, this.NavigationService as NavigationService), obj => true);

            SaveCommand = new ActionCommand<object>(
                obj => LoadSaveHelper.SaveToFile(), obj => true);

            //ReloadCommand = new ActionCommand<object>(
            //    obj => LoadSaveHelper.ReloadFromIsoStore(this.SynchronizationService, this.EventAggregator, this.NavigationService), obj => true);

            if (this.SharepointMenuSource == null || this.MenuSource == null)
            {
                loadMenu();
            }
        }
        #endregion

        #region Commands

        public IActionCommand SaveCommand { get; private set; }

        public IActionCommand LoadCommand { get; private set; }

        //public IActionCommand ReloadCommand { get; private set; }

        #endregion

        #region Properties

        private Boolean _isEnabled = true;
        public Boolean IsEnable
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => this.IsEnable);
            }
        }

        private MenuItemsCollection _menuSource = new MenuItemsCollection();
        public MenuItemsCollection MenuSource
        {
            get { return _menuSource; }
            set
            {
                _menuSource = value;
                RaisePropertyChanged(() => MenuSource);
            }
        }

        private MenuItemsCollection _sharepointMenuSource = new MenuItemsCollection();
        public MenuItemsCollection SharepointMenuSource
        {
            get { return _sharepointMenuSource; }
            set
            {
                _sharepointMenuSource = value;
                RaisePropertyChanged(() => SharepointMenuSource);
            }
        }

        /// <summary>
        /// Sélectionne le menu en fonction de l'URL d'entrée
        /// </summary>
        /// <param name="Url">L'url d'entrée</param>
        public void ChangeSelection(String Url)
        {
            MenuItem mi = MenuSource.ItemSelected;

            if (mi != null)
            {
                mi.IsSelected = false;
            }
            mi = MenuSource.findByURL(Url);
            if (mi != null)
            {
                mi.IsSelected = true;

                //EventAggregator.Publish("FilAriane".AsViewNavigationArgs().AddNamedParameter("Title", this.SynchronizationService.LibelleTournee));
            }

            // Forcer le binding du menu pour raffraichir les styles
            MenuItemsCollection BackupMenuSource = MenuSource;
            MenuSource = new MenuItemsCollection();
            MenuSource = BackupMenuSource;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            EventAggregator.SubscribeOnDispatcher<CurrentNavigation>(this);
            EventAggregator.SubscribeOnDispatcher<ViewMode>(this);
            loadMenu();
        }

        private void loadMenu()
        {
            setMenu();
        }

        /// <summary>
        /// Evènement levé lors d'un changement d'URL
        /// </summary>
        /// <param name="publishedEvent">L'élément de navigation</param>
        public void HandleEvent(CurrentNavigation publishedEvent)
        {
            this.ChangeSelection(publishedEvent.BaseUrlWithOutFilter);
        }

        /// <summary>
        /// Défini le menu
        /// </summary>
        private void setMenu()
        {
            MenuSource = new MenuItemsCollection();

            // Définition du menu "SiteAction"
            MenuItem sl = new MenuItem("Données", "");
            // Définition des menuItems
            MenuItem load = new MenuItem("Importer une tournée", "");
            MenuItem save = new MenuItem("Sauvegarder", "");
            //MenuItem reload = new MenuItem("Recharger", "");

            // Ajout des commandes aux menuItems
            load.MenuCommand = this.LoadCommand;
            save.MenuCommand = this.SaveCommand;
            //reload.MenuCommand = this.ReloadCommand;

            // Ajout des items dans le menu SiteAction.
            sl.Items.Add(load);
            sl.Items.Add(save);
            // Suppression de la commande 
            //sl.Items.Add(reload);

            MenuSource = new MenuItemsCollection() { sl };

            EventAggregator.Publish("FilAriane".AsViewNavigationArgs().AddNamedParameter("Title", this.SynchronizationService.LibelleTournee));


            if (CurrentNavigation.Current != null)
            {
                ChangeSelection(CurrentNavigation.Current.BaseUrlWithOutFilter);
            }
        }
        #endregion

        /// <summary>
        /// Evènement déclenché lors d'un changement de Mode d'affichage
        /// </summary>
        /// <param name="publishedEvent">Le nouveau mode d'affichage</param>
        public void HandleEvent(ViewMode publishedEvent)
        {
            switch (publishedEvent)
            {
                case ViewMode.NavigationMode:
                    IsEnable = true;
                    break;
                case ViewMode.EditMode:
                    IsEnable = false;
                    break;
                default:
                    break;
            }
        }
    }
}
