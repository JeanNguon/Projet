using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;
using Jounce.Core.Application;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using Proteca.Web.Services;
using System.Collections.ObjectModel;
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
    [Export(typeof(IEntityService<MesClassementMesure>))]
    public class MesClassementMesureService : IEntityService<MesClassementMesure>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<MesClassementMesure> Entities { get; set; }

        public MesClassementMesure DetailEntity { get; set; }
        #endregion

        #region Constructor
        public MesClassementMesureService()
        {
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(MesClassementMesure entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(MesClassementMesure entity)
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
        }

        /// <summary>
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.MesClassementMesures.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.MesClassementMesures.EntityContainer.GetChanges())
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

                    // Invoke completion callback
                    completed(null);

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
        }
        #endregion

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            completed(null);

            //EntityQuery<MesClassementMesure> query = domainContext.GetMesClassementMesureQuery();
            //domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            //{
            //    // Declare error and result
            //    Exception error = null;
            //    IEnumerable<MesClassementMesure> MesClassementMesures = null;

            //    // Set error or result
            //    if (loadOp.HasError)
            //    {
            //        error = loadOp.Error;
            //        Logger.Log(LogSeverity.Error, GetType().FullName, error);
            //        loadOp.MarkErrorAsHandled();
            //    }
            //    else
            //    {
            //        MesClassementMesures = loadOp.Entities;
            //        if (MesClassementMesures.All(u => u.MesTypeMesure != null))
            //        {
            //            Entities = new ObservableCollection<MesClassementMesure>(MesClassementMesures.OrderBy(u => u.MesTypeMesure.NumeroOrdre).ToList());
            //        }
            //        else
            //        {
            //            Entities = new ObservableCollection<MesClassementMesure>(MesClassementMesures.ToList());
            //        }
            //    }

            //    // Invoke completion callback
            //    completed(error);
            //}, null);
        }

        public void GetMesClassementMesureWithMesNiveauProtection(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.MesClassementMesures.Where(m => m.MesTypeMesure.MesureEnService == true);

            // Declare error and result
            Exception error = null;
            IEnumerable<MesClassementMesure> MesClassementMesures = null;


            MesClassementMesures = query;
            if (MesClassementMesures.All(u => u.MesTypeMesure != null))
            {
                Entities = new ObservableCollection<MesClassementMesure>(MesClassementMesures.OrderBy(u => u.MesTypeMesure.NumeroOrdre).ToList());
            }
            else
            {
                Entities = new ObservableCollection<MesClassementMesure>(MesClassementMesures.ToList());
            }


            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // Invoke completion callback
            completed(null);

        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<MesClassementMesure, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.MesClassementMesures.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            // Declare error and result
            Exception error = null;
            IEnumerable<MesClassementMesure> entities = null;


            entities = query;
            Entities = new ObservableCollection<MesClassementMesure>(entities.ToList());


            // Invoke completion callback
            completed(error);
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
