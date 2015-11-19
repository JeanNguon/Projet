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
using Telerik.Windows.Controls;
using System.Collections.Generic;
using Telerik.Windows.Controls.GridView;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Collections;
using System.Windows.Data;
using Proteca.Silverlight.Helpers;

namespace Proteca.Silverlight.Views.UserContols
{
    public class CustomGrid : Grid
    {
        #region Public Properties

        public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register("IsEnabled", typeof(Boolean), typeof(CustomGrid), new PropertyMetadata(false, OnIsEnabledModeChanged));

        public Boolean IsEnabled
        {
            get { return (Boolean)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        private static void OnIsEnabledModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArg)
        {
            IEnumerable<DependencyObject> children = ((CustomGrid)d).AllChildren();
            foreach (Control childrenToEnabled in children.Where(c => c is Control))
            {
                childrenToEnabled.IsEnabled = (Boolean)eventArg.NewValue;
            }
        }

        #endregion

        #region Constructor

        public CustomGrid()
            : base()
        {
            this.Loaded += new RoutedEventHandler(CustomGrid_Loaded);
        }

        void CustomGrid_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<DependencyObject> children = this.AllChildren();
            foreach (Control childrenToEnabled in children.Where(c => c is Control))
            {
                Binding b = new Binding(this.GetBindingExpression(CustomGrid.IsEnabledProperty).ParentBinding);
                childrenToEnabled.SetBinding(Control.IsEnabledProperty, b);
            }
        }

        #endregion Constructor

    }
}
