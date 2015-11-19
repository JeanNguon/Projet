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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;

namespace Proteca.Web.Models
{
    public partial class UsrRole
    {
        public ObservableCollection<RefUsrPortee> Portees
        {
            get;
            set;
        }

        public Dictionary<string, Entity> GetParentWithPropName()
        {
            Dictionary<string,Entity> retour = new Dictionary<string,Entity>();
            retour.Add("UsrRole", this.UsrProfil);
            return retour;
        }
    }
}
