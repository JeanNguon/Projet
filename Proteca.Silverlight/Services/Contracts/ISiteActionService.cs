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
using System.ComponentModel.Composition;
using Jounce.Core.Fluent;
using Jounce.Core.View;
using System.Collections.Generic;

namespace Proteca.Silverlight.Services.Contracts
{

    public interface ISiteActionService
    {

        void CheckoutPage(string pageUri, Action<Exception> completed);
    }
}
