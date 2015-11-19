using System;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using System.Linq;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Helpers;

namespace Proteca.Web.Models
{
    public partial class PpTmp
    {
        public Uri NaviagtionPPUrl
        {
            get
            {
                return new Uri(string.Format("/{0}/{1}/PP/Id={2}",
                    MainNavigation.GestionOuvrages.GetStringValue(),
                    OuvrageNavigation.Equipement.GetStringValue(),
                    this.ClePp), UriKind.Relative);
            }
        }

        private bool _valider = false;
        public bool Valider
        {
            get
            {
                return _valider;
            }
            set
            {
                _valider = value;
                this.RaisePropertyChanged("Valider");
            }
        }

        private bool _canValidGeo;
        public bool CanValidGeo
        {
            get
            {
                return _canValidGeo;
            }
            set
            {
                _canValidGeo = value;
                this.RaisePropertyChanged("CanValidGeo");
            }
        }

        public bool CleCategoriePpChanged { get { return this.Pp.CleCategoriePp != this.CleCategoriePp; } }
        public bool CleNiveauSensibiliteChanged { get { return this.Pp.CleNiveauSensibilite != this.CleNiveauSensibilite; } }

        public bool EnumSurfaceTmeChanged { get { return this.Pp.EnumSurfaceTme != this.EnumSurfaceTme; } }
        public bool EnumSurfaceTmsChanged { get { return this.Pp.EnumSurfaceTms != this.EnumSurfaceTms; } }
        public bool EnumDureeEnrgChanged { get { return this.Pp.EnumDureeEnrg != this.EnumDureeEnrg; } }
        public bool EnumPolarisationChanged { get { return this.Pp.EnumPolarisation != this.EnumPolarisation; } }

        public bool CourantsAlternatifsInduitsChanged { get { return this.Pp.CourantsAlternatifsInduits != this.CourantsAlternatifsInduits; } }
        public bool CourantsVagabondsChanged { get { return this.Pp.CourantsVagabonds != this.CourantsVagabonds; } }
        public bool ElectrodeEnterreeAmovibleChanged { get { return this.Pp.ElectrodeEnterreeAmovible != this.ElectrodeEnterreeAmovible; } }
        public bool TemoinEnterreAmovibleChanged { get { return this.Pp.TemoinEnterreAmovible != this.TemoinEnterreAmovible; } }
        public bool TemoinMetalliqueDeSurfaceChanged { get { return this.Pp.TemoinMetalliqueDeSurface != this.TemoinMetalliqueDeSurface; } }
        public bool PresenceDUneTelemesureChanged { get { return this.Pp.PresenceDUneTelemesure != this.PresenceDUneTelemesure; } }

        public bool PositionGpsLatChanged { get { return this.Pp.PositionGpsLat != this.PositionGpsLat; } }
        public bool PositionGpsLongChanged { get { return this.Pp.PositionGpsLong != this.PositionGpsLong; } }
        public bool CoordonneeGpsFiabiliseeChanged { get { return this.Pp.CoordonneeGpsFiabilisee != this.CoordonneeGpsFiabilisee; } }

        public bool HasAnyChangesFromOriginalPP
        {
            get
            {
                return CleCategoriePpChanged
                || CleNiveauSensibiliteChanged
                
                || EnumSurfaceTmeChanged
                || EnumSurfaceTmsChanged
                || EnumDureeEnrgChanged
                || EnumPolarisationChanged
                
                || CourantsAlternatifsInduitsChanged
                || CourantsVagabondsChanged
                || ElectrodeEnterreeAmovibleChanged
                || TemoinEnterreAmovibleChanged
                || TemoinMetalliqueDeSurfaceChanged
                || PresenceDUneTelemesureChanged
                
                || PositionGpsLatChanged
                || PositionGpsLongChanged
                || CoordonneeGpsFiabiliseeChanged
                ;
            }
        }


