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
using Telerik.Windows.Controls;
using Proteca.Silverlight.ViewModels;
using Proteca.Silverlight.Models;
using Jounce.Core.View;
using Jounce.Regions.Core;
using System.ComponentModel.Composition;
using Jounce.Core.ViewModel;

namespace Proteca.Silverlight.Views.UserContols
{
    [ExportAsView("Menu")]
    [ExportViewToRegion("Menu", "MenuContainer")]
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("Menu", "Menu");
            }
        }
    }
}
