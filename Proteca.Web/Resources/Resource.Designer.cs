﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.34209
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proteca.Web.Resources {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Proteca.Web.Resources.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Impossible de supprimer l&apos;agence &quot;{0}&quot; :.
        /// </summary>
        internal static string DecoupageGeo_DeleteAgenceMainMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeleteAgenceMainMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à - Aucun secteur ne doit être rattaché à cette agence.
        /// </summary>
        internal static string DecoupageGeo_DeleteAgenceSecteurMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeleteAgenceSecteurMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à - Aucun utilisateur ne doit être rattaché à cette agence.
        /// </summary>
        internal static string DecoupageGeo_DeleteAgenceUserMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeleteAgenceUserMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à - Les instruments rattachés doivent être supprimés..
        /// </summary>
        internal static string DecoupageGeo_DeleteInstrumentMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeleteInstrumentMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à - Aucune portion intégrité ne doit être rattachée à ce secteur.
        /// </summary>
        internal static string DecoupageGeo_DeletePiMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeletePiMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à - Aucune prise de potentiel ne doit être rattachée à ce secteur .
        /// </summary>
        internal static string DecoupageGeo_DeletePpsMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeletePpsMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à - Aucune agence ne doit être rattachée à cette région.
        /// </summary>
        internal static string DecoupageGeo_DeleteRegionAgenceMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeleteRegionAgenceMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Impossible de supprimer la région &quot;{0}&quot; :.
        /// </summary>
        internal static string DecoupageGeo_DeleteRegionMainMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeleteRegionMainMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Impossible de supprimer le secteur &quot;{0}&quot; :.
        /// </summary>
        internal static string DecoupageGeo_DeleteSecteurMainMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeleteSecteurMainMsgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à - Aucun utilisateur ne doit être rattaché à ce secteur.
        /// </summary>
        internal static string DecoupageGeo_DeleteSecteurUserMsgError {
            get {
                return ResourceManager.GetString("DecoupageGeo_DeleteSecteurUserMsgError", resourceCulture);
            }
        }
    }
}
