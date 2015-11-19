using System.Collections.ObjectModel;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Helpers;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using System.Linq;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for TypeEquipement entity
    /// </summary>
    [ExportAsViewModel("TypeEquipement")]
    public class TypeEquipementViewModel : BaseViewModel, IEventSink<CurrentNavigation>, IEventSink<ViewMode>, IPartImportsSatisfiedNotification
    {
        #region Service

        [Import(typeof(INavigationService))]
        public INavigationService NavService { get; set; }

        [Import]
        public IEntityService<TypeEquipement> Service { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<TypeEquipement> TypeEquipements
        {
            get
            {
                return Service.Entities;
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

        #endregion

        #region Constructor

        public TypeEquipementViewModel()
            : base()
        {
            
            // Define commands
            GetEquipementCommand = new ActionCommand<object>(
                obj => GetEquipement(obj));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        protected virtual void GetEquipement(object MonEquipement)
        {

            NavService.NavigateUri(new Uri(string.Format("{0}/{1}",
                       CurrentNavigation.Current.BaseUrlWithOutFilter,
                       (string)MonEquipement), UriKind.Relative));

            //foreach (var equip in this.TypeEquipements.Where(r => r.IsSelected != true))
            //{
            //    equip.IsSelected = false;
            //}
            //RaisePropertyChanged(() => this.TypeEquipements);
        }

        #endregion

        #region Command

        /// <summary>
        /// Déclaration de l'objet de command de réintégration de l'utilisateur
        /// </summary>
        public IActionCommand GetEquipementCommand { get; private set; }

        #endregion

        public void OnImportsSatisfied()
        {
            EventAggregator.Subscribe<CurrentNavigation>(this);
            EventAggregator.Subscribe<ViewMode>(this);

            //Service.GetEntities((err) => EntitiesLoaded(err));
        }

        protected override void ActivateView(string viewName, System.Collections.Generic.IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);
            Service.GetEntities((err) => EntitiesLoaded(err));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        private void EntitiesLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(TypeEquipement).Name));
            }
            // If no error is returned, set the model to entities
            else
            {
                RaisePropertyChanged(() => this.TypeEquipements);
            }
        }

        public void HandleEvent(CurrentNavigation publishedEvent)
        {
            if (publishedEvent.Filtre != null && TypeEquipements != null)
            {
                var typeEqu = TypeEquipements.Where(te => te.CodeEquipement == publishedEvent.Filtre.GetStringValue());
                if (typeEqu.Any())
                {
                    typeEqu.First().IsSelected = true;
                }
            }
        }

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
