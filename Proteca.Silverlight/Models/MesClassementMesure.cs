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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Proteca.Web.Models
{
    public partial class MesClassementMesure
    {
        public override string ToString()
        {
            return MesTypeMesure.LibTypeMesure + MesTypeMesure.CleTypeMesure.ToString();
        }
    }
}
