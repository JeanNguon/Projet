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

namespace Proteca.Silverlight.Views.Behaviors
{

    public class SetUnderlineAction : Microsoft.Expression.Interactivity.Core.ChangePropertyAction
    {
        protected override void Invoke(object parameter)
        {
            PropertyName = "TextDecorations";
            Value = TextDecorations.Underline;
            base.Invoke(parameter);

        }
    }
}
