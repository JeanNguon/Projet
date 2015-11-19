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

namespace Proteca.Silverlight
{
    [Export]
    public class Global
    {
        public static class Constants
        {
            #region APPLICATION_DEFAULT_VALUE

            /// <summary>
            /// Retourne la valeur par défaut du paramètre d'état
            /// </summary>
            public const string PARM_STATE = "State";

            /// <summary>
            /// Retourne la valeur par défaut du paramètre déifnissant l'ID dans l'URL
            /// </summary>
            public const string PARM_ID = "Id";

            /// <summary>
            /// Retourne la valeur par défaut du paramètre déifnissant l'ID dans l'URL
            /// </summary>
            public const string PARM_ID_TMP = "IdTmp";

            /// <summary>
            /// Retourne la valeur par défaut de l'état de fermeture
            /// </summary>
            public const string STATE_CLOSED = "Closed";

            /// <summary>
            /// Retourne la valeur par défaut de l'état Nouveau
            /// </summary>
            public const string STATE_NEW = "New";

            /// <summary>
            /// Retourne la valeur par défaut de l'état d'édition
            /// </summary>
            public const string STATE_EDIT = "Edit";

            /// <summary>
            /// Retourne la valeur par défaut du paramètre déifnissant le texte de recherche dans l'URL
            /// </summary>
            public const string PARM_SEARCH_TEXT = "Text";

            #endregion

            #region ENTITY_DEFAULT_VALUE

            /// <summary>
            /// Retourne la valeur par défaut d'un profil utilisateur à "Suivi Secteur"
            /// </summary>
            public const string UsrProfil_DEFAULT = "Suivi Secteur";

            /// <summary>
            /// Retourne la valeur par défaut de la portée de gestion des comptes utilisateur à "05" (Interdite)
            /// </summary>
            public const string RefUsrPortee_DEFAULT = "05";

            /// <summary>
            /// Retourne la valeur par défaut du type des RefUsrPortee rapatrié (TypePortee = "USR")
            /// </summary>
            public const string RefUsrPortee_TYPE = "USR";

            #endregion
             
        }
    }
}
