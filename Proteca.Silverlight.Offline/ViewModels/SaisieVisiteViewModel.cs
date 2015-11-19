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
using Proteca.Silverlight.Helpers;
using Jounce.Framework.Command;
using Jounce.Core.Command;
using Proteca.Silverlight.Services;
using Telerik.Windows.Controls;
using Proteca.Silverlight.Views.Windows;
using System.Windows.Controls;
using System.ServiceModel.DomainServices.Client;
using System.Resources;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for SaisieVisite entity
    /// </summary>
    [ExportAsViewModel("SaisieVisite")]
    public class SaisieVisiteViewModel : BaseProtecaEntityViewModel<Composition>, IEventSink<UsrUtilisateur>, IEventSink<Dictionary<string, object>>, IEventSink<InsInstrument>
    {
        #region Services

        /// <summary>
        /// Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;

        /// <summary>
        /// Service utilisé pour gérer la suppression des AnAnalyseSerieMesure
        /// </summary>
        [Import]
        public IEntityService<AnAnalyseSerieMesure> ServiceAnAnalyseSerieMesure { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type Utilisateur
        /// </summary>
        [Import]
        public IEntityService<UsrUtilisateur> ServiceUtilisateur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type equipement
        /// </summary>
        [Import]
        public IEntityService<EqEquipement> ServiceEquipement { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type PP
        /// </summary>
        [Import]
        public IEntityService<Pp> ServicePp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> ServiceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer instruments et supprmier les InstrumentsUtilises
        /// </summary>
        [Import]
        public IEntityService<InsInstrument> ServiceInstrument { get; set; }

        /// <summary>
        /// Service pour récupérer les MesClassementMesure
        /// </summary>
        [Import]
        public IEntityService<MesClassementMesure> ServiceMesClassementMesure { get; set; }

        /// <summary>
        /// Service pour supprimer les alertes
        /// </summary>
        [Import]
        public IEntityService<Alerte> ServiceAlerte { get; set; }

        /// <summary>
        /// Service pour récupérer les Paramètres
        /// </summary>
        [Import]
        public IEntityService<RefParametre> ServiceRefParametre { get; set; }

        /// <summary>
        /// Service pour récupérer les RefUsrPortee
        /// </summary>
        [Import]
        public IEntityService<RefUsrPortee> ServiceRefUsrPortee { get; set; }

        /// <summary>
        /// Service pour gérer les LogOuvrages pour les PP
        /// </summary>
        [Import]
        public IEntityService<LogOuvrage> ServiceLogOuvrage { get; set; }

        /// <summary>
        /// Service pour supprimer des MesMesure
        /// </summary>
        [Import]
        public IEntityService<MesMesure> ServiceMesMesure { get; set; }

        /// <summary>
        /// Service pour supprimer des Visite
        /// </summary>
        [Import]
        public IEntityService<Visite> ServiceVisite { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les PpJumelee
        /// </summary>
        [Import]
        public IEntityService<PpJumelee> ServicePpJumelee { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les TypeEquipements (pour la popup de création d'équipement)
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> ServiceTypeEquipement { get; set; }

        #endregion Services

        #region Enums
        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les types de statuts en base
        /// </summary>
        private string enumTypeEval = RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue();

        #endregion

        #region properties

        #region Ecran


        public DateTime TodayDate
        {
            get
            {
                return DateTime.Now;
            }
        }


        private bool _disableGestionUAlt = false;
        /// <summary>
        /// Booléen permettant de ne pas appeler la callback de gestion du Ualt
        /// </summary>
        public bool DisableGestionUAlt
        {
            get { return _disableGestionUAlt; }
            set { _disableGestionUAlt = value; }
        }

        private int? CleRegion
        {
            get
            {
                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.PpAttachee != null
                    && this.SelectedEntity.Ouvrage.PpAttachee.GeoSecteur != null
                    && this.SelectedEntity.Ouvrage.PpAttachee.GeoSecteur.GeoAgence != null)
                {
                    return this.SelectedEntity.Ouvrage.PpAttachee.GeoSecteur.GeoAgence.CleRegion;
                }
                else
                {
                    return null;
                }
            }
        }

        private int? CleSecteur
        {
            get
            {
                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.PpAttachee != null)
                {
                    return this.SelectedEntity.Ouvrage.PpAttachee.CleSecteur;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Récupération du type d'eval à faire en fonction des caractéristiques de l'équipement
        /// </summary>
        private int? TypeEval
        {
            get
            {
                if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null)
                {
                    int evalEG = ServiceEnumValeur.Entities.Where(ev => ev.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue() && ev.Valeur == "1").Select(ev => ev.CleEnumValeur).FirstOrDefault();
                    int evalECD = ServiceEnumValeur.Entities.Where(ev => ev.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue() && ev.Valeur == "2").Select(ev => ev.CleEnumValeur).FirstOrDefault();

                    int typeEval = this.SelectedEntity.EnumTypeEval;

                    // Si différent de EG ou ECD => pas de vérification sur la sensibilité PP
                    if (typeEval != evalEG && typeEval != evalECD)
                    {
                        return typeEval;
                    }

                    if (this.SelectedEntity.Ouvrage is Pp
                        && ((Pp)this.SelectedEntity.Ouvrage).RefNiveauSensibilitePp != null
                        && !((Pp)this.SelectedEntity.Ouvrage).RefNiveauSensibilitePp.EnumTypeEval.HasValue)
                    {
                        return (int?)null;
                    }
                    else if (this.SelectedEntity.Ouvrage is Pp
                        && ((Pp)this.SelectedEntity.Ouvrage).CategoriePp != null
                        && ((Pp)this.SelectedEntity.Ouvrage).CategoriePp.RefNiveauSensibilitePp != null
                        && ((Pp)this.SelectedEntity.Ouvrage).CategoriePp.RefNiveauSensibilitePp.EnumTypeEval != this.SelectedEntity.EnumTypeEval)
                    {
                        typeEval = evalEG;
                    }

                    return typeEval;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Backup des visites mesures pour le rafraichissement
        /// </summary>
        public List<VisiteMesure> BackupVisiteMesures { get; set; }

        /// <summary>
        /// Backup des visites mesures complementaires pour le rafraichissement
        /// </summary>
        public List<VisiteMesure> BackupVisiteMesuresComplementaires { get; set; }

        public List<RefEnumValeur> EnumGraphique
        {
            get
            {
                return ServiceEnumValeur.Entities.Where(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.ENUM_TYPE_GRAPHIQUE.ToString()).ToList();
            }
        }

        private Graphique graphEon;
        public Graphique GraphEon
        {
            get
            {
                if (graphEon == null)
                {
                    graphEon = new Graphique();
                }
                return graphEon;
            }
            set
            {
                graphEon = value;
            }
        }
        private Graphique graphEoff;
        public Graphique GraphEoff
        {
            get
            {
                if (graphEoff == null)
                {
                    graphEoff = new Graphique();
                }
                return graphEoff;
            }
            set
            {
                graphEoff = value;
            }
        }
        private Graphique graphIdc;
        public Graphique GraphIdc
        {
            get
            {
                if (graphIdc == null)
                {
                    graphIdc = new Graphique();
                }
                return graphIdc;
            }
            set
            {
                graphIdc = value;
            }
        }
        private Graphique graphUalt;
        public Graphique GraphUalt
        {
            get
            {
                if (graphUalt == null)
                {
                    graphUalt = new Graphique();
                }
                return graphUalt;
            }
            set
            {
                graphUalt = value;
            }
        }

        public RefParametre SeuilInferieurUCana
        {
            get
            {
                return this.ServiceRefParametre.Entities.FirstOrDefault(p => p.Libelle == "SEUIL_INFERIEUR_UCANA");
            }
        }

        public bool AlerteEnable
        {
            get
            {
                return this.IsEditMode
                    && this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null
                    && !this.SelectedEntity.Ouvrage.LastVisite.RelevePartiel;
            }
        }

        private bool _alerteDeclenchee = false;
        public bool AlerteDeclenchee
        {
            get
            {
                return _alerteDeclenchee;
            }
            set
            {
                _alerteDeclenchee = value;
                RaisePropertyChanged(() => this.AlerteDeclenchee);
            }
        }

        private bool _isPPExpanded;
        /// <summary>
        /// Retourne si l'expander Caractéristiques PP est développé
        /// </summary>
        public bool IsPPExpanded
        {
            get
            {
                return _isPPExpanded;
            }
            set
            {
                if (value == true)
                {
                    InitVisiteTileExpanders();
                }
                _isPPExpanded = value;

                RefreshVisiteTileExpanders();
            }
        }

        private bool _isMesuresExpanded;
        /// <summary>
        /// Retourne si l'expander Mesures est développé
        /// </summary>
        public bool IsMesuresExpanded
        {
            get
            {
                return _isMesuresExpanded;
            }
            set
            {
                if (value == true)
                {
                    InitVisiteTileExpanders();
                }
                _isMesuresExpanded = value;

                RefreshVisiteTileExpanders();
            }
        }

        private bool _isAnalyseExpanded;
        /// <summary>
        /// Retourne si l'expander Analyse EE est développé
        /// </summary>
        public bool IsAnalyseExpanded
        {
            get
            {
                return _isAnalyseExpanded;
            }
            set
            {
                if (value == true)
                {
                    InitVisiteTileExpanders();
                }
                _isAnalyseExpanded = value;

                RefreshVisiteTileExpanders();
            }
        }

        public List<RefEnumValeur> EtatPC
        {
            get
            {
                return ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.AN_ETAT_PC.GetStringValue()).OrderBy(e => e.NumeroOrdre).ToList();
            }
        }

        public List<RefEnumValeur> TypeAlerte
        {
            get
            {
                return ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue()).OrderBy(e => e.NumeroOrdre).ToList();
            }
        }

        public bool IsPpNonEditMode
        {
            get
            {
                return this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage is Pp
                    && !this.IsEditMode;
            }
        }

        private TileViewItemState _graphiqueTileItemState = TileViewItemState.Minimized;
        public TileViewItemState GraphiqueTileItemState
        {
            get { return _graphiqueTileItemState; }
            set
            {
                _graphiqueTileItemState = value;
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null)
                {
                    this.IsBusy = true;
                    (this.service as CompositionService).GetPortionGraphique(this.SelectedEntity.Ouvrage.PpAttachee.ClePortion, LoadPortionGraphiqueDone);
                }
                RaisePropertyChanged(() => this.GraphiqueTileItemState);
            }
        }

        private TileViewItemState _tableauxTileItemState = TileViewItemState.Minimized;
        public TileViewItemState TableauxTileItemState
        {
            get { return _tableauxTileItemState; }
            set
            {
                _tableauxTileItemState = value;
                if (TableauxTileItemState == TileViewItemState.Maximized && this.SelectedEntity != null)
                {
                    this.IsBusy = true;
                    (this.service as CompositionService).GetTourneeTableauBord(this.Entities.Select(c => c.CleTournee).FirstOrDefault(i => i != 0), LoadTourneeTableauBordDone);
                }
                RaisePropertyChanged(() => this.TableauxTileItemState);
            }
        }

        public List<SelectTourneeTableauBord_Result> TableauBordEntities
        {
            get
            {
                return (this.service as CompositionService).TableauBordEntities
                    .OrderBy(t => t.Libelle_EE)
                    .ThenBy(t => t.Libelle_PI)
                    .ThenBy(t => t.PK)
                    .ThenBy(t => t.CODE_EQUIPEMENT)
                    .ToList();
            }
        }

        public ObservableCollection<InsInstrument> ListInstruments
        {
            get
            {
                return new ObservableCollection<InsInstrument>(ServiceInstrument.Entities.Where(i => this.SelectedEntity != null
                                                                    && this.SelectedEntity.Ouvrage != null
                                                                    && this.SelectedEntity.Ouvrage.LastVisite != null
                                                                    && this.SelectedEntity.Ouvrage.PpAttachee != null
                                                                    && this.SelectedEntity.Ouvrage.PpAttachee.GeoSecteur != null
                                                                    && this.SelectedEntity.Ouvrage.PpAttachee.GeoSecteur.GeoAgence != null
                                                                    && !this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.Select(iu => iu.InsInstrument).Contains(i)
                                                                    && !i.Supprime
                                                                    && ((i.CleSecteur.HasValue && i.CleSecteur.Value == this.SelectedEntity.Ouvrage.PpAttachee.CleSecteur)
                                                                        || (i.CleAgence.HasValue && i.CleAgence.Value == this.SelectedEntity.Ouvrage.PpAttachee.GeoSecteur.CleAgence)
                                                                        || (i.CleRegion.HasValue && i.CleRegion.Value == this.SelectedEntity.Ouvrage.PpAttachee.GeoSecteur.GeoAgence.CleRegion)))
                                                                .OrderBy(e => e.Libelle));
            }
        }

        public List<UsrUtilisateur> ListUtilisateurs
        {
            get
            {
                return ServiceUtilisateur.Entities.OrderBy(u => u.Nom).ThenBy(u => u.Prenom).ToList();
            }
        }

        private InsInstrument _instrument;
        public InsInstrument Instrument
        {
            get
            {
                return _instrument;
            }
            set
            {
                _instrument = value;
                RaisePropertyChanged(() => this.Instrument);
            }
        }

        /// <summary>
        /// Indique si le bouton Ajouter est visible ou non
        /// </summary>
        public Boolean IsAddVisible
        {
            get
            {
                // Visible uniquement si il y a un équipement de sélectionné
                if (this.SelectedEntity != null)
                {
                    return !IsEditMode && this.SelectedEntity.Ouvrage.LastVisite == null;
                }
                return false;
            }
        }

        /// <summary>
        /// Indique si il y a eu des changements sur l'écran
        /// Dans le cas d'un ajout, si il n'y a pas de changement, on autorise à passer au suivant/précédent
        /// </summary>
        private Boolean _hasChanged;
        public Boolean HasChanged
        {
            get
            {
                return _hasChanged;
            }
            set
            {
                if (_hasChanged != value)
                {
                    _hasChanged = value;
                    UpdateIsEditMode();
                }
            }
        }

        /// <summary>
        /// Retourne le libellé du secteur et de l'ensemble electrique
        /// </summary>
        public string SecteurEnsElecLibelle
        {
            get
            {
                if (SelectedEntity != null)
                {
                    String libelleSecteur = null;
                    String libelleEnsElec = null;

                    if (Secteurs.Count > 0 && this.CleSecteur != null)
                    {
                        libelleSecteur = Secteurs.FirstOrDefault(s => s.CleSecteur == this.CleSecteur).LibelleSecteur;
                    }

                    IOuvrage ouvrage = SelectedEntity.Ouvrage;
                    if (ouvrage != null)
                    {
                        if (ouvrage.PpAttachee != null)
                        {
                            // Si PP
                            if (ouvrage.CodeEquipement.Equals("PP"))
                            {
                                libelleEnsElec = (ouvrage as Pp).EnsElec;
                            }
                            // Sinon autre equipement
                            else
                            {
                                libelleEnsElec = ouvrage.PpAttachee.EnsElec;
                            }
                        }
                    }                    
                    return libelleSecteur + " / " + libelleEnsElec;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retourne le libelle de la portion
        /// </summary>
        public string PortionPpAttacheeLibelle
        {
            get
            {
                if (SelectedEntity != null)
                {
                    String LibellePortion = null;

                    IOuvrage ouvrage = SelectedEntity.Ouvrage;
                    if (ouvrage != null)
                    {
                        if (ouvrage.PpAttachee != null)
                        {
                            if (ouvrage.CodeEquipement.Equals("PP"))
                            {
                                LibellePortion = (ouvrage as Pp).Portion;
                            }
                            else
                            {
                                LibellePortion = ouvrage.PpAttachee.Portion;
                            }
                        }
                    }
                    return LibellePortion;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retourne le libelle de la PP et son point kilométrique
        /// </summary>
        public string PPwithPKLibelle
        {
            get
            {
                if (SelectedEntity != null)
                {
                    String libellePP = null;
                    String pkPP = null;

                    IOuvrage ouvrage = SelectedEntity.Ouvrage;
                    if (ouvrage != null)
                    {
                        if (ouvrage.PpAttachee != null)
                        {
                            if (ouvrage.CodeEquipement.Equals("PP"))
                            {
                                pkPP = (ouvrage as Pp).Pk.ToString();
                                libellePP = (ouvrage as Pp).Libelle;
                            }
                            else
                            {
                                pkPP = ouvrage.PpAttachee.Pk.ToString();
                                libellePP = ouvrage.PpAttachee.Libelle;
                            }
                        }
                    }
                    return pkPP + " - " + libellePP;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Récupération du type d'eval de l'ouvrage de la tournee
        /// </summary>
        private int? TypeEvalOuvrage
        {
            get
            {
                if (SelectedEntity != null)
                {
                    return this.SelectedEntity.EnumTypeEval;
                }
                return null;
            }
        }

        /// <summary>
        /// Retourne le libelle du type d'évaluation
        /// </summary>
        public string TypeEvalLibelle
        {
            get
            {
                if (TypeEvalOuvrage != null && TypeEvalOuvrage != 0)
                {
                    return ListTypeEval.FirstOrDefault(t => t.CleEnumValeur == TypeEvalOuvrage).LibelleCourt;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retourne la liste des types d'évaluation
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListTypeEval
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return
                        new ObservableCollection<RefEnumValeur>(
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue())
                                .OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        #endregion

        #region Navigation SaisieVisite Proteca

        /// <summary>
        /// 
        /// </summary>
        private Composition _selectedItem;
        public Composition SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                Composition previous = _selectedItem;
                if (value != previous)
                {
                    HasChanged = false;

                    _selectedItem = value;
                    if (_selectedItem != null)
                    {
                        // suppression des modifications avant la navigation
                        service.RejectChanges();
                        IsEditMode = false;
                    }
                    else
                    {
                        SelectedEntity = null;
                        SelectedId = null;
                    }

                    MainTileItemState = TileViewItemState.Maximized;
                    (this.service as CompositionService).GraphiqueEntities = new ObservableCollection<SelectPortionGraphique_Result>();

                    RaisePropertyChanged(() => this.PreviousEntity);
                    RaisePropertyChanged(() => this.PreviousUri);
                    RaisePropertyChanged(() => this.NextEntity);
                    RaisePropertyChanged(() => this.NextUri);

                    RaisePropertyChanged(() => this.IsAddVisible);
                }
            }
        }

        public Boolean IsNavigationEnabled
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    return !HasChanged || !IsEditMode;
                }
                return !IsEditMode;
            }
        }

        public Boolean IsNewMode2 { get; set; }

        #endregion

        #region Saved Properties

        private int _savedCleTournee;
        public int SavedCleTournee
        {
            get { return this._savedCleTournee; }
            set { this._savedCleTournee = value; }
        }

        private bool HasSavedData
        {
            get
            {
                return SavedUtilisateur != null || SavedInstruments.Any() || SavedDate.HasValue;
            }
        }

        public DateTime? SavedDate;

        public UsrUtilisateur SavedUtilisateur;

        private List<InsInstrument> _savedInstruments;
        public List<InsInstrument> SavedInstruments
        {
            get
            {
                if (this._savedInstruments == null)
                {
                    this._savedInstruments = new List<InsInstrument>();
                }
                return this._savedInstruments;
            }
            set
            {
                this._savedInstruments = value;
            }
        }

        #endregion

        #region Expander

        public int? FiltreCleSecteurRecherche
        {
            get
            {
                return (this.service as CompositionService).FiltreCleSecteurRecherche;
            }
            set
            {
                if ((this.service as CompositionService).FiltreCleSecteurRecherche != value)
                {
                    (this.service as CompositionService).FiltreCleSecteurRecherche = value;

                    RaisePropertyChanged(() => this.Entities);
                    RaisePropertyChanged(() => this.FiltreCleSecteurRecherche);

                    if (value.HasValue
                        && this.SelectedEntity != null
                        && this.Entities.Any()
                        && this.SelectedEntity.Ouvrage.PpAttachee.CleSecteur != value.Value)
                    {
                        LoadSaveHelper.Refresh(this.NavigationService as NavigationService);
                    }
                }
            }
        }

        /// <summary>
        /// Retourne les Secteurs disponibles 
        /// </summary>
        public List<GeoSecteur> Secteurs
        {
            get
            {
                return (this.service as CompositionService).OriginEntities.Where(c => c.Ouvrage != null && c.Ouvrage.PpAttachee != null).Select(c => c.Ouvrage.PpAttachee.GeoSecteur)
                    .Distinct(new InlineEqualityComparer<GeoSecteur>((a, b) =>
                        {
                            return a.CleSecteur.Equals(b.CleSecteur) && a.LibelleSecteur.Equals(b.LibelleSecteur);
                        })).OrderBy(s => s.LibelleSecteur).ToList();
            }
        }

        /// <summary>
        /// Libelle de la tournee présente dans le 
        /// </summary>
        public String LibelleTournee
        {
            get
            {
                return (this.Entities.Any(c => c.Tournee != null)) ? this.Entities.FirstOrDefault(c => c.Tournee != null).Tournee.Libelle : String.Empty;
            }
        }

        #endregion

        #endregion

        #region Constructeur

        public SaisieVisiteViewModel()
            : base()
        {
            // Désactive l'ajout automatique en édition
            IsAutoAddOnEditMode = false;

            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("Visite_DetailPP".AsViewNavigationArgs().AddNamedParameter("IsLightVersion", true));
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.SaisieVisite_ExpanderTitle));
                    EventAggregator.Publish("SaisieVisite_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }

                if (this.Entities.Any(c => c.CleTournee != 0 && c.CleTournee != SavedCleTournee))
                {
                    this.LoadEntities();
                }

                this.MainTileItemState = TileViewItemState.Maximized;
            };

            this.OnDetailLoaded += (o, e) =>
            {
                HasChanged = false;

                RaisePropertyChanged(() => this.Entities);
                RaisePropertyChanged(() => this.SelectedId);
                RaisePropertyChanged(() => this.SelectedEntity);

                RaisePropertyChanged(() => this.PPwithPKLibelle);
                RaisePropertyChanged(() => this.PortionPpAttacheeLibelle);
                RaisePropertyChanged(() => this.SecteurEnsElecLibelle);
                RaisePropertyChanged(() => this.TypeEvalLibelle);
                
                (this.service as CompositionService).DeleteDuplicatedVisites();

                this.LoadEquipement();

                this.MainTileItemState = TileViewItemState.Maximized;
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                IsBusy = true;
                ((MesClassementMesureService)this.ServiceMesClassementMesure).GetMesClassementMesureWithMesNiveauProtection(LoadMesNiveauProtectionDone);

                //NavigateToId((int)Entities.First().GetCustomIdentity());
                if (SelectedEntity != null)
                {
                    this.LoadDetailEntity();
                }

                // MAJ des services
                RaisePropertyChanged(() => this.Secteurs);
                RaisePropertyChanged(() => this.Entities);
            };

            this.OnAllServicesLoadedSync += (o, e) =>
            {
                IsBusy = true;
                ((MesClassementMesureService)this.ServiceMesClassementMesure).GetMesClassementMesureWithMesNiveauProtection(LoadMesNiveauProtectionDone);

                // MAJ des services
                RaisePropertyChanged(() => this.Secteurs);
                RaisePropertyChanged(() => this.Entities);
            };

            this.OnEntitiesLoaded += (o, e) =>
            {
                if (Entities.Count() > 0)
                {
                    NavigateToId((int)Entities.First().GetCustomIdentity());
                }
            };

            this.OnAddedEntity += (o, e) =>
            {
                EventAggregator.Publish("Visite_DetailPP".AsViewNavigationArgs().AddNamedParameter("IsLightVersion", true).AddNamedParameter("SelectedEntity", this.SelectedEntity != null ? this.SelectedEntity.Ouvrage : null));
                AlerteDeclenchee = false;
            };

            this.OnViewModeChanged += (o, e) =>
            {
                CheckAnAnalyse();

                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null)
                {
                    this.SelectedEntity.Ouvrage.LastVisite.IsNewInOfflineMode = IsEditMode;
                }

                this.Instrument = null;

                UpdateIsEditMode();
                RaisePropertyChanged(() => this.IsPpNonEditMode);
                RaisePropertyChanged(() => this.AlerteEnable);
                RaisePropertyChanged(() => this.PreviousEntity);
                RaisePropertyChanged(() => this.PreviousUri);
                RaisePropertyChanged(() => this.NextEntity);
                RaisePropertyChanged(() => this.NextUri);
                RaisePropertyChanged(() => this.IsAddVisible);

                AddInstrumentCommand.RaiseCanExecuteChanged();
                RemoveInstrumentCommand.RaiseCanExecuteChanged();
                GetDialogUtilisateurCommand.RaiseCanExecuteChanged();
                GetDialogInstrumentCommand.RaiseCanExecuteChanged();
            };

            this.OnSaveSuccess += (o, e) =>
            {
                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null)
                {
                    if (IsNewMode2)
                    {
                        IsNewMode2 = false;
                    }

                    this.DisableGestionUAlt = true;
                    this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged, true);
                    this.AlerteDeclenchee = this.SelectedEntity.Ouvrage.LastVisite.Alerte != null;
                    this.DisableGestionUAlt = false;
                    this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");

                    if (this.SelectedEntity.Ouvrage is Pp)
                    {
                        (this.SelectedEntity.Ouvrage as Pp).CommitCoordonneeGPSFiabilisee();
                    }

                    NavigateToId(Entities.IndexOf(this.SelectedEntity), true);
                }
                RaisePropertyChanged(() => this.IsPpNonEditMode);

                HasChanged = false;

                this.NotifyError = false;
            };

            this.OnCanceled += (o, e) =>
            {
                if (this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null)
                {
                    if (this.SelectedEntity.Ouvrage.LastVisite != null)
                    {
                        NavigateToId(Entities.IndexOf(this.SelectedEntity), true);
                    }
                    else
                    {
                        RaisePropertyChanged(() => this.SelectedEntity);
                    }
                }
            };

            this.OnSaveError += (o, e) =>
            {
                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null)
                {
                    if (IsNewMode2)
                    {
                        this.SelectedEntity.Ouvrage.LastVisite.IsNewInOfflineMode = true;
                    }

                    this.SavedDate = null;
                    this.SavedUtilisateur = null;
                    this.SavedInstruments = null;
                    this.CheckAnAnalyse();
                }

                this.NotifyError = true;

                RaisePropertyChanged(() => this.SelectedEntity);
            };

            this.OnCanceled += (o, e) =>
            {
                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null)
                {
                    this.DisableGestionUAlt = true;

                    this.SelectedEntity.Ouvrage.LastVisite.IsDuplicated = false;

                    this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged, true);
                    this.AlerteDeclenchee = this.SelectedEntity.Ouvrage.LastVisite.Alerte != null;



                    this.DisableGestionUAlt = false;
                    this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");
                }
                RaisePropertyChanged(() => this.IsPpNonEditMode);

                this.HasChanged = false;

                this.NotifyError = false;
            };

            this.OnDeleteSuccess += (o, e) =>
            {
                this.LoadEquipement(true);
            };

            this.OnImportsSatisfiedEvent += (o, e) =>
            {
                EventAggregator.Subscribe<UsrUtilisateur>(this);
                EventAggregator.Subscribe<Dictionary<string, object>>(this);
                EventAggregator.Subscribe<InsInstrument>(this);
            };

            // Define commands
            AddInstrumentCommand = new ActionCommand<object>(
                obj => AddInstrument(), obj => IsEditMode);
            RemoveInstrumentCommand = new ActionCommand<object>(
                obj => RemoveInstrument(obj), obj => IsEditMode);
            GetDialogUtilisateurCommand = new ActionCommand<object>(
                obj => ShowDialogUtilisateur(), obj => IsEditMode);
            GetDialogEquipementCommand = new ActionCommand<object>(
                obj => ShowDialogEquipement(), obj => !HasChanged && SelectedEntity != null);
            GetDialogInstrumentCommand = new ActionCommand<object>(
                obj => ShowDialogInstrument(), obj => IsEditMode);
        }

        #endregion

        #region Overrided Methods

        protected override void DeactivateView(string viewName)
        {


            // désactivation de la vue de la popup utilisateur.
            Router.DeactivateView("CreateUsrUtilisateur");

            Router.DeactivateView("CreateInsInstrument");

            Router.DeactivateView("CreateEqEquipementTmp");

            // désactivation de la vue de détail PP
            Router.DeactivateView("Visite_DetailPP");

            // Remise en place du IsEditMode de l'expander
            CustomExpanderViewModel expanderVM = Router.ResolveViewModel<CustomExpanderViewModel>(false, "CustomExpander");
            expanderVM.IsEnable = !IsEditMode;
            this.SavedDate = null;
            this.SavedUtilisateur = null;
            this._savedInstruments = null;

            this.FiltreCleSecteurRecherche = null;

            base.DeactivateView(viewName);
        }

        //private static void AddRange<T>(EntityCollection<T> collection, IEnumerable<T> values) where T : System.ServiceModel.DomainServices.Client.Entity
        //{
        //    var valuesArray = values.ToArray();
        //    foreach (var value in valuesArray)
        //        collection.Add(value);
        //}
        /// <summary>
        /// Create a new visite in the domainContext identical to this one
        /// </summary>
        private Visite DuplicateVisite(Visite current)
        {
            var context = ((Proteca.Silverlight.App)Proteca.Silverlight.App.Current).domainContext;

            Visite newVisite = new Visite()
            {

                Pp = current.Pp,
                EqEquipement = current.EqEquipement,
                PpTmp = current.PpTmp,
                EqEquipementTmp = current.EqEquipementTmp,
                UsrUtilisateur = current.UsrUtilisateur,
                UsrUtilisateur1 = current.UsrUtilisateur1,
                UsrUtilisateur2 = current.UsrUtilisateur2,
                UsrUtilisateur3 = current.UsrUtilisateur3,
                EnumTypeEval = current.EnumTypeEval,
                EnumTypeEvalComposition = current.EnumTypeEvalComposition,
                DateImport = current.DateImport,
                DateSaisie = current.DateSaisie,
                DateValidation = current.DateValidation,
                DateVisite = current.DateVisite,
                EstValidee = current.EstValidee,
                RelevePartiel = current.RelevePartiel,
                Commentaire = current.Commentaire,
                Telemesure = current.Telemesure,
                EnumConformiteTournee = current.EnumConformiteTournee
            };



            //AddRange(newVisite.InstrumentsUtilises, current.InstrumentsUtilises.Select(iu => new InstrumentsUtilises() { InsInstrument = iu.InsInstrument }));
            foreach (InstrumentsUtilises ins in current.InstrumentsUtilises)
            {
                newVisite.InstrumentsUtilises.Add(new InstrumentsUtilises() { InsInstrument = ins.InsInstrument });
            }


            //AddRange(newVisite.AnAnalyseSerieMesure, current.AnAnalyseSerieMesure.Select(ana =>
            //    {
            //        AnAnalyseSerieMesure newAna = new AnAnalyseSerieMesure()
            //        {
            //            UsrUtilisateur = ana.UsrUtilisateur,
            //            Commentaire = ana.Commentaire,
            //            DateAnalyse = ana.DateAnalyse,
            //            EnumEtatPc = ana.EnumEtatPc
            //        };

            //        var alertesAna = ana.Alertes.Select(ale => new Alerte()
            //            {
            //                EnumTypeAlerte = ale.EnumTypeAlerte,
            //                Date = ale.Date,
            //                Supprime = ale.Supprime
            //            }).ToArray();
            //        AddRange(newAna.Alertes, alertesAna);
            //        AddRange(newVisite.Alertes, alertesAna);

            //        return ana;
            //    }));
            foreach (AnAnalyseSerieMesure ana in current.AnAnalyseSerieMesure)
            {
                AnAnalyseSerieMesure newAna = new AnAnalyseSerieMesure()
                {
                    UsrUtilisateur = ana.UsrUtilisateur,
                    Commentaire = ana.Commentaire,
                    DateAnalyse = ana.DateAnalyse,
                    EnumEtatPc = ana.EnumEtatPc
                };

                //var alertesAna = ana.Alertes.Select(ale => new Alerte()
                //    {
                //        EnumTypeAlerte = ale.EnumTypeAlerte,
                //        Date = ale.Date,
                //        Supprime = ale.Supprime
                //    }).ToArray();
                //AddRange(newAna.Alertes, alertesAna);
                //AddRange(newVisite.Alertes, alertesAna);

                foreach (Alerte ale in ana.Alertes)
                {
                    Alerte newAle = new Alerte()
                    {
                        EnumTypeAlerte = ale.EnumTypeAlerte,
                        Date = ale.Date,
                        Supprime = ale.Supprime
                    };
                    newAna.Alertes.Add(newAle);
                    newVisite.Alertes.Add(newAle);
                }
                newVisite.AnAnalyseSerieMesure.Add(newAna);
            }

            //var mesuresDatas = current.MesMesure.Select(mes =>
            //{
            //    var alertes = mes.Alertes.Select(ale => new Alerte()
            //        {
            //            EnumTypeAlerte = ale.EnumTypeAlerte,
            //            Date = ale.Date,
            //            Supprime = ale.Supprime
            //        }).ToArray();
            //    var data = new
            //    {
            //        newMes = new MesMesure()
            //        {
            //            CleTypeMesure = mes.CleTypeMesure,
            //            Valeur = mes.Valeur
            //        },
            //        alertes
            //    };
            //    AddRange(data.newMes.Alertes, alertes);
            //    return data;
            //}).ToArray();
            //AddRange(newVisite.MesMesure, mesuresDatas.Select(md => md.newMes));
            //AddRange(newVisite.Alertes, mesuresDatas.SelectMany(md => md.alertes));

            //AddRange(newVisite.MesMesure, current.MesMesure.Select(mes =>
            //{
            //    var newMes = new MesMesure()
            //    {
            //        CleTypeMesure = mes.CleTypeMesure,
            //        Valeur = mes.Valeur
            //    };
            //    var alertes = mes.Alertes.Select(ale => new Alerte()
            //        {
            //            EnumTypeAlerte = ale.EnumTypeAlerte,
            //            Date = ale.Date,
            //            Supprime = ale.Supprime
            //        }).ToArray();
            //    AddRange(newMes.Alertes, alertes);
            //    AddRange(newVisite.Alertes, alertes);
            //    return newMes;
            //}));

            foreach (MesMesure mes in current.MesMesure)
            {
                MesMesure newMes = new MesMesure()
                {
                    CleTypeMesure = mes.CleTypeMesure,
                    Valeur = mes.Valeur
                };
                foreach (Alerte ale in mes.Alertes)
                {
                    Alerte newAle = new Alerte()
                    {
                        EnumTypeAlerte = ale.EnumTypeAlerte,
                        Date = ale.Date,
                        Supprime = ale.Supprime
                    };
                    newMes.Alertes.Add(newAle);
                    newVisite.Alertes.Add(newAle);
                }
                newVisite.MesMesure.Add(newMes);
            }



            //AddRange(newVisite.Alertes,
            //    current.Alertes.Where(a => !a.CleAnalyse.HasValue && !a.CleMesure.HasValue && a.AnAnalyseSerieMesure == null && a.MesMesure == null)
            //    .Select(ale => new Alerte()
            //    {
            //        EnumTypeAlerte = ale.EnumTypeAlerte,
            //        Date = ale.Date,
            //        Supprime = ale.Supprime
            //    }));
            foreach (Alerte ale in current.Alertes.Where(a => !a.CleAnalyse.HasValue && !a.CleMesure.HasValue && a.AnAnalyseSerieMesure == null && a.MesMesure == null))
            {
                newVisite.Alertes.Add(new Alerte()
                {
                    EnumTypeAlerte = ale.EnumTypeAlerte,
                    Date = ale.Date,
                    Supprime = ale.Supprime
                });
            }

            current.IsDuplicated = true;

            return newVisite;
        }


        protected override void Add()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (!UserCanAdd)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else if (this.SelectedEntity.Ouvrage != null)
            {
                IsNewMode2 = true;
                this.SelectedEntity.Ouvrage.Visites.Add(new Visite()
                {
                    DateSaisie = DateTime.Now,
                    DateVisite = DateTime.Now,
                    EnumTypeEval = this.TypeEval.HasValue ? this.TypeEval.Value : 0,
                    EnumTypeEvalComposition = this.SelectedEntity.EnumTypeEval,
                    EstValidee = false,
                    IsNewInOfflineMode = true
                });

                if (this.SelectedEntity.Ouvrage is Pp)
                {
                    (this.SelectedEntity.Ouvrage as Pp).PropertyChanged -= PpToVisiteMesures_PropertyChanged;
                    (this.SelectedEntity.Ouvrage as Pp).PropertyChanged += PpToVisiteMesures_PropertyChanged;
                }

                CheckAnAnalyse();

                this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");

                this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, SetPropertyChanged);

                if (this.HasSavedData)
                {
                    foreach (InsInstrument item in this.SavedInstruments)
                    {
                        if (!this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.Any(iu => iu.InsInstrument == item))
                        {
                            this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.Add(new InstrumentsUtilises() { InsInstrument = item });
                        }
                    }
                    this.SelectedEntity.Ouvrage.LastVisite.UsrUtilisateur2 = SavedUtilisateur;
                    this.SelectedEntity.Ouvrage.LastVisite.DateVisite = SavedDate.Value;
                }

                AlerteDeclenchee = false;

                this.IsEditMode = true;

                // Force l'évènement OnIsEditModeChanged du maincontainer pour faire le focus sur le premier élément
                HasChanged = true;

                RegisterPropertyChanged();

                this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");

                HasChanged = false;

                RefreshScreen();
            }
        }

        protected override void Save()
        {
            if (this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null
                && this.SelectedEntity.Ouvrage.LastVisite != null)
            {
                if (!this.SelectedEntity.Ouvrage.PpAttachee.BypassPkLimitation
                    && this.SelectedEntity.Ouvrage.PpAttachee.HasChanges
                    && this.SelectedEntity.Ouvrage.PpAttachee.Pk > this.SelectedEntity.Ouvrage.PpAttachee.PortionIntegrite.Longueur)
                {
                    this.SelectedEntity.Ouvrage.PpAttachee.BypassPkLimitation = true;
                }

                //Propagation d'un changement de date visite aux alertes 
                if (this.SelectedEntity.Ouvrage.LastVisite.Alertes.Any())
                {
                    foreach (Alerte alerte in this.SelectedEntity.Ouvrage.LastVisite.Alertes)
                    {
                        alerte.Date = this.SelectedEntity.Ouvrage.LastVisite.DateVisite != null ? (DateTime)this.SelectedEntity.Ouvrage.LastVisite.DateVisite : DateTime.Now;
                    }
                }

                // Gestion des Visites sans type d'évaluation 
                if (this.SelectedEntity.Ouvrage is Pp
                    && this.SelectedEntity.Ouvrage.LastVisite.EnumTypeEval == 0)
                {
                    ErrorWindow.CreateNew(Resource.Visite_NonMesuree_Erreur);
                    return;
                }

                // Sauvegarde des données de tournée
                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null
                    && this.SelectedEntity.Ouvrage.LastVisite.IsNew())
                {
                    this.SavedInstruments.Clear();
                    foreach (InstrumentsUtilises item in this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises)
                    {
                        this.SavedInstruments.Add(item.InsInstrument);
                    }
                    SavedUtilisateur = this.SelectedEntity.Ouvrage.LastVisite.UsrUtilisateur2;
                    SavedDate = this.SelectedEntity.Ouvrage.LastVisite.DateVisite;
                }

                //Alertes Utilisateurs
                if (this.AlerteDeclenchee && this.SelectedEntity.Ouvrage.LastVisite.Alerte == null)
                {
                    Alerte alerte = new Alerte()
                    {
                        Supprime = false,
                        Date = this.SelectedEntity.Ouvrage.LastVisite.DateVisite != null ? (DateTime)this.SelectedEntity.Ouvrage.LastVisite.DateVisite : DateTime.Now,
                        RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "U")
                    };

                    this.SelectedEntity.Ouvrage.LastVisite.Alertes.Add(alerte);
                }
                else if (!this.AlerteDeclenchee && this.SelectedEntity.Ouvrage.LastVisite.Alerte != null)
                {
                    ServiceAlerte.Delete(this.SelectedEntity.Ouvrage.LastVisite.Alerte);
                }

                //Alertes Seuil
                List<MesMesure> mesuresToFlag = this.SelectedEntity.Ouvrage.LastVisite.MesMesure.Where(m => m.IsDepassementSeuil && m.Alerte == null).ToList();
                foreach (MesMesure mes in mesuresToFlag)
                {
                    Alerte alerte = new Alerte()
                    {
                        Supprime = false,
                        Date = this.SelectedEntity.Ouvrage.LastVisite.DateVisite != null ? (DateTime)this.SelectedEntity.Ouvrage.LastVisite.DateVisite : DateTime.Now,
                        RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "S")
                    };

                    this.SelectedEntity.Ouvrage.LastVisite.Alertes.Add(alerte);
                    mes.Alertes.Add(alerte);
                }
                List<MesMesure> mesuresToUnflag = this.SelectedEntity.Ouvrage.LastVisite.MesMesure.Where(m => !m.IsDepassementSeuil && m.Alerte != null).ToList();
                foreach (MesMesure mes in mesuresToUnflag)
                {
                    ServiceAlerte.Delete(mes.Alerte);
                }

                //Alertes Analyse
                if (this.SelectedEntity.Ouvrage.LastVisite.Analyse.IsNew() && (String.IsNullOrEmpty(this.SelectedEntity.Ouvrage.LastVisite.Analyse.Commentaire) && this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur == null))
                {
                    ServiceAnAnalyseSerieMesure.Delete(this.SelectedEntity.Ouvrage.LastVisite.Analyse);
                }
                else if (((this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur != null && this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur.Valeur == "01") || this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur == null) && this.SelectedEntity.Ouvrage.LastVisite.Analyse.Alerte != null)
                {
                    ServiceAlerte.Delete(this.SelectedEntity.Ouvrage.LastVisite.Analyse.Alerte);
                }
                else if (this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur != null && this.SelectedEntity.Ouvrage.LastVisite.Analyse.Alerte == null)
                {
                    Alerte alerte = new Alerte()
                    {
                        Supprime = false,
                        Date = this.SelectedEntity.Ouvrage.LastVisite.DateVisite != null ? (DateTime)this.SelectedEntity.Ouvrage.LastVisite.DateVisite : DateTime.Now,
                        RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "A")
                    };

                    this.SelectedEntity.Ouvrage.LastVisite.Alertes.Add(alerte);
                    this.SelectedEntity.Ouvrage.LastVisite.Analyse.Alertes.Add(alerte);
                }


                //UsrUtilisateur à supprimer
                List<UsrUtilisateur> users = ServiceUtilisateur.Entities.Where(u => u.IsNew() && String.IsNullOrEmpty(u.Identifiant) && !u.Visites2.Any()).ToList();
                foreach (UsrUtilisateur user in users)
                {
                    ((UsrUtilisateurService)ServiceUtilisateur).Delete(user);
                }

                this.SelectedEntity.Ouvrage.LastVisite.DateSaisie = DateTime.Now;

                //MANTIS 15797, l'objet analyse est supprimé de la visite temp que si rien n'est renseigné dans refEnumValeur
                //il n'est pas supprimé en cas d'erreur car l'association RefEnumValeur/Commentaires doit être validée.
                //les lignes ci-dessous detecte cette erreur non prise en charge et valide l'objet sous conditions.

                bool invalidCom = false;
                if (this.SelectedEntity.Ouvrage.LastVisite.Analyse != null)
                {
                    string value = this.SelectedEntity.Ouvrage.LastVisite.Analyse.Commentaire;
                    List<String> tags = new List<string>() { "style", "script" };
                    String valueStripped = Proteca.Web.Helpers.HTMLHelper.StripHtml((value != null && value.ToString() != " ") ? value.ToString() : String.Empty, tags);
                    invalidCom = ((this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur.Valeur == "02" || this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur.Valeur == "03") && (String.IsNullOrEmpty(valueStripped) || valueStripped == "&nbsp;"));
                }

                if (!invalidCom)
                {
                    //validation qui permet de ne pas supprimer la derniére edition avec annuler.
                    this.SelectedEntity.Ouvrage.LastVisite.IsNewInOfflineMode = false;
                    this.SelectedEntity.Ouvrage.LastVisite.EstValidee = true;
                }

            }
            base.Save();
        }

        protected override void Delete()
        {
            base.Delete(true);
        }

        #endregion

        #region Public Methods

        public void HandleEvent(UsrUtilisateur publishedEvent)
        {
            if (IsActive)
            {
                // Récupération du nouvel utilisateur
                UsrUtilisateur newUser = new UsrUtilisateur()
                {
                    Nom = publishedEvent.Nom,
                    Prenom = publishedEvent.Prenom,
                    Societe = publishedEvent.Societe,
                    Externe = publishedEvent.Externe,
                    Mail = publishedEvent.Mail,
                    Identifiant = publishedEvent.Identifiant,
                    RefUsrPortee = this.ServiceRefUsrPortee.Entities.FirstOrDefault(rup => rup.CodePortee == "05" && rup.TypePortee == "USR")
                };

                publishedEvent = null;

                ServiceUtilisateur.Add(newUser);
                RaisePropertyChanged(() => this.ListUtilisateurs);

                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null)
                {
                    this.SelectedEntity.Ouvrage.LastVisite.UsrUtilisateur2 = newUser;
                }
            }
        }

        public void HandleEvent(InsInstrument publishedEvent)
        {
            if (IsActive)
            {
                // Récupération du nouvel instrument
                InsInstrument newIns = new InsInstrument()
                {
                    Libelle = publishedEvent.Libelle,
                    CleRegion = this.CleRegion
                };

                publishedEvent = null;

                ServiceInstrument.Add(newIns);
                RaisePropertyChanged(() => this.ListInstruments);
                Instrument = newIns;
            }
        }

        public void HandleEvent(Dictionary<string, object> equipementTmpProperties)
        {
            if (IsActive)
            {
                Composition newCompo = new Composition()
                {
                    CleTournee = this.service.Entities.Select(c => c.CleTournee).FirstOrDefault(i => i != 0),
                    EnumTypeEval = (int)equipementTmpProperties["EnumTypeEval"],
                    EqEquipementTmp = new EqEquipementTmp()
                    {
                        Libelle = (string)equipementTmpProperties["Libelle"],
                        CleTypeEq = (int)equipementTmpProperties["CleTypeEq"],
                        ClePp = (int)equipementTmpProperties["ClePp"]
                    }
                };

                this.service.Add(newCompo);

                if (this.FiltreCleSecteurRecherche.HasValue && newCompo.EqEquipementTmp.Pp.CleSecteur != this.FiltreCleSecteurRecherche.Value)
                {
                    this.FiltreCleSecteurRecherche = newCompo.EqEquipementTmp.Pp.CleSecteur;
                }
                RaisePropertyChanged(() => this.Entities);

                NavigateToId(Entities.IndexOf(newCompo), true);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Chargement de l'équipement depuis la base et rafraichissement des éléments de navigation correspondant
        /// </summary>
        private void LoadEquipement(Boolean reloadAfterDelete = false)
        {
            if (this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null)
            {
                IsBusy = true;

                // Reset des éléments sauvegardés si tournée différente
                if (this.SelectedEntity.CleTournee != this.SavedCleTournee)
                {
                    this.SavedCleTournee = this.SelectedEntity.CleTournee;
                    this.SavedInstruments = null;
                    this.SavedUtilisateur = null;
                    this.SavedDate = null;
                }

                SearchEntityEquipement(null, reloadAfterDelete);

                this.HasChanged = false;

                RaisePropertyChanged(() => this.IsAddVisible);
            }
        }

        private void CheckAnAnalyse()
        {
            if (IsEditMode
                && this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null
                && this.SelectedEntity.Ouvrage.LastVisite != null
                && this.SelectedEntity.Ouvrage.LastVisite.Analyse == null)
            {
                this.SelectedEntity.Ouvrage.LastVisite.AnAnalyseSerieMesure.Add(new AnAnalyseSerieMesure()
                {
                    CleUtilisateur = this.CurrentUser.CleUtilisateur,
                    DateAnalyse = DateTime.Now,
                    EnumEtatPc = null
                });
            }
            if (this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null
                && this.SelectedEntity.Ouvrage.LastVisite != null)
            {
                this.SelectedEntity.Ouvrage.LastVisite.ForceRaisePropertyChanged("Analyse");
            }
        }

        /// <summary>
        /// Récupération des modifications sur la Pp pour raffraichir les VisiteMesures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PpToVisiteMesures_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Abonnement aux propriétés qui doivent raffraichier les visites mesures à leur modification
            if (this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null
                && this.SelectedEntity.Ouvrage is Pp
                && (e.PropertyName == "CourantsAlternatifsInduits"
                    || e.PropertyName == "CourantsVagabonds"
                    || e.PropertyName == "ElectrodeEnterreeAmovible"
                    || e.PropertyName == "PresenceDUneTelemesure"
                    || e.PropertyName == "TemoinMetalliqueDeSurface"
                    || e.PropertyName == "TemoinEnterreAmovible"
                    || e.PropertyName == "CleCategoriePp"
                    || e.PropertyName == "CleNiveauSensibilite")
                && this.SelectedEntity.Ouvrage.LastVisite != null)
            {
                if (e.PropertyName == "CleCategoriePp" || e.PropertyName == "CleNiveauSensibilite")
                {
                    this.SelectedEntity.Ouvrage.LastVisite.EnumTypeEval = this.TypeEval.HasValue ? this.TypeEval.Value : 0;
                    RaisePropertyChanged(() => this.SelectedEntity.Ouvrage.LastVisite.EnumTypeEval);
                }

                //Backup des VisiteMesures
                BackupVisiteMesures = new List<VisiteMesure>(this.SelectedEntity.Ouvrage.LastVisite.VisiteMesures);
                BackupVisiteMesuresComplementaires = new List<VisiteMesure>(this.SelectedEntity.Ouvrage.LastVisite.VisiteMesuresComplementaires);

                this.DisableGestionUAlt = true;

                //Reload des VisiteMesures
                this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged);

                VisiteMesure temp = null;

                //Import des valeurs rentrées dans les backup vers les nouvelles
                foreach (VisiteMesure vm in this.SelectedEntity.Ouvrage.LastVisite.VisiteMesures)
                {
                    //On récupère dans le Backup la VisiteMesure correspondant à la nouvelle en cours
                    temp = BackupVisiteMesures.FirstOrDefault(v => v.Libelle == vm.Libelle && 
                        (v.Maxi != null && v.Maxi.Valeur.HasValue 
                        || v.Moyen != null && v.Moyen.Valeur.HasValue 
                        || v.Mini != null && v.Mini.Valeur.HasValue));
                    //Si il y a correspondance on copie la valeur
                    if (temp != null)
                    {
                        if (temp.Maxi.IsNew() && temp.IsMaxiEnable && temp.Maxi != null && temp.Maxi.Valeur.HasValue && vm.IsMaxiEnable && vm.Maxi != null && !vm.Maxi.Valeur.HasValue)
                        {
                            vm.Maxi.Valeur = temp.Maxi.Valeur;
                        }
                        if (temp.Mini.IsNew() && temp.IsMiniEnable && temp.Mini != null && temp.Mini.Valeur.HasValue && vm.IsMiniEnable && vm.Mini != null && !vm.Mini.Valeur.HasValue)
                        {
                            vm.Mini.Valeur = temp.Mini.Valeur;
                        }
                        if (temp.Moyen.IsNew() && temp.IsMoyenEnable && temp.Moyen != null && temp.Moyen.Valeur.HasValue && vm.IsMoyenEnable && vm.Moyen != null && !vm.Moyen.Valeur.HasValue)
                        {
                            vm.Moyen.Valeur = temp.Moyen.Valeur;
                        }
                    }
                }
                foreach (VisiteMesure vm in this.SelectedEntity.Ouvrage.LastVisite.VisiteMesuresComplementaires)
                {
                    //On récupère dans le Backup la VisiteMesureComplementaire correspondant à la nouvelle en cours
                    temp = BackupVisiteMesuresComplementaires.FirstOrDefault(v => v.Libelle == vm.Libelle && 
                        (v.Maxi != null && v.Maxi.Valeur.HasValue 
                        || v.Moyen != null && v.Moyen.Valeur.HasValue 
                        || v.Mini != null && v.Mini.Valeur.HasValue));
                    //Si il y a correspondance on copie la valeur
                    if (temp != null)
                    {
                        if (temp.Maxi.IsNew() && temp.IsMaxiEnable && temp.Maxi != null && temp.Maxi.Valeur.HasValue && vm.IsMaxiEnable && vm.Maxi != null && !vm.Maxi.Valeur.HasValue)
                        {
                            vm.Maxi.Valeur = temp.Maxi.Valeur;
                        }
                        if (temp.Mini.IsNew() && temp.IsMiniEnable && temp.Mini != null && temp.Mini.Valeur.HasValue && vm.IsMiniEnable && vm.Mini != null && !vm.Mini.Valeur.HasValue)
                        {
                            vm.Mini.Valeur = temp.Mini.Valeur;
                        }
                        if (temp.Moyen.IsNew() && temp.IsMoyenEnable && temp.Moyen != null && temp.Moyen.Valeur.HasValue && vm.IsMoyenEnable && vm.Moyen != null && !vm.Moyen.Valeur.HasValue)
                        {
                            vm.Moyen.Valeur = temp.Moyen.Valeur;
                        }
                    }
                }

                //On récupère par référence les éléments Mesures que l'on doit supprimer ceux qui ont été créé par le loadvisitemesure
                List<MesMesure> trashMaxi = (BackupVisiteMesures.Union(BackupVisiteMesuresComplementaires)).Select(vm => vm.Maxi).Where(m => m != null && m.IsNew()).ToList();
                List<MesMesure> trashMoyen = (BackupVisiteMesures.Union(BackupVisiteMesuresComplementaires)).Select(vm => vm.Moyen).Where(m => m != null && m.IsNew()).ToList();
                List<MesMesure> trashMini = (BackupVisiteMesures.Union(BackupVisiteMesuresComplementaires)).Select(vm => vm.Mini).Where(m => m != null && m.IsNew()).ToList();
                List<MesMesure> trash = trashMaxi.Union(trashMoyen.Union(trashMini)).ToList();
                //On parcours la liste à revers pour supprimer ou mettre à jour les valeurs
                int count = this.SelectedEntity.Ouvrage.LastVisite.MesMesure.Count;
                for (int i = count - 1; i > -1; i--)
                {
                    MesMesure mesureToDelete = this.SelectedEntity.Ouvrage.LastVisite.MesMesure.ElementAt(i);
                    if (trash.Contains(mesureToDelete))
                    {
                        this.ServiceMesMesure.Delete(mesureToDelete);
                    }
                }


                //Vidange des backups
                BackupVisiteMesures = new List<VisiteMesure>();
                BackupVisiteMesuresComplementaires = new List<VisiteMesure>();

                this.DisableGestionUAlt = false;

                //Rafraichissement de la vue pour afficher les changements
                RaisePropertyChanged(() => this.SelectedEntity);
            }
        }

        /// <summary>
        /// Abonnement à propertyChanged pour la gestion des coordonnées GPS et du niveau de sensibilité
        /// </summary>
        private void RegisterPropertyChanged()
        {
            if (this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null
                && this.SelectedEntity.Ouvrage.LastVisite != null)
            {
                this.SelectedEntity.Ouvrage.LastVisite.ActivateChangePropagation();
                this.SelectedEntity.Ouvrage.LastVisite.PropertyChanged += (oo, ee) =>
                {
                    if (ee.PropertyName == "RelevePartiel")
                    {
                        RaisePropertyChanged(() => this.AlerteEnable);
                        if (this.SelectedEntity.Ouvrage.LastVisite.RelevePartiel)
                        {
                            this.AlerteDeclenchee = true;
                        }
                        // Si on veux décocher automatiquement l'alerte déclenchée ou la remettre dans son état initial
                        //this.AlerteDeclenchee = this.SelectedEntity.LastVisite.RelevePartiel || this.SelectedEntity.LastVisite.CleAlerte.HasValue;
                    }

                    // Si l'utilisateur modifie autre chose que les éléments communs (date, agent de mesure, instruments)
                    // Dans ce cas l'objet est indiqué comme modifié, et donc il n'est plus possible de naviguer
                    if (ee.PropertyName != "CleUtilisateurMesure"
                        && ee.PropertyName != "UsrUtilisateur2"
                        && ee.PropertyName != "CleUtilisateurCreation"
                        && ee.PropertyName != "UsrUtilisateur"
                        && ee.PropertyName != "CleUtilisateurValidation"
                        && ee.PropertyName != "UsrUtilisateur3"
                        && ee.PropertyName != "DateVisite"
                        && ee.PropertyName != "DateSaisie"
                        && ee.PropertyName != "EstValidee"
                        && ee.PropertyName != "EntityState")
                    {
                        this.HasChanged = true;
                    }
                };
            }
        }

        private void LoadPortionGraphiqueDone(Exception ex)
        {
            GraphEon.SourceMesures = (this.service as CompositionService).GraphiqueEntities.Where(g => g.CLE_GRAPHIQUE == EnumGraphique.FirstOrDefault(v => v.Valeur == "Eon").CleEnumValeur).ToList();
            GraphEon.TitreY = EnumGraphique.FirstOrDefault(v => v.Valeur == "Eon").Libelle;
            GraphEoff.SourceMesures = (this.service as CompositionService).GraphiqueEntities.Where(g => g.CLE_GRAPHIQUE == EnumGraphique.FirstOrDefault(v => v.Valeur == "Eoff").CleEnumValeur).ToList();
            GraphEoff.TitreY = EnumGraphique.FirstOrDefault(v => v.Valeur == "Eoff").Libelle;
            GraphIdc.SourceMesures = (this.service as CompositionService).GraphiqueEntities.Where(g => g.CLE_GRAPHIQUE == EnumGraphique.FirstOrDefault(v => v.Valeur == "Itm ~").CleEnumValeur).ToList();
            GraphIdc.TitreY = EnumGraphique.FirstOrDefault(v => v.Valeur == "Itm ~").Libelle;
            GraphUalt.SourceMesures = (this.service as CompositionService).GraphiqueEntities.Where(g => g.CLE_GRAPHIQUE == EnumGraphique.FirstOrDefault(v => v.Valeur == "Ucana ~").CleEnumValeur).ToList();
            GraphUalt.TitreY = EnumGraphique.FirstOrDefault(v => v.Valeur == "Ucana ~").Libelle;
            RaisePropertyChanged(() => this.GraphEon);
            RaisePropertyChanged(() => this.GraphEoff);
            RaisePropertyChanged(() => this.GraphIdc);
            RaisePropertyChanged(() => this.GraphUalt);
            this.IsBusy = false;
        }

        /// <summary>
        /// Le chargement du tableau de bord vient d'être effectué.
        /// </summary>
        /// <param name="ex"></param>
        private void LoadTourneeTableauBordDone(Exception ex)
        {
            RaisePropertyChanged(() => this.TableauBordEntities);
            this.IsBusy = false;
        }

        /// <summary>
        /// Le chargement des niveaux de protection vient d'être effectué.
        /// </summary>
        /// <param name="ex"></param>
        private void LoadMesNiveauProtectionDone(Exception ex)
        {
            this.IsBusy = false;
        }

        /// <summary>
        /// Mise à jour du mode d'édition personalisé lié à l'écran
        /// </summary>
        private void UpdateIsEditMode()
        {
            RaisePropertyChanged(() => this.IsNavigationEnabled);
            GetDialogEquipementCommand.RaiseCanExecuteChanged();

            CustomExpanderViewModel expanderVM = Router.ResolveViewModel<CustomExpanderViewModel>(false, "CustomExpander");
            expanderVM.IsEnable = IsNavigationEnabled;
        }

        /// <summary>
        /// Ajout d'un instrument à la liste de la visite
        /// </summary>
        private void AddInstrument()
        {
            if (this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null
                && this.SelectedEntity.Ouvrage.LastVisite != null
                && this.Instrument != null)
            {
                this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.Add(new InstrumentsUtilises()
                {
                    InsInstrument = this.Instrument
                });

                this.Instrument = null;
                RaisePropertyChanged(() => this.ListInstruments);
            }
        }

        /// <summary>
        /// Suppression d'un instrument utilisé
        /// </summary>
        private void RemoveInstrument(object obj)
        {
            if (obj is InsInstrument)
            {
                InstrumentsUtilises inst = this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.FirstOrDefault(i => i.InsInstrument == (obj as InsInstrument));
                if (inst != null)
                {
                    this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.Remove(inst);
                    ((InsInstrumentService)ServiceInstrument).DeleteInstrumentUtilise(inst);
                }
            }

            RaisePropertyChanged(() => this.ListInstruments);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshScreen()
        {
            EventAggregator.Publish("Visite_DetailPP".AsViewNavigationArgs().AddNamedParameter("IsLightVersion", true).AddNamedParameter("SelectedEntity", (this.SelectedEntity != null) ? this.SelectedEntity.Ouvrage : null));

            RaisePropertyChanged(() => this.AlerteEnable);
            RaisePropertyChanged(() => this.SelectedEntity);
            RaisePropertyChanged(() => this.ListInstruments);
            RaisePropertyChanged(() => this.ListUtilisateurs);

            RaisePropertyChanged(() => this.IsPpNonEditMode);
            RaisePropertyChanged(() => this.EtatPC);
            RaisePropertyChanged(() => this.IsAddVisible);


            GetDialogEquipementCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// La recherche des tournee est terminée
        /// </summary>
        private void SearchEntityEquipement(Exception error, Boolean reloadAfterDelete = false)
        {
            IsNewMode2 = false;

            if (this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null)
            {
                //Création de la visite si non chargée
                if (this.SelectedEntity.Ouvrage.LastVisite == null)
                {
                    if (!reloadAfterDelete)
                    {
                        Add();
                    }
                    else
                    {
                        // Force le passage en consultation car on a pas de visite mais on vient d'effectuer une suppression
                        // L'utilisateur doit cliquer sur le bouton ajouter une visite pour être de nouveau en mode ajout.
                        IsEditMode = false;
                        RefreshScreen();
                    }
                }
                //Sinon chargement des éléments necéssaires à l'affichage
                else
                {
                    Visite visite = DuplicateVisite(this.SelectedEntity.Ouvrage.LastVisite);

                    if (this.SelectedEntity.Ouvrage is Pp)
                    {
                        (this.SelectedEntity.Ouvrage as Pp).PropertyChanged -= PpToVisiteMesures_PropertyChanged;
                        (this.SelectedEntity.Ouvrage as Pp).PropertyChanged += PpToVisiteMesures_PropertyChanged;
                    }

                    // Force le passage en consultation car on est sur une Visite existante
                    IsEditMode = false;

                    this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged, true);
                    AlerteDeclenchee = this.SelectedEntity.Ouvrage.LastVisite.Alerte != null;
                    RegisterPropertyChanged();
                    this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");
                    RefreshScreen();
                }

                RaisePropertyChanged(() => this.PreviousEntity);
                RaisePropertyChanged(() => this.PreviousUri);
                RaisePropertyChanged(() => this.NextEntity);
                RaisePropertyChanged(() => this.NextUri);
                RaisePropertyChanged(() => this.IsNavigationEnabled);
            }
            this.IsBusy = false;
        }

        private void SetPropertyChanged(MesMesure mesure)
        {
            decimal seuilInferieurUCana;
            if (this.SeuilInferieurUCana != null && decimal.TryParse(this.SeuilInferieurUCana.Valeur, out seuilInferieurUCana)
                && mesure.Valeur.HasValue && mesure.Valeur.Value > seuilInferieurUCana
                && this.SelectedEntity != null
                && this.SelectedEntity.Ouvrage != null
                && this.SelectedEntity.Ouvrage is Pp
                && !(this.SelectedEntity.Ouvrage as Pp).CourantsAlternatifsInduits
                && !DisableGestionUAlt)
            {
                (this.SelectedEntity.Ouvrage as Pp).CourantsAlternatifsInduits = true;
                MessageBox.Show(String.Format(Resource.SaisieVisiteChangePP_InformationBox, seuilInferieurUCana + " " + mesure.MesTypeMesure.MesModeleMesure.MesUnite.Symbole), "", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        private void GetInstrumentDispoDone(Exception error)
        {
            RaisePropertyChanged(() => this.ListInstruments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        private void GetUtilisateurDispoDone(Exception error)
        {
            RaisePropertyChanged(() => this.SelectedEntity);
        }

        /// <summary>
        /// Affichage de la popup de création d'un utilisateur
        /// </summary>
        private void ShowDialogUtilisateur()
        {
            ChildWindow.Title = "Création d'un utilisateur externe";
            EventAggregator.Publish("CreateUsrUtilisateur".AsViewNavigationArgs());
            ChildWindow.Show();
        }

        /// <summary>
        /// Affichage de la popup de création d'un utilisateur
        /// </summary>
        private void ShowDialogInstrument()
        {
            ChildWindow.Title = "Création d'un instrument de mesure";
            EventAggregator.Publish("CreateInsInstrument".AsViewNavigationArgs());
            ChildWindow.Show();
        }

        /// <summary>
        /// Affichage de la popup de création d'un équipement
        /// </summary>
        private void ShowDialogEquipement()
        {
            ChildWindow.Title = "Création d'un équipement";
            EventAggregator.Publish("CreateEqEquipementTmp".AsViewNavigationArgs());
            ChildWindow.Show();
        }

        private void RefreshVisiteTileExpanders()
        {
            if (MainTileItemState != TileViewItemState.Minimized)
            {
                RaisePropertyChanged(() => this.IsPPExpanded);
                RaisePropertyChanged(() => this.IsMesuresExpanded);
                RaisePropertyChanged(() => this.IsAnalyseExpanded);
            }
        }

        private void InitVisiteTileExpanders()
        {
            if (MainTileItemState != TileViewItemState.Minimized)
            {
                _isPPExpanded = false;
                _isMesuresExpanded = false;
                _isAnalyseExpanded = false;
            }
        }

        #endregion Private method

        #region Commands

        /// <summary>
        /// Déclaration de l'objet de command d'ajout d'un instrument utilisé
        /// </summary>
        public IActionCommand AddInstrumentCommand { get; set; }

        /// <summary>
        /// Déclaration de l'objet de command de suppression d'un instrument utilisé
        /// </summary>
        public IActionCommand RemoveInstrumentCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de création d'un utilisateur
        /// </summary>
        public IActionCommand GetDialogUtilisateurCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de création d'un équipement
        /// </summary>
        public IActionCommand GetDialogEquipementCommand { get; private set; }

        /// <summary>
        /// Déclaration de l'objet de command de création d'un instrument
        /// </summary>
        public IActionCommand GetDialogInstrumentCommand { get; private set; }

        #endregion
    }
}
