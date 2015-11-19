using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using Jounce.Framework;
using Jounce.Framework.Command;
using Jounce.Core.Event;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Core.Application;
using Proteca.Web.Models;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Services;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Silverlight.Views.Windows;
using Telerik.Windows.Controls;
using System.ServiceModel.DomainServices.Client;
using System.Resources;
using Proteca.Web.Resources;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for EditionVisite entity
    /// </summary>
    [ExportAsViewModel("EditionVisite")]
    public class EditionVisiteViewModel : BaseProtecaEntityViewModel<Visite>, IEventSink<UsrUtilisateur>
    {

        #region Services

        /// <summary>
        /// Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;

        /// <summary>
        /// Service utilisé pour récupérer l'entité de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> ServiceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour récupérer les GEO ensembles électriques / portions
        /// </summary>
        [Import]
        public IEntityService<GeoEnsElecPortion> ServiceGeoEnsElecPortion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les alertes
        /// </summary>
        [Import]
        public IEntityService<Alerte> ServiceAlerte { get; set; }

        /// <summary>
        /// Service utilisé pour récupérer les RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceRefEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour récupérer les TypeEquipement
        /// </summary>
        [Import]
        public IEntityService<TypeEquipement> ServiceTypeEquipement { get; set; }

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
        /// Service pour récupérer les RefUsrPortee
        /// </summary>
        [Import]
        public IEntityService<RefUsrPortee> ServiceRefUsrPortee { get; set; }

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
        /// Service pour récupérer les Paramètres
        /// </summary>
        [Import]
        public IEntityService<RefParametre> ServiceRefParametre { get; set; }

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
        /// Service pour récupérer l'objet TourneePpEq
        /// </summary>
        [Import]
        public IEntityService<TourneePpEq> ServiceTourneePpEq { get; set; }


        #endregion Services

        //#region Enums
        ///// <summary>
        /////     Déclaration de l'énum permettant d'afficher les types de statuts en base
        ///// </summary>
        //private string enumTypeEval = RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue();

        //#endregion

        #region Properties

        public DateTime TodayDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        private TourneePpEq _selectedTourneePpEq;
        public TourneePpEq SelectedTourneePpEq
        {
            get
            {
                return _selectedTourneePpEq;
            }
            set
            {
                _selectedTourneePpEq = value;
                RaisePropertyChanged(() => this.SelectedTourneePpEq);
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

                    IOuvrage ouvrage = SelectedEntity.Ouvrage;
                    if (ouvrage != null)
                    {
                        if (ouvrage.CodeEquipement.Equals("PP"))
                        {
                            libelleSecteur = (SelectedEntity.Ouvrage as Pp).Secteur;
                            libelleEnsElec = (SelectedEntity.Ouvrage as Pp).EnsElec;
                        }
                        else
                        {
                            if (ouvrage.PpAttachee != null)
                            {
                                libelleSecteur = (SelectedEntity.Ouvrage as EqEquipement).PpAttachee.Secteur;
                                libelleEnsElec = (SelectedEntity.Ouvrage as EqEquipement).PpAttachee.EnsElec;
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
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue())
                                .OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        #region Override Properties

        /// <summary>
        /// Surcharge des entities pour rajout d'un filtre
        /// </summary>
        public override ObservableCollection<Visite> Entities
        {
            get
            {
                if (this.FiltreOuvrage != null && this.service.Entities != null && this.service.Entities.Any())
                {
                    if (this.FiltreOuvrage is Pp)
                    {
                        return new ObservableCollection<Visite>(this.service.Entities
                            .Where(v => v.ClePp.HasValue && v.ClePp.Value == (this.FiltreOuvrage as Pp).ClePp)
                            .OrderByDescending(v => v.DateVisite)
                            .ThenByDescending(v => v.CleVisite));
                    }
                    else
                    {
                        return new ObservableCollection<Visite>(this.service.Entities
                            .Where(v => v.CleEquipement.HasValue && v.CleEquipement.Value == (this.FiltreOuvrage as EqEquipement).CleEquipement)
                            .OrderByDescending(v => v.DateVisite)
                            .ThenByDescending(v => v.CleVisite));
                    }
                }
                return new ObservableCollection<Visite>();
            }
        }

        #endregion Override Properties

        #region Expander Properties

        /// <summary>
        /// Définition de l'élément de filtre IncludeDeleted
        /// </summary>
        private bool _includeDeleted = false;

        /// <summary>
        /// Retourne l'état du filtre IncludeDeleted
        /// </summary>
        public bool IncludeDeleted
        {
            get { return _includeDeleted; }
            set { _includeDeleted = value; }
        }

        /// <summary>
        /// Définition de la clé FiltreCleEnsElec
        /// </summary>
        private int? _filtreCleEnsElec;

        /// <summary>
        /// Retourne la clé de l'ensemble électrique
        /// </summary>
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
        /// Definition du filtre sur le Pk minimum
        /// </summary>
        public decimal? PkMin { get; set; }

        /// <summary>
        /// Definition du filtre sur le Pk maximum
        /// </summary>
        public decimal? PkMax { get; set; }

        /// <summary>
        /// Définition du filtre sur le type d'équipement
        /// </summary>
        public String FiltreCodeTypeEq { get; set; }

        /// <summary>
        /// Définition du IOuvrage
        /// </summary>
        private IOuvrage _filtreOuvrage;

        /// <summary>
        /// Retourne le IOuvrage sélectionné
        /// </summary>
        public IOuvrage FiltreOuvrage
        {
            get
            {
                //if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null && _filtreOuvrage == null)
                //{
                //    FiltreOuvrage = this.SelectedEntity.Ouvrage;
                //}
                return _filtreOuvrage;
            }
            set
            {
                if (_filtreOuvrage != value)
                {
                    _filtreOuvrage = value;
                    RaisePropertyChanged(() => this.FiltreOuvrage);
                    RaisePropertyChanged(() => this.Entities);
                    if (this.Entities != null && this.Entities.Any())
                    {
                        NavigateToId(this.Entities.First().CleVisite);
                        RaisePropertyChanged(() => this.SelectedId);
                    }
                    RaisePropertyChanged(() => this.ResultIndicator);
                    RaisePropertyChanged(() => this.EntitiesCount);
                }
            }
        }

        /// <summary>
        /// Liste des régions
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return ServiceRegion.Entities; }
        }

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

        /// <summary>
        /// Retourne les types d'équipement
        /// </summary>
        public List<TypeEquipement> ListTypeEq
        {
            get
            {
                return this.ServiceTypeEquipement.Entities.OrderBy(te => te.NumeroOrdre).ToList();
            }
        }

        /// <summary>
        /// Retourne les ouvrages disponibles dans les visites retournées
        /// </summary>
        public List<IOuvrage> ListOuvrages
        {
            get
            {
                if (this.service.Entities != null && this.service.Entities.Any())
                {
                    return ((this.service.Entities.Where(v => v.ClePp.HasValue).Select(v => v.Pp).OrderBy(p => p.Libelle).Cast<IOuvrage>())
                        .Union(this.service.Entities.Where(v => v.CleEquipement.HasValue).Select(v => v.EqEquipement).OrderBy(e => e.TypeEquipement.NumeroOrdre).ThenBy(e => e.Libelle).Cast<IOuvrage>())).ToList();
                }
                return new List<IOuvrage>();
            }
        }

        /// <summary>
        /// Retourne une string d'affichage pour le resultat
        /// </summary>
        public String ResRecherche
        {
            get
            {
                return String.Format(Resource.EditionVisite_ResRecherche, this.ListOuvrages.Count, (this.ListOuvrages.Count > 1) ? "s" : String.Empty);
            }
        }

        #endregion Expander Properties

        #region Screen Properties

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
                if (this.SelectedEntity != null)
                {
                    int evalEG = ServiceRefEnumValeur.Entities.Where(ev => ev.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue() && ev.Valeur == "1").Select(ev => ev.CleEnumValeur).FirstOrDefault();
                    int evalECD = ServiceRefEnumValeur.Entities.Where(ev => ev.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue() && ev.Valeur == "2").Select(ev => ev.CleEnumValeur).FirstOrDefault();

                    int typeEval = this.SelectedEntity.EnumTypeEval;

                    // Si différent de EG ou ECD => pas de vérification sur la sensibilité PP
                    if (typeEval != evalEG && typeEval != evalECD)
                    {
                        return typeEval;
                    }

                    if (this.SelectedEntity.Ouvrage != null
                        && this.SelectedEntity.Ouvrage is Pp
                        && ((Pp)this.SelectedEntity.Ouvrage).RefNiveauSensibilitePp != null
                        && !((Pp)this.SelectedEntity.Ouvrage).RefNiveauSensibilitePp.EnumTypeEval.HasValue)
                    {
                        return null;
                    }
                    else if (this.SelectedEntity.Ouvrage != null
                        && this.SelectedEntity.Ouvrage is Pp
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
        /// MesClassementsMesure en fonction de l'état de IsMesureEditable
        /// </summary>
        public ObservableCollection<MesClassementMesure> ListClassementMesureForLoad
        {
            get
            {
                return (IsMesureEditable) ? this.ServiceMesClassementMesure.Entities : new ObservableCollection<MesClassementMesure>();
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

        /// <summary>
        /// Retourne le RefParametre du SeuilInferieurUCana
        /// </summary>
        public RefParametre SeuilInferieurUCana
        {
            get
            {
                return this.ServiceRefParametre.Entities.FirstOrDefault(p => p.Libelle == "SEUIL_INFERIEUR_UCANA");
            }
        }

        /// <summary>
        /// Retourne le RefParametre du délai d'édition des mesures
        /// </summary>
        public RefParametre DelaiEditionMesure
        {
            get
            {
                return this.ServiceRefParametre.Entities.FirstOrDefault(p => p.Libelle == "DELAI_EDITION_MESURE");
            }
        }

        /// <summary>
        /// Retourne si on doit avoir accès ou non en édition au mesures de la visite
        /// </summary>
        public Boolean IsMesureEditable
        {
            get
            {
                int delaiEditionMesure;
                if (this.DelaiEditionMesure != null
                    && int.TryParse(this.DelaiEditionMesure.Valeur, out delaiEditionMesure)
                    && this.SelectedEntity != null
                    && !this.SelectedEntity.DateImport.HasValue
                    && this.SelectedEntity.DateSaisie.HasValue)
                {
                    switch (this.DelaiEditionMesure.LibUnite)
                    {
                        case "Jours":
                            return this.SelectedEntity.DateSaisie.Value.AddDays(delaiEditionMesure).Date >= DateTime.Now.Date;
                        case "Heures":
                            return this.SelectedEntity.DateSaisie.Value.AddHours(delaiEditionMesure) >= DateTime.Now;
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

        /// <summary>
        /// Retourne pour le binding l'état d'edition des boxes
        /// </summary>
        public Boolean IsEditMesureMode
        {
            get
            {
                return IsEditMode && IsMesureEditable;
            }
        }

        /// <summary>
        /// Retourne l'état d'édition de la checkBox alerte déclenchée de la visite
        /// </summary>
        public bool AlerteEnable
        {
            get
            {
                return this.IsEditMode
                    && this.SelectedEntity != null
                    && !this.SelectedEntity.RelevePartiel;
            }
        }

        private bool _alerteDeclenchee = false;
        /// <summary>
        /// Retourne si il y'a une alerte déclenchée sur la visite
        /// </summary>
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

        private TileViewItemState _histoTileItemState = TileViewItemState.Minimized;
        /// <summary>
        /// Etat du TileViewItem Historique
        /// </summary>
        public TileViewItemState HistoTileItemState
        {
            get { return _histoTileItemState; }
            set
            {
                _histoTileItemState = value;
                RaisePropertyChanged(() => this.HistoTileItemState);
            }
        }

        private int? _filtreCleInstrument;
        /// <summary>
        /// Id de l'InsInstrument à ajouter à la visite
        /// </summary>
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

        /// <summary>
        /// Liste des InsInstrument disponible sur la GEO de la visite
        /// </summary>
        public List<InsInstrument> ListInstruments
        {
            get
            {
                if (this.SelectedEntity != null && this.IsEditMode)
                {
                    List<int> listInstruments = this.SelectedEntity.InstrumentsUtilises.Select(i => i.CleInstrument).ToList();
                    return ServiceInstrument.Entities.Where(i => !listInstruments.Contains(i.CleInstrument)).OrderBy(e => e.Libelle).ToList();
                }
                else
                {
                    return new List<InsInstrument>();
                }
            }
        }

        /// <summary>
        /// Ancien Utilisateur de la visite au chargement
        /// </summary>
        public UsrUtilisateur OldUser { get; set; }

        /// <summary>
        /// Liste des utilisateurs disponibles sur la GEO de la visite
        /// </summary>
        public List<UsrUtilisateur> ListUtilisateurs
        {
            get
            {
                if (this.SelectedEntity != null
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

        /// <summary>
        /// Retourne la liste des RefEnumValeur des Etat PC
        /// </summary>
        public ObservableCollection<RefEnumValeur> EtatPC
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(ServiceRefEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.AN_ETAT_PC.GetStringValue()).OrderBy(e => e.NumeroOrdre));
            }
        }

        /// <summary>
        /// Retourne la liste des RefEnumValeur des Type Alerte
        /// </summary>
        public List<RefEnumValeur> TypeAlerte
        {
            get
            {
                return ServiceRefEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.ENUM_TYPE_ALERTE.GetStringValue()).OrderBy(e => e.NumeroOrdre).ToList();
            }
        }

        #endregion Screen Properties

        #endregion Properties

        #region Constructeur

        public EditionVisiteViewModel()
            : base()
        {
            this.OnRegionSelected += (o, e) =>
            {
                this.FiltreCleEnsElec = null;
                this.FiltreClePortion = null;
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
                RaisePropertyChanged(() => this.FiltreClePortion);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

            this.OnAgenceSelected += (o, e) =>
            {
                this.FiltreCleEnsElec = null;
                this.FiltreClePortion = null;
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
                RaisePropertyChanged(() => this.FiltreClePortion);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

            this.OnSecteurSelected += (o, e) =>
            {
                this.FiltreCleEnsElec = null;
                this.FiltreClePortion = null;
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
                RaisePropertyChanged(() => this.FiltreClePortion);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
            };

            this.OnEntitiesLoaded += (o, e) =>
            {
                IsBusy = true;
                ((MesClassementMesureService)this.ServiceMesClassementMesure).GetMesClassementMesureWithMesNiveauProtection(LoadMesNiveauProtectionDone);

                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
                RaisePropertyChanged(() => this.ListTypeEq);
                RaisePropertyChanged(() => this.ListOuvrages);
                RaisePropertyChanged(() => this.IncludeDeleted);
                RaisePropertyChanged(() => this.FiltreCodeTypeEq);
                RaisePropertyChanged(() => this.PkMax);
                RaisePropertyChanged(() => this.PkMin);
                RaisePropertyChanged(() => this.ResRecherche);

                RaisePropertyChanged(() => this.SelectedTourneePpEq);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                RaisePropertyChanged(() => this.GeoEnsElecPortions);
                RaisePropertyChanged(() => this.PPwithPKLibelle);
                RaisePropertyChanged(() => this.PortionPpAttacheeLibelle);
                RaisePropertyChanged(() => this.SecteurEnsElecLibelle);
                RaisePropertyChanged(() => this.TypeEval);
                RaisePropertyChanged(() => this.ListTypeEval);
                RaisePropertyChanged(() => this.TypeEvalLibelle);
            };

            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("VisiteEdit_DetailPP".AsViewNavigationArgs().AddNamedParameter("IsLightVersion", true));
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", Resources.Resource.EditionVisite_ExpanderTitle));
                    EventAggregator.Publish("EditionVisite_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
            };

            this.OnDetailLoaded += (o, e) =>
            {
                if (this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null && this.SelectedEntity.PpReliee != null)
                {
                    this.FiltreOuvrage = this.SelectedEntity.Ouvrage;
                    ((InsInstrumentService)ServiceInstrument).FindInsInstrumentByGeoSecteur(this.SelectedEntity.PpReliee.CleSecteur, GetInstrumentDispoDone);
                    ((UsrUtilisateurService)ServiceUtilisateur).FindUsrUtilisateurByGeoSecteur(this.SelectedEntity.PpReliee.CleSecteur, GetUtilisateurDispoDone);
                    this.SelectedEntity.LoadVisiteMesures(ListClassementMesureForLoad, this.SetPropertyChanged);

                    string type = "PP";
                    int? cle = this.SelectedEntity.ClePp;
                    if (this.SelectedEntity.CleEquipement != null)
                    {
                        cle = this.SelectedEntity.CleEquipement;
                        type = "NonPP";
                    }

                    this.IsBusy = true;
                    ((TourneePpEqService)this.ServiceTourneePpEq).GetEntityByCle(cle.Value, type, LoadTourneePpEqDone);

                    AlerteDeclenchee = this.SelectedEntity.Alerte != null;
                    RegisterPropertyChanged();
                    OldUser = this.SelectedEntity.UsrUtilisateur2;

                    EventAggregator.Publish("VisiteEdit_DetailPP".AsViewNavigationArgs().AddNamedParameter("IsLightVersion", true).AddNamedParameter("SelectedEntity", this.SelectedEntity.Ouvrage));
                    if (this.SelectedEntity.Ouvrage is Pp)
                    {
                        (this.SelectedEntity.Ouvrage as Pp).PropertyChanged -= PpToVisiteMesures_PropertyChanged;
                        (this.SelectedEntity.Ouvrage as Pp).PropertyChanged += PpToVisiteMesures_PropertyChanged;
                    }

                    this.SelectedEntity.ForceRaisePropertyChanged("Analyse");
                    RaisePropertyChanged(() => this.AlerteEnable);
                    RaisePropertyChanged(() => this.SelectedEntity);
                    RaisePropertyChanged(() => this.ListInstruments);
                    RaisePropertyChanged(() => this.ListUtilisateurs);

                    RaisePropertyChanged(() => this.TypeEval);
                    RaisePropertyChanged(() => this.ListTypeEval);
                    RaisePropertyChanged(() => this.TypeEvalLibelle);

                }
            };



            this.OnViewModeChanged += (o, e) =>
            {
                RaisePropertyChanged(() => this.IsEditMesureMode);
                RaisePropertyChanged(() => this.AlerteEnable);
                RaisePropertyChanged(() => this.ListInstruments);
                if (IsEditMode
                    && this.SelectedEntity != null
                    && this.SelectedEntity.Analyse == null)
                {
                    this.SelectedEntity.AnAnalyseSerieMesure.Add(new AnAnalyseSerieMesure()
                    {
                        CleUtilisateur = this.CurrentUser.CleUtilisateur,
                        DateAnalyse = DateTime.Now,
                        EnumEtatPc = null
                    });
                    this.SelectedEntity.ForceRaisePropertyChanged("Analyse");
                }
            };

            this.OnSaveError += (o, e) =>
            {
                if (this.SelectedEntity != null)
                {
                    this.DisableGestionUAlt = true;
                    this.SelectedEntity.LoadVisiteMesures(ListClassementMesureForLoad, this.SetPropertyChanged);
                    if (this.SelectedEntity.Analyse == null)
                    {
                        this.SelectedEntity.AnAnalyseSerieMesure.Add(new AnAnalyseSerieMesure()
                        {
                            CleUtilisateur = this.CurrentUser.CleUtilisateur,
                            DateAnalyse = DateTime.Now,
                            EnumEtatPc = null
                        });
                    }
                    this.DisableGestionUAlt = false;
                }
                this.NotifyError = true;
                RaisePropertyChanged(() => this.SelectedEntity);
                this.SelectedEntity.ForceRaisePropertyChanged("Analyse");
            };

            this.OnCanceled += (o, e) =>
            {
                if (this.SelectedEntity != null)
                {
                    this.DisableGestionUAlt = true;
                    this.SelectedEntity.LoadVisiteMesures(ListClassementMesureForLoad, this.SetPropertyChanged);
                    AlerteDeclenchee = this.SelectedEntity.Alerte != null;
                    this.DisableGestionUAlt = false;
                }
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
        }

        #endregion Constructeur

        #region Methods

        #region Private Methods

        private void LoadTourneePpEqDone(Exception ex)
        {
            SelectedTourneePpEq = this.ServiceTourneePpEq.DetailEntity;

            RaisePropertyChanged(() => this.SelectedTourneePpEq);
            RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            RaisePropertyChanged(() => this.GeoEnsElecPortions);
            RaisePropertyChanged(() => this.PPwithPKLibelle);
            RaisePropertyChanged(() => this.PortionPpAttacheeLibelle);
            RaisePropertyChanged(() => this.SecteurEnsElecLibelle);

            this.IsBusy = false;
        }

        /// <summary>
        /// Récupération des modifications sur la Pp pour raffraichir les VisiteMesures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PpToVisiteMesures_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Abonnement aux propriétés qui doivent raffraichier les visites mesures à leur modification
            if (this.IsEditMesureMode && (e.PropertyName == "CourantsAlternatifsInduits"
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
                    this.SelectedEntity.EnumTypeEval = this.TypeEval.HasValue ? this.TypeEval.Value : 0;
                    RaisePropertyChanged(() => this.SelectedEntity.EnumTypeEval);
                }
                //Backup des VisiteMesures
                BackupVisiteMesures = new List<VisiteMesure>(this.SelectedEntity.VisiteMesures);
                BackupVisiteMesuresComplementaires = new List<VisiteMesure>(this.SelectedEntity.VisiteMesuresComplementaires);

                //Reload des VisiteMesures
                this.SelectedEntity.LoadVisiteMesures(ListClassementMesureForLoad, this.SetPropertyChanged);

                VisiteMesure temp = null;
                DisableGestionUAlt = true;

                //Import des valeurs rentrées dans les backup vers les nouvelles
                foreach (VisiteMesure vm in this.SelectedEntity.VisiteMesures)
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
                foreach (VisiteMesure vm in this.SelectedEntity.VisiteMesuresComplementaires)
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
                List<MesMesure> trash = (trashMaxi.Union(trashMoyen.Union(trashMini))).ToList();
                //On parcours la liste à revers pour supprimer ou mettre à jour les valeurs
                int count = this.SelectedEntity.MesMesure.Count;
                for (int i = count - 1; i > -1; i--)
                {
                    MesMesure mesureToDelete = this.SelectedEntity.MesMesure.ElementAt(i);
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
            }
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

        /// <summary>
        /// Abonnement à propertyChanged pour la gestion des coordonnées GPS et du niveau de sensibilité
        /// </summary>
        private void RegisterPropertyChanged()
        {
            if (this.SelectedEntity != null)
            {
                this.SelectedEntity.ActivateChangePropagation();
                this.SelectedEntity.PropertyChanged += (oo, ee) =>
                {
                    if (ee.PropertyName == "RelevePartiel")
                    {
                        RaisePropertyChanged(() => this.AlerteEnable);
                        if (this.SelectedEntity.RelevePartiel)
                        {
                            this.AlerteDeclenchee = true;
                        }
                    }
                };
            }
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
        /// Ajout d'un instrument à la liste de la visite
        /// </summary>
        private void AddInstrument()
        {
            if (this.SelectedEntity != null && this.FiltreCleInstrument.HasValue)
            {
                this.SelectedEntity.InstrumentsUtilises.Add(new InstrumentsUtilises()
                {
                    CleInstrument = this.FiltreCleInstrument.Value,
                    CleVisite = this.SelectedEntity.CleVisite
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
                InstrumentsUtilises inst = this.SelectedEntity.InstrumentsUtilises.FirstOrDefault(i => i.CleInsUtilises == cle);
                if (inst != null)
                {
                    this.SelectedEntity.InstrumentsUtilises.Remove(inst);
                    ((InsInstrumentService)ServiceInstrument).DeleteInstrumentUtilise(inst);
                }
            }

            RaisePropertyChanged(() => this.ListInstruments);
        }

        /// <summary>
        /// Gestion de l'affichage d'une popup d'information pour le cochage automatique 
        /// de courant alternatif induit au dépassement du niveau alternatif induit
        /// </summary>
        /// <param name="mesure"></param>
        private void SetPropertyChanged(MesMesure mesure)
        {
            decimal seuilInferieurUCana;
            if (this.SeuilInferieurUCana != null && decimal.TryParse(this.SeuilInferieurUCana.Valeur, out seuilInferieurUCana)
                && mesure.Valeur.HasValue && mesure.Valeur.Value > seuilInferieurUCana
                && this.SelectedEntity != null && this.SelectedEntity.Ouvrage != null && this.SelectedEntity.Ouvrage is Pp
                && !(this.SelectedEntity.Ouvrage as Pp).CourantsAlternatifsInduits
                && !DisableGestionUAlt)
            {
                (this.SelectedEntity.Ouvrage as Pp).CourantsAlternatifsInduits = true;
                MessageBox.Show(String.Format(Resource.SaisieVisiteChangePP_InformationBox, seuilInferieurUCana + " " + mesure.MesTypeMesure.MesModeleMesure.MesUnite.Symbole), "", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Le chargement des InsInstrument disponibles vient d'être effectué.
        /// </summary>
        /// <param name="error"></param>
        private void GetInstrumentDispoDone(Exception error)
        {
            RaisePropertyChanged(() => this.ListInstruments);
        }

        /// <summary>
        /// Le chargement des UsrUtilisateur disponibles vient d'être effectué.
        /// </summary>
        /// <param name="error"></param>
        private void GetUtilisateurDispoDone(Exception error)
        {
            RaisePropertyChanged(() => this.SelectedEntity);
        }

        /// <summary>
        /// RaisePropertyChanged des Expanders de la Visite
        /// </summary>
        private void RefreshVisiteTileExpanders()
        {
            if (MainTileItemState != TileViewItemState.Minimized)
            {
                RaisePropertyChanged(() => this.IsPPExpanded);
                RaisePropertyChanged(() => this.IsMesuresExpanded);
                RaisePropertyChanged(() => this.IsAnalyseExpanded);
            }
        }

        /// <summary>
        /// Remise à Minimized des Expanders de la Visite
        /// </summary>
        private void InitVisiteTileExpanders()
        {
            if (MainTileItemState != TileViewItemState.Minimized)
            {
                _isPPExpanded = false;
                _isMesuresExpanded = false;
                _isAnalyseExpanded = false;
            }
        }

        /// <summary>
        /// La recherche vient d'être effectuée.
        /// </summary>
        /// <param name="error"></param>
        private void FindDone(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, typeof(Visite).FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(Visite).Name));
            }
            // If no error is returned, set the model to entities
            else
            {
                if (this.ListOuvrages.Any())
                {
                    FiltreOuvrage = this.ListOuvrages.FirstOrDefault();
                }
                else if (this.service.Entities == null || !this.service.Entities.Any())
                {
                    this.SelectedEntity = null;
                    NavigationService.NavigateRootUrl();
                }
            }

            RaisePropertyChanged(() => this.SelectedEntity);
            RaisePropertyChanged(() => this.ListOuvrages);
            RaisePropertyChanged(() => this.ResRecherche);

            // We're done
            IsBusy = false;
        }

        #endregion Private Methods

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

            LogOuvrageList = this.SelectedEntity.Pp.LogOuvrage;
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
                RefEnumValeur = ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_LOG_OUVRAGE.GetStringValue() && r.Valeur == "M").FirstOrDefault(),
                DateHistorisation = DateTime.Now
            };

            // En cas de changement du sélected entity, on log l'enregistrement
            if (this.SelectedEntity.Pp.HasChanges)
            {
                string Modifiedproperties = null;

                Entity original = this.SelectedEntity.Pp.GetOriginal();
                if (original == null)
                {
                    original = this.SelectedEntity.Pp;
                }
                List<string> elements = new List<string>();

                foreach (PropertyInfo p in this.SelectedEntity.Pp.GetType().GetProperties())
                {
                    // Gestion des propriétés Nullable définies coté Silverlight
                    if (p.Name.EndsWith("Nullable"))
                    {
                        continue;
                    }

                    //récupération de la valeur à afficher. Si pas de valeurs on prend le nom de la propriété
                    string propertyName = String.IsNullOrEmpty(resourceManager.GetString(p.Name)) ? p.Name : resourceManager.GetString(p.Name);

                    if (propertyName == String.Empty)
                    {
                        continue;
                    }

                    if (p.CanWrite && !(p.PropertyType.BaseType == typeof(Entity)))
                    {
                        Object originalValue = p.GetValue(original, null);
                        Object newValue = p.GetValue(this.SelectedEntity.Pp, null);
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

        /// <summary>
        /// Gère la réception du nouvel utilisateur depuis la popup de création
        /// </summary>
        /// <param name="publishedEvent"></param>
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

                ServiceUtilisateur.Add(newUser);
                RaisePropertyChanged(() => this.ListUtilisateurs);

                if (this.SelectedEntity != null)
                {
                    this.SelectedEntity.UsrUtilisateur2 = newUser;
                }

                // MAJ de la vue
                RaisePropertyChanged(() => this.SelectedEntity);
            }
        }

        #endregion Public Methods

        #region Override Methods

        /// <summary>
        /// Permet de désactiver la vue et la vue popup de création d'un utilisateur
        /// </summary>
        /// <param name="viewName"></param>
        protected override void DeactivateView(string viewName)
        {
            Router.DeactivateView("CreateUsrUtilisateur");

            // désactivation de la vue de détail PP
            Router.DeactivateView("VisiteEdit_DetailPP");

            this._filtreOuvrage = null;
            this.PkMax = null;
            this.PkMin = null;
            this._includeDeleted = false;
            this.FiltreCodeTypeEq = String.Empty;

            base.DeactivateView(viewName);
        }

        /// <summary>
        /// Méthode de recherche appellé par la commande FindCommand
        /// cette méthode appelle la méthode Find du service
        /// pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            if (PkMin.HasValue && PkMax.HasValue && PkMin.Value > PkMax.Value)
            {
                ErrorWindow.CreateNew(Resource.Visite_PkIntervalleError.ToString());
            }
            else
            {
                this.IsBusy = true;

                this.saveGeoPreferences();

                (this.service as VisiteService).FindVisitesValideesByCriterias(FiltreCleRegion, FiltreCleAgence, FiltreCleSecteur, FiltreCleEnsElec,
                    FiltreClePortion, PkMin, PkMax, FiltreCodeTypeEq, IncludeDeleted, this.FindDone);
            }

        }

        /// <summary>
        /// Sauvegarde de la Visite 
        /// </summary>
        protected override void Save()
        {
            if (this.SelectedEntity != null)
            {
                // Gestion des Visites sans type d'évaluation 
                if (this.SelectedEntity.Pp != null && this.SelectedEntity.EnumTypeEval == 0)
                {
                    ErrorWindow.CreateNew(Resource.Visite_NonMesuree_Erreur);
                    return;
                }

                if (this.SelectedEntity.Pp != null && this.SelectedEntity.Pp.HasChanges)
                {
                    // Si la fiabilisation n'est pas active, on supprime les champs liés
                    if (!this.SelectedEntity.Pp.DdeDeverrouillageCoordGps)
                    {
                        this.SelectedEntity.Pp.UsrUtilisateur1 = null;
                        this.SelectedEntity.Pp.DateDdeDeverrouillageCoordGps = null;
                    }

                    this.LogPp();
                }

                //Alertes Utilisateurs
                if (this.AlerteDeclenchee && this.SelectedEntity.Alerte == null && this.SelectedEntity.DateVisite.HasValue)
                {
                    this.SelectedEntity.Alertes.Add(new Alerte()
                    {
                        Supprime = false,
                        Date = this.SelectedEntity.DateVisite.Value,
                        RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "U")
                    });
                }
                else if (!this.AlerteDeclenchee && this.SelectedEntity.Alerte != null)
                {
                    ServiceAlerte.Delete(this.SelectedEntity.Alerte);
                }

                //Alertes Seuil
                List<MesMesure> mesuresToFlag = this.SelectedEntity.MesMesure.Where(m => m.IsDepassementSeuil && m.Alerte == null).ToList();
                foreach (MesMesure mes in mesuresToFlag)
                {
                    mes.Alertes.Add(new Alerte()
                    {
                        Supprime = false,
                        Date = this.SelectedEntity.DateVisite.Value,
                        CleVisite = this.SelectedEntity.CleVisite,
                        RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "S")
                    });
                }
                List<MesMesure> mesuresToUnflag = this.SelectedEntity.MesMesure.Where(m => !m.IsDepassementSeuil && m.Alerte != null).ToList();
                foreach (MesMesure mes in mesuresToUnflag)
                {
                    ServiceAlerte.Delete(mes.Alerte);
                }

                //Alertes Analyse
                if (this.SelectedEntity.Analyse.IsNew() && (String.IsNullOrEmpty(this.SelectedEntity.Analyse.Commentaire) && this.SelectedEntity.Analyse.RefEnumValeur == null))
                {
                    ServiceAnAnalyseSerieMesure.Delete(this.SelectedEntity.Analyse);
                }
                else if (((this.SelectedEntity.Analyse.RefEnumValeur != null && this.SelectedEntity.Analyse.RefEnumValeur.Valeur == "01") || this.SelectedEntity.Analyse.RefEnumValeur == null) && this.SelectedEntity.Analyse.Alerte != null)
                {
                    ServiceAlerte.Delete(this.SelectedEntity.Analyse.Alerte);
                }
                else if (this.SelectedEntity.Analyse.RefEnumValeur != null && this.SelectedEntity.Analyse.RefEnumValeur.Valeur != "01" && this.SelectedEntity.Analyse.Alerte == null)
                {
                    this.SelectedEntity.Analyse.Alertes.Add(new Alerte()
                    {
                        Supprime = false,
                        Date = this.SelectedEntity.DateVisite.Value,
                        CleVisite = this.SelectedEntity.CleVisite,
                        RefEnumValeur = TypeAlerte.FirstOrDefault(t => t.Valeur == "A")
                    });
                }

                //UsrUtilisateur à supprimer
                List<UsrUtilisateur> users = ServiceUtilisateur.Entities.Where(u => u.IsNew() && u != this.SelectedEntity.UsrUtilisateur2).ToList();
                foreach (UsrUtilisateur user in users)
                {
                    ((UsrUtilisateurService)ServiceUtilisateur).Delete(user);
                }

                this.SelectedEntity.CleUtilisateurValidation = this.CurrentUser.CleUtilisateur;
                this.SelectedEntity.DateValidation = DateTime.Now;
                this.SelectedEntity.EstValidee = true;
            }
            base.Save();
        }

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

        /// <summary>
        /// Supression de la visite et si celle si était éditable au niveau des mesures => suppression des nouvelles mesures ajoutées
        /// </summary>
        protected override void Delete()
        {
            List<MesMesure> nouvellesMesures = this.SelectedEntity.MesMesure.Where(m => m.IsNew()).ToList();
            for (int i = nouvellesMesures.Count - 1; i > -1; i--)
            {
                this.ServiceMesMesure.Delete(nouvellesMesures.ElementAt(i));
            }
            base.Delete();
        }

        #endregion Override Methods

        #endregion Methods

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
            // Pas d'ajout sur cet écran
            return false;
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
            return this.SelectedEntity != null && GetAutorisation();
        }

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            if (this.CurrentUser != null && this.SelectedEntity != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_VISITE_NIV);
                string codePortee = role.RefUsrPortee.CodePortee;

                if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                {
                    return this.SelectedEntity.PpReliee.GeoSecteur.CleAgence == CurrentUser.CleAgence;
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
                    return this.SelectedEntity.PpReliee.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion;
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                {
                    return this.SelectedEntity.PpReliee.CleSecteur == CurrentUser.CleSecteur;
                }
            }
            return false;
        }

        #endregion Autorisations

    }
}
