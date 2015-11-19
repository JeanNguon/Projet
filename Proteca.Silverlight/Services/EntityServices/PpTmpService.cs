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
    [Export(typeof(IEntityService<PpTmp>))]
    public class PpTmpService : IEntityService<PpTmp>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<PpTmp> Entities { get; set; }

        private ObservableCollection<PpTmp> _originEntities;
        public ObservableCollection<PpTmp> OriginEntities
        {
            get
            {
                if (this._originEntities == null)
                {
                    this._originEntities = new ObservableCollection<PpTmp>();
                }
                return _originEntities;
            }
            set
            {
                _originEntities = value;
            }
        }

        public PpTmp DetailEntity { get; set; }
        #endregion

        #region Constructor
        public PpTmpService()
        {
            this.Entities = new ObservableCollection<PpTmp>();
            this.OriginEntities = new ObservableCollection<PpTmp>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(PpTmp entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(PpTmp entity)
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
            domainContext.PpTmps.Clear();
            domainContext.Pps.Clear();
            this.Entities = new ObservableCollection<PpTmp>();
            this.OriginEntities = new ObservableCollection<PpTmp>();
        }

        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.PpTmps != null && domainContext.PpTmps.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.PpTmps.EntityContainer.GetChanges())
                {
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

            Entities = new ObservableCollection<PpTmp>(OriginEntities);
        }
        #endregion

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
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

            EntityQuery<PpTmp> query = domainContext.GetPpTmpByCleQuery(cle);
            domainContext.Load(query, loadOp =>
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
                        Entities = new ObservableCollection<PpTmp>(loadOp.Entities);
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
        public void FindEntities(List<Expression<Func<PpTmp, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            completed(null);

            //EntityQuery<PpTmp> query = domainContext.GetPpTmpQuery();

            //foreach (var filtre in filtres)
            //{
            //    query = query.Where(filtre);
            //}

            //domainContext.Load(query, loadOp =>
            //{
            //    // Declare error and result
            //    Exception error = null;
            //    IEnumerable<PpTmp> entities = null;

            //    // Set error or result
            //    if (loadOp.HasError)
            //    {
            //        error = loadOp.Error;
            //        Logger.Log(LogSeverity.Error, GetType().FullName, error);
            //        loadOp.MarkErrorAsHandled();
            //    }
            //    else
            //    {
            //        entities = loadOp.Entities;
            //        Entities = new ObservableCollection<PpTmp>(entities.ToList());
            //    }

            //    // Invoke completion callback
            //    completed(error);
            //}, null);
        }


        public void FindPpTmpByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                           int? filtreCleEnsElec, int? filtreClePortion,
                                           DateTime? dateMin, DateTime? dateMax, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<PpTmp> query = domainContext.FindPpTmpByCriteriasQuery(filtreCleRegion, filtreCleAgence, filtreCleSecteur,
                                                                    filtreCleEnsElec, filtreClePortion,
                                                                    dateMin, dateMax);

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<PpTmp> entities = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    entities = loadOp.Entities.OrderBy(e => e.Pp.PortionIntegrite.Libelle).ThenBy(e => e.Pp.Pk).ThenBy(e => e.Pp.Libelle);
                    Entities = new ObservableCollection<PpTmp>(entities.ToList());
                    OriginEntities = new ObservableCollection<PpTmp>(Entities);
                }

                // Invoke completion callback
                completed(error);
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
