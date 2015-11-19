using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Jounce.Core.Fluent;
using Jounce.Core.View;
using System.Collections.Generic;

namespace Proteca.Silverlight.Services.Contracts
{

    public interface INavigationService
    {
        Uri getUriById(int id);

        Uri getUriByFiltreId(int id,string filtre);

        void Navigate(int id);

        void Navigate(int id, bool forceReload);

        void Navigate(ViewNavigationArgs args);

        void Navigate(string viewName);

        void Navigate(string viewName, string parameterName, object param);

        void NavigateRootUrl();

        void NavigateUri(Uri uri);

        void NavigateUri(Uri uri, bool forceReload);
   
        void NavigateToPreviousView();
        
        void NavigateToNextView();

        string CurrentView
        {
            get;
        }

        string DefaultView
        {
            get;
        }

        Stack<Tuple<string, string, object>> VisitedViews
        {
            get;
        }
    }
}
