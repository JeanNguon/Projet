using System.ComponentModel.Composition;
using System.Windows.Controls;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;

namespace Proteca.Silverlight.Views.Windows
{

    [ExportAsView("Selected_Secteur")]
    [ExportViewToRegion("Selected_Secteur", "WindowContainer")]
    /// <summary>
    /// <see cref="ChildWindow"/> class that displays errors to the user.
    /// </summary>
    public partial class Selected_Secteur : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="Selected_Secteur"/> instance.
        /// </summary>
        public Selected_Secteur()
        {
            InitializeComponent();
        }
                
        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Selected_Secteur", "Selected_Secteur");
            }
        }

        #region DependencyProperty

        #endregion
    }
}