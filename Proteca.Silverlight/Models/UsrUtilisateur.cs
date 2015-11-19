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
using Proteca.Silverlight.Models;
using Jounce.Core.ViewModel;
using System.ServiceModel.DomainServices.Client;
using Proteca.Silverlight.Enums.NavigationEnums;
using Proteca.Silverlight.Enums;
using System.Linq;

namespace Proteca.Web.Models
{
    public partial class UsrUtilisateur
    {
        #region Private Members
        
        private GeoRegion _selectedRegion;
        private int? _preferenceCleRegion;
        private int? _preferenceCleAgence;
        private int? _preferenceCleSecteur;
        private int? _preferenceCleEnsembleElectrique;
        private int? _preferenceClePortion;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Région sélectionnée
        /// </summary>
        public GeoRegion SelectedRegion
        {
            get
            {
                if (GeoAgence != null)
                {
                    _selectedRegion = GeoAgence.GeoRegion;
                }
                return _selectedRegion;
            }
            set
            {
                _selectedRegion = value;

                // Suppression de l'erreur de validation lors de la sélection d'une région
                if (_selectedRegion != null && this.ValidationErrors.Any(ve => ve.MemberNames.Contains("SelectedRegion")))
                {
                    this.ValidationErrors.Remove(this.ValidationErrors.First(ve => ve.MemberNames.Contains("SelectedRegion")));
                }
            }
        }

        /// <summary>
        /// Agence sélectionnée
        /// </summary>
        public GeoAgence SelectedAgence
        {
            get { return GeoAgence; }
            set
            {
                if (IsEditable)
                {
                    GeoAgence = value;
                }
            }
        }

        /// <summary>
        /// Secteur sélectionné
        /// </summary>
        public GeoSecteur SelectedSecteur
        {
            get { return GeoSecteur; }
            set
            {
                if (IsEditable)
                {
                    GeoSecteur = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsEditable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Nom_Prenom
        {
            get
            {
                return Nom != null && Prenom != null ? Nom.ToUpper() + " " + Prenom.ToUpper() : "";
            }
        }
        
        /// <summary>
        /// Cle de la région de préférence
        /// </summary>
        public int? PreferenceCleRegion
        {
            get
            {
                return _preferenceCleRegion;
            }
        }
        
        /// <summary>
        /// Cle de l'agence de préférence
        /// </summary>
        public int? PreferenceCleAgence
        {
            get
            {
                return _preferenceCleAgence;
            }
        }
        
        /// <summary>
        /// Clé du secteur de préférence
        /// </summary>
        public int? PreferenceCleSecteur
        {
            get
            {
                return _preferenceCleSecteur;
            }
        }

        /// <summary>
        /// Clé de l'ensemble électrique de préférence
        /// </summary>
        public int? PreferenceCleEnsembleElectrique
        {
            get
            {
                return _preferenceCleEnsembleElectrique;
            }
        }

         /// <summary>
        /// Clé de la portion de préférence
        /// </summary>
        public int? PreferenceClePortion
        {
            get
            {
                return _preferenceClePortion;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAdministrateur
        {
            get
            {
                return this.UsrProfil != null && this.UsrProfil.ProfilAdmin;
            }
        }
        
        #endregion Public Properties

        #region Public Functions

        /// <summary>
        /// Fixe la région de préférence
        /// </summary>
        /// <param name="cleRegion"></param>
        public void SetPreferenceCleRegion(int? cleRegion)
        {
            _preferenceCleRegion = cleRegion;
        }

        /// <summary>
        /// Fixe la l'agence de péférence
        /// </summary>
        /// <param name="cleAgence"></param>
        public void SetPreferenceCleAgence(int? cleAgence)
        {
            _preferenceCleAgence = cleAgence;
        }

        /// <summary>
        /// Fixe le secteur de préférence
        /// </summary>
        /// <param name="cleSecteur"></param>
        public void SetPreferenceCleSecteur(int? cleSecteur)
        {
            _preferenceCleSecteur = cleSecteur;
        }

        /// <summary>
        /// Fixe l'ensemble électrique de préférence
        /// </summary>
        /// <param name="cleEnsembleElectrique"></param>
        public void SetPreferenceCleEnsembleElectrique(int? cleEnsembleElectrique)
        {
            _preferenceCleEnsembleElectrique = cleEnsembleElectrique;
        }

        /// <summary>
        /// Fixe la portion de préférence
        /// </summary>
        /// <param name="clePortion"></param>
        public void SetPreferenceClePortion(int? clePortion)
        {
            _preferenceClePortion = clePortion;
        }

        /// <summary>
        /// Récupère le role en fonction du code d'autorisation
        /// </summary>
        /// <param name="autorisationCode"></param>
        /// <returns></returns>
        public UsrRole GetRoleByAutorisationCode(Proteca.Web.Models.RefUsrAutorisation.ListAutorisationsEnum autorisationCode)
        {
            UsrRole role = null;
            if (this.UsrProfil != null && this.UsrProfil.UsrRole != null)
            {
                role = this.UsrProfil.UsrRole.Where(r => r.RefUsrAutorisation != null && r.RefUsrAutorisation.CodeAutorisation == autorisationCode.ToString()).FirstOrDefault();
            }
            return role;
        }

        #endregion Public Functions

    }
}
