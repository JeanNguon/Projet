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
        /// Récupère les entités de la region puis a l'aide du domaincontext
        /// on obtient la liste des Agences
        /// </summary>
        /// <param name="completed"></param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (domainContext.GeoAgences.Count > 0 && domainContext.GeoAgences.First().EntityState != EntityState.Detached)
            {
                Entities = new ObservableCollection<GeoAgence>(domainContext.GeoAgences.ToList().OrderBy(u => u.LibelleAgence));
                completed(null);
            }
            else
            {

                // Invoke completion callback
                completed(null);

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

            var query = domainContext.GeoAgences.Where(a => a.CleAgence == cle);

            // Declare error and result
            Exception error = null;


            if (query != null)
            {
                this.DetailEntity = query.First();
            }
            else
            {
                this.DetailEntity = null;
            }

            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<GeoAgence, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.GeoAgences.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            // Declare error and result
            Exception error = null;
            IEnumerable<GeoAgence> entities = null;


            entities = query;
            Entities = new ObservableCollection<GeoAgence>(entities.ToList());

            // Invoke completion callback
            completed(error);

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
