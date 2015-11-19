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
using Proteca.Silverlight.Enums.NavigationEnums;
using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Enums;
using System.Text;

namespace Proteca.Silverlight.Helpers
{
    public class CurrentNavigation
    {
        public MainNavigation Module { get; set; }
        public Enum Fonction { get; set; }
        public Enum View { get; set; }
        public Enum Filtre { get; set; }
        public String Selection { get; set; }
        public String SelectionTmp { get; set; }
        
        #region Singleton

        private static CurrentNavigation _current;
        public static CurrentNavigation Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new CurrentNavigation();
                }
                return _current;
            }
        }

        #endregion

        private CurrentNavigation()
        {
        }

        public void SetNavigation(Dictionary<String, String> queryString)
        {
            if (queryString.Any(p => p.Key == "module"))
            {
                Enum tmpModul;
                String query = queryString.First(p => p.Key == "module").Value;
                tmpModul = typeof(MainNavigation).FindByStringValue(query);
                if (tmpModul != null)
                {
                    Module = (MainNavigation)tmpModul;
                }
            }

            Filtre = null;
            Enum tmpFiltre;
            String queryFiltre = String.Empty;
            if (queryString.Any(p => p.Key == "filter"))
            {
                queryFiltre = queryString.First(p => p.Key == "filter").Value;
                
            }
            else if (queryString.Any(p => p.Key == "view"))
            {
                queryFiltre = queryString.First(p => p.Key == "view").Value;
            }

            tmpFiltre = typeof(FiltreNavigation).FindByStringValue(queryFiltre);
            if (tmpFiltre != null)
            {
                Filtre = (FiltreNavigation)tmpFiltre;
            }


            Fonction = null;
            Type FonctionEnumType;
            if (queryString.Any(p => p.Key == "fonction") && queryString.Any(p => p.Key == "view")
            && (queryString.Any(p => p.Key == "filter") || Filtre == null) )
            {
                Enum tmpfonction;
                String query = queryString.First(p => p.Key == "fonction").Value;
                switch (Module)
                {
                    case MainNavigation.GestionOuvrages:
                        FonctionEnumType = typeof(OuvrageNavigation);
                        break;
                    case MainNavigation.Visite:
                        FonctionEnumType = typeof(VisiteNavigation);
                        break;
                    case MainNavigation.AnalyseRestitution:
                        FonctionEnumType = typeof(AnalyseRestitutionNavigation);
                        break;
                    case MainNavigation.Administration:
                        FonctionEnumType = typeof(AdministrationNavigation);
                        break;
                    case MainNavigation.Parametres:
                        FonctionEnumType = typeof(ParametresNavigation);
                        break;
                    default:
                        FonctionEnumType = typeof(OuvrageNavigation);
                        break;
                }
                tmpfonction= FonctionEnumType.FindByStringValue(query);
                if (tmpfonction != null)
                {
                    Fonction = tmpfonction;
                }
            }

            View = null;
            Type ViewType = null;
            String viewStr = String.Empty;
            if (queryString.Any(p => p.Key == "view") && (queryString.Any(p => p.Key == "filter") || Filtre == null))
            {
                viewStr = queryString.First(p => p.Key == "view").Value;
            }
            else if (queryString.Any(p => p.Key == "fonction"))
            {
                viewStr = queryString.First(p => p.Key == "fonction").Value;
            }

            if (!String.IsNullOrEmpty(viewStr))
            {
                if (Fonction == null)
                {
                    switch (Module)
                    {
                        case MainNavigation.GestionOuvrages:
                            ViewType = typeof(OuvrageNavigation);
                            break;
                        case MainNavigation.Visite:
                            ViewType = typeof(VisiteNavigation);
                            break;
                        case MainNavigation.AnalyseRestitution:
                            ViewType = typeof(AnalyseRestitutionNavigation);
                            break;
                        case MainNavigation.Administration:
                            ViewType = typeof(AdministrationNavigation);
                            break;
                        case MainNavigation.Parametres:
                            ViewType = typeof(ParametresNavigation);
                            break;
                        case MainNavigation.Search:
                            ViewType = typeof(SearchNavigation);
                            break;
                        case MainNavigation.Odima:
                            ViewType = typeof(OdimaNavigation);
                            break;
                        default:
                            ViewType = typeof(OuvrageNavigation);
                            break;
                    }
                }
                else if (Fonction is AdministrationNavigation)
                {
                    switch ((AdministrationNavigation)Fonction)
                    {
                        case AdministrationNavigation.RegoupementRegion:
                        case AdministrationNavigation.DeplacementPp:
                        case AdministrationNavigation.DecoupagePortion:
                            break;
                        case AdministrationNavigation.DecoupageGeo:
                            ViewType = typeof(Adm_DecoupageGeoNavigation);
                            break;
                    }
                }
                if (ViewType != null)
                {
                    View = ViewType.FindByStringValue(viewStr);
                }
            }

            Selection = "";
            if (queryString.Any(p => p.Key == "selection"))
            {
                Selection = queryString.First(p => p.Key == "selection").Value.ToString();
            }

            SelectionTmp = "";
            if (queryString.Any(p => p.Key == "selectionTmp"))
            {
                SelectionTmp = queryString.First(p => p.Key == "selectionTmp").Value.ToString();
            }

            //Gestion des liens d'entrée Micado
            if (queryString.Any(p => p.Key.ToLower() == "pp") && View == null)
            {
                Module = MainNavigation.GestionOuvrages;
                View = OuvrageNavigation.Equipement;
                Filtre = FiltreNavigation.PP;
                Selection = queryString.First(p => p.Key.ToLower() == "pp").Value;
            }
            else
            if (queryString.Any(p => p.Key.ToLower() == "equipement") && View == null)
            {
                Module = MainNavigation.GestionOuvrages;
                View = OuvrageNavigation.Equipement;
                Selection = queryString.First(p => p.Key.ToLower() == "equipement").Value;
            }
        }

        public String Url
        {
            get
            {
                StringBuilder url = new StringBuilder(BaseUrl);                
                if (!String.IsNullOrEmpty(Selection))
                {
                    url.Append("/Id=" + Selection);
                }
                else if (!String.IsNullOrEmpty(SelectionTmp))
                {
                    url.Append("/IdTmp=" + SelectionTmp);
                }
                return url.ToString();
            }
        }

        public String BaseUrl
        {
            get
            {
                StringBuilder url = new StringBuilder();

                url.Append("/" + Module.GetStringValue());

                if (Fonction != null)
                {
                    url.Append("/" + Fonction.GetStringValue());
                }
                if (View != null)
                {
                    url.Append("/" + View.GetStringValue());
                }
                if (Filtre != null)
                {
                    url.Append("/" + Filtre.GetStringValue());
                }
                return url.ToString();
            }
        }

        public String BaseUrlWithOutFilter
        {
            get
            {
                StringBuilder url = new StringBuilder();
               
                url.Append("/" + Module.GetStringValue());

                if (Fonction != null)
                {
                    url.Append("/" + Fonction.GetStringValue());
                }
                if (View != null)
                {
                    url.Append("/" + View.GetStringValue());
                }
                return url.ToString();
            }
        }
    }
}
