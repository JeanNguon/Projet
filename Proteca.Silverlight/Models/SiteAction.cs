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
    /// Gestion du Site Action
    /// </summary>
    public class SiteAction : Entity
    {
        #region Constructor
        /* DJ 20130328 : A supprimer
        public SiteAction(int cle, string libelle, string serverRelativeUrl)
        */
        public SiteAction()
        {
            /*
            Cle = cle;
            ////Entities = new List<SiteAction>();
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
            */
        }
        #endregion

        #region Methods

        /* DJ 20130328 : A supprimer
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
            //if (this.Parent != null && this.Parent.Entities != null)
            //{
            //    if (this.Parent.Entities.Any(d => !d.IsDeleted && d.Cle != this.Cle && d.Libelle.ToLower() == ("" + value).ToLower()))
            //    {
            //        this.ValidationErrors.Add(new ValidationResult(Resource.TypeDocument_SharepointFolderExistError, new List<string>() { "Libelle" }));
            //    }
            //}
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
                return _serverRelativeUrl;
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
        */

        #endregion
    }
}
