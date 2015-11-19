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
using Proteca.Silverlight.Views.UserContols;
using Proteca.Silverlight.Helpers;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("CategoriePp")]
    [ExportViewToRegion("CategoriePp", "MainContainer")]
    public partial class CategoriePpView : Page
    {

        public CategoriePpView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(CategoriePpView_Loaded);
        }

        void CategoriePpView_Loaded(object sender, RoutedEventArgs e)
        {
            ExportAndPrint export = ((Panel)(this.container.MainContent)).FindChildByType<ExportAndPrint>();
            RadGridView grid = ((Panel)(this.container.MainContent)).FindChildByType<RadGridView>();
            export.GridView = grid;
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
                return ViewModelRoute.Create("CategoriePp", "CategoriePp");
            }
        }

        #region Event

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            // Commit automatiquement les modifications effectuées dans la grille des documents
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewCategoriePP");
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

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            // Commit automatiquement les modifications effectuées dans la grille des documents
            var allChildren = this.container.AllChildren();
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewCategoriePP");
            if (grid != null)
            {
                ((RadGridView)grid).CommitEdit();

                // Si le commit ne fonctionne pas (erreur sur le champs), on force le cancel Edit pour sortir du champ
                if (((RadGridView)grid).CurrentCell != null && ((RadGridView)grid).CurrentCell.IsInEditMode)
                {
                    ((RadGridView)grid).CancelEdit();
                }
            }
        }

        #endregion
    }
}
