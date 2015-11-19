using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Xml.Linq;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.Event;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Jounce.Framework.Workflow;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Models;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.SearchQueryService;
using Proteca.Silverlight.Services;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Web.Models;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for Search entity
    /// </summary>
    [ExportAsViewModel("Search")]
    public class SearchViewModel : BaseViewModel, IEventSink<CurrentNavigation>, IEventSink<ViewMode>, IPartImportsSatisfiedNotification
    {
        #region Commandes

        public IActionCommand FindCommand { get; protected set; }

        #endregion

        #region Private Properties

        private string _sharepointSearchLink = "/_vti_bin/search.asmx";

        private Nullable<DateTime> _dateDeb;
        private Nullable<DateTime> _dateFin;

        private Boolean _isEnabled = true;
        private Boolean _isColumnDateVisible;
        private Boolean _isBusy;
        private Boolean _resultTruncated;

        private String _searchText = String.Empty;

        private int _nbResults;
        private bool _filterAll;
        private bool _filterEnsElec;
        private bool _filterPortion;
        private bool _filterPp;
        private bool _filterEquipement;
        private bool _filterDocument;

        private TypeDocument _ouvrage;
        private TypeDocument _dossier;

        #endregion

        #region public Properties

        /// <summary>
        /// Ouvrage sélectionné
        /// </summary>
        public TypeDocument Ouvrage
        {
            get
            {
                return _ouvrage;
            }
            set
            {
                _ouvrage = value;
                this.RaisePropertyChanged(() => this.Ouvrage);
                this.RaisePropertyChanged(() => this.Dossiers);

            }
        }

        /// <summary>
        /// Dossier sélectionné
        /// </summary>
        public TypeDocument Dossier
        {
            get
            {
                return _dossier;
            }
            set
            {
                _dossier = value;
                this.RaisePropertyChanged(() => this.Dossier);
            }
        }

        /// <summary>
        /// Liste des dossiers de l'ouvrage sélectionné
        /// </summary>
        public ObservableCollection<TypeDocument> Dossiers
        {
            get
            {
                if (Ouvrage != null)
                    return new ObservableCollection<TypeDocument>(Ouvrage.Entities);

                return null;
            }

        }

        /// <summary>
        /// Liste des ouvrages
        /// </summary>
        public ObservableCollection<TypeDocument> Ouvrages
        {
            get
            {
                return ((TypeDocumentService)serviceTypeDocument).Entities;
            }
        }

        /// <summary>
        /// Indique que tous les services sont bien chargés
        /// </summary>
        public bool AllServicesLoaded { get; set; }

        /// <summary>
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return serviceRegion.Entities; }
        }

        /// <summary>
        /// Retourne la date de début filtrer
        /// </summary>
        public Nullable<DateTime> DateDeb
        {
            get
            {
                return _dateDeb;
            }
            set
            {
                _dateDeb = value;
            }
        }

        /// <summary>
        /// Retourne la date de fin filtrer
        /// </summary>
        public Nullable<DateTime> DateFin
        {
            get
            {
                return _dateFin;
            }
            set
            {
                _dateFin = value;
            }
        }

        /// <summary>
        /// Filtrage sur l'ensemble des éléments
        /// </summary>
        public bool FilterAll
        {
            get
            {
                return _filterAll;
            }
            set
            {
                _filterAll = value;
                RaisePropertyChanged(() => this.FilterAll);
            }
        }

        /// <summary>
        /// Filtrage sur l'ensemble électrique
        /// </summary>
        public bool FilterEnsElec
        {
            get
            {
                return _filterEnsElec;
            }
            set
            {
                _filterEnsElec = value;
            }
        }

        /// <summary>
        /// Filtrage sur les portions
        /// </summary>
        public bool FilterPortion
        {
            get
            {
                return _filterPortion;
            }
            set
            {
                _filterPortion = value;
            }
        }

        /// <summary>
        /// Filtrage sur les Pp
        /// </summary>
        public bool FilterPp
        {
            get
            {
                return _filterPp;
            }
            set
            {
                _filterPp = value;
            }
        }

        /// <summary>
        /// Filtrage sur les équipements
        /// </summary>
        public bool FilterEquipement
        {
            get
            {
                return _filterEquipement;
            }
            set
            {
                _filterEquipement = value;
            }
        }

        /// <summary>
        /// Filtrage sur les document sharepoint uniquement
        /// </summary>
        public bool FilterDocument
        {
            get
            {
                return _filterDocument;
            }
            set
            {
                _filterDocument = value;
                RaisePropertyChanged(() => this.FilterDocument);
                RaisePropertyChanged(() => this.Ouvrages);
            }
        }

        /// <summary>
        /// Chaine recherché sur le libellé de l'entitié
        /// </summary>
        public String SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => this.SearchText);
            }
        }

        public Boolean IsEnable
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => this.IsEnable);
            }
        }

        /// <summary>
        /// Retourne les résultats de la recherche
        /// </summary>
        public ObservableCollection<SearchResult> Results { get; set; }

        /// <summary>
        /// Retourne si la colonne date est visible
        /// </summary>
        public Boolean IsColumnDateVisible
        {
            get
            {
                return _isColumnDateVisible;
            }
            set
            {
                _isColumnDateVisible = value;
                RaisePropertyChanged(() => this.IsColumnDateVisible);
            }
        }

        /// <summary>
        /// Retourne si l'écran est en attente
        /// </summary>
        public Boolean IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        /// <summary>
        /// Retourne le nombre de résultat formaté
        /// </summary>
        public string ResultIndicator
        {
            get
            {
                return (_nbResults > 0) ? _nbResults + (_nbResults == 1 ? " résultat trouvé" : (!_resultTruncated ? " résultats trouvés" : " résultats affichés")) : "Aucun résultat trouvé";
            }
        }

        #endregion

        #region Région/Agence/Secteur

        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnRegionSelected;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnAgenceSelected;
        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnSecteurSelected;

        /// <summary>
        /// Déclaration de la variable FiltreCleRegion
        /// </summary>
        private int? _filtreCleRegion;

        /// <summary>
        /// Déclaration de la variable FiltreCleAgence
        /// </summary>
        private int? _filtreCleAgence;

        /// <summary>
        /// Déclaration de la variable FiltreCleSecteur
        /// </summary>
        private int? _filtreCleSecteur;

        /// <summary>
        /// Retourne la clé de région filtré
        /// </summary>
        public int? FiltreCleRegion
        {
            get
            {
                return _filtreCleRegion;
            }
            set
            {
                _filtreCleRegion = value;

                if (OnRegionSelected != null)
                {
                    OnRegionSelected(this, null);
                }

                RaisePropertyChanged(() => this.FiltreCleRegion);
            }
        }

        /// <summary>
        /// Retourne la clé d'agence filtré
        /// </summary>
        public int? FiltreCleAgence
        {
            get
            {
                return _filtreCleAgence;
            }
            set
            {
                _filtreCleAgence = value;

                if (OnAgenceSelected != null)
                {
                    OnAgenceSelected(this, null);
                }

                RaisePropertyChanged(() => this.FiltreCleAgence);
            }
        }

        /// <summary>
        /// Retourne la clé de secteur filtré
        /// </summary>
        public int? FiltreCleSecteur
        {
            get
            {
                return _filtreCleSecteur;
            }
            set
            {
                _filtreCleSecteur = value;

                if (OnSecteurSelected != null)
                {
                    OnSecteurSelected(this, null);
                }

                RaisePropertyChanged(() => this.FiltreCleSecteur);
            }
        }

        #endregion

        #region Services

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'utilisateur connecté
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> userService { get; set; }


        /// <summary>
        /// Service utilisé pour gérer les types de document
        /// </summary>
        [Import]
        public IEntityService<TypeDocument> serviceTypeDocument { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les types de document
        /// </summary>
        [Import]
        public IConfigurator ServiceConfigurator { get; set; }

        #endregion Services

        #region Constructeur

        public SearchViewModel()
            : base()
        {
            IsBusy = false;

            // Define commands
            FindCommand = new ActionCommand<object>(
                obj => Find(), obj => true);

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.Ouvrages);
            };
        }

        #endregion

        #region Override Methode

        /// <summary>
        /// Evènement du chargement de la page
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            if (!viewParameters.ContainsKey("IsExpanderLoaded"))
            {
                if (viewParameters.ContainsKey(Global.Constants.PARM_SEARCH_TEXT))
                {
                    SearchText = (String)viewParameters[Global.Constants.PARM_SEARCH_TEXT];
                }

                Find();
            }

            base.ActivateView(viewName, viewParameters);

            //Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
            //la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
            if (!viewParameters.Any(p => p.Key == "IsExpanderLoaded"))
            {
                EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.Search_ExpanderTitle));
                EventAggregator.Publish("Search_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
            }

            LoadAllServices();

            if (!FilterAll && !FilterDocument && !FilterEnsElec && !FilterEquipement && !FilterPortion && !FilterPp)
            {
                FilterAll = true;
            }

            RaisePropertyChanged(() => this.Ouvrages);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Lancement de la recherche
        /// </summary>
        protected void Find()
        {
            if (String.IsNullOrEmpty(SearchText.Trim()))
            {
                Results = new ObservableCollection<SearchResult>();

                // Mise à jour du nombre de résultats
                _nbResults = Results.Count();
                this.RaisePropertyChanged(() => this.ResultIndicator);

                // Mise à jour du tableau
                this.RaisePropertyChanged(() => this.Results);
            }
            else
            {
                IsBusy = true;

                // Variables
                string UrlA = string.Empty;
                string WordQuery = string.Empty;
                string UrlQuery = string.Empty;
                string UrlFilter = string.Empty;
                string UrlSecteur = string.Empty;
                string UrlAgence = string.Empty;
                string UrlRegion = string.Empty;
                string HostUrl = App.Current.Host.Source.Scheme + "://" + App.Current.Host.Source.Host;
                string searchQuery = String.Empty; // Requête Web Service SharePoint

                // Metadata Name
                string _region = SearchScope.metadata.region.GetStringValue();
                string _regionint = SearchScope.metadata.regionint.GetStringValue();
                string _agence = SearchScope.metadata.agence.GetStringValue();
                string _agenceint = SearchScope.metadata.agenceint.GetStringValue();
                string _secteur = SearchScope.metadata.secteur.GetStringValue();
                string _secteurint = SearchScope.metadata.secteurint.GetStringValue();

                // Initialisation de la requête SQL
                searchQuery = "Select region, regionint, agence, agenceint, secteur, secteurint, created, fileextension, Title, Author, Size, Path, Write, HitHighlightedSummary, HitHighlightedProperties ";

                // Modification de la recherche
                string[] words = SearchText.Split(' ');
                string wordsToSearch = string.Empty;
                foreach (string word in words)
                {
                    // Ajout des étoiles pour activer la recherche relative du mot et non la recherche exacte du mot complet
                    wordsToSearch += " *" + word + "* ";
                }

                // Mot recherché
                searchQuery += "FROM Scope() WHERE FREETEXT(DefaultProperties, '" + wordsToSearch + "') ";

                // Définition du scope
                string SetScope = SearchScope.Scopes.All.GetStringValue(); //Par défaut on recherche sur l'ensemble des données proteca

                if (FilterAll)
                {
                    SetScope = SearchScope.Scopes.All.GetStringValue();
                    IsColumnDateVisible = true;
                }
                if (FilterEnsElec)
                {
                    SetScope = SearchScope.Scopes.EnsElec.GetStringValue();
                    IsColumnDateVisible = false;
                }
                if (FilterPortion)
                {
                    SetScope = SearchScope.Scopes.Portion.GetStringValue();
                    IsColumnDateVisible = false;
                }
                if (FilterPp)
                {
                    SetScope = SearchScope.Scopes.Pp.GetStringValue();
                    IsColumnDateVisible = false;
                }
                if (FilterEquipement)
                {
                    SetScope = SearchScope.Scopes.Equipements.GetStringValue();
                    IsColumnDateVisible = false;
                }
                if (FilterDocument)
                {
                    SetScope = SearchScope.Scopes.DocOnly.GetStringValue();
                    IsColumnDateVisible = true;
                }

                searchQuery += "AND ((\"SCOPE\"='" + SetScope + "'))";

                // Construction de l'url métadata
                if (FilterDocument)
                {
                    if (FiltreCleRegion != null)
                    {
                        if (FiltreCleAgence != null)
                        {
                            if (FiltreCleSecteur != null)
                            {
                                searchQuery += "AND CONTAINS(\"" + _region + "\", '" + FiltreCleRegion + "')";
                                searchQuery += "AND CONTAINS(\"" + _agence + "\", '" + FiltreCleAgence + "')";
                                searchQuery += "AND CONTAINS(\"" + _secteur + "\", '" + FiltreCleSecteur + "')";
                            }
                            else
                            {
                                searchQuery += "AND CONTAINS(\"" + _region + "\", '" + FiltreCleRegion + "')";
                                searchQuery += "AND CONTAINS(\"" + _agence + "\", '" + FiltreCleAgence + "')";
                            }
                        }
                        else
                        {
                            searchQuery += "AND CONTAINS(\"" + _region + "\", '" + FiltreCleRegion + "')";
                        }
                    }
                }
                else if (FilterEnsElec || FilterEquipement || FilterPortion || FilterPp)
                {
                    if (FiltreCleRegion != null)
                    {
                        if (FiltreCleAgence != null)
                        {
                            if (FiltreCleSecteur != null)
                            {
                                searchQuery += "AND ((\"" + _regionint + "\"=" + FiltreCleRegion + "))";
                                searchQuery += "AND ((\"" + _agenceint + "\"=" + FiltreCleAgence + "))";
                                searchQuery += "AND ((\"" + _secteurint + "\"=" + FiltreCleSecteur + "))";
                            }
                            else
                            {
                                searchQuery += "AND ((\"" + _regionint + "\"=" + FiltreCleRegion + "))";
                                searchQuery += "AND ((\"" + _agenceint + "\"=" + FiltreCleAgence + "))";
                            }
                        }
                        else
                        {
                            searchQuery += "AND ((\"" + _regionint + "\"=" + FiltreCleRegion + "))";
                        }
                    }
                }
                else if (FilterAll)
                {
                    if (FiltreCleRegion != null)
                    {
                        if (FiltreCleAgence != null)
                        {
                            if (FiltreCleSecteur != null)
                            {
                                searchQuery += "AND (CONTAINS(\"" + _region + "\", '" + FiltreCleRegion + "') OR ((\"" + _regionint + "\"=" + FiltreCleRegion + ")))";
                                searchQuery += "AND (CONTAINS(\"" + _agence + "\", '" + FiltreCleAgence + "') OR ((\"" + _agenceint + "\"=" + FiltreCleAgence + ")))";
                                searchQuery += "AND (CONTAINS(\"" + _secteur + "\", '" + FiltreCleSecteur + "') OR ((\"" + _secteurint + "\"=" + FiltreCleSecteur + ")))";
                            }
                            else
                            {
                                searchQuery += "AND (CONTAINS(\"" + _region + "\", '" + FiltreCleRegion + "') OR ((\"" + _regionint + "\"=" + FiltreCleRegion + ")))";
                                searchQuery += "AND (CONTAINS(\"" + _agence + "\", '" + FiltreCleAgence + "') OR ((\"" + _agenceint + "\"=" + FiltreCleAgence + ")))";
                            }
                        }
                        else
                        {
                            searchQuery += "AND (CONTAINS(\"" + _region + "\", '" + FiltreCleRegion + "') OR ((\"" + _regionint + "\"=" + FiltreCleRegion + ")))";
                        }
                    }
                }

                // DOCUMENTS > filtre sur url
                if (FilterDocument)
                {
                    // ##### FILTRE PAR DOSSIER ######
                    if (this.Ouvrage != null)
                    {

                        UrlQuery = this.Ouvrage.ServerRelativeUrl + "/" + this.Ouvrage.Libelle;
                        searchQuery += "AND CONTAINS(\"Path\", '" + UrlQuery + "')";
                    }
                }

                // pass the search query to the method to actually call the search service
                QuerySearchService(searchQuery);
            }
        }

        /// <summary>
        /// Envoi de la requête XML au web service SharePoint
        /// </summary>
        /// <param name="searchQuery"></param>
        private void QuerySearchService(string searchQuery)
        {
            QueryServiceSoapClient queryService;

            //Récupération de l'url de base de SharePoint
            Microsoft.SharePoint.Client.ClientContext cc = ServiceConfigurator.GetClientContext();
            if (cc != null)
            {
                if (cc.Url.StartsWith("https"))
                {
                    queryService = new QueryServiceSoapClient("QueryServiceSoap_Https", cc.Url + _sharepointSearchLink);
                }
                else
                {
                    queryService = new QueryServiceSoapClient("QueryServiceSoap", cc.Url + _sharepointSearchLink);
                }
            }
            else
            {
                queryService = new QueryServiceSoapClient();
            }

            queryService.QueryExCompleted += queryService_QueryExCompleted;

            StringBuilder queryXml = new StringBuilder();

            queryXml.Append("<QueryPacket Revision=\"1000\">");
            queryXml.Append("<Query>");
            queryXml.Append("<Context>");
            queryXml.Append("<QueryText language=\"en-US\" type=\"MSSQLFT\">");
            queryXml.Append("<![CDATA[");
            queryXml.Append(searchQuery);
            queryXml.Append("]]>");
            queryXml.Append("</QueryText>");
            queryXml.Append("</Context>");
            queryXml.Append("<SupportedFormats Format=\"urn:Microsoft.Search.Response.Document.Document\" />");
            queryXml.Append("<ResultProvider>SharepointSearch</ResultProvider>");
            queryXml.Append("<Range>");
            queryXml.Append("<StartAt>1</StartAt>");
            queryXml.Append("<Count>1001</Count>");
            queryXml.Append("</Range>");
            queryXml.Append("<EnableStemming>true</EnableStemming>");
            queryXml.Append("<EnableSpellCheck>Suggest</EnableSpellCheck>");
            queryXml.Append("<IncludeSpecialTermsResults>true</IncludeSpecialTermsResults>");
            queryXml.Append("<IncludeRelevantResults>true</IncludeRelevantResults>");
            queryXml.Append("<ImplicitAndBehavior>true</ImplicitAndBehavior>");
            queryXml.Append("<TrimDuplicates>true</TrimDuplicates>");
            queryXml.Append("<Properties>");
            queryXml.Append("<Property name=\"Rank\" />");
            queryXml.Append("<Property name=\"Title\" />");
            queryXml.Append("<Property name=\"Author\" />");
            queryXml.Append("<Property name=\"Size\" />");
            queryXml.Append("<Property name=\"Path\" />");
            queryXml.Append("<Property name=\"Write\" />");
            queryXml.Append("<Property name=\"Created\" />");
            queryXml.Append("<Property name=\"HitHighlightedSummary\" />");
            queryXml.Append("<Property name=\"HitHighlightedProperties\" />");
            queryXml.Append("<Property name=\"FileExtension\" />");
            queryXml.Append("</Properties>");
            queryXml.Append("</Query>");
            queryXml.Append("</QueryPacket>");

            //IsBusy = true;
            queryService.QueryExAsync(queryXml.ToString());
        }

        /// <summary>
        /// Callback du service web de recherche SharePoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void queryService_QueryExCompleted(object sender, QueryExCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var backresults = from result in
                                      e.Result.Nodes[1].Descendants("RelevantResults")

                                  select new SearchResult
                                  {
                                      Title = (result.Elements("TITLE").Any())
                                        ? result.Element("TITLE").Value : string.Empty,
                                      Path = (result.Elements("PATH").Any())
                                        ? System.Uri.UnescapeDataString(result.Element("PATH").Value) : string.Empty,
                                      Author = (result.Elements("AUTHOR").Any())
                                        ? result.Element("AUTHOR").Value : string.Empty,
                                      Size = (result.Elements("SIZE").Any())
                                        ? result.Element("SIZE").Value : string.Empty,
                                      Created = (result.Elements("CREATED").Any())
                                        ? DateTime.Parse(result.Element("CREATED").Value) : DateTime.MinValue,
                                      Write = (result.Elements("WRITE").Any())
                                        ? DateTime.Parse(result.Element("WRITE").Value) : DateTime.MinValue,
                                      SiteName = (result.Elements("SITENAME").Any())
                                        ? result.Element("SITENAME").Value : string.Empty,
                                      HitHighlightedSummary = (result.Elements("HITHIGHLIGHTEDSUMMARY").Any())
                                        ? result.Element("HITHIGHLIGHTEDSUMMARY").Value : string.Empty,
                                      ContentClass = (result.Elements("CONTENTCLASS").Any())
                                        ? result.Element("CONTENTCLASS").Value : string.Empty,
                                      IsDocument = (result.Elements("ISDOCUMENT").Any())
                                        ? bool.Parse(result.Element("ISDOCUMENT").Value) : false,
                                      FileExtension = (result.Elements("FILEEXTENSION").Any())
                                        ? result.Element("FILEEXTENSION").Value : string.Empty
                                  };

                // Modification des items
                ObservableCollection<SearchResult> ModifiedResults = new ObservableCollection<SearchResult>();
                foreach (SearchResult item in backresults)
                {
                    bool ItemToDelete = false;

                    // Application des filtrages secondaires
                    // ---- Documents : Sous-Dossier / DateDeb + DateFin
                    // DOCUMENTS > filtre sur url
                    if (FilterDocument)
                    {
                        // ##### FILTRE PAR SOUS-DOSSIER ######
                        if (this.Ouvrage != null)
                        {
                            if (this.Dossier != null)
                            {
                                if (!item.Path.Contains(this.Ouvrage.ServerRelativeUrl + "/" + this.Ouvrage.Libelle + "/" + this.Dossier.Libelle))
                                {
                                    ItemToDelete = true;
                                }
                            }
                        }

                        // ##### FILTRE PAR DATE ######
                        if (DateDeb.HasValue)
                        {
                            if (!item.Created.HasValue)
                            {
                                ItemToDelete = true;
                            }
                            else
                            {
                                string DateDebDay = DateDeb.Value.Day.ToString();
                                string DateDebMonth = DateDeb.Value.Month.ToString("d2");
                                string DateDebYear = DateDeb.Value.Year.ToString();

                                if (DateFin.HasValue)
                                {
                                    string DateFinDay = DateFin.Value.Day.ToString();
                                    string DateFinMonth = DateFin.Value.Month.ToString("d2");
                                    string DateFinYear = DateFin.Value.Year.ToString();

                                    if (item.Created < DateDeb || item.Created > DateFin)
                                    {
                                        ItemToDelete = true;
                                    }
                                }
                                else
                                {
                                    if (item.Created < DateDeb)
                                    {
                                        ItemToDelete = true;
                                    }
                                }
                            }
                        }
                    }

                    if (!ItemToDelete)
                    {
                        // Modification du format de la date
                        if (item.Created.HasValue)
                        {
                            item.Date = DateTime.Parse(String.Format("{0:dd/MM/yyyy}", item.Created));
                        }

                        // Gestion des extensions
                        switch (item.FileExtension.ToUpper())
                        {
                            case "ZIP":
                                item.ImageExtension = ResourceSearch.archive;
                                item.Extension = ResourceSearch.ZIP;
                                break;
                            case "XLS":
                                item.ImageExtension = ResourceSearch.excel;
                                item.Extension = ResourceSearch.XLS;
                                break;
                            case "XLSX":
                                item.ImageExtension = ResourceSearch.excel;
                                item.Extension = ResourceSearch.XLSX;
                                break;
                            case "DOC":
                                item.ImageExtension = ResourceSearch.word;
                                item.Extension = ResourceSearch.DOC;
                                break;
                            case "DOCX":
                                item.ImageExtension = ResourceSearch.word;
                                item.Extension = ResourceSearch.DOCX;
                                break;
                            case "RAR":
                                item.ImageExtension = ResourceSearch.archive;
                                item.Extension = ResourceSearch.RAR;
                                break;
                            case "PDF":
                                item.ImageExtension = ResourceSearch._pdf;
                                item.Extension = ResourceSearch.PDF;
                                break;
                            case "ASPX":
                                item.ImageExtension = ResourceSearch.other;
                                item.Extension = ResourceSearch.na;
                                item.Date = null;
                                break;
                            default:
                                item.ImageExtension = ResourceSearch.other;
                                item.Extension = ResourceSearch.na;
                                break;
                        }

                        // Modification du Summary
                        // Remplacement des caractères spéciaux
                        item.HitHighlightedSummary = item.HitHighlightedSummary.Replace(" <ddd/> ", "...");
                        item.HitHighlightedSummary = item.HitHighlightedSummary.Replace("<c0>", "");
                        item.HitHighlightedSummary = item.HitHighlightedSummary.Replace("</c0>", "");
                        // Limitiation du nombre de caractères à 150
                        if (item.HitHighlightedSummary.Length > 150)
                        {
                            item.HitHighlightedSummary = item.HitHighlightedSummary.Substring(0, 150);
                            item.HitHighlightedSummary += "...";
                        }

                        ModifiedResults.Add(item);
                    }
                }

                Results = ModifiedResults;

                // Suppression du dernier résultat si 1001 résultats
                if (Results.Count > 1000)
                {
                    _resultTruncated = true;
                    Results.Remove(Results.Last());
                }

                // Mise à jour du nombre de résultats
                _nbResults = Results.Count();
                this.RaisePropertyChanged(() => this.ResultIndicator);

                // Mise à jour du tableau
                this.RaisePropertyChanged(() => this.Results);
            }
            else
            {
                Logger.Log(LogSeverity.Warning, "Search", e.Error.Message);
            }

            IsBusy = false;
        }

        #endregion

        #region Private Methods

        //private int servicesLoadedCount = 0;
        //private String lockObject = String.Empty;
        public EventHandler OnAllServicesLoaded;

        /// <summary>
        ///  Charge la liste de toutes les entitées
        /// </summary>
        /// <returns></returns>
        private void LoadAllServices()
        {

            userService.GetEntities((err) =>
            {
                if (err != null)
                {
                    Logger.Log(LogSeverity.Error, GetType().FullName, err);
                    ErrorWindow.CreateNew(Resource.Error_UserNotFound);
                }
                else
                {

                    EntityServiceHelper.LoadAllServicesAsync(
                        this,
                        (svc, error) =>
                        {
                            if (error != null)
                            {
                                Logger.Log(LogSeverity.Error, GetType().FullName, error);
                            }
                        }, () =>
                        {
                            AllServicesLoaded = true;
                            if (OnAllServicesLoaded != null)
                                OnAllServicesLoaded(this, null);
                        });
                }
            });


            //var properties = this.GetType().GetProperties();
            //lock (lockObject)
            //{
            //    servicesLoadedCount = properties.Count(p => p.PropertyType.IsGenericType && (p.PropertyType.GetGenericTypeDefinition() == typeof(IEntityService<>)
            //        || p.PropertyType.GetGenericTypeDefinition() == typeof(IUserService<>)));
            //}
            //// Pour chaque service de type IEntityService<>
            //foreach (var prop in properties.Where(p => p.PropertyType.IsGenericType && (p.PropertyType.GetGenericTypeDefinition() == typeof(IEntityService<>)
            //    || p.PropertyType.GetGenericTypeDefinition() == typeof(IUserService<>))))
            //{
            //    var serv = prop.GetValue(this, null);
            //    if (serv != null)
            //    {
            //        var getEntitiesMethod = serv.GetType().GetMethod("GetEntities");
            //        if (getEntitiesMethod != null)
            //        {
            //            var serviceStep = new WorkflowAction();

            //            var getEntitiesMethodInfo = serv.GetType().GetMethod("GetEntities");
            //            getEntitiesMethodInfo.Invoke(serv, new object[]{(Action<Exception>)((error) =>
            //            {
            //                if (error != null)
            //                {
            //                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
            //                    if (serv is UserService)
            //                    {
            //                        ErrorWindow.CreateNew(Resource.Error_UserNotFound);
            //                    }
            //                }
            //                lock (lockObject)
            //                {
            //                    servicesLoadedCount--;
            //                    if (servicesLoadedCount <= 0)
            //                    {
            //                        // A ce niveau, toutes les entités sont chargées
            //                        AllServicesLoaded = true;

            //                        if (OnAllServicesLoaded != null)
            //                        {
            //                            OnAllServicesLoaded(this, null);
            //                        }                                    
            //                    }
            //                }
            //            })});
            //        }
            //        else
            //        {
            //            lock (lockObject)
            //            {
            //                servicesLoadedCount--;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        lock (lockObject)
            //        {
            //            servicesLoadedCount--;
            //        }
            //    }
            //}
        }

        #endregion

        #region Event

        public void OnImportsSatisfied()
        {
            EventAggregator.Subscribe<CurrentNavigation>(this);
            EventAggregator.Subscribe<ViewMode>(this);
        }

        public void HandleEvent(ViewMode publishedEvent)
        {
            switch (publishedEvent)
            {
                case ViewMode.NavigationMode:
                    IsEnable = true;
                    break;
                case ViewMode.EditMode:
                    IsEnable = false;
                    break;
                default:
                    break;
            }
        }

        public void HandleEvent(CurrentNavigation publishedEvent)
        {

        }

        #endregion
    }

    public class SearchResult
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public string Author { get; set; }
        public string Size { get; set; }
        public DateTime? Write { get; set; }
        public DateTime? Created { get; set; }
        public string SiteName { get; set; }
        public string HitHighlightedSummary { get; set; }
        public string ContentClass { get; set; }
        public bool IsDocument { get; set; }
        public string FileExtension { get; set; }

        // Propriété pour Silverlight
        public DateTime? Date { get; set; }
        public string ImageExtension { get; set; }
        public string Extension { get; set; }

    }
}
