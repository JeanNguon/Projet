using System;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Helpers;
using System.Xml.Linq;
using Proteca.Silverlight;
using System.Windows.Browser;

namespace Proteca.Web.Models
{
    public partial class Pp : IOuvrage
    {
        #region Gestion de la propagation des modifications des EntityCollection

        public Boolean HasChildChanges
        {
            get { return ChildChanges.Any(); }
        }

        private List<String> _childChanges;
        public List<String> ChildChanges
        {
            get
            {
                if (_childChanges == null)
                {
                    _childChanges = new List<String>();
                }
                return _childChanges;
            }
            set { _childChanges = value; }
        }

        public void RaiseAnyDataMemberChanged(string prop)
        {
            if (prop == "PpJumelee" || prop == "PpJumelee1" || prop == "MesNiveauProtection")
            {
                if (!ChildChanges.Contains(prop))
                {
                    ChildChanges.Add(prop);
                    //this.RaiseDataMemberChanged(prop);
                }
            }
            // on ne fait pas de RaiseDataMemberChanged sur la PP pour ne pas modifier l'entité
            else if (prop == "Images" && !ChildChanges.Contains(prop))
            {
                ChildChanges.Add(prop);
            }
        }

        //protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    base.OnPropertyChanged(e);
        //    if (e.PropertyName == "EntityState" && this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
        //    {
        //        ChildChanges.Clear();
        //    }
        //}

        #endregion

        #region Public Properties

        private bool? _coordonneeGpsFiabiliseeOnLoaded = (bool?)null;
        public bool CoordonneeGpsFiabiliseeOnLoaded
        {
            get
            {
                if (!this._coordonneeGpsFiabiliseeOnLoaded.HasValue)
                {
                    this._coordonneeGpsFiabiliseeOnLoaded = this.CoordonneeGpsFiabilisee;
                }
                return this._coordonneeGpsFiabiliseeOnLoaded.Value;
            }
            private set
            {
                this._coordonneeGpsFiabiliseeOnLoaded = value;
            }
        }

        /// <summary>
        /// Private propertie pour gérer la version nullable de la capacité d'un condensateur
        /// </summary>
        private Nullable<decimal> _pkNullable;

        /// <summary>
        /// Public propertie pour gérer la version nullable de la capacité d'un condensateur
        /// </summary>
        [RequiredCustom()]
        public Nullable<decimal> PkNullable
        {
            get
            {
                return _pkNullable;
            }
            set
            {
                if (_pkNullable != value)
                {
                    this.ValidateProperty("PkNullable", value);

                    if (value.HasValue)
                    {
                        this.Pk = value.Value;
                    }
                    else
                    {
                        this.RaiseDataMemberChanged("Pk");
                    }

                    _pkNullable = value;
                }
            }
        }

        /// <summary>
        /// Sélection de la portion intégritée attachée
        /// </summary>
        public GeoEnsElecPortion SelectedPortion
        {
            get
            {
                if (this.PortionIntegrite != null)
                {
                    return new GeoEnsElecPortion()
                    {
                        ClePortion = this.ClePortion,
                        LibellePortion = this.PortionIntegrite.Libelle
                    };
                }
                return null;
            }
            set
            {
                if (value != null)
                    this.ClePortion = value.ClePortion;
                else
                    this.ClePortion = 0;

                this.RaisePropertyChanged("PortionsIntGeoSecteurs");
            }
        }

        /// <summary>
        /// Liste des secteurs de la portion intégrité
        /// </summary>
        public List<GeoSecteur> PortionsIntGeoSecteurs
        {
            get
            {
                if (this.PortionIntegrite != null && this.PortionIntegrite.PiSecteurs != null)
                {
                    return this.PortionIntegrite.PiSecteurs.Select(pi => pi.GeoSecteur).ToList();
                }
                return new List<GeoSecteur>();
            }
        }

        /// <summary>
        /// Date Ecd
        /// </summary>
        public string DateEcd
        {
            get
            {
                string result = String.Empty;
                if (Visites != null && Visites.Any() && Visites.First().DateVisite.HasValue)
                {
                    result = Visites.First().DateVisite.Value.ToString("dd/MM/yyyy");
                }
                return result;
            }
        }

