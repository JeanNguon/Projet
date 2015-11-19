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

namespace Proteca.Silverlight.ViewModels
{
    [ExportAsViewModel("CreateInsInstrument")]
    public class CreateInsInstrumentViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        #region Services

        [Import]
        public ChildWindowControl Childwindow;

        #endregion

        #region Porperties

        private InsInstrument _ins;
        public InsInstrument Ins
        {
            get
            {
                if(_ins == null)
                {
                    _ins = new InsInstrument();
                }
                return _ins;
            }
            set
            {
                _ins = value;
                RaisePropertyChanged(() => this.Ins);
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

        public CreateInsInstrumentViewModel()
            : base()
        {
            ValidateCommand = new ActionCommand<object>(
                obj => Validate(), obj => true);
            CancelCommand = new ActionCommand<object>(
                obj => Cancel(), obj => true);
        }

        #endregion

        #region Private Methods

        private void Validate()
        {
            Collection<ValidationResult> errors = new Collection<ValidationResult>();

            if (!Validator.TryValidateObject(Ins, new ValidationContext(Ins, null, null), errors, true))
            {
                foreach (var err in errors)
                {
                    Ins.ValidationErrors.Add(err);
                }
            }
            else
            {
                // Publication de l'Utilisateur
                EventAggregator.Publish(Ins);

                Ins = null;
                // Fermeture de la popup
                Childwindow.DialogResult = true;
            }
        }

        private void Cancel()
        {
            Ins = null;

            // Fermeture de la popup
            Childwindow.DialogResult = false;
        }

        public void OnImportsSatisfied()
        {
        }

        #endregion

    }
}
