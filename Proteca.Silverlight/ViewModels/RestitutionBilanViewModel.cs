using System.Collections.ObjectModel;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System.Linq;
using System;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.EntityServices;
using System.Windows;
using System.Collections.Generic;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using System.Windows.Browser;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for RestitutionBilan entity
    /// </summary>
    [ExportAsViewModel("RestitutionBilan")]
    public class RestitutionBilanViewModel : BaseViewModel
    {
        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'utilisateur connecté
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> userService { get; set; }

        #endregion

        #region private members
        private string _urlEnsElec;
        private string _urlEq;
        private string _urlPortions;
        private string _urlExport;
        private string _urlBilanExport;
        private string _urlBilanExportGeo;
        private string _urlBilanPerso1;
        private string _urlBilanPerso2;
        private string _urlBilanPerso3;
        private string _urlBilanPerso4;


        #endregion

        #region properties

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

        /// <summary>
        /// lien vers le rapport ens elec
        /// </summary>
        public string URLEnsElec
        {
            get { return _urlEnsElec; }
            set { _urlEnsElec = value; }
        }

        /// <summary>
        /// lien vers le rapport equipement
        /// </summary>
        public string URLEq
        {
            get { return _urlEq; }
            set { _urlEq = value; }
        }

        /// <summary>
        /// lien vers le rapport portion
        /// </summary>
        public string URLPortions
        {
            get { return _urlPortions; }
            set { _urlPortions = value; }
        }

        /// <summary>
        /// lien vers le rapport export
        /// </summary>
        public string URLExport
        {
            get { return _urlExport; }
            set { _urlExport = value; }
        }

        /// <summary>
        /// lien vers le rapport export
        /// </summary>
        public string URLBilanExport
        {
            get { return _urlBilanExport; }
            set { _urlBilanExport = value; }
        }

        /// <summary>
        /// lien vers le rapport bilan geo
        /// </summary>
        public string URLBilanExportGeo
        {
            get { return _urlBilanExportGeo; }
            set { _urlBilanExportGeo = value; }
        }

        /// <summary>
        /// lien vers le rapport perso 1
        /// </summary>
        public string URLBilanPerso1
        {
            get { return _urlBilanPerso1; }
            set { _urlBilanPerso1 = value; }
        }

        /// <summary>
        /// lien vers le rapport perso 2
        /// </summary>
        public string URLBilanPerso2
        {
            get { return _urlBilanPerso2; }
            set { _urlBilanPerso2 = value; }
        }

        /// <summary>
        /// lien vers le rapport perso 3
        /// </summary>
        public string URLBilanPerso3
        {
            get { return _urlBilanPerso3; }
            set { _urlBilanPerso3 = value; }
        }

        /// <summary>
        /// lien vers le rapport perso 4
        /// </summary>
        public string URLBilanPerso4
        {
            get { return _urlBilanPerso4; }
            set { _urlBilanPerso4 = value; }
        }
        #endregion

        #region Constructor

        public RestitutionBilanViewModel()
            : base()
        {
            // Boutons générique
            RestitutionCommand = new ActionCommand<object>(
                obj => ShowRestitution(obj));
        }

        #endregion

        #region Commands

        // Boutons générique
        public IActionCommand RestitutionCommand { get; set; }

        #endregion

        #region private methods

        /// <summary>
        /// Affiche le rapport demandé
        /// </summary>
        private void ShowRestitution(object FileName)
        {
            if (FileName is String)
            {
                String rapportUrl = Rapports.printDocumentUrl;

                String urlDetail = FileName as String;

                int cle_region = userService.CurrentUser.PreferenceCleRegion.HasValue ? userService.CurrentUser.PreferenceCleRegion.Value : 0;
                int cle_agence = userService.CurrentUser.PreferenceCleAgence.HasValue ? userService.CurrentUser.PreferenceCleAgence.Value : 0;
                int cle_secteur = userService.CurrentUser.PreferenceCleSecteur.HasValue ? userService.CurrentUser.PreferenceCleSecteur.Value : 0;
                int cle_EE = userService.CurrentUser.PreferenceCleEnsembleElectrique.HasValue ? userService.CurrentUser.PreferenceCleEnsembleElectrique.Value : 0;
                int cle_portion = userService.CurrentUser.PreferenceClePortion.HasValue ? userService.CurrentUser.PreferenceClePortion.Value : 0;
                rapportUrl += String.Format(urlDetail, cle_region, cle_agence, cle_secteur, cle_EE, cle_portion);

                HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
            }
        }

        #endregion

        #region Override methods

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

        #endregion
    }
}
