using System;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    public class VisiteImport
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public VisiteImport()
        {
            VisiteRapport = new VisiteImportRapport();
        }

        // Identification de la ligne
        public string NomFichier { get; set; }
        public int IndexOnFile { get; set; }
        public string TypeEquipementOrigine { get; set; }
        public string TypeEquipement { get; set; }
        public string Line { get; set; }
        //public string MatriculeCCol { get; set; }

        // Champs VISITES
        public string[] Row;
        public int CleEquipement { get; set; }
        public int ClePpAssocie { get; set; }
        public int EnumTypeEval { get; set; }
        public int EnumTypeEvalComposition { get; set; }
        public bool IsTelemesure { get; set; }
        public bool IsAlerteUtilisateur { get; set; }
        public bool EstValide { get; set; }
        public bool HasAnalyse { get; set; }
        public string LibelleEq { get; set; }
        public string CommentaireVisite { get; set; }
        public DateTime DateVisite { get; set; }
        public DateTime DateImport { get; set; }
        public DateTime DateSaisie { get; set; }

        // Correction mantis 19928
        public bool RelevePartiel { get; set; }

        private ObservableCollection<InsInstrument> _listeInstrument;
        public ObservableCollection<InsInstrument> ListInstrument 
        {
            get
            {
                if (this._listeInstrument == null)
                {
                    this._listeInstrument = new ObservableCollection<InsInstrument>();
                }
                return this._listeInstrument;
            }
            set
            {
                this._listeInstrument = value;
            }
        }
        private List<int> _listCleInstrument;
        public List<int> ListCleInstrument
        {
            get
            {
                if (this._listCleInstrument == null)
                {
                    this._listCleInstrument = new List<int>();
                }
                return this._listCleInstrument;
            }
            set
            {
                this._listCleInstrument = value;
            }
        }
        private List<String> _listNewInstrument;
        public List<String> ListNewInstrument
        {
            get
            {
                if (this._listNewInstrument == null)
                {
                    this._listNewInstrument = new List<String>();
                }
                return this._listNewInstrument;
            }
            set
            {
                this._listNewInstrument = value;
            }
        }
        private List<int> _listCleTypeMesure;
        public List<int> ListCleTypeMesure
        {
            get
            {
                if (this._listCleTypeMesure == null)
                {
                    this._listCleTypeMesure = new List<int>();
                }
                return this._listCleTypeMesure;
            }
            set
            {
                this._listCleTypeMesure = value;
            }
        }
        public UsrUtilisateur Utilisateur { get; set; }
        public int CleUtilisateur { get; set; }

        // Spécifique Protein
        public bool IsEquipementTempo { get; set; }
        public bool IsPPModifed { get; set; }
        public bool IsPpGPSFiabilisee { get; set; }
        public bool DemandeDeverrouillage { get; set; }
        public bool DemandeFiabilisation { get; set; }
        public XElement MaPP { get; set; }
        public XElement MesChampsChangesPp { get; set; }
        public XElement MaVisiteProtein { get; set; }
        public IEnumerable<XElement> MesVisitesMesures { get; set; }
        public TypeEquipement TypeEqAssocie { get; set; }

        // Rapport
        public bool IsOnError { get; set; }
        public bool IsOnWarning { get; set; }
        public bool IsOnSuccess { get; set; }
        public VisiteImportRapport VisiteRapport { get; set; }

        /// <summary>
        /// Ajoute la ligne en succès
        /// </summary>
        public void AddOnSucess()
        {
            if (!IsOnWarning && !IsOnError)
            {
                IsOnSuccess = true;
                VisiteRapport.Message = String.Empty;
                VisiteRapport.ImgError = ResourceImg.Valider;
                VisiteRapport.StatutImport = "Importé";
                //VisiteRapport.RaisePropertyChanged("Sortie");
            }
        }

        /// <summary>
        /// Ajoute la ligne en erreur
        /// </summary>
        /// <param name="message"></param>
        /// <param name="erase"></param>
        public void AddOnError(string message, bool erase = false)
        {
            if (IsOnError == true && !erase)
            {
                VisiteRapport.Message += "\n" + message;
            }
            else
            {
                IsOnError = true;
                VisiteRapport.Message = message;
                VisiteRapport.ImgError = ResourceImg.Error;
                VisiteRapport.StatutImport = "Non importé";
            }
            //VisiteRapport.RaisePropertyChanged("Sortie");
        }

        /// <summary>
        /// Ajoute la ligne en warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="erase"></param>
        public void AddOnWarning(string message, bool erase = false)
        {
            if (IsOnWarning == true && !erase)
            {
                VisiteRapport.Message += "\n" + message;
            }
            if (IsOnError == true && !erase)
            {
                VisiteRapport.Message += "\n" + message;
            }
            else
            {
                IsOnWarning = true;
                VisiteRapport.Message = message;
                VisiteRapport.ImgError = ResourceImg.Warning;
                VisiteRapport.StatutImport = "Partiellement importé";
            }
            //VisiteRapport.RaisePropertyChanged("Sortie");
        }

        public String TextImport
        {
            get
            {
                if (this.MaVisiteProtein != null)
                {
                    if (this.MaPP != null)
                    {
                        return MaPP.ToString();
                    }
                    else
                    {
                        return this.MaVisiteProtein.ToString();
                    }
                }
                else
                {
                    return Line;
                }
            }
        }
    }
}
