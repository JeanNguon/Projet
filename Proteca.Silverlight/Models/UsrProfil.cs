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
using System.Reflection;
using System.ServiceModel.DomainServices.Client;
using System.Collections;
using Proteca.Silverlight.Helpers;

namespace Proteca.Web.Models
{
    public partial class UsrProfil
    {
        public IEnumerable<RefUsrGroupe> Groupes
        {
            get { return this.UsrRole.Select(r => r.RefUsrAutorisation.RefUsrGroupe).Distinct(); }
        }

        public void RaiseAnyPropertyChanged(string prop)
        {
            this.RaisePropertyChanged(prop);
        }

        public bool IsAdministrateur
        {
            get
            {
                return this.ProfilAdmin;
            }
        }
    }
}
