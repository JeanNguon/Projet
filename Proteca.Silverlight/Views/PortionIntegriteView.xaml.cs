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
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Views.UserContols;


namespace Proteca.Silverlight.Views
{
    [ExportAsView("PortionIntegrite")]
    [ExportViewToRegion("PortionIntegrite", "MainContainer")]
    public partial class PortionIntegriteView : Page
    {
        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        public PortionIntegriteView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PortionIntegriteView_Loaded);
        }

        #endregion Constructeur

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("PortionIntegrite", "PortionIntegrite");
            }
        }

        #region Events

        /// <summary>
        /// Commit automatiquement les modifications effectuées dans la grille des documents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderDocument_Click(object sender, RoutedEventArgs e)
        {
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
        protected void PortionIntegriteView_Loaded(object sender, EventArgs e)
        {
            Grid myGrid = ((RadTileViewItem)((RadTileView)((Panel)(this.container.MainContent)).Children[0]).Items[2]).Content as Grid;
            ((ExportAndPrint)myGrid.Children[0]).GridView = myGrid.Children[1] as RadGridView;
        }

        /// <summary>
        /// Valide les modifications du tableau avant de valider les données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewNiveauProtection");

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
        /// Annule les modifications du tableau avant d'annuler les modifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewNiveauProtection");

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

        #endregion Events
    }
}
