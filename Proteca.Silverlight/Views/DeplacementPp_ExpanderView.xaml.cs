using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;

namespace Proteca.Silverlight.Views
{
    [ExportAsView("DeplacementPp_Expander")]
    [ExportViewToRegion("DeplacementPp_Expander", "ExpanderContainer")]
    public partial class DeplacementPp_ExpanderView : Page
    {

        public DeplacementPp_ExpanderView()
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
                return ViewModelRoute.Create("DeplacementPp", "DeplacementPp_Expander");
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
