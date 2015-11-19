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
using Proteca.Silverlight.Views.Converters;
using System.Windows.Markup;

namespace Proteca.Silverlight.Views.UserContols
{
    public class NumericTextBox : TextBox
    {
        public NumericTextBox()
            : base()
        {
            this.TextChanged += new TextChangedEventHandler(NumericTextBox_TextChanged);
            this.Loaded += new RoutedEventHandler(NumericTextBox_Loaded);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(NumericTextBox_IsEnabledChanged);
        }

        public String StringFormat
        {
            get { return (String)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register("StringFormat", typeof(String), typeof(NumericTextBox), new PropertyMetadata("", StringFormatChanged));

        static void StringFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var tb = obj as NumericTextBox;
            BindingExpression bindingExp = tb.GetBindingExpression(TextBox.TextProperty);
            Binding binding = new Binding(bindingExp.ParentBinding);
            binding.StringFormat = tb.StringFormat;
            tb.SetBinding(TextBox.TextProperty, binding);
        }

        private Boolean _enableBinding;
        void NumericTextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _enableBinding = (bool)e.NewValue;
        }

        void NumericTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            BindingExpression bindingExp = tb.GetBindingExpression(TextBox.TextProperty);
            Binding binding = new Binding(bindingExp.ParentBinding);
            binding.Converter = new EmptyToNullConverter();
            tb.SetBinding(TextBox.TextProperty, binding);
        }

        void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Contains(","))
            {
                int position = this.SelectionStart;
                tb.Text = tb.Text.Replace(",", ".");
                this.SelectionStart = position;
            }
            //pourquoi reassigner la source ? ca crée des bugs de saisie
            //else if (_enableBinding && !tb.Text.EndsWith(",") && !tb.Text.EndsWith("."))
            //{
            //    BindingExpression binding = tb.GetBindingExpression(TextBox.TextProperty);
            //    if (binding != null)
            //    {
            //        binding.UpdateSource();
            //    }
            //}
        }

    }
}
