using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("Equipement_Expander")]
    [ExportViewToRegion("Equipement_Expander", "ExpanderContainer")]
    public partial class Equipement_ExpanderView : Page
    {

        public Equipement_ExpanderView()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Equipement", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_SO et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingSO
        {
            get
            {
                return ViewModelRoute.Create("Equipement_SO", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_DR et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingDR
        {
            get
            {
                return ViewModelRoute.Create("Equipement_DR", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_LI et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingLI
        {
            get
            {
                return ViewModelRoute.Create("Equipement_LI", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_LE et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingLE
        {
            get
            {
                return ViewModelRoute.Create("Equipement_LE", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_TC et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingTC
        {
            get
            {
                return ViewModelRoute.Create("Equipement_TC", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_FM et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingFM
        {
            get
            {
                return ViewModelRoute.Create("Equipement_FM", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_PO et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingPO
        {
            get
            {
                return ViewModelRoute.Create("Equipement_PO", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_AG et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingAG
        {
            get
            {
                return ViewModelRoute.Create("Equipement_AG", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_DE et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingDE
        {
            get
            {
                return ViewModelRoute.Create("Equipement_DE", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_RI et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingRI
        {
            get
            {
                return ViewModelRoute.Create("Equipement_RI", "Equipement_Expander");
            }
        }

        /// <summary>
        /// Lien entre le viewmodel Equipement_PI et l'expander
        /// </summary>
        [Export]
        public ViewModelRoute BindingPI
        {
            get
            {
                return ViewModelRoute.Create("Equipement_PI", "Equipement_Expander");
            }
        }
    }
}
