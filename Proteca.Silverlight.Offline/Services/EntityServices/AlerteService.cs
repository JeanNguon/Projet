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
    [Export(typeof(IEntityService<Alerte>))]
    public class AlerteService : IEntityService<Alerte>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<Alerte> Entities { get; set; }

        public ObservableCollection<AlerteDetail> DetailEntities { get; set; }

        public Alerte DetailEntity { get; set; }
        #endregion

        #region Constructor
        public AlerteService()
        {
            this.Entities = new ObservableCollection<Alerte>();
            this.DetailEntities = new ObservableCollection<AlerteDetail>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Alerte entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Alerte entity)
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
            this.Entities = new ObservableCollection<Alerte>();
            this.DetailEntities = new ObservableCollection<AlerteDetail>();
        }

        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.Alertes != null && domainContext.Alertes.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.Alertes.EntityContainer.GetChanges())
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
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
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

            var query = domainContext.Alertes.FirstOrDefault(a => a.CleAlerte == cle);

            // Declare error and result
            Exception error = null;

            DetailEntity = query;
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<Alerte>(new List<Alerte>() { query });
            }


            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<Alerte, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.Alertes.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            // Declare error and result
            Exception error = null;
            IEnumerable<Alerte> entities = null;


            entities = query.ToList();
            Entities = new ObservableCollection<Alerte>(entities.ToList());

            // Invoke completion callback
            completed(error);
        }


        public void FindAlertesByListCleAlerte(List<int> listCleAlerte, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            List<Alerte> alertes = new List<Alerte>();
            int step = 2000;

            if (listCleAlerte.Count > step)
            {

                int index = 0;
                while (index < listCleAlerte.Count)
                {
                    IEnumerable<int> list = listCleAlerte.Skip(index).Take(step);
                    if (index == 0)
                    {
                        alertes = this.domainContext.Alertes.Where(a => list.Contains(a.CleAlerte)).ToList();
                    }
                    else
                    {
                        alertes.AddRange(this.domainContext.Alertes.Where(a => list.Contains(a.CleAlerte)).ToList());
                    }
                    index += step;
                }
            }

            var query = alertes.AsQueryable();

            // Declare error and result
            Exception error = null;
            IEnumerable<Alerte> Alertes = null;


            Alertes = query;
            Entities = new ObservableCollection<Alerte>(Alertes.ToList());

            // Invoke completion callback
            completed(error);

        }

        public void FindAlerteDetailsByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElec, int? clePortion,
            decimal? pkMin, decimal? pkMax, DateTime dateMin, DateTime dateMax, bool includeDisabled, ObservableCollection<RefEnumValeur> listTypeAlerte,
            Action<Exception> completed)
        {
            ObservableCollection<String> serializedListTypeAlerte = new ObservableCollection<string>();
            bool allFalse = !listTypeAlerte.Any(t => t.IsSelected);

            foreach (RefEnumValeur type in listTypeAlerte)
            {
                if (allFalse || type.IsSelected)
                {
                    serializedListTypeAlerte.Add(type.Valeur);
                }
            }

            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            //Mise en place des triggers de selection pour la date
            dateMin = dateMin.Date;
            dateMax = dateMax.Date.AddDays(1);

            IQueryable<AlerteDetail> queryDetail = this.domainContext.AlerteDetails.Where(a => serializedListTypeAlerte.Contains(a.Type)).AsQueryable();

            if (!includeDisabled)
            {
                queryDetail = queryDetail.Where(a => a.Supprime == false);
            }

            queryDetail = queryDetail.Where(a => a.Date >= dateMin);
            queryDetail = queryDetail.Where(a => a.Date < dateMax);

            if (pkMin.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.Pk >= pkMin.Value);
            }
            if (pkMax.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.Pk <= pkMax.Value);
            }

            if (clePortion.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.ClePortion == clePortion.Value);
            }
            else if (cleEnsElec.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.CleEnsElectrique == cleEnsElec.Value);
            }
            else if (cleSecteur.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.CleSecteur == cleSecteur.Value);
            }
            else if (cleAgence.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.CleAgence == cleAgence.Value);
            }
            else if (cleRegion.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.CleRegion == cleRegion.Value);
            }

            queryDetail = queryDetail.OrderBy(ad => ad.LibellePortion).ThenBy(ad => ad.Pk).ThenBy(ad => ad.LibelleType).ThenBy(ad => ad.Date);

            IEnumerable<AlerteDetail> Alertes = null;

            Alertes = queryDetail;
            DetailEntities = new ObservableCollection<AlerteDetail>(Alertes.ToList());

            FindAlertesByListCleAlerte(Alertes.Select(a => a.CleAlerte).ToList(), completed);

        }

        public void FindAlerteByClePP(int clePP, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            var query = domainContext.Alertes.Where(a => a.Visite.ClePp == clePP || a.Visite.EqEquipement.ClePp == clePP);

            // Declare error and result
            Exception error = null;
            IEnumerable<Alerte> Alertes = null;


            Alertes = query;
            Entities = new ObservableCollection<Alerte>(Alertes.ToList());

            FindAlerteDetailByClePP(clePP, completed);


            // Invoke completion callback
            completed(error);

        }

        public void FindAlerteDetailByClePP(int clePP, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IQueryable<int> ClesAlerte = this.domainContext.Alertes.Where(a => a.Visite.ClePp == clePP || a.Visite.EqEquipement.ClePp == clePP).Select(a => a.CleAlerte).AsQueryable();
            IQueryable<AlerteDetail> query = this.domainContext.AlerteDetails.Where(a => ClesAlerte.Contains(a.CleAlerte)).Distinct().AsQueryable();

            // Declare error and result
            Exception error = null;
            IEnumerable<AlerteDetail> Alertes = null;

            Alertes = query;
            DetailEntities = new ObservableCollection<AlerteDetail>(Alertes.ToList());

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