        /// <summary>
        /// Libelle afficher dans les résultat de recherche
        /// Format PK - Libelle
        /// </summary>
        public string LibelleRecherche
        {
            get { return string.Format("{0} - {1}", this.Pk, this.Libelle); }
        }


        /// <summary>
        /// Affiche la clé PPO utilisé dans l'application Micado
        /// </summary>
        public string CleMicado
        {
            get
            {
                return string.Format(Resource.CleMicado, Resource.MicadoPp, this.ClePp);
            }
        }

        /// <summary>
        /// Affiche la clé EPC formatée pour les libellés
        /// </summary>
        public string CleMicadoLibelle
        {
            get
            {
                return string.Format(Resource.CleMicadoLibelle, this.CleMicado);
            }
        }


        /// <summary>
        /// Retourne la source de l'image à afficher suivant l'état
        /// </summary>
        public string EtatSourceImage
        {
            get
            {
                string result = String.Empty;
                if (Visites != null && Visites.Any() && Visites.First().Analyse != null)
                {
                    result = Visites.First().Analyse.EtatSourceImage;
                }
                return result;
            }
        }

        /// <summary>
        /// Retourne le libellé avec la PK
        /// </summary>
        public string LibellePPwithPK
        {
            get
            {
                return Pk + " - " + Libelle;
            }
        }

        partial void OnCleNiveauSensibiliteChanged()
        {
            RaisePropertyChanged("CleNiveauSensibiliteExtended");
        }
     
        /// <summary>
        /// Surcharge de la propriété RefNiveauSensibilite
        /// </summary>
        [RequiredCustom()]
        public int CleNiveauSensibiliteExtended
        {
            get
            {
                return this.CleNiveauSensibilite;
            }
            set
            {
                if (this.CleNiveauSensibilite != value)
                {
                    this.CleNiveauSensibilite = value;
                    //Si le niveau de sensibilité est "Non mesurée" on remet la catégorie à null
                    if (value == 4)
                    {
                        this.CleCategoriePp = null;
                    }
                }
                RaisePropertyChanged("CleNiveauSensibiliteExtended");
            }
        }

        /// <summary>
        /// Retourne le libellé avec la PK
        /// </summary>
        public string LibellePPNiveauSensibilite2
        {
            get
            {
                string result = string.Empty;
                string format = " ({0})";
                if (this.RefNiveauSensibilitePp != null)
                {
                    //4 est le niveau de sensibilité "Non mesurée" NOTA : on ne devrait pas avoir de catégoriePP à ce niveau
                    if (this.CleNiveauSensibilite == 4)
                    {
                        result = this.RefNiveauSensibilitePp.Libelle;
                    }
                    else if (this.CategoriePp != null && this.CategoriePp.RefNiveauSensibilitePp != null)
                    {
                        result = this.CategoriePp.RefNiveauSensibilitePp.Libelle;
                    }
                }
                if (!String.IsNullOrEmpty(result))
                {
                    return this.Libelle + string.Format(format, result);
                }
                else
                {
                    return this.Libelle;
                }
            }
        }

        /// <summary>
        /// Indique si l'équipement est issu d'un déplacement
        /// </summary>
        public bool IssuDeplacement
        {
            get
            {
                return this.ClePpOrigine.HasValue;
            }
        }

        /// <summary>
        /// Indique si l'equipement est supprimé
        /// </summary>
        public bool IsDelete
        {
            get
            {
                return this.Supprime;
            }
        }

        /// <summary>
        /// Indique si l'equipement est déplacé
        /// </summary>
        public bool IsDeplace
        {
            get
            {
                return this.Pp1.Any();
            }
        }

