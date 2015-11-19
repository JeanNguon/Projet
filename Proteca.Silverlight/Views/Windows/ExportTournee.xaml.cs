using System.ComponentModel.Composition;
using System.Windows.Controls;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;

namespace Proteca.Silverlight.Views.Windows
{
    [ExportAsView("ExportTournee")]
    [ExportViewToRegion("ExportTournee", "WindowContainer")]
    /// <summary>
    /// <see cref="ChildWindow"/> class that displays errors to the user.
    /// </summary>
    public partial class ExportTournee : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="CreateUsrUtilisateur"/> instance.
        /// </summary>
        public ExportTournee()
        {
            InitializeComponent();
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("ExportTournee", "ExportTournee");
            }
        }

        #region DependencyProperty

        #endregion
    }
}
