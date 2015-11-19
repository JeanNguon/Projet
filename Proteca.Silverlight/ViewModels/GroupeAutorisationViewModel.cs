using System.Collections.ObjectModel;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for GroupeAutorisation entity
    /// </summary>
    [ExportAsViewModel("GroupeAutorisation")]
    public class GroupeAutorisationViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {

        #region Services

        [Import]
        public IEntityService<RefUsrPortee> servicePortee { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<RefUsrPortee> PorteeList
        {
            get { return servicePortee.Entities;}
        }

        #endregion


        #region Constructor

        public GroupeAutorisationViewModel()
            : base()
        {
           
        }

        #endregion

        #region Public Methods

        public void OnImportsSatisfied()
        {
            servicePortee.GetEntities((error) => PorteesLoaded(error));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        private void PorteesLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GroupeAutorisationViewModel).Name));
            }
            // If no error is returned, set the model to entities
            else
            {
                RaisePropertyChanged(() => this.PorteeList);
            }
        }

        #endregion
    }
}
