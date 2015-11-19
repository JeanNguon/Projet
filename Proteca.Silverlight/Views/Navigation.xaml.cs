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
using System.ComponentModel.Composition;
using Jounce.Core.Event;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce;
using Jounce.Framework;
using Proteca.Silverlight.Views.UserContols;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Enums.NavigationEnums;
using System.Windows.Browser;
using System.Text.RegularExpressions;

namespace Proteca.Silverlight.Views
{
    public partial class Navigation : IPartImportsSatisfiedNotification
    {

        /// <summary>
        /// Event aggregator
        /// </summary>
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        [Import(typeof(INavigationService))]
        public INavigationService NavService { get; set; }

        /// <summary>
        /// Navigation container (holds the region for target views)
        /// </summary>
        [Import]
        public NavigationContainer NavContainer { get; set; }

        private static string _lastView = string.Empty;

        public Navigation()
        {
            InitializeComponent();
            CompositionInitializer.SatisfyImports(this);
            if (NavContainer.Parent != null)
            {
                ((Grid)NavContainer.Parent).Children.Remove(NavContainer);
            }
            LayoutRoot.Children.Add(NavContainer);       
        }

        /// <summary>
        /// Executes when the user navigates to this page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter<Double>("MaxWidth", 280));
            EventAggregator.Publish("CustomTopContainer".AsViewNavigationArgs());
            CurrentNavigation.Current.SetNavigation(new Dictionary<String, String>(NavigationContext.QueryString));

            if (CurrentNavigation.Current.Module == MainNavigation.Accueil)
            {
                if (HtmlPage.IsEnabled && HtmlPage.Document.DocumentUri.AbsolutePath.ToLower().Contains("proteca.aspx"))
                {
                    HtmlPage.Window.Navigate(new Uri("/", UriKind.Relative));
                    return;
                }
                this.Visibility = System.Windows.Visibility.Collapsed;
                _lastView = CurrentNavigation.Current.Module.ToString();
            }
            else if (NavigationContext.QueryString.Any(p => p.Key.ToLower() == "pp")
                && CurrentNavigation.Current.Filtre is FiltreNavigation && ((FiltreNavigation)CurrentNavigation.Current.Filtre) == FiltreNavigation.PP
                && CurrentNavigation.Current.View is OuvrageNavigation && ((OuvrageNavigation)CurrentNavigation.Current.View) == OuvrageNavigation.Equipement)
            {
                HtmlPage.Window.Navigate(new Uri("/Pages/proteca.aspx#" + CurrentNavigation.Current.Url, UriKind.Relative));
                return;
            }
            else
            {
                if (HtmlPage.IsEnabled && !HtmlPage.Document.DocumentUri.AbsolutePath.ToLower().Contains("proteca.aspx"))
                {
                    HtmlPage.Window.Navigate(new Uri("/Pages/proteca.aspx#" + CurrentNavigation.Current.Url, UriKind.Relative));
                    return;
                }
                this.Visibility = System.Windows.Visibility.Visible;

                // Define NavContainer Title
                if (CurrentNavigation.Current.Fonction != null)
                {
                    NavContainer.Title = Resource.ResourceManager.GetString(CurrentNavigation.Current.Fonction.ToString());
                }
                else if (CurrentNavigation.Current.View != null)
                {
                    if (CurrentNavigation.Current.View is OuvrageNavigation && ((OuvrageNavigation)CurrentNavigation.Current.View) == OuvrageNavigation.Equipement && CurrentNavigation.Current.Filtre != null)
                    {
                        NavContainer.Title = Resource.ResourceManager.GetString(string.Format("{0}_{1}", 
                            CurrentNavigation.Current.View.ToString(), CurrentNavigation.Current.Filtre.ToString()));
                    }
                    else if (CurrentNavigation.Current.View is AdministrationNavigation && ((AdministrationNavigation)CurrentNavigation.Current.View) == AdministrationNavigation.Ressources && CurrentNavigation.Current.Filtre != null)
                    {
                        NavContainer.Title = Resource.ResourceManager.GetString(string.Format("{0}_{1}",
                            CurrentNavigation.Current.View.ToString(), CurrentNavigation.Current.Filtre.ToString()));
                    }
                    else
                    {
                        NavContainer.Title = Resource.ResourceManager.GetString(CurrentNavigation.Current.View.ToString());
                    }
                }

                // Navigate to View
                if (CurrentNavigation.Current.View != null)
                {
                    _lastView = CurrentNavigation.Current.View.ToString();

                    if (CurrentNavigation.Current.View is OuvrageNavigation && ((OuvrageNavigation)CurrentNavigation.Current.View) == OuvrageNavigation.Equipement && CurrentNavigation.Current.Filtre != null)
                    {
                        _lastView = string.Format("{0}_{1}", _lastView, CurrentNavigation.Current.Filtre.ToString());
                    }
                    else if (CurrentNavigation.Current.View is AdministrationNavigation && ((AdministrationNavigation)CurrentNavigation.Current.View) == AdministrationNavigation.Ressources && CurrentNavigation.Current.Filtre != null)
                    {
                        _lastView = string.Format("{0}_{1}", _lastView, CurrentNavigation.Current.Filtre.ToString());
                    }

                    int id;
                    if (!String.IsNullOrEmpty(CurrentNavigation.Current.Selection))
                    {
                        if (CurrentNavigation.Current.Selection.ToLower() == "new")
                        {
                            NavService.Navigate(_lastView, Global.Constants.PARM_STATE, Global.Constants.STATE_NEW);
                        }
                        else if (CurrentNavigation.Current.Selection.ToLower() == "edit")
                        {
                            NavService.Navigate(_lastView, Global.Constants.PARM_STATE, Global.Constants.STATE_EDIT);
                        }
                        else
                        {
                            NavService.Navigate(_lastView, Global.Constants.PARM_ID, int.TryParse(Regex.Match(CurrentNavigation.Current.Selection, @"\d+").Value, out id) ? id : 0);
                        }
                    }
                    else if (!String.IsNullOrEmpty(CurrentNavigation.Current.SelectionTmp))
                    {
                        NavService.Navigate(_lastView, Global.Constants.PARM_ID_TMP, int.TryParse(Regex.Match(CurrentNavigation.Current.SelectionTmp, @"\d+").Value, out id) ? id : 0);
                    }
                    else if (CurrentNavigation.Current.Module == MainNavigation.Search && NavigationContext.QueryString.ContainsKey("k"))
                    {
                        CurrentNavigation.Current.Selection = NavigationContext.QueryString["k"];
                        NavService.Navigate(_lastView, Global.Constants.PARM_SEARCH_TEXT, CurrentNavigation.Current.Selection);
                    }
                    else
                    {
                        NavService.Navigate(_lastView);
                    }
                }
            }

            // Publish Navigation context
            EventAggregator.Publish(NavigationContext);
            EventAggregator.Publish(CurrentNavigation.Current);
        }

        public void OnImportsSatisfied()
        {
            if (string.IsNullOrEmpty(_lastView))
            {
                _lastView = NavService.DefaultView;
                NavService.Navigate(_lastView);
            }
        }
    }
}
