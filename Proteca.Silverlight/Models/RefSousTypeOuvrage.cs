using System;
using System.Text.RegularExpressions;
using Proteca.Silverlight.Helpers;

namespace Proteca.Web.Models
{
    public partial class RefSousTypeOuvrage
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
            else if (e.PropertyName == "EntityState")
            {
                if (this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
                {
                    if (IsNew)
                    {
                        _numeroOrdreNullable = null;
                    }
                    else
                    {
                        _numeroOrdreNullable = this.NumeroOrdre;
                    }
                    this.RaisePropertyChanged("NumeroOrdreNullable");
                }
            }
        }

        //public String NumeroOrdreBinding
        //{
        //    get
        //    {
        //        if (this.NumeroOrdre < 0)
        //        {
        //            return null;
        //        }
        //        return this.NumeroOrdre.ToString();
        //    }
        //    set
        //    {
        //        int intValue = -1;
        //        if (!string.IsNullOrWhiteSpace(value) && Regex.Match(value, @"(^\d*$)").Success)
        //        {
        //            Int32.TryParse(value, out intValue);
        //        }
        //        NumeroOrdre = intValue;
        //    }
        //}
        #region Gestion des valeurs par défaut des champs obligatoires
        private Nullable<int>_numeroOrdreNullable;
        [RequiredCustom()]
        public Nullable<int> NumeroOrdreNullable
        {
            get
            {
                return _numeroOrdreNullable;
            }
            set
            {
                if (_numeroOrdreNullable != value)
                {
                    this.ValidateProperty("NumeroOrdreNullable", value);

                    if (value.HasValue)
                    {
                        this.NumeroOrdre = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("NumeroOrdre");
                    }
                    _numeroOrdreNullable = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// Initialisation des données au chargement des écrans
        /// </summary>
        /// <param name="isInitialLoad"></param>
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew)
            {
                _numeroOrdreNullable = this.NumeroOrdre;
            }
        }
    }
}
