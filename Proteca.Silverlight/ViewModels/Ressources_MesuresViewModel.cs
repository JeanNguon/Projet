using System.Collections.ObjectModel;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System;
using System.Linq;
using Proteca.Silverlight.Enums;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using Jounce.Core.Command;
using System.Windows;
using Proteca.Silverlight.Resources;
using Jounce.Framework.Command;
using Proteca.Silverlight.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Ressources_Mesures entity
    /// </summary>
    [ExportAsViewModel("Ressources_Mesures")]
    public class Ressources_MesuresViewModel : BaseProtecaEntityViewModel<MesCoutMesure>
    {
        #region Properties

        /// <summary>
        /// Liste des filtres enum valeur
        /// </summary>
        private string enumPP_POLARISATION = RefEnumValeurCodeGroupeEnum.PP_POLARISATION.GetStringValue();
        private string enumPP_DUREE_ENRG = RefEnumValeurCodeGroupeEnum.PP_DUREE_ENRG.GetStringValue();
        private string enumTYPE_EVAL = RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue();

        /// <summary>
        /// Retourne la liste des couts de mesure
        /// </summary>
        public ObservableCollection<MesCoutMesure> ListCoutMesure
        {
            get
            {
                if (this.Entities != null)
                {
                    return new ObservableCollection<MesCoutMesure>(this.Entities);
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Retourne les liste des évaluation du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListEvaluation
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumTYPE_EVAL).OrderBy(r => r.LibelleCourt));
            }
        }

        /// <summary>
        /// Retourne les liste des DureeEnreg du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListDureeEnreg
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumPP_DUREE_ENRG).OrderBy(r => r.Libelle));
            }
        }

        /// <summary>
        /// Retourne les liste des polarisations du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListPolarisation
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumPP_POLARISATION).OrderBy(r => r.Libelle));
            }
        }

        /// <summary>
        /// Retourne les liste des types équipement du service TypeEquipement
        /// </summary>
        public ObservableCollection<TypeEquipement> ListTypeEquipement
        {
            get
            {
                return serviceTypeEquipement.Entities;
            }
        }

        #endregion

        #region Contructor

        public Ressources_MesuresViewModel()
            : base()
        {
            this.IsAutoNavigateToFirst = false;

            this.OnViewModeChanged += (o, e) =>
            {
                RaisePropertyChanged(() => this.SelectedEntity.IsPP);
            };

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(r => r.Key == "IsTopContainerLoaded"))
                {
                    EventAggregator.Publish("CustomTopContainer".AsViewNavigationArgs().AddNamedParameter("HideContainer", false));
                    EventAggregator.Publish("TypeRessource".AsViewNavigationArgs().AddNamedParameter("IsTopContainerLoaded", true));
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.ListDureeEnreg);
                RaisePropertyChanged(() => this.ListEvaluation);
                RaisePropertyChanged(() => this.ListPolarisation);
                RaisePropertyChanged(() => this.ListTypeEquipement);
                RaisePropertyChanged(() => this.ListCoutMesure);
            };

            // Define commands
            DeleteLineCommand = new ActionCommand<object>(
                obj => DeleteLine(obj), obj => true);

            SelectedCellChangedCommand = new ActionCommand<object>(
                obj => RaisePropertyChanged(() => this.Entities), obj => true);
        }

        #endregion

        #region Command

        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DeleteLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'événement d'une modification d'un item
        /// </summary>
        public IActionCommand SelectedCellChangedCommand { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        ///  Contrôle l'unicité du couple Eval/Type Eq/Polarisation/Durée
        /// </summary>
        /// <returns></returns>
        public void CheckUniques()
        {
            bool Response = true;

            if (Entities != null)
            {
                foreach (var mesureToCheck in Entities.Where(s => s.IsNew() || s.HasChanges))
                {
                    mesureToCheck.ValidationErrors.Clear();

                    foreach (var mesure in Entities.Except(Entities.Where(s => s == mesureToCheck)))
                    {
                        Response =
                            mesure.EnumTypeEval != mesureToCheck.EnumTypeEval ? true :                          // Evaluation
                            mesure.CleTypeEq != mesureToCheck.CleTypeEq ? true :                                // Type Equipement
                            mesure.EnumTempsPolarisation != mesureToCheck.EnumTempsPolarisation ? true :        // Polarisation
                            mesure.EnumDureeEnregistrement != mesureToCheck.EnumDureeEnregistrement ? true :    // Durée d'enregistrement
                            false;

                        if (!Response)
                        {
                            mesureToCheck.ValidationErrors.Add(new ValidationResult("L'unicité n'est pas respecté", new List<String>() { "RefEnumValeur", "TypeEquipement", "RefEnumValeur2", "RefEnumValeur1" }));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        protected virtual void DeleteLine(object Obj)
        {
            var result = MessageBox.Show(Resource.Ressource_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                this.service.Delete((MesCoutMesure)Obj);
                this.Entities.Remove((MesCoutMesure)Obj);
                RaisePropertyChanged(() => this.Entities);
            };
        }

        /// <summary>
        /// Activation de la vue
        /// Lors de l'activation on cache la partie customExpander
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);
            EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("HideExpander", true));
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Sauvegarde si unique
        /// </summary>
        protected override void Save()
        {
            CheckUniques();
            if (Entities.Any(e => e.HasValidationErrors))
            {
                this.NotifyError = true;
            }
            else
            {
                base.Save();
            }
        }

        /// <summary>
        /// Fonction d'ajout de ligne dans le tableau
        /// </summary>
        protected override void Add()
        {
            base.Add();
            this.SelectedEntity = null;
            RaisePropertyChanged(() => this.ListCoutMesure);
        }

        /// <summary>
        /// MAJ de la vue après l'annulation
        /// </summary>
        protected override void Cancel()
        {
            base.Cancel();

            LoadEntities();
            RaisePropertyChanged(() => this.ListCoutMesure);
        }

        #endregion

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> serviceRefEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type TypeEquipement
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> serviceTypeEquipement { get; set; }

        #endregion
    }
}
