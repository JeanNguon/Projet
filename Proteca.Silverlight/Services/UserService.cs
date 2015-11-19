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
using System.Threading;

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

        private ClientContext clientContext;
        
        [ImportingConstructor]
        public UserService([Import(AllowDefault = true)] IConfigurator configurator)
        {
            clientContext = configurator.GetClientContext();

            if (clientContext != null)
            {
                clientContext.Load(clientContext.Web, s => s.CurrentUser);
                clientContext.ExecuteQueryAsync((sender, args) =>
                {
                    if (clientContext.Web != null)
                    {
                        CurrentSharepointUser = clientContext.Web.CurrentUser;
                    }
                    if (CurrentSharepointUser == null)
                    {
                        Logger.Log(LogSeverity.Error, "UserService", "Impossible de charger l'utilisateur sharepoint");
                    }
                }, null);
            }
            // Uniquement en mode débug, permet de récupérer le login donné dans les initParams
#if DEBUG
            if (clientContext == null && App.Current.Resources.Contains("DebugLogin"))
            {
                string login = App.Current.Resources["DebugLogin"].ToString();
                EventHandler onImportSatisfied = null;
                onImportSatisfied = (o, e) =>
                {
                    loadUsrUtilisateur(login, null);
                    ImportSatisfied -= onImportSatisfied;
                };
                ImportSatisfied += onImportSatisfied;
            }
#endif

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
                    if (IsImportSatisfied)
                    {
                        loadUsrUtilisateur(value.LoginName, _getEntitiesCompleted);
                    }
                    else
                    {
                        EventHandler onImportSatisfied = null;
                        onImportSatisfied = (o, e) =>
                        {
                            loadUsrUtilisateur(value.LoginName, _getEntitiesCompleted);
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

        public bool IsLoadUsrInProgress
        {
            get;
            set;
        }

        public bool IsGetEntitiesInProgress
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


        //private void loadUsrUtilisateurAsync(object login)
        //{
        //    string _login = (string)login;
        //    loadUsrUtilisateur(_login);  
        //}

        private void loadUsrUtilisateur(string login, Action<Exception> action = null)
        {
            // On récupère le login sans le domaine
            string simpleLogin = login.Contains("\\") ? login.Substring(login.IndexOf("\\") + 1) : login;

            IsLoadUsrInProgress = true;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            ((UsrUtilisateurService)serviceUtilisateur).GetEntityByIdentifiant(simpleLogin, (error, user) =>
            {
                if (error == null && user != null)
                {
                    if (CurrentUser == null || CurrentUser.CleUtilisateur != user.CleUtilisateur)
                    {
                        CurrentUser = user;
                        CurrentUser.SetPreferenceCleRegion(user.GeoAgence.CleRegion);
                        CurrentUser.SetPreferenceCleAgence(user.CleAgence);
                        CurrentUser.SetPreferenceCleSecteur(user.CleSecteur);
                    }
                    else
                    {
                        int? portion = CurrentUser.PreferenceClePortion;
                        int? ee = CurrentUser.PreferenceCleEnsembleElectrique;
                        int? secteur = CurrentUser.PreferenceCleSecteur;
                        int? agence = CurrentUser.PreferenceCleAgence;
                        int? region = CurrentUser.PreferenceCleRegion;
                        CurrentUser = user;
                        CurrentUser.SetPreferenceClePortion(portion);
                        CurrentUser.SetPreferenceCleEnsembleElectrique(ee);
                        CurrentUser.SetPreferenceCleSecteur(secteur);
                        CurrentUser.SetPreferenceCleAgence(agence);
                        CurrentUser.SetPreferenceCleRegion(region);
                    }
                    
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
                    Logger.Log(LogSeverity.Error, "UserService", string.Format("Impossible de charger l'utilisateur proteca correspondant à l'utilisateur sharepoint suivant : {0}", login));
                    if (action != null)
                    {
                        action(error);
                    }
                }
                
                IsLoadUsrInProgress = false;
                IsGetEntitiesInProgress = false;
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
            IsGetEntitiesInProgress = true;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if ((this.CurrentUser == null || this.CurrentUser.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Detached) && CurrentSharepointUser != null)
            {
                if (!IsLoadUsrInProgress)
                {
                   // System.Threading.Thread T = new System.Threading.Thread(new ParameterizedThreadStart(this.loadUsrUtilisateurAsync));
                   //   T.Start(CurrentSharepointUser.LoginName);
                   
                    this.loadUsrUtilisateur(CurrentSharepointUser.LoginName, completed);
                }
                else
                {
                    EventHandler onCurrentUserLoaded = null;
                    onCurrentUserLoaded = (o, e) =>
                    {
                        CurrentUserLoaded -= onCurrentUserLoaded;
                        completed(null);
                        IsGetEntitiesInProgress = false;
                    };
                    CurrentUserLoaded += onCurrentUserLoaded;
                }
                
            }
            else if (this.CurrentSharepointUser != null || CurrentUser != null)
            {
                completed(null);
                IsGetEntitiesInProgress = false;
            }
            else if (!IsGetEntitiesInProgress)
            {
                _getEntitiesCompleted = completed;
            }
            else
            {
                EventHandler onCurrentUserLoaded = null;
                onCurrentUserLoaded = (o, e) =>
                {
                    CurrentUserLoaded -= onCurrentUserLoaded;
                    completed(null);
                    IsGetEntitiesInProgress = false;
                };
                CurrentUserLoaded += onCurrentUserLoaded;
            }
            // Uniquement en mode débug, permet de récupérer le login donné dans les initParams
#if DEBUG
            if ((this.CurrentUser == null || this.CurrentUser.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Detached) && clientContext == null && App.Current.Resources.Contains("DebugLogin"))
            {
                string login = App.Current.Resources["DebugLogin"].ToString();
                loadUsrUtilisateur(login, completed);
            }
#endif
        }
    }
}
