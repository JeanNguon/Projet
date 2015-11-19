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
    public partial class GraphiqueMesures : UserControl
    {
        public GraphiqueMesures()
        {
            InitializeComponent();
        }

        public Graphique Graphique
        {
            get { return (Graphique)GetValue(GraphiqueProperty); }
            set { SetValue(GraphiqueProperty, value); }
        }

        public static readonly DependencyProperty GraphiqueProperty =
            DependencyProperty.Register("Graphique", typeof(Graphique), typeof(GraphiqueMesures), new PropertyMetadata(null));
    }
}
