using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;

namespace Proteca.Web.Models
{
    /// <summary>
    /// Classe de visualisation des Mesures de la visite pour l'écran de Validation des visites
    /// </summary>
    public class VisiteMesure
    {
        /// <summary>
        /// Champs du label contient le Libelle du type de mesure et l'unité
        /// </summary>
        public string Libelle { get; set; }

        /// <summary>
        /// Indique si la valeur Maxi est saisissable
        /// </summary>
        public Boolean IsMaxiEnable { get; set; }

        /// <summary>
        /// Valeur maximale du niveau de protection associé
        /// </summary>
        public MesMesure Maxi { get; set; }

        /// <summary>
        /// Indique si la valeur Moyenne est saisissable
        /// </summary>
        public Boolean IsMoyenEnable { get; set; }

        /// <summary>
        /// Valeur moyenne ou mesurée
        /// </summary>
        public MesMesure Moyen { get; set; }

        /// <summary>
        /// Indique si la valeur Mini est saisissable
        /// </summary>
        public Boolean IsMiniEnable { get; set; }

        /// <summary>
        /// Valeur minimale du niveau de protection associé
        /// </summary>
        public MesMesure Mini { get; set; }

        /// <summary>
        /// Valeur de la mesure précédente 
        /// </summary>
        public MesMesure Precedente { get; set; }
    }
}