        /// <summary>
        /// Si l'equipement à été déplacé ou supprimé ou les deux on affiche un message sinon rien
        /// </summary>
        public string InfosEquipment
        {
            get
            {
                if (IsDeplace)
                {
                    return Resource.EqEquipement_EqDeplace;
                }
                else if (IsDelete)
                {
                    return Resource.EqEquipement_EqSupprime;
                }
                else if (IssuDeplacement)
                {

                    return Resource.EqEquipement_EqIssuDeplacement;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Permet de construire le libelle de navigation de la PP
        /// </summary>
        public string LibelleNavigateToPpText { get; set; }

        /// <summary>
        /// Retourne le libelle par defaut de la PP à prendre en compte
        /// </summary>
        public string LibelleNavigateToPp { get; set; }

        /// <summary>
        /// Retourne la Pp dupliqué
        /// </summary>
        /// <returns></returns>
        public Pp DuplicatePp()
        {
            // Affectation des propriétés à l'élément dupliqué
            Pp PpToReturn = new Pp()
            {
                // Champs communs EqEquipement
                Libelle = this.Libelle,
                Commentaire = this.Commentaire,
                DateMajCommentaire = this.DateMajCommentaire,
                DateMiseEnService = this.DateMiseEnService,
                Supprime = this.Supprime,

                // Champs spécifiques
                ClePpOrigine = this.ClePpOrigine,
                CommentairePositionnement = this.CommentairePositionnement,
                CoordonneeGpsFiabilisee = this.CoordonneeGpsFiabilisee,
                CourantsAlternatifsInduits = this.CourantsAlternatifsInduits,
                CourantsVagabonds = this.CourantsVagabonds,
                DateMajPp = this.DateMajPp,
                DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure,
                ElectrodeEnterreeAmovible = this.ElectrodeEnterreeAmovible,
                PkNullable = this.Pk,
                PositionGpsLat = this.PositionGpsLat,
                PositionGpsLong = this.PositionGpsLong,
                PositionnementPostal = this.PositionnementPostal,
                PpPoste = this.PpPoste,
                PresenceDUneTelemesure = this.PresenceDUneTelemesure,
                TemoinEnterreAmovible = this.TemoinEnterreAmovible,
                TemoinMetalliqueDeSurface = this.TemoinMetalliqueDeSurface,

                // Champs de référence
                CleSecteur = this.CleSecteur,
                CleCommune = this.CleCommune,
                EnumDureeEnrg = this.EnumDureeEnrg,
                EnumPolarisation = this.EnumPolarisation,
                EnumSurfaceTme = this.EnumSurfaceTme,
                EnumSurfaceTms = this.EnumSurfaceTms,
                CleCategoriePp = this.CleCategoriePp,
                ClePortion = this.ClePortion,
                Pp2 = this,
                CleUtilisateur = this.CleUtilisateur,
                CleNiveauSensibilite = this.CleNiveauSensibilite,
            };

            Image tmpImage = this.Images.FirstOrDefault();
            if (tmpImage != null)
            {
                PpToReturn.Images.Add(new Image() { EnumTypeImage = tmpImage.EnumTypeImage, Image1 = tmpImage.Image1 });
            }

            foreach (var np in this.MesNiveauProtection)
            {
                PpToReturn.MesNiveauProtection.Add(np.DupliquerMesNiveauProtection());
            }
            return PpToReturn;
        }

        #region Saisie Visite

        public string EnsElec
        {
            get
            {
                if (this.PortionIntegrite != null && this.PortionIntegrite.EnsembleElectrique != null)
                {
                    return this.PortionIntegrite.EnsembleElectrique.Libelle;
                }
                return "";
            }
        }

        public string SecteurEnsElec
        {
            get
            {
                return Secteur + " / " + EnsElec;
            }
        }

        public string Portion
        {
            get
            {
                return this.PortionIntegrite != null ? this.PortionIntegrite.Libelle : "";
            }
        }

        public string Secteur
        {
            get
            {
                return this.GeoSecteur != null ? this.GeoSecteur.LibelleSecteur : "";
            }
        }

        #endregion

        #endregion

        #region Liens

        /// <summary>
        /// Url de l'élément courant
        /// </summary>
        public string NaviagtionUrl
        {
            get
            {
                return string.Format("/{0}/{1}/PP/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.Equipement.GetStringValue(),
                   ClePp);
            }
        }

        /// <summary>
        /// URL vers Micado
        /// </summary>
        public Uri MicadoURI
        {
            get //TODO : Mettre à jour le lien vers MICADO
            {
                String urlMicado = Resource.MicadoURIRoot;

                if (App.Current.Resources.Contains("MicadoURL"))
                {
                    urlMicado = HttpUtility.UrlDecode(App.Current.Resources["MicadoURL"].ToString());
                }
                if (!urlMicado.EndsWith("/"))
                {
                    urlMicado += '/';
                }
                return new Uri(urlMicado + string.Format(Resource.MicadoURIParams, this.CleMicado));
            }
        }

        /// <summary>
        /// Url de la portion de rattachement
        /// </summary>
        public Uri NaviagtionPortionUrl
        {
            get
            {
                return new Uri(string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.GestionOuvrages.GetStringValue(),
                   OuvrageNavigation.PortionIntegrite.GetStringValue(),
                   ClePortion), UriKind.Relative);
            }
        }

        /// <summary>
        /// Permet de naviguer soit vers l'equipement d'origine  pour un objet ayant été déplacé
        /// Soit vers le nouvel objet pour un objet ayant subi un dépacement
        /// Si on est dans les deux cas le lien vers le nouvel objet est affiché
        /// </summary>
        public Uri NavigateToPP
        {
            get
            {
                if (IsDeplace)
                {
                    LibelleNavigateToPpText = "Duplicata : ";
                    LibelleNavigateToPp = this.Pp1.FirstOrDefault().Libelle;
                    return new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                        MainNavigation.GestionOuvrages.GetStringValue(),
                        OuvrageNavigation.Equipement.GetStringValue(),
                        "Pp",
                        this.Pp1.FirstOrDefault().ClePp), UriKind.Relative);
                }
                else if (IssuDeplacement)
                {
                    LibelleNavigateToPpText = "Origine : ";
                    LibelleNavigateToPp = this.Pp2.Libelle;
                    return new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                    MainNavigation.GestionOuvrages.GetStringValue(),
                    OuvrageNavigation.Equipement.GetStringValue(),
                    "Pp",
                    ClePpOrigine), UriKind.Relative);
                }
                else
                {
                    return null;
                }
            }

        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Sur la méthode ToString, on retourne le libellé par défaut
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Libelle;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Déplace la PP
        /// cf : écran des déplacements de Pp
        /// </summary>
        public Pp DeplacerPp()
        {
            // Duplication de la Pp
            Pp PpDupliquer = this.DuplicatePp();

            // Modification du nom de l'ancienne Pp
            int increment = 0;
            int incrementParsed;
            if (this.Pp2 != null)
            {
                increment = int.TryParse(this.Pp2.Libelle.Substring(1, 1).ToString(), out incrementParsed) ? incrementParsed : 0;
                increment += 1;
            }

            this.Libelle = string.Format("X{0}-" + this.Libelle, increment);


            // On duplique les équipements liés
            foreach (EqEquipement item in this.EqEquipement)
            {
                if (!item.Supprime)
                {
                    PpDupliquer.EqEquipement.Add((EqEquipement)item.DeplacerEq());
                }
            }

            return PpDupliquer;
        }

