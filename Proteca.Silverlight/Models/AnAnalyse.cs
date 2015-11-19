using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    public partial class AnAnalyse
    {
        /// <summary>
        /// Retourne la source de l'image à afficher suivant l'état de l'analyse
        /// </summary>
        public string EtatSourceImage
        {
            get
            {
                string result = String.Empty;
                if (this.RefEnumValeur != null)
                {
                    switch (this.RefEnumValeur.Valeur)
                    {
                        case "01":
                            result = ResourceImg.Flag_Green;
                            break;
                        case "02":
                            result = ResourceImg.Flag_Yellow;
                            break;
                        case "03":
                            result = ResourceImg.Flag_Red;
                            break;
                        default:
                            break;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// On force la MAJ des propriétés
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "EnumEtatPc")
            {
                RaisePropertyChanged("EtatSourceImage");
            }
        }

        public void ForceRaisePropertyChanged(String propertyName)
        {
            this.RaisePropertyChanged(propertyName);
        }
    }
}
