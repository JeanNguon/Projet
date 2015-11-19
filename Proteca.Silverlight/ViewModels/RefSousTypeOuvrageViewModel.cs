using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework.Command;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Silverlight.Views.Windows;
using Proteca.Web.Models;
using Proteca.Silverlight.Resources;
using System.Collections.ObjectModel;
using Proteca.Silverlight.Enums;

namespace Proteca.Silverlight.ViewModels
{
    [ExportAsViewModel("RefSousTypeOuvrage")]
    public class RefSousTypeOuvrageViewModel : BaseProtecaEntityViewModel<RefSousTypeOuvrage>
    {
        #region Properties

        /// <summary>
        /// Liste des sous types d'ouvrage
        /// </summary>
        public ObservableCollection<RefSousTypeOuvrage> SousTypeOuvrageList
        {
            get
            {
                if (!string.IsNullOrEmpty(CodeGroupeTypeOuvrage))
                {
                    if (DisplayNumOrdre)
                        return new ObservableCollection<RefSousTypeOuvrage>(
                            ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == CodeGroupeTypeOuvrage)
                           .OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle).ToList());
                    else
                        return new ObservableCollection<RefSousTypeOuvrage>(
                            ServiceRefSousTypeOuvrage.Entities.Where(r => r.CodeGroupe == CodeGroupeTypeOuvrage)
                           .OrderBy(r => r.Libelle).ThenBy(r => r.NumeroOrdre).ToList());
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Nom du groupe sur lequel on doit filtrer
        /// </summary>
        public string CodeGroupeTypeOuvrage { get; set; }

        /// <summary>
        /// Le texte spécifique du nombre d'éléments du tableau
        /// </summary>
        public string NbElementsText
        {
            get { return Resource.ResourceManager.GetString(string.Format("RefSousTypeOuvrage_Nb{0}", CodeGroupeTypeOuvrage)); }
        }

        /// <summary>
        /// Affiche la colonne numéro d'ordre
        /// </summary>
        public bool DisplayNumOrdre { get; set; }

        private RefSousTypeOuvrage ElementToDelete { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Commande de suppression d'un sous type d'ouvrage
        /// </summary>
        public IActionCommand DeleteSousTypeCommand { get; set; }

        /// <summary>
        /// Déclaration de l'événement d'une modification d'un item
        /// </summary>
        public IActionCommand SelectedCellChangedCommand { get; private set; }

        #endregion Commands

        #region Services

        /// <summary>
        /// Service utilisé pour gérer les entités de type RefSousTypeOuvrage
        /// </summary>
        [Import]
        public IEntityService<RefSousTypeOuvrage> ServiceRefSousTypeOuvrage { get; set; }

        [Import]
        public ChildWindowControl Childwindow;

        #endregion Services

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public RefSousTypeOuvrageViewModel()
            : base()
        {
            IsAutoNavigateToFirst = false;

            DeleteSousTypeCommand = new ActionCommand<object>(
                obj => DeleteSousTypeOuvrage(obj), obj => true);

            SelectedCellChangedCommand = new ActionCommand<object>(
                obj => RaisePropertyChanged(() => this.SousTypeOuvrageList), obj => true);

            this.OnSaveSuccess += (o, e) =>
            {
                IsEditMode = true;
                Childwindow.DialogResult = true;
            };

            this.OnCanceled += (o, e) =>
            {
                IsEditMode = true;
            };

            this.OnAddedEntity += (o, e) =>
            {
                this.SelectedEntity.CodeGroupe = this.CodeGroupeTypeOuvrage;

                if (!this.DisplayNumOrdre)
                    this.SelectedEntity.NumeroOrdre = 0;

                this.SelectedEntity.Valeur = string.Empty;
                RaisePropertyChanged(() => this.SousTypeOuvrageList);
            };

            this.OnCanceled += (o, e) =>
            {
                Childwindow.DialogResult = true;
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Suppression du sous type d'ouvrage sélectionné
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteSousTypeOuvrage(object obj)
        {
            int cle;
            if (obj != null && int.TryParse(obj.ToString(), out cle))
            {
                if (cle > 0)
                {
                    MessageBoxResult result = MessageBox.Show(Resource.ResourceManager.GetString(string.Format("RefSousTypeOuvrage_Delete{0}CaptionMsg", CodeGroupeTypeOuvrage)),
                        Resource.ResourceManager.GetString(string.Format("RefSousTypeOuvrage_Delete{0}Confirmation", CodeGroupeTypeOuvrage)), MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        ElementToDelete = this.Entities.FirstOrDefault(r => r.CleSousTypeOuvrage == cle);
                        ((RefSousTypeOuvrageService)ServiceRefSousTypeOuvrage).CheckCanDeleteRefSousTypeOuvrage(this.CodeGroupeTypeOuvrage, cle, CheckCanDeleteRefSousTypeOuvrageDone);
                        Logger.Log(LogSeverity.Information, GetType().FullName, string.Format("Suppression de l'objet RefSousTypeOuvrage ayant l'id : {0}", cle));
                    }
                }
                else
                {
                    this.service.Delete(this.SelectedEntity);
                    this.Entities.Remove(this.SelectedEntity);
                    this.SelectedEntity = null;
                    RaisePropertyChanged(() => this.SousTypeOuvrageList);
                    Logger.Log(LogSeverity.Information, GetType().FullName, "L'utilisateur tente de supprimer l'objet RefSousTypeOuvrage en cours de création");
                }
            }
            else
            {
                Logger.Log(LogSeverity.Warning, GetType().FullName, string.Format("Impossible de supprimer l'objet RefSousTypeOuvrage. L'id récupéré a la valeur : {0}", obj));
            }
        }

        /// <summary>
        /// Les entités viennent d'être chargées
        /// </summary>
        /// <param name="error"></param>
        private void ServiceRefSousTypeOuvrageLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(RefSousTypeOuvrage).Name));
            }
            else
            {
                RaisePropertyChanged(() => this.SousTypeOuvrageList);
            }
            RaiseOnAllServicesLoadedEvent();
        }

