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
        /// Service utilisé pour gérer l'utilisateur connecté.
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> UserService { get; set; }
        #endregion


        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public MenuViewModel()
        {
            // Define commands
            EditHomePageCommand = new ActionCommand<object>(
               obj => RedirectToEditHomePage(), obj => CanEditHomePage);

            EditGlossaryListCommand = new ActionCommand<object>(
               obj => RedirectToGlossaryListPage(), obj => CanEditGlossaryList);

            EditOnlineHelpListCommand = new ActionCommand<object>(
               obj => RedirectToOnlineHelpListPage(), obj => CanEditOnlineHelpList);

            EditDiagnosticHelpListCommand = new ActionCommand<object>(
               obj => RedirectToDiagnosticHelpListPage(), obj => CanEditDiagnosticHelpList);

            EditHomeLinkListCommand = new ActionCommand<object>(
               obj => RedirectToEditLinkHomePage(), obj => CanEditHomeLinkList); 

            if (this.SharepointMenuSource == null || this.MenuSource == null)
            {
                loadMenu();
            }
        }
        #endregion


        #region Commands
        public IActionCommand EditHomePageCommand { get; private set; }
        public IActionCommand EditGlossaryListCommand { get; private set; }
        public IActionCommand EditOnlineHelpListCommand { get; private set; }
        public IActionCommand EditDiagnosticHelpListCommand { get; private set; }
        public IActionCommand EditHomeLinkListCommand { get; private set; }
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

        private MenuItemsCollection _menuSourceHidden = new MenuItemsCollection();
        public MenuItemsCollection MenuSourceHidden
        {
            get { return _menuSourceHidden; }
            set
            {
                _menuSourceHidden = value;
                RaisePropertyChanged(() => MenuSourceHidden);
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
            }
            mi = MenuSourceHidden.findByURL(Url);
            if (mi != null)
            {
                EventAggregator.Publish("FilAriane".AsViewNavigationArgs().AddNamedParameter("SelectedMenu", mi));
            }

            // Forcer le binding du menu pour raffraichir les styles
            MenuItemsCollection BackupMenuSource = MenuSource;
            MenuSource = new MenuItemsCollection();
            MenuSource = BackupMenuSource;
        }

        /// <summary>
        /// Booleen pour le droit d'accès à la fonction d'impression.
        /// </summary>
        private bool canPrint = true;
        public bool CanPrint
        {
            get { return canPrint; }
            set
            {
                canPrint = value;
                RaisePropertyChanged(() => this.CanPrint);
            }
        }

        /// <summary>
        /// Booleen pour le droit d'accès à l'édition de la Home Page.
        /// </summary>
        private bool canEditHomePage = true;
        public bool CanEditHomePage
        {
            get { return canEditHomePage; }
            set
            {
                canEditHomePage = value;
                RaisePropertyChanged(() => this.CanEditHomePage);
            }
        }

        /// <summary>
        /// Booleen pour le droit d'accès à la liste du glossaire.
        /// </summary>
        private bool canEditGlossaryList = true;
        public bool CanEditGlossaryList
        {
            get { return canEditGlossaryList; }
            set
            {
                canEditGlossaryList = value;
                RaisePropertyChanged(() => this.CanEditGlossaryList);
            }
        }

        /// <summary>
        /// Booleen pour le droit d'accès à la liste OnlineHelp.
        /// </summary>
        private bool canEditOnlineHelpList = true;
        public bool CanEditOnlineHelpList
        {
            get { return canEditOnlineHelpList; }
            set
            {
                canEditOnlineHelpList = value;
                RaisePropertyChanged(() => this.CanEditOnlineHelpList);
            }
        }

        /// <summary>
        /// Booleen pour le droit d'accès à la liste Diagnostic Help.
        /// </summary>
        private bool canEditDiagnosticHelpList = true;
        public bool CanEditDiagnosticHelpList
        {
            get { return canEditDiagnosticHelpList; }
            set
            {
                canEditDiagnosticHelpList = value;
                RaisePropertyChanged(() => this.CanEditDiagnosticHelpList);
            }
        }

        /// <summary>
        /// Booleen pour le droit d'accès à la liste Home Link.
        /// </summary>
        private bool canEditHomeLinkList = true;
        public bool CanEditHomeLinkList
        {
            get { return canEditHomeLinkList; }
            set
            {
                canEditHomeLinkList = value;
                RaisePropertyChanged(() => this.CanEditHomeLinkList);
            }
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
            if (this.UserService != null)
            {
                if (this.UserService.CurrentUser != null)
                {
                    setMenu();
                }
                else
                {
                    this.UserService.CurrentUserLoaded += (o, e) =>
                    {
                        setMenu();
                    };
                }
            }
        }

        /// <summary>
        /// Méthode de redirection vers la home page en mode edit.
        /// </summary>
        public void RedirectToEditHomePage()
        {
            string homePageUrl = string.Empty;

            // Get HomePage Url
            homePageUrl = SiteActionMenus.HomePageUrl.GetStringValue();

            // Checkout la home page avant redirection.
            ((SiteActionService)this.SiteActionService).CheckoutPage(homePageUrl, e =>
                {
                    RedirectToEditHomePage_callback();
                }
            );
        }

        /// <summary>
        /// Redirige l'utilisateur vers la home page en mode edit.
        /// </summary>
        private void RedirectToEditHomePage_callback()
        {
            // Redirige l'utilisateur vers la home page en mode edit
            HtmlPage.Window.Navigate(new Uri(SiteActionMenus.EditModeHomePageUrl.GetStringValue(), UriKind.Relative));
        }

        /// <summary>
        /// Redirige l'utilisateur vers les liens de la page d'accueil.
        /// </summary>
        private void RedirectToEditLinkHomePage()
        {
            HtmlPage.Window.Navigate(new Uri(SiteActionMenus.HomeLinkListUrl.GetStringValue(), UriKind.Relative));
        }

        /// <summary>
        /// Méthode de redirection vers le glossaire.
        /// </summary>
        public void RedirectToGlossaryListPage()
        {
            HtmlPage.Window.Navigate(new Uri(SiteActionMenus.GlossaryListUrl.GetStringValue(), UriKind.Relative));
        }

        /// <summary>
        /// Méthode de redirection vers online help.
        /// </summary>
        public void RedirectToOnlineHelpListPage()
        {
            HtmlPage.Window.Navigate(new Uri(SiteActionMenus.OnlineHelpListUrl.GetStringValue(), UriKind.Relative));
        }

        /// <summary>
        /// Méthode de redirection vers diagnostic help.
        /// </summary>
        public void RedirectToDiagnosticHelpListPage()
        {
            HtmlPage.Window.Navigate(new Uri(SiteActionMenus.DiagnosticHelpListUrl.GetStringValue(), UriKind.Relative));
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
            MenuSourceHidden = new MenuItemsCollection();

            MenuItem acc = new MenuItem("Accueil", MainNavigation.Accueil.GetStringValue());

            MenuItem go = new MenuItem("Gestion des Ouvrages", MainNavigation.GestionOuvrages.GetStringValue());
            go.Items.Add(new MenuItem("Equipement", OuvrageNavigation.Equipement.GetStringValue() + "/" + FiltreNavigation.PP.GetStringValue()));
            go.Items.Add(new MenuItem("Portion Intégrité", OuvrageNavigation.PortionIntegrite.GetStringValue()));
            go.Items.Add(new MenuItem("Ensemble Electrique", OuvrageNavigation.EnsembleElectrique.GetStringValue()));
            go.Items.Add(new MenuItem("Documentation", OuvrageNavigation.Documentation.GetStringValue()));

            MenuItem vi = new MenuItem("Visite", MainNavigation.Visite.GetStringValue());
            vi.Items.Add(new MenuItem("Saisie Visite", VisiteNavigation.SaisieVisite.GetStringValue()));
            vi.Items.Add(new MenuItem("Edition Visite", VisiteNavigation.EditionVisite.GetStringValue()));
            vi.Items.Add(new MenuItem("Validation Visite", VisiteNavigation.ValidationVisite.GetStringValue()));
            vi.Items.Add(new MenuItem("Import Visite", VisiteNavigation.ImportVisite.GetStringValue()));
            vi.Items.Add(new MenuItem("Tournée", VisiteNavigation.Tournee.GetStringValue()));
			//MANTIS 10815, 06/05/14, FSI : Filtre des écrans Alerte et Action pour les utilisateurs prestataires
            if (this.UserService.CurrentUser != null && !this.UserService.CurrentUser.EstPresta)
            {
                vi.Items.Add(new MenuItem("Alerte", VisiteNavigation.Alerte.GetStringValue()));
                vi.Items.Add(new MenuItem("Action", VisiteNavigation.FicheAction.GetStringValue()));
            }
            vi.Items.Add(new MenuItem("Validation Import", VisiteNavigation.ValidationEquipement.GetStringValue()));

            MenuItem ar = new MenuItem("Analyse et Restitution", MainNavigation.AnalyseRestitution.GetStringValue());
            ar.Items.Add(new MenuItem("Analyse et Rapport par Ens. Elec.", AnalyseRestitutionNavigation.AnAnalyseEe.GetStringValue()));
            //ar.Items.Add(new MenuItem("Analyse par Série de Mesures", ""));
            ar.Items.Add(new MenuItem("Restitution et Bilan", AnalyseRestitutionNavigation.RestitutionBilan.GetStringValue()));

            MenuItem adm = new MenuItem("Administration", MainNavigation.Administration.GetStringValue());
            adm.Items.Add(new MenuItem("Utilisateurs", AdministrationNavigation.UsrUtilisateur.GetStringValue()));
            adm.Items.Add(new MenuItem("Profils", AdministrationNavigation.UsrProfil.GetStringValue()));
            adm.Items.Add(new MenuItem("Instruments de Mesures", AdministrationNavigation.InsInstrument.GetStringValue()));

            // Menus réservés aux administrateurs
            if (this.UserService.CurrentUser != null && this.UserService.CurrentUser.IsAdministrateur)
            {
                adm.Items.Add(new MenuItem("Déplacement de PP", AdministrationNavigation.DeplacementPp.GetStringValue()));
                adm.Items.Add(new MenuItem("Découpage Portion Intégrité", AdministrationNavigation.DecoupagePortion.GetStringValue()));
            }

            adm.Items.Add(new MenuItem("Gestion des Découpages Géographiques", AdministrationNavigation.DecoupageGeo.GetStringValue()));

            // Menus réservés aux administrateurs
            if (this.UserService.CurrentUser != null && this.UserService.CurrentUser.IsAdministrateur)
            {
                // suppression de la fonctionnalité de regroupement de régions
                //adm.Items.Add(new MenuItem("Regroupement de Régions", AdministrationNavigation.RegoupementRegion.GetStringValue()));
                adm.Items.Add(new MenuItem("Ressources", AdministrationNavigation.Ressources.GetStringValue() + "/" + FiltreNavigation.Actions.GetStringValue()));
            }

            MenuItem par = new MenuItem("Paramètres", MainNavigation.Parametres.GetStringValue());
            par.Items.Add(new MenuItem("Type de Mesures", ParametresNavigation.MesModeleMesure.GetStringValue() + "/" + FiltreNavigation.PP.GetStringValue()));
            par.Items.Add(new MenuItem("Unités", ParametresNavigation.MesUnites.GetStringValue()));
            par.Items.Add(new MenuItem("Classification Mesure", ParametresNavigation.MesClassementMesure.GetStringValue() + "/" + FiltreNavigation.PP.GetStringValue()));
            par.Items.Add(new MenuItem("Paramètres Généraux", ParametresNavigation.RefParametre.GetStringValue()));
            par.Items.Add(new MenuItem("Type de Document", ParametresNavigation.TypeDocument.GetStringValue()));
            par.Items.Add(new MenuItem("Catégorisation des PP", ParametresNavigation.CategoriePp.GetStringValue()));
            par.Items.Add(new MenuItem("Action", ParametresNavigation.Action.GetStringValue()));

            // Recherche
            MenuItem se = new MenuItem("Recherche", MainNavigation.Search.GetStringValue());
            se.Items.Add(new MenuItem("Recherche Avancée", SearchNavigation.Search.GetStringValue()));

            // Personnalisation ODIMA GOP
            // ####### ENLEVER POUR LIVRAISON #########
            // ############ IMPORTANT #################
            MenuItem GOP = new MenuItem("Odima", MainNavigation.Odima.GetStringValue());
            GOP.Items.Add(new MenuItem("Générateur", OdimaNavigation.AdminGenerateur.GetStringValue()));

            // Définition du menu "SiteAction"
            MenuItem sa = new MenuItem("Action du Site", "");

            // Create SiteAction Menu Items
            if (UserService.CurrentUser.IsAdministrateur)
            {
                // Définition des menuItems
                MenuItem editPageMenuItem = new MenuItem("Editer la page d'accueil", "");
                MenuItem editGlossaryMenuItem = new MenuItem("Editer le glossaire", "");
                MenuItem editOnlineHelpMenuItem = new MenuItem("Editer l'aide en ligne", "");
                MenuItem editDiagnosticHelpMenuItem = new MenuItem("Editer l'aide au diagnostic", "");
                MenuItem editHomeLinkMenuItem = new MenuItem("Editer les liens de l'Accueil", "");

                // Ajout des commandes aux menuItems
                editPageMenuItem.MenuCommand = this.EditHomePageCommand;
                editGlossaryMenuItem.MenuCommand = this.EditGlossaryListCommand;
                editOnlineHelpMenuItem.MenuCommand = this.EditOnlineHelpListCommand;
                editDiagnosticHelpMenuItem.MenuCommand = this.EditDiagnosticHelpListCommand;
                editHomeLinkMenuItem.MenuCommand = this.EditHomeLinkListCommand;

                // Ajout des items dans le menu SiteAction.
                sa.Items.Add(editPageMenuItem);
                sa.Items.Add(editHomeLinkMenuItem);
                sa.Items.Add(editGlossaryMenuItem);
                sa.Items.Add(editOnlineHelpMenuItem);
                sa.Items.Add(editDiagnosticHelpMenuItem);
            }

            MenuSource = new MenuItemsCollection() { acc, go, vi, ar, adm, par };
            MenuSourceHidden = new MenuItemsCollection() { acc, go, vi, ar, adm, par, se, GOP };

            SharepointMenuSource = new MenuItemsCollection() { sa };

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