        public bool AssocieCompEtVisites(Pp PPorigine)
        {
            // On duplique les Compositions liées
            foreach (var Composition in PPorigine.Compositions)
            {
                Composition.ClePp = this.ClePp;
            }

            // On duplique les visites liées
            foreach (var visite in PPorigine.Visites)
            {
                visite.ClePp = this.ClePp;
            }

            // Pour les RI/FM qui ont la Pp courante en Pp secondaire, on les set à null
            foreach (EqRaccordIsolant item in PPorigine.EqRaccordIsolant)
            {
                if (!item.Supprime)
                {
                    item.Pp2 = this;
                    item.ClePp2 = this.ClePp;
                }
            }
            foreach (EqFourreauMetallique item in PPorigine.EqFourreauMetallique)
            {
                if (!item.Supprime)
                {
                    item.Pp2 = this;
                    item.ClePp2 = this.ClePp;
                }
            }

            // Pour les LI qui ont la Pp courante en Pp secondaire,  la Pp2 prend la nouvelle Pp
            foreach (EqLiaisonInterne item in PPorigine.EqLiaisonInterne)
            {
                if (!item.Supprime)
                {
                    //  EqLiaisonInterne itemDeplace = (EqLiaisonInterne)item.DeplacerEq();
                    item.Pp2 = this;
                    item.ClePp2 = this.ClePp;
                }
            }
            return true;
        }

