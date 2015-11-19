using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using Proteca.Silverlight.Enums;
using System.ComponentModel.DataAnnotations;
using Proteca.Web.Resources;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Models
{
    /// <summary>
    /// Enumeration des différentes clé correspondant aux types d'ouvrage
    /// </summary>
    public enum CleOuvrage
    {
        [StringValue("ClePp")]
        ClePP,
        [StringValue("CleEquipement")]
        CleEquipement,
        [StringValue("ClePortion")]
        ClePortion,
        [StringValue("CleEnsElectrique")]
        CleEnsembleElectrique
    }

    /// <summary>
    /// Gestion des documents sharepoint
    /// </summary>
    public class Document : Entity
    {

        #region Constructor
        public Document(string libelle = null, bool archive = false, TypeDocument designation = null)
        {
            initDocument(libelle, archive, designation);
        }

        public Document(CleOuvrage typeOuvrage, int cleOuvrage = 0, string numeroVersion = null, string libelle = null, bool archive = false, TypeDocument designation = null)
        {
            TypeOuvrage = typeOuvrage;
            CleOuvrage = cleOuvrage;
            NumeroVersion = numeroVersion;
            if (libelle != null && libelle.StartsWith(this.PrefixeFileName))
            {
                libelle = libelle.Remove(0, this.PrefixeFileName.Length);
            }
            initDocument(libelle, archive, designation);
        }

        private void initDocument(string libelle = null, bool archive = false, TypeDocument designation = null)
        {
            Libelle = libelle;
            LibelleOriginal = libelle;
            Archive = archive;
            Designation = designation;
            DesignationOriginale = designation;
            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Archive" || e.PropertyName == "NumeroVersion")
                {
                    this.IsModified = true;
                }
                else if (e.PropertyName == "Designation" && !this.IsNew)
                {
                    this.IsMoved = true;
                }
                else if (e.PropertyName == "Libelle")
                {
                    this.IsNewEntity = true;
                }
            };
        }

        #endregion

        #region Properties
        /// <summary>
        /// Propriété pour le binding afin de déterminer si l'entité est nouvelle
        /// </summary>
        public Boolean IsNew
        {
            get
            {
                return this.IsNew();
            }
        }

        /// <summary>
        /// Propriété pour le binding afin de déterminer si l'entité est nouvelle ou si des modifications ont été faites
        /// </summary>
        public Boolean HasChangesOrIsNew
        {
            get
            {
                return this.IsModified || this.IsMoved || this.IsNew;
            }
        }

        /// <summary>
        /// On force la MAJ des propriétés
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "HasChanges")
            {
                RaisePropertyChanged("HasChangesOrIsNew");
            }
        }

        public int ItemId { get; set; }

        private TypeDocument _dossier;
        /// <summary>
        /// Dossier parent du document (niveau 2 de l'arbo)
        /// </summary>
        public TypeDocument Dossier
        {

            get
            {
                return this._dossier;
            }
            set
            {
                _dossier = value;
                this.RaisePropertyChanged("Dossier");
                this.RaisePropertyChanged("Designations");
            }
        }

        private TypeDocument _designation;
        [Required(ErrorMessageResourceType = typeof(ValidationErrorResources),
                   ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
        /// <summary>
        /// Designation du document (niveau 3 de l'arbo)
        /// </summary>
        public TypeDocument Designation
        {
            get
            {
                return this._designation;
            }
            set
            {
                this.ValidateProperty("Designation", value);
                _designation = value;
                this.RaisePropertyChanged("Designation");
            }
        }

        public TypeDocument DesignationOriginale { get; set; }

        private ObservableCollection<TypeDocument> _designations;
        /// <summary>
        /// Liste de toutes les désignations du dossier sélectionné (utilisation uniquemnet technique)
        /// </summary>
        public ObservableCollection<TypeDocument> Designations
        {
            get
            {
                if (_designations != null && Dossier != null)
                {
                    return new ObservableCollection<TypeDocument>(_designations.Where(t => t.ServerRelativeUrl == Dossier.ServerRelativeUrl + "/" + Dossier.Libelle));
                }
                return new ObservableCollection<TypeDocument>();
            }
            set
            {
                _designations = value;
                this.RaisePropertyChanged("Designations");
            }
        }

        private string _libelle;
        [Required(ErrorMessageResourceType = typeof(ValidationErrorResources),
                    ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
        [StringLengthCustom(128)]
        /// <summary>
        /// Nom du document
        /// </summary>
        /// 
        public string Libelle
        {
            get
            {
                return this._libelle;
            }
            set
            {
                this.ValidateProperty("Libelle", value);
                _libelle = value;
                this.RaisePropertyChanged("Libelle");
            }
        }

        /// <summary>
        /// Préfixe du nom physique du document
        /// </summary>
        /// 
        public string PrefixeFileName
        {
            get
            {
                string _nom = string.Empty;
                switch (this.TypeOuvrage)
                {
                    case Proteca.Silverlight.Models.CleOuvrage.ClePP:
                        _nom = "PP_";
                        break;
                    case Proteca.Silverlight.Models.CleOuvrage.CleEquipement:
                        _nom = "EQ_";
                        break;
                    case Proteca.Silverlight.Models.CleOuvrage.ClePortion:
                        _nom = "PI_";
                        break;
                    case Proteca.Silverlight.Models.CleOuvrage.CleEnsembleElectrique:
                        _nom = "EE_";
                        break;
                    default:
                        break;
                }

                return _nom + this.CleOuvrage.ToString() + "_";
            }
        }        
        private bool _archive;
        /// <summary>
        /// Indique si le document est archivé ou non
        /// </summary>
        public bool Archive
        {
            get
            {
                return this._archive;
            }
            set
            {
                _archive = value;
                this.RaisePropertyChanged("Archive");
            }
        }

        private string _numeroVersion;

        [Required(ErrorMessageResourceType = typeof(ValidationErrorResources),
                ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
        [StringLengthCustom(30)]
        /// <summary>
        /// Numéro de version du document (géré par sharepoint)
        /// </summary>
        public string NumeroVersion 
        {
            get
            {
                return this._numeroVersion;
            }
            set
            {
                if (this._numeroVersion != value)
                {
                    this.RaiseDataMemberChanging("NumeroVersion");
                    this.ValidateProperty("NumeroVersion", value);
                    this._numeroVersion = value;
                    this.RaiseDataMemberChanged("NumeroVersion");
                }
            }
        }
        /// <summary>
        /// Date d'enregistrement du document  (géré par sharepoint)
        /// </summary>
        public DateTime DateEnregistrement { get; set; }
        /// <summary>
        /// Clé 
        /// </summary>
        public int Cle { get; set; }
        /// <summary>
        /// Indique si le document a été modifié (modification du flag archive uniquement)
        /// </summary>
        public bool IsModified { get; set; }
        /// <summary>
        /// Indique si le documnent a été déplacé dans une autre désignation
        /// </summary>
        public bool IsMoved { get; set; }
        /// <summary>
        /// Indique si le document est supprimé
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Indique que le document est nouveau
        /// </summary>
        public bool IsNewEntity { get; set; }
        /// <summary>
        /// Nom du document avant modification
        /// </summary>
        public string LibelleOriginal { get; set; }
        /// <summary>
        /// Clé ouvrage
        /// </summary>
        public int CleOuvrage { get; set; }
        /// <summary>
        /// Type d'ouvrage
        /// </summary>
        public CleOuvrage TypeOuvrage { get; set; }
        /// <summary>
        /// Contenu binaire correspondant au document (uniquement au chargement d'un nouveau document)
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// Url du document dans sharepoint
        /// </summary>
        public Uri DocumentUrl { get; set; }

        /// <summary>
        /// Url du dossier sharepoint
        /// </summary>
        public string ServerRelativeUrl
        {
            get
            {
                string res = string.Empty;
                if (this.Designation != null)
                {
                    res = this.Designation.ServerRelativeUrl + "/" + this.Designation.Libelle;
                }
                return res;
            }
        }

        /// <summary>
        /// Url du dossier sharepoint (avant modification)
        /// </summary>
        public string OriginalServerRelativeUrl
        {
            get
            {
                string res = string.Empty;
                if (this.DesignationOriginale != null)
                {
                    res = this.DesignationOriginale.ServerRelativeUrl + "/" + this.DesignationOriginale.Libelle;
                }
                return res;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Retourne l'identité de l'entité
        /// </summary>
        /// <returns></returns>
        public override object GetIdentity()
        {
            return this.Cle;
        }
        #endregion
    }
}
