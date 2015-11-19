using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;

namespace Proteca.Silverlight.Helpers
{
    /// <summary>
    /// Détermine un "Behavior" qui permet le déclenchement d'une action sur la touche Entrée
    /// </summary>
    public class KeyDownBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(
        "Command", typeof(ICommand),
        typeof(KeyDownBehavior), null
        );
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter
            && this.Command != null
            && (e.OriginalSource == null || !(e.OriginalSource is TextBox) || !((TextBox)e.OriginalSource).AcceptsReturn)) // vérification que l'on est pas dans un textbox multiligne
            {
                this.Command.Execute(null);
            }
        }
    }
}
