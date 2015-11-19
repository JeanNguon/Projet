using System.Linq;
using System.Reflection;
using Jounce.Core;
using Jounce.Core.Application;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel pour les fourreaux métalliques
    /// </summary>
    [ExportAsViewModel("Equipement_FM")]
    public class Equipement_FMViewModel : EquipementViewModel
    {
        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Equipement_FMViewModel()
            : base()
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
                    EventAggregator.Publish("Equipement_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true).AddNamedParameter(Constants.SPECIFIC_VIEWMODEL_NAME, "Equipement_FM"));
                }
            };
        }

        #endregion Constructor

        #region Override Functions

        /// <summary>
        /// surcharge la Réintégration de l'équipement
        /// </summary>
        protected override void ReintegrateEquipement()
        {
            if (((EqFourreauMetallique)this.SelectedEntity).Pp2 != null && ((EqFourreauMetallique)this.SelectedEntity).Pp2.Supprime)
            {
                ((EqFourreauMetallique)this.SelectedEntity).ClePp2 = 0;
            }

            base.ReintegrateEquipement();
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

            if (this.SelectedEntity != null && ((EqFourreauMetallique)this.SelectedEntity).Pp2 != null)
            {
                this.SelectedEntity.Portion2Selected = this.ListPortions.FirstOrDefault(p => p.ClePortion == ((EqFourreauMetallique)this.SelectedEntity).Pp2.ClePortion);
            }

            RaisePropertyChanged(() => this.SelectedEntity);

            base.refreshPortions();
        }

        /// <summary>
        /// Surcharge du chargement de l'entité
        /// </summary>
        public override void LoadDetailEntity()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IsBusy = true;
            ((EqEquipementService)service).GetEntityByCle<EqFourreauMetallique>(SelectedId.Value, (error) => DetailEntityLoaded(error));
        }

        /// <summary>
        /// On override l'ajout d'un équipement
        /// </summary>
        protected override void Add()
        {
            this.AddEquipement<EqFourreauMetallique>();
        }

        #endregion Override Functions
    }
}
