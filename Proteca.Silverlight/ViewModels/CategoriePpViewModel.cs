using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using System.Text;
using Proteca.Web.Models;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.EntityServices;
using System.Windows;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for CategoriePp entity
    /// </summary>
    [ExportAsViewModel("CategoriePp")]
    public class CategoriePpViewModel : BaseProtecaEntityViewModel<CategoriePp>
    {
        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type type equipement
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> serviceTypeEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefNiveauSensibilitePp
        /// </summary>
        [Import]
        public IEntityService<RefNiveauSensibilitePp> serviceNiveauSensibilitePp { get; set; }

        #endregion

        #region Properties

        private RefNiveauSensibilitePp _currentSelectedFiltre;
        public RefNiveauSensibilitePp CurrentSelectedFiltre 
        {
            get
            {
                return _currentSelectedFiltre;
            }
            set
            {
                _currentSelectedFiltre = value;
            }
        }

        /// <summary>
        /// Retourne les niveaux de sensibilités liés aux catégories PP
        /// </summary>
        public ObservableCollection<RefNiveauSensibilitePp> NiveauSensibilites
        {
            get
            {
                if (this.serviceNiveauSensibilitePp.Entities != null)
                {
                    return new ObservableCollection<RefNiveauSensibilitePp>(this.serviceNiveauSensibilitePp.Entities.Where(ns => ns.TypeSensibilite == 2 && ns.EnumTypeEval.HasValue));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retourne la liste des categories PP
        /// </summary>
        public ObservableCollection<CategoriePp> ListCategoriePP
        {
            get
            {
                if (CurrentSelectedFiltre != null)
                {
                    if (this.NiveauSensibilites.Where(r => r.IsSelected == true).FirstOrDefault() == null)
                    {
                        this.NiveauSensibilites.Where(r => r == CurrentSelectedFiltre).FirstOrDefault().IsSelected = true;
                    }
                    else
                    {
                        CurrentSelectedFiltre = this.NiveauSensibilites.Where(r => r.IsSelected == true).FirstOrDefault();
                    }
                    return new ObservableCollection<CategoriePp>(this.Entities.Where(r => r.CleNiveauSensibilite == CurrentSelectedFiltre.CleNiveauSensibilite).OrderBy(r => r.NumeroOrdre));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retourne la liste de type équipement du service correspondant
        /// </summary>
        public ObservableCollection<TypeEquipement> TypeEquipements
        {
            get
            {
                return serviceTypeEquipement.Entities;
            }
        }

        #endregion

        #region Contructor

        public CategoriePpViewModel()
            : base()
        {

            this.IsAutoNavigateToFirst = false;

            this.OnAllServicesLoaded += (o, e) =>
            {
                TypeEquipementLoaded(null);

                if (this.Entities != null)
                {
                    if (!this.NiveauSensibilites.Any(n => n.IsSelected))
                    {
                        this.NiveauSensibilites.FirstOrDefault().IsSelected = true;
                        CurrentSelectedFiltre = this.NiveauSensibilites.Where(r => r.IsSelected == true).FirstOrDefault();
                    }
                    this.RaisePropertyChanged(() => this.NiveauSensibilites);
                    this.RaisePropertyChanged(() => this.ListCategoriePP);
                }
            };
            
            // Define commands
            DeleteLineCommand = new ActionCommand<object>(
                obj => DeleteLine(obj), obj => true);

            CheckCommand = new ActionCommand<object>(
                obj => CheckFiltre(obj), obj => true);
        }

        #endregion

        #region Command


        /// <summary>
        /// Déclaration de l'objet de command de suppression de ligne
        /// </summary>
        public IActionCommand DeleteLineCommand { get; private set; }

        /// <summary>
        /// Déclaration de la variable de sélection du niveau de sensibilités
        /// </summary>
        public IActionCommand CheckCommand { get; private set; }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        protected virtual void DeleteLine(object Obj)
        {
            var result = MessageBox.Show(Resource.CategoriePP_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                ((CategoriePpService)this.service).CheckCanDeleteCategoriesPp(((CategoriePp)Obj).CleCategoriePp, (errors, retour) =>
                    {
                        if (!retour || errors != null)
                        {
                            MessageBox.Show(Resource.CategoriePP_DeleteInterdiction, "", MessageBoxButton.OK);
                        }
                        else
                        {
                            this.service.Delete((CategoriePp)Obj);
                            this.Entities.Remove((CategoriePp)Obj);
                            RaisePropertyChanged(() => this.ListCategoriePP);
                        }
                    }
                );
                    
            };
        }

        /// <summary>
        /// Fonction de MAJ du tableau en fonction du niveau de sensibilité
        /// </summary>
        protected virtual void CheckFiltre(object Obj)
        {
            RaisePropertyChanged(() => this.ListCategoriePP);
        }

        /// <summary>
        /// Méthode utilisé pour charger l'entité de type Region
        /// </summary>
        private void TypeEquipementLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(TypeEquipement).Name));
            }
            else
            {
                if (this.TypeEquipements != null)
                {
                    RaisePropertyChanged(() => this.TypeEquipements);
                }
            }

            // We're done
            IsBusy = false;
        }

        /// <summary>
        /// Rechargement des données (merge depuis la base)
        /// </summary>
        /// <param name="error"></param>
        private void ReloadData(Exception error)
        {
            RaisePropertyChanged(() => this.ListCategoriePP);
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Fonction d'ajout de ligne dans le tableau
        /// </summary>
        protected override void Add()
        {
            base.Add();
            this.SelectedEntity.RefNiveauSensibilitePp = NiveauSensibilites.FirstOrDefault(ns => ns.IsSelected == true);
            this.SelectedEntity = null;
            RaisePropertyChanged(() => this.ListCategoriePP);
        }

        /// <summary>
        /// MAJ de la vue après l'annulation
        /// </summary>
        protected override void Cancel()
        {
            base.Cancel();
            //this.service.GetEntities(ReloadData);
            this.RaisePropertyChanged(() => this.Entities);
            this.RaisePropertyChanged(() => this.ListCategoriePP);
        }
        
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
