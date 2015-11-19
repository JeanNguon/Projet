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
using System.ServiceModel.DomainServices.Client;
using System.Xml.Linq;
using Proteca.Silverlight.Resources;

namespace Proteca.Web.Models
{
    public partial class MesMesure
    {
        #region Gestion du ActivateChangePropagation pour les visites

        public Dictionary<string, Entity> GetParentWithPropName()
        {
            Dictionary<string, Entity> retour = new Dictionary<string, Entity>();
            retour.Add("MesMesure", this.Visite);
            return retour;
        }

        #endregion

        public Alerte Alerte
        {
            get
            {
                return this.Alertes.FirstOrDefault();
            }
        }
        
        #region override Methodes

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == "Valeur")
            {
                RaisePropertyChanged("IsDepassementSeuil");
                RaisePropertyChanged("ValeurStringFormated");
            }
        }

        #endregion

        public XElement CreateXMesMesure()
        {
            XElement xMesMesure = new XElement("MesMesure");

            //xMesMesure.Add(new XElement("ModeleMesureLib", (MesTypeMesure != null && MesTypeMesure.MesModeleMesure != null) ? MesTypeMesure.MesModeleMesure.LibGenerique : null));
            xMesMesure.Add(new XElement("Valeur", Valeur));
            xMesMesure.Add(new XElement("CleTypeMesure", CleTypeMesure));

            return xMesMesure;
        }

        public String MesMesureToText
        {
            get
            {
                if (this.MesTypeMesure != null
                    && this.MesTypeMesure.RefEnumValeur != null
                    && this.MesTypeMesure.MesModeleMesure != null
                    && this.MesTypeMesure.MesModeleMesure.MesUnite != null)
                {
                    return this.MesTypeMesure.MesModeleMesure.LibGenerique + " ("
                    + this.MesTypeMesure.RefEnumValeur.Libelle.ToLower() + ") : "
                    + this.Valeur + " "
                    + this.MesTypeMesure.MesModeleMesure.MesUnite.Symbole;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// Retourne le format de l'unité à utiliser
        /// </summary>
        public String StringFormat
        {
            get 
            {
                String result = "0.###";
                if (this.MesTypeMesure != null && this.MesTypeMesure.MesModeleMesure != null && this.MesTypeMesure.MesModeleMesure.MesUnite != null)
                {
                    result = this.MesTypeMesure.MesModeleMesure.MesUnite.UniteFormat;
                }
                return "{0:" + result + "}";
            }
        }

        /// <summary>
        /// Retourne la Valeur de la mesure formatée correctement par rapport à son unité
        /// </summary>
        public String ValeurStringFormated
        {
            get
            {
                if (this.Valeur.HasValue)
                {
                    return String.Format(this.StringFormat, this.Valeur.Value);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}
