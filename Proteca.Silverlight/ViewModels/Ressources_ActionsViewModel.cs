using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using Proteca.Silverlight.Services.EntityServices;
using Jounce.Core.Application;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Ressources_Actions entity
    /// </summary>
    [ExportAsViewModel("Ressources_Actions")]
    public class Ressources_ActionsViewModel : BaseProtecaEntityViewModel<ParametreAction>
    {
        #region Private Functions

        /// <summary>
        /// Indique si un ou plusieurs éléments sont enajout
        /// </summary>
        private bool _add = false;

        #endregion Private Functions

        #region Constantes

        /// <summary>
        /// Liste des filtres enum valeur
        /// </summary>
        private string enumACTION_CATEGORIE_ANOMALIE = RefEnumValeurCodeGroupeEnum.ACTION_CATEGORIE_ANOMALIE.GetStringValue();
        private string enumACTION_TYPE = RefEnumValeurCodeGroupeEnum.ACTION_TYPE.GetStringValue();
        private string enumACTION_PRIORITE= RefEnumValeurCodeGroupeEnum.ACTION_PRIORITE.GetStringValue();
        private string enumACTION_DELAI_REAL = RefEnumValeurCodeGroupeEnum.ACTION_DELAI_REAL.GetStringValue();

        #endregion Constantes

        #region Properties

        /// <summary>
        /// Retourne les liste des categorie d'anomalie du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListCategorieAnomalie
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities
                    .Where(r => r.CodeGroupe == enumACTION_CATEGORIE_ANOMALIE)
                    .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle));
            }
        }

        /// <summary>
        /// Retourne les liste des types actions du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListTypeAction
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities
                    .Where(r => r.CodeGroupe == enumACTION_TYPE)
                    .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle));
            }
        }

        /// <summary>
        /// Retourne les liste des degrés du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListPriorite
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities
                    .Where(r => r.CodeGroupe == enumACTION_PRIORITE)
                    .OrderBy(r => r.Valeur));
            }
        }
        
        /// <summary>
        /// Retourne les liste des délais de réalisation du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListDelais
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(serviceRefEnumValeur.Entities
                    .Where(r => r.CodeGroupe == enumACTION_DELAI_REAL)
                    .OrderBy(r => r.NumeroOrdre));
            }
        }

        /// <summary>
        /// Liste des paramètre action triées par catégorie d'anomalie
        /// </summary>
        public ObservableCollection<ParametreAction> ParametreActionList
        {
            get 
            {
                if (this.Entities != null)
                {
                    if (_add)
                        return this.Entities;
                    else
                        return new ObservableCollection<ParametreAction>(this.Entities.OrderBy(e => e.RefEnumValeur.Libelle));
                }
                else
                    return null;
            }
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

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> serviceRefEnumValeur { get; set; }

        #endregion

        #region Contructor

        public Ressources_ActionsViewModel()
            : base()
        {
            this.IsAutoNavigateToFirst = false;

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
                RaisePropertyChanged(() => this.ListCategorieAnomalie);
                RaisePropertyChanged(() => this.ListPriorite);
                RaisePropertyChanged(() => this.ListTypeAction);
                RaisePropertyChanged(() => this.ListDelais);
                RaisePropertyChanged(() => this.ParametreActionList);
            };

            this.OnSaveSuccess += (o, e) =>
            {
                _add = false;
                this.RaisePropertyChanged(() => this.ParametreActionList);
            };

            // Define commands
            DeleteLineCommand = new ActionCommand<object>(
                obj => DeleteLine(obj), obj => true);

            SelectedCellChangedCommand = new ActionCommand<object>(
                obj => RaisePropertyChanged(() => this.ParametreActionList), obj => true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///  Contrôle l'unicité du couple Eval/Type Eq/Polarisation/Durée
        /// </summary>
        /// <returns></returns>
        private void CheckUniques()
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
                            mesure.EnumCategorieAnomalie != mesureToCheck.EnumCategorieAnomalie ? true :    // Categorie d'anomalie
                            mesure.EnumTypeAction != mesureToCheck.EnumTypeAction ? true :                  // Type d'action
                            false;

                        if (!Response)
                        {
                            mesureToCheck.ValidationErrors.Add(new ValidationResult("L'unicité n'est pas respecté", new List<String>() { "RefEnumValeur", "RefEnumValeur2" }));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        private void DeleteLine(object Obj)
        {
            var result = MessageBox.Show(Resource.Ressource_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                ((ParametreActionService)this.service).CheckCanDeleteParametreAction(((ParametreAction)Obj).CleParametreAction, (error, retour) =>
                {
                    if (error != null)
                    {
                        Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                        ErrorWindow.CreateNew(Resource.DefaultErrorOnDelete);
                    }
                    else if (retour)
                    {
                        this.service.Delete((ParametreAction)Obj);
                        this.Entities.Remove((ParametreAction)Obj);
                        RaisePropertyChanged(() => this.ParametreActionList);
                    }
                    else
                    {
                        MessageBox.Show(Resource.ParametreAction_ErrorOnDelete, "", MessageBoxButton.OK);
                    }
                });
            };
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

        /// <summary>
        /// Fonction d'ajout de ligne dans le tableau
        /// </summary>
        protected override void Add()
        {
            base.Add();
            _add = true;
            RaisePropertyChanged(() => this.ParametreActionList);
        }

        /// <summary>
        /// MAJ de la vue après l'annulation
        /// </summary>
        protected override void Cancel()
        {
            base.Cancel();
            _add = false;
            RaisePropertyChanged(() => this.ParametreActionList);
        }

        #endregion
        
    }
}
