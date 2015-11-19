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
    [Export(typeof(IEntityService<GeoAgence>))]
    public class GeoAgenceService : IEntityService<GeoAgence>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<GeoAgence> Entities { get; set; }

        public GeoAgence DetailEntity { get; set; }
        #endregion

        #region Constructor

        public GeoAgenceService()
        {
            Entities = new ObservableCollection<GeoAgence>();
        }
        #endregion

        #region Methods
        
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(GeoAgence entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(GeoAgence entity)
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
            this.domainContext.GeoRegions.Clear();
            this.domainContext.GeoAgences.Clear();
            this.domainContext.GeoSecteurs.Clear();
            this.Entities = new ObservableCollection<GeoAgence>();
            this.DetailEntity = null;
        }

        /// <summary>
        /// Enregistrement des agences
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.GeoAgences.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.GeoAgences.EntityContainer.GetChanges())
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
        /// Récupère les entités de la region puis a l'aide du domaincontext
        /// on obtient la liste des Agences
        /// </summary>
        /// <param name="completed"></param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // > 1 car l'utilisateur possède une agence et le service Utilisateur est le premier à se charger.
            if (domainContext.GeoAgences.Count > 1 && domainContext.GeoAgences.First().EntityState != EntityState.Detached)
            {
                Entities = new ObservableCollection<GeoAgence>(domainContext.GeoAgences.ToList().OrderBy(u => u.LibelleAgence));
                if (completed != null)
                {
                    completed(null);
                }
            }
            else
            {
                EntityQuery<GeoRegion> query = domainContext.GetRegionsWithChildEntitiesQuery();
                domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
                {
                    // Declare error and result
                    Exception error = null;
                    IEnumerable<GeoRegion> GeoRegions = null;

                    // Set error or result
                    if (loadOp.HasError)
                    {
                        error = loadOp.Error;
                        Logger.Log(LogSeverity.Error, GetType().FullName, error);
                        loadOp.MarkErrorAsHandled();
                    }
                    else
                    {
                        GeoRegions = loadOp.Entities;
                        Entities = new ObservableCollection<GeoAgence>(domainContext.GeoAgences.ToList().OrderBy(u => u.LibelleAgence));
                    }

                    // Invoke completion callback
                    if (completed != null)
                    {
                        completed(error);
                    }
                }, null);
            }
        }

        /// <summary>
        /// Récupère une seule entitée en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'ntité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            this.DetailEntity = this.Entities.Where(a => a.CleAgence == cle).FirstOrDefault();
            completed(null);
        }

        /// <summary>
        /// Retourne l'agence avec les instruments
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="completed"></param>
        public void GetEntityWithInstrumentByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<GeoAgence> query = domainContext.GetAgenceWithInstrumentsByCleQuery(cle);
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
                    if (loadOp.Entities != null)
                    {
                        this.DetailEntity = loadOp.Entities.FirstOrDefault();
                    }
                    else
                    {
                        this.DetailEntity = null;
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
        public void FindEntities(List<Expression<Func<GeoAgence, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<GeoAgence> query = domainContext.GetGeoAgenceQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<GeoAgence> entities = null;

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
                    Entities = new ObservableCollection<GeoAgence>(entities.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Vérifie les conditions de suppression de l'agence.
        /// Si les conditions sont remplies l'agence est supprimée
        /// </summary>
        /// <param name="cle">identifiant de l'agence a supprimer</param>
        /// <param name="completed">Fonction de retour</param>
        public void CheckAndDeleteAgenceByCle(int cle, Action<Exception, string> completed)
        {
            InvokeOperation invokeOp = domainContext.CheckAndDeleteAgenceByCle(cle, invOp =>
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

        #endregion
    }
}
