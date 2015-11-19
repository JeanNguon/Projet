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
using Proteca.Silverlight.Helpers;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("ValidationEquipement")]
    [ExportViewToRegion("ValidationEquipement", "MainContainer")]
    public partial class ValidationEquipementView : Page
    {

        public ValidationEquipementView()
        {
            InitializeComponent();
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
                return ViewModelRoute.Create("ValidationEquipement", "ValidationEquipement");
            }
        }

        #region Events

        /// <summary>
        /// Permet de recalculer les aggregateFunctions Pour mettre à jour les checkbox de colones et groupes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxEqTmp_Click(object sender, RoutedEventArgs e)
        {
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewValidationEquipements");
            if (grid != null && ((RadGridView)grid).CurrentCell != null)
            {
                ((RadGridView)grid).Columns["Valider"].IsReadOnly = false;
                ((RadGridView)grid).Items.CommitEdit();
                ((RadGridView)grid).CancelEdit();
                ((RadGridView)grid).Columns["Valider"].IsReadOnly = true;
            }
        }

        /// <summary>
        /// Permet de recalculer les aggregateFunctions Pour mettre à jour les checkbox de colones et groupes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxPpTmp_Click(object sender, RoutedEventArgs e)
        {
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewValidationPps");
            if (grid != null && ((RadGridView)grid).CurrentCell != null)
            {
                ((RadGridView)grid).Columns["Valider"].IsReadOnly = false;
                ((RadGridView)grid).Items.CommitEdit();
                ((RadGridView)grid).CancelEdit();
                ((RadGridView)grid).Columns["Valider"].IsReadOnly = true;
            }
        }

        /// <summary>
        /// Valide les modifications du tableau avant de valider les données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            // Commit automatiquement les modifications effectuées dans la grille des documents
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewValidationEquipements");
            if (grid != null)
            {
                ((RadGridView)grid).CommitEdit();

                // Si le commit ne fonctionne pas (erreur sur le champ), on force le cancel Edit pour sortir du champ
                if (((RadGridView)grid).CurrentCell != null && ((RadGridView)grid).CurrentCell.IsInEditMode)
                {
                    ((RadGridView)grid).CancelEdit();
                }
            }
            grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewValidationPps");
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
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewValidationEquipements");
            if (grid != null)
            {
                ((RadGridView)grid).CommitEdit();

                // Si le commit ne fonctionne pas (erreur sur le champ), on force le cancel Edit pour sortir du champ
                if (((RadGridView)grid).CurrentCell != null && ((RadGridView)grid).CurrentCell.IsInEditMode)
                {
                    ((RadGridView)grid).CancelEdit();
                }
            }
            grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewValidationPps");
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
