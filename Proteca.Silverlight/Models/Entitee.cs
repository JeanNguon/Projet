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
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using Jounce.Core.ViewModel;
using Proteca.Silverlight.Models;
using System.Collections.Generic;
using Proteca.Silverlight.Enums.NavigationEnums;

namespace Proteca.Web.Models
{
    public class Entitee : BaseViewModel, IGeoCommun
    {
        public int Id = 0;

        public string Libelle { get; set; }

        public List<GeoRegion> _regions = new List<GeoRegion>();
        public List<GeoRegion> Regions { get { return _regions; } set { _regions = value; RaisePropertyChanged("Regions"); } }
        
        private Boolean _isSelected = false;
        public Boolean IsSelected { get { return _isSelected; } set { _isSelected = value; RaisePropertyChanged("IsSelected"); } }

        private Boolean _isExpanded = false;
        public Boolean IsExpanded { get { return _isExpanded; } set { _isExpanded = value; RaisePropertyChanged("IsExpanded"); } }

        public Uri NavigateURI
        {
            get
            {
                return new Uri("/" + MainNavigation.Administration.ToString() + "/" + AdministrationNavigation.DecoupageGeo.ToString(), UriKind.Relative);
            }
        }
    }
}
