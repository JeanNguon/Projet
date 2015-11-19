using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;
using Telerik.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using Proteca.Silverlight.Helpers;
using System.ComponentModel;

namespace Proteca.Silverlight.Views.UserContols
{
    /// <summary>
    /// User Control permettant de les boutons type d'équipements 
    /// </summary>
    [ExportAsView("TypeEquipement")]
    [ExportViewToRegion("TypeEquipement", "RegionTopContainer")]
    public partial class TypeEquipement : UserControl
    {
        #region Properties


        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public TypeEquipement()
        {
            InitializeComponent();
        }

        #endregion Constructor

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("TypeEquipement", "TypeEquipement");
            }
        }

    }
}
