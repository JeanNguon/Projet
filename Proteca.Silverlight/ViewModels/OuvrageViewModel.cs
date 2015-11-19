using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Models;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using Telerik.Windows.Controls;
using Proteca.Silverlight.Services.EntityServices;
using System.Collections;
using System.Windows.Browser;
using System.IO;
using Telerik.Windows.Media.Imaging;
using System.Windows.Media.Imaging;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel de base pour gérer les entités ouvrage (equipement, ensemble electrique, portion intégrité, pp)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OuvrageViewModel<T> : BaseOuvrageViewModel<T> where T : Entity
    {

        #region Commands

        /// <summary>
        /// Déclaration de l'objet de command d'upload d'une image.
        /// </summary>
        public IActionCommand UploadImageCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression d'une image.
        /// </summary>
        public IActionCommand DeleteImageCommand { get; private set; }


        /// <summary>
        /// Déclaration de l'objet de command d'impression des équipements.
        /// </summary>
        public IActionCommand PrintEQCommand { get; private set; }

        #endregion

        #region Constructor

        public OuvrageViewModel()
            : base()
        {
            // Define commands
            AddDocumentCommand = new ActionCommand<object>(
                obj => AddDocument(), obj => IsEditMode && GetUserCanAddDocument());
            EditDocumentCommand = new ActionCommand<object>(
                obj => Edit(), obj => !IsEditMode && GetUserCanEditDocument());
            DeleteDocumentCommand = new ActionCommand<object>(
                obj => DeleteDocument(obj), obj => IsEditMode && GetUserCanDeleteDocument());
            UploadDocumentCommand = new ActionCommand<object>(
                obj => UploadDocument(obj), obj => IsEditMode && GetUserCanAddDocument());
            ArchiveDocumentCommand = new ActionCommand<object>(
                obj => ArchiveDocument(obj, true), obj => IsEditMode && GetUserCanAddDocument());
            RestaureDocumentCommand = new ActionCommand<object>(
                obj => ArchiveDocument(obj, false), obj => IsEditMode && GetUserCanAddDocument());
            SaveDocumentCommand = new ActionCommand<object>(
                obj => SaveDocument(), obj => IsEditMode);
            CancelDocumentCommand = new ActionCommand<object>(
               obj => CancelDocument(), obj => IsEditMode);
            SaveHistorisationCommand = new ActionCommand<object>(
               obj => HistoriserOuvrage(), obj => CanSave);
            HistoViewCommand = new ActionCommand<object>(
               obj => VisualiseHistorique(obj), obj => true);
            PrintEQCommand = new ActionCommand<object>(
               obj => Print(obj), obj => true);
            UploadImageCommand = new ActionCommand<object>(
               obj => UploadImage(obj), obj => true);
            DeleteImageCommand = new ActionCommand<object>(
               obj => DeleteImage(obj), obj => true);


            this.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "MainTileItemState")
                {
                    RaisePropertyChanged(() => IsNonEditableTileItemState);
                    this.RaisePropertyChanged(() => IsDocumentEditMode);
                    this.RaisePropertyChanged(() => IsMainEditMode);
                }
                else if (e.PropertyName == "IsEditMode")
                {
                    AddDocumentCommand.RaiseCanExecuteChanged();
                    DeleteDocumentCommand.RaiseCanExecuteChanged();
                    UploadDocumentCommand.RaiseCanExecuteChanged();
                    ArchiveDocumentCommand.RaiseCanExecuteChanged();
                    RestaureDocumentCommand.RaiseCanExecuteChanged();
                    SaveDocumentCommand.RaiseCanExecuteChanged();
                    CancelDocumentCommand.RaiseCanExecuteChanged();
                    EditDocumentCommand.RaiseCanExecuteChanged();
                    this.RaisePropertyChanged(() => IsDocumentEditMode);
                    this.RaisePropertyChanged(() => IsMainEditMode);
                }
            };

            this.OnSaveError += (o, e) =>
            {
                // Sauvegarde en erreur (cas de la réintégration)
                // => passage en mode édition
                if (!IsEditMode)
                {
                    this.IsEditMode = true;
                }
                IsBusy = false;
            };

            this.OnViewActivated += (o, e) =>
            {
                this.ListLogOuvrages = null;
                RaisePropertyChanged(() => this.IsDeleteEnable);
                this.RaisePropertyChanged(() => this.IsPrintable);
                RaisePropertyChanged(() => this.IsDeletableImage);
                RaisePropertyChanged(() => this.SelectedEntityImage);
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                // filtrer en fonction du type d'ouvrage
                if (this.serviceTypeDocument.Entities != null && this.serviceTypeDocument.Entities.Count > 0)
                {
                    var typeOuvrage = getDossierTypeOuvrage();
                    var dossier = this.serviceTypeDocument.Entities.Where(t => t.Libelle == typeOuvrage).FirstOrDefault();
                    if (dossier != null)
                    {
                        this.Dossiers = new ObservableCollection<TypeDocument>(dossier.Entities);
                    }
                }

                // MAJ de la vue
                RaisePropertyChanged(() => this.IsLinkToMicadoEnable);
                RaisePropertyChanged(() => this.HistoEntityExist);
            };

            this.OnDetailLoaded += (o, e) =>
            {
                // Si le tileview des documents est affiché et que l'entité change on recharge les documents
                if (this.DocumentsTileItemState == Telerik.Windows.Controls.TileViewItemState.Maximized)
                {
                    LoadDocuments();
                }

                ListLogOuvrages = null;

                // Si le tileview des documents est affiché et que l'entité change on recharge les documents
                if (this.HistoriquesTileItemState == TileViewItemState.Maximized)
                {
                    LoadHistoriques();
                }

                this.RaisePropertyChanged(() => this.IsSaveWithHistoEnable);
                this.RaisePropertyChanged(() => this.IsReintegrateEnable);
                this.RaisePropertyChanged(() => this.IsDeleteEnable);
                this.RaisePropertyChanged(() => this.IsPrintable);
                RaisePropertyChanged(() => this.IsDeletableImage);
                RaisePropertyChanged(() => this.SelectedEntityImage);

                // Réinitialisation de la vue d'un historique
                EqEquipementHisto = null;
            };

            this.OnAddedEntity += (o, e) =>
            {
                // Réinitialisation de la vue d'un historique
                EqEquipementHisto = null;
            };

            this.OnCanceled += (o, e) =>
            {
                // MAJ de la vue
                RaisePropertyChanged(() => this.HistoEntityExist);
                RaisePropertyChanged(() => this.EqEquipementHisto);

                ObjetToDuplicate = null;
            };

            this.OnSaveSuccess += (o, e) =>
            {
                if ((SelectedEntity is EqEquipement && (SelectedEntity as EqEquipement).Supprime)
                    || (SelectedEntity is Pp && (SelectedEntity as Pp).Supprime)
                    || (SelectedEntity is PortionIntegrite && (SelectedEntity as PortionIntegrite).Supprime)
                    || (SelectedEntity is EnsembleElectrique && (SelectedEntity as EnsembleElectrique).Supprime)
                    )
                {
                    MessageBox.Show(Resource.BaseProtecaEntityViewModel_DeleteSuccess, "", MessageBoxButton.OK);
                }

                // On stocke l'élément charger dans une variable pour l'historisation
                //GetHistorisation();

                ObjetToDuplicate = null;

                ListLogOuvrages = null;

                this.RaisePropertyChanged(() => this.IsSaveWithHistoEnable);
                this.RaisePropertyChanged(() => this.IsReintegrateEnable);
                this.RaisePropertyChanged(() => this.IsDeleteEnable);
                this.RaisePropertyChanged(() => this.IsPrintable);
                RaisePropertyChanged(() => this.IsDeletableImage);
                RaisePropertyChanged(() => this.SelectedEntityImage);
                IsBusy = false;
            };

            this.OnViewModeChanging += (o, e) =>
            {
                if (ObjetToDuplicate == null && IsEditMode && !IsNewMode)
                {
                    // On stocke l'élément charger dans une variable pour l'historisation
                    GetHistorisation();
                }
            };

            this.OnViewModeChanged += (o, e) =>
            {
                // Réinitialisation de la vue d'un historique
                EqEquipementHisto = null;

                this.RaisePropertyChanged(() => this.IsSaveWithHistoEnable);
                this.RaisePropertyChanged(() => this.IsReintegrateEnable);
                this.RaisePropertyChanged(() => this.IsDeleteEnable);
                this.RaisePropertyChanged(() => this.IsPrintable);
                RaisePropertyChanged(() => this.IsDeletableImage);
                RaisePropertyChanged(() => this.SelectedEntityImage);

                // MAJ de la vue
                RaisePropertyChanged(() => this.HistoEntityExist);


            };

            this.OnFindLoaded += (o, e) =>
            {
                if (this.SelectedEntity == null)
                {
                    serviceDocument.Clear();
                    RaisePropertyChanged(() => this.Documents);
                }

            };
        }

        #endregion

        #region Events

        /// <summary>
        /// Evenements lorsque l'ouvrage est totalement chargé (y compris les documents liés)
        /// </summary>
        public EventHandler OnOuvrageLoaded;

        #endregion

        #region Services

        /// <summary>
        /// Service utilisé pour gérer les documents sharepoint
        /// </summary>
        [Import]
        public IEntityService<Document> serviceDocument { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les types de documents sharepoint
        /// </summary>
        [Import]
        public IEntityService<TypeDocument> serviceTypeDocument { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les images des équipements
        /// </summary>
        [Import]
        public IEntityService<Proteca.Web.Models.Image> serviceImage { get; set; }

        /// <summary>
        /// Service pour récupérer les Paramètres
        /// </summary>
        [Import]
        public IEntityService<RefParametre> ServiceRefParametre { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Commande pour ajouter un document
        /// </summary>
        public IActionCommand AddDocumentCommand { get; protected set; }

        /// <summary>
        /// Commande pour éditer les documents
        /// </summary>
        public IActionCommand EditDocumentCommand { get; protected set; }

        /// <summary>
        /// Commande pour supprimer un document
        /// </summary>
        public IActionCommand DeleteDocumentCommand { get; protected set; }

        /// <summary>
        /// Commande pour uploader un document
        /// </summary>
        public IActionCommand UploadDocumentCommand { get; protected set; }

        /// <summary>
        /// Commande pour archiver le document
        /// </summary>
        public IActionCommand ArchiveDocumentCommand { get; protected set; }

        /// <summary>
        /// Commande pour restaurer le document
        /// </summary>
        public IActionCommand RestaureDocumentCommand { get; protected set; }

        /// <summary>
        /// Commande pour enregistrer les documents
        /// </summary>
        public IActionCommand SaveDocumentCommand { get; protected set; }

        /// <summary>
        /// Commande pour annuler les modifications sur les documents
        /// </summary>
        public IActionCommand CancelDocumentCommand { get; protected set; }

        /// <summary>
        /// Commande pour sauvegarder avec historisation
        /// </summary>
        public IActionCommand SaveHistorisationCommand { get; protected set; }


        #endregion

        #region Properties

        private ObservableCollection<TypeDocument> _dossiers;
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<TypeDocument> Dossiers
        {
            get
            {
                if (_dossiers == null)
                {
                    _dossiers = new ObservableCollection<TypeDocument>();
                }
                return _dossiers;
            }
            set
            {
                _dossiers = value;
                this.RaisePropertyChanged(() => this.Dossiers);
            }

        }

        private List<TypeDocument> _designations;
        protected List<TypeDocument> Designations 
        {
            get
            {
                if (_designations == null)
                {
                    _designations = new List<TypeDocument>();
                }
                return _designations;
            }
            set
            {
                _designations = value;
                this.RaisePropertyChanged(() => this.Designations);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Document> Documents
        {
            get
            {
                if (serviceDocument.Entities.Any())
                {
                    return new ObservableCollection<Document>(serviceDocument.Entities.Where(d => !d.IsDeleted).OrderBy(d => d.Cle));
                }
                return serviceDocument.Entities;
            }
        }

        public BitmapImage SelectedEntityImage
        {
            get
            {
                if (SelectedEntity == null)
                    return null;

                if (SelectedEntity is EqEquipement)
                {
                    if ((SelectedEntity as EqEquipement).Images.FirstOrDefault() != null
                        && (SelectedEntity as EqEquipement).Images.FirstOrDefault().Image1 != null)
                    {
                        using (var ms = new MemoryStream((SelectedEntity as EqEquipement).Images.FirstOrDefault().Image1))
                        {
                            BitmapImage im = new BitmapImage();
                            im.SetSource(ms);
                            return im;
                        }
                    }
                }
                else
                {
                    if (SelectedEntity is Pp)
                    {
                        if ((SelectedEntity as Pp).Images.FirstOrDefault() != null
                            && (SelectedEntity as Pp).Images.FirstOrDefault().Image1 != null)
                        {
                            using (var ms = new MemoryStream((SelectedEntity as Pp).Images.FirstOrDefault().Image1))
                            {
                                BitmapImage im = new BitmapImage();
                                im.SetSource(ms);
                                return im;
                            }
                        }
                    }
                }

                return null;
            }
        }

        private Document _selectedDocument;
        /// <summary>
        /// 
        /// </summary>
        public Document SelectedDocument
        {
            get
            {
                return _selectedDocument;
            }
            set
            {
                _selectedDocument = value;
                this.RaisePropertyChanged(() => this.SelectedDocument);
            }
        }

        /// <summary>
        /// Indique si l'entité peut être enregistré avec historisation (Si ce n'est pas une nouvelle entité)
        /// </summary>
        public Boolean IsSaveWithHistoEnable
        {
            get
            {
                if (SelectedEntity != null && !SelectedEntity.IsNew())
                {
                    return IsEditMode;
                }
                return false;
            }
        }

        /// <summary>
        /// Indique si l'entité peut être réintégrée (donc  déjà supprimée)
        /// </summary>
        public Boolean IsReintegrateEnable
        {
            get
            {
                if (SelectedEntity != null)
                {
                    if ((SelectedEntity is EqEquipement && (SelectedEntity as EqEquipement).Supprime && !(SelectedEntity as EqEquipement).IsDeplace)
                        || (SelectedEntity is Pp && (SelectedEntity as Pp).Supprime && !(SelectedEntity as Pp).IsDeplace)
                        || (SelectedEntity is PortionIntegrite && (SelectedEntity as PortionIntegrite).Supprime)
                        || (SelectedEntity is EnsembleElectrique && (SelectedEntity as EnsembleElectrique).Supprime)
                        )
                    {
                        return !IsEditMode;
                    }

                }
                return false;
            }
        }

        public Boolean IsPrintable
        {
            get
            {
                return (SelectedEntity != null && !IsEditMode);
            }
        }
        /// <summary>
        /// Indique si l'entité peut être supprimée
        /// </summary>
        public Boolean IsDeleteEnable
        {
            get
            {
                if (SelectedEntity != null)
                {
                    if ((SelectedEntity is EqEquipement && !(SelectedEntity as EqEquipement).Supprime)
                        || (SelectedEntity is Pp && !(SelectedEntity as Pp).Supprime)
                        || (SelectedEntity is PortionIntegrite && !(SelectedEntity as PortionIntegrite).Supprime)
                        || (SelectedEntity is EnsembleElectrique && !(SelectedEntity as EnsembleElectrique).Supprime)
                        )
                    {
                        return !IsEditMode;
                    }

                }
                return false;
            }
        }

        /// <summary>
        /// Indique si l'image de l'ouvrage/equipement peut être supprimée.
        /// </summary>
        public Boolean IsDeletableImage
        {
            get
            {
                bool result = false;

                if (SelectedEntity == null)
                    return false;

                if (SelectedEntity is EqEquipement)
                {
                    if ((SelectedEntity as EqEquipement).Images.FirstOrDefault() != null
                        && (SelectedEntity as EqEquipement).Images.FirstOrDefault().Image1 != null)
                    {
                        result = true;
                    }
                }
                else
                {
                    if (SelectedEntity is Pp)
                    {
                        if ((SelectedEntity as Pp).Images.FirstOrDefault() != null
                            && (SelectedEntity as Pp).Images.FirstOrDefault().Image1 != null)
                        {
                            result = true;
                        }
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Command pour visualiser l'historique
        /// </summary>
        public IActionCommand HistoViewCommand { get; protected set; }

        /// <summary>
        /// Retourne si le tileview correspondant doit être affiché
        /// </summary>
        public bool HistoEntityExist
        {
            get
            {
                return !IsEditMode && EqEquipementHisto != null;
            }
        }

        /// <summary>
        /// Propriété gérant l'entité historisé à afficher
        /// </summary>
        private Entity _eqEquipementHisto;

        /// <summary>
        /// Propriété gérant l'entité historisé à afficher
        /// </summary>
        public Entity EqEquipementHisto
        {
            get
            {
                return _eqEquipementHisto;
            }
            set
            {
                _eqEquipementHisto = value;
                HistoViewTileItemState = value != null ? TileViewItemState.Maximized : TileViewItemState.Minimized;
                this.RaisePropertyChanged(() => HistoEntityExist);
                this.RaisePropertyChanged(() => EqEquipementHisto);
            }
        }

        /// <summary>
        /// Propriété qui retourne si les liens vers Micado doivent être accessibles
        /// </summary>
        public Boolean IsLinkToMicadoEnable
        {
            get
            {
                return this.ServiceRefParametre.Entities.Any(p => p.CodeGroupe == "ECHANGES" && p.Libelle == "ECHANGE_PROTECA_MICADO" && p.Valeur == "1");
            }
        }

        #region TileView

        private Telerik.Windows.Controls.TileViewItemState _documentsTileItemState = Telerik.Windows.Controls.TileViewItemState.Minimized;
        public Telerik.Windows.Controls.TileViewItemState DocumentsTileItemState
        {
            get { return _documentsTileItemState; }
            set
            {
                if (value == Telerik.Windows.Controls.TileViewItemState.Maximized && this.SelectedEntity != null)
                {
                    this.LoadDocuments();
                }
                _documentsTileItemState = value;
                RaisePropertyChanged(() => this.DocumentsTileItemState);
                RaisePropertyChanged(() => IsNonEditableTileItemState);
                RaisePropertyChanged(() => IsDocumentTileItemState);
                RaisePropertyChanged(() => IsMainTileItemState);
            }
        }

        /// <summary>
        /// Gestion de l'affichage sur le tileview des historiques
        /// </summary>
        private TileViewItemState _historiquesTileItemState = TileViewItemState.Minimized;
        public TileViewItemState HistoriquesTileItemState
        {
            get { return _historiquesTileItemState; }
            set
            {
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null && this.ListLogOuvrages == null)
                {
                    this.LoadHistoriques();
                }
                _historiquesTileItemState = value;

                RaisePropertyChanged(() => HistoriquesTileItemState);
                RaisePropertyChanged(() => IsMainTileItemState);
            }
        }


        /// <summary>
        /// Propriété pour l'état du tile view du formulaire d'historisation
        /// </summary>
        private TileViewItemState _histoViewTileItemState = TileViewItemState.Minimized;

        /// <summary>
        /// Retourne l'état du tile view du formulaire d'historisation
        /// </summary>
        public TileViewItemState HistoViewTileItemState
        {
            get
            {
                return _histoViewTileItemState;
            }
            set
            {
                _histoViewTileItemState = value;
                this.RaisePropertyChanged(() => HistoViewTileItemState);
                this.RaisePropertyChanged(() => IsHistoViewTileItemState);
                this.RaisePropertyChanged(() => IsNonEditableTileItemState);
                this.RaisePropertyChanged(() => IsMainTileItemState);
            }
        }

        public bool IsNonEditableTileItemState
        {
            get
            {
                return (DocumentsTileItemState == Telerik.Windows.Controls.TileViewItemState.Minimized
                    && MainTileItemState == Telerik.Windows.Controls.TileViewItemState.Minimized
                    && HistoViewTileItemState == Telerik.Windows.Controls.TileViewItemState.Minimized);
            }
        }

        public bool IsDocumentTileItemState
        {
            get
            {
                return DocumentsTileItemState == Telerik.Windows.Controls.TileViewItemState.Maximized;
            }
        }

        public bool IsHistoViewTileItemState
        {
            get
            {
                return HistoViewTileItemState == Telerik.Windows.Controls.TileViewItemState.Maximized;
            }
        }

        public bool IsMainEditMode
        {
            get
            {
                return this.IsEditMode && MainTileItemState == Telerik.Windows.Controls.TileViewItemState.Maximized;
            }
        }

        public bool IsDocumentEditMode
        {
            get
            {
                return this.IsEditMode && DocumentsTileItemState == Telerik.Windows.Controls.TileViewItemState.Maximized;
            }
        }

        #endregion

        #endregion

        #region protected Methods

        /// <summary>
        /// On stocke l'élément charger dans une variable pour l'historisation
        /// </summary>
        protected virtual void GetHistorisation()
        {
            // On stocke l'élément charger dans une variable pour l'historisation
            if (this.SelectedEntity != null && this.SelectedEntity is IOuvrage)
            {
                ObjetToDuplicate = ((IOuvrage)this.SelectedEntity).GetHisto();
            }
        }

        /// <summary>
        /// Charge les documents liés à l'entité à partir de sharepoint
        /// </summary>
        protected void LoadDocuments()
        {
            int cleOuvrage = getCleOuvrageValue();
            if (cleOuvrage > 0)
            {
                IsBusy = true;
                ((DocumentService)this.serviceDocument).GetEntitiesByCleOuvrage(exception =>
                {
                    if (exception == null && this.Dossiers != null)
                    {
                        Designations = new List<TypeDocument>();
                        foreach (var dossier in this.Dossiers)
                        {
                            Designations.AddRange(dossier.Entities);
                        }
                        foreach (var doc in this.Documents)
                        {
                            doc.Designations = new ObservableCollection<TypeDocument>(Designations);
                            doc.Designation = Designations.Where(d => d.ServerRelativeUrl + "/" + d.Libelle == doc.Designation.ServerRelativeUrl).FirstOrDefault();
                            if (doc.Designation != null)
                            {
                                doc.Dossier = doc.Designation.Parent;
                            }
                            // On réinitialise la propriété IsMoved après le chargement initial
                            doc.IsMoved = false;
                        }
                        RaisePropertyChanged(() => this.Documents);
                        EditDocumentCommand.RaiseCanExecuteChanged();
                    }
                    else
                    {
                        ErrorWindow.CreateNew(exception.Message);
                    }
                    if (OnOuvrageLoaded != null)
                    {
                        OnOuvrageLoaded(this, null);
                    }
                    IsBusy = false;
                }, getTypeCleOuvrage(), cleOuvrage);
            }
            else
            {
                RaisePropertyChanged(() => this.Documents);
            }
        }

        /// <summary>
        /// Chargements des historique liés à l'entité 
        /// </summary>
        protected void LoadHistoriques()
        {
            IsBusy = true;
            ((LogOuvrageService)this.serviceLogOuvrage).GetLogOuvrageByCleOuvrage(getTypeCleOuvrage().GetStringValue(), getCleOuvrageValue(), LoadLogOuvragesDone);
        }

        /// <summary>
        /// Récupération des historiques
        /// </summary>
        /// <param name="error"></param>
        /// <param name="listOuvrage"></param>
        private void LoadLogOuvragesDone(Exception error, List<LogOuvrage> listOuvrage)
        {
            IsBusy = false;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(LogOuvrage).Name));
            }
            else
            {
                this.ListLogOuvrages = listOuvrage;
            }
        }

        /// <summary>
        /// Fonction de suppression de ligne dans le tableau
        /// </summary>
        protected void DeleteDocument(object Obj)
        {
            var result = MessageBox.Show(Resource.Document_DeleteConfirmation, "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                this.serviceDocument.Delete((Document)Obj);
                RaisePropertyChanged(() => this.Documents);
            };
        }

        /// <summary>
        /// Ajout d'un document
        /// </summary>
        protected void AddDocument()
        {
            Document doc = new Document();
            doc.TypeOuvrage = getTypeCleOuvrage();
            doc.DateEnregistrement = DateTime.Now;
            doc.NumeroVersion = "1.0";
            doc.Designations = new ObservableCollection<TypeDocument>(Designations);
            doc.CleOuvrage = (int)this.SelectedEntity.GetCustomIdentity();
            this.serviceDocument.Add(doc);
            RaisePropertyChanged(() => this.Documents);
        }

        /// <summary>
        /// Enregistrement des documents dans sharepoint
        /// </summary>
        protected void SaveDocument()
        {
            if (IsEditMode)
            {
                Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
                if (!UserCanSave)
                {
                    ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
                }
                else
                {



                    //Si valide, enregistrement des documents.
                    Collection<ValidationResult> errors = new Collection<ValidationResult>();
                    bool isValid = true;
                    foreach (var doc in this.Documents)
                    {
                        if (doc != null)
                        {
                            doc.ValidationErrors.Clear();
                            isValid &= Validator.TryValidateObject(doc, new ValidationContext(doc, null, null), errors, true);
                        }
                    }
                    if (isValid)
                    {
                        try
                        {                            
                            // Si l'image a changée
                            if (!this.SelectedEntity.IsNew())
                            {
                                this.LogOuvrage("M", this.SelectedEntity);
                                serviceImage.SaveChanges(error =>
                                {
                                }
                                );
                            }                         

                            IsBusy = true;
                            this.serviceDocument.SaveChanges(error =>
                            {
                                if (error == null)
                                {
                                    this.IsEditMode = false;
                                    this.NotifyError = false;
                                    RaisePropertyChanged(() => this.Documents);
                                }
                                else
                                {
                                    this.NotifyError = true;
                                }
                                IsBusy = false;
                            });
                        }
                        catch (ValidationException)
                        {
                            this.NotifyError = true;
                            IsBusy = false;
                        }
                    }
                    else
                    {
                        this.NotifyError = true;
                    }
                }
            }
        }

        protected void CancelDocument()
        {
            this.IsEditMode = false;
            Cancel();
            RaisePropertyChanged(() => this.IsDeletableImage);
            RaisePropertyChanged(() => this.SelectedEntityImage);
            LoadDocuments();
        }

        /// <summary>
        /// Archive ou restaure un document
        /// </summary>
        private void ArchiveDocument(object obj, bool archive)
        {
            Document doc = obj as Document;
            doc.Archive = archive;
        }

        private void UploadDocument(object obj)
        {
            Document doc = obj as Document;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            bool? userClickedOK = openFileDialog.ShowDialog();
            if (userClickedOK == true)
            {
                this.IsBusy = true;
                using (System.IO.Stream fileStream = openFileDialog.File.OpenRead())
                {
                    int maxSizeMo = 5;
                    MessageBoxResult result = MessageBoxResult.OK;

                    if (openFileDialog.File.Length > 5 * 1024 * 1024)
                    {
                        result = MessageBox.Show(String.Format(Proteca.Silverlight.Resources.Resource.Document_MessageErreur, maxSizeMo), "", MessageBoxButton.OKCancel);
                    }

                    if (result == MessageBoxResult.OK)
                    {
                        IsBusy = true;
                        using (System.IO.BinaryReader reader = new System.IO.BinaryReader(fileStream))
                        {
                            doc.Content = StreamHelper.ReadToEnd(fileStream);
                            doc.Libelle = openFileDialog.File.Name;
                            doc.IsNewEntity = true;
                        }
                        IsBusy = false;
                    }

                    fileStream.Close();
                }
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Sélection d'une image à associer à l'équipement.
        /// </summary>
        private void UploadImage(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.gif, *.png, *.bmp) | *.jpg; *.jpeg; *.gif; *.png; *.bmp;";

            bool? userClickedOK = dlg.ShowDialog();
            if (userClickedOK == true && dlg.File != null)
            {
                this.IsBusy = true;
                using (System.IO.Stream fileStream = dlg.File.OpenRead())
                {
                    Byte[] img = StreamHelper.ReadToEnd(fileStream);
                    int maxSizeKo = 0;
                    if (int.TryParse(ServiceRefParametre.Entities.FirstOrDefault(x => x.Libelle.Equals("POIDS_MAX_IMAGE")).Valeur, out maxSizeKo) && dlg.File.Length <= 1024 * maxSizeKo)
                    {
                        RefEnumValeur val = ServiceEnumValeur.Entities.FirstOrDefault(x => x.CodeGroupe.Equals("ENUM_TYPE_IMAGE")
                                                                                    && ("." + x.LibelleCourt.ToLower()).Equals(dlg.File.Extension.ToLower()));
                        int imageType = (val != null) ? val.CleEnumValeur : 1;

                        if (SelectedEntity is EqEquipement)
                        {
                            if ((SelectedEntity as EqEquipement).Images.FirstOrDefault() == null)
                            {
                                (SelectedEntity as EqEquipement).Images.Add(new Web.Models.Image());
                            }
                            (SelectedEntity as EqEquipement).Images.FirstOrDefault().Image1 = img;
                            (SelectedEntity as EqEquipement).Images.FirstOrDefault().EnumTypeImage = imageType;

                            RaisePropertyChanged(() => this.IsDeletableImage);
                            RaisePropertyChanged(() => this.SelectedEntityImage);
                        }
                        else if (SelectedEntity is Pp)
                        {
                            if ((SelectedEntity as Pp).Images.FirstOrDefault() == null)
                            {
                                (SelectedEntity as Pp).Images.Add(new Web.Models.Image());
                            }
                            (SelectedEntity as Pp).Images.FirstOrDefault().Image1 = img;
                            (SelectedEntity as Pp).Images.FirstOrDefault().EnumTypeImage = imageType;
                            RaisePropertyChanged(() => this.IsDeletableImage);
                            RaisePropertyChanged(() => this.SelectedEntityImage);
                        }
                    }
                    else
                    {
                        MessageBox.Show(String.Format(Proteca.Silverlight.Resources.Resource.Image_MessageErreur, maxSizeKo), "", MessageBoxButton.OK);
                    }
                    fileStream.Close();
                }
                this.IsBusy = false;
            }
            else
            {
            }
        }

        /// <summary>
        /// Suppression de l'image liée à l'ouvrage.
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteImage(object obj)
        {
            if (SelectedEntity != null)
            {
                Proteca.Web.Models.Image image = null;

                if (SelectedEntity is EqEquipement)
                {
                    image = (SelectedEntity as EqEquipement).Images.FirstOrDefault();
                    if (image != null)
                    {
                        serviceImage.Delete(image);
                        (SelectedEntity as EqEquipement).Images.Remove(image);
                    }
                    RaisePropertyChanged(() => this.IsDeletableImage);
                    RaisePropertyChanged(() => this.SelectedEntityImage);
                }
                else if (SelectedEntity is Pp)
                {
                    image = (SelectedEntity as Pp).Images.FirstOrDefault();
                    if (image != null)
                    {
                        serviceImage.Delete(image);
                        (SelectedEntity as Pp).Images.Remove(image);
                    }
                    RaisePropertyChanged(() => this.IsDeletableImage);
                    RaisePropertyChanged(() => this.SelectedEntityImage);
                }
            }
        }

        /// <summary>
        /// Imprimer le rapport de l'équipement ou PP.
        /// </summary>
        /// <param name="obj"></param>
        private void Print(object obj)
        {
            String rapportUrl = Rapports.printDocumentUrl;
            if (SelectedEntity is EqEquipement)
	        {
                String urlDetail = (SelectedEntity as EqEquipement).TypeEquipement.CodeEquipement;
                int cleEq = (SelectedEntity as EqEquipement).CleEquipement;
                if (IsHistoViewTileItemState)
	            {
		            urlDetail += "_histo";
                    cleEq = (EqEquipementHisto as HistoEquipement) != null ? (EqEquipementHisto as HistoEquipement).CleHistoEquipement : 0;
                }
                rapportUrl += String.Format(Rapports.printFicheEquipementFileName, urlDetail, cleEq);
	        }
            else if (SelectedEntity is Pp)
            {
                String urlDetail = "PP";
                int cleEq = (SelectedEntity as Pp).ClePp;
                if (IsHistoViewTileItemState)
                {
                    urlDetail += "_histo";
                    cleEq = (EqEquipementHisto as HistoPp) != null ? (EqEquipementHisto as HistoPp).CleHistoPp : 0;
                }
                rapportUrl += String.Format(Rapports.printFicheEquipementFileName, urlDetail, cleEq);
            }

            HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
        }

        /// <summary>
        /// Récupère le type de clé ouvrage en fonction du type de l'entité sélectionnée
        /// </summary>
        /// <returns></returns>
        private CleOuvrage getTypeCleOuvrage()
        {
            CleOuvrage res = CleOuvrage.CleEnsembleElectrique;
            if (this.SelectedEntity != null)
            {
                if (this.SelectedEntity is EnsembleElectrique)
                {
                    res = CleOuvrage.CleEnsembleElectrique;
                }
                else if (this.SelectedEntity is EqEquipement)
                {
                    res = CleOuvrage.CleEquipement;
                }
                else if (this.SelectedEntity is PortionIntegrite)
                {
                    res = CleOuvrage.ClePortion;
                }
                else if (this.SelectedEntity is Pp)
                {
                    res = CleOuvrage.ClePP;
                }
            }
            return res;
        }

        private int getCleOuvrageValue()
        {
            int value = 0;
            if (this.SelectedEntity != null)
            {
                CleOuvrage typeCle = getTypeCleOuvrage();
                var prop = this.SelectedEntity.GetType().GetProperty(typeCle.GetStringValue());
                if (prop != null)
                {
                    value = (int)prop.GetValue(this.SelectedEntity, null);
                }
            }
            return value;
        }

        private string getDossierTypeOuvrage()
        {
            string res = string.Empty;
            Type typeEntity = typeof(T);

            if (typeEntity == typeof(EnsembleElectrique))
            {
                res = Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.ENSEMBLES_ELECTRIQUES.GetStringValue();
            }
            else if (typeEntity == typeof(PortionIntegrite))
            {
                res = Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.PORTIONS.GetStringValue();
            }
            else if (typeEntity == typeof(EqEquipement) || typeEntity.BaseType == typeof(EqEquipement) || typeEntity == typeof(Pp))
            {
                res = Proteca.Silverlight.Enums.NavigationEnums.DocumentationNavigation.EQUIPEMENTS.GetStringValue();
            }
            return res;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// On met à jour la date de la mise à jour avant la sauvegarde si le commentaire actuel est différent
        /// de l'ancien commentaire
        /// </summary>
        protected override void Save()
        {
            Save(false);
        }

        protected override void Save(bool forceSave)
        {
            Save(forceSave, false);
        }

        protected virtual void Save(bool forceSave, bool withHisto)
        {
            IsBusy = true;
            if (IsNewMode)
            {
                LogOuvrage("C");
            }
            else if (!withHisto)
            {
                if (this.SelectedEntity.HasChanges || this.SelectedEntity.HasChildChanges() || (this.SelectedEntity is PortionIntegrite && (this.SelectedEntity as PortionIntegrite).HasChildChanges))
                {
                    bool IsSupprime = false;
                    bool IsOrigineSupprime = false;

                    var type = this.SelectedEntity.GetType();

                    PropertyInfo prop = type.GetProperty("Supprime");
                    if (prop != null && !this.SelectedEntity.IsNew() && this.SelectedEntity.GetOriginal() != null)
                    {
                        IsSupprime = (bool)prop.GetValue(this.SelectedEntity, null);
                        IsOrigineSupprime = (bool)prop.GetValue(this.SelectedEntity.GetOriginal(), null);
                    }

                    if (!IsSupprime && IsOrigineSupprime)
                    {
                        LogOuvrage("R");
                    }
                    else if (IsSupprime && !IsOrigineSupprime)
                    {
                        LogOuvrage("S");
                    }
                    else
                    {
                        LogOuvrage("M");
                    }
                }
            }

            this.ListLogOuvrages = null;
            base.Save(forceSave);
        }

        /// <summary>
        /// Override de la fonction Delete
        /// </summary>
        protected override void Delete(bool skipNavigation, bool skipConfirmation)
        {
            LogOuvrage("S");
            this.ListLogOuvrages = null;
            base.Delete(skipNavigation, skipConfirmation);
        }

        #region Gestion des droits
        protected bool GetUserCanEditDocument()
        {
            bool res = true;
            if (this.CurrentUser != null)
            {
                if (this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GESTION_DOC).RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    res &= false;
                }
            }
            return res && !IsEditMode;
        }

        protected bool GetUserCanAddDocument()
        {
            bool res = true;
            if (this.CurrentUser != null)
            {
                if (this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GESTION_DOC).RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    res &= false;
                }
            }
            return res && IsEditMode;
        }

        protected bool GetUserCanDeleteDocument()
        {
            bool res = true;
            if (this.CurrentUser != null)
            {
                if (this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.SUP_DOC).RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    res &= false;
                }
            }
            return res && IsEditMode;
        }
        #endregion

        #endregion Protected Methods

        #region Historisation

        /// <summary>
        /// Permet d'afficher l'historique sélectionner
        /// </summary>
        /// <param name="obj"></param>
        public void VisualiseHistorique(object obj)
        {
            LogOuvrage LogOuvrageHisto = (LogOuvrage)obj;
            if (this.SelectedEntity is EqEquipement) { EqEquipementHisto = LogOuvrageHisto.HistoEquipement.FirstOrDefault(); };
            if (this.SelectedEntity is Pp) { EqEquipementHisto = LogOuvrageHisto.HistoPp.FirstOrDefault(); };
        }

        /// <summary>
        /// Instanciation de l'historisation et du logouvrage associé
        /// </summary>
        private void HistoriserOuvrage()
        {
            // On log l'élément en mode historisation
            LogOuvrage("H");

            // On rattache l'élément au logouvrage correspondant
            if (this.SelectedEntity is EqEquipement) { (SelectedEntity as EqEquipement).LogOuvrage.Where(lo => lo.IsNew()).FirstOrDefault().HistoEquipement.Add((HistoEquipement)ObjetToDuplicate); };
            if (this.SelectedEntity is Pp) { (SelectedEntity as Pp).LogOuvrage.Where(lo => lo.IsNew()).FirstOrDefault().HistoPp.Add((HistoPp)ObjetToDuplicate); }

            // On sauvegarde
            Save(false, true);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Ajout d'un enregistrement dans logOuvrage
        /// </summary>
        public void AddNewLogOuvrage(string Operation, Entity entity)
        {
            // Instanciation des propriétés
            EqEquipement currenteq = null;
            PortionIntegrite currentPortion = null;
            EnsembleElectrique currentEnsElect = null;
            Pp currentPp = null;
            EntityCollection<LogOuvrage> LogOuvrageList = null;
            LogOuvrage _logAajouter;

            // Détermination du type d'équipement
            if (entity is EqEquipement)
            {
                currenteq = entity as EqEquipement;
                LogOuvrageList = currenteq.LogOuvrage;
            }
            else if (entity is PortionIntegrite)
            {
                currentPortion = entity as PortionIntegrite;
                LogOuvrageList = currentPortion.LogOuvrage;
            }
            else if (entity is EnsembleElectrique)
            {
                currentEnsElect = entity as EnsembleElectrique;
                LogOuvrageList = currentEnsElect.LogOuvrage;
            }
            else if (entity is Pp)
            {
                currentPp = entity as Pp;
                LogOuvrageList = currentPp.LogOuvrage;
            }

            // Suppression des logs existant
            if (LogOuvrageList != null && LogOuvrageList.Any(lo => lo.IsNew()))
            {
                int nbLogs = LogOuvrageList.Where(lo => lo.IsNew()).Count();
                for (int i = 0; i < nbLogs; i++)
                {
                    LogOuvrage log = LogOuvrageList.FirstOrDefault(lo => lo.IsNew());
                    LogOuvrageList.Remove(log);
                    serviceLogOuvrage.Delete(log);
                }
                _logAajouter = null;
            }

            // Instanciation du log ouvrage
            _logAajouter = new LogOuvrage
            {
                CleUtilisateur = this.CurrentUser.CleUtilisateur,
                EnumTypeModification = ServiceEnumValeur.Entities.Where(r => r.Valeur == Operation).FirstOrDefault().CleEnumValeur,
                DateHistorisation = DateTime.Now
            };

            // En cas de changement du sélected entity, on log l'enregistrement
            if (entity.HasChanges || Operation == "H" || Operation == "S" || Operation == "R")
            {
                if (entity.HasChanges && !IsNewMode)
                {
                    string Modifiedproperties = null;

                    Entity original = entity.GetOriginal();
                    List<string> elements = new List<string>();

                    foreach (PropertyInfo p in entity.GetType().GetProperties())
                    {
                        if (p.CanWrite && !(p.PropertyType.BaseType == typeof(Entity)))
                        {
                            Object originalValue = p.GetValue(original, null);
                            Object newValue = p.GetValue(entity, null);
                            if ((originalValue == null && newValue == null) || (originalValue != null && originalValue.Equals(newValue)))
                            {
                                continue;
                            }
                            else
                            {
                                Modifiedproperties += Modifiedproperties == null ? p.Name : " / " + p.Name;
                            }
                        }
                        else if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>))
                        {
                            IEnumerable childEntities = p.GetValue(original, null) as IEnumerable;
                            IEnumerable newValue = p.GetValue(entity, null) as IEnumerable;

                            if (p.Name != "LogOuvrage" && childEntities != null && newValue != null)
                            {
                                foreach (var childEntity in newValue) // on regarde si il y a de nouveaux éléments ou des éléments modifiés
                                {
                                    if (childEntity.GetType().BaseType == typeof(Entity) &&
                                        ((((Entity)childEntity).EntityState == EntityState.New) || ((Entity)childEntity).EntityState == EntityState.Modified))
                                    {
                                        elements.Add(p.Name);
                                        Modifiedproperties += Modifiedproperties == null ? p.Name : " / " + p.Name;
                                        break;
                                    }
                                }

                                if (!elements.Contains(p.Name))
                                {
                                    foreach (var childEntity in childEntities) // on regarde si il y a des éléments supprimés
                                    {
                                        if (childEntity.GetType().BaseType == typeof(Entity) && ((Entity)childEntity).EntityState == EntityState.Deleted)
                                        {
                                            Modifiedproperties += Modifiedproperties == null ? p.Name : " / " + p.Name;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    _logAajouter.ListeChamps = Modifiedproperties;
                }

                // On ajoute le log au contexte
                LogOuvrageList.Add(_logAajouter);
            }
        }

        #endregion Public Functions
    }
}
