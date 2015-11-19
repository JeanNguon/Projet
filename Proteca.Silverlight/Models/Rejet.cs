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
using System.Collections.ObjectModel;
using Jounce.Core.ViewModel;
using Jounce.Framework.Command;
using Jounce.Core.Command;

namespace Proteca.Silverlight.Models
{
    public class Rejet : BaseViewModel
    {
        public Nullable<int> CleVisite { get; set; }
        public Nullable<int> ClePpTmp { get; set; }
        public Nullable<int> CleEqTmp { get; set; }

        public string LibellePortion { get; set; }
        public string CodeEquipement { get; set; }
        public string LibelleOuvrage { get; set; }
        public Nullable<DateTime> DateVisite { get; set; }
        public string TypeEval { get; set; }
        public string PpTmpContent { get; set; }
        public string VisiteContent { get; set; }
    }
}
