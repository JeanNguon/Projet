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
using Proteca.Silverlight.Enums;

namespace Proteca.Web.Models
{
    public partial class RefUsrPortee
    {
        public enum ListPorteesEnum
        {
            [StringValue("05")]
            Interdite,
            [StringValue("06")]
            Autorisee,
            [StringValue("01")]
            National,
            [StringValue("02")]
            Region,
            [StringValue("03")]
            Agence,
            [StringValue("04")]
            Secteur
        }

        public ListPorteesEnum GetPorteesEnum()
        {
            ListPorteesEnum result = ListPorteesEnum.Interdite;
            if (this.CodePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
            {
                result = ListPorteesEnum.Interdite;
            }
            else if (this.CodePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue())
            {
                result = ListPorteesEnum.Autorisee;
            }
            else if (this.CodePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
            {
                result = ListPorteesEnum.National;
            }
            else if (this.CodePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
            {
                result = ListPorteesEnum.Region;
            }
            else if (this.CodePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
            {
                result = ListPorteesEnum.Agence;
            }
            else if (this.CodePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
            {
                result = ListPorteesEnum.Secteur;
            }
            return result;
        }
    }
}
