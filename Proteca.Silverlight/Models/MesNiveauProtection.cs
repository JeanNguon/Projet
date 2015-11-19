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
using System.Collections.Generic;
using Proteca.Web.Models;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Proteca.Web.Resources;
using System.Collections.ObjectModel;
using Proteca.Silverlight.Helpers;

namespace Proteca.Web.Models
{
    public partial class MesNiveauProtection
    {
        /// <summary>
        /// Duplique le niveau de protection
        /// </summary>
        /// <returns></returns>
        public MesNiveauProtection DupliquerMesNiveauProtection()
        {
            MesNiveauProtection tempitem = new MesNiveauProtection();
            tempitem = this.Clone();
            tempitem.CleNiveauProtection = 0;
            return tempitem;

            //return new MesNiveauProtection()
            //{
            //    MesModeleMesure = this.MesModeleMesure,
            //    SeuilMaxi = this.SeuilMaxi,
            //    SeuilMini = this.SeuilMini,

            //};

        }

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

        public void ForceValidateSeuil()
        {
            this.ValidateProperty("SeuilMini", SeuilMini);
            this.ValidateProperty("SeuilMaxi", SeuilMaxi);
        }

        /// <summary>
        /// On force la MAJ des propriétés
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "CleModeleMesure" || e.PropertyName == "MesModeleMesure")
            {
                RaisePropertyChanged("TypeEquipement");
                RaisePropertyChanged("StringFormat");
                RaisePropertyChanged("SeuilMaxiStringFormatted");
                RaisePropertyChanged("SeuilMiniStringFormatted");
            }

            if (e.PropertyName == "HasChanges")
            {
                RaisePropertyChanged("HasChangesOrIsNew");
            }
        }
        
        private TypeEquipement _typeEquipement;
        public TypeEquipement TypeEquipement 
        {
            get
            {
                if (MesModeleMesure != null && MesModeleMesure.TypeEquipement != null)
                {
                    _typeEquipement = MesModeleMesure.TypeEquipement;
                }
                return _typeEquipement;
            }
            set
            {
                _typeEquipement = value;
                this.RaisePropertyChanged("TypeEquipement");
                this.RaisePropertyChanged("ListeModeleMesure");
            }
        }

        private ObservableCollection<MesModeleMesure> _listeModeleMesure;
        public ObservableCollection<MesModeleMesure> ListeModeleMesure
        {
            get
            {
                if (TypeEquipement != null)
                {
                    return new ObservableCollection<MesModeleMesure>(_listeModeleMesure.Where(mm=>mm.CleTypeEq == TypeEquipement.CleTypeEq));
                }
                return new ObservableCollection<MesModeleMesure>();
            }
            set
            {
                _listeModeleMesure = value;
                this.RaisePropertyChanged("ListeModeleMesure");
            }
        }

        protected override void ValidateProperty(ValidationContext validationContext, object value)
        {
            base.ValidateProperty(validationContext, value);
        }

        /// <summary>
        /// Retourne le format de l'unité à utiliser
        /// </summary>
        public String StringFormat
        {
            get
            {
                String result = "0.###";
                if (this.MesModeleMesure != null && this.MesModeleMesure.MesUnite != null)
                {
                    result = this.MesModeleMesure.MesUnite.UniteFormat;
                }
                return "{0:" + result + "}";
            }
        }

        /// <summary>
        /// Retourne la Valeur de la mesure formatée correctement par rapport à son unité
        /// </summary>
        public String SeuilMaxiStringFormatted
        {
            get
            {
                if (this.SeuilMaxi.HasValue)
                {
                    return String.Format(this.StringFormat, this.SeuilMaxi.Value);
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if (value != null && value != string.Empty)
                {
                    this.SeuilMaxi = Convert.ToDecimal(value);
                }
                else
                {
                    this.SeuilMaxi = null;
                }
            }
        }

        /// <summary>
        /// Retourne la Valeur de la mesure formatée correctement par rapport à son unité
        /// </summary>
        public String SeuilMiniStringFormatted
        {
            get
            {
                if (this.SeuilMini.HasValue)
                {
                    return String.Format(this.StringFormat, this.SeuilMini.Value);
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if (value != null && value != string.Empty)
                {
                    this.SeuilMini = Convert.ToDecimal(value);
                }
                else
                {
                    this.SeuilMini = null;
                }
              
            }
        }
    }
}
