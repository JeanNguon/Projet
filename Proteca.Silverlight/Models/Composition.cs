using System;
using System.ServiceModel.DomainServices.Client;
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
    public partial class Composition
    {
        #region properties

        private int _entityIndex;

        /// <summary>
        /// Id secondaire pour le mode déconnecté
        /// </summary>
        public int EntityIndex 
        {
            get { return this._entityIndex; }
            set { this._entityIndex = value; }
        }

        /// <summary>
        /// Libelle de l'ensemble électrique
        /// </summary>
        public string LibelleEe
        {
            get
            {
              
                if (this.CleEnsElectrique.HasValue && this.EnsembleElectrique != null)
                    return this.EnsembleElectrique.Libelle;
                else if (this.ClePp.HasValue && this.Pp != null)
                    return this.Pp.PortionIntegrite.EnsembleElectrique.Libelle;
                else if (this.CleEquipement.HasValue && this.EqEquipement != null && this.EqEquipement.Pp != null)
                    return this.EqEquipement.Pp.PortionIntegrite.EnsembleElectrique.Libelle;
                else if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null && this.EqEquipementTmp.Pp != null)
                    return this.EqEquipementTmp.Pp.PortionIntegrite.EnsembleElectrique.Libelle;
                else if (this.ClePortion.HasValue && PortionIntegrite != null && this.PortionIntegrite.EnsembleElectrique != null)
                    return this.PortionIntegrite.EnsembleElectrique.Libelle;
                else 
                    return String.Empty;
            }
        }

        /// <summary>
        /// Libellé de la portion intégrité
        /// </summary>
        public string LibellePortion
        {
            get
            {
            
                if (this.CleEnsElectrique.HasValue && this.EnsembleElectrique != null)
                    return String.Empty;
                else if (this.ClePp.HasValue && this.Pp != null)
                    return this.Pp.PortionIntegrite.Libelle;
                else if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null && this.EqEquipementTmp.Pp != null)
                    return this.EqEquipementTmp.Pp.PortionIntegrite.Libelle;
                else if (this.CleEquipement.HasValue && this.EqEquipement != null && this.EqEquipement.Pp != null)
                    return this.EqEquipement.Pp.PortionIntegrite.Libelle;
                else if (this.ClePortion.HasValue && PortionIntegrite != null)
                    return this.PortionIntegrite.Libelle;
                else 
                    return String.Empty;
            }
        }

        public string Libelle
        {
            get
            {
              
                if (this.CleEnsElectrique.HasValue && this.EnsembleElectrique != null)
                    return this.EnsembleElectrique.Libelle;
                else if (this.CleEquipement.HasValue && this.EqEquipement != null)
                    return this.EqEquipement.Libelle;
                else if (this.ClePortion.HasValue && this.PortionIntegrite != null)
                    return this.PortionIntegrite.Libelle;
                else if (this.ClePp.HasValue && this.Pp != null)
                    return this.Pp.LibellePPNiveauSensibilite2;
                else if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null)
                    return this.EqEquipementTmp.Libelle;
                else
                    return String.Empty;
            }
        }

        public bool HasDeleted { get
        {
            if (this.EqEquipement != null && this.EqEquipement.Supprime) return true;
            if (this.Pp != null && this.Pp.Supprime) return true;
            if (this.PortionIntegrite != null && this.PortionIntegrite.Supprime) return true;
            if (this.Tournee != null && this.Tournee.Supprime) return true;
            return false;

        }  }

        public string LibelleCourt
        {
            get
            {
                if (HasDeleted) return String.Empty;
                if (this.CleEnsElectrique.HasValue && this.EnsembleElectrique != null )
                    return this.EnsembleElectrique.Libelle;
                else if (this.CleEquipement.HasValue && this.EqEquipement != null )
                    return this.EqEquipement.Libelle;
                else if (this.ClePortion.HasValue && this.PortionIntegrite != null )
                    return this.PortionIntegrite.Libelle;
                else if (this.ClePp.HasValue && this.Pp != null)
                    return this.Pp.Libelle;
                else if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null)
                    return this.EqEquipementTmp.Libelle;
                else
                    return String.Empty;
            }
        }

        /// <summary>
        /// Retourne le type de la composition
        /// </summary>
        public string Type
        {
            get
            {
               
                if (this.CleEnsElectrique.HasValue)
                    return "EE";
                else if (this.CleEquipement.HasValue && this.EqEquipement != null && this.EqEquipement.TypeEquipement != null )
                    return this.EqEquipement.TypeEquipement.CodeEquipement;
                else if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null && this.EqEquipementTmp.TypeEquipement != null)
                    return this.EqEquipementTmp.TypeEquipement.CodeEquipement;
                else if (this.ClePortion.HasValue)
                    return "PI";
                else if (this.ClePp.HasValue)
                    return "PP";
                else
                    return "";
            }
        }

        /// <summary>
        /// Retourne la PK de la composition
        /// </summary>
        public string Pk
        {
            get
            {
     
                if (this.CleEquipement.HasValue && this.EqEquipement != null && this.EqEquipement.Pp != null )
                    return this.EqEquipement.Pp.Pk.ToString("0.###");
                if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null && this.EqEquipementTmp.Pp != null)
                    return this.EqEquipementTmp.Pp.Pk.ToString("0.###");
                else if (this.ClePp.HasValue && this.Pp != null )
                    return this.Pp.Pk.ToString("0.###");
                else
                    return String.Empty;
            }
        }

        public override string ToString()
        {
            return this.Libelle;
        }

        public IOuvrage Ouvrage
        {
            get
            {
           
                if (this.ClePp.HasValue)
                    return this.Pp;
                else if (this.CleEquipement.HasValue)
                    return this.EqEquipement;
                else if (this.CleEqTmp.HasValue)
                    return this.EqEquipementTmp;
                else
                    return null;
            }
        }

        #endregion
    }
}
