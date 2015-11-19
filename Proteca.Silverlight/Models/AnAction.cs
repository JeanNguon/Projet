using Jounce.Core.Model;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Resources;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Proteca.Web.Models
{
    public partial class AnAction
    {

        /// <summary>
        /// Liste des champs éditables pour chaque couple profil/statut
        /// </summary>
        public static List<String> ListeChampsEnCreation;

        public static List<String> ListeChampsAPlanifierResponsableAction;
        public static List<String> ListeChampsAPlanifierResponsablePC;
        public static List<String> ListeChampsEnCoursResponsableAction;
        public static List<String> ListeChampsACloturerResponsablePC;

        /// <summary>
        /// Liste des champs obligatoires pour chaque couple profil/statut
        /// </summary>
        public static List<String> ListeChampsObligatoiresCréation;
        public static List<String> ListeChampsObligatoiresAPlanifierResponsableAction;
        public static List<String> ListeChampsObligatoiresAPlanifierResponsablePC;
        public static List<String> ListeChampsObligatoiresEnCoursRespAction;
        public static List<String> ListeChampsObligatoiresEnCoursRespPC;
        public static List<String> ListeChampsObligatoiresACloturerResponsableAction;
        public static List<String> ListeChampsObligatoiresACloturerResponsablePC;
        public static List<String> ListeChampsObligatoiresTermine;

        /// <summary>
        /// Liste des champs uniquement disponibles pour les actions hors analyse
        /// </summary>
        public static List<String> ListeChampsHorsAnalyseUniquement;

        static AnAction()
        {

            //Définition de la liste des champs disponible pour chaque statut/profil
            var act = new AnAction();

            ListeChampsEnCreation = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.CleRegion),
                                BaseNotify.ExtractPropertyName(() => act.CleEnsembleElec),
                                BaseNotify.ExtractPropertyName(() => act.PortionIntegriteAnAction),
                                BaseNotify.ExtractPropertyName(() => act.FiltreCleCategorie),
                                BaseNotify.ExtractPropertyName(() => act.ConstatAnomalie),
                                BaseNotify.ExtractPropertyName(() => act.ParametreAction),
                                BaseNotify.ExtractPropertyName(() => act.TypeEval),
                                BaseNotify.ExtractPropertyName(() => act.CleUtilisateurAgent)
            };

            ListeChampsAPlanifierResponsableAction = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.Description),
                                BaseNotify.ExtractPropertyName(() => act.DateDebut),
                                BaseNotify.ExtractPropertyName(() => act.DateRealisationTravaux), // ?
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut),
                                BaseNotify.ExtractPropertyName(() => act.CommentaireStatut),
                                BaseNotify.ExtractPropertyName(() => act.DateCloture),
                                BaseNotify.ExtractPropertyName(() => act.TypeEval),
                                BaseNotify.ExtractPropertyName(() => act.Commentaire)
            };

            ListeChampsAPlanifierResponsablePC = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut),
                                BaseNotify.ExtractPropertyName(() => act.CleUtilisateurResponsable),
                                BaseNotify.ExtractPropertyName(() => act.Quantite),
                                BaseNotify.ExtractPropertyName(() => act.FiltreCleCategorie),
                                BaseNotify.ExtractPropertyName(() => act.ParametreAction),
                                BaseNotify.ExtractPropertyName(() => act.ConstatAnomalie),
                                BaseNotify.ExtractPropertyName(() => act.Description),
                                BaseNotify.ExtractPropertyName(() => act.ProgrammeBudgetaire),
                                BaseNotify.ExtractPropertyName(() => act.EntiteTraitement),
                                BaseNotify.ExtractPropertyName(() => act.TypeEval),
                                BaseNotify.ExtractPropertyName(() => act.Commentaire)
            };

            ListeChampsEnCoursResponsableAction = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.Description),
                                BaseNotify.ExtractPropertyName(() => act.DateDebut),
                                BaseNotify.ExtractPropertyName(() => act.DateRealisationTravaux),
                                BaseNotify.ExtractPropertyName(() => act.CommentaireStatut),
                                BaseNotify.ExtractPropertyName(() => act.DateCloture),
                                BaseNotify.ExtractPropertyName(() => act.TempsTravailGlobalReel),
                                BaseNotify.ExtractPropertyName(() => act.CoutGlobalReel),
                                BaseNotify.ExtractPropertyName(() => act.CoutGlobalReel2),
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut),
                                BaseNotify.ExtractPropertyName(() => act.TypeEval),
                                BaseNotify.ExtractPropertyName(() => act.Commentaire)
            };

            ListeChampsACloturerResponsablePC = new List<string>() {
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut),
                                BaseNotify.ExtractPropertyName(() => act.TypeEval),
                                BaseNotify.ExtractPropertyName(() => act.Commentaire)
            };

            //Définition de la liste des champs obligatoire pour chaque statut/profil
            ListeChampsObligatoiresCréation = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.CleRegion),
                                BaseNotify.ExtractPropertyName(() => act.CleEnsembleElec),
                                BaseNotify.ExtractPropertyName(() => act.PortionIntegriteAnAction),
                                BaseNotify.ExtractPropertyName(() => act.FiltreCleCategorie),
                                BaseNotify.ExtractPropertyName(() => act.ConstatAnomalie),
                                BaseNotify.ExtractPropertyName(() => act.ParametreAction),
                                BaseNotify.ExtractPropertyName(() => act.CleUtilisateurAgent)
            };

            ListeChampsObligatoiresAPlanifierResponsableAction = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.DateRealisationTravaux),
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut)
            };

            ListeChampsObligatoiresAPlanifierResponsablePC = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.CleUtilisateurResponsable),
                                BaseNotify.ExtractPropertyName(() => act.EntiteTraitement),
                                BaseNotify.ExtractPropertyName(() => act.Quantite)
            };

            ListeChampsObligatoiresEnCoursRespAction = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.DateRealisationTravaux),
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut)
            };

            ListeChampsObligatoiresEnCoursRespPC = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.CleUtilisateurResponsable),
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut),
                                BaseNotify.ExtractPropertyName(() => act.Quantite)
            };

            ListeChampsObligatoiresACloturerResponsableAction = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.DateRealisationTravaux),
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut),
                                BaseNotify.ExtractPropertyName(() => act.DateCloture)
            };

            ListeChampsObligatoiresACloturerResponsablePC = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut)
            };

            ListeChampsObligatoiresTermine = new List<string>(){
                                BaseNotify.ExtractPropertyName(() => act.EnumStatut)    

            };

            ListeChampsHorsAnalyseUniquement = new List<string>()
            {
                BaseNotify.ExtractPropertyName(() => act.CleRegion),
                BaseNotify.ExtractPropertyName(() => act.CleEnsembleElec),
                BaseNotify.ExtractPropertyName(() => act.PortionIntegriteAnAction)
            };
        }



        private Nullable<int> _filtreCleCategorie;

        /// <summary>
        /// Filtre clé catégorie d'anomalie
        /// </summary>
        public Nullable<int> FiltreCleCategorie
        {
            get
            {
                if (_filtreCleCategorie == null && this.CleParametreAction != null && this.ParametreAction != null)
                {
                    _filtreCleCategorie = this.ParametreAction.EnumCategorieAnomalie;
                }
                return _filtreCleCategorie;
            }
            set
            {
                _filtreCleCategorie = value;
                this.RaisePropertyChanged("FiltreCleCategorie");
            }
        }

        /// <summary>
        /// Vrai si action hors analyse
        /// </summary>
        public bool IsActionHorsAnalyse
        {
            get { return this.AnAnalyse == null; }
        }

        /// <summary>
        /// Url de l'élément courant
        /// </summary>
        public string NaviagtionUrl
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "/{0}/{1}/Id={2}",
                   MainNavigation.Visite.GetStringValue(),
                   VisiteNavigation.FicheAction.GetStringValue(),
                   CleAction);
            }
        }

        /// <summary>
        /// Si la fiche action a été supprimée on affiche un message sinon rien
        /// </summary>
        public string InfosAction
        {
            get
            {
                if (this.Supprime)
                {
                    return Resource.Action_Supprimee;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string Libelle
        {
            get
            {
                return string.Format(Proteca.Silverlight.Resources.Resource.FicheAction_Libelle,
                    (this.CleParametreAction.HasValue) ? Proteca.Silverlight.Resources.Resource.FicheAction_PC + " " : String.Empty,
                    this.NumActionPc);
            }
        }

        public decimal CoutGlobalReel2
        {
            get
            {
                if (ParametreAction != null)
                {
                    return (this.CleParametreAction.HasValue && this.ParametreAction.Cout.HasValue && this.Quantite.HasValue) ?
                        (this.ParametreAction.Cout.Value * this.Quantite.Value) : Decimal.Zero;
                }
                else
                {
                    return Decimal.Zero;
                }
            }
        }

        public decimal TempsTravailGlobalReel2
        {
            get
            {
                if (this.ParametreAction != null)
                {
                    return (this.CleParametreAction.HasValue && this.ParametreAction.Temps.HasValue && this.Quantite.HasValue) ?
                        (this.ParametreAction.Temps.Value * this.Quantite.Value) : Decimal.Zero;
                }
                else
                {
                    return Decimal.Zero;
                }
            }
        }

        public string Historique
        {
            get
            {
                if (CleAction != 0)
                {
                    return string.Format(Proteca.Silverlight.Resources.Resource.FicheAction_HistoCreation, this.DateCreation.ToString(Proteca.Silverlight.Resources.Resource.DateFormat), this.UsrUtilisateur.Prenom + " " + this.UsrUtilisateur.Nom) +
                        ((this.DateModification.HasValue) ? "\n" + string.Format(Proteca.Silverlight.Resources.Resource.FicheAction_HistoComplet, this.DateModification.Value.ToString(Proteca.Silverlight.Resources.Resource.DateFormat), this.UsrUtilisateur1.Prenom + " " + this.UsrUtilisateur1.Nom) : String.Empty);
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string DateFin
        {
            get
            {
                if (this.CleParametreAction.HasValue && this.ParametreAction != null)
                {
                    int number = 0;
                    string[] elements = this.ParametreAction.RefEnumValeur2.Valeur.Split(' ');
                    if (int.TryParse(elements[0], out number))
                    {
                        switch (elements[1])
                        {
                            case "j":
                                return this.DateCreation.AddDays(number).ToString(Proteca.Silverlight.Resources.Resource.DateFormat);
                            case "m":
                                return this.DateCreation.AddMonths(number).ToString(Proteca.Silverlight.Resources.Resource.DateFormat);
                            case "a":
                                return this.DateCreation.AddYears(number).ToString(Proteca.Silverlight.Resources.Resource.DateFormat);
                            default:
                                break;
                        }
                    }
                    return "Valeur erronnée";
                }
                else
                {
                    return String.Empty;
                }
            }
        }


        public string DateRapport
        {
            get { return AnAnalyse != null && AnAnalyse.DateEdition.HasValue ? AnAnalyse.DateEdition.Value.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture) : null; }
        }

        public string NumeroRapport
        {
            get { return AnAnalyse != null && AnAnalyse is AnAnalyseEe ? ((AnAnalyseEe)AnAnalyse).RefRapportAction : null; }
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e != null)
            {
                base.OnPropertyChanged(e);

                if (e.PropertyName == "NumActionPc")
                {
                    this.RaisePropertyChanged("Libelle");
                }
                if (e.PropertyName == "Quantite")
                {
                    this.RaisePropertyChanged("CoutGlobalReel2");
                    this.RaisePropertyChanged("TempsTravailGlobalReel2");
                }
                if (e.PropertyName == "CleParametreAction")
                {
                    this.RaisePropertyChanged("DateFin");
                    this.RaisePropertyChanged("CoutGlobalReel2");
                    this.RaisePropertyChanged("TempsTravailGlobalReel2");
                }
            }
        }

        /// <summary>
        /// Retourne vrai si l'utilisateur en paramètre est du profil Responsable PC
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <param name="geoEns"></param>
        /// <returns></returns>

        private static bool IsUtilisateurResponsablePc(UsrUtilisateur utilisateur, ReadOnlyCollection<GeoEnsembleElectrique> geoEns)
        {

            var role = utilisateur.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GES_FICHE_ACTION_NIV);

            return VerifierDroitPortee(utilisateur, role, geoEns);
        }

        /// <summary>
        /// Vérification des droits par rapport à un role, à l'ensemble électrique et à la portee 
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <param name="role"></param>
        /// <param name="geoEns"></param>
        /// <returns></returns>
        public static bool VerifierDroitPortee(UsrUtilisateur utilisateur, UsrRole role, ReadOnlyCollection<GeoEnsembleElectrique> geoEns)
        {
            bool canEdit = false;

            if (geoEns == null || role == null)
            {
                canEdit = true;
            }
            else
            {
                string codePortee = role.RefUsrPortee.CodePortee;

                if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                {
                    canEdit = geoEns.Any(g => g.CleAgence == utilisateur.CleAgence);
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                    codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                {
                    canEdit = true;
                }

                else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                {
                    canEdit = geoEns.Any(g => g.CleRegion == utilisateur.GeoAgence.CleRegion);
                }
                else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                {
                    canEdit = geoEns.Any(g => g.CleAgence == utilisateur.CleAgence && g.CleSecteur == utilisateur.CleSecteur);
                }
            }
            return canEdit;
        }

        /// <summary>
        /// Liste des statuts possible pour une action
        /// </summary>
        private enum EnumStatutAction
        {
            APlanifier = 0,
            EnCours = 1,
            ACloturer = 2,
            Terminer = 3
        }

        /// <summary>
        /// Indique si le champ indiqué en paramètre est éditable par l'utilisateur
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="utilisateur"></param>
        /// <param name="geoEns"></param>
        /// <returns></returns>
        public bool CanEditField<T>(Expression<Func<T>> field, UsrUtilisateur utilisateur, ReadOnlyCollection<GeoEnsembleElectrique> geoEns)
        {
            bool canEdit = false;
            if (utilisateur != null)
            {
                List<string> fields = new List<string>();
                var cleUtilisateur = utilisateur.CleUtilisateur;
                int valeurStatut;
                if (this.RefEnumValeur != null)
                {

                    valeurStatut = int.Parse(this.RefEnumValeur.Valeur, CultureInfo.InvariantCulture);
                }
                else
                {
                    valeurStatut = (int)EnumStatutAction.APlanifier;
                }

                // Si on est en cours de création d'une action
                if (this.CleAction == 0 && valeurStatut == (int)EnumStatutAction.APlanifier)
                {
                    fields = fields.Union(ListeChampsEnCreation).ToList();
                }
                else
                {
                    if (this.CleUtilisateurResponsable == cleUtilisateur)
                    {
                        // Responsable Action
                        switch (valeurStatut)
                        {
                            case (int)EnumStatutAction.APlanifier:
                                fields = fields.Union(ListeChampsAPlanifierResponsableAction).ToList();
                                break;
                            case (int)EnumStatutAction.EnCours:
                                fields = fields.Union(ListeChampsEnCoursResponsableAction).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    if (IsUtilisateurResponsablePc(utilisateur, geoEns))
                    {
                        // Responsable PC ?
                        switch (valeurStatut)
                        {
                            case (int)EnumStatutAction.APlanifier:
                                fields = fields.Union(ListeChampsAPlanifierResponsablePC).ToList();
                                break;
                            case (int)EnumStatutAction.ACloturer:
                                fields = fields.Union(ListeChampsACloturerResponsablePC).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (!IsActionHorsAnalyse)
                {
                    fields.RemoveAll(f => ListeChampsHorsAnalyseUniquement.Any(l => l == f));

                }
                canEdit = CanEditField(fields, field);
            }
            return canEdit;
        }

        /// <summary>
        /// Retourne vrai si le champ en paramètre est obligatoire selon l'utilisateur
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        public bool IsRequiredField(string fieldName, UsrUtilisateur utilisateur = null)
        {
            if (utilisateur == null)
            {
                utilisateur = CurrentUser;
            }
            if (utilisateur != null)
            {
                List<string> fields = new List<string>();
                int valeurStatut = 0;
                if (this.RefEnumValeur != null)
                {
                    valeurStatut = int.Parse(this.RefEnumValeur.Valeur, CultureInfo.InvariantCulture);
                }

                //Création
                if (valeurStatut == (int)EnumStatutAction.APlanifier && this.CleAction == 0)
                {
                    fields = fields.Union(ListeChampsObligatoiresCréation).ToList();
                }

                var oldValue = this.GetOriginal() as AnAction;
                // Responsable Action
                if (oldValue != null && oldValue.CleUtilisateurResponsable == utilisateur.CleUtilisateur)
                {
                    //A planifier
                    if (valeurStatut == (int)EnumStatutAction.APlanifier && this.CleAction != 0)
                    {
                        fields = fields.Union(ListeChampsObligatoiresAPlanifierResponsableAction).ToList();
                    }
                    // En cours
                    else if (valeurStatut == (int)EnumStatutAction.EnCours)
                    {
                        fields = fields.Union(ListeChampsObligatoiresEnCoursRespAction).ToList();
                    }
                    // A Cloturer
                    else if (valeurStatut == (int)EnumStatutAction.ACloturer)
                    {
                        fields = fields.Union(ListeChampsObligatoiresACloturerResponsableAction).ToList();
                    }
                }

                // Responsable PC
                if (IsUtilisateurResponsablePc(utilisateur, null))
                {
                    if (valeurStatut == (int)EnumStatutAction.APlanifier && this.CleAction != 0)
                    {
                        //A planifier
                        fields = fields.Union(ListeChampsObligatoiresAPlanifierResponsablePC).ToList();
                    }
                    else if (valeurStatut == (int)EnumStatutAction.EnCours)
                    {
                        //En cours
                        fields = fields.Union(ListeChampsObligatoiresEnCoursRespPC).ToList();
                    }
                }

                if (!IsActionHorsAnalyse)
                {
                    fields.RemoveAll(f => ListeChampsHorsAnalyseUniquement.Any(l => l == f));
                }

                if (fields.Contains(fieldName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retourne vrai si le champ en paramètre est contenu dans la liste des champs en paramètre
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static bool CanEditField<T>(List<string> fields, Expression<Func<T>> field)
        {
            bool canEdit = false;
            var fieldName = BaseNotify.ExtractPropertyName(field);
            if (fields != null && fields.Any() && fields.Any(fi => fi == fieldName))
            {
                canEdit = true;
            }
            return canEdit;
        }

        /// <summary>
        ///  Retourne la liste des statuts en fonction du statut en cours
        /// </summary>
        /// <param name="listeStatuts"></param>
        /// <returns></returns>
        public List<RefEnumValeur> GetListeStatuts(List<RefEnumValeur> listeStatuts)
        {
            List<RefEnumValeur> listeStatutsFiltre = listeStatuts;
            if (this.RefEnumValeur != null)
            {
                int valeurStatut = this.RefEnumValeur.CleEnumValeur;

                switch (int.Parse(this.RefEnumValeur.Valeur, CultureInfo.InvariantCulture))
                {
                    case (int)EnumStatutAction.APlanifier:
                        // Si responsable action affecté
                        if (this.CleUtilisateurResponsable != null && this.CleUtilisateurResponsable == CurrentUser.CleUtilisateur)
                        {
                            listeStatutsFiltre = listeStatutsFiltre.Where(s => s.CleEnumValeur == valeurStatut || s.Valeur == EnumStatutActionToRefEnumValeur(EnumStatutAction.EnCours) || s.Valeur == EnumStatutActionToRefEnumValeur(EnumStatutAction.ACloturer)).ToList();
                        }
                        // Sinon en attente de responsable action
                        else
                        {
                            listeStatutsFiltre = listeStatutsFiltre.Where(s => s.CleEnumValeur == valeurStatut || s.Valeur == EnumStatutActionToRefEnumValeur(EnumStatutAction.EnCours)).ToList();
                        }
                        break;
                    case (int)EnumStatutAction.EnCours:
                        listeStatutsFiltre = listeStatutsFiltre.Where(s => s.CleEnumValeur == valeurStatut || s.Valeur == EnumStatutActionToRefEnumValeur(EnumStatutAction.ACloturer)).ToList();
                        break;
                    case (int)EnumStatutAction.ACloturer:
                        listeStatutsFiltre = listeStatutsFiltre.Where(s => s.CleEnumValeur == valeurStatut || s.Valeur == EnumStatutActionToRefEnumValeur(EnumStatutAction.EnCours) || s.Valeur == EnumStatutActionToRefEnumValeur(EnumStatutAction.Terminer)).ToList();
                        break;
                    case (int)EnumStatutAction.Terminer:
                        listeStatutsFiltre = listeStatutsFiltre.Where(s => s.CleEnumValeur == valeurStatut).ToList();
                        break;
                    default:
                        listeStatutsFiltre = listeStatutsFiltre.Where(s => s.Valeur == EnumStatutActionToRefEnumValeur(EnumStatutAction.APlanifier)).ToList();
                        break;
                }
            }
            else
            {
                listeStatutsFiltre = listeStatutsFiltre.Where(s => s.Valeur == EnumStatutActionToRefEnumValeur(EnumStatutAction.APlanifier)).ToList();
            }
            return listeStatutsFiltre;
        }

        private static string EnumStatutActionToRefEnumValeur(EnumStatutAction enumValue)
        {
            return ((int)enumValue).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Indique si l'utilisateur passé en paramètre peut éditer l'action
        /// Si statut = « A planifier » et utilisateur = Reponsable PC alors statuts = « A planifier, En cours » 
        /// Si statut = « A planifier » et utilisateur = Reponsable Action alors statuts = « En cours » 
        /// Si statut = « En cours» alors statuts = « En cours, A clôturer» 
        /// Si statut = « A clôturer» alors statuts = « En cours, A clôturer, Terminée » 
        /// Si statut = « Terminée› alors statuts = « Terminée » 
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <param name="geoEns"></param>
        /// <returns></returns>
        public bool CanEditAction(UsrUtilisateur utilisateur, ReadOnlyCollection<GeoEnsembleElectrique> geoEns)
        {
            bool canEdit = false;
            if (utilisateur != null && this.RefEnumValeur != null && geoEns != null)
            {
                // Vérification droit utilisateur sur le droit GES_FICHE_ACTION_NIV
                UsrRole role = utilisateur.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.GES_FICHE_ACTION_NIV);
                var cleUtilisateur = utilisateur.CleUtilisateur;

                // l'utilisateur responsable de l'action a toujours le droit d'éditer l'action même s'il n'a pas les droits applicatifs suffisant
                if (CleUtilisateurResponsable != cleUtilisateur)
                {
                    canEdit = VerifierDroitPortee(utilisateur, role, geoEns);
                }
                else
                {
                    canEdit = true;
                }

                if (canEdit)
                {
                    //Selon le workflow d'une action hors analyse les seuls le créateur, les resp. Action ou PC peuvent éditer en fonction du statut de l'action
                    switch (int.Parse(RefEnumValeur.Valeur, CultureInfo.InvariantCulture))
                    {
                        case (int) EnumStatutAction.APlanifier:
                            canEdit = CleUtilisateurResponsable == cleUtilisateur ||
                                      IsUtilisateurResponsablePc(utilisateur, geoEns);
                            break;
                        case (int) EnumStatutAction.EnCours:
                            canEdit = CleUtilisateurResponsable == cleUtilisateur;
                            break;
                        case (int) EnumStatutAction.ACloturer:
                            canEdit = IsUtilisateurResponsablePc(utilisateur, geoEns);
                            break;
                        case (int) EnumStatutAction.Terminer:
                            canEdit = false;
                            break;
                        default:
                            canEdit = CleUtilisateurAgent == cleUtilisateur;
                            break;
                    }
                }
            }
            return canEdit;
        }
    }
}
