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
using System.ServiceModel.DomainServices.Client;
using System.Collections.ObjectModel;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Services.Contracts
{
    public interface IUserService<T> where T : class
    {
        bool AuthenticateUser();
     
        T CurrentSharepointUser
        {
            get;
            set;
        }

        UsrUtilisateur CurrentUser
        {
            get;
            set;
        }

        void GetEntities(Action<Exception> completed);

        EventHandler CurrentUserLoaded { get; set; }
    }
}
