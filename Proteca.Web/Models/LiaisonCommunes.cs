
using System;
namespace Proteca.Web.Models
{
    public class LiaisonCommunes
    {

        #region Public Properties
        
        /// <summary>
        /// Libelle de la liaison
        /// </summary>
        public string LibelleLiaison { get; set; }

        /// <summary>
        /// Libelle de la portion
        /// </summary>
        public string LibellePortion { get; set; }

        /// <summary>
        /// Identifiant de l'équipement
        /// </summary>
        public int CleEquipement { get; set; }

        /// <summary>
        /// Identifiant de la portion
        /// </summary>
        public int ClePortion { get; set; }

        /// <summary>
        /// Type de l'équipement
        /// </summary>
        public string TypeEquipement { get; set; }

        #endregion Public Properties

        #region Conctructor

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public LiaisonCommunes()
        {
            LibelleLiaison = string.Empty;
            LibellePortion = string.Empty;
            TypeEquipement = string.Empty;
        }

        /// <summary>
        /// constructeur avec Libelle de la liaison et Libelle de la portion
        /// </summary>
        /// <param name="libelleLiaison"></param>
        /// <param name="libellePortion"></param>
        /// <param name="typeEquip"></param>
        public LiaisonCommunes(string libelleLiaison, string libellePortion, string typeEquip)
        {
            LibelleLiaison = libelleLiaison;
            LibellePortion = libellePortion;
            TypeEquipement = typeEquip;
        }

        #endregion Constructor
    }
}
