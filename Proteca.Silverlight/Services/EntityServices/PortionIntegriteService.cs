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
using System.Resources;
using Proteca.Web.Resources;
using System.Text;


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
    [Export(typeof(IEntityService<PortionIntegrite>))]
    public class PortionIntegriteService : IEntityService<PortionIntegrite>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<PortionIntegrite> Entities { get; set; }

        private ObservableCollection<PortionIntegrite> _originEntities;
        public ObservableCollection<PortionIntegrite> OriginEntities
        {
            get
            {
                if (this._originEntities == null)
                {
                    this._originEntities = new ObservableCollection<PortionIntegrite>();
                }
                return _originEntities;
            }
            set
            {
                _originEntities = value;
            }
        }

        public PortionIntegrite DetailEntity { get; set; }

        public List<String> CustomErrors { get; set; }
        #endregion

        #region Constructor
        public PortionIntegriteService()
        {
            Entities = new ObservableCollection<PortionIntegrite>();
            OriginEntities = new ObservableCollection<PortionIntegrite>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(PortionIntegrite entity)
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
        public void Delete(PortionIntegrite entity)
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
            domainContext.PortionIntegrites.Clear();
            domainContext.PiSecteurs.Clear();
            domainContext.MesNiveauProtections.Clear();
            this.Entities = new ObservableCollection<PortionIntegrite>();
            this.OriginEntities = new ObservableCollection<PortionIntegrite>();
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteAssociateEntity(Entity entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            
            es.Remove(entity);           
        }

        /// <summary>
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            CustomErrors = new List<String>();

            // Instanciation du resource manager
            ResourceManager resourceManager = ResourceHisto.ResourceManager;

            // See if any products have changed
            if (domainContext.PortionIntegrites != null && (domainContext.PortionIntegrites.HasChanges || domainContext.PiSecteurs.HasChanges || domainContext.MesNiveauProtections.HasChanges))
            {
                bool isValid = true;
                foreach (var obj in domainContext.PortionIntegrites.EntityContainer.GetChanges())
                {
                    obj.ValidationErrors.Clear();
                    Collection<ValidationResult> errors = new Collection<ValidationResult>();
                    bool isEntityValid = Validator.TryValidateObject(obj, new ValidationContext(obj, null, null), errors, true);
                    if (!isEntityValid)
                    {
                        // récupération du type d'objet
                        var type = obj.GetType();                        
                        String typeName = type.Name;

                        if (obj is PortionIntegrite)
	                    {
		                    typeName = "Portion";
	                    }
                        else if (obj is Pp)
	                    {
		                    typeName = "PP";
	                    }
                        else if (obj is EqEquipement)
	                    {
		                    typeName = "Equipement";
	                    }

                        // récupération du libellé de l'objet si il existe
                        String libelle = String.Empty;
                        PropertyInfo prop = type.GetProperty("Libelle");                            
                        if (prop != null)
                        {
                            libelle = (String)prop.GetValue(obj, null);
                        }

                        foreach (var err in errors)
                        {
                            obj.ValidationErrors.Add(err);                            

                            // Gestion des erreurs custom
                            string propertyName = err.MemberNames.FirstOrDefault();
                            
                            //récupération de la valeur à afficher. Si pas de valeurs on prend le nom de la propriété                            
                            propertyName = String.IsNullOrEmpty(propertyName) || resourceManager.GetString(propertyName) == null ? propertyName : resourceManager.GetString(propertyName);
                            
                            CustomErrors.Add(typeName + " : " + libelle + " => Propriété en erreur : " + propertyName);
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
                            foreach (Entity entityInError in submitOp.EntitiesInError)
                            {
                                string messages = null;
                                foreach (ValidationResult vres in entityInError.ValidationErrors)
                                {
                                    string fieldsConcerned = null;
                                    foreach (string fld in vres.MemberNames)
                                    {
                                        if (fieldsConcerned == null)
                                            fieldsConcerned += fld;
                                        else
                                            fieldsConcerned += "," + fld;
                                    }
                                    messages += string.Format("Member(s): [{0}] Error Message: [{1}]", fieldsConcerned, vres.ErrorMessage);
                                }
                                Logger.Log(LogSeverity.Error, GetType().FullName, messages);
                            }
                            submitOp.MarkErrorAsHandled();
                        }
                        else
                        {
                            this.OriginEntities = new ObservableCollection<PortionIntegrite>(this.Entities);

                            // Nettoyage des Logs pour pallier le problème de LogOuvrages sur Portions supprimées
                            if (domainContext.LogOuvrages.Any(l => l.IsNew()))
                            {
                                domainContext.LogOuvrages.Clear();
                            }
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

            this.Entities = new ObservableCollection<PortionIntegrite>(OriginEntities);
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
                Entities = new ObservableCollection<PortionIntegrite>();
                OriginEntities = new ObservableCollection<PortionIntegrite>();
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

            EntityQuery<PortionIntegrite> query = domainContext.GetPortionIntegriteByCleQuery(cle);
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
                        Entities = new ObservableCollection<PortionIntegrite>(loadOp.Entities);
                    }
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCleWithEquipement(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<PortionIntegrite> query = domainContext.GetPortionIntegriteByCleWithEqEquipementsQuery(cle);
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
                        Entities = new ObservableCollection<PortionIntegrite>(loadOp.Entities);
                    }
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère la liste des actions de l'ensemble électrique
        /// </summary>
        /// <param name="cle">identifiant de l'ensemble électrique</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListActionsByPortionIntegrite(int clePI, Action<Exception, List<AnAction>> completed)
        {
            EntityQuery<AnAction> query = domainContext.GetActionsByPortionIntegriteQuery(clePI);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                Exception error = null;
                List<AnAction> actions = new List<AnAction>();
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    actions = loadOp.Entities.ToList();
                }

                completed(error, actions);
            }, null);
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<PortionIntegrite, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<PortionIntegrite> query = domainContext.GetPortionIntegriteQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<PortionIntegrite> entities = null;

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
                    Entities = new ObservableCollection<PortionIntegrite>(entities.ToList());
                    OriginEntities = new ObservableCollection<PortionIntegrite>(entities.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        public void FindPortionIntegritesByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,
           int? cleEnsElec, bool isDelete, bool isPosteGaz, bool isStation, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<PortionIntegrite> query = domainContext.FindPortionIntegritesByCriteriasQuery(
                cleRegion, cleAgence, cleSecteur, cleEnsElec, isDelete, isPosteGaz, isStation);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<PortionIntegrite> PortionsIntegrite = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    PortionsIntegrite = loadOp.Entities;
                    Entities = new ObservableCollection<PortionIntegrite>(PortionsIntegrite.ToList());
                    OriginEntities = new ObservableCollection<PortionIntegrite>(PortionsIntegrite.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Vérifie si la portion peut être supprimée Physiquement ou logiquement.
        /// Supprime les PP et equipements associés
        /// </summary>
        public void DeletePortionIntegriteAndCascade(int cle, int cleUtilisateur, Action<Exception, int> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);


            domainContext.DeletePortionIntegriteAndCascade(cle, cleUtilisateur, invOp =>
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
        /// Récupère la liste des portions de l'ensemble électrique avec les dates ECD / EG et CF
        /// </summary>
        /// <param name="cle">identifiant de l'ensemble électrique</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListPortionsByCleEnsElec(int cleEnsElec, int cleRegion, Action<Exception, List<PortionIntegrite>> completed)
        {
            EntityQuery<PortionIntegrite> query = domainContext.GetPortionIntegriteByCleEnsElecQuery(cleEnsElec, cleRegion);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                Exception error = null;
                List<PortionIntegrite> portions = new List<PortionIntegrite>();
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    portions = loadOp.Entities.ToList();
                }

                completed(error, portions);
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
