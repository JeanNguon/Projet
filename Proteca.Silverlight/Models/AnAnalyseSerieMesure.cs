using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using Proteca.Silverlight.Resources;

namespace Proteca.Web.Models
{
    public partial class AnAnalyseSerieMesure
    {
        #region Gestion du ActivateChangePropagation pour les visites

        public Dictionary<string, Entity> GetParentWithPropName()
        {
            Dictionary<string, Entity> retour = new Dictionary<string, Entity>();
            retour.Add("AnAnalyseSerieMesure", this.Visite);
            return retour;
        }

        #endregion

        public Alerte Alerte
        {
            get
            {
                return this.Alertes.FirstOrDefault();
            }
        }

        public string CommentaireHtmlStriped
        {
            get
            {
                return String.IsNullOrEmpty(this.Commentaire) ? String.Empty : System.Windows.Browser.HttpUtility.HtmlDecode(this.Commentaire);
            }
        }

        public List<String> AnalyseToText
        {
            get
            {
                List<String> result = new List<String>();
                if (this.RefEnumValeur != null)
                {
                    result.Add(Resource.AnAnalyse_EtatPC.Replace("*", "") + ' ' + this.RefEnumValeur.Libelle);
                    result.Add(Resource.AnAnalyseEe_Commentaire.Replace("*", "") + " :" + '\n' + this.CommentaireHtmlStriped);
                }
                return result;
            }
        }
    }
}
