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
using Proteca.Silverlight.Models;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.Text.RegularExpressions;
using Proteca.Silverlight.Helpers;
using System.ComponentModel;

namespace Proteca.Web.Models
{
    public partial class CategoriePp
    {
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew)
            {
                _numeroOrdreNullable = this.NumeroOrdre;
            }
        }

        #region Gestion des valeurs par défaut des champs obligatoires

        private Nullable<int> _numeroOrdreNullable;
        [RequiredCustom(AllowZero=true)]
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
            else if (e.PropertyName == "NumeroOrdre")
            {
                RaisePropertyChanged("NumeroOrdreNullable");
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

        private Boolean _isEditableLine = false;
        public Boolean IsEditableLine
        {
            get { return _isEditableLine; }
            set { _isEditableLine = value; RaisePropertyChanged("IsEditableLine"); }
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
    }
}
