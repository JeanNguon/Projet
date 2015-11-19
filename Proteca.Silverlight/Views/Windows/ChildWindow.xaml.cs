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
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Views.UserContols;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.Views.Windows
{
    [Export(typeof(ChildWindowControl))]
    public partial class ChildWindowControl : ChildWindow
    {

        public ChildWindowControl()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public Action ActionOnOpened { get; set; }

        protected override void OnOpened()
        {
            base.OnOpened();

            IEnumerable<DependencyObject> children = this.AllChildren();
            Control ctrl = children.FirstOrDefault(c => (c is TextBox || c is CustomAutoCompleteBox || c is CheckBox || c is RadTimePicker || c is ComboBox || c is RadComboBox)) as Control;

            if (ctrl != null)
            {
                ctrl.Focus();
            }

            if (ActionOnOpened != null)
            {
                ActionOnOpened.Invoke();
                ActionOnOpened = null;
            }
        }
    }
}
