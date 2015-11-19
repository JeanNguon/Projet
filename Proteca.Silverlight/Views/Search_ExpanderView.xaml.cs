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
    [ExportAsView("Search_Expander")]
    [ExportViewToRegion("Search_Expander", "ExpanderContainer")]
    public partial class Search_ExpanderView : Page
    {
        public Search_ExpanderView()
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
                return ViewModelRoute.Create("Search", "Search_Expander");
            }
        }
    }
}
