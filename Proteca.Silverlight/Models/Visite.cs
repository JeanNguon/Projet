using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Proteca.Silverlight.Helpers;
using System.Xml.Linq;
using Proteca.Silverlight.Resources;
using System.ComponentModel;

namespace Proteca.Web.Models
{
    public partial class Visite
    {
        public Boolean IsNewInOfflineMode { get; set; }

        public Boolean IsDuplicated { get; set; }

        private bool _canEditGeo;
        public bool CanEditGeo
        {
            get
            {
                return _canEditGeo;
            }
            set
            {
                _canEditGeo = value;
                this.RaisePropertyChanged("CanEditGeo");
            }
        }

        public IOuvrage Ouvrage
        {
            get
            {
                if (this.CleEqTmp.HasValue)
                {
                    return this.EqEquipementTmp;
                }
                else if (this.CleEquipement.HasValue)
                {
                    return this.EqEquipement;
                }
                else if (this.ClePp.HasValue)
                {
                    return this.Pp;
                }
                else
                {
                    return null;
                }
            }
        }

        public string Libelle
        {
            get
            {
                if (this.ClePp.HasValue && this.Pp != null)
                {
                    return this.Pp.Libelle + " / " + this.DateVisiteFormate;
                }
                else if (this.CleEquipement.HasValue && this.EqEquipement != null)
                {
                    return this.EqEquipement.Libelle + " / " + this.DateVisiteFormate;
                }
                else if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null)
                {
                    return this.EqEquipementTmp.Libelle + " / " + this.DateVisiteFormate;
                }
                else
                {
                    return this.DateVisiteFormate;
                }
            }
        }

