using System;
using System.Net;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proteca.Web.Models
{
    public class VisiteImportRapport
    {
        public VisiteImportRapport()
        {
            Id = Guid.NewGuid();
        }

        [KeyAttribute]
        public Guid Id { get; set; }

        public int NumLigne { get; set; }
        public string NomFichier { get; set; }
        public string TypeEquipement { get; set; }
        public int CleEquipement { get; set; }
        public string LibelleEq { get; set; }
        public DateTime DateVisite { get; set; }
        public string StatutImport { get; set; }
        public string Message { get; set; }
        public string ImgError { get; set; }
        public string TextImport { get; set; }
    }
}
