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
using System.ServiceModel.DomainServices.Client;
using System.Xml.Linq;

namespace Proteca.Web.Models
{
    public interface IOuvrage
    {
        void ForceRaisePropertyChanged(String propertyName);
        Visite LastVisite { get; }

        EntityCollection<Visite> Visites { get; }

        DateTime? VisitePeriodeDebut { get; set; }
        DateTime? VisitePeriodeFin { get; set; }

        string Libelle { get; set; }

        Entity GetHisto();
        object GetIdentity();

        XElement CreateXElement();

        Composition Composition { get; }
        Pp PpAttachee { get; }

        String CodeEquipement { get; }

        String LibelleCheminGeo { get; }

        String LibelleExtended { get; }
    }

}
