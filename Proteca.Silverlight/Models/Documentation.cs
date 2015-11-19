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

namespace Proteca.Silverlight.Models
{
    /// <summary>
    /// Objet utilisé pour afficher la grille de l'écran bilan documentation
    /// </summary>
    public class Documentation
    {

        public CleOuvrage cleOuvrage;


        public Documentation() { }

        public Documentation(Web.Models.GeoEnsElecPortionEqPp GeoEqPp, TypeDocument d, CleOuvrage cleOuvrage)
        {
            // TODO: Complete member initialization

            this.cleOuvrage = cleOuvrage;
            this.Cle = GeoEqPp.Id;
            this.CodeEquipement = GeoEqPp.CodeEquipement;
            this.Designation = d;
            this.Dossier = d.Parent.Libelle;
            this.TypeOuvrage = null;
            this.NumeroVersion = null;
            this.Libelle = null;
            this.DateEnregistrement = null;
            this.DocumentUrl = null;
            this.ClePp = GeoEqPp.ClePp;
            this.CleEnsElectrique = GeoEqPp.CleEnsElectrique;
            this.ClePortion = GeoEqPp.ClePortion;

            this.LibelleEe = GeoEqPp.LibelleEe;
            this.LibellePortion = GeoEqPp.LibellePortion;
            if (cleOuvrage == CleOuvrage.CleEquipement)
            {
                this.CleEquipement = GeoEqPp.CleEquipement;
                this.LibelleEquipement = GeoEqPp.LibelleEquipement;
            }
            this.LibellePp = GeoEqPp.LibellePp;
            this.Code = GeoEqPp.Code;
        }

        public Documentation(Web.Models.GeoEnsElecPortion GeoEqPp, TypeDocument d, CleOuvrage cleOuvrage)
        {
            // TODO: Complete member initialization

            this.cleOuvrage = cleOuvrage;
            this.Cle = GeoEqPp.Id;
            this.CodeEquipement = null;
            this.Designation = d;
            this.Dossier = d.Parent.Libelle;
            this.TypeOuvrage = null;
            this.NumeroVersion = null;
            this.Libelle = null;
            this.DateEnregistrement = null;
            this.DocumentUrl = null;
            this.ClePp = null;
            this.CleEnsElectrique = GeoEqPp.CleEnsElectrique;
            this.ClePortion = GeoEqPp.ClePortion;
            this.CleEquipement = null;
            this.LibelleEe = GeoEqPp.LibelleEe;
            this.LibellePortion = GeoEqPp.LibellePortion;
            this.LibelleEquipement = null;
            this.LibellePp = null;
            this.Code = GeoEqPp.Code;
        }

        public static Documentation AddDocToDocumentation(Documentation model, Document doc)
        {
            Documentation outDoc = new Documentation();


            //CLONAGE
            outDoc.cleOuvrage = model.cleOuvrage;
            outDoc.Cle = model.Cle;
            outDoc.CodeEquipement = model.CodeEquipement;
            outDoc.ClePp = model.ClePp;
            outDoc.CleEnsElectrique = model.CleEnsElectrique;
            outDoc.ClePortion = model.ClePortion;
            outDoc.CleEquipement = model.CleEquipement;
            outDoc.LibelleEe = model.LibelleEe;
            outDoc.LibellePortion = model.LibellePortion;
            outDoc.LibelleEquipement = model.LibelleEquipement;
            outDoc.LibellePp = model.LibellePp;
            outDoc.Code = model.Code;
            outDoc.LibelleTypeEquipement = model.LibelleTypeEquipement;
            outDoc.NumeroOrdre = model.NumeroOrdre;
            outDoc.Region = model.Region;

            //AJOUT DU DOC
            outDoc.Designation = doc.Designation;
            outDoc.Dossier = doc.Designation.TypeDossier;
            outDoc.TypeOuvrage = doc.TypeOuvrage;
            outDoc.NumeroVersion = doc.NumeroVersion;
            outDoc.Libelle = doc.Libelle;
            outDoc.DateEnregistrement = doc.DateEnregistrement;
            outDoc.DocumentUrl = doc.DocumentUrl;

            return outDoc;
        }



        public int Cle { get; set; }
        public TypeDocument Designation { get; set; }
        public string LibelleDesignation
        {
            get
            {
                return Designation != null ? Designation.Libelle : string.Empty;
            }
        }
        public string Dossier { get; set; }
        public string NumeroVersion { get; set; }
        public CleOuvrage? TypeOuvrage { get; set; }
        public string Libelle { get; set; }
        public DateTime? DateEnregistrement { get; set; }
        public Uri DocumentUrl { get; set; }
        public int? ClePp { get; set; }
        public int? CleEnsElectrique { get; set; }
        public int? ClePortion { get; set; }
        public int? CleEquipement { get; set; }
        public string LibellePp { get; set; }
        public string LibelleEe { get; set; }
        public string LibellePortion { get; set; }
        public string LibelleEquipement { get; set; }
        public string LibelleTypeEquipement { get; set; }
        public string Code { get; set; }
        public string Region { get; set; }
        public string CodeEquipement { get; set; }
        public int? NumeroOrdre { get; set; }

        public string LibellePpDisplay
        {
            get { return this.CodeEquipement == "PP" ? String.Empty : LibellePp; }
        }

        public Uri NaviagtionUrlEnsembleElectrique
        {
            get
            {
                return new Uri(string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.EnsembleElectrique.GetStringValue(),
                   CleEnsElectrique), UriKind.Relative);
            }
        }
        public Uri NaviagtionUrlPortion
        {
            get
            {
                return new Uri(string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.PortionIntegrite.GetStringValue(),
                   ClePortion), UriKind.Relative);
            }
        }
        public Uri NaviagtionUrlPp
        {
            get
            {
                return new Uri(string.Format("/{0}/{1}/PP/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.Equipement.GetStringValue(),
                   this.ClePp), UriKind.Relative);
            }
        }

        public Uri NavigationUrlEquipement
        {
            get
            {
                return new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                      MainNavigation.GestionOuvrages.GetStringValue(),
                      OuvrageNavigation.Equipement.GetStringValue(),
                      this.CodeEquipement,
                      this.CleEquipement), UriKind.Relative);
            }
        }
    }
}
