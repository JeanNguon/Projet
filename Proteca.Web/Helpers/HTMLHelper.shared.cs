using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Proteca.Web.Helpers
{
    public class HTMLHelper
    {
        //public static string comments = @"<!--(.|\n)*?-->";
        //public static string tagsAndContent = @"<{0}(.|\n)*?</{1}>";
        //public static string allTags = @"<[^>]*>";

        /// <summary>
        /// Fonction de suppression des balises html du text avec supression du contenu si besoin par balise
        /// </summary>
        /// <param name="html">Texte encodé en html (ou du moins en xml)</param>
        /// <param name="tagsContentToStrip">Liste de nom de balises pour lequelles on veut supprimer aussi le contenu</param>
        /// <returns>Texte formaté en texte ne contenant que le contenu des balises non sélectionnées</returns>
        public static String StripHtml(String html, List<String> tagsContentToStrip = null)
        {
            // Initialisation de la variable de sortie
            String sortie = String.Empty;

            // Si pas de html ou sting vide on retourne string.Empty
            if (html == null || String.IsNullOrEmpty(html))
            {
                return sortie;
            }
            // Sinon on strip le html 
            else
            {
                sortie = html;

                // On essaye d'abord de supprimer du html les contenus commentés
                sortie = Regex.Replace(sortie, "<!--(.|\n)*?-->", String.Empty);

                // Ensuite on vient supprimer les contenus des balises données en argument
                tagsContentToStrip = tagsContentToStrip ?? new List<String>();
                foreach(String tag in tagsContentToStrip)
                {
                    sortie = Regex.Replace(sortie, String.Format("<{0}(.|\n)*?</{0}>", tag), String.Empty);
                }

                // Puis on supprime le reste des balises et on renvois le resultat
                return Regex.Replace(sortie, "<[^>]*>", String.Empty);
            }
        }

        //public static String StripHtml(String html)
        //{
        //    return StripHtml(html, new List<String>());
        //}
    }
}
