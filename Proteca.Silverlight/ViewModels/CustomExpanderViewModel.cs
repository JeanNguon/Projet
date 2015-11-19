using System;
using System.ComponentModel.Composition;
using Jounce.Core.ViewModel;
using Jounce.Core.Event;
using Proteca.Silverlight.Enums;
using System.Collections.Generic;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for CustomExpander
    /// </summary>
    [ExportAsViewModel("CustomExpander")]
    public class CustomExpanderViewModel : BaseViewModel, IPartImportsSatisfiedNotification, IEventSink<ViewMode>
    {

        #region Constructor
        // Default ctor
        public CustomExpanderViewModel()
        {
        }

        #endregion

        #region Properties

        private Boolean _isExpanded = true;
        public Boolean IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(() => this.IsExpanded);
                EventAggregator.Publish(value ? ExpanderMode.Expanded : ExpanderMode.Collapsed);
            }
        }

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

        private Boolean _hideExpander = true;
        public Boolean HideExpander
        {
            get { return _hideExpander; }
            set 
            {
                _hideExpander = value;
                
                RaisePropertyChanged(() => this.HideExpander);
            }
        }

        private Double _maxWidth;
        public Double MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                _maxWidth = value;

                RaisePropertyChanged(() => this.MaxWidth);
            }
        }

        #endregion

        #region Methods

        protected override void ActivateView(string viewName,
          IDictionary<string, object> viewParameters)
        {
            if (viewParameters.ContainsKey("Title"))
            {
                Title = (String)viewParameters["Title"];
                HideExpander = false;
            }
            if (viewParameters.ContainsKey("HideExpander"))
            {
                HideExpander = (bool)viewParameters["HideExpander"];
            }
            if (viewParameters.ContainsKey("MaxWidth"))
            {
                MaxWidth = (Double)viewParameters["MaxWidth"];
            }
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
