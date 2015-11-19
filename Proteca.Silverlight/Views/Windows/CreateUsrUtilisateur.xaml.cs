using System.ComponentModel.Composition;
using System.Windows.Controls;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;

namespace Proteca.Silverlight.Views.Windows
{

    [ExportAsView("CreateUsrUtilisateur")]
    [ExportViewToRegion("CreateUsrUtilisateur", "WindowContainer")]
    /// <summary>
    /// <see cref="ChildWindow"/> class that displays errors to the user.
    /// </summary>
    public partial class CreateUsrUtilisateur : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="CreateUsrUtilisateur"/> instance.
        /// </summary>
        public CreateUsrUtilisateur()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                tbxNom.Focus();
            }; 
        }
                
        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("CreateUsrUtilisateur", "CreateUsrUtilisateur");
            }
        }

        #region DependencyProperty

        #endregion
    }
}