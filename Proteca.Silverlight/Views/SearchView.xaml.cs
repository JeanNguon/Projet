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
using System.Windows.Browser;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Search")]
    [ExportViewToRegion("Search", "MainContainer")]
    public partial class SearchView : Page
    {
        public String UrlToRedirect
        {
            get { return (String)GetValue(UrlToRedirectProperty); }
            set { SetValue(UrlToRedirectProperty, value); }
        }

        public static readonly DependencyProperty UrlToRedirectProperty =
            DependencyProperty.Register("UrlToRedirect", typeof(String), typeof(SearchView), new PropertyMetadata(null));

        public SearchView()
        {
            InitializeComponent();
            HtmlPage.RegisterScriptableObject("SearchView", this);
        }

        [ScriptableMember]
        public void Redirect(string link)
        {
            UrlToRedirect = link.Replace("#", "/Pages/proteca.aspx#");
            HtmlPage.Window.Navigate(new Uri(UrlToRedirect, UriKind.Relative));
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Search", "Search");
            }
        }
        
        
    }
}