        public XElement CreateXElement()
        {
            XElement xPp = new XElement("PP");

            xPp.Add(new XElement("PPMesuree", 1));
            xPp.Add(new XElement("CleTypeEvaluation", Composition.EnumTypeEval));
            xPp.Add(new XElement("TypeEvaluation", Composition.RefEnumValeur.LibelleCourt));
            xPp.Add(new XElement("ClePP", ClePp));
            xPp.Add(new XElement("CleNiveauSensibilite", CleNiveauSensibilite));
            xPp.Add(new XElement("CleCategoriePP", CleCategoriePp));
            xPp.Add(new XElement("EnumSurfaceTme", EnumSurfaceTme));
            xPp.Add(new XElement("EnumSurfaceTms", EnumSurfaceTms));
            xPp.Add(new XElement("EnumDureeEnrg", EnumDureeEnrg));
            xPp.Add(new XElement("EnumPolarisation", EnumPolarisation));
            xPp.Add(new XElement("Libelle", Libelle));
            xPp.Add(new XElement("Pk", Pk));
            xPp.Add(new XElement("CourantsVagabonds", CourantsVagabonds));
            xPp.Add(new XElement("CourantsAlternatifsInduits", CourantsAlternatifsInduits));
            xPp.Add(new XElement("ElectrodeEnterreAmovible", ElectrodeEnterreeAmovible));
            xPp.Add(new XElement("TemoinEnterreAmovible", TemoinEnterreAmovible));
            xPp.Add(new XElement("TemoinMetalliqueDeSurface", TemoinMetalliqueDeSurface));
            xPp.Add(new XElement("Telemesure", PresenceDUneTelemesure));
            xPp.Add(new XElement("PositionGPSLat", PositionGpsLat));
            xPp.Add(new XElement("PositionGPSLong", PositionGpsLong));
            xPp.Add(new XElement("CoordonneesGPSFiabilisee", CoordonneeGpsFiabilisee));

            XElement xVisites = new XElement("Visites");
            foreach (Visite visite in Visites)
            {
                xVisites.Add(visite.IsNew() ? visite.CreateXVisite() : new XElement("Visite"));
            }
            xPp.Add(xVisites);

            return xPp;
        }

        public void CommitCoordonneeGPSFiabilisee()
        {
            this.CoordonneeGpsFiabiliseeOnLoaded = this.CoordonneeGpsFiabilisee;
        }

        #endregion

        #region Events

