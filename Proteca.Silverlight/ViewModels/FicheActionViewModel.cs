using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Browser;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Services.EntityServices;
using Proteca.Silverlight.Views.Windows;
using Proteca.Web.Models;
using Proteca.Web.Resources;
using System.ComponentModel;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    ///  ViewModel for FicheAction entity
    /// </summary>
    [ExportAsViewModel("FicheAction")]
    public class FicheActionViewModel : BaseProtecaEntityViewModel<AnAction>, INotifyDataErrorInfo
    {
        #region Private Members

        public int? CleEnsembleElec
        {
            get
            {
                return this.SelectedEntity != null ? this.SelectedEntity.CleEnsembleElec : null;
            }
            set
            {
                if (this.SelectedEntity != null)
                {
                    this.SelectedEntity.CleEnsembleElec = value;

                    this.ErrorsChanged(this, new DataErrorsChangedEventArgs("CleEnsembleElec"));
                    RaisePropertyChanged(() => CleEnsembleElec);
                }
            }
        }

        public int? CleRegion
        {
            get
            {
                return this.SelectedEntity != null ? this.SelectedEntity.CleRegion : null;
            }
            set
            {
                if (this.SelectedEntity != null)
                {
                    this.SelectedEntity.CleRegion = value;

                    this.ErrorsChanged(this, new DataErrorsChangedEventArgs("CleRegion"));
                    RaisePropertyChanged(() => CleRegion);
                }
            }
        }

        /// <summary>
        ///     Gestion de la version Popup des fiches action
        /// </summary>
        private bool _isPopupMode;

        public bool IsPopupMode
        {
            get { return _isPopupMode; }
            set
            {
                _isPopupMode = value;
                IsAutoNavigateToFirst = !value;
                RaisePropertyChanged(() => IsPopupMode);
            }
        }

        public AnAnalyse Analyse { get; set; }

        /// <summary>
        ///     Gestion de l'affichage du bouton imprimer.
        /// </summary>
        public bool IsNotPrintableMode
        {
            get { return IsEditMode || SelectedEntity == null; }
        }

        #region Enums

        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les niveaux de priorité en base
        /// </summary>
        private string enumNiveauxPriorite = RefEnumValeurCodeGroupeEnum.ACTION_PRIORITE.GetStringValue();

        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les entités de traitement en base
        /// </summary>
        private string enumEntiteDeTraitement = RefEnumValeurCodeGroupeEnum.ENTITE_TRAITEMENT.GetStringValue();
        
        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les types d'états d'analyse en base
        /// </summary>
        private string enumEtatsAnalyse = RefEnumValeurCodeGroupeEnum.AN_ETAT_PC.GetStringValue();

        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les types de statuts en base
        /// </summary>
        private string enumStatuts = RefEnumValeurCodeGroupeEnum.ENUM_STATUT_ACTION.GetStringValue();

        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les types de catégories d'anomalie en base
        /// </summary>
        private string enumCategorie = RefEnumValeurCodeGroupeEnum.ACTION_CATEGORIE_ANOMALIE.GetStringValue();

        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les types d'actions en base
        /// </summary>
        private string enumTypeAction = RefEnumValeurCodeGroupeEnum.ACTION_TYPE.GetStringValue();

        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les types de délais de réalisation en base
        /// </summary>
        private string enumDelaiReal = RefEnumValeurCodeGroupeEnum.ACTION_DELAI_REAL.GetStringValue();

        /// <summary>
        ///     Déclaration de l'énum permettant d'afficher les types d'évaluation en base
        /// </summary>
        private string enumTypeEval = RefEnumValeurCodeGroupeEnum.TYPE_EVAL.GetStringValue();

        private ReadOnlyCollection<GeoEnsembleElectrique> _geoEnsembleElectriquesAction;

        /// <summary>
        ///     Liste des geoEnsembleElectrique liés à l'action en cours d'édition ou d'ajout
        /// </summary>
        private ReadOnlyCollection<GeoEnsembleElectrique> GeoEnsembleElectriquesAction
        {
            get
            {
                if (_geoEnsembleElectriquesAction == null)
                {
                    var geoEns = new List<GeoEnsembleElectrique>();

                    if (SelectedEntity.CleEnsembleElec != null)
                    {
                        geoEns =
                            ServiceGeoEnsElec.Entities.Where(
                                c => c.CleEnsElectrique == SelectedEntity.CleEnsembleElec.Value).ToList();
                    }

                    _geoEnsembleElectriquesAction = new ReadOnlyCollection<GeoEnsembleElectrique>(geoEns);
                }
                return _geoEnsembleElectriquesAction;
            }
        }

        #endregion Enums

        #region Filters

        /// <summary>
        ///     Déclaration de la variable FiltreCleEnsElec
        /// </summary>
        private int? _filtreCleEnsElec;

        /// <summary>
        ///     Déclaration de la variable FiltreCleAnalyse
        /// </summary>
        private int? _filtreCleAnalyse;

        /// <summary>
        ///     Déclaration de la variable FiltreClePriorite
        /// </summary>
        private int? _filtreClePriorite;

        /// <summary>
        ///     Déclaration de la variable FiltreCleStatut
        /// </summary>
        private int? _filtreCleStatut;

        /// <summary>
        ///     Déclaration de la variable FiltreCleUser
        /// </summary>
        private int? _filtreCleUser;

        #endregion Filters

        #endregion Private Members

        #region Public Members

        public bool IsActionHorsAnalyse
        {
            get { return !IsPopupMode && SelectedEntity != null && SelectedEntity.IsActionHorsAnalyse; }
        }

        /// <summary>
        ///     Gère l'affichage du panel d'info
        /// </summary>
        public bool IsInfoAffiche
        {
            get { return SelectedEntity != null && !IsNewMode && SelectedEntity.Supprime; }
        }

        /// <summary>
        ///     Retourne la valeur True si l'action sélectionnée n'est pas supprimée, que ne nous ne sommes pas en mode création et
        ///     que le mode d'édition est désactivé
        /// </summary>
        public bool IsDeleteEnable
        {
            get
            {
                if (SelectedEntity != null)
                {
                    return !SelectedEntity.Supprime && !IsEditMode && !IsNewMode;
                }
                return false;
            }
        }

        /// <summary>
        ///     Retourne la valeur True si l'action sélectionnée est supprimée, que nous ne sommes pas en mode création et que le
        ///     mode d'édition est désactivé
        /// </summary>
        public bool IsReintegrateEnable
        {
            get
            {
                if (SelectedEntity != null)
                {
                    return SelectedEntity.Supprime && !IsEditMode && !IsNewMode;
                }
                return false;
            }
        }

        public bool IsPortionIntegriteAnActionNonValide
        {
            get
            {
                return IsActionHorsAnalyse && SelectedEntity.ValidationErrors.Any(v => v.MemberNames.Any(m => m == "PortionIntegriteAnAction"));
            }
        }

        #region Liste propriétés "IsXXXXEnable pour activer désactiver les champs du formulaire en fonction des droits utilisateurs

        public bool IsCleRegionEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.CleRegion, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsCleUtilisateurResponsableEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.CleUtilisateurResponsable, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsQuantiteEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.Quantite, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsFiltreCleCategorieEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.FiltreCleCategorie, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsParametreActionEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.ParametreAction, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsConstatAnomalieEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.ConstatAnomalie, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsDescriptionEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.Description, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsProgrammeBudgetaireEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.ProgrammeBudgetaire, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsEntiteTraitementEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.EntiteTraitement, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsEnumStatutEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.EnumStatut, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsDateClotureEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.DateCloture, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsCommentaireStatutEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.CommentaireStatut, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsCommentaireEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.Commentaire, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsCoutGlobalReelEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.CoutGlobalReel, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsTempsTravailGlobalReelEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.TempsTravailGlobalReel, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsDateDebutEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.DateDebut, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsDateFinEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.DateFin, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsCleUtilisateurAgentEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.CleUtilisateurAgent, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsDateRealisationTravauxEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.DateRealisationTravaux, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        public bool IsTypeEvalEnable
        {
            get
            {
                return SelectedEntity != null && IsEditMode &&
                       SelectedEntity.CanEditField(() => SelectedEntity.TypeEval, CurrentUser,
                           GeoEnsembleElectriquesAction);
            }
        }

        #endregion

        #region Liste propriétés "IsXXXXRequired pour déterminer si le champ du formulaire est obligatoire en fonction des droits utilisateurs
        public string LabelCleRegion
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.CleRegion), CurrentUser);
                return Resource.Utilisateurs_Region.ToRequiredLabel(isRequired);

            }
        }

        public string LabelCleEnsembleElec
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.CleEnsembleElec), CurrentUser);
                return Resource.FicheAction_EnsembleElectrique.ToRequiredLabel(isRequired);

            }
        }


        public string LabelPortionsIntegriteAction
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.PortionIntegriteAnAction), CurrentUser);
                return Resource.FicheAction_PortionsIntegrites.ToRequiredLabel(isRequired);

            }
        }

        public string LabelCleUtilisateurResponsable
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.CleUtilisateurResponsable), CurrentUser);
                return Resource.FicheAction_Resp.ToRequiredLabel(isRequired);

            }
        }

        public string LabelQuantite
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.Quantite), CurrentUser);
                return Resource.FicheAction_Quantite.ToRequiredLabel(isRequired);
            }
        }

        public string LabelFiltreCleCategorie
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.FiltreCleCategorie), CurrentUser);
                return Resource.FicheAction_Categorie.ToRequiredLabel(isRequired);
            }
        }

        public string LabelParametreAction
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.ParametreAction), CurrentUser);
                return Resource.FicheAction_Type.ToRequiredLabel(isRequired);
            }
        }

        public string LabelConstatAnomalie
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.ConstatAnomalie), CurrentUser);
                return Resource.FicheAction_Constat.ToRequiredLabel(isRequired);

            }
        }

        public string LabelDescription
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.Description), CurrentUser);
                return Resource.FicheAction_Description.ToRequiredLabel(isRequired);

            }
        }

        public string LabelProgrammeBudgetaire
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.ProgrammeBudgetaire), CurrentUser);
                return Resource.FicheAction_Programme.ToRequiredLabel(isRequired);

            }
        }

        public string LabelEntiteTraitement
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.EntiteTraitement), CurrentUser);
                return Resource.FicheAction_Entite.ToRequiredLabel(isRequired);

            }
        }

        public string LabelEnumStatut
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.EnumStatut), CurrentUser);
                return Resource.FicheAction_Statut.ToRequiredLabel(isRequired);

            }
        }

        public string LabelDateCloture
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.DateCloture), CurrentUser);
                return Resource.FicheAction_DateCloture.ToRequiredLabel(isRequired);

            }
        }

        public string LabelCommentaireStatut
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.CommentaireStatut), CurrentUser);
                return Resource.FicheAction_ComStatut.ToRequiredLabel(isRequired);

            }
        }

        public string LabelCoutGlobalReel
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.CoutGlobalReel), CurrentUser);
                return Resource.FicheAction_Reel.ToRequiredLabel(isRequired);
            }
        }

        public string LabelTempsTravailGlobalReel
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.TempsTravailGlobalReel), CurrentUser);
                return Resource.FicheAction_Reel.ToRequiredLabel(isRequired);

            }
        }

        public string LabelDateDebut
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.DateDebut), CurrentUser);
                return Resource.FicheAction_DateDebut.ToRequiredLabel(isRequired);

            }
        }

        public string LabelDateFin
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.DateFin), CurrentUser);
                return Resource.FicheAction_DateFin.ToRequiredLabel(isRequired);

            }
        }

        public string LabelCleUtilisateurAgent
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.CleUtilisateurAgent), CurrentUser);
                return Resource.FicheAction_NomAgent.ToRequiredLabel(isRequired);

            }
        }

        public string LabelTypeEval
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.TypeEval), CurrentUser);
                return Resource.FicheAction_TypeEval.ToRequiredLabel(isRequired);
            }
        }

        public string LabelDateRealisationTravaux
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.DateRealisationTravaux), CurrentUser);
                return Resource.FicheAction_DateFin.ToRequiredLabel(isRequired);

            }
        }

        public string LabelCommentaire
        {
            get
            {
                var isRequired = SelectedEntity != null && IsEditMode &&
                       SelectedEntity.IsRequiredField(ExtractPropertyName(() => SelectedEntity.Commentaire), CurrentUser);
                return Resource.FicheAction_Commentaire.ToRequiredLabel(isRequired);

            }
        }
        #endregion


        #region Lists ComboBoxes

        /// <summary>
        ///     Retourne les liste des niveaux de priorité du service RefEnumValeur
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListNiveauxPriorite
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return
                        new ObservableCollection<RefEnumValeur>(
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumNiveauxPriorite)
                                .OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les liste des etats d'analyse du service ListEtatAnalyse
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListEntiteDeTraitement
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return
                        new ObservableCollection<RefEnumValeur>(
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumEntiteDeTraitement)
                                .OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les liste des etats d'analyse du service ListEtatAnalyse
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListTypeEval
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return
                        new ObservableCollection<RefEnumValeur>(
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumTypeEval && !(r.Valeur.Equals("3")))
                                .OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les liste des etats d'analyse du service ListEtatAnalyse
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListEtatsAnalyseComplete
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return
                        new ObservableCollection<RefEnumValeur>(
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumEtatsAnalyse)
                                .OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les liste des etats d'analyse du service ListEtatAnalyse sans l'état Satisfaisant
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListEtatsAnalyse
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return
                        new ObservableCollection<RefEnumValeur>(
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumEtatsAnalyse)
                                .OrderBy(r => r.NumeroOrdre));
                    //cette version est utilisée pour exclure "satisfaisant"
                    //return new ObservableCollection<RefEnumValeur>(ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumEtatsAnalyse).Where(r => r.Valeur != "01").OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les liste des statuts du service ListStatuts
        /// </summary>
        public ObservableCollection<RefEnumValeur> ListStatuts
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return
                        new ObservableCollection<RefEnumValeur>(
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumStatuts)
                                .OrderBy(r => r.NumeroOrdre));
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les liste des statuts du service ListStatuts
        /// </summary>
        public List<RefEnumValeur> ListStatuts2
        {
            get
            {
                if (ServiceRefEnumValeur != null && SelectedEntity != null)
                {
                    List<RefEnumValeur> statuts =
                        SelectedEntity.GetListeStatuts(
                            ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumStatuts).ToList())
                            .OrderBy(r => r.NumeroOrdre)
                            .ToList();
                    return statuts;
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les GEO ensembles électrique
        /// </summary>
        public List<ParametreAction> ListCategories
        {
            get
            {
                return
                    ServiceParametreAction.Entities.Distinct(
                        new InlineEqualityComparer<ParametreAction>(
                            (a, b) => a.EnumCategorieAnomalie.Equals(b.EnumCategorieAnomalie)))
                        .OrderBy(pa => pa.RefEnumValeur.NumeroOrdre)
                        .ThenBy(pa => pa.RefEnumValeur.Libelle)
                        .ToList();
            }
        }

        /// <summary>
        ///     Retourne les GEO ensembles électrique / portions
        /// </summary>
        public List<ParametreAction> ListCategorieTypesAction
        {
            get
            {
                if (SelectedEntity != null && SelectedEntity.FiltreCleCategorie != null)
                {
                    return
                        ServiceParametreAction.Entities.Where(
                            pa => pa.EnumCategorieAnomalie == SelectedEntity.FiltreCleCategorie)
                            .Distinct(
                                new InlineEqualityComparer<ParametreAction>(
                                    (a, b) => a.EnumTypeAction.Equals(b.EnumTypeAction)))
                            .OrderBy(pa => pa.RefEnumValeur3.NumeroOrdre)
                            .ThenBy(pa => pa.RefEnumValeur3.Libelle)
                            .ToList();
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les liste des délais réalisation du service ListDelaiReal
        /// </summary>
        public List<RefEnumValeur> ListDelaiReal
        {
            get
            {
                if (ServiceRefEnumValeur != null)
                {
                    return
                        ServiceRefEnumValeur.Entities.Where(r => r.CodeGroupe == enumDelaiReal)
                            .OrderBy(r => r.NumeroOrdre)
                            .ThenBy(r => r.Libelle)
                            .ToList();
                }
                return null;
            }
        }

        /// <summary>
        ///     Retourne les liste des agents du service ServiceUsers
        /// </summary>
        public List<UsrUtilisateur> ListUsers
        {
            get
            {
                if (FiltreCleRegion != null)
                {
                    if (FiltreCleAgence != null)
                    {
                        if (FiltreCleSecteur != null)
                        {
                            return
                                ServiceUsers.Entities.Where(
                                    u => u.CleSecteur == FiltreCleSecteur || u.CleAgence == FiltreCleAgence)
                                    .Distinct(
                                        new InlineEqualityComparer<UsrUtilisateur>(
                                            (a, b) => a.CleUtilisateur.Equals(b.CleUtilisateur) &&
                                                      a.Nom_Prenom.Equals(b.Nom_Prenom)))
                                    .ToList();
                        }
                        return
                            ServiceUsers.Entities.Where(u => u.CleAgence == FiltreCleAgence)
                                .Distinct(
                                    new InlineEqualityComparer<UsrUtilisateur>(
                                        (a, b) => a.CleUtilisateur.Equals(b.CleUtilisateur) &&
                                                  a.Nom_Prenom.Equals(b.Nom_Prenom)))
                                .ToList();
                    }
                    return
                        ServiceUsers.Entities.Where(u => u.GeoAgence.CleRegion == FiltreCleRegion)
                            .Distinct(
                                new InlineEqualityComparer<UsrUtilisateur>(
                                    (a, b) => a.CleUtilisateur.Equals(b.CleUtilisateur) &&
                                              a.Nom_Prenom.Equals(b.Nom_Prenom)))
                            .ToList();
                }
                return
                    ServiceUsers.Entities.Distinct(
                        new InlineEqualityComparer<UsrUtilisateur>(
                            (a, b) => a.CleUtilisateur.Equals(b.CleUtilisateur) && a.Nom_Prenom.Equals(b.Nom_Prenom)))
                        .ToList();
            }
        }

        /// <summary>
        ///     Retourne les liste des agents du service ServiceUsers
        /// </summary>
        public List<UsrUtilisateur> ListUsersAction
        {
            get
            {
                return ServiceAllUsers.Entities.Distinct(
                             new InlineEqualityComparer<UsrUtilisateur>(
                                 (a, b) => a.CleUtilisateur.Equals(b.CleUtilisateur) &&
                                           a.Nom_Prenom.Equals(b.Nom_Prenom)))
                             .ToList();
            }
        }

        /// <summary>
        ///     Retourne les régions du service région
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return new ObservableCollection<GeoRegion>(ServiceRegion.Entities.OrderBy(r => r.LibelleRegion)); }
        }

        /// <summary>
        ///     Retourne les régions du service région
        /// </summary>
        public ObservableCollection<GeoRegion> RegionsByUser
        {
            get
            {
                if (CurrentUser != null)
                {
                    UsrRole role = CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.CRE_FICHE_ACTION_NIV);
                    string codePortee = role.RefUsrPortee.CodePortee;
                    var canAdd = false;
                    if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                        codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                    {
                        canAdd = true;
                    }
                    if (SelectedEntity != null)
                    {
                        return new ObservableCollection<GeoRegion>(ServiceRegion.Entities.Where(r => canAdd || r.CleRegion == CurrentUser.GeoAgence.CleRegion || ((!this.IsEditMode || this.SelectedEntity.CleUtilisateurResponsable == this.CurrentUser.CleUtilisateur) && SelectedEntity.CleRegion.HasValue && r.CleRegion == SelectedEntity.CleRegion.Value)).OrderBy(r => r.LibelleRegion));
                    }
                }
                return new ObservableCollection<GeoRegion>();
            }
        }

        /// <summary>
        ///     Retourne les GEO ensembles électrique du service EnsElec
        /// </summary>
        public List<GeoEnsembleElectrique> GeoEnsemblesElectrique
        {
            get
            {
                if (FiltreCleRegion != null)
                {
                    if (FiltreCleAgence != null)
                    {
                        if (FiltreCleSecteur != null)
                        {
                            return ServiceGeoEnsElec.Entities.Where(i => i.CleSecteur == FiltreCleSecteur).ToList();
                        }
                        return
                            ServiceGeoEnsElec.Entities.Where(i => i.CleAgence == FiltreCleAgence)
                                .Distinct(
                                    new InlineEqualityComparer<GeoEnsembleElectrique>(
                                        (a, b) => a.CleEnsElectrique.Equals(b.CleEnsElectrique) &&
                                                  a.Libelle.Equals(b.Libelle)))
                                .ToList();
                    }
                    return
                        ServiceGeoEnsElec.Entities.Where(i => i.CleRegion == FiltreCleRegion)
                            .Distinct(
                                new InlineEqualityComparer<GeoEnsembleElectrique>(
                                    (a, b) => a.CleEnsElectrique.Equals(b.CleEnsElectrique) &&
                                              a.Libelle.Equals(b.Libelle)))
                            .ToList();
                }
                return
                    ServiceGeoEnsElec.Entities.Distinct(
                        new InlineEqualityComparer<GeoEnsembleElectrique>(
                            (a, b) => a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle)))
                        .ToList();
            }
        }

        /// <summary>
        ///     Retourne les GEO ensembles électrique du service EnsElec utilisé par l'écran de création d'action hors analyse
        /// </summary>
        public ObservableCollection<GeoEnsembleElectrique> GeoEnsemblesElectriqueByRegionAndUser
        {
            get
            {
                if (SelectedEntity != null && SelectedEntity.CleRegion.HasValue)
                {
                    //TODO filtrer sur les ee correspondants aux droits de l'utilisateur
                    UsrRole autorisationCreation = CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.CRE_FICHE_ACTION_NIV);
                    string porteeAutorisation = autorisationCreation.RefUsrPortee.CodePortee;

                    if (porteeAutorisation == RefUsrPortee.ListPorteesEnum.National.GetStringValue() || porteeAutorisation == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                    {
                        return new ObservableCollection<GeoEnsembleElectrique>(
                        ServiceGeoEnsElec.Entities.Where(i => i.CleRegion == SelectedEntity.CleRegion)
                            .Distinct(
                                new InlineEqualityComparer<GeoEnsembleElectrique>(
                                    (a, b) => a.CleEnsElectrique.Equals(b.CleEnsElectrique) &&
                                              a.Libelle.Equals(b.Libelle)))
                            .ToList());
                    }
                    else if (porteeAutorisation == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                    {
                        return new ObservableCollection<GeoEnsembleElectrique>(
                            ServiceGeoEnsElec.Entities.Where(i => (i.CleRegion == SelectedEntity.CleRegion && i.CleAgence == CurrentUser.CleAgence) || ((!this.IsEditMode || this.SelectedEntity.CleUtilisateurResponsable == this.CurrentUser.CleUtilisateur) && SelectedEntity.CleEnsembleElec.HasValue && i.CleEnsElectrique == SelectedEntity.CleEnsembleElec.Value))
                            .Distinct(
                                new InlineEqualityComparer<GeoEnsembleElectrique>(
                                    (a, b) => a.CleEnsElectrique.Equals(b.CleEnsElectrique) &&
                                              a.Libelle.Equals(b.Libelle)))
                            .ToList());
                    }
                    else if (porteeAutorisation == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                    {
                        return
                        new ObservableCollection<GeoEnsembleElectrique>(ServiceGeoEnsElec.Entities.Where(i => (i.CleRegion == SelectedEntity.CleRegion && i.CleSecteur == CurrentUser.CleSecteur) || ((!this.IsEditMode || this.SelectedEntity.CleUtilisateurResponsable == this.CurrentUser.CleUtilisateur) && SelectedEntity.CleEnsembleElec.HasValue && i.CleEnsElectrique == SelectedEntity.CleEnsembleElec.Value))
                            .Distinct(
                                new InlineEqualityComparer<GeoEnsembleElectrique>(
                                    (a, b) => a.CleEnsElectrique.Equals(b.CleEnsElectrique) &&
                                              a.Libelle.Equals(b.Libelle)))
                            .ToList());
                    }
                    else if (porteeAutorisation == RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                    {
                        return
                            new ObservableCollection<GeoEnsembleElectrique>(ServiceGeoEnsElec.Entities.Where(i => ((!this.IsEditMode || this.SelectedEntity.CleUtilisateurResponsable == this.CurrentUser.CleUtilisateur) && SelectedEntity.CleEnsembleElec.HasValue && i.CleEnsElectrique == SelectedEntity.CleEnsembleElec.Value))
                            .Distinct(
                                new InlineEqualityComparer<GeoEnsembleElectrique>(
                                    (a, b) => a.CleEnsElectrique.Equals(b.CleEnsElectrique) &&
                                              a.Libelle.Equals(b.Libelle)))
                            .ToList());
                    }
                }
                return new ObservableCollection<GeoEnsembleElectrique>();
            }
        }

        private List<PortionIntegrite> _portionsIntegriteAction;

        /// <summary>
        ///     Retourne les portions intégrité liées à l'ensemble électrique
        /// </summary>
        public List<PortionIntegrite> PortionsIntegriteAction
        {
            get 
            { 
                return _portionsIntegriteAction; 
            }
            set
            {
                _portionsIntegriteAction = value;
                RaisePropertyChanged(() => PortionsIntegriteAction);
            }
        }

        #endregion Lists ComboBoxes

        #region Filters

        /// <summary>
        ///     Retourne la clé de l'ensemble électrique
        /// </summary>
        public int? FiltreCleEnsElec
        {
            get { return _filtreCleEnsElec; }
            set
            {
                _filtreCleEnsElec = value;
                RaisePropertyChanged(() => FiltreCleEnsElec);
            }
        }

        //public int? FiltreCleEnsElecAction
        //{
        //    get
        //    {
        //        return SelectedEntity != null ? SelectedEntity.CleEnsembleElec : null;
        //    }
        //    set
        //    {
        //        if (SelectedEntity != null)
        //        {
        //            if (this.IsEditMode)
        //            {
        //                SelectedEntity.CleEnsembleElec = value;
        //                if (OnEnsembleElectriqueActionSelected != null)
        //                {
        //                    OnEnsembleElectriqueActionSelected(this, null);
        //                }
        //            }
        //            RaisePropertyChanged(() => FiltreCleEnsElecAction);
        //        }
        //    }
        //}

        //public int? FiltreCleRegionAction
        //{
        //    get
        //    {
        //        return SelectedEntity != null ? SelectedEntity.CleRegion : null;
        //    }
        //    set
        //    {
        //        if (SelectedEntity != null)
        //        {
        //            if (this.IsEditMode)
        //            {
        //                SelectedEntity.CleRegion = value;
        //                if (OnRegionActionSelected != null)
        //                {
        //                    OnRegionActionSelected(this, null);
        //                }
        //            }
        //            RaisePropertyChanged(() => FiltreCleRegionAction);
        //        }
        //    }
        //}

        //public int? FiltreCleCategorieAction
        //{
        //    get
        //    {
        //        return SelectedEntity != null ? SelectedEntity.FiltreCleCategorie : null;
        //    }
        //    set
        //    {
        //        if (SelectedEntity != null)
        //        {
        //            if (this.IsEditMode)
        //            {
        //                SelectedEntity.FiltreCleCategorie = value;
        //            }
        //            RaisePropertyChanged(() => FiltreCleCategorieAction);
        //            RaisePropertyChanged(() => ListCategorieTypesAction);
        //        }
        //    }
        //}

        /// <summary>
        ///     Retourne la clé de l'analyse
        /// </summary>
        public int? FiltreCleAnalyse
        {
            get { return _filtreCleAnalyse; }
            set
            {
                _filtreCleAnalyse = value;
                RaisePropertyChanged(() => FiltreCleAnalyse);
            }
        }

        /// <summary>
        ///     Retourne la clé du niveau de priorité
        /// </summary>
        public int? FiltreClePriorite
        {
            get { return _filtreClePriorite; }
            set
            {
                _filtreClePriorite = value;
                RaisePropertyChanged(() => FiltreClePriorite);
            }
        }

        /// <summary>
        ///     Retourne la clé du statut
        /// </summary>
        public int? FiltreCleStatut
        {
            get { return _filtreCleStatut; }
            set
            {
                _filtreCleStatut = value;
                RaisePropertyChanged(() => FiltreCleStatut);
            }
        }

        /// <summary>
        ///     Retourne la clé de l'agent
        /// </summary>
        public int? FiltreCleUser
        {
            get { return _filtreCleUser; }
            set
            {
                _filtreCleUser = value;
                RaisePropertyChanged(() => FiltreCleUser);
            }
        }

        /// <summary>
        ///     Retourne la date de debut du filtre
        /// </summary>
        public DateTime? DateMin { get; set; }

        /// <summary>
        ///     Retourne la date de fin du filtre
        /// </summary>
        public DateTime? DateMax { get; set; }

        /// <summary>
        ///     Déclaration de la variable incluant les actions supprimées
        /// </summary>
        public bool? IncludeDeletedAction { get; set; }

        #endregion Filters

        #endregion Public Members

        #region Services

        /// <summary>
        ///     Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;

        /// <summary>
        ///     Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceRefEnumValeur { get; set; }

        /// <summary>
        ///     Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> ServiceRegion { get; set; }

        /// <summary>
        ///     Service utilisé pour gérer les GEO ensembles électriques
        /// </summary>
        [Import]
        public IEntityService<GeoEnsembleElectrique> ServiceGeoEnsElec { get; set; }

        /// <summary>
        ///     Service utilisé pour gérer les entités de type utilisateurs
        /// </summary>
        [Import]
        public IEntityService<UsrUtilisateur> ServiceUsers { get; set; }

        [Import(typeof(AllUsrUtilisateurService))]
        public IEntityService<UsrUtilisateur> ServiceAllUsers { get; set; }
        
        /// <summary>
        ///     Service utilisé pour gérer les entités de type ParametreAction
        /// </summary>
        [Import]
        public IEntityService<ParametreAction> ServiceParametreAction { get; set; }

        /// <summary>
        ///     Service utilisé pour gérer les portions integrites
        /// </summary>
        [Import]
        public IEntityService<PortionIntegrite> ServicePortionIntegrite { get; set; }

        #endregion Services

        #region Events
        public EventHandler OnRegionActionSelected;

        public EventHandler OnEnsembleElectriqueActionSelected;

        #endregion

        #region Constructor

        public FicheActionViewModel()
        {
            OnViewActivated += (o, e) =>
            {
                IsPopupMode = e.ViewParameter.Any(p => p.Key == "IsPopupMode");

                if (e.ViewParameter.ContainsKey("Analyse"))
                {
                    Analyse = e.ViewParameter["Analyse"] as AnAnalyse;
                }

                // Si les utilisateurs ne sont pas encore chargés, on attend qu'ils le soient (dans le UtilisateursLoaded)
                if (IsPopupMode && ListUsersAction != null && ListUsersAction.Any())
                {
                    Add();
                }

                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                else if (!IsPopupMode && e.ViewParameter.All(p => p.Key != "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs()
                        .AddNamedParameter("Title", Resource.FicheAction_ExpanderTitle));
                    EventAggregator.Publish(
                        "FicheAction_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }
                RaisePropertyChanged(() => GeoEnsemblesElectrique);

                if (IncludeDeletedAction == null)
                {
                    IncludeDeletedAction = false;
                }
                RaisePropertyChanged(() => IncludeDeletedAction);
                RefreshPrintButton();
            };

            // Boutons générique
            RestitutionCommand = new ActionCommand<object>(
                obj => ShowRestitution(obj));

            // Boutons spécifiques
            FicheActionCommand = new ActionCommand<object>(
                obj => ShowFicheAction(obj));

            ReintegrateActionCommand = new ActionCommand<object>(
                obj => ReintegrateAction(), obj => IsReintegrateEnable);

            // Désactive l'ajout automatique en édition
            IsAutoAddOnEditMode = false;

            AllowEditEmptyEntities = false;

            OnRegionSelected += (o, e) =>
            {
                RaisePropertyChanged(() => GeoEnsemblesElectrique);
                RaisePropertyChanged(() => ListUsers);
                RefreshPrintButton();
            };

            OnRegionActionSelected += OnActionRegionSelected;

            OnEnsembleElectriqueActionSelected += OnActionEnsembleElectriqueSelected;

            OnAgenceSelected += (o, e) =>
            {
                RaisePropertyChanged(() => GeoEnsemblesElectrique);
                RaisePropertyChanged(() => ListUsers);
                RefreshPrintButton();
            };

            OnSecteurSelected += (o, e) =>
            {
                RaisePropertyChanged(() => GeoEnsemblesElectrique);
                RaisePropertyChanged(() => ListUsers);
                RefreshPrintButton();
            };

            OnAddedEntity += (o, e) =>
            {
                /* 
                 * Pour combler un bug telerik (Lors de l'ajout d'une action: le binding de la cleEnsElec ne se faisait pas après l'affichage d'une action par analyse suivi d'une recherche sans résultat),
                 * Nous chargeons puis déchargeons immédiatement la liste des ensembles électriques lorsque nous sommes en ajout d'action hors analyse et qu'aucun résultat n'avait été trouvé précédemment 
                 */
                if (this.IsPopupMode == false)
                {
                    RaisePropertyChanged(() => RegionsByUser);
                    OnEnsembleElectriqueActionSelected -= OnActionEnsembleElectriqueSelected;
                    CleRegion = this.RegionsByUser.Last().CleRegion;
                    CleEnsembleElec = this.GeoEnsemblesElectriqueByRegionAndUser.First().CleEnsElectrique;

                    CleEnsembleElec = null;
                    CleRegion = null;
                    this.PortionsIntegriteAction = new List<PortionIntegrite>();

                    this.SelectedEntity.ValidationErrors.Clear();
                    this.ErrorsChanged(this, new DataErrorsChangedEventArgs("CleEnsembleElec"));
                    this.ErrorsChanged(this, new DataErrorsChangedEventArgs("CleRegion"));
                    OnEnsembleElectriqueActionSelected += OnActionEnsembleElectriqueSelected;
                }

                this.PortionsIntegriteAction = new List<PortionIntegrite>();
                if (SelectedEntity != null)
                {
                    if (IsPopupMode && Analyse != null)
                    {
                        SelectedEntity.AnAnalyse = Analyse;
                    }

                    SelectedEntity.CleUtilisateurCreation = userService.CurrentUser.CleUtilisateur;

                    SelectedEntity.DateCreation = DateTime.Now;
                    RaisePropertyChanged(() => ListStatuts2);
                    SelectedEntity.EnumStatut = ListStatuts2.First().CleEnumValeur;
                    RaisePropertyChanged(() => RegionsByUser);
                    RaisePropertyChanged(() => ListUsersAction);
                    RaisePropertyChanged(() => ListCategories);
                    RaisePropertyChanged(() => ListEntiteDeTraitement);
                    RaisePropertyChanged(() => ListTypeEval);
                    RaisePropertyChanged(() => ListCategorieTypesAction);
                    RaisePropertyChanged(() => SelectedEntity);
                    RaiseAllIsEnableProperties();
                }
            };


            // ré-activation de la navigation automatique

            OnCanceled += (o, e) =>
            {
                if (SelectedEntity != null && SelectedEntity.ParametreAction != null)
                {
                    SelectedEntity.FiltreCleCategorie = SelectedEntity.ParametreAction.EnumCategorieAnomalie;
                    if (SelectedEntity.CleEnsembleElec.HasValue && SelectedEntity.CleRegion.HasValue && SelectedEntity.CleEnsembleElec > 0)
                    {
                        var intCleEns = SelectedEntity.CleEnsembleElec.Value;
                        ((PortionIntegriteService)ServicePortionIntegrite).GetListPortionsByCleEnsElec(intCleEns, SelectedEntity.CleRegion.Value, GetListPortionsDone);
                    }
                }
                RaisePropertyChanged(() => GeoEnsemblesElectrique);
                RaisePropertyChanged(() => ListCategories);
                RaisePropertyChanged(() => ListEntiteDeTraitement);
                RaisePropertyChanged(() => ListTypeEval);
                RaisePropertyChanged(() => ListCategorieTypesAction);
                RaisePropertyChanged(() => SelectedEntity);
                RefreshPrintButton();
            };

            OnDetailLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => CleRegion);
                // on enregistre la valeur de la clé de l'ensemble electrique ...
                int? cleEE = SelectedEntity.CleEnsembleElec;
                RaisePropertyChanged(() => RegionsByUser);
                RaisePropertyChanged(() => GeoEnsemblesElectriqueByRegionAndUser);
                // .. Pour la remettre ici car le raisePropertyChanged de GeoEnsemblesElectriqueByRegionAndUser change sa valeur
                SelectedEntity.CleEnsembleElec = cleEE;
                RaisePropertyChanged(() => CleRegion);

                registerPropertyChanged();
                if (SelectedEntity != null && SelectedEntity.ParametreAction != null)
                {
                    SelectedEntity.FiltreCleCategorie = SelectedEntity.ParametreAction.EnumCategorieAnomalie;
                    RaisePropertyChanged(() => ListStatuts2);

                    if (!SelectedEntity.CleEnsembleElec.HasValue && !SelectedEntity.IsNew())
                    {
                        if (SelectedEntity.AnAnalyse != null && SelectedEntity.AnAnalyse is AnAnalyseEe)
                        {
                            SelectedEntity.CleEnsembleElec = (SelectedEntity.AnAnalyse as AnAnalyseEe).CleEnsElectrique;
                        }
                        else
                        {
                            if (SelectedEntity.PortionIntegriteAnAction.Any() && SelectedEntity.PortionIntegriteAnAction.First().PortionIntegrite != null)
                            {
                                SelectedEntity.CleEnsembleElec = SelectedEntity.PortionIntegriteAnAction.First().PortionIntegrite.CleEnsElectrique;
                            }
                        }
                    }

                    OnEnsembleElectriqueActionSelected(this, null);
                }
                RaisePropertyChanged(() => ListCategories);
                RaisePropertyChanged(() => ListCategorieTypesAction);
                RaisePropertyChanged(() => ListEntiteDeTraitement);
                RaisePropertyChanged(() => ListTypeEval);
                RaisePropertyChanged(() => SelectedEntity);
                RaisePropertyChanged(() => IsInfoAffiche);
                RaisePropertyChanged(() => IsReintegrateEnable);
                RaisePropertyChanged(() => IsDeleteEnable);
                RaisePropertyChanged(() => IsActionHorsAnalyse);

                RaisePropertyChanged(() => SelectedEntity.EnumStatut);
                RefreshPrintButton();
            };

            OnViewModeChanged += (o, e) =>
            {
                if (IsEditMode)
                {
                    // on enregistre la valeur de la clé de l'ensemble electrique ...
                    int? cleEE = SelectedEntity.CleEnsembleElec;
                    RaisePropertyChanged(() => RegionsByUser);
                    RaisePropertyChanged(() => GeoEnsemblesElectriqueByRegionAndUser);
                    // .. Pour la remettre ici car le raisePropertyChanged de GeoEnsemblesElectriqueByRegionAndUser change sa valeur
                    SelectedEntity.CleEnsembleElec = cleEE;
                    RaisePropertyChanged(() => CleRegion);

                    OnEnsembleElectriqueActionSelected(this, null);
                }
                RaisePropertyChanged(() => GeoEnsemblesElectrique);
                RaisePropertyChanged(() => ListStatuts2);
                RaisePropertyChanged(() => IsInfoAffiche);
                RaisePropertyChanged(() => IsReintegrateEnable);
                RaisePropertyChanged(() => IsDeleteEnable);
                RaisePropertyChanged(() => IsActionHorsAnalyse);
                RaisePropertyChanged(() => GeoEnsemblesElectriqueByRegionAndUser);
                RaisePropertyChanged(() => PortionsIntegriteAction);
                RaisePropertyChanged(() => IsPortionIntegriteAnActionNonValide);
                RaiseAllIsEnableProperties();
                registerPropertyChanged();
                RefreshPrintButton();
            };

            OnSaveSuccess += (o, e) =>
            {
                RaisePropertyChanged(() => SelectedEntity);
                RaisePropertyChanged(() => IsInfoAffiche);
                RaisePropertyChanged(() => IsReintegrateEnable);
                RaisePropertyChanged(() => IsDeleteEnable);
                RaisePropertyChanged(() => IsActionHorsAnalyse);
                RefreshPrintButton();
            };

            OnAllServicesLoaded += (o, e) =>
            {
                AnAction.CurrentUser = userService.CurrentUser;
                if (SelectedEntity != null && SelectedEntity.ParametreAction != null)
                {
                    SelectedEntity.FiltreCleCategorie = SelectedEntity.ParametreAction.EnumCategorieAnomalie;
                }

                RaisePropertyChanged(() => Regions);
                RaisePropertyChanged(() => RegionsByUser);
                RaisePropertyChanged(() => ListNiveauxPriorite);
                RaisePropertyChanged(() => ListEtatsAnalyse);
                RaisePropertyChanged(() => ListStatuts);
                RaisePropertyChanged(() => ListStatuts2);
                RaisePropertyChanged(() => ListCategories);
                RaisePropertyChanged(() => ListTypeEval);
                RaisePropertyChanged(() => ListEntiteDeTraitement);
                RaisePropertyChanged(() => ListCategorieTypesAction);
                RaisePropertyChanged(() => ListDelaiReal);
                RaisePropertyChanged(() => GeoEnsemblesElectrique);
                RefreshPrintButton();

                RaisePropertyChanged(() => ListUsersAction);
                ((UsrUtilisateurService)ServiceUsers).FindUsrUtilisateurbyInternalCriterias(false, UsrUtilisateurLoaded);
            };
        }

        private void OnActionRegionSelected(object sender, EventArgs e)
        {

            RaisePropertyChanged(() => GeoEnsemblesElectriqueByRegionAndUser);
            RefreshPrintButton();

        }

        private void OnActionEnsembleElectriqueSelected(object sender, EventArgs e)
        {

            RaisePropertyChanged(() => CleEnsembleElec);
            if (SelectedEntity != null)
            {
                if (SelectedEntity.CleEnsembleElec.HasValue && SelectedEntity.CleRegion.HasValue && SelectedEntity.CleEnsembleElec > 0)
                {
                    var intCleEns = SelectedEntity.CleEnsembleElec.Value;
                    ((PortionIntegriteService)ServicePortionIntegrite).GetListPortionsByCleEnsElec(intCleEns, SelectedEntity.CleRegion.Value, GetListPortionsDone);
                }
                else
                {
                    PortionsIntegriteAction = new List<PortionIntegrite>();
                    if (SelectedEntity.PortionIntegriteAnAction.Any())
                    {
                        var listePortionsASupprimer = SelectedEntity.PortionIntegriteAnAction.ToList();
                        foreach (var portionIntegriteAnAction in listePortionsASupprimer)
                        {
                            SelectedEntity.PortionIntegriteAnAction.Remove(portionIntegriteAnAction);
                        }
                    }
                }
                RaisePropertyChanged(() => PortionsIntegriteAction);
            }

        }

        /// <summary>
        ///     La liste des portions de l'ensemble électrique vient d'être chargée
        /// </summary>
        /// <param name="error"></param>
        /// <param name="listPortions"></param>
        private void GetListPortionsDone(Exception error, List<PortionIntegrite> listPortions)
        {
            IsBusy = false;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError,
                    typeof(GeoRegion).Name));
            }
            else
            {
                // Ajouter les portions IsSelected
                foreach (var clePortion in SelectedEntity.PortionIntegriteAnAction.Select(p => p.ClePortion).ToList())
                {
                    var portion = listPortions.FirstOrDefault(p => p.ClePortion == clePortion);
                    if (portion != null)
                    {
                        portion.IsSelected = true;
                    }
                }
                if (IsCleRegionEnable)
                {
                    PortionsIntegriteAction = listPortions;
                    if (IsNewMode)
                    {
                        foreach (PortionIntegrite p in PortionsIntegriteAction)
                        {
                            p.IsSelected = true;
                        }
                    }
                }
                else
                {
                    PortionsIntegriteAction = listPortions.Where(p => p.IsSelected).ToList();
                }
            }
        }

        private void UsrUtilisateurLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError,
                    typeof(UsrUtilisateur).Name));
            }
            else
            {
                RaisePropertyChanged(() => ListUsers);
                RaisePropertyChanged(() => ListUsersAction);

                // Si on est en création d'action par analyse, il était nécessaire d'attendre le chargement de la liste des utilisateurs avant d'ajouter une nouvelle entité 
                if (IsPopupMode)
                {
                    Add();
                }
            }
        }

        private void RaiseAllIsEnableProperties()
        {
            RaisePropertyChanged(() => IsCleRegionEnable);
            RaisePropertyChanged(() => IsCleUtilisateurResponsableEnable);
            RaisePropertyChanged(() => IsCleUtilisateurAgentEnable);
            RaisePropertyChanged(() => IsQuantiteEnable);
            RaisePropertyChanged(() => IsFiltreCleCategorieEnable);
            RaisePropertyChanged(() => IsParametreActionEnable);
            RaisePropertyChanged(() => IsConstatAnomalieEnable);
            RaisePropertyChanged(() => IsDescriptionEnable);
            RaisePropertyChanged(() => IsProgrammeBudgetaireEnable);
            RaisePropertyChanged(() => IsEntiteTraitementEnable);
            RaisePropertyChanged(() => IsEnumStatutEnable);
            RaisePropertyChanged(() => IsDateClotureEnable);
            RaisePropertyChanged(() => IsCommentaireStatutEnable);
            RaisePropertyChanged(() => IsCoutGlobalReelEnable);
            RaisePropertyChanged(() => IsTempsTravailGlobalReelEnable);
            RaisePropertyChanged(() => IsDateDebutEnable);
            RaisePropertyChanged(() => IsDateRealisationTravauxEnable);
            RaisePropertyChanged(() => IsDateFinEnable);
            RaisePropertyChanged(() => IsDescriptionEnable);
            RaisePropertyChanged(() => IsTypeEvalEnable);
            RaisePropertyChanged(() => LabelCleRegion);
            RaisePropertyChanged(() => LabelCleUtilisateurResponsable);
            RaisePropertyChanged(() => LabelQuantite);
            RaisePropertyChanged(() => LabelFiltreCleCategorie);
            RaisePropertyChanged(() => LabelParametreAction);
            RaisePropertyChanged(() => LabelConstatAnomalie);
            RaisePropertyChanged(() => LabelDescription);
            RaisePropertyChanged(() => LabelProgrammeBudgetaire);
            RaisePropertyChanged(() => LabelEntiteTraitement);
            RaisePropertyChanged(() => LabelEnumStatut);
            RaisePropertyChanged(() => LabelDateCloture);
            RaisePropertyChanged(() => LabelCoutGlobalReel);
            RaisePropertyChanged(() => LabelTempsTravailGlobalReel);
            RaisePropertyChanged(() => LabelDateDebut);
            RaisePropertyChanged(() => LabelDateFin);
            RaisePropertyChanged(() => LabelCommentaireStatut);
            RaisePropertyChanged(() => LabelDescription);
            RaisePropertyChanged(() => LabelCleUtilisateurAgent);
            RaisePropertyChanged(() => LabelCleEnsembleElec);
            RaisePropertyChanged(() => LabelCommentaire);
            RaisePropertyChanged(() => LabelDateRealisationTravaux);
            RaisePropertyChanged(() => LabelPortionsIntegriteAction);
            RaisePropertyChanged(() => LabelTypeEval);
            RaisePropertyChanged(() => IsCommentaireEnable);
        }

        #endregion Constructor

        #region Commands

        // Boutons générique
        public IActionCommand RestitutionCommand { get; set; }

        // Bouton specifique
        public IActionCommand FicheActionCommand { get; set; }

        /// <summary>
        ///     Commande de réintégration
        /// </summary>
        public IActionCommand ReintegrateActionCommand { get; private set; }

        #endregion Commands

        #region Override Methods

        /// <summary>
        ///     Surcharge de la méthode SetCanProperties
        ///     Pour les droits de réintégration d'une action
        /// </summary>
        protected override void SetCanProperties()
        {
            base.SetCanProperties();
            ReintegrateActionCommand.RaiseCanExecuteChanged();
        }


        /// <summary>
        ///     Méthode de recherche appellé par la commande FindCommand
        ///     cette méthode appelle la méthode Find du service
        ///     pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            if (DateMin.HasValue && DateMax.HasValue && DateMin.Value.Date > DateMax.Value.Date)
            {
                ErrorWindow.CreateNew(Resource.SaisieVisite_SearchErrorDate);
            }
            else
            {
                IsBusy = true;

                saveGeoPreferences();

                ((ActionService)service).FindAnActionByCriterias(
                    FiltreCleRegion, FiltreCleAgence, FiltreCleSecteur, FiltreCleEnsElec,
                    FiltreCleAnalyse, FiltreClePriorite, FiltreCleStatut, FiltreCleUser,
                    DateMin, DateMax, IncludeDeletedAction, SearchDone);
            }
            RefreshPrintButton();
        }

        /// <summary>
        ///     Surcharge de la méthode Delete du service
        /// </summary>
        protected override void Delete()
        {
            var result = MessageBox.Show(Resource.FicheAction_DeleteConfirmation, "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                IsBusy = true;

                // Récupération des équipements
                ((ActionService)service).LogicalDeleteAction(SelectedEntity.CleAction, CurrentUser.CleUtilisateur,
                    error =>
                    {
                        IsBusy = false;
                        if (error != null)
                        {
                            Logger.Log(LogSeverity.Error, GetType().FullName, error.ToString());
                            ErrorWindow.CreateNew(Resource.Action_ErrorOnDelete);
                        }
                        else
                        {
                            //Rechargement de l'entité pour que l'écran soit mis à jour
                            LoadDetailEntity();
                        }
                    });
            }
            else
            {
                IsBusy = false;
            }
        }

        /// <summary>
        ///     Surcharge du save
        /// </summary>
        /// <param name="forceSave"></param>
        protected override void Save(bool forceSave)
        {
            SelectedEntity.CleUtilisateurModification = CurrentUser.CleUtilisateur;
            SelectedEntity.DateModification = DateTime.Now;

            //Si création d'action par analyse
            if (IsPopupMode)
            {
                // Validation de l'entité
                SelectedEntity.ValidationErrors.Clear();
                var errors = new Collection<ValidationResult>();
                var isValid = Validator.TryValidateObject(SelectedEntity,
                    new ValidationContext(SelectedEntity, null, null), errors, true);

                if (!isValid)
                {
                    foreach (var err in errors)
                    {
                        SelectedEntity.ValidationErrors.Add(err);
                    }
                    NotifyError = errors.Any() || HasFormatValidationError;
                }
                else
                {
                    NotifyError = false;

                    // Instanciation de la liste de secteur à renvoyer
                    var actionAEnvoyer = SelectedEntity;

                    // Publication de la liste
                    EventAggregator.Publish(actionAEnvoyer);

                    base.Save(forceSave);

                    // Fermeture de la popup
                    ChildWindow.DialogResult = true;
                }
            }
            // Sinon action hors analyse
            else
            {
                //mise à jour de la liste des portions intégrité sélectionnées si action hors analyse
                IEnumerable<PortionIntegrite> portionsSelected;
                if (PortionsIntegriteAction != null)
                {
                    portionsSelected = PortionsIntegriteAction.Where(pia => pia.IsSelected);

                    // Supprimer les portions !(IsSelected)
                    if (SelectedEntity.PortionIntegriteAnAction.Any())
                    {
                        var listePortionsASupprimer =
                            SelectedEntity.PortionIntegriteAnAction.Where(
                                pa => portionsSelected.All(p => p.ClePortion != pa.ClePortion)).ToList();
                        foreach (var portionIntegriteAnAction in listePortionsASupprimer)
                        {
                            SelectedEntity.PortionIntegriteAnAction.Remove(portionIntegriteAnAction);
                        }
                    }

                    // Ajouter les portions IsSelected
                    foreach (var portion in portionsSelected)
                    {
                        // Si la portion n'existe pas déjà on l'ajoute
                        if (SelectedEntity.PortionIntegriteAnAction.All(pa => pa.ClePortion != portion.ClePortion))
                        {
                            SelectedEntity.PortionIntegriteAnAction.Add(new PortionIntegriteAnAction
                            {
                                CleAction = SelectedEntity.CleAction,
                                ClePortion = portion.ClePortion
                            });
                        }
                    }

                    var region = this.RegionsByUser.FirstOrDefault(r => r.CleRegion == SelectedEntity.CleRegion);
                    if (region != null)
                    {
                        SelectedEntity.CodeRegion = region.CodeRegion;
                    }
                }


                SelectedEntity.ForceValidationOnCleEnsembleElec(SelectedEntity.CleEnsembleElec);
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs("CleEnsembleElec"));


                base.Save(forceSave);
            }

            //// Ajout de la validation sur le filtreclecategorie
            if (IsEditMode && SelectedEntity.FiltreCleCategorie == null || SelectedEntity.FiltreCleCategorie == 0)
            {
                SelectedEntity.ValidationErrors.Add(
                    new ValidationResult(ValidationErrorResources.DefaultRequiredFieldErrorMessage,
                        new List<string> { "FiltreCleCategorie" }));
            }

            RaisePropertyChanged(() => IsPortionIntegriteAnActionNonValide);

            RefreshPrintButton();
        }

        /// <summary>
        ///     RaisePropertyChanged to refresh Print Button Visibility
        /// </summary>
        private void RefreshPrintButton()
        {
            RaisePropertyChanged(() => IsNotPrintableMode);
        }


        protected override void Cancel()
        {
            SelectedEntity.ValidationErrors.Clear();
            if (IsPopupMode)
            {
                NotifyError = false;
                ChildWindow.DialogResult = false;
                this.IsEditMode = false;

                this.Entities.Remove(this.SelectedEntity);
            }
            else
            {
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs("CleEnsembleElec"));
            }
            base.Cancel();
        }

        /// <summary>
        ///     La recherche des ensemble électrique vient être terminée
        /// </summary>
        /// <param name="error"></param>
        private void SearchDone(Exception error)
        {
            RaisePropertyChanged(() => Entities);
            RaisePropertyChanged(() => EntitiesCount);
            RaisePropertyChanged(() => ResultIndicator);

            if (IsAutoNavigateToFirst && Entities != null && Entities.Any())
            {
                var cleAction = (int)Entities.First().GetCustomIdentity();
                if (SelectedEntity != null && SelectedEntity.CleAction == cleAction)
                {
                    NavigateToId((int)this.SelectedEntity.GetCustomIdentity(), true);
                    IsBusy = false;
                }
                else
                {
                    NavigationService.Navigate(cleAction);
                }
            }
            else if (Entities == null || !Entities.Any())
            {
                SelectedEntity = null;
                RaisePropertyChanged(() => CleEnsembleElec);
                this.PortionsIntegriteAction = new List<PortionIntegrite>();
                NavigationService.NavigateRootUrl();
            }

            IsBusy = false;
        }


        /// <summary>
        ///     Initialisation des préférence de l'ensemble électrique et de la portion
        /// </summary>
        protected override void initGeoPreferences()
        {
            base.initGeoPreferences();

            if (userService.CurrentUser != null)
            {
                FiltreCleEnsElec = userService.CurrentUser.PreferenceCleEnsembleElectrique;
            }
        }

        /// <summary>
        ///     Enregistrement des préférences de l'ensemble électrique et de la portion
        /// </summary>
        protected override void saveGeoPreferences()
        {
            base.saveGeoPreferences();
            if (userService.CurrentUser != null)
            {
                userService.CurrentUser.SetPreferenceCleEnsembleElectrique(FiltreCleEnsElec);

                if (FiltreCleEnsElec != userService.CurrentUser.PreferenceCleEnsembleElectrique)
                {
                    userService.CurrentUser.SetPreferenceClePortion(null);
                }
            }
        }

        #endregion Override Methods

        #region Private Methods

        private void registerPropertyChanged()
        {
            if (SelectedEntity != null)
            {
                SelectedEntity.PropertyChanged += SelectedEntity_PropertyChanged;
            }
        }

        void SelectedEntity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FiltreCleCategorie")
            {
                RaisePropertyChanged(() => this.ListCategorieTypesAction);
            }
            else if (e.PropertyName == "CleRegion")
            {
                if (this.IsEditMode)
                {
                    if (OnRegionActionSelected != null)
                    {
                        OnRegionActionSelected(this, null);
                    }
                }
            }
            else if (e.PropertyName == "CleEnsembleElec")
            {
                if (this.IsEditMode)
                {
                    if (OnEnsembleElectriqueActionSelected != null)
                    {
                        OnEnsembleElectriqueActionSelected(this, null);
                    }
                }
            }
        }

        /// <summary>
        ///     Réintégration de l'équipement
        /// </summary>
        protected virtual void ReintegrateAction()
        {
            IsBusy = true;

            // Récupération des équipements
            ((ActionService)service).ReintegrateAction(SelectedEntity.CleAction, CurrentUser.CleUtilisateur, error =>
            {
                IsBusy = false;
                if (error != null)
                {
                    Logger.Log(LogSeverity.Error, GetType().FullName, error.ToString());
                    ErrorWindow.CreateNew(Resource.Action_ErrorOnDelete);
                }
                else
                {
                    //Rechargement de l'entité pour que l'écran soit mis à jour
                    LoadDetailEntity();
                }
            });
        }

        /// <summary>
        ///     Affiche le rapport demandé
        /// </summary>
        private void ShowRestitution(object fileName)
        {
            if (fileName is string)
            {
                var rapportUrl = Rapports.printDocumentUrl;

                var urlDetail = fileName as string;

                var cle_region = userService.CurrentUser.PreferenceCleRegion ?? 0;
                var cle_agence = userService.CurrentUser.PreferenceCleAgence ?? 0;
                var cle_secteur = userService.CurrentUser.PreferenceCleSecteur ?? 0;
                var cle_EE = userService.CurrentUser.PreferenceCleEnsembleElectrique ?? 0;
                var cle_portion = userService.CurrentUser.PreferenceClePortion ?? 0;
                rapportUrl += string.Format(urlDetail, cle_region, cle_agence, cle_secteur, cle_EE, cle_portion);

                HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
            }
        }

        /// <summary>
        ///     Affiche le rapport fiche action demandé
        /// </summary>
        private void ShowFicheAction(object fileName)
        {
            if (fileName is string)
            {
                var rapportUrl = Rapports.printDocumentUrl;

                var urlDetail = fileName as string;

                var cle_action = SelectedEntity.CleAction;
                rapportUrl += string.Format(urlDetail, cle_action);

                HtmlPage.Window.Navigate(new Uri(rapportUrl, UriKind.Relative), "_blank");
            }
        }

        #endregion

        #region Autorisations

        /// <summary>
        ///     Détermine les droist de l'utilisateur courant pour l'ajout
        /// </summary>
        /// <returns></returns>
        protected override bool GetUserCanAdd()
        {
            if (CurrentUser != null)
            {
                var role =
                    CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.CRE_FICHE_ACTION_NIV);
                var codePortee = role.RefUsrPortee.CodePortee;

                if (codePortee != RefUsrPortee.ListPorteesEnum.Interdite.GetStringValue())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Détermine les droist de l'utilisateur courant
        ///     sur la suppression d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanDelete()
        {
            if (CurrentUser != null && SelectedEntity != null &&
                (SelectedEntity.AnAnalyse is AnAnalyseEe || SelectedEntity.PortionIntegriteAnAction.Any()) &&
                SelectedEntity.CleEnsembleElec.HasValue)
            {
                var role =
                    CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.SUP_FICHE_ACTION_NIV);

                return AnAction.VerifierDroitPortee(CurrentUser, role, GeoEnsembleElectriquesAction);
            }
            return false;
        }

        /// <summary>
        ///     Détermine les droits de l'utilisateur courant
        ///     sur l'édition d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanEdit()
        {
            var canEdit = false;

            if (CurrentUser != null && SelectedEntity != null &&
                (SelectedEntity.AnAnalyse is AnAnalyseEe || ((SelectedEntity.PortionIntegriteAnAction.Any()) && SelectedEntity.CleEnsembleElec.HasValue)))
            {
                canEdit = SelectedEntity.CanEditAction(CurrentUser, GeoEnsembleElectriquesAction);
            }
            return canEdit;
        }

        /// <summary>
        ///     Détermine les droits de l'utilisateur courant
        ///     sur la sauvegarde d'un ensemble électrique
        /// </summary>
        /// <returns>true si il a les droits false sinon</returns>
        protected override bool GetUserCanSave()
        {
            if (CurrentUser != null && SelectedEntity != null &&
                (SelectedEntity.AnAnalyse is AnAnalyseEe || (SelectedEntity.PortionIntegriteAnAction.Any() && SelectedEntity.CleEnsembleElec.HasValue)))
            {
                if (SelectedEntity.IsNew())
                {
                    return GetUserCanAdd();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///     MANTIS 10815, 06/05/14, FSI : Droits spécifiques pour les utilisateurs externes
        ///     Retourne si true l'utilisateur n'est pas prestataire
        /// </summary>
        /// <returns></returns>
        protected override bool GetUserCanRead()
        {
            return CurrentUser != null && !CurrentUser.EstPresta;
        }

        #endregion Autorisations

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return SelectedEntity != null && SelectedEntity.ValidationErrors != null ? this.SelectedEntity.ValidationErrors.Where(e => e.MemberNames.Any(m => m == propertyName)) : null;
        }

        public bool HasErrors
        {
            get
            {
                return SelectedEntity != null ? this.SelectedEntity.HasValidationErrors : false;
            }
        }
    }
}