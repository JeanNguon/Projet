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

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Equipement_DR entity
    /// </summary>
    [ExportAsViewModel("Equipement_DR")]
    public class Equipement_DRViewModel : EquipementViewModel
    {
        #region Public Properties

        /// <summary>
        /// Liste des types de drainages
        /// </summary>
        public List<RefSousTypeOuvrage> TypeDrainages
        {
            get
            {
                return ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == RefSousTypeOuvrageGroupEnum.TypeDrainage.GetStringValue())
                    .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle).ToList();
            }
        }

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
        public Equipement_DRViewModel(): base()
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
                    EventAggregator.Publish("Equipement_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true).AddNamedParameter(Constants.SPECIFIC_VIEWMODEL_NAME, "Equipement_DR"));
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.TypeDrainages);
            };

            this.OnDetailLoaded += (o, e) =>
            {
            };
        }

        #endregion Constructor

        #region override Methods

        /// <summary>
        /// Surcharge du chargement du détail de l'entité
        /// </summary>
        public override void LoadDetailEntity()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IsBusy = true;
            ((EqEquipementService)service).GetEntityByCle<EqDrainage>(SelectedId.Value, (error) => DetailEntityLoaded(error));
        }

        /// <summary>
        /// On override l'ajout d'un équipement
        /// </summary>
        protected override void Add()
        {
            this.AddEquipement<EqDrainage>();
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
            if (SelectedEntity != null && SelectedEntity is EqDrainage)
            {
                if (!((EqDrainage)SelectedEntity).PresenceTelemesure)
                {
                    ((EqDrainage)SelectedEntity).DateMiseEnServiceTelemesure = null;
                }
            }
            base.Save(forceSave, withHisto);
        }

        #endregion override Methods
    }
}
