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
using System.Collections.ObjectModel;
using System;
using Proteca.Silverlight.Resources;
using System.ComponentModel;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Equipement_RI entity
    /// </summary>
    [ExportAsViewModel("Equipement_RI")]
    public class Equipement_RIViewModel : EquipementViewModel
    {
        #region Private Properties

        private ObservableCollection<LiaisonCommunes> _liaisonsCommunes;

        #endregion Private Properties

        #region Public Properties

        /// <summary>
        /// Liste des types de redresseurs
        /// </summary>
        public List<RefSousTypeOuvrage> TypeRaccordsIsolants
        {
            get
            {
                return ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == RefSousTypeOuvrageGroupEnum.TypeRaccord.GetStringValue())
                    .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle).ToList();
            }
        }

        /// <summary>
        /// Liste des type de deversoir
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
        /// Liste des liaison qui ont la même PP que le raccord électrique courant
        /// </summary>
        public ObservableCollection<LiaisonCommunes> LiaisonsPPCommunes
        {
            get { return _liaisonsCommunes; }
            set
            {
                _liaisonsCommunes = value;
                RaisePropertyChanged(() => this.LiaisonsPPCommunes);
                RaisePropertyChanged(() => this.NbLiaisonsPPCommune);
                RaisePropertyChanged(() => this.SelectedEntity);
            }
        }

        /// <summary>
        /// Nombre de liaison communes
        /// </summary>
        public int NbLiaisonsPPCommune
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
        /// Liste des types des états électriques
        /// </summary>
        public List<RefEnumValeur> EtatsElecs
        {
            get { return ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.RACCORD_IS_ETAT_ELEC.GetStringValue()).ToList(); }
        }

        /// <summary>
        /// Liste des types des configs électrique normale
        /// </summary>
        public List<RefEnumValeur> ConfigElecNormale
        {
            get { return ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.RACCORD_IS_CONF_ELEC.GetStringValue()).ToList(); }
        }

        #endregion Public Properties

        #region Services

        /// <summary>
        /// Service utilisé pour gérer les entités de type RefSousTypeOuvrage
        /// </summary>
        [Import]
        public IEntityService<RefSousTypeOuvrage> ServiceRefSousTypeOuvrage { get; set; }

        #endregion Services

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Equipement_RIViewModel()
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
                    EventAggregator.Publish("Equipement_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true).AddNamedParameter(Constants.SPECIFIC_VIEWMODEL_NAME, "Equipement_RI"));
                }
            };

            this.OnDetailLoaded += (o, e) =>
            {
                this.ListenToClePPChangesAndRefreshListLiaison();
            };

            this.OnAddedEntity += (o, e) =>
            {
                this.ListenToClePPChangesAndRefreshListLiaison();
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.TypeRaccordsIsolants);
                RaisePropertyChanged(() => this.TypeLiaisons);
                RaisePropertyChanged(() => this.EtatsElecs);
                RaisePropertyChanged(() => this.ConfigElecNormale);
            };
        }

        #endregion Constructor

        #region override Methods

        /// <summary>
        /// surcharge la Réintégration de l'équipement
        /// </summary>
        protected override void ReintegrateEquipement()
        {
            if (((EqRaccordIsolant)this.SelectedEntity).Pp2 != null && ((EqRaccordIsolant)this.SelectedEntity).Pp2.Supprime)
            {
                ((EqRaccordIsolant)this.SelectedEntity).ClePp2 = 0;
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
            ((EqEquipementService)service).GetEntityByCle<EqRaccordIsolant>(SelectedId.Value, (error) => DetailEntityLoaded(error));
        }

        /// <summary>
        /// On override l'ajout d'un équipement
        /// </summary>
        protected override void Add()
        {
            this.AddEquipement<EqRaccordIsolant>();
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

            if (this.SelectedEntity != null && ((EqRaccordIsolant)this.SelectedEntity).Pp2 != null)
            {
                this.SelectedEntity.Portion2Selected = this.ListPortions.FirstOrDefault(p => p.ClePortion == ((EqRaccordIsolant)this.SelectedEntity).Pp2.ClePortion);
            }

            RaisePropertyChanged(() => this.SelectedEntity);

            base.refreshPortions();
        }

        #endregion override Methods

        #region Private Functions

        /// <summary>
        /// Abonnement au changements de la clePP et mise à jour de la liste
        /// </summary>
        private void ListenToClePPChangesAndRefreshListLiaison()
        {
            if (this.SelectedEntity != null)
            {
                PropertyChangedEventHandler changePP = (oo, ee) =>
                {
                    if (ee.PropertyName == "ClePp")
                    {
                        if (this.SelectedEntity.ClePp > 0)
                            ((EqEquipementService)this.service).GetListLiaisonPPCommun(this.SelectedEntity.ClePp, ListLiaisonPPCommunLoaded);
                        else
                            LiaisonsPPCommunes = null;
                    }
                };

                SelectedEntity.PropertyChanged -= changePP;
                SelectedEntity.PropertyChanged += changePP;

                if (this.SelectedEntity.ClePp > 0)
                    ((EqEquipementService)this.service).GetListLiaisonPPCommun(this.SelectedEntity.ClePp, ListLiaisonPPCommunLoaded);
                else
                    LiaisonsPPCommunes = null;
            }
        }

        /// <summary>
        /// Les liaisons ayant la même PP 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="liaisonsCommunes"></param>
        private void ListLiaisonPPCommunLoaded(Exception error, ObservableCollection<LiaisonCommunes> liaisonsCommunes)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            else
            {
                this.LiaisonsPPCommunes = liaisonsCommunes;
            }
        }

        #endregion Private Functions
    }
}
