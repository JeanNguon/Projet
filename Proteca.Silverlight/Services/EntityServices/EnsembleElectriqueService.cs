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
    [Export(typeof(IEntityService<EnsembleElectrique>))]
    public class EnsembleElectriqueService : IEntityService<EnsembleElectrique>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<EnsembleElectrique> Entities { get; set; }

        public EnsembleElectrique DetailEntity { get; set; }
        #endregion

        #region Constructor
        public EnsembleElectriqueService()
        {
            Entities = new ObservableCollection<EnsembleElectrique>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(EnsembleElectrique entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(EnsembleElectrique entity)
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
            domainContext.EnsembleElectriques.Clear();
            this.Entities = new ObservableCollection<EnsembleElectrique>();
        }

        /// <summary>
        /// Enregistre les changements de l'ensemble électrique en cours
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.EnsembleElectriques != null && domainContext.EnsembleElectriques.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.EnsembleElectriques.EntityContainer.GetChanges())
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
                        else
                        {
                            Entities = new ObservableCollection<EnsembleElectrique>(Entities.OrderBy(ee => ee.Libelle));

                            // Nettoyage des Logs pour pallier le problème de LogOuvrages sur EE supprimés
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

            EntityQuery<EnsembleElectrique> query = domainContext.GetEnsembleElectriqueByCleQuery(cle);
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
                        Entities = new ObservableCollection<EnsembleElectrique>(loadOp.Entities);
                    }
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère la liste des actions de l'ensemble électrique
        /// </summary>
        /// <param name="cleEE">identifiant de l'ensemble électrique</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListActionsByEnsembleElectriqueQuery(int cleEE, Action<Exception, List<AnAction>> completed)
        {
            EntityQuery<AnAction> query = domainContext.GetActionsByEnsembleElectriqueQuery(cleEE);
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
        /// Récupère la liste des portions de l'ensemble électrique avec les dates ECD / EG et CF
        /// </summary>
        /// <param name="cle">identifiant de l'ensemble électrique</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListPortions(int cle, Action<Exception, List<PortionDates>> completed)
        {
            EntityQuery<PortionDates> query = domainContext.GetListPortionsQuery(cle);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                Exception error = null;
                List<PortionDates> portions = new List<PortionDates>();
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

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<EnsembleElectrique, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<EnsembleElectrique> query = domainContext.GetEnsembleElectriqueQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<EnsembleElectrique> entities = null;

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
                    Entities = new ObservableCollection<EnsembleElectrique>(entities.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Retourne la liste des ensembles d'instrument correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="includeWithoutPortion">inclue ou non les ensemble electrique sans portion</param>
        /// <param name="includeStation">inclue ou non les ensemble électrique de type station</param>
        /// <param name="includePosteGaz">inclue ou non les ensemble électrique de type poste gaz</param>
        /// <param name="libelleEnsElec">filtre sur le libelle de l'ensemble électrique</param>
        /// <returns>Une liste d'ensemble électrique</returns>
        public void FindEnsembleElectriqueByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,
            bool includeWithoutPortion, bool includeStation, bool includePosteGaz, string libelleEnsElec, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<EnsembleElectrique> query = domainContext.FindEnsembleElectriqueByCriteriasQuery(cleRegion, cleAgence, cleSecteur,
                                                                                                    includeWithoutPortion, includeStation, 
                                                                                                    includePosteGaz, libelleEnsElec);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<EnsembleElectrique> EnsembleElectriques = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    EnsembleElectriques = loadOp.Entities;
                    Entities = new ObservableCollection<EnsembleElectrique>(EnsembleElectriques.ToList().Where(r => r.Supprime == false));
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère le code de suppression d'un ensemble électrique
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="completed"></param>
        public void GetDeleteCodeByEnsembleElectrique(int cle, Action<Exception, int> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            domainContext.GetDeleteCodeByEnsembleElectrique(cle, invOp =>
            {
                // Declare error and result
                Exception error = null;

                if (invOp.HasError)
                {
                    error = invOp.Error;
                    Logger.Log(LogSeverity.Verbose, GetType().FullName, error);
                }

                completed(error, invOp.Value);
            }, null);
        }

        #endregion
    }
}
