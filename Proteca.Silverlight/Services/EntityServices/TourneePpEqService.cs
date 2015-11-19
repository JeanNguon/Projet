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
using Proteca.Silverlight.Enums.NavigationEnums;

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
    [Export(typeof(IEntityService<TourneePpEq>))]
    public class TourneePpEqService : IEntityService<TourneePpEq>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<TourneePpEq> Entities { get; set; }
        public ObservableCollection<SelectTourneeTableauBord_Result> TableauBordEntities { get; set; }
        public ObservableCollection<SelectPortionGraphique_Result> GraphiqueEntities { get; set; }

        public TourneePpEq DetailEntity { get; set; }
        #endregion

        #region Constructor
        public TourneePpEqService()
        {
            this.GraphiqueEntities = new ObservableCollection<SelectPortionGraphique_Result>();
            this.TableauBordEntities = new ObservableCollection<SelectTourneeTableauBord_Result>();
            this.Entities = new ObservableCollection<TourneePpEq>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(TourneePpEq entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(TourneePpEq entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (entity != null && entity.Ouvrage != null && entity.Ouvrage.LastVisite != null)
            {
                // Suppression des mesures nouvellement créés sur la visite
                List<MesMesure> nouvellesMesures = entity.Ouvrage.LastVisite.MesMesure.Where(m => m.IsNew()).ToList();
                for (int i = nouvellesMesures.Count - 1; i > -1; i--)
                {
                    domainContext.MesMesures.Remove(nouvellesMesures.ElementAt(i));
                }

                domainContext.Visites.Remove(entity.Ouvrage.LastVisite);
            }
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            domainContext.TourneePpEqs.Clear();
            domainContext.Visites.Clear();
            domainContext.MesMesures.Clear();
            domainContext.AnAnalyses.Clear();
            domainContext.Alertes.Clear();
            this.Entities = new ObservableCollection<TourneePpEq>();
        }

        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // Nous gardons finalement les mesures vides pour garder les informations des mesures non effectuées qui auraient du l'être
            //// Suppression des mesures vides
            //List<MesMesure> mes = domainContext.MesMesures.Where(m => m.IsNew() && m.Visite.RelevePartiel && !m.Valeur.HasValue).ToList();
            //foreach (MesMesure m in mes)
            //{
            //    domainContext.MesMesures.Remove(m);
            //}

            // See if any products have changed
            if ((domainContext.Visites != null              && domainContext.Visites.HasChanges)
               ||(domainContext.UsrUtilisateurs != null     && domainContext.UsrUtilisateurs.HasChanges)
               ||(domainContext.InstrumentsUtilises != null && domainContext.InstrumentsUtilises.HasChanges)
               ||(domainContext.Pps != null                 && domainContext.Pps.HasChanges)
               ||(domainContext.MesMesures != null          && domainContext.MesMesures.HasChanges)
               ||(domainContext.Alertes != null             && domainContext.Alertes.HasChanges)
               ||(domainContext.LogOuvrages != null         && domainContext.LogOuvrages.HasChanges)
               ||(domainContext.AnAnalyses != null          && domainContext.AnAnalyses.HasChanges))
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
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
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
            if (Entities == null)
            {
                Entities = new ObservableCollection<TourneePpEq>();
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
            GetEntityByCle(cle, null, completed);
        }


        public void GetEntityByCle(int cle, string type, Action<Exception> completed)
        {
            // Declare error and result
            Exception error = null;
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            
            string enumType = string.Empty;
            if (CurrentNavigation.Current.Filtre != null)
            {
                enumType = ((FiltreNavigation)CurrentNavigation.Current.Filtre).ToString();
            }
            if (type != null)
            {
                enumType = type;
            }
            if (enumType.Equals(FiltreNavigation.PP.ToString()))
            {
                DetailEntity = this.Entities.Where(t => t.ClePp == cle).FirstOrDefault();
            }
            else
            {
                DetailEntity = this.Entities.Where(c => c.CleEquipement == cle).FirstOrDefault();
            }

            if (DetailEntity == null)
            {
                EntityQuery<TourneePpEq> query = domainContext.GetTourneePpEqByCleEquipmentQuery(cle, enumType);
                domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
                {
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
                            Entities = new ObservableCollection<TourneePpEq>(loadOp.Entities);
                        }
                    }
                    // Invoke completion callback
                    completed(error);
                }, null);
            }
            else
            {
                // Invoke completion callback
                completed(error);
            }
        }

        ///// <summary>
        ///// Récupère une seule entité en fonction de sa clé
        ///// </summary>
        ///// <param name="cle">clé de l'entité</param>
        ///// <param name="completed">callback fonction</param>
        //public void GetEntityByCle(int cle,string type, Action<Exception> completed)
        //{

        //    // Declare error and result
        //    Exception error = null;
        //    Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
        //    CurrentNavigation.Current.Filtre.ToString();
        //    DetailEntity = this.Entities.Where(c => c.ClePp == cle).FirstOrDefault();

        //    if (DetailEntity == null)
        //    {
        //        EntityQuery<TourneePpEq> query = domainContext.GetTourneePpEqByCleQuery(cle);
        //        domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
        //        {

        //            // Set error or result
        //            if (loadOp.HasError)
        //            {
        //                error = loadOp.Error;
        //                Logger.Log(LogSeverity.Error, GetType().FullName, error);
        //                loadOp.MarkErrorAsHandled();
        //            }
        //            else
        //            {
        //                DetailEntity = loadOp.Entities.FirstOrDefault();
        //                if (Entities == null || Entities.Count == 0)
        //                {
        //                    Entities = new ObservableCollection<TourneePpEq>(loadOp.Entities);
        //                }
        //            }
        //        }, null);
        //    }
        //    // Invoke completion callback
        //    completed(error);

        //}

        public void GetEntityByCleTournee(int? cleTournee, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<TourneePpEq> query = domainContext.GetTourneePpEqByCleTourneeQuery(cleTournee);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<TourneePpEq> tournees = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    tournees = loadOp.Entities;
                    Entities = new ObservableCollection<TourneePpEq>(tournees.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        public void GetTourneeTableauBord(int cleTournee, DateTime dateDebut, DateTime dateFin, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<SelectTourneeTableauBord_Result> query = domainContext.GetTourneeTableauBordQuery(cleTournee, dateDebut, dateFin);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<SelectTourneeTableauBord_Result> tourneeTableauDeBord = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    tourneeTableauDeBord = loadOp.Entities;
                    TableauBordEntities = new ObservableCollection<SelectTourneeTableauBord_Result>(tourneeTableauDeBord.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        public void GetPortionGraphique(int clePortion, DateTime dateDebut, DateTime dateFin, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntityQuery<SelectPortionGraphique_Result> query = domainContext.GetPortionGraphiqueQuery(clePortion, dateDebut, dateFin);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<SelectPortionGraphique_Result> portionGraphique = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    portionGraphique = loadOp.Entities;
                    GraphiqueEntities = new ObservableCollection<SelectPortionGraphique_Result>(portionGraphique.ToList());
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
        public void FindEntities(List<Expression<Func<TourneePpEq, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<TourneePpEq> query = domainContext.GetTourneePpEqQuery();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<TourneePpEq> entities = null;

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
                    Entities = new ObservableCollection<TourneePpEq>(entities.ToList());
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
        public void FindTourneePpEqByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
            int? filtreCleEnsElec, int? filtreClePortion, string filtreRechercheLibelle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<TourneePpEq> query = domainContext.FindTourneePpEqByCriteriasQuery(filtreCleRegion, filtreCleAgence, filtreCleSecteur, filtreCleEnsElec, filtreClePortion, filtreRechercheLibelle);

            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<TourneePpEq> entities = null;

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
                    Entities = new ObservableCollection<TourneePpEq>(entities.ToList());
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
