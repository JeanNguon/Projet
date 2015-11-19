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
using System.Windows;
using System.Collections.Generic;
using Proteca.Silverlight.Models;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using Proteca.Silverlight.Services;
using Jounce.Framework.Workflow;
using System.Windows.Controls;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for AdminGenerateur entity
    /// </summary>
    [ExportAsViewModel("AdminGenerateur")]
    public class AdminGenerateurViewModel : BaseViewModel, IPartImportsSatisfiedNotification
    {

        #region Public Properties

        /// <summary>
        /// Indique que tous les services sont bien chargés
        /// </summary>
        public bool AllServicesLoaded { get; set; }

        public bool IsImportSatified { get; set; }

        public string OuvrageSuffixe01 { get; set; }
        public string VisiteSuffixe01 { get; set; }

        #endregion

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type T
        /// </summary>
        [Import]
        public IEntityService<EnsembleElectrique> ServiceEnsElectrique { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Import]
        public IEntityService<MesClassementMesure> ServiceMesClassementMesure { get; set; }

        /// <summary>
        /// Service utilisé pour naviguer dans l'application
        /// </summary>
        [Import(typeof(INavigationService))]
        public INavigationService NavigationService { get; set; }

        [Import]
        public IEntityService<RefEnumValeur> ServiceEnumValeur { get; set; }


        [Import]
        public SynchronizationService serviceSynchro { get; set; }


        #endregion

        #region Event

        /// <summary>
        /// 
        /// </summary>
        public EventHandler OnAllServicesLoaded;

        #endregion

        #region Command

        public IActionCommand OuvrageJeuTestCommand { get; set; }
        public IActionCommand VisiteJeuTestCommand { get; set; }
        public IActionCommand ExportContextCommand { get; set; }
        public IActionCommand ImportContextCommand { get; set; }

        #endregion

        #region Constructeur

        public AdminGenerateurViewModel()
            : base()
        {
            this.OnAllServicesLoaded += (o, e) =>
            {
                (this.ServiceMesClassementMesure as MesClassementMesureService).GetMesClassementMesureWithMesNiveauProtection(error =>
                {
                    if (error != null || ServiceMesClassementMesure.Entities == null || !ServiceMesClassementMesure.Entities.Any())
                    {
                        ErrorWindow.CreateNew("Erreur au chargement des MesMesures\nImpossible de créer un jeu de test équipement temporaire");
                    }
                });
            };

            OuvrageJeuTestCommand = new ActionCommand<object>(
                    obj => GenererJeuTestOuvrage());
            VisiteJeuTestCommand = new ActionCommand<object>(
                    obj => GenererJeuTestVisite());
            ExportContextCommand = new ActionCommand<object>(
               obj => ExporterContext());
            ImportContextCommand = new ActionCommand<object>(
               obj => ImporterContext());
        }

        #endregion

        #region Private Function

        private void ImporterContext()
        {
            string content = UploadContext();
            if (!string.IsNullOrEmpty(content))
            {
                serviceSynchro.ImporterContext(content);
            }
        }
        private void ExporterContext()
        {
            serviceSynchro.ExporterContext();
        }

        private string UploadContext()
        {
            string content = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            bool? userClickedOK = openFileDialog.ShowDialog();
            if (userClickedOK == true)
            {
                using (System.IO.Stream fileStream = openFileDialog.File.OpenRead())
                {
                    using (System.IO.TextReader reader = new System.IO.StreamReader(fileStream))
                    {
                        content = reader.ReadToEnd();
                    }
                    fileStream.Close();
                }
            }
            return content;

        }
        //private int servicesLoadedCount = 0;
        //private String lockObject = String.Empty;

        private void GenererJeuTestVisite()
        {
            string MonSuffixe = " " + VisiteSuffixe01;

            // Création d'un ensemble électrique
            EnsembleElectrique EnsElecToAdd = new EnsembleElectrique()
            {
                Libelle = "GOP - EnsElecTest" + MonSuffixe,
                EnumPeriodiciteNullable = 27
            };

            // Ajout de l'ensemble électrique au service
            ServiceEnsElectrique.Add(EnsElecToAdd);

            // Création de 2 portions intégrités
            PortionIntegrite PIToAdd01 = new PortionIntegrite()
            {
                Code = "TestPI01",
                DateMiseEnService = DateTime.Now,
                Libelle = "GOP - Portion01Test" + MonSuffixe,
                CleDiametre = 21,
                CleRevetement = 14,
                DateMajCommentaire = DateTime.Now
            };

            EnsElecToAdd.PortionIntegrite.Add(PIToAdd01);

            // Ajout d'un secteur sur chaque portion
            PiSecteurs PISToPortion01 = new PiSecteurs()
            {
                CleSecteur = 55
            };

            PIToAdd01.PiSecteurs.Add(PISToPortion01);

            // Création d'une PP
            Pp PpToPortion01 = new Pp()
            {
                Libelle = "GOP - PP01Test" + MonSuffixe,
                CleNiveauSensibilite = 6,
                CleCategoriePp = 21,
                PkNullable = 1,
                CleSecteur = 55,
                CleCommune = 21488,
                CleUtilisateur = 1745,
                DateMajPp = DateTime.Now,
                CourantsVagabonds = true,
                CourantsAlternatifsInduits = false,
                ElectrodeEnterreeAmovible = false,
                TemoinEnterreAmovible = false,
                TemoinMetalliqueDeSurface = true,
                EnumSurfaceTms = ServiceEnumValeur.Entities.FirstOrDefault(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.PP_SURFACE_TMS.ToString()).CleEnumValeur
            };

            PIToAdd01.Pps.Add(PpToPortion01);

            //Création d'un équipement temporaire
            EqEquipementTmp EqTmpToPp01 = new EqEquipementTmp()
            {
                Libelle = "GOP - EqTmp01Test" + MonSuffixe,
                CleTypeEq = 3,
                EstValide = false
            };

            PpToPortion01.EqEquipementTmp.Add(EqTmpToPp01);

            //Création de deux visite et de leur mesures
            Visite VisiteToEqTmp01 = new Visite()
            {
                EnumTypeEval = 1,
                DateImport = new DateTime(2013, 4, 1),
                EstValidee = false,
                RelevePartiel = true,
                Telemesure = false,
                CleUtilisateurMesure = 1745,
                DateVisite = new DateTime(2013, 4, 1),
            };

            Visite VisiteToEqTmp02 = new Visite()
            {
                EnumTypeEval = 1,
                DateImport = new DateTime(2013, 4, 1),
                EstValidee = false,
                RelevePartiel = true,
                Telemesure = false,
                CleUtilisateurMesure = 1745,
                DateVisite = new DateTime(2013, 4, 1),

            };

            MesMesure MesureToVisite01 = new MesMesure()
            {
                CleTypeMesure = 166,
                Valeur = 0
            };

            MesMesure MesureToVisite02 = new MesMesure()
            {
                CleTypeMesure = 83,
                Valeur = 2
            };

            MesMesure MesureToVisite03 = new MesMesure()
            {
                CleTypeMesure = 166,
                Valeur = 0
            };

            MesMesure MesureToVisite04 = new MesMesure()
            {
                CleTypeMesure = 83,
                Valeur = 2
            };

            VisiteToEqTmp01.MesMesure.Add(MesureToVisite01);
            VisiteToEqTmp01.MesMesure.Add(MesureToVisite02);

            VisiteToEqTmp02.MesMesure.Add(MesureToVisite03);
            VisiteToEqTmp02.MesMesure.Add(MesureToVisite04);

            EqTmpToPp01.Visites.Add(VisiteToEqTmp01);
            EqTmpToPp01.Visites.Add(VisiteToEqTmp02);


            // Sauvegarde
            ServiceEnsElectrique.SaveChanges(error =>
            {
                if (error != null)
                {
                    ErrorWindow.CreateNew("Erreur");
                }
                else
                {
                    InfoWindow.CreateNew("Le jeu de test a bien été créé.");
                }

            });
        }

        private void GenererJeuTestOuvrage()
        {
            string MonSuffixe = " " + OuvrageSuffixe01;

            // Création d'un ensemble électrique
            EnsembleElectrique EnsElecToAdd = new EnsembleElectrique()
                {
                    Libelle = "GOP - EnsElecTest" + MonSuffixe,
                    EnumPeriodiciteNullable = 27
                };

            // Ajout de l'ensemble électrique au service
            ServiceEnsElectrique.Add(EnsElecToAdd);

            // Création de 2 portions intégrités
            PortionIntegrite PIToAdd01 = new PortionIntegrite()
                {
                    Code = "TestPI01",
                    DateMiseEnService = DateTime.Now,
                    Libelle = "GOP - Portion01Test" + MonSuffixe,
                    CleDiametre = 21,
                    CleRevetement = 14,
                    DateMajCommentaire = DateTime.Now
                };
            PortionIntegrite PIToAdd02 = new PortionIntegrite()
                {
                    Code = "TestPI02",
                    DateMiseEnService = DateTime.Now,
                    DateMajCommentaire = DateTime.Now,
                    Libelle = "GOP - Portion02Test" + MonSuffixe,
                    CleDiametre = 21,
                    CleRevetement = 14
                };

            EnsElecToAdd.PortionIntegrite.Add(PIToAdd01);
            EnsElecToAdd.PortionIntegrite.Add(PIToAdd02);

            // Ajout d'un secteur sur chaque portion
            PiSecteurs PISToPortion01 = new PiSecteurs()
                {
                    CleSecteur = 55
                };

            PiSecteurs PISToPortion02 = new PiSecteurs()
                {
                    CleSecteur = 56
                };

            PIToAdd01.PiSecteurs.Add(PISToPortion01);
            PIToAdd02.PiSecteurs.Add(PISToPortion02);

            // Création d'une PP par portions
            Pp PpToPortion01 = new Pp()
                {
                    Libelle = "GOP - PP01Test" + MonSuffixe,
                    CleNiveauSensibilite = 6,
                    CleCategoriePp = 21,
                    PkNullable = 1,
                    CleSecteur = 55,
                    CleCommune = 21488,
                    CleUtilisateur = 1745,
                    DateMajPp = DateTime.Now
                };
            Pp PpToPortion02 = new Pp()
                {
                    Libelle = "GOP - PP02Test" + MonSuffixe,
                    CleNiveauSensibilite = 6,
                    CleCategoriePp = 21,
                    PkNullable = 1,
                    CleSecteur = 56,
                    CleCommune = 21488,
                    CleUtilisateur = 1745,
                    DateMajPp = DateTime.Now
                };

            PIToAdd01.Pps.Add(PpToPortion01);
            PIToAdd02.Pps.Add(PpToPortion02);

            // Création d'un équipement de chaque type pour chaque PP
            EqSoutirage MonSOToPp01 = new EqSoutirage()
                {
                    Libelle = "GOP - Sourtirage01Test" + MonSuffixe,
                    CleRedresseur = 265,
                    CleDeversoir = 240,
                    DateMiseEnServiceRedresseurNullable = DateTime.Now,
                    TensionReglageNullable = 5,
                    IntensiteReglageNullable = 10,
                    DateControleNullable = DateTime.Now,
                    LongueurDeversoirNullable = 10,
                    CleTypeEq = 2,
                    DateMajEquipement = DateTime.Now
                };
            EqLiaisonExterne MaLEToPp01 = new EqLiaisonExterne()
                {
                    Libelle = "GOP - LiaisonExterne01Test" + MonSuffixe,
                    CleNomTiersAss = 185,
                    TypeFluide = "GAZ",
                    CleTypeLiaison = 21,
                    CleTypeEq = 5,
                    DateMajEquipement = DateTime.Now
                };
            EqDrainage MonDRToPp01 = new EqDrainage()
                {
                    Libelle = "GOP - Drainage01Test" + MonSuffixe,
                    CleTypeDrainage = 246,
                    CleTypeEq = 3,
                    IntensiteMaximaleSupporteeNullable = 12,
                    DateMajEquipement = DateTime.Now
                };
            EqPostegaz MonPGToPp01 = new EqPostegaz()
            {
                Libelle = "GOP - PosteGaz01Test" + MonSuffixe,
                TypePoste = "DP",
                CleTypeEq = 8,
                DateMajEquipement = DateTime.Now
            };
            EqFourreauMetallique MonFMToPp01 = new EqFourreauMetallique()
            {
                Libelle = "GOP - FourreauMetallique01Test" + MonSuffixe,
                CleTypeEq = 7,
                DateMajEquipement = DateTime.Now,
                LongueurNullable = 1
            };
            EqAnodeGalvanique MonAGToPp01 = new EqAnodeGalvanique()
            {
                Libelle = "GOP - AnodeGalvanique01Test" + MonSuffixe,
                CleTypeAnode = 233,
                CleTypeEq = 9,
                DateMajEquipement = DateTime.Now
            };
            EqLiaisonInterne MaLIToPp01 = new EqLiaisonInterne()
            {
                Libelle = "GOP - LiaisonInterbe01Test" + MonSuffixe,
                CleTypeLiaison = 21,
                CleTypeEq = 4,
                DateMajEquipement = DateTime.Now
            };
            MaLIToPp01.Pp2 = PpToPortion02;
            EqRaccordIsolant MonRIToPp01 = new EqRaccordIsolant()
            {
                Libelle = "GOP - RaccordIsolant01Test" + MonSuffixe,
                CleTypeRi = 260,
                EnumEtatElect = 11,
                EnumConfigElectNormale = 16,
                CleTypeLiaison = 251,
                CleTypeEq = 11,
                DateMajEquipement = DateTime.Now
            };
            EqPile MaPIToPp01 = new EqPile()
            {
                Libelle = "GOP - Pile01Test" + MonSuffixe,
                CleCaracteristiquePile = 22,
                CleTypeDeversoir = 235,
                CleTypeEq = 12,
                DateMajEquipement = DateTime.Now
            };
            EqTiersCroiseSansLiaison MaTCToPp01 = new EqTiersCroiseSansLiaison()
            {
                Libelle = "GOP - TiersCroisé01Test" + MonSuffixe,
                CleTypeEq = 6,
                DateMajEquipement = DateTime.Now
            };
            EqDispoEcoulementCourantsAlternatifs MonDEToPp01 = new EqDispoEcoulementCourantsAlternatifs()
            {
                Libelle = "GOP - DispEcoulement01Test" + MonSuffixe,
                CapaciteCondensateurNullable = 16000,
                CleTypePriseDeTerre = 257,
                CleTypeEq = 10,
                DateMajEquipement = DateTime.Now
            };

            PpToPortion01.EqEquipement.Add(MonSOToPp01);
            PpToPortion01.EqEquipement.Add(MaLEToPp01);
            PpToPortion01.EqEquipement.Add(MonDRToPp01);
            PpToPortion01.EqEquipement.Add(MonPGToPp01);
            PpToPortion01.EqEquipement.Add(MonFMToPp01);
            PpToPortion01.EqEquipement.Add(MonAGToPp01);
            PpToPortion01.EqEquipement.Add(MaLIToPp01);
            PpToPortion01.EqEquipement.Add(MonRIToPp01);
            PpToPortion01.EqEquipement.Add(MaPIToPp01);
            PpToPortion01.EqEquipement.Add(MaTCToPp01);
            PpToPortion01.EqEquipement.Add(MonDEToPp01);

            EqLiaisonExterne MaLE2ToPp01 = new EqLiaisonExterne()
            {
                Libelle = "GOP - LiaisonExterneDelete01Test" + MonSuffixe,
                CleNomTiersAss = 185,
                TypeFluide = "GAZ",
                CleTypeLiaison = 21,
                CleTypeEq = 5,
                DateMajEquipement = DateTime.Now,
                Supprime = true
            };
            EqDrainage MonDR2ToPp01 = new EqDrainage()
            {
                Libelle = "GOP - DrainageDelete01Test" + MonSuffixe,
                CleTypeDrainage = 246,
                CleTypeEq = 3,
                IntensiteMaximaleSupporteeNullable = 12,
                DateMajEquipement = DateTime.Now,
                Supprime = true
            };

            PpToPortion01.EqEquipement.Add(MaLE2ToPp01);
            PpToPortion01.EqEquipement.Add(MonDR2ToPp01);

            // ---------------------------------------------------------------------

            EqSoutirage MonSOToPp02 = new EqSoutirage()
            {
                Libelle = "GOP - Sourtirage02Test" + MonSuffixe,
                CleRedresseur = 265,
                CleDeversoir = 240,
                DateMiseEnServiceRedresseurNullable = DateTime.Now,
                TensionReglageNullable = 5,
                IntensiteReglageNullable = 10,
                DateControleNullable = DateTime.Now,
                LongueurDeversoirNullable = 10,
                CleTypeEq = 2,
                DateMajEquipement = DateTime.Now
            };
            EqLiaisonExterne MaLEToPp02 = new EqLiaisonExterne()
            {
                Libelle = "GOP - LiaisonExterne02Test" + MonSuffixe,
                CleNomTiersAss = 185,
                TypeFluide = "GAZ",
                CleTypeLiaison = 21,
                CleTypeEq = 5,
                DateMajEquipement = DateTime.Now
            };
            EqDrainage MonDRToPp02 = new EqDrainage()
            {
                Libelle = "GOP - Drainage02Test" + MonSuffixe,
                CleTypeDrainage = 246,
                IntensiteMaximaleSupporteeNullable = 12,
                CleTypeEq = 3,
                DateMajEquipement = DateTime.Now
            };
            EqPostegaz MonPGToPp02 = new EqPostegaz()
            {
                Libelle = "GOP - PosteGaz02Test" + MonSuffixe,
                TypePoste = "DP",
                CleTypeEq = 8,
                DateMajEquipement = DateTime.Now
            };
            EqFourreauMetallique MonFMToPp02 = new EqFourreauMetallique()
            {
                Libelle = "GOP - FourreauMetallique02Test" + MonSuffixe,
                CleTypeEq = 7,
                DateMajEquipement = DateTime.Now,
                LongueurNullable = 1
            };
            EqAnodeGalvanique MonAGToPp02 = new EqAnodeGalvanique()
            {
                Libelle = "GOP - AnodeGalvanique02Test" + MonSuffixe,
                CleTypeAnode = 233,
                CleTypeEq = 9,
                DateMajEquipement = DateTime.Now
            };
            EqLiaisonInterne MaLIToPp02 = new EqLiaisonInterne()
            {
                Libelle = "GOP - LiaisonInterbe02Test" + MonSuffixe,
                CleTypeLiaison = 21,
                CleTypeEq = 4,
                DateMajEquipement = DateTime.Now
            };
            MaLIToPp02.Pp2 = PpToPortion02;

            EqRaccordIsolant MonRIToPp02 = new EqRaccordIsolant()
            {
                Libelle = "GOP - RaccordIsolant02Test" + MonSuffixe,
                CleTypeRi = 260,
                EnumEtatElect = 11,
                EnumConfigElectNormale = 16,
                CleTypeLiaison = 251,
                CleTypeEq = 11,
                DateMajEquipement = DateTime.Now
            };
            EqPile MaPIToPp02 = new EqPile()
            {
                Libelle = "GOP - Pile02Test" + MonSuffixe,
                CleCaracteristiquePile = 22,
                CleTypeDeversoir = 235,
                CleTypeEq = 12,
                DateMajEquipement = DateTime.Now
            };
            EqTiersCroiseSansLiaison MaTCToPp02 = new EqTiersCroiseSansLiaison()
            {
                Libelle = "GOP - TiersCroisé02Test" + MonSuffixe,
                CleTypeEq = 6,
                DateMajEquipement = DateTime.Now
            };
            EqDispoEcoulementCourantsAlternatifs MonDEToPp02 = new EqDispoEcoulementCourantsAlternatifs()
            {
                Libelle = "GOP - DispEcoulement02Test" + MonSuffixe,
                CapaciteCondensateurNullable = 16000,
                CleTypePriseDeTerre = 257,
                CleTypeEq = 10,
                DateMajEquipement = DateTime.Now
            };

            PpToPortion02.EqEquipement.Add(MonSOToPp02);
            PpToPortion02.EqEquipement.Add(MaLEToPp02);
            PpToPortion02.EqEquipement.Add(MonDRToPp02);
            PpToPortion02.EqEquipement.Add(MonPGToPp02);
            PpToPortion02.EqEquipement.Add(MonFMToPp02);
            PpToPortion02.EqEquipement.Add(MonAGToPp02);
            PpToPortion02.EqEquipement.Add(MaLIToPp02);
            PpToPortion02.EqEquipement.Add(MonRIToPp02);
            PpToPortion02.EqEquipement.Add(MaPIToPp02);
            PpToPortion02.EqEquipement.Add(MaTCToPp02);
            PpToPortion02.EqEquipement.Add(MonDEToPp02);

            EqLiaisonExterne MaLE2ToPp02 = new EqLiaisonExterne()
            {
                Libelle = "GOP - LiaisonExterneDelete01Test" + MonSuffixe,
                CleNomTiersAss = 185,
                TypeFluide = "GAZ",
                CleTypeLiaison = 21,
                CleTypeEq = 5,
                DateMajEquipement = DateTime.Now,
                Supprime = true
            };
            EqDrainage MonDR2ToPp02 = new EqDrainage()
            {
                Libelle = "GOP - DrainageDelete02Test" + MonSuffixe,
                CleTypeDrainage = 246,
                CleTypeEq = 3,
                IntensiteMaximaleSupporteeNullable = 12,
                DateMajEquipement = DateTime.Now,
                Supprime = true
            };

            PpToPortion01.EqEquipement.Add(MaLE2ToPp02);
            PpToPortion01.EqEquipement.Add(MonDR2ToPp02);

            // Sauvegarde
            ServiceEnsElectrique.SaveChanges(error =>
            {
                if (error != null)
                {
                    ErrorWindow.CreateNew("Erreur");
                }
                else
                {
                    InfoWindow.CreateNew("Le jeu de test a bien été créé.");
                }

            });
        }

        /// <summary>
        ///  Charge la liste de toutes les entitées
        /// </summary>
        /// <returns></returns>
        private void LoadAllServices()
        {
            EntityServiceHelper.LoadAllServicesAsync(
                this,
                (svc, error) =>
                {
                    if (error != null)
                    {
                        Logger.Log(LogSeverity.Error, GetType().FullName, error);
                        if (svc is UserService)
                            ErrorWindow.CreateNew(Resource.Error_UserNotFound);
                    }
                }, () =>
                {
                    AllServicesLoaded = true;
                    if (OnAllServicesLoaded != null)
                        OnAllServicesLoaded(this, null);
                });
            //var properties = this.GetType().GetProperties();
            //lock (lockObject)
            //{
            //    servicesLoadedCount = properties.Count(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(IEntityService<>));
            //}
            //// Pour chaque service de type IEntityService<>
            //foreach (var prop in properties.Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(IEntityService<>)))
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

        #endregion

        #region Override Function

        protected override void DeactivateView(string viewName)
        {
            base.DeactivateView(viewName);

            if (NavigationService.CurrentView != viewName)
            {
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
            }
        }

        /// <summary>
        /// Activation de la vue
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewParameters"></param>
        protected override void ActivateView(string viewName, IDictionary<string, object> viewParameters)
        {
            base.ActivateView(viewName, viewParameters);

            if (IsImportSatified && !AllServicesLoaded)
            {
                LoadAllServices();
            }

            EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("HideExpander", true));
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            IsImportSatified = true;
        }

        #endregion Override function
    }
}
