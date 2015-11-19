using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;
using Telerik.Windows.Controls;
using Proteca.Silverlight.Helpers;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for EnsembleElectrique entity
    /// </summary>
    [ExportAsViewModel("EnsembleElectrique")]
    public class EnsembleElectriqueViewModel : OuvrageViewModel<EnsembleElectrique>
    {
        #region Private Members

        /// <summary>
        /// Commentaire initiale avant les éventuelles modifications du commentaire
        /// </summary>
        private string CommentaireBeforeUpdate { get; set; }
        
        /// <summary>
        /// Déclaration de la liste des portions
        /// </summary>
        private List<AnAction> _listActions;

        /// <summary>
        /// Déclaration de la liste des portions
        /// </summary>
        private List<PortionDates> _listPortions;

        /// <summary>
        /// Déclaration de l'état du tileview des actions
        /// </summary>
        private TileViewItemState _actionsTileItemState = TileViewItemState.Minimized;

        /// <summary>
        /// Déclaration de l'état du tileview des portions
        /// </summary>
        private TileViewItemState _portionsTileItemState = TileViewItemState.Minimized;

        #endregion Private Members

        #region Public Properties
        /// <summary>
        /// Définit l'état du TileView des actions et permet de charger les actions
        /// </summary>
        public TileViewItemState ActionsTileItemState
        {
            get { return _actionsTileItemState; }
            set
            {
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null && this.ListActions == null)
                {
                    IsBusy = true;
                    ((EnsembleElectriqueService)service).GetListActionsByEnsembleElectriqueQuery(this.SelectedEntity.CleEnsElectrique, GetListActionsDone);
                }
                _actionsTileItemState = value;
                RaisePropertyChanged(() => ActionsTileItemState);
            }
        }

        /// <summary>
        /// Définit l'état du TileView des portions permet de charger les portions
        /// </summary>
        public TileViewItemState PortionsTileItemState
        {
            get { return _portionsTileItemState; }
            set
            {
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null && this.ListPortions == null)
                {
                    IsBusy = true;
                    ((EnsembleElectriqueService)service).GetListPortions(this.SelectedEntity.CleEnsElectrique, GetListPortionsDone);
                }

                _portionsTileItemState = value;
                RaisePropertyChanged(() => PortionsTileItemState);
            }
        }

        /// <summary>
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return serviceRegion.Entities; }
        }

        /// <summary>
        /// Inclue ou non les ensembles électrique sans portions
        /// </summary>
        public bool IncludeWhitoutPortion { get; set; }

        /// <summary>
        /// Inclue ou non les Ensemble électrique de type station
        /// </summary>
        public bool IncludeStation { get; set; }

        /// <summary>
        /// Inclue ou non les ensemble électrique de type Poste Gaz
        /// </summary>
        public bool IncludePosteGaz { get; set; }

        /// <summary>
        /// Filtre sur le libelle de l'ensemble électrique
        /// </summary>
        public string EnsembleElectriqueTitle { get; set; }

        /// <summary>
        /// Indique si la structure complexe est de type Station
        /// </summary>
        public bool IsStation
        {
            get { return this.SelectedEntity != null && this.SelectedEntity.RefEnumValeur1 != null && this.SelectedEntity.RefEnumValeur1.Valeur == "1"; }
            set
            {
                if (value == true)
                {
                    this.SelectedEntity.RefEnumValeur1 = RefEnumValeurStructureCplx.FirstOrDefault(r => r.Valeur == "1");
                    if (this.IsPosteGaz)
                    {
                        this.IsPosteGaz = false;
                    }
                }
                else
                {
                    this.SelectedEntity.RefEnumValeur1 = null;
                }
                RaisePropertyChanged(() => this.IsStation);
                RaisePropertyChanged(() => this.IsPosteGaz);
            }
        }

        /// <summary>
        /// Indique si la structure complexe est de type Poste Gaz
        /// </summary>
        public bool IsPosteGaz
        {
            get { return this.SelectedEntity != null && this.SelectedEntity.RefEnumValeur1 != null && this.SelectedEntity.RefEnumValeur1.Valeur == "2"; }
            set
            {
                if (value == true)
                {
                    this.SelectedEntity.RefEnumValeur1 = RefEnumValeurStructureCplx.FirstOrDefault(r => r.Valeur == "2");
                }
                else
                {
                    this.SelectedEntity.RefEnumValeur1 = null;
                }
                RaisePropertyChanged(() => this.IsStation);
                RaisePropertyChanged(() => this.IsPosteGaz);
            }
        }

        /// <summary>
        /// Liste des périodicité
        /// </summary>
        public List<RefEnumValeur> RefEnumValeurPeriodicite
        {
            get
            {
                return ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.EE_PERIODICITE.GetStringValue()).ToList();
            }
        }

        /// <summary>
        /// Liste des périodicité
        /// </summary>
        public List<RefEnumValeur> RefEnumValeurStructureCplx
        {
            get
            {
                return ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.EE_STRUCTURE_CPLX.GetStringValue()).ToList();
            }
        }

        /// <summary>
        /// Retourne la liste des colonne du tableau des Portions intégrités a ne pas exporter
        /// </summary>
        public List<string> ColumnsHiddenToExport
        {
            get { return null; }
        }

        /// <summary>
        /// Liste des portions avec les dates
        /// </summary>
        public List<PortionDates> ListPortions
        {
            get
            {
                if (_listPortions != null)
                {
                    return _listPortions.OrderBy(ee => ee.Libelle).ToList();
                }
                return null;
            }
            set
            {
                _listPortions = value;
                RaisePropertyChanged(() => this.ListPortions);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<AnAction> ListActions
        {
            get
            {
                if (_listActions != null)
                {
                    return _listActions.OrderBy(ee => ee.Libelle).ToList();
                }
                return null;
            }
            set
            {
                _listActions = value;
                RaisePropertyChanged(() => this.ListActions);
            }
        }

        #endregion Public Properties

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public EnsembleElectriqueViewModel()
            : base()
        {
            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.EnsembleElecExpanderTitle));
                    EventAggregator.Publish("EnsembleElectrique_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.RefEnumValeurPeriodicite);
                RaisePropertyChanged(() => this.RefEnumValeurStructureCplx);
                ListPortions = null;
                ListActions = null;
                IsBusy = false;
            };

            this.OnDetailLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.IsStation);
                RaisePropertyChanged(() => this.IsPosteGaz);
                RaisePropertyChanged(() => this.Documents);
                this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                ListActions = null;
                ListPortions = null;
                //Au changement d'ensemble électrique on charge les portions si l'utilisateur se trouve sur l'onglet des portions
                if (this.PortionsTileItemState == TileViewItemState.Maximized)
                {
                    IsBusy = true;
                    ((EnsembleElectriqueService)service).GetListPortions(this.SelectedEntity.CleEnsElectrique, GetListPortionsDone);
                }
                //Au changement d'ensemble électrique on charge les actions si l'utilisateur se trouve sur l'onglet des actions
                if (this.ActionsTileItemState == TileViewItemState.Maximized)
                {
                    IsBusy = true;
                    ((EnsembleElectriqueService)service).GetListActionsByEnsembleElectriqueQuery(this.SelectedEntity.CleEnsElectrique, GetListActionsDone);
                }
            };

            this.OnAddedEntity += (o, e) =>
            {
                RaisePropertyChanged(() => this.IsStation);
                RaisePropertyChanged(() => this.IsPosteGaz);
            };

            this.OnCanceled += (o, e) =>
            {
                RaisePropertyChanged(() => this.IsStation);
                RaisePropertyChanged(() => this.IsPosteGaz);
                if (this.SelectedEntity != null)
                {
                    this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                }
            };
        }

        #endregion Constructor

        #region Services

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        #endregion Services

        #region Override Functions

        /// <summary>
        /// Méthode de recherche appellé par la commande FindCommand
        /// cette méthode appelle la méthode Find du service
        /// pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            IsBusy = true;

            base.saveGeoPreferences();

            ((EnsembleElectriqueService)this.service).FindEnsembleElectriqueByCriterias(this.FiltreCleRegion, this.FiltreCleAgence, this.FiltreCleSecteur,
                   this.IncludeWhitoutPortion, this.IncludeStation, this.IncludePosteGaz, this.EnsembleElectriqueTitle, SearchDone);
        }

        /// <summary>
        /// Suppression logique d'un ensemble électrique
        /// </summary>
        protected override void Delete()
        {
            MessageBoxResult result = MessageBox.Show(Resource.EnsElec_DeleteConfirmation,
               Resource.EnsElec_DeleteCaption, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                ((EnsembleElectriqueService)service).GetDeleteCodeByEnsembleElectrique(this.SelectedEntity.CleEnsElectrique, (error, code) =>
                    {
                        if (code == 1)// Suppression physique
                        {
                            base.Delete(false, true);
                        }
                        else if (code == 2) // Suppression logique 
                        {
                            this.SelectedEntity.Supprime = true;
                            service.SaveChanges(DeleteEnseElecDone);
                        }
                        else if (code == 3) // suppression impossible
                        {
                            MessageBox.Show(Resource.EnsElec_HasNoDeletedPis, Resource.EnsElec_DeleteCaption, MessageBoxButton.OK);
                        }
                    }
                );
            }
        }

        protected override void Save()
        {
            this.Save(false);
        }

        protected override void Save(bool forceSave)
        {
            this.Save(forceSave, false);
        }

        /// <summary>
        /// On met à jour la date de la mise à jour avant la sauvegarde si le commentaire actuel est différent
        /// de l'ancien commentaire
        /// </summary>
        protected override void Save(bool forceSave, bool withHisto)
        {
            if (this.CommentaireBeforeUpdate != this.SelectedEntity.Commentaire)
            {
                if ((string.IsNullOrEmpty(this.CommentaireBeforeUpdate) ^ string.IsNullOrEmpty(this.SelectedEntity.Commentaire)) ||
                    !string.IsNullOrEmpty(this.CommentaireBeforeUpdate) && !string.IsNullOrEmpty(this.SelectedEntity.Commentaire))
                {
                    this.SelectedEntity.DateMajCommentaire = DateTime.Now;
                    this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                }
            }
            base.Save(forceSave, withHisto);
        }

        #endregion Override Functions

        #region Private Functions

        /// <summary>
        /// La liste des portions de l'ensemble électrique vient d'être chargée
        /// </summary>
        /// <param name="error"></param>
        private void GetListPortionsDone(Exception error, List<PortionDates> listPortions)
        {
            IsBusy = false;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                ListPortions = listPortions;
            }
        }

        /// <summary>
        /// La liste des actions de l'ensemble électrique vient d'être chargée
        /// </summary>
        /// <param name="error"></param>
        private void GetListActionsDone(Exception error, List<AnAction> listActions)
        {
            IsBusy = false;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                ListActions = listActions;
            }
        }

        /// <summary>
        /// La suppression de l'ensemble électrique vient d'être réalisée
        /// </summary>
        /// <param name="ex"></param>
        private void DeleteEnseElecDone(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                MessageBox.Show(Resource.EnsElec_IsDeleted, Resource.EnsElec_DeleteCaption, MessageBoxButton.OK);
                Find();
            }
        }

        /// <summary>
        /// La recherche des ensemble électrique vient être terminée
        /// </summary>
        /// <param name="error"></param>
        private void SearchDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(EnsembleElectrique).Name));
            }
            else
            {
                RaisePropertyChanged(() => this.Entities);
                RaisePropertyChanged(() => EntitiesCount);
                RaisePropertyChanged(() => ResultIndicator);
                if (IsAutoNavigateToFirst && this.Entities != null && this.Entities.Any())
                {
                    int _cleEnsElectrique = Entities.First().CleEnsElectrique;
                    if (this.SelectedEntity != null && this.SelectedEntity.CleEnsElectrique == _cleEnsElectrique)
                    {
                        this.IsBusy = false;
                    }
                    else
                    {
                        NavigationService.Navigate(_cleEnsElectrique);
                    }
                }
                else if (this.Entities == null || !this.Entities.Any())
                {
                    ListActions = null;
                    this.SelectedEntity = null;
                    RaisePropertyChanged(() => this.IsStation);
                    RaisePropertyChanged(() => this.IsPosteGaz);
                    NavigationService.NavigateRootUrl();
                    this.IsBusy = false;
                }
                else
                {
                    this.IsBusy = false;
                }
            }
            this.IsBusy = false;
            if (OnFindLoaded != null)
            {
                OnFindLoaded(this, null);
            }
        }

        #endregion Private Functions

        #region Autorisations

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'ajout d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanAdd()
        {
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EE_NIV);

                return (role != null && role.RefUsrPortee != null &&
                    new[] { RefUsrPortee.ListPorteesEnum.National.GetStringValue() ,
                    RefUsrPortee.ListPorteesEnum.Region.GetStringValue() ,
                    RefUsrPortee.ListPorteesEnum.Agence.GetStringValue() ,
                    RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue() ,
                    RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() }
                    .Contains(role.RefUsrPortee.CodePortee));
            }
            return false;
        }

        /// <summary>
        /// Détermine les droist de l'utilisateur courant 
        /// sur la suppression d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanDelete()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'édition d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanEdit()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur la sauvegarde d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanSave()
        {
            return this.SelectedEntity != null && (this.SelectedEntity.IsNew() || GetAutorisation());
        }

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            if (this.CurrentUser != null && this.SelectedEntity != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EE_NIV);
                string codePortee = role.RefUsrPortee.CodePortee;

                if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                    return this.SelectedEntity.PortionIntegrite.Any(pi => pi.PiSecteurs.Any(s => s.GeoSecteur.CleAgence == CurrentUser.CleAgence));
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() || codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                    return true;
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                    return false;
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                    return this.SelectedEntity.PortionIntegrite.Any(pi => pi.PiSecteurs.Any(s => s.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion));
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                    return this.SelectedEntity.PortionIntegrite.Any(pi => pi.PiSecteurs.Any(s => s.CleSecteur == CurrentUser.CleSecteur));
            }
            return false;
        }

        #endregion
    }
}
