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
using Proteca.Silverlight.Models;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class Documents : UserControl
    {

        public Documents()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Workaround pour corriger le bug de l'hyperlinkbutton avec l'url externe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton button = (HyperlinkButton)sender;
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(button.Tag.ToString()), "_blank");
        }
    }
}
