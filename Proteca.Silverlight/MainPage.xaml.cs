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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Proteca.Silverlight.ViewModels;
using Proteca.Silverlight.Models;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Browser;
using System.Reflection;
using System.Windows.Markup;
using System.Threading;


namespace Proteca.Silverlight
{
    [ExportAsView("Main", IsShell = true)]
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        
        public String Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                String version = assembly.FullName.Split(',')[1];
                String fullversion = version.Split('=')[1];
                return "V" + fullversion;
            }
        }

        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (HtmlPage.IsEnabled && (sender == null || !(sender is NavigationService) || ((NavigationService)sender).CurrentSource == null))
            {
                String url = HtmlPage.Document.DocumentUri.Fragment.Replace("#", "");
                if (!String.IsNullOrEmpty(url) && url != e.Uri.OriginalString)
                {
                    e.Cancel = true;
                    ContentFrame.Navigate(new Uri(url, UriKind.Relative));
                }
            }
        }

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ErrorWindow.CreateNew(e.Exception);

        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Main", "Main");
            }
        }
    }
}