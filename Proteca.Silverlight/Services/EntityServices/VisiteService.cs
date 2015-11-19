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
using Proteca.Silverlight.Helpers;
using Offline;

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
    [Export(typeof(IEntityService<Visite>))]
    public class VisiteService : IEntityService<Visite>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<Visite> Entities { get; set; }

        private ObservableCollection<Visite> _originEntities;
        public ObservableCollection<Visite> OriginEntities
        {
            get
            {
                if (this._originEntities == null)
                {
                    this._originEntities = new ObservableCollection<Visite>();
                }
                return _originEntities;
            }
            set
            {
                _originEntities = value;
            }
        }

        public ObservableCollection<Visite> ImportedEntities { get; set; }
        public ObservableCollection<UsrUtilisateur> ImportedUsrUtilisateurs { get; set; }
        public ObservableCollection<InsInstrument> ImportedInsInstruments { get; set; }
        public Tournee ImportedTournee { get; set; }

        public Visite DetailEntity { get; set; }

        //// Gestion de la création d'un nouveau domainContext à part pour l'import
        //public IConfigurator serviceConfigurator { get; set; }
        public ProtecaDomainContext domainContextImport { get; set; }
        #endregion

        #region Constructor
        [ImportingConstructor]
        public VisiteService([Import(AllowDefault = true)] IConfigurator configurator)
        {
            this.Entities = new ObservableCollection<Visite>();
            this.OriginEntities = new ObservableCollection<Visite>();
            this.ImportedEntities = new ObservableCollection<Visite>();
            this.ImportedUsrUtilisateurs = new ObservableCollection<UsrUtilisateur>();
            this.ImportedInsInstruments = new ObservableCollection<InsInstrument>();
            //serviceConfigurator = configurator;
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Visite entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Visite entity)
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
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            domainContext.Visites.Clear();
            domainContext.MesMesures.Clear();
            domainContext.AnAnalyses.Clear();
            domainContext.Alertes.Clear();

            if (domainContextImport != null)
            {
                domainContextImport.EntityContainer.Clear();
            }
            this.Entities = new ObservableCollection<Visite>();
            this.OriginEntities = new ObservableCollection<Visite>();
            this.ImportedEntities = new ObservableCollection<Visite>();
            this.ImportedUsrUtilisateurs = new ObservableCollection<UsrUtilisateur>();
            this.ImportedInsInstruments = new ObservableCollection<InsInstrument>();
            this.ImportedTournee = null;
        }

        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // Nous gardons finalement les mesures vides pour garder les informations des mesures non effectuées qui auraient du l'être
            //// Suppression des mesures vides
            //List<MesMesure> mes = domainContext.MesMesures.Where(m => m.IsNew() && m.Visite.RelevePartiel && !m.Valeur.HasValue).ToList();
            //foreach (MesMesure m in mes)
            //{
            //    domainContext.MesMesures.Remove(m);
            //}

            // See if any products have changed
            if ((domainContext.Visites != null && domainContext.Visites.HasChanges)
               || (domainContext.UsrUtilisateurs != null && domainContext.UsrUtilisateurs.HasChanges)
               || (domainContext.InstrumentsUtilises != null && domainContext.InstrumentsUtilises.HasChanges)
               || (domainContext.Pps != null && domainContext.Pps.HasChanges)
               || (domainContext.MesMesures != null && domainContext.MesMesures.HasChanges)
               || (domainContext.Alertes != null && domainContext.Alertes.HasChanges)
               || (domainContext.LogOuvrages != null && domainContext.LogOuvrages.HasChanges)
               || (domainContext.AnAnalyses != null && domainContext.AnAnalyses.HasChanges))
            {

                bool isValid = true;
                foreach (var obj in domainContext.Visites.EntityContainer.GetChanges())
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
                        else
                        {
                            this.OriginEntities = new ObservableCollection<Visite>(this.Entities);
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
            if (domainContextImport != null)
            {
                this.domainContextImport.EntityContainer.Clear();
            }
            this.Entities = new ObservableCollection<Visite>(OriginEntities);
            this.ImportedEntities = new ObservableCollection<Visite>();
        }
        #endregion

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>        
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (Entities == null)
            {
                Entities = new ObservableCollection<Visite>();
                OriginEntities = new ObservableCollection<Visite>();
            }
            completed(null);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Visite> query = domainContext.GetVisiteByCleQuery(cle);
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
                        Entities = new ObservableCollection<Visite>(loadOp.Entities);
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
        public void GetVisiteByCleLight(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Visite> query = domainContext.GetVisiteByCleLightQuery(cle);
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
                        Entities = new ObservableCollection<Visite>(loadOp.Entities);
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
        public void FindEntities(List<Expression<Func<Visite, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Visite> query = domainContext.GetVisitesQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Visite> entities = null;

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
                    OriginEntities = new ObservableCollection<Visite>(entities.ToList());
                    Entities = new ObservableCollection<Visite>(entities.ToList());
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
        public void FindVisitesNonValideesByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                           int? filtreCleEnsElec, int? filtreClePortion,
                                           DateTime? dateMin, DateTime? dateMax, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Visite> query = domainContext.FindVisitesNonValideesByCriteriasQuery(filtreCleRegion, filtreCleAgence, filtreCleSecteur,
                                                                    filtreCleEnsElec, filtreClePortion,
                                                                    dateMin, dateMax);

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Visite> entities = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    entities = loadOp.Entities.OrderBy(v => v.LibellePortion).ThenBy(v => v.Pk).ThenBy(v => v.LibelleOuvrage);
                    OriginEntities = new ObservableCollection<Visite>(entities.ToList());
                    Entities = new ObservableCollection<Visite>(entities.ToList());
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
        public void FindVisitesValideesByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                                   int? filtreCleEnsElec, int? filtreClePortion,
                                                   decimal? pkMin, decimal? pkMax, string typeEq, bool includeDeleted, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Visite> query = domainContext.FindVisitesValideesByCriteriasQuery(filtreCleRegion, filtreCleAgence, filtreCleSecteur,
                                                                                          filtreCleEnsElec, filtreClePortion,
                                                                                          pkMin, pkMax, typeEq, includeDeleted);

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Visite> entities = null;

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
                    OriginEntities = new ObservableCollection<Visite>(entities.ToList());
                    Entities = new ObservableCollection<Visite>(entities.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cleEnsElec"></param>
        /// <param name="dateDebut"></param>
        /// <param name="dateFin"></param>
        /// <param name="completed"></param>
        public void FindVisitesByAnalyseEECriterias(int cleEnsElec, DateTime dateDebut, DateTime dateFin, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Visite> query = domainContext.FindVisitesByAnalyseEECriteriasQuery(cleEnsElec, dateDebut, dateFin);

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Visite> entities = null;

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
                    OriginEntities = new ObservableCollection<Visite>(entities.ToList());
                    Entities = new ObservableCollection<Visite>(entities.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        public void LoadDomainContextImport(ProtecaDomainContext domainContextToUseForImport, string jsonContent, Action<Exception> completed, Proteca.Silverlight.ViewModels.ImportVisiteViewModel.ProgressInfo loadProgress)
        {
            loadProgress.StartNewProcess("Analyse du fichier");
            domainContextImport = domainContextToUseForImport;
            domainContextImport.RestoreContext(jsonContent);
            loadProgress.IncrementCurrentValue();


            SynchronizationService.RemoveTmpPPDuplications(
                domainContextImport,
                maxCount => loadProgress.StartNewProcess("Contrôle d'intégrité des données", maxCount),
                () => loadProgress.IncrementCurrentValue());

            loadProgress.StartNewProcess("Import des données");
            this.ImportedEntities = new ObservableCollection<Visite>(domainContextImport.Visites.Where(v => v.IsNew()));
            this.ImportedTournee = domainContextImport.Tournees.FirstOrDefault();
            this.ImportedUsrUtilisateurs = new ObservableCollection<UsrUtilisateur>(domainContextImport.UsrUtilisateurs);
            this.ImportedInsInstruments = new ObservableCollection<InsInstrument>(domainContextImport.InsInstruments);
            loadProgress.IncrementCurrentValue();
            completed(null);
        }

        public void ImportTelemesures(int cleUtilisateur, ObservableCollection<String> lines, Action<Exception, ObservableCollection<VisiteImportRapport>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<VisiteImportRapport> query = domainContext.ImportTelemesuresQuery(cleUtilisateur, lines);
            domainContext.Load(query, loadOp =>
            {
                // Declare error and result
                Exception error = null;

                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Verbose, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }

                completed(error, new ObservableCollection<VisiteImportRapport>(loadOp.Entities));
            }, null);
        }

        #region Load Data Examples
        ///// <summary>
        ///// Complex Load Example - creates a series of queries,
        ///// Loads all the data, and calls the callback Action
        ///// once all loads are complete, this is particularly helpful
        ///// when loading lots of data that will be used in static resultsets.
        ///// </summary>
        ///// <param name="jobID"></param>
        ///// <param name="callback">Action to perform after load</param>
        //public void ComplexLoadExample(Guid jobID, Action callback)
        //{
        //    if (domainContext == null)
        //    {
        //        callback();
        //        return;
        //    }

        //    // Batch these Queries together
        //    contextManager.StartBatch(callback);    // The callback occurs with the batch

        //    // No Callback Needed for LoadData operations, they are completed with Batch
        //    contextManager.LoadData(domainContext.GetJobsQuery(jobID));

        //    // Example of using a Process to Selectively delete items upon load
        //    // This is used to remove items which may have been deleted by another user
        //    // or within another Domain Context.  The DeleteFilter does not have to be
        //    // used.  If not used, everything in cache, not returned from the Query is deleted
        //    contextManager.LoadData(domainContext.GetResourcesQuery(),
        //    ProcessDeletes: true,
        //    DeleteFilter: (delArgs) =>
        //    {
        //        Resource r = delArgs.Item as Resource;
        //        return (r.JobID == jobID);
        //    });

        //    contextManager.EndBatch();              // Signifies the End of the Batching Process
        //}
        #endregion
        #endregion
    }
}
