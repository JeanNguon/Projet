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
    [Export(typeof(IEntityService<LogOuvrage>))]
    public class LogOuvrageService : IEntityService<LogOuvrage>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<LogOuvrage> Entities { get; set; }

        public LogOuvrage DetailEntity { get; set; }
        #endregion

        #region Constructor
        public LogOuvrageService()
        {
            this.Entities = new ObservableCollection<LogOuvrage>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(LogOuvrage entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(LogOuvrage entity)
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
            this.Entities = new ObservableCollection<LogOuvrage>();
        }

        /// <summary>
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.LogOuvrages != null && domainContext.LogOuvrages.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.LogOuvrages.EntityContainer.GetChanges())
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
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.LogOuvrages.FirstOrDefault(l => l.CleLogOuvrage == cle);

            // Declare error and result
            Exception error = null;


            DetailEntity = query;
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<LogOuvrage>(new List<LogOuvrage>() { DetailEntity });
            }

            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<LogOuvrage, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.LogOuvrages.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            // Declare error and result
            Exception error = null;
            IEnumerable<LogOuvrage> entities = null;

            // Set error or result

            entities = query;
            Entities = new ObservableCollection<LogOuvrage>(entities.ToList());

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère la liste des ouvrage en fonction de la cle de l'ouvrage et de son type
        /// </summary>
        /// <param name="typeOuvrage"></param>
        /// <param name="cleOuvrage"></param>
        /// <param name="completed"></param>
        public void GetLogOuvrageByCleOuvrage(string typeOuvrage, int cleOuvrage, Action<Exception, List<LogOuvrage>> completed)
        {
            IQueryable<LogOuvrage> query = null;
            switch (typeOuvrage)
            {
                case "ClePp":
                    query = this.domainContext.LogOuvrages.Where(l => l.ClePp == cleOuvrage).AsQueryable();
                    break;
                case "CleEquipement":
                    query = this.domainContext.LogOuvrages.Where(l => l.CleEquipement == cleOuvrage).AsQueryable();
                    break;
                case "ClePortion":
                    query = this.domainContext.LogOuvrages.Where(l => l.ClePortion == cleOuvrage).AsQueryable();
                    break;
                case "CleEnsElectrique":
                    query = this.domainContext.LogOuvrages.Where(l => l.CleEnsElectrique == cleOuvrage).AsQueryable();
                    break;
            }


            Exception error = null;
            List<LogOuvrage> logOuvrage = new List<LogOuvrage>();

            logOuvrage = query.ToList();

            completed(error, logOuvrage);
        }

        #endregion
    }
}
