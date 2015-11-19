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
using System.Windows;
using System.Linq.Expressions;
using Proteca.Silverlight.Resources;

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
    [Export(typeof(IEntityService<UsrUtilisateur>))]
    public class UsrUtilisateurService : IEntityService<UsrUtilisateur>
    {
        #region Properties

        /// <summary>
        /// Importation du DomainContext
        /// </summary>
        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        /// <summary>
        /// Importation du DomainContext AD
        /// </summary>
        [Import]
        public ProtecaADDomainContext ADDomainContext { get; set; }

        /// <summary>
        /// Importation du Logger
        /// </summary>
        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Déclaration de la variable de liste des entités UsrUtilisateur
        /// </summary>
        public ObservableCollection<UsrUtilisateur> Entities { get; set; }

        /// <summary>
        /// Déclaration de la variable d'une seule entité UsrUtilisateur
        /// </summary>
        public UsrUtilisateur DetailEntity { get; set; }

        /// <summary>
        /// Déclaration de la variable de liste des entités ADUser
        /// </summary>
        public List<ADUser> ADEntities { get; set; }

        /// <summary>
        /// Déclaration de la variable d'une seule entité ADUser
        /// </summary>
        public ADUser DetailADEntity { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur du service UsrUtilisateur
        /// </summary>
        public UsrUtilisateurService()
        {
            Entities = new ObservableCollection<UsrUtilisateur>();
        }

        #endregion

        #region Methods

        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(UsrUtilisateur entity)
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
        public void Delete(UsrUtilisateur entity)
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
            this.domainContext.UsrUtilisateurs.Clear();
            this.Entities = new ObservableCollection<UsrUtilisateur>();
        }

        /// <summary>
        /// Mars an Entity in the collection as Save
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.UsrUtilisateurs != null && domainContext.UsrUtilisateurs.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.UsrUtilisateurs.EntityContainer.GetChanges())
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
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // Ne charge rien par défaut
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

            EntityQuery<UsrUtilisateur> query = domainContext.GetUsrUtilisateurByCleQuery(cle);
            domainContext.Load(query, LoadBehavior.RefreshCurrent, loadOp =>
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
                        Entities = new ObservableCollection<UsrUtilisateur>(loadOp.Entities);
                    }
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="identifiant">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByIdentifiant(string identifiant, Action<Exception, UsrUtilisateur> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            UsrUtilisateur user = domainContext.UsrUtilisateurs.FirstOrDefault( u => u.Identifiant != null && u.Identifiant.ToUpper() == identifiant.ToUpper());
            if (user !=null && user.EntityState != EntityState.Detached)
            {
                completed(null, user);
            }
            else
            {
                EntityQuery<UsrUtilisateur> query = domainContext.GetUsrUtilisateurByIdentifiantQuery(identifiant);
                domainContext.Load(query, LoadBehavior.RefreshCurrent, loadOp =>
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
                        user = loadOp.Entities.FirstOrDefault();
                    }

                    // Invoke completion callback
                    completed(error, user);
                }, null);
            }
        }

        /// <summary>
        /// Retourne la valeur true si une dépendance dans les autres table est trouvé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void CheckDeleteUsrUtilisateurList(int cle, Action<bool, Exception> completed)
        {
            InvokeOperation<bool> invokeOp = domainContext.CheckDeleteUsrUtilisateurList(cle, invOp =>
            {
                // Declare error and result
                Exception error = null;

                if (invOp.HasError)
                {
                    error = invOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    invOp.MarkErrorAsHandled();
                }

                completed(invOp.Value, error);
            }, null);
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<UsrUtilisateur, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<UsrUtilisateur> query = domainContext.GetUsrUtilisateurListQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<UsrUtilisateur> UsrUtilisateurs = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    UsrUtilisateurs = loadOp.Entities;
                    Entities = new ObservableCollection<UsrUtilisateur>(UsrUtilisateurs.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère la liste des entités Active Directory en fonction du filtre défini
        /// </summary>
        public void FindADEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            ADDomainContext.GetUsers(invokeOp =>
            {
                // Declare error and result
                Exception error = null;

                // Set error or result
                if (invokeOp.HasError)
                {
                    error = invokeOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    invokeOp.MarkErrorAsHandled();
                }
                else
                {
                    ADEntities = invokeOp.Value;
                }

                completed(error);
            }, null);

        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void FindADEntityByCle(string cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            ADDomainContext.GetUsersByAccountName(cle, invokeOp =>
            {
                // Declare error and result
                Exception error = null;

                // Set error or result
                if (invokeOp.HasError)
                {
                    error = invokeOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    invokeOp.MarkErrorAsHandled();
                }
                else
                {
                    DetailADEntity = invokeOp.Value;
                }

                completed(error);
            }, null);
        }

        public void FindUsrUtilisateurByCleTournee(int cleTournee, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<UsrUtilisateur> query = domainContext.FindUsrUtilisateurByCleTourneeQuery(cleTournee);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<UsrUtilisateur> usrUtilisateur = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    usrUtilisateur = loadOp.Entities;
                    Entities = new ObservableCollection<UsrUtilisateur>(usrUtilisateur.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        public void FindUsrUtilisateurbyGeoCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<UsrUtilisateur> query = domainContext.FindUsrUtilisateurbyGeoCriteriasQuery(cleRegion, cleAgence, cleSecteur);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<UsrUtilisateur> usrUtilisateur = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    usrUtilisateur = loadOp.Entities;
                    Entities = new ObservableCollection<UsrUtilisateur>(usrUtilisateur.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        public void FindUsrUtilisateurByGeoSecteur(int cleSecteur, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<UsrUtilisateur> query = domainContext.FindUsrUtilisateurByGeoSecteurQuery(cleSecteur);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<UsrUtilisateur> usrUtilisateur = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    usrUtilisateur = loadOp.Entities;
                    Entities = new ObservableCollection<UsrUtilisateur>(usrUtilisateur.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        public void FindUsrUtilisateurbyInternalCriterias(bool externe, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<UsrUtilisateur> query = domainContext.FindUsrUtilisateurbyInternalCriteriasQuery(externe);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<UsrUtilisateur> usrUtilisateur = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    usrUtilisateur = loadOp.Entities;
                    Entities = new ObservableCollection<UsrUtilisateur>(usrUtilisateur.ToList());
                }

                // Invoke completion callback
                completed(error);
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
