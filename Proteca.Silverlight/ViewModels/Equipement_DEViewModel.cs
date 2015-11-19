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
using Proteca.Silverlight.Resources;
using System;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Equipement_DE entity
    /// </summary>
    [ExportAsViewModel("Equipement_DE")]
    public class Equipement_DEViewModel : EquipementViewModel
    {
        #region Public Properties

        /// <summary>
        /// Liste des type de prise de terre
        /// </summary>
        public List<RefSousTypeOuvrage> TypePrisesTerre
        {
            get
            {
                return ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == RefSousTypeOuvrageGroupEnum.TypePriseTerre.GetStringValue())
                    .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle).ToList();
            }
        }

        #region HasFormatValidationError

        /// <summary>
        /// Gestion des erreurs de format pour la propriété CoordDebPriseTerreLat
        /// </summary>
        private bool _coordDebPriseTerreLatFormatValidationError = false;
        public bool HasCoordDebPriseTerreLatFormatValidationError
        {
            get
            {
                return _coordDebPriseTerreLatFormatValidationError;
            }
            set
            {
                _coordDebPriseTerreLatFormatValidationError = value;
                HasFormatValidationError = HasCoordDebPriseTerreLatFormatValidationError
                                        || HasCoordDebPriseTerreLongFormatValidationError
                                        || HasCoordFinPriseTerreLatFormatValidationError
                                        || HasCoordFinPriseTerreLongFormatValidationError
                                        || HasResistanceInitPriseDeTerreFormatValidationError;
                RaisePropertyChanged(() => this.HasCoordDebPriseTerreLatFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété CoordDebPriseTerreLong
        /// </summary>
        private bool _coordDebPriseTerreLongFormatValidationError = false;
        public bool HasCoordDebPriseTerreLongFormatValidationError
        {
            get
            {
                return _coordDebPriseTerreLongFormatValidationError;
            }
            set
            {
                _coordDebPriseTerreLongFormatValidationError = value;
                HasFormatValidationError = HasCoordDebPriseTerreLatFormatValidationError
                                        || HasCoordDebPriseTerreLongFormatValidationError
                                        || HasCoordFinPriseTerreLatFormatValidationError
                                        || HasCoordFinPriseTerreLongFormatValidationError
                                        || HasResistanceInitPriseDeTerreFormatValidationError;
                RaisePropertyChanged(() => this.HasCoordDebPriseTerreLongFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété CoordFinPriseTerreLat
        /// </summary>
        private bool _coordFinPriseTerreLatFormatValidationError = false;
        public bool HasCoordFinPriseTerreLatFormatValidationError
        {
            get
            {
                return _coordFinPriseTerreLatFormatValidationError;
            }
            set
            {
                _coordFinPriseTerreLatFormatValidationError = value;
                HasFormatValidationError = HasCoordDebPriseTerreLatFormatValidationError
                                        || HasCoordDebPriseTerreLongFormatValidationError
                                        || HasCoordFinPriseTerreLatFormatValidationError
                                        || HasCoordFinPriseTerreLongFormatValidationError
                                        || HasResistanceInitPriseDeTerreFormatValidationError;
                RaisePropertyChanged(() => this.HasCoordFinPriseTerreLatFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété CoordFinbPriseTerreLong
        /// </summary>
        private bool _coordFinPriseTerreLongFormatValidationError = false;
        public bool HasCoordFinPriseTerreLongFormatValidationError
        {
            get
            {
                return _coordFinPriseTerreLongFormatValidationError;
            }
            set
            {
                _coordFinPriseTerreLongFormatValidationError = value;
                HasFormatValidationError = HasCoordDebPriseTerreLatFormatValidationError
                                        || HasCoordDebPriseTerreLongFormatValidationError
                                        || HasCoordFinPriseTerreLatFormatValidationError
                                        || HasCoordFinPriseTerreLongFormatValidationError
                                        || HasResistanceInitPriseDeTerreFormatValidationError;
                RaisePropertyChanged(() => this.HasCoordFinPriseTerreLongFormatValidationError);
            }
        }

        /// <summary>
        /// Gestion des erreurs de format pour la propriété ResistanceInitPriseDeTerre
        /// </summary>
        private bool _resistanceInitPriseDeTerreFormatValidationError = false;
        public bool HasResistanceInitPriseDeTerreFormatValidationError
        {
            get
            {
                return _resistanceInitPriseDeTerreFormatValidationError;
            }
            set
            {
                _resistanceInitPriseDeTerreFormatValidationError = value;
                HasFormatValidationError = HasCoordDebPriseTerreLatFormatValidationError
                                        || HasCoordDebPriseTerreLongFormatValidationError
                                        || HasCoordFinPriseTerreLatFormatValidationError
                                        || HasCoordFinPriseTerreLongFormatValidationError
                                        || HasResistanceInitPriseDeTerreFormatValidationError;
                RaisePropertyChanged(() => this.HasResistanceInitPriseDeTerreFormatValidationError);
            }
        }

        #endregion

        #endregion Public Properties

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
        public Equipement_DEViewModel(): base()
        {
            
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
                    EventAggregator.Publish("Equipement_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true).AddNamedParameter(Constants.SPECIFIC_VIEWMODEL_NAME, "Equipement_DE"));
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.TypePrisesTerre);
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
            ((EqEquipementService)service).GetEntityByCle<EqDispoEcoulementCourantsAlternatifs>(SelectedId.Value, (error) => DetailEntityLoaded(error));
        }

        /// <summary>
        /// On override l'ajout d'un équipement
        /// </summary>
        protected override void Add()
        {
            this.AddEquipement<EqDispoEcoulementCourantsAlternatifs>();
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
            if (SelectedEntity != null && SelectedEntity is EqDispoEcoulementCourantsAlternatifs)
            {
                if (!((EqDispoEcoulementCourantsAlternatifs)SelectedEntity).PresenceTelemesure)
                {
                    ((EqDispoEcoulementCourantsAlternatifs)SelectedEntity).DateMiseEnServiceTelemesure = null;
                }
            }
            base.Save(forceSave, withHisto);
        }
        
        #endregion override Methods
    }
}
