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

namespace Proteca.Silverlight.Enums.NavigationEnums
{
    public enum FiltreNavigation
    {
        [StringValue("PP")]
        PP,
        [StringValue("SO")]
        SO,
        [StringValue("DR")]
        DR,
        [StringValue("LI")]
        LI,
        [StringValue("LE")]
        LE,
        [StringValue("TC")]
        TC,
        [StringValue("FM")]
        FM,
        [StringValue("PO")]
        PO,
        [StringValue("AG")]
        AG,
        [StringValue("DE")]
        DE,
        [StringValue("RI")]
        RI,
        [StringValue("PI")]
        PI,
        [StringValue("Actions")]
        Actions,
        [StringValue("Mesures")]
        Mesures,
        [StringValue("EQ")]
        EQ,
    }

    public enum MainNavigation
    {
        [StringValue("Accueil")]
        Accueil,
        [StringValue("ouvrages")]
        GestionOuvrages,
        [StringValue("Visite")]
        Visite,
        [StringValue("AnalyseRestitution")]
        AnalyseRestitution,
        [StringValue("Administration")]
        Administration,
        [StringValue("Parametres")]
        Parametres,
        [StringValue("Search")]
        Search,
        [StringValue("Odima")]
        Odima
    }

    public enum OuvrageNavigation
    {
        [StringValue("Equipement")]
        Equipement,
        [StringValue("ValidationEquipement")]
        ValidationEquipement,
        [StringValue("Portions")]
        PortionIntegrite,
        [StringValue("EnsembleElectrique")]
        EnsembleElectrique,
        [StringValue("Documentation")]
        Documentation
    }

    public enum VisiteNavigation
    {
        [StringValue("SaisieVisite")]
        SaisieVisite,
        [StringValue("EditionVisite")]
        EditionVisite,
        [StringValue("ValidationVisite")]
        ValidationVisite,
        [StringValue("Import")]
        ImportVisite,
        [StringValue("Tournee")]
        Tournee,
        [StringValue("Alerte")]
        Alerte,
        [StringValue("FicheAction")]
        FicheAction,
        [StringValue("ValidationEquipement")]
        ValidationEquipement
    }

    public enum AnalyseRestitutionNavigation
    {
        [StringValue("AnAnalyseEe")]
        AnAnalyseEe,
        [StringValue("AnaRap_Mesures")]
        AnAnalyseSerieMesure,
        [StringValue("RestitutionBilan")]
        RestitutionBilan
    }

    public enum AdministrationNavigation
    {
        [StringValue("Utilisateur")]
        UsrUtilisateur,
        [StringValue("Profil")]
        UsrProfil,
        [StringValue("InstrumentsMesures")]
        InsInstrument,
        [StringValue("DeplacementPp")]
        DeplacementPp,
        [StringValue("DecoupagePortion")]
        DecoupagePortion,
        [StringValue("DecoupageGeo")]
        DecoupageGeo,
        [StringValue("RegoupementRegion")]
        RegoupementRegion,
        [StringValue("Ressources")]
        Ressources
    }

    public enum ParametresNavigation
    {
        [StringValue("TypeMesures")]
        MesModeleMesure,
        [StringValue("Unites")]
        MesUnites,
        [StringValue("ClassificationMesures")]
        MesClassementMesure,
        [StringValue("Generaux")]
        RefParametre,
        [StringValue("ListeValeurs")]
        RefEnumValeur,
        [StringValue("TypeDocument")]
        TypeDocument,
        [StringValue("CategoriePP")]
        CategoriePp,
        [StringValue("Action")]
        Action
    }

    public enum Adm_DecoupageGeoNavigation
    {
        [StringValue("Region")]
        GeoRegion,
        [StringValue("Agence")]
        GeoAgence,
        [StringValue("Secteur")]
        GeoSecteur
    }

    public enum SearchNavigation
    {
        [StringValue("Search")]
        Search
    }

    public enum OdimaNavigation
    {
        [StringValue("AdminGenerateur")]
        AdminGenerateur
    }

    /// <summary>
    /// Enum comportant les liens des menus du Site Action
    /// </summary>
    public enum SiteActionMenus
    {
        [StringValue("/Pages/default.aspx")] // ?PageView=Shared&ToolPaneView=2 - "/Pages/default.aspx?ToolPaneView=2&pagemode=edit"
        HomePageUrl,
        [StringValue("/Pages/default.aspx?ControlMode=Edit&DisplayMode=Design")] // ?PageView=Shared&ToolPaneView=2 - "/Pages/default.aspx?ToolPaneView=2&pagemode=edit"
        EditModeHomePageUrl,
        [StringValue("/Lists/Glossary/AllItems.aspx")]
        GlossaryListUrl,
        [StringValue("/Lists/OnlineHelp/AllItems.aspx")]
        OnlineHelpListUrl,
        [StringValue("/Lists/DiagnosticHelp/AllItems.aspx")]
        DiagnosticHelpListUrl,
        [StringValue("/Lists/HomeLink/AllItems.aspx")]
        HomeLinkListUrl
    }

    public enum DocumentationNavigation
    {
        [StringValue("EQUIPEMENTS")]
        EQUIPEMENTS,
        [StringValue("PORTIONS")]
        PORTIONS,
        [StringValue("ENSEMBLES_ELECTRIQUES")]
        ENSEMBLES_ELECTRIQUES
    }

    public enum ModulesNavigation
    {
        [StringValue("ProtOn.zip")]
        ProtOn,
        [StringValue("ProteIn.zip")]
        ProteIn
    }
}
