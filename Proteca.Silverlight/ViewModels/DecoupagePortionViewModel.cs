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
using Proteca.Silverlight.Views.Windows;
using Proteca.Silverlight.Helpers;
using System.Windows;
using System.Collections.Generic;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using System.ServiceModel.DomainServices.Client;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for DecoupagePortion entity
    /// </summary>
    [ExportAsViewModel("DecoupagePortion")]
    public class DecoupagePortionViewModel : BaseProtecaEntityViewModel<PortionIntegrite>
    {

        #region Private Members

        /// <summary>
        /// Déclaration de variable FiltreCleEnsEle
        /// </summary>
        private int? _filtreCleEnsEle;

        #endregion Private Members

        #region Public Properties

        public string SavedPortionCible { get; set; }
        public string Erreur { get; set; }

        /// <summary>
        /// Retourne la clé EnsembleElectrique sélectionner
        /// </summary>
        public int? FiltreCleEnsElec
        {
            get { return _filtreCleEnsEle; }
            set
            {
                _filtreCleEnsEle = value;
                RaisePropertyChanged(() => this.FiltreCleEnsElec);
            }
        }

        /// <summary>
        /// Indique l'action en cours
        /// </summary>
        public bool ActionDecoupe { get; set; }

        /// <summary>
        /// Indique l'action en cours
        /// </summary>
        public bool ActionAssemblage { get; set; }

        /// <summary>
        /// Inversement de la PK pour l'assemblage
        /// </summary>
        public bool InversePK { get; set; }

        /// <summary>
        /// Collection of buddy portions to choose from.
        /// </summary>
        public ObservableCollection<PortionIntegrite> PortionsCibles
        {
            get
            {
                return new ObservableCollection<PortionIntegrite>(this.Entities.Where(e => e.ClePortion != this.SelectedId.Value).ToList());
            }
        }

        /// <summary>
        /// Retourne les régions du service région
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get
            {
                return new ObservableCollection<GeoRegion>(serviceRegion.Entities.OrderBy(r => r.LibelleRegion));
            }
        }

        /// <summary>
        /// Retourne les GEO ensembles électrique du service EnsElec
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
                            return serviceGeoEnsElec.Entities.Where(i => i.CleSecteur == FiltreCleSecteur).ToList();
                        }
                        return serviceGeoEnsElec.Entities.Where(i => i.CleAgence == FiltreCleAgence).Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                            {
                                return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                            })).ToList();
                    }
                    return serviceGeoEnsElec.Entities.Where(i => i.CleRegion == FiltreCleRegion).Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                            {
                                return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                            })).ToList();
                }
                else
                {
                    return serviceGeoEnsElec.Entities.Distinct(new InlineEqualityComparer<GeoEnsembleElectrique>((a, b) =>
                            {
                                return a.CleEnsElectrique.Equals(b.CleEnsElectrique) && a.Libelle.Equals(b.Libelle);
                            })).ToList();
                }
            }
        }


        #endregion Public Properties

        #region Services
        
        /// <summary>
        /// Import de la childwindows pour afficher une popup
        /// </summary>
        [Import(typeof(ChildWindowControl))]
        public ChildWindowControl ChildWindow;


        /// <summary>
        /// Service utilisé pour gérer l'entité de type Region
        /// </summary>
        [Import]
        public IEntityService<GeoRegion> serviceRegion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les GEO ensembles électriques
        /// </summary>
        [Import]
        public IEntityService<GeoEnsembleElectrique> serviceGeoEnsElec { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de type PortionIntegrite
        /// </summary>
        [Import]
        public IEntityService<PortionIntegrite> servicePortion { get; set; }


        #endregion

        #region Constructor

        public DecoupagePortionViewModel()
            : base()
        {

            this.OnRegionSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            };

            this.OnAgenceSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            };

            this.OnSecteurSelected += (o, e) =>
            {
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            };

            this.OnViewActivated += (o, e) =>
            {
                // Au chargement d'un écran composé d'un expander basé sur le même ViewModel, 
                // la présente méthode sera exécuté 2 fois, on teste donc si l'expander a déjà été charger.  
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", "Recherche des Portions Intégrités"));
                    EventAggregator.Publish("DecoupagePortion_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }

                // MAJ de la vue
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
                AssemblerCommand.RaiseCanExecuteChanged();
                DecouperCommand.RaiseCanExecuteChanged();
            };

            this.OnAllServicesLoaded += (o, e) =>
            {
                // MAJ des services
                RaisePropertyChanged(() => this.Regions);
                RaisePropertyChanged(() => this.GeoEnsemblesElectrique);
            };
            this.OnCanceled += (o, e) =>
            {
                RaisePropertyChanged(() => this.PortionsCibles);
                if (!String.IsNullOrEmpty(Erreur))
                {
                    MessageBox.Show(Erreur, "", MessageBoxButton.OK);
                }
                Erreur = "";
            };
            this.OnSaveError += (o, e) =>
            {
                this.SelectedEntity.ValidationErrors.Clear();

                if (ActionDecoupe)
                {
                    Erreur = "";

                    foreach (PortionIntegrite item in Entities.Where(ent => ent.HasValidationErrors))
                    {
                        foreach (ValidationResult vres in item.ValidationErrors)
                        {
                            foreach (string fld in vres.MemberNames)
                            {
                                if (fld == "Libelle")
                                {
                                    string LibelleAcontroler = "BIS " + SelectedEntity.Libelle;
                                    Erreur = string.Format(Resource.DecoupagePortion_Prompt_PortionExist, LibelleAcontroler);
                                    break;
                                }
                            }

                        }
                    }
                }
                //else
                //{
                //    StringBuilder sb = new StringBuilder();
                //    sb.AppendLine(Resource.Erreur_Donnees);
                //    sb.AppendLine(" - " + this.SelectedEntity.Libelle);
                //    sb.AppendLine(" - " + (this.SavedPortionCible ?? ""));
                //    Erreur = sb.ToString();
                //}

                StringBuilder sb = new StringBuilder();
                if (String.IsNullOrEmpty(Erreur))
                {
                    sb.AppendLine(Resource.Erreur_Donnees);
                }
                else
                {
                    sb.AppendLine(Erreur);
                }

                if (((PortionIntegriteService)service).CustomErrors.Any())
                {
                    sb.AppendLine("");
                    sb.AppendLine("Liste des erreurs : ");
                    sb.AppendLine("");
                }                
                foreach (String err in ((PortionIntegriteService)service).CustomErrors)
                {
                    sb.AppendLine(err);
                } 

                Erreur = sb.ToString();

                this.Cancel();
            };
            this.OnSaveSuccess += (o, e) =>
            {
                MessageBox.Show(Resource.DecoupagePortion_Prompt_SaveSuccess, "", MessageBoxButton.OK);
                this.IsEditMode = false;
            };
            this.OnDetailLoaded += (o, e) =>
            {
                this.SelectedEntity.ValidationErrors.Clear();
                RaisePropertyChanged(() => this.PortionsCibles);
            };

            // Define commands
            DecouperCommand = new ActionCommand<object>(
                obj => Decouper(), obj => GetAutorisation());
            AssemblerCommand = new ActionCommand<object>(
                obj => Assembler(), obj => GetAutorisation());
        }

        #endregion

        #region Command

        public IActionCommand DecouperCommand { get; protected set; }
        public IActionCommand AssemblerCommand { get; protected set; }

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

                if (this.FiltreCleEnsElec != userService.CurrentUser.PreferenceCleEnsembleElectrique)
                    userService.CurrentUser.SetPreferenceClePortion(null);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        protected override void DeactivateView(string viewName)
        {
            Router.DeactivateView("Selected_Secteur");
            base.DeactivateView(viewName);
        }

        /// <summary>
        /// 
        /// </summary>
        private void FinChargement()
        {
            // MAJ de la vue
            RaisePropertyChanged(() => this.SelectedEntity);
        }

        /// <summary>
        /// Affichage de la popup de sélection d'un secteur
        /// </summary>
        private void ShowDialog()
        {
            ChildWindow.Title = "Sélection de secteur(s)";
            ChildWindow.Show();
            EventAggregator.Publish("Selected_Secteur".AsViewNavigationArgs());
        }


        /// <summary>
        /// Méthode de recherche appellé par la commande FindCommand
        /// cette méthode appelle la méthode Find du service
        /// pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            IsBusy = true;

            this.saveGeoPreferences();

            ((PortionIntegriteService)this.service).FindPortionIntegritesByCriterias(
                this.FiltreCleRegion, this.FiltreCleAgence, this.FiltreCleSecteur, this.FiltreCleEnsElec, false, false,false, SearchDone);
        }

        /// <summary>
        /// La recherche des ensemble électrique vient être terminée
        /// </summary>
        /// <param name="error"></param>
        private void SearchDone(Exception error)
        {
            RaisePropertyChanged(() => this.Entities);
            RaisePropertyChanged(() => EntitiesCount);
            RaisePropertyChanged(() => ResultIndicator);

            if (IsAutoNavigateToFirst && this.Entities != null && this.Entities.Any())
            {
                int _clePortion = (int)Entities.First().GetCustomIdentity();
                if (this.SelectedEntity != null && this.SelectedEntity.ClePortion == _clePortion)
                {
                    this.IsBusy = false;
                }
                else
                {
                    NavigationService.Navigate(_clePortion);
                }
            }
            else if (this.Entities == null || !this.Entities.Any())
            {
                this.SelectedEntity = null;
                NavigationService.NavigateRootUrl();
            }

            this.IsBusy = false;
        }

        /// <summary>
        /// Permet de découper la portion sélectionner
        /// </summary>
        private void Decouper()
        {
            ActionDecoupe = true;
            ActionAssemblage = false;

            if (this.SelectedEntity != null)
            {
                this.SelectedEntity.ValidationErrors.Clear();

                if (SelectedEntity.PointKiloDecouper <= 0 || SelectedEntity.PointKiloDecouper == null)
                {
                    this.SelectedEntity.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(Resource.DecoupagePortion_Prompt_PKTooSmall, new List<string>() { "PointKiloDecouper" }));
                }
                else if (SelectedEntity.PointKiloDecouper >= SelectedEntity.Longueur)
                {
                    string prompt = string.Format(Resource.DecoupagePortion_Prompt_PKTooBig, SelectedEntity.Longueur);
                    this.SelectedEntity.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(prompt, new List<string>() { "PointKiloDecouper" }));
                }
                else if (SelectedEntity.Longueur == null)
                {
                    string prompt = string.Format(Resource.DecoupagePortion_Prompt_ErrorLongueur, SelectedEntity.Longueur);
                    this.SelectedEntity.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(prompt, new List<string>() { "PointKiloDecouper" }));
                }
                else
                {
                    // Création de la nouvelle portion
                    PortionIntegrite PortionAjouter = new PortionIntegrite
                    {
                        Branchement = SelectedEntity.Branchement,
                        CleCommuneArrivee = SelectedEntity.CleCommuneArrivee,
                        CleCommuneDepart = SelectedEntity.CleCommuneDepart,
                        CleDiametre = SelectedEntity.CleDiametre,
                        CleEnsElectrique = SelectedEntity.CleEnsElectrique,
                        CleRevetement = SelectedEntity.CleRevetement,
                        Code = SelectedEntity.Code,
                        CodeGmao = SelectedEntity.CodeGmao,
                        Commentaire = SelectedEntity.Commentaire,
                        DateMajCommentaire = SelectedEntity.DateMajCommentaire,
                        DateMiseEnService = SelectedEntity.DateMiseEnService,
                        DatePose = SelectedEntity.DatePose,
                        Idtroncon = SelectedEntity.Idtroncon,
                        Libelle = "BIS " + SelectedEntity.Libelle,
                        Longueur = SelectedEntity.Longueur - SelectedEntity.PointKiloDecouper,
                        Supprime = SelectedEntity.Supprime
                    };

                    // Affectation des secteurs
                    foreach (PiSecteurs pis in SelectedEntity.PiSecteurs)
                    {
                        PortionAjouter.PiSecteurs.Add(new PiSecteurs() { CleSecteur = pis.CleSecteur });
                    }

                    // Modification de la longueur de la portion actuelle
                    SelectedEntity.Longueur = SelectedEntity.PointKiloDecouper;

                    // Affectation des PP sur la nouvelle portion
                    foreach (Pp pp in SelectedEntity.Pps)
                    {
                        if (pp.Pk > SelectedEntity.PointKiloDecouper) // From this PP on, all PPs are to be moved into the second section.
                        {
                            SelectedEntity.Pps.Remove(pp);
                            pp.BypassCategoriePp = true;
                            pp.PkNullable = pp.Pk - SelectedEntity.PointKiloDecouper.Value;
                            pp.PortionIntegrite = PortionAjouter;
                            PortionAjouter.Pps.Add(pp);
                        }
                    }

                    // Sauvegarde des éléments
                    this.service.Add(PortionAjouter); // Ajout de la nouvelle portion
                    this.Save(true); // Sauvegarde de la portion actuelle

                    // MAJ de la vue
                    RaisePropertyChanged(() => this.PortionsCibles);
                    RaisePropertyChanged(() => this.Entities);
                }
            }
            else
            {
                InfoWindow.CreateNew(Resource.Global_EmptyEntity);
            }
        }

        /// <summary>
        /// Permet d'assembler la portion sélectionner avec une autre
        /// </summary>
        private void Assembler()
        {
            ActionAssemblage = true;
            ActionDecoupe = false;

            if (this.SelectedEntity != null)
            {
                if (SelectedEntity.PortionCibleItem == null)
                {
                    this.SelectedEntity.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(Resource.DecoupagePortion_Prompt_SelectSecondPortion, new List<string>() { "PortionCibleItem" }));
                }
                else if (!SelectedEntity.Longueur.HasValue)
                {
                    string prompt = string.Format(Resource.DecoupagePortion_Prompt_ErrorLongueur, SelectedEntity.Longueur);
                    this.SelectedEntity.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(prompt, new List<string>() { "PortionCibleItem" }));
                }
                else
                {
                    this.SavedPortionCible = SelectedEntity.PortionCibleItem.Libelle;
                    this.service.GetEntityByCle(SelectedEntity.PortionCibleItem.ClePortion, e =>
                    {
                        if (e == null)
                        {
                            PortionIntegrite portionCible = this.service.DetailEntity;
                            bool confirmed = false;

                            // Si la portion contient des PP qui sont sur des PK supérieurs à sa longueur => que faire car bug !

                            if (portionCible.CleDiametre != SelectedEntity.CleDiametre || portionCible.CleRevetement != SelectedEntity.CleRevetement)
                            {
                                confirmed = (MessageBox.Show(Resource.DecoupagePortion_Confirmation_AssembleIncompatibles, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK);
                            }
                            else
                            {
                                confirmed = (MessageBox.Show(Resource.DecoupagePortion_Confirmation_Assemble, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK);
                            }

                            if (confirmed)
                            {
                                if (InversePK)
                                {
                                    foreach (Pp pp in portionCible.Pps)
                                    {
                                        pp.PkNullable = SelectedEntity.Longueur.Value + portionCible.Longueur.Value - pp.Pk;
                                        pp.PortionIntegrite = SelectedEntity;
                                        pp.BypassCategoriePp = true;
                                        SelectedEntity.Pps.Add(pp);
                                    }
                                }
                                else
                                {
                                    foreach (Pp pp in portionCible.Pps)
                                    {
                                        pp.PkNullable += SelectedEntity.Longueur.Value;
                                        pp.PortionIntegrite = SelectedEntity;
                                        pp.BypassCategoriePp = true;
                                        SelectedEntity.Pps.Add(pp);
                                    }
                                }

                                SelectedEntity.Longueur += portionCible.Longueur;
                                this.service.Delete(portionCible);
                                RaisePropertyChanged(() => this.PortionsCibles);
                                this.Save(true);
                            }
                        }

                        RaisePropertyChanged(() => this.PortionsCibles);
                    });
                }
            }
            else
            {
                InfoWindow.CreateNew(Resource.Global_EmptyEntity);
            }
        }

        #endregion

        #region Autorisations

        /// <summary>
        /// Retourne true si l'utilisateur à les droits false sinon.
        /// </summary>
        /// <returns></returns>
        private bool GetAutorisation()
        {
            if (this.CurrentUser != null && this.CurrentUser.IsAdministrateur)
                return true;
            return false;
        }

        #endregion Autorisations
    }
}