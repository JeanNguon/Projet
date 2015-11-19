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
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Jounce.Core.Model;

namespace Proteca.Web.Models
{
    public class PointGraphique
    {
        private decimal _x;
        private decimal? _y;

        public Decimal X 
        {
            get { return _x; }
            set { _x = value; } 
        }

        public Nullable<Decimal> Y
        {
            get { return _y; }
            set { _y = value; }
        }
    }
}