        public string DateVisiteFormate
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy HH:mm}", this.DateVisite);
            }
        }


        public Nullable<DateTime> DateVisiteEditable
        {
            get
            {
                return this.DateVisite;
            }
            set
            {
                this.DateVisite = value;
                if (this.Ouvrage != null && this.DateVisite.HasValue)
                {
                    if (this.Ouvrage.VisitePeriodeDebut.HasValue && this.Ouvrage.VisitePeriodeDebut > this.DateVisite)
                    {
                        this.Ouvrage.VisitePeriodeDebut = this.DateVisite;
                    }
                    if (this.Ouvrage.VisitePeriodeFin.HasValue && this.Ouvrage.VisitePeriodeFin < this.DateVisite)
                    {
                        this.Ouvrage.VisitePeriodeFin = this.DateVisite;
                    }
                }
            }
        }

        public AnAnalyseSerieMesure Analyse
        {
            get
            {
                return this.AnAnalyseSerieMesure.OrderByDescending(a => a.CleAnalyse).FirstOrDefault();
            }
        }

        public void RaiseAnyPropertyChanged(string prop)
        {
            this.RaisePropertyChanged(prop);
        }

        public Alerte Alerte
        {
            get
            {
                return this.Alertes.FirstOrDefault(a => a.RefEnumValeur != null && a.RefEnumValeur.Valeur == "U");
            }
        }

        public void ForceRaisePropertyChanged(String propertyName)
        {
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Url de l'élément courant
        /// </summary>
        public string NaviagtionUrl
        {
            get
            {
                return string.Format("/{0}/{1}/Id={2}",
                   MainNavigation.Visite.GetStringValue(),
                   VisiteNavigation.SaisieVisite.GetStringValue(),
                   CleVisite);

            }
        }

        public List<MesMesure> MesuresStandarts
        {
            get
            {
                return this.MesMesure.Where(m => m.MesTypeMesure.MesureComplementaire == false).ToList();
            }
        }

        public List<MesMesure> MesureComplementaires
        {
            get
            {
                return this.MesMesure.Where(m => m.MesTypeMesure.MesureComplementaire == true).ToList();
            }
        }

        private ObservableCollection<VisiteMesure> _visiteMesures;
        public ObservableCollection<VisiteMesure> VisiteMesures
        {
            get
            {
                if (_visiteMesures == null)
                {
                    _visiteMesures = new ObservableCollection<VisiteMesure>();
                }
                return _visiteMesures;
            }
            set
            {
                _visiteMesures = value;
            }
        }

        private ObservableCollection<VisiteMesure> _visiteMesuresComplementaire;
        public ObservableCollection<VisiteMesure> VisiteMesuresComplementaires
        {
            get
            {
                if (_visiteMesuresComplementaire == null)
                {
                    _visiteMesuresComplementaire = new ObservableCollection<VisiteMesure>();
                }
                return _visiteMesuresComplementaire;
            }
            set
            {
                _visiteMesuresComplementaire = value;
            }
        }

        public Boolean HasVisiteMesureComplementaire
        {
            get
            {
                return VisiteMesuresComplementaires.Any();
            }
        }

        public Boolean HasVisiteMesure
        {
            get
            {
                return VisiteMesures.Any();
            }
        }

        public Boolean IsPartiel
        {
            get
            {
                return VisiteMesures.Any(vm => (vm.IsMiniEnable && vm.Mini != null && !vm.Mini.Valeur.HasValue)
                                            || (vm.IsMoyenEnable && vm.Moyen != null && !vm.Moyen.Valeur.HasValue)
                                            || (vm.IsMaxiEnable && vm.Maxi != null && !vm.Maxi.Valeur.HasValue));
            }
        }

        /// <summary>
        /// Chargement des VisiteMesures
        /// </summary>
        public void LoadVisiteMesures(ObservableCollection<MesClassementMesure> classementsMesure, Action<MesMesure> gestionUAltCallBack, Boolean ForceLoadNewMesure = false, Boolean DisableCreateNewMesure = false)
        {
            if (classementsMesure != null)
            {
                VisiteMesures.Clear();
                VisiteMesuresComplementaires.Clear();

                List<MesClassementMesure> classements = new List<MesClassementMesure>();
                if (this.ClePp.HasValue && this.Pp != null && classementsMesure.Any())
                {
                    classements = classementsMesure.Where(c => c.MesTypeMesure.MesureEnService && c.MesTypeMesure.MesModeleMesure.TypeEquipement != null && c.MesTypeMesure.MesModeleMesure.TypeEquipement.CodeEquipement == "PP"
                        && c.CourantsAlternatifsInduits == this.Pp.CourantsAlternatifsInduits
                        && c.CourantsVagabons == this.Pp.CourantsVagabonds
                        && c.ElectrodeEnterreeAmovible == this.Pp.ElectrodeEnterreeAmovible
                        && c.Telemesure == this.Pp.PresenceDUneTelemesure
                        && c.TemoinDeSurface == this.Pp.TemoinMetalliqueDeSurface
                        && c.TemoinEnterre == this.Pp.TemoinEnterreAmovible
                        && (c.MesTypeMesure.TypeEvaluation == this.EnumTypeEval
                            || (c.MesTypeMesure.RefEnumValeur1 != null && c.MesTypeMesure.RefEnumValeur1.Valeur == "1" && this.RefEnumValeur != null && this.RefEnumValeur.Valeur == "2"))
                       ).ToList();
                }
                else if (this.EqEquipement != null && this.EqEquipement.Pp != null && classementsMesure.Any())
                {
                    classements = classementsMesure.Where(c => c.MesTypeMesure.MesureEnService && c.MesTypeMesure.MesModeleMesure.CleTypeEq == this.EqEquipement.CleTypeEq
                        && c.CourantsAlternatifsInduits == this.EqEquipement.Pp.CourantsAlternatifsInduits
                        && c.CourantsVagabons == this.EqEquipement.Pp.CourantsVagabonds
                        && c.ElectrodeEnterreeAmovible == this.EqEquipement.Pp.ElectrodeEnterreeAmovible
                        && c.Telemesure == this.EqEquipement.Pp.PresenceDUneTelemesure
                        && c.TemoinDeSurface == this.EqEquipement.Pp.TemoinMetalliqueDeSurface
                        && c.TemoinEnterre == this.EqEquipement.Pp.TemoinEnterreAmovible
                        && (c.MesTypeMesure.TypeEvaluation == this.EnumTypeEval
                            || (c.MesTypeMesure.RefEnumValeur1 != null && c.MesTypeMesure.RefEnumValeur1.Valeur == "1" && this.RefEnumValeur != null && this.RefEnumValeur.Valeur == "2"))
                       ).ToList();
                }
                else if (this.EqEquipementTmp != null && this.EqEquipementTmp.Pp != null && classementsMesure.Any())
                {
                    classements = classementsMesure.Where(c => c.MesTypeMesure.MesureEnService && c.MesTypeMesure.MesModeleMesure.CleTypeEq == this.EqEquipementTmp.CleTypeEq
                        && c.CourantsAlternatifsInduits == this.EqEquipementTmp.Pp.CourantsAlternatifsInduits
                        && c.CourantsVagabons == this.EqEquipementTmp.Pp.CourantsVagabonds
                        && c.ElectrodeEnterreeAmovible == this.EqEquipementTmp.Pp.ElectrodeEnterreeAmovible
                        && c.Telemesure == this.EqEquipementTmp.Pp.PresenceDUneTelemesure
                        && c.TemoinDeSurface == this.EqEquipementTmp.Pp.TemoinMetalliqueDeSurface
                        && c.TemoinEnterre == this.EqEquipementTmp.Pp.TemoinEnterreAmovible
                        && (c.MesTypeMesure.TypeEvaluation == this.EnumTypeEval
                            || (c.MesTypeMesure.RefEnumValeur1 != null && c.MesTypeMesure.RefEnumValeur1.Valeur == "1" && this.RefEnumValeur != null && this.RefEnumValeur.Valeur == "2"))
                       ).ToList();
                }

                IEnumerable<MesModeleMesure> modeles = classements.Select(c => c.MesTypeMesure.MesModeleMesure);
                // Ajout des anciens modèles de mesure
                modeles = modeles.Union(this.MesMesure.Where(m => !m.IsNew()).Select(m => m.MesTypeMesure.MesModeleMesure)).Distinct().OrderBy(m => m.NumeroOrdre);
                foreach (MesModeleMesure modele in modeles)
                {
                    String LibGenerique = (modele.LibGenerique == null) ? String.Empty : modele.LibGenerique + " (" + modele.MesUnite.Symbole + ")";
                    if (modele.MesTypeMesure.Any(t => !t.MesureComplementaire) && !VisiteMesures.Any(vm => vm.Libelle == LibGenerique))
                    {
                        VisiteMesure vmStd = CreateVisiteMesure(classements.Where(c => c.MesTypeMesure.MesModeleMesure.LibGenerique == modele.LibGenerique && c.MesTypeMesure.MesModeleMesure.CleUnite == modele.CleUnite), modeles.Where(m => m.LibGenerique == modele.LibGenerique && m.CleUnite == modele.CleUnite), false, gestionUAltCallBack, ForceLoadNewMesure, DisableCreateNewMesure);
                        VisiteMesures.Add(vmStd);
                    }

                    if (modele.MesTypeMesure.Any(t => t.MesureComplementaire) && !VisiteMesuresComplementaires.Any(vm => vm.Libelle == LibGenerique))
                    {
                        VisiteMesure vmComp = CreateVisiteMesure(classements.Where(c => c.MesTypeMesure.MesModeleMesure.LibGenerique == modele.LibGenerique && c.MesTypeMesure.MesModeleMesure.CleUnite == modele.CleUnite), modeles.Where(m => m.LibGenerique == modele.LibGenerique && m.CleUnite == modele.CleUnite), true, gestionUAltCallBack, ForceLoadNewMesure, DisableCreateNewMesure);
                        VisiteMesuresComplementaires.Add(vmComp);
                    }
                }

                RaisePropertyChanged("HasVisiteMesure");
                RaisePropertyChanged("HasVisiteMesureComplementaire");
            }
        }

        private VisiteMesure CreateVisiteMesure(IEnumerable<MesClassementMesure> classements, IEnumerable<MesModeleMesure> modeles, Boolean isComplementaire, Action<MesMesure> gestionUAltCallBack, Boolean ForceLoadNewMesure = false, Boolean DisableCreateNewMesure = false)
        {
            MesModeleMesure firstModele = modeles.FirstOrDefault();
            VisiteMesure vm = new VisiteMesure()
            {
                Libelle = (firstModele.LibGenerique == null) ? String.Empty : firstModele.LibGenerique + " (" + firstModele.MesUnite.Symbole + ")",
                Maxi = this.MesMesure.Where(m => m != null
                                            && (!m.IsNew() || ForceLoadNewMesure)
                                            && m.MesTypeMesure != null
                                            && m.MesTypeMesure.MesureComplementaire == isComplementaire
                                            && m.MesTypeMesure.MesModeleMesure != null
                                            && m.MesTypeMesure.MesModeleMesure.LibGenerique == firstModele.LibGenerique
                                            && m.MesTypeMesure.RefEnumValeur != null
                                            && m.MesTypeMesure.RefEnumValeur.Valeur == "2")
                                    .OrderByDescending(m => m.Valeur.HasValue)
                                    .ThenBy(m => m.MesTypeMesure.MesModeleMesure.NumeroOrdre)
                                    .ThenBy(m => m.MesTypeMesure.NumeroOrdre)
                                    .ThenByDescending(m => m.CleMesure)
                                    .FirstOrDefault(),
                Mini = this.MesMesure.Where(m => m != null
                                            && (!m.IsNew() || ForceLoadNewMesure)
                                            && m.MesTypeMesure != null
                                            && m.MesTypeMesure.MesureComplementaire == isComplementaire
                                            && m.MesTypeMesure.MesModeleMesure != null
                                            && m.MesTypeMesure.MesModeleMesure.LibGenerique == firstModele.LibGenerique
                                            && m.MesTypeMesure.RefEnumValeur != null
                                            && m.MesTypeMesure.RefEnumValeur.Valeur == "0")
                                    .OrderByDescending(m => m.Valeur.HasValue)
                                    .ThenBy(m => m.MesTypeMesure.MesModeleMesure.NumeroOrdre)
                                    .ThenBy(m => m.MesTypeMesure.NumeroOrdre)
                                    .ThenByDescending(m => m.CleMesure)
                                    .FirstOrDefault(),
                Moyen = this.MesMesure.Where(m => m != null
                                            && (!m.IsNew() || ForceLoadNewMesure)
                                            && m.MesTypeMesure != null
                                            && m.MesTypeMesure.MesureComplementaire == isComplementaire
                                            && m.MesTypeMesure.MesModeleMesure != null
                                            && m.MesTypeMesure.MesModeleMesure.LibGenerique == firstModele.LibGenerique
                                            && m.MesTypeMesure.RefEnumValeur != null
                                            && (m.MesTypeMesure.RefEnumValeur.Valeur == "1" || m.MesTypeMesure.RefEnumValeur.Valeur == "4"))
                                    .OrderByDescending(m => m.Valeur.HasValue)
                                    .ThenBy(m => m.MesTypeMesure.MesModeleMesure.NumeroOrdre)
                                    .ThenBy(m => m.MesTypeMesure.NumeroOrdre)
                                    .ThenByDescending(m => m.CleMesure)
                                    .FirstOrDefault(),
                IsMaxiEnable = classements.Any(c => c != null && c.MesTypeMesure != null && c.MesTypeMesure.MesureEnService && c.MesTypeMesure.MesureComplementaire == isComplementaire && c.MesTypeMesure.RefEnumValeur != null && c.MesTypeMesure.RefEnumValeur.Valeur == "2"),
                IsMiniEnable = classements.Any(c => c != null && c.MesTypeMesure != null && c.MesTypeMesure.MesureEnService && c.MesTypeMesure.MesureComplementaire == isComplementaire && c.MesTypeMesure.RefEnumValeur != null && c.MesTypeMesure.RefEnumValeur.Valeur == "0"),
                IsMoyenEnable = classements.Any(c => c != null && c.MesTypeMesure != null && c.MesTypeMesure.MesureEnService && c.MesTypeMesure.MesureComplementaire == isComplementaire && c.MesTypeMesure.RefEnumValeur != null && (c.MesTypeMesure.RefEnumValeur.Valeur == "1" || c.MesTypeMesure.RefEnumValeur.Valeur == "4"))
            };

            Visite previous = null;
            if ((this.ClePp.HasValue && this.Pp != null)
                || (this.CleEquipement.HasValue && this.EqEquipement != null)
                || this.EqEquipementTmp != null)
            {
                previous = ((this.ClePp.HasValue) ? this.Pp.Visites.Where(v => v.EstValidee && v.CleVisite != this.CleVisite && (v.DateVisite < this.DateVisite || !this.DateVisite.HasValue)).OrderBy(v => v.DateVisite).LastOrDefault()
                            : ((this.CleEquipement.HasValue) ? this.EqEquipement.Visites.Where(v => v.EstValidee && v.CleVisite != this.CleVisite && (v.DateVisite < this.DateVisite || !this.DateVisite.HasValue)).OrderBy(v => v.DateVisite).LastOrDefault()
                            : this.EqEquipementTmp.Visites.Where(v => v.EstValidee && v.CleVisite != this.CleVisite && (v.DateVisite < this.DateVisite || !this.DateVisite.HasValue)).OrderBy(v => v.DateVisite).LastOrDefault()));
                vm.Precedente = (previous == null) ? null
                                                   : previous.MesMesure.Where(m => m != null
                                                                                && m.MesTypeMesure != null
                                                                                && m.MesTypeMesure.MesureComplementaire == isComplementaire
                                                                                && m.MesTypeMesure.MesureEnService
                                                                                && m.MesTypeMesure.MesModeleMesure != null
                                                                                && m.MesTypeMesure.MesModeleMesure.LibGenerique == firstModele.LibGenerique
                                                                                && m.MesTypeMesure.RefEnumValeur != null
                                                                                && (m.MesTypeMesure.RefEnumValeur.Valeur == "1" || m.MesTypeMesure.RefEnumValeur.Valeur == "4"))
                                                                       .OrderByDescending(m => m.Valeur.HasValue)
                                                                       .ThenBy(m => m.MesTypeMesure.MesModeleMesure.NumeroOrdre)
                                                                       .ThenBy(m => m.MesTypeMesure.NumeroOrdre)
                                                                       .OrderByDescending(m => m.CleMesure)
                                                                       .FirstOrDefault();
            }

            if (!DisableCreateNewMesure && vm.Mini == null && vm.IsMiniEnable)
            {
                vm.Mini = new MesMesure() { CleTypeMesure = modeles.SelectMany(m => m.MesTypeMesure).OrderBy(t => t.MesModeleMesure.NumeroOrdre).ThenBy(t => t.NumeroOrdre).First(t => t.MesureEnService && t.MesureComplementaire == isComplementaire && t.RefEnumValeur != null && t.RefEnumValeur.Valeur == "0").CleTypeMesure };
                this.MesMesure.Add(vm.Mini);
            }

            if (!DisableCreateNewMesure && vm.Maxi == null && vm.IsMaxiEnable)
            {
                vm.Maxi = new MesMesure() { CleTypeMesure = modeles.SelectMany(m => m.MesTypeMesure).OrderBy(t => t.MesModeleMesure.NumeroOrdre).ThenBy(t => t.NumeroOrdre).First(t => t.MesureEnService && t.MesureComplementaire == isComplementaire && t.RefEnumValeur != null && t.RefEnumValeur.Valeur == "2").CleTypeMesure };
                this.MesMesure.Add(vm.Maxi);
            }

            if (!DisableCreateNewMesure && vm.Moyen == null && vm.IsMoyenEnable)
            {
                vm.Moyen = new MesMesure() { CleTypeMesure = modeles.SelectMany(m => m.MesTypeMesure).OrderBy(t => t.MesModeleMesure.NumeroOrdre).ThenBy(t => t.NumeroOrdre).First(t => t.MesureEnService && t.MesureComplementaire == isComplementaire && t.RefEnumValeur != null && (t.RefEnumValeur.Valeur == "1" || t.RefEnumValeur.Valeur == "4")).CleTypeMesure };
                this.MesMesure.Add(vm.Moyen);
            }

            // 
            if (firstModele.EnumTypeGraphique != null && firstModele.RefEnumValeur.Valeur == "Ucana ~" && gestionUAltCallBack != null)
            {
                if (vm.Mini != null)
                {
                    PropertyChangedEventHandler anyPropertyChanged = null;
                    anyPropertyChanged = (o, e) =>
                    {
                        if (e.PropertyName == "Valeur" && (o as MesMesure) != null && (o as MesMesure).Valeur.HasValue)
                        {
                            gestionUAltCallBack((o as MesMesure));
                        }
                    };

                    vm.Mini.PropertyChanged -= anyPropertyChanged;
                    vm.Mini.PropertyChanged += anyPropertyChanged;

                    //vm.Mini.PropertyChanged += (o, e) =>
                    //{
                    //    if (e.PropertyName == "Valeur" && (o as MesMesure) != null && (o as MesMesure).Valeur.HasValue)
                    //    {
                    //        gestionUAltCallBack((o as MesMesure));
                    //    }
                    //};
                }
                if (vm.Moyen != null)
                {
                    PropertyChangedEventHandler anyPropertyChanged = null;
                    anyPropertyChanged = (o, e) =>
                    {
                        if (e.PropertyName == "Valeur" && (o as MesMesure) != null && (o as MesMesure).Valeur.HasValue)
                        {
                            gestionUAltCallBack((o as MesMesure));
                        }
                    };

                    vm.Moyen.PropertyChanged -= anyPropertyChanged;
                    vm.Moyen.PropertyChanged += anyPropertyChanged;
                }
                if (vm.Maxi != null)
                {
                    PropertyChangedEventHandler anyPropertyChanged = null;
                    anyPropertyChanged = (o, e) =>
                    {
                        if (e.PropertyName == "Valeur" && (o as MesMesure) != null && (o as MesMesure).Valeur.HasValue)
                        {
                            gestionUAltCallBack((o as MesMesure));
                        }
                    };

                    vm.Maxi.PropertyChanged -= anyPropertyChanged;
                    vm.Maxi.PropertyChanged += anyPropertyChanged;
                }
            }

            return vm;
        }

        private Pp _ppReliee;
        public Pp PpReliee
        {
            get
            {
                if (_ppReliee == null)
                {
                    if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null && this.EqEquipementTmp.Pp != null)
                    {
                        _ppReliee = this.EqEquipementTmp.Pp;
                    }
                    else if (this.CleEquipement.HasValue && this.EqEquipement != null && this.EqEquipement.Pp != null)
                    {
                        _ppReliee = this.EqEquipement.Pp;
                    }
                    else if (this.ClePp.HasValue && this.Pp != null)
                    {
                        _ppReliee = this.Pp;
                    }
                    else if (this.ClePpTmp.HasValue && this.PpTmp != null && this.PpTmp.Pp != null)
                    {
                        _ppReliee = this.PpTmp.Pp;
                    }
                }
                return _ppReliee;
            }
        }

        public string LibellePortion
        {
            get
            {
                if (this.PpReliee != null && this.PpReliee.PortionIntegrite != null)
                {
                    return this.PpReliee.PortionIntegrite.Libelle;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string LibelleOuvrage
        {
            get
            {
                if (this.CleEqTmp.HasValue && this.EqEquipementTmp != null)
                {
                    return this.EqEquipementTmp.Libelle;
                }
                else if (this.CleEquipement.HasValue && this.EqEquipement != null)
                {
                    return this.EqEquipement.Libelle;
                }
                else if (this.ClePp.HasValue && Pp != null)
                {
                    return this.Pp.Libelle;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public Nullable<decimal> Pk
        {
            get
            {
                if (this.PpReliee != null)
                {
                    return this.PpReliee.Pk;
                }
                else
                {
                    return null;
                }
            }
        }

        public XElement CreateXVisite()
        {
            XElement xVisite = new XElement("VisiteProteIN");
            xVisite.Add(new XElement("CleUtilisateur", this.CleUtilisateurMesure));
            xVisite.Add(new XElement("EnumTypeEval", this.EnumTypeEval));
            xVisite.Add(new XElement("ClePP", this.ClePp));
            xVisite.Add(new XElement("CleEquipement", this.CleEquipement));

            xVisite.Add(new XElement("DateVisite", this.DateVisite));
            xVisite.Add(new XElement("CommentaireVisite", this.Commentaire));
            xVisite.Add(new XElement("CommentaireAnalyse", this.Analyse != null ? this.Analyse.Commentaire : null));
            xVisite.Add(new XElement("CleAnalyse", this.Analyse != null ? this.Analyse.EnumEtatPc : null));

            xVisite.Add(new XElement("EstConfirmee", true));

            XElement xMesMesures = new XElement("MesMesures");
            foreach (MesMesure mesMesure in MesMesure)
            {
                xMesMesures.Add(mesMesure.CreateXMesMesure());
            }
            xVisite.Add(xMesMesures);

            return xVisite;
        }

        public String VisiteSerialized
        {
            get
            {
                return String.Join("\n", this.VisiteToText());
            }
        }

        public List<String> VisiteToText()
        {
            List<String> result = new List<String>();
            string temp;
            List<String> tmp = new List<String>();
            if (this.UsrUtilisateur2 != null)
            {
                //Partie Visite
                result.Add(Resource.SaisieVisite_AgentMesure.Replace("*", "") + ' ' + this.UsrUtilisateur2.Nom_Prenom);
                result.Add(Resource.SaisieVisite_DateVisite.Replace("*", "") + ' ' + this.DateVisiteFormate);
                result.Add("Date d'import : " + String.Format("{0:dd/MM/yyyy HH:mm}", this.DateImport));
                result.Add(Resource.SaisieVisite_InstrumentMesure);
                foreach (InstrumentsUtilises ins in this.InstrumentsUtilises.Where(i => i.InsInstrument != null))
                {
                    result.Add('\t' + "- " + ins.InsInstrument.Libelle);
                }
                //Partie Mesures
                List<MesMesure> mesures = this.MesMesure.Where(m => m.MesTypeMesure != null && m.MesTypeMesure.MesModeleMesure != null && !m.MesTypeMesure.MesureComplementaire && m.Valeur.HasValue).OrderBy(m => m.MesTypeMesure.MesModeleMesure.NumeroOrdre).ToList();
                if (mesures.Any())
                {
                    result.Add(Resource.ValidationVisite_MesuresStd);
                    foreach (MesMesure mesure in mesures)
                    {
                        temp = mesure.MesMesureToText;
                        if (!String.IsNullOrEmpty(temp))
                        {
                            result.Add('\t' + temp);
                        }
                    }
                }
                mesures = this.MesMesure.Where(m => m.MesTypeMesure != null && m.MesTypeMesure.MesModeleMesure != null && m.MesTypeMesure.MesureComplementaire && m.Valeur.HasValue).OrderBy(m => m.MesTypeMesure.MesModeleMesure.NumeroOrdre).ToList();
                if (mesures.Any())
                {
                    result.Add(Resource.ValidationVisite_MesuresComp);
                    foreach (MesMesure mesure in mesures)
                    {
                        temp = mesure.MesMesureToText;
                        if (!String.IsNullOrEmpty(temp))
                        {
                            result.Add('\t' + temp);
                        }
                    }
                }
                //Partie Analyse
                if (this.Analyse != null)
                {
                    tmp = this.Analyse.AnalyseToText;
                    if (tmp.Any())
                    {
                        result.Add(Resource.VisiteAnalyse_Titre + " :");
                        result.AddRange(tmp.Select(s => { return '\t' + s; }));
                    }
                }
                result.Add(Resource.VisiteCommentaire + ' ' + this.Commentaire);
            }

            return result;
        }



    }
}
