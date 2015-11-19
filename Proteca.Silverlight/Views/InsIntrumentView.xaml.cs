using System.ComponentModel.Composition;
using System.Linq;
using Proteca.Silverlight.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;
using Proteca.Silverlight.Views.UserContols;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("InsInstrument")]
    [ExportViewToRegion("InsInstrument", "MainContainer")]
    public partial class InsInstrumentView : Page
    {

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public InsInstrumentView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(InsInstrumentView_Loaded);
        }

        #region Events

        /// <summary>
        /// Initialisation de la vue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InsInstrumentView_Loaded(object sender, RoutedEventArgs e)
        {
            ((ExportAndPrint)((Panel)(this.container.MainContent)).Children[0]).GridView = (RadGridView)((Panel)(this.container.MainContent)).Children[1];
        }

        /// <summary>
        ///  Executes when the user navigates to this page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("InsInstrument", "InsInstrument");
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
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewInstruments");
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
            var grid = allChildren.FirstOrDefault(r => r is RadGridView && ((FrameworkElement)r).Name == "RadGridViewInstruments");
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
