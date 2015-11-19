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
using Proteca.Silverlight.Views.UserContols;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Documentation")]
    [ExportViewToRegion("Documentation", "MainContainer")]
    public partial class DocumentationView : Page
    {

        public DocumentationView()
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
                return ViewModelRoute.Create("Documentation", "Documentation");
            }
        }

        /// <summary>
        /// Workaround pour corriger le bug de l'hyperlinkbutton avec l'url externe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton button = (HyperlinkButton)sender;
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(button.Tag.ToString()), "_blank");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((Panel)container.MainContent).ChildrenOfType<ExportAndPrint>().First().GridView = ((Panel)container.MainContent).ChildrenOfType<CustomGridView>().First();      
        }

        private void container_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
