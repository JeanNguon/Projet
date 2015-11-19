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

namespace Proteca.Web.Models
{
    public partial class Alerte
    {
        public Visite DerivedVisite
        {
            get
            {
                if (CleVisite.HasValue)
                {
                    return Visite;
                }
                else if (CleMesure.HasValue)
                {
                    return MesMesure != null ? MesMesure.Visite : null;
                }
                else if (CleAnalyse.HasValue)
                {
                    return AnAnalyseSerieMesure != null ? AnAnalyseSerieMesure.Visite : null;
                }
                return null;
            }
        }

        public String DerivedCommentaire
        {
            get
            {
                if (CleMesure.HasValue)
                {
                    if(MesMesure != null && MesMesure.MesTypeMesure != null && MesMesure.MesTypeMesure.MesModeleMesure != null)
                    {
                        MesNiveauProtection niv = null;
                        if (PP != null)
	                    {
		                    niv = MesMesure.MesTypeMesure.MesModeleMesure.MesNiveauProtection.FirstOrDefault(n=>n.ClePortion.HasValue && n.ClePortion.Value == PP.ClePortion);
	                    }
                        if (niv == null && this.Visite != null && this.Visite.ClePp.HasValue)
                        {
                            niv = MesMesure.MesTypeMesure.MesModeleMesure.MesNiveauProtection.FirstOrDefault(n => n.ClePp.HasValue && n.ClePp.Value == this.Visite.ClePp.Value);
                        }
                        else if (niv == null && this.Visite != null && this.Visite.CleEquipement.HasValue)
                        {
                            niv = MesMesure.MesTypeMesure.MesModeleMesure.MesNiveauProtection.FirstOrDefault(n => n.CleEquipement.HasValue && n.CleEquipement.Value == this.Visite.CleEquipement.Value);
                        }
                        if (niv == null)
                        {
                            niv = MesMesure.MesTypeMesure.MesModeleMesure.MesNiveauProtection.FirstOrDefault(n=> !n.ClePortion.HasValue && !n.ClePp.HasValue && !n.CleEquipement.HasValue);
                        }
                        String unite = String.Empty;
                        if (MesMesure.MesTypeMesure.MesModeleMesure.MesUnite != null)
                        {
                            unite = MesMesure.MesTypeMesure.MesModeleMesure.MesUnite.Symbole;
                        }
                        String commentaire = MesMesure.MesTypeMesure.LibTypeMesure + " : " + MesMesure.Valeur.ToString() + " " + unite;

                        if (niv != null)
                        {
                            commentaire += "&#13; Seuil Minimum : " + niv.SeuilMini.ToString() + " " + unite;
                            commentaire += " Seuil Maximum : " + niv.SeuilMaxi.ToString() + " " + unite;
                        }
                        
                        return commentaire;
                    }
                    return String.Empty;
                }
                else if (CleAnalyse.HasValue)
                {
                    return AnAnalyseSerieMesure != null ? AnAnalyseSerieMesure.RefEnumValeur.Libelle + " : " + AnAnalyseSerieMesure.Commentaire : String.Empty;
                }
                else if (CleVisite.HasValue)
                {
                    return Visite.Commentaire;
                }
                return String.Empty;
            }
        }

        public Pp PP
        {
            get
            {
                if (DerivedVisite != null && (DerivedVisite.Pp != null || (DerivedVisite.EqEquipement != null && DerivedVisite.EqEquipement.Pp != null)))
                {
                    return DerivedVisite.Pp != null ? DerivedVisite.Pp : DerivedVisite.EqEquipement.Pp;
                }

                return null;
            }
        }

        public String OuvrageLib
        {
            get
            {
                if (DerivedVisite != null && (DerivedVisite.Pp != null || DerivedVisite.EqEquipement != null))
                {
                    return DerivedVisite.Pp != null ? DerivedVisite.Pp.Libelle : DerivedVisite.EqEquipement.Libelle;
                }

                return String.Empty;
            }
        }

        public String PortionLib
        {
            get
            {
                if (PP != null && PP.PortionIntegrite != null)
                {
                    return PP.PortionIntegrite.Libelle;
                }

                return String.Empty;
            }
        }
    }
}
