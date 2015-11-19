using System;
using Jounce.Core.ViewModel;
using Proteca.Silverlight.Services;
using System.Collections.ObjectModel;
using Microsoft.SharePoint.Client;
using Proteca.Silverlight.Enums;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using Jounce.Framework;
using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Models;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Web.Models;
using Jounce.Framework.Command;
using System.Windows;


namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// Gestion des types de document
    /// </summary>
    [ExportAsViewModel("TypeDocument")]
    public class TypeDocumentViewModel : BaseProtecaEntityViewModel<TypeDocument>
    {
        #region Properties

        private bool _isContextMenuOpen;
        public bool IsContextMenuOpen
        {
            get
            {
                return _isContextMenuOpen;
            }
            set
            {
                _isContextMenuOpen = value;
                this.RaisePropertyChanged(() => this.IsContextMenuOpen);
            }
        }

        public bool IsDesignation
        {
            get
            {
                return this.SelectedEntity != null ? this.SelectedEntity.Niveau == 3 : false;
            }
        }

        public bool IsDossier
        {
            get
            {
                return this.SelectedEntity != null ? this.SelectedEntity.Niveau == 2 : false;
            }
        }

        public bool IsOuvrage
        {
            get
            {
                return this.SelectedEntity != null ? this.SelectedEntity.Niveau == 1 : true;
            }
        }

        public bool IsDossierOrDesignation
        {
            get
            {
                return this.IsDesignation || this.IsDossier;
            }
        }

        public bool IsDeleteDisplayed
        {
            get
            {
                return !IsEditMode && (this.IsDesignation || (this.IsDossier && !this.SelectedEntity.Entities.Any(e => !e.IsDeleted)));
            }
        }

        public bool IsEditDisplayed
        {
            get
            {
                return this.IsDesignation || this.IsDossier;
            }
        }

        public bool IsAddDisplayed
        {
            get
            {
                return !IsEditMode && (this.IsDossier || this.IsOuvrage);
            }
        }

        public string LibelleAjout
        {
            get
            {
                return this.IsOuvrage ? Resource.TypeDocument_BtnAjouterTypeDocument : Resource.TypeDocument_BtnAjouterDesignation;
            }
        }

        private TypeDocument _selectedItem;
        public TypeDocument SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value && value != null && _selectedItem != null)
                {
                    NavigationService.NavigateUri(new Uri(string.Format("/{0}/{1}/{2}={3}",
                        MainNavigation.Parametres.GetStringValue(),
                        ParametresNavigation.TypeDocument.GetStringValue(),
                        Global.Constants.PARM_ID,
                        ((TypeDocument)value).Cle), UriKind.Relative));
                }
                _selectedItem = value;

                if (_selectedItem != null && _selectedItem.Niveau == 1)
                {
                    _selectedItem.IsExpanded = true;
                }
                else if (_selectedItem != null && _selectedItem.Niveau == 2 && _selectedItem.Parent != null)
                {
                    _selectedItem.IsExpanded = true;
                    _selectedItem.Parent.IsExpanded = true;
                }
                else if (_selectedItem != null && _selectedItem.Niveau == 3 && _selectedItem.Parent != null && _selectedItem.Parent.Parent != null)
                {
                    _selectedItem.IsExpanded = true;
                    _selectedItem.Parent.IsExpanded = true;
                    _selectedItem.Parent.Parent.IsExpanded = true;
                }

                this.SelectedEntity = _selectedItem;
                this.RaisePropertyChanged(() => this.SelectedItem);
                this.AddCommand.RaiseCanExecuteChanged();
                this.DeleteCommand.RaiseCanExecuteChanged();
                this.EditCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged(() => this.IsDesignation);
                this.RaisePropertyChanged(() => this.IsDossier);
                this.RaisePropertyChanged(() => this.IsOuvrage);
                this.RaisePropertyChanged(() => this.IsDeleteDisplayed);
                this.RaisePropertyChanged(() => this.IsEditDisplayed);
                this.RaisePropertyChanged(() => this.IsAddDisplayed);
                this.RaisePropertyChanged(() => this.LibelleAjout);
                this.RaisePropertyChanged(() => this.IsDossierOrDesignation);
            }
        }

        public List<TypeDocument> TypeDocuments
        {
            get
            {
                List<TypeDocument> result = new List<TypeDocument>();
                if (this.Entities != null && this.Entities.Any())
                {
                    result = this.Entities.Where(e => e.IsDeleted == false).OrderBy(e => e.Libelle).ToList();
                }
                return result;
            }
        }

        #endregion

        #region Constructor

        public TypeDocumentViewModel()
        {
            IsAutoNavigateToFirst = true;

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Sélection arborescence documentaire").AddNamedParameter<Double>("MaxWidth", 250));
                    EventAggregator.Publish("TypeDocument_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }

            };

            this.OnEntitiesLoaded += (o, e) =>
            {
                this.RaisePropertyChanged(() => this.TypeDocuments);
                if (this.SelectedId == null)
                {
                    this.RaisePropertyChanged(() => this.TypeDocuments);
                }
                this.Entities.CollectionChanged += (oo, ee) =>
                {
                    this.RaisePropertyChanged(() => this.TypeDocuments);
                };
            };

            this.OnDetailLoaded += (o, e) =>
            {
                this.RaisePropertyChanged(() => this.IsDesignation);
                this.RaisePropertyChanged(() => this.IsDossier);
                this.RaisePropertyChanged(() => this.IsOuvrage);
                if (SelectedEntity != null && SelectedEntity.Cle > 0)
                {
                    selecteItem(this.SelectedEntity.Cle, this.TypeDocuments);
                    this.RaisePropertyChanged(() => this.IsDeleteDisplayed);
                    this.RaisePropertyChanged(() => this.IsEditDisplayed);
                    this.RaisePropertyChanged(() => this.IsAddDisplayed);
                    this.RaisePropertyChanged(() => this.LibelleAjout);
                    this.RaisePropertyChanged(() => this.IsDossierOrDesignation);
                    this.AddCommand.RaiseCanExecuteChanged();
                    this.DeleteCommand.RaiseCanExecuteChanged();
                    this.EditCommand.RaiseCanExecuteChanged();
                    this.SelectedEntity.ValidationErrors.Clear();
                }
                this.RaisePropertyChanged(() => this.TypeDocuments);
            };

            this.OnCanceled += (o, e) =>
            {
                this.RaisePropertyChanged(() => this.TypeDocuments);
                this.LoadDetailEntity();
            };

            this.OnSaveSuccess += (o, e) =>
            {
                this.SelectedEntity.ValidationErrors.Clear();
                if (IsAutoNavigateToFirst && this.SelectedEntity.IsNewEntity && this.SelectedEntity.GetIdentity() != null)
                {
                    this.SelectedEntity.IsNewEntity = false;
                    NavigationService.Navigate((int)this.SelectedEntity.GetIdentity());
                }
                this.SelectedEntity.LibelleOriginal = this.SelectedEntity.Libelle;
                this.RaisePropertyChanged(() => this.IsDeleteDisplayed);
                this.RaisePropertyChanged(() => IsAddDisplayed); ;
                this.AddCommand.RaiseCanExecuteChanged();
                this.DeleteCommand.RaiseCanExecuteChanged();
                this.EditCommand.RaiseCanExecuteChanged();
            };

            this.OnSaveError += (o, e) =>
            {
                this.SelectedEntity.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(e.Error.Message, new List<string>() { "Libelle" }));
            };

            this.OnDeleteSuccess += (o, e) =>
            {
                this.RaisePropertyChanged(() => this.IsDeleteDisplayed);
                this.AddCommand.RaiseCanExecuteChanged();
                this.DeleteCommand.RaiseCanExecuteChanged();
                this.EditCommand.RaiseCanExecuteChanged();
            };

            this.OnDeleteError += (o, e) =>
            {
                MessageBox.Show(e.Error.Message, "", MessageBoxButton.OK);
                SelectedEntity.IsDeleted = false;
            };
        }

        #endregion

        #region Methods
        /// <summary>
        /// Annuler les modifications en cours
        /// </summary>
        protected override void Cancel()
        {
            this.IsEditMode = false;
            if (this.SelectedEntity.IsNewEntity)
            {
                var parent = this.SelectedItem.Parent;
                parent.Entities.Remove(this.SelectedEntity);
                this._selectedItem = parent;
                this.SelectedEntity = parent;
                this.service.DetailEntity = parent;
            }
            else
            {
                this.SelectedEntity.Libelle = this.SelectedEntity.LibelleOriginal;
            }
            RaisePropertyChanged(() => SelectedEntity);
            RaisePropertyChanged(() => this.TypeDocuments);

            if (OnCanceled != null)
            {
                OnCanceled(this, null);
            }

            NotifyError = false;
        }

        /// <summary>
        /// Edition d'un type de document
        /// </summary>
        protected override void Edit()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (!UserCanEdit)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else
            {
                this.IsEditMode = true;
                this.RaisePropertyChanged(() => this.IsDeleteDisplayed);
                this.RaisePropertyChanged(() => this.IsAddDisplayed);
            }
        }

        /// <summary>
        /// Ajout d'un type de document
        /// </summary>
        protected override void Add()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (!UserCanAdd)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else
            {
                TypeDocument doc = this.SelectedEntity;
                doc.IsExpanded = true;
                TypeDocument newDoc = new TypeDocument(getMaxCle(0, this.TypeDocuments) + 1, string.Empty, string.Empty);
                newDoc.Parent = doc;
                newDoc.IsNewEntity = true;
                this.SelectedEntity = newDoc;
                if (doc != null)
                {
                    newDoc.ServerRelativeUrl = doc.ServerRelativeUrl + "/" + doc.Libelle;
                    doc.Entities.Add(newDoc);
                    this.RaisePropertyChanged(() => this.TypeDocuments);
                    this.RaisePropertyChanged(() => this.IsOuvrage);
                    this.RaisePropertyChanged(() => this.IsDossier);
                    this.RaisePropertyChanged(() => this.IsDesignation);
                    this.RaisePropertyChanged(() => this.IsDossierOrDesignation);
                    this.RaisePropertyChanged(() => this.SelectedEntity);
                }
                else
                {
                    newDoc.ServerRelativeUrl = ((TypeDocumentService)service).DefaultFolder;
                    this.Entities.Add(newDoc);
                }
                selecteItem(newDoc.Cle, this.TypeDocuments);
                this.service.DetailEntity = newDoc;
                this.IsEditMode = true;
            }

            this.RaisePropertyChanged(() => this.TypeDocuments);
            this.RaisePropertyChanged(() => this.IsDeleteDisplayed);
            this.RaisePropertyChanged(() => this.IsAddDisplayed);
        }

        private void selecteItem(int cle, List<TypeDocument> list)
        {
            var doc = list.FirstOrDefault(d => d.Cle == cle);
            if (doc == null)
            {
                foreach (var document in list)
                {
                    selecteItem(cle, document.Entities);
                }
            }
            else
            {
                _selectedItem = doc;
                this.RaisePropertyChanged(() => this.SelectedItem);
            }
        }

        /// <summary>
        /// Récupère la clé ayant la valeur la plus grande
        /// </summary>
        /// <param name="maxCle"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private int getMaxCle(int maxCle, List<TypeDocument> list)
        {
            if (list != null && list.Any())
            {
                int cle = list.Max(d => d.Cle);
                if (cle > maxCle)
                {
                    maxCle = cle;
                }
                foreach (var doc in list)
                {
                    if (doc.Entities != null && doc.Entities.Any())
                    {
                        cle = getMaxCle(maxCle, doc.Entities);
                        if (cle > maxCle)
                        {
                            maxCle = cle;
                        }
                    }
                }
            }
            return maxCle;
        }

        #region Gestion des droits
        protected override bool GetUserCanEdit()
        {
            bool res = false;
            if (this.SelectedEntity != null)
            {
                res = this.SelectedEntity.Niveau >= 2;
            }
            if (this.CurrentUser != null)
            {
                if (this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GESTION_DOC).RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    res &= false;
                }
            }
            return res && !IsEditMode;
        }

        protected override bool GetUserCanAdd()
        {
            bool res = false;
            if (this.SelectedEntity != null)
            {
                res = this.SelectedEntity.Niveau >= 1;
            }
            if (this.CurrentUser != null)
            {
                if (this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GESTION_DOC).RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    res &= false;
                }
            }
            return res && !IsEditMode;
        }

        protected override bool GetUserCanDelete()
        {
            bool res = false;
            if (this.SelectedEntity != null)
            {
                res = this.SelectedEntity.Niveau >= 2 && !this.SelectedEntity.Entities.Any(e => !e.IsDeleted);
            }
            if (this.CurrentUser != null)
            {
                if (this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.SUP_DOC).RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    res &= false;
                }
            }
            return res && !IsEditMode;
        }
        #endregion
        #endregion
    }
}
