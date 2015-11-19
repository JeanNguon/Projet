using System.ComponentModel.Composition;
using System.Windows.Controls;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;

namespace Proteca.Silverlight.Views.Windows
{

    [ExportAsView("CreateEqEquipementTmp")]
    [ExportViewToRegion("CreateEqEquipementTmp", "WindowContainer")]
    /// <summary>
    /// <see cref="ChildWindow"/> class that displays errors to the user.
    /// </summary>
    public partial class CreateEqEquipementTmp : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="CreateEqEquipementTmp"/> instance.
        /// </summary>
        public CreateEqEquipementTmp()
        {
            InitializeComponent();
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("CreateEqEquipementTmp", "CreateEqEquipementTmp");
            }
        }

        #region DependencyProperty

        #endregion
    }
}
