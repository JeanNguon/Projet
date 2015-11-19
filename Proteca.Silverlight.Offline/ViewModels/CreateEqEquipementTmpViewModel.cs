using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework.Command;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Views.Windows;
using Proteca.Web.Models;
using System.Linq;
using System.Collections.Generic;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Enums;
using Jounce.Framework.Workflow;
using Proteca.Silverlight.Services.EntityServices;

namespace Proteca.Silverlight.ViewModels
{
    [ExportAsViewModel("CreateEqEquipementTmp")]
    public class CreateEqEquipementTmpViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        #region Services

        [Import]
        public ChildWindowControl Childwindow;

        [Import]
        public IEntityService<Pp> servicePp;

        [Import]
        public IEntityService<TypeEquipement> serviceTypeEquipement;

        [Import]
        public IEntityService<RefEnumValeur> serviceRefEnumValeur;

        [Import]
        public IEntityService<Composition> serviceComposition;

        #endregion

        #region Properties

        public ObservableCollection<PortionIntegrite> ListPortions
        {
            get
            {
                return new ObservableCollection<PortionIntegrite>(this.servicePp.Entities.Select(p => p.PortionIntegrite).Distinct(new InlineEqualityComparer<PortionIntegrite>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.Libelle.Equals(b.Libelle);
                                })));
            }
        }

        private int? _clePortion;
        public int? ClePortion
        {
            get
            {
                return _clePortion;
            }
            set
            {
                if (value != _clePortion)
                {
                    _clePortion = value;
                    RaisePropertyChanged(() => this.ClePortion);
                    RaisePropertyChanged(() => this.PpList);
                }
            }
        }

        public ObservableCollection<Pp> PpList
        {
            get
            {
                return new ObservableCollection<Pp>(this.servicePp.Entities.Where(p => ClePortion.HasValue && p.ClePortion == ClePortion.Value));
            }
        }

        private Composition _composition = null;
        public Composition Composition
        {
            get
            {
                if (_composition == null)
                {
                    _composition = new Composition()
                        {
                            EqEquipementTmp = new EqEquipementTmp()
                        };
                }
                return _composition;
            }
            set
            {
                _composition = value;
                RaisePropertyChanged(() => this.Composition);
            }
        }

        public ObservableCollection<RefEnumValeur> ListTypeEval
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(this.serviceRefEnumValeur.Entities.Where(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue() && r.LibelleCourt != "TLM" && r.LibelleCourt != "DS" && r.LibelleCourt != "MI").OrderBy(r => r.NumeroOrdre));
            }
        }

        public ObservableCollection<TypeEquipement> ListTypeEq
        {
            get
            {
                return new ObservableCollection<TypeEquipement>(this.serviceTypeEquipement.Entities.Where(te => te.CodeEquipement != "PP").OrderBy(te => te.NumeroOrdre));
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Commande de validation de la sélection
        /// </summary>
        public IActionCommand ValidateCommand { get; set; }

        /// <summary>
        /// Commande d'annulation de la sélection
        /// </summary>
        public IActionCommand CancelCommand { get; set; }

        #endregion

        #region Constructor

        public CreateEqEquipementTmpViewModel()
            : base()
        {
            ValidateCommand = new ActionCommand<object>(
                obj => Validate(), obj => true);
            CancelCommand = new ActionCommand<object>(
                obj => Cancel(), obj => true);
        }

        #endregion

        #region Private Methods

        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            RaisePropertyChanged(() => this.ListPortions);
            RaisePropertyChanged(() => this.PpList);
            RaisePropertyChanged(() => this.ListTypeEq);
            RaisePropertyChanged(() => this.ListTypeEval);
            base.ActivateView(viewName, viewParameters);
        }

        protected override void DeactivateView(string viewName)
        {
            this.ClePortion = null;
        }

        private void Validate()
        {
            Collection<ValidationResult> errorsEq = new Collection<ValidationResult>();
            Collection<ValidationResult> errorsComp = new Collection<ValidationResult>();

            if (!Validator.TryValidateObject(Composition.EqEquipementTmp, new ValidationContext(Composition.EqEquipementTmp, null, null), errorsEq, true) || !Validator.TryValidateObject(Composition, new ValidationContext(Composition, null, null), errorsComp, true))
            {
                foreach (var err in errorsEq)
                {
                    Composition.EqEquipementTmp.ValidationErrors.Add(err);
                }

                foreach (var err in errorsComp)
                {
                    Composition.ValidationErrors.Add(err);
                }

            }
            else
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("ClePp", this.Composition.EqEquipementTmp.ClePp);
                result.Add("CleTypeEq", this.Composition.EqEquipementTmp.CleTypeEq);
                result.Add("Libelle", this.Composition.EqEquipementTmp.Libelle);
                result.Add("EnumTypeEval", this.Composition.EnumTypeEval);

                Refresh();
                // Publication de l'Utilisateur
                EventAggregator.Publish(result);

                this.Close(true);
            }
        }

        private void Cancel()
        {
            Refresh();
            this.Close(false);
        }

        private void Close(Boolean result)
        {
            // Fermeture de la popup
            this.Childwindow.DialogResult = result;
        }

        private void Refresh()
        {
            this.ClePortion = null;
            (this.serviceComposition as CompositionService).DeleteEqTmp(this.Composition.EqEquipementTmp);
            this.Composition.EqEquipementTmp = null;
            this.Composition = null;
        }

        #endregion

        #region Public Properties

        public void OnImportsSatisfied()
        {
        }

        #endregion
    }
}
