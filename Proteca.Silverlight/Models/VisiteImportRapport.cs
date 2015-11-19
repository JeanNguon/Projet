using System;
using System.Net;
using System.ComponentModel;

namespace Proteca.Web.Models
{
    public partial class VisiteImportRapport
    {
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "Message" || e.PropertyName == "StatutImport")
            {
                this.RaisePropertyChanged("Sortie");
            }
        }

        public string Sortie
        {
            get
            {
                return (String.IsNullOrEmpty(this.Message)) ? String.Empty : this.StatutImport + " : " + this.Message;
            }
        }
    }
}
