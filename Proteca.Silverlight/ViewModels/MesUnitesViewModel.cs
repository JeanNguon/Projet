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
    /// ViewModel for Unites entity
    /// </summary>
    [ExportAsViewModel("MesUnites")]
    public class MesUnitesViewModel : BaseProtecaEntityViewModel<MesUnite>
    {
        #region Properties

        /// <summary>
        /// Liste des types d'unités
        /// </summary>
        public List<RefEnumValeur> RefEnumValeurList
        {
            get { return serviceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.UNITE_TYP_DONNEE.GetStringValue()).ToList(); }
        }

        /// <summary>
        /// Rend Enable le nombre de decimal
        /// </summary>
        public bool EnableNbDecimal
        {
            get 
            { 
                if(this.SelectedEntity != null)
                    return this.SelectedEntity.IsRealType && IsEditMode; 
                else
                    return false;
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public MesUnitesViewModel(): base()
        {
            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.Unites_ExpanderTitle));
                    EventAggregator.Publish("MesUnites_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };

            this.OnDetailLoaded += (o, e) =>
            {
                SelectedEntity.PropertyChanged += (oo, ee) =>
                {
                    if (ee.PropertyName == "TypeDonnee" && !SelectedEntity.IsRealType)
                    {
                        this.SelectedEntity.NombreDeDecimales = 0;
                        RaisePropertyChanged(() => this.SelectedEntity);
                    }
                    
                    RaisePropertyChanged(() => EnableNbDecimal);
                };
                RaisePropertyChanged(() => EnableNbDecimal);
            };
            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.RefEnumValeurList);
            };
            this.OnAddedEntity += (o, e) =>
            {
                if (this.RefEnumValeurList != null && this.RefEnumValeurList.Count > 0)
                {
                    this.SelectedEntity.TypeDonnee = RefEnumValeurList[0].CleEnumValeur;
                    RaisePropertyChanged(() => this.SelectedEntity);
                }

                SelectedEntity.PropertyChanged += (oo, ee) =>
                {
                    if (ee.PropertyName == "TypeDonnee" && !SelectedEntity.IsRealType)
                    {
                        this.SelectedEntity.NombreDeDecimales = 0;
                        RaisePropertyChanged(() => this.SelectedEntity);
                    }

                    RaisePropertyChanged(() => EnableNbDecimal);
                };
                RaisePropertyChanged(() => EnableNbDecimal);
            };
            this.OnSaveSuccess += (o, e) =>
            {
                //On recharge les données pour réaliser le tri sur les unites
                this.service.GetEntities(ReloadData);
            };

            this.OnViewModeChanged += (o, e) =>
            {
                RaisePropertyChanged(() => this.EnableNbDecimal);
            };
        }

        #endregion Constructor

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> serviceEnumValeur { get; set; }

        #endregion Services

        #region Override Functions

        /// <summary>
        /// Suppression d'une Unite de mesure
        /// </summary>
        protected override void Delete()
        {
            MessageBoxResult result = MessageBox.Show(Resource.MesUnite_DeleteConfirmation,
                Resource.MesUnites_DeleteUniteCaptionMsgError, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                if (this.SelectedEntity.MesModeleMesure.Count > 0)
                {
                    string msgError = string.Format(Resources.Resource.MesUnites_DeleteUniteMainMsgError, this.SelectedEntity.Libelle);
                    msgError += Environment.NewLine + Resources.Resource.MesUnites_DeleteUniteMsgError;
                    MessageBox.Show(msgError, Resources.Resource.MesUnites_DeleteUniteCaptionMsgError, MessageBoxButton.OK);
                }
                else
                {
                    base.Delete(false, true);
                }
            }
        }
        
        #endregion Override Functions

        #region Private Functions

        /// <summary>
        /// On recharge les données pour mettre à jour la lsite déroulante de recherche 
        /// ainsi que les URLs de navigation des les unités précédente et suivante
        /// </summary>
        /// <param name="error"></param>
        private void ReloadData(Exception error)
        {
            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => this.PreviousEntity);
            RaisePropertyChanged(() => this.PreviousUri);
            RaisePropertyChanged(() => this.NextEntity);
            RaisePropertyChanged(() => this.NextUri);
        }

        #endregion Private Functions      
    }
}