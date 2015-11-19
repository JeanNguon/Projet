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
using Jounce.Core.View;
using Jounce.Regions.Core;
using Jounce.Core.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Markup;
using System.Threading;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("MesModeleMesure")]
    [ExportViewToRegion("MesModeleMesure", "MainContainer")]
    public partial class MesModeleMesureView : Page
    {

        public MesModeleMesureView()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("MesModeleMesure", "MesModeleMesure");
            }
        }

    }
}
