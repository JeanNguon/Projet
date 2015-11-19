using System;
using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Models;

namespace Proteca.Web.Models
{
    public partial class GeoRegion : IGeoCommun
    {

        public Uri NavigateURI
        {
            get
            {
                return new Uri("/" + MainNavigation.Administration.ToString() + "/" + AdministrationNavigation.DecoupageGeo.ToString() + "/" + EnumExtension.GetStringValue(Adm_DecoupageGeoNavigation.GeoRegion) + "/Id=" + CleRegion.ToString(), UriKind.Relative);
            }
        }

        private Boolean _isSelected = false;
        public Boolean IsSelected { get { return _isSelected; } set { _isSelected = value; RaisePropertyChanged("IsSelected"); } }

        private Boolean _isExpanded = false;
        public Boolean IsExpanded { get { return _isExpanded; } set { _isExpanded = value; RaisePropertyChanged("IsExpanded"); } }
        
        private bool _isLoaded = false;
        public List<GeoAgence> AgencesTriees 
        {
            get
            {
                if (!_isLoaded)
                {
                    this.GeoAgence.EntityAdded += (o, e) =>
                    {
                        this.RaisePropertyChanged("AgencesTriees");
                    };
                    this.GeoAgence.EntityRemoved += (o, e) =>
                    {
                        this.RaisePropertyChanged("AgencesTriees");
                    };
                    _isLoaded = true;
                }
                return this.GeoAgence.OrderBy(a => a.LibelleAgence).ToList();
            } 
        }
    }
}
