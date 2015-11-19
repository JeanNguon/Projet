using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework.Command;
using Proteca.Silverlight.Services;
using Proteca.Web.Models;
using System.Linq;
using Jounce.Framework;
using Proteca.Silverlight.Models;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for FilAriane entity
    /// </summary>
    [ExportAsViewModel("FilAriane")]
    public class FilArianeViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        #region Constructor
        // Default ctor
        public FilArianeViewModel()
        {
        }

        #endregion

        #region Properties

        private ObservableCollection<MenuItem> FilAriane;
        public ObservableCollection<MenuItem> SelectedFilAriane
        {
            get { return FilAriane; }
            set
            {
                FilAriane = value;
                RaisePropertyChanged(() => this.SelectedFilAriane);
            }
        }

        private String _mainContent;
        public String MainContent
        {
            get
            {
                if (String.IsNullOrEmpty(_mainContent) && !Application.Current.IsRunningOutOfBrowser)
                {
                    _mainContent = "Accueil";
                }
                return _mainContent;
            }
            set
            {
                _mainContent = value;
                RaisePropertyChanged(() => this.MainContent);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
        }

        #region Default Methods        

        protected override void ActivateView(string viewName,
          IDictionary<string, object> viewParameters)
        {
            if (viewParameters.ContainsKey("SelectedMenu"))
            {
                MenuItem SelectedMenu = (MenuItem)viewParameters["SelectedMenu"];
                ObservableCollection<MenuItem> fil = new ObservableCollection<MenuItem>();
                while (SelectedMenu != null)
                {
                    fil.Insert(0, SelectedMenu);
                    SelectedMenu = SelectedMenu.Parent;
                }
                SelectedFilAriane = fil;
            }
            else if (viewParameters.ContainsKey("Title"))
            {
                MainContent = (String)viewParameters["Title"];
            }
        }

        #endregion

        #endregion

        #region Completion Callbacks

        #endregion
    }
}
