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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Enums;

namespace Proteca.Web.Models
{
    public partial class TypeRessource : Entity
    {
        #region Properties

        /// <summary>
        /// Retourne si le type de ressource est sélectionné ou non
        /// </summary>
        public Boolean IsSelected
        {
            get { return CurrentNavigation.Current.Filtre != null && this.Libelle == CurrentNavigation.Current.Filtre.GetStringValue(); }
            set { RaisePropertyChanged("IsSelected"); }
        }

        /// <summary>
        /// Retourne le libelle du type de ressource
        /// </summary>
        public string Libelle { get; set; }

        public int Cle { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsModified { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsNewEntity { get; set; }
        private Boolean _isExpanded = false;
        public Boolean IsExpanded { get { return _isExpanded; } set { _isExpanded = value; RaisePropertyChanged("IsExpanded"); } }

        public bool IsNew()
        {
            return this.IsNewEntity;
        }

        private List<TypeRessource> _entities;
        public List<TypeRessource> Entities
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value;
                this.RaisePropertyChanged("Entities");
                this.RaisePropertyChanged("TypeRessources");
            }
        }

        #endregion

        #region Constructor

        public TypeRessource(string libelle)
        {
            Entities = new List<TypeRessource>();
            IsDeleted = false;
            Libelle = libelle;
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

        public override object GetIdentity()
        {
            return this.Libelle;
        }

        #endregion
    }
}
