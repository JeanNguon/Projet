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
using System.ComponentModel.Composition;

namespace Proteca.Web.Models
{
    public partial class TourneePpEq
    {
        

        private IOuvrage _ouvrage;
        
        public IOuvrage Ouvrage
        {
            get 
            {
                return _ouvrage;
            }
            set 
            {
                _ouvrage = value; 
            }
        }


        /// <summary>
        /// Renvoi la cle de l'equipement , qu'il s'agissent d'une PP ou  d'un Equipement
        /// </summary>
        public int? CleEqui
        {
            get {
                if (this.ClePp.HasValue)
                {
                    return this.ClePp;
                }
                else
                {
                    return this.CleEquipement;
                }
            }
        }

        public bool IsPP
        {
            get
            {
                return this.ClePp.HasValue;
            }
        }
        
        ///// <summary>
        ///// PP de rattachement sous forme de String
        ///// </summary>
        //public string Pp
        //{
        //    get
        //    {
        //        if (IsPP)
        //        {
        //            return ((Pp)this.Ouvrage).Libelle;
        //        }
        //        else
        //        {
        //            return ((EqEquipement)this.Ouvrage).Pp.Libelle;
        //        }
        //    }
        //}

        /// <summary>
        /// Retourne la date de saisie de la visite
        /// </summary>
        public string DateSaisie
        {
            get
            {
                if (this.Ouvrage != null)
                {
                    if (this.ClePp.HasValue && ((Pp)this.Ouvrage).LastVisite != null && ((Pp)this.Ouvrage).LastVisite.DateSaisie != null)
                    {
                        return string.Format("Saisie le {0}", ((Pp)this.Ouvrage).LastVisite.DateSaisie.Value.Date.ToShortDateString());
                    }
                    else if (this.CleEquipement.HasValue && ((EqEquipement)this.Ouvrage).LastVisite != null && ((EqEquipement)this.Ouvrage).LastVisite.DateSaisie != null)
                    {
                        return string.Format("Saisie le {0}", ((EqEquipement)this.Ouvrage).LastVisite.DateSaisie.Value.Date.ToShortDateString());
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Retourne la date de validation de la visite
        /// </summary>
        public string DateValidation
        {
            get
            {
                if (this.Ouvrage != null)
                {
                    if (this.ClePp.HasValue && ((Pp)this.Ouvrage).LastVisite != null && ((Pp)this.Ouvrage).LastVisite.DateValidation != null)
                    {
                        return string.Format("Validé le {0}", ((Pp)this.Ouvrage).LastVisite.DateValidation.Value.Date);
                    }
                    else if (this.CleEquipement.HasValue && ((EqEquipement)this.Ouvrage).LastVisite != null && ((EqEquipement)this.Ouvrage).LastVisite.DateValidation != null)
                    {
                        return string.Format("Validé le {0}", ((EqEquipement)this.Ouvrage).LastVisite.DateValidation.Value.Date);
                    }
                }
                return string.Empty;
            }
        }

        //public string EnsElec
        //{
        //    get
        //    {
        //        if (IsPP)
        //            return ((Pp)this.Ouvrage).PortionIntegrite.EnsembleElectrique.Libelle;
        //        else
        //            return ((EqEquipement)this.Ouvrage).Pp.PortionIntegrite.EnsembleElectrique.Libelle;
        //    }
        //}

        //public string Portion
        //{
        //    get
        //    {
        //        if (IsPP)
        //            return ((Pp)this.Ouvrage).PortionIntegrite.Libelle;
        //        else
        //            return ((EqEquipement)this.Ouvrage).Pp.PortionIntegrite.Libelle;
        //    }
        //}

        //public string PKSecteur
        //{
        //    get
        //    {
        //        if (IsPP)
        //            return string.Format("{0} / {1}", ((Pp)this.Ouvrage).Pk.ToString(),
        //                ((Pp)this.Ouvrage).GeoSecteur.LibelleSecteur);
        //        else
        //            return string.Format("{0} / {1}", ((EqEquipement)this.Ouvrage).Pp.Pk.ToString(),
        //                ((EqEquipement)this.Ouvrage).Pp.GeoSecteur.LibelleSecteur);
        //    }
        //}
    }
}
