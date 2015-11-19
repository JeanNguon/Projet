using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Views.UserContols;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Ressources_Actions")]
    [ExportViewToRegion("Ressources_Actions", "MainContainer")]
    public partial class Ressources_ActionsView : Page
    {
        #region Constructeur

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Ressources_ActionsView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Ressources_ActionsView_Loaded);
        }

        #endregion Constructeur

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Ressources_Actions", "Ressources_Actions");
            }
        }

        #region Event

        /// <summary>
        /// Validation des modifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            // Commit automatiquement les modifications effectuées dans la grille
            var allChildren = this.container.AllChildren();
            var grid = allChildren.Where(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewCoutAction").FirstOrDefault();
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
        /// Annulation des modifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            // Commit automatiquement les modifications effectuées dans la grille
            var allChildren = this.container.AllChildren();
            var grid = allChildren.Where(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewCoutAction").FirstOrDefault();
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
        /// Association de l'export à la grille au chargement de la vue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Ressources_ActionsView_Loaded(object sender, RoutedEventArgs e)
        {
            ExportAndPrint export = ((Panel)(this.container.MainContent)).FindChildByType<ExportAndPrint>();
            RadGridView grid = ((Panel)(this.container.MainContent)).FindChildByType<RadGridView>();
            export.GridView = grid;
        }
        #endregion
    }
}
