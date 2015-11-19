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
using Microsoft.SharePoint.Client;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Resources;
using System.Collections.Generic;
using Proteca.Silverlight.Resources;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Models
{
    /// <summary>
    /// Gestion des types de document
    /// </summary>
    public class TypeDocument : Entity
    {
        #region Constructor
        public TypeDocument(int cle, string libelle, string serverRelativeUrl)
        {
            Cle = cle;
            Entities = new List<TypeDocument>();
            IsDeleted = false;
            BaseObject = null;
            _libelle = libelle;
            LibelleOriginal = libelle;
            if (!String.IsNullOrEmpty(serverRelativeUrl))
            {
                serverRelativeUrl = serverRelativeUrl.Remove(serverRelativeUrl.LastIndexOf('/'));
            }
            ServerRelativeUrl = serverRelativeUrl;
            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Libelle")
                {
                    this.IsModified = true;
                }
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retounre l'identité de l'entité
        /// </summary>
        /// <returns></returns>
        public override object GetIdentity()
        {
            return this.Cle;
        }
        /// <summary>
        /// Indique si l'entité est nouvelle
        /// </summary>
        /// <returns></returns>
        public bool IsNew()
        {
            return this.IsNewEntity;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Object sharepoint
        /// </summary>
        public Folder BaseObject { get; set; }

        /// <summary>
        /// Libellé du type de document
        /// </summary>
        private string _libelle;
        [Required(ErrorMessageResourceType = typeof(ValidationErrorResources),
                    ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
        [StringLengthCustom(128)]
        public string Libelle
        {
            get
            {
                return _libelle;
            }
            set
            {
                if (_libelle != value)
                {
                    _libelle = value;
                    this.ValidateProperty("Libelle", value);
                    this.RaisePropertyChanged("Libelle");
                }
            }
        }

        protected override void ValidateProperty(ValidationContext validationContext, object value)
        {
            base.ValidateProperty(validationContext, value);
            if (this.Parent != null && this.Parent.Entities != null)
            {
                if (this.Parent.Entities.Any(d => !d.IsDeleted && d.Cle != this.Cle && d.Libelle.ToLower() == ("" + value).ToLower()))
                {
                    this.ValidationErrors.Add(new ValidationResult(Resource.TypeDocument_SharepointFolderExistError, new List<string>() { "Libelle" }));
                }
            }
        }

        /// <summary>
        /// Libellé avant modification
        /// </summary>
        public string LibelleOriginal { get; set; }

        /// <summary>
        /// Url sharepoint
        /// </summary>
        private string _serverRelativeUrl;
        public string ServerRelativeUrl
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.ServerRelativeUrl + "/" + this.Parent.Libelle;
                }
                else
                {
                    return _serverRelativeUrl;
                }
            }
            set
            {
                _serverRelativeUrl = value;
            }
        }

        /// <summary>
        /// Clé de l'entité
        /// </summary>
        public int Cle { get; set; }

        /// <summary>
        /// Indique si l'entité est modifiée
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// Indique si l'entité est supprimée
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Inidique s'il s'agit d'une nouvelle entité
        /// </summary>
        public bool IsNewEntity { get; set; }

        private Boolean _isExpanded = false;
        /// <summary>
        /// Indique s'il le noeud correspondant dans le treeview doit être ouvert
        /// </summary>
        public Boolean IsExpanded { get { return _isExpanded; } set { _isExpanded = value; RaisePropertyChanged("IsExpanded"); } }


        /// <summary>
        /// TypeDocument fils
        /// </summary>
        private List<TypeDocument> _entities;
        public List<TypeDocument> Entities
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value;
                this.RaisePropertyChanged("Entities");
                this.RaisePropertyChanged("TypeDocuments");
            }
        }

        /// <summary>
        /// TypeDocument parent
        /// </summary>
        public TypeDocument Parent { get; set; }

        /// <summary>
        /// TypeDocuments fils (excepté les éléments supprimés)
        /// </summary>
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

        /// <summary>
        /// Niveau du dossier
        /// </summary>
        public int Niveau
        {
            get
            {
                return ServerRelativeUrl.Split('/').Count() - 1;
            }
        }

        /// <summary>
        /// Le type d'ouvrage est le dossier de niveau 2 dans l'arborescence sharepoint
        /// </summary>
        public string TypeOuvrage
        {
            get
            {
                return GetNiveau(2);
            }
        }

        /// <summary>
        /// Le type de dossier est le dossier de niveau 3 dans l'arborescence sharepoint
        /// </summary>
        public string TypeDossier
        {
            get
            {
                return GetNiveau(3);
            }
        }
        /// <summary>
        /// La désignation est le dossier de niveau 4 dans l'arborescence sharepoint
        /// </summary>
        public string Designation
        {
            get
            {
                return GetNiveau(4);
            }
        }
        /// <summary>
        /// Le dossier de niveau X dans l'arborescence sharepoint
        /// </summary>
        private string GetNiveau(int niveau)
        {
            string res = string.Empty;
            var niveaux = ServerRelativeUrl.Split('/');
            if (niveaux.Count() > niveau)
            {
                res = niveaux[niveau];
            }
            return res;
        }

        #endregion
    }
}
