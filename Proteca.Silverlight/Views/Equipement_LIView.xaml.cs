using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;
using System.Windows;
using Telerik.Windows.Controls;
using System.Linq;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.ViewModels;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Equipement_LI")]
    [ExportViewToRegion("Equipement_LI", "MainContainer")]
    public partial class Equipement_LIView : Page
    {

        public Equipement_LIView()
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
                return ViewModelRoute.Create("Equipement_LI", "Equipement_LI");
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
