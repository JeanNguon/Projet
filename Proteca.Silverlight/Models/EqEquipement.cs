using System;
using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Resources;
using System.Reflection;
using System.Xml.Linq;
using Proteca.Silverlight;
using System.Windows.Browser;

namespace Proteca.Web.Models
{
    public partial class EqEquipement : IOuvrage
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
            if (prop == "EqDrainageLiaisonsext" || prop == "EqSoutirageLiaisonsext")
            {
                if (!ChildChanges.Contains(prop))
                {
                    ChildChanges.Add(prop);
                    //this.RaiseDataMemberChanged(prop);
                }
            }
            // on ne fait pas de RaiseDataMemberChanged sur l'équipement pour ne pas modifier l'entité
            else if (prop == "Images" && !ChildChanges.Contains(prop))
            {
                ChildChanges.Add(prop);
            }
            else if (prop == "MesNiveauProtection" && !ChildChanges.Contains(prop))
            {
                ChildChanges.Add(prop);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retourne la Pp liée à l'ouvrage
        /// </summary>
        public String CodeEquipement
        {
            get
            {
                return (this.TypeEquipement != null)? this.TypeEquipement.CodeEquipement : String.Empty;
            }
        }

        /// <summary>
        /// Retourne la Pp liée à l'ouvrage
        /// </summary>
        public Pp PpAttachee
        {
            get
            {
                return this.Pp;
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
        /// Affiche Origine : ou Dulpicaata : si l'equipement a été déplacé ou est issu d'un déplacement
        /// </summary>
        public string LibelleNavigateToEqText { get; set; }

        /// <summary>
        /// Affiche le libellé de l'equipement vers lequel naviger
        /// </summary>
        public string LibelleNavigateToEquipement { get; set; }

        /// <summary>
        /// Libelle de la portion secondaire sélectionnée
        /// </summary>
        private GeoEnsElecPortion _portion2Selected;
        public GeoEnsElecPortion Portion2Selected
        {
            get { return _portion2Selected; }
            set
            {
                //LibellePortion2 = "";

                _portion2Selected = value;
                //if (_portion2Selected != null && _portion2Selected.ClePortion > 0)
                //{

                //    LibellePortion2 = _portion2Selected.LibellePortion;
                //}

                RaisePropertyChanged("Portion2Selected");
                RaisePropertyChanged("LibellePortion2");
            }
        }

        /// <summary>
        /// Libelle de la portion sélectionnée
        /// </summary>
        private GeoEnsElecPortion _portionSelected;
        public GeoEnsElecPortion PortionSelected
        {
            get { return _portionSelected; }
            set
            {
                //LibellePortion = "";

                _portionSelected = value;
                //if (_portionSelected != null && _portionSelected.ClePortion > 0)
                //{

                //    LibellePortion = _portionSelected.LibellePortion;
                //}
                RaisePropertyChanged("PortionSelected");
                RaisePropertyChanged("LibellePortion");
            }
        }

        /// <summary>
        /// Affiche la clé EPC utilisé dans l'application Micado
        /// </summary>
        public string CleMicado
        {
            get
            {
                return string.Format(Resource.CleMicado, Resource.MicadoEquipement, this.CleEquipement);
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
        /// Propriété permettant d'indiquer le libelle de la portion associée (utile pour les Histos)
        /// </summary>
        public String LibellePortion
        {
            get
            {
                if (this.PortionSelected != null && this.PortionSelected.ClePortion > 0)
                {
                    return this.PortionSelected.LibellePortion;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Propriété permettant d'indiquer le libelle de la portion secondaire associée (utile pour les Histos)
        /// </summary>
        public String LibellePortion2
        {
            get
            {
                if (this.Portion2Selected != null && this.Portion2Selected.ClePortion > 0)
                {
                    return this.Portion2Selected.LibellePortion;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Retourne la dernière visite
        /// </summary>
        public Visite LastVisite
        {
            get
            {
                return this.Visites.Where(v => v.IsNew() || (v.DateVisite.HasValue && v.DateVisite >= VisitePeriodeDebut && v.DateVisite <= VisitePeriodeFin)).OrderBy(v => v.IsNew()).ThenBy(v => v.IsNewInOfflineMode).ThenBy(v => v.DateVisite).LastOrDefault();
            }
        }

        public DateTime? VisitePeriodeDebut { get; set; }
        public DateTime? VisitePeriodeFin { get; set; }

        /// <summary>
        /// Indique si l'équipement est issu d'un déplacement
        /// </summary>
        public bool IssuDeplacement
        {
            get
            {
                return this.CleEquipementOrigine.HasValue;
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
                return this.EqEquipementEqEquipement.Any();
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
        /// Retourne le libelle avec le code de l'équipement
        /// </summary>
        public String LibelleExtended
        {
            get
            {
                string result = string.Empty;
                if (this.TypeEquipement != null)
                {
                    result = this.TypeEquipement.CodeEquipement + " - " + this.Libelle;
                }
                return result;
            }
        }

        /// <summary>
        /// Propriété pour le binding afin de déterminer si l'entité est nouvelle
        /// </summary>
        public Boolean IsNew
        {
            get
            {
                return this.IsNew();
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
                if (this.Pp != null)
                {
                    result = this.Pp.LibelleCheminGeo;
                }
                return result;
            }
        }

        #endregion

        #region Liens

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
                if (!IsNew && this.Pp != null)
                {
                    return new Uri(string.Format("/{0}/{1}/Id={2}",
                       MainNavigation.GestionOuvrages.GetStringValue(),
                       OuvrageNavigation.PortionIntegrite.GetStringValue(),
                       this.Pp.ClePortion), UriKind.Relative);
                }
                return null;
            }
        }

        /// <summary>
        /// Permet de naviguer soit vers l'equipement d'origine  pour un objet ayant été déplacé
        /// Soit vers le nouvel objet pour un objet ayant subi un dépacement
        /// Si on est dans les deux cas le lien vers le nouvel objet est affiché
        /// </summary>
        /// <param name="obj"></param>
        public Uri NavigateToEquipement
        {
            get
            {
                if (TypeEquipement != null)
                {
                    if (IsDeplace)
                    {
                        LibelleNavigateToEqText = "Duplicata : ";
                        LibelleNavigateToEquipement = this.EqEquipementEqEquipement.FirstOrDefault().Libelle;
                        

                        return new Uri(string.Format("/{0}/{1}/{2}/Id={3}",
                              MainNavigation.GestionOuvrages.GetStringValue(),
                              OuvrageNavigation.Equipement.GetStringValue(),
                              this.TypeEquipement.CodeEquipement,
                              this.EqEquipementEqEquipement.FirstOrDefault().CleEquipement), UriKind.Relative);

                    }
                    else if (IssuDeplacement)
                    {
                        LibelleNavigateToEqText = "Origine : ";
                        LibelleNavigateToEquipement = this.EqEquipement2.Libelle;
                        
                        return new Uri(String.Format("/{0}/{1}/{2}/Id={3}",
                          MainNavigation.GestionOuvrages.GetStringValue(),
                          OuvrageNavigation.Equipement.GetStringValue(),
                          this.TypeEquipement.CodeEquipement,
                          CleEquipementOrigine), UriKind.Relative);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Url de la PP de rattachement
        /// </summary>
        public Uri NaviagtionPPUrl
        {
            get
            {
                if (!IsNew)
                {
                    return new Uri(string.Format("/{0}/{1}/PP/Id={2}",
                       MainNavigation.GestionOuvrages.GetStringValue(),
                       OuvrageNavigation.Equipement.GetStringValue(),
                       ClePp), UriKind.Relative);
                }
                return null;
            }
        }

        /// <summary>
        /// Url de l'élément courant
        /// </summary>
        public string NaviagtionUrl
        {
            get
            {
                if (TypeEquipement != null)
                    return string.Format("/{0}/{1}/{2}/Id={3}",
                       MainNavigation.GestionOuvrages.GetStringValue(),
                       OuvrageNavigation.Equipement.GetStringValue(),
                       TypeEquipement.CodeEquipement,
                       CleEquipement);
                else
                    return string.Empty;
            }
        }
        #endregion

        #region Override Methods

        /// <summary>
        /// retourne le libelle
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Libelle;
        }

        #endregion

        #region Public Methods

        public void ForceRaisePropertyChanged(String propertyName)
        {
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Fonction générant l'historique de l'entité
        /// </summary>
        /// <returns></returns>
        public virtual Entity GetHisto()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            Type HistoType = ass.GetType("Proteca.Web.Models.Histo" + this.GetType().Name);

            HistoEquipement monHisto = (HistoEquipement)Activator.CreateInstance(HistoType);

            // Champs communs EqEquipement
            monHisto.Libelle = this.Libelle;
            monHisto.LibellePortion = this.LibellePortion;
            monHisto.LibellePp = this.Pp.Libelle;
            monHisto.Commentaire = this.Commentaire;
            monHisto.DateMajCommentaire = this.DateMajCommentaire;
            monHisto.DateMiseEnService = this.DateMiseEnService;
            monHisto.Supprime = this.Supprime;
            monHisto.DateMajEquipement = this.DateMajEquipement;
            monHisto.TypeEquipement = this.TypeEquipement;

            return monHisto;
        }

        /// <summary>
        /// Fonction de duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public virtual Entity DeplacerEq()
        {
            // Duplication de l'équipement
            EqEquipement EqDupliquer = this.DuplicateEq() as EqEquipement;

            // Renommage des éléments
            this.RenommerEq();

            // On duplique les niveaux de protection
            foreach (var item in this.MesNiveauProtection)
            {
                EqDupliquer.MesNiveauProtection.Add(item.DupliquerMesNiveauProtection());
            }

            // Suppression logique de l'équipement
            this.Supprime = true;

            return EqDupliquer;
        }

        /// <summary>
        /// Fonction de duplication de l'équipement
        /// </summary>
        /// <returns></returns>
        public virtual Entity DuplicateEq()
        {
            EqEquipement moneq = (EqEquipement)Activator.CreateInstance(this.GetType());

            // Champs communs EqEquipement
            moneq.Libelle = this.Libelle;
            //moneq.LibellePortion = this.LibellePortion;
            moneq.Commentaire = this.Commentaire;
            moneq.DateMajCommentaire = this.DateMajCommentaire;
            moneq.DateMiseEnService = this.DateMiseEnService;
            moneq.Supprime = this.Supprime;
            moneq.DateMajEquipement = this.DateMajEquipement;
            moneq.TypeEquipement = this.TypeEquipement;
            moneq.CleEquipementOrigine = this.CleEquipement;
            return moneq;
        }

        /// <summary>
        /// Fonction de renommage de l'équipement
        /// </summary>
        /// <returns></returns>
        public virtual void RenommerEq()
        {
            // Modification du nom de l'ancien Equipement
            int increment = 0;
            int incrementParsed = increment;
            if (this.EqEquipement2 != null)
            {
                if(int.TryParse(this.EqEquipement2.Libelle.Substring(1, 1).ToString(), out incrementParsed))
                { 
                    increment = incrementParsed; 
                }
                increment += 1;
            }

            this.Libelle = string.Format("X{0}-" + this.Libelle, increment);
        }

        #endregion

        #region Event

        /// <summary>
        /// Specifités à la modification d'une propriété de l'équipement
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "ClePp")
            {
                RaisePropertyChanged("NaviagtionPPUrl");
            }
            if (e.PropertyName == "EntityState" && this.EntityState == System.ServiceModel.DomainServices.Client.EntityState.Unmodified)
            {
                ChildChanges.Clear();
            }
        }

        #endregion

        public XElement CreateXElement()
        {
            XElement xEqEquipement = new XElement("EQ");

            xEqEquipement.Add(new XElement("CleTypeEvaluation", Composition.EnumTypeEval));
            xEqEquipement.Add(new XElement("TypeEvaluation", Composition.RefEnumValeur.LibelleCourt));

            xEqEquipement.Add(new XElement("CleEquipement", CleEquipement));
            xEqEquipement.Add(new XElement("ClePp", ClePp));
            xEqEquipement.Add(new XElement("CleTypeEq", CleTypeEq));
            xEqEquipement.Add(new XElement("TypeEquipement", TypeEquipement.CodeEquipement));
            xEqEquipement.Add(new XElement("Libelle", Libelle));

            XElement xVisites = new XElement("Visites");
            foreach (Visite visite in Visites)
            {
                xVisites.Add(visite.IsNew() ? visite.CreateXVisite() : new XElement("Visite"));
            }
            xEqEquipement.Add(xVisites);

            return xEqEquipement;
        }
    }
}
