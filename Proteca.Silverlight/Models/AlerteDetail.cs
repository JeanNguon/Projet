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

namespace Proteca.Web.Models
{
    public partial class AlerteDetail
    {
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);
            CanDisable = !Supprime;
        }

        private bool _canDisable;
        public bool CanDisable
        {
            get
            {
                return _canDisable;
            }
            set
            {
                _canDisable = value;
                this.RaisePropertyChanged("CanDisable");
            }
        }

        private bool _canDisableGeo;
        public bool CanDisableGeo
        {
            get
            {
                return _canDisableGeo && CanDisable;
            }
            set
            {
                _canDisableGeo = value;
                this.RaisePropertyChanged("CanDisableGeo");
            }
        }


        private Alerte alerte;
        public Alerte Alerte
        {
            get
            {
                return alerte;
            }
            set
            {
                alerte = value;
                this.RaisePropertyChanged("Alerte");
            }
        }

        public String RichLibelle
        {
            get
            {
                return this.CodeEquipement + " - " + this.Libelle;
            }
        }

        public String CommentaireASCII
        {
            get
            {
                return String.IsNullOrEmpty(this.Commentaire) ? String.Empty : System.Windows.Browser.HttpUtility.HtmlDecode(this.Commentaire);
            }
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "CanDisable")
            {
                RaisePropertyChanged("CanDisableGeo");
            }
        }
    }
}
