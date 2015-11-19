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
    [ExportAsView("CustomExpander")]
    [ExportViewToRegion("CustomExpander", "LeftContainer")]
    public partial class CustomExpander : UserControl
    {
        public CustomExpander()
        {
            InitializeComponent();
        }

        public String Title
        {
            get { return (String)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(String), typeof(CustomExpander), new PropertyMetadata(null));

        [Export]
        public ViewModelRoute Binding
        {
            get
            {
                return ViewModelRoute.Create("CustomExpander", "CustomExpander");
            }
        }
    }
}
