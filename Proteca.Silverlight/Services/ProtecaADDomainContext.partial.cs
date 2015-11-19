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
    [Export(typeof(ProtecaADDomainContext))]
    public sealed partial class ProtecaADDomainContext : DomainContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtecaDomainContext"/> class.
        /// </summary>
        [ImportingConstructor]
        public ProtecaADDomainContext([Import(AllowDefault = true)] IConfigurator configurator)
            : this(new WebDomainClient<IProtecaADDomainServiceContract>(configurator.GetServiceAdress("Proteca-Web-Services-ProtecaADDomainService.svc")))
        {
        }
    }
}
