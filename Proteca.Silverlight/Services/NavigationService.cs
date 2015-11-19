using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Jounce.Core.Event;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Silverlight.Services.Contracts;
using System.Windows;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Enums.NavigationEnums;

namespace Proteca.Silverlight.Services
{
    /// <summary>
    /// Permet de factoriser la gestion de la navigation.
    /// </summary>
    [Export(typeof(INavigationService))]
    public class NavigationService : INavigationService
    {
        #region Services

        [Import]
        public IEventAggregator EventAggregator { get; set; }

        [Import]
        public IViewModelRouter ViewModelRouter { get; set; }

        #endregion

        #region Constructor
        public NavigationService()
        {
            VisitedViews = new Stack<Tuple<string, string, object>>();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Liste toutes les vues disponibles
        /// </summary>
        public List<string> Views
        {
            get
            {
                List<string> views = new List<string>();
                if (ViewModelRouter != null && ViewModelRouter.RouteList != null && ViewModelRouter.RouteList.Any())
                {
                    ViewModelRouter.RouteList.Select(r => r.ViewType).ToList();
                }
                return views;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Uri GetCurrentUri
        {
            get { return ((MainPage)Application.Current.RootVisual).ContentFrame.Source; }
        }

        /// <summary>
        /// Page actuellement affichée
        /// </summary>
        private string currentView;
        public string CurrentView
        {
            get
            {
                return currentView;
            }
            private set
            {
                currentView = value;
            }
        }

        /// <summary>
        /// Page par défaut (exemple : page d'accueil, page de login, etc)
        /// </summary>
        public string DefaultView
        {
            get
            {
                return "Home";
            }
        }

        //Enregistre les pages visitées dans une pile
        public Stack<Tuple<string, string, object>> VisitedViews
        {
            get;
            set;
        }
        #endregion

        #region Methods

        public Uri getUriById(int id)
        {
            return new Uri(CurrentNavigation.Current.BaseUrl + "/Id=" + id.ToString(), UriKind.Relative);
        }

        public Uri getUriByFiltreId(int id,string filtre)
        {
            //if (filtre == "EQ")
            //    CurrentNavigation.Current.Filtre = FiltreNavigation.EQ;
            //else
            //    CurrentNavigation.Current.Filtre = FiltreNavigation.PP;

            return new Uri(CurrentNavigation.Current.BaseUrlWithOutFilter +"/"+ filtre + "/Id=" + id.ToString(), UriKind.Relative);
        }

        public void Navigate(int id, string filtre, bool forceReload = false)
        {
            NavigateUri(getUriByFiltreId(id, filtre), forceReload);
        }

        public void Navigate(int id, string filtre)
        {
            NavigateUri(getUriByFiltreId(id, filtre), false);
        }

        public void Navigate(int id)
        {
            Navigate(id, false);
        }

        public void Navigate(int id, bool forceReload)
        {
            if (CurrentNavigation.Current.View != null)
            {
                if (GetCurrentUri.OriginalString.ToLower().Contains(CurrentNavigation.Current.BaseUrl.ToLower()))
                {

                    //if (CurrentNavigation.Current.Url.ToLower().Contains("id="))
                    //{
                    //    string[] separator = new string[] { "id=" };
                    //    string[] split = CurrentNavigation.Current.Url.ToLower().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    //    string splitId = split.LastOrDefault();
                    //    id = Convert.ToInt32(splitId);
                    //}
                        
                        NavigateUri(getUriById(id), forceReload);
                    
                  //  else
                  //  {
                  //      NavigateUri(new Uri(GetCurrentUri.OriginalString,UriKind.Relative), forceReload);
                  //  }
                    
                }
            }
            else
            {
                Navigate(CurrentView, Global.Constants.PARM_ID, id);
            }
        }

        public void Navigate(string viewName)
        {
            Navigate(viewName, null, null);
        }

        public void Navigate(string viewName, string parameterName, object param)
        {
            ViewNavigationArgs args = viewName.AsViewNavigationArgs();
            Navigate(args, parameterName, param);
        }

        public void Navigate(ViewNavigationArgs args)
        {
            Navigate(args, null, null);
        }

        private void Navigate(ViewNavigationArgs args, string parameterName = null, object param = null)
        {
            if (CurrentView != args.ViewType)
            {
                String oldView = CurrentView;
                CurrentView = args.ViewType;
                ViewModelRouter.DeactivateView(oldView);
            }

            VisitedViews.Push(new Tuple<string, string, object>(args.ViewType, parameterName, param));
            
            if (parameterName == null)
            {
                if (args != null && CurrentView != null)
                {
                    EventAggregator.Publish(args);
                }
                else
                {
                    // TODO log
                }
            }
            else
            {
                if (args != null && CurrentView != null)
                {
                    EventAggregator.Publish(args.AddNamedParameter(parameterName, param));
                }
                else
                {
                    // TODO log
                }
            }
        }

        /// <summary>
        /// Navigate vers l'url principale
        /// </summary>
        public void NavigateRootUrl()
        {
            if (CurrentNavigation.Current.View != null &&
                GetCurrentUri.OriginalString.ToLower().Contains(CurrentNavigation.Current.BaseUrl.ToLower()) &&
                GetCurrentUri.OriginalString.ToLower() != CurrentNavigation.Current.BaseUrl.ToLower())
            {
                NavigateUri(new Uri(CurrentNavigation.Current.BaseUrl, UriKind.Relative));
            }
        }

        /// <summary>
        /// Navigue vers l'URL indiquée en paramètre
        /// </summary>
        /// <param name="uri">Url vers où naviguer</param>
        public void NavigateUri(Uri uri)
        {
            NavigateUri(uri, false);
        }

        /// <summary>
        /// Navigue vers l'URL indiquée en paramètre
        /// </summary>
        /// <param name="uri">Url vers où naviguer</param>
        /// <param name="forceReload">Force le rafraichissement de la page si page identique</param>
        public void NavigateUri(Uri uri, bool forceReload)
        {
            if (Application.Current.RootVisual is MainPage)
            {
                if (((MainPage)Application.Current.RootVisual).ContentFrame.Source != uri)
                {
                    ((MainPage)Application.Current.RootVisual).ContentFrame.Navigate(uri);
                }
                else if (forceReload)
                {
                    ((MainPage)Application.Current.RootVisual).ContentFrame.Refresh();
                }
            }
        }

        /// <summary>
        /// Navigue vers la page précédente
        /// </summary>
        public void NavigateToPreviousView()
        {
            Tuple<string, string, object> lastView = VisitedViews.Pop();
            Navigate(lastView.Item1, lastView.Item2, lastView.Item3);
        }

        //TODO: à implémenter
        public void NavigateToNextView()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Appelle la méthode désactivate View du ViewModel courant
        /// </summary>
        public void DesactivateCurrentView()
        {
            ViewModelRouter.DeactivateView(CurrentView);
        }

        #endregion

    }
}
