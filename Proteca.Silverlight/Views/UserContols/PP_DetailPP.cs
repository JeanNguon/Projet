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
using Proteca.Silverlight.Helpers;

namespace Proteca.Silverlight.Views.UserContols
{
    [ExportAsView("PP_DetailPP")]
    [ExportViewToRegion("PP_DetailPP", "PP_DetailPPContainer")]
    public partial class PP_DetailPP : DetailPP
    {
        public PP_DetailPP()
            : base()
        {
            InitializeComponent();
        }

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("DetailPP", "PP_DetailPP");
            }
        }
    }
}

