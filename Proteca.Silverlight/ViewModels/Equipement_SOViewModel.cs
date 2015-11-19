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
using Proteca.Silverlight.Views.Windows;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using Proteca.Silverlight.Resources;
using System;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Equipement_SO entity
    /// </summary>
    [ExportAsViewModel("Equipement_SO")]
    public class Equipement_SOViewModel : EquipementViewModel
    {
        #region Public Properties
        
        /// <summary>
        /// Liste des types de redresseurs
        /// </summary>
        public List<RefSousTypeOuvrage> TypeRedresseurs
        {
            get
            {
                return ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == RefSousTypeOuvrageGroupEnum.TypeRedresseur.GetStringValue())
                    .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle).ToList();
            }
        }

        /// <summary>
        /// Liste des type de deversoir
        /// </summary>
        public List<RefSousTypeOuvrage> TypeDeversoirs
        {
            get
            {
                return ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == RefSousTypeOuvrageGroupEnum.TypeDeversoirSoutirage.GetStringValue())
                    .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle).ToList();
            }
        }

        #region HasFormatValidationError

        /// <summary>
        /// Gestion des erreurs de format pour la propriété MasseAuMetreLineaire
        /// </summary>
        private bool _masseAuMetreLineaireFormatValidationError = false;
        public bool HasMasseAuMetreLineaireFormatValidationError
        {
            get
            {
                return _masseAuMetreLineaireFormatValidationError;
            }
            set
            {
                _masseAuMetreLineaireFormatValidationError = value;
                HasFormatValidationError = HasCoordDebDeversoirLatFormatValidationError
                                        || HasCoordDebDeversoirLongFormatValidationError
                                        || HasCoordFinDeversoirLatFormatValidationError
                                        || HasCoordFinDeversoirLongFormatValidationError
                                        || HasIntensiteReglageFormatValidationError
                                        || HasLongueurDeversoirFormatValidationError
                                        || HasMasseAuMetreLineaireFormatValidationError;
                RaisePropertyChanged(() => this.HasMasseAuMetreLineaireFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété LongueurDeversoir
        /// </summary>
        private bool _longueurDeversoirFormatValidationError = false;
        public bool HasLongueurDeversoirFormatValidationError
        {
            get
            {
                return _longueurDeversoirFormatValidationError;
            }
            set
            {
                _longueurDeversoirFormatValidationError = value;
                HasFormatValidationError = HasCoordDebDeversoirLatFormatValidationError
                                        || HasCoordDebDeversoirLongFormatValidationError
                                        || HasCoordFinDeversoirLatFormatValidationError
                                        || HasCoordFinDeversoirLongFormatValidationError
                                        || HasIntensiteReglageFormatValidationError
                                        || HasLongueurDeversoirFormatValidationError
                                        || HasMasseAuMetreLineaireFormatValidationError;
                RaisePropertyChanged(() => this.HasLongueurDeversoirFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété IntensiteReglage
        /// </summary>
        private bool _intensiteReglageFormatValidationError = false;
        public bool HasIntensiteReglageFormatValidationError
        {
            get
            {
                return _intensiteReglageFormatValidationError;
            }
            set
            {
                _intensiteReglageFormatValidationError = value;
                HasFormatValidationError = HasCoordDebDeversoirLatFormatValidationError
                                        || HasCoordDebDeversoirLongFormatValidationError
                                        || HasCoordFinDeversoirLatFormatValidationError
                                        || HasCoordFinDeversoirLongFormatValidationError
                                        || HasIntensiteReglageFormatValidationError
                                        || HasLongueurDeversoirFormatValidationError
                                        || HasMasseAuMetreLineaireFormatValidationError;
                RaisePropertyChanged(() => this.HasIntensiteReglageFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété CoordFinDeversoirLong
        /// </summary>
        private bool _coordFinDeversoirLongFormatValidationError = false;
        public bool HasCoordFinDeversoirLongFormatValidationError
        {
            get
            {
                return _coordFinDeversoirLongFormatValidationError;
            }
            set
            {
                _coordFinDeversoirLongFormatValidationError = value;
                HasFormatValidationError = HasCoordDebDeversoirLatFormatValidationError
                                        || HasCoordDebDeversoirLongFormatValidationError
                                        || HasCoordFinDeversoirLatFormatValidationError
                                        || HasCoordFinDeversoirLongFormatValidationError
                                        || HasIntensiteReglageFormatValidationError
                                        || HasLongueurDeversoirFormatValidationError
                                        || HasMasseAuMetreLineaireFormatValidationError;
                RaisePropertyChanged(() => this.HasCoordFinDeversoirLongFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété CoordFinDeversoirLat
        /// </summary>
        private bool _coordFinDeversoirLatFormatValidationError = false;
        public bool HasCoordFinDeversoirLatFormatValidationError
        {
            get
            {
                return _coordFinDeversoirLatFormatValidationError;
            }
            set
            {
                _coordFinDeversoirLatFormatValidationError = value;
                HasFormatValidationError = HasCoordDebDeversoirLatFormatValidationError
                                        || HasCoordDebDeversoirLongFormatValidationError
                                        || HasCoordFinDeversoirLatFormatValidationError
                                        || HasCoordFinDeversoirLongFormatValidationError
                                        || HasIntensiteReglageFormatValidationError
                                        || HasLongueurDeversoirFormatValidationError
                                        || HasMasseAuMetreLineaireFormatValidationError;
                RaisePropertyChanged(() => this.HasCoordFinDeversoirLatFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété CoordDebDeversoirLong
        /// </summary>
        private bool _coordDebDeversoirLongFormatValidationError = false;
        public bool HasCoordDebDeversoirLongFormatValidationError
        {
            get
            {
                return _coordDebDeversoirLongFormatValidationError;
            }
            set
            {
                _coordDebDeversoirLongFormatValidationError = value;
                HasFormatValidationError = HasCoordDebDeversoirLatFormatValidationError
                                        || HasCoordDebDeversoirLongFormatValidationError
                                        || HasCoordFinDeversoirLatFormatValidationError
                                        || HasCoordFinDeversoirLongFormatValidationError
                                        || HasIntensiteReglageFormatValidationError
                                        || HasLongueurDeversoirFormatValidationError
                                        || HasMasseAuMetreLineaireFormatValidationError;
                RaisePropertyChanged(() => this.HasCoordDebDeversoirLongFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété CoordDebDeversoirLat
        /// </summary>
        private bool _coordDebDeversoirLatFormatValidationError = false;
        public bool HasCoordDebDeversoirLatFormatValidationError
        {
            get
            {
                return _coordDebDeversoirLatFormatValidationError;
            }
            set
            {
                _coordDebDeversoirLatFormatValidationError = value;
                HasFormatValidationError = HasCoordDebDeversoirLatFormatValidationError
                                        || HasCoordDebDeversoirLongFormatValidationError
                                        || HasCoordFinDeversoirLatFormatValidationError
                                        || HasCoordFinDeversoirLongFormatValidationError
                                        || HasIntensiteReglageFormatValidationError
                                        || HasLongueurDeversoirFormatValidationError
                                        || HasMasseAuMetreLineaireFormatValidationError;
                RaisePropertyChanged(() => this.HasCoordDebDeversoirLatFormatValidationError);
            }
        }

        #endregion

        

        #endregion Public Properties

        #region Commande

        /// <summary>
        /// Déclaration de l'objet de command de réintégration de l'utilisateur
        /// </summary>
        public IActionCommand ManageSousTypeOuvrageCommand { get; private set; }

        #endregion Commande

        #region Services

        /// <summary>
        /// Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<RefSousTypeOuvrage> ServiceRefSousTypeOuvrage { get; set; }

        #endregion Services

        #region Constructor

        /// <summary>
        /// Constructeur.
        /// </summary>
        public Equipement_SOViewModel(): base()
        {
            ManageSousTypeOuvrageCommand = new ActionCommand<object>(
                obj => ManageSousTypeOuvrage(), obj => true);
            
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
                    EventAggregator.Publish("Equipement_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true).AddNamedParameter(Constants.SPECIFIC_VIEWMODEL_NAME, "Equipement_SO"));
                }
            };

            this.OnAddedEntity += (o, e) =>
            {
                ((EqSoutirage)this.SelectedEntity).DateControle = DateTime.Now;
                ((EqSoutirage)this.SelectedEntity).DateMiseEnServiceRedresseur = DateTime.Now;
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.TypeRedresseurs);
                RaisePropertyChanged(() => this.TypeDeversoirs);
            };
        }

        #endregion Constructor

        #region override Methods

        /// <summary>
        /// Surcharge du chargement de l'entité
        /// </summary>
        public override void LoadDetailEntity()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IsBusy = true;
            ((EqEquipementService)service).GetEntityByCle<EqSoutirage>(SelectedId.Value, (error) => DetailEntityLoaded(error));
        }

        /// <summary>
        /// On override l'ajout d'un équipement
        /// </summary>
        protected override void Add()
        {
            this.AddEquipement<EqSoutirage>();
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
            if (SelectedEntity != null && SelectedEntity is EqSoutirage)
            {
                if (!((EqSoutirage)SelectedEntity).PresenceTelemesure)
                {
                    ((EqSoutirage)SelectedEntity).DateMiseEnServiceTelemesure = null;
                }
            }
            base.Save(forceSave, withHisto);
        }

        #endregion override Methods

        #region Private Functions
        
        /// <summary>
        /// Affichage de la popup de sélection d'un secteur
        /// </summary>
        private void ManageSousTypeOuvrage()
        {
            ChildWindow.Title = Resource.RefSousTypeOuvrage_Redresseur;
            ChildWindow.Closed += (o, e) =>
            {
                //MAJ des type de redresseurs
                RaisePropertyChanged(() => this.TypeRedresseurs);
                RaisePropertyChanged(() => this.SelectedEntity);
            };
            ChildWindow.Show();
            EventAggregator.Publish("RefSousTypeOuvrage".AsViewNavigationArgs()
                .AddNamedParameter("RefSousTypeOuvrageGroupEnum", RefSousTypeOuvrageGroupEnum.TypeRedresseur.GetStringValue())
                .AddNamedParameter("DisplayNumOrdre", true));
        }

        #endregion Private Functions

    }
}
