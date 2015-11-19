using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Proteca.Silverlight.Views.UserContols
{
    public class CustomAutoCompleteBox : AutoCompleteBox
    {
        private object _lastSelectedItem { get; set; }

        public CustomAutoCompleteBox():base()
        {
            this.LostFocus += new RoutedEventHandler(CustomAutoCompleteBox_LostFocus);
            this.SelectionChanged += new SelectionChangedEventHandler(CustomAutoCompleteBox_SelectionChanged);

            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(CustomAutoCompleteBox_IsEnabledChanged);
        }

        void CustomAutoCompleteBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool)e.NewValue) && SelectedItem == null)
            {
                this.Text = String.Empty;
            }
        }

        void CustomAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _lastSelectedItem = e.AddedItems[0];
            }
            else if (e.RemovedItems.Count > 0 && !((CustomAutoCompleteBox)sender).HasFocus())
            {
                _lastSelectedItem = null;
                this.Text = String.Empty;
            }
        }


        void CustomAutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                SelectedItem = _lastSelectedItem;
            }
        }
    }
}