        /// <summary>
        /// Retour de la méthode qui vérifie si l'entité peut être supprimée, si c'est le cas alors elle est supprimé.
        /// </summary>
        /// <param name="error">L'erreur qui a pu se produire durant la suppression</param>
        /// <param name="isDeleted">True si la suppression a été effectué, False sinon</param>
        private void CheckCanDeleteRefSousTypeOuvrageDone(Exception error, bool isDeleted)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                MessageBox.Show(Resource.ResourceManager.GetString(string.Format("RefSousTypeOuvrage_Delete{0}CaptionMsgError", CodeGroupeTypeOuvrage)),
                    Resource.ResourceManager.GetString(string.Format("RefSousTypeOuvrage_Delete{0}Confirmation", CodeGroupeTypeOuvrage)), MessageBoxButton.OKCancel);

            }
            else if (!isDeleted)
            {
                //Aucun equipement ne doit utilisé ce sous type d'ouvrage pour pouvoir le supprimer
                MessageBox.Show(Resource.RefSousTypeOuvrageViewModelTextMsgError, Resource.RefSousTypeOuvrageViewModelCaptionMsgError, MessageBoxButton.OK);
            }
            else if (ElementToDelete != null)
            {
                service.Delete(ElementToDelete);
                MessageBox.Show(Resource.BaseProtecaEntityViewModel_DeleteSuccess, string.Empty, MessageBoxButton.OK);
                RaisePropertyChanged(() => this.SousTypeOuvrageList);
            }
        }

        #endregion

        #region Override Functions

        /// <summary>
        /// On fixe le Code du groupe du type ouvrage pour filtrer les résultats
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            if (viewParameters.ContainsKey("RefSousTypeOuvrageGroupEnum") && viewParameters.ContainsKey("DisplayNumOrdre"))
            {
                this.CodeGroupeTypeOuvrage = viewParameters["RefSousTypeOuvrageGroupEnum"].ToString();
                if (viewParameters["DisplayNumOrdre"] != null)
                {
                    this.DisplayNumOrdre = (bool)viewParameters["DisplayNumOrdre"];
                    RaisePropertyChanged(() => this.DisplayNumOrdre);
                }
            }
            RaisePropertyChanged(() => this.NbElementsText);
            this.ServiceRefSousTypeOuvrage.GetEntities((err) => ServiceRefSousTypeOuvrageLoaded(err));
        }

        /// <summary>
        /// On passe en mode edition pour pouvoir enregistrer les modifications
        /// </summary>
        protected override void Save()
        {
            this.SousTypeOuvrageList.OrderBy(c => c.Libelle);
            if (this.CodeGroupeTypeOuvrage == RefSousTypeOuvrageGroupEnum.TypeNomTiers.GetStringValue())
            {
                int i = this.SousTypeOuvrageList.Max(c => c.NumeroOrdre);
                foreach (RefSousTypeOuvrage st in this.SousTypeOuvrageList.Where(c => c.IsNew == true))
                {
                    i++;
                    st.NumeroOrdreNullable = i;
                }
            }
            IsEditMode = true;
            base.Save();
        }

        #endregion Override Functions

        #region

        protected override bool GetUserCanAdd()
        {
            return GetAutorisation();
        }

        protected override bool GetUserCanDelete()
        {
            return GetAutorisation();
        }

        protected override bool GetUserCanEdit()
        {
            return GetAutorisation();
        }

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
            // Seules les personnes autorisées à cet écran y ont accès donc droits ok par défault
            return true;
        }

        #endregion

    }
}
