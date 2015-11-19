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

namespace Proteca.Silverlight.Helpers
{
    public static class StringExtension
    {
        /// <summary>
        /// Converter utilisé pour ajouter automatiquement un "*" si le champ est obligatoire en se basant sur le parameter passé au converter 
        /// Ce paramètre doit êter un booléen. Si vrai alors le champ est obligatoire et le libellé doit s'afficher avec un *. Dans le cas contraire on supprime l'étoile si présente dans le libellé
        /// </summary>
        /// <param name="label"></param>
        /// <param name="isRequired"></param>
        public static string ToRequiredLabel(this string label, Boolean isRequired)
        {
            string stringValue = label;
            if (stringValue != null)
            {
              
                if (isRequired && !stringValue.Contains("*"))
                {
                    if (stringValue.Contains(":"))
                    {
                        stringValue = stringValue.Replace(":", "*:");
                    }
                    else
                    {
                        stringValue += "*";
                    }
                }
                else if (!isRequired && stringValue.Contains("*"))
                {
                    stringValue = stringValue.Replace("*", "");
                }
            }
            return stringValue;
        }
    }
}
