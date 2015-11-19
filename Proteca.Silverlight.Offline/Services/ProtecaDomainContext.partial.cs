using System.ComponentModel.Composition;
using Jounce.Core.Application;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Services.Contracts;
using Offline;

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
            //if (!this.RestoreFromIsoStore())
            //{
            //}
        }
    }
}
