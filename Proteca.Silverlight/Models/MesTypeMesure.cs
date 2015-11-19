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
using System.Text.RegularExpressions;
using Proteca.Silverlight.Helpers;

namespace Proteca.Web.Models
{
    public partial class MesTypeMesure
    {
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew())
            {
                _numeroOrdreNullable = this.NumeroOrdre;
                _niveauTypeNullable = this.NiveauType;
                _typeEvaluationNullable = this.TypeEvaluation;
            }
        }

        #region Gestion des valeurs par défaut des champs obligatoires

        private Nullable<int> _numeroOrdreNullable;
        [RequiredCustom(AllowZero = true)]
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

        private Nullable<int> _typeEvaluationNullable;
        [RequiredCustom()]
        public Nullable<int> TypeEvaluationNullable
        {
            get
            {
                return _typeEvaluationNullable;
            }
            set
            {
                if (_typeEvaluationNullable != value)
                {
                    this.ValidateProperty("TypeEvaluationNullable", value);

                    if (value.HasValue)
                    {
                        this.TypeEvaluation = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("TypeEvaluation");
                    }
                    _typeEvaluationNullable = value;
                }
            }
        }

        private Nullable<int> _niveauTypeNullable;
        [RequiredCustom()]
        public Nullable<int> NiveauTypeNullable
        {
            get
            {
                return _niveauTypeNullable;
            }
            set
            {
                if (_niveauTypeNullable != value)
                {
                    this.ValidateProperty("NiveauTypeNullable", value);

                    if (value.HasValue)
                    {
                        this.NiveauType = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("NiveauType");
                    }
                    _niveauTypeNullable = value;
                }
            }
        }

        #endregion

        public override string ToString()
        {
            return LibTypeMesure;
        }


        /// <summary>
        /// On force la MAJ des propriétés
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "EntityState")
            {
                if (this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
                {
                    if (this.IsNew())
                    {
                        _numeroOrdreNullable = null;
                        _niveauTypeNullable = null;
                        _typeEvaluationNullable = null;
                    }
                    else
                    {
                        _numeroOrdreNullable = this.NumeroOrdre;
                        _niveauTypeNullable = this.NiveauType;
                        _typeEvaluationNullable = this.TypeEvaluation;
                    }
                    this.RaisePropertyChanged("NumeroOrdreNullable");
                    this.RaisePropertyChanged("NiveauTypeNullable");
                    this.RaisePropertyChanged("TypeEvaluationNullable");
                }
            }
        }
    }
}
