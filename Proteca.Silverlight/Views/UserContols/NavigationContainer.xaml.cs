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
using System.ComponentModel.Composition;

namespace Proteca.Silverlight.Views.UserContols
{
    [Export]
    public partial class NavigationContainer
    {
        public String Title { get { return HeaderText.Text; } set { HeaderText.Text = value; } }

        public NavigationContainer()
        {
            InitializeComponent();
        }
    }
}
