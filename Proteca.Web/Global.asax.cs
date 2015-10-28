using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using log4net;
using System.Threading.Tasks;
using Proteca.Web.Models;
using Proteca.Web.Services;

namespace Proteca.Web
{
    public class Global : System.Web.HttpApplication
    {
        private ILog logger = LogManager.GetLogger("ProtecaV4");

        protected void Application_Start(object sender, EventArgs e)
        {
            // Au démarrage de l'application on charge automatiquement les requêtes les plus couteuses pour s'assurer que leurs plans d'exécution soit en cache            
            var task = Task.Factory.StartNew(() =>
            {
                // warmup 
                using (ProtecaDomainService context = new ProtecaDomainService())
                {
                    var pp = context.GetPps().FirstOrDefault();
                    if (pp != null)
                    {
                        context.GetDeplacementPpByCle(pp.ClePp);
                    }
                }
            }
            );
            var task2 = Task.Factory.StartNew(() =>
            {
                // warmup 
                using (ProtecaDomainService context = new ProtecaDomainService())
                {
                    var tournee = context.GetTournees().FirstOrDefault();
                    if (tournee != null)
                    {
                        context.GetTourneeByCle(tournee.CleTournee);
                    }
                }
            }
          );

            var taskVisite1 = Task.Factory.StartNew(() =>
            {
                // warmup 
                using (ProtecaDomainService context = new ProtecaDomainService())
                {
                    var portion = context.GetPortionIntegrite().FirstOrDefault();
                    if (portion != null)
                    {
                        //context.FindVisitesValideesByCriterias(null, null, null, null, portion.ClePortion, null, null, String.Empty, false).ToList();
                    }
                }
            }
            );            

            var taskVisite2 = Task.Factory.StartNew(() =>
            {
                // warmup 
                using (ProtecaDomainService context = new ProtecaDomainService())
                {                    
                    var portion = context.GetPortionIntegrite().FirstOrDefault();

                    if (portion != null && portion.PiSecteurs.Any())
                    {
                        var secteur = portion.PiSecteurs.FirstOrDefault().GeoSecteur;

                        context.FindVisitesNonValideesByCriterias(secteur.GeoAgence.CleRegion, secteur.CleAgence, null, null, null, null, null).ToList();
                    }
                }
            }
            );
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                logger.Error(e.ToString());
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}