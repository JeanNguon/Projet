using System;
using System.Collections.Generic;
using System.Linq;

namespace Proteca.Web.Models
{
    public partial class MesMesure
    {
        /// <summary>
        ///  Récuppère les seuils à appliquer
        /// </summary>
        private MesNiveauProtection _niveauProtection;
        public MesNiveauProtection NiveauProtection
        {
            get
            {
                if (_niveauProtection == null
                    && this.MesTypeMesure != null
                    && this.MesTypeMesure.MesModeleMesure != null
                    && this.MesTypeMesure.MesModeleMesure.MesNiveauProtection.Any()
                    && this.Visite != null)
                {
                    int clePortion = 0;
                    int clePP = 0;
                    if (this.Visite.ClePp.HasValue && this.Visite.Pp != null)
                    {
                        clePortion = this.Visite.Pp.ClePortion;
                        clePP = this.Visite.ClePp.Value;
                    }
                    else if (this.Visite.ClePpTmp.HasValue && this.Visite.PpTmp != null && this.Visite.PpTmp.Pp != null)
                    {
                        clePortion = this.Visite.PpTmp.Pp.ClePortion;
                        clePP = this.Visite.PpTmp.ClePp;
                    }
                    else if (this.Visite.CleEquipement.HasValue && this.Visite.EqEquipement != null && this.Visite.EqEquipement.Pp != null)
                    {
                        clePortion = this.Visite.EqEquipement.Pp.ClePortion;
                        clePP = this.Visite.EqEquipement.ClePp;
                    }
                    else if (this.Visite.CleEqTmp.HasValue && this.Visite.EqEquipementTmp != null && this.Visite.EqEquipementTmp.Pp2 != null)
                    {
                        clePortion = this.Visite.EqEquipementTmp.Pp2.ClePortion;
                        clePP = this.Visite.EqEquipementTmp.ClePp;
                    }

                    int cleEq = this.Visite.CleEquipement.HasValue ? this.Visite.CleEquipement.Value : -1;

                    _niveauProtection = this.MesTypeMesure.MesModeleMesure.MesNiveauProtection
                        .Where(n => n.ClePortion == clePortion || n.ClePp == clePP || n.CleEquipement == cleEq || (!n.ClePortion.HasValue && !n.ClePp.HasValue && !n.CleEquipement.HasValue))
                        .OrderByDescending(n => n.CleEquipement).ThenByDescending(n => n.ClePp).ThenByDescending(n => n.ClePortion)
                        .FirstOrDefault();
                }

                return _niveauProtection;
            }
        }

        /// <summary>
        ///  Indique si la valeur dépasse un seuil
        /// </summary>
        public Boolean IsDepassementSeuil
        {
            get
            {
                if (Valeur.HasValue
                && NiveauProtection != null
                && (
                        (NiveauProtection.SeuilMini.HasValue && !NiveauProtection.SeuilMaxi.HasValue && Valeur.Value < NiveauProtection.SeuilMini)
                        ||
                        (NiveauProtection.SeuilMaxi.HasValue && !NiveauProtection.SeuilMini.HasValue && Valeur.Value > NiveauProtection.SeuilMaxi)
                        ||
                        (NiveauProtection.SeuilMini.HasValue && NiveauProtection.SeuilMaxi.HasValue && (Valeur.Value < NiveauProtection.SeuilMini || Valeur.Value > NiveauProtection.SeuilMaxi))
                    ))
                {
                    return true;
                }

                return false;
            }
        }
    }
}