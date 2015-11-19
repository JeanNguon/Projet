using System;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Models;
using System.Linq;

namespace Proteca.Web.Models
{
    public partial class GeoAgence : Entity, IGeoCommun
    {
        public Uri NavigateURI
        {
            get
            {
                return new Uri("/" + MainNavigation.Administration.GetStringValue() + "/" + AdministrationNavigation.DecoupageGeo.GetStringValue() + "/" + Adm_DecoupageGeoNavigation.GeoAgence.GetStringValue() + "/Id=" + CleAgence.ToString(), UriKind.Relative);
             }
        }

        private Boolean _isSelected = false;
        public Boolean IsSelected { get { return _isSelected; } set { _isSelected = value; RaisePropertyChanged("IsSelected"); } }

        private Boolean _isExpanded = false;
        public Boolean IsExpanded { get { return _isExpanded; } set { _isExpanded = value; RaisePropertyChanged("IsExpanded"); } }

        private bool _isLoaded = false;
        public List<GeoSecteur> SecteursTries
        {
            get
            {
                if (!_isLoaded)
                {
                    this.GeoSecteur.EntityAdded += (o, e) =>
                    {
                        this.RaisePropertyChanged("SecteursTries");
                    };
                    this.GeoSecteur.EntityRemoved += (o, e) =>
                    {
                        this.RaisePropertyChanged("SecteursTries");
                    };
                    _isLoaded = true;
                }
                return this.GeoSecteur.OrderBy(s => s.LibelleSecteur).ToList();
            }
        }
    }
}
