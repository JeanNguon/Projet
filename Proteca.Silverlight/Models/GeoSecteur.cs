using System;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Models;

namespace Proteca.Web.Models
{
    public partial class GeoSecteur : IGeoCommun
    {
        public Uri NavigateURI
        {
            get
            {
                return new Uri("/" + MainNavigation.Administration.ToString() + "/" + AdministrationNavigation.DecoupageGeo.ToString() + "/" + EnumExtension.GetStringValue(Adm_DecoupageGeoNavigation.GeoSecteur) + "/Id=" + CleSecteur.ToString(), UriKind.Relative);
            }
        }
        
        private Boolean _isSelected = false;
        public Boolean IsSelected { get { return _isSelected; } set { _isSelected = value; RaisePropertyChanged("IsSelected"); } }


        private Boolean _isExpanded = false;
        public Boolean IsExpanded { get { return _isExpanded; } set { _isExpanded = value; RaisePropertyChanged("IsExpanded"); } }
        
        private Boolean _isChecked;
        public Boolean IsChecked { get { return _isChecked; } set { _isChecked = value; RaisePropertyChanged("IsChecked"); } }


    }
}
