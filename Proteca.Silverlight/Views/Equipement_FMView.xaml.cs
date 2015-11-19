﻿using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;
using Proteca.Silverlight.Helpers;
using Telerik.Windows.Controls;
using Proteca.Silverlight.ViewModels;



namespace Proteca.Silverlight.Views
{
    [ExportAsView("Equipement_FM")]
    [ExportViewToRegion("Equipement_FM", "MainContainer")]
    public partial class Equipement_FMView : Page
    {

        public Equipement_FMView()
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
                return ViewModelRoute.Create("Equipement_FM", "Equipement_FM");
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

    }
}
