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

        public Visite DetailEntity { get; set; }
        #endregion

        #region Constructor
        public VisiteService()
        {
            this.Entities = new ObservableCollection<Visite>();
            this.OriginEntities = new ObservableCollection<Visite>();
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
            this.Entities = new ObservableCollection<Visite>();
            this.OriginEntities = new ObservableCollection<Visite>();
        }

        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);


            // Suppression des mesures vides
            List<MesMesure> mes = domainContext.MesMesures.Where(m => m.IsNew() && !m.Valeur.HasValue).ToList();
            foreach (MesMesure m in mes)
            {
                domainContext.MesMesures.Remove(m);
            }

            // See if any products have changed
            if ((domainContext.Visites != null && domainContext.Visites.HasChanges) || (domainContext.MesMesures != null && domainContext.MesMesures.HasChanges))
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

            this.Entities = new ObservableCollection<Visite>(OriginEntities);
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

            var query = domainContext.Visites.FirstOrDefault(v => v.CleVisite == cle);

            // Declare error and result
            Exception error = null;


            DetailEntity = query;
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<Visite>(new List<Visite>() { query });
            }


            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<Visite, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.Visites.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }


            // Declare error and result
            Exception error = null;
            IEnumerable<Visite> entities = null;

            entities = query;
            OriginEntities = new ObservableCollection<Visite>(entities.ToList());
            Entities = new ObservableCollection<Visite>(entities.ToList());


            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtreCleRegion">liste des filtres</param>
        /// <param name="dateMax"></param>
        /// <param name="completed">callback fonction</param>
        /// <param name="filtreCleAgence"></param>
        /// <param name="filtreCleSecteur"></param>
        /// <param name="filtreCleEnsElec"></param>
        /// <param name="filtreClePortion"></param>
        /// <param name="dateMin"></param>
        public void FindVisitesNonValideesByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                           int? filtreCleEnsElec, int? filtreClePortion,
                                           DateTime? dateMin, DateTime? dateMax, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            IEnumerable<Visite> entities = null;
            IQueryable<Visite> queryPp = this.domainContext.Visites.Where(v => !v.EstValidee && v.ClePp.HasValue).AsQueryable();

            IQueryable<Visite> queryEq = this.domainContext.Visites.Where(v => !v.EstValidee && v.CleEquipement.HasValue).AsQueryable();

            if (filtreClePortion.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.ClePortion == filtreClePortion.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.PortionIntegrite.ClePortion == filtreClePortion.Value);
            }
            else if (filtreCleEnsElec.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
            }
            else if (filtreCleSecteur.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.CleSecteur == filtreCleSecteur.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.CleSecteur == filtreCleSecteur.Value);
            }
            else if (filtreCleAgence.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
            }
            else if (filtreCleRegion.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
            }

            if (dateMin.HasValue)
            {
                dateMin = dateMin.Value.Date;
                queryPp = queryPp.Where(v => v.DateImport.HasValue && v.DateImport.Value >= dateMin);
                queryEq = queryEq.Where(v => v.DateImport.HasValue && v.DateImport.Value >= dateMin);
            }
            if (dateMax.HasValue)
            {
                dateMax = dateMax.Value.AddDays(1).Date;
                queryPp = queryPp.Where(v => v.DateImport.HasValue && v.DateImport.Value < dateMax);
                queryEq = queryEq.Where(v => v.DateImport.HasValue && v.DateImport.Value < dateMax);
            }

            // Chargement en mémoire de la dernière visite validée de même type pour chaque visite non validée.
            // le chargement en mémoire permet de transmettre au Silverlight les éléments chargés en plus de la requete principale.

            IQueryable<Visite> queryOldVisitePp = from v in queryPp
                                                  select this.domainContext.Visites
                                                                .Where(vp => vp.EstValidee
                                                                            && vp.ClePp.HasValue
                                                                            && vp.ClePp == v.ClePp
                                                                            && (vp.EnumTypeEval == v.EnumTypeEval || (vp.RefEnumValeur.Valeur == "2" && v.RefEnumValeur.Valeur == "1")))
                                                                .OrderByDescending(vp => vp.DateVisite).FirstOrDefault();

            IQueryable<Visite> queryOldVisiteEq = from v in queryEq
                                                  select this.domainContext.Visites
                                                                .Where(vp => vp.EstValidee
                                                                            && vp.CleEquipement.HasValue
                                                                            && vp.CleEquipement == v.CleEquipement
                                                                            && (vp.EnumTypeEval == v.EnumTypeEval || (vp.RefEnumValeur.Valeur == "2" && v.RefEnumValeur.Valeur == "1")))
                                                                .OrderByDescending(vp => vp.DateVisite).FirstOrDefault();

            var oldVisite = queryOldVisitePp.Where(v => v != null).Select(v => v.CleVisite).ToList().Union(queryOldVisiteEq.Where(v => v != null).Select(v => v.CleVisite).ToList());

            IQueryable<Visite> queryOldVisite = this.domainContext.Visites.Where(v => oldVisite.Contains(v.CleVisite)).AsQueryable();

            var oldVisiteLoad = queryOldVisite.ToList();

            entities = (queryPp.OrderBy(v => v.Pp.PortionIntegrite.Libelle).ThenBy(v => v.Pp.Pk).ThenBy(v => v.Pp.Libelle).ThenBy(v => v.DateVisite))
                .Union(queryEq.OrderBy(v => v.EqEquipement.Pp.PortionIntegrite.Libelle).ThenBy(v => v.EqEquipement.Pp.Pk).ThenBy(v => v.EqEquipement.Libelle).ThenBy(v => v.DateVisite));

            // Declare error and result
            Exception error = null;

            entities = entities.OrderBy(v => v.Pk).ThenBy(v => v.LibelleOuvrage);
            OriginEntities = new ObservableCollection<Visite>(entities.ToList());
            Entities = new ObservableCollection<Visite>(entities.ToList());


            // Invoke completion callback
            completed(error);

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
            IEnumerable<Visite> entities = null;
            ////Initialisation de la query sans resultats
            IQueryable<Visite> queryPp = this.domainContext.Visites.Where(v => v.EstValidee && v.ClePp.HasValue).AsQueryable();

            IQueryable<Visite> queryEq = this.domainContext.Visites.Where(v => v.EstValidee && v.CleEquipement.HasValue).AsQueryable();

            if (filtreClePortion.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.ClePortion == filtreClePortion.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.PortionIntegrite.ClePortion == filtreClePortion.Value);
            }
            else if (filtreCleEnsElec.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
            }
            else if (filtreCleSecteur.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.CleSecteur == filtreCleSecteur.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.CleSecteur == filtreCleSecteur.Value);
            }
            else if (filtreCleAgence.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
            }
            else if (filtreCleRegion.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
            }

            if (pkMin.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.Pk >= pkMin.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.Pk >= pkMin.Value);
            }
            if (pkMax.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.Pk <= pkMax.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.Pk <= pkMax.Value);
            }

            if (!String.IsNullOrEmpty(typeEq))
            {
                if (typeEq != "PP")
                {
                    entities = queryEq.Where(v => v.EqEquipement.TypeEquipement.CodeEquipement == typeEq && (includeDeleted || !v.EqEquipement.Supprime)).OrderBy(v => v.EqEquipement.TypeEquipement.NumeroOrdre).ThenBy(v => v.EqEquipement.Libelle).ThenByDescending(v => v.DateVisite);
                }
                else
                {
                    entities = queryPp.Where(v => includeDeleted || !v.Pp.Supprime).OrderBy(v => v.Pp.Libelle).ThenByDescending(v => v.DateVisite);
                }
            }

            else
            {
                entities = (queryPp.Where(v => includeDeleted || !v.Pp.Supprime).OrderBy(v => v.Pp.Libelle).ThenByDescending(v => v.DateVisite))
                    .Union(queryEq.Where(v => includeDeleted || !v.EqEquipement.Supprime).OrderBy(v => v.EqEquipement.TypeEquipement.NumeroOrdre).ThenBy(v => v.EqEquipement.Libelle).ThenByDescending(v => v.DateVisite));
            }
            // Declare error and result
            Exception error = null;

            OriginEntities = new ObservableCollection<Visite>(entities.ToList());
            Entities = new ObservableCollection<Visite>(entities.ToList());

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
