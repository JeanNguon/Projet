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
    [Export(typeof(IEntityService<Pp>))]
    public class PpService : IEntityService<Pp>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<Pp> Entities { get; set; }

        public Pp DetailEntity { get; set; }
        #endregion

        #region Constructor

        public PpService()
        {
            this.Entities = new ObservableCollection<Pp>();
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

                completed(error, invOp.Value);
            }, null);
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            this.Entities = new ObservableCollection<Pp>();
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

            if (domainContext.Pps.Count > 0)
            {
                Entities = new ObservableCollection<Pp>(domainContext.Pps.ToList());
            }

            completed(null);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="datedeb"></param>
        /// <param name="datefin"></param>
        /// <param name="enumTypeEval"></param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCleAndDateAndTypeEval(int cle, DateTime? datedeb, DateTime? datefin, int enumTypeEval, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            Pp tmp;
            if (datedeb.HasValue && datefin.HasValue)
            {
                tmp = domainContext.Pps.FirstOrDefault(p => p.ClePp == cle);

                var visites = domainContext.Visites.Where(v => v.ClePp == cle && v.EstValidee && v.DateVisite.Value >= datedeb && v.DateVisite <= datefin
                            && (v.EnumTypeEval == enumTypeEval || (domainContext.RefEnumValeurs.FirstOrDefault(r => r.CleEnumValeur == enumTypeEval).Valeur == "1" && v.RefEnumValeur.Valeur == "2")));
                if (visites.Any())
                {
                    tmp.Visites.Add(visites.OrderByDescending(v => v.DateVisite).FirstOrDefault());
                }
            }
            else
            {
                tmp = this.domainContext.Pps.Where(p => p.ClePp == cle).FirstOrDefault();
            }

            // Declare error and result
            Exception error = null;


            DetailEntity = tmp;
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<Pp>(new List<Pp>() { tmp });
            }


            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.Pps.Where(p => p.ClePp == cle);

            // Declare error and result
            Exception error = null;

            DetailEntity = query.First();
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<Pp>(query.ToList());
            }

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetDeplacementPpEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<Pp, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            IQueryable<Pp> query = domainContext.Pps.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            // Declare error and result
            Exception error = null;
            IEnumerable<Pp> entities = null;

            entities = query.OrderBy(pp => pp.Libelle).ToList();
            Entities = new ObservableCollection<Pp>(entities.ToList());

            // Invoke completion callback
            completed(error);
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
            IQueryable<Pp> query = null;

            if (clePortion.HasValue)
            {
                query = domainContext.Pps.Where(p => (p.ClePortion == clePortion.Value)
                     && (includeDeletedEquipment || p.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else if (cleEnsElectrique.HasValue)
            {
                query = domainContext.Pps.Where(p => (p.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value)
                   && (includeDeletedEquipment || p.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else if (cleSecteur.HasValue)
            {
                query = domainContext.Pps.Where(p => (p.PortionIntegrite.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value))
                    && (includeDeletedEquipment || p.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else if (cleAgence.HasValue)
            {
                query = domainContext.Pps.Where(p => (p.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.CleAgence == cleAgence.Value))
                   && (includeDeletedEquipment || p.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else if (cleRegion.HasValue)
            {
                query = domainContext.Pps.Where(p => (p.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value))
                    && (includeDeletedEquipment || p.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else
            {
                query = domainContext.Pps.Where(p => (includeDeletedEquipment || p.Supprime == includeDeletedEquipment)).AsQueryable();
            }

            query = query.OrderBy(p => p.Pk).ThenBy(p => p.Libelle);


            // Declare error and result
            Exception error = null;
            IEnumerable<Pp> Pps = null;


            Pps = query.ToList();
            Entities = new ObservableCollection<Pp>(Pps.ToList());

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Recherche la liste des PP rattachées à la portion ayant comme identifiant la clé passé en paramètre
        /// </summary>
        /// <param name="clePortion">identifiant de la portion </param>
        /// <param name="completed">function de retour</param>
        public void GetPpsByClePortion(int clePortion, Action<Exception> completed)
        {
            IQueryable<Pp> query = domainContext.Pps.Where(pp => pp.ClePortion == clePortion).AsQueryable();

            // Declare error and result
            Exception error = null;

            Entities = new ObservableCollection<Pp>(query.ToList());

            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// Récupère la liste des équipements de la PP
        /// </summary>
        /// <param name="clePP"></param>
        /// <param name="completed"></param>
        public void GetListEquipement(int cle, Action<Exception, List<EqEquipement>> completed)
        {
            var query = domainContext.EqEquipements.Where(e => e.ClePp == cle);

            Exception error = null;
            List<EqEquipement> equipements = new List<EqEquipement>();

            equipements = query.ToList();

            completed(error, equipements);
        }

        /// <summary>
        /// On récupère la liste des tournées de l'équipement ayant comme identifiant la cle passée en paramètre
        /// </summary>
        /// <param name="cle">identifiant de l'équipement</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListTournnees(int cle, Action<Exception, List<Tournee>> completed)
        {
            var query = domainContext.Tournees.Where(t => t.Compositions.Any(c => c.ClePp == cle)).OrderBy(t => t.Libelle);

            Exception error = null;
            List<Tournee> tournees = new List<Tournee>();

            tournees = query.ToList();

            completed(error, tournees);
        }

        #endregion
    }
}
