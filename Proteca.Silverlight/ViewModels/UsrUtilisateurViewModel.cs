using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for UsrUtilisateur entity
    /// </summary>
    [ExportAsViewModel("UsrUtilisateur")]
    public class UsrUtilisateurViewModel : BaseProtecaEntityViewModel<UsrUtilisateur>
    {

        #region Properties

        public bool IsNonEditableTileItemState
        {
            get
            {
                return MainTileItemState == Telerik.Windows.Controls.TileViewItemState.Minimized;
            }
        }

        /// <summary>
        /// Retourne si l'identifiant est mode read only
        /// </summary>
        public Boolean IsIdentifiantEnable
        {
            get
            {
                return IsEditMode && (this.SelectedEntity != null && !this.SelectedEntity.Externe);
            }
        }

        /// <summary>
        /// Retourne si l'utilisateur courant est un admin et si on est mode read only
        /// </summary>
        public Boolean IsAdministrateurViewMode
        {
            get
            {
                return this.CurrentUser != null ? !IsEditMode && this.CurrentUser.IsAdministrateur : false;
            }
        }

        /// <summary>
        /// Retourne la liste des Profils utilisateurs
        /// </summary>
        public ObservableCollection<UsrProfil> Profils
        {
            get
            {
                if (this.CurrentUser != null && this.IsEditMode && !this.CurrentUser.IsAdministrateur)
                {
                    return new ObservableCollection<UsrProfil>(serviceProfil.Entities.Where(p => !p.ProfilAdmin && p.RefUsrPortee != null && (int.Parse(p.RefUsrPortee.CodePortee) >= int.Parse(this.CurrentUser.RefUsrPortee.CodePortee) || this.SelectedEntity.CleProfil.HasValue && p.CleProfil == this.SelectedEntity.CleProfil)).OrderBy(p => p.LibelleProfil));
                }
                else
                {
                    return new ObservableCollection<UsrProfil>(serviceProfil.Entities.OrderBy(p => p.LibelleProfil));
                }
            }
        }

        /// <summary>
        /// Retourne l'historique Admin
        /// </summary>
        public ObservableCollection<HistoAdmin> HistoAdmins
        {
            get
            {
                return serviceHistoAdmin.Entities;
            }
        }


        public ObservableCollection<RefEnumValeur> TypesOperation
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities.Where(r => r.CodeGroupe == "TYPE_HISTO_ADMIN"));
            }
        }

        /// <summary>
        /// Retourne les régions du service région
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return new ObservableCollection<GeoRegion>(serviceRegion.Entities.OrderBy(r => r.LibelleRegion)); }
        }

        /// <summary>
        /// Retourne la liste d'entités RefUsrPortee
        /// </summary>
        public ObservableCollection<RefUsrPortee> RefUsrPortees
        {
            get
            {
                if (serviceRefUsrPortee.Entities != null && serviceRefUsrPortee.Entities.Any(u => u.TypePortee == Global.Constants.RefUsrPortee_TYPE))
                {
                    return new ObservableCollection<RefUsrPortee>(serviceRefUsrPortee.Entities.Where(u => u.TypePortee == Global.Constants.RefUsrPortee_TYPE));
                }
                return new ObservableCollection<RefUsrPortee>();
            }
        }

        private bool _canEditRegion;
        public bool CanEditRegion
        {
            get
            {
                return _canEditRegion && IsEditMode;
            }
            set
            {
                _canEditRegion = value;
                this.RaisePropertyChanged(() => this.CanEditRegion);
            }
        }

        private bool _canEditAgence;
        public bool CanEditAgence
        {
            get
            {
                return _canEditAgence && IsEditMode;
            }
            set
            {
                _canEditAgence = value;
                this.RaisePropertyChanged(() => this.CanEditAgence);
            }
        }

        private bool _canEditSecteur;
        public bool CanEditSecteur
        {
            get
            {
                return _canEditSecteur && IsEditMode;
            }
            set
            {
                _canEditSecteur = value;
                this.RaisePropertyChanged(() => this.CanEditSecteur);
            }
        }

        /// <summary>
        /// Retourne si c'est un admin au chargement initial
        /// </summary>
        public bool CurrentIsAdmin { get; set; }

        /// <summary>
        /// Déclaration de la variable FiltreNomPrenom
        /// </summary>
        public string FiltreNomPrenom { get; set; }

        /// <summary>
        /// Déclaration de la variable FiltreIdentifiant
        /// </summary>
        public string FiltreIdentifiant { get; set; }

        /// <summary>
        /// Déclaration de la variable SelectedSearchEntities
        /// </summary>
        public UsrUtilisateur SelectedSearchEntities { get; set; }

        /// <summary>
        /// Déclaration de la variable IsManager
        /// </summary>
        public Boolean IsManager { get; set; }

        /// <summary>
        /// Déclaration de la variable IsDelete
        /// </summary>
        public Boolean IsDelete { get; set; }

        /// <summary>
        /// Retourne la valeur True si l'utilisateur sélectionné est "Supprime" et que le mode d'édition est activé
        /// </summary>
        public Boolean IsReintegrateEnable
        {
            get
            {
                if (SelectedEntity != null)
                {
                    return SelectedEntity.Supprime && IsEditMode;
                }
                return false;
            }
        }

        /// <summary>
        /// Retourne la valeur True si l'utilisateur sélectionné est "Supprime" et que le mode d'édition est activé
        /// </summary>
        public Boolean IsDeleteEnable
        {
            get
            {
                if (SelectedEntity != null)
                {
                    return !SelectedEntity.Supprime && IsEditMode && !IsNewMode;
                }
                return false;
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Constructeur du ViewModel UsrUtilisateur
        /// </summary>
        public UsrUtilisateurViewModel()
            : base()
        {
            this.OnViewModeChanged += (o, e) =>
            {
                if (IsNewMode)
                {
                    if (this.EntitiesCount > 0)
                    {
                        this.SelectedEntity.UsrProfil = Profils.Where(p => p.LibelleProfil == Global.Constants.UsrProfil_DEFAULT).FirstOrDefault();
                        this.SelectedEntity.RefUsrPortee = RefUsrPortees.Where(p => p.CodePortee == Global.Constants.RefUsrPortee_DEFAULT).FirstOrDefault();
                    }
                }

                RaisePropertyChanged(() => this.IsIdentifiantEnable);
                RaisePropertyChanged(() => this.IsReintegrateEnable);
                RaisePropertyChanged(() => this.IsDeleteEnable);
                RaisePropertyChanged(() => this.CanEditRegion);
                RaisePropertyChanged(() => this.CanEditAgence);
                RaisePropertyChanged(() => this.CanEditSecteur);
                RaisePropertyChanged(() => this.Profils);
                RaisePropertyChanged(() => this.IsAdministrateurViewMode);

                if (SelectedEntity != null)
                {
                    SelectedEntity.IsEditable = IsEditMode;
                }

            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.Profils);
                RaisePropertyChanged(() => this.RefUsrPortees);
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.IsAdministrateurViewMode);
                RaisePropertyChanged(() => this.HistoAdmins);
            };

            this.OnDetailLoaded += (o, e) =>
            {
                CurrentIsAdmin = this.SelectedEntity.IsAdministrateur;

                RaisePropertyChanged(() => this.IsIdentifiantEnable);
                RaisePropertyChanged(() => this.IsReintegrateEnable);
                RaisePropertyChanged(() => this.IsDeleteEnable);
                RaisePropertyChanged(() => this.SelectedEntity);
                RaisePropertyChanged(() => this.HistoAdmins);
                RaisePropertyChanged(() => this.IsAdministrateurViewMode);

                if (this.CurrentUser != null && this.CurrentUser.RefUsrPortee != null)
                {
                    switch (this.CurrentUser.RefUsrPortee.GetPorteesEnum())
                    {
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                        case RefUsrPortee.ListPorteesEnum.Interdite:
                            this.CanEditRegion = false;
                            this.CanEditAgence = false;
                            this.CanEditSecteur = false;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Autorisee:
                        case RefUsrPortee.ListPorteesEnum.National:
                            this.CanEditRegion = true;
                            this.CanEditAgence = true;
                            this.CanEditSecteur = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Region:
                            this.CanEditRegion = false;
                            this.CanEditAgence = true;
                            this.CanEditSecteur = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            this.CanEditRegion = false;
                            this.CanEditAgence = false;
                            this.CanEditSecteur = true;
                            break;
                    }
                }

                RaisePropertyChanged(() => this.Profils);

                ((HistoAdminService)serviceHistoAdmin).GetEntities((err) => HistoAdminLoaded(err));
            };

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Recherche des Utilisateurs"));
                    EventAggregator.Publish("UsrUtilisateur_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };

            this.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "MainTileItemState")
                {
                    RaisePropertyChanged(() => IsNonEditableTileItemState);
                }
            };

            // Define commands
            IntegrateUserCommand = new ActionCommand<object>(
                obj => IntegrateUser(), obj => true);

            RequestADCommand = new ActionCommand<object>(
                obj => RequestAD(), obj => true);

            this.OnAddedEntity += (o, e) =>
            {
                if (this.CurrentUser != null && this.CurrentUser.RefUsrPortee != null)
                {
                    switch (this.CurrentUser.RefUsrPortee.GetPorteesEnum())
                    {
                        case RefUsrPortee.ListPorteesEnum.Secteur:
                        case RefUsrPortee.ListPorteesEnum.Interdite:
                            this.SelectedEntity.SelectedRegion = CurrentUser.GeoAgence.GeoRegion;
                            this.SelectedEntity.SelectedAgence = CurrentUser.GeoAgence;
                            this.SelectedEntity.SelectedSecteur = CurrentUser.GeoSecteur;
                            this.CanEditRegion = false;
                            this.CanEditAgence = false;
                            this.CanEditSecteur = false;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Region:
                            this.SelectedEntity.SelectedRegion = CurrentUser.GeoAgence.GeoRegion;
                            this.CanEditRegion = false;
                            this.CanEditAgence = true;
                            this.CanEditSecteur = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Agence:
                            this.SelectedEntity.SelectedRegion = CurrentUser.GeoAgence.GeoRegion;
                            this.SelectedEntity.SelectedAgence = CurrentUser.GeoAgence;
                            this.CanEditRegion = false;
                            this.CanEditAgence = false;
                            this.CanEditSecteur = true;
                            break;
                        case RefUsrPortee.ListPorteesEnum.Autorisee:
                        case RefUsrPortee.ListPorteesEnum.National:
                            this.CanEditRegion = true;
                            this.CanEditAgence = true;
                            this.CanEditSecteur = true;
                            break;
                    }
                    RaisePropertyChanged(() => this.SelectedEntity);
                }
            };

            this.OnDeleteSuccess += (o, e) =>
            {
                ((HistoAdminService)serviceHistoAdmin).GetEntities((err) => HistoAdminLoaded(err));
                RaisePropertyChanged(() => this.HistoAdmins);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                CurrentIsAdmin = this.SelectedEntity.IsAdministrateur;
            };
        }

        #endregion

        #region Command

        /// <summary>
        /// Déclaration de l'objet de command de réintégration de l'utilisateur
        /// </summary>
        public IActionCommand IntegrateUserCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command d'interogation de l'ActiveDirectory
        /// </summary>
        public IActionCommand RequestADCommand { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Surcharge de la méthode Delete du service
        /// </summary>
        /// <param name="skipNavigation"></param>
        protected override void Delete(bool skipNavigation)
        {
            var result = System.Windows.MessageBox.Show(Resource.UsrUtilisateur_DeleteConfirmation, "", System.Windows.MessageBoxButton.OKCancel);
            if (result == System.Windows.MessageBoxResult.OK)
            {
                ((UsrUtilisateurService)this.service).CheckDeleteUsrUtilisateurList(this.SelectedEntity.CleUtilisateur, (retour, error) =>
                    {
                        serviceHistoAdmin.Clear();

                        // Si l'utilisateur sélectionné est de type admin, on enregistre la modif dans HistoAdmin
                        if (CurrentIsAdmin || this.SelectedEntity.IsAdministrateur)
                        {
                            HistoAdmin NewItem =
                                new HistoAdmin()
                                {
                                    IdConnecte = CurrentUser.Identifiant,
                                    NomConnecte = CurrentUser.Nom,
                                    PrenomConnecte = CurrentUser.Prenom,
                                    IdUtilisateur = SelectedEntity.Identifiant,
                                    NomUtilisateur = SelectedEntity.Nom,
                                    PrenomUtilisateur = SelectedEntity.Prenom,
                                    TypeCompte = SelectedEntity.UsrProfil.LibelleProfil,
                                    DateModification = DateTime.Now,
                                    RefEnumValeur = TypesOperation.Where(r => r.Valeur == "S").FirstOrDefault()
                                };

                            //serviceHistoAdmin.Add(NewItem);
                            HistoAdmins.Add(NewItem);
                        }

                        if (error != null)
                        {
                            //todo
                        }
                        else if (retour)
                        {
                            // Suppression logique
                            this.SelectedEntity.Supprime = true;

                            // MAJ du contenu
                            RaisePropertyChanged(() => this.SelectedEntity);
                            RaisePropertyChanged(() => this.IsReintegrateEnable);
                            RaisePropertyChanged(() => this.IsDeleteEnable);
                            RaisePropertyChanged(() => this.HistoAdmins);
                        }
                        else
                        {
                            // MAJ du mode d'édition
                            IsEditMode = false;

                            // On supprime si aucun lien dans d'autres tables
                            base.Delete(false, true);

                            // MAJ de la vue
                            RaisePropertyChanged(() => this.SelectedEntity);
                            RaisePropertyChanged(() => this.IsReintegrateEnable);
                            RaisePropertyChanged(() => this.IsDeleteEnable);
                            RaisePropertyChanged(() => this.HistoAdmins);
                        }
                    }
                    );
            }


        }

        /// <summary>
        /// Fonction de récupération des données ActiveDirectory
        /// </summary>
        protected virtual void RequestAD()
        {
            IsBusy = true;
            // Récupération de l'ID
            string id_ActiveDirectory = this.SelectedEntity.Identifiant;

            // Appel du service AD
            ((UsrUtilisateurService)this.service).FindADEntityByCle(id_ActiveDirectory, (error) => ADUserByCleLoaded(id_ActiveDirectory, error));

        }

        /// <summary>
        /// Fonction de réintégration de l'utilisateur
        /// </summary>
        protected virtual void IntegrateUser()
        {
            this.SelectedEntity.Supprime = false;
            RaisePropertyChanged(() => this.IsReintegrateEnable);
            RaisePropertyChanged(() => this.IsDeleteEnable);
        }

        /// <summary>
        /// Méthode utilisé pour charger les entités de type ADUser
        /// </summary>
        private void ADUsersLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(ADUser).Name));
            }
            else
            {
                RaisePropertyChanged(() => ((UsrUtilisateurService)this.service).ADEntities);
            }

            // We're done
            IsBusy = false;
        }

        /// <summary>
        /// Méthode utilisé pour charger les entités de type ADUser
        /// </summary>
        /// <param name="AccountName">Compte Utilisateur windows/ Identifiant</param>
        /// <param name="error"></param>
        private void ADUserByCleLoaded(string AccountName, Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(ADUser).Name));
            }
            else
            {
                if (((UsrUtilisateurService)this.service).DetailADEntity.IsError)
                {
                    InfoWindow.CreateNew(Resource.Utilisateur_EmptyAD);
                }
                else
                {
                    //Mise à jour des champs

                    this.SelectedEntity.Nom = ((UsrUtilisateurService)this.service).DetailADEntity.SN.ToString();
                    this.SelectedEntity.Prenom = ((UsrUtilisateurService)this.service).DetailADEntity.Givenname.ToString();
                    int countstring = ((UsrUtilisateurService)this.service).DetailADEntity.Email.Count();
                    if (countstring > 0)
                    {
                        this.SelectedEntity.Mail = ((UsrUtilisateurService)this.service).DetailADEntity.Email;
                    }
                }

                RaisePropertyChanged(() => ((UsrUtilisateurService)this.service).ADEntities);
            }

            // We're done
            IsBusy = false;
        }

        protected override void Save()
        {
            // Si l'utilisateur sélectionné est de type admin, on enregistre la modif dans HistoAdmin
            if (CurrentIsAdmin || this.SelectedEntity.IsAdministrateur)
            {
                HistoAdmin NewItem =
                    new HistoAdmin()
                    {
                        IdConnecte = CurrentUser.Identifiant,
                        NomConnecte = CurrentUser.Nom,
                        PrenomConnecte = CurrentUser.Prenom,
                        IdUtilisateur = SelectedEntity.Identifiant,
                        NomUtilisateur = SelectedEntity.Nom,
                        PrenomUtilisateur = SelectedEntity.Prenom,
                        TypeCompte = SelectedEntity.UsrProfil.LibelleProfil,
                        DateModification = DateTime.Now
                    };

                if (IsNewMode)
                {
                    NewItem.RefEnumValeur = TypesOperation.Where(r => r.Valeur == "C").FirstOrDefault();
                }
                else
                {
                    NewItem.RefEnumValeur = TypesOperation.Where(r => r.Valeur == "M").FirstOrDefault();
                }

                HistoAdmins.Insert(0, NewItem);

                RaisePropertyChanged(() => this.HistoAdmins);
            }

            base.Save();

            // Ajout de la validation sur le SelectedRegion dans le cas où l'erreur est remonté par l'agence et que la région est null
            if (IsEditMode && SelectedEntity.ValidationErrors.Any(ve => ve.MemberNames.Contains("CleAgence")) && SelectedEntity.SelectedRegion == null)
            {
                ValidationResult vr = SelectedEntity.ValidationErrors.First(ve => ve.MemberNames.Contains("CleAgence"));
                SelectedEntity.ValidationErrors.Add(new ValidationResult(vr.ErrorMessage, new List<String>() { "SelectedRegion" }));
            }
        }

        /// <summary>
        /// Méthode de recherche appellé par la commande FindCommand
        /// cette méthode appelle la méthode Find du service
        /// pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            this.Filtres = new List<System.Linq.Expressions.Expression<Func<UsrUtilisateur, bool>>>();
            this.Filtres.Add(u => this.FiltreCleRegion == null || u.GeoAgence.CleRegion == this.FiltreCleRegion);
            this.Filtres.Add(u => this.FiltreCleSecteur == null || u.CleSecteur == FiltreCleSecteur);
            this.Filtres.Add(u => this.FiltreCleAgence == null || u.CleAgence == FiltreCleAgence);


            if (FiltreNomPrenom != null)
            {
                List<String> wordsFiltreNomPrenom = FiltreNomPrenom.Split(' ').ToList<string>();

                foreach (string word in wordsFiltreNomPrenom)
                {
                    // Workaround : On force l'utilisation de constantes dans l'expression à la place des variables "word" en construisant "à la main" l'expression lambda et
                    // en utilisant Expression.Constant(..)
                    // Le code suivant est équivalent à : this.Filtres.Add(u => u.Nom.Contains(word) || u.Prenom.Contains(word));
                    var param = Expression.Parameter(typeof(UsrUtilisateur), "u");
                    MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var body = Expression.OrElse(
                        Expression.Call(Expression.PropertyOrField(param, "Nom"), method,
                        Expression.Constant(word, typeof(string))),
                        Expression.Call(Expression.PropertyOrField(param, "Prenom"), method,
                        Expression.Constant(word, typeof(string))));
                    var exp = Expression.Lambda<Func<UsrUtilisateur, bool>>(body, param);
                    this.Filtres.Add(exp);
                }
            }

            this.Filtres.Add(u => this.FiltreIdentifiant == null || u.Identifiant.Contains(FiltreIdentifiant));
            this.Filtres.Add(u => this.IsManager == false || (this.IsManager == true && u.RefUsrPortee.CodePortee != "05"));
            this.Filtres.Add(u => this.IsDelete == true || (this.IsDelete == false && u.Supprime == false));
            base.Find();

        }

        #endregion

        #region Autorisation

        protected override bool GetUserCanEdit()
        {
            return getDroitUtilisateur();
        }

        protected override bool GetUserCanDelete()
        {
            return getDroitUtilisateur();
        }

        protected override bool GetUserCanAdd()
        {
            bool result = false;
            if (this.CurrentUser != null && this.CurrentUser.RefUsrPortee != null)
            {
                if (this.CurrentUser.RefUsrPortee.CodePortee != RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    result = true;
                }
            }
            return result;
        }

        protected override bool GetUserCanSave()
        {
            if (this.SelectedEntity.IsNew())
                return true;
            return getDroitUtilisateur();
        }

        private bool getDroitUtilisateur()
        {
            bool result = false;
            if (this.CurrentUser != null && this.SelectedEntity != null && !this.SelectedEntity.IsNew())
            {
                switch (this.CurrentUser.RefUsrPortee.GetPorteesEnum())
                {
                    case RefUsrPortee.ListPorteesEnum.Interdite:
                        result = false;
                        break;
                    case RefUsrPortee.ListPorteesEnum.Autorisee:
                    case RefUsrPortee.ListPorteesEnum.National:
                        result = true;
                        break;
                    case RefUsrPortee.ListPorteesEnum.Region:
                        result = this.SelectedEntity.SelectedRegion.CleRegion == this.userService.CurrentUser.GeoAgence.CleRegion;
                        break;
                    case RefUsrPortee.ListPorteesEnum.Agence:
                        result = this.SelectedEntity.CleAgence == this.userService.CurrentUser.CleAgence;
                        break;
                    case RefUsrPortee.ListPorteesEnum.Secteur:
                        result = this.SelectedEntity.CleSecteur == this.userService.CurrentUser.CleSecteur;
                        break;
                }

                if (this.SelectedEntity.IsAdministrateur && !this.CurrentUser.IsAdministrateur)
                {
                    result = false;
                }
            }

            return result;
        }


        #endregion

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type Profil
        /// </summary>
        [Import]
        public IEntityService<UsrProfil> serviceProfil { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type REf_USER_Portee
        /// </summary>
        [Import]
        public IEntityService<RefUsrPortee> serviceRefUsrPortee { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type HistoAdmin
        /// </summary>
        [Import]
        public IEntityService<HistoAdmin> serviceHistoAdmin { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> serviceRefEnumValeur { get; set; }

        /// <summary>
        /// Méthode utilisé pour charger l'entité de type HistoAdmin
        /// </summary>
        private void HistoAdminLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(HistoAdmin).Name));
            }
            else
            {
                RaisePropertyChanged(() => this.HistoAdmins);
            }
        }

        #endregion

    }
}
