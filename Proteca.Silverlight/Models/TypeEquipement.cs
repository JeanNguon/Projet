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
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Enums;

namespace Proteca.Web.Models
{
    public partial class TypeEquipement
    {
        public Boolean IsSelected
        {
            get { return CurrentNavigation.Current.Filtre != null && this.CodeEquipement == CurrentNavigation.Current.Filtre.GetStringValue(); }
            set { RaisePropertyChanged("IsSelected"); }
        }
    }
}
