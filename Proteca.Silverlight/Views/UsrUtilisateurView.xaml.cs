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
using Proteca.Silverlight.ViewModels;
using Jounce.Core.Event;
using Proteca.Silverlight.Enums;
using Jounce.Framework;
using Telerik.Windows.Controls;
using Proteca.Silverlight.Views.UserContols;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("UsrUtilisateur")]
    [ExportViewToRegion("UsrUtilisateur", "MainContainer")]
    public partial class UsrUtilisateurView : Page
    {
        /// <summary>
        /// Event aggregator
        /// </summary>
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        public UsrUtilisateurView()
        {
            InitializeComponent();
            this.Loaded +=new RoutedEventHandler(UsrUtilisateurView_Loaded);
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
                return ViewModelRoute.Create("UsrUtilisateur", "UsrUtilisateur");
            }
        }

        /// <summary>
        /// Mapping des boutons d'exports avec le tableau correspondant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UsrUtilisateurView_Loaded(object sender, RoutedEventArgs e)
        {
            RadTileView subTileView = ((Panel)(this.container.MainContent)).ChildrenOfType<RadTileView>().First();
            RadTileViewItem subTileViewItem = subTileView.Items.Where(r => ((RadTileViewItem)r).Name == "HistoAdminItem").First() as RadTileViewItem;
            ((Grid)((ScrollViewer)subTileViewItem.Content).Content).ChildrenOfType<ExportAndPrint>().First().GridView = ((Grid)((ScrollViewer)subTileViewItem.Content).Content).ChildrenOfType<RadGridView>().First();
        }

        /// <summary>
        /// On cache l'expander à l'affichage de l'historisation des modifications admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadTileViewItem_TileStateChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (EventAggregator != null)
            {
                switch (((RadTileViewItem)sender).TileState)
                {
                    case TileViewItemState.Minimized:
                        EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("HideExpander", false));
                        break;
                    case TileViewItemState.Maximized:
                        EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("HideExpander", true));
                        break;
                    default:
                        EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("HideExpander", false));
                        break;
                }
            }
        }

    }
}
