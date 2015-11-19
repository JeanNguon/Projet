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
    [Export(typeof(IEntityService<GeoRegion>))]
    public class GeoRegionService : IEntityService<GeoRegion>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<GeoRegion> Entities { get; set; }

        public ObservableCollection<GeoRegion> EntitiesGrouping { get; set; }

        public GeoRegion DetailEntity { get; set; }

        #endregion

        #region Constructor

        public GeoRegionService()
        {
            Entities = new ObservableCollection<GeoRegion>();
            EntitiesGrouping = new ObservableCollection<GeoRegion>();
        }

        #endregion

        #region Methods

        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(GeoRegion entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(GeoRegion entity)
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
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.GeoRegions.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.GeoRegions.EntityContainer.GetChanges())
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
        /// Récupère les entités de la region.
        /// </summary>
        /// <param name="completed"></param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (domainContext.GeoRegions.Count > 0)
            {
                Entities = new ObservableCollection<GeoRegion>(domainContext.GeoRegions.ToList().OrderBy(u => u.LibelleRegion));
            }
            completed(null);
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
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
        /// Vérifie les conditions de suppression de la region.
        /// Si les conditions sont remplies la region est supprimée
        /// </summary>
        /// <param name="cle">identifiant de la region a supprimer</param>
        /// <param name="completed">Fonction de retour</param>
        public void CheckAndDeleteRegionByCle(int cle, Action<Exception, string> completed)
        {
            InvokeOperation invokeOp = domainContext.CheckAndDeleteRegionByCle(cle, invOp =>
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
        /// Regroupe 2 régions et recharge les régions
        /// </summary>
        /// <param name="cleOfRegrouping">identifiant de la region de regroupement</param>
        /// <param name="cleToRegrouping">identifiant de la region à regrouper</param>
        /// <param name="libelle"></param>
        /// <param name="libelleAbrege"></param>
        /// <param name="completed">Fonction de retour</param>
        public void CheckAndRegoupeRegionByCle(int cleOfRegrouping, int cleToRegrouping, string libelle, string libelleAbrege, Action<Exception> completed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retourne la région avec les instruments
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="completed"></param>
        public void GetEntityWithInstrumentByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            this.GetEntityByCle(cle, completed);
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<GeoRegion, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.GeoRegions.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre).AsQueryable();
            }


            // Declare error and result
            Exception error = null;
            IEnumerable<GeoRegion> entities = null;

            entities = query;
            Entities = new ObservableCollection<GeoRegion>(entities.ToList());

            // Invoke completion callback
            completed(error);
        }

        #endregion
    }
}
