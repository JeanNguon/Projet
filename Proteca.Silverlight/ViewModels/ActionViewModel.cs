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
using Proteca.Silverlight.Enums;
using Jounce.Core.Command;
using System.Windows;
using Proteca.Silverlight.Resources;
using Jounce.Framework.Command;
using Proteca.Silverlight.Services.EntityServices;
using Jounce.Core.Application;
using Proteca.Silverlight.Services.Contracts;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Action entity
    /// </summary>
    [ExportAsViewModel("Action")]
    public class ActionViewModel : BaseProtecaEntityViewModel<RefEnumValeur>
    {
        #region Private Members

        /// <summary>
        /// Liste des filtres enum valeur
        /// </summary>
        private string enumACTION_CATEGORIE_ANOMALIE = RefEnumValeurCodeGroupeEnum.ACTION_CATEGORIE_ANOMALIE.GetStringValue();
        private string enumACTION_TYPE = RefEnumValeurCodeGroupeEnum.ACTION_TYPE.GetStringValue();
        private string enumACTION_CATEGORIE_ANOMALIE_ToString = RefEnumValeurCodeGroupeEnum.ACTION_CATEGORIE_ANOMALIE_TOSTRING.GetStringValue();
        private string enumACTION_TYPE_ToString = RefEnumValeurCodeGroupeEnum.ACTION_TYPE_TOSTRING.GetStringValue();


        /// <summary>
        /// Propriété de la liste des filtres
        /// </summary>
        private Dictionary<string,bool> _searchList;

        /// <summary>
        /// Propriété du filtre pour le code groupe
        /// </summary>
        private string _filtreCodeGroupe;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// Retourne les liste des statuts du service ListStatuts
        /// </summary>
        public List<RefEnumValeur> ListStatuts
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumStatuts).OrderBy(r => r.NumeroOrdre).ToList();
                }
                return null;
            }
        }
                   
        /// <summary>
        /// Retourne la liste des filtres
        /// </summary>
        public Dictionary<string, bool> SearchList
        {
            get
            {
                if (_searchList == null)
                {
                    // Initialisation de la liste de recherche
                    _searchList = new Dictionary<string,bool>();
                    _searchList.Add(enumACTION_CATEGORIE_ANOMALIE_ToString,true);
                    _searchList.Add(enumACTION_TYPE_ToString,false);
                    RaisePropertyChanged(() => this.SearchList);
                }
                return _searchList;
            }
        }

        /// <summary>
        /// Retourne le filtre sélectionné
        /// </summary>
        public string FiltreCodeGroupe
        {
            get
            {
                if (_filtreCodeGroupe == null)
                {
                    // MAJ du filtre sur le premier élément
                    _filtreCodeGroupe = this.SearchList.FirstOrDefault().Key;
                }
                return _filtreCodeGroupe;
            }
            set
            {
                _filtreCodeGroupe = value;
                RaisePropertyChanged(() => this.FiltreCodeGroupe);
                RaisePropertyChanged(() => this.ListActions);
            }
        }

        /// <summary>
        /// Retourne la liste des action categorie anomalie
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListActions
        {
            get
            {
                if (this.Entities != null && this.FiltreCodeGroupe != null)
                {
                    if (FiltreCodeGroupe == enumACTION_CATEGORIE_ANOMALIE_ToString)
                    {
                        return new ObservableCollection<RefEnumValeur>(this.Entities.Where(s => s.CodeGroupe == enumACTION_CATEGORIE_ANOMALIE).OrderBy(s => s.Libelle));
                    }
                    else if (FiltreCodeGroupe == enumACTION_TYPE_ToString)
                    {
                        return new ObservableCollection<RefEnumValeur>(this.Entities.Where(s => s.CodeGroupe == enumACTION_TYPE).OrderBy(s => s.Libelle));
                    }
                    return null;
                }
                return null;
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Déclaration de l'énum permettant d'afficher les types de statuts en base
        /// </summary>
        private string enumStatuts = RefEnumValeurCodeGroupeEnum.ENUM_STATUT_ACTION.GetStringValue();

        #endregion

        #region Constructor

        public ActionViewModel()
            : base()
        {
            IsAutoNavigateToFirst = false;

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    //EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Sélection du type de liste"));
                    //EventAggregator.Publish("Action_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                    EventAggregator.Publish("CustomTopContainer".AsViewNavigationArgs().AddNamedParameter("HideContainer", true));
                    //EventAggregator.Publish("TypeRessource".AsViewNavigationArgs().AddNamedParameter("IsTopContainerLoaded", true));
                   // EventAggregator.Publish("Action_Expander".AsViewNavigationArgs().AddNamedParameter("HideContainer", true));
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("HideExpander", true));
                }
            };

            this.OnEntitiesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.ListActions);

            };

            this.OnSaveSuccess += (o, e) =>
            {
                this.RaisePropertyChanged(() => this.ListActions);
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
        /// Déclaration de la variable de sélection du filtre
        /// </summary>
        public IActionCommand CheckCommand { get; private set; }
        #endregion

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceRefEnumValeur { get; set; }

        #endregion

        #region Override Methods

        /// <summary>
        /// Fonction d'ajout de ligne dans le tableau
        /// </summary>
        protected override void Add()
        {
            base.Add();
            
            if (FiltreCodeGroupe == enumACTION_CATEGORIE_ANOMALIE_ToString)
            {
                this.SelectedEntity.CodeGroupe = enumACTION_CATEGORIE_ANOMALIE;
            }
            else if (FiltreCodeGroupe == enumACTION_TYPE_ToString)
            {
                this.SelectedEntity.CodeGroupe = enumACTION_TYPE;
            }

            this.SelectedEntity.NumeroOrdre = 0;
            this.SelectedEntity.Valeur = "0";

            RaisePropertyChanged(() => this.ListActions);
        }

        /// <summary>
        /// MAJ de l'écran à l'annulation
        /// </summary>
        protected override void Cancel()
        {
            base.Cancel();
            LoadEntities();
            RaisePropertyChanged(() => this.ListActions);
        }



        #endregion

        #region Private Methods

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        protected virtual void DeleteLine(object Obj)
        {
            String msg = Resource.ActionType_DeleteConfirmation;
            if (FiltreCodeGroupe == enumACTION_CATEGORIE_ANOMALIE_ToString)
            {
                msg = Resource.ActionCategorie_DeleteConfirmation;
            } 
            
            var result = MessageBox.Show(msg, string.Empty, MessageBoxButton.OKCancel);
            
            if (result == MessageBoxResult.OK)
            {   
                if (FiltreCodeGroupe == enumACTION_CATEGORIE_ANOMALIE_ToString)
                {
                    ((RefEnumValeurService)service).CheckCanDeleteCategorieAnomalie(((RefEnumValeur)Obj).CleEnumValeur, (error, retour)=>
                        {
                            if (error != null)
	                        {
		                        Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                                ErrorWindow.CreateNew(Resource.DefaultErrorOnDelete);  
	                        }
                            else if (retour)
	                        {	
	                            this.service.Delete((RefEnumValeur)Obj);
                                this.Entities.Remove((RefEnumValeur)Obj);
                                RaisePropertyChanged(() => this.ListActions);
	                        }
                            else
	                        {
                                MessageBox.Show(Resource.CategorieAnomalie_ErrorOnDelete, "", MessageBoxButton.OK);
	                        }
                        }
                    );
                }
                else
                {
                    ((RefEnumValeurService)service).CheckCanDeleteTypeAction(((RefEnumValeur)Obj).CleEnumValeur, (error, retour)=>
                        {
                            if (error != null)
	                        {
		                        Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                                ErrorWindow.CreateNew(Resource.DefaultErrorOnDelete);  
	                        }
                            else if (retour)
	                        {	
	                            this.service.Delete((RefEnumValeur)Obj);
                                this.Entities.Remove((RefEnumValeur)Obj);
                                RaisePropertyChanged(() => this.ListActions);
	                        }
                            else
	                        {
                                MessageBox.Show(Resource.TypeAction_ErrorOnDelete, "", MessageBoxButton.OK);
	                        }
                        }
                    );
                }              
            };
        }

        #endregion

        #region public methods
        /// <summary>
        /// Fonction de MAJ du tableau en fonction du niveau de sensibilité
        /// </summary>
        protected virtual void CheckFiltre(object Obj)
        {
            FiltreCodeGroupe = Obj.ToString(); ;
            RaisePropertyChanged(() => this.ListActions);
        }
        #endregion

        #region Autorisations

        /// <summary>
        /// Gestion des autorisations. L'utilisateur ne peut pas éditer s'il n'a pas une autorisation d'affectation différente d'"interdite"
        /// </summary>
        /// <returns></returns>
        protected override bool GetUserCanEdit()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droist de l'utilisateur courant 
        /// sur la suppression d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanDelete()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur la sauvegarde d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanSave()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            return this.CurrentUser != null && this.CurrentUser.IsAdministrateur;
        }

        #endregion Autorisations
    }
}
