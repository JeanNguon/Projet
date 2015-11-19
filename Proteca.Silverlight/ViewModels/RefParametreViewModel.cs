using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for RefParametre entity
    /// </summary>
    [ExportAsViewModel("RefParametre")]
    public class RefParametreViewModel : BaseProtecaEntityViewModel<RefParametre>
    {

        #region Properties

        public ObservableCollection<RefParametre> ListVisites
        {
            get
            {
                return this.Entities != null ? new ObservableCollection<RefParametre>(this.Entities.Where(r => r.CodeGroupe == "VISITES").OrderBy(u => u.NumeroOrdre)) : null;
            }
        }

        public ObservableCollection<RefParametre> ListEchanges
        {
            get
            {
                return this.Entities != null ? new ObservableCollection<RefParametre>(this.Entities.Where(r => r.CodeGroupe == "ECHANGES").OrderBy(u => u.NumeroOrdre)) : null;
            }
        }

        public ObservableCollection<RefParametre> ListSoutirages
        {
            get
            {
                return this.Entities != null ? new ObservableCollection<RefParametre>(this.Entities.Where(r => r.CodeGroupe == "BILANS_SOUTIRAGES").OrderBy(u => u.NumeroOrdre)) : null;
            }
        }

        public ObservableCollection<RefParametre> ListPP
        {
            get
            {
                return this.Entities != null ? new ObservableCollection<RefParametre>(this.Entities.Where(r => r.CodeGroupe == "BILANS_PP").OrderBy(u => u.NumeroOrdre)) : null;
            }
        }

        public ObservableCollection<RefParametre> ListImages
        {
            get
            {
                return this.Entities != null ? new ObservableCollection<RefParametre>(this.Entities.Where(r => r.CodeGroupe == "GESTION_IMAGES").OrderBy(u => u.NumeroOrdre)) : null;
            }
        }

        public ObservableCollection<RefParametre> ListPI
        {
            get
            {
                return this.Entities != null ? new ObservableCollection<RefParametre>(this.Entities.Where(r => r.CodeGroupe == "PORTIONS_INTEGRITES").OrderBy(u => u.NumeroOrdre)) : null;
            }
        }

        public ObservableCollection<MesModeleMesure> ListMesModeleMesure
        {
            get
            {
                if (this.serviceMesModeleMesure!=null && this.serviceMesModeleMesure.Entities != null && this.serviceMesModeleMesure.Entities.Any())
                {
                    return new ObservableCollection<MesModeleMesure>(this.serviceMesModeleMesure.Entities.Where(m => m.TypeEquipement != null && m.TypeEquipement.CodeEquipement == "SO").OrderBy(m => m.NumeroOrdre).ThenBy(m => m.LibGenerique));
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region "Services"
        [Import]
        public IEntityService<MesModeleMesure> serviceMesModeleMesure { get; set; }
        #endregion

        #region Constructor

        public RefParametreViewModel()
            : base()
        {
            IsAutoNavigateToFirst = false;

            this.OnAllServicesLoaded += (o, e) =>
            {
                if (this.Entities != null)
                {
                    foreach (var entity in Entities)
                    {
                        entity.PropertyChanged += (oo, ez) =>
                        {
                            SetCanProperties();
                            if (this.NotifyError)
                            {
                                this.NotifyError = this.Entities.Any(p => p.HasValidationErrors);
                            }
                        };
                    }
                    RaisePropertyChanged(() => ListEchanges);
                    RaisePropertyChanged(() => ListImages);
                    RaisePropertyChanged(() => ListPI);
                    RaisePropertyChanged(() => ListPP);
                    RaisePropertyChanged(() => ListMesModeleMesure);
                    RaisePropertyChanged(() => ListSoutirages);
                    RaisePropertyChanged(() => ListVisites);
                   
                }
            };

            this.OnCanceled += (o, e) =>
            {
                this.LoadEntities();
            };

            this.OnEntitiesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => ListEchanges);
                RaisePropertyChanged(() => ListImages);
                RaisePropertyChanged(() => ListPI);
                RaisePropertyChanged(() => ListPP);
                RaisePropertyChanged(() => ListSoutirages);
                RaisePropertyChanged(() => ListVisites);
            };

            this.OnImportsSatisfiedEvent += (o, e) =>
            {
                ((MesModeleMesureService)this.serviceMesModeleMesure).LoadEntities = true;
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Activation de la vue de regroiupement de région.
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
    }
}
