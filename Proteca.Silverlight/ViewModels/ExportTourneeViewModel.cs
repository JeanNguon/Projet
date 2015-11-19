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
    [ExportAsViewModel("ExportTournee")]
    public class ExportTourneeViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {
        #region Services

        [Import]
        public ChildWindowControl Childwindow;

        #endregion

        #region Command

        /// <summary>
        /// Commande de choix de la sélection
        /// </summary>
        public IActionCommand ExportCommand { get; set; }

        /// <summary>
        /// Commande d'annulation de la sélection
        /// </summary>
        public IActionCommand CancelCommand { get; set; }

        #endregion

        #region Constructor

        public ExportTourneeViewModel()
            : base()
        {
            ExportCommand = new ActionCommand<object>(
                obj => Clicked(obj));
            CancelCommand = new ActionCommand<object>(
                obj => Cancel(), obj => true);
        }

        #endregion

        #region Private Methods

        private void Clicked(object obj)
        {
            if (obj != null)
            {
                // Publication de l'Utilisateur
                EventAggregator.Publish(obj.ToString());

                // Fermeture de la popup
                Childwindow.DialogResult = true;
            }
        }

        private void Cancel()
        {
            // Fermeture de la popup
            Childwindow.DialogResult = false;
        }

        public void OnImportsSatisfied()
        {
        }

        #endregion

    }
}
