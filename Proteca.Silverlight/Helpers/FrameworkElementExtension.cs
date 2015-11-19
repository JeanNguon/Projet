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
using System.Windows.Data;
using System.Collections.Generic;

namespace Proteca.Silverlight.Helpers
{
    public static class FrameworkElementExtension
    {

        /// Listen for change of the dependency property  
        public static void RegisterForNotification(this FrameworkElement element, string propertyName,  PropertyChangedCallback callback)
        {
            //Bind to a depedency property  
            Binding b = new Binding(propertyName) { Source = element };
            var prop = System.Windows.DependencyProperty.RegisterAttached(
                "ListenAttached" + propertyName,
                typeof(object),
                typeof(UserControl),
                new System.Windows.PropertyMetadata(callback));

            element.SetBinding(prop, b);
        }

        public static IEnumerable<DependencyObject> AllChildren(this DependencyObject element)
        {
            yield return element;
            int n = VisualTreeHelper.GetChildrenCount(element);
            for (int k = 0; k < n; k++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, k);
                foreach (var descendent in child.AllChildren())
                {
                    yield return descendent;
                }
            }
        }
    }
}
