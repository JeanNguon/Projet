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
using System.ComponentModel.Composition;
using Jounce.Core.Application;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Services.Contracts;
using System.Linq;

namespace Proteca.Web.Services
{
    /// <summary>
    /// Classe partielle permettant de redéfinir un constructor par défaut pour injecter l'adresse du service
    /// </summary>
    [Export(typeof(ProtecaDomainContext))]
    public sealed partial class ProtecaDomainContext : DomainContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtecaDomainContext"/> class.
        /// </summary>
        [ImportingConstructor]
        public ProtecaDomainContext([Import(AllowDefault = true)] IConfigurator configurator)
            : this(new WebDomainClient<IProtecaDomainServiceContract>(configurator.GetServiceAdress("Proteca-Web-Services-ProtecaDomainService.svc")))
        {
        }

        partial void OnCreated()
        {
            var proxy = (WebDomainClient<IProtecaDomainServiceContract>)this.DomainClient;
            proxy.ChannelFactory.Endpoint.Binding.SendTimeout = new TimeSpan(0, 5, 0);
        }
    }
}
