using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class VisiteMesures : UserControl
    {
        public VisiteMesures()
        {
            InitializeComponent();
        }

        public Visite Visite
        {
            get { return (Visite)GetValue(VisiteProperty); }
            set { SetValue(VisiteProperty, value); }
        }

        public static readonly DependencyProperty VisiteProperty =
            DependencyProperty.Register("Visite", typeof(Visite), typeof(VisiteMesures), new PropertyMetadata(null));

        public Boolean IsEditMode
        {
            get { return (Boolean)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(Boolean), typeof(VisiteMesures), new PropertyMetadata(null));

        public Boolean HidePrecedent
        {
            get { return (Boolean)GetValue(HidePrecedentProperty); }
            set { SetValue(HidePrecedentProperty, value); }
        }

        public static readonly DependencyProperty HidePrecedentProperty =
            DependencyProperty.Register("HidePrecedent", typeof(Boolean), typeof(VisiteMesures), new PropertyMetadata(null));
    }
}
