
namespace Proteca.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies AlerteMetadata as the class
    // that carries additional metadata for the Alerte class.
    [MetadataTypeAttribute(typeof(Alerte.AlerteMetadata))]
    public partial class Alerte
    {

        // This class allows you to attach custom attributes to properties
        // of the Alerte class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AlerteMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private AlerteMetadata()
            {
            }

            public AnAnalyseSerieMesure AnAnalyseSerieMesure { get; set; }

            public int CleAlerte { get; set; }

            public Nullable<int> CleAnalyse { get; set; }

            public Nullable<int> CleMesure { get; set; }

            public Nullable<int> CleVisite { get; set; }

            public DateTime Date { get; set; }

            public int EnumTypeAlerte { get; set; }

            public MesMesure MesMesure { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public bool Supprime { get; set; }

            public Visite Visite { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies AlerteDetailMetadata as the class
    // that carries additional metadata for the AlerteDetail class.
    [MetadataTypeAttribute(typeof(AlerteDetail.AlerteDetailMetadata))]
    public partial class AlerteDetail
    {

        // This class allows you to attach custom attributes to properties
        // of the AlerteDetail class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AlerteDetailMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private AlerteDetailMetadata()
            {
            }

            public Nullable<int> CleAgence { get; set; }

            public int CleAlerte { get; set; }

            public Nullable<int> CleEnsElectrique { get; set; }

            public Nullable<int> ClePortion { get; set; }

            public Nullable<int> CleRegion { get; set; }

            public Nullable<int> CleSecteur { get; set; }

            public string CodeEquipement { get; set; }

            public string Commentaire { get; set; }

            public DateTime Date { get; set; }

            public string Libelle { get; set; }

            public string LibellePortion { get; set; }

            public string LibelleType { get; set; }

            public decimal Pk { get; set; }

            public bool Supprime { get; set; }

            public string Type { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies AnActionMetadata as the class
    // that carries additional metadata for the AnAction class.
    [MetadataTypeAttribute(typeof(AnAction.AnActionMetadata))]
    public partial class AnAction
    {

        // This class allows you to attach custom attributes to properties
        // of the AnAction class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AnActionMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private AnActionMetadata()
            {
            }

            public AnAnalyse AnAnalyse { get; set; }

            public int CleAction { get; set; }

            public int CleAnalyse { get; set; }

            public Nullable<int> CleParametreAction { get; set; }

            public int CleUtilisateurCreation { get; set; }

            public Nullable<int> CleUtilisateurModification { get; set; }

            public string Commentaire { get; set; }

            public string CommentaireStatut { get; set; }

            public string ConstatAnomalie { get; set; }

            public Nullable<decimal> CoutGlobalReel { get; set; }

            public DateTime DateCreation { get; set; }

            public Nullable<DateTime> DateModification { get; set; }

            public Nullable<DateTime> DateRealisationTravaux { get; set; }

            public string Description { get; set; }

            public string EntiteTraitement { get; set; }

            public int EnumStatut { get; set; }

            public string NumActionPc { get; set; }

            public ParametreAction ParametreAction { get; set; }

            public string ProgrammeBudgetaire { get; set; }

            public Nullable<int> Quantite { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public string ResponsableAction { get; set; }

            public Nullable<decimal> TempsTravailGlobalReel { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }

            public UsrUtilisateur UsrUtilisateur1 { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies AnAnalyseMetadata as the class
    // that carries additional metadata for the AnAnalyse class.
    [MetadataTypeAttribute(typeof(AnAnalyse.AnAnalyseMetadata))]
    public partial class AnAnalyse
    {

        // This class allows you to attach custom attributes to properties
        // of the AnAnalyse class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AnAnalyseMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private AnAnalyseMetadata()
            {
            }

            public EntityCollection<AnAction> AnAction { get; set; }

            public int CleAnalyse { get; set; }

            public Nullable<int> CleUtilisateur { get; set; }

            public string Commentaire { get; set; }

            public Nullable<DateTime> DateAnalyse { get; set; }

            public Nullable<int> EnumEtatPc { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }
        }
    }

   
    // The MetadataTypeAttribute identifies AnAnalyseSerieMesureMetadata as the class
    // that carries additional metadata for the AnAnalyseSerieMesure class.
    [MetadataTypeAttribute(typeof(AnAnalyseSerieMesure.AnAnalyseSerieMesureMetadata))]
    public partial class AnAnalyseSerieMesure
    {

        // This class allows you to attach custom attributes to properties
        // of the AnAnalyseSerieMesure class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AnAnalyseSerieMesureMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private AnAnalyseSerieMesureMetadata()
            {
            }

            public EntityCollection<Alerte> Alertes { get; set; }

            public int CleVisite { get; set; }

            public Visite Visite { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies CategoriePpMetadata as the class
    // that carries additional metadata for the CategoriePp class.
    [MetadataTypeAttribute(typeof(CategoriePp.CategoriePpMetadata))]
    public partial class CategoriePp
    {

        // This class allows you to attach custom attributes to properties
        // of the CategoriePp class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class CategoriePpMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private CategoriePpMetadata()
            {
            }

            public int CleCategoriePp { get; set; }

            public int CleNiveauSensibilite { get; set; }

            public Nullable<int> CleTypeEq { get; set; }

            public EntityCollection<HistoPp> HistoPp { get; set; }

            public string Libelle { get; set; }

            public bool NonLieAUnEquipement { get; set; }

            public int NumeroOrdre { get; set; }

            public EntityCollection<Pp> Pps { get; set; }

            public EntityCollection<PpTmp> PpTmp { get; set; }

            public RefNiveauSensibilitePp RefNiveauSensibilitePp { get; set; }

            public TypeEquipement TypeEquipement { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies CompositionMetadata as the class
    // that carries additional metadata for the Composition class.
    [MetadataTypeAttribute(typeof(Composition.CompositionMetadata))]
    public partial class Composition
    {

        // This class allows you to attach custom attributes to properties
        // of the Composition class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class CompositionMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private CompositionMetadata()
            {
            }

            public int CleComposition { get; set; }

            public Nullable<int> CleEnsElectrique { get; set; }

            public Nullable<int> CleEqTmp { get; set; }

            public Nullable<int> CleEquipement { get; set; }

            public Nullable<int> ClePortion { get; set; }

            public Nullable<int> ClePp { get; set; }

            public int CleTournee { get; set; }

            public EnsembleElectrique EnsembleElectrique { get; set; }

            public int EnumTypeEval { get; set; }

            public EqEquipement EqEquipement { get; set; }

            public EqEquipementTmp EqEquipementTmp { get; set; }

            public int NumeroOrdre { get; set; }

            public PortionIntegrite PortionIntegrite { get; set; }

            public Pp Pp { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public Tournee Tournee { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EnsembleElectriqueMetadata as the class
    // that carries additional metadata for the EnsembleElectrique class.
    [MetadataTypeAttribute(typeof(EnsembleElectrique.EnsembleElectriqueMetadata))]
    public partial class EnsembleElectrique
    {

        // This class allows you to attach custom attributes to properties
        // of the EnsembleElectrique class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EnsembleElectriqueMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EnsembleElectriqueMetadata()
            {
            }



            public int CleEnsElectrique { get; set; }

            public string Code { get; set; }

            public string Commentaire { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            public int EnumPeriodicite { get; set; }

            public Nullable<int> EnumStructureCplx { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            public decimal LongueurReseau { get; set; }

            public EntityCollection<PortionIntegrite> PortionIntegrite { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public bool Supprime { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqAnodeGalvaniqueMetadata as the class
    // that carries additional metadata for the EqAnodeGalvanique class.
    [MetadataTypeAttribute(typeof(EqAnodeGalvanique.EqAnodeGalvaniqueMetadata))]
    public partial class EqAnodeGalvanique
    {

        // This class allows you to attach custom attributes to properties
        // of the EqAnodeGalvanique class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqAnodeGalvaniqueMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqAnodeGalvaniqueMetadata()
            {
            }

            public int CleTypeAnode { get; set; }

            public bool PileAssociee { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqDispoEcoulementCourantsAlternatifsMetadata as the class
    // that carries additional metadata for the EqDispoEcoulementCourantsAlternatifs class.
    [MetadataTypeAttribute(typeof(EqDispoEcoulementCourantsAlternatifs.EqDispoEcoulementCourantsAlternatifsMetadata))]
    public partial class EqDispoEcoulementCourantsAlternatifs
    {

        // This class allows you to attach custom attributes to properties
        // of the EqDispoEcoulementCourantsAlternatifs class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqDispoEcoulementCourantsAlternatifsMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqDispoEcoulementCourantsAlternatifsMetadata()
            {
            }

            public int CapaciteCondensateur { get; set; }

            public int CleTypePriseDeTerre { get; set; }

            public Nullable<decimal> CoordDebPriseTerreLat { get; set; }

            public Nullable<decimal> CoordDebPriseTerreLong { get; set; }

            public Nullable<decimal> CoordFinPriseTerreLat { get; set; }

            public Nullable<decimal> CoordFinPriseTerreLong { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public Nullable<DateTime> DatePosePriseDeTerre { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public Nullable<decimal> ResistanceInitPriseDeTerre { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqDrainageMetadata as the class
    // that carries additional metadata for the EqDrainage class.
    [MetadataTypeAttribute(typeof(EqDrainage.EqDrainageMetadata))]
    public partial class EqDrainage
    {

        // This class allows you to attach custom attributes to properties
        // of the EqDrainage class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqDrainageMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqDrainageMetadata()
            {
            }

            public int CleTypeDrainage { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public EntityCollection<EqDrainageLiaisonsext> EqDrainageLiaisonsext { get; set; }

            public int IntensiteMaximaleSupportee { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqDrainageLiaisonsextMetadata as the class
    // that carries additional metadata for the EqDrainageLiaisonsext class.
    [MetadataTypeAttribute(typeof(EqDrainageLiaisonsext.EqDrainageLiaisonsextMetadata))]
    public partial class EqDrainageLiaisonsext
    {

        // This class allows you to attach custom attributes to properties
        // of the EqDrainageLiaisonsext class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqDrainageLiaisonsextMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqDrainageLiaisonsextMetadata()
            {
            }

            public int CleDrainage { get; set; }

            public int CleDrainageLext { get; set; }

            public int CleLiaisonExt { get; set; }

            public EqDrainage EqDrainage { get; set; }

            public EqLiaisonExterne EqLiaisonExterne { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqEquipementMetadata as the class
    // that carries additional metadata for the EqEquipement class.
    [MetadataTypeAttribute(typeof(EqEquipement.EqEquipementMetadata))]
    public partial class EqEquipement
    {

        // This class allows you to attach custom attributes to properties
        // of the EqEquipement class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqEquipementMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqEquipementMetadata()
            {
            }

            public int CleEquipement { get; set; }

            public Nullable<int> CleEquipementOrigine { get; set; }

            public int ClePp { get; set; }

            public int CleTypeEq { get; set; }

            public Nullable<int> CleUtilisateur { get; set; }

            public string Commentaire { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            public DateTime DateMajEquipement { get; set; }

            public Nullable<DateTime> DateMiseEnService { get; set; }

            public EqEquipement EqEquipement2 { get; set; }

            public EntityCollection<EqEquipement> EqEquipementEqEquipement { get; set; }

            public EntityCollection<EqRaccordIsolant> EqRaccordIsolant1 { get; set; }

            public EntityCollection<Image> Images { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            public EntityCollection<MesNiveauProtection> MesNiveauProtection { get; set; }

            public Pp Pp { get; set; }

            public bool Supprime { get; set; }

            public TypeEquipement TypeEquipement { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }

            public EntityCollection<Visite> Visites { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqEquipementTmpMetadata as the class
    // that carries additional metadata for the EqEquipementTmp class.
    [MetadataTypeAttribute(typeof(EqEquipementTmp.EqEquipementTmpMetadata))]
    public partial class EqEquipementTmp
    {

        // This class allows you to attach custom attributes to properties
        // of the EqEquipementTmp class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqEquipementTmpMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqEquipementTmpMetadata()
            {
            }

            public int CleEqTmp { get; set; }

            public int ClePp { get; set; }

            public int CleTypeEq { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateValidation { get; set; }

            public bool EstValide { get; set; }

            public string Libelle { get; set; }

            public Pp Pp2 { get; set; }

            public TypeEquipement TypeEquipement { get; set; }

            public EntityCollection<Visite> Visites { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqFourreauMetalliqueMetadata as the class
    // that carries additional metadata for the EqFourreauMetallique class.
    [MetadataTypeAttribute(typeof(EqFourreauMetallique.EqFourreauMetalliqueMetadata))]
    public partial class EqFourreauMetallique
    {

        // This class allows you to attach custom attributes to properties
        // of the EqFourreauMetallique class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqFourreauMetalliqueMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqFourreauMetalliqueMetadata()
            {
            }

            public Nullable<int> ClePp2 { get; set; }

            public int Longueur { get; set; }

            public Pp Pp2 { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqLiaisonExterneMetadata as the class
    // that carries additional metadata for the EqLiaisonExterne class.
    [MetadataTypeAttribute(typeof(EqLiaisonExterne.EqLiaisonExterneMetadata))]
    public partial class EqLiaisonExterne
    {

        // This class allows you to attach custom attributes to properties
        // of the EqLiaisonExterne class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqLiaisonExterneMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqLiaisonExterneMetadata()
            {
            }

            public int CleNomTiersAss { get; set; }

            public int CleTypeLiaison { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public EntityCollection<EqDrainageLiaisonsext> EqDrainageLiaisonsext { get; set; }

            public EntityCollection<EqSoutirageLiaisonsext> EqSoutirageLiaisonsext { get; set; }

            public bool LiaisonTechnique { get; set; }

            public string LibelleEquipementTiers { get; set; }

            public string LibellePointCommun { get; set; }

            public bool PresencePcSurOuvrageTiers { get; set; }

            public bool PresenceTelemesure { get; set; }

            public bool ProtectionTiersParUnite { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage1 { get; set; }

            public string TypeFluide { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqLiaisonInterneMetadata as the class
    // that carries additional metadata for the EqLiaisonInterne class.
    [MetadataTypeAttribute(typeof(EqLiaisonInterne.EqLiaisonInterneMetadata))]
    public partial class EqLiaisonInterne
    {

        // This class allows you to attach custom attributes to properties
        // of the EqLiaisonInterne class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqLiaisonInterneMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqLiaisonInterneMetadata()
            {
            }

            public Nullable<int> CleLiaisonInterEe { get; set; }

            public int ClePp2 { get; set; }

            public int CleTypeLiaison { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public EntityCollection<EqLiaisonInterne> EqLiaisonInterne1 { get; set; }

            public EqLiaisonInterne EqLiaisonInterne2 { get; set; }

            public bool LiaisonInterEe { get; set; }

            public string LibellePointCommun { get; set; }

            public Pp Pp2 { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqPileMetadata as the class
    // that carries additional metadata for the EqPile class.
    [MetadataTypeAttribute(typeof(EqPile.EqPileMetadata))]
    public partial class EqPile
    {

        // This class allows you to attach custom attributes to properties
        // of the EqPile class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqPileMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqPileMetadata()
            {
            }

            public int CleCaracteristiquePile { get; set; }

            public int CleTypeDeversoir { get; set; }

            public Nullable<DateTime> DatePrevisionRenouvellementPile { get; set; }

            public Nullable<DateTime> DateRenouvellementDeversoir { get; set; }

            public Nullable<int> NombrePiles { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqPostegazMetadata as the class
    // that carries additional metadata for the EqPostegaz class.
    [MetadataTypeAttribute(typeof(EqPostegaz.EqPostegazMetadata))]
    public partial class EqPostegaz
    {

        // This class allows you to attach custom attributes to properties
        // of the EqPostegaz class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqPostegazMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqPostegazMetadata()
            {
            }

            public string CodePosteGaz { get; set; }

            public string TypePoste { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqRaccordIsolantMetadata as the class
    // that carries additional metadata for the EqRaccordIsolant class.
    [MetadataTypeAttribute(typeof(EqRaccordIsolant.EqRaccordIsolantMetadata))]
    public partial class EqRaccordIsolant
    {

        // This class allows you to attach custom attributes to properties
        // of the EqRaccordIsolant class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqRaccordIsolantMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqRaccordIsolantMetadata()
            {
            }

            public Nullable<int> CleLiaison { get; set; }

            public Nullable<int> ClePp2 { get; set; }

            public Nullable<int> CleTypeLiaison { get; set; }

            public int CleTypeRi { get; set; }

            public Nullable<int> EnumConfigElectNormale { get; set; }

            public int EnumEtatElect { get; set; }

            public EqEquipement EqEquipement1 { get; set; }

            public Pp Pp2 { get; set; }

            public bool PresenceEclateur { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage1 { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqSoutirageMetadata as the class
    // that carries additional metadata for the EqSoutirage class.
    [MetadataTypeAttribute(typeof(EqSoutirage.EqSoutirageMetadata))]
    public partial class EqSoutirage
    {

        // This class allows you to attach custom attributes to properties
        // of the EqSoutirage class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqSoutirageMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqSoutirageMetadata()
            {
            }

            public bool Autoregule { get; set; }

            public int CleDeversoir { get; set; }

            public int CleRedresseur { get; set; }

            public Nullable<decimal> CoordDebDeversoirLat { get; set; }

            public Nullable<decimal> CoordDebDeversoirLong { get; set; }

            public Nullable<decimal> CoordFinDeversoirLat { get; set; }

            public Nullable<decimal> CoordFinDeversoirLong { get; set; }

            public DateTime DateControle { get; set; }

            public DateTime DateMiseEnServiceRedresseur { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public Nullable<DateTime> DatePoseDeversoir { get; set; }

            public Nullable<DateTime> DateRenouvellementDeversoir { get; set; }

            public EntityCollection<EqSoutirageLiaisonsext> EqSoutirageLiaisonsext { get; set; }

            public decimal IntensiteReglage { get; set; }

            public decimal LongueurDeversoir { get; set; }

            public Nullable<decimal> MasseAuMetreLineaire { get; set; }

            public bool PresenceReenclencheur { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage1 { get; set; }

            public int TensionReglage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqSoutirageLiaisonsextMetadata as the class
    // that carries additional metadata for the EqSoutirageLiaisonsext class.
    [MetadataTypeAttribute(typeof(EqSoutirageLiaisonsext.EqSoutirageLiaisonsextMetadata))]
    public partial class EqSoutirageLiaisonsext
    {

        // This class allows you to attach custom attributes to properties
        // of the EqSoutirageLiaisonsext class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqSoutirageLiaisonsextMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqSoutirageLiaisonsextMetadata()
            {
            }

            public int CleLiaisonExt { get; set; }

            public int CleSoutirage { get; set; }

            public int CleSoutirageLext { get; set; }

            public EqLiaisonExterne EqLiaisonExterne { get; set; }

            public EqSoutirage EqSoutirage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies EqTiersCroiseSansLiaisonMetadata as the class
    // that carries additional metadata for the EqTiersCroiseSansLiaison class.
    [MetadataTypeAttribute(typeof(EqTiersCroiseSansLiaison.EqTiersCroiseSansLiaisonMetadata))]
    public partial class EqTiersCroiseSansLiaison
    {

        // This class allows you to attach custom attributes to properties
        // of the EqTiersCroiseSansLiaison class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqTiersCroiseSansLiaisonMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private EqTiersCroiseSansLiaisonMetadata()
            {
            }

            public string NomTiersAssocie { get; set; }

            public bool PresencePcSurOuvrageTiers { get; set; }

            public string TypeFluide { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies GeoAgenceMetadata as the class
    // that carries additional metadata for the GeoAgence class.
    [MetadataTypeAttribute(typeof(GeoAgence.GeoAgenceMetadata))]
    public partial class GeoAgence
    {

        // This class allows you to attach custom attributes to properties
        // of the GeoAgence class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoAgenceMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private GeoAgenceMetadata()
            {
            }

            public int CleAgence { get; set; }

            public int CleRegion { get; set; }

            public string CodeAgence { get; set; }

            public GeoRegion GeoRegion { get; set; }

            public EntityCollection<GeoSecteur> GeoSecteur { get; set; }

            public EntityCollection<InsInstrument> InsInstrument { get; set; }

            public string LibelleAbregeAgence { get; set; }

            public string LibelleAgence { get; set; }

            public EntityCollection<UsrUtilisateur> UsrUtilisateur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies GeoEnsElecPortionMetadata as the class
    // that carries additional metadata for the GeoEnsElecPortion class.
    [MetadataTypeAttribute(typeof(GeoEnsElecPortion.GeoEnsElecPortionMetadata))]
    public partial class GeoEnsElecPortion
    {

        // This class allows you to attach custom attributes to properties
        // of the GeoEnsElecPortion class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoEnsElecPortionMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private GeoEnsElecPortionMetadata()
            {
            }

            public int CleAgence { get; set; }

            public Nullable<int> CleEnsElectrique { get; set; }

            public int ClePortion { get; set; }

            public int CleRegion { get; set; }

            public int CleSecteur { get; set; }

            public string Code { get; set; }

            public Nullable<int> EnumStructureCplx { get; set; }

            public int Id { get; set; }

            public string LibelleEe { get; set; }

            public string LibellePortion { get; set; }

            public Nullable<decimal> LongueurPortion { get; set; }

            public int NbPp { get; set; }

            public bool PortionSupprime { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies GeoEnsElecPortionEqPpMetadata as the class
    // that carries additional metadata for the GeoEnsElecPortionEqPp class.
    [MetadataTypeAttribute(typeof(GeoEnsElecPortionEqPp.GeoEnsElecPortionEqPpMetadata))]
    public partial class GeoEnsElecPortionEqPp
    {

        // This class allows you to attach custom attributes to properties
        // of the GeoEnsElecPortionEqPp class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoEnsElecPortionEqPpMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private GeoEnsElecPortionEqPpMetadata()
            {
            }

            public int CleAgence { get; set; }

            public Nullable<int> CleEnsElectrique { get; set; }

            public Nullable<int> CleEquipement { get; set; }

            public int ClePortion { get; set; }

            public Nullable<int> ClePp { get; set; }

            public int CleRegion { get; set; }

            public int CleSecteur { get; set; }

            public string Code { get; set; }

            public string CodeEquipement { get; set; }

            public Nullable<int> EnumStructureCplx { get; set; }

            public int Id { get; set; }

            public string LibelleEe { get; set; }

            public string LibelleEquipement { get; set; }

            public string LibellePortion { get; set; }

            public string LibellePp { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies GeoEnsembleElectriqueMetadata as the class
    // that carries additional metadata for the GeoEnsembleElectrique class.
    [MetadataTypeAttribute(typeof(GeoEnsembleElectrique.GeoEnsembleElectriqueMetadata))]
    public partial class GeoEnsembleElectrique
    {

        // This class allows you to attach custom attributes to properties
        // of the GeoEnsembleElectrique class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoEnsembleElectriqueMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private GeoEnsembleElectriqueMetadata()
            {
            }

            public Nullable<int> CleAgence { get; set; }

            public int CleEnsElectrique { get; set; }

            public Nullable<int> CleRegion { get; set; }

            public Nullable<int> CleSecteur { get; set; }

            public Nullable<int> EnumStructureCplx { get; set; }

            public int Id { get; set; }

            public string Libelle { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies GeoRegionMetadata as the class
    // that carries additional metadata for the GeoRegion class.
    [MetadataTypeAttribute(typeof(GeoRegion.GeoRegionMetadata))]
    public partial class GeoRegion
    {

        // This class allows you to attach custom attributes to properties
        // of the GeoRegion class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoRegionMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private GeoRegionMetadata()
            {
            }

            public int CleRegion { get; set; }

            public string CodeRegion { get; set; }

            public EntityCollection<GeoAgence> GeoAgence { get; set; }

            public EntityCollection<InsInstrument> InsInstrument { get; set; }

            public string LibelleAbregeRegion { get; set; }

            public string LibelleRegion { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies GeoSecteurMetadata as the class
    // that carries additional metadata for the GeoSecteur class.
    [MetadataTypeAttribute(typeof(GeoSecteur.GeoSecteurMetadata))]
    public partial class GeoSecteur
    {

        // This class allows you to attach custom attributes to properties
        // of the GeoSecteur class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoSecteurMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private GeoSecteurMetadata()
            {
            }

            public int CleAgence { get; set; }

            public int CleSecteur { get; set; }

            public string CodeSecteur { get; set; }

            public GeoAgence GeoAgence { get; set; }

            public EntityCollection<InsInstrument> InsInstrument { get; set; }

            public string LibelleAbregeSecteur { get; set; }

            public string LibelleSecteur { get; set; }

            public EntityCollection<PiSecteurs> PiSecteurs { get; set; }

            public EntityCollection<Pp> Pps { get; set; }

            public EntityCollection<UsrUtilisateur> UsrUtilisateur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoAdminMetadata as the class
    // that carries additional metadata for the HistoAdmin class.
    [MetadataTypeAttribute(typeof(HistoAdmin.HistoAdminMetadata))]
    public partial class HistoAdmin
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoAdmin class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoAdminMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoAdminMetadata()
            {
            }

            public int CleHistoAdmin { get; set; }

            public DateTime DateModification { get; set; }

            public int EnumTypeModification { get; set; }

            public string IdConnecte { get; set; }

            public string IdUtilisateur { get; set; }

            public string NomConnecte { get; set; }

            public string NomUtilisateur { get; set; }

            public string PrenomConnecte { get; set; }

            public string PrenomUtilisateur { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public string TypeCompte { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqAnodeGalvaniqueMetadata as the class
    // that carries additional metadata for the HistoEqAnodeGalvanique class.
    [MetadataTypeAttribute(typeof(HistoEqAnodeGalvanique.HistoEqAnodeGalvaniqueMetadata))]
    public partial class HistoEqAnodeGalvanique
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqAnodeGalvanique class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqAnodeGalvaniqueMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqAnodeGalvaniqueMetadata()
            {
            }

            public int CleTypeAnode { get; set; }

            public bool PileAssociee { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqDispoEcoulementCourantsAlternatifsMetadata as the class
    // that carries additional metadata for the HistoEqDispoEcoulementCourantsAlternatifs class.
    [MetadataTypeAttribute(typeof(HistoEqDispoEcoulementCourantsAlternatifs.HistoEqDispoEcoulementCourantsAlternatifsMetadata))]
    public partial class HistoEqDispoEcoulementCourantsAlternatifs
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqDispoEcoulementCourantsAlternatifs class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqDispoEcoulementCourantsAlternatifsMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqDispoEcoulementCourantsAlternatifsMetadata()
            {
            }

            public int CapaciteCondensateur { get; set; }

            public int CleTypePriseDeTerre { get; set; }

            public Nullable<decimal> CoordDebPriseTerreLat { get; set; }

            public Nullable<decimal> CoordDebPriseTerreLong { get; set; }

            public Nullable<decimal> CoordFinPriseTerreLat { get; set; }

            public Nullable<decimal> CoordFinPriseTerreLong { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public Nullable<DateTime> DatePosePriseDeTerre { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public Nullable<decimal> ResistanceInitPriseDeTerre { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqDrainageMetadata as the class
    // that carries additional metadata for the HistoEqDrainage class.
    [MetadataTypeAttribute(typeof(HistoEqDrainage.HistoEqDrainageMetadata))]
    public partial class HistoEqDrainage
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqDrainage class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqDrainageMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqDrainageMetadata()
            {
            }

            public int CleTypeDrainage { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public int IntensiteMaximaleSupportee { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqFourreauMetalliqueMetadata as the class
    // that carries additional metadata for the HistoEqFourreauMetallique class.
    [MetadataTypeAttribute(typeof(HistoEqFourreauMetallique.HistoEqFourreauMetalliqueMetadata))]
    public partial class HistoEqFourreauMetallique
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqFourreauMetallique class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqFourreauMetalliqueMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqFourreauMetalliqueMetadata()
            {
            }

            public string LibellePortionPp2 { get; set; }

            public string LibellePp2 { get; set; }

            public decimal Longueur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqLiaisonExterneMetadata as the class
    // that carries additional metadata for the HistoEqLiaisonExterne class.
    [MetadataTypeAttribute(typeof(HistoEqLiaisonExterne.HistoEqLiaisonExterneMetadata))]
    public partial class HistoEqLiaisonExterne
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqLiaisonExterne class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqLiaisonExterneMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqLiaisonExterneMetadata()
            {
            }

            public int CleTypeLiaison { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public bool LiaisonTechnique { get; set; }

            public string LibelleEquipementTiers { get; set; }

            public string LibellePointCommun { get; set; }

            public string NomTiers { get; set; }

            public bool PresencePcSurOuvrageTiers { get; set; }

            public bool PresenceTelemesure { get; set; }

            public bool ProtectionTiersParUnite { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public string TypeFluide { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqLiaisonInterneMetadata as the class
    // that carries additional metadata for the HistoEqLiaisonInterne class.
    [MetadataTypeAttribute(typeof(HistoEqLiaisonInterne.HistoEqLiaisonInterneMetadata))]
    public partial class HistoEqLiaisonInterne
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqLiaisonInterne class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqLiaisonInterneMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqLiaisonInterneMetadata()
            {
            }

            public int CleTypeLiaison { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public bool LiaisonInterEe { get; set; }

            public string LibelleDoublonInterEe { get; set; }

            public string LibellePointCommun { get; set; }

            public string LibellePortionPp2 { get; set; }

            public string LibellePp2 { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqPileMetadata as the class
    // that carries additional metadata for the HistoEqPile class.
    [MetadataTypeAttribute(typeof(HistoEqPile.HistoEqPileMetadata))]
    public partial class HistoEqPile
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqPile class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqPileMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqPileMetadata()
            {
            }

            public int CleCaracteristiquePile { get; set; }

            public int CleTypeDeversoir { get; set; }

            public Nullable<DateTime> DatePrevisionRenouvellementPile { get; set; }

            public Nullable<DateTime> DateRenouvellementDeversoir { get; set; }

            public Nullable<int> NombrePiles { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqPostegazMetadata as the class
    // that carries additional metadata for the HistoEqPostegaz class.
    [MetadataTypeAttribute(typeof(HistoEqPostegaz.HistoEqPostegazMetadata))]
    public partial class HistoEqPostegaz
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqPostegaz class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqPostegazMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqPostegazMetadata()
            {
            }

            public string CodePosteGaz { get; set; }

            public string TypePoste { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqRaccordIsolantMetadata as the class
    // that carries additional metadata for the HistoEqRaccordIsolant class.
    [MetadataTypeAttribute(typeof(HistoEqRaccordIsolant.HistoEqRaccordIsolantMetadata))]
    public partial class HistoEqRaccordIsolant
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqRaccordIsolant class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqRaccordIsolantMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqRaccordIsolantMetadata()
            {
            }

            public Nullable<int> CleTypeLiaison { get; set; }

            public int CleTypeRi { get; set; }

            public Nullable<int> EnumConfigElectNormale { get; set; }

            public int EnumEtatElect { get; set; }

            public string LibelleLiaison { get; set; }

            public string LibellePortionPp2 { get; set; }

            public string LibellePp2 { get; set; }

            public bool PresenceEclateur { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage1 { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqSoutirageMetadata as the class
    // that carries additional metadata for the HistoEqSoutirage class.
    [MetadataTypeAttribute(typeof(HistoEqSoutirage.HistoEqSoutirageMetadata))]
    public partial class HistoEqSoutirage
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqSoutirage class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqSoutirageMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqSoutirageMetadata()
            {
            }

            public bool Autoregule { get; set; }

            public Nullable<decimal> CoordDebDeversoirLat { get; set; }

            public Nullable<decimal> CoordDebDeversoirLong { get; set; }

            public Nullable<decimal> CoordFinDeversoirLat { get; set; }

            public Nullable<decimal> CoordFinDeversoirLong { get; set; }

            public DateTime DateControle { get; set; }

            public DateTime DateMiseEnServiceRedresseur { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public Nullable<DateTime> DatePoseDeversoir { get; set; }

            public Nullable<DateTime> DateRenouvellementDeversoir { get; set; }

            public decimal IntensiteReglage { get; set; }

            public decimal LongueurDeversoir { get; set; }

            public Nullable<decimal> MasseAuMetreLineaire { get; set; }

            public bool PresenceReenclencheur { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public int TensionReglage { get; set; }

            public int TypeDeversoir { get; set; }

            public string TypeRedresseur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEqTiersCroiseSansLiaisonMetadata as the class
    // that carries additional metadata for the HistoEqTiersCroiseSansLiaison class.
    [MetadataTypeAttribute(typeof(HistoEqTiersCroiseSansLiaison.HistoEqTiersCroiseSansLiaisonMetadata))]
    public partial class HistoEqTiersCroiseSansLiaison
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEqTiersCroiseSansLiaison class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqTiersCroiseSansLiaisonMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEqTiersCroiseSansLiaisonMetadata()
            {
            }

            public string NomTiersAssocie { get; set; }

            public bool PresencePcSurOuvrageTiers { get; set; }

            public string TypeFluide { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoEquipementMetadata as the class
    // that carries additional metadata for the HistoEquipement class.
    [MetadataTypeAttribute(typeof(HistoEquipement.HistoEquipementMetadata))]
    public partial class HistoEquipement
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoEquipement class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEquipementMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoEquipementMetadata()
            {
            }

            public int CleHistoEquipement { get; set; }

            public int CleLogOuvrage { get; set; }

            public int CleTypeEq { get; set; }

            public string Commentaire { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            public DateTime DateMajEquipement { get; set; }

            public Nullable<DateTime> DateMiseEnService { get; set; }

            public string Libelle { get; set; }

            public string LibellePortion { get; set; }

            public string LibellePp { get; set; }

            public LogOuvrage LogOuvrage { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage1 { get; set; }

            public bool Supprime { get; set; }

            public TypeEquipement TypeEquipement { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies HistoPpMetadata as the class
    // that carries additional metadata for the HistoPp class.
    [MetadataTypeAttribute(typeof(HistoPp.HistoPpMetadata))]
    public partial class HistoPp
    {

        // This class allows you to attach custom attributes to properties
        // of the HistoPp class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoPpMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private HistoPpMetadata()
            {
            }

            public CategoriePp CategoriePp { get; set; }

            public Nullable<int> CleCategoriePp { get; set; }

            public int CleCommune { get; set; }

            public int CleHistoPp { get; set; }

            public int CleLogOuvrage { get; set; }

            public int CleNiveauSensibilite { get; set; }

            public string Commentaire { get; set; }

            public string CommentairePositionnement { get; set; }

            public bool CoordonneeGpsFiabilisee { get; set; }

            public bool CourantsAlternatifsInduits { get; set; }

            public bool CourantsVagabonds { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            public DateTime DateMajPp { get; set; }

            public Nullable<DateTime> DateMiseEnService { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public bool ElectrodeEnterreeAmovible { get; set; }

            public Nullable<int> EnumDureeEnrg { get; set; }

            public Nullable<int> EnumPolarisation { get; set; }

            public Nullable<int> EnumSurfaceTme { get; set; }

            public Nullable<int> EnumSurfaceTms { get; set; }

            public string Libelle { get; set; }

            public string LibellePortion { get; set; }

            public LogOuvrage LogOuvrage { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage1 { get; set; }

            public decimal Pk { get; set; }

            public Nullable<decimal> PositionGpsLat { get; set; }

            public Nullable<decimal> PositionGpsLong { get; set; }

            public string PositionnementPostal { get; set; }

            public bool PpPoste { get; set; }

            public bool PresenceTelemesure { get; set; }

            public RefCommune RefCommune { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public RefEnumValeur RefEnumValeur2 { get; set; }

            public RefEnumValeur RefEnumValeur3 { get; set; }

            public RefNiveauSensibilitePp RefNiveauSensibilitePp { get; set; }

            public bool Supprime { get; set; }

            public bool TemoinEnterreAmovible { get; set; }

            public bool TemoinMetalliqueDeSurface { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ImageMetadata as the class
    // that carries additional metadata for the Image class.
    [MetadataTypeAttribute(typeof(Image.ImageMetadata))]
    public partial class Image
    {

        // This class allows you to attach custom attributes to properties
        // of the Image class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ImageMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ImageMetadata()
            {
            }

            public Nullable<int> CleEquipement { get; set; }

            public int CleImage { get; set; }

            public Nullable<int> ClePp { get; set; }

            public int EnumTypeImage { get; set; }

            public EqEquipement EqEquipement { get; set; }

            public byte[] Image1 { get; set; }

            public Pp Pp { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies InsInstrumentMetadata as the class
    // that carries additional metadata for the InsInstrument class.
    [MetadataTypeAttribute(typeof(InsInstrument.InsInstrumentMetadata))]
    public partial class InsInstrument
    {

        // This class allows you to attach custom attributes to properties
        // of the InsInstrument class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class InsInstrumentMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private InsInstrumentMetadata()
            {
            }

            public Nullable<int> CleAgence { get; set; }

            public int CleInstrument { get; set; }

            public Nullable<int> CleRegion { get; set; }

            public Nullable<int> CleSecteur { get; set; }

            public GeoAgence GeoAgence { get; set; }

            public GeoRegion GeoRegion { get; set; }

            public GeoSecteur GeoSecteur { get; set; }

            public EntityCollection<InstrumentsUtilises> InstrumentsUtilises { get; set; }

            public string Libelle { get; set; }

            public bool Supprime { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies InstrumentsUtilisesMetadata as the class
    // that carries additional metadata for the InstrumentsUtilises class.
    [MetadataTypeAttribute(typeof(InstrumentsUtilises.InstrumentsUtilisesMetadata))]
    public partial class InstrumentsUtilises
    {

        // This class allows you to attach custom attributes to properties
        // of the InstrumentsUtilises class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class InstrumentsUtilisesMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private InstrumentsUtilisesMetadata()
            {
            }

            public int CleInstrument { get; set; }

            public int CleInsUtilises { get; set; }

            public int CleVisite { get; set; }

            public InsInstrument InsInstrument { get; set; }

            public Visite Visite { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies LogOuvrageMetadata as the class
    // that carries additional metadata for the LogOuvrage class.
    [MetadataTypeAttribute(typeof(LogOuvrage.LogOuvrageMetadata))]
    public partial class LogOuvrage
    {

        // This class allows you to attach custom attributes to properties
        // of the LogOuvrage class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class LogOuvrageMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private LogOuvrageMetadata()
            {
            }

            public Nullable<int> CleEnsElectrique { get; set; }

            public Nullable<int> CleEquipement { get; set; }

            public Nullable<int> CleHistoEquipement { get; set; }

            public Nullable<int> CleHistoPp { get; set; }

            public int CleLogOuvrage { get; set; }

            public Nullable<int> ClePortion { get; set; }

            public Nullable<int> ClePp { get; set; }

            public int CleUtilisateur { get; set; }

            public DateTime DateHistorisation { get; set; }

            public EnsembleElectrique EnsembleElectrique { get; set; }

            public int EnumTypeModification { get; set; }

            public EqEquipement EqEquipement { get; set; }

            public EntityCollection<HistoEquipement> HistoEquipement { get; set; }

            public HistoEquipement HistoEquipement1 { get; set; }

            public EntityCollection<HistoPp> HistoPp { get; set; }

            public HistoPp HistoPp1 { get; set; }

            public string ListeChamps { get; set; }

            public PortionIntegrite PortionIntegrite { get; set; }

            public Pp Pp { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies LogTourneeMetadata as the class
    // that carries additional metadata for the LogTournee class.
    [MetadataTypeAttribute(typeof(LogTournee.LogTourneeMetadata))]
    public partial class LogTournee
    {

        // This class allows you to attach custom attributes to properties
        // of the LogTournee class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class LogTourneeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private LogTourneeMetadata()
            {
            }

            public int CleLogTournee { get; set; }

            public int CleTournee { get; set; }

            public int CleUtilisateur { get; set; }

            public DateTime DateHistorisation { get; set; }

            public int EnumTypeModification { get; set; }

            public string ListeChamps { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public Tournee Tournee { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies MesClassementMesureMetadata as the class
    // that carries additional metadata for the MesClassementMesure class.
    [MetadataTypeAttribute(typeof(MesClassementMesure.MesClassementMesureMetadata))]
    public partial class MesClassementMesure
    {

        // This class allows you to attach custom attributes to properties
        // of the MesClassementMesure class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesClassementMesureMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private MesClassementMesureMetadata()
            {
            }

            public int CleClassementMesure { get; set; }

            public int CleTypeMesure { get; set; }

            public bool CourantsAlternatifsInduits { get; set; }

            public bool CourantsVagabons { get; set; }

            public bool ElectrodeEnterreeAmovible { get; set; }

            public MesTypeMesure MesTypeMesure { get; set; }

            public bool Telemesure { get; set; }

            public bool TemoinDeSurface { get; set; }

            public bool TemoinEnterre { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies MesCoutMesureMetadata as the class
    // that carries additional metadata for the MesCoutMesure class.
    [MetadataTypeAttribute(typeof(MesCoutMesure.MesCoutMesureMetadata))]
    public partial class MesCoutMesure
    {

        // This class allows you to attach custom attributes to properties
        // of the MesCoutMesure class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesCoutMesureMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private MesCoutMesureMetadata()
            {
            }

            public int CleCoutMesure { get; set; }

            public int CleTypeEq { get; set; }

            public Nullable<decimal> Cout { get; set; }

            public Nullable<int> EnumDureeEnregistrement { get; set; }

            public Nullable<int> EnumTempsPolarisation { get; set; }

            public int EnumTypeEval { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public RefEnumValeur RefEnumValeur2 { get; set; }

            public Nullable<decimal> Temps { get; set; }

            public TypeEquipement TypeEquipement { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies MesMesureMetadata as the class
    // that carries additional metadata for the MesMesure class.
    [MetadataTypeAttribute(typeof(MesMesure.MesMesureMetadata))]
    public partial class MesMesure
    {

        // This class allows you to attach custom attributes to properties
        // of the MesMesure class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesMesureMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private MesMesureMetadata()
            {
            }

            public EntityCollection<Alerte> Alertes { get; set; }

            public int CleMesure { get; set; }

            public int CleTypeMesure { get; set; }

            public int CleVisite { get; set; }

            public MesTypeMesure MesTypeMesure { get; set; }

            public Nullable<decimal> Valeur { get; set; }

            public Visite Visite { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies MesModeleMesureMetadata as the class
    // that carries additional metadata for the MesModeleMesure class.
    [MetadataTypeAttribute(typeof(MesModeleMesure.MesModeleMesureMetadata))]
    public partial class MesModeleMesure
    {

        // This class allows you to attach custom attributes to properties
        // of the MesModeleMesure class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesModeleMesureMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private MesModeleMesureMetadata()
            {
            }

            public int CleModeleMesure { get; set; }

            public int CleTypeEq { get; set; }

            public int CleUnite { get; set; }

            public Nullable<int> EnumTypeGraphique { get; set; }

            public string Libelle { get; set; }

            public string LibGenerique { get; set; }

            public EntityCollection<MesNiveauProtection> MesNiveauProtection { get; set; }

            public EntityCollection<MesTypeMesure> MesTypeMesure { get; set; }

            public MesUnite MesUnite { get; set; }

            public int NumeroOrdre { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public TypeEquipement TypeEquipement { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies MesNiveauProtectionMetadata as the class
    // that carries additional metadata for the MesNiveauProtection class.
    [MetadataTypeAttribute(typeof(MesNiveauProtection.MesNiveauProtectionMetadata))]
    public partial class MesNiveauProtection
    {

        // This class allows you to attach custom attributes to properties
        // of the MesNiveauProtection class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesNiveauProtectionMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private MesNiveauProtectionMetadata()
            {
            }

            public Nullable<int> CleEquipement { get; set; }

            public int CleModeleMesure { get; set; }

            public int CleNiveauProtection { get; set; }

            public Nullable<int> ClePortion { get; set; }

            public Nullable<int> ClePp { get; set; }

            public EqEquipement EqEquipement { get; set; }

            public MesModeleMesure MesModeleMesure { get; set; }

            public PortionIntegrite PortionIntegrite { get; set; }

            public Pp Pp { get; set; }

            public Nullable<decimal> SeuilMaxi { get; set; }

            public Nullable<decimal> SeuilMini { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies MesTypeMesureMetadata as the class
    // that carries additional metadata for the MesTypeMesure class.
    [MetadataTypeAttribute(typeof(MesTypeMesure.MesTypeMesureMetadata))]
    public partial class MesTypeMesure
    {

        // This class allows you to attach custom attributes to properties
        // of the MesTypeMesure class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesTypeMesureMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private MesTypeMesureMetadata()
            {
            }

            public int CleModeleMesure { get; set; }

            public int CleTypeMesure { get; set; }

            public string LibNivAutre { get; set; }

            public string LibTypeMesure { get; set; }

            public EntityCollection<MesClassementMesure> MesClassementMesure { get; set; }

            public EntityCollection<MesMesure> MesMesure { get; set; }

            public MesModeleMesure MesModeleMesure { get; set; }

            public bool MesureComplementaire { get; set; }

            public bool MesureEnService { get; set; }

            public int NiveauType { get; set; }

            public int NumeroOrdre { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public int TypeEvaluation { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies MesUniteMetadata as the class
    // that carries additional metadata for the MesUnite class.
    [MetadataTypeAttribute(typeof(MesUnite.MesUniteMetadata))]
    public partial class MesUnite
    {

        // This class allows you to attach custom attributes to properties
        // of the MesUnite class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesUniteMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private MesUniteMetadata()
            {
            }

            public int CleUnite { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<MesModeleMesure> MesModeleMesure { get; set; }

            public Nullable<int> NombreDeDecimales { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public string Symbole { get; set; }

            public int TypeDonnee { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ParametreActionMetadata as the class
    // that carries additional metadata for the ParametreAction class.
    [MetadataTypeAttribute(typeof(ParametreAction.ParametreActionMetadata))]
    public partial class ParametreAction
    {

        // This class allows you to attach custom attributes to properties
        // of the ParametreAction class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ParametreActionMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ParametreActionMetadata()
            {
            }

            public EntityCollection<AnAction> AnAction { get; set; }

            public int CleParametreAction { get; set; }

            public string Codification { get; set; }

            public Nullable<decimal> Cout { get; set; }

            public int EnumCategorieAnomalie { get; set; }

            public int EnumDegreUrgence { get; set; }

            public int EnumDelaiRealisation { get; set; }

            public int EnumTypeAction { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public RefEnumValeur RefEnumValeur2 { get; set; }

            public RefEnumValeur RefEnumValeur3 { get; set; }

            public Nullable<decimal> Temps { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies PiSecteursMetadata as the class
    // that carries additional metadata for the PiSecteurs class.
    [MetadataTypeAttribute(typeof(PiSecteurs.PiSecteursMetadata))]
    public partial class PiSecteurs
    {

        // This class allows you to attach custom attributes to properties
        // of the PiSecteurs class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PiSecteursMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private PiSecteursMetadata()
            {
            }

            public int ClePortion { get; set; }

            public int ClePortionSecteurs { get; set; }

            public int CleSecteur { get; set; }

            public GeoSecteur GeoSecteur { get; set; }

            public PortionIntegrite PortionIntegrite { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies PortionDatesMetadata as the class
    // that carries additional metadata for the PortionDates class.
    [MetadataTypeAttribute(typeof(PortionDates.PortionDatesMetadata))]
    public partial class PortionDates
    {

        // This class allows you to attach custom attributes to properties
        // of the PortionDates class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PortionDatesMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private PortionDatesMetadata()
            {
            }

            public int CleEnsElectrique { get; set; }

            public int ClePortion { get; set; }

            public Nullable<DateTime> DateCf { get; set; }

            public Nullable<DateTime> DateEcd { get; set; }

            public Nullable<DateTime> DateEg { get; set; }

            public int Id { get; set; }

            public string Libelle { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies PortionIntegriteMetadata as the class
    // that carries additional metadata for the PortionIntegrite class.
    [MetadataTypeAttribute(typeof(PortionIntegrite.PortionIntegriteMetadata))]
    public partial class PortionIntegrite
    {

        // This class allows you to attach custom attributes to properties
        // of the PortionIntegrite class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PortionIntegriteMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private PortionIntegriteMetadata()
            {
            }

            public bool Branchement { get; set; }

            public Nullable<int> CleCommuneArrivee { get; set; }

            public Nullable<int> CleCommuneDepart { get; set; }

            public int CleDiametre { get; set; }

            public int CleEnsElectrique { get; set; }

            public int ClePortion { get; set; }

            public int CleRevetement { get; set; }

            public string Code { get; set; }

            public string CodeGmao { get; set; }

            public string Commentaire { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            public Nullable<DateTime> DateMiseEnService { get; set; }

            public Nullable<DateTime> DatePose { get; set; }

            public EnsembleElectrique EnsembleElectrique { get; set; }

            public Nullable<int> Idtroncon { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            public Nullable<decimal> Longueur { get; set; }

            public EntityCollection<MesNiveauProtection> MesNiveauProtection { get; set; }

            public EntityCollection<PiSecteurs> PiSecteurs { get; set; }

            public EntityCollection<Pp> Pps { get; set; }

            public RefCommune RefCommune { get; set; }

            public RefCommune RefCommune1 { get; set; }

            public RefDiametre RefDiametre { get; set; }

            public RefRevetement RefRevetement { get; set; }

            public bool Supprime { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies PpMetadata as the class
    // that carries additional metadata for the Pp class.
    [MetadataTypeAttribute(typeof(Pp.PpMetadata))]
    public partial class Pp
    {

        // This class allows you to attach custom attributes to properties
        // of the Pp class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PpMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private PpMetadata()
            {
            }

            public bool BypassCategoriePp { get; set; }

            public CategoriePp CategoriePp { get; set; }

            public Nullable<int> CleCategoriePp { get; set; }

            public int CleCommune { get; set; }

            public int CleNiveauSensibilite { get; set; }

            public int ClePortion { get; set; }

            public int ClePp { get; set; }

            public Nullable<int> ClePpOrigine { get; set; }

            public int CleSecteur { get; set; }

            public Nullable<int> CleUtiDdeDeverrouillage { get; set; }

            public int CleUtilisateur { get; set; }

            public string Commentaire { get; set; }

            public string CommentairePositionnement { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public bool CoordonneeGpsFiabilisee { get; set; }

            public bool CourantsAlternatifsInduits { get; set; }

            public bool CourantsVagabonds { get; set; }

            public Nullable<DateTime> DateDdeDeverrouillageCoordGps { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            public DateTime DateMajPp { get; set; }

            public Nullable<DateTime> DateMiseEnService { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public bool DdeDeverrouillageCoordGps { get; set; }

            public bool ElectrodeEnterreeAmovible { get; set; }

            public Nullable<int> EnumDureeEnrg { get; set; }

            public Nullable<int> EnumPolarisation { get; set; }

            public Nullable<int> EnumSurfaceTme { get; set; }

            public Nullable<int> EnumSurfaceTms { get; set; }

            public EntityCollection<EqEquipement> EqEquipement { get; set; }

            public EntityCollection<EqEquipementTmp> EqEquipementTmp { get; set; }

            public EntityCollection<EqFourreauMetallique> EqFourreauMetallique { get; set; }

            public EntityCollection<EqLiaisonInterne> EqLiaisonInterne { get; set; }

            public EntityCollection<EqRaccordIsolant> EqRaccordIsolant { get; set; }

            public GeoSecteur GeoSecteur { get; set; }

            public EntityCollection<Image> Images { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            public EntityCollection<MesNiveauProtection> MesNiveauProtection { get; set; }

            public decimal Pk { get; set; }

            public PortionIntegrite PortionIntegrite { get; set; }

            public Nullable<decimal> PositionGpsLat { get; set; }

            public Nullable<decimal> PositionGpsLong { get; set; }

            public string PositionnementPostal { get; set; }

            public EntityCollection<Pp> Pp1 { get; set; }

            public Pp Pp2 { get; set; }

            public EntityCollection<PpJumelee> PpJumelee { get; set; }

            public EntityCollection<PpJumelee> PpJumelee1 { get; set; }

            public bool PpPoste { get; set; }

            public EntityCollection<PpTmp> PpTmp { get; set; }

            public bool PresenceDUneTelemesure { get; set; }

            public RefCommune RefCommune { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public RefEnumValeur RefEnumValeur2 { get; set; }

            public RefEnumValeur RefEnumValeur3 { get; set; }

            public RefNiveauSensibilitePp RefNiveauSensibilitePp { get; set; }

            public bool Supprime { get; set; }

            public bool TemoinEnterreAmovible { get; set; }

            public bool TemoinMetalliqueDeSurface { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }

            public UsrUtilisateur UsrUtilisateur1 { get; set; }

            public EntityCollection<Visite> Visites { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies PpEquipementMetadata as the class
    // that carries additional metadata for the PpEquipement class.
    [MetadataTypeAttribute(typeof(PpEquipement.PpEquipementMetadata))]
    public partial class PpEquipement
    {

        // This class allows you to attach custom attributes to properties
        // of the PpEquipement class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PpEquipementMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private PpEquipementMetadata()
            {
            }

            public Nullable<int> CleEquipement { get; set; }

            public int ClePortion { get; set; }

            public int ClePp { get; set; }

            public Nullable<int> CleTypeEq { get; set; }

            public string CodeEquipement { get; set; }

            public int Id { get; set; }

            public string LibelleEquipement { get; set; }

            public string LibellePp { get; set; }

            public string LibelleType { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies PpJumeleeMetadata as the class
    // that carries additional metadata for the PpJumelee class.
    [MetadataTypeAttribute(typeof(PpJumelee.PpJumeleeMetadata))]
    public partial class PpJumelee
    {

        // This class allows you to attach custom attributes to properties
        // of the PpJumelee class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PpJumeleeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private PpJumeleeMetadata()
            {
            }

            public int ClePp { get; set; }

            public int ClePpJumelee { get; set; }

            public Pp Pp { get; set; }

            public Pp Pp1 { get; set; }

            public int PpClePp { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies PpTmpMetadata as the class
    // that carries additional metadata for the PpTmp class.
    [MetadataTypeAttribute(typeof(PpTmp.PpTmpMetadata))]
    public partial class PpTmp
    {

        // This class allows you to attach custom attributes to properties
        // of the PpTmp class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PpTmpMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private PpTmpMetadata()
            {
            }

            public CategoriePp CategoriePp { get; set; }

            public Nullable<int> CleCategoriePp { get; set; }

            public int CleNiveauSensibilite { get; set; }

            public int ClePp { get; set; }

            public int ClePpTmp { get; set; }

            public Nullable<int> CleUtiDdeDeverrouillage { get; set; }

            public bool CoordonneeGpsFiabilisee { get; set; }

            public bool CourantsAlternatifsInduits { get; set; }

            public bool CourantsVagabonds { get; set; }

            public Nullable<DateTime> DateDdeDeverrouillageCoordGps { get; set; }

            public DateTime DateMajPp { get; set; }

            public bool DdeDeverrouillageCoordGps { get; set; }

            public bool ElectrodeEnterreeAmovible { get; set; }

            public Nullable<int> EnumDureeEnrg { get; set; }

            public Nullable<int> EnumPolarisation { get; set; }

            public Nullable<int> EnumSurfaceTme { get; set; }

            public Nullable<int> EnumSurfaceTms { get; set; }

            public Nullable<decimal> PositionGpsLat { get; set; }

            public Nullable<decimal> PositionGpsLong { get; set; }

            public Pp Pp { get; set; }

            public bool PresenceDUneTelemesure { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public RefEnumValeur RefEnumValeur2 { get; set; }

            public RefEnumValeur RefEnumValeur3 { get; set; }

            public RefNiveauSensibilitePp RefNiveauSensibilitePp { get; set; }

            public bool TemoinEnterreAmovible { get; set; }

            public bool TemoinMetalliqueDeSurface { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }

            public EntityCollection<Visite> Visites { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefCommuneMetadata as the class
    // that carries additional metadata for the RefCommune class.
    [MetadataTypeAttribute(typeof(RefCommune.RefCommuneMetadata))]
    public partial class RefCommune
    {

        // This class allows you to attach custom attributes to properties
        // of the RefCommune class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefCommuneMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefCommuneMetadata()
            {
            }

            public int CleCommune { get; set; }

            public string CodeCommune { get; set; }

            public string CodeDepartement { get; set; }

            public string CodeInsee { get; set; }

            public EntityCollection<HistoPp> HistoPp { get; set; }

            public Nullable<int> Idmicado { get; set; }

            public string Libelle { get; set; }

            public string LibelleMajuscule { get; set; }

            public EntityCollection<PortionIntegrite> PortionIntegrite { get; set; }

            public EntityCollection<PortionIntegrite> PortionIntegrite1 { get; set; }

            public EntityCollection<Pp> Pps { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefDiametreMetadata as the class
    // that carries additional metadata for the RefDiametre class.
    [MetadataTypeAttribute(typeof(RefDiametre.RefDiametreMetadata))]
    public partial class RefDiametre
    {

        // This class allows you to attach custom attributes to properties
        // of the RefDiametre class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefDiametreMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefDiametreMetadata()
            {
            }

            public int CleDiametre { get; set; }

            public string Code { get; set; }

            public Nullable<int> Idmicado { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<PortionIntegrite> PortionIntegrite { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefEnumValeurMetadata as the class
    // that carries additional metadata for the RefEnumValeur class.
    [MetadataTypeAttribute(typeof(RefEnumValeur.RefEnumValeurMetadata))]
    public partial class RefEnumValeur
    {

        // This class allows you to attach custom attributes to properties
        // of the RefEnumValeur class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefEnumValeurMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefEnumValeurMetadata()
            {
            }

            public EntityCollection<Alerte> Alertes { get; set; }

            public EntityCollection<AnAction> AnAction { get; set; }

            public EntityCollection<AnAnalyse> AnAnalyse { get; set; }

            public int CleEnumValeur { get; set; }

            public string CodeGroupe { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public EntityCollection<EnsembleElectrique> EnsembleElectrique { get; set; }

            public EntityCollection<EnsembleElectrique> EnsembleElectrique1 { get; set; }

            public EntityCollection<EqPile> EqPile { get; set; }

            public EntityCollection<EqRaccordIsolant> EqRaccordIsolant { get; set; }

            public EntityCollection<EqRaccordIsolant> EqRaccordIsolant1 { get; set; }

            public EntityCollection<HistoAdmin> HistoAdmin { get; set; }

            public EntityCollection<HistoEqPile> HistoEqPile { get; set; }

            public EntityCollection<HistoEqRaccordIsolant> HistoEqRaccordIsolant { get; set; }

            public EntityCollection<HistoEqRaccordIsolant> HistoEqRaccordIsolant1 { get; set; }

            public EntityCollection<HistoPp> HistoPp { get; set; }

            public EntityCollection<HistoPp> HistoPp1 { get; set; }

            public EntityCollection<HistoPp> HistoPp2 { get; set; }

            public EntityCollection<HistoPp> HistoPp3 { get; set; }

            public EntityCollection<Image> Images { get; set; }

            public string Libelle { get; set; }

            public string LibelleCourt { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            public EntityCollection<LogTournee> LogTournee { get; set; }

            public EntityCollection<MesCoutMesure> MesCoutMesure { get; set; }

            public EntityCollection<MesCoutMesure> MesCoutMesure1 { get; set; }

            public EntityCollection<MesCoutMesure> MesCoutMesure2 { get; set; }

            public EntityCollection<MesModeleMesure> MesModeleMesure { get; set; }

            public EntityCollection<MesTypeMesure> MesTypeMesure { get; set; }

            public EntityCollection<MesTypeMesure> MesTypeMesure1 { get; set; }

            public EntityCollection<MesUnite> MesUnite { get; set; }

            public int NumeroOrdre { get; set; }

            public EntityCollection<ParametreAction> ParametreAction { get; set; }

            public EntityCollection<ParametreAction> ParametreAction1 { get; set; }

            public EntityCollection<ParametreAction> ParametreAction2 { get; set; }

            public EntityCollection<ParametreAction> ParametreAction3 { get; set; }

            public EntityCollection<Pp> Pps { get; set; }

            public EntityCollection<Pp> Pps1 { get; set; }

            public EntityCollection<Pp> Pps2 { get; set; }

            public EntityCollection<Pp> Pps3 { get; set; }

            public EntityCollection<PpTmp> PpTmp { get; set; }

            public EntityCollection<PpTmp> PpTmp1 { get; set; }

            public EntityCollection<PpTmp> PpTmp2 { get; set; }

            public EntityCollection<PpTmp> PpTmp3 { get; set; }

            public EntityCollection<RefNiveauSensibilitePp> RefNiveauSensibilitePp { get; set; }

            public string Valeur { get; set; }

            public EntityCollection<Visite> Visites { get; set; }

            public EntityCollection<Visite> Visites1 { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefNiveauSensibilitePpMetadata as the class
    // that carries additional metadata for the RefNiveauSensibilitePp class.
    [MetadataTypeAttribute(typeof(RefNiveauSensibilitePp.RefNiveauSensibilitePpMetadata))]
    public partial class RefNiveauSensibilitePp
    {

        // This class allows you to attach custom attributes to properties
        // of the RefNiveauSensibilitePp class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefNiveauSensibilitePpMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefNiveauSensibilitePpMetadata()
            {
            }

            public EntityCollection<CategoriePp> CategoriePp { get; set; }

            public int CleNiveauSensibilite { get; set; }

            public Nullable<int> EnumTypeEval { get; set; }

            public EntityCollection<HistoPp> HistoPp { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<Pp> Pps { get; set; }

            public EntityCollection<PpTmp> PpTmp { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public int TypeSensibilite { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefParametreMetadata as the class
    // that carries additional metadata for the RefParametre class.
    [MetadataTypeAttribute(typeof(RefParametre.RefParametreMetadata))]
    public partial class RefParametre
    {

        // This class allows you to attach custom attributes to properties
        // of the RefParametre class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefParametreMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefParametreMetadata()
            {
            }

            public int CleParametre { get; set; }

            public string CodeGroupe { get; set; }

            public string Libelle { get; set; }

            public string LibelleLong { get; set; }

            public string LibUnite { get; set; }

            public int NumeroOrdre { get; set; }

            public string TypeDeDonnee { get; set; }

            public string Valeur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefRevetementMetadata as the class
    // that carries additional metadata for the RefRevetement class.
    [MetadataTypeAttribute(typeof(RefRevetement.RefRevetementMetadata))]
    public partial class RefRevetement
    {

        // This class allows you to attach custom attributes to properties
        // of the RefRevetement class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefRevetementMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefRevetementMetadata()
            {
            }

            public int CleRevetement { get; set; }

            public string Code { get; set; }

            public Nullable<int> Idmicado { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<PortionIntegrite> PortionIntegrite { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefSousTypeOuvrageMetadata as the class
    // that carries additional metadata for the RefSousTypeOuvrage class.
    [MetadataTypeAttribute(typeof(RefSousTypeOuvrage.RefSousTypeOuvrageMetadata))]
    public partial class RefSousTypeOuvrage
    {

        // This class allows you to attach custom attributes to properties
        // of the RefSousTypeOuvrage class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefSousTypeOuvrageMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefSousTypeOuvrageMetadata()
            {
            }

            public int CleSousTypeOuvrage { get; set; }

            public string CodeGroupe { get; set; }

            public EntityCollection<EqAnodeGalvanique> EqAnodeGalvanique { get; set; }

            public EntityCollection<EqDispoEcoulementCourantsAlternatifs> EqDispoEcoulementCourantsAlternatifs { get; set; }

            public EntityCollection<EqDrainage> EqDrainage { get; set; }

            public EntityCollection<EqLiaisonExterne> EqLiaisonExterne { get; set; }

            public EntityCollection<EqLiaisonExterne> EqLiaisonExterne1 { get; set; }

            public EntityCollection<EqLiaisonInterne> EqLiaisonInterne { get; set; }

            public EntityCollection<EqPile> EqPile { get; set; }

            public EntityCollection<EqRaccordIsolant> EqRaccordIsolant { get; set; }

            public EntityCollection<EqRaccordIsolant> EqRaccordIsolant1 { get; set; }

            public EntityCollection<EqSoutirage> EqSoutirage { get; set; }

            public EntityCollection<EqSoutirage> EqSoutirage1 { get; set; }

            public EntityCollection<HistoEqAnodeGalvanique> HistoEqAnodeGalvanique { get; set; }

            public EntityCollection<HistoEqDispoEcoulementCourantsAlternatifs> HistoEqDispoEcoulementCourantsAlternatifs { get; set; }

            public EntityCollection<HistoEqDrainage> HistoEqDrainage { get; set; }

            public EntityCollection<HistoEqLiaisonExterne> HistoEqLiaisonExterne { get; set; }

            public EntityCollection<HistoEqLiaisonInterne> HistoEqLiaisonInterne { get; set; }

            public EntityCollection<HistoEqPile> HistoEqPile { get; set; }

            public EntityCollection<HistoEqRaccordIsolant> HistoEqRaccordIsolant { get; set; }

            public EntityCollection<HistoEqRaccordIsolant> HistoEqRaccordIsolant1 { get; set; }

            public EntityCollection<HistoEqSoutirage> HistoEqSoutirage { get; set; }

            public string Libelle { get; set; }

            public int NumeroOrdre { get; set; }

            public string Valeur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefUsrAutorisationMetadata as the class
    // that carries additional metadata for the RefUsrAutorisation class.
    [MetadataTypeAttribute(typeof(RefUsrAutorisation.RefUsrAutorisationMetadata))]
    public partial class RefUsrAutorisation
    {

        // This class allows you to attach custom attributes to properties
        // of the RefUsrAutorisation class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefUsrAutorisationMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefUsrAutorisationMetadata()
            {
            }

            public int CleAutorisation { get; set; }

            public Nullable<int> CleGroupe { get; set; }

            public string CodeAutorisation { get; set; }

            public string LibelleAutorisation { get; set; }

            public RefUsrGroupe RefUsrGroupe { get; set; }

            public string TypePortee { get; set; }

            public EntityCollection<UsrRole> UsrRole { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefUsrGroupeMetadata as the class
    // that carries additional metadata for the RefUsrGroupe class.
    [MetadataTypeAttribute(typeof(RefUsrGroupe.RefUsrGroupeMetadata))]
    public partial class RefUsrGroupe
    {

        // This class allows you to attach custom attributes to properties
        // of the RefUsrGroupe class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefUsrGroupeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefUsrGroupeMetadata()
            {
            }

            public int CleGroupe { get; set; }

            public string LibelleGroupe { get; set; }

            public EntityCollection<RefUsrAutorisation> RefUsrAutorisation { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies RefUsrPorteeMetadata as the class
    // that carries additional metadata for the RefUsrPortee class.
    [MetadataTypeAttribute(typeof(RefUsrPortee.RefUsrPorteeMetadata))]
    public partial class RefUsrPortee
    {

        // This class allows you to attach custom attributes to properties
        // of the RefUsrPortee class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefUsrPorteeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RefUsrPorteeMetadata()
            {
            }

            public int ClePortee { get; set; }

            public string CodePortee { get; set; }

            public string LibellePortee { get; set; }

            public string TypePortee { get; set; }

            public EntityCollection<UsrProfil> UsrProfil { get; set; }

            public EntityCollection<UsrRole> UsrRole { get; set; }

            public EntityCollection<UsrUtilisateur> UsrUtilisateur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies TourneeMetadata as the class
    // that carries additional metadata for the Tournee class.
    [MetadataTypeAttribute(typeof(Tournee.TourneeMetadata))]
    public partial class Tournee
    {

        // This class allows you to attach custom attributes to properties
        // of the Tournee class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class TourneeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private TourneeMetadata()
            {
            }

            public int CleTournee { get; set; }

            public Nullable<int> CleUtilisateur { get; set; }

            public string CodeTournee { get; set; }

            public string Commentaire { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateCreation { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<LogTournee> LogTournee { get; set; }

            public Nullable<int> Numero { get; set; }

            public bool Supprime { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }

            public bool Verrouille { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies TourneePpEqMetadata as the class
    // that carries additional metadata for the TourneePpEq class.
    [MetadataTypeAttribute(typeof(TourneePpEq.TourneePpEqMetadata))]
    public partial class TourneePpEq
    {

        // This class allows you to attach custom attributes to properties
        // of the TourneePpEq class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class TourneePpEqMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private TourneePpEqMetadata()
            {
            }

            public int CleAgence { get; set; }

            public int CleEnsElectrique { get; set; }

            public Nullable<int> CleEquipement { get; set; }

            public int ClePortion { get; set; }

            public Nullable<int> ClePp { get; set; }

            public int CleRegion { get; set; }

            public int CleSecteur { get; set; }

            public int CleTournee { get; set; }

            public int EnumTypeEval { get; set; }

            public int Id { get; set; }

            public string Libelle { get; set; }

            public string LibelleSecteur { get; set; }

            public string LibelleTournee { get; set; }

            public int NumeroOrdre { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies TypeEquipementMetadata as the class
    // that carries additional metadata for the TypeEquipement class.
    [MetadataTypeAttribute(typeof(TypeEquipement.TypeEquipementMetadata))]
    public partial class TypeEquipement
    {

        // This class allows you to attach custom attributes to properties
        // of the TypeEquipement class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class TypeEquipementMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private TypeEquipementMetadata()
            {
            }

            public EntityCollection<CategoriePp> CategoriePp { get; set; }

            public int CleTypeEq { get; set; }

            public string CodeEquipement { get; set; }

            public EntityCollection<EqEquipement> EqEquipement { get; set; }

            public EntityCollection<EqEquipementTmp> EqEquipementTmp { get; set; }

            public EntityCollection<HistoEquipement> HistoEquipement { get; set; }

            public string Libelle { get; set; }

            public EntityCollection<MesCoutMesure> MesCoutMesure { get; set; }

            public EntityCollection<MesModeleMesure> MesModeleMesure { get; set; }

            public int NumeroOrdre { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies UsrProfilMetadata as the class
    // that carries additional metadata for the UsrProfil class.
    [MetadataTypeAttribute(typeof(UsrProfil.UsrProfilMetadata))]
    public partial class UsrProfil
    {

        // This class allows you to attach custom attributes to properties
        // of the UsrProfil class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class UsrProfilMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private UsrProfilMetadata()
            {
            }

            public int ClePortee { get; set; }

            public int CleProfil { get; set; }

            public bool Editable { get; set; }

            public string LibelleProfil { get; set; }

            public bool ProfilAdmin { get; set; }

            public RefUsrPortee RefUsrPortee { get; set; }

            public EntityCollection<UsrRole> UsrRole { get; set; }

            public EntityCollection<UsrUtilisateur> UsrUtilisateur { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies UsrRoleMetadata as the class
    // that carries additional metadata for the UsrRole class.
    [MetadataTypeAttribute(typeof(UsrRole.UsrRoleMetadata))]
    public partial class UsrRole
    {

        // This class allows you to attach custom attributes to properties
        // of the UsrRole class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class UsrRoleMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private UsrRoleMetadata()
            {
            }

            public int CleAutorisation { get; set; }

            public int ClePortee { get; set; }

            public int CleProfil { get; set; }

            public int CleRole { get; set; }

            public RefUsrAutorisation RefUsrAutorisation { get; set; }

            public RefUsrPortee RefUsrPortee { get; set; }

            public UsrProfil UsrProfil { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies UsrUtilisateurMetadata as the class
    // that carries additional metadata for the UsrUtilisateur class.
    [MetadataTypeAttribute(typeof(UsrUtilisateur.UsrUtilisateurMetadata))]
    public partial class UsrUtilisateur
    {

        // This class allows you to attach custom attributes to properties
        // of the UsrUtilisateur class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class UsrUtilisateurMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private UsrUtilisateurMetadata()
            {
            }

            public EntityCollection<AnAction> AnAction { get; set; }

            public EntityCollection<AnAction> AnAction1 { get; set; }

            public EntityCollection<AnAnalyse> AnAnalyse { get; set; }

            public Nullable<int> CleAgence { get; set; }

            public Nullable<int> CleProfil { get; set; }

            public Nullable<int> CleSecteur { get; set; }

            public int CleUtilisateur { get; set; }

            public EntityCollection<EqEquipement> EqEquipement { get; set; }

            public bool EstPresta { get; set; }

            public bool Externe { get; set; }

            public GeoAgence GeoAgence { get; set; }

            public GeoSecteur GeoSecteur { get; set; }

            public int GestionDesComptes { get; set; }

            public string Identifiant { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            public EntityCollection<LogTournee> LogTournee { get; set; }

            public string Mail { get; set; }

            public string Nom { get; set; }

            public EntityCollection<Pp> Pps { get; set; }

            public EntityCollection<Pp> Pps1 { get; set; }

            public EntityCollection<PpTmp> PpTmp { get; set; }

            public string Prenom { get; set; }

            public RefUsrPortee RefUsrPortee { get; set; }

            public string Societe { get; set; }

            public bool Supprime { get; set; }

            public EntityCollection<Tournee> Tournees { get; set; }

            public UsrProfil UsrProfil { get; set; }

            public EntityCollection<Visite> Visites { get; set; }

            public EntityCollection<Visite> Visites1 { get; set; }

            public EntityCollection<Visite> Visites2 { get; set; }

            public EntityCollection<Visite> Visites3 { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies VisiteMetadata as the class
    // that carries additional metadata for the Visite class.
    [MetadataTypeAttribute(typeof(Visite.VisiteMetadata))]
    public partial class Visite
    {

        // This class allows you to attach custom attributes to properties
        // of the Visite class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class VisiteMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private VisiteMetadata()
            {
            }

            public EntityCollection<Alerte> Alertes { get; set; }

            public EntityCollection<AnAnalyseEeVisite> AnAnalyseEeVisite { get; set; }

            public EntityCollection<AnAnalyseSerieMesure> AnAnalyseSerieMesure { get; set; }

            public Nullable<int> CleEqTmp { get; set; }

            public Nullable<int> CleEquipement { get; set; }

            public Nullable<int> ClePp { get; set; }

            public Nullable<int> ClePpTmp { get; set; }

            public Nullable<int> CleUtilisateurCreation { get; set; }

            public Nullable<int> CleUtilisateurImport { get; set; }

            public Nullable<int> CleUtilisateurMesure { get; set; }

            public Nullable<int> CleUtilisateurValidation { get; set; }

            public int CleVisite { get; set; }

            public string Commentaire { get; set; }

            public Nullable<DateTime> DateImport { get; set; }

            public Nullable<DateTime> DateSaisie { get; set; }

            public Nullable<DateTime> DateValidation { get; set; }

            public Nullable<DateTime> DateVisite { get; set; }

            public Nullable<int> EnumConformiteTournee { get; set; }

            public int EnumTypeEval { get; set; }

            public int EnumTypeEvalComposition { get; set; }

            public EqEquipement EqEquipement { get; set; }

            public EqEquipementTmp EqEquipementTmp { get; set; }

            public bool EstValidee { get; set; }

            public EntityCollection<InstrumentsUtilises> InstrumentsUtilises { get; set; }

            public EntityCollection<MesMesure> MesMesure { get; set; }

            public Pp Pp { get; set; }

            public PpTmp PpTmp { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            public bool RelevePartiel { get; set; }

            public bool Telemesure { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }

            public UsrUtilisateur UsrUtilisateur1 { get; set; }

            public UsrUtilisateur UsrUtilisateur2 { get; set; }

            public UsrUtilisateur UsrUtilisateur3 { get; set; }
        }
    }
}
