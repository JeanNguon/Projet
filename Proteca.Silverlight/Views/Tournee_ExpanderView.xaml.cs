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
using System.Windows.Navigation;
using Jounce.Core.ViewModel;
using System.ComponentModel.Composition;
using Jounce.Core.View;
using Jounce.Regions.Core;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Tournee_Expander")]
    [ExportViewToRegion("Tournee_Expander", "ExpanderContainer")]
    public partial class Tournee_ExpanderView : Page
    {
        public Tournee_ExpanderView()
        {
            InitializeComponent();
        }

        // S'exécute lorsque l'utilisateur navigue vers cette page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Tournee", "Tournee_Expander");
            }
        }
    }
}
