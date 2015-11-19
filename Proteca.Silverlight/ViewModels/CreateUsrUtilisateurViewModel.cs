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
    [ExportAsViewModel("CreateUsrUtilisateur")]
    public class CreateUsrUtilisateurViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        #region Services

        [Import]
        public ChildWindowControl Childwindow;

        #endregion

        #region Porperties

        private UsrUtilisateur _user;
        public UsrUtilisateur User
        {
            get
            {
                if(_user == null)
                {
                    _user = new UsrUtilisateur()
                                {
                                    Supprime = false,
                                    Externe = true,
                                    Mail = String.Empty,
                                    Identifiant = String.Empty
                                };
                }
                return _user;
            }
            set
            {
                _user = value;
                RaisePropertyChanged(() => this.User);
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

        public CreateUsrUtilisateurViewModel()
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

            if (!Validator.TryValidateObject(User, new ValidationContext(User, null, null), errors, true))
            {
                foreach (var err in errors)
                {
                    User.ValidationErrors.Add(err);
                }
            }
            else
            {
                // Publication de l'Utilisateur
                EventAggregator.Publish(User);

                User = null;
                // Fermeture de la popup
                Childwindow.DialogResult = true;
            }
        }

        private void Cancel()
        {
            User = null;

            // Fermeture de la popup
            Childwindow.DialogResult = false;
        }

        public void OnImportsSatisfied()
        {
        }

        #endregion

    }
}
