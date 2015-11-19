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
    [Export(typeof(IEntityService<EqEquipement>))]
    public class EqEquipementService : IEntityService<EqEquipement>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<EqEquipement> Entities { get; set; }

        private ObservableCollection<EqEquipement> _originEntities;
        public ObservableCollection<EqEquipement> OriginEntities
        {
            get
            {
                if (this._originEntities == null)
                {
                    this._originEntities = new ObservableCollection<EqEquipement>();
                }
                return _originEntities;
            }
            set
            {
                _originEntities = value;
            }
        }

        public EqEquipement DetailEntity { get; set; }
        #endregion

        #region Constructor
        public EqEquipementService()
        {
        }
        #endregion

        #region Methods
        
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(EqEquipement entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(EqEquipement entity)
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
        public void DeleteSoutirageLiaisonsext(EqSoutirageLiaisonsext entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            domainContext.EqSoutirageLiaisonsexts.Remove(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteDrainageLiaisonsext(EqDrainageLiaisonsext entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            domainContext.EqDrainageLiaisonsexts.Remove(entity);
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            domainContext.EqEquipements.Clear();
            this.Entities = new ObservableCollection<EqEquipement>();
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
        /// 
        /// </summary>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (domainContext.EqEquipements.EntityContainer.GetChanges().Any(o => o is HistoEquipement && ((HistoEquipement)o).LogOuvrage == null))
            {
                // Si pas d'historisation, on supprime le HistoEquipement du domainContext
                domainContext.HistoEquipements.Clear();
            }

            // See if any products have changed
            if (domainContext.EqEquipements.HasChanges 
                || domainContext.EqDrainageLiaisonsexts.HasChanges 
                || domainContext.EqSoutirageLiaisonsexts.HasChanges
                || domainContext.HistoEquipements.HasChanges
                || domainContext.Visites.HasChanges
                || domainContext.EqEquipementTmps.HasChanges
                || domainContext.MesNiveauProtections.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.EqEquipements.EntityContainer.GetChanges())
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
                            OriginEntities = new ObservableCollection<EqEquipement>(Entities);

                            // Nettoyage des Logs pour pallier le problème de LogOuvrages sur EQ supprimés
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
            Entities = new ObservableCollection<EqEquipement>(OriginEntities);
        }
        
        #endregion

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            this.Entities = new ObservableCollection<EqEquipement>();
            OriginEntities = new ObservableCollection<EqEquipement>();
            
            completed(null);
        }

        /// <summary>
        /// Indique si l'équipement peut être supprimé physiquement
        /// </summary>
        /// <param name="cle">identifiant de l'équipement</param>
        /// <param name="completed">fonction de retour</param>
        public void CanPhysicalDeleteByEquipement(int cle, Action<Exception, bool> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            domainContext.CanPhysicalDeleteByEquipement(cle, invOp =>
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

        /// <summary>
        /// Retourne une liste d'équipement
        /// </summary>
        /// <param name="Listcle">liste de clé d'équipement</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListEqEquipementOnly(List<int> Listcle, Action<Exception, IEnumerable<EqEquipement>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<EqEquipement> query = domainContext.FindEqEquipementsByListCleQuery(Listcle);
            domainContext.Load(query, loadOp =>
            {
                // Declare error and result
                Exception error = null;

                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Verbose, GetType().FullName, error);
                }

                completed(error, loadOp.Entities);
            }, null);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCleAndDateAndTypeEval(int cle, DateTime? datedeb, DateTime? datefin, int enumTypeEval, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<EqEquipement> query = domainContext.GetEqEquipementByCleAndDateAndTypeEvalQuery(cle, datedeb.Value, datefin.Value, enumTypeEval);

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
                        Entities = new ObservableCollection<EqEquipement>(loadOp.Entities);
                        OriginEntities = new ObservableCollection<EqEquipement>(Entities);
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
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<EqEquipement> query = domainContext.GetEqEquipementByCleQuery(cle);
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
                        Entities = new ObservableCollection<EqEquipement>(loadOp.Entities);
                        OriginEntities = new ObservableCollection<EqEquipement>(Entities);
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
        public void GetEntityByCle<T>(int cle, Action<Exception> completed) where T : Entity
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var getEntityMethod = domainContext.GetType().GetMethod(string.Format("Get{0}ByCleQuery", typeof(T).Name));
            if (getEntityMethod != null)
            {
                EntityQuery<T> query = (EntityQuery<T>)getEntityMethod.Invoke(domainContext, new object[1] { cle });
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
                        DetailEntity = loadOp.Entities.FirstOrDefault() as EqEquipement;
                       
                    }

                    // Invoke completion callback
                    completed(error);
                }, null);
            }
            else
            {
                Logger.Log(LogSeverity.Error, GetType().FullName, string.Format("La méhode Get{0}ByCleQuery n'existe pas.", typeof(T).Name));
            }
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<EqEquipement, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<EqEquipement> query = domainContext.GetEqEquipementQuery().OrderBy(e => e.Libelle);

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<EqEquipement> entities = null;

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
                    Entities = new ObservableCollection<EqEquipement>(entities.ToList());
                    OriginEntities = new ObservableCollection<EqEquipement>(Entities);
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
        public void FindEquipementByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElectrique, int? clePortion,
            bool includeDeletedEquipment, string codeEquipement, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<EqEquipement> query = domainContext.FindEqEquipementByCriteriasQuery(cleRegion, cleAgence, cleSecteur, cleEnsElectrique, clePortion,
                                                                                            includeDeletedEquipment, codeEquipement);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<EqEquipement> Equipements = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    Equipements = loadOp.Entities;
                    Entities = new ObservableCollection<EqEquipement>(Equipements.ToList());
                    OriginEntities = new ObservableCollection<EqEquipement>(Entities);
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
        public void FindEquipementByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElectrique, int? clePortion,
            bool includeDeletedEquipment, List<String> filtreEq, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<EqEquipement> query = domainContext.FindTourneeEquipementByCriteriasQuery(cleRegion, cleAgence, cleSecteur, cleEnsElectrique, clePortion,
                                                                                            includeDeletedEquipment, filtreEq);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<EqEquipement> Equipements = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    Equipements = loadOp.Entities;
                    Entities = new ObservableCollection<EqEquipement>(Equipements.ToList());
                    OriginEntities = new ObservableCollection<EqEquipement>(Entities);
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// On récupère la liste des tournées de l'équipement ayant comme identifiant la cle passée en paramètre
        /// </summary>
        /// <param name="cle">identifiant de l'équipement</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListTournnees(int cle, Action<Exception, List<Tournee>> completed)
        {
            EntityQuery<Tournee> query = domainContext.GetListTourneesByEquipementQuery(cle);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                Exception error = null;
                List<Tournee> tournees = new List<Tournee>();
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    tournees = loadOp.Entities.ToList();
                }

                completed(error, tournees);
            }, null);
        }

        /// <summary>
        /// Récupère une liste d'entité filtré sur le codeGroupe
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListPointCommun(Action<Exception, List<string>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            domainContext.GetListPointCommun(invOp =>
            {
                // Declare error and result
                Exception error = null;

                if (invOp.HasError)
                {
                    error = invOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                }

                completed(error, invOp.Value.ToList());
            }, null);
        }

        /// <summary>
        /// Récupère la liste de liaison ayant le même libellé de point commun
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListLiaisonPointCommun(string libelle, Action<Exception, ObservableCollection<LiaisonCommunes>> completed)
        {
            domainContext.GetLiaisonsCommunes(libelle, loadOp =>
            {
                Exception error = null;
                List<LiaisonCommunes> liaisonCommune = null;
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    liaisonCommune = loadOp.Value;
                }

                completed(error, new ObservableCollection<LiaisonCommunes>(liaisonCommune.OrderBy(l => l.LibelleLiaison)));
            }, null);
        }

        /// <summary>
        /// Récupère la liste de liaison ayant le même libellé de point commun
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListLiaisonPPCommun(int clePP, Action<Exception, ObservableCollection<LiaisonCommunes>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            domainContext.GetLiaisonsByPP(clePP, loadOp =>
            {
                Exception error = null;
                List<LiaisonCommunes> liaisonCommune = null;
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    liaisonCommune = loadOp.Value;
                }

                completed(error, new ObservableCollection<LiaisonCommunes>(liaisonCommune.OrderBy(l => l.LibelleLiaison)));
            }, null);
        }

        /// <summary>
        /// Récupère la liste de liaison ayant le même libellé de point commun
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListSoutirageExt(Action<Exception, ObservableCollection<EqSoutirage>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<EqSoutirage> query = domainContext.GetListSoutirageExtQuery();
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                Exception error = null;
                ObservableCollection<EqSoutirage> listSoutirage = null;
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    listSoutirage = new ObservableCollection<EqSoutirage>(loadOp.Entities.OrderBy(e => e.Libelle).ToList());
                }

                completed(error, listSoutirage);
            }, null);
        }

        /// <summary>
        /// Récupère la liste de liaison ayant le même libellé de point commun
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListDrainageExt(Action<Exception, ObservableCollection<EqDrainage>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<EqDrainage> query = domainContext.GetListDrainageExtQuery();
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                Exception error = null;
                ObservableCollection<EqDrainage> listEqDrainage = null;
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    listEqDrainage = new ObservableCollection<EqDrainage>(loadOp.Entities.OrderBy(e => e.Libelle).ToList());
                }

                completed(error, listEqDrainage);
            }, null);
        }

        #endregion
    }
}
