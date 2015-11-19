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


namespace Proteca.Silverlight.Views.Windows
{
    [ExportAsView("RefSousTypeOuvrage")]
    [ExportViewToRegion("RefSousTypeOuvrage", "WindowContainer")]
    /// <summary>
    /// <see cref="ChildWindow"/> class that displays errors to the user.
    /// </summary>
    public partial class RefSousTypeOuvrageView : UserControl
    {
        #region Constructor

        /// <summary>
        /// Construetceur par défaut
        /// </summary>
        public RefSousTypeOuvrageView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Public Functions

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("RefSousTypeOuvrage", "RefSousTypeOuvrage");
            }
        }

        #endregion Public Functions

        #region Events

        /// <summary>
        /// Commit de l'édition pour que les modifications soient prise en compte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            this.RadGridViewSousTypeOuvrage.CommitEdit();

            // Si le commit ne fonctionne pas (erreur sur le champ), on force le cancel Edit pour sortir du champ
            if (RadGridViewSousTypeOuvrage.CurrentCell != null && RadGridViewSousTypeOuvrage.CurrentCell.IsInEditMode)
            {
                RadGridViewSousTypeOuvrage.CancelEdit();
            }
        }

        /// <summary>
        /// Commit de l'édition pour que les modifications soient prise en compte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.RadGridViewSousTypeOuvrage.CommitEdit();

            // Si le commit ne fonctionne pas (erreur sur le champ), on force le cancel Edit pour sortir du champ
            if (RadGridViewSousTypeOuvrage.CurrentCell != null && RadGridViewSousTypeOuvrage.CurrentCell.IsInEditMode)
            {
                RadGridViewSousTypeOuvrage.CancelEdit();
            }
        }
        
        #endregion Events

    }
}