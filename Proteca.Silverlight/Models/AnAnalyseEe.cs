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

namespace Proteca.Web.Models
{
    public partial class AnAnalyseEe
    {
        /// <summary>
        /// Retourne la date formaté
        /// </summary>
        public string DateAnalyseFormate
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", DateAnalyse);
            }
        }

        /// <summary>
        /// Retourne le libellé d'analyse
        /// </summary>
        public string LibelleAnalyse
        {
            get
            {
                if (this.EnsembleElectrique != null)
                {
                    return "Analyse n°"+ this.CleAnalyse + " " + this.DateAnalyseFormate;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Property privée pour les ensembles électriques
        /// </summary>
        private GeoEnsembleElectrique _geoEnsElec = null;

        /// <summary>
        /// Retourne les ensembles électriques
        /// </summary>
        public GeoEnsembleElectrique GeoEnsElec
        {
            get
            {
                if (this.EnsembleElectrique != null)
                {
                    _geoEnsElec = new GeoEnsembleElectrique() { CleEnsElectrique = this.CleEnsElectrique, Libelle = this.EnsembleElectrique.Libelle };
                }
                return _geoEnsElec;
            }
            set
            {
                _geoEnsElec = value;
                if (value != null)
                {
                    this.CleEnsElectrique = value.CleEnsElectrique;
                }
                else
                {
                    this.CleEnsElectrique = 0;
                }
                this.RaisePropertyChanged("GeoEnsElec");
            }
        }
    }
}
