using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Resources;
using System.Windows;
using Jounce.Core.Application;
using System.Reflection;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for InsInstrument entity
    /// </summary>
    [ExportAsViewModel("InsInstrument")]
    public class InsInstrumentViewModel : BaseProtecaEntityViewModel<InsInstrument>
    {
        #region Private Members

        /// <summary>
        /// Déclaration de la variable CanSearch
        /// </summary>
        private bool _canSearch;

        private List<InsInstrument> _trash;

        #endregion Private Members

        #region Private Properties

        /// <summary>
        /// Cle secteur enregistrer après la recherche pour attacher les nouveaux instruments
        /// </summary>
        private int? CleSecteurToAttach { get; set; }

        /// <summary>
        /// Cle agence enregistrer après la recherche pour attacher les nouveaux instruments
        /// </summary>
        private int? CleAgenceToAttach { get; set; }

        /// <summary>
        /// Cle region enregistrer après la recherche pour attacher les nouveaux instruments
        /// </summary>
        private int? CleRegionToAttach { get; set; }

        /// <summary>
        /// Indique si la dernière recherche incluait les instrument supprimés
        /// </summary>
        private bool InstrumentDeletedAfterLastSearch { get; set; }

        #endregion Private Properties

        #region Public Properties

        /// <summary>
        /// Liste des colonnes dont la visibilité doit être inersée pour l'export
        /// </summary>
        public ObservableCollection<string> ColumnsHiddenToExport
        {
            get
            {
                return new ObservableCollection<string> { Resource.Instrument_SupprimeNameHeader };
            }
        }

        public List<InsInstrument> Trash
        {
            get
            {
                if (_trash == null)
                {
                    _trash = new List<InsInstrument>();
                }
                return _trash;
            }
            set { _trash = value; }
        }

        /// <summary>
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return serviceRegion.Entities; }
        }

        /// <summary>
        /// Listes des instruments triees
        /// </summary>
        public ObservableCollection<InsInstrument> InstrumentTriees
        {
            get
            {
                ObservableCollection<InsInstrument> instr = null;
                if (this.Entities != null && this.Entities.Any())
                {
                    instr = new ObservableCollection<InsInstrument>(this.Entities.OrderBy(i => i.Libelle));
                }
                else
                {
                    instr = new ObservableCollection<InsInstrument>();
                }
                return instr;
            }
        }

        /// <summary>
        /// Indique si l'on réalise la recherche sur les instruments supprimés
        /// </summary>
        public bool InstrumentDeleted { get; set; }

        /// <summary>
        /// Indique si l'utilisateur peut réaliser la recherche sur les instruments
        /// </summary>
        public new bool CanSearch
        {
            get { return _canSearch; }
            set
            {
                _canSearch = value;
                if (this.FindInstrumentsCommand != null)
                {
                    this.FindInstrumentsCommand.RaiseCanExecuteChanged();
                }
                RaisePropertyChanged(() => this.CanSearch);
            }
        }

        public bool IsEditable
        {
            get 
            {   
                return this.CleSecteurToAttach.HasValue || this.CleAgenceToAttach.HasValue || this.CleRegionToAttach.HasValue;
            }
        }

        #endregion Public Properties

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public InsInstrumentViewModel()
            : base()
        {
            IsAutoNavigateToFirst = false;
            IsEditMode = false;

            this.OnRegionSelected += (o, e) =>
            {
                CanSearch = FiltreCleRegion.HasValue;
            };

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Recherche des Instruments de mesures"));
                    EventAggregator.Publish("InsInstrument_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };
            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.InstrumentTriees);
                RaisePropertyChanged(() => this.Regions);
            };

            this.OnCanceled += (o, e) =>
            {
                RaisePropertyChanged(() => this.InstrumentTriees);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                if (this.InstrumentDeletedAfterLastSearch)
                {
                    FindInstrumentDone(null);
                }
                else
                {
                    IsBusy = true;
                    ((InsInstrumentService)this.service).FindInstrumentsByCriterias(this.CleRegionToAttach.Value,
                    this.CleAgenceToAttach, this.CleSecteurToAttach, InstrumentDeletedAfterLastSearch, FindInstrumentDone);
                }
                this.Trash = null;
            };
        }

        #endregion Constuctor

        #region Command

        /// <summary>
        /// Déclaration de commande de changement de statut
        /// </summary>
        public IActionCommand ChangeStatusCommand { get; private set; }

        /// <summary>
        /// Déclaration de la commande de recherche des instruments
        /// </summary>
        public IActionCommand FindInstrumentsCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'événement d'une modification d'un item
        /// </summary>
        public IActionCommand SelectedCellChangedCommand { get; private set; }

        #endregion Command

        #region Services

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type Agence
        /// </summary>
        [Import]
        public IEntityService<GeoAgence> serviceAgence { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type Secteur
        /// </summary>
        [Import]
        public IEntityService<GeoSecteur> serviceSecteur { get; set; }

        #endregion Services

        #region Protected Functions

        /// <summary>
        /// Initialisation de la commande d'information d'edition
        /// </summary>
        protected override void InitializeVm()
        {
            base.InitializeVm();

            ChangeStatusCommand = new ActionCommand<object>(
                obj => ChangeStatusToDelete(obj), obj => true);

            FindInstrumentsCommand = new ActionCommand<object>(
                obj => FindInstruments(), obj => CanSearch);

            SelectedCellChangedCommand = new ActionCommand<object>(
                obj => RaisePropertyChanged(() => this.InstrumentTriees), obj => true);
        }

        /// <summary>
        /// Ajout d'un nouvel instrument
        /// </summary>
        protected override void Add()
        {
            base.Add();
            this.AddRelationGeo();
            this.SelectedEntity = null;
            RaisePropertyChanged(() => this.InstrumentTriees);
        }

        protected override void Save()
        {
            if (Trash.Any() || this.Entities.Any(i => i.IsNew && i.Supprime))
            {
                this.IsBusy = true;
                List<int> trashKeyList = Trash.Select(i => i.CleInstrument).ToList();
                (this.service as InsInstrumentService).GetInsInstrumentUtilisesByListInsInstrument(trashKeyList, GetInsInstrumentUtilisesByListInsInstrumentDone);
            }
            else
            {
                base.Save();
            }
        }

        #endregion Protected Functions

        #region Private Functions

        /// <summary>
        /// Change le status de l'instrument à supprimé
        /// </summary>
        /// <param name="id"></param>
        private void ChangeStatusToDelete(object id)
        {
            var result = MessageBoxResult.None;
            String ConfirmMsg = Resource.ResourceManager.GetString("InsIntrument_DeleteConfirmation");

            if (ConfirmMsg == null)
            {
                ConfirmMsg = Resource.BaseProtecaEntityViewModel_DeleteConfirmation;
            }

            int cleInstrument = int.Parse(id.ToString());
            InsInstrument instrument = this.Entities.FirstOrDefault(i => i.CleInstrument == cleInstrument);
            if (instrument != null)
            {
                //Si supprimé = true alors c'est qu'on reintegre l'instrument du coup pas d'affichage du message
                if (!instrument.Supprime)
                    result = MessageBox.Show(ConfirmMsg, "", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK || instrument.Supprime)
                {
                    instrument.Supprime = !instrument.Supprime;
                }

                if (instrument.Supprime && !instrument.IsNew)
                {
                    Trash.Add(instrument);
                }
            }
        }

        /// <summary>
        /// Réalise le lien entre l'instrument et la zone géographique 
        /// suivant les éléments sélectionnés dans l'expander
        /// </summary>
        private void AddRelationGeo()
        {
            if (this.CleSecteurToAttach.HasValue) // on réalise le lien avec le secteur sélectionné
            {
                this.SelectedEntity.CleRegion = null;
                this.SelectedEntity.CleAgence = null;
                this.SelectedEntity.CleSecteur = this.CleSecteurToAttach;
            }
            else if (this.CleAgenceToAttach.HasValue)// on réalise le lien avec l'agence sélectionnée
            {
                this.SelectedEntity.CleRegion = null;
                this.SelectedEntity.CleAgence = this.CleAgenceToAttach;
                this.SelectedEntity.CleSecteur = null;
            }
            else if (this.CleRegionToAttach.HasValue)// on réalise le lien avec la région sélectionnée
            {
                this.SelectedEntity.CleRegion = this.CleRegionToAttach;
                this.SelectedEntity.CleAgence = null;
                this.SelectedEntity.CleSecteur = null;
            }

            this.SelectedEntity.IsEditable = true;
        }

        /// <summary>
        /// Lance la recherche sur les instrument en fonction des critères sélectionnés
        /// </summary>
        private void FindInstruments()
        {
            if (this.FiltreCleRegion.HasValue)
            {
                base.saveGeoPreferences();

                IsBusy = true;
                ((InsInstrumentService)this.service).FindInstrumentsByCriterias(this.FiltreCleRegion.Value,
                    this.FiltreCleAgence, this.FiltreCleSecteur, InstrumentDeleted, FindInstrumentDone);

                //on enregistre les clés de la recherche pour pouvoir associer les nouveaux instruments sans utiliser les critères de recherche
                this.CleSecteurToAttach = this.FiltreCleSecteur;
                this.CleAgenceToAttach = this.FiltreCleAgence;
                this.CleRegionToAttach = this.FiltreCleRegion;
                this.RaisePropertyChanged(() => this.IsEditable);
                this.InstrumentDeletedAfterLastSearch = this.InstrumentDeleted;
            }
        }

        /// <summary>
        /// La recherche des instruments vient d'être exécutée
        /// </summary>
        /// <param name="ex"></param>
        private void FindInstrumentDone(Exception ex)
        {
            ManageInstrumentEditMode();

            //On initilaise un selectedEntity pour ne pas faire faire planter la sauvegarde
            if (this.Entities.Count > 0)
            {
                this.SelectedEntity = this.Entities.FirstOrDefault();
            }
            RaisePropertyChanged(() => this.InstrumentTriees);
            IsBusy = false;
        }

        /// <summary>
        /// On rend editable seulement les instrument ayant un secteur de rattachement
        /// </summary>
        private void ManageInstrumentEditMode()
        {
            if (this.Entities != null && this.Entities.Any())
            {
                if (this.FiltreCleSecteur.HasValue)
                {
                    foreach (InsInstrument instru in this.Entities)
                    {
                        instru.IsEditable = instru.CleSecteur.HasValue;
                    }
                }
                else if (this.FiltreCleAgence.HasValue)
                {
                    foreach (InsInstrument instru in this.Entities)
                    {
                        instru.IsEditable = instru.CleAgence.HasValue;
                    }
                }
                else
                {
                    foreach (InsInstrument instru in this.Entities)
                    {
                        instru.IsEditable = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gestion de la suppression physique des instruments non utilisés
        /// </summary>
        /// <param name="error"></param>
        /// <param name="utilisation"></param>
        private void GetInsInstrumentUtilisesByListInsInstrumentDone(Exception error, IEnumerable<int> utilisation)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(InsInstrument).Name));
            }
            else
            {
                if (utilisation != null)
                {
                    for (int i = Trash.Count - 1; i > -1; i--)
                    {
                        if (utilisation.Contains(Trash.ElementAt(i).CleInstrument))
                        {
                            this.service.Delete(Trash.ElementAt(i));
                        }
                    }
                }

                if (this.Entities.Any(i => i.IsNew && i.Supprime))
                {
                    List<InsInstrument> newAndSupprimes = this.Entities.Where(i => i.IsNew && i.Supprime).ToList();
                    for (int i = newAndSupprimes.Count - 1; i > -1; i--)
                    {
                        this.service.Delete(newAndSupprimes.ElementAt(i));
                    }
                }
                base.Save();
            }
        }

        #endregion Private Functions

        #region Autorisations
        protected override bool GetUserCanAdd()
        {
            return GetAutorisation();
        }
        /// <summary>
        /// Gestion des autorisations. L'utilisateur ne peut pas éditer s'il n'a pas une autorisation d'affectation différente d'"interdite"
        /// </summary>
        /// <returns></returns>
        protected override bool GetUserCanEdit()
        {
            return GetAutorisation();
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
        /// sur la sauvegarde d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanSave()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            bool result = true;
            if (this.CurrentUser != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_INS_NIV);
                result = role.RefUsrPortee.CodePortee != RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue();
            }
            return result;
        }

        #endregion Autorisations
    }
}
