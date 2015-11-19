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
    [ExportAsViewModel("TypeRessource")]
    public class TypeRessourceViewModel : BaseViewModel, IEventSink<CurrentNavigation>, IEventSink<ViewMode>, IPartImportsSatisfiedNotification
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
        private ObservableCollection<TypeRessource> _typeRessources;
        public ObservableCollection<TypeRessource> TypeRessources
        {
            get
            {
                if (_typeRessources == null)
                {
                    _typeRessources = new ObservableCollection<TypeRessource>();
                    _typeRessources.Add(new TypeRessource(FiltreNavigation.Actions.ToString()));
                    _typeRessources.Add(new TypeRessource(FiltreNavigation.Mesures.ToString()));
                }
                return _typeRessources;
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

        public TypeRessourceViewModel()
            : base()
        {
            
            // Define commands
            GetRessourceCommand = new ActionCommand<object>(
                obj => GetRessource(obj));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        protected virtual void GetRessource(object MaRessource)
        {

            NavService.NavigateUri(new Uri(string.Format("{0}/{1}",
                       CurrentNavigation.Current.BaseUrlWithOutFilter,
                       ((TypeRessource)MaRessource).Libelle), UriKind.Relative));

            //foreach (var Resx in this.TypeRessources.Where(r => r.IsSelected != true))
            //{
            //    Resx.IsSelected = false;
            //}
            //RaisePropertyChanged(() => this.TypeRessources);
        }

        #endregion

        #region Command

        /// <summary>
        /// Déclaration de l'objet de command de réintégration de l'utilisateur
        /// </summary>
        public IActionCommand GetRessourceCommand { get; private set; }

        #endregion

        public void OnImportsSatisfied()
        {
            EventAggregator.Subscribe<CurrentNavigation>(this);
            EventAggregator.Subscribe<ViewMode>(this);

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
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(TypeRessource).Name));
            }
            // If no error is returned, set the model to entities
            else
            {
                RaisePropertyChanged(() => this.TypeRessources);               
            }
        }

        public void HandleEvent(CurrentNavigation publishedEvent)
        {
            if (publishedEvent.Filtre != null && TypeRessources != null)
            {
                var typeResx = TypeRessources.Where(te => te.Libelle == publishedEvent.Filtre.GetStringValue());
                if (typeResx.Any())
                {
                    typeResx.First().IsSelected = true;
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
