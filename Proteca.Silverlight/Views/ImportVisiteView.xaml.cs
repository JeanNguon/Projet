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
using Telerik.Windows.Controls;
using Proteca.Silverlight.Views.UserContols;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("ImportVisite")]
    [ExportViewToRegion("ImportVisite", "MainContainer")]
    public partial class ImportVisiteView : Page
    {

        public ImportVisiteView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ImportVisiteView_Loaded);
        }

        void ImportVisiteView_Loaded(object sender, RoutedEventArgs e)
        {
            ((Panel)container.MainContent).ChildrenOfType<ExportAndPrint>().First().GridView = ((Panel)container.MainContent).ChildrenOfType<CustomGridView>().First();
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
                return ViewModelRoute.Create("ImportVisite", "ImportVisite");
            }
        }

    }
}
