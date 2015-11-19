using System.Collections.ObjectModel;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System.Linq;
using System;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.EntityServices;
using System.Windows;
using System.Collections.Generic;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for EqEquipement entity
    /// </summary>
    [ExportAsViewModel("Equipement")]
    public class EqEquipementViewModel : BaseProtecaEntityViewModel<EqEquipement>
    {
        /// <summary>
        /// Service utilisé pour pouvoir associé les type d'équipement sur l'équipement en ajout
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> ServiceTypeEquipement { get; set; }

        public EqEquipementViewModel()
            : base()
        {
            this.OnDetailLoaded += (o, e) =>
            {
                if (this.SelectedEntity != null)
                {
                    NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                                  MainNavigation.GestionOuvrages.GetStringValue(),
                                  OuvrageNavigation.Equipement.GetStringValue(),
                                  this.SelectedEntity.CodeEquipement,
                                  this.SelectedEntity.CleMicado), UriKind.Relative));
                }
                else
                {
                    NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}",
                                  MainNavigation.GestionOuvrages.GetStringValue(),
                                  OuvrageNavigation.Equipement.GetStringValue(),
                                  "PP"), UriKind.Relative));
                }
            };
        }
    }
}
