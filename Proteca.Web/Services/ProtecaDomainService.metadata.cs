
namespace Proteca.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.Linq.Expressions;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using Proteca.Web.Resources;
    // MetadataTypeAttribute identifie AlerteMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe Alerte.
    [MetadataTypeAttribute(typeof(Alerte.AlerteMetadata))]
    public partial class Alerte
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe Alerte.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AlerteMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private AlerteMetadata()
            {
            }

            [Include]
            public AnAnalyseSerieMesure AnAnalyseSerieMesure { get; set; }

            public int CleAlerte { get; set; }

            public Nullable<int> CleAnalyse { get; set; }

            public Nullable<int> CleMesure { get; set; }

            public Nullable<int> CleVisite { get; set; }

            public DateTime Date { get; set; }

            public int EnumTypeAlerte { get; set; }

            [Include]
            public MesMesure MesMesure { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public bool Supprime { get; set; }

            [Include]
            public Visite Visite { get; set; }
        }
    }

    // MetadataTypeAttribute identifie AlerteDetailMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe AlerteDetail.
    [MetadataTypeAttribute(typeof(AlerteDetail.AlerteDetailMetadata))]
    public partial class AlerteDetail
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe AlerteDetail.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AlerteDetailMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie AnActionMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe AnAction.
    [MetadataTypeAttribute(typeof(AnAction.AnActionMetadata))]
    public partial class AnAction
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe AnAction.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AnActionMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private AnActionMetadata()
            {
            }

            [RequiredCustomAction]
            public Nullable<int> CleRegion { get; set; }

            [RequiredCustomAction]
            public Nullable<int> CleEnsembleElec { get; set; }

            [Include]
            public AnAnalyse AnAnalyse { get; set; }

            public int CleAction { get; set; }
            //MANTIS-18602 - Clé analyse est maintenant nullable car l'action peut être créée hors analyse
            public int? CleAnalyse { get; set; }

            [RequiredCustom]
            [RequiredReference("ParametreAction")]
            public Nullable<int> CleParametreAction { get; set; }

            [RequiredReference("UsrUtilisateur")]
            public int CleUtilisateurCreation { get; set; }

            [RequiredReference("UsrUtilisateur1")]
            public Nullable<int> CleUtilisateurModification { get; set; }

            [RequiredCustomAction]
            public string Commentaire { get; set; }

            [RequiredCustomAction]
            public string CommentaireStatut { get; set; }

            [RequiredCustomAction]
            public string ConstatAnomalie { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 8, MaxDecimalPartSize = 2, PositiveOrZero = true)]
            [RequiredCustomAction]
            public Nullable<decimal> CoutGlobalReel { get; set; }

            public DateTime DateCreation { get; set; }

            public Nullable<DateTime> DateModification { get; set; }

            [CustomValidation(typeof(CustomValidators), "CheckDateRealisationSupDateDebut")]
            [RequiredCustomAction]
            public Nullable<DateTime> DateRealisationTravaux { get; set; }

            [RequiredCustomAction]
            public string Description { get; set; }

            [RequiredReference("RefEnumValeur1")]
            [RequiredCustomAction]
            public string EntiteTraitement { get; set; }

            [RequiredReference("RefEnumValeur")]
            [RequiredCustomAction]
            public int EnumStatut { get; set; }

            public string NumActionPc { get; set; }

            [Include]
            public ParametreAction ParametreAction { get; set; }

            [RequiredCustomAction]
            public string ProgrammeBudgetaire { get; set; }

            [RangeCustom(Minimum = "1", Maximum = "9999")]
            [RequiredCustomAction]
            public Nullable<int> Quantite { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 8, MaxDecimalPartSize = 2, PositiveOrZero = true)]
            public Nullable<decimal> TempsTravailGlobalReel { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur1 { get; set; }

            // MANTIS-18602 - Nouveaux champs 
            [RequiredReference("UsrUtilisateurResp")]
            [RequiredCustomAction]
            public int CleUtilisateurResponsable { get; set; }

            [RequiredReference("UsrUtilisateurAgent")]
            [RequiredCustomAction]
            public Nullable<int> CleUtilisateurAgent { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateurResp { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateurAgent { get; set; }

            [Include]
            [RequiredCustomAction]
            public EntityCollection<PortionIntegriteAnAction> PortionIntegriteAnAction { get; set; }

            [RequiredCustomAction]
            public Nullable<DateTime> DateDebut { get; set; }

            [CustomValidation(typeof(CustomValidators), "CheckDateClotureSupDateDebut")]
            [RequiredCustomAction]
            public Nullable<DateTime> DateCloture { get; set; }

            [RequiredReference("RefEnumValeur2")]
            [RequiredCustomAction]
            public Nullable<int> TypeEval { get; set; }
        }
    }

    // MetadataTypeAttribute identifie AnAnalyseMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe AnAnalyse.
    [MetadataTypeAttribute(typeof(AnAnalyse.AnAnalyseMetadata))]
    public partial class AnAnalyse
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe AnAnalyse.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal class AnAnalyseMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            protected AnAnalyseMetadata()
            {
            }

            [Include]
            public EntityCollection<AnAction> AnAction { get; set; }

            public int CleAnalyse { get; set; }

            public Nullable<int> CleUtilisateur { get; set; }

            [CheckCommentaireAnalyse]
            public string Commentaire { get; set; }

            public Nullable<DateTime> DateAnalyse { get; set; }

            public Nullable<DateTime> DateEdition { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur")]
            public Nullable<int> EnumEtatPc { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public UsrUtilisateur UsrUtilisateur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie AnAnalyseEeMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe AnAnalyseEe.
    [MetadataTypeAttribute(typeof(AnAnalyseEe.AnAnalyseEeMetadata))]
    public partial class AnAnalyseEe
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe AnAnalyseEe.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AnAnalyseEeMetadata : AnAnalyseMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private AnAnalyseEeMetadata()
            {
            }

            [Include]
            public EntityCollection<AnAnalyseEeVisite> AnAnalyseEeVisite { get; set; }

            public int CleEnsElectrique { get; set; }

            public Nullable<DateTime> DateDebutPeriode { get; set; }

            public Nullable<DateTime> DateFinPeriode { get; set; }

            [Include]
            public EnsembleElectrique EnsembleElectrique { get; set; }

            public string RefRapportAction { get; set; }
        }
    }

    // MetadataTypeAttribute identifie AnAnalyseEeVisiteMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe AnAnalyseEeVisite.
    [MetadataTypeAttribute(typeof(AnAnalyseEeVisite.AnAnalyseEeVisiteMetadata))]
    public partial class AnAnalyseEeVisite
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe AnAnalyseEeVisite.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AnAnalyseEeVisiteMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private AnAnalyseEeVisiteMetadata()
            {
            }

            public AnAnalyseEe AnAnalyseEe { get; set; }

            public int CleAnalyse { get; set; }

            public int CleAnalyseEeVisite { get; set; }

            public int CleVisite { get; set; }

            [Include]
            public Visite Visite { get; set; }
        }
    }

    // MetadataTypeAttribute identifie AnAnalyseSerieMesureMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe AnAnalyseSerieMesure.
    [MetadataTypeAttribute(typeof(AnAnalyseSerieMesure.AnAnalyseSerieMesureMetadata))]
    public partial class AnAnalyseSerieMesure
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe AnAnalyseSerieMesure.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AnAnalyseSerieMesureMetadata : AnAnalyseMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private AnAnalyseSerieMesureMetadata()
            {
            }

            [Include]
            public EntityCollection<Alerte> Alertes { get; set; }

            public int CleVisite { get; set; }

            [Include]
            public Visite Visite { get; set; }
        }
    }

    // MetadataTypeAttribute identifie CategoriePpMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe CategoriePp.
    [MetadataTypeAttribute(typeof(CategoriePp.CategoriePpMetadata))]
    public partial class CategoriePp
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe CategoriePp.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class CategoriePpMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private CategoriePpMetadata()
            {
            }

            public int CleCategoriePp { get; set; }

            public int CleNiveauSensibilite { get; set; }

            public Nullable<int> CleTypeEq { get; set; }

            [Include]
            public EntityCollection<HistoPp> HistoPp { get; set; }

            [Unique]
            public string Libelle { get; set; }

            public bool NonLieAUnEquipement { get; set; }

            [Unique]
            [RequiredReference("NumeroOrdreNullable")] // propriété déclarée coté client
            public int NumeroOrdre { get; set; }

            [Include]
            public EntityCollection<Pp> Pps { get; set; }

            public EntityCollection<PpTmp> PpTmp { get; set; }

            [Include]
            public RefNiveauSensibilitePp RefNiveauSensibilitePp { get; set; }

            [Include]
            public TypeEquipement TypeEquipement { get; set; }
        }
    }

    // MetadataTypeAttribute identifie CompositionMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe Composition.
    [MetadataTypeAttribute(typeof(Composition.CompositionMetadata))]
    public partial class Composition
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe Composition.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class CompositionMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Include]
            public EnsembleElectrique EnsembleElectrique { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur")]
            public int EnumTypeEval { get; set; }

            [Include]
            public EqEquipement EqEquipement { get; set; }

            public EqEquipementTmp EqEquipementTmp { get; set; }

            public int NumeroOrdre { get; set; }

            [Include]
            public PortionIntegrite PortionIntegrite { get; set; }

            [Include]
            public Pp Pp { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public Tournee Tournee { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EnsembleElectriqueMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EnsembleElectrique.
    [MetadataTypeAttribute(typeof(EnsembleElectrique.EnsembleElectriqueMetadata))]
    public partial class EnsembleElectrique
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EnsembleElectrique.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EnsembleElectriqueMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EnsembleElectriqueMetadata()
            {
            }

            [Include]
            public EntityCollection<AnAnalyseEe> AnAnalyseEe { get; set; }

            public int CleEnsElectrique { get; set; }

            [Unique]
            [StringLengthCustom(5)]
            public string Code { get; set; }

            [StringLengthCustom(500)]
            public string Commentaire { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            [RequiredCustom]
            [RequiredReference("EnumPeriodiciteNullable")] // propriété déclarée coté client
            public int EnumPeriodicite { get; set; }

            public Nullable<int> EnumStructureCplx { get; set; }

            [RequiredCustom]
            [Unique]
            [StringLengthCustom(50, 3)]
            public string Libelle { get; set; }

            [Include]
            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            public decimal LongueurReseau { get; set; }

            [Include]
            public EntityCollection<PortionIntegrite> PortionIntegrite { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur1 { get; set; }

            public bool Supprime { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqAnodeGalvaniqueMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqAnodeGalvanique.
    [MetadataTypeAttribute(typeof(EqAnodeGalvanique.EqAnodeGalvaniqueMetadata))]
    public partial class EqAnodeGalvanique
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqAnodeGalvanique.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqAnodeGalvaniqueMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqAnodeGalvaniqueMetadata()
            {
            }

            [RequiredCustom]
            public int CleTypeAnode { get; set; }

            public bool PileAssociee { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqDispoEcoulementCourantsAlternatifsMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqDispoEcoulementCourantsAlternatifs.
    [MetadataTypeAttribute(typeof(EqDispoEcoulementCourantsAlternatifs.EqDispoEcoulementCourantsAlternatifsMetadata))]
    public partial class EqDispoEcoulementCourantsAlternatifs
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqDispoEcoulementCourantsAlternatifs.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqDispoEcoulementCourantsAlternatifsMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqDispoEcoulementCourantsAlternatifsMetadata()
            {
            }

            [EntierPositif(true)]
            [RangeCustom(Minimum = "0", Maximum = "999999")]
            [RequiredReference("CapaciteCondensateurNullable")]
            public int CapaciteCondensateur { get; set; }

            [RequiredCustom]
            public int CleTypePriseDeTerre { get; set; }

            [RegularExpression(@"^\d{1,2}(?:[\,\.]\d{1,9})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultCoordonneesLatFieldErrorMessage")]
            [RangeCustom(Minimum = "41.000000000", Maximum = "52.000000000")]
            public Nullable<decimal> CoordDebPriseTerreLat { get; set; }

            [RegularExpression(@"^[+-]{0,1}\d{1,2}(?:[\,\.]\d{1,9})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultCoordonneesLongFieldErrorMessage")]
            [RangeCustom(Minimum = "-8.000000000", Maximum = "8.500000000")]
            public Nullable<decimal> CoordDebPriseTerreLong { get; set; }

            [RegularExpression(@"^\d{1,2}(?:[\,\.]\d{1,9})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultCoordonneesLatFieldErrorMessage")]
            [RangeCustom(Minimum = "41.000000000", Maximum = "52.000000000")]
            public Nullable<decimal> CoordFinPriseTerreLat { get; set; }

            [RegularExpression(@"^[+-]{0,1}\d{1,2}(?:[\,\.]\d{1,9})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultCoordonneesLongFieldErrorMessage")]
            [RangeCustom(Minimum = "-8.000000000", Maximum = "8.500000000")]
            public Nullable<decimal> CoordFinPriseTerreLong { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public Nullable<DateTime> DatePosePriseDeTerre { get; set; }

            public bool PresenceTelemesure { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            [RegularExpression(@"^\d{1,4}(?:[\,\.]\d{0,1})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultResistanceFieldErrorMessage")]
            public Nullable<decimal> ResistanceInitPriseDeTerre { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqDrainageMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqDrainage.
    [MetadataTypeAttribute(typeof(EqDrainage.EqDrainageMetadata))]
    public partial class EqDrainage
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqDrainage.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqDrainageMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqDrainageMetadata()
            {
            }

            [RequiredCustom]
            public int CleTypeDrainage { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            [Include]
            public EntityCollection<EqDrainageLiaisonsext> EqDrainageLiaisonsext { get; set; }

            [EntierPositif]
            [RangeCustom(Minimum = "0", Maximum = "999")]
            [RequiredReference("IntensiteMaximaleSupporteeNullable")]
            public int IntensiteMaximaleSupportee { get; set; }

            public bool PresenceTelemesure { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqDrainageLiaisonsextMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqDrainageLiaisonsext.
    [MetadataTypeAttribute(typeof(EqDrainageLiaisonsext.EqDrainageLiaisonsextMetadata))]
    public partial class EqDrainageLiaisonsext
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqDrainageLiaisonsext.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqDrainageLiaisonsextMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqDrainageLiaisonsextMetadata()
            {
            }

            public int CleDrainage { get; set; }

            public int CleDrainageLext { get; set; }

            public int CleLiaisonExt { get; set; }

            public EqDrainage EqDrainage { get; set; }

            [Include]
            public EqLiaisonExterne EqLiaisonExterne { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqEquipementMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqEquipement.
    [MetadataTypeAttribute(typeof(EqEquipement.EqEquipementMetadata))]
    public partial class EqEquipement : IOuvrage
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqEquipement.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal class EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            protected EqEquipementMetadata()
            {
            }

            public int CleEquipement { get; set; }

            public Nullable<int> CleEquipementOrigine { get; set; }

            [RequiredCustom]
            [RequiredReference("Pp")]
            public int ClePp { get; set; }

            [RequiredCustom]
            [RequiredReference("TypeEquipement")]
            public int CleTypeEq { get; set; }

            public Nullable<int> CleUtilisateur { get; set; }

            [StringLengthCustom(500)]
            public string Commentaire { get; set; }

            [Include]
            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            [RequiredCustom]
            public DateTime DateMajEquipement { get; set; }

            public Nullable<DateTime> DateMiseEnService { get; set; }

            [Include]
            public EqEquipement EqEquipement2 { get; set; }

            [Include]
            public EntityCollection<EqEquipement> EqEquipementEqEquipement { get; set; }

            [Include]
            public EntityCollection<EqRaccordIsolant> EqRaccordIsolant1 { get; set; }

            [Include]
            public EntityCollection<Image> Images { get; set; }

            [RequiredCustom]
            [StringLengthCustom(50)]
            [RequiredReference("LibellePrincipale")]
            public string Libelle { get; set; }

            [Include]
            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            [Include]
            public EntityCollection<MesNiveauProtection> MesNiveauProtection { get; set; }

            [Include]
            [DeleteStateValue]
            public Pp Pp { get; set; }

            public bool Supprime { get; set; }

            [Include]
            public TypeEquipement TypeEquipement { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur { get; set; }

            [Include]
            public EntityCollection<Visite> Visites { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqEquipementTmpMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqEquipementTmp.
    [MetadataTypeAttribute(typeof(EqEquipementTmp.EqEquipementTmpMetadata))]
    public partial class EqEquipementTmp
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqEquipementTmp.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqEquipementTmpMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqEquipementTmpMetadata()
            {
            }

            public int CleEqTmp { get; set; }

            [RequiredCustom]
            [RequiredReference("Pp2")]
            public int ClePp { get; set; }

            [RequiredCustom]
            [RequiredReference("TypeEquipement")]
            public int CleTypeEq { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateValidation { get; set; }

            public bool EstValide { get; set; }

            [RequiredCustom]
            [StringLengthCustom(50)]
            public string Libelle { get; set; }


            [Include]
            public Pp Pp2 { get; set; }


            [Include]
            public TypeEquipement TypeEquipement { get; set; }

            [Include]
            public EntityCollection<Visite> Visites { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqFourreauMetalliqueMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqFourreauMetallique.
    [MetadataTypeAttribute(typeof(EqFourreauMetallique.EqFourreauMetalliqueMetadata))]
    public partial class EqFourreauMetallique
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqFourreauMetallique.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqFourreauMetalliqueMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqFourreauMetalliqueMetadata()
            {
            }

            [RequiredReference("Pp2")]
            public Nullable<int> ClePp2 { get; set; }

            [EntierPositif(true)]
            [RangeCustom(Minimum = "0", Maximum = "999")]
            public int Longueur { get; set; }

            [Include]
            public Pp Pp2 { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqLiaisonExterneMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqLiaisonExterne.
    [MetadataTypeAttribute(typeof(EqLiaisonExterne.EqLiaisonExterneMetadata))]
    public partial class EqLiaisonExterne
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqLiaisonExterne.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqLiaisonExterneMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqLiaisonExterneMetadata()
            {
            }

            [RequiredCustom]
            public int CleNomTiersAss { get; set; }

            [RequiredCustom]
            public int CleTypeLiaison { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            [Include]
            public EntityCollection<EqDrainageLiaisonsext> EqDrainageLiaisonsext { get; set; }

            [Include]
            public EntityCollection<EqSoutirageLiaisonsext> EqSoutirageLiaisonsext { get; set; }

            public bool LiaisonTechnique { get; set; }

            [StringLengthCustom(50)]
            public string LibelleEquipementTiers { get; set; }

            [StringLengthCustom(30)]
            public string LibellePointCommun { get; set; }

            public bool PresencePcSurOuvrageTiers { get; set; }

            public bool PresenceTelemesure { get; set; }

            public bool ProtectionTiersParUnite { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage1 { get; set; }

            [StringLengthCustom(30)]
            [RequiredCustom]
            public string TypeFluide { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqLiaisonInterneMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqLiaisonInterne.
    [MetadataTypeAttribute(typeof(EqLiaisonInterne.EqLiaisonInterneMetadata))]
    public partial class EqLiaisonInterne
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqLiaisonInterne.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqLiaisonInterneMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqLiaisonInterneMetadata()
            {
            }

            public Nullable<int> CleLiaisonInterEe { get; set; }

            [RequiredCustom]
            [RequiredReference("Pp2")]
            public int ClePp2 { get; set; }

            [RequiredCustom]
            public int CleTypeLiaison { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public EntityCollection<EqLiaisonInterne> EqLiaisonInterne1 { get; set; }

            [Include]
            public EqLiaisonInterne EqLiaisonInterne2 { get; set; }

            public bool LiaisonInterEe { get; set; }

            [StringLengthCustom(30)]
            public string LibellePointCommun { get; set; }

            [Include]
            public Pp Pp2 { get; set; }

            public bool PresenceTelemesure { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqPileMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqPile.
    [MetadataTypeAttribute(typeof(EqPile.EqPileMetadata))]
    public partial class EqPile
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqPile.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqPileMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqPileMetadata()
            {
            }

            [RequiredCustom]
            public int CleCaracteristiquePile { get; set; }

            [RequiredCustom]
            public int CleTypeDeversoir { get; set; }

            public Nullable<DateTime> DatePrevisionRenouvellementPile { get; set; }

            public Nullable<DateTime> DateRenouvellementDeversoir { get; set; }

            public Nullable<int> NombrePiles { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqPostegazMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqPostegaz.
    [MetadataTypeAttribute(typeof(EqPostegaz.EqPostegazMetadata))]
    public partial class EqPostegaz
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqPostegaz.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqPostegazMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqPostegazMetadata()
            {
            }

            [StringLengthCustom(30)]
            public string CodePosteGaz { get; set; }

            [StringLengthCustom(50)]
            [RequiredCustom]
            public string TypePoste { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqRaccordIsolantMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqRaccordIsolant.
    [MetadataTypeAttribute(typeof(EqRaccordIsolant.EqRaccordIsolantMetadata))]
    public partial class EqRaccordIsolant
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqRaccordIsolant.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqRaccordIsolantMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqRaccordIsolantMetadata()
            {
            }

            public Nullable<int> CleLiaison { get; set; }

            [RequiredReference("Pp2")]
            public Nullable<int> ClePp2 { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur1")]
            public Nullable<int> CleTypeLiaison { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur1")]
            public int CleTypeRi { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur")]
            public Nullable<int> EnumConfigElectNormale { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur1")]
            public int EnumEtatElect { get; set; }

            [Include]
            public EqEquipement EqEquipement1 { get; set; }

            [Include]
            public Pp Pp2 { get; set; }

            public bool PresenceEclateur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur1 { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage1 { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqSoutirageMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqSoutirage.
    [MetadataTypeAttribute(typeof(EqSoutirage.EqSoutirageMetadata))]
    public partial class EqSoutirage
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqSoutirage.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqSoutirageMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqSoutirageMetadata()
            {
            }

            public bool Autoregule { get; set; }

            [RequiredCustom]
            public int CleDeversoir { get; set; }

            [RequiredCustom]
            public int CleRedresseur { get; set; }

            [RegularExpression(@"^\d{1,2}(?:[\,\.]\d{1,9})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultCoordonneesLatFieldErrorMessage")]
            [RangeCustom(Minimum = "41.000000000", Maximum = "52.000000000")]
            public Nullable<decimal> CoordDebDeversoirLat { get; set; }

            [RegularExpression(@"^[+-]{0,1}\d{1,2}(?:[\,\.]\d{1,9})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultCoordonneesLongFieldErrorMessage")]
            [RangeCustom(Minimum = "-8.000000000", Maximum = "8.500000000")]
            public Nullable<decimal> CoordDebDeversoirLong { get; set; }

            [RegularExpression(@"^\d{1,2}(?:[\,\.]\d{1,9})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultCoordonneesLatFieldErrorMessage")]
            [RangeCustom(Minimum = "41.000000000", Maximum = "52.000000000")]
            public Nullable<decimal> CoordFinDeversoirLat { get; set; }

            [RegularExpression(@"^[+-]{0,1}\d{1,2}(?:[\,\.]\d{1,9})?$",
            ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultCoordonneesLongFieldErrorMessage")]
            [RangeCustom(Minimum = "-8.000000000", Maximum = "8.500000000")]
            public Nullable<decimal> CoordFinDeversoirLong { get; set; }

            [DateTimeRequiredAttribute]
            [RequiredReference("DateControleNullable")]
            public DateTime DateControle { get; set; }

            [RequiredReference("DateMiseEnServiceRedresseurNullable")]
            public DateTime DateMiseEnServiceRedresseur { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public Nullable<DateTime> DatePoseDeversoir { get; set; }

            public Nullable<DateTime> DateRenouvellementDeversoir { get; set; }

            [Include]
            public EntityCollection<EqSoutirageLiaisonsext> EqSoutirageLiaisonsext { get; set; }

            //[RegularExpression(@"^\d{0,2}(?:[\,\.]\d)?$",
            //ErrorMessageResourceType = typeof(ValidationErrorResources),
            //ErrorMessageResourceName = "SOIntensiteReglageFormatError")]

            [MaxDecimalValue(MaxIntegerPartSize = 2, MaxDecimalPartSize = 2, PositiveOrZero = true)]
            [RequiredReference("IntensiteReglageNullable")]
            [RequiredCustom]
            public decimal IntensiteReglage { get; set; }

            //[RegularExpression(@"^\d{0,3}(?:[\,\.]\d{1,2})?$",
            //ErrorMessageResourceType = typeof(ValidationErrorResources),
            //ErrorMessageResourceName = "SOLongueurDeversoirFormatError")]
            [MaxDecimalValue(MaxIntegerPartSize = 3, MaxDecimalPartSize = 2, Positive = true)]
            [RequiredReference("LongueurDeversoirNullable")]
            [RequiredCustom]
            public decimal LongueurDeversoir { get; set; }

            //[RegularExpression(@"^\d{0,2}(?:[\,\.]\d{0,2})?$",
            //ErrorMessageResourceType = typeof(ValidationErrorResources),
            //ErrorMessageResourceName = "SOMasseMetreFormatError")]
            [MaxDecimalValue(MaxIntegerPartSize = 2, MaxDecimalPartSize = 2, Positive = true)]
            public Nullable<decimal> MasseAuMetreLineaire { get; set; }

            public bool PresenceReenclencheur { get; set; }

            public bool PresenceTelemesure { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage1 { get; set; }

            [EntierPositif]
            [RangeCustom(Minimum = "0", Maximum = "99")]
            [RequiredReference("TensionReglageNullable")]
            public int TensionReglage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqSoutirageLiaisonsextMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqSoutirageLiaisonsext.
    [MetadataTypeAttribute(typeof(EqSoutirageLiaisonsext.EqSoutirageLiaisonsextMetadata))]
    public partial class EqSoutirageLiaisonsext
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqSoutirageLiaisonsext.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqSoutirageLiaisonsextMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqSoutirageLiaisonsextMetadata()
            {
            }

            public int CleLiaisonExt { get; set; }

            public int CleSoutirage { get; set; }

            public int CleSoutirageLext { get; set; }

            [Include]
            public EqLiaisonExterne EqLiaisonExterne { get; set; }

            public EqSoutirage EqSoutirage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie EqTiersCroiseSansLiaisonMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe EqTiersCroiseSansLiaison.
    [MetadataTypeAttribute(typeof(EqTiersCroiseSansLiaison.EqTiersCroiseSansLiaisonMetadata))]
    public partial class EqTiersCroiseSansLiaison
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe EqTiersCroiseSansLiaison.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class EqTiersCroiseSansLiaisonMetadata : EqEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private EqTiersCroiseSansLiaisonMetadata()
            {
            }

            public string NomTiersAssocie { get; set; }

            public bool PresencePcSurOuvrageTiers { get; set; }

            public string TypeFluide { get; set; }
        }
    }

    // MetadataTypeAttribute identifie GeoAgenceMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe GeoAgence.
    [MetadataTypeAttribute(typeof(GeoAgence.GeoAgenceMetadata))]
    public partial class GeoAgence
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe GeoAgence.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoAgenceMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private GeoAgenceMetadata()
            {
            }

            public int CleAgence { get; set; }

            [RequiredCustom]
            [RequiredReference("GeoRegion")]
            public int CleRegion { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le code agence doit être unique en base de données
            [StringLengthCustom(3)]
            public string CodeAgence { get; set; }

            [Include]
            public GeoRegion GeoRegion { get; set; }

            [Include]
            public EntityCollection<GeoSecteur> GeoSecteur { get; set; }

            [Include]
            public EntityCollection<InsInstrument> InsInstrument { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le libelle abrege agence doit être unique en base de données
            [StringLengthCustom(20)]
            public string LibelleAbregeAgence { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le libelle agence doit être unique en base de données
            [StringLengthCustom(50)]
            public string LibelleAgence { get; set; }

            public EntityCollection<UsrUtilisateur> UsrUtilisateur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie GeoEnsElecPortionMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe GeoEnsElecPortion.
    [MetadataTypeAttribute(typeof(GeoEnsElecPortion.GeoEnsElecPortionMetadata))]
    public partial class GeoEnsElecPortion
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe GeoEnsElecPortion.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoEnsElecPortionMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie GeoEnsElecPortionEqPpMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe GeoEnsElecPortionEqPp.
    [MetadataTypeAttribute(typeof(GeoEnsElecPortionEqPp.GeoEnsElecPortionEqPpMetadata))]
    public partial class GeoEnsElecPortionEqPp
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe GeoEnsElecPortionEqPp.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoEnsElecPortionEqPpMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie GeoEnsembleElectriqueMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe GeoEnsembleElectrique.
    [MetadataTypeAttribute(typeof(GeoEnsembleElectrique.GeoEnsembleElectriqueMetadata))]
    public partial class GeoEnsembleElectrique
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe GeoEnsembleElectrique.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoEnsembleElectriqueMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie GeoRegionMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe GeoRegion.
    [MetadataTypeAttribute(typeof(GeoRegion.GeoRegionMetadata))]
    public partial class GeoRegion
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe GeoRegion.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoRegionMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private GeoRegionMetadata()
            {
            }

            public int CleRegion { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le code région doit être unique en base de données
            [StringLengthCustom(4)]
            public string CodeRegion { get; set; }

            [Include]
            public EntityCollection<GeoAgence> GeoAgence { get; set; }

            [Include]
            public EntityCollection<InsInstrument> InsInstrument { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le libelle abrege région doit être unique en base de données
            [StringLengthCustom(20)]
            public string LibelleAbregeRegion { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le libelle région doit être unique en base de données
            [StringLengthCustom(50)]
            public string LibelleRegion { get; set; }
        }
    }

    // MetadataTypeAttribute identifie GeoSecteurMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe GeoSecteur.
    [MetadataTypeAttribute(typeof(GeoSecteur.GeoSecteurMetadata))]
    public partial class GeoSecteur
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe GeoSecteur.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class GeoSecteurMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private GeoSecteurMetadata()
            {
            }

            [RequiredCustom]
            [RequiredReference("GeoAgence")]
            public int CleAgence { get; set; }

            public int CleSecteur { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le code secteur doit être unique en base de données
            [StringLengthCustom(3)]
            public string CodeSecteur { get; set; }

            [Include]
            public GeoAgence GeoAgence { get; set; }

            [Include]
            public EntityCollection<InsInstrument> InsInstrument { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le libelle abrege secteur doit être unique en base de données
            [StringLengthCustom(20)]
            public string LibelleAbregeSecteur { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le libelle secteur doit être unique en base de données
            [StringLengthCustom(50)]
            public string LibelleSecteur { get; set; }

            public EntityCollection<PiSecteurs> PiSecteurs { get; set; }

            public EntityCollection<Pp> Pps { get; set; }

            [Include]
            public EntityCollection<UsrUtilisateur> UsrUtilisateur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoAdminMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoAdmin.
    [MetadataTypeAttribute(typeof(HistoAdmin.HistoAdminMetadata))]
    public partial class HistoAdmin
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoAdmin.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoAdminMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private HistoAdminMetadata()
            {
            }

            public int CleHistoAdmin { get; set; }

            [RequiredCustom]
            public DateTime DateModification { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur")]
            public int EnumTypeModification { get; set; }

            [RequiredCustom]
            public string IdConnecte { get; set; }

            [RequiredCustom]
            public string IdUtilisateur { get; set; }

            [RequiredCustom]
            public string NomConnecte { get; set; }

            [RequiredCustom]
            public string NomUtilisateur { get; set; }

            [RequiredCustom]
            public string PrenomConnecte { get; set; }

            [RequiredCustom]
            public string PrenomUtilisateur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [RequiredCustom]
            public string TypeCompte { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqAnodeGalvaniqueMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqAnodeGalvanique.
    [MetadataTypeAttribute(typeof(HistoEqAnodeGalvanique.HistoEqAnodeGalvaniqueMetadata))]
    public partial class HistoEqAnodeGalvanique
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqAnodeGalvanique.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqAnodeGalvaniqueMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private HistoEqAnodeGalvaniqueMetadata()
            {
            }

            public int CleTypeAnode { get; set; }

            public bool PileAssociee { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqDispoEcoulementCourantsAlternatifsMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqDispoEcoulementCourantsAlternatifs.
    [MetadataTypeAttribute(typeof(HistoEqDispoEcoulementCourantsAlternatifs.HistoEqDispoEcoulementCourantsAlternatifsMetadata))]
    public partial class HistoEqDispoEcoulementCourantsAlternatifs
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqDispoEcoulementCourantsAlternatifs.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqDispoEcoulementCourantsAlternatifsMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public Nullable<decimal> ResistanceInitPriseDeTerre { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqDrainageMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqDrainage.
    [MetadataTypeAttribute(typeof(HistoEqDrainage.HistoEqDrainageMetadata))]
    public partial class HistoEqDrainage
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqDrainage.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqDrainageMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private HistoEqDrainageMetadata()
            {
            }

            public int CleTypeDrainage { get; set; }

            public Nullable<DateTime> DateMiseEnServiceTelemesure { get; set; }

            public int IntensiteMaximaleSupportee { get; set; }

            public bool PresenceTelemesure { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqFourreauMetalliqueMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqFourreauMetallique.
    [MetadataTypeAttribute(typeof(HistoEqFourreauMetallique.HistoEqFourreauMetalliqueMetadata))]
    public partial class HistoEqFourreauMetallique
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqFourreauMetallique.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqFourreauMetalliqueMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private HistoEqFourreauMetalliqueMetadata()
            {
            }

            public string LibellePortionPp2 { get; set; }

            public string LibellePp2 { get; set; }

            public decimal Longueur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqLiaisonExterneMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqLiaisonExterne.
    [MetadataTypeAttribute(typeof(HistoEqLiaisonExterne.HistoEqLiaisonExterneMetadata))]
    public partial class HistoEqLiaisonExterne
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqLiaisonExterne.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqLiaisonExterneMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public string TypeFluide { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqLiaisonInterneMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqLiaisonInterne.
    [MetadataTypeAttribute(typeof(HistoEqLiaisonInterne.HistoEqLiaisonInterneMetadata))]
    public partial class HistoEqLiaisonInterne
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqLiaisonInterne.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqLiaisonInterneMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqPileMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqPile.
    [MetadataTypeAttribute(typeof(HistoEqPile.HistoEqPileMetadata))]
    public partial class HistoEqPile
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqPile.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqPileMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private HistoEqPileMetadata()
            {
            }

            public int CleCaracteristiquePile { get; set; }

            public int CleTypeDeversoir { get; set; }

            public Nullable<DateTime> DatePrevisionRenouvellementPile { get; set; }

            public Nullable<DateTime> DateRenouvellementDeversoir { get; set; }

            public Nullable<int> NombrePiles { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqPostegazMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqPostegaz.
    [MetadataTypeAttribute(typeof(HistoEqPostegaz.HistoEqPostegazMetadata))]
    public partial class HistoEqPostegaz
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqPostegaz.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqPostegazMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private HistoEqPostegazMetadata()
            {
            }

            public string CodePosteGaz { get; set; }

            public string TypePoste { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqRaccordIsolantMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqRaccordIsolant.
    [MetadataTypeAttribute(typeof(HistoEqRaccordIsolant.HistoEqRaccordIsolantMetadata))]
    public partial class HistoEqRaccordIsolant
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqRaccordIsolant.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqRaccordIsolantMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage1 { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqSoutirageMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqSoutirage.
    [MetadataTypeAttribute(typeof(HistoEqSoutirage.HistoEqSoutirageMetadata))]
    public partial class HistoEqSoutirage
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqSoutirage.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqSoutirageMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Include]
            public RefSousTypeOuvrage RefSousTypeOuvrage { get; set; }

            public int TensionReglage { get; set; }

            public int TypeDeversoir { get; set; }

            public string TypeRedresseur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEqTiersCroiseSansLiaisonMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEqTiersCroiseSansLiaison.
    [MetadataTypeAttribute(typeof(HistoEqTiersCroiseSansLiaison.HistoEqTiersCroiseSansLiaisonMetadata))]
    public partial class HistoEqTiersCroiseSansLiaison
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEqTiersCroiseSansLiaison.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoEqTiersCroiseSansLiaisonMetadata : HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private HistoEqTiersCroiseSansLiaisonMetadata()
            {
            }

            public string NomTiersAssocie { get; set; }

            public bool PresencePcSurOuvrageTiers { get; set; }

            public string TypeFluide { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoEquipementMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoEquipement.
    [MetadataTypeAttribute(typeof(HistoEquipement.HistoEquipementMetadata))]
    public partial class HistoEquipement
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoEquipement.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal class HistoEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            protected HistoEquipementMetadata()
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

            [Include]
            public LogOuvrage LogOuvrage { get; set; }

            ////Non uilisé dans cette appli
            //public EntityCollection<LogOuvrage> LogOuvrage1 { get; set; }

            public bool Supprime { get; set; }

            public TypeEquipement TypeEquipement { get; set; }
        }
    }

    // MetadataTypeAttribute identifie HistoPpMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe HistoPp.
    [MetadataTypeAttribute(typeof(HistoPp.HistoPpMetadata))]
    public partial class HistoPp
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe HistoPp.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class HistoPpMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private HistoPpMetadata()
            {
            }

            [Include]
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

            [Include]
            public LogOuvrage LogOuvrage { get; set; }

            ////Non uilisé dans cette appli
            //public EntityCollection<LogOuvrage> LogOuvrage1 { get; set; }

            public decimal Pk { get; set; }

            public Nullable<decimal> PositionGpsLat { get; set; }

            public Nullable<decimal> PositionGpsLong { get; set; }

            public string PositionnementPostal { get; set; }

            public bool PpPoste { get; set; }

            public bool PresenceTelemesure { get; set; }

            [Include]
            public RefCommune RefCommune { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur1 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur2 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur3 { get; set; }

            [Include]
            public RefNiveauSensibilitePp RefNiveauSensibilitePp { get; set; }

            public bool Supprime { get; set; }

            public bool TemoinEnterreAmovible { get; set; }

            public bool TemoinMetalliqueDeSurface { get; set; }
        }
    }

    // MetadataTypeAttribute identifie ImageMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe Image.
    [MetadataTypeAttribute(typeof(Image.ImageMetadata))]
    public partial class Image
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe Image.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ImageMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie InsInstrumentMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe InsInstrument.
    [MetadataTypeAttribute(typeof(InsInstrument.InsInstrumentMetadata))]
    public partial class InsInstrument
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe InsInstrument.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class InsInstrumentMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private InsInstrumentMetadata()
            {
            }

            public Nullable<int> CleAgence { get; set; }

            public int CleInstrument { get; set; }

            public Nullable<int> CleRegion { get; set; }

            public Nullable<int> CleSecteur { get; set; }

            [Include]
            public GeoAgence GeoAgence { get; set; }

            [Include]
            public GeoRegion GeoRegion { get; set; }

            [Include]
            public GeoSecteur GeoSecteur { get; set; }

            public EntityCollection<InstrumentsUtilises> InstrumentsUtilises { get; set; }

            [RequiredCustom]
            [StringLengthCustom(50)]
            //[Unique("CleSecteur", "CleAgence", "CleRegion")]
            public string Libelle { get; set; }

            public bool Supprime { get; set; }
        }
    }

    // MetadataTypeAttribute identifie InstrumentsUtilisesMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe InstrumentsUtilises.
    [MetadataTypeAttribute(typeof(InstrumentsUtilises.InstrumentsUtilisesMetadata))]
    public partial class InstrumentsUtilises
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe InstrumentsUtilises.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class InstrumentsUtilisesMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private InstrumentsUtilisesMetadata()
            {
            }

            public int CleInstrument { get; set; }

            public int CleInsUtilises { get; set; }

            [RequiredCustom]
            [RequiredReference("Visite")]
            public int CleVisite { get; set; }

            [Include]
            public InsInstrument InsInstrument { get; set; }

            public Visite Visite { get; set; }
        }
    }

    // MetadataTypeAttribute identifie LogOuvrageMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe LogOuvrage.
    [MetadataTypeAttribute(typeof(LogOuvrage.LogOuvrageMetadata))]
    public partial class LogOuvrage
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe LogOuvrage.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class LogOuvrageMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private LogOuvrageMetadata()
            {
            }

            public Nullable<int> CleEnsElectrique { get; set; }

            public Nullable<int> CleEquipement { get; set; }

            //// Champs non utilisés car ils surcontraignent la relation 1-1 entre ces tables

            //public Nullable<int> CleHistoEquipement { get; set; }

            //public Nullable<int> CleHistoPp { get; set; }

            public int CleLogOuvrage { get; set; }

            public Nullable<int> ClePortion { get; set; }

            public Nullable<int> ClePp { get; set; }

            public int CleUtilisateur { get; set; }

            public DateTime DateHistorisation { get; set; }

            public EnsembleElectrique EnsembleElectrique { get; set; }

            public int EnumTypeModification { get; set; }

            public EqEquipement EqEquipement { get; set; }

            [Include]
            public EntityCollection<HistoEquipement> HistoEquipement { get; set; }

            //public HistoEquipement HistoEquipement1 { get; set; }

            [Include]
            public EntityCollection<HistoPp> HistoPp { get; set; }

            //public HistoPp HistoPp1 { get; set; }

            public string ListeChamps { get; set; }

            public PortionIntegrite PortionIntegrite { get; set; }

            public Pp Pp { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie LogTourneeMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe LogTournee.
    [MetadataTypeAttribute(typeof(LogTournee.LogTourneeMetadata))]
    public partial class LogTournee
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe LogTournee.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class LogTourneeMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Include]
            public UsrUtilisateur UsrUtilisateur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie MesClassementMesureMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe MesClassementMesure.
    [MetadataTypeAttribute(typeof(MesClassementMesure.MesClassementMesureMetadata))]
    public partial class MesClassementMesure
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe MesClassementMesure.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesClassementMesureMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private MesClassementMesureMetadata()
            {
            }

            public int CleClassementMesure { get; set; }

            [RequiredCustom]
            [RequiredReference("MesTypeMesure")]
            public int CleTypeMesure { get; set; }

            public bool CourantsAlternatifsInduits { get; set; }

            public bool CourantsVagabons { get; set; }

            public bool ElectrodeEnterreeAmovible { get; set; }

            [Include]
            public MesTypeMesure MesTypeMesure { get; set; }

            public bool Telemesure { get; set; }

            public bool TemoinDeSurface { get; set; }

            public bool TemoinEnterre { get; set; }
        }
    }

    // MetadataTypeAttribute identifie MesCoutMesureMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe MesCoutMesure.
    [MetadataTypeAttribute(typeof(MesCoutMesure.MesCoutMesureMetadata))]
    public partial class MesCoutMesure
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe MesCoutMesure.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesCoutMesureMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private MesCoutMesureMetadata()
            {
            }

            public int CleCoutMesure { get; set; }

            [RequiredCustom]
            [RequiredReference("TypeEquipement")]
            public int CleTypeEq { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 2, MaxDecimalPartSize = 2)]
            public Nullable<decimal> Cout { get; set; }

            [RequiredReference("RefEnumValeur1")]
            public Nullable<int> EnumDureeEnregistrement { get; set; }

            [RequiredReference("RefEnumValeur2")]
            public Nullable<int> EnumTempsPolarisation { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur")]
            public int EnumTypeEval { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur1 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur2 { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 4, MaxDecimalPartSize = 2, PositiveOrZero = true)]
            public Nullable<decimal> Temps { get; set; }

            [Include]
            public TypeEquipement TypeEquipement { get; set; }
        }
    }

    // MetadataTypeAttribute identifie MesMesureMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe MesMesure.
    [MetadataTypeAttribute(typeof(MesMesure.MesMesureMetadata))]
    public partial class MesMesure
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe MesMesure.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesMesureMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private MesMesureMetadata()
            {
            }

            [Include]
            public EntityCollection<Alerte> Alertes { get; set; }

            public int CleMesure { get; set; }

            [RequiredCustom]
            [RequiredReference("MesTypeMesure")]
            public int CleTypeMesure { get; set; }

            [RequiredCustom]
            [RequiredReference("Visite")]
            public int CleVisite { get; set; }

            [Include]
            public MesTypeMesure MesTypeMesure { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 9, MaxDecimalPartSize = 3)]
            [CheckMesureAttribute]
            public Nullable<decimal> Valeur { get; set; }

            [Include]
            public Visite Visite { get; set; }
        }
    }

    // MetadataTypeAttribute identifie MesModeleMesureMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe MesModeleMesure.
    [MetadataTypeAttribute(typeof(MesModeleMesure.MesModeleMesureMetadata))]
    public partial class MesModeleMesure
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe MesModeleMesure.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesModeleMesureMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private MesModeleMesureMetadata()
            {
            }

            public int CleModeleMesure { get; set; }

            [RequiredCustom]
            [RequiredReference("TypeEquipement")]
            public int CleTypeEq { get; set; }

            [RequiredCustom]
            [RequiredReference("MesUnite")]
            public int CleUnite { get; set; }

            public Nullable<int> EnumTypeGraphique { get; set; }

            [RequiredCustom]
            [StringLengthCustom(80)]
            [Unique("CleTypeEq")]
            public string Libelle { get; set; }

            [RequiredCustom]
            [StringLengthCustom(50)]
            public string LibGenerique { get; set; }

            [Include]
            public EntityCollection<MesNiveauProtection> MesNiveauProtection { get; set; }

            [Include]
            public EntityCollection<MesTypeMesure> MesTypeMesure { get; set; }

            [Include]
            public MesUnite MesUnite { get; set; }

            [EntierPositif]
            [RequiredReference("NumeroOrdreNullable")] // propriété ajouté coté client pour le binding
            public int NumeroOrdre { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public TypeEquipement TypeEquipement { get; set; }
        }
    }

    // MetadataTypeAttribute identifie MesNiveauProtectionMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe MesNiveauProtection.
    [MetadataTypeAttribute(typeof(MesNiveauProtection.MesNiveauProtectionMetadata))]
    public partial class MesNiveauProtection
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe MesNiveauProtection.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesNiveauProtectionMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private MesNiveauProtectionMetadata()
            {
            }

            public Nullable<int> CleEquipement { get; set; }

            [RequiredCustom]
            [RequiredReference("MesModeleMesure")]
            public int CleModeleMesure { get; set; }

            public int CleNiveauProtection { get; set; }

            public Nullable<int> ClePortion { get; set; }

            public Nullable<int> ClePp { get; set; }

            public EqEquipement EqEquipement { get; set; }

            [Include]
            public MesModeleMesure MesModeleMesure { get; set; }

            public PortionIntegrite PortionIntegrite { get; set; }

            public Pp Pp { get; set; }

            [CheckSeuilAttribute]
            public Nullable<decimal> SeuilMaxi { get; set; }

            [CheckSeuilAttribute]
            public Nullable<decimal> SeuilMini { get; set; }
        }
    }

    // MetadataTypeAttribute identifie MesTypeMesureMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe MesTypeMesure.
    [MetadataTypeAttribute(typeof(MesTypeMesure.MesTypeMesureMetadata))]
    public partial class MesTypeMesure
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe MesTypeMesure.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesTypeMesureMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private MesTypeMesureMetadata()
            {
            }

            public int CleModeleMesure { get; set; }

            public int CleTypeMesure { get; set; }

            [StringLengthCustom(50)]
            [CustomValidation(typeof(CustomValidators),
                "CheckLibelleAutre", // le champ est obligatoire si le type de niveau est "Autre"
                ErrorMessageResourceType = typeof(ValidationErrorResources),
                ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            public string LibNivAutre { get; set; }

            [RequiredCustom]
            [StringLengthCustom(80)]
            [Unique("CleModeleMesure")]
            public string LibTypeMesure { get; set; }

            [Include]
            public EntityCollection<MesClassementMesure> MesClassementMesure { get; set; }

            public EntityCollection<MesMesure> MesMesure { get; set; }

            [Include]
            public MesModeleMesure MesModeleMesure { get; set; }

            public bool MesureComplementaire { get; set; }

            public bool MesureEnService { get; set; }

            [RequiredCustom]
            public int NiveauType { get; set; }

            [EntierPositif]
            [RequiredReference("NumeroOrdreNullable")] // propriété ajouté coté client pour le binding
            public int NumeroOrdre { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            [RequiredCustom]
            public int TypeEvaluation { get; set; }
        }
    }

    // MetadataTypeAttribute identifie MesUniteMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe MesUnite.
    [MetadataTypeAttribute(typeof(MesUnite.MesUniteMetadata))]
    public partial class MesUnite
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe MesUnite.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class MesUniteMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private MesUniteMetadata()
            {
            }

            public int CleUnite { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le libelle doit être unique en base de données
            [StringLengthCustom(50)]
            public string Libelle { get; set; }

            [Include]
            public EntityCollection<MesModeleMesure> MesModeleMesure { get; set; }

            [RangeCustom(Minimum = "0", Maximum = "3")]
            [CustomValidation(typeof(CustomValidators), "CheckNbDecimalUnit")]
            public Nullable<int> NombreDeDecimales { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [RequiredCustom]
            [StringLengthCustom(5)]
            public string Symbole { get; set; }

            [RequiredCustom]
            public int TypeDonnee { get; set; }
        }
    }

    // MetadataTypeAttribute identifie ParametreActionMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe ParametreAction.
    [MetadataTypeAttribute(typeof(ParametreAction.ParametreActionMetadata))]
    public partial class ParametreAction
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe ParametreAction.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ParametreActionMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private ParametreActionMetadata()
            {
            }

            public int CleParametreAction { get; set; }

            [StringLengthCustom(30)]
            public string Codification { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 2, MaxDecimalPartSize = 2, PositiveOrZero = true)]
            public Nullable<decimal> Cout { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur")]
            public int EnumCategorieAnomalie { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur1")]
            public int EnumPriorite { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur2")]
            public int EnumDelaiRealisation { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur3")]
            public int EnumTypeAction { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur1 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur2 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur3 { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 4, MaxDecimalPartSize = 2, PositiveOrZero = true)]
            public Nullable<decimal> Temps { get; set; }
        }
    }

    // MetadataTypeAttribute identifie PiSecteursMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe PiSecteurs.
    [MetadataTypeAttribute(typeof(PiSecteurs.PiSecteursMetadata))]
    public partial class PiSecteurs
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe PiSecteurs.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PiSecteursMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private PiSecteursMetadata()
            {
            }

            public int ClePortion { get; set; }

            public int ClePortionSecteurs { get; set; }

            public int CleSecteur { get; set; }

            [Include]
            public GeoSecteur GeoSecteur { get; set; }

            public PortionIntegrite PortionIntegrite { get; set; }
        }
    }

    // MetadataTypeAttribute identifie PortionDatesMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe PortionDates.
    [MetadataTypeAttribute(typeof(PortionDates.PortionDatesMetadata))]
    public partial class PortionDates
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe PortionDates.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PortionDatesMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie PortionIntegriteMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe PortionIntegrite.
    [MetadataTypeAttribute(typeof(PortionIntegrite.PortionIntegriteMetadata))]
    public partial class PortionIntegrite
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe PortionIntegrite.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PortionIntegriteMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private PortionIntegriteMetadata()
            {
            }

            public bool Branchement { get; set; }

            public Nullable<int> CleCommuneArrivee { get; set; }

            public Nullable<int> CleCommuneDepart { get; set; }

            [RequiredCustom]
            [RequiredReference("RefDiametre")] // Pour le binding dans la vue
            public int CleDiametre { get; set; }

            [RequiredCustom]
            [RequiredReference("GeoEnsElec")] // Pour le binding dans la vue
            [RequiredReference("EnsembleElectrique")] // Pour le binding dans la vue
            public int CleEnsElectrique { get; set; }

            public int ClePortion { get; set; }

            [RequiredCustom]
            [RequiredReference("RefRevetement")] // Pour le binding dans la vue
            public int CleRevetement { get; set; }

            [RequiredCustom]
            [StringLengthCustom(16)]
            public string Code { get; set; }

            [StringLengthCustom(16)]
            public string CodeGmao { get; set; }

            [StringLengthCustom(500)]
            public string Commentaire { get; set; }

            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateMajCommentaire { get; set; }

            [RequiredCustom]
            public Nullable<DateTime> DateMiseEnService { get; set; }

            public Nullable<DateTime> DatePose { get; set; }

            [Include]
            public EnsembleElectrique EnsembleElectrique { get; set; }

            public Nullable<int> Idtroncon { get; set; }

            [RequiredCustom]
            [StringLengthCustom(80, 3)]
            [Unique]
            public string Libelle { get; set; }

            [Include]
            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 7, MaxDecimalPartSize = 3, PositiveOrZero = true)]
            [RequiredCustom]
            public Nullable<decimal> Longueur { get; set; }

            [Include]
            public EntityCollection<MesNiveauProtection> MesNiveauProtection { get; set; }

            [Include]
            [RequiredCollectionAttribute(ErrorMessageResourceType = typeof(ValidationErrorResources),
            ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            public EntityCollection<PiSecteurs> PiSecteurs { get; set; }

            [Include]
            public EntityCollection<Pp> Pps { get; set; }

            [Include]
            public RefCommune RefCommune { get; set; }

            [Include]
            public RefCommune RefCommune1 { get; set; }

            [Include]
            public RefDiametre RefDiametre { get; set; }

            [Include]
            public RefRevetement RefRevetement { get; set; }

            public bool Supprime { get; set; }
        }
    }

    // MetadataTypeAttribute identifie PpMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe Pp.
    [MetadataTypeAttribute(typeof(Pp.PpMetadata))]
    public partial class Pp : IOuvrage
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe Pp.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PpMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private PpMetadata()
            {
            }

            public bool BypassCategoriePp { get; set; }

            public bool BypassPkLimitation { get; set; }

            [Include]
            public CategoriePp CategoriePp { get; set; }

            [CustomValidation(typeof(CustomValidators),
              "CheckClassification",
              ErrorMessageResourceType = typeof(ValidationErrorResources),
              ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            [RequiredReference("CategoriePp")]
            public Nullable<int> CleCategoriePp { get; set; }

            [RequiredCustom]
            [RequiredReference("RefCommune")]
            public int CleCommune { get; set; }

            [RequiredCustom]
            public int CleNiveauSensibilite { get; set; }

            [RequiredCustom]
            [RequiredReference("PortionIntegrite")]
            public int ClePortion { get; set; }

            public int ClePp { get; set; }

            public Nullable<int> ClePpOrigine { get; set; }

            [RequiredCustom]
            [RequiredReference("GeoSecteur")]
            public int CleSecteur { get; set; }

            [RequiredReference("UsrUtilisateur1")]
            public Nullable<int> CleUtiDdeDeverrouillage { get; set; }

            [RequiredCustom]
            [RequiredReference("UsrUtilisateur")]
            public int CleUtilisateur { get; set; }

            public string Commentaire { get; set; }

            public string CommentairePositionnement { get; set; }

            [Include]
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

            [CustomValidation(typeof(CustomValidators),
              "CheckTMERaccorde",
              ErrorMessageResourceType = typeof(ValidationErrorResources),
              ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            public Nullable<int> EnumSurfaceTme { get; set; }

            [CustomValidation(typeof(CustomValidators),
              "CheckTMSRaccorde",
              ErrorMessageResourceType = typeof(ValidationErrorResources),
              ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            public Nullable<int> EnumSurfaceTms { get; set; }

            [Include]
            public EntityCollection<EqEquipement> EqEquipement { get; set; }

            public EntityCollection<EqEquipementTmp> EqEquipementTmp { get; set; }

            [Include]
            public EntityCollection<EqFourreauMetallique> EqFourreauMetallique { get; set; }

            [Include]
            public EntityCollection<EqLiaisonInterne> EqLiaisonInterne { get; set; }

            [Include]
            public EntityCollection<EqRaccordIsolant> EqRaccordIsolant { get; set; }

            public GeoSecteur GeoSecteur { get; set; }

            [Include]
            public EntityCollection<Image> Images { get; set; }

            [RequiredCustom]
            public string Libelle { get; set; }

            [Include]
            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            [Include]
            public EntityCollection<MesNiveauProtection> MesNiveauProtection { get; set; }

            [RequiredCustom]
            [MaxDecimalValue(MaxIntegerPartSize = 7, MaxDecimalPartSize = 3)]
            [CustomValidation(typeof(CustomValidators), "CheckPkPP")]
            [RequiredReference("PkNullable")]
            public decimal Pk { get; set; }

            [Include]
            public PortionIntegrite PortionIntegrite { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 2, MaxDecimalPartSize = 9)]
            [RangeCustom(Minimum = "41.000000000", Maximum = "52.000000000")]
            public Nullable<decimal> PositionGpsLat { get; set; }

            [MaxDecimalValue(MaxIntegerPartSize = 1, MaxDecimalPartSize = 9)]
            [RangeCustom(Minimum = "-8.000000000", Maximum = "8.500000000")]
            public Nullable<decimal> PositionGpsLong { get; set; }

            public string PositionnementPostal { get; set; }

            [Include]
            public EntityCollection<Pp> Pp1 { get; set; }

            [Include]
            public Pp Pp2 { get; set; }

            [Include]
            public EntityCollection<PpJumelee> PpJumelee { get; set; }

            [Include]
            public EntityCollection<PpJumelee> PpJumelee1 { get; set; }

            public bool PpPoste { get; set; }

            public EntityCollection<PpTmp> PpTmp { get; set; }

            public bool PresenceDUneTelemesure { get; set; }

            [Include]
            public RefCommune RefCommune { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur1 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur2 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur3 { get; set; }

            [Include]
            public RefNiveauSensibilitePp RefNiveauSensibilitePp { get; set; }

            public bool Supprime { get; set; }

            public bool TemoinEnterreAmovible { get; set; }

            public bool TemoinMetalliqueDeSurface { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur1 { get; set; }

            [Include]
            public EntityCollection<Visite> Visites { get; set; }
        }
    }

    // MetadataTypeAttribute identifie PpEquipementMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe PpEquipement.
    [MetadataTypeAttribute(typeof(PpEquipement.PpEquipementMetadata))]
    public partial class PpEquipement
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe PpEquipement.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PpEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie PpJumeleeMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe PpJumelee.
    [MetadataTypeAttribute(typeof(PpJumelee.PpJumeleeMetadata))]
    public partial class PpJumelee
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe PpJumelee.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PpJumeleeMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private PpJumeleeMetadata()
            {
            }

            public int ClePp { get; set; }

            public int ClePpJumelee { get; set; }

            [Include]
            public Pp Pp { get; set; }

            [Include]
            public Pp Pp1 { get; set; }

            public int PpClePp { get; set; }
        }
    }

    // MetadataTypeAttribute identifie PpTmpMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe PpTmp.
    [MetadataTypeAttribute(typeof(PpTmp.PpTmpMetadata))]
    public partial class PpTmp
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe PpTmp.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class PpTmpMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Include]
            public Pp Pp { get; set; }

            public bool PresenceDUneTelemesure { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur1 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur2 { get; set; }

            [Include]
            public RefEnumValeur RefEnumValeur3 { get; set; }

            [Include]
            public RefNiveauSensibilitePp RefNiveauSensibilitePp { get; set; }

            public bool TemoinEnterreAmovible { get; set; }

            public bool TemoinMetalliqueDeSurface { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur { get; set; }

            [Include]
            public EntityCollection<Visite> Visites { get; set; }
        }
    }

    // MetadataTypeAttribute identifie RefCommuneMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefCommune.
    [MetadataTypeAttribute(typeof(RefCommune.RefCommuneMetadata))]
    public partial class RefCommune
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefCommune.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefCommuneMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private RefCommuneMetadata()
            {
            }

            public int CleCommune { get; set; }

            public string CodeCommune { get; set; }

            public string CodeDepartement { get; set; }

            public string CodeInsee { get; set; }

            [Include]
            public EntityCollection<HistoPp> HistoPp { get; set; }

            public Nullable<int> Idmicado { get; set; }

            public string Libelle { get; set; }

            public string LibelleMajuscule { get; set; }

            public EntityCollection<PortionIntegrite> PortionIntegrite { get; set; }

            public EntityCollection<PortionIntegrite> PortionIntegrite1 { get; set; }

            public EntityCollection<Pp> Pps { get; set; }
        }
    }

    // MetadataTypeAttribute identifie RefDiametreMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefDiametre.
    [MetadataTypeAttribute(typeof(RefDiametre.RefDiametreMetadata))]
    public partial class RefDiametre
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefDiametre.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefDiametreMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie RefEnumValeurMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefEnumValeur.
    [MetadataTypeAttribute(typeof(RefEnumValeur.RefEnumValeurMetadata))]
    public partial class RefEnumValeur
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefEnumValeur.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefEnumValeurMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [Unique("CodeGroupe")] // Indique que l'identifiant doit être unique en base de données
            [StringLengthCustom(100)]
            [RequiredCustom]
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

            [Required(AllowEmptyStrings = true)]
            public string Valeur { get; set; }

            public EntityCollection<Visite> Visites { get; set; }

            public EntityCollection<Visite> Visites1 { get; set; }
        }
    }

    // MetadataTypeAttribute identifie RefNiveauSensibilitePpMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefNiveauSensibilitePp.
    [MetadataTypeAttribute(typeof(RefNiveauSensibilitePp.RefNiveauSensibilitePpMetadata))]
    public partial class RefNiveauSensibilitePp
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefNiveauSensibilitePp.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefNiveauSensibilitePpMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie RefParametreMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefParametre.
    [MetadataTypeAttribute(typeof(RefParametre.RefParametreMetadata))]
    public partial class RefParametre
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefParametre.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefParametreMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

            [CustomValidation(typeof(CustomValidators), "ValidateRefParametreValeur")]
            public string Valeur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie RefRevetementMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefRevetement.
    [MetadataTypeAttribute(typeof(RefRevetement.RefRevetementMetadata))]
    public partial class RefRevetement
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefRevetement.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefRevetementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie RefSousTypeOuvrageMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefSousTypeOuvrage.
    [MetadataTypeAttribute(typeof(RefSousTypeOuvrage.RefSousTypeOuvrageMetadata))]
    public partial class RefSousTypeOuvrage
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefSousTypeOuvrage.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefSousTypeOuvrageMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private RefSousTypeOuvrageMetadata()
            {
            }

            public int CleSousTypeOuvrage { get; set; }

            [RequiredCustom]
            [StringLengthCustom(25)]
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

            [StringLengthCustom(30)]
            [RequiredCustom]
            [Unique("CodeGroupe")]
            public string Libelle { get; set; }

            [EntierPositif]
            [Unique("CodeGroupe")]
            [RequiredReference("DisplayNumOrdreNullable")]
            public int NumeroOrdre { get; set; }

            [StringLengthCustom(100)]
            public string Valeur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie RefUsrAutorisationMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefUsrAutorisation.
    [MetadataTypeAttribute(typeof(RefUsrAutorisation.RefUsrAutorisationMetadata))]
    public partial class RefUsrAutorisation
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefUsrAutorisation.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefUsrAutorisationMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private RefUsrAutorisationMetadata()
            {
            }

            public int CleAutorisation { get; set; }

            public Nullable<int> CleGroupe { get; set; }

            public string CodeAutorisation { get; set; }

            public string LibelleAutorisation { get; set; }

            [Include]
            public RefUsrGroupe RefUsrGroupe { get; set; }

            public string TypePortee { get; set; }

            public EntityCollection<UsrRole> UsrRole { get; set; }
        }
    }

    // MetadataTypeAttribute identifie RefUsrGroupeMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefUsrGroupe.
    [MetadataTypeAttribute(typeof(RefUsrGroupe.RefUsrGroupeMetadata))]
    public partial class RefUsrGroupe
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefUsrGroupe.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefUsrGroupeMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private RefUsrGroupeMetadata()
            {
            }

            public int CleGroupe { get; set; }

            public string LibelleGroupe { get; set; }

            [Include]
            public EntityCollection<RefUsrAutorisation> RefUsrAutorisation { get; set; }
        }
    }

    // MetadataTypeAttribute identifie RefUsrPorteeMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe RefUsrPortee.
    [MetadataTypeAttribute(typeof(RefUsrPortee.RefUsrPorteeMetadata))]
    public partial class RefUsrPortee
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe RefUsrPortee.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class RefUsrPorteeMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie TourneeMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe Tournee.
    [MetadataTypeAttribute(typeof(Tournee.TourneeMetadata))]
    public partial class Tournee
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe Tournee.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class TourneeMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private TourneeMetadata()
            {
            }

            public int CleTournee { get; set; }

            [RequiredReference("UsrUtilisateur")]
            public Nullable<int> CleUtilisateur { get; set; }

            [StringLengthCustom(10)]
            public string CodeTournee { get; set; }

            [StringLengthCustom(100)]
            public string Commentaire { get; set; }

            [Include]
            public EntityCollection<Composition> Compositions { get; set; }

            public Nullable<DateTime> DateCreation { get; set; }

            [RequiredCustom]
            [StringLengthCustom(50)]
            [Unique]
            public string Libelle { get; set; }

            [Include]
            public EntityCollection<LogTournee> LogTournee { get; set; }

            public Nullable<int> Numero { get; set; }

            public bool Supprime { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur { get; set; }

            public bool Verrouille { get; set; }
        }
    }

    // MetadataTypeAttribute identifie TourneePpEqMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe TourneePpEq.
    [MetadataTypeAttribute(typeof(TourneePpEq.TourneePpEqMetadata))]
    public partial class TourneePpEq
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe TourneePpEq.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class TourneePpEqMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie TypeEquipementMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe TypeEquipement.
    [MetadataTypeAttribute(typeof(TypeEquipement.TypeEquipementMetadata))]
    public partial class TypeEquipement
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe TypeEquipement.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class TypeEquipementMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
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

    // MetadataTypeAttribute identifie UsrProfilMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe UsrProfil.
    [MetadataTypeAttribute(typeof(UsrProfil.UsrProfilMetadata))]
    public partial class UsrProfil
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe UsrProfil.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class UsrProfilMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private UsrProfilMetadata()
            {
            }

            [RequiredCustom]
            [RequiredReference("RefUsrPortee")] // Membre lié => Signifie que si la clé <= 0 alors l'erreur de validation sera également déclenchée sur ce membre
            public int ClePortee { get; set; }

            public int CleProfil { get; set; }

            public bool Editable { get; set; }

            [RequiredCustom]
            [Unique] // Indique que le libelle profil doit être unique en base de données
            [StringLengthCustom(30)]
            public string LibelleProfil { get; set; }

            public bool ProfilAdmin { get; set; }

            public RefUsrPortee RefUsrPortee { get; set; }

            [Include]
            public EntityCollection<UsrRole> UsrRole { get; set; }

            [Include]
            public EntityCollection<UsrUtilisateur> UsrUtilisateur { get; set; }
        }
    }

    // MetadataTypeAttribute identifie UsrRoleMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe UsrRole.
    [MetadataTypeAttribute(typeof(UsrRole.UsrRoleMetadata))]
    public partial class UsrRole
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe UsrRole.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class UsrRoleMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private UsrRoleMetadata()
            {
            }

            [RequiredCustom]
            [RequiredReference("RefUsrAutorisation")]
            public int CleAutorisation { get; set; }

            [RequiredCustom]
            [RequiredReference("RefUsrPortee")] // Membre lié => Signifie que si la clé <= 0 alors l'erreur de validation sera également déclenchée sur ce membre
            public int ClePortee { get; set; }

            public int CleProfil { get; set; }

            public int CleRole { get; set; }

            [Include]
            public RefUsrAutorisation RefUsrAutorisation { get; set; }

            [Include]
            public RefUsrPortee RefUsrPortee { get; set; }

            public UsrProfil UsrProfil { get; set; }
        }
    }

    // MetadataTypeAttribute identifie UsrUtilisateurMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe UsrUtilisateur.
    [MetadataTypeAttribute(typeof(UsrUtilisateur.UsrUtilisateurMetadata))]
    public partial class UsrUtilisateur
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe UsrUtilisateur.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class UsrUtilisateurMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private UsrUtilisateurMetadata()
            {
            }

            public EntityCollection<AnAction> AnAction { get; set; }

            public EntityCollection<AnAction> AnAction1 { get; set; }

            public EntityCollection<AnAnalyse> AnAnalyse { get; set; }

            [CustomValidation(typeof(CustomValidators),
                "CheckUsrIfNotExterneAndNotSupprime", // vérification de l'agence en fonction si l'utilisateur est un prestataire ou non
                ErrorMessageResourceType = typeof(ValidationErrorResources),
                ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            [RequiredReference("SelectedAgence")] // C'est le nom de la propriété utilisé au niveau du ViewModel plutot que [RequiredReference("GeoAgence")]
            public Nullable<int> CleAgence { get; set; }

            [CustomValidation(typeof(CustomValidators),
                "CheckUsrIfNotExterneAndNotSupprime", // vérification de l'agence en fonction si l'utilisateur est un prestataire ou non
                ErrorMessageResourceType = typeof(ValidationErrorResources),
                ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            [RequiredReference("UsrProfil")]
            public Nullable<int> CleProfil { get; set; }

            public Nullable<int> CleSecteur { get; set; }

            public int CleUtilisateur { get; set; }

            public EntityCollection<EqEquipement> EqEquipement { get; set; }

            public bool EstPresta { get; set; }

            public bool Externe { get; set; }

            [Include]
            public GeoAgence GeoAgence { get; set; }

            [Include]
            public GeoSecteur GeoSecteur { get; set; }

            public int GestionDesComptes { get; set; }

            [CustomValidation(typeof(CustomValidators),
                "CheckUsrIfNotExterne", // vérification de l'identifiant en fonction si l'utilisateur est un prestataire ou non
                ErrorMessageResourceType = typeof(ValidationErrorResources),
                ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            [StringLengthCustom(10)]
            [Unique] // Indique que l'identifiant doit être unique en base de données
            public string Identifiant { get; set; }

            public EntityCollection<LogOuvrage> LogOuvrage { get; set; }

            public EntityCollection<LogTournee> LogTournee { get; set; }

            [CustomValidation(typeof(CustomValidators),
                "CheckUsrIfNotExterne", // vérification du mail en fonction si l'utilisateur est un prestataire ou non
                ErrorMessageResourceType = typeof(ValidationErrorResources),
                ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            [EmailCustom]
            [StringLengthCustom(70)]
            [Required(AllowEmptyStrings = true)]
            [Unique]
            public string Mail { get; set; }

            [RequiredCustom]
            [StringLengthCustom(20)]
            public string Nom { get; set; }

            public EntityCollection<Pp> Pps { get; set; }

            public EntityCollection<Pp> Pps1 { get; set; }

            public EntityCollection<PpTmp> PpTmp { get; set; }

            [RequiredCustom]
            [StringLengthCustom(15)]
            public string Prenom { get; set; }

            [Include]
            public RefUsrPortee RefUsrPortee { get; set; }

            [CustomValidation(typeof(CustomValidators),
                "CheckUsrIfExterne", // vérification de la société en fonction si l'utilisateur est un prestataire ou non
                ErrorMessageResourceType = typeof(ValidationErrorResources),
                ErrorMessageResourceName = "DefaultRequiredFieldErrorMessage")]
            [StringLengthCustom(30)]
            public string Societe { get; set; }

            public bool Supprime { get; set; }

            public EntityCollection<Tournee> Tournees { get; set; }

            [Include]
            public UsrProfil UsrProfil { get; set; }

            public EntityCollection<Visite> Visites { get; set; }

            public EntityCollection<Visite> Visites1 { get; set; }

            public EntityCollection<Visite> Visites2 { get; set; }

            public EntityCollection<Visite> Visites3 { get; set; }
        }
    }

    // MetadataTypeAttribute identifie VisiteMetadata comme la classe
    // qui comporte des métadonnées supplémentaires pour la classe Visite.
    [MetadataTypeAttribute(typeof(Visite.VisiteMetadata))]
    public partial class Visite
    {

        // Cette classe vous permet d'attacher des attributs personnalisés aux propriétés 
        // de la classe Visite.
        //
        // Par exemple, le code suivant marque la propriété Xyz en tant que
        // propriété requise et spécifie le format pour les valeurs valides :
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class VisiteMetadata
        {

            // Les classes de métadonnées ne sont pas conçues pour être instanciées.
            private VisiteMetadata()
            {
            }

            [Include]
            public EntityCollection<Alerte> Alertes { get; set; }

            public EntityCollection<AnAnalyseEeVisite> AnAnalyseEeVisite { get; set; }

            [Include]
            public EntityCollection<AnAnalyseSerieMesure> AnAnalyseSerieMesure { get; set; }

            [RequiredReference("EqEquipement")]
            public Nullable<int> CleEqTmp { get; set; }

            [RequiredReference("EqEquipementTmp")]
            public Nullable<int> CleEquipement { get; set; }

            [RequiredReference("Pp")]
            public Nullable<int> ClePp { get; set; }

            public Nullable<int> ClePpTmp { get; set; }

            public Nullable<int> CleUtilisateurCreation { get; set; }

            public Nullable<int> CleUtilisateurImport { get; set; }

            [RequiredCustom(AllowZero = true)]
            [RequiredReference("UsrUtilisateur2")]
            public Nullable<int> CleUtilisateurMesure { get; set; }

            public Nullable<int> CleUtilisateurValidation { get; set; }

            public int CleVisite { get; set; }

            [CheckCommentaireVisiteAttribute]
            [StringLengthCustom(500)]
            public string Commentaire { get; set; }

            public Nullable<DateTime> DateImport { get; set; }

            public Nullable<DateTime> DateSaisie { get; set; }

            public Nullable<DateTime> DateValidation { get; set; }

            [RequiredCustom]
            public Nullable<DateTime> DateVisite { get; set; }

            public Nullable<int> EnumConformiteTournee { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur")]
            public int EnumTypeEval { get; set; }

            [RequiredCustom]
            [RequiredReference("RefEnumValeur1")]
            public int EnumTypeEvalComposition { get; set; }

            [Include]
            public EqEquipement EqEquipement { get; set; }

            [Include]
            public EqEquipementTmp EqEquipementTmp { get; set; }

            [RequiredCustom]
            public bool EstValidee { get; set; }

            [Include]
            public EntityCollection<InstrumentsUtilises> InstrumentsUtilises { get; set; }

            [Include]
            public EntityCollection<MesMesure> MesMesure { get; set; }

            [Include]
            public Pp Pp { get; set; }

            public PpTmp PpTmp { get; set; }

            public RefEnumValeur RefEnumValeur { get; set; }

            public RefEnumValeur RefEnumValeur1 { get; set; }

            [RequiredCustom]
            public bool RelevePartiel { get; set; }

            [RequiredCustom]
            public bool Telemesure { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur1 { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur2 { get; set; }

            [Include]
            public UsrUtilisateur UsrUtilisateur3 { get; set; }
        }
    }


    // Classe pour SP_TOURNEE_TABLEAU_BORD_Result
    [MetadataTypeAttribute(typeof(SelectTourneeTableauBord_Result.SelectTourneeTableauBord_Result_Metadata))]
    public partial class SelectTourneeTableauBord_Result
    {
        internal sealed class SelectTourneeTableauBord_Result_Metadata
        {
            // Metadata classes are not meant to be instantiated, so mark constructor as private
            private SelectTourneeTableauBord_Result_Metadata()
            { }

            [Key]
            public int ID { get; set; }
        }
    }

    // Classe pour SP_PORTION_GRAPHIQUE_Result
    [MetadataTypeAttribute(typeof(SelectPortionGraphique_Result.SelectPortionGraphique_Result_Metadata))]
    public partial class SelectPortionGraphique_Result
    {
        internal sealed class SelectPortionGraphique_Result_Metadata
        {
            // Metadata classes are not meant to be instantiated, so mark constructor as private
            private SelectPortionGraphique_Result_Metadata()
            { }

            [Key]
            public int ID { get; set; }
        }
    }
}
