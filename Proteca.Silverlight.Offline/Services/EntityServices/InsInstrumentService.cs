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
    [Export(typeof(IEntityService<InsInstrument>))]
    public class InsInstrumentService : IEntityService<InsInstrument>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<InsInstrument> Entities { get; set; }

        public InsInstrument DetailEntity { get; set; }
        #endregion

        #region Constructor

        public InsInstrumentService()
        {
            Entities = new ObservableCollection<InsInstrument>();
        }

        #endregion

        #region Methods

        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(InsInstrument entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
            if (!this.Entities.Contains(entity))
            {
                this.Entities.Add(entity);
            }
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(InsInstrument entity)
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
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteInstrumentUtilise(InstrumentsUtilises entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            domainContext.InstrumentsUtilises.Remove(entity);
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            this.Entities = new ObservableCollection<InsInstrument>();
        }

        /// <summary>
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.InsInstruments.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.InsInstruments.EntityContainer.GetChanges())
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

            var entitiesToDelete = new ObservableCollection<InsInstrument>(this.Entities.Where(e => e.IsNew()));
            foreach (var item in entitiesToDelete)
            {
                Entities.Remove(item);
            }
        }

        #endregion

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (domainContext.InsInstruments.Count > 0)
            {
                Entities = new ObservableCollection<InsInstrument>(domainContext.InsInstruments.ToList());
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

            var query = domainContext.InsInstruments.FirstOrDefault(i => i.CleInstrument == cle);

            // Declare error and result
            Exception error = null;


            DetailEntity = query;
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<InsInstrument>(new List<InsInstrument>() { query });
            }

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<InsInstrument, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cleRegion"></param>
        /// <param name="cleAgence"></param>
        /// <param name="cleSecteur"></param>
        /// <param name="includeDeleted"></param>
        /// <param name="completed"></param>
        public void FindInstrumentsByCriterias(int cleRegion, int? cleAgence, int? cleSecteur, bool includeDeleted, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            IEnumerable<InsInstrument> InsInstruments = null;
            if (includeDeleted)
            {
                var instruments = domainContext.InsInstruments
                    .Where(i => i.CleRegion == cleRegion);
                if (cleAgence.HasValue)
                {
                    instruments = instruments.Union(domainContext.InsInstruments
                        .Where(i => i.CleAgence == cleAgence.Value));

                    if (cleSecteur.HasValue)
                    {
                        instruments = instruments.Union(domainContext.InsInstruments
                            .Where(i => i.CleSecteur == cleSecteur.Value));
                    }
                }
                InsInstruments = instruments;
            }
            else
            {
                var instruments = domainContext.InsInstruments
                   .Where(i => i.CleRegion == cleRegion && i.Supprime == false);
                if (cleAgence.HasValue)
                {
                    instruments = instruments.Union(domainContext.InsInstruments
                        .Where(i => i.CleAgence == cleAgence.Value && i.Supprime == false));

                    if (cleSecteur.HasValue)
                    {
                        instruments = instruments.Union(domainContext.InsInstruments
                            .Where(i => i.CleSecteur == cleSecteur.Value && i.Supprime == false));
                    }
                }
                InsInstruments = instruments;
            }

            // Declare error and result
            Exception error = null;


            Entities = new ObservableCollection<InsInstrument>(InsInstruments.ToList().OrderBy(i => i.Libelle));

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Retrouve les instrument par clé tournée
        /// </summary>
        /// <param name="cleTournee"></param>
        /// <param name="completed"></param>
        public void FindInsInstrumentByCleTournee(int cleTournee, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IEnumerable<InsInstrument> InsInstruments = null;

            IEnumerable<GeoSecteur> secteurs = this.domainContext.Compositions.Where(co => co.CleTournee == cleTournee && co.ClePp.HasValue && !co.Pp.Supprime).Select(co => co.Pp.GeoSecteur)
                .Union(this.domainContext.Compositions.Where(co => co.CleTournee == cleTournee && co.CleEquipement.HasValue && !co.EqEquipement.Supprime).Select(co => co.EqEquipement.Pp.GeoSecteur)).Distinct();

            IEnumerable<int> cleSecteurs = secteurs.Select(s => s.CleSecteur);
            IEnumerable<int> cleAgences = secteurs.Select(a => a.CleAgence).Distinct();
            IEnumerable<int> cleRegions = this.domainContext.GeoAgences.Where(a => cleAgences.Contains(a.CleAgence)).Select(a => a.CleRegion).Distinct();

            InsInstruments = this.domainContext.InsInstruments.Where(i => !i.Supprime && ((i.CleSecteur.HasValue && cleSecteurs.Contains(i.CleSecteur.Value)) || (i.CleAgence.HasValue && cleAgences.Contains(i.CleAgence.Value)) || (i.CleRegion.HasValue && cleRegions.Contains(i.CleRegion.Value)))).OrderBy(i => i.Libelle);


            // Declare error and result
            Exception error = null;

            Entities = new ObservableCollection<InsInstrument>(InsInstruments.ToList().OrderBy(i => i.Libelle));

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Retrouve les instruments par secteur
        /// </summary>
        /// <param name="cleSecteur"></param>
        /// <param name="completed"></param>
        public void FindInsInstrumentByGeoSecteur(int cleSecteur, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            IEnumerable<InsInstrument> InsInstruments = null;

            GeoAgence agence = this.domainContext.GeoSecteurs.Where(s => s.CleSecteur == cleSecteur).Select(s => s.GeoAgence).FirstOrDefault();

            InsInstruments = this.domainContext.InsInstruments
                .Where(i => !i.Supprime && ((i.CleSecteur.HasValue && i.CleSecteur.Value == cleSecteur) || (i.CleAgence.HasValue && i.CleAgence.Value == agence.CleAgence) || (i.CleRegion.HasValue && i.CleRegion.Value == agence.CleRegion)))
                .OrderBy(i => i.Libelle);

            // Declare error and result
            Exception error = null;

            Entities = new ObservableCollection<InsInstrument>(InsInstruments.ToList().OrderBy(i => i.Libelle));

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
