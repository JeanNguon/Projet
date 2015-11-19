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
using Proteca.Silverlight.Enums;

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
    [Export(typeof(IEntityService<RefEnumValeur>))]
    public class RefEnumValeurService : IEntityService<RefEnumValeur>
    {
        #region Events
        public EventHandler GetEntitiesLoaded { get; set; }
        #endregion

        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<RefEnumValeur> Entities { get; set; }

        public RefEnumValeur DetailEntity { get; set; }

        public bool IsGetEntitiesInProgress
        {
            get;
            set;
        }

        #endregion

        #region Constructor
        public RefEnumValeurService()
        {
            Entities = new ObservableCollection<RefEnumValeur>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(RefEnumValeur entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(RefEnumValeur entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
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
            this.domainContext.RefEnumValeurs.Clear();
            this.Entities = new ObservableCollection<RefEnumValeur>();
            this.DetailEntity = null;
        }

        /// <summary>
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.RefEnumValeurs.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.RefEnumValeurs.EntityContainer.GetChanges())
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
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            this.domainContext.RejectChanges();
        }
        #endregion

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (IsGetEntitiesInProgress)
            {
                EventHandler onGetEntitiesLoaded = null;
                onGetEntitiesLoaded = (o, e) =>
                {
                    GetEntitiesLoaded -= onGetEntitiesLoaded;
                    completed(null);
                    IsGetEntitiesInProgress = false;
                };
                GetEntitiesLoaded += onGetEntitiesLoaded;
            }
            else if (Entities.Count > 0 && !Entities.Any(r => r.EntityState == EntityState.Detached))
            {
                //Entities = new ObservableCollection<RefEnumValeur>(domainContext.RefEnumValeurs.OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle));
                completed(null);
            }
            else
            {
                IsGetEntitiesInProgress = true;
                EntityQuery<RefEnumValeur> query = domainContext.GetRefEnumValeurQuery();
                domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
                {
                    // Declare error and result
                    Exception error = null;
                    IEnumerable<RefEnumValeur> RefEnumValeurs = null;

                    // Set error or result
                    if (loadOp.HasError)
                    {
                        error = loadOp.Error;
                        Logger.Log(LogSeverity.Error, GetType().FullName, error);
                        loadOp.MarkErrorAsHandled();
                    }
                    else
                    {
                        RefEnumValeurs = loadOp.Entities;
                        Entities = new ObservableCollection<RefEnumValeur>(RefEnumValeurs.ToList().OrderBy(u => u.Libelle));
                    }

                    // Invoke completion callback
                    completed(error);

                    IsGetEntitiesInProgress = false;

                    if (GetEntitiesLoaded != null)
                    {
                        GetEntitiesLoaded(this, null);
                    }                    
                }, null);
            }
        }

        /// <summary>
        /// Récupère une liste d'entité filtré sur le codeGroupe
        /// </summary>
        /// <param name="codeGroupe"></param>
        /// <param name="completed"></param>
        public void GetEntitiesByCodeGroup(RefEnumValeurCodeGroupeEnum codeGroupe, Action<Exception, ObservableCollection<RefEnumValeur>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<RefEnumValeur> query = domainContext.GetRefEnumValeursByCodeGroupQuery(codeGroupe.GetStringValue());
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<RefEnumValeur> RefEnumValeurs = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                    completed(error, null);
                }
                else
                {
                    RefEnumValeurs = loadOp.Entities;
                    completed(error, new ObservableCollection<RefEnumValeur>(RefEnumValeurs.OrderBy(u => u.Libelle).ToList()));
                }

                // Invoke completion callback
            }, null);
        }

        /// <summary>
        /// Vérifie si il est possible de supprimer un type d'action
        /// </summary>
        public void CheckCanDeleteTypeAction(int cle, Action<Exception, bool> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            domainContext.CheckCanDeleteTypeAction(cle, invOp =>
            {
                // Declare error and result
                Exception error = null;

                if (invOp.HasError)
                {
                    error = invOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    invOp.MarkErrorAsHandled();
                }

                completed(error, invOp.Value);
            }, null);
        }

        /// <summary>
        /// Vérifie si il est possible de supprimer une catégorie d'anomalie
        /// </summary>
        public void CheckCanDeleteCategorieAnomalie(int cle, Action<Exception, bool> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            domainContext.CheckCanDeleteCategorieAnomalie(cle, invOp =>
            {
                // Declare error and result
                Exception error = null;

                if (invOp.HasError)
                {
                    error = invOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    invOp.MarkErrorAsHandled();
                }

                completed(error, invOp.Value);
            }, null);
        } 



        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<RefEnumValeur> query = domainContext.GetRefEnumValeurByCleQuery(cle);
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
                        Entities = new ObservableCollection<RefEnumValeur>(loadOp.Entities);
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
        public void FindEntities(List<Expression<Func<RefEnumValeur, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<RefEnumValeur> query = domainContext.GetRefEnumValeurQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<RefEnumValeur> entities = null;

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
                    Entities = new ObservableCollection<RefEnumValeur>(entities.ToList().OrderBy(u => u.Libelle));
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
