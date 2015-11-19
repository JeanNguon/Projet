using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for MesModeleMesure entity
    /// </summary>
    [ExportAsViewModel("MesModeleMesure")]
    public class MesModeleMesureViewModel : BaseProtecaEntityViewModel<MesModeleMesure>
    {
        #region Private Members

        /// <summary>
        /// Déclaration de la variable NiveauProtection
        /// </summary>
        private MesNiveauProtection _niveauProtection;
        
        private String _currentTypeEq;

        //private MesModeleMesure _monModeleMesure;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override ObservableCollection<MesModeleMesure> Entities
        {
	        get 
	        {
                if (this.service.Entities != null && this.service.Entities.Any() && CurrentNavigation.Current.Filtre != null)
                {
                    return new ObservableCollection<MesModeleMesure>(this.service.Entities.Where(m => m.TypeEquipement != null && m.TypeEquipement.CodeEquipement == CurrentNavigation.Current.Filtre.ToString()).OrderBy(m => m.NumeroOrdre).ThenBy(m => m.LibGenerique));
                }
                else
                {
                    return new ObservableCollection<MesModeleMesure>();
                }
	        }
        }

        /// <summary>
        /// Niveau de protection
        /// </summary>
        public MesNiveauProtection NiveauProtection
        {
            get
            {
                if (_niveauProtection == null)
                {
                    _niveauProtection = new MesNiveauProtection();
                }
                return _niveauProtection;
            }
            set
            {
                if (value != null)
                {
                    _niveauProtection = value;
                    RaisePropertyChanged(() => this.NiveauProtection);
                }
            }
        }

        /// <summary>
        /// Liste des unités
        /// </summary>
        public ObservableCollection<MesUnite> MesUnite
        {
            get { return serviceMesUnite.Entities; }
        }

        /// <summary>
        /// Liste des types d'évaluation
        /// </summary>
        public ObservableCollection<RefEnumValeur> TypeGraphiqueList
        {
            get
            {
                if (serviceEnumValeur.Entities.Count > 0)
                {
                    return new ObservableCollection<RefEnumValeur>(serviceEnumValeur.Entities.Where(ee => ee.CodeGroupe == RefEnumValeurCodeGroupeEnum.ENUM_TYPE_GRAPHIQUE.ToString()));
                }
                else
                {
                    return new ObservableCollection<RefEnumValeur>();
                }
            }
        }

        /// <summary>
        /// Liste des types d'évaluation
        /// </summary>
        public ObservableCollection<RefEnumValeur> TypeEvaluationList
        {
            get
            {
                if (serviceEnumValeur.Entities.Count > 0)
                {
                    return new ObservableCollection<RefEnumValeur>(serviceEnumValeur.Entities.Where(ee => ee.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_EVAL.ToString()
                                                                                                    && (ee.LibelleCourt == "EG" || ee.LibelleCourt == "ECD" || ee.LibelleCourt == "CF" || ee.LibelleCourt == "TLM"))
                                                                                                    );
                }
                else
                {
                    return new ObservableCollection<RefEnumValeur>();
                }
            }
        }

        /// <summary>
        /// Liste des Niveau de type
        /// </summary>
        public ObservableCollection<RefEnumValeur> NiveauTypeList
        {
            get
            {
                if (serviceEnumValeur.Entities.Count > 0)
                {
                    return new ObservableCollection<RefEnumValeur>(serviceEnumValeur.Entities.Where(ee => ee.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYP_MES_NIVEAU_TYPE.ToString()));
                }
                else
                {
                    return new ObservableCollection<RefEnumValeur>();
                }
            }
        }

        #endregion Properties

        #region Commandes

        /// <summary>
        /// Commande pour supprimer un type de mesure
        /// </summary>
        public IActionCommand DeleteTypeMesureCommand { get; private set; }

        /// <summary>
        /// Commande pour Ajouter un type de mesure
        /// </summary>
        public IActionCommand AddTypeMesureCommand { get; private set; }

        #endregion Commandes

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type MesUnite
        /// </summary>
        [Import]
        public IEntityService<MesUnite> serviceMesUnite { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> serviceEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type Type Equipement
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> serviceTypeEquipement { get; set; }


        #endregion Services

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public MesModeleMesureViewModel()
            : base()
        {
            this.DeleteTypeMesureCommand = new ActionCommand<object>(
                obj => DeleteTypeMesure(obj), obj => true);

            this.AddTypeMesureCommand = new ActionCommand<object>(
                obj => AddTypeMesure(), obj => true);

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.MesurePP_TitreExpender));
                    EventAggregator.Publish("MesModeleMesure_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }

                if (!e.ViewParameter.Any(r => r.Key == "IsTopContainerLoaded"))
                {
                    EventAggregator.Publish("CustomTopContainer".AsViewNavigationArgs().AddNamedParameter("HideContainer", false));
                    EventAggregator.Publish("TypeEquipement".AsViewNavigationArgs().AddNamedParameter("IsTopContainerLoaded", true));
                }

                if (!this.Entities.Any() && this._currentTypeEq != CurrentNavigation.Current.Filtre.ToString())
                {
                    this.SelectedEntity = null;
                }

                this._currentTypeEq = CurrentNavigation.Current.Filtre.ToString();

                RaisePropertyChanged(() => this.Entities);
            };

            this.OnDetailLoaded += (o, e) =>
            {
                var temp = this.SelectedEntity.MesNiveauProtection.Where(n => n.CleEquipement == null && n.ClePortion == null && n.ClePp == null);
                if (temp.Any())
                {
                    this.NiveauProtection = temp.First() as MesNiveauProtection;
                }
                else
                {
                    this.NiveauProtection = new MesNiveauProtection() { SeuilMaxi = null, SeuilMini = null };
                    this.SelectedEntity.MesNiveauProtection.Add(this.NiveauProtection);
                }
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                this.Filtres = new List<System.Linq.Expressions.Expression<Func<MesModeleMesure, bool>>>();

                initGeoPreferences();
                base.Find();

                // MAJ des listes
                RaisePropertyChanged(() => this.TypeEvaluationList);
                RaisePropertyChanged(() => this.NiveauTypeList);
                RaisePropertyChanged(() => this.TypeGraphiqueList);
                RaisePropertyChanged(() => this.MesUnite);
            };

            this.OnCanceled += (o, e) =>
            {
                if (this.SelectedEntity != null)
                {
                    var temp = this.SelectedEntity.MesNiveauProtection.Where(n => n.CleEquipement == null && n.ClePortion == null && n.ClePp == null);
                    if (temp.Count() > 0)
                    {
                        this.NiveauProtection = temp.First() as MesNiveauProtection;
                    }
                    else
                    {
                        this.NiveauProtection = new MesNiveauProtection() { SeuilMaxi = null, SeuilMini = null };
                    }
                }
            };

            this.OnSaveSuccess += (o, e) =>
            {
                RaisePropertyChanged(() => this.Entities);
                RaisePropertyChanged(() => this.PreviousEntity);
                RaisePropertyChanged(() => this.PreviousUri);
                RaisePropertyChanged(() => this.NextEntity);
                RaisePropertyChanged(() => this.NextUri);
            };
        }

        #endregion Constructor

        #region Private Functions

        /// <summary>
        /// Supprime un type de mesure associé au modèle de mesures
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteTypeMesure(object obj)
        {
            MessageBoxResult result = MessageBox.Show(Resource.MesTypeMesure_ValidationDeleteText,
                Resource.MesTypeMesure_ValidationCaption, MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                MesTypeMesure typeMesure = this.SelectedEntity.MesTypeMesure.FirstOrDefault(t => t.CleTypeMesure == int.Parse(obj.ToString()));
                if (typeMesure != null)
                {
                    ((MesModeleMesureService)service).CheckMesTypeMesureToDelete(typeMesure.CleTypeMesure, (retour, error) =>
                        {
                            if (retour || error != null)
                            {
                                MessageBox.Show(string.Format(Resource.MesTypeMesure_ValidationText, typeMesure.LibTypeMesure),
                                    Resource.MesTypeMesure_ValidationCaption, MessageBoxButton.OK);
                            }
                            else
                            {
                                ((MesModeleMesureService)service).DeleteMesTypeMesure(typeMesure);
                                this.SelectedEntity.MesTypeMesure.Remove(typeMesure);
                            }
                        }
                    );
                }
            }
        }

        /// <summary>
        /// Ajout un nouveau type de mesure
        /// </summary>
        private void AddTypeMesure()
        {
            this.SelectedEntity.MesTypeMesure.Add(new MesTypeMesure());
        }

        /// <summary>
        /// Autorise les droits suppression / ajout / etc.. suivant les droits utilisateurs
        /// Suite à la modification de la matrice des droits => ce droit est universel
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            //if (this.CurrentUser != null)
            //{
            //    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.RESTIT_MESURES);
            //    return role.RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue();
            //}
            //else
            //{
            //    return false;
            //}
            return true;
        }

        #endregion Private Functions

        #region Override

        protected override void DeactivateView(string viewName)
        {
            base.DeactivateView(viewName);
            _currentTypeEq = String.Empty;
        }

        /// <summary>
        /// Ajout d'un nouveau modele de mesure
        /// </summary>
        protected override void Add()
        {
            base.Add();
            this.NiveauProtection = new MesNiveauProtection() { SeuilMaxi = null, SeuilMini = null };
            this.SelectedEntity.MesNiveauProtection.Add(this.NiveauProtection);
            this.SelectedEntity.TypeEquipement = serviceTypeEquipement.Entities.FirstOrDefault(t => t.CodeEquipement == CurrentNavigation.Current.Filtre.ToString());

            // Géré de la même manière que les autres champs obligatoires sans initialisation
            //// initialisation du numéro d'ordre
            //// pour que l'écran soit valide, il doit avoir une valeur > 0
            //// il doit donc être saisi explicitement par l'utilisateur (pas possible de garder la valeur par défaut)
            //this.SelectedEntity.NumeroOrdre = -1;
            //this.SelectedEntity.ValidationErrors.Clear();
        }

        /// <summary>
        /// Surcharge de la suppression test + Suppression en cascade des MesNiveauProtection
        /// </summary>
        protected override void Delete()
        {
            MessageBoxResult result = MessageBox.Show(Resource.MesModeleMesure_DeleteConfirmation,
                Resource.MesModeleMesure_ValidationCaption, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                if (this.SelectedEntity.MesTypeMesure != null && this.SelectedEntity.MesTypeMesure.Count > 0)
                {
                    MessageBox.Show(Resource.MesModeleMesure_ValidationText, Resource.MesModeleMesure_ValidationCaption, MessageBoxButton.OK);
                }
                else
                {
                    base.Delete(false, true);
                }
            }
        }

        #endregion Override

    }
}
