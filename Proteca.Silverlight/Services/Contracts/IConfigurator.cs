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
using Microsoft.SharePoint.Client;

namespace Proteca.Silverlight.Services.Contracts
{
    public interface IConfigurator
    {
        ClientContext GetClientContext();

        Uri GetServiceAdress(string defaultAdress);
    }
}
