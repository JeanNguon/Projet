using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;
using Proteca.Silverlight.Helpers;
using Telerik.Windows.Controls;
using System;
using Proteca.Silverlight.Views.UserContols;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Equipement_PP")]
    [ExportViewToRegion("Equipement_PP", "MainContainer")]
    public partial class Equipement_PPView : Page
    {

        public Equipement_PPView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Equipement_PPView_Loaded);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Equipement_PP", "Equipement_PP");
            }
        }

        private void btnValiderDocument_Click(object sender, RoutedEventArgs e)
        {
            // Commit automatiquement les modifications effectuées dans la grille des documents
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridDocuments");
            if (grid != null)
            {
                ((RadGridView)grid).CommitEdit();

                // Si le commit ne fonctionne pas (erreur sur le champ), on force le cancel Edit pour sortir du champ
                if (((RadGridView)grid).CurrentCell != null && ((RadGridView)grid).CurrentCell.IsInEditMode)
                {
                    ((RadGridView)grid).CancelEdit();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Equipement_PPView_Loaded(object sender, EventArgs e)
        {
            Grid myGrid = ((RadTileViewItem)((RadTileView)((Panel)(this.container.MainContent)).Children[0]).Items[2]).Content as Grid;
            ((ExportAndPrint)myGrid.Children[0]).GridView = myGrid.Children[1] as RadGridView;
        }

    }
}
