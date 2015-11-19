using System.Collections.ObjectModel;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Proteca.Web.Models;
using System.Linq;
using System;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.ComponentModel.Composition;
using Proteca.Silverlight.Services.Contracts;
using Jounce.Core.Application;
using System.Reflection;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.EntityServices;
using System.Windows;
using System.Collections.Generic;
using Jounce.Framework.Command;
using Jounce.Core.Command;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.IO;
using Proteca.Silverlight.Models;
using System.Globalization;
using Ionic.Zip;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Proteca.Silverlight.Services;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for ImportVisite entity
    /// </summary>
    [ExportAsViewModel("ImportVisite")]
    public class ImportVisiteViewModel : BaseAsyncProtecaEntityViewModel<Visite>
    {
        #region Private Properties

        private bool _isFicTelemesureSelected;
        private bool _isFicProteinSelected;
        private bool _isFicProtOnSelected;
        private bool _isModuleProteinSelected;
        private bool _isModuleProtOnSelected;
        private bool _isFicProteinLoaded;
        private bool _isFicProtOnLoaded;
        private bool _isWorking;
        private bool _isWorkingModule;
        private int indexImport = 0;
        private string _ficProteinLoaded;
        private string _ficProtOnLoaded;
        private string _moduleProteinLoaded;
        private string _moduleProtOnLoaded;
        private string _ficTelemesureLoaded;
        private List<string[]> _rowTable;
        private UsrUtilisateur _userSelected;
        private byte[] _fileContent;

        private bool _portOnAvailable;
        private bool _porteinAvailable;

        private ProgressInfo _LoadProgress;

        /// <summary>
        /// Definition de la liste pour les Visites créées pour les Pp Jumelées
        /// </summary>
        private List<Visite> _visitesForPpJumelee;

        // Taille dans les metadata
        private int _insLibelleSize = 50;
        private int _eqLibelleSize = 50;

        #endregion

        #region Public Properties

        /// <summary>
        /// Tournée en cours d'import retournée depuis la base
        /// </summary>
        public Tournee TourneeLoaded { get; set; }

        /// <summary>
        /// Liste des colonnes dont la visibilité doit être inersée pour l'export
        /// </summary>
        public ObservableCollection<string> ColumnsHiddenToExport
        {
            get
            {
                return new ObservableCollection<string> { Resource.ImportVisite_ContenuEnErreur };
            }
        }

        public Uri ProtOnURI
        {
            get
            {
                return (this.serviceSharePoint as SharepointService).LinkModule(ModulesNavigation.ProtOn);
            }
        }

        public Uri ProteinURI
        {
            get
            {
                return (this.serviceSharePoint as SharepointService).LinkModule(ModulesNavigation.ProteIn);
            }
        }

        public bool ProtOnAvailable
        {
            get { return _portOnAvailable; }
            set
            {
                _portOnAvailable = value;
                RaisePropertyChanged(() => this.ProtOnAvailable);
                SupprimerModuleProtOnCommand.RaiseCanExecuteChanged();
            }
        }
        public ProgressInfo LoadProgress
        {
            get { return _LoadProgress; }
            set { _LoadProgress = value; RaisePropertyChanged(() => LoadProgress); }
        }

        public bool ProteinAvailable
        {
            get { return _porteinAvailable; }
            set
            {
                _porteinAvailable = value;
                RaisePropertyChanged(() => this.ProteinAvailable);
                SupprimerModuleProteinCommand.RaiseCanExecuteChanged();
            }
        }

        public int CleRegionInstrument { get; set; }


        public byte[] FileContent
        {
            get
            {
                return _fileContent;
            }
            set
            {
                _fileContent = value;
            }
        }

        private bool CanChargerModule
        {
            get
            {
                return !IsWorkingModule && ModuleFileInfo != null &&
                    ((IsModuleProteinSelected && IsModuleProteinLoaded && !String.IsNullOrEmpty(this.VersionModuleProtein)) || (IsModuleProtOnSelected && IsModuleProtOnLoaded));
            }
        }

        private bool CanImport
        {
            get
            {
                return !IsWorking && ((IsFicProteinSelected && IsFicProteinLoaded && CanImportProteIn)
                    || (IsFicProtOnSelected && IsFicProtOnLoaded && CanImportProtOn)
                    || (IsFicTelemesureSelected && !String.IsNullOrEmpty(FicTelemesureLoaded) && CanImportTelemesure));
            }
        }

        /// <summary>
        /// Liste permettant de stocker toutes les Visites créées pour les Pp Jumelées
        /// </summary>
        public List<Visite> VisitesForPpJumelee
        {
            get
            {
                if (_visitesForPpJumelee == null)
                {
                    _visitesForPpJumelee = new List<Visite>();
                }
                return _visitesForPpJumelee;
            }
            set { _visitesForPpJumelee = value; }
        }

        public Boolean CanImportTelemesure
        {
            get
            {
                if (this.CurrentUser != null && IsFicTelemesureSelected && !IsWorking)
                {
                    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.IMPORT_TELEM);
                    if (role != null && role.RefUsrPortee != null)
                    {
                        return role.RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue();
                    }
                }
                return false;
            }
        }

        public Boolean CanImportProteIn
        {
            get
            {
                if (this.CurrentUser != null && IsFicProteinSelected && !IsWorking)
                {
                    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.IMPORT_FICHE_VISITE);
                    if (role != null && role.RefUsrPortee != null)
                    {
                        return role.RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue();
                    }
                }
                return false;
            }
        }

        public Boolean CanImportProtOn
        {
            get
            {
                if (this.CurrentUser != null && IsFicProtOnSelected && !IsWorking)
                {
                    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.IMPORT_FICHE_VISITE);
                    if (role != null && role.RefUsrPortee != null)
                    {
                        return role.RefUsrPortee.CodePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue();
                    }
                }
                return false;
            }
        }

        private List<InsInstrument> _listInstrument;
        public List<InsInstrument> ListInstrument
        {
            get
            {
                if (_listInstrument == null)
                {
                    _listInstrument = new List<InsInstrument>();
                }
                return _listInstrument;
            }
            set
            {
                _listInstrument = value;
            }
        }

        private List<UsrUtilisateur> _listNewUser;
        public List<UsrUtilisateur> ListNewUser
        {
            get
            {
                if (_listNewUser == null)
                {
                    _listNewUser = new List<UsrUtilisateur>();
                }
                return _listNewUser;
            }
            set
            {
                _listNewUser = value;
            }
        }

        private string _libelleTourneeProtein;
        public String LibelleTourneeProtein
        {
            get
            {
                return _libelleTourneeProtein;
            }
            set
            {
                _libelleTourneeProtein = value;
                RaisePropertyChanged(() => this.LibelleTourneeProtein);
            }
        }

        private string _libelleTourneeProtOn;
        public String LibelleTourneeProtOn
        {
            get
            {
                return _libelleTourneeProtOn;
            }
            set
            {
                _libelleTourneeProtOn = value;
                RaisePropertyChanged(() => this.LibelleTourneeProtOn);
            }
        }

        private string _version;
        public String Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                RaisePropertyChanged(() => this.Version);
            }
        }

        private string _versionModuleProtein;
        public String VersionModuleProtein
        {
            get
            {
                return _versionModuleProtein;
            }
            set
            {
                _versionModuleProtein = value;
                RaisePropertyChanged(() => this.VersionModuleProtein);
                ChargerModuleCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserName { get; set; }
        public string UserPName { get; set; }
        public string UserSociete { get; set; }

        /// <summary>
        /// Retourne le ficheir protein contenant les datas
        /// </summary>
        public XDocument XmlDocProtein { get; set; }

        /// <summary>
        /// Retourne le ficheir protein contenant les datas
        /// </summary>
        public XDocument XmlDocProtOn { get; set; }

        /// <summary>
        /// Retourne le nom du fichier sans son extension
        /// </summary>
        public string FileNameWithoutExtension { get; set; }

        /// <summary>
        /// Retourne l'utilisateur servant pour l'import
        /// </summary>
        public UsrUtilisateur UserImport { get; set; }

        /// <summary>
        /// Retourne l'utilisateur nouvellement créé dans la fenêtre
        /// </summary>
        public UsrUtilisateur UserInput { get; set; }

        /// <summary>
        /// Retourne l'utilisateur de télémesure
        /// </summary>
        public UsrUtilisateur UserTelemesure { get; set; }

        /// <summary>
        /// Retourne l'utilisateur sélectionné
        /// </summary>
        public UsrUtilisateur UserSelected
        {
            get
            {
                return _userSelected;
            }
            set
            {
                _userSelected = value;
                RaisePropertyChanged(() => this.UserSelected);
            }
        }

        private ObservableCollection<String> _lines;
        /// <summary>
        /// Retourne la liste des lignes importés
        /// </summary>
        public ObservableCollection<String> Lines
        {
            get
            {
                if (_lines == null)
                {
                    _lines = new ObservableCollection<string>();
                }
                return _lines;
            }
            set
            {
                _lines = value;
            }
        }

        /// <summary>
        /// Retourne la liste des lignes importés
        /// </summary>
        public List<string[]> RowTable
        {
            get
            {
                if (_rowTable == null)
                {
                    _rowTable = new List<string[]>();
                }
                return _rowTable;
            }
            set
            {
                _rowTable = value;
                RaisePropertyChanged(() => this.RowTable);
            }
        }

        private ObservableCollection<VisiteImport> _visitesImport;
        /// <summary>
        /// Retourne les visites contenus dans le fichier à importer
        /// </summary>
        public ObservableCollection<VisiteImport> VisitesImport
        {
            get
            {
                if (_visitesImport == null)
                {
                    _visitesImport = new ObservableCollection<VisiteImport>();
                }
                return _visitesImport;
            }
            set
            {
                _visitesImport = value;
                RaisePropertyChanged(() => this.VisitesImport);
                RaisePropertyChanged(() => this.VisitesImportRapportFinal);
                RaisePropertyChanged(() => this.ResultCountCustom);
            }
        }

        private ObservableCollection<VisiteImportRapport> _visitesImportRapportFinal;
        /// <summary>
        /// Retourne les imports en erreur
        /// </summary>
        public ObservableCollection<VisiteImportRapport> VisitesImportRapportFinal
        {
            get
            {
                if (_visitesImportRapportFinal == null)
                {
                    _visitesImportRapportFinal = new ObservableCollection<VisiteImportRapport>();
                }
                return _visitesImportRapportFinal;
            }
            set
            {
                _visitesImportRapportFinal = value;
                RaisePropertyChanged(() => this.VisitesImportRapportFinal);
                RaisePropertyChanged(() => this.ResultCountCustom);
            }
        }

        /// <summary>
        /// MANTIS 10882 FSI 23/06/2014 : Import télémesure V3
        /// Retourne une string formattée affichant le nombre d'éléments importés et ceux en erreur
        /// </summary>
        public String ResultCountCustom
        {
            get
            {
                string result = this.VisitesImportRapportFinal.Count.ToString();
                int nbErrors = this.VisitesImportRapportFinal.Where(f => f.ImgError == Proteca.Web.Resources.ResourceImg.Error).Count();
                if (nbErrors > 0)
                {
                    result += String.Format(Resource.ImportVisite_TabCount, nbErrors);
                }
                return result;
            }
        }

        /// <summary>
        /// Retourne le fichier sélectionné pour les protein
        /// </summary>
        public string FicProteinLoaded
        {
            get
            {
                return _ficProteinLoaded;
            }
            set
            {
                _ficProteinLoaded = value;
                RaisePropertyChanged(() => this.FicProteinLoaded);
            }
        }

        /// <summary>
        /// Retourne le fichier sélectionné pour les protOn
        /// </summary>
        public string FicProtOnLoaded
        {
            get
            {
                return _ficProtOnLoaded;
            }
            set
            {
                _ficProtOnLoaded = value;
                RaisePropertyChanged(() => this.FicProtOnLoaded);
            }
        }

        /// <summary>
        /// Retourne le fichier sélectionné pour les télémesure
        /// </summary>
        public string FicTelemesureLoaded
        {
            get
            {
                return _ficTelemesureLoaded;
            }
            set
            {
                _ficTelemesureLoaded = value;
                RaisePropertyChanged(() => this.FicTelemesureLoaded);
            }
        }

        /// <summary>
        /// Retourne le fichier sélectionné pour le module protein
        /// </summary>
        public string ModuleProteinLoaded
        {
            get
            {
                return _moduleProteinLoaded;
            }
            set
            {
                _moduleProteinLoaded = value;
                if (!String.IsNullOrEmpty(value))
                {
                    ModuleProtOnLoaded = String.Empty;
                }
                RaisePropertyChanged(() => this.ModuleProteinLoaded);
            }
        }

        /// <summary>
        /// Retourne le fichier sélectionné pour le module protOn
        /// </summary>
        public string ModuleProtOnLoaded
        {
            get
            {
                return _moduleProtOnLoaded;
            }
            set
            {
                _moduleProtOnLoaded = value;
                if (!String.IsNullOrEmpty(value))
                {
                    ModuleProteinLoaded = String.Empty;
                    VersionModuleProtein = String.Empty;
                }
                RaisePropertyChanged(() => this.ModuleProtOnLoaded);
            }
        }

        /// <summary>
        /// Déclaration de la variable de filtre de l'entité Utilisateur
        /// </summary>
        protected List<Expression<Func<UsrUtilisateur, bool>>> FiltresUser { get; set; }

        /// <summary>
        /// Booléen de sélection du mode d'import des fichiers télémesure
        /// </summary>
        public bool IsFicTelemesureSelected
        {
            get
            {
                return _isFicTelemesureSelected;
            }
            set
            {
                _isFicTelemesureSelected = value;
                RaisePropertyChanged(() => this.IsFicTelemesureSelected);
                ChargerFicProteinCommand.RaiseCanExecuteChanged();
                ChargerFicProtOnCommand.RaiseCanExecuteChanged();
                ChargerFicTelemesureCommand.RaiseCanExecuteChanged();
                ImportCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Booléen de sélection du mode d'import d'un fichier Prote'in
        /// </summary>
        public bool IsFicProteinSelected
        {
            get
            {
                return _isFicProteinSelected;
            }
            set
            {
                _isFicProteinSelected = value;
                RaisePropertyChanged(() => this.IsFicProteinSelected);
                RaisePropertyChanged(() => this.IsFicProteinWorking);
                ChargerFicProteinCommand.RaiseCanExecuteChanged();
                ChargerFicProtOnCommand.RaiseCanExecuteChanged();
                ChargerFicTelemesureCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsFicProteinWorking
        {
            get
            {
                return !IsWorking && IsFicProteinSelected;
            }
        }

        public bool IsFicProtOnSelected
        {
            get
            {
                return _isFicProtOnSelected;
            }
            set
            {
                _isFicProtOnSelected = value;
                RaisePropertyChanged(() => this.IsFicProtOnSelected);
                ChargerFicProteinCommand.RaiseCanExecuteChanged();
                ChargerFicProtOnCommand.RaiseCanExecuteChanged();
                ChargerFicTelemesureCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Booléen de sélection du mode d'import d'un fichier Prote'in
        /// </summary>
        public bool IsModuleProteinSelected
        {
            get
            {
                return _isModuleProteinSelected;
            }
            set
            {
                _isModuleProteinSelected = value;
                _isModuleProtOnSelected = !value;
                RaisePropertyChanged(() => this.IsModuleProteinSelected);
                RaisePropertyChanged(() => this.IsModuleProtOnSelected);
                ChargerModuleProteinCommand.RaiseCanExecuteChanged();
                ChargerModuleProtOnCommand.RaiseCanExecuteChanged();
                ChargerModuleCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Booléen de sélection du mode d'import d'un fichier Prote'in
        /// </summary>
        public bool IsModuleProtOnSelected
        {
            get
            {
                return _isModuleProtOnSelected;
            }
            set
            {
                _isModuleProtOnSelected = value;
                _isModuleProteinSelected = !value;
                RaisePropertyChanged(() => this.IsModuleProteinSelected);
                RaisePropertyChanged(() => this.IsModuleProtOnSelected);
                ChargerModuleProteinCommand.RaiseCanExecuteChanged();
                ChargerModuleProtOnCommand.RaiseCanExecuteChanged();
                ChargerModuleCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Booléen de validation d'import d'un fichier Prote'in
        /// </summary>
        public bool IsFicProteinLoaded
        {
            get
            {
                if (IsWorking)
                {
                    return false;
                }
                return _isFicProteinLoaded && _isFicProteinSelected;
            }
            set
            {
                _isFicProteinLoaded = value;
                RaisePropertyChanged(() => this.IsFicProteinLoaded);

                ImportCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Booléen de validation d'import d'un fichier Prote'in
        /// </summary>
        public bool IsFicProtOnLoaded
        {
            get
            {
                if (IsWorking)
                {
                    return false;
                }
                return _isFicProtOnLoaded && _isFicProtOnSelected;
            }
            set
            {
                _isFicProtOnLoaded = value;
                RaisePropertyChanged(() => this.IsFicProtOnLoaded);

                ImportCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Booléen de validation d'import d'un fichier Prote'in
        /// </summary>
        public bool IsModuleProteinLoaded
        {
            get
            {
                if (!IsWorkingModule && ModuleFileInfo != null)
                {
                    return this.ModuleNavigation == ModulesNavigation.ProteIn;
                }
                return false;
            }
        }

        /// <summary>
        /// Booléen de validation d'import d'un fichier Prote'in
        /// </summary>
        public bool IsModuleProtOnLoaded
        {
            get
            {
                if (!IsWorkingModule && ModuleFileInfo != null)
                {
                    return this.ModuleNavigation == ModulesNavigation.ProtOn;
                }
                return false;
            }
        }

        private FileInfo _moduleFileInfo;
        /// <summary>
        /// Accès au fichier
        /// </summary>
        public FileInfo ModuleFileInfo
        {
            get
            {
                return _moduleFileInfo;
            }
            set
            {
                _moduleFileInfo = value;
                RaisePropertyChanged(() => this.ModuleFileInfo);
            }
        }

        /// <summary>
        /// Enum du fichier en cours
        /// </summary>
        public ModulesNavigation ModuleNavigation
        {
            get
            {
                return String.IsNullOrEmpty(ModuleProtOnLoaded) ? ModulesNavigation.ProteIn : ModulesNavigation.ProtOn;
            }
        }

        /// <summary>
        /// Booléen indiquant si le travaille d'import est en cours
        /// </summary>
        public bool IsWorking
        {
            get
            {
                return _isWorking;
            }
            set
            {
                _isWorking = value;
                RaisePropertyChanged(() => this.IsWorking);
                RaisePropertyChanged(() => this.IsFicProteinWorking);
                ChargerFicProteinCommand.RaiseCanExecuteChanged();
                ChargerFicProtOnCommand.RaiseCanExecuteChanged();
                ChargerFicTelemesureCommand.RaiseCanExecuteChanged();
                ImportCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Booléen indiquant si le travaille d'import de module est en cours
        /// </summary>
        public bool IsWorkingModule
        {
            get
            {
                return _isWorkingModule;
            }
            set
            {
                _isWorkingModule = value;
                RaisePropertyChanged(() => this.IsWorkingModule);
                ChargerModuleProteinCommand.RaiseCanExecuteChanged();
                ChargerModuleProtOnCommand.RaiseCanExecuteChanged();
                ChargerModuleCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Retourne la liste des utilisateurs abilité à être qualifié comme agents de mesure
        /// </summary>
        public ObservableCollection<UsrUtilisateur> Agents
        {
            get
            {
                return new ObservableCollection<UsrUtilisateur>(serviceUtilisateur.Entities.OrderBy(u => u.Nom).ThenBy(u => u.Prenom));
            }
        }

        /// <summary>
        /// Liste des visites nouvelles trouvées dans le fichier protOn importé
        /// </summary>
        public ObservableCollection<Visite> ImportedEntities
        {
            get
            {
                return (this.service as VisiteService).ImportedEntities;
            }
        }

        /// <summary>
        /// Tournee ayant servi à l'export du fichier protOn importé
        /// </summary>
        public Tournee ImportedTournee
        {
            get
            {
                return (this.service as VisiteService).ImportedTournee;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Déclaration de l'objet de command d'importation des lignes
        /// </summary>
        public IActionCommand ImportCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de chargment du fichier protein
        /// </summary>
        public IActionCommand ChargerFicProteinCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de chargment du fichier proton
        /// </summary>
        public IActionCommand ChargerFicProtOnCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de chargment du fichier télémesure
        /// </summary>
        public IActionCommand ChargerFicTelemesureCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de chargment du module protein
        /// </summary>
        public IActionCommand ChargerModuleProteinCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de chargment du module proton
        /// </summary>
        public IActionCommand ChargerModuleProtOnCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de chargment du module déporté
        /// </summary>
        public IActionCommand ChargerModuleCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de chargment du module protein
        /// </summary>
        public IActionCommand SupprimerModuleProteinCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de chargment du module proton
        /// </summary>
        public IActionCommand SupprimerModuleProtOnCommand { get; private set; }

        #endregion

        #region Service

        /// <summary>
        /// Service utilisé pour gérer les documents sharepoint
        /// </summary>
        [Import]
        public IEntityService<Document> serviceSharePoint { get; set; }

        /// <summary>
        /// Service utilisé pour récupérer la tournée en cours d'import
        /// </summary>
        [Import]
        public IEntityService<Tournee> serviceTournee { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les listes des utilisateurs
        /// </summary>
        [Import]
        public IEntityService<UsrUtilisateur> serviceUtilisateur { get; set; }

        /// <summary>
        /// Service pour récupérer les RefUsrPortee
        /// </summary>
        [Import]
        public IEntityService<RefUsrPortee> serviceRefUsrPortee { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les refenumvaleurs
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> serviceRefEnum { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les équipements
        /// </summary>
        [Import]
        public IEntityService<EqEquipement> serviceEqEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les équipements temporaires
        /// </summary>
        [Import]
        public IEntityService<EqEquipementTmp> serviceEquipementTmp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les équipements temporaires
        /// </summary>
        [Import]
        public IEntityService<PpTmp> servicePpTmp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les PPs
        /// </summary>
        [Import]
        public IEntityService<Pp> servicePP { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les types de mesure
        /// </summary>
        [Import]
        public IEntityService<MesTypeMesure> serviceMesTypeMesure { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les dépassement seuils
        /// </summary>
        [Import]
        public IEntityService<MesClassementMesure> serviceMesClassementMesure { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les mesures
        /// </summary>
        [Import]
        public IEntityService<MesMesure> serviceMesMesure { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les instruments
        /// </summary>
        [Import]
        public IEntityService<InsInstrument> serviceInstrument { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les type d'équipements
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> serviceTypeEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les type de région
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les alertes
        /// </summary>
        [Import]
        public IEntityService<Alerte> serviceAlerte { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les alertes
        /// </summary>
        [Import]
        public IEntityService<AnAnalyseSerieMesure> serviceAnAnalyseSerieMesure { get; set; }

        #endregion


        [Import]
        public IConfigurator serviceConfigurator { get; set; }

        #region Constructor

        public ImportVisiteViewModel()
            : base()
        {
            IsAutoNavigateToFirst = false;

            this.OnViewActivated += (o, e) =>
            {
                (this.serviceMesClassementMesure as MesClassementMesureService).GetMesClassementMesureWithMesNiveauProtection(error =>
                {
                    if (error != null || serviceMesClassementMesure.Entities == null || !serviceMesClassementMesure.Entities.Any())
                    {
                        ErrorWindow.CreateNew("Erreur au chargement des niveaux de protection");
                    }
                });

                Initialisation();
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                ChargerFicProteinCommand.RaiseCanExecuteChanged();
                ChargerFicProtOnCommand.RaiseCanExecuteChanged();
                ChargerFicTelemesureCommand.RaiseCanExecuteChanged();
            };

            // Define commands
            ImportCommand = new ProtecaActionCommand<object>(
                obj => Importer(), obj => CanImport);
            ChargerFicProteinCommand = new ProtecaActionCommand<object>(
                obj => PreChargerFicProtein(), obj => CanImportProteIn);
            ChargerFicProtOnCommand = new ProtecaActionCommand<object>(
                obj => PreChargerFicProtOn(), obj => CanImportProtOn);
            ChargerFicTelemesureCommand = new ProtecaActionCommand<object>(
                obj => PreChargerFicTelemesure(), obj => CanImportTelemesure);
            ChargerModuleCommand = new ProtecaActionCommand<object>(
                obj => ChargerModule(), obj => CanChargerModule);
            ChargerModuleProteinCommand = new ProtecaActionCommand<object>(
                obj => ChargerModuleProtein(), obj => IsModuleProteinSelected);
            ChargerModuleProtOnCommand = new ProtecaActionCommand<object>(
                obj => ChargerModuleProtOn(), obj => IsModuleProtOnSelected);
            SupprimerModuleProteinCommand = new ProtecaActionCommand<object>(
                obj => SupprimerModule(ModulesNavigation.ProteIn), obj => ProteinAvailable);
            SupprimerModuleProtOnCommand = new ProtecaActionCommand<object>(
                obj => SupprimerModule(ModulesNavigation.ProtOn), obj => ProtOnAvailable);

            LoadProgress = new ProgressInfo();
        }

        #endregion

        #region Private Methods

        private void ChargerModule()
        {
            //TODO : Intelligence du chargement de module
            if (ModuleFileInfo != null && FileContent.Length > 0)
            {
                this.IsBusy = true;
                ((DocumentService)this.serviceSharePoint).UploadModule(this.ModuleNavigation,
                    (this.ModuleNavigation == ModulesNavigation.ProteIn) ? VersionModuleProtein : String.Empty,
                    FileContent, this.ModuleLoaded);
            }
        }

        private void ChargerModuleProtein()
        {
            this.LookupForModule();
            if (ModuleFileInfo != null)
            {
                ModuleProteinLoaded = ModuleFileInfo.Name;
                ChargerModuleCommand.RaiseCanExecuteChanged();
            }
        }

        private void ChargerModuleProtOn()
        {
            this.LookupForModule();
            if (ModuleFileInfo != null)
            {
                ModuleProtOnLoaded = ModuleFileInfo.Name;
                ChargerModuleCommand.RaiseCanExecuteChanged();
            }
        }

        private void SupprimerModule(ModulesNavigation module)
        {
            this.IsBusy = true;
            (this.serviceSharePoint as SharepointService).DeleteModule(module, DeleteDone);
        }

        /// <summary>
        /// Fonction de set du fichier d'import
        /// </summary>
        /// <param name="filter">On peut définir un nom de type de fichier</param>
        private void LookupForModule(String filter = "")
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = string.IsNullOrEmpty(filter) ? "Fichier Zip (*.zip)|*.zip" : string.Format("{0} (*.zip)|*.zip", filter);

            bool? userClickedOK = dlg.ShowDialog();
            if (userClickedOK == true)
            {
                this.IsBusy = true;
                ModuleFileInfo = dlg.File;
                using (Stream input = dlg.File.OpenRead())
                {
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        this.FileContent = ms.ToArray();
                    }
                }
                this.IsBusy = false;
            }
            else
            {
                Initialisation();
            }
        }

        /// <summary>
        /// Import des données
        /// </summary>
        private void Importer()
        {
            if (IsFicTelemesureSelected && Lines != null && Lines.Any())
            {
                IsWorking = true;
                IsBusy = true;

                (this.service as VisiteService).ImportTelemesures(this.CurrentUser.CleUtilisateur, this.Lines, this.ImportTelemesuresDone);
            }
            else if (IsFicProteinSelected && IsFicProteinLoaded)
            {
                if (UserSelected != null ||
                    (UserInput != null
                    && !String.IsNullOrEmpty(UserInput.Nom)
                    && !String.IsNullOrEmpty(UserInput.Prenom)
                    && !String.IsNullOrEmpty(UserInput.Societe)))
                {
                    IsWorking = true;
                    IsBusy = true;

                    // Affectation de l'utilisateur, création si aucun utilisateur sélectionné dans la liste
                    if (UserSelected == null)
                    {
                        UserInput.RefUsrPortee = this.serviceRefUsrPortee.Entities.FirstOrDefault(rup => rup.CodePortee == "05" && rup.TypePortee == "USR");

                        // Ajout du nouvel utilisateur
                        serviceUtilisateur.Add(UserInput);

                        UserImport = UserInput;
                    }
                    else
                    {
                        UserInput = null;
                        UserImport = UserSelected;
                    }

                    // Fonction d'import
                    ImporterLesLignes();
                }
                else
                {
                    ErrorWindow.CreateNew("Veuillez sélectionner un utilisateur dans la liste d'agents de mesure ou renseigner les champs correspondants");
                }
            }
            else if (IsFicProtOnSelected && IsFicProtOnLoaded)
            {
                IsWorking = true;
                IsBusy = true;

                // Fonction d'import
                ImporterLesLignes();
            }
            else
            {
                ErrorWindow.CreateNew("Aucun traitement à réaliser");
            }
        }

        /// <summary>
        /// Import une ligne du fichier
        /// </summary>
        private void ImporterLesLignes()
        {
            CleRegionInstrument = 0;

            // Instanciation de la liste des clés
            List<int> ListCleTypeMesure = new List<int>();

            // Initialisation de la visiteimport
            if (IsFicProteinSelected)
            {
                // Construction des requêtes
                var queryEQs = "/ExportTournee/Tournee/Equipements/EQ[Visites/VisiteProteIN/Valide='true']";
                var queryPPs = "/ExportTournee/Tournee/PPs/PP[Visites/VisiteProteIN/Valide='true']";
                var queryAllPPs = "/ExportTournee/Tournee/PPs/PP";
                var queryEQsCleTypeMsr = "MesMesures/*/CleTypeMesure";
                var queryInstrumentCle = "/ExportTournee/Tournee/Instruments/*/CleInstrument";

                // Récupération des résultats
                IEnumerable<XElement> mesEQs = XmlDocProtein.XPathSelectElements(queryEQs);
                IEnumerable<XElement> mesPPs = XmlDocProtein.XPathSelectElements(queryPPs);

                XElement firstPP = XmlDocProtein.XPathSelectElements(queryAllPPs).FirstOrDefault();
                if (firstPP != null)
                {
                    int cleSecteur = int.Parse(firstPP.XPathSelectElement("CleSecteur").Value);
                    GeoRegion region = serviceRegion.Entities.FirstOrDefault(r => r.GeoAgence.Any(a => a.GeoSecteur.Any(s => s.CleSecteur == cleSecteur)));
                    if (region != null)
                    {
                        CleRegionInstrument = region.CleRegion;
                    }
                }

                int index = 0;

                // Pour chaque équipements récupérés, on traite la visite
                foreach (XElement monEQ in mesEQs)
                {
                    VisiteImport monImport = new VisiteImport() { };
                    VisitesImport.Add(monImport);

                    try
                    {
                        // Champs VISITEIMPORT
                        monImport.NomFichier = FicProteinLoaded;
                        monImport.IndexOnFile = index++;

                        // SPECIFIQUE PROTE'IN
                        monImport.MaVisiteProtein = monEQ.XPathSelectElement("Visites/VisiteProteIN");
                        monImport.MesVisitesMesures = monImport.MaVisiteProtein.XPathSelectElements("MesMesures/MesMesure");

                        // Ajout des clés de type de mesures
                        foreach (XElement MaCleTypeMesure in monImport.MaVisiteProtein.XPathSelectElements(queryEQsCleTypeMsr))
                        {
                            ListCleTypeMesure.Add((int)MaCleTypeMesure);
                            monImport.ListCleTypeMesure.Add((int)MaCleTypeMesure);
                        }

                        // Ajout des clés des intruments
                        foreach (XElement MaCleInstrument in XmlDocProtein.XPathSelectElements(queryInstrumentCle))
                        {
                            monImport.ListCleInstrument.Add(MaCleInstrument.IsEmpty || String.IsNullOrEmpty(MaCleInstrument.Value) ? 0 : int.Parse(MaCleInstrument.Value));
                        }

                        // Champs VISITES
                        monImport.CleEquipement = monEQ.XPathSelectElement("CleEquipement").IsEmpty ? 0 : int.Parse(monEQ.XPathSelectElement("CleEquipement").Value);
                        monImport.ClePpAssocie = int.Parse(monEQ.XPathSelectElement("ClePp").Value);
                        monImport.EnumTypeEval = monImport.MaVisiteProtein.XPathSelectElement("EnumTypeEval").IsEmpty ? int.Parse(monEQ.XPathSelectElement("CleTypeEvaluation").Value) : int.Parse(monImport.MaVisiteProtein.XPathSelectElement("EnumTypeEval").Value);
                        monImport.EnumTypeEvalComposition = int.Parse(monEQ.XPathSelectElement("CleTypeEvaluation").Value);
                        monImport.HasAnalyse = !monImport.MaVisiteProtein.XPathSelectElement("CleAnalyse").IsEmpty && !String.IsNullOrEmpty(monImport.MaVisiteProtein.XPathSelectElement("CleAnalyse").Value);
                        monImport.TypeEquipement = ((FiltreNavigation)typeof(FiltreNavigation).FindByStringValue(monEQ.XPathSelectElement("TypeEquipement").Value)).GetStringValue();
                        monImport.TypeEqAssocie = monImport.TypeEquipement != null ? serviceTypeEquipement.Entities.Where(c => c.CodeEquipement == monImport.TypeEquipement).FirstOrDefault() : null;
                        monImport.LibelleEq = monEQ.XPathSelectElement("Libelle").Value;
                        monImport.DateVisite = DateTime.Parse(monImport.MaVisiteProtein.XPathSelectElement("DateVisite").Value, new CultureInfo("fr-FR"));
                        monImport.DateImport = DateTime.Now;
                        monImport.DateSaisie = monImport.DateVisite;
                        monImport.IsTelemesure = false;
                        monImport.EstValide = false;
                        monImport.CommentaireVisite = monImport.MaVisiteProtein.XPathSelectElement("CommentaireVisite").IsEmpty ? String.Empty : monImport.MaVisiteProtein.XPathSelectElement("CommentaireVisite").Value;
                        monImport.Utilisateur = UserImport;

                        // Si aucune clé d'équipement alors il faut créer un équipement temporaire
                        monImport.IsEquipementTempo = monImport.CleEquipement == 0;

                        if (monImport.IsEquipementTempo)
                        {
                            monImport.MaPP = monEQ;
                        }
                    }
                    catch (Exception ex)
                    {
                        monImport.AddOnError(String.Format("Une erreur est survenue : {0}", ex.Message));
                    }

                    // Erreur si le type d'équipement est null
                    if (!monImport.IsOnError && monImport.TypeEquipement == null)
                    {
                        monImport.AddOnError("Le type d'équipement ne correspond à aucun type équipement connu");
                    }
                }

                // Pour chaque PPs récupérées, on traite la visite
                foreach (XElement maPP in mesPPs)
                {
                    VisiteImport monImport = new VisiteImport() { };
                    VisitesImport.Add(monImport);

                    try
                    {
                        // Champs VISITEIMPORT
                        monImport.NomFichier = FicProteinLoaded;
                        monImport.IndexOnFile = index++;

                        // SPECIFIQUE PROTE'IN
                        monImport.MaVisiteProtein = maPP.XPathSelectElement("Visites/VisiteProteIN");
                        monImport.MesVisitesMesures = monImport.MaVisiteProtein.XPathSelectElements("MesMesures/MesMesure");
                        monImport.MaPP = maPP;

                        // Ajout des clés de type de mesures
                        foreach (XElement MaCleTypeMesure in monImport.MaVisiteProtein.XPathSelectElements(queryEQsCleTypeMsr))
                        {
                            ListCleTypeMesure.Add((int)MaCleTypeMesure);
                            monImport.ListCleTypeMesure.Add((int)MaCleTypeMesure);
                        }

                        // Ajout des clés des intruments
                        foreach (XElement MaCleInstrument in XmlDocProtein.XPathSelectElements(queryInstrumentCle))
                        {
                            monImport.ListCleInstrument.Add(MaCleInstrument.IsEmpty || String.IsNullOrEmpty(MaCleInstrument.Value) ? 0 : int.Parse(MaCleInstrument.Value));
                        }

                        // Champs VISITES
                        monImport.CleEquipement = int.Parse(maPP.XPathSelectElement("ClePP").Value);
                        monImport.TypeEquipement = FiltreNavigation.PP.GetStringValue();
                        monImport.EnumTypeEval = int.Parse(monImport.MaVisiteProtein.XPathSelectElement("EnumTypeEval").Value);
                        monImport.EnumTypeEvalComposition = int.Parse(maPP.XPathSelectElement("CleTypeEvaluation").Value);
                        monImport.IsPpGPSFiabilisee = bool.Parse(maPP.XPathSelectElement("CoordonneesGPSFiabilisee").Value);
                        bool? isPpGPSFiabiliseeUser = (maPP.XPathSelectElement("CoordonneesGPSFiabiliseeUser") != null && !maPP.XPathSelectElement("CoordonneesGPSFiabiliseeUser").IsEmpty) ? bool.Parse(maPP.XPathSelectElement("CoordonneesGPSFiabiliseeUser").Value) : (bool?)null;
                        if (isPpGPSFiabiliseeUser.HasValue)
                        {
                            monImport.DemandeDeverrouillage = !isPpGPSFiabiliseeUser.Value && monImport.IsPpGPSFiabilisee;
                            monImport.DemandeFiabilisation = isPpGPSFiabiliseeUser.Value && !monImport.IsPpGPSFiabilisee;
                        }

                        monImport.IsPPModifed = maPP.Elements().Any(e => e.HasAttributes) || monImport.DemandeFiabilisation;
                        monImport.HasAnalyse = !monImport.MaVisiteProtein.XPathSelectElement("CleAnalyse").IsEmpty && !String.IsNullOrEmpty(monImport.MaVisiteProtein.XPathSelectElement("CleAnalyse").Value);
                        monImport.DateVisite = DateTime.Parse(monImport.MaVisiteProtein.XPathSelectElement("DateVisite").Value, new CultureInfo("fr-FR"));
                        monImport.LibelleEq = maPP.XPathSelectElement("Libelle").Value;
                        monImport.DateImport = DateTime.Now;
                        monImport.DateSaisie = monImport.DateVisite;
                        monImport.IsTelemesure = false;
                        monImport.EstValide = false;
                        monImport.CommentaireVisite = monImport.MaVisiteProtein.XPathSelectElement("CommentaireVisite").IsEmpty ? String.Empty : monImport.MaVisiteProtein.XPathSelectElement("CommentaireVisite").Value;
                        monImport.Utilisateur = UserImport;

                        monImport.IsEquipementTempo = monImport.IsPPModifed;
                    }
                    catch (Exception ex)
                    {
                        monImport.AddOnError(String.Format("Une erreur est survenue : {0}", ex.Message));
                    }
                }
            }
            else if (IsFicProtOnSelected)
            {
                int index = 0;

                foreach (Visite maVisite in ImportedEntities)
                {
                    VisiteImport monImport = new VisiteImport();
                    VisitesImport.Add(monImport);

                    // Champs VISITEIMPORT
                    monImport.NomFichier = FicProtOnLoaded;
                    monImport.IndexOnFile = index++;

                    // SPECIFIQUE PROTE'IN
                    if (!maVisite.CleEquipement.HasValue)
                    {
                        monImport.MaPP = maVisite.Ouvrage.CreateXElement();
                    }
                    monImport.MaVisiteProtein = maVisite.CreateXVisite();
                    monImport.MesVisitesMesures = monImport.MaVisiteProtein.XPathSelectElements("MesMesures/MesMesure");

                    // Champs VISITES
                    monImport.CleEquipement = (int)maVisite.Ouvrage.GetIdentity();
                    monImport.ClePpAssocie = maVisite.Ouvrage.PpAttachee.ClePp;
                    monImport.EnumTypeEval = maVisite.EnumTypeEval;
                    monImport.EnumTypeEvalComposition = maVisite.EnumTypeEvalComposition;
                    monImport.IsPpGPSFiabilisee = maVisite.ClePp.HasValue && maVisite.Pp.CoordonneeGpsFiabilisee;
                    monImport.IsPPModifed = maVisite.ClePp.HasValue && maVisite.Pp.HasChanges;
                    monImport.DemandeDeverrouillage = maVisite.ClePp.HasValue && maVisite.Pp.DdeDeverrouillageCoordGps;
                    monImport.HasAnalyse = maVisite.Analyse != null;
                    monImport.TypeEqAssocie = this.serviceTypeEquipement.Entities.FirstOrDefault(t => t.CodeEquipement == maVisite.Ouvrage.CodeEquipement);
                    monImport.TypeEquipement = maVisite.Ouvrage.CodeEquipement;
                    monImport.LibelleEq = maVisite.Ouvrage.Libelle;
                    monImport.DateVisite = maVisite.DateVisite.Value;
                    monImport.DateImport = DateTime.Now;
                    monImport.DateSaisie = maVisite.DateSaisie.Value;
                    monImport.IsTelemesure = false;
                    monImport.IsAlerteUtilisateur = maVisite.Alerte != null;
                    monImport.EstValide = false;
                    monImport.CommentaireVisite = maVisite.Commentaire;
                    monImport.CleUtilisateur = maVisite.CleUtilisateurMesure ?? 0;

                    // Correction mantis 19928 : prise en compte case à cocher RelevePartiel
                    monImport.RelevePartiel = maVisite.RelevePartiel;

                    // Si ajout d'utilisateur
                    if (monImport.CleUtilisateur == 0)
                    {
                        UsrUtilisateur newUser = ListNewUser.FirstOrDefault(u => u.Prenom == maVisite.UsrUtilisateur2.Prenom && u.Nom == maVisite.UsrUtilisateur2.Nom && u.Societe == maVisite.UsrUtilisateur2.Societe);
                        if (newUser == null)
                        {
                            newUser = new UsrUtilisateur()
                                {
                                    Prenom = maVisite.UsrUtilisateur2.Prenom,
                                    Nom = maVisite.UsrUtilisateur2.Nom,
                                    GestionDesComptes = maVisite.UsrUtilisateur2.GestionDesComptes,
                                    Externe = true,
                                    Societe = maVisite.UsrUtilisateur2.Societe,
                                    Mail = maVisite.UsrUtilisateur2.Mail,
                                    Identifiant = maVisite.UsrUtilisateur2.Identifiant
                                };

                            ListNewUser.Add(newUser);
                        }


                        monImport.Utilisateur = newUser;
                    }



                    // Si aucune clé d'équipement alors il faut créer un équipement temporaire
                    monImport.IsEquipementTempo = maVisite.CleEqTmp.HasValue;

                    // Ajout des clés des intruments
                    foreach (InstrumentsUtilises iu in maVisite.InstrumentsUtilises)
                    {
                        if (iu.InsInstrument.IsNew)
                        {
                            monImport.ListNewInstrument.Add(iu.InsInstrument.Libelle);
                        }
                        else
                        {
                            monImport.ListCleInstrument.Add(iu.CleInstrument);
                        }
                    }

                    // Ajout des clés de type de mesures
                    foreach (MesMesure mes in maVisite.MesMesure)
                    {
                        if (!ListCleTypeMesure.Contains(mes.CleTypeMesure))
                        {
                            ListCleTypeMesure.Add(mes.CleTypeMesure);
                        }
                    }
                }
            }


            foreach (VisiteImport vi in this.VisitesImport)
            {
                vi.VisiteRapport.NumLigne = vi.IndexOnFile;
                vi.VisiteRapport.NomFichier = vi.NomFichier;
                vi.VisiteRapport.TypeEquipement = vi.TypeEquipement;
                vi.VisiteRapport.CleEquipement = vi.CleEquipement;
                vi.VisiteRapport.LibelleEq = vi.LibelleEq;
                vi.VisiteRapport.DateVisite = vi.DateVisite;
                vi.VisiteRapport.TextImport = vi.TextImport;
            }



            //REFACTOR----------------

            // Chargement des listes 
            IEnumerable<Pp> _Pps = null;
            IEnumerable<EqEquipement> _Equips = null;
            IEnumerable<MesTypeMesure> _typesMesures = null;
            IEnumerable<InsInstrument> _Instruments = null;


            ((MesTypeMesureService)serviceMesTypeMesure).GetListMesTypeMesureOnly(ListCleTypeMesure, (errortypemesure, TypesMesures) =>
            {
                if (errortypemesure != null)
                {
                    Logger.Log(LogSeverity.Error, this.GetType().FullName, errortypemesure.ToString());
                    IsBusy = false;
                }
                else
                {
                    this.ListInstrument = new List<InsInstrument>();
                    _typesMesures = TypesMesures;

                    if (this.IsFicProteinSelected)
                    {
                        var queryInstruments = "/ExportTournee/Tournee/Instruments/Instrument";
                        IEnumerable<XElement> mesNewIns = XmlDocProtein.XPathSelectElements(queryInstruments).Where(x => !x.XPathSelectElement("Libelle").IsEmpty
                                                                                                                        && !String.IsNullOrEmpty(x.XPathSelectElement("Libelle").Value)
                                                                                                                        && (x.XPathSelectElement("CleInstrument").IsEmpty || String.IsNullOrEmpty(x.XPathSelectElement("CleInstrument").Value)));
                        // Ajout des nouveaux instruments dans le domain context
                        foreach (XElement monNewIns in mesNewIns)
                        {
                            string libelle = monNewIns.XPathSelectElement("Libelle").Value;

                            if (libelle.Length > _insLibelleSize)
                            {
                                // On peut rajouter ici un Warning mais sur tout les éléments VisiteImport...
                                libelle = libelle.Substring(0, _insLibelleSize);
                            }


                            InsInstrument newIns = new InsInstrument()
                            {
                                Libelle = libelle,
                                CleRegion = 0
                            };
                            this.ListInstrument.Add(newIns);
                            this.serviceInstrument.Add(newIns);
                        }
                    }
                    else if (IsFicProtOnSelected)
                    {
                        foreach (InsInstrument ins in (this.service as VisiteService).ImportedInsInstruments.Where(i => i.IsNew))
                        {
                            InsInstrument newIns = new InsInstrument()
                            {
                                Libelle = ins.Libelle,
                                CleRegion = ins.CleRegion
                            };

                            this.ListInstrument.Add(newIns);
                            this.serviceInstrument.Add(newIns);
                        }
                    }

                    List<int> TotalListCleInstruments = new List<int>();

                    foreach (VisiteImport vi in VisitesImport)
                    {
                        foreach (int cle in vi.ListCleInstrument)
                        {
                            if (!TotalListCleInstruments.Any(tcle => tcle == cle))
                            {
                                TotalListCleInstruments.Add(cle);
                            }

                        }

                    }


                    ((InsInstrumentService)serviceInstrument).GetListInstrumentOnly(TotalListCleInstruments, (errorinstruments, Instruments) =>
                    {

                        if (errorinstruments != null)
                        {
                            Logger.Log(LogSeverity.Error, this.GetType().FullName, errorinstruments.ToString());
                            IsBusy = false;
                        }
                        else
                        {
                            _Instruments = Instruments;
                            foreach (InsInstrument oldIns in Instruments)
                            {
                                ListInstrument.Add(oldIns);
                            }

                            // Rapatriement de l'ensemble des équipements liés
                            List<int> listdecleEq = VisitesImport.Where(d => d.IsOnError == false && d.TypeEquipement != FiltreNavigation.PP.GetStringValue() && !d.IsEquipementTempo).Select(c => c.CleEquipement).ToList();
                            List<int> listdeclePP = ((VisitesImport.Where(d => d.IsOnError == false && d.TypeEquipement != FiltreNavigation.PP.GetStringValue()).Select(c => c.ClePpAssocie))
                                .Union(VisitesImport.Where(d => d.IsOnError == false && d.TypeEquipement == FiltreNavigation.PP.GetStringValue()).Select(c => c.CleEquipement))).Distinct().ToList();

                            // Récupération des PPs
                            ((PpService)servicePP).GetListPPOnly(listdeclePP, (error, PPs) =>
                            {
                                _Pps = PPs;
                                if (error != null)
                                {
                                    Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                                    IsBusy = false;
                                }

                                else
                                {
                                    if (IsFicProteinSelected)
                                    {
                                        foreach (InsInstrument ins in this.ListInstrument)
                                        {
                                            if (ins.CleRegion.HasValue && ins.CleRegion.Value == 0)
                                            {
                                                ins.CleRegion = CleRegionInstrument;
                                            }
                                        }
                                    }

                                    // Récupération des Equipements
                                    ((EqEquipementService)serviceEqEquipement).GetListEqEquipementOnly(listdecleEq, (errorEq, Equips) =>
                                    {
                                        if (errorEq != null)
                                        {
                                            Logger.Log(LogSeverity.Error, this.GetType().FullName, errorEq.ToString());
                                            IsBusy = false;
                                        }
                                        else
                                        {
                                            _Equips = Equips;
                                            /////SUITE ...... 
                                            ControlerList(_Pps, _Equips, _typesMesures, () =>
                                            {

                                                indexImport = 0;

                                                AjouterLigne(_Pps, _Equips, () =>
                                                {
                                                    this.VisitesImportRapportFinal = new ObservableCollection<VisiteImportRapport>(VisitesImport.OrderByDescending(c => c.IsOnError).ThenByDescending(c => c.IsOnWarning).ThenByDescending(e => e.IsOnSuccess).Select(vi => vi.VisiteRapport).ToList());
                                                    IsBusy = false;
                                                });

                                            });
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });

            /// ChargerList(ListCleTypeMesure, (erreurs, mesPPs, mesEquips, typesdemesures) =>


        }




        private void ImportTelemesuresDone(Exception error, ObservableCollection<VisiteImportRapport> result)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(VisiteImportRapport).Name));
            }
            else
            {
                this.VisitesImportRapportFinal = result;
            }
            this.IsBusy = false;
        }


        /// <summary>
        /// Contrôle la correspondance des clés d'équipement, la correspondance des instruments et des types de mesures
        /// </summary>
        /// <param name="PPs"></param>
        /// <param name="Equips"></param>
        /// <param name="LesTypesMesures"></param>
        /// <param name="completed"></param>
        private void ControlerList(IEnumerable<Pp> PPs, IEnumerable<EqEquipement> Equips, IEnumerable<MesTypeMesure> LesTypesMesures, Action completed)
        {
            if (indexImport < VisitesImport.Count())
            {
                // Récupération de la visite
                VisiteImport monImport = VisitesImport.ElementAt(indexImport);

                List<int> cleInsNonEx = new List<int>();
                // Contrôle des INSTRUMENTS
                foreach (int cleins in monImport.ListCleInstrument.Where(i => i != 0))
                {
                    if (ListInstrument.Where(c => c.CleInstrument == cleins).FirstOrDefault() == null)
                    {
                        cleInsNonEx.Add(cleins);
                    }
                }
                if (cleInsNonEx.Any())
                {
                    monImport.AddOnWarning(string.Format("Un (ou plusieurs) instrument(s) spécifié(s) dans le fichier n'existe(nt) pas dans la base (Clé(s) : {0})", string.Join(", ", cleInsNonEx.ToArray())));
                }

                // Contrôle des TYPES DE MESURE
                foreach (int cletypeMsr in monImport.ListCleTypeMesure)
                {
                    if (LesTypesMesures.Where(t => t.CleTypeMesure == cletypeMsr).FirstOrDefault() == null)
                    {
                        monImport.AddOnWarning("Certaines mesures ne peuvent être créées, un (ou plusieurs) type(s) de mesure spécifié(s) dans le fichier n'existe(nt) pas dans la base");
                    }
                }

                // Contrôle des EQUIPEMENTS et PPS
                if (!monImport.IsEquipementTempo)
                {
                    IOuvrage moneq = null;

                    // Récupération de l'entité IOuvrage correspondant à la clé
                    if (monImport.TypeEquipement == FiltreNavigation.PP.GetStringValue())
                    {
                        moneq = PPs.Where(pp => pp.ClePp == monImport.CleEquipement).FirstOrDefault();
                    }
                    else
                    {
                        moneq = Equips.Where(eq => eq.CleEquipement == monImport.CleEquipement).FirstOrDefault();
                    }

                    if (moneq == null)
                    {
                        monImport.AddOnError("La clé équipement ne correspond à aucun équipement connu");
                    }
                    else
                    {
                        // Vérification de la concordance des type d'équipement
                        if (moneq is EqEquipement && ((EqEquipement)moneq).TypeEquipement.CodeEquipement != monImport.TypeEquipement)
                        {
                            monImport.AddOnError("la clé d'équipement et le type ne correspondent pas");
                        }
                    }
                }
                else if (!monImport.IsPPModifed && !PPs.Any(pp => pp.ClePp == monImport.ClePpAssocie))
                {
                    monImport.AddOnError("La clé pp ne correspond à aucun équipement connu");
                }
                else if (monImport.IsPPModifed && !PPs.Any(pp => pp.ClePp == monImport.CleEquipement))
                {
                    monImport.AddOnError("La clé pp ne correspond à aucun équipement connu");
                }

                indexImport++;
                ControlerList(PPs, Equips, LesTypesMesures, completed);
            }
            else
            {
                completed();
            }
        }

        /// <summary>
        /// Ajoute la ligne en mode asynchrone
        /// </summary>
        /// <param name="PPs"></param>
        /// <param name="Equips"></param>
        /// <param name="completed"></param>
        private void AjouterLigne(IEnumerable<Pp> PPs, IEnumerable<EqEquipement> Equips, Action completed)
        {
            if (indexImport < VisitesImport.Count())
            {
                // Récupération de la visite
                VisiteImport monImport = VisitesImport.ElementAt(indexImport);

                if (!monImport.IsOnError)
                {
                    // Création de la visite
                    Visite nouvelleVisite = new Visite();

                    // Remplissage de la visite avec les propriétés disponibles dans le VisiteImport
                    if (TryCreateVisite(monImport, nouvelleVisite))
                    {
                        // Création des équipements temporaires si besoin
                        // et traitement si besoin de la demande de déverrouillage
                        // et traitement des mesures
                        // et ajout des instruments de mesure
                        // et création d'une analyse si celle-ci a été créée
                        if (TryCreateTmp(monImport, nouvelleVisite, PPs)
                            && TryDdeDeverrouillage(monImport, nouvelleVisite)
                            && TryCreateMesures(monImport, nouvelleVisite)
                            && TryAddInstruments(monImport, nouvelleVisite)
                            && TryCreateAnalyse(monImport, nouvelleVisite))
                        {
                            // Création des alertes
                            if (TryCreateAlertes(monImport, nouvelleVisite))
                            {
                                //Gestion de l'affectation de l'enum de différence par rapport à la tournée
                                SetConformite(nouvelleVisite);

                                // On ajoute la visite avec ses mesures dans le contexte
                                this.service.Add(nouvelleVisite);
                                this.CreateVisitesForPpJumelees(nouvelleVisite, true);

                                this.service.SaveChanges((errorsave) =>
                                {
                                    if (errorsave != null)
                                    {
                                        monImport.AddOnError("Erreur lors de l'enregistrement, réessayez ultérieurement ou contactez votre administrateur");
                                        this.DeleteVisitesForPpJumelee(true);
                                        this.DeleteVisite(nouvelleVisite);
                                    }
                                    else
                                    {
                                        monImport.AddOnSucess();
                                        this.DeleteVisitesForPpJumelee();
                                    }
                                    indexImport++;
                                    AjouterLigne(PPs, Equips, completed);
                                });
                            }
                        }
                    }
                }
                else
                {
                    indexImport++;
                    AjouterLigne(PPs, Equips, completed);
                }
            }
            else
            {
                completed();
            }
        }

        /// <summary>
        /// Initialise le EnumConformite de la visite si celui-ci doit l'être
        /// </summary>
        /// <param name="nouvelleVisite"></param>
        private void SetConformite(Visite nouvelleVisite)
        {
            if (!IsFicTelemesureSelected && !nouvelleVisite.CleEqTmp.HasValue)
            {
                if (this.TourneeLoaded != null
                    && this.TourneeLoaded.Compositions != null)
                {
                    Composition compoRef = null;
                    if (nouvelleVisite.ClePp.HasValue || nouvelleVisite.ClePpTmp.HasValue)
                    {
                        compoRef = TourneeLoaded.Compositions.FirstOrDefault(c => c.ClePp.HasValue && nouvelleVisite.ClePp == c.ClePp.Value);
                    }
                    else if (nouvelleVisite.CleEquipement.HasValue)
                    {
                        compoRef = TourneeLoaded.Compositions.FirstOrDefault(c => c.CleEquipement.HasValue && c.CleEquipement.Value == nouvelleVisite.CleEquipement.Value);
                    }
                    //Si la tournee ne contient pas la composition relative à cet équipement ou que le type éval est différent
                    if (compoRef == null || compoRef.EnumTypeEval != nouvelleVisite.EnumTypeEvalComposition)
                    {
                        nouvelleVisite.EnumConformiteTournee = 1;
                    }
                }
                else
                {
                    //Si la tournée n'existe pas/plus
                    nouvelleVisite.EnumConformiteTournee = 1;
                }
            }
        }

        /// <summary>
        /// Créé la PpTmp ou l'EquipementTmp si besoin
        /// </summary>
        /// <param name="monImport"></param>
        /// <param name="maVisite"></param>
        /// <returns></returns>
        private bool TryCreateTmp(VisiteImport monImport, Visite maVisite, IEnumerable<Pp> PPs)
        {
            try
            {
                // si l'équipement est temporaire, on le créé
                if (monImport.IsEquipementTempo && !monImport.IsPPModifed && monImport.TypeEquipement != FiltreNavigation.PP.GetStringValue())
                {
                    if (monImport.LibelleEq.Length > _eqLibelleSize)
                    {
                        monImport.AddOnWarning("Libellé de l'équipement trop long, ajustement à la taille maximum");
                    }

                    // Création de l'équipement temporaire
                    maVisite.EqEquipementTmp = new EqEquipementTmp()
                    {
                        ClePp = monImport.ClePpAssocie,
                        TypeEquipement = monImport.TypeEqAssocie,
                        EstValide = false,
                        Libelle = (monImport.LibelleEq.Length > _eqLibelleSize) ? monImport.LibelleEq.Substring(0, _eqLibelleSize) : monImport.LibelleEq
                    };
                }

                // sinon si l'équipement est une Pp modifiée on créé une PpTmp
                else if (monImport.IsPPModifed && monImport.TypeEquipement == FiltreNavigation.PP.GetStringValue())
                {
                    // Création de la Pp temporaire
                    maVisite.PpTmp = new PpTmp()
                    {
                        //
                        Pp = PPs.FirstOrDefault(p => p.ClePp == monImport.CleEquipement),
                        DateMajPp = monImport.DateSaisie,
                        UsrUtilisateur = monImport.Utilisateur
                    };

                    // Catégorie Pp
                    maVisite.PpTmp.CleNiveauSensibilite = int.Parse(monImport.MaPP.XPathSelectElement("CleNiveauSensibilite").Value);
                    maVisite.PpTmp.CleCategoriePp = monImport.MaPP.XPathSelectElement("CleCategoriePP").IsEmpty || String.IsNullOrEmpty(monImport.MaPP.XPathSelectElement("CleCategoriePP").Value) ? (int?)null : int.Parse(monImport.MaPP.XPathSelectElement("CleCategoriePP").Value);

                    // Enums
                    maVisite.PpTmp.EnumSurfaceTme = monImport.MaPP.XPathSelectElement("EnumSurfaceTme").IsEmpty || String.IsNullOrEmpty(monImport.MaPP.XPathSelectElement("EnumSurfaceTme").Value) ? (int?)null : int.Parse(monImport.MaPP.XPathSelectElement("EnumSurfaceTme").Value);
                    maVisite.PpTmp.EnumSurfaceTms = monImport.MaPP.XPathSelectElement("EnumSurfaceTms").IsEmpty || String.IsNullOrEmpty(monImport.MaPP.XPathSelectElement("EnumSurfaceTms").Value) ? (int?)null : int.Parse(monImport.MaPP.XPathSelectElement("EnumSurfaceTms").Value);
                    maVisite.PpTmp.EnumDureeEnrg = monImport.MaPP.XPathSelectElement("EnumDureeEnrg").IsEmpty || String.IsNullOrEmpty(monImport.MaPP.XPathSelectElement("EnumDureeEnrg").Value) ? (int?)null : int.Parse(monImport.MaPP.XPathSelectElement("EnumDureeEnrg").Value);
                    maVisite.PpTmp.EnumPolarisation = monImport.MaPP.XPathSelectElement("EnumPolarisation").IsEmpty || String.IsNullOrEmpty(monImport.MaPP.XPathSelectElement("EnumPolarisation").Value) ? (int?)null : int.Parse(monImport.MaPP.XPathSelectElement("EnumPolarisation").Value);

                    // Booleens de caractéristiques
                    maVisite.PpTmp.CourantsVagabonds = bool.Parse(monImport.MaPP.XPathSelectElement("CourantsVagabonds").Value);
                    maVisite.PpTmp.CourantsAlternatifsInduits = bool.Parse(monImport.MaPP.XPathSelectElement("CourantsAlternatifsInduits").Value);
                    maVisite.PpTmp.ElectrodeEnterreeAmovible = bool.Parse(monImport.MaPP.XPathSelectElement("ElectrodeEnterreAmovible").Value);
                    maVisite.PpTmp.TemoinEnterreAmovible = bool.Parse(monImport.MaPP.XPathSelectElement("TemoinEnterreAmovible").Value);
                    maVisite.PpTmp.TemoinMetalliqueDeSurface = bool.Parse(monImport.MaPP.XPathSelectElement("TemoinMetalliqueDeSurface").Value);
                    maVisite.PpTmp.PresenceDUneTelemesure = bool.Parse(monImport.MaPP.XPathSelectElement("Telemesure").Value);

                    // Position GPS
                    maVisite.PpTmp.PositionGpsLat = monImport.MaPP.XPathSelectElement("PositionGPSLat").IsEmpty || String.IsNullOrEmpty(monImport.MaPP.XPathSelectElement("PositionGPSLat").Value) ? (decimal?)null : decimal.Parse((monImport.MaPP.XPathSelectElement("PositionGPSLat").Value).Replace(",", "."), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, new CultureInfo("en-US"));
                    maVisite.PpTmp.PositionGpsLong = monImport.MaPP.XPathSelectElement("PositionGPSLong").IsEmpty || String.IsNullOrEmpty(monImport.MaPP.XPathSelectElement("PositionGPSLong").Value) ? (decimal?)null : decimal.Parse((monImport.MaPP.XPathSelectElement("PositionGPSLong").Value).Replace(",", "."), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, new CultureInfo("en-US"));
                    maVisite.PpTmp.CoordonneeGpsFiabilisee = maVisite.PpTmp.Pp.CoordonneeGpsFiabilisee || monImport.IsPpGPSFiabilisee || monImport.DemandeFiabilisation;

                }

                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de la création de l'équipement à valider : {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Gère si besoin les demandes de déverrouillages des coordonnées faites sur la Pp
        /// </summary>
        /// <param name="monImport"></param>
        /// <param name="maVisite"></param>
        /// <returns></returns>
        private bool TryDdeDeverrouillage(VisiteImport monImport, Visite maVisite)
        {
            try
            {
                if (monImport.DemandeDeverrouillage)
                {
                    if (maVisite.ClePp.HasValue && maVisite.Pp != null && maVisite.Pp.CoordonneeGpsFiabilisee)
                    {
                        maVisite.Pp.DateDdeDeverrouillageCoordGps = monImport.DateSaisie;
                        maVisite.Pp.DdeDeverrouillageCoordGps = true;
                        maVisite.Pp.UsrUtilisateur1 = monImport.Utilisateur;

                    }
                    else if (maVisite.ClePpTmp.HasValue && maVisite.PpTmp != null && maVisite.PpTmp.Pp != null && maVisite.PpTmp.Pp.CoordonneeGpsFiabilisee)
                    {
                        maVisite.PpTmp.Pp.DateDdeDeverrouillageCoordGps = monImport.DateSaisie;
                        maVisite.PpTmp.Pp.DdeDeverrouillageCoordGps = true;
                        maVisite.PpTmp.Pp.UsrUtilisateur1 = monImport.Utilisateur;
                    }

                    if (maVisite.ClePpTmp.HasValue && maVisite.PpTmp != null && maVisite.PpTmp.Pp != null && maVisite.PpTmp.Pp.UsrUtilisateur1 == null)
                    {
                        maVisite.PpTmp.Pp.CleUtiDdeDeverrouillage = monImport.CleUtilisateur;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de la prise en compte de la demande de déverrouillage des coordonnées GPS : {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Création la visite de l'import
        /// </summary>
        /// <param name="monImport"></param>
        /// <param name="maVisite"></param>
        /// <returns></returns>
        private bool TryCreateVisite(VisiteImport monImport, Visite maVisite)
        {
            try
            {
                // champs obligatoire
                if (monImport.Utilisateur != null)
                {
                    maVisite.UsrUtilisateur2 = monImport.Utilisateur;
                }
                else
                {
                    maVisite.CleUtilisateurMesure = monImport.CleUtilisateur;
                }

                maVisite.EstValidee = monImport.EstValide;
                maVisite.Telemesure = monImport.IsTelemesure;

                // Correction mantis 19928
                maVisite.RelevePartiel = monImport.RelevePartiel;

                // Dates
                maVisite.DateVisite = monImport.DateVisite;
                maVisite.DateImport = monImport.DateImport;
                maVisite.DateSaisie = monImport.DateSaisie;

                // Spec import
                maVisite.CleUtilisateurImport = CurrentUser.CleUtilisateur;
                maVisite.CleUtilisateurCreation = CurrentUser.CleUtilisateur;
                maVisite.Commentaire = monImport.CommentaireVisite;
                maVisite.ClePp = monImport.TypeEquipement == FiltreNavigation.PP.GetStringValue() && !monImport.IsPPModifed ? monImport.CleEquipement : (int?)null;
                maVisite.CleEquipement = !monImport.IsEquipementTempo && monImport.TypeEquipement != FiltreNavigation.PP.GetStringValue() ? monImport.CleEquipement : (int?)null;
                maVisite.EnumTypeEval = monImport.EnumTypeEval;
                maVisite.EnumTypeEvalComposition = monImport.EnumTypeEvalComposition;

                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de l'ajout de la visite : {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Ajoute les instruments sur la visite
        /// </summary>
        /// <param name="monImport"></param>
        /// <param name="maVisite"></param>
        /// <returns></returns>
        private bool TryAddInstruments(VisiteImport monImport, Visite maVisite)
        {
            try
            {
                if (IsFicProteinSelected)
                {
                    monImport.ListInstrument = new ObservableCollection<InsInstrument>(this.ListInstrument);
                }
                else if (IsFicProtOnSelected)
                {
                    monImport.ListInstrument = new ObservableCollection<InsInstrument>(this.ListInstrument.Where(i => (monImport.ListCleInstrument != null && monImport.ListCleInstrument.Contains(i.CleInstrument))
                                                                                                                    || (monImport.ListNewInstrument != null && monImport.ListNewInstrument.Contains(i.Libelle))));
                }

                foreach (InsInstrument item in monImport.ListInstrument)
                {
                    if (this.ListInstrument.Contains(item))
                    {
                        maVisite.InstrumentsUtilises.Add(new InstrumentsUtilises()
                        {
                            InsInstrument = item
                        });
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de l'ajout des instruments : {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Création des mesures
        /// </summary>
        /// <param name="monImport"></param>
        /// <param name="maVisite"></param>
        /// <returns></returns>
        private bool TryCreateMesures(VisiteImport monImport, Visite maVisite)
        {
            try
            {
                if (IsFicProteinSelected || IsFicProtOnSelected)
                {
                    foreach (XElement Mavisitemesure in monImport.MesVisitesMesures)
                    {
                        maVisite.MesMesure.Add(new MesMesure()
                        {
                            Valeur = Mavisitemesure.XPathSelectElement("Valeur").IsEmpty || String.IsNullOrEmpty(Mavisitemesure.XPathSelectElement("Valeur").Value) ? (decimal?)null : decimal.Parse((Mavisitemesure.XPathSelectElement("Valeur").Value).Replace(",", "."), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, new CultureInfo("en-US")),
                            CleTypeMesure = int.Parse(Mavisitemesure.XPathSelectElement("CleTypeMesure").Value)
                        });
                    }
                }

                // Correction mantis 19928
                // maVisite.RelevePartiel = maVisite.MesMesure.Any(m => !m.Valeur.HasValue);

                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de la création des mesures : {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Création des alertes
        /// </summary>
        /// <param name="monImport"></param>
        /// <param name="maVisite"></param>
        /// <returns></returns>
        private bool TryCreateAlertes(VisiteImport monImport, Visite maVisite)
        {
            try
            {
                // On sélectionne uniquement les mesures qui sont en dépassement de seuil
                foreach (MesMesure mamesure in maVisite.MesMesure.Where(a => a.IsDepassementSeuil))
                {
                    // création de l'alerte de type seuil
                    mamesure.Alertes.Add(new Alerte()
                    {
                        RefEnumValeur = serviceRefEnum.Entities.Where(c => c.CodeGroupe == RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue() && c.Valeur == "S").FirstOrDefault(),
                        Date = monImport.DateVisite,
                        Supprime = false
                    });
                    maVisite.Alertes.Add(mamesure.Alerte);
                }

                // Création Alerte utilisateur dans la visite si relevé partiel et non télémesure
                if (!maVisite.Telemesure && (maVisite.RelevePartiel || monImport.IsAlerteUtilisateur))
                {
                    maVisite.Alertes.Add(new Alerte()
                    {
                        RefEnumValeur = serviceRefEnum.Entities.Where(c => c.CodeGroupe == RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue() && c.Valeur == "U").FirstOrDefault(),
                        Date = monImport.DateVisite,
                        Supprime = false
                    });
                }

                //mantis 0019931: Alerte - Remontée d'une alerte Analyse "Non satisfaisante"
                //la RefEnumValeur n'est pas renseignée alors qu'il y a l'ID qui va bien. 
                //dans les RIA Services il y a des NoTracking, c'est pourquoi le lien n'est pas fait (dans le cas des equipements , pas des PP)
                if (maVisite.Analyse != null && maVisite.Analyse.RefEnumValeur == null && maVisite.Analyse.EnumEtatPc.HasValue)
                    maVisite.Analyse.RefEnumValeur = serviceRefEnum.Entities.Where(c => c.CleEnumValeur == maVisite.Analyse.EnumEtatPc.Value).FirstOrDefault();

                // Création Alerte Analyse si le type d'analyse est sujet à une alerte
                if (!maVisite.Telemesure
                    && maVisite.AnAnalyseSerieMesure.Any()
                    && maVisite.Analyse.RefEnumValeur != null
                    && maVisite.Analyse.RefEnumValeur != serviceRefEnum.Entities.Where(c => c.CodeGroupe == RefEnumValeurCodeGroupeEnum.AN_ETAT_PC.GetStringValue() && c.Valeur == "01").FirstOrDefault())
                {
                    maVisite.Analyse.Alertes.Add(new Alerte()
                    {
                        RefEnumValeur = serviceRefEnum.Entities.Where(c => c.CodeGroupe == RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue() && c.Valeur == "A").FirstOrDefault(),
                        Date = monImport.DateVisite,
                        Supprime = false
                    });

                    maVisite.Alertes.Add(maVisite.Analyse.Alerte);
                }

                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de la création des alertes : {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Création de l'analyse
        /// </summary>
        /// <param name="monImport"></param>
        /// <param name="maVisite"></param>
        /// <returns></returns>
        private bool TryCreateAnalyse(VisiteImport monImport, Visite maVisite)
        {
            try
            {
                // Création Alerte utilisateur dans la visite si relevé partiel
                if (!maVisite.AnAnalyseSerieMesure.Any() && monImport.HasAnalyse)
                {
                    maVisite.AnAnalyseSerieMesure.Add(new AnAnalyseSerieMesure()
                    {
                        DateAnalyse = monImport.DateSaisie,
                        UsrUtilisateur = monImport.Utilisateur,
                        EnumEtatPc = int.Parse(monImport.MaVisiteProtein.XPathSelectElement("CleAnalyse").Value),
                        Commentaire = monImport.MaVisiteProtein.XPathSelectElement("CommentaireAnalyse").IsEmpty ? String.Empty : monImport.MaVisiteProtein.XPathSelectElement("CommentaireAnalyse").Value
                    });
                }
                // TODO : RefEnumValeur NULL ! mantis 19931
                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de la création de l'analyse : {0}", ex.Message));

                return false;
            }
        }

        /// <summary>
        /// Creation des visites, copies de la visite nouvellement créée, sur les Pp jumelées de cette Pp
        /// </summary>
        /// <param name="maVisite"></param>
        private void CreateVisitesForPpJumelees(Visite maVisite, bool andInDomainContext = false)
        {
            if (maVisite.ClePp.HasValue || maVisite.ClePpTmp.HasValue)
            {
                //Récupération des Pp qui sont jumelees à cette Pp
                List<int> MesClesPpJumelees = maVisite.ClePp.HasValue ? maVisite.Pp.PpJumelee.Select(pj => pj.PpClePp).Union(maVisite.Pp.PpJumelee1.Select(pj => pj.ClePp)).ToList()
                    : maVisite.PpTmp.Pp.PpJumelee.Select(pj => pj.PpClePp).Union(maVisite.PpTmp.Pp.PpJumelee1.Select(pj => pj.ClePp)).ToList();

                foreach (int clePpJumelee in MesClesPpJumelees)
                {
                    //Création de ma copie de Visite
                    Visite visiteCopy = new Visite()
                    {
                        ClePp = clePpJumelee,
                        CleUtilisateurValidation = maVisite.CleUtilisateurValidation,
                        CleUtilisateurCreation = maVisite.CleUtilisateurCreation,
                        CleUtilisateurMesure = maVisite.CleUtilisateurMesure,
                        DateValidation = maVisite.DateValidation,
                        DateSaisie = maVisite.DateSaisie,
                        DateVisite = maVisite.DateVisite,
                        RelevePartiel = maVisite.RelevePartiel,
                        EnumTypeEval = maVisite.EnumTypeEval,
                        EnumTypeEvalComposition = maVisite.EnumTypeEvalComposition,
                        Commentaire = maVisite.Commentaire,
                        EstValidee = maVisite.EstValidee,
                        EnumConformiteTournee = maVisite.EnumConformiteTournee
                    };

                    //Copie de l'alertes
                    if (maVisite.Alerte != null)
                    {
                        Alerte alerte = new Alerte()
                        {
                            Supprime = false,
                            Date = maVisite.Alerte.Date,
                            EnumTypeAlerte = maVisite.Alerte.EnumTypeAlerte
                        };

                        visiteCopy.Alertes.Add(alerte);
                    }

                    //Copie des InstrumentsUtilises
                    foreach (InstrumentsUtilises instrumentUtiliseOrigin in maVisite.InstrumentsUtilises)
                    {
                        visiteCopy.InstrumentsUtilises.Add(new InstrumentsUtilises()
                        {
                            CleInstrument = instrumentUtiliseOrigin.CleInstrument
                        });
                    }

                    //Copie des Mesures
                    foreach (MesMesure mesureOrigin in maVisite.MesMesure)
                    {
                        //Création de ma copie de Mesure
                        MesMesure mesureCopy = new MesMesure()
                        {
                            Valeur = mesureOrigin.Valeur,
                            CleTypeMesure = mesureOrigin.CleTypeMesure
                        };

                        if (mesureOrigin.Alerte != null)
                        {
                            Alerte alerte = new Alerte()
                            {
                                Supprime = false,
                                Date = mesureOrigin.Alerte.Date,
                                EnumTypeAlerte = mesureOrigin.Alerte.EnumTypeAlerte
                            };

                            visiteCopy.Alertes.Add(alerte);
                            mesureCopy.Alertes.Add(alerte);
                        }

                        visiteCopy.MesMesure.Add(mesureCopy);
                    }

                    //Copie de l'analyse
                    if (maVisite.AnAnalyseSerieMesure.Any())
                    {
                        AnAnalyseSerieMesure analyseCopy = new AnAnalyseSerieMesure()
                        {
                            CleUtilisateur = maVisite.Analyse.CleUtilisateur,
                            DateAnalyse = maVisite.Analyse.DateAnalyse,
                            EnumEtatPc = maVisite.Analyse.EnumEtatPc,
                            Commentaire = maVisite.Analyse.Commentaire
                        };

                        if (maVisite.Analyse.Alerte != null)
                        {
                            Alerte alerte = new Alerte()
                            {
                                Supprime = false,
                                Date = maVisite.Analyse.Alerte.Date,
                                EnumTypeAlerte = maVisite.Analyse.Alerte.EnumTypeAlerte
                            };

                            visiteCopy.Alertes.Add(alerte);
                            analyseCopy.Alertes.Add(alerte);
                        }

                        visiteCopy.AnAnalyseSerieMesure.Add(analyseCopy);
                    }

                    this.VisitesForPpJumelee.Add(visiteCopy);
                    if (andInDomainContext)
                    {
                        this.service.Add(visiteCopy);
                    }
                }
            }
        }

        /// <summary>
        /// Supression des Visites créées sur les Pp Jumelées à la sauvegarde 
        /// </summary>
        /// <param name="andInDomainContext">Suppression complète des visite du domain context</param>
        private void DeleteVisitesForPpJumelee(bool andInDomainContext = false)
        {
            if (andInDomainContext)
            {
                foreach (Visite visiteCopy in this.VisitesForPpJumelee)
                {
                    DeleteVisite(visiteCopy);
                }
            }
            this.VisitesForPpJumelee.Clear();
        }

        /// <summary>
        /// Suppression d'une visite dans le domain context
        /// </summary>
        /// <param name="visite"></param>
        private void DeleteVisite(Visite visite)
        {
            foreach (Alerte alerte in visite.Alertes)
            {
                this.serviceAlerte.Delete(alerte);
            }

            foreach (MesMesure mesure in visite.MesMesure)
            {
                this.serviceMesMesure.Delete(mesure);
            }

            foreach (AnAnalyseSerieMesure analyse in visite.AnAnalyseSerieMesure)
            {
                this.serviceAnAnalyseSerieMesure.Delete(analyse);
            }

            foreach (InstrumentsUtilises instrumentUtilise in visite.InstrumentsUtilises)
            {
                (this.serviceInstrument as InsInstrumentService).DeleteInstrumentUtilise(instrumentUtilise);
            }

            this.service.Delete(visite);
        }

        /// <summary>
        /// Initialisation de l'écran
        /// </summary>
        private void Initialisation()
        {
            if (CurrentUser != null && CurrentUser.IsAdministrateur)
            {
                this.IsFicProteinSelected = true;
            }
            this.ModuleFileInfo = null;
            this.IsModuleProteinSelected = true;
            this.IsModuleProtOnSelected = true;
            this.IsWorking = false;
            this.IsWorkingModule = false;
            this.FicProteinLoaded = string.Empty;
            this.FicProtOnLoaded = string.Empty;
            this.FicTelemesureLoaded = string.Empty;
            this.ModuleProteinLoaded = string.Empty;
            this.ModuleProtOnLoaded = string.Empty;
            this.RowTable = new List<string[]>();
            this.Lines = new ObservableCollection<string>();
            this.LibelleTourneeProtein = null;
            this.Version = null;
            this.VersionModuleProtein = null;
            this.UserSelected = null;
            this.VisitesImport = null;
            this.VisitesImportRapportFinal = null;
            this.UserInput = null;
            this.TourneeLoaded = null;
            RaisePropertyChanged(() => this.UserSelected);
            RaisePropertyChanged(() => this.UserInput);
            RaisePropertyChanged(() => this.VisitesImportRapportFinal);
            RaisePropertyChanged(() => this.ResultCountCustom);
            ((SharepointService)serviceSharePoint).GetModulesAvailability(this.AvailabilityLoaded);
        }

        /// <summary>
        /// Fonction de mise à jour de l'état des commandes
        /// </summary>
        private void RefreshCommands()
        {
            ChargerFicProteinCommand.RaiseCanExecuteChanged();
            ChargerFicProtOnCommand.RaiseCanExecuteChanged();
            ChargerFicTelemesureCommand.RaiseCanExecuteChanged();
            ChargerModuleCommand.RaiseCanExecuteChanged();
            ChargerModuleProteinCommand.RaiseCanExecuteChanged();
            ChargerModuleProtOnCommand.RaiseCanExecuteChanged();
            ImportCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Lecture de l'entête du fichier Protein avant import
        /// </summary>
        private void PreChargerFicProtein()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Proteca (*.pro)|*.pro";

            bool? userClickedOK = dlg.ShowDialog();
            if (userClickedOK == true)
            {
                IsBusy = true;

                using (ZipFile zip = ZipFile.Read(dlg.File.OpenRead()))
                {
                    zip.Password = "Grt!Pr0t3c@";
                    FicProteinLoaded = dlg.File.Name;

                    using (Stream s = zip["data.xml"].OpenReader())
                    {
                        XmlDocProtein = XDocument.Load(XmlReader.Create(s));

                        IsFicProteinSelected = true;
                        IsFicProteinLoaded = true;

                        int cleTournee;
                        if (int.TryParse(XmlDocProtein.XPathSelectElement("/ExportTournee/Tournee/CleTournee").Value, out cleTournee))
                        {

                            LibelleTourneeProtein = XmlDocProtein.XPathSelectElement("/ExportTournee/Tournee/Libelle").Value;
                            Version = (XmlDocProtein.XPathSelectElement("/ExportTournee").HasAttributes) ? XmlDocProtein.XPathSelectElement("/ExportTournee").FirstAttribute.Value : String.Empty;

                            // Pour une clé de tournée on rapatrie les utilisateurs associés
                            ((TourneeService)serviceTournee).GetEntityByCle(cleTournee, (error) =>
                            {
                                if (error == null)
                                {
                                    this.TourneeLoaded = serviceTournee.DetailEntity;

                                    ((UsrUtilisateurService)serviceUtilisateur).FindUsrUtilisateurByCleTournee(cleTournee, (ex) =>
                                    {
                                        if (ex == null)
                                        {
                                            RaisePropertyChanged(() => this.Agents);

                                            // Si aucune clé utilisateur, il faut créer l'utilisateur indiqué
                                            UserSelected = null;
                                            var queryUsr = "/ExportTournee/Tournee/CleUtilisateur";
                                            var queryUsrPName = "/ExportTournee/Tournee/PrenomAgent";
                                            var queryUsrName = "/ExportTournee/Tournee/NomAgent";
                                            int monuser;

                                            UserInput = new UsrUtilisateur()
                                            {
                                                Supprime = false,
                                                Externe = true,
                                                Mail = String.Empty,
                                                Identifiant = String.Empty
                                            };

                                            if (!XmlDocProtein.XPathSelectElement(queryUsr).IsEmpty && int.TryParse(XmlDocProtein.XPathSelectElement(queryUsr).Value, out monuser))
                                            {
                                                UserSelected = serviceUtilisateur.Entities.Where(c => c.CleUtilisateur == monuser).FirstOrDefault();
                                            }

                                            if (!XmlDocProtein.XPathSelectElement(queryUsrPName).IsEmpty
                                                && !String.IsNullOrEmpty(XmlDocProtein.XPathSelectElement(queryUsrPName).Value)
                                                && !XmlDocProtein.XPathSelectElement(queryUsrName).IsEmpty
                                                && !String.IsNullOrEmpty(XmlDocProtein.XPathSelectElement(queryUsrName).Value))
                                            {
                                                UserInput.Prenom = XmlDocProtein.XPathSelectElement(queryUsrPName).Value;
                                                UserInput.Nom = XmlDocProtein.XPathSelectElement(queryUsrName).Value;
                                            }

                                            RaisePropertyChanged(() => this.UserInput);
                                        }
                                        else
                                        {
                                            ErrorWindow.CreateNew(string.Format("Erreur lors de la recherche des utilisateurs liés à la tournée : {0}", ex.Message));
                                        }
                                    });
                                }
                                else
                                {
                                    ErrorWindow.CreateNew(string.Format("Erreur lors de la recherche de la tournée : {0}", error.Message));
                                }
                            });
                        }
                        else
                        {
                            ErrorWindow.CreateNew("Fichier corrompu impossible de lire les données");
                        }
                    }
                }

                IsBusy = false;
            }
            else
            {
                Initialisation();
            }
        }

        /// <summary>
        /// Lecture de l'entête du fichier ProtOn avant import
        /// </summary>
        private void PreChargerFicProtOn()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "ProtOn (*.pon)|*.pon";

            bool? userClickedOK = dlg.ShowDialog();
            if (userClickedOK == true)
            {
                this.IsBusy = true;
                string data = String.Empty;
                FicProtOnLoaded = null;
                var fileLoaded = dlg.File.Name;
                var fileData = dlg.File.OpenRead();

                var domainContextImport = new Proteca.Web.Services.ProtecaDomainContext(serviceConfigurator);
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    using (ZipFile zip = ZipFile.Read(fileData))
                    {
                        zip.Password = "Grt!Pr0t3c@";

                        using (Stream fileStream = zip[0].OpenReader())
                        {
                            using (StreamReader reader = new StreamReader(fileStream))
                            {

                                LoadProgress.StartNewProcess("Lecture du fichier");
                                string lineOfData = String.Empty;
                                while ((lineOfData = reader.ReadLine()) != null)
                                    data += lineOfData;
                                LoadProgress.IncrementCurrentValue();

                                IsFicProtOnSelected = true;

                                (this.service as VisiteService).LoadDomainContextImport(domainContextImport, data, ProtOnLoaded, LoadProgress);
                                FicProtOnLoaded = fileLoaded;
                            }
                        }
                    }
                });
            }
            else
            {
                Initialisation();
            }
        }



        public class ProgressInfo : BaseViewModel
        {
            private bool _IsLoading;
            public bool IsLoading
            {
                get { return _IsLoading; }
                private set { _IsLoading = value; RaisePropertyChanged("IsLoading"); }
            }
            private bool _IsIndeterminate;

            public bool IsIndeterminate
            {
                get { return _IsIndeterminate; }
                private set { _IsIndeterminate = value; RaisePropertyChanged("IsIndeterminate"); }
            }

            private int _Maximum;
            public int Maximum
            {
                get { return _Maximum; }
                private set { _Maximum = value; RaisePropertyChanged("Maximum"); }
            }

            private int _CurrentValue;
            public int CurrentValue
            {
                get { return _CurrentValue; }
                private set { _CurrentValue = value; RaisePropertyChanged("CurrentValue"); }
            }

            private string _TextInfo;
            public string TextInfo
            {
                get { return _TextInfo; }
                private set { _TextInfo = value; RaisePropertyChanged("TextInfo"); }
            }

            public void StartNewProcess(string textInfo, int maximumProgression)
            {
                TextInfo = textInfo;
                Maximum = maximumProgression;
                CurrentValue = 0;
                IsLoading = true;
            }

            public void StartNewProcess(string textInfo)
            {
                TextInfo = textInfo;
                Maximum = 0;
                CurrentValue = 0;
                IsIndeterminate = true;
                IsLoading = true;
            }
            public void IncrementCurrentValue()
            {
                if (IsIndeterminate)
                {
                    IsIndeterminate = false;
                    IsLoading = false;
                }
                else
                {
                    CurrentValue++;
                    IsLoading = CurrentValue < Maximum;
                }
            }

            protected override void RaisePropertyChanged(string propertyName)
            {
                JounceHelper.ExecuteOnUI(() => base.RaisePropertyChanged(propertyName));
            }
        }
        /// <summary>
        /// Callback du chargement du fichier *.pon
        /// </summary>
        /// <param name="error"></param>
        private void ProtOnLoaded(Exception error)
        {
            if ((this.service as VisiteService).ImportedTournee != null)
            {

                // Pour une clé de tournée on rapatrie les utilisateurs associés
                ((TourneeService)serviceTournee).GetEntityByCle(ImportedTournee.CleTournee, (err) =>
                {
                    if (error == null)
                    {
                        this.TourneeLoaded = serviceTournee.DetailEntity;
                        LibelleTourneeProtOn = (this.service as VisiteService).ImportedTournee.Libelle;
                        IsFicProtOnLoaded = true;
                        this.IsBusy = false;
                    }
                    else
                    {
                        ErrorWindow.CreateNew(string.Format("Erreur lors de la recherche de la tournée : {0}", err.Message));
                    }
                });
            }
            else
            {
                ErrorWindow.CreateNew("Fichier corrompu, impossible de lire les données");
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Callback du chargement du module déporté
        /// </summary>
        /// <param name="error"></param>
        private void ModuleLoaded(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Warning, "Erreur lors du chargement du module déporté", error);
                ErrorWindow.CreateNew("Erreur lors du chargement du module déporté");
            }
            else
            {
                MessageBox.Show("Chargement du module déporté réussi", "", MessageBoxButton.OK);
                Initialisation();
            }
            this.IsBusy = false;
        }

        /// <summary>
        /// Callback du chargement du module déporté
        /// </summary>
        /// <param name="error"></param>
        private void DeleteDone(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Warning, "Erreur lors de la suppression du module déporté", error);
                ErrorWindow.CreateNew("Erreur lors de la suppression du module déporté");
            }
            else
            {
                MessageBox.Show("Suppression du module réussie", "", MessageBoxButton.OK);
                ((SharepointService)serviceSharePoint).GetModulesAvailability(this.AvailabilityLoaded);
            }
            this.IsBusy = false;
        }

        /// <summary>
        /// Callback du chargement du module déporté
        /// </summary>
        /// <param name="error"></param>
        private void AvailabilityLoaded(Exception error, List<string> files)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Warning, "Erreur lors de la récupération de la disponibilité des modules", error);
                ErrorWindow.CreateNew("Erreur lors de la récupération de la disponibilité des modules");
            }
            else if (files != null)
            {
                this.ProteinAvailable = files.Any(f => f == ModulesNavigation.ProteIn.GetStringValue());
                this.ProtOnAvailable = files.Any(f => f == ModulesNavigation.ProtOn.GetStringValue());
            }
            this.IsBusy = false;
        }

        /// <summary>
        /// Lecture de l'entête des fichiers Télémesure avant import
        /// </summary>
        private void PreChargerFicTelemesure()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "Export Files (*.csv)|*.csv";

            bool? userClickedOK = dlg.ShowDialog();
            if (userClickedOK == true)
            {
                IsBusy = true;

                // Traitement multi-fichiers
                foreach (FileInfo monfic in dlg.Files)
                {
                    using (Stream fileStream = monfic.OpenRead())
                    {
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            if (String.IsNullOrEmpty(FicTelemesureLoaded))
                            {
                                FicTelemesureLoaded = monfic.Name;
                            }
                            else
                            {
                                FicTelemesureLoaded += "\n" + monfic.Name;
                            }

                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                var array = new string[41];
                                var currentArray = line.Split(';');
                                if (currentArray.Length < array.Length)
                                    currentArray.CopyTo(array, 0);
                                else
                                    array = currentArray;

                                Lines.Add(string.Join(";", array) + ";" + monfic.Name);
                            }

                            IsFicTelemesureSelected = true;
                            ImportCommand.RaiseCanExecuteChanged();

                        }
                        fileStream.Close();
                    }
                }

                IsBusy = false;
            }
            else
            {
                Initialisation();
            }
        }

        #endregion

        #region Override Methods

        protected override void DeactivateView(string viewName)
        {
            VisitesImport.Clear();
            VisitesForPpJumelee.Clear();
            ListInstrument.Clear();
            ListNewUser.Clear();
            VisitesImportRapportFinal.Clear();

            base.DeactivateView(viewName);

            ((TypeEquipementService)this.serviceTypeEquipement).ForceClear();
        }

        /// <summary>
        /// Réinitialisation de l'écran
        /// </summary>
        protected override void Cancel()
        {
            base.Cancel();

            Initialisation();
        }

        /// <summary>
        /// Activation de la vue de regroiupement de région.
        /// Lors de l'activation on cache la partie customExpander
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);
            EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("HideExpander", true));
        }

        #endregion
    }
}