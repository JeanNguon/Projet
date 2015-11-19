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
using Jounce.Core.View;
using Jounce.Regions.Core;
using System.ComponentModel.Composition;
using Jounce.Core.ViewModel;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Proteca.Silverlight.Models;
using Proteca.Web.Models;
using System.Collections.ObjectModel;

namespace Proteca.Silverlight.Views
{
    /// <summary>
    /// Vue du treeview utilsisé dans le découpage géographique
    /// </summary>
    [ExportAsView("TreeViewGeo")]
    [ExportViewToRegion("TreeViewGeo", "ExpanderContainer")]
    public partial class TreeViewGeoView : UserControl
    {
        public TreeViewGeoView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gére l'affichage du menu contextuel suivant le type d'élément sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            RadTreeViewItem treeViewItem = (sender as RadContextMenu).GetClickedElement<RadTreeViewItem>();
            if (treeViewItem == null)
            {
                (sender as RadContextMenu).IsOpen = false;
                return;
            }

            if (!treeViewItem.IsSelected)
            {
                GeoTree.SelectedItems.Clear();
                GeoTree.SelectedItems.Add(treeViewItem.Item);
            }

            if (treeViewItem.Item is Entitee)
            {
                AddRegion.Visibility = Visibility.Visible;
                AddAgence.Visibility = Visibility.Collapsed;
                AddSecteur.Visibility = Visibility.Collapsed;
                EditEntity.Visibility = Visibility.Collapsed;
                DeleteEntity.Visibility = Visibility.Collapsed;
            }
            else if (treeViewItem.Item is GeoRegion)
            {
                AddRegion.Visibility = Visibility.Collapsed;
                AddAgence.Visibility = Visibility.Visible;
                AddSecteur.Visibility = Visibility.Collapsed;
                EditEntity.Visibility = Visibility.Visible;
                DeleteEntity.Visibility = Visibility.Visible;
            }
            else if (treeViewItem.Item is GeoAgence)
            {
                AddRegion.Visibility = Visibility.Collapsed;
                AddAgence.Visibility = Visibility.Collapsed;
                AddSecteur.Visibility = Visibility.Visible;
                EditEntity.Visibility = Visibility.Visible;
                DeleteEntity.Visibility = Visibility.Visible;
            }
            else if (treeViewItem.Item is GeoSecteur)
            {
                AddRegion.Visibility = Visibility.Collapsed;
                AddAgence.Visibility = Visibility.Collapsed;
                AddSecteur.Visibility = Visibility.Collapsed;
                EditEntity.Visibility = Visibility.Visible;
                DeleteEntity.Visibility = Visibility.Visible;
            }
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("TreeViewGeo", "TreeViewGeo");
            }
        }
    }
}
