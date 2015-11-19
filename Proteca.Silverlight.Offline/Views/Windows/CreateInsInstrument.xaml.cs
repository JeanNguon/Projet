using System.ComponentModel.Composition;
using System.Windows.Controls;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;

namespace Proteca.Silverlight.Views.Windows
{

    [ExportAsView("CreateInsInstrument")]
    [ExportViewToRegion("CreateInsInstrument", "WindowContainer")]
    /// <summary>
    /// <see cref="ChildWindow"/> class that displays errors to the user.
    /// </summary>
    public partial class CreateInsInstrument : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="CreateInsInstrument"/> instance.
        /// </summary>
        public CreateInsInstrument()
        {
            InitializeComponent();
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("CreateInsInstrument", "CreateInsInstrument");
            }
        }

        #region DependencyProperty

        #endregion
    }
}
