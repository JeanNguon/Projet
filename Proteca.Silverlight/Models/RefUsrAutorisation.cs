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
    public partial class RefUsrAutorisation
    {
        // TODO ajouter un code sur Autorisation dans la base et le modèle puis saisir ces codes dans l'enum suivante
        public enum ListAutorisationsEnum
        {
            MAJ_PI_NIV,
            //RESTIT_PI,
            MAJ_EE_NIV,
            MAJ_EQ_NIV,
            //RESTIT_EQ,

            MAJ_TOURNEE_NIV,
            //MAJ_FICHE_VISITE_NIV,
            MAJ_INS_NIV,
            CREA_VISITE_NIV,
            IMPORT_FICHE_VISITE,

            MAJ_VISITE_NIV,
            //RESTIT_MESURES,
            //GRAPHES,
            //CLASS_SERIE_MES_NIV,
            ANALYSE_NIV,

            GENERATION_RAPPORT,
            GESTION_ALERTES_NIV,
            CREA_FICHE_ACTION,
            MAJ_FICHE_ACTION_NIV,
            //AUTRES,

            //ACCES_DOC,
            GESTION_DOC,
            SUP_DOC,
            VERROUIL_COORD_GPS,
            DEVERROUIL_COORD_GPS,

            GESTION_TIERS,
            IMPORT_TELEM,
            IMPORT_VALIDATION,
            VERROU_TOURNEE,
            SUP_FICHE_ACTION_NIV,

            CRE_FICHE_ACTION_NIV,
            GES_FICHE_ACTION_NIV
            //,ACCES_TAB_LIAIS_GRDF
        }
    }
}
