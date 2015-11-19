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
    [Export(typeof(IEntityService<Pp>))]
    public class PpService : IEntityService<Pp>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<Pp> Entities { get; set; }

        private ObservableCollection<Pp> _originEntities;
        public ObservableCollection<Pp> OriginEntities
        {
            get
            {
                if (this._originEntities == null)
                {
                    this._originEntities = new ObservableCollection<Pp>();
                }
                return _originEntities;
            }
            set
            {
                _originEntities = value;
            }
        }

        private ObservableCollection<EqEquipement> _equipementsSecondaires;
        public ObservableCollection<EqEquipement> EquipementsSecondaires
        {
            get
            {
                if (this._equipementsSecondaires == null)
                {
                    this._equipementsSecondaires = new ObservableCollection<EqEquipement>();
                }
                return _equipementsSecondaires;
            }
            set
            {
                _equipementsSecondaires = value;
            }
        }

        public Pp DetailEntity { get; set; }
        #endregion

        #region Constructor
        
        public PpService()
        {
            Entities = new ObservableCollection<Pp>();
            OriginEntities = new ObservableCollection<Pp>();
            EquipementsSecondaires = new ObservableCollection<EqEquipement>();
        }
        
        #endregion

        #region Methods
       
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Pp entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Pp entity)
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
        public void DeleteImage(Image entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            domainContext.Images.Remove(entity);
        }

        /// <summary>
        /// Suppression d'une PP
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="completed"></param>
        public void GetEquipementsToDeleteByPP(int cle, Action<Exception, int> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            domainContext.GetDeleteCodeByPP(cle, invOp =>
            {
                // Declare error and result
                Exception error = null;

                if (invOp.HasError)
                {
                    error = invOp.Error;
                    Logger.Log(LogSeverity.Verbose, GetType().FullName, error);
                }

                int code = invOp.Value;

                if (code > 4)
                {
                    EntityQuery<EqEquipement> query = domainContext.GetListEquipementByPP2Query(cle);
                    domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
                    {
                        Exception error2 = null;
                        List<EqEquipement> equipements = new List<EqEquipement>();
                        if (loadOp.HasError)
                        {
                            error2 = loadOp.Error;
                            Logger.Log(LogSeverity.Error, GetType().FullName, error2);
                            loadOp.MarkErrorAsHandled();
                        }
                        else
                        {
                            equipements = loadOp.Entities.ToList();
                            EquipementsSecondaires = new ObservableCollection<EqEquipement>(equipements);
                        }

                        completed(error, code);
                    }, null);
                }
                else
                {
                    completed(error, code);
                }
            }, null);
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            domainContext.Pps.Clear();
            this.Entities = new ObservableCollection<Pp>();
            this.OriginEntities.Clear();
            domainContext.EqEquipements.Clear();
            this.EquipementsSecondaires = new ObservableCollection<EqEquipement>();
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

            // See if any products have changed
            if (domainContext.Pps != null && (domainContext.Pps.HasChanges || domainContext.HistoPps.HasChanges || domainContext.PpJumelees.HasChanges))
            {
                bool isValid = true;
                foreach (var obj in domainContext.Pps.EntityContainer.GetChanges())
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
                            OriginEntities = new ObservableCollection<Pp>(Entities);

                            // Nettoyage des Logs pour pallier le problème de LogOuvrages sur PP supprimées
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
            Entities = new ObservableCollection<Pp>(OriginEntities);
        }

        #endregion

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (this.Entities == null || !this.Entities.Any())
            {
                this.Entities = new ObservableCollection<Pp>();
                OriginEntities = new ObservableCollection<Pp>();
            }

            completed(null);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="enumTypeEval"></param>
        /// <param name="datedeb"></param>
        /// <param name="datefin"></param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCleAndDateAndTypeEval(int cle,DateTime? datedeb, DateTime? datefin, int enumTypeEval, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Pp> query = domainContext.GetPpByCleAndDateAndTypeEvalQuery(cle, datedeb.Value.Date, datefin, enumTypeEval);
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
                        Entities = new ObservableCollection<Pp>(loadOp.Entities);
                        OriginEntities = new ObservableCollection<Pp>(Entities);
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

            EntityQuery<Pp> query = domainContext.GetPpByCleQuery(cle);
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
                        Entities = new ObservableCollection<Pp>(loadOp.Entities);
                        OriginEntities = new ObservableCollection<Pp>(Entities);
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
        public void GetDeplacementPpEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Pp> query = domainContext.GetDeplacementPpByCleQuery(cle);
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
                        Entities = new ObservableCollection<Pp>(loadOp.Entities);
                        OriginEntities = new ObservableCollection<Pp>(Entities);
                    }
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<Pp, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<Pp> query = domainContext.GetPpsQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            query.OrderBy(pp => pp.Libelle);

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Pp> entities = null;

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
                    Entities = new ObservableCollection<Pp>(entities.ToList());
                    OriginEntities = new ObservableCollection<Pp>(Entities);
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cleRegion"></param>
        /// <param name="cleAgence"></param>
        /// <param name="cleSecteur"></param>
        /// <param name="cleEnsElectrique"></param>
        /// <param name="clePortion"></param>
        /// <param name="includeDeletedEquipment"></param>
        /// <param name="codeEquipement"></param>
        /// <param name="completed"></param>
        public void FindPpByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElectrique, int? clePortion,
            bool includeDeletedEquipment, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<Pp> query = domainContext.FindPpByCriteriasQuery(cleRegion, cleAgence, cleSecteur, cleEnsElectrique, clePortion,
                                                                                            includeDeletedEquipment);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Pp> Pps = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    Pps = loadOp.Entities;
                    Entities = new ObservableCollection<Pp>(Pps.ToList());
                    OriginEntities = new ObservableCollection<Pp>(Entities);
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Recherche la liste des PP rattachées à la portion ayant comme identifiant la clé passé en paramètre
        /// </summary>
        /// <param name="clePortion">identifiant de la portion </param>
        /// <param name="completed">function de retour</param>
        public void GetPpsByClePortion(int clePortion, Action<Exception> completed)
        {
            EntityQuery<Pp> query = domainContext.GetPpsByClePortionQuery(clePortion);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Pp> pps = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    pps = loadOp.Entities;
                    Entities = new ObservableCollection<Pp>(pps.ToList());
                    OriginEntities = new ObservableCollection<Pp>(Entities);
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Recherche la liste des PP rattachées à la portion ayant comme identifiant la clé passé en paramètre
        /// </summary>
        /// <param name="clePortion">identifiant de la portion </param>
        /// <param name="completed">function de retour</param>
        public void GetPpsAndPpJumeleeByClePortion(int clePortion, Action<Exception> completed)
        {
            EntityQuery<Pp> query = domainContext.GetPpsAndPpJumeleeByClePortionQuery(clePortion);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<Pp> pps = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    pps = loadOp.Entities;
                    Entities = new ObservableCollection<Pp>(pps.ToList());
                    OriginEntities = new ObservableCollection<Pp>(Entities);
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        /// <summary>
        /// Récupère la liste des équipements de la PP
        /// </summary>
        /// <param name="clePP"></param>
        /// <param name="completed"></param>
        public void GetListEquipement(int cle, Action<Exception, List<EqEquipement>> completed)
        {
            EntityQuery<EqEquipement> query = domainContext.GetListEquipementByPPQuery(cle);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                Exception error = null;
                List<EqEquipement> equipements = new List<EqEquipement>();
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    equipements = loadOp.Entities.ToList();
                }

                completed(error, equipements);
            }, null);
        }

        /// <summary>
        /// On récupère la liste des tournées de l'équipement ayant comme identifiant la cle passée en paramètre
        /// </summary>
        /// <param name="cle">identifiant de l'équipement</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListTournnees(int cle, Action<Exception, List<Tournee>> completed)
        {
            EntityQuery<Tournee> query = domainContext.GetListTourneesByPpQuery(cle);
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
        /// Retourne une liste d'équipement
        /// </summary>
        /// <param name="cle">liste de clé d'équipement</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListPPOnly(List<int> Listcle, Action<Exception, IEnumerable<Pp>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<Pp> query = domainContext.FindPpsByListCleQuery(Listcle);
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

        #endregion
    }
}
