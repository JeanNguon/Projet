using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;
using Jounce.Core.Application;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using Proteca.Web.Services;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.IO.IsolatedStorage;
using Telerik.Windows.Zip;
using Offline;
using System.Resources;
using Proteca.Web.Resources;
using Proteca.Silverlight.Resources;

namespace Proteca.Silverlight.Services.EntityServices
{
    /// <summary>
    /// A service class should be created for each RIA DomainContext in your project
    /// This class should expose Collections of Data.  Do not add properties, such as SelectedItems
    /// to your service.  Allow your ViewModel to handle these items.
    /// 
    /// Follow the TODO: items in the file to complete the implementation in your project
    /// 
    /// </summary>
    [Export(typeof(IEntityService<Tournee>))]
    public class TourneeService : IEntityService<Tournee>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        //[Import]
        public IConfigurator serviceConfigurator { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<Tournee> Entities { get; set; }

        public Tournee DetailEntity { get; set; }

        public List<String> CustomErrors { get; set; }
        #endregion

        #region Constructor
        [ImportingConstructor]
        public TourneeService([Import(AllowDefault = true)] IConfigurator configurator)
        {
            this.Entities = new ObservableCollection<Tournee>();
            serviceConfigurator = configurator;
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Tournee entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Tournee entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Remove(entity);
            if (this.Entities.Contains(entity))
            {
                this.Entities.Remove(entity);
            }
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteComposition(Composition entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (domainContext.Compositions.Contains(entity))
            {
                domainContext.Compositions.Remove(entity);
            }
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            domainContext.Tournees.Clear();
            domainContext.Compositions.Clear();
            this.Entities = new ObservableCollection<Tournee>();
        }

        /// <summary>
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.Tournees != null && (domainContext.Tournees.HasChanges || domainContext.Compositions.HasChanges))
            {
                bool isValid = true;
                foreach (var obj in domainContext.Tournees.EntityContainer.GetChanges())
                {
                    obj.ValidationErrors.Clear();
                    Collection<ValidationResult> errors = new Collection<ValidationResult>();
                    bool isEntityValid = Validator.TryValidateObject(obj, new ValidationContext(obj, null, null), errors, true);
                    if (!isEntityValid)
                    {
                        foreach (var err in errors)
                        {
                            obj.ValidationErrors.Add(err);
                        }
                    }

                    isValid &= isEntityValid;
                }

                if (isValid)
                {
                    // Submit bulk update
                    domainContext.SubmitChanges(submitOp =>
                    {
                        // Declare error
                        Exception error = null;

                        // Set error or result
                        if (submitOp.HasError)
                        {
                            error = submitOp.Error;
                            Logger.Log(LogSeverity.Error, GetType().FullName, error);
                            submitOp.MarkErrorAsHandled();
                        }

                        // Invoke completion callback
                        completed(error);
                    }, null);
                }
                else
                {
                    completed(new Exception("Des champs sont obligatoire"));
                }
            }
            else
            {
                completed(null);
            }
        }

