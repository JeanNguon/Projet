using System;
using System.ComponentModel.Composition;
using Jounce.Core.ViewModel;
using Jounce.Core.Event;
using Proteca.Silverlight.Enums;
using System.Collections.Generic;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for CustomTopContainer
    /// </summary>
    [ExportAsViewModel("CustomTopContainer")]
    public class CustomTopContainerViewModel : BaseViewModel, IPartImportsSatisfiedNotification, IEventSink<ViewMode>
    {

        #region Constructor
        // Default ctor
        public CustomTopContainerViewModel()
        {
        }

        #endregion

        #region Properties

        private Boolean _isEnabled = true;
        public Boolean IsEnable
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => this.IsEnable);
            }
        }
        
        private String _title;
        public String Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => this.Title);
            }
        }

        private Boolean _hideContainer;
        public Boolean HideContainer
        {
            get { return _hideContainer; }
            set 
            {
                _hideContainer = value;
                
                RaisePropertyChanged(() => this.HideContainer);
            }
        }

        #endregion

        #region Methods

        protected override void ActivateView(string viewName,
          IDictionary<string, object> viewParameters)
        {
            HideContainer = viewParameters.ContainsKey("HideContainer") ?  Convert.ToBoolean(viewParameters.GetValueOrNull("HideContainer").ToString()):true;
        }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            EventAggregator.SubscribeOnDispatcher(this);
        }

        #endregion

        public void HandleEvent(ViewMode publishedEvent)
        {
            switch (publishedEvent)
            {
                case ViewMode.NavigationMode:
                    IsEnable = true;
                    break;
                case ViewMode.EditMode:
                    IsEnable = false;
                    break;
                default:
                    break;
            }
        }
    }
}
