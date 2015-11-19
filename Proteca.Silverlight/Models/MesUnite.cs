using System;
using System.Net;
using System.Reflection;
using System.Text;
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
    public partial class MesUnite
    {
        /// <summary>
        /// Indique si l'unité est de type Réel
        /// </summary>
        public bool IsRealType
        {
            get 
            {
                if (this.RefEnumValeur != null)
                    return this.RefEnumValeur.Valeur == "20";
                else
                    return false;
            }
        }

        /// <summary>
        /// On renvoie le libelle de l'unité
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Libelle;
        }

        /// <summary>
        /// Retourne le StringFormat de l'unité à utiliser
        /// </summary>
        public String UniteFormat
        {
            get
            {
                //Instanciation de la nouvelle chaine de format avec au moins un chiffre obligatoire
                int decimalNumber = this.NombreDeDecimales ?? 0;
                StringBuilder builder = new StringBuilder("0.");
                //Recherche dans les metadatas de la plus grande valeur de décimaux possible
                int maxValue = 3;
                int maxValueParsed = maxValue;
                PropertyInfo prop = this.GetType().GetProperty("NombreDeDecimales");
                var attributes = prop.GetCustomAttributes(typeof(RangeCustomAttribute), true);
                if (attributes.Any() && int.TryParse(((RangeCustomAttribute)attributes.FirstOrDefault()).Maximum, out maxValueParsed))
                {
                    maxValue = maxValueParsed;
                }
                //Majoration du nombre de décimales à la valeur max pour éviter les Exception ci-dessous
                if (decimalNumber > maxValue)
                {
                    decimalNumber = maxValue;
                }
                //Remplissage de la chaîne avec le bon nombre de decimaux obligatoires
                builder.Append(new String('0', decimalNumber));
                //Et on complète jusqu'au maximum avec des nombres facultatifs (si enregistrés en base ainsi)
                builder.Append(new String('#', maxValue - decimalNumber));
                return builder.ToString();
            }
        }
    }
}
