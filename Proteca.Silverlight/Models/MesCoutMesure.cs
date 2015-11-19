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
    public partial class MesCoutMesure
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

        public bool IsPP
        {
            get
            {
                return this.TypeEquipement != null && this.TypeEquipement.CodeEquipement == "PP";
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
            if (e.PropertyName == "TypeEquipement")
            {
                RaisePropertyChanged("IsPP");
                if (!IsPP)
                {
                    this.RefEnumValeur1 = null;
                    this.RefEnumValeur2 = null;
                }
            }
        }
    }
}
