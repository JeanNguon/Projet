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
    [ExportAsView("EnsembleElectrique")]
    [ExportViewToRegion("EnsembleElectrique", "MainContainer")]
    public partial class EnsembleElectriqueView : Page
    {
        #region Public Properties

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("EnsembleElectrique", "EnsembleElectrique");
            }
        }

        #endregion Public Properties

        #region Constructor

        public EnsembleElectriqueView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(EnsembleElectriqueView_Loaded);
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EnsembleElectriqueView_Loaded(object sender, EventArgs e)
        {
            Grid myGrid = ((RadTileViewItem)((RadTileView)((Panel)(this.container.MainContent)).Children[0]).Items[2]).Content as Grid;
            ((ExportAndPrint)myGrid.Children[0]).GridView = myGrid.Children[1] as RadGridView;
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
        #endregion Events
    }
}
