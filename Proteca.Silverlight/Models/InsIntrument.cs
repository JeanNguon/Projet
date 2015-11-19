using System.Linq;
using Proteca.Silverlight.Helpers;
using System;

namespace Proteca.Web.Models
{
    public partial class InsInstrument
    {
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
                return this.HasChanges || this.IsNew;
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

        /// <summary>
        /// Libelle de la zone géographique de rattachement
        /// </summary>
        public string Rattachement
        {
            get 
            {
                if(this.CleRegion.HasValue)
                {
                    return this.GeoRegion.LibelleRegion;
                }
                else if (this.CleAgence.HasValue)
                {
                    return this.GeoAgence.LibelleAgence;
                }
                else if (this.CleSecteur.HasValue)
                {
                    return this.GeoSecteur.LibelleSecteur;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Permet l'édition ou non de l'instrument
        /// </summary>
        private bool _isEditable;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged("IsEditable");
            }
        }
        
    }
}
