using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Models;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using System.Collections.Generic;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Services.EntityServices;
using Jounce.Core.Event;
using Jounce.Framework.Workflow;
using Jounce.Core.Application;
using Proteca.Silverlight.Services;
using Proteca.Silverlight.Resources;
using System.Windows;
using System.Reflection;
using Proteca.Web.Resources;

namespace Proteca.Silverlight.ViewModels
{
    [ExportAsViewModel("DetailPP")]
    public partial class DetailPPViewModel : BaseViewModel, IPartImportsSatisfiedNotification, IEventSink<ViewMode>
    {

        ///MANTIS 15800 : l'association de l'utilisateur qui effectue la demande de déverrouillage
        ///a été modifiée dans PROTON pour éviter la sauvegarde de l'utilisateur PROTON dans la Base
        ///Ce View Model n'est donc pas partagé avec celui de silverlight OFFLINE
        /// finalement ce fichier est partagé de nouveau, maintenant c'est une classe partielle


        #region Private Members

        /// <summary>
        /// Commentaire initiale avant les éventuelles modifications du commentaire
        /// </summary>
        private string CommentaireBeforeUpdate { get; set; }

        /// <summary>
        /// Déclaration de la variable CodeDepartement
        /// </summary>
        private string _codeDepartement;

        /// <summary>
        /// Liste des Pp filtrées par la portion sélectionnée
        /// </summary>
        private List<Pp> _listPPJumelage;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Accesseur vers la liste des Pp jumelées à la Pp en cours d'édition
        /// </summary>
        public ObservableCollection<Pp> ListPpJumelees
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    var listPpJumelees = this.SelectedEntity.PpJumelees.Where(p => p.Supprime == false);
                    return new ObservableCollection<Pp>(listPpJumelees);
                }
                else
                {
                    return new ObservableCollection<Pp>();
                }
            }
        }

        partial void SetCleUtiDdeDeverrouillage();
        partial void SetNeedCheckUser();
        private bool NeedCheckUser;
        /// <summary>
        /// Retourne si le déverrouillage des coordonnées GPS est activé ou non
        /// </summary>
        public bool ActiveDdeDeverrouillageCoordGps
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    return this.SelectedEntity.DdeDeverrouillageCoordGps;
                }
                return false;
            }
            set
            {
                if (this.SelectedEntity != null && ((!NeedCheckUser) || this.CurrentUser != null))
                {
                    if (value && !this.SelectedEntity.DdeDeverrouillageCoordGps)
                    {
                        SetCleUtiDdeDeverrouillage();
                        //this.SelectedEntity.CleUtiDdeDeverrouillage = this.CurrentUser.CleUtilisateur;
                        this.SelectedEntity.DateDdeDeverrouillageCoordGps = DateTime.Now;
                    }
                    else
                    {
                        this.SelectedEntity.UsrUtilisateur1 = null;
                        this.SelectedEntity.DateDdeDeverrouillageCoordGps = null;
                    }

                    this.SelectedEntity.DdeDeverrouillageCoordGps = value;
                }
                ////Pour éviter de perdre la checkbox de demande de déverrouillage on ne rafraichit pas le Txt
                //RaisePropertyChanged(() => this.TxtDeverrouillage);
            }
        }

        /// <summary>
        /// Retourne le texte pour la zone de déverouillage
        /// </summary>
        public string TxtDeverrouillage
        {
            get
            {
                if (this.SelectedEntity != null && this.SelectedEntity.CoordonneeGpsFiabilisee)
                {
                    if (this.SelectedEntity.DdeDeverrouillageCoordGps)
                    {
                        return Resource.EqEquipementPp_Deverouillage;
                    }
                    else
                    {
                        if (CanDdeDeverrouillage)
                        {
                            return Resource.EqEquipementPp_DdeDeverouillage;
                        }
                        else
                        {
                            return String.Empty;
                        }
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gestion d'une version restreinte pour les Equipements
        /// </summary>
        public bool _isEQVersion;
        public bool IsEQVersion
        {
            get { return _isEQVersion; }
            set { _isEQVersion = value; RaisePropertyChanged(() => this.IsEQVersion); RaisePropertyChanged(() => this.IsLightVersion); }
        }

        /// <summary>
        /// Gestion d'une version light pour les fiches de visite
        /// </summary>
        public bool _isLightVersion;
        public bool IsLightVersion
        {
            get { return _isLightVersion || IsEQVersion; }
            set { _isLightVersion = value; RaisePropertyChanged(() => this.IsLightVersion); }
        }

        /// <summary>
        /// Indique que tous les services sont bien chargés
        /// </summary>
        public bool AllServicesLoaded { get; set; }

        /// <summary>
        /// Indique si les services sont en cours de chargement
        /// </summary>
        public bool AllServicesLoading { get; set; }

        /// <summary>
        /// Utilisateur connecté
        /// </summary>
        public UsrUtilisateur CurrentUser
        {
            get
            {
                return this.userService.CurrentUser;
            }
        }

        /// <summary>
        /// PP sélectionnée
        /// </summary>
        private Pp _selectedEntity;
        public Pp SelectedEntity
        {
            get
            {
                if (_selectedEntity != null && _selectedEntity.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Detached)
                {
                    _selectedEntity = null;
                }
                return _selectedEntity;
            }
            set
            {
                _selectedEntity = value;


                RaisePropertyChanged(() => this.SelectedEntity);

                // Mise à jour des sous éléments
                registerPropertyChanged();
                if (SelectedEntity != null)
                {
                    this.CommentaireBeforeUpdate = this.SelectedEntity.Commentaire;
                    this.CodeDepartement = this.SelectedEntity.RefCommune != null ? this.SelectedEntity.RefCommune.CodeDepartement : String.Empty;
                }
                else
                {
                    this.CommentaireBeforeUpdate = string.Empty;
                    this.CodeDepartement = string.Empty;
                }
                RaisePropertyChanged(() => this.IsCoordGPSEnable);
                RaisePropertyChanged(() => this.ListSecteurs);
                RaisePropertyChanged(() => this.ListPortions);
                RaisePropertyChanged(() => this.CanDdeDeverrouillage);
                RaisePropertyChanged(() => this.CanEditFiabilisationGPS);
                RaisePropertyChanged(() => this.ActiveDdeDeverrouillageCoordGps);
                RaisePropertyChanged(() => this.EnableDdeGPS);
                RaisePropertyChanged(() => this.TxtDeverrouillage);
                RaisePropertyChanged(() => this.IsSensibilite2Visible);
                RaisePropertyChanged(() => this.LibelleNiveau2);
                RaisePropertyChanged(() => this.IsTmeTmsEnabled);


                RaisePropertyChanged(() => this.SelectedEntity);

                PortionSelected = null;
            }
        }

        private bool _isEditMode;
        public Boolean IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                if (!value)
                {
                    PortionSelected = null;
                    PPJumelageSelected = null;
                }

                _isEditMode = value;
                RaisePropertyChanged(() => this.IsEditMode);

                // Mise à jour des informations sur la PP
                RaisePropertyChanged(() => this.IsCoordGPSEnable);
                RaisePropertyChanged(() => this.CanDdeDeverrouillage);
                RaisePropertyChanged(() => this.CanEditFiabilisationGPS);
                RaisePropertyChanged(() => this.ActiveDdeDeverrouillageCoordGps);
                RaisePropertyChanged(() => this.EnableDdeGPS);
                RaisePropertyChanged(() => this.TxtDeverrouillage);
                RaisePropertyChanged(() => this.IsTmeTmsEnabled);
            }
        }

        // Active / désactive les listes déroulante TME / TMS si la case ) cocher courant alternatif est cochée
        public bool IsTmeTmsEnabled
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    return IsEditMode;// && !this.SelectedEntity.CourantsAlternatifsInduits;
                }
                return IsEditMode;
            }
        }

        /// <summary>
        /// Liste des secteurs
        /// </summary>
        public ObservableCollection<GeoSecteur> ListSecteurs
        {
            get
            {
                if (this.CurrentUser != null && this.SelectedEntity != null && this.SelectedEntity.PortionIntegrite != null && this.SelectedEntity.PortionIntegrite.PiSecteurs.Any())
                {
                    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                    if (role != null && role.RefUsrPortee != null)
                    {
                        string codePortee = role.RefUsrPortee.CodePortee;
                        if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue() && CurrentUser.CleAgence.HasValue)
                        {
                            return new ObservableCollection<GeoSecteur>(this.SelectedEntity.PortionIntegrite.PiSecteurs.OrderBy(p => p.GeoSecteur.LibelleSecteur).Select(pis => pis.GeoSecteur).Where(g => g.CleAgence == this.CurrentUser.CleAgence.Value));
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                        {
                            return new ObservableCollection<GeoSecteur>(this.SelectedEntity.PortionIntegrite.PiSecteurs.OrderBy(p => p.GeoSecteur.LibelleSecteur).Select(pis => pis.GeoSecteur));
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue() && CurrentUser.GeoAgence != null)
                        {
                            return new ObservableCollection<GeoSecteur>(this.SelectedEntity.PortionIntegrite.PiSecteurs.OrderBy(p => p.GeoSecteur.LibelleSecteur).Select(pis => pis.GeoSecteur).Where(g => g.GeoAgence != null && g.GeoAgence.CleRegion == this.CurrentUser.GeoAgence.CleRegion));
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue() && CurrentUser.CleSecteur.HasValue)
                        {
                            return new ObservableCollection<GeoSecteur>(this.SelectedEntity.PortionIntegrite.PiSecteurs.OrderBy(p => p.GeoSecteur.LibelleSecteur).Select(pis => pis.GeoSecteur).Where(g => g.CleSecteur == this.CurrentUser.CleSecteur.Value));
                        }
                    }
                }
                return new ObservableCollection<GeoSecteur>();
            }
        }

        /// <summary>
        /// Liste des portions permettant de filtrer les PP pour l'association avce l'équipement
        /// </summary>
        public List<GeoEnsElecPortion> ListPortions
        {
            get
            {
                if (this.CurrentUser != null && this.SelectedEntity != null)
                {
                    UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                    if (role != null && role.RefUsrPortee != null)
                    {
                        string codePortee = role.RefUsrPortee.CodePortee;
                        if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue())
                        {
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleAgence == CurrentUser.CleAgence.Value && i.NbPp > 0 && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                            })).OrderBy(pi => pi.LibellePortion).ToList();
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                        {
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.NbPp > 0 && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                            })).OrderBy(pi => pi.LibellePortion).ToList();
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue())
                        {
                            return ServiceGeoEnsElecPortion.Entities.Where(i => i.CleRegion == CurrentUser.GeoAgence.CleRegion && i.NbPp > 0 && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                            {
                                return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                            })).OrderBy(pi => pi.LibellePortion).ToList();
                        }
                        else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue())
                        {
                            if (CurrentUser.CleSecteur.HasValue)
                            {
                                return ServiceGeoEnsElecPortion.Entities.Where(i => CurrentUser.CleSecteur.HasValue && i.CleSecteur == CurrentUser.CleSecteur.Value && i.NbPp > 0 && i.PortionSupprime == false).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                                {
                                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                                })).OrderBy(pi => pi.LibellePortion).ToList();
                            }
                        }
                    }
                }
                return ServiceGeoEnsElecPortion.Entities.Where(p => p.NbPp > 0 && !p.PortionSupprime).Distinct(new InlineEqualityComparer<GeoEnsElecPortion>((a, b) =>
                {
                    return a.ClePortion.Equals(b.ClePortion) && a.LibellePortion.Equals(b.LibellePortion);
                })).OrderBy(pi => pi.LibellePortion).ToList();
            }
        }

        /// <summary>
        /// Liste des catégories de PP
        /// </summary>
        public ObservableCollection<CategoriePp> Categories
        {
            get { return this.ServiceCategoriePp.Entities; }
        }

        /// <summary>
        /// Liste des niveaux de sensibilités de PP
        /// </summary>
        public List<RefNiveauSensibilitePp> NiveauSensibilitePP
        {
            get
            {
                if (ServiceNiveauSensibilitePp.Entities != null)
                    return ServiceNiveauSensibilitePp.Entities.Where(s => s.TypeSensibilite == 1).OrderBy(s => s.Libelle).ToList();
                else
                    return null;
            }
        }

        //public RefNiveauSensibilitePp NiveauSensibilitePPSelected
        //{
        //    get
        //    {
        //        if (this.SelectedEntity != null)
        //            return this.SelectedEntity.RefNiveauSensibilitePp;
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        this.SelectedEntity.RefNiveauSensibilitePp = value;

        //        if (value != null && value.CleNiveauSensibilite == 4)
        //        {
        //            this.CategorieSelected = null;
        //        }
        //        RaisePropertyChanged(() => IsSensibilite2Visible);

        //    }
        //}

        /// <summary>
        /// Catégorie PP sélectionnée
        /// </summary>
        public CategoriePp CategorieSelected
        {
            get
            {
                if (this.SelectedEntity != null)
                    return this.SelectedEntity.CategoriePp;
                else
                    return null;
            }
            set
            {
                RaisePropertyChanged(() => LibelleNiveau2);
            }
        }

        /// <summary>
        /// Libelle de Niveau 2
        /// </summary>
        public string LibelleNiveau2
        {
            get
            {
                if (ServiceNiveauSensibilitePp.Entities != null && CategorieSelected != null && ServiceNiveauSensibilitePp.Entities.Any(n => n.CleNiveauSensibilite == this.CategorieSelected.CleNiveauSensibilite))
                    return ServiceNiveauSensibilitePp.Entities.First(n => n.CleNiveauSensibilite == this.CategorieSelected.CleNiveauSensibilite).Libelle;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Liste des temps de polarisation
        /// </summary>
        public List<RefEnumValeur> TempsPolarisation
        {
            get
            {
                return ServiceEnumValeur.Entities
                    .Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.PP_POLARISATION.GetStringValue())
                    .OrderBy(e => e.NumeroOrdre)
                    .ToList();
            }
        }

        ///// <summary>
        ///// temps de polarisation sélectionné
        ///// </summary>
        //public RefEnumValeur TempsPolarisationSelected
        //{
        //    get
        //    {
        //        if (this.SelectedEntity != null)
        //            return this.SelectedEntity.RefEnumValeur1;
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        if (this.SelectedEntity != null)
        //            this.SelectedEntity.RefEnumValeur1 = value;
        //    }
        //}

        /// <summary>
        /// Liste des durée d'enregistrement
        /// </summary>
        public List<RefEnumValeur> DureeEnregistrement
        {
            get
            {
                return ServiceEnumValeur.Entities
                    .Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.PP_DUREE_ENRG.GetStringValue())
                    .OrderBy(e => e.Libelle)
                    .ToList();
            }
        }

        ///// <summary>
        ///// durée d'enregistrement sélectionné
        ///// </summary>
        //public RefEnumValeur DureeEnregistrementSelected
        //{
        //    get
        //    {
        //        if (this.SelectedEntity != null)
        //            return this.SelectedEntity.RefEnumValeur;
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        if (this.SelectedEntity != null)
        //            this.SelectedEntity.RefEnumValeur = value;
        //    }
        //}
        /// <summary>
        /// Liste des surface TME
        /// </summary>
        public ObservableCollection<RefEnumValeur> SurfaceTME
        {
            get
            {
                return new ObservableCollection<RefEnumValeur>(ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.PP_SURFACE_TME.GetStringValue())
                .OrderBy(e => e.NumeroOrdre).ThenBy(e => e.Libelle));
            }
        }

        ///// <summary>
        ///// surface TME sélectionnée
        ///// </summary>
        //public RefEnumValeur SurfaceTMESelected
        //{
        //    get
        //    {
        //        if (this.SelectedEntity != null)
        //            return this.SelectedEntity.RefEnumValeur2;
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        if (this.SelectedEntity != null)
        //            this.SelectedEntity.RefEnumValeur2 = value;
        //    }
        //}


        /// <summary>
        /// Liste des surface TMS
        /// </summary>
        public List<RefEnumValeur> SurfaceTMS
        {
            get
            {
                return ServiceEnumValeur.Entities.Where(e => e.CodeGroupe == RefEnumValeurCodeGroupeEnum.PP_SURFACE_TMS.GetStringValue())
                    .OrderBy(e => e.NumeroOrdre).ThenBy(e => e.Libelle).ToList();
            }
        }

        ///// <summary>
        ///// surface TMS sélectionnée
        ///// </summary>
        //public RefEnumValeur SurfaceTMSSelected
        //{
        //    get
        //    {
        //        if (this.SelectedEntity != null)
        //            return this.SelectedEntity.RefEnumValeur3;
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        if (this.SelectedEntity != null)
        //            this.SelectedEntity.RefEnumValeur3 = value;
        //    }
        //}

        /// <summary>
        /// Liste des communes filtrées par départememnt
        /// </summary>
        public ObservableCollection<RefCommune> Communes
        {
            get
            {
                if (this.CodeDepartement != null)
                {
                    if (this.CodeDepartement.ToString().Length == 1)
                    {
                        return null;
                    }
                    return new ObservableCollection<RefCommune>(ServiceRefCommune.Entities.Where(c => c.CodeDepartement.ToLower() == CodeDepartement.ToString().ToLower()).OrderBy(c => c.Libelle));
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Indique si la sensibilité 2 est éditable (non visible pour les non-mesurés)
        /// </summary>
        public bool IsSensibilite2Visible
        {
            get
            {
                return SelectedEntity != null && SelectedEntity.RefNiveauSensibilitePp != null
                    && SelectedEntity.RefNiveauSensibilitePp.TypeSensibilite == 1       // Type de sensibilité 1
                    && SelectedEntity.RefNiveauSensibilitePp.EnumTypeEval.HasValue;     // Sans évaluation => PP Non-Mesurée
            }
        }

        /// <summary>
        /// Indique si les coordonnées GPS sont éditables
        /// </summary>
        public bool IsCoordGPSEnable
        {
            get
            {
                return SelectedEntity != null && !SelectedEntity.CoordonneeGpsFiabilisee && IsEditMode;
            }
        }

        /// <summary>
        /// Indique si la demande de défiabilisation des coordonnées GPS est possible ou non
        /// </summary>
        public bool EnableDdeGPS
        {
            get
            {
                if (this.SelectedEntity != null)
                {
                    if (this.SelectedEntity.UsrUtilisateur1 != null && this.CurrentUser == this.SelectedEntity.UsrUtilisateur1)
                    {
                        return CanDdeDeverrouillage;
                    }
                    return CanDdeDeverrouillage && !this.SelectedEntity.DdeDeverrouillageCoordGps;
                }
                return false;
            }
        }

        /// <summary>
        /// Retourne l'autorisation de demander le déverrouillage des coordonnées GPS ou non
        /// </summary>
        public bool CanDdeDeverrouillage
        {
            get
            {
                if (this.SelectedEntity != null && Application.Current.IsRunningOutOfBrowser)
                {
                    return SelectedEntity.CoordonneeGpsFiabilisee && IsEditMode;
                }
                if (this.CurrentUser != null && this.SelectedEntity != null)
                {
                    UsrRole role = null;
                    if (SelectedEntity.CoordonneeGpsFiabilisee)
                    {
                        role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.DEVERROUIL_COORD_GPS);
                    }

                    if (role != null && role.RefUsrPortee != null)
                    {
                        string codePortee = role.RefUsrPortee.CodePortee;
                        if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue())
                        {
                            return false;
                        }
                        return SelectedEntity.CoordonneeGpsFiabilisee && IsEditMode;
                    }
                    return SelectedEntity.CoordonneeGpsFiabilisee && IsEditMode;
                }
                return false;
            }
        }

        /// <summary>
        /// Indique si l'utilisateur peut changer l'état de fiabiliation des coordonnées GPS
        /// </summary>
        public bool CanEditFiabilisationGPS
        {
            get
            {
                if (this.SelectedEntity != null && Application.Current.IsRunningOutOfBrowser)
                {
                    return SelectedEntity.CoordonneeGpsFiabiliseeOnLoaded ? false : IsEditMode;
                }
                if (this.CurrentUser != null && this.SelectedEntity != null && SelectedEntity.PositionGpsLat.HasValue && SelectedEntity.PositionGpsLong.HasValue)
                {
                    UsrRole role = null;
                    if (SelectedEntity.CoordonneeGpsFiabilisee)
                    {
                        role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.DEVERROUIL_COORD_GPS);
                    }
                    else
                    {
                        role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.VERROUIL_COORD_GPS);
                    }

                    if (role != null && role.RefUsrPortee != null)
                    {
                        string codePortee = role.RefUsrPortee.CodePortee;
                        if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue())
                        {
                            return IsEditMode;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Définit le code du département pour filtre les Communes
        /// </summary>
        public string CodeDepartement
        {
            get
            {
                if (_codeDepartement == null && SelectedEntity != null && SelectedEntity.RefCommune != null)
                {
                    _codeDepartement = SelectedEntity.RefCommune.CodeDepartement;
                }
                return _codeDepartement;
            }
            set
            {
                _codeDepartement = value;
                RaisePropertyChanged(() => this.CodeDepartement);
                RaisePropertyChanged(() => this.Communes);
            }
        }

        /// <summary>
        /// Liste des PP que l'on peut jumeler à la PP courante
        /// </summary>
        public List<Pp> ListPPJumelage
        {
            get
            {
                if (_listPPJumelage == null)
                {
                    _listPPJumelage = new List<Pp>();
                }
                if (this.SelectedEntity != null)
                {
                    List<Pp> ppsToRemove = this.SelectedEntity.PpJumelees.Union(new List<Pp>() { this.SelectedEntity }).ToList();
                    return _listPPJumelage.Except(ppsToRemove).OrderBy(p => p.Libelle).ToList();
                }
                else
                {
                    return _listPPJumelage;
                }
            }
            set
            {
                _listPPJumelage = value.ToList();
                RaisePropertyChanged(() => this.ListPPJumelage);
                this.PPJumelageSelected = null;
            }
        }

        private Pp _PpJumelageSelected;
        /// <summary>
        /// Pp jumelée sélectionnée
        /// </summary>
        public Pp PPJumelageSelected
        {
            get { return this._PpJumelageSelected; }
            set
            {
                _PpJumelageSelected = value;
                RaisePropertyChanged(() => this.PPJumelageSelected);
            }
        }

        /// <summary>
        /// Portion intégrité sélectionné pour filtre les pp pour les pp jumelée
        /// </summary>
        private GeoEnsElecPortion _portionSelected;

        /// <summary>
        /// Libelle de la portion sélectionnée
        /// </summary>
        public GeoEnsElecPortion PortionSelected
        {
            get { return _portionSelected; }
            set
            {
                _portionSelected = value;
                if (_portionSelected != null && _portionSelected.ClePortion > 0)
                {
                    ((PpJumeleeService)ServicePpJumelee).GetPpsAndPpJumeleeByClePortion(_portionSelected.ClePortion, GetPpsLoaded);
                }
                else
                {
                    ListPPJumelage = new List<Pp>();
                }
                this.RaisePropertyChanged(() => this.PortionSelected);
            }
        }

        public bool IsImportSatisfied
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Commands

        /// <summary>
        /// Commande utilisé pour l'ajout d'une PP Jumelée
        /// </summary>
        public IActionCommand AddPPJumeleeCommand { get; private set; }

        /// <summary>
        /// Commande utilisé pour supprimer une PP jumelée
        /// </summary>
        public IActionCommand RemovePPJumeleeCommand { get; private set; }

        #endregion Commands

        #region Event

        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnAllServicesLoaded;

        public EventHandler ImportSatisfied { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public DetailPPViewModel()
            : base()
        {
            SetNeedCheckUser();
            this.AddPPJumeleeCommand = new ActionCommand<object>(
                obj => AddPPJumelee(), obj => true);

            this.RemovePPJumeleeCommand = new ActionCommand<object>(
                obj => RemovePPJumelee(obj), obj => true);

            this.OnAllServicesLoaded += (o, e) =>
            {
                RaisePropertyChanged(() => this.NiveauSensibilitePP);
                RaisePropertyChanged(() => this.Categories);
                RaisePropertyChanged(() => this.CodeDepartement);
                RaisePropertyChanged(() => this.Communes);
                RaisePropertyChanged(() => this.TempsPolarisation);
                RaisePropertyChanged(() => this.DureeEnregistrement);
                RaisePropertyChanged(() => this.SurfaceTME);
                RaisePropertyChanged(() => this.SurfaceTMS);
            };

        }

        #endregion Constructor

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'utilisateur connecté
        /// </summary>
        [Import]
        public IUserService<Microsoft.SharePoint.Client.User> userService { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les entités de type Region
        /// </summary>
        [Import]
        public IEntityService<PortionIntegrite> servicePortion { get; set; }

        /// <summary>
        /// Service utilisé pour gérer la liste des Catégorie
        /// </summary>
        [Import]
        public IEntityService<CategoriePp> ServiceCategoriePp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer la liste des Sensibilité de PP
        /// </summary>
        [Import]
        public IEntityService<RefNiveauSensibilitePp> ServiceNiveauSensibilitePp { get; set; }

        /// <summary>
        /// Service utilisé pour gérer la liste des communes
        /// </summary>
        [Import]
        public IEntityService<RefCommune> ServiceRefCommune { get; set; }

        /// <summary>
        /// Service utilisé pour gérer les PpJumelee
        /// </summary>
        [Import]
        public IEntityService<PpJumelee> ServicePpJumelee { get; set; }

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

        #endregion Services

        #region override function

        protected override void DeactivateView(string viewName)
        {
            base.DeactivateView(viewName);

            AllServicesLoaded = false;

            var properties = this.GetType().GetProperties();
            // Pour chaque service de type IEntityService<>
            foreach (var prop in properties.Where(p => p.PropertyType.IsGenericType && (p.PropertyType.GetGenericTypeDefinition() == typeof(IEntityService<>))))
            {
                var serv = prop.GetValue(this, null);
                if (serv != null)
                {
                    var clearMethod = serv.GetType().GetMethod("Clear");
                    if (clearMethod != null)
                    {
                        clearMethod.Invoke(serv, null);
                    }
                }
            }
            this.CodeDepartement = null;
            this.SelectedEntity = null;
            IsEQVersion = false;
        }

        /// <summary>
        /// Activation de la vue
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);

            if (!AllServicesLoaded && !AllServicesLoading)
            {
                if (IsImportSatisfied)
                {
                    LoadAllServices();
                }
                else
                {
                    this.ImportSatisfied += (o, e) =>
                    {
                        LoadAllServices();
                    };
                }

                this.SelectedEntity = null;
                IsEQVersion = false;
            }

            if (!viewParameters.Any() || (viewParameters.Count == 1 && viewParameters.ContainsKey("IsLightVersion")))
            {
                IsLightVersion = viewParameters.ContainsKey("IsLightVersion") ? (bool)viewParameters["IsLightVersion"] : false;
            }
            else
            {
                IsLightVersion = viewParameters.ContainsKey("IsLightVersion") ? (bool)viewParameters["IsLightVersion"] : false;
                IOuvrage ouvrage = viewParameters.ContainsKey("SelectedEntity") ? (IOuvrage)viewParameters["SelectedEntity"] : null;
                if (ouvrage is Pp)
                {
                    this.SelectedEntity = ouvrage as Pp;
                    IsEQVersion = false;

                    if (this.SelectedEntity != null && !IsLightVersion)
                    {
                        //Au chargement, on charge les PP jumelés via le service
                        ((PpJumeleeService)ServicePpJumelee).GetPpsByClePp(this.SelectedEntity.ClePp, GetListPpJumeleeDone);
                    }
                }
                else if (ouvrage is EqEquipement || ouvrage is EqEquipementTmp)
                {
                    this.SelectedEntity = ouvrage.PpAttachee;
                    IsEQVersion = true;
                }
                else
                {
                    this.SelectedEntity = null;
                }
            }
        }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EventAggregator.Subscribe<ViewMode>(this);
            IsImportSatisfied = true;
            if (ImportSatisfied != null)
            {
                this.ImportSatisfied(this, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedEvent"></param>
        public void HandleEvent(ViewMode publishedEvent)
        {
            IsEditMode = publishedEvent == ViewMode.EditMode;

            // On recharge la liste des PPs jumelées quand on passe en mode consultation
            if (!IsEditMode)
            {
                if (this.SelectedEntity != null && this.SelectedEntity is Pp)
                {
                    //Au chargement, on charge les PP jumelés via le service
                    ((PpJumeleeService)ServicePpJumelee).GetPpsByClePp(this.SelectedEntity.ClePp, GetListPpJumeleeDone);
                }
            }
        }

        #endregion

        #region Private Functions

        //private int servicesLoadedCount = 0;
        //private String lockObject = String.Empty;

        /// <summary>
        ///  Charge la liste de toutes les entitées
        /// </summary>
        /// <returns></returns>
        private void LoadAllServices()
        {
            AllServicesLoading = true;

            EntityServiceHelper.LoadAllServicesAsync(
                this,
                svc => !(svc is IEntityService<GeoEnsElecPortion>),
                (svc, error) =>
                {
                    if (error != null)
                        Logger.Log(LogSeverity.Error, GetType().FullName, error);
                }, () =>
                {
                    AllServicesLoaded = true;
                    AllServicesLoading = false;
                    if (OnAllServicesLoaded != null)
                        OnAllServicesLoaded(this, null);
                });




            //var properties = this.GetType().GetProperties();
            //lock (lockObject)
            //{
            //    servicesLoadedCount = properties.Count(p => p.PropertyType.IsGenericType
            //            && p.PropertyType.GetGenericTypeDefinition() == typeof(IEntityService<>)
            //            && p.PropertyType != typeof(IEntityService<GeoEnsElecPortion>)
            //            );
            //}
            //// Pour chaque service de type IEntityService<>
            //foreach (var prop in properties.Where(p => p.PropertyType.IsGenericType
            //    && p.PropertyType.GetGenericTypeDefinition() == typeof(IEntityService<>)
            //    && p.PropertyType != typeof(IEntityService<GeoEnsElecPortion>))
            //    )
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
            //                    if (servicesLoadedCount == 0)
            //                    {
            //                        // A ce niveau, toutes les entités sont chargées
            //                        AllServicesLoaded = true;
            //                        AllServicesLoading = false;

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

        /// <summary>
        /// Chargement de la portions pour obtenir la liste des secteurs associé
        /// </summary>
        /// <param name="error"></param>
        private void AttachedPortionLoaded(Exception error)
        {
            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(GeoRegion).Name));
            }
            this.RaisePropertyChanged(() => this.ListSecteurs);
        }


        /// <summary>
        /// Récupération de la liste des PP jumelée
        /// </summary>
        /// <param name="error"></param>
        /// <param name="listPpJumelee"></param>
        private void GetListPpJumeleeDone(Exception error)
        {
            RaisePropertyChanged(() => this.ListPpJumelees);
        }

        /// <summary>
        /// La liste des Pps en fonction de la cle portion vien dd'être chargée
        /// </summary>
        /// <param name="error"></param>
        private void GetPpsLoaded(Exception error)
        {
            List<Pp> result = new List<Pp>();
            if (this.CurrentUser != null && this.SelectedEntity != null)
            {
                UsrRole role = this.CurrentUser.GetRoleByAutorisationCode(RefUsrAutorisation.ListAutorisationsEnum.MAJ_EQ_NIV);
                if (role != null && role.RefUsrPortee != null)
                {
                    string codePortee = role.RefUsrPortee.CodePortee;

                    if (codePortee == RefUsrPortee.ListPorteesEnum.Agence.GetStringValue() && CurrentUser.CleAgence.HasValue)
                    {
                        result = ((PpJumeleeService)ServicePpJumelee).Pps.Where(p => p.GeoSecteur != null && p.GeoSecteur.CleAgence == CurrentUser.CleAgence.Value).OrderBy(p => p.Libelle).ToList();
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Autorisee.GetStringValue() ||
                        codePortee == RefUsrPortee.ListPorteesEnum.National.GetStringValue())
                    {
                        result = ((PpJumeleeService)ServicePpJumelee).Pps.OrderBy(p => p.Libelle).ToList();
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Region.GetStringValue() && CurrentUser.GeoAgence != null)
                    {
                        result = ((PpJumeleeService)ServicePpJumelee).Pps.Where(p => p.GeoSecteur != null && p.GeoSecteur.GeoAgence != null && p.GeoSecteur.GeoAgence.CleRegion == CurrentUser.GeoAgence.CleRegion).OrderBy(p => p.Libelle).ToList();
                    }
                    else if (codePortee == RefUsrPortee.ListPorteesEnum.Secteur.GetStringValue() && CurrentUser.CleSecteur.HasValue)
                    {
                        result = ((PpJumeleeService)ServicePpJumelee).Pps.Where(p => p.CleSecteur == CurrentUser.CleSecteur.Value).OrderBy(p => p.Libelle).ToList();
                    }
                }
            }
            ListPPJumelage = result;
        }

        /// <summary>
        /// Ajout d'une PP Jumelée
        /// </summary>
        private void AddPPJumelee()
        {
            if (this.PPJumelageSelected != null)
            {
                string ErreurMsg = null;
                string Passline = " \r";

                if (this.SelectedEntity.CourantsVagabonds != PPJumelageSelected.CourantsVagabonds)
                {
                    ErreurMsg += "- " + ResourceHisto.CourantsVagabonds + Passline;
                }

                if (this.SelectedEntity.CourantsAlternatifsInduits != PPJumelageSelected.CourantsAlternatifsInduits)
                {
                    ErreurMsg += "- " + ResourceHisto.CourantsAlternatifsInduits + Passline;
                }

                if (this.SelectedEntity.ElectrodeEnterreeAmovible != PPJumelageSelected.ElectrodeEnterreeAmovible)
                {
                    ErreurMsg += "- " + ResourceHisto.ElectrodeEnterreeAmovible + Passline;
                }

                if (this.SelectedEntity.TemoinEnterreAmovible != PPJumelageSelected.TemoinEnterreAmovible)
                {
                    ErreurMsg += "- " + ResourceHisto.TemoinEnterreAmovible + Passline;
                }

                if (this.SelectedEntity.TemoinMetalliqueDeSurface != PPJumelageSelected.TemoinMetalliqueDeSurface)
                {
                    ErreurMsg += "- " + ResourceHisto.TemoinMetalliqueDeSurface + Passline;
                }

                if (this.SelectedEntity.PresenceDUneTelemesure != PPJumelageSelected.PresenceDUneTelemesure)
                {
                    ErreurMsg += "- " + ResourceHisto.PresenceDUneTelemesure + Passline;
                }

                if (ErreurMsg != null)
                {
                    MessageBoxResult resultPPJumele = MessageBox.Show(string.Format(Resource.DetailPp_ErrorCheckbox, Passline + ErreurMsg), Resource.AddPpJumeleeCaption, MessageBoxButton.OK);
                }
                else
                {
                    // Instanciation de la nouvelle PP jumelee pour le selected entity
                    ObservableCollection<PpJumelee> ListePPJumeleeACree = new ObservableCollection<PpJumelee>();

                    // Création de la liste des Pps à jumeler côté SelectedEntity
                    List<Pp> ListPpToJumelee1 = new List<Pp>() { this.SelectedEntity };
                    ListPpToJumelee1.AddRange(this.SelectedEntity.PpJumelees);

                    // Création de la liste des Pps à jumeler côté Pp selectionnée pour le jumelage
                    List<Pp> ListPpToJumelee2 = new List<Pp>() { this.PPJumelageSelected };
                    ListPpToJumelee2.AddRange(this.PPJumelageSelected.PpJumelees);

                    // Création des PpJumelees pour les Pps
                    foreach (Pp PpToJumelee1 in ListPpToJumelee1)
                    {
                        foreach (Pp PpToJumelee2 in ListPpToJumelee2)
                        {
                            if (PpToJumelee1.ClePp != PpToJumelee2.ClePp)
                            {
                                ListePPJumeleeACree.Add(new PpJumelee() { Pp = PpToJumelee1, Pp1 = PpToJumelee2 });
                            }
                        }
                    }

                    foreach (PpJumelee PPjumeleeAtraiter in ListePPJumeleeACree)
                    {
                        // Ajout des nouvelles PpJumelees
                        this.ServicePpJumelee.Add(PPjumeleeAtraiter);
                    }

                    RaisePropertyChanged(() => this.ListPpJumelees);
                    this.PPJumelageSelected = null;
                    RaisePropertyChanged(() => this.ListPPJumelage);

                    // Pour éviter de passer par la validation des Pp associées, on réalise un reject changes
                    foreach (Pp mapptoreject in ListPpToJumelee1.Union(ListPpToJumelee2))
                    {
                        if (mapptoreject != this.SelectedEntity)
                        {
                            mapptoreject.RejectChanges();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Suppression d'une PP Jumelée
        /// </summary>
        /// <param name="obj"></param>
        private void RemovePPJumelee(object obj)
        {
            if (obj is Pp)
            {
                // Suppression des pp jumelee précédemment trouvés
                foreach (PpJumelee PPJumtodelete in ((Pp)obj).PpJumelee.Union(((Pp)obj).PpJumelee1))
                {
                    this.ServicePpJumelee.Delete(PPJumtodelete);
                }

                // MAJ de la liste des PP jumelées
                RaisePropertyChanged(() => this.ListPpJumelees);

                // MAJ de la liste disponible des PP
                RaisePropertyChanged(() => this.ListPPJumelage);
            }
            // Ici pas besoin de RejectChanges sur les Pp jumelées... Car sinon la suppression est annulée
        }

        /// <summary>
        /// Abonnement à propertyChanged pour la gestion des coordonnées GPS et du niveau de sensibilité
        /// </summary>
        private void registerPropertyChanged()
        {
            if (SelectedEntity != null)
            {
                SelectedEntity.PropertyChanged += (oo, ee) =>
                {
                    if (ee.PropertyName == "CoordonneeGpsFiabilisee" || ee.PropertyName == "PositionGpsLat" || ee.PropertyName == "PositionGpsLong")
                    {
                        RaisePropertyChanged(() => this.IsCoordGPSEnable);
                        RaisePropertyChanged(() => this.CanEditFiabilisationGPS); // A supprimer si on autorise l'utilisateur à annuler sa modification autrement que par le bouton annuler
                    }
                    else if (ee.PropertyName == "CleNiveauSensibilite" || ee.PropertyName == "CleCategoriePp")
                    {
                        RaisePropertyChanged(() => this.IsSensibilite2Visible);
                        RaisePropertyChanged(() => this.LibelleNiveau2);
                    }
                    else if (ee.PropertyName == "ClePortion")
                    {
                        if (this.SelectedEntity.ClePortion == 0)
                        {
                            this.RaisePropertyChanged(() => this.ListSecteurs);
                        }
                        else
                        {
                            servicePortion.GetEntityByCle(this.SelectedEntity.ClePortion, AttachedPortionLoaded);
                        }
                    }
                    // SUppression de la règle à la demande de GRTGaz
                    //else if (ee.PropertyName == "CourantsAlternatifsInduits" || ee.PropertyName == "TemoinEnterreAmovible" || ee.PropertyName == "TemoinMetalliqueDeSurface")
                    //{
                    //    if (this.SelectedEntity.CourantsAlternatifsInduits)
                    //    {
                    //        if (this.SelectedEntity.TemoinEnterreAmovible)
                    //        {
                    //            this.SelectedEntity.EnumSurfaceTme = SurfaceTME.Where(i => i.Valeur == "1").FirstOrDefault().CleEnumValeur;
                    //            RaisePropertyChanged(() => this.SelectedEntity.EnumSurfaceTme);
                    //        }
                    //        if (this.SelectedEntity.TemoinMetalliqueDeSurface)
                    //        {
                    //            this.SelectedEntity.EnumSurfaceTms = SurfaceTMS.Where(i => i.Valeur == "1").FirstOrDefault().CleEnumValeur;
                    //            RaisePropertyChanged(() => this.SelectedEntity.EnumSurfaceTms);
                    //        }
                    //    }
                    //    RaisePropertyChanged(() => this.IsTmeTmsEnabled);
                    //}
                    else if (ee.PropertyName == "ElectrodeEnterreeAmovible" || ee.PropertyName == "PresenceDUneTelemesure" || ee.PropertyName == "CourantsVagabonds")
                    {

                    }
                };
            }
        }

        #endregion Private Functions
    }
}
