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

namespace Proteca.Web.Models
{
    public partial class PiSecteurs
    {
        public Dictionary<string, Entity> GetParentWithPropName()
        {
            Dictionary<string, Entity> retour = new Dictionary<string, Entity>();
            retour.Add("PiSecteurs", this.PortionIntegrite);
            return retour;
        }
    }
}
