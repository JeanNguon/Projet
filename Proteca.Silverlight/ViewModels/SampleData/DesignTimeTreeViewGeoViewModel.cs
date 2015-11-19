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
using Proteca.Silverlight.Models;
using Proteca.Web.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;

namespace Proteca.Silverlight.ViewModels.SampleData
{
    public class DesignTimeTreeViewGeoViewModel
    {
        public DesignTimeTreeViewGeoViewModel()
        {
            GeoSecteur sec1 = new GeoSecteur() { CodeSecteur = "S1", LibelleSecteur = "GeoSecteur 1", LibelleAbregeSecteur = "Sect1" };
            GeoSecteur sec2 = new GeoSecteur() { CodeSecteur = "S2", LibelleSecteur = "GeoSecteur 2", LibelleAbregeSecteur = "Sect2" };
            GeoSecteur sec3 = new GeoSecteur() { CodeSecteur = "S3", LibelleSecteur = "GeoSecteur 3", LibelleAbregeSecteur = "Sect3" };
            GeoSecteur sec4 = new GeoSecteur() { CodeSecteur = "S4", LibelleSecteur = "GeoSecteur 4", LibelleAbregeSecteur = "Sect4" };
            GeoSecteur sec5 = new GeoSecteur() { CodeSecteur = "S5", LibelleSecteur = "GeoSecteur 5", LibelleAbregeSecteur = "Sect5" };
            GeoSecteur sec6 = new GeoSecteur() { CodeSecteur = "S6", LibelleSecteur = "GeoSecteur 6", LibelleAbregeSecteur = "Sect6"};
            GeoSecteur sec7 = new GeoSecteur() { CodeSecteur = "S7", LibelleSecteur = "GeoSecteur 7", LibelleAbregeSecteur = "Sect7" };
            GeoSecteur sec8 = new GeoSecteur() { CodeSecteur = "S8", LibelleSecteur = "GeoSecteur 8", LibelleAbregeSecteur = "Sect8" };
            GeoSecteur sec9 = new GeoSecteur() { CodeSecteur = "S9", LibelleSecteur = "GeoSecteur 9", LibelleAbregeSecteur = "Sect9"};
            GeoSecteur sec10 = new GeoSecteur() { CodeSecteur = "S10", LibelleSecteur = "GeoSecteur 10", LibelleAbregeSecteur = "Sect10" };
            GeoSecteur sec11 = new GeoSecteur() { CodeSecteur = "S11", LibelleSecteur = "GeoSecteur 11", LibelleAbregeSecteur = "Sect11" };
            GeoSecteur sec12 = new GeoSecteur() { CodeSecteur = "S12", LibelleSecteur = "GeoSecteur 12", LibelleAbregeSecteur = "Sect12"};
            GeoSecteur sec13 = new GeoSecteur() { CodeSecteur = "S13", LibelleSecteur = "GeoSecteur 13", LibelleAbregeSecteur = "Sect13" };
            GeoSecteur sec14 = new GeoSecteur() { CodeSecteur = "S14", LibelleSecteur = "GeoSecteur 14", LibelleAbregeSecteur = "Sect14" };
            GeoSecteur sec15 = new GeoSecteur() { CodeSecteur = "S15", LibelleSecteur = "GeoSecteur 15", LibelleAbregeSecteur = "Sect15"};
            
            GeoAgence ag1 = new GeoAgence() { CodeAgence = "A1", LibelleAgence = "GeoAgence 1", LibelleAbregeAgence = "Ag1"};
            GeoAgence ag2 = new GeoAgence() { CodeAgence = "A2", LibelleAgence = "GeoAgence 2", LibelleAbregeAgence = "Ag2"};
            GeoAgence ag3 = new GeoAgence() { CodeAgence = "A3", LibelleAgence = "GeoAgence 3", LibelleAbregeAgence = "Ag3"};
            GeoAgence ag4 = new GeoAgence() { CodeAgence = "A4", LibelleAgence = "GeoAgence 4", LibelleAbregeAgence = "Ag4"};
            GeoAgence ag5 = new GeoAgence() { CodeAgence = "A5", LibelleAgence = "GeoAgence 5", LibelleAbregeAgence = "Ag5"};
            GeoAgence ag6 = new GeoAgence() { CodeAgence = "A6", LibelleAgence = "GeoAgence 6", LibelleAbregeAgence = "Ag6"};
            GeoAgence ag7 = new GeoAgence() { CodeAgence = "A7", LibelleAgence = "GeoAgence 7", LibelleAbregeAgence = "Ag7" };

            GeoRegion reg1 = new GeoRegion() { CodeRegion = "R1", LibelleRegion = "Région 1", LibelleAbregeRegion = "Reg1"  };
            GeoRegion reg2 = new GeoRegion() { CodeRegion = "R2", LibelleRegion = "Région 2", LibelleAbregeRegion = "Reg2"  };
            GeoRegion reg3 = new GeoRegion() { CodeRegion = "R3", LibelleRegion = "Région 3", LibelleAbregeRegion = "Reg3" };
            GeoRegion reg4 = new GeoRegion() { CodeRegion = "R4", LibelleRegion = "Région 4", LibelleAbregeRegion = "Reg4" };

            SelectedItem = TreeViewGeo.First();
        }

        public ObservableCollection<Entity> TreeViewGeo { get; set; }

        public Entity SelectedItem { get; set; }
    }
}
