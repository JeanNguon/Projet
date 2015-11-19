using Jounce.Core.ViewModel;
using Jounce;
using Jounce.Framework;
using Proteca.Silverlight.Helpers;
using Jounce.Core.Event;
using System.ComponentModel.Composition;
using System;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Services.Contracts;
namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// Sample view model showing design-time resolution of data
    /// </summary>
    [ExportAsViewModel("Main")]
    public class MainViewModel : BaseViewModel, IPartImportsSatisfiedNotification, IEventSink<CurrentNavigation>, IEventSink<IsBusyEnum>
    {

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'utilisateur connecté
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> userService { get; set; }

        #endregion

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisePropertyChanged(() => this.IsBusy);
            }

        }
        private Boolean _isAccueil = true;
        public Boolean IsAccueil
        {
            get { return _isAccueil; }
            set { _isAccueil = value; RaisePropertyChanged(() => this.IsAccueil); }
        }

        public MainViewModel()
        {
        }

        protected override void ActivateView(string viewName, System.Collections.Generic.IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);
            EventAggregator.Publish("FilAriane".AsViewNavigationArgs());
            EventAggregator.Publish("Menu".AsViewNavigationArgs());
        }

        public void HandleEvent(CurrentNavigation publishedEvent)
        {
            IsAccueil = publishedEvent.Module == Enums.NavigationEnums.MainNavigation.Accueil;
        }

        public void HandleEvent(IsBusyEnum isBusy)
        {
            IsBusy = isBusy == IsBusyEnum.IsBusy ? true : false;
        }

        public void OnImportsSatisfied()
        {
            EventAggregator.Subscribe<CurrentNavigation>(this);
            EventAggregator.Subscribe<IsBusyEnum>(this);
        }
    }
}