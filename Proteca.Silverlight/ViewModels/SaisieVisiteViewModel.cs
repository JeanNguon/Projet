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
using Proteca.Web.Resources;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for SaisieVisite entity
    /// </summary>
    [ExportAsViewModel("SaisieVisite")]
    public class SaisieVisiteViewModel : BaseProtecaEntityViewModel<TourneePpEq>, IEventSink<UsrUtilisateur>
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
        /// Service utilisé pour gérer les GEO ensembles électriques / portions
        /// </summary>
        [Import]
        public IEntityService<GeoEnsElecPortion> ServiceGeoEnsElecPortion { get; set; }

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
        /// Service utilisé pour gérer les PpJumelee
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> ServiceTypeEquipement { get; set; }

        #endregion Services

        #region properties

        #region Enums
        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les types de statuts en base
        /// </summary>
        private string enumTypeEval = RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue();

        #endregion

        #region Ecran

        private bool _disableGestionUAlt = false;
        /// <summary>
        /// Booléen permettant de ne pas appeler la callback de gestion du Ualt
        /// </summary>
        public bool DisableGestionUAlt 
        {
            get { return _disableGestionUAlt; }
            set { _disableGestionUAlt = value; } 
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

                    if (this.SelectedEntity.IsPP
                        && ((Pp)this.SelectedEntity.Ouvrage).RefNiveauSensibilitePp != null
                        && !((Pp)this.SelectedEntity.Ouvrage).RefNiveauSensibilitePp.EnumTypeEval.HasValue)
                    {
                        return null;
                    }
                    else if (this.SelectedEntity.IsPP
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
        /// Definition de la liste pour les Visites créées pour les Pp Jumelées
        /// </summary>
        private List<Visite> _visitesForPpJumelee;

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

        /// <summary>
        /// Backup des visites mesures pour le rafraichissement
        /// </summary>
        public List<VisiteMesure> BackupVisiteMesures { get; set; }

        /// <summary>
        /// Backup des visites mesures complementaires pour le rafraichissement
        /// </summary>
        public List<VisiteMesure> BackupVisiteMesuresComplementaires { get; set; }

        /// <summary>
        /// Retourne le RefParametre du délai d'édition des mesures
        /// </summary>
        public RefParametre DelaiEditionMesure
        {
            get
            {
                return this.ServiceRefParametre.Entities.FirstOrDefault(p => p.Libelle == "DELAI_SAISIE_MESURE");
            }
        }

        /// <summary>
        /// Retourne si on doit avoir accès ou non en édition au mesures de la visite
        /// </summary>
        public Boolean IsEditMesureMode
        {
            get
            {
                int delaiEditionMesure;
                if (this.DelaiEditionMesure != null
                    && int.TryParse(this.DelaiEditionMesure.Valeur, out delaiEditionMesure)
                    && this.IsEditMode
                    && this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null
                    && !this.SelectedEntity.Ouvrage.LastVisite.DateImport.HasValue
                    && this.SelectedEntity.Ouvrage.LastVisite.DateSaisie.HasValue)
                {
                    switch (this.DelaiEditionMesure.LibUnite)
                    {
                        case "Jours":
                            return this.SelectedEntity.Ouvrage.LastVisite.DateSaisie.Value.AddDays(delaiEditionMesure).Date >= DateTime.Now.Date;
                        case "Heures":
                            return this.SelectedEntity.Ouvrage.LastVisite.DateSaisie.Value.AddHours(delaiEditionMesure) >= DateTime.Now;
                        default:
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public DateTime TodayDate
        {
            get 
            {
                return DateTime.Now;
            }
        }


        public UsrUtilisateur OldUser { get; set; }

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
                return this.IsEditMesureMode
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

        /// <summary>
        /// Indique si le bouton Ajouter est visible ou non
        /// </summary>
        public Boolean IsAddVisible
        {
            get
            {
                // Visible uniquement si il y a un équipement de sélectionné
                if (this.SelectedItem != null)
                {
                    return !IsEditMode;
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
        /// <summary>
        /// 
        /// </summary>
        private TourneePpEq _selectedItem;
        public TourneePpEq SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                TourneePpEq previous = _selectedItem;
                if (value != previous)
                {
                    HasChanged = false;

                    _selectedItem = value;
                    if (_selectedItem != null)
                    {
                        // suppression des modifications avant la navigation
                        service.RejectChanges();
                        IsEditMode = false;

                        // Navigation
                        if (SelectedItem.ClePp.HasValue)
                        {
                            NavigationService.NavigateUri(NavigationService.getUriByFiltreId((int)SelectedItem.ClePp, FiltreNavigation.PP.GetStringValue()), true);
                        }
                        else
                        {
                            NavigationService.NavigateUri(NavigationService.getUriByFiltreId((int)SelectedItem.CleEquipement, FiltreNavigation.EQ.GetStringValue()),true);
                        }
                    }
                    else
                    {
                        SelectedEntity = null;
                        SelectedId = null;

                        // ceci devrait corriger le cas du non changement du detail PP en cas de tournée remise à null
                        // en attente de demande de correction
                        RefreshScreen();
                    }

                    MainTileItemState = TileViewItemState.Maximized;
                    (this.service as TourneePpEqService).GraphiqueEntities = new ObservableCollection<SelectPortionGraphique_Result>();

                    RaisePropertyChanged(() => this.PreviousEntity);
                    RaisePropertyChanged(() => this.PreviousUri);
                    RaisePropertyChanged(() => this.NextEntity);
                    RaisePropertyChanged(() => this.NextUri);

                    RaisePropertyChanged(() => this.SelectedItem);
                    RaisePropertyChanged(() => this.IsAddVisible);
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
                if (SelectedItem != null)
                {
                    String libelleSecteur = null;
                    String libelleEnsElec = null;

                    TourneePpEq currentTournee = service.Entities.FirstOrDefault(s => s.CleSecteur == SelectedItem.CleSecteur);
                    GeoEnsElecPortion ensElec = ServiceGeoEnsElecPortion.Entities.FirstOrDefault(ee => ee.CleEnsElectrique == SelectedItem.CleEnsElectrique);
                    if (currentTournee != null)
                    {
                        libelleSecteur = currentTournee.LibelleSecteur;
                    }
                    if (ensElec != null)
                    {
                        libelleEnsElec = ensElec.LibelleEe;
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
                        if (ouvrage.CodeEquipement.Equals("PP"))
                        {
                            LibellePortion = (SelectedEntity.Ouvrage as Pp).Portion;
                        }
                        else
                        {
                            if (ouvrage.PpAttachee != null)
                            {
                                LibellePortion = (SelectedEntity.Ouvrage as EqEquipement).PpAttachee.Portion;
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
                        if (ouvrage.CodeEquipement.Equals("PP"))
                        {
                            pkPP = (SelectedEntity.Ouvrage as Pp).Pk.ToString();
                            libellePP = (SelectedEntity.Ouvrage as Pp).Libelle;
                        }
                        else
                        {
                            if (ouvrage.PpAttachee != null)
                            {
                                pkPP = (SelectedEntity.Ouvrage as EqEquipement).PpAttachee.Pk.ToString();
                                libellePP = (SelectedEntity.Ouvrage as EqEquipement).PpAttachee.Libelle;
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
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumTypeEval)
                                .OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        /// Element de type T suivant
        /// </summary>
        public override TourneePpEq NextEntity
        {
            get
            {
                if (SelectedEntity != null && !SelectedEntity.IsNew())
                {
                    int index = Equipements.IndexOf(SelectedEntity);
                    if (index >= 0 && index < Equipements.Count - 1)
                    {
                        return Equipements.ElementAt(index + 1);
                    }
                }
                return null;
            }
        }

        public override Uri NextUri
        {
            get
            {
                if (NextEntity != null)
                {
                    if (NextEntity.ClePp.HasValue)
                    {
                        return NavigationService.getUriByFiltreId((int)NextEntity.ClePp, FiltreNavigation.PP.GetStringValue());
                    }
                    else
                    {
                        return NavigationService.getUriByFiltreId((int)NextEntity.CleEquipement, FiltreNavigation.EQ.GetStringValue());
                    }
                }
                return null;
            }

        }

        /// <summary>
        /// Element de type T précédent
        /// </summary>
        public override TourneePpEq PreviousEntity
        {
            get
            {
                if (SelectedEntity != null && !SelectedEntity.IsNew())
                {
                    int index = Equipements.IndexOf(SelectedEntity);
                    if (index > 0)
                    {
                        return Equipements.ElementAt(index - 1);
                    }
                }
                return null;
            }
        }

        public override Uri PreviousUri
        {
            get
            {
                if (PreviousEntity != null)
                {
                    if (PreviousEntity.ClePp.HasValue)
                    {
                        return NavigationService.getUriByFiltreId((int)PreviousEntity.ClePp, FiltreNavigation.PP.GetStringValue());
                    }
                    else
                    {
                        return NavigationService.getUriByFiltreId((int)PreviousEntity.CleEquipement, FiltreNavigation.EQ.GetStringValue());
                    }
                }
                return null;
            }
        }

        public ObservableCollection<RefEnumValeur> EtatPC
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.AN_ETAT_PC.GetStringValue()).OrderBy(e => e.NumeroOrdre));
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
                return this.SelectedEntity != null && this.SelectedEntity.IsPP && !this.IsEditMode;
            }
        }

        private TileViewItemState _graphiqueTileItemState = TileViewItemState.Minimized;
        public TileViewItemState GraphiqueTileItemState
        {
            get { return _graphiqueTileItemState; }
            set
            {
                _graphiqueTileItemState = value;
                if (value == TileViewItemState.Maximized && this.SelectedEntity != null && SavedDateDebut.HasValue && SavedDateFin.HasValue)
                {
                    this.IsBusy = true;
                    (this.service as TourneePpEqService).GetPortionGraphique(this.SelectedEntity.ClePortion, this.SavedDateDebut.Value, this.SavedDateFin.Value, LoadPortionGraphiqueDone);
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
                if (TableauxTileItemState == TileViewItemState.Maximized && this.SelectedEntity != null && SavedDateDebut.HasValue && SavedDateFin.HasValue)
                {
                    this.IsBusy = true;
                    (this.service as TourneePpEqService).GetTourneeTableauBord(this.SelectedEntity.CleTournee, this.SavedDateDebut.Value, this.SavedDateFin.Value, LoadTourneeTableauBordDone);
                }
                RaisePropertyChanged(() => this.TableauxTileItemState);
            }
        }

        private TileViewItemState _histoTileItemState = TileViewItemState.Minimized;
        public TileViewItemState HistoTileItemState
        {
            get { return _histoTileItemState; }
            set
            {
                _histoTileItemState = value;
                RaisePropertyChanged(() => this.HistoTileItemState);
            }
        }

        public List<SelectTourneeTableauBord_Result> TableauBordEntities
        {
            get
            {
                return (this.service as TourneePpEqService).TableauBordEntities.ToList();
            }
        }

        public List<InsInstrument> ListInstruments
        {
            get
            {
                if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null && this.SelectedEntity.Ouvrage.LastVisite != null)
                {
                    List<int> listInstruments = this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.Select(i => i.CleInstrument).ToList();
                    return ServiceInstrument.Entities.Where(i => !listInstruments.Contains(i.CleInstrument)).OrderBy(e => e.Libelle).ToList();
                }
                else
                    return new List<InsInstrument>();
            }
        }

        public List<UsrUtilisateur> ListUtilisateurs
        {
            get
            {
                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null
                    && !this.SelectedEntity.Ouvrage.LastVisite.IsNew()
                    && OldUser != null
                    && !ServiceUtilisateur.Entities.Contains(OldUser))
                {
                    //ListUtilisateurs.Add(OldUser);
                    return ServiceUtilisateur.Entities.Union(new List<UsrUtilisateur>() { OldUser })
                        .OrderBy(u => u.Nom).ThenBy(u => u.Prenom)
                        .ToList();
                }
                else
                {
                    return ServiceUtilisateur.Entities.OrderBy(u => u.Nom).ThenBy(u => u.Prenom).ToList();
                }
            }
        }

        private int? _filtreCleInstrument;
        public int? FiltreCleInstrument
        {
            get
            {
                return _filtreCleInstrument;
            }
            set
            {
                _filtreCleInstrument = value;
                RaisePropertyChanged(() => this.FiltreCleInstrument);
            }
        }

        #endregion

        #region Saved Properties

        private bool HasSavedData
        {
            get
            {
                return SavedUtilisateur != null || SavedDate.HasValue || SavedInstruments.Count > 0;
            }
        }

        public DateTime? SavedDate;

        public UsrUtilisateur SavedUtilisateur;

        private List<InsInstrument> _savedInstruments;
        public List<InsInstrument> SavedInstruments
        {
            get
            {
                if (_savedInstruments == null)
                {
                    _savedInstruments = new List<InsInstrument>();
                }
                return _savedInstruments;
            }
            set
            {
                _savedInstruments = value;
            }
        }

        #endregion

        #region Expander

        public String ResRecherche
        {
            get
            {

                return String.Format(Resource.SaisieVisite_ResultatRecherche, this.Tournees.Count, (this.Tournees.Count > 1) ? "s" : String.Empty);
            }
        }

        private int? _filtreCleSecteurRecherche;
        public int? FiltreCleSecteurRecherche
        {
            get
            {
                return _filtreCleSecteurRecherche;
            }
            set
            {
                _filtreCleSecteurRecherche = value;

                RaisePropertyChanged(() => this.FiltreCleSecteurRecherche);
                RaisePropertyChanged(() => this.Equipements);
            }
        }

        private int? _filtreCleTournee;
        public int? FiltreCleTournee
        {
            get
            {
                return _filtreCleTournee;
            }
            set
            {
                _filtreCleTournee = value;

                SelectedEntity = Equipements.FirstOrDefault();

                RaisePropertyChanged(() => this.FiltreCleTournee);
                RaisePropertyChanged(() => this.Secteurs);
                RaisePropertyChanged(() => this.Equipements);
                SavedUtilisateur = null;
                SavedInstruments = new List<InsInstrument>();
                SavedDate = null;

                MainTileItemState = TileViewItemState.Maximized;
                (this.service as TourneePpEqService).TableauBordEntities = new ObservableCollection<SelectTourneeTableauBord_Result>();

                if (value.HasValue && this.Equipements.Count() > 0 && value != 0)
                {
                    //Récupération des Utilisateurs et Instruments disponibles
                    ((InsInstrumentService)ServiceInstrument).FindInsInstrumentByCleTournee(value.Value, GetInstrumentDispoDone);
                    ((UsrUtilisateurService)ServiceUtilisateur).FindUsrUtilisateurByCleTournee(value.Value, GetUtilisateurDispoDone);

                    SelectedItem = this.Equipements.FirstOrDefault();
                }
                else
                {
                    SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// Retourne les Secteurs disponibles 
        /// </summary>
        public List<TourneePpEq> Secteurs
        {
            get
            {
                if (FiltreCleTournee.HasValue)
                {
                    return service.Entities.Where(s => s.CleTournee == FiltreCleTournee.Value).Distinct(new InlineEqualityComparer<TourneePpEq>((a, b) =>
                    {
                        return a.CleSecteur.Equals(b.CleSecteur) && a.LibelleSecteur.Equals(b.LibelleSecteur);
                    })).OrderBy(s => s.LibelleSecteur).ToList();
                }
                return new List<TourneePpEq>();
            }
        }

        /// <summary>
        /// Retourne les Tournées correspondantes 
        /// </summary>
        public List<TourneePpEq> Tournees
        {
            get
            {
                return service.Entities.Distinct(new InlineEqualityComparer<TourneePpEq>((a, b) =>
                {
                    return a.CleTournee.Equals(b.CleTournee) && a.LibelleTournee.Equals(b.LibelleTournee);
                })).OrderBy(t => t.LibelleTournee).ToList();
            }
        }

        /// <summary>
        /// Retourne les Equipements filtrés 
        /// </summary>
        public List<TourneePpEq> Equipements
        {
            get
            {
                if (FiltreCleTournee.HasValue)
                {
                    if (FiltreCleSecteurRecherche.HasValue)
                    {
                        return service.Entities.Where(e => e.CleTournee == FiltreCleTournee.Value && e.CleSecteur == FiltreCleSecteurRecherche.Value).OrderBy(e => e.NumeroOrdre).ToList();
                    }
                    return service.Entities.Where(e => e.CleTournee == FiltreCleTournee.Value).OrderBy(e => e.NumeroOrdre).ToList();
                }
                return new List<TourneePpEq>();
            }
        }

        /// <summary>
        /// Retourne la clé de l'ensemble électrique
        /// </summary>
        private int? _filtreCleEnsElec;
        public int? FiltreCleEnsElec
        {
            get { return _filtreCleEnsElec; }
            set
            {
                _filtreCleEnsElec = value;
                RaisePropertyChanged(() => FiltreCleEnsElec);
                RaisePropertyChanged(() => GeoEnsElecPortions);
            }
        }

        /// <summary>
        /// Définition de la clé FiltreClePortion
        /// </summary>
        private int? _filtreClePortion;

        /// <summary>
        /// Retourne la clé de la portion intégrité
        /// </summary>
        public int? FiltreClePortion
        {
            get { return _filtreClePortion; }
            set
            {
                _filtreClePortion = value;
                RaisePropertyChanged(() => this.FiltreClePortion);
            }
        }

        /// <summary>
        /// Retourne la date de debut du filtre
        /// </summary>
        public DateTime? DateDebut { get; set; }

        /// <summary>
        /// Retourne la date de fin du filtre
        /// </summary>
        public DateTime? DateFin { get; set; }

        /// <summary>
        /// Retourne la date de debut de la recherche
        /// </summary>
        public DateTime? SavedDateDebut { get; set; }

        /// <summary>
        /// Retourne la date de fin de la recherche
        /// </summary>
        public DateTime? SavedDateFin { get; set; }

        /// <summary>
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return ServiceRegion.Entities; }
        }

        /// <summary>
        /// Libelle de la tournée recherché
        /// </summary>
        public String FiltreRechercheLibelle { get; set; }

        /// <summary>
        /// Retourne les GEO ensembles électrique 
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsemblesElectrique
        {
            get
            {
                if (FiltreCleRegion.HasValue)
                {
                    if (FiltreCleAgence.HasValue)
                    {
                        if (FiltreCleSecteur.HasValue)
                        {
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteur.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                            })).ToList();
                        }
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgence.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                        })).ToList();
                    }
                    return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegion.Value).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                    })).ToList();
                }
                else
                {
                    return ServiceGeoEnsElecPortion.Entities.Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                    {
                        return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.LibelleEe.Equals(b.LibelleEe);
                    })).ToList();
                }
            }
        }

        /// <summary>
        /// Retourne les GEO ensembles électrique / portions 
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsElecPortions
        {
            get
            {
                if (FiltreCleEnsElec.HasValue)
                {
                    if (FiltreCleRegion.HasValue)
                    {
                        if (FiltreCleAgence.HasValue)
                        {
                            if (FiltreCleSecteur.HasValue)
                            {
                                return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteur.Value && i.CleEnsElectrique == FiltreCleEnsElec.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                            }
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgence.Value && i.CleEnsElectrique == FiltreCleEnsElec.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                            })).OrderBy(pi => pi.LibellePortion).ToList();
                        }
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegion.Value && i.CleEnsElectrique == FiltreCleEnsElec.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else
                    {
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleEnsElectrique == FiltreCleEnsElec.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                }
                else
                {
                    if (FiltreCleRegion.HasValue)
                    {
                        if (FiltreCleAgence.HasValue)
                        {
                            if (FiltreCleSecteur.HasValue)
                            {
                                return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleSecteur == FiltreCleSecteur.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                            }
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == FiltreCleAgence.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                            })).OrderBy(pi => pi.LibellePortion).ToList();
                        }
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == FiltreCleRegion.Value && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                    else
                    {
                        return ServiceGeoEnsElecPortion.Entities.Where(i => i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                        {
                            return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                        })).OrderBy(pi => pi.LibellePortion).ToList();
                    }
                }
            }
        }

        private List<PortionIntegrite> _portionsEnsElec;
        public List<PortionIntegrite> PortionsEnsElec
        {
            get
            {
                if (_portionsEnsElec == null)
                {
                    _portionsEnsElec = new List<PortionIntegrite>();
                }
                return _portionsEnsElec;

            }
            set
            {
                _portionsEnsElec = value;
                RaisePropertyChanged(() => this.PortionsEnsElec);
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
            IsAutoNavigateToFirst = false;

            this.OnRegionSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

            this.OnAgenceSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

            this.OnSecteurSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

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
                if (!e.ViewParameter.Any())
                {
                    // ré-activation de la navigation automatique
                    IsAutoNavigateToFirst = true;
                }

                RaisePropertyChanged(() => this.SecteurEnsElecLibelle);
                RaisePropertyChanged(() => this.PortionPpAttacheeLibelle);
                RaisePropertyChanged(() => this.PPwithPKLibelle);
                RaisePropertyChanged(() => this.TypeEvalLibelle);
            };

            this.OnDetailLoaded += (o, e) =>
            {
                this.LoadEquipement();
            };

            this.OnEntitiesLoaded += (o, e) =>
            {
                IsBusy = true;
                ((MesClassementMesureService)this.ServiceMesClassementMesure).GetMesClassementMesureWithMesNiveauProtection(LoadMesNiveauProtectionDone);
                // MAJ des services
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
                RaisePropertyChanged(() => this.FiltreRechercheLibelle);
                RaisePropertyChanged(() => this.DateFin);
                RaisePropertyChanged(() => this.DateDebut);
                RaisePropertyChanged(() => this.Tournees);
                RaisePropertyChanged(() => this.Secteurs);
                RaisePropertyChanged(() => this.ResRecherche);

                RaisePropertyChanged(() => this.Equipements);
                RaisePropertyChanged(() => this.DateDebut);
                RaisePropertyChanged(() => this.DateFin);

            };

            this.OnAddedEntity += (o, e) =>
            {
                EventAggregator.Publish("Visite_DetailPP".AsViewNavigationArgs().AddNamedParameter("IsLightVersion", true).AddNamedParameter("SelectedEntity", this.SelectedEntity != null ? this.SelectedEntity.Ouvrage : null));
                AlerteDeclenchee = false;
                DeleteVisitesForPpJumelee(false);
            };

            this.OnViewModeChanged += (o, e) =>
            {
                CheckAnAnalyse();

                UpdateIsEditMode();
                RaisePropertyChanged(() => this.IsPpNonEditMode);
                RaisePropertyChanged(() => this.AlerteEnable);
                RaisePropertyChanged(() => this.PreviousEntity);
                RaisePropertyChanged(() => this.PreviousUri);
                RaisePropertyChanged(() => this.NextEntity);
                RaisePropertyChanged(() => this.NextUri);
                RaisePropertyChanged(() => this.IsAddVisible);

                RaisePropertyChanged(() => this.SelectedItem);
                RaisePropertyChanged(() => this.SelectedItem.Ouvrage);
                RaisePropertyChanged(() => this.PortionsEnsElec);
                RaisePropertyChanged(() => this.Equipements);
                RaisePropertyChanged(() => this.PPwithPKLibelle);
                RaisePropertyChanged(() => this.PortionPpAttacheeLibelle);
                RaisePropertyChanged(() => this.SecteurEnsElecLibelle);

                RaisePropertyChanged(() => this.TypeEval);
                RaisePropertyChanged(() => this.ListTypeEval);
                RaisePropertyChanged(() => this.TypeEvalLibelle);

                RefreshScreen();
            };

            this.OnSaveSuccess += (o, e) =>
            {
                if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null)
                {
                    this.SelectedEntity.Ouvrage.VisitePeriodeDebut = SavedDateDebut;
                    this.SelectedEntity.Ouvrage.VisitePeriodeFin = SavedDateFin;
                    this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");
                    if (this.SelectedEntity.Ouvrage.LastVisite != null)
                    {
                        this.DisableGestionUAlt = true;
                        this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged);
                        AlerteDeclenchee = this.SelectedEntity.Ouvrage.LastVisite.Alerte != null;
                        this.DisableGestionUAlt = false;
                        this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");
                    }
                }
                RaisePropertyChanged(() => this.IsPpNonEditMode);

                HasChanged = false;
               // DeleteVisitesForPpJumelee(false);

                RefreshScreen();
            };

            this.OnSaveError += (o, e) =>
            {
                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null
                    && this.SelectedEntity.Ouvrage.LastVisite.IsNew())
                {
                    this.SavedDate = null;
                    this.SavedUtilisateur = null;
                    this.SavedInstruments = null;
                }

                if (this.SelectedEntity != null
                    && this.SelectedEntity.Ouvrage != null
                    && this.SelectedEntity.Ouvrage.LastVisite != null)
                {
                    // EP : Lors d'une erreur de validation, on n'a pas besoin de recharger les mesures
                    //this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged);

                    CheckAnAnalyse();
                }

                DeleteVisitesForPpJumelee(true);

                this.NotifyError = true;

                RaisePropertyChanged(() => this.SelectedEntity);

            };

            this.OnCanceled += (o, e) =>
            {
                if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null)
                {
                    this.SelectedEntity.Ouvrage.VisitePeriodeDebut = SavedDateDebut;
                    this.SelectedEntity.Ouvrage.VisitePeriodeFin = SavedDateFin;
                    this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");
                    if (this.SelectedEntity.Ouvrage.LastVisite != null)
                    {
                        this.DisableGestionUAlt = true;
                        this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged);
                        AlerteDeclenchee = this.SelectedEntity.Ouvrage.LastVisite.Alerte != null;
                        this.DisableGestionUAlt = false;
                        this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");
                    }
                }

                HasChanged = false;
            };

            this.OnDeleteSuccess += (o, e) =>
            {
                this.LoadEquipement();
            };

            this.OnImportsSatisfiedEvent += (o, e) =>
            {
                EventAggregator.Subscribe<UsrUtilisateur>(this);
            };

            // Define commands
            AddInstrumentCommand = new ActionCommand<object>(
                obj => AddInstrument(), obj => true);
            RemoveInstrumentCommand = new ActionCommand<object>(
                obj => RemoveInstrument(obj), obj => true);
            GetDialogUtilisateurCommand = new ActionCommand<object>(
                obj => ShowDialog(), obj => true);

            this.DateDebut = DateTime.Now;
            this.DateFin = DateTime.Now;
        }

        /// <summary>
        /// Chargement de l'équipement depuis la base et rafraichissement des éléments de navigation correspondant
        /// </summary>
        private void LoadEquipement()
        {
            if (this.SelectedEntity != null)
            {
                IsBusy = true;

                if (this.SelectedEntity.IsPP)
                {
                    _selectedItem = this.Equipements.FirstOrDefault(eq => eq.ClePp == this.SelectedEntity.ClePp);
                    SelectedEntity = SelectedItem;
                    ((PpService)ServicePp).GetEntityByCleAndDateAndTypeEval(this.SelectedEntity.ClePp.Value, this.SavedDateDebut, this.SavedDateFin, this.SelectedEntity.EnumTypeEval, SearchEntityEquipement);
                }
                else
                {
                    _selectedItem = this.Equipements.FirstOrDefault(eq => eq.CleEquipement == this.SelectedEntity.CleEquipement);
                    SelectedEntity = SelectedItem;
                    ((EqEquipementService)ServiceEquipement).GetEntityByCleAndDateAndTypeEval(this.SelectedEntity.CleEquipement.Value, this.SavedDateDebut, this.SavedDateFin, this.SelectedEntity.EnumTypeEval, SearchEntityEquipement);
                }

                this.HasChanged = false;

                RaisePropertyChanged(() => this.SelectedItem);
                RaisePropertyChanged(() => this.IsAddVisible);

                RaisePropertyChanged(() => this.PortionsEnsElec);
                RaisePropertyChanged(() => this.Equipements);
                RaisePropertyChanged(() => this.PPwithPKLibelle);
                RaisePropertyChanged(() => this.PortionPpAttacheeLibelle);
                RaisePropertyChanged(() => this.SecteurEnsElecLibelle);

                RaisePropertyChanged(() => this.TypeEval);
                RaisePropertyChanged(() => this.ListTypeEval);
                RaisePropertyChanged(() => this.TypeEvalLibelle);
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Initialisation des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void initGeoPreferences()
        {
            base.initGeoPreferences();
            if (userService.CurrentUser != null)
            {
                this.FiltreCleEnsElec = userService.CurrentUser.PreferenceCleEnsembleElectrique;
                this.FiltreClePortion = userService.CurrentUser.PreferenceClePortion;
            }
        }

        /// <summary>
        ///Enregistrement des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void saveGeoPreferences()
        {
            base.saveGeoPreferences();
            if (userService.CurrentUser != null)
            {
                userService.CurrentUser.SetPreferenceCleEnsembleElectrique(this.FiltreCleEnsElec);
                userService.CurrentUser.SetPreferenceClePortion(this.FiltreClePortion);
            }
        }

        protected override void DeactivateView(string viewName)
        {
            // désactivation de la vue de la popup utilisateur.
            Router.DeactivateView("CreateUsrUtilisateur");

            // désactivation de la vue de détail PP
            Router.DeactivateView("Visite_DetailPP");

            // Remise en place du IsEditMode de l'expander
            CustomExpanderViewModel expanderVM = Router.ResolveViewModel<CustomExpanderViewModel>(false, "CustomExpander");
            expanderVM.IsEnable = !IsEditMode;
            this.SavedDate = null;
            this.SavedUtilisateur = null;
            this._savedInstruments = null;

            this.DateDebut = DateTime.Now;
            this.DateFin = DateTime.Now;
            this.SavedDateDebut = null;
            this.SavedDateFin = null;

            this.FiltreRechercheLibelle = String.Empty;

            this.FiltreCleTournee = null;
            this.FiltreCleSecteurRecherche = null;

            base.DeactivateView(viewName);
        }

        protected override void Add()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (!UserCanAdd)
            {
                ErrorWindow.CreateNew(Resource.BaseProtecaEntityViewModel_ActionNotAllowed);
            }
            else if (this.SelectedEntity != null)
            {
                if (this.SelectedEntity.IsPP)
                {
                    ((Pp)this.SelectedEntity.Ouvrage).Visites.Add(new Visite()
                    {
                        DateSaisie = DateTime.Now,
                        DateVisite = DateTime.Now,
                        CleUtilisateurCreation = this.CurrentUser.CleUtilisateur,
                        EnumTypeEval = this.TypeEval.HasValue ? this.TypeEval.Value : 0,
                        EnumTypeEvalComposition = this.SelectedEntity.EnumTypeEval,
                        EstValidee = false
                    });

                    (this.SelectedEntity.Ouvrage as Pp).PropertyChanged -= PpToVisiteMesures_PropertyChanged;
                    (this.SelectedEntity.Ouvrage as Pp).PropertyChanged += PpToVisiteMesures_PropertyChanged;
                }
                else
                {
                    ((EqEquipement)this.SelectedEntity.Ouvrage).Visites.Add(new Visite()
                    {
                        DateSaisie = DateTime.Now,
                        DateVisite = DateTime.Now,
                        CleUtilisateurCreation = this.CurrentUser.CleUtilisateur,
                        EnumTypeEval = this.SelectedEntity.EnumTypeEval,
                        EnumTypeEvalComposition = this.SelectedEntity.EnumTypeEval,
                        EstValidee = false
                    });
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

                OldUser = null;

                this.IsEditMode = true;

                // Force l'évènement OnIsEditModeChanged du maincontainer pour faire le focus sur le premier élément
                HasChanged = true;

                RegisterPropertyChanged();

                this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");

                HasChanged = false;

                RefreshScreen();
            }
        }

        /// <summary>
        /// Lancement de la recherche
        /// </summary>
        protected override void Find()
        {
            if (!DateDebut.HasValue || !DateFin.HasValue)
            {
                ErrorWindow.CreateNew(Resource.SaisieVisite_SearchEmpty.ToString());
            }
            else if (DateDebut.Value > DateFin.Value)
            {
                ErrorWindow.CreateNew(Resource.SaisieVisite_SearchErrorDate.ToString());
            }
            else
            {
                // désactivation de la navigation automatique
                IsAutoNavigateToFirst = false;

                IsBusy = true;

                this.service.Entities.Clear();
                FiltreCleTournee = null;
                if (DateDebut.HasValue)
                {
                    DateDebut = DateDebut.Value.Date;
                }

                if (DateFin.HasValue)
                {
                    DateFin = DateFin.Value.Date.AddDays(1).AddSeconds(-1);
                }

                // Sauvegarde de la date de recherche :
                SavedDateDebut = DateDebut;
                SavedDateFin = DateFin;

                if (!SavedDateFin.HasValue)
                {
                    SavedDateFin = DateTime.Now.Date.AddDays(1);
                }

                saveGeoPreferences();
                (this.service as TourneePpEqService).FindTourneePpEqByCriterias(FiltreCleRegion, FiltreCleAgence, FiltreCleSecteur, FiltreCleEnsElec, FiltreClePortion, FiltreRechercheLibelle, SearchDone);
            }


        }

        /// <summary>
        /// Surcharge de la navigation vers un ID en particulier (pour la gestion du filtre)
        /// </summary>
        /// <param name="id"></param>
        protected override void NavigateToId(int id, bool forceReload)
        {
            TourneePpEq tournee = this.Entities.FirstOrDefault(t => t.Id == id);
            if (tournee != null)
            {
                if (tournee.ClePp.HasValue)
                {
                    ((NavigationService)NavigationService).Navigate(tournee.ClePp.Value, FiltreNavigation.PP.GetStringValue());
                }
                else
                {
                    ((NavigationService)NavigationService).Navigate(tournee.CleEquipement.Value, FiltreNavigation.EQ.GetStringValue());
                }
            }
        }

        protected override void Save()
        {
            if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null && this.SelectedEntity.Ouvrage.LastVisite != null)
            {
                // Gestion des Visites sans type d'évaluation 
                if (this.SelectedEntity.IsPP
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

                if (this.SelectedEntity.IsPP && (this.SelectedEntity.Ouvrage as Pp).HasChanges)
                {
                    // Si la fiabilisation n'est pas active, on supprime les champs liés
                    if (!(this.SelectedEntity.Ouvrage as Pp).DdeDeverrouillageCoordGps)
                    {
                        (this.SelectedEntity.Ouvrage as Pp).UsrUtilisateur1 = null;
                        (this.SelectedEntity.Ouvrage as Pp).DateDdeDeverrouillageCoordGps = null;
                    }

                    this.LogPp();
                }

                //Alertes Utilisateurs
                if (this.AlerteDeclenchee && this.SelectedEntity.Ouvrage.LastVisite.Alerte == null && this.SelectedEntity.Ouvrage.LastVisite.DateVisite.HasValue)
                {
                    Alerte alerte = new Alerte()
                    {
                        Supprime = false,
                        Date = this.SelectedEntity.Ouvrage.LastVisite.DateVisite.Value,
                        RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "U")
                    };

                    this.SelectedEntity.Ouvrage.LastVisite.Alertes.Add(alerte);
                }
                else if (!this.AlerteDeclenchee && this.SelectedEntity.Ouvrage.LastVisite.Alerte != null)
                {
                    ServiceAlerte.Delete(this.SelectedEntity.Ouvrage.LastVisite.Alerte);
                }

                //Alertes Seuil
                if(this.SelectedEntity.Ouvrage.LastVisite.DateVisite.HasValue)
                {
                    List<MesMesure> mesuresToFlag = this.SelectedEntity.Ouvrage.LastVisite.MesMesure.Where(m => m.IsDepassementSeuil && m.Alerte == null).ToList();
                    foreach (MesMesure mes in mesuresToFlag)
                    {
                        Alerte alerte = new Alerte()
                        {
                            Supprime = false,
                            Date = this.SelectedEntity.Ouvrage.LastVisite.DateVisite.Value,
                            RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "S")
                        };

                        this.SelectedEntity.Ouvrage.LastVisite.Alertes.Add(alerte);
                        mes.Alertes.Add(alerte);
                    }
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
                else if (this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur != null 
                        && this.SelectedEntity.Ouvrage.LastVisite.Analyse.RefEnumValeur.Valeur != "01" 
                        && this.SelectedEntity.Ouvrage.LastVisite.Analyse.Alerte == null 
                        && this.SelectedEntity.Ouvrage.LastVisite.DateVisite.HasValue)
                {
                    Alerte alerte = new Alerte()
                    {
                        Supprime = false,
                        Date = this.SelectedEntity.Ouvrage.LastVisite.DateVisite.Value,
                        RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "A")
                    };

                    this.SelectedEntity.Ouvrage.LastVisite.Alertes.Add(alerte);
                    this.SelectedEntity.Ouvrage.LastVisite.Analyse.Alertes.Add(alerte);
                }


                //UsrUtilisateur à supprimer
                List<UsrUtilisateur> users = ServiceUtilisateur.Entities.Where(u => u.IsNew() && u != this.SelectedEntity.Ouvrage.LastVisite.UsrUtilisateur2).ToList();
                foreach (UsrUtilisateur user in users)
                {
                    ((UsrUtilisateurService)ServiceUtilisateur).Delete(user);
                }

                this.SelectedEntity.Ouvrage.LastVisite.CleUtilisateurValidation = this.CurrentUser.CleUtilisateur;
                this.SelectedEntity.Ouvrage.LastVisite.DateValidation = DateTime.Now;
                this.SelectedEntity.Ouvrage.LastVisite.EstValidee = true;



                //Recopie de la Visite sur les Pp jumelees à celle-ci si cette une Pp et que l'on est en création de visite
                CreateVisitesForPpJumelees();
            }
            base.Save();
        }

        protected override void Delete()
        {
            base.Delete(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Ajout d'un enregistrement dans logOuvrage
        /// </summary>
        public void LogPp()
        {
            // Instanciation des propriétés
            EntityCollection<LogOuvrage> LogOuvrageList = null;
            LogOuvrage _logAajouter;

            // Instanciation du resource manager
            ResourceManager resourceManager = ResourceHisto.ResourceManager;

            LogOuvrageList = (this.SelectedEntity.Ouvrage as Pp).LogOuvrage;

            // Suppression des logs existant
            if (LogOuvrageList != null && LogOuvrageList.Any(lo => lo.IsNew()))
            {
                foreach (LogOuvrage log in LogOuvrageList.Where(lo => lo.IsNew()))
                {
                    LogOuvrageList.Remove(log);
                    ServiceLogOuvrage.Delete(log);
                }
                _logAajouter = null;
            }

            // Instanciation du log ouvrage
            _logAajouter = new LogOuvrage
            {
                CleUtilisateur = this.CurrentUser.CleUtilisateur,
                RefEnumValeur = ServiceEnumValeur.Entities.Where(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_LOG_OUVRAGE.GetStringValue() && r.Valeur == "M").FirstOrDefault(),
                DateHistorisation = DateTime.Now
            };

            // En cas de changement du sélected entity, on log l'enregistrement
            if ((this.SelectedEntity.Ouvrage as Pp).HasChanges)
            {
                string Modifiedproperties = null;

                Entity original = (this.SelectedEntity.Ouvrage as Pp).GetOriginal();
                if (original == null)
                {
                    original = this.SelectedEntity.Ouvrage as Pp;
                }
                List<string> elements = new List<string>();

                foreach (PropertyInfo p in (this.SelectedEntity.Ouvrage as Pp).GetType().GetProperties())
                {
                    // Gestion des propriétés Nullable définies coté Silverlight
                    if (p.Name.EndsWith("Nullable"))
                    {
                        continue;
                    }

                    //récupération de la valeur à afficher. Si pas de valeurs on prend le nom de la propriété
                    string propertyName = resourceManager.GetString(p.Name) == null ? p.Name : resourceManager.GetString(p.Name);

                    if (String.IsNullOrEmpty(propertyName))
                    {
                        continue;
                    }

                    if (p.CanWrite && !(p.PropertyType.BaseType == typeof(Entity)))
                    {
                        Object originalValue = p.GetValue(original, null);
                        Object newValue = p.GetValue((this.SelectedEntity.Ouvrage as Pp), null);
                        if ((originalValue == null && newValue == null) || (originalValue != null && originalValue.Equals(newValue)))
                        {
                            continue;
                        }
                        else
                        {
                            Modifiedproperties += Modifiedproperties == null ? propertyName : " / " + propertyName;
                        }
                    }
                }

                _logAajouter.ListeChamps = Modifiedproperties;

                // On ajoute le log au contexte
                LogOuvrageList.Add(_logAajouter);
            }
        }

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

                if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null && this.SelectedEntity.Ouvrage.LastVisite != null)
                {
                    this.SelectedEntity.Ouvrage.LastVisite.UsrUtilisateur2 = newUser;
                }
            }
        }

        #endregion

        #region Private Methods

        


        /// <summary>
        /// Creation des visites, copies de la visite nouvellement créée, sur les Pp jumelées de cette Pp
        /// </summary>
        private void CreateVisitesForPpJumelees()
        {
            if (this.SelectedEntity.IsPP
                && this.SelectedEntity.Ouvrage.LastVisite.IsNew()
                && this.SelectedEntity.Ouvrage.LastVisite.EnumTypeEval != 0
                && !this.VisitesForPpJumelee.Any())
            {
                //Récupération des Pp qui sont jumelees à cette Pp
                List<int> MesClesPpJumelees = (this.SelectedEntity.Ouvrage as Pp).PpJumelee.Select(pj => pj.PpClePp).Union((this.SelectedEntity.Ouvrage as Pp).PpJumelee1.Select(pj => pj.ClePp)).ToList();

                foreach (int clePpJumelee in MesClesPpJumelees)
                {
                    Pp PPjumelé = this.SelectedEntity.Ouvrage.PpAttachee.PpJumelees.FirstOrDefault(pp => pp.ClePp == clePpJumelee);
                   if (PPjumelé != null)
                   {
                       //Création de ma copie de Visite
                       Visite visiteCopy = new Visite()
                       {
                           CleUtilisateurValidation = this.CurrentUser.CleUtilisateur,
                           CleUtilisateurCreation = this.CurrentUser.CleUtilisateur,
                           CleUtilisateurMesure = this.SelectedEntity.Ouvrage.LastVisite.CleUtilisateurMesure,
                           DateValidation = this.SelectedEntity.Ouvrage.LastVisite.DateValidation,
                           DateSaisie = this.SelectedEntity.Ouvrage.LastVisite.DateSaisie,
                           DateVisite = this.SelectedEntity.Ouvrage.LastVisite.DateVisite,
                           RelevePartiel = this.SelectedEntity.Ouvrage.LastVisite.RelevePartiel,
                           EstValidee = true,
                           EnumTypeEval = this.SelectedEntity.Ouvrage.LastVisite.EnumTypeEval,
                           EnumTypeEvalComposition = this.SelectedEntity.Ouvrage.LastVisite.EnumTypeEvalComposition,
                           Commentaire = this.SelectedEntity.Ouvrage.LastVisite.Commentaire
                       };

                       //Copie de l'alertes
                       if (this.SelectedEntity.Ouvrage.LastVisite.Alerte != null)
                       {
                           Alerte alerte = new Alerte()
                           {
                               Supprime = false,
                               Date = this.SelectedEntity.Ouvrage.LastVisite.Alerte.Date,
                               RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "U")
                           };

                           visiteCopy.Alertes.Add(alerte);
                       }

                       //Copie des InstrumentsUtilises
                       foreach (InstrumentsUtilises instrumentUtiliseOrigin in this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises)
                       {
                           visiteCopy.InstrumentsUtilises.Add(new InstrumentsUtilises()
                           {
                               CleInstrument = instrumentUtiliseOrigin.CleInstrument
                           });
                       }

                       //Copie des Mesures
                       foreach (MesMesure mesureOrigin in this.SelectedEntity.Ouvrage.LastVisite.MesMesure)
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
                                   RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "S")
                               };

                               visiteCopy.Alertes.Add(alerte);
                               mesureCopy.Alertes.Add(alerte);
                           }

                           visiteCopy.MesMesure.Add(mesureCopy);
                       }

                       //Copie de l'analyse
                       if (this.SelectedEntity.Ouvrage.LastVisite.AnAnalyseSerieMesure.Any())
                       {
                           AnAnalyseSerieMesure analyseCopy = new AnAnalyseSerieMesure()
                           {
                               CleUtilisateur = this.CurrentUser.CleUtilisateur,
                               DateAnalyse = this.SelectedEntity.Ouvrage.LastVisite.Analyse.DateAnalyse,
                               EnumEtatPc = this.SelectedEntity.Ouvrage.LastVisite.Analyse.EnumEtatPc,
                               Commentaire = this.SelectedEntity.Ouvrage.LastVisite.Analyse.Commentaire
                           };

                           if (this.SelectedEntity.Ouvrage.LastVisite.Analyse.Alerte != null)
                           {
                               Alerte alerte = new Alerte()
                               {
                                   Supprime = false,
                                   Date = this.SelectedEntity.Ouvrage.LastVisite.Analyse.Alerte.Date,
                                   RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "A")
                               };

                               visiteCopy.Alertes.Add(alerte);
                               analyseCopy.Alertes.Add(alerte);
                           }

                           visiteCopy.AnAnalyseSerieMesure.Add(analyseCopy);
                       }

                       this.VisitesForPpJumelee.Add(visiteCopy);
                       PPjumelé.Visites.Add(visiteCopy);
                   }
                }
            }
        }

        /// <summary>
        /// Supression des Visites créées sur les Pp Jumelées à la sauvegarde 
        /// </summary>
        /// <param name="andInDomainContext">Suppression complète des visite du domain context</param>
        private void DeleteVisitesForPpJumelee(bool andInDomainContext)
        {
            if (andInDomainContext)
            {
                foreach (Visite visiteCopy in this.VisitesForPpJumelee)
                {
                    foreach (Alerte alerteCopy in visiteCopy.Alertes)
                    {
                        this.ServiceAlerte.Delete(alerteCopy);
                    }

                    foreach (MesMesure mesureCopy in visiteCopy.MesMesure)
                    {
                        this.ServiceMesMesure.Delete(mesureCopy);
                    }

                    foreach (AnAnalyseSerieMesure analyseCopy in visiteCopy.AnAnalyseSerieMesure)
                    {
                        this.ServiceAnAnalyseSerieMesure.Delete(analyseCopy);
                    }

                    foreach (InstrumentsUtilises instrumentUtiliseCopy in visiteCopy.InstrumentsUtilises)
                    {
                        (this.ServiceInstrument as InsInstrumentService).DeleteInstrumentUtilise(instrumentUtiliseCopy);
                    }

                    this.ServiceVisite.Delete(visiteCopy);
                }
            }
            this.VisitesForPpJumelee.Clear();
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
            if (this.IsEditMesureMode && this.SelectedEntity.Ouvrage is Pp
                && (e.PropertyName == "CourantsAlternatifsInduits"
                || e.PropertyName == "CourantsVagabonds"
                || e.PropertyName == "ElectrodeEnterreeAmovible"
                || e.PropertyName == "PresenceDUneTelemesure"
                || e.PropertyName == "TemoinMetalliqueDeSurface"
                || e.PropertyName == "TemoinEnterreAmovible"
                || e.PropertyName == "CleCategoriePp"
                || e.PropertyName == "CleNiveauSensibilite"))
            {
                if (e.PropertyName == "CleCategoriePp" || e.PropertyName == "CleNiveauSensibilite")
                {
                    this.SelectedEntity.Ouvrage.LastVisite.EnumTypeEval = this.TypeEval.HasValue ? this.TypeEval.Value : 0;
                    RaisePropertyChanged(() => this.SelectedEntity.Ouvrage.LastVisite.EnumTypeEval);
                }

                //Backup des VisiteMesures
                BackupVisiteMesures = new List<VisiteMesure>(this.SelectedEntity.Ouvrage.LastVisite.VisiteMesures);
                BackupVisiteMesuresComplementaires = new List<VisiteMesure>(this.SelectedEntity.Ouvrage.LastVisite.VisiteMesuresComplementaires);

                //Reload des VisiteMesures
                this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged);

                VisiteMesure temp = null;
                DisableGestionUAlt = true;

                //Import des valeurs rentrées dans les backup vers les nouvelles
                foreach (VisiteMesure vm in this.SelectedEntity.Ouvrage.LastVisite.VisiteMesures)
                {
                    //On récupère dans le Backup la VisiteMesure correspondant à la nouvelle en cours
                    temp = BackupVisiteMesures.FirstOrDefault(v => v.Libelle == vm.Libelle && (v.Maxi != null && v.Maxi.Valeur.HasValue || v.Moyen != null && v.Moyen.Valeur.HasValue || v.Mini != null && v.Mini.Valeur.HasValue));
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
                    temp = BackupVisiteMesuresComplementaires.FirstOrDefault(v => v.Libelle == vm.Libelle && (v.Maxi.Valeur.HasValue || v.Moyen.Valeur.HasValue || v.Mini.Valeur.HasValue));
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

                DisableGestionUAlt = false;

                //Rafraichissement de la vue pour afficher les changements
                RaisePropertyChanged(() => this.SelectedEntity);

                RaisePropertyChanged(() => this.SelectedItem);
                RaisePropertyChanged(() => this.PortionsEnsElec);
                RaisePropertyChanged(() => this.Equipements);
                RaisePropertyChanged(() => this.PPwithPKLibelle);
                RaisePropertyChanged(() => this.PortionPpAttacheeLibelle);
                RaisePropertyChanged(() => this.SecteurEnsElecLibelle);
                RaisePropertyChanged(() => this.TypeEval);
                RaisePropertyChanged(() => this.ListTypeEval);
                RaisePropertyChanged(() => this.TypeEvalLibelle);
            }
        }
        /// <summary>
        /// Abonnement à propertyChanged pour la gestion des coordonnées GPS et du niveau de sensibilité
        /// </summary>
        private void RegisterPropertyChanged()
        {
            if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null && this.SelectedEntity.Ouvrage.LastVisite != null)
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
                        //this.AlerteDeclenchee = this.SelectedEntity.Ouvrage.LastVisite.RelevePartiel || this.SelectedEntity.Ouvrage.LastVisite.CleAlerte.HasValue;
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
            GraphEon.SourceMesures = (this.service as TourneePpEqService).GraphiqueEntities.Where(g => g.CLE_GRAPHIQUE == EnumGraphique.FirstOrDefault(v => v.Valeur == "Eon").CleEnumValeur).OrderBy(g => g.PK).ToList();
            GraphEon.TitreY = EnumGraphique.FirstOrDefault(v => v.Valeur == "Eon").Libelle;
            GraphEoff.SourceMesures = (this.service as TourneePpEqService).GraphiqueEntities.Where(g => g.CLE_GRAPHIQUE == EnumGraphique.FirstOrDefault(v => v.Valeur == "Eoff").CleEnumValeur).OrderBy(g => g.PK).ToList();
            GraphEoff.TitreY = EnumGraphique.FirstOrDefault(v => v.Valeur == "Eoff").Libelle;
            GraphIdc.SourceMesures = (this.service as TourneePpEqService).GraphiqueEntities.Where(g => g.CLE_GRAPHIQUE == EnumGraphique.FirstOrDefault(v => v.Valeur == "Itm ~").CleEnumValeur).OrderBy(g => g.PK).ToList();
            GraphIdc.TitreY = EnumGraphique.FirstOrDefault(v => v.Valeur == "Itm ~").Libelle;
            GraphUalt.SourceMesures = (this.service as TourneePpEqService).GraphiqueEntities.Where(g => g.CLE_GRAPHIQUE == EnumGraphique.FirstOrDefault(v => v.Valeur == "Ucana ~").CleEnumValeur).OrderBy(g => g.PK).ToList();
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

            CustomExpanderViewModel expanderVM = Router.ResolveViewModel<CustomExpanderViewModel>(false, "CustomExpander");
            expanderVM.IsEnable = IsNavigationEnabled;
        }

        /// <summary>
        /// Ajout d'un instrument à la liste de la visite
        /// </summary>
        private void AddInstrument()
        {
            if (this.SelectedEntity != null && this.FiltreCleInstrument.HasValue)
            {
                this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.Add(new InstrumentsUtilises()
                {
                    CleInstrument = this.FiltreCleInstrument.Value,
                    CleVisite = this.SelectedEntity.Ouvrage.LastVisite.CleVisite
                });

                this.FiltreCleInstrument = null;
                RaisePropertyChanged(() => this.FiltreCleInstrument);
                RaisePropertyChanged(() => this.ListInstruments);
            }
        }

        /// <summary>
        /// Suppression d'un instrument utilisé
        /// </summary>
        private void RemoveInstrument(object obj)
        {
            int cle;
            if (int.TryParse(obj.ToString(), out cle))
            {
                InstrumentsUtilises inst = this.SelectedEntity.Ouvrage.LastVisite.InstrumentsUtilises.FirstOrDefault(i => i.CleInsUtilises == cle);
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
            EventAggregator.Publish("Visite_DetailPP".AsViewNavigationArgs().AddNamedParameter("IsLightVersion", true).AddNamedParameter("SelectedEntity", this.SelectedEntity != null ? this.SelectedEntity.Ouvrage : null));

            
            this.FiltreCleInstrument = null;

            if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null)
            {
                this.SelectedEntity.Ouvrage.VisitePeriodeDebut = SavedDateDebut;
                this.SelectedEntity.Ouvrage.VisitePeriodeFin = SavedDateFin;
            }
            
            RaisePropertyChanged(() => this.SelectedEntity);
            RaisePropertyChanged(() => this.ListInstruments);
            RaisePropertyChanged(() => this.ListUtilisateurs);
            RaisePropertyChanged(() => this.IsEditMesureMode);
            RaisePropertyChanged(() => this.AlerteEnable);

            RaisePropertyChanged(() => this.IsPpNonEditMode);
            RaisePropertyChanged(() => this.EtatPC);
        }

        /// <summary>
        /// La recherche des tournee est terminée
        /// </summary>
        private void SearchEntityEquipement(Exception error)
        {
            if (this.SelectedEntity != null)
            {
                //Recupération de l'ouvrage
                if (this.SelectedEntity.IsPP)
                {
                    this.SelectedEntity.Ouvrage = ServicePp.DetailEntity;
                }
                else
                {
                    this.SelectedEntity.Ouvrage = ServiceEquipement.DetailEntity;
                }

                this.SelectedEntity.Ouvrage.VisitePeriodeDebut = SavedDateDebut;
                this.SelectedEntity.Ouvrage.VisitePeriodeFin = SavedDateFin;

                //Création de la visite si non chargée
                if (this.SelectedEntity.Ouvrage.LastVisite == null)
                {
                    Add();
                }
                //Sinon chargement des éléments necéssaires à l'affichage
                else
                {
                    if (this.SelectedEntity.IsPP)
                    {
                        (this.SelectedEntity.Ouvrage as Pp).PropertyChanged -= PpToVisiteMesures_PropertyChanged;
                        (this.SelectedEntity.Ouvrage as Pp).PropertyChanged += PpToVisiteMesures_PropertyChanged;
                    }

                    // Force le passage en consultation car on est sur une Visite existante
                    IsEditMode = false;

                    this.SelectedEntity.Ouvrage.LastVisite.LoadVisiteMesures(ServiceMesClassementMesure.Entities, this.SetPropertyChanged);
                    AlerteDeclenchee = this.SelectedEntity.Ouvrage.LastVisite.Alerte != null;
                    RegisterPropertyChanged();
                    this.SelectedEntity.Ouvrage.ForceRaisePropertyChanged("LastVisite");
                    OldUser = this.SelectedEntity.Ouvrage.LastVisite.UsrUtilisateur2;
                    RefreshScreen();
                }

                RaisePropertyChanged(() => this.SelectedEntity.Ouvrage);
                RaisePropertyChanged(() => this.PortionsEnsElec);
                RaisePropertyChanged(() => this.Equipements);
                RaisePropertyChanged(() => this.PPwithPKLibelle);
                RaisePropertyChanged(() => this.PortionPpAttacheeLibelle);
                RaisePropertyChanged(() => this.SecteurEnsElecLibelle);

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
                && this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null && this.SelectedEntity.IsPP
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
            RaisePropertyChanged(() => this.ListUtilisateurs);
            RaisePropertyChanged(() => this.SelectedEntity);
         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        private void SearchDone(Exception error)
        {

            NavigationService.NavigateRootUrl();

            HasChanged = false;

            this.IsBusy = false;
            this.IsEditMode = false;
            this.SelectedEntity = null;
            RaisePropertyChanged(() => this.ResRecherche);
            RaisePropertyChanged(() => this.Tournees);
        }

        /// <summary>
        /// Affichage de la popup de création d'un utilisateur
        /// </summary>
        private void ShowDialog()
        {
            ChildWindow.Title = "Création d'un utilisateur externe";
            ChildWindow.Show();
            EventAggregator.Publish("CreateUsrUtilisateur".AsViewNavigationArgs());
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

        #endregion

        #region Autorisations

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'ajout d'une visite
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanAdd()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droist de l'utilisateur courant 
        /// sur la suppression d'une visite
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanDelete()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur l'édition d'une visite
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanEdit()
        {
            return GetAutorisation();
        }

        /// <summary>
        /// Détermine les droits de l'utilisateur courant 
        /// sur la sauvegarde d'une visite
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanSave()
        {
            return this.SelectedItem != null && (this.SelectedEntity.IsNew() || GetAutorisation());
        }

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            if (this.CurrentUser != null && this.SelectedItem != null && this.SelectedItem.Ouvrage != null)
            {
                Pp pp = SelectedItem.IsPP ? (Pp)SelectedItem.Ouvrage : ((EqEquipement)SelectedItem.Ouvrage).Pp;
                if (pp != null)
                {

                    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.CREA_VISITE_NIV);
                    string codePortee = role.RefUsrPortee.CodePortee;

                    if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                    {
                        return pp.GeoSecteur.CleAgence == CurrentUser.CleAgence;
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                        codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                    {
                        return true;
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                    {
                        return false;
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                    {
                        return pp.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion;
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                    {
                        return pp.GeoSecteur.CleSecteur == CurrentUser.CleSecteur;
                    }
                }
            }
            return false;
        }

        #endregion Autorisations
    }
}
