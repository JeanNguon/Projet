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
    public partial class HistoriqueVisite : UserControl
    {
        public HistoriqueVisite()
        {
            InitializeComponent();
        }

        public Visite Visite
        {
            get { return (Visite)GetValue(VisiteProperty); }
            set { SetValue(VisiteProperty, value); }
        }

        public static readonly DependencyProperty VisiteProperty =
            DependencyProperty.Register("Visite", typeof(Visite), typeof(HistoriqueVisite), new PropertyMetadata(null));
    }
}
