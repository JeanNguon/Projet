using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Jounce.Core.Event;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Silverlight.Services.Contracts;
using System.Windows;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Enums.NavigationEnums;
using Microsoft.SharePoint.Client;
using System.Windows.Browser;

namespace Proteca.Silverlight.Services
{
    /// <summary>
    /// Gestion du menu Site Action.
    /// </summary>
    [Export(typeof(ISiteActionService))]
    public class SiteActionService : SharepointService, ISiteActionService
    {
        #region Services
        #endregion

        #region Constructor
        public SiteActionService()
        {
            //
        }
        #endregion

        #region Properties
        #endregion

        #region Methods

        /// <summary>
        /// Checkout distant de la page passé en paramètre.
        /// </summary>
        /// <param name="pageUri"></param>
        /// <param name="completed"></param>
        public void CheckoutPage(string pageUri, Action<Exception> completed)
        {
            if (ContextClientSharePoint != null)
            {
                File homePage = ContextClientSharePoint.Site.RootWeb.GetFileByServerRelativeUrl(pageUri);
                homePage.CheckOut();

                ContextClientSharePoint.ExecuteQueryAsync(
                    (o, e) =>
                    {
                        // Redirect si ok.
                        _syncCtxt.Post(unused => completed(null), null);
                    },
                    (o, e) =>
                    {
                        // Exception si la page est déjà checkout.
                        // Redirection, et Sharepoint affichera un message adapté.
                        _syncCtxt.Post(unused => completed(null), null);
                    }
                );
            }            
        }
        
        #endregion
    }
}
