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
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;

namespace Proteca.Web.Models
{
    public partial class RefCommune
    {
        /// <summary>
        /// Surcharge de la méthode tostring(), retourne le libelle
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Libelle;
        }
    }
}
