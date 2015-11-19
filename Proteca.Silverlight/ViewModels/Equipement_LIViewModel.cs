using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Jounce.Core;
using Jounce.Core.Application;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;
using System;
using Proteca.Silverlight.Resources;
using System.Collections.ObjectModel;
using System.Windows;
using System.ServiceModel.DomainServices.Client;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums.NavigationEnums;
using System.ComponentModel;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Equipement_LI entity
    /// </summary>
    [ExportAsViewModel("Equipement_LI")]
    public class Equipement_LIViewModel : EquipementViewModel
    {
        #region Private Members

        /// <summary>
        /// Déclaration de la propriété ListPointCommun
        /// </summary>
        private List<string> _listPointCommun;

        /// <summary>
        /// Déclaration de la propriété LiaisonsCommunes
        /// </summary>
        public ObservableCollection<LiaisonCommunes> _liaisonsCommunes;

        #endregion Private Members

        #region Public Properties

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
            get
            {
                return _liaisonsCommunes;
            }
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
        /// Indique si le libelle est au format inter EE
        /// </summary>
        public bool IsLibelleInterEE
        {
            get
            {
                EqLiaisonInterne li = this.SelectedEntity as EqLiaisonInterne;

                if (li != null && !string.IsNullOrWhiteSpace(li.LibellePrefix))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Valuer de la liaison inter EE avant modification
        /// </summary>
        public bool LiaisonInterEeBeforeChange { get; set; }

        #endregion Public Properties

        #region Commands

        /// <summary>
        /// Déclaration de l'objet de command de navigation vers une liaison
        /// </summary>
        public IActionCommand NavigateLiaisonCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de navigation vers une portion
        /// </summary>
        public IActionCommand NavigatePortionCommand { get; private set; }

        #endregion

        #region Services

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<RefSousTypeOuvrage> ServiceRefSousTypeOuvrage { get; set; }

        #endregion Services

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Equipement_LIViewModel()
            : base()
        {
            NavigateLiaisonCommand = new ActionCommand<object>(
                obj => NavigateToLiaison(obj), obj => true);

            NavigatePortionCommand = new ActionCommand<object>(
                obj => NavigateToPortion(obj), obj => true);

            this.OnSaveSuccess += (o, e) =>
            {
                //Si on a ajouté une LIEE, on doit ajouter le lien entre les LIEE
                this.RaisePropertyChanged(() => this.SelectedEntity);
            };

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
                    EventAggregator.Publish("Equipement_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true).AddNamedParameter(Constants.SPECIFIC_VIEWMODEL_NAME, "Equipement_LI"));
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
                RaisePropertyChanged(() => this.TypeLiaisons);
                ((EqEquipementService)this.service).GetListPointCommun(ListPointCommunLoaded);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                LiaisonInterEeBeforeChange = ((EqLiaisonInterne)this.SelectedEntity).LiaisonInterEe;
                RaisePropertyChanged(() => this.IsLibelleInterEE);
                ((EqEquipementService)this.service).GetListPointCommun(ListPointCommunLoaded);
            };

            this.OnDeleteSuccess += (o, e) =>
            {
                ((EqEquipementService)this.service).GetListPointCommun(ListPointCommunLoaded);
            };
        }

        #endregion Constructor

        #region Override Functions

        /// <summary>
        /// surcharge la Réintégration de l'équipement
        /// </summary>
        protected override void ReintegrateEquipement()
        {
            if (((EqLiaisonInterne)this.SelectedEntity).Pp2 != null && ((EqLiaisonInterne)this.SelectedEntity).Pp2.Supprime)
            {
                ((EqLiaisonInterne)this.SelectedEntity).ClePp2 = 0;
                this.NotifyError = true;
                this.IsEditMode = true;
            }

            base.ReintegrateEquipement();
        }

        /// <summary>
        /// Surcharge du chargement de l'entité
        /// </summary>
        public override void LoadDetailEntity()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IsBusy = true;
            ((EqEquipementService)service).GetEntityByCle<EqLiaisonInterne>(SelectedId.Value, (error) => DetailEntityLoaded(error));
        }

        /// <summary>
        /// On override l'ajout d'un équipement
        /// </summary>
        protected override void Add()
        {
            this.AddEquipement<EqLiaisonInterne>();
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
            if (SelectedEntity != null && SelectedEntity is EqLiaisonInterne)
            {
                if (!((EqLiaisonInterne)SelectedEntity).PresenceTelemesure)
                {
                    ((EqLiaisonInterne)SelectedEntity).DateMiseEnServiceTelemesure = null;
                }
            }

            //gestion de la liaison inter EE
            bool liaisonInterEe = ((EqLiaisonInterne)this.SelectedEntity).LiaisonInterEe;
            if (liaisonInterEe != LiaisonInterEeBeforeChange)
            {
                if (liaisonInterEe)
                {
                    //on ajoute son doublon
                    EqLiaisonInterne principale = this.SelectedEntity as EqLiaisonInterne;
                    EqLiaisonInterne liaisonInt = new EqLiaisonInterne();
                    if (principale.EqLiaisonInterne2 != null)
                    {
                        liaisonInt = principale.EqLiaisonInterne2;
                    }
                    liaisonInt.ClePp = principale.ClePp2;
                    liaisonInt.ClePp2 = principale.ClePp;
                    liaisonInt.CleTypeEq = principale.CleTypeEq;
                    liaisonInt.CleTypeLiaison = principale.CleTypeLiaison;
                    liaisonInt.CleUtilisateur = CurrentUser.CleUtilisateur;
                    liaisonInt.Commentaire = principale.Commentaire;
                    liaisonInt.DateMajCommentaire = null;
                    liaisonInt.DateMajEquipement = DateTime.Now;
                    liaisonInt.DateMiseEnService = principale.DateMiseEnService;
                    liaisonInt.DateMiseEnServiceTelemesure = principale.DateMiseEnServiceTelemesure;
                    liaisonInt.LiaisonInterEe = true;
                    liaisonInt.LibellePointCommun = principale.LibellePointCommun;
                    liaisonInt.PresenceTelemesure = principale.PresenceTelemesure;
                    if (principale.EqLiaisonInterne2 == null)
                    {
                        liaisonInt.Libelle = string.Format("LIEE - {0} - 2", this.SelectedEntity.Libelle);
                        this.SelectedEntity.Libelle = string.Format("LIEE - {0} - 1", this.SelectedEntity.Libelle);
                        //on créer le lien entre les deux liaison
                        ((EqLiaisonInterne)this.SelectedEntity).EqLiaisonInterne2 = liaisonInt;

                    }

                    if (!this.SelectedEntity.IsNew)
                    {
                        liaisonInt.EqLiaisonInterne2 = (EqLiaisonInterne)this.SelectedEntity;
                    }

                    base.Save(forceSave, withHisto);
                }
                else
                {
                    EqLiaisonInterne li = (EqLiaisonInterne)this.SelectedEntity;
                    //On renomme la liaison actuelle
                    this.SelectedEntity.Libelle = li.LibellePrincipaleRemove;

                    //on supprime la liaison associé
                    ((EqEquipementService)this.service).CanPhysicalDeleteByEquipement(li.CleLiaisonInterEe.Value,
                        (error, canPhysicalDelete) =>
                        {
                            if (canPhysicalDelete)
                            {
                                service.Delete(li.EqLiaisonInterne2);
                            }
                            else
                            {
                                li.EqLiaisonInterne2.CleLiaisonInterEe = null;
                                li.EqLiaisonInterne2.LiaisonInterEe = false;
                                li.EqLiaisonInterne2.Supprime = true;
                                li.EqLiaisonInterne2.DateMajEquipement = DateTime.Now;

                                foreach (EqRaccordIsolant ri in li.EqLiaisonInterne2.EqRaccordIsolant1)
                                {
                                    ri.CleLiaison = null;
                                }

                                this.LogOuvrage("S", li.EqLiaisonInterne2);
                            }

                            //On supprime le lien vers la seconde liaison interne
                            li.CleLiaisonInterEe = null;
                            li.LiaisonInterEe = false;

                            base.Save(forceSave, withHisto);
                        }
                    );
                }


            }
            else
            {
                base.Save(forceSave, withHisto);
            }
        }

        /// <summary>
        /// surcharge de la méthode pour le rafraichissement de la PP et Portion secondaire
        /// </summary>
        protected override void refreshPortions()
        {
            if (this.SelectedEntity != null)
            {
                this.SelectedEntity.Portion2Selected = null;
            }

            if (this.SelectedEntity != null && ((EqLiaisonInterne)this.SelectedEntity).Pp2 != null)
            {
                this.SelectedEntity.Portion2Selected = this.ListPortions.FirstOrDefault(p => p.ClePortion == ((EqLiaisonInterne)this.SelectedEntity).Pp2.ClePortion);
            }

            RaisePropertyChanged(() => this.SelectedEntity);

            base.refreshPortions();
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
        /// Initialisation des propriétés // Abonnement à certains évènements
        /// </summary>
        public void Initialisation()
        {
            if (SelectedEntity != null)
            {

                string libPointCom = string.Empty;
                SelectedEntity.PropertyChanged -= SelectedEntity_PropertyChanged;
                SelectedEntity.PropertyChanged += SelectedEntity_PropertyChanged;
                //SelectedEntity.PropertyChanged += (oo, ee) =>
                //{
                //    if (ee.PropertyName == "LibellePointCommun")
                //    {
                //        libPointCom = ((EqLiaisonInterne)this.SelectedEntity).LibellePointCommun;
                //        if (this.ListPointCommun.Contains(libPointCom))
                //            ((EqEquipementService)this.service).GetListLiaisonPointCommun(libPointCom, ListLiaisonPointCommunLoaded);
                //        else
                //            LiaisonsCommunes = null;
                //    }
                //    EqLiaisonInterne li = (EqLiaisonInterne)this.SelectedEntity;
                //    if (ee.PropertyName == "LiaisonInterEe" && !li.LiaisonInterEe &&
                //        li.CleLiaisonInterEe.HasValue && li.EqLiaisonInterne2.LiaisonInterEe)
                //    {
                //        MessageBox.Show(Resource.DeleteLiaisonInterEE, string.Empty, MessageBoxButton.OK);
                //    }
                //};

                LiaisonInterEeBeforeChange = ((EqLiaisonInterne)this.SelectedEntity).LiaisonInterEe;
                libPointCom = ((EqLiaisonInterne)this.SelectedEntity).LibellePointCommun;

                if (this.ListPointCommun != null && this.ListPointCommun.Contains(libPointCom))
                    ((EqEquipementService)this.service).GetListLiaisonPointCommun(libPointCom, ListLiaisonPointCommunLoaded);
                else
                    LiaisonsCommunes = null;

                RaisePropertyChanged(() => this.IsLibelleInterEE);
            }
        }

        void SelectedEntity_PropertyChanged(object oo, PropertyChangedEventArgs ee)
        {
            string libPointCom = string.Empty;
                if (ee.PropertyName == "LibellePointCommun")
                {
                    libPointCom = ((EqLiaisonInterne)this.SelectedEntity).LibellePointCommun;
                    if (this.ListPointCommun.Contains(libPointCom))
                        ((EqEquipementService)this.service).GetListLiaisonPointCommun(libPointCom, ListLiaisonPointCommunLoaded);
                    else
                        LiaisonsCommunes = null;
                }
                EqLiaisonInterne li = (EqLiaisonInterne)this.SelectedEntity;
                if (ee.PropertyName == "LiaisonInterEe" && !li.LiaisonInterEe &&
                    li.CleLiaisonInterEe.HasValue && li.EqLiaisonInterne2.LiaisonInterEe)
                {
                    MessageBox.Show(Resource.DeleteLiaisonInterEE, string.Empty, MessageBoxButton.OK);
                }
        
        }

        protected override void CanPhysicalDeleteByEquipement()
        {
            EqLiaisonInterne li = (EqLiaisonInterne)this.SelectedEntity;
            if (li.LiaisonInterEe)
            {
                MessageBox.Show(Resource.EqLiaisonInterne_SuppressionImpossible, string.Empty, MessageBoxButton.OK);
            }
            else
            {
                base.CanPhysicalDeleteByEquipement();
            }
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
            else
            {
                this.LiaisonsCommunes = new ObservableCollection<LiaisonCommunes>(liaisonsCommunes.Where(l => l.CleEquipement != this.SelectedEntity.CleEquipement));
            }
        }

        #endregion Private Functions
    }
}
