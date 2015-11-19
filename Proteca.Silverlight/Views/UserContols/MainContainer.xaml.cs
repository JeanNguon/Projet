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
using System.ComponentModel;
using Telerik.Windows.Controls;
using Proteca.Silverlight.Helpers;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class MainContainer : UserControl
    {
        public MainContainer()
        {
            InitializeComponent();
            this.Loaded += MainContainer_Loaded;
        }

        void MainContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsEditMode)
            {
                IEnumerable<DependencyObject> children = this.Content.AllChildren();
                //(c is Control && (c as Control).Visibility == System.Windows.Visibility.Visible) && (c as Control).IsEnabled &&
                Control ctrl = children.FirstOrDefault(c => (c is TextBox || c is CustomAutoCompleteBox || c is CheckBox || c is RadTimePicker)) as Control;

                if (ctrl != null)
                {                   
                    ctrl.Focus();
                    MainScroll.ScrollToTop();
                }
            }
        }

        public object FooterContent
        {
            get { return (object)GetValue(FooterContentProperty); }
            set { SetValue(FooterContentProperty, value); }
        }

        public static readonly DependencyProperty FooterContentProperty =
            DependencyProperty.Register("FooterContent", typeof(object), typeof(MainContainer), new PropertyMetadata(null));

        public object MainContent
        {
            get { return (object)GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }

        public static readonly DependencyProperty MainContentProperty =
            DependencyProperty.Register("MainContent", typeof(object), typeof(MainContainer), new PropertyMetadata(null));

        public String CurrentElementHeader
        {
            get { return (string)GetValue(CurrentElementHeaderProperty); }
            set { SetValue(CurrentElementHeaderProperty, value); }
        }

        public static readonly DependencyProperty CurrentElementHeaderProperty =
        DependencyProperty.Register("CurrentElementHeader", typeof(string), typeof(MainContainer), new PropertyMetadata("", null));
    

        public Boolean HideTopNavigation
        {
            get { return (Boolean)GetValue(HideTopNavigationProperty); }
            set { SetValue(HideTopNavigationProperty, value); }
        }

        public static readonly DependencyProperty HideTopNavigationProperty =
        DependencyProperty.Register("HideTopNavigation", typeof(Boolean), typeof(MainContainer), new PropertyMetadata(false, null));

        public Uri PreviousUri
        {
            get { return (Uri)GetValue(PreviousUriProperty); }
            set { SetValue(PreviousUriProperty, value); }
        }

        public static readonly DependencyProperty PreviousUriProperty =
        DependencyProperty.Register("PreviousUri", typeof(Uri), typeof(MainContainer), new PropertyMetadata(null, null));

        public Uri NextUri
        {
            get { return (Uri)GetValue(NextUriProperty); }
            set { SetValue(NextUriProperty, value); }
        }

        public static readonly DependencyProperty NextUriProperty =
        DependencyProperty.Register("NextUri", typeof(Uri), typeof(MainContainer), new PropertyMetadata(null, null));

        public Boolean HideFooter
        {
            get { return (Boolean)GetValue(HideFooterProperty); }
            set { SetValue(HideFooterProperty, value); }
        }

        public static readonly DependencyProperty HideFooterProperty =
        DependencyProperty.Register("HideFooter", typeof(Boolean), typeof(MainContainer), new PropertyMetadata(false, null));

        public Boolean IsEditMode
        {
            get { return (Boolean)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public static readonly DependencyProperty IsEditModeProperty =
        DependencyProperty.Register("IsEditMode", typeof(Boolean), typeof(MainContainer), new PropertyMetadata(false, OnIsEditModeChanged));

        private static void OnIsEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArg)
        {
            if ((bool)eventArg.NewValue && d is MainContainer && ((MainContainer)d).Content is UIElement)
            {
                IEnumerable<DependencyObject> children = ((UIElement)((MainContainer)d).Content).AllChildren();
                //(c is Control && (c as Control).Visibility == System.Windows.Visibility.Visible) && (c as Control).IsEnabled &&
                Control ctrl = children.FirstOrDefault(c => (c is TextBox || c is CustomAutoCompleteBox || c is CheckBox || c is RadTimePicker)) as Control;

                if (ctrl != null)
                {
                    DependencyPropertyChangedEventHandler setFocus = null;
                    setFocus = (o, e) =>
                    {
                        ((Control)o).IsEnabledChanged -= setFocus;
                        if ((bool)e.NewValue)
                        {
                            ((Control)o).Focus();
                            ((MainContainer)d).MainScroll.ScrollToTop();
                        }
                    };
                    ctrl.IsEnabledChanged += setFocus;
                }
            }
        }

        public Boolean DisableScrollViewer
        {
            get { return (Boolean)GetValue(DisableScrollViewerProperty); }
            set { SetValue(DisableScrollViewerProperty, value); }
        }

        public static readonly DependencyProperty DisableScrollViewerProperty =
        DependencyProperty.Register("DisableScrollViewer", typeof(Boolean), typeof(MainContainer), new PropertyMetadata(false, null));

    }
}
