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
using Telerik.Windows.Controls;
using Proteca.Silverlight.Helpers;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Alerte")]
    [ExportViewToRegion("Alerte", "MainContainer")]
    public partial class AlerteView : Page
    {

        public AlerteView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gestion de l'auto expand des groupes d'un gridview contenant des elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RadGridViewAlerte_DataLoaded(object sender, EventArgs e)
        {
            // EPI : Ticket Mantis GRT 0009032
            //if (sender is DataControl 
            //    && (sender as DataControl).Items.Count > 0)
            //{
            //    (sender as RadGridView).ExpandAllGroups();
            //}
        }

        private void RadGridViewAlerte_Grouped(object sender, GridViewGroupedEventArgs e)
        {
            // EPI : Ticket Mantis GRT 0009032
            //if (sender is DataControl
            //    && (sender as DataControl).Items.Count > 0)
            //{
            //    (sender as RadGridView).ExpandAllGroups();
            //}
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
                return ViewModelRoute.Create("Alerte", "Alerte");
            }
        }



        #region Events

        /// <summary>
        /// Valide les modifications du tableau avant de valider les données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            // Commit automatiquement les modifications effectuées dans la grille des documents
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewAlerte");
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
            // Commit automatiquement les modifications effectuées dans la grille des documents
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewAlerte");
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