        /// <summary>
        /// Reverses all pending changes since the data was loaded
        /// </summary>
        public void RejectChanges()
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            this.domainContext.RejectChanges();
        }

        /// <summary>
        /// Validation des éléments de la tournée
        /// MANTIS 11962 FSI 24/06/2014 : affichage des erreurs dans les ouvrages de la tournée avant export
        /// </summary>
        public bool ValidateTournee()
        {
            bool isValid = true;

            CustomErrors = new List<String>();

            if (DetailEntity != null)
            {
                // Instanciation du resource manager
                ResourceManager resourceManager = ResourceHisto.ResourceManager;

                foreach (var compo in DetailEntity.CompositionEqs)
                {
                    var ouvrage = compo.ClePp != null ? (Entity)compo.Pp : (Entity)compo.EqEquipement;

                    if (ouvrage != null)
                    {
                        ouvrage.ValidationErrors.Clear();
                        Collection<ValidationResult> errors = new Collection<ValidationResult>();
                        bool isEntityValid = Validator.TryValidateObject(ouvrage, new ValidationContext(ouvrage, null, null), errors, true);
                        if (!isEntityValid && errors.Any())
                        {
                            StringBuilder sb = new StringBuilder(compo.Ouvrage.LibelleCheminGeo + " \\ " + compo.Ouvrage.Libelle + " \\ ");

                            foreach (var err in errors)
                            {
                                // Gestion des erreurs custom
                                string propertyName = err.MemberNames.FirstOrDefault();

                                //récupération de la valeur à afficher. Si pas de valeurs on prend le nom de la propriété
                                propertyName = String.IsNullOrEmpty(propertyName) || resourceManager.GetString(propertyName) == null ? propertyName : resourceManager.GetString(propertyName);

                                sb.Append(propertyName);
                                if (err != errors.LastOrDefault())
                                {
                                    sb.Append(", ");
                                }
                            }

                            CustomErrors.Add(sb.ToString());
                        }
                        isValid &= isEntityValid;
                    }
                }
            }
            return isValid;
        }
        #endregion

        /// <summary>
        /// Retourne la liste des tournées correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="cleEnsElectrique"></param>
        /// <param name="clePortion"></param>
        /// <param name="libelle"></param>
        /// <param name="isDelete"></param>
        /// <param name="completed"></param>
        /// <returns>Une liste d'ensemble électrique</returns>
        public void FindTourneeByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElectrique, int? clePortion,
            string libelle, bool isDelete, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<Tournee> query = domainContext.FindTourneesByCriteriasQuery(cleRegion, cleAgence, cleSecteur, cleEnsElectrique, clePortion,
                                                                                            libelle, isDelete);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Tournee> tournees = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    tournees = loadOp.Entities;
                    Entities = new ObservableCollection<Tournee>(tournees.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (Entities == null)
            {
                Entities = new ObservableCollection<Tournee>();
            }
            completed(null);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle<T>(int cle, Action<Exception> completed) where T : Entity
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var getEntityMethod = domainContext.GetType().GetMethod(string.Format("Get{0}ByCleQuery", typeof(T).Name));
            if (getEntityMethod != null)
            {
                EntityQuery<T> query = (EntityQuery<T>)getEntityMethod.Invoke(domainContext, new object[1] { cle });
                domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
                {
                    // Declare error and result
                    Exception error = null;

                    // Set error or result
                    if (loadOp.HasError)
                    {
                        error = loadOp.Error;
                        Logger.Log(LogSeverity.Error, GetType().FullName, error);
                        loadOp.MarkErrorAsHandled();
                    }
                    else
                    {
                        DetailEntity = loadOp.Entities.FirstOrDefault() as Tournee;
                    }

                    // Invoke completion callback
                    completed(error);
                }, null);
            }
            else
            {
                Logger.Log(LogSeverity.Error, GetType().FullName, string.Format("La méhode Get{0}ByCleQuery n'existe pas.", typeof(T).Name));
            }
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Tournee> query = domainContext.GetTourneeByCleQuery(cle);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    DetailEntity = loadOp.Entities.FirstOrDefault();
                    if (Entities == null || Entities.Count == 0)
                    {
                        Entities = new ObservableCollection<Tournee>(loadOp.Entities);
                    }
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetTourneeOnlyByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Tournee> query = domainContext.GetTourneeOnlyByCleQuery(cle);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    DetailEntity = loadOp.Entities.FirstOrDefault();
                    if (Entities == null || Entities.Count == 0)
                    {
                        Entities = new ObservableCollection<Tournee>(loadOp.Entities);
                    }
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<Tournee, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Tournee> query = domainContext.GetTourneesQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Tournee> entities = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    entities = loadOp.Entities;
                    Entities = new ObservableCollection<Tournee>(entities.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }
        /// <summary>
        /// Retourne la liste des equipements correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="includeWithoutPortion">inclue ou non les ensemble electrique sans portion</param>
        /// <param name="includeStation">inclue ou non les ensemble électrique de type station</param>
        /// <param name="includePosteGaz">inclue ou non les ensemble électrique de type poste gaz</param>
        /// <param name="libelleEnsElec">filtre sur le libelle de l'ensemble électrique</param>
        /// <returns>Une liste d'ensemble électrique</returns>
        public void FindEquipementByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,  bool eqAG, bool eqDE, bool eqDR, bool eqFM,
            bool eqLE, bool eqLI, bool eqPI, bool eqPO, bool eqPP, bool eqRI, bool eqSO, bool eqTC, Action<Exception, List<Entity>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<EqEquipement> query = domainContext.FindEqEquipementByCriteriasQuery(cleRegion, cleAgence, cleSecteur, null, null, false, "");
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Entity> eqEquipementsPP = null;
                List<Entity> eqEquipements = new List<Entity>();

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    eqEquipementsPP = loadOp.Entities;
                    foreach (Entity eq in eqEquipementsPP)
                    {
                        if(eq is EqAnodeGalvanique && eqAG)
                            eqEquipements.Add(eq);
                        else if (eq is EqDispoEcoulementCourantsAlternatifs && eqDE)
                            eqEquipements.Add(eq);
                        else if (eq is EqDrainage && eqDR)
                            eqEquipements.Add(eq);
                        else if (eq is Pp && eqPP) 
                            eqEquipements.Add(eq);
                        else if (eq is EqFourreauMetallique && eqFM)
                            eqEquipements.Add(eq);
                        else if (eq is EqLiaisonExterne && eqLE)
                            eqEquipements.Add(eq);
                        else if (eq is EqLiaisonInterne && eqLI)
                            eqEquipements.Add(eq);
                        else if (eq is EqPile && eqPI)
                            eqEquipements.Add(eq);
                        else if (eq is EqPostegaz && eqPO)
                            eqEquipements.Add(eq);
                        else if (eq is EqRaccordIsolant && eqRI)
                            eqEquipements.Add(eq);
                        else if (eq is EqSoutirage && eqSO)
                            eqEquipements.Add(eq);
                        else if (eq is EqTiersCroiseSansLiaison && eqTC)
                            eqEquipements.Add(eq);
                    }
                }

                // Invoke completion callback
                completed(error,eqEquipements);
            }, null);
        }

        /// <summary>
        /// Retourne la portion avec ses PPs et ses équipements
        /// </summary>
        /// <param name="clePortion"></param>
        /// <param name="completed"></param>
        public void GetTourneePortionIntegriteByCle(int clePortion, Action<Exception, PortionIntegrite> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<PortionIntegrite> query = domainContext.GetTourneePortionIntegriteByCleQuery(clePortion);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                PortionIntegrite portion = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    portion = loadOp.Entities.FirstOrDefault();                    
                }

                // Invoke completion callback
                completed(error, portion);
            }, null);
        }

        /// <summary>
        /// Retourne l'Ensemble Electrique avec ses Portions, ses PPs et ses équipements
        /// </summary>
        /// <param name="cleEE"></param>
        /// <param name="completed"></param>
        public void GetTourneeEnsElecByCle(int cleEE, Action<Exception, EnsembleElectrique> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<EnsembleElectrique> query = domainContext.GetTourneeEnsElecByCleQuery(cleEE);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                EnsembleElectrique EE = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    EE = loadOp.Entities.FirstOrDefault();                    
                }

                // Invoke completion callback
                completed(error, EE);
            }, null);
        }
        
        #endregion

        #region Export

        /// <summary>
        /// Export la tournée en XML
        /// </summary>
        public void ExportTourneeToXml(int cle, Action<Exception, List<String>> completed)
        {

            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            domainContext.GetTourneeExportXML(this.DetailEntity.CleTournee, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                List<String> retour = new List<string>();

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    // retourne le fichier de tournée
                    retour = loadOp.Value.ToList();
                }

                // Invoke completion callback
                completed(error, retour);
            }, null);
        }

        /// <summary>
        /// Export la tournée en json
        /// </summary>
        public void ExportTourneeToJson(int cle, Action<Exception, String> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            //On créé un nouveau domainContext spécifique à 
            ProtecaDomainContext domainContextExport = new ProtecaDomainContext(serviceConfigurator);

            //Etapes restantes à faire
            int stepsLeftUntilTheEnd = 12;

            domainContextExport.Load(domainContextExport.GetTourneeCompleteByCleQuery(cle),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.FindInsInstrumentByCleTourneeQuery(cle),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.FindUsrUtilisateurByCleTourneeQuery(cle),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetMesClassementMesureWithMesNiveauProtectionQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetRefEnumValeurQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetRegionsWithChildEntitiesQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetRefParametreQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetRefUsrPorteeQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetTypeEquipementQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetUsrProfilQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetCategoriePpQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);

            domainContextExport.Load(domainContextExport.GetRefNiveauSensibilitePpQuery(),
                LoadBehavior.MergeIntoCurrent, loadOp => ExportJsonCallBack(loadOp, ref stepsLeftUntilTheEnd, domainContextExport, completed), null);
        }

        /// <summary>
        /// CallBack des appels de chargement de l'export Json
        /// </summary>
        /// <param name="loadOp"></param>
        /// <param name="stepsLeftUntilTheEnd"></param>
        /// <param name="domainContextExport"></param>
        /// <param name="completed"></param>
        private void ExportJsonCallBack(LoadOperation loadOp, ref int stepsLeftUntilTheEnd, ProtecaDomainContext domainContextExport, Action<Exception, String> completed)
        {
            // Declare error and result
            Exception error = null;

            // Set error or result
            if (loadOp.HasError)
            {
                error = loadOp.Error;
                Logger.Log(LogSeverity.Error, GetType().FullName, error);
                loadOp.MarkErrorAsHandled();
                stepsLeftUntilTheEnd = -1;
                completed(error, String.Empty);
            }
            else
            {
                stepsLeftUntilTheEnd--;
                if (stepsLeftUntilTheEnd == 0)
                {
                    //Création de l'utilisateur de ProtOn
                    domainContextExport.UsrUtilisateurs.Add(new UsrUtilisateur()
                    {
                        Identifiant = "ProtOn",
                        UsrProfil = domainContextExport.UsrProfils.FirstOrDefault(u => u.IsAdministrateur),
                        GeoAgence = domainContextExport.GeoAgences.FirstOrDefault(),
                        Nom = "Utilisateur",
                        Prenom = "ProtOn",
                        Mail = "Proteca@ProtOn.com"
                    });

                    completed(error, domainContextExport.SerializeContext());
                }
            }
        }



        #endregion
    }
}
