using Jounce.Core;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System.Linq;
using Jounce.Core.Application;
using Proteca.Silverlight.Services.EntityServices;
using System.Reflection;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using System.Collections.Generic;
using Proteca.Silverlight.Enums;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using System;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Views.Windows;
using System.Collections.ObjectModel;
using Proteca.Silverlight.Enums.NavigationEnums;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Equipement_LE entity
    /// </summary>
    [ExportAsViewModel("Equipement_LE")]
    public class Equipement_LEViewModel : EquipementViewModel
    {
        #region Private Members

        /// <summary>
        /// Déclaration de la propriété ListPointCommun
        /// </summary>
        private List<string> _listPointCommun;

        /// <summary>
        /// Déclaration de la propriété LiaisonsCommunes
        /// </summary>
        private ObservableCollection<LiaisonCommunes> _liaisonsCommunes;

        /// <summary>
        /// Liste des soutirages que l'on peut associé
        /// </summary>
        private ObservableCollection<EqSoutirage> _soutirages;

        /// <summary>
        /// Listde des drainages que l'on peut associé
        /// </summary>
        private ObservableCollection<EqDrainage> _drainages;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Liste des types de Nom tiers
        /// </summary>
        public List<RefSousTypeOuvrage> TypeNomTiers
        {
            get
            {
                return ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == RefSousTypeOuvrageGroupEnum.TypeNomTiers.GetStringValue())
                .OrderBy(r => r.Libelle).ThenBy(r => r.NumeroOrdre).ToList();
            }
        }

        /// <summary>
        /// Liste des types de liaison
        /// </summary>
        public List<RefSousTypeOuvrage> TypeLiaisons
        {
            get
            {
                return ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == RefSousTypeOuvrageGroupEnum.TypeLiaison.GetStringValue())
                    .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle).ToList();
            }
        }

        /// <summary>
        /// Liste des libellés de point commun existant
        /// </summary>
        public List<string> ListPointCommun
        {
            get { return _listPointCommun; }
            set
            {
                _listPointCommun = value;
                RaisePropertyChanged(() => ListPointCommun);
            }
        }

        /// <summary>
        /// Liste des liaison qui ont le même libellé de point commun
        /// </summary>
        public ObservableCollection<LiaisonCommunes> LiaisonsCommunes
        {
            get { return _liaisonsCommunes; }
            set
            {
                _liaisonsCommunes = value;
                RaisePropertyChanged(() => this.LiaisonsCommunes);
                RaisePropertyChanged(() => this.NbLiaisonsCommunes);
            }
        }

        /// <summary>
        /// Nombre de liaison communes
        /// </summary>
        public int NbLiaisonsCommunes
        {
            get
            {
                if (_liaisonsCommunes != null)
                    return _liaisonsCommunes.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Liste des soutirages que l'on peut associé
        /// </summary>
        public ObservableCollection<EqSoutirage> Soutirages
        {
            get { return _soutirages; }
            set
            {
                _soutirages = value;
                RaisePropertyChanged(() => this.Soutirages);
            }
        }

        /// <summary>
        /// Listde des drainages que l'on peut associé
        /// </summary>
        public ObservableCollection<EqDrainage> Drainages
        {
            get { return _drainages; }
            set
            {
                _drainages = value;
                RaisePropertyChanged(() => this.Drainages);
            }
        }

        /// <summary>
        /// Soutirage sélectionné à ajouter dans la liste des soutirages associés
        /// </summary>
        public EqSoutirage SoutirageSelected { get; set; }

        /// <summary>
        /// Drainage sélectionné à ajouter dans la liste des drainages associés
        /// </summary>
        public EqDrainage DrainageSelected { get; set; }

        #endregion Public Properties

        #region Commands

        /// <summary>
        /// Déclaration de l'objet de command de suppression d'un soutirage associé
        /// </summary>
        public IActionCommand RemoveSoutirageCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command d'ajout d'un soutirage associé
        /// </summary>
        public IActionCommand AddSoutirageCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression d'un drainage associé
        /// </summary>
        public IActionCommand RemoveDrainageCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command d'ajout d'un drainage associé
        /// </summary>
        public IActionCommand AddDrainageCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de réintégration de l'utilisateur
        /// </summary>
        public IActionCommand ManageSousTypeOuvrageCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de navigation vers une liaison
        /// </summary>
        public IActionCommand NavigateLiaisonCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de navigation vers un soutirage
        /// </summary>
        public IActionCommand NavigateSoutirageCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de navigation vers un drainage
        /// </summary>
        public IActionCommand NavigateDrainageCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de navigation vers une portion
        /// </summary>
        public IActionCommand NavigatePortionCommand { get; private set; }

        #endregion Commandes

        #region Services

        /// <summary>
        /// Service utilisé pour gérer les entités de type RefSousTypeOuvrage
        /// </summary>
        [Import]
        public IEntityService<RefSousTypeOuvrage> ServiceRefSousTypeOuvrage { get; set; }

        /// <summary>
        /// Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;

        #endregion Services

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Equipement_LEViewModel()
            : base()
        {
            RemoveSoutirageCommand = new ActionCommand<object>(
                obj => RemoveSoutirage(obj), obj => true);
            NavigateLiaisonCommand = new ActionCommand<object>(
                obj => NavigateToLiaison(obj), obj => true);
            NavigatePortionCommand = new ActionCommand<object>(
                obj => NavigateToPortion(obj), obj => true);
            NavigateSoutirageCommand = new ActionCommand<object>(
                obj => NavigateToSoutirage(obj), obj => true);
            NavigateDrainageCommand = new ActionCommand<object>(
                obj => NavigateToDrainage(obj), obj => true);
            AddSoutirageCommand = new ActionCommand<object>(
                obj => AddSoutirage(), obj => true);
            RemoveDrainageCommand = new ActionCommand<object>(
                obj => RemoveDrainage(obj), obj => true);
            AddDrainageCommand = new ActionCommand<object>(
                obj => AddDrainage(), obj => true);
            ManageSousTypeOuvrageCommand = new ActionCommand<object>(
                obj => ManageSousTypeOuvrage(), obj => GetCanGestionNomsTiers());

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(r => r.Key == "IsTopContainerLoaded"))
                {
                    EventAggregator.Publish("CustomTopContainer".AsViewNavigationArgs().AddNamedParameter("HideContainer", false));
                    EventAggregator.Publish("TypeEquipement".AsViewNavigationArgs().AddNamedParameter("IsTopContainerLoaded", true));
                }
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.EquipementExpanderTitle));
                    EventAggregator.Publish("Equipement_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true).AddNamedParameter(Constants.SPECIFIC_VIEWMODEL_NAME, "Equipement_LE"));
                }
                LiaisonsCommunes = null;
            };

            this.OnDetailLoaded += (o, e) =>
            {
                Initialisation();
            };

            this.OnAddedEntity += (o, e) =>
            {
                Initialisation();
                ((EqEquipementService)this.service).GetListPointCommun(ListPointCommunLoaded);
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.TypeNomTiers);
                RaisePropertyChanged(() => this.TypeLiaisons);
                ((EqEquipementService)this.service).GetListPointCommun(ListPointCommunLoaded);
                ((EqEquipementService)this.service).GetListSoutirageExt(ListSoutirageLoaded);
                ((EqEquipementService)this.service).GetListDrainageExt(ListDrainageLoaded);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                ((EqEquipementService)this.service).GetListPointCommun(ListPointCommunLoaded);
                ManageSousTypeOuvrageCommand.RaiseCanExecuteChanged();
            };

            this.OnDeleteSuccess += (o, e) =>
            {
                ((EqEquipementService)this.service).GetListPointCommun(ListPointCommunLoaded);
            };

            this.OnViewModeChanged += (o, e) =>
            {
                Initialisation();
            };

            this.OnCanceled += (o, e) =>
            {
                ManageSousTypeOuvrageCommand.RaiseCanExecuteChanged();
            };
        }

        #endregion Constructor

        #region Override Functions

        /// <summary>
        /// Surcharge du chargement de l'entité
        /// </summary>
        public override void LoadDetailEntity()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IsBusy = true;
            ((EqEquipementService)service).GetEntityByCle<EqLiaisonExterne>(SelectedId.Value, (error) => DetailEntityLoaded(error));
        }

        /// <summary>
        /// On override l'ajout d'un équipement
        /// </summary>
        protected override void Add()
        {
            this.AddEquipement<EqLiaisonExterne>();
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
        /// Surcharge de la méthode save pour mettre a jour les valeurs conditionnées
        /// </summary>
        protected override void Save(bool forceSave, bool withHisto)
        {
            if (SelectedEntity != null && SelectedEntity is EqLiaisonExterne)
            {
                if (!((EqLiaisonExterne)SelectedEntity).PresenceTelemesure)
                {
                    ((EqLiaisonExterne)SelectedEntity).DateMiseEnServiceTelemesure = null;
                }
            }
            base.Save(forceSave, withHisto);
        }

        protected override void Delete()
        {
            base.Delete();
        }

        #endregion Override Functions

        #region Private Functions

        /// <summary>
        /// Permet de naviguer vers une liaison
        /// </summary>
        /// <param name="cleEquipement"></param>
        public void NavigateToLiaison(object obj)
        {
            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.Equipement.GetStringValue(),
                   ((LiaisonCommunes)obj).TypeEquipement.ToString(),
                   ((LiaisonCommunes)obj).CleEquipement.ToString()),
                   UriKind.Relative));
        }

        /// <summary>
        /// Permet de naviguer vers une liaison
        /// </summary>
        /// <param name="cleEquipement"></param>
        public void NavigateToPortion(object obj)
        {

            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.PortionIntegrite.GetStringValue(),
                   ((LiaisonCommunes)obj).ClePortion.ToString()),
                   UriKind.Relative));
        }
        /// <summary>
        /// Permet de naviguer vers un drainage
        /// </summary>
        /// <param name="cleEquipement"></param>
        public void NavigateToDrainage(object obj)
        {
            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.Equipement.GetStringValue(),
                   FiltreNavigation.DR.GetStringValue(),
                   ((EqDrainage)obj).CleEquipement.ToString()),
                   UriKind.Relative));
        }

        /// <summary>
        /// Permet de naviguer vers un soutirage
        /// </summary>
        /// <param name="cleEquipement"></param>
        public void NavigateToSoutirage(object obj)
        {
            NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.Equipement.GetStringValue(),
                   FiltreNavigation.SO.GetStringValue(),
                   ((EqSoutirage)obj).CleEquipement.ToString()),
                   UriKind.Relative));
        }

        /// <summary>
        /// Initialisation des propriétés // Abonnement à certains évènements
        /// </summary>
        public void Initialisation()
        {
            if (SelectedEntity != null)
            {
                string libPointCom = string.Empty;

                SelectedEntity.PropertyChanged += (oo, ee) =>
                {
                    if (ee.PropertyName == "LibellePointCommun")
                    {
                        libPointCom = ((EqLiaisonExterne)this.SelectedEntity).LibellePointCommun;
                        if (this.ListPointCommun.Contains(libPointCom))
                            ((EqEquipementService)this.service).GetListLiaisonPointCommun(libPointCom, ListLiaisonPointCommunLoaded);
                        else
                            LiaisonsCommunes = null;
                    }
                };

                SoutirageSelected = null;
                DrainageSelected = null;

                libPointCom = ((EqLiaisonExterne)this.SelectedEntity).LibellePointCommun;
                if (this.ListPointCommun != null && this.ListPointCommun.Contains(libPointCom))
                    ((EqEquipementService)this.service).GetListLiaisonPointCommun(libPointCom, ListLiaisonPointCommunLoaded);
                else
                    LiaisonsCommunes = null;

                RefreshListDrainages();
                RefreshListSoutirages();
                ManageSousTypeOuvrageCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Suppression d'un soutirage associé
        /// </summary>
        private void RemoveSoutirage(object obj)
        {
            EqSoutirageLiaisonsext entity = ((EqLiaisonExterne)this.SelectedEntity).EqSoutirageLiaisonsext.FirstOrDefault(e => e.CleSoutirageLext == int.Parse(obj.ToString()));
            if (entity != null)
            {
                ((EqLiaisonExterne)this.SelectedEntity).EqSoutirageLiaisonsext.Remove(entity);
                ((EqSoutirageLiaisonsextService)ServiceSoutirageLiaisonsext).Delete(entity);
                this.Soutirages.Add(entity.EqSoutirage);
                this.Soutirages.OrderBy(e => e.Libelle);
            }
            RaisePropertyChanged(() => this.Soutirages);
        }

        /// <summary>
        /// Ajout d'un soutirage
        /// </summary>
        private void AddSoutirage()
        {
            if (this.SoutirageSelected != null)
            {
                ((EqLiaisonExterne)this.SelectedEntity).EqSoutirageLiaisonsext.Add(new EqSoutirageLiaisonsext()
                {
                    CleSoutirage = this.SoutirageSelected.CleEquipement,
                    CleLiaisonExt = this.SelectedEntity.CleEquipement
                });
                RefreshListSoutirages();
            }
        }

        /// <summary>
        /// Suppression d'un drainage associé
        /// </summary>
        private void RemoveDrainage(object obj)
        {
            EqDrainageLiaisonsext entity = ((EqLiaisonExterne)this.SelectedEntity).EqDrainageLiaisonsext.FirstOrDefault(e => e.CleDrainageLext == int.Parse(obj.ToString()));
            if (entity != null)
            {
                ((EqLiaisonExterne)this.SelectedEntity).EqDrainageLiaisonsext.Remove(entity);
                ((EqDrainageLiaisonsextService)ServiceDrainageLiaisonsext).Delete(entity);
                this.Drainages.Add(entity.EqDrainage);
                this.Drainages.OrderBy(e => e.Libelle);
            }
            RaisePropertyChanged(() => this.Drainages);
        }

        /// <summary>
        /// Ajout d'un drainage
        /// </summary>
        private void AddDrainage()
        {
            if (this.DrainageSelected != null)
            {
                ((EqLiaisonExterne)this.SelectedEntity).EqDrainageLiaisonsext.Add(new EqDrainageLiaisonsext()
                {
                    CleDrainage = this.DrainageSelected.CleEquipement,
                    CleLiaisonExt = this.SelectedEntity.CleEquipement
                });
                RefreshListDrainages();
            }
        }

        /// <summary>
        /// Affichage de la popup de sélection d'un secteur
        /// </summary>
        private void ManageSousTypeOuvrage()
        {
            ChildWindow.Title = Resource.RefSousTypeOuvrage_NomTiers;
            ChildWindow.Closed += (o, e) =>
            {
                //MAJ des type de redresseurs
                RaisePropertyChanged(() => this.TypeNomTiers);
                RaisePropertyChanged(() => this.SelectedEntity);
            };
            ChildWindow.Show();
            EventAggregator.Publish("RefSousTypeOuvrage".AsViewNavigationArgs()
                .AddNamedParameter("RefSousTypeOuvrageGroupEnum", RefSousTypeOuvrageGroupEnum.TypeNomTiers.GetStringValue())
                .AddNamedParameter("DisplayNumOrdre", false));
        }

        /// <summary>
        /// La liste des libellés points communs 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="pointCommun"></param>
        private void ListPointCommunLoaded(Exception error, List<string> libellespointCommun)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                this.ListPointCommun = libellespointCommun;
            }
        }

        /// <summary>
        /// Retourne la liste des liaison qui ont le même libellé de point commun
        /// </summary>
        /// <param name="error"></param>
        /// <param name="liaisonsCommunes"></param>
        private void ListLiaisonPointCommunLoaded(Exception error, ObservableCollection<LiaisonCommunes> liaisonsCommunes)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else if (liaisonsCommunes.Any() && this.SelectedEntity != null)
            {
                this.LiaisonsCommunes = new ObservableCollection<LiaisonCommunes>(liaisonsCommunes.Where(l => l.CleEquipement != this.SelectedEntity.CleEquipement));
            }
            else
            {
                this.LiaisonsCommunes = new ObservableCollection<LiaisonCommunes>();
            }
        }

        /// <summary>
        /// Liste des soutirages que l'on peut associer avec la liaison externe
        /// </summary>
        /// <param name="error"></param>
        /// <param name="soutirages"></param>
        private void ListSoutirageLoaded(Exception error, ObservableCollection<EqSoutirage> soutirages)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                this.Soutirages = soutirages;
            }
        }

        /// <summary>
        /// Liste des drainages que l'on peut associer avec la liaison externe
        /// </summary>
        /// <param name="error"></param>
        /// <param name="soutirages"></param>
        private void ListDrainageLoaded(Exception error, ObservableCollection<EqDrainage> drainages)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                this.Drainages = drainages;
            }
        }

        /// <summary>
        /// Met à jour la liste des soutirages en fonction des soutirages associés
        /// </summary>
        private void RefreshListSoutirages()
        {
            foreach (EqSoutirageLiaisonsext soutirageLiaisonsext in ((EqLiaisonExterne)this.SelectedEntity).EqSoutirageLiaisonsext)
            {
                this.Soutirages.Remove(soutirageLiaisonsext.EqSoutirage);
            }
            RaisePropertyChanged(() => this.Soutirages);
            this.SoutirageSelected = null;
            RaisePropertyChanged(() => this.SoutirageSelected);
        }

        /// <summary>
        /// Met à jour la liste des drainages en fonction des drainages associés
        /// </summary>
        private void RefreshListDrainages()
        {
            foreach (EqDrainageLiaisonsext drainageLiaisonsext in ((EqLiaisonExterne)this.SelectedEntity).EqDrainageLiaisonsext)
            {
                this.Drainages.Remove(drainageLiaisonsext.EqDrainage);
            }
            RaisePropertyChanged(() => this.Drainages);
            this.DrainageSelected = null;
            RaisePropertyChanged(() => this.DrainageSelected);
        }

        #endregion Private Functions

        #region Autorisations

        public bool GetCanGestionNomsTiers()
        {
            if (CurrentUser != null && IsEditMode)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GESTION_TIERS);
                return role != null && role.RefUsrPortee != null && role.RefUsrPortee.GetPorteesEnum() != RefUsrPortee.ListPorteesEnum.Interdite;
            }
            return false;
        }

        #endregion
    }
}
