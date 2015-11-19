using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using System.Text;
using Proteca.Web.Models;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Services.EntityServices;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for UsrProfil entity
    /// </summary>
    [ExportAsViewModel("UsrProfil")]
    public class UsrProfilViewModel : BaseProtecaEntityViewModel<UsrProfil>
    {

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de Autorisation
        /// </summary>
        [Import]
        public IEntityService<RefUsrAutorisation> serviceAutorisation { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de Portee
        /// </summary>
        [Import]
        public IEntityService<RefUsrPortee> servicePortee { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité utilisateur
        /// </summary>
        [Import]
        public IEntityService<UsrUtilisateur> serviceUtilisateur { get; set; }

        #endregion

        #region Attributes

        /// <summary>
        /// 
        /// </summary>
        private Boolean IsDetailLoaded = false;

        #endregion

        #region Properties
        
        /// <summary>
        /// Redéfinition des entities pour avoir uniquement les profils éditables
        /// </summary>
        public override ObservableCollection<UsrProfil> Entities
        {
            get
            {
                return new ObservableCollection<UsrProfil>(base.Entities.Where(u => u.Editable == true));
            }
        }
        
        /// <summary>
        /// Retourne la liste des portées
        /// </summary>
        public ObservableCollection<RefUsrPortee> Portees
        {
            get
            {
                return servicePortee.Entities;
            }
        }

        /// <summary>
        /// Retourne la liste des portées liés au profil
        /// </summary>
        public ObservableCollection<RefUsrPortee> PorteesProfil
        {
            get
            {
                return servicePortee.Entities != null ? 
                    new ObservableCollection<RefUsrPortee>(servicePortee.Entities.Where(r => r.TypePortee == "PRO").OrderBy(u => u.CodePortee)) 
                    : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<UsrUtilisateur> Utilisateurs
        {
            get
            {
                return serviceUtilisateur.Entities;
            }
        }

        /// <summary>
        /// Retourne la liste des Rôles
        /// </summary>
        public List<UsrRole> GetListRole
        {
            get
            {
                return this.SelectedEntity.UsrRole.ToList();
            }
        }

        /// <summary>
        /// Retourne la liste des rôles par groupe d'autorisation
        /// </summary>
        public ObservableCollection<RefUsrGroupe> GroupeRoles
        {
            get
            {
                if (SelectedEntity != null)
                {
                    return new ObservableCollection<RefUsrGroupe>(this.SelectedEntity.UsrRole.Select(r => r.RefUsrAutorisation.RefUsrGroupe).Distinct());
                }
                else
                {
                    return new ObservableCollection<RefUsrGroupe>();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur UsrProfil
        /// </summary>
        public UsrProfilViewModel()
            : base()
        {
            IsAutoNavigateToFirst = true;
            
            this.OnAllServicesLoaded += (oo, ee) =>
            {
                RaisePropertyChanged(() => this.Portees);
                RaisePropertyChanged(() => this.PorteesProfil);
            };

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Recherche des Profils"));
                    EventAggregator.Publish("UsrProfil_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };
            
            this.OnDetailLoaded += (o, e) =>
            {
                IsDetailLoaded = true;
                CheckAutorisations();

                // MAJ des listes
                RaisePropertyChanged(() => this.GroupeRoles);
                RaisePropertyChanged(() => this.PorteesProfil);
            };

            this.OnAddedEntity += (o, e) =>
            {
                SelectedEntity.Editable = true;
                CheckAutorisations();
                RaisePropertyChanged(() => this.GroupeRoles);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                RaisePropertyChanged(() => this.PreviousUri);
                RaisePropertyChanged(() => this.NextUri);
            };

            this.OnCanceled += (o, e) =>
            {
                NavigationService.NavigateToPreviousView();
            };

        }

        #endregion

        #region Private Methods

        private void populatePortees()
        {
            foreach (UsrRole role in this.SelectedEntity.UsrRole)
            {
                role.Portees = new ObservableCollection<RefUsrPortee>(Portees.Where(p => p.TypePortee == role.RefUsrPortee.TypePortee));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        private void autorisationLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(UsrProfilViewModel).Name));
            }
            // If no error is returned, set the model to entities
            else
            {
                CheckAutorisations();
            }

            IsBusy = false;
        }
        
        /// <summary>
        /// Vérifie que le profil sélectionné contient bien un role
        /// pour chaque autorisation présente en base de donnée.
        /// Auquel cas il ajoutera un nouveau rôle au profil
        /// </summary>
        private void CheckAutorisations()
        {
            if (IsDetailLoaded && serviceAutorisation.Entities != null && serviceAutorisation.Entities.Any())
            {
                IEnumerable<RefUsrAutorisation> autorisationsToCreate = serviceAutorisation.Entities.Except(SelectedEntity.UsrRole.Select(r => r.RefUsrAutorisation));
                foreach (RefUsrAutorisation autorisation in autorisationsToCreate)
                {
                    SelectedEntity.UsrRole.Add(new UsrRole() { RefUsrAutorisation = autorisation, RefUsrPortee = Portees.FirstOrDefault(p => p.TypePortee == autorisation.TypePortee) });
                }

                populatePortees();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skipNavigation"></param>
        protected override void Delete(bool skipNavigation)
        {
            if (this.SelectedEntity.UsrUtilisateur.Where(u => u.Supprime != true).Count() > 0)
            {
                var result = MessageBox.Show(Resource.BaseProtecaEntityViewModel_DeleteProfilNotAllowed, "", MessageBoxButton.OK);
            }
            else
            {
                foreach (UsrUtilisateur usrtochange in this.SelectedEntity.UsrUtilisateur)
                {
                    usrtochange.UsrProfil = null;
                }

                base.Delete(skipNavigation);
            }
        }
        
        #endregion

    }
}
