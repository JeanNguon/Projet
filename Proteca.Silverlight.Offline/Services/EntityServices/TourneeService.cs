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
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.IO.IsolatedStorage;
using Telerik.Windows.Zip;

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
    [Export(typeof(IEntityService<Tournee>))]
    public class TourneeService : IEntityService<Tournee>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<Tournee> Entities { get; set; }

        public Tournee DetailEntity { get; set; }
        #endregion

        #region Constructor
        public TourneeService()
        {
            this.Entities = new ObservableCollection<Tournee>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Tournee entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Tournee entity)
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
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteComposition(Composition entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (domainContext.Compositions.Contains(entity))
            {
                domainContext.Compositions.Remove(entity);
            }
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            this.Entities = new ObservableCollection<Tournee>();
        }

        /// <summary>
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.Tournees != null && (domainContext.Tournees.HasChanges || domainContext.Compositions.HasChanges))
            {
                bool isValid = true;
                foreach (var obj in domainContext.Tournees.EntityContainer.GetChanges())
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
        }
        #endregion

        /// <summary>
        /// Retourne la liste des tournées correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="cleEnsElectrique">inclue ou non les ensemble electrique sans portion</param>
        /// <param name="clePortion">inclue ou non les ensemble électrique de type station</param>
        /// <param name="libelle">filtre sur le libelle de l'ensemble électrique</param>
        /// <param name="isDelete">filtre les tournées supprimée ou non</param>
        /// <param name="completed"></param>
        /// <returns>Une liste d'ensemble électrique</returns>
        public void FindTourneeByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElectrique, int? clePortion,
            string libelle, bool isDelete, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // Declare error and result
            Exception error = null;
            IEnumerable<Tournee> tournees = null;

            //TODO filtrer
            tournees = domainContext.Tournees;
            Entities = new ObservableCollection<Tournee>(tournees.ToList());

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            if (Entities == null)
            {
                Entities = new ObservableCollection<Tournee>();
            }
            completed(null);
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
                DetailEntity = query.FirstOrDefault() as Tournee;

                completed(null);

            }
            else
            {
                Logger.Log(LogSeverity.Error, GetType().FullName, string.Format("La méhode Get{0}ByCleQuery n'existe pas.", typeof(T).Name));
            }
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // Declare error and result
            Exception error = null;

            DetailEntity = domainContext.Tournees.Where(t => t.CleTournee == cle).FirstOrDefault();
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<Tournee>(domainContext.Tournees.Where(t => t.CleTournee == cle));
            }

            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<Tournee, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            IQueryable<Tournee> query = domainContext.Tournees.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }

            // Declare error and result
            Exception error = null;
            IEnumerable<Tournee> entities = null;

            entities = query.ToList();
            Entities = new ObservableCollection<Tournee>(entities.ToList());

            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Export la tournée en XML
        /// </summary>
        public void ExportTourneeToXml(int cle, Action<Exception, List<String>> completed)
        {

            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            throw new NotImplementedException();
            //domainContext.GetTourneeExportXML(this.DetailEntity.CleTournee, loadOp =>
            //{
            //    // Declare error and result
            //    Exception error = null;
            //    List<String> retour = new List<string>();

            //    // Set error or result
            //    if (loadOp.HasError)
            //    {
            //        error = loadOp.Error;
            //        Logger.Log(LogSeverity.Error, GetType().FullName, error);
            //        loadOp.MarkErrorAsHandled();
            //    }
            //    else
            //    {
            //        // retourne le fichier de tournée
            //        retour = loadOp.Value.ToList();
            //    }

            //    // Invoke completion callback
            //    completed(error, retour);
            //}, null);
        }
        /// <summary>
        /// Retourne la liste des equipements correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="includeWithoutPortion">inclue ou non les ensemble electrique sans portion</param>
        /// <param name="includeStation">inclue ou non les ensemble électrique de type station</param>
        /// <param name="includePosteGaz">inclue ou non les ensemble électrique de type poste gaz</param>
        /// <param name="libelleEnsElec">filtre sur le libelle de l'ensemble électrique</param>
        /// <returns>Une liste d'ensemble électrique</returns>
        public void FindEquipementByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, bool eqAG, bool eqDE, bool eqDR, bool eqFM,
            bool eqLE, bool eqLI, bool eqPI, bool eqPO, bool eqPP, bool eqRI, bool eqSO, bool eqTC, Action<Exception, List<Entity>> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IEnumerable<Entity> eqEquipementsPP = null;
            List<Entity> eqEquipements = new List<Entity>();
            IQueryable<EqEquipement> query;

            if (cleSecteur.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value)
                    && e.Supprime == false).AsQueryable();
            }
            else if (cleAgence.HasValue)
            {
                query = this.domainContext.EqEquipements
                    .Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.CleAgence == cleAgence.Value)
                     && e.Supprime == false).AsQueryable();
            }
            else if (cleRegion.HasValue)
            {
                query = this.domainContext.EqEquipements.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value) && e.Supprime == false).AsQueryable();
            }
            else
            {
                query = this.domainContext.EqEquipements.Where(e => e.Supprime == false).AsQueryable();
            }

            eqEquipementsPP = query.OrderBy(ee => ee.Libelle);
            // Declare error and result
            Exception error = null;



            foreach (Entity eq in eqEquipementsPP)
            {
                if (eq is EqAnodeGalvanique && eqAG)
                    eqEquipements.Add(eq);
                else if (eq is EqDispoEcoulementCourantsAlternatifs && eqDE)
                    eqEquipements.Add(eq);
                else if (eq is EqDrainage && eqDR)
                    eqEquipements.Add(eq);
                else if (eq is Pp && eqPP)
                    eqEquipements.Add(eq);
                else if (eq is EqFourreauMetallique && eqFM)
                    eqEquipements.Add(eq);
                else if (eq is EqLiaisonExterne && eqLE)
                    eqEquipements.Add(eq);
                else if (eq is EqLiaisonInterne && eqLI)
                    eqEquipements.Add(eq);
                else if (eq is EqPile && eqPI)
                    eqEquipements.Add(eq);
                else if (eq is EqPostegaz && eqPO)
                    eqEquipements.Add(eq);
                else if (eq is EqRaccordIsolant && eqRI)
                    eqEquipements.Add(eq);
                else if (eq is EqSoutirage && eqSO)
                    eqEquipements.Add(eq);
                else if (eq is EqTiersCroiseSansLiaison && eqTC)
                    eqEquipements.Add(eq);
            }


            // Invoke completion callback
            completed(error, eqEquipements);

        }

        /// <summary>
        /// Retourne la portion avec ses PPs et ses équipements
        /// </summary>
        /// <param name="clePortion"></param>
        /// <param name="completed"></param>
        public void GetTourneePortionIntegriteByCle(int clePortion, Action<Exception, PortionIntegrite> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            var query = domainContext.PortionIntegrites.FirstOrDefault(p => p.ClePortion == clePortion);

            // Declare error and result
            Exception error = null;
            PortionIntegrite portion = null;

            portion = query;

            // Invoke completion callback
            completed(error, portion);
        }

        /// <summary>
        /// Retourne l'Ensemble Electrique avec ses Portions, ses PPs et ses équipements
        /// </summary>
        /// <param name="cleEE"></param>
        /// <param name="completed"></param>
        public void GetTourneeEnsElecByCle(int cleEE, Action<Exception, EnsembleElectrique> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.EnsembleElectriques.FirstOrDefault(e => e.CleEnsElectrique == cleEE);

            // Declare error and result
            Exception error = null;
            EnsembleElectrique EE = null;


            EE = query;

            // Invoke completion callback
            completed(error, EE);

        }

        #endregion
    }
}
