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
using Proteca.Silverlight.Models;
using Jounce.Core.ViewModel;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;

namespace Proteca.Web.Models
{
    public partial class RefParametre
    {
        public Object ObjValue
        {
            get
            {
                switch (TypeDeDonnee)
                {
                    case ("MESMODELEMESURE"):
                    case ("INT"):
                        return Convert.ToInt32(Valeur);
                    case ("BOOLEAN"):
                        return (Convert.ToInt32(Valeur) == 1);
                    case ("DOUBLE"):
                        return Convert.ToDouble(Valeur.Replace('.', ','));
                    case ("STRING"):
                        return Valeur;
                    default:
                        return Valeur;
                }
            }
            set
            {
                switch (TypeDeDonnee)
                {
                    case ("BOOLEAN"):
                        Boolean boolVal;
                        if (!Boolean.TryParse(value.ToString(), out boolVal))
                        {
                            Valeur = value.ToString();
                        }
                        else
                        {
                            Valeur = Convert.ToInt32(value).ToString();
                        }
                        break;
                    default:
                        if (value != null)
                        {
                            Valeur = value.ToString();
                        }
                        break;
                }
            }
        }
    }
}
