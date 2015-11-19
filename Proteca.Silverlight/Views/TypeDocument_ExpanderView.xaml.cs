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
using Proteca.Silverlight.Resources;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("TypeDocument_Expander")]
    [ExportViewToRegion("TypeDocument_Expander", "ExpanderContainer")]

    public partial class TypeDocument_ExpanderView : UserControl
    {

        public TypeDocument_ExpanderView()
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
                FoldersTree.SelectedItems.Clear();
                FoldersTree.SelectedItems.Add(treeViewItem.Item);
            }

            switch (treeViewItem.Level)
            {
                case 0:
                    AddEntity.Header = Resource.MenuContextAddFolder;
                    AddEntity.Visibility = Visibility.Visible;
                    EditEntity.Visibility = Visibility.Collapsed;
                    DeleteEntity.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    AddEntity.Header = Resource.MenuContextAddDesignation;
                    AddEntity.Visibility = Visibility.Visible;
                    EditEntity.Visibility = Visibility.Visible;
                    DeleteEntity.Visibility = Visibility.Visible;
                    break;
                case 2:
                    AddEntity.Visibility = Visibility.Collapsed;
                    EditEntity.Visibility = Visibility.Visible;
                    DeleteEntity.Visibility = Visibility.Visible;
                    break;
            }
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("TypeDocument", "TypeDocument_Expander");
            }
        }

    }
}