        /// <summary>
        /// MAj de l'écran à la modif de certaines properties
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "ClePortion") { RaisePropertyChanged("NaviagtionPortionUrl"); }

            if (e.PropertyName == "EntityState")
            {
                if (this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
                {
                    ChildChanges.Clear();

                    if (this.IsNew())
                    {
                        _pkNullable = null;

                    }
                    else
                    {
                        _cleNiveauSensibilite = this.CleNiveauSensibilite;
                        _pkNullable = this.Pk;
                    }
                    this.RaisePropertyChanged("PkNullable");

                    this.ValidationErrors.Clear();
                }
            }
            if (e.PropertyName == "PresenceDUneTelemesure" && PresenceDUneTelemesure && this.HasChanges)
            {
                this.ElectrodeEnterreeAmovible = true;
                this.TemoinEnterreAmovible = true;
                this.RaisePropertyChanged("ElectrodeEnterreeAmovible");
                this.RaisePropertyChanged("TemoinEnterreAmovible");
            }
            if (e.PropertyName == "PpJumelee" || e.PropertyName == "PpJumelee1")
            {
                this.RaisePropertyChanged("PpJumelees");
                this.RaisePropertyChanged("ClesPpJumelees");
            }
        }

        /// <summary>
        /// Initialisation des données au chargement des écrans
        /// </summary>
        /// <param name="isInitialLoad"></param>
        protected override void OnLoaded(bool isInitialLoad)
        {
            base.OnLoaded(isInitialLoad);

            // Gestion des valeurs par défaut des champs obligatoires
            if (!this.IsNew())
            {
                _pkNullable = this.Pk;
            }
        }

        #endregion

        #region IOuvrage

        public void ForceRaisePropertyChanged(String propertyName)
        {
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Retourne la Pp liée à l'ouvrage
        /// </summary>
        public String CodeEquipement
        {
            get
            {
                return "PP";
            }
        }

        /// <summary>
        /// Retourne la Pp liée à l'ouvrage
        /// </summary>
        public Pp PpAttachee
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Retourne la première composition liée à l'ouvrage
        /// </summary>
        public Composition Composition
        {
            get
            {
                return this.Compositions.FirstOrDefault();
            }
        }

        /// <summary>
        /// Retourne le libellé précédé de 'PP -' 
        /// </summary>
        public String LibelleExtended
        {
            get
            {
                return "PP - " + this._libelle;
            }
        }

        /// <summary>
        /// Propriété d'affichage du chemin géographique de l'élément
        /// Region / Agence / Secteur / EE / Portion
        /// MANTIS 11962 FSI 25/06/2014 : Bloquage de l'export d'une tournée
        /// </summary>
        public String LibelleCheminGeo
        {
            get
            {
                string result = string.Empty;
                string separator = " \\ ";
                if (this.GeoSecteur != null
                    && this.GeoSecteur.GeoAgence != null
                    && this.GeoSecteur.GeoAgence.GeoRegion != null
                    && this.PortionIntegrite != null
                    && this.PortionIntegrite.EnsembleElectrique != null)
                {
                    result = this.GeoSecteur.GeoAgence.GeoRegion.LibelleRegion + separator +
                        this.GeoSecteur.GeoAgence.LibelleAgence + separator +
                        this.GeoSecteur.LibelleSecteur + separator +
                        this.PortionIntegrite.EnsembleElectrique.Libelle + separator +
                        this.PortionIntegrite.Libelle;
                }
                return result;
            }
        }

        /// <summary>
        /// Retourne la dernière visite
        /// </summary>
        public Visite LastVisite
        {
            get
            {
                return this.Visites.Where(v => v.IsNew() || (v.DateVisite.HasValue && v.DateVisite >= VisitePeriodeDebut && v.DateVisite <= VisitePeriodeFin))
                    .OrderBy(v => v.IsNew()).ThenBy(v => v.IsNewInOfflineMode).ThenBy(v => v.DateVisite).LastOrDefault();
            }
        }

        /// <summary>
        /// Retourne les Pp Jumelees de la Pp en cours
        /// </summary>
        public List<Pp> PpJumelees
        {
            get
            {
                List<Pp> result = new List<Pp>();
                if (this.PpJumelee != null)
                {
                    result.AddRange(this.PpJumelee.Select(pj => pj.Pp1));
                }
                if (this.PpJumelee1 != null)
                {
                    result.AddRange(this.PpJumelee1.Select(pj => pj.Pp));
                }
                if (result.Any() && !result.Any(p => p == null))
                {
                    //Equivalent à ce que pourrait être en LINQ : DistinctBy(p => p.ClePp).ToList();
                    result = result.GroupBy(p => p.ClePp)
                                 .Select(g => g.First())
                                 .ToList();
                }
                return result;
            }
        }

        /// <summary>
        /// Retourne les Pp Jumelees de la Pp en cours
        /// </summary>
        public List<int> ClesPpJumelees
        {
            get
            {
                List<int> result = new List<int>();
                if (this.PpJumelee != null)
                {
                    result.AddRange(this.PpJumelee.Select(pj => pj.PpClePp));
                }
                if (this.PpJumelee1 != null)
                {
                    result.AddRange(this.PpJumelee1.Select(pj => pj.ClePp));
                }
                return result.Distinct().ToList();
            }
        }
        private DateTime? _visitePeriodeDebut;

        public DateTime? VisitePeriodeDebut
        {
            get { return _visitePeriodeDebut; }
            set { _visitePeriodeDebut = value; }
        }

        public DateTime? VisitePeriodeFin { get; set; }

        public Entity GetHisto()
        {
            // Affectation des propriétés à l'élément dupliqué
            return new HistoPp()
            {
                // Champs communs EqEquipement
                Libelle = this.Libelle,
                LibellePortion = this.PortionIntegrite.Libelle,
                Commentaire = this.Commentaire,
                DateMiseEnService = this.DateMiseEnService,
                Supprime = this.Supprime,

                // Champs spécifiques
                CommentairePositionnement = this.CommentairePositionnement,
                CoordonneeGpsFiabilisee = this.CoordonneeGpsFiabilisee,
                CourantsAlternatifsInduits = this.CourantsAlternatifsInduits,
                CourantsVagabonds = this.CourantsVagabonds,
                DateMajCommentaire = this.DateMajCommentaire,
                DateMiseEnServiceTelemesure = this.DateMiseEnServiceTelemesure,
                ElectrodeEnterreeAmovible = this.ElectrodeEnterreeAmovible,
                EnumDureeEnrg = this.EnumDureeEnrg,
                EnumPolarisation = this.EnumPolarisation,
                EnumSurfaceTme = this.EnumSurfaceTme,
                EnumSurfaceTms = this.EnumSurfaceTms,
                Pk = this.Pk,
                PositionGpsLat = this.PositionGpsLat,
                PositionGpsLong = this.PositionGpsLong,
                PositionnementPostal = this.PositionnementPostal,
                PpPoste = this.PpPoste,
                PresenceTelemesure = this.PresenceDUneTelemesure,
                CleCommune = this.CleCommune,
                TemoinEnterreAmovible = this.TemoinEnterreAmovible,
                TemoinMetalliqueDeSurface = this.TemoinMetalliqueDeSurface,
                DateMajPp = this.DateMajPp,

                // Champs de référence
                CleCategoriePp = this.CleCategoriePp,
                CleNiveauSensibilite = this.CleNiveauSensibilite
            };
        }

        #endregion


        /// <summary>
        /// Commit des changements loggés dans cette PpTmp vers la Pp
        /// </summary>
        public void SavePropertiesInPpTmp()
        {
            bool created = false;
            var pp = this.PpTmp.LastOrDefault();
            if (pp == null)
            {
                created = true;
                pp = new PpTmp();
            }
            pp.CleCategoriePp = this.CleCategoriePp;
            pp.CleNiveauSensibilite = this.CleNiveauSensibiliteExtended;

            pp.EnumDureeEnrg = this.EnumDureeEnrg;
            pp.EnumPolarisation = this.EnumPolarisation;
            pp.EnumSurfaceTme = this.EnumSurfaceTme;
            pp.EnumSurfaceTms = this.EnumSurfaceTms;

            pp.CourantsAlternatifsInduits = this.CourantsAlternatifsInduits;
            pp.CourantsVagabonds = this.CourantsVagabonds;
            pp.ElectrodeEnterreeAmovible = this.ElectrodeEnterreeAmovible;
            pp.TemoinEnterreAmovible = this.TemoinEnterreAmovible;
            pp.TemoinMetalliqueDeSurface = this.TemoinMetalliqueDeSurface;
            pp.PresenceDUneTelemesure = this.PresenceDUneTelemesure;

            pp.PositionGpsLat = this.PositionGpsLat;
            pp.PositionGpsLong = this.PositionGpsLong;
            pp.CoordonneeGpsFiabilisee = this.CoordonneeGpsFiabilisee;
            if (created)
                this.PpTmp.Add(pp);
        }


        /// <summary>
        /// Commit des changements loggés dans cette PpTmp vers la Pp
        /// </summary>
        public void RevertPropertiesFromPpTmp()
        {
            PpTmp pptmp = this.PpTmp.LastOrDefault();
            if (pptmp != null)
            {
                //this.CleNiveauSensibilite = pptmp.CleNiveauSensibilite;
                this.CleCategoriePp = pptmp.CleCategoriePp;
                this.CleNiveauSensibiliteExtended = pptmp.CleNiveauSensibilite;

                this.CourantsAlternatifsInduits = pptmp.CourantsAlternatifsInduits;
                this.CourantsVagabonds = pptmp.CourantsVagabonds;
                this.ElectrodeEnterreeAmovible = pptmp.ElectrodeEnterreeAmovible;
                this.TemoinEnterreAmovible = pptmp.TemoinEnterreAmovible;
                this.TemoinMetalliqueDeSurface = pptmp.TemoinMetalliqueDeSurface;
                this.PresenceDUneTelemesure = pptmp.PresenceDUneTelemesure;

                this.EnumDureeEnrg = pptmp.EnumDureeEnrg;
                this.EnumPolarisation = pptmp.EnumPolarisation;
                this.EnumSurfaceTme = pptmp.EnumSurfaceTme;
                this.EnumSurfaceTms = pptmp.EnumSurfaceTms;

                this.PositionGpsLat = pptmp.PositionGpsLat;
                this.PositionGpsLong = pptmp.PositionGpsLong;
                this.CoordonneeGpsFiabilisee = pptmp.CoordonneeGpsFiabilisee;
            }
        }

        partial void OnTemoinEnterreAmovibleChanged()
        {
            RaisePropertyChanged("EnumSurfaceTme");
        }
        partial void OnTemoinMetalliqueDeSurfaceChanged()
        {
            RaisePropertyChanged("EnumSurfaceTms");
        }
    }
}