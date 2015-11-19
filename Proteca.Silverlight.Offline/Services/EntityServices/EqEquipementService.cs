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
    [Export(typeof(IEntityService<EqEquipement>))]
    public class EqEquipementService : IEntityService<EqEquipement>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<EqEquipement> Entities { get; set; }

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
            this.Entities = new ObservableCollection<EqEquipement>();
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
                || domainContext.HistoEquipements.HasChanges)
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

            this.Entities = new ObservableCollection<EqEquipement>();

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
            completed(null, !this.domainContext.Visites.Any(v => v.CleEquipement == cle));
        }

        /// <summary>
        /// Indique si l'équipement peut être supprimé physiquement
        /// </summary>
        /// <param name="listcle"></param>
        /// <param name="completed">fonction de retour</param>
        public void GetListEqEquipementOnly(List<int> listcle, Action<Exception, IEnumerable<EqEquipement>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IQueryable<EqEquipement> query = null;
            int step = 100;
            ObservableCollection<int> listCleEquipement = new ObservableCollection<int>(listcle);
            if (listCleEquipement.Count > step)
            {
                List<EqEquipement> equipements = new List<EqEquipement>();
                int index = 0;
                while (index < listCleEquipement.Count)
                {
                    IEnumerable<int> list = listCleEquipement.Skip(index).Take(step);
                    if (index == 0)
                    {
                        equipements = domainContext.EqEquipements
                            .Where(a => list.Contains(a.CleEquipement)).ToList();
                    }
                    else
                    {
                        equipements.AddRange(domainContext.EqEquipements
                            .Where(a => list.Contains(a.CleEquipement)).ToList());
                    }
                    index += step;
                }

                query = equipements.AsQueryable();
            }
            else
            {
                query = domainContext.EqEquipements.Where(a => listCleEquipement.Contains(a.CleEquipement)).AsQueryable();
            }

            // Declare error and result
            Exception error = null;


            completed(error, query.ToList());
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="enumTypeEval"></param>
        /// <param name="completed">callback fonction</param>
        /// <param name="datedeb"></param>
        /// <param name="datefin"></param>
        public void GetEntityByCleAndDateAndTypeEval(int cle, DateTime? datedeb, DateTime? datefin, int enumTypeEval, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EqEquipement tmp;
            RefEnumValeur tmpRef;
            if (datedeb.HasValue && datefin.HasValue)
            {
                tmp = this.domainContext.EqEquipements

                    .FirstOrDefault(e => e.CleEquipement == cle);

                tmpRef = this.domainContext.RefEnumValeurs.FirstOrDefault(r => r.CleEnumValeur == enumTypeEval);

                var visites = this.domainContext.Visites
                   .Where(v => v.CleEquipement == cle && v.EstValidee && v.DateVisite.Value >= datedeb && v.DateVisite <= datefin
                            && (v.EnumTypeEval == enumTypeEval || (this.domainContext.RefEnumValeurs.FirstOrDefault(r => r.CleEnumValeur == enumTypeEval).Valeur == "1" && v.RefEnumValeur.Valeur == "2")));
                if (visites.Count() > 0)
                {
                    tmp.Visites.Add(visites.OrderByDescending(v => v.DateVisite).FirstOrDefault());
                }
            }
            else
            {
                tmp = this.domainContext.EqEquipements.Where(eq => eq.CleEquipement == cle).FirstOrDefault();
            }

            EntityQuery<EqEquipement> query = domainContext.GetEqEquipementByCleAndDateAndTypeEvalQuery(cle, datedeb.Value.Date, datefin, enumTypeEval);

            // Declare error and result
            Exception error = null;


            DetailEntity = tmp;
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<EqEquipement>(new List<EqEquipement>() { tmp });
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

            EntityQuery<EqEquipement> query = domainContext.GetEqEquipementByCleQuery(cle);

            // Declare error and result
            Exception error = null;


            DetailEntity = this.domainContext.EqEquipements.Where(eq => eq.CleEquipement == cle).FirstOrDefault();
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<EqEquipement>(new List<EqEquipement>() { DetailEntity });
            }

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle<T>(int cle, Action<Exception> completed) where T : Entity
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var getEntityMethod = domainContext.GetType().GetProperty(string.Format("{0}s", typeof(T).Name));
            if (getEntityMethod != null)
            {
                var set = (EntitySet<T>)getEntityMethod.GetValue(domainContext, null);
                var cleProp = typeof(T).GetProperties().First(p => p.Name.StartsWith("Cle"));
                var query = set.AsQueryable<T>().Where(e => (int)cleProp.GetValue(e, null) == cle);
                DetailEntity = query.FirstOrDefault() as EqEquipement;

                completed(null);

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

            var query = domainContext.EqEquipements.OrderBy(e => e.Libelle).AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            // Declare error and result
            Exception error = null;
            IEnumerable<EqEquipement> entities = null;

            entities = query.ToList();
            Entities = new ObservableCollection<EqEquipement>(entities.ToList());

            // Invoke completion callback
            completed(error);
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

            IQueryable<EqEquipement> query;

            if (clePortion.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.ClePortion == clePortion.Value
                    && (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else if (cleEnsElectrique.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value
                    && (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else if (cleSecteur.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value)
                    && (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else if (cleAgence.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.CleAgence == cleAgence.Value)
                    && (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else if (cleRegion.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value)
                    && (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment)).AsQueryable();
            }
            else
            {
                query = this.domainContext.EqEquipements.Where(e => (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment)).AsQueryable();
            }



            // Declare error and result
            Exception error = null;
            IEnumerable<EqEquipement> Equipements = null;


            Equipements = query.OrderBy(ee => ee.Libelle);
            Entities = new ObservableCollection<EqEquipement>(Equipements.ToList());


            // Invoke completion callback
            completed(error);

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
            var query = domainContext.EqEquipements.AsQueryable();
            if (clePortion.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.ClePortion == clePortion.Value).AsQueryable();
            }
            else if (cleEnsElectrique.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value).AsQueryable();
            }
            else if (cleSecteur.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value)).AsQueryable();
            }
            else if (cleAgence.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.CleAgence == cleAgence.Value)).AsQueryable();
            }
            else if (cleRegion.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value)).AsQueryable();
            }

            if (filtreEq.Count > 0)
            {
                query = query.Where(e => filtreEq.Contains(e.TypeEquipement.CodeEquipement));
            }
            if (!includeDeletedEquipment)
            {
                query = query.Where(e => e.Supprime == false);
            }


            // Declare error and result
            Exception error = null;
            IEnumerable<EqEquipement> Equipements = null;


            Equipements = query.OrderBy(ee => ee.TypeEquipement.NumeroOrdre).ThenBy(ee => ee.Libelle);
            Entities = new ObservableCollection<EqEquipement>(Equipements.ToList());


            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// On récupère la liste des tournées de l'équipement ayant comme identifiant la cle passée en paramètre
        /// </summary>
        /// <param name="cle">identifiant de l'équipement</param>
        /// <param name="completed">fonction de retour</param>
        public void GetListTournnees(int cle, Action<Exception, List<Tournee>> completed)
        {
            var query = domainContext.Tournees.Where(t => t.Compositions.Any(c => c.CleEquipement == cle)).OrderBy(t => t.Libelle);

            Exception error = null;
            List<Tournee> tournees = new List<Tournee>();

            tournees = query.ToList();


            completed(error, tournees);

        }

        /// <summary>
        /// Récupère une liste d'entité filtré sur le codeGroupe
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListPointCommun(Action<Exception, List<string>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);


            List<string> listPointCommun = new List<string>();

            listPointCommun.AddRange(this.domainContext.EqEquipements.OfType<EqLiaisonExterne>()
                .Where(e => !string.IsNullOrEmpty(e.LibellePointCommun) && e.Supprime != true)
                .Select(e => e.LibellePointCommun).Distinct().ToList());
            listPointCommun.AddRange(this.domainContext.EqEquipements.OfType<EqLiaisonInterne>()
                .Where(e => !string.IsNullOrEmpty(e.LibellePointCommun) && e.Supprime != true)
                .Select(e => e.LibellePointCommun).Distinct().ToList());

            // Declare error and result
            Exception error = null;

            completed(error, listPointCommun.Distinct().ToList());

        }

        /// <summary>
        /// Récupère la liste de liaison ayant le même libellé de point commun
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListLiaisonPointCommun(string libelle, Action<Exception, ObservableCollection<LiaisonCommunes>> completed)
        {

            Exception error = null;
            List<LiaisonCommunes> liaisonCommune = null;

            liaisonCommune = this.domainContext.EqEquipements.OfType<EqLiaisonExterne>()
         .Where(e => e.LibellePointCommun == libelle)
         .Select(e => new LiaisonCommunes
             {
                 LibelleLiaison = e.Libelle,
                 LibellePortion = e.Pp.PortionIntegrite.Libelle,
                 CleEquipement = e.CleEquipement,
                 TypeEquipement = e.TypeEquipement.CodeEquipement,
                 ClePortion = e.Pp.ClePortion
             })
         .Union(
             this.domainContext.EqEquipements.OfType<EqLiaisonInterne>()
             .Where(e => e.LibellePointCommun == libelle)
             .Select(e => new LiaisonCommunes
             {
                 LibelleLiaison = e.Libelle,
                 LibellePortion = e.Pp.PortionIntegrite.Libelle,
                 CleEquipement = e.CleEquipement,
                 TypeEquipement = e.TypeEquipement.CodeEquipement,
                 ClePortion = e.Pp.ClePortion
             })
         ).ToList();


            completed(error, new ObservableCollection<LiaisonCommunes>(liaisonCommune.OrderBy(l => l.LibelleLiaison)));
        }

        /// <summary>
        /// Récupère la liste de liaison ayant le même libellé de point commun
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListLiaisonPPCommun(int clePP, Action<Exception, ObservableCollection<LiaisonCommunes>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Récupère la liste de liaison ayant le même libellé de point commun
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListSoutirageExt(Action<Exception, ObservableCollection<EqSoutirage>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            var query = domainContext.EqEquipements.OfType<EqSoutirage>();

            Exception error = null;
            ObservableCollection<EqSoutirage> listSoutirage = null;

            listSoutirage = new ObservableCollection<EqSoutirage>(query.OrderBy(e => e.Libelle).ToList());
            completed(error, listSoutirage);

        }

        /// <summary>
        /// Récupère la liste de liaison ayant le même libellé de point commun
        /// </summary>
        /// <param name="codeGroupe"></param>
        public void GetListDrainageExt(Action<Exception, ObservableCollection<EqDrainage>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            var query = domainContext.EqEquipements.OfType<EqDrainage>();

            Exception error = null;
            ObservableCollection<EqDrainage> list = null;

            list = new ObservableCollection<EqDrainage>(query.OrderBy(e => e.Libelle).ToList());
            completed(error, list);

        }

        #endregion
    }
}
