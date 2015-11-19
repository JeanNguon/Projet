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
using Proteca.Silverlight.Services.Contracts;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;
using Proteca.Web.Models;
using Proteca.Silverlight.Services.EntityServices;
using Jounce.Core.Application;
using System.Reflection;

namespace Proteca.Silverlight.Services
{
    /// <summary>
    /// Service utilisé pour retrouver l'utilisateur sharepoint connecté et pour récupérer l'utilisateur proteca correspondant
    /// </summary>
    [Export(typeof(IUserService<User>))]
    public class UserService : IUserService<User>, IPartImportsSatisfiedNotification
    {

        #region Events
        public EventHandler CurrentUserLoaded { get; set; }
        public EventHandler ImportSatisfied { get; set; }
        #endregion

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité utilisateur
        /// </summary>
        [Import]
        public IEntityService<UsrUtilisateur> serviceUtilisateur { get; set; }

        
        [ImportingConstructor]
        public UserService([Import(AllowDefault = true)] IConfigurator configurator)
        {
            EventHandler onImportSatisfied = null;
            onImportSatisfied = (o, e) =>
            {
                loadUsrUtilisateur("ProtOn", null);
                ImportSatisfied -= onImportSatisfied;
            };
            ImportSatisfied += onImportSatisfied;
        }

        public bool AuthenticateUser()
        {
            return CurrentSharepointUser != null;
        }

        /// <summary>
        /// Utilisateur sharepoint connecté
        /// </summary>
        private User _currentSharepoinUser;
        public User CurrentSharepointUser
        {
            get
            {
                return _currentSharepoinUser;
            }
            set
            {
                if (value != null && !string.IsNullOrEmpty(value.LoginName))
                {
                    // On récupère le login sans le domaine
                    string login = value.LoginName.Contains("\\") ? value.LoginName.Substring(value.LoginName.IndexOf("\\") + 1) : value.LoginName;
                    if (IsImportSatisfied)
                    {
                        loadUsrUtilisateur(login, _getEntitiesCompleted);
                    }
                    else
                    {
                        EventHandler onImportSatisfied = null;
                        onImportSatisfied = (o, e) =>
                        {
                            loadUsrUtilisateur(login, _getEntitiesCompleted);
                            ImportSatisfied -= onImportSatisfied;
                        };
                        ImportSatisfied += onImportSatisfied;

                    }
                }
                _currentSharepoinUser = value;
            }
        }

        /// <summary>
        /// Utilisateur proteca
        /// </summary>
        public Proteca.Web.Models.UsrUtilisateur CurrentUser
        {
            get;
            set;
        }

        public bool IsImportSatisfied
        {
            get;
            set;
        }

        public void OnImportsSatisfied()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IsImportSatisfied = true;
            if (ImportSatisfied != null)
            {
                this.ImportSatisfied(this, null);
            }
        }

        private void loadUsrUtilisateur(string login, Action<Exception> action = null)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ((UsrUtilisateurService)serviceUtilisateur).GetEntityByIdentifiant(login, (error, user) =>
            {
                if (error == null && user != null)
                {
                    CurrentUser = user;
                    CurrentUser.SetPreferenceCleRegion(user.GeoAgence.CleRegion);
                    CurrentUser.SetPreferenceCleAgence(user.CleAgence);
                    CurrentUser.SetPreferenceCleSecteur(user.CleSecteur);
                    if (action != null)
                    {
                        action(null);
                    }
                    if (CurrentUserLoaded != null)
                    {
                        CurrentUserLoaded(this, null);
                    }
                }
                else
                {
                    Logger.Log(LogSeverity.Error, "UserService", string.Format("Impossible de charger l'utilisateur proteca suivant : {0}", login));
                    if (action != null)
                    {
                        // en mode déporté, on a les droit même sans utilisateur
                        //action(error);
                        action(null);
                    }
                }
            });
        }

        // Callback de retour de getEntities
        Action<Exception> _getEntitiesCompleted;

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            _getEntitiesCompleted = completed;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (this.CurrentSharepointUser != null || (CurrentUser != null && CurrentUser.EntityState != System.ServiceModel.DomainServices.Client.EntityState.Detached))
            {
                completed(null);
            }
            // Uniquement en mode débug, permet de récupérer le login donné dans les initParams
            if (this.CurrentUser == null || CurrentUser.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Detached)
            {
                loadUsrUtilisateur("ProtOn", completed);
            }
        }
    }
}