        /// <summary>
        /// Commit des changements loggés dans cette PpTmp vers la Pp
        /// </summary>
        public void CommitChangesToPp()
        {
            if (this.Pp != null)
            {
                // 
                this.Pp.CleCategoriePp = this.CleCategoriePp;
                this.Pp.CleNiveauSensibilite = this.CleNiveauSensibilite;

                this.Pp.EnumDureeEnrg = this.EnumDureeEnrg;
                this.Pp.EnumPolarisation = this.EnumPolarisation;
                this.Pp.EnumSurfaceTme = this.EnumSurfaceTme;
                this.Pp.EnumSurfaceTms = this.EnumSurfaceTms;

                this.Pp.CourantsAlternatifsInduits = this.CourantsAlternatifsInduits;
                this.Pp.CourantsVagabonds = this.CourantsVagabonds;
                this.Pp.ElectrodeEnterreeAmovible = this.ElectrodeEnterreeAmovible;
                this.Pp.TemoinEnterreAmovible = this.TemoinEnterreAmovible;
                this.Pp.TemoinMetalliqueDeSurface = this.TemoinMetalliqueDeSurface;
                this.Pp.PresenceDUneTelemesure = this.PresenceDUneTelemesure;

                if (!this.Pp.CoordonneeGpsFiabilisee)
                {
                    this.Pp.PositionGpsLat = this.PositionGpsLat;
                    this.Pp.PositionGpsLong = this.PositionGpsLong;
                }

                this.Pp.CoordonneeGpsFiabilisee = this.CoordonneeGpsFiabilisee;

                for (int i = this.Visites.Count - 1; i > -1; i--)
                {
                    this.Pp.Visites.Add(this.Visites.ElementAt(i));
                    this.Visites.Remove(this.Visites.ElementAt(i));
                }

                this.Pp.BypassCategoriePp = true;
                this.Pp.BypassPkLimitation = true;
            }
        }

        public string LibellePortion
        {
            get
            {
                return this.Pp != null && this.Pp.PortionIntegrite != null ? this.Pp.PortionIntegrite.Libelle : String.Empty;
            }
        }

        public List<string> PpTmpToText()
        {
            string yes = "oui";
            string no = "non";

            List<string> result = new List<string>();

            result.Add(Resource.EqEquipement_PP_Classification.Replace("*", "") + ' ' + this.RefNiveauSensibilitePp.Libelle);
            if (this.CategoriePp != null && this.CategoriePp.RefNiveauSensibilitePp != null)
            {
                result.Add(Resource.EqEquipement_PP_Categorie.Replace("*", "") + ' ' + this.CategoriePp.RefNiveauSensibilitePp.Libelle);
            }
            //TODO : ajouter un titre ?
            result.Add(Resource.EqEquipement_chxCourantsVagabonds + " : " + (this.CourantsVagabonds ? yes : no));
            result.Add(Resource.EqEquipement_chxCourantsInduits + " : " + (this.CourantsAlternatifsInduits ? yes : no));
            result.Add(Resource.EqEquipement_chxElectrode + " : " + (this.ElectrodeEnterreeAmovible ? yes : no));
            result.Add(Resource.EqEquipement_chxTemoinEnterre + " : " + (this.TemoinEnterreAmovible ? yes : no));
            if (this.TemoinEnterreAmovible)
            {
                result.Add('\t' + Resource.EqEquipement_SurfaceTME.Replace("*", "") + ' ' + this.RefEnumValeur.Libelle + ' ' + Resource.EqEquipement_Cm2);
            }
            result.Add(Resource.EqEquipement_chxTemoinSurface + " : " + (this.TemoinMetalliqueDeSurface ? yes : no));
            if (this.TemoinMetalliqueDeSurface)
            {
                result.Add('\t' + Resource.EqEquipement_SurfaceTMS.Replace("*", "") + ' ' + this.RefEnumValeur1.Libelle + ' ' + Resource.EqEquipement_Cm2);
            }
            result.Add(Resource.EqEquipement_chxTelemesure + " : " + (this.PresenceDUneTelemesure ? yes : no));
            result.Add(Resource.EqEquipement_TpsPolarisation + (this.RefEnumValeur3 != null ? (' ' + this.RefEnumValeur3.Libelle) : String.Empty));
            result.Add(Resource.EqEquipement_DureeEnrgesitrement + (this.RefEnumValeur2 != null ? (' ' + this.RefEnumValeur2.Libelle) : String.Empty));
            result.Add(Resource.EqEquipementCoordonnees + " :" + (this.CoordonneeGpsFiabilisee ? (" (" + Resource.EqEquipement_Fiabilisees + ')') : String.Empty));
            result.Add('\t' + Resource.EqEquipement_Lattitude + ' ' + this.PositionGpsLat);
            result.Add('\t' + Resource.EqEquipement_Longitude + ' ' + this.PositionGpsLong);

            return result;
        }
    }
}
