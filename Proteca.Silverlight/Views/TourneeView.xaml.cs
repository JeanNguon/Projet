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
using Proteca.Silverlight.Helpers;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Tournee")]
    [ExportViewToRegion("Tournee", "MainContainer")]
    public partial class TourneeView : Page
    {
        public TourneeView()
        {
            InitializeComponent();

            this.Loaded += TourneeView_Loaded;
        }

        void TourneeView_Loaded(object sender, RoutedEventArgs e)
        {
            Grid myGrid = ((RadTileViewItem)((RadTileView)((Panel)(this.container.MainContent)).Children[0]).Items[1]).Content as Grid;
            ((ExportAndPrint)myGrid.Children[0]).GridView = myGrid.Children[1] as RadGridView;
        }
        
        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Tournee", "Tournee");
            }
        }

        private void RadGridViewEquipements_Drop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {

        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            // Commit automatiquement les modifications effectuées dans la grille des documents
            var allChildren = this.container.AllChildren();
            var gridEquip = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewEquipements");
            if (gridEquip != null)
            {
                ((RadGridView)gridEquip).CommitEdit();

                // Si le commit ne fonctionne pas (erreur sur le champ), on force le cancel Edit pour sortir du champ
                if (((RadGridView)gridEquip).CurrentCell != null && ((RadGridView)gridEquip).CurrentCell.IsInEditMode)
                {
                    ((RadGridView)gridEquip).CancelEdit();
                }
            }

            var gridPortion = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewPortions");
            if (gridPortion != null)
            {
                ((RadGridView)gridPortion).CommitEdit();

                // Si le commit ne fonctionne pas (erreur sur le champ), on force le cancel Edit pour sortir du champ
                if (((RadGridView)gridPortion).CurrentCell != null && ((RadGridView)gridPortion).CurrentCell.IsInEditMode)
                {
                    ((RadGridView)gridPortion).CancelEdit();
                }
            }
        }
    }
}
