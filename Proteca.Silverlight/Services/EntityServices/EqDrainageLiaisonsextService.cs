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
using Proteca.Silverlight.Models;
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
    [Export(typeof(IEntityService<EqDrainageLiaisonsext>))]
    public class EqDrainageLiaisonsextService : IEntityService<EqDrainageLiaisonsext>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<EqDrainageLiaisonsext> Entities { get; set; }

        public EqDrainageLiaisonsext DetailEntity { get; set; }
        #endregion

        #region Constructor

        public EqDrainageLiaisonsextService()
        {
        }
        
        #endregion

        #region Methods

        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(EqDrainageLiaisonsext entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(EqDrainageLiaisonsext entity)
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
            if (domainContext.MesUnites.HasChanges)
            {
                try
                {
                    bool isValid = true;
                    foreach (var obj in domainContext.MesUnites.EntityContainer.GetChanges())
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
                        throw new ValidationException("L'entité n'est pas valide");
                    }
                }
                catch (ValidationException ex)
                {
                    Logger.Log(LogSeverity.Error, GetType().FullName, ex.ToString());
                    throw ex;
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
            if (this.Entities == null)
            {
                this.Entities = new ObservableCollection<EqDrainageLiaisonsext>();
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

            EntityQuery<EqDrainageLiaisonsext> query = domainContext.GetEqDrainageLiaisonsextByCleQuery(cle);
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
                        Entities = new ObservableCollection<EqDrainageLiaisonsext>(loadOp.Entities);
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
        public void FindEntities(List<Expression<Func<EqDrainageLiaisonsext, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<EqDrainageLiaisonsext> query = domainContext.GetEqDrainageLiaisonsextQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<EqDrainageLiaisonsext> entities = null;

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
                    Entities = new ObservableCollection<EqDrainageLiaisonsext>(entities.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        #endregion
    }
}
