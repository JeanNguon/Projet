using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Proteca.Silverlight.Services.Contracts;
using System.ComponentModel.Composition;
using System.Windows.Browser;
using Microsoft.SharePoint.Client;

namespace Proteca.Silverlight.Services
{
    /// <summary>
    /// Utilisée pour injecter l'adresse de base des services utilisés par le client silverlight
    /// </summary>
    [Export(typeof(IConfigurator))]
    public class ServicesConfirugator : IConfigurator
    {

        public ClientContext GetClientContext()
        {
            ClientContext clientContext = ClientContext.Current;

            String ClientContextUrl = getClientContextUrl();
            if (!String.IsNullOrEmpty(ClientContextUrl))
            {
                clientContext = new ClientContext(ClientContextUrl);
            }

            return clientContext;
        }

        private String getClientContextUrl()
        {
            string url = null;
            var ClientContextUrl = ((App)App.Current).ClientContextUrl;
            if (ClientContextUrl != null)
            {
                url = HttpUtility.UrlDecode(ClientContextUrl);
            }
            return url;
        }

        public Uri GetServiceAdress(string defaultAdress)
        {
            Uri adress = new Uri(defaultAdress, UriKind.Relative);
            string config = getConfigAdress();
            if (!String.IsNullOrEmpty(config))
            {
                adress = new Uri(config + defaultAdress, UriKind.Absolute);
            }
            return adress;
        }

        private string getConfigAdress()
        {
            string config = null;
            var ServiceHostAdress = ((App)App.Current).ServiceHostAdress;
            if (ServiceHostAdress != null)
            {
                config = HttpUtility.UrlDecode(ServiceHostAdress);
                if (!config.EndsWith("/"))
                {
                    config += "/";
                }
            }
            return config;
        }
    }
}
