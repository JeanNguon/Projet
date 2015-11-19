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

            if (domainContext.UsrUtilisateurs.Count > 0)
            {
                Entities = new ObservableCollection<UsrUtilisateur>(domainContext.UsrUtilisateurs.ToList());
            }
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

            var query = domainContext.UsrUtilisateurs.FirstOrDefault(u => u.CleUtilisateur == cle);

            // Declare error and result
            Exception error = null;


            DetailEntity = query;
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<UsrUtilisateur>(new List<UsrUtilisateur>() { DetailEntity });
            }


            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="identifiant">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByIdentifiant(string identifiant, Action<Exception, UsrUtilisateur> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            UsrUtilisateur user = domainContext.UsrUtilisateurs.FirstOrDefault(u => !String.IsNullOrEmpty(u.Identifiant) && u.Identifiant.ToUpper() == identifiant.ToUpper());
            if (user != null)
            {
                completed(null, user);
            }
            else
            {
                completed(new Exception("Utilisateur introuvable"), domainContext.UsrUtilisateurs.FirstOrDefault());
            }
        }

        /// <summary>
        /// Retourne la valeur true si une dépendance dans les autres table est trouvé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void CheckDeleteUsrUtilisateurList(int cle, Action<bool, Exception> completed)
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<UsrUtilisateur, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.UsrUtilisateurs.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }


            // Declare error and result
            Exception error = null;
            IEnumerable<UsrUtilisateur> UsrUtilisateurs = null;


            UsrUtilisateurs = query;
            Entities = new ObservableCollection<UsrUtilisateur>(UsrUtilisateurs.ToList());


            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Récupère la liste des entités Active Directory en fonction du filtre défini
        /// </summary>
        public void FindADEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Récupère une seule entité en fonction de sa clé
        /// </summary>
        /// <param name="cle">clé de l'entité</param>
        /// <param name="completed">callback fonction</param>
        public void FindADEntityByCle(string cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            throw new NotImplementedException();
        }

        public void FindUsrUtilisateurByCleTournee(int cleTournee, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IEnumerable<UsrUtilisateur> usrUtilisateur = null;
            IEnumerable<GeoSecteur> secteurs = this.domainContext.Compositions.Where(co => co.CleTournee == cleTournee && co.ClePp.HasValue && !co.Pp.Supprime).Select(co => co.Pp.GeoSecteur)
               .Union(this.domainContext.Compositions.Where(co => co.CleTournee == cleTournee && co.CleEquipement.HasValue && !co.EqEquipement.Supprime).Select(co => co.EqEquipement.Pp.GeoSecteur)).Distinct();

            IEnumerable<int> cleSecteurs = secteurs.Select(s => s.CleSecteur);
            IEnumerable<int> cleAgences = secteurs.Select(a => a.CleAgence).Distinct();

            usrUtilisateur = this.domainContext.UsrUtilisateurs.Where(u => !u.Supprime && (u.Externe || (u.CleSecteur.HasValue && cleSecteurs.Contains(u.CleSecteur.Value)) || (!u.CleSecteur.HasValue && u.CleAgence.HasValue && cleAgences.Contains(u.CleAgence.Value)))).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);

            // Declare error and result
            Exception error = null;

            Entities = new ObservableCollection<UsrUtilisateur>(usrUtilisateur.ToList());

            // Invoke completion callback
            completed(error);
        }

        public void FindUsrUtilisateurbyGeoCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IEnumerable<UsrUtilisateur> usrUtilisateur = null;

            if (cleSecteur.HasValue && cleAgence.HasValue)
                usrUtilisateur = this.domainContext.UsrUtilisateurs.Where(u => u.CleSecteur == cleSecteur.Value || u.CleAgence == cleAgence.Value).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
            else if (cleAgence.HasValue)
                usrUtilisateur = this.domainContext.UsrUtilisateurs.Where(u => u.CleAgence == cleAgence.Value).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
            else if (cleRegion.HasValue)
                usrUtilisateur = this.domainContext.UsrUtilisateurs.Where(u => u.GeoAgence.CleRegion == cleRegion.Value).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
            else
                usrUtilisateur = this.domainContext.UsrUtilisateurs.Where(u => u.Externe == false).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);

            // Declare error and result
            Exception error = null;

            Entities = new ObservableCollection<UsrUtilisateur>(usrUtilisateur.ToList());
            // Invoke completion callback
            completed(error);

        }

        public void FindUsrUtilisateurByGeoSecteur(int cleSecteur, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            IEnumerable<UsrUtilisateur> usrUtilisateur = null;
            GeoAgence agence = this.domainContext.GeoSecteurs.Where(s => s.CleSecteur == cleSecteur).Select(s => s.GeoAgence).FirstOrDefault();

            usrUtilisateur = this.domainContext.UsrUtilisateurs
                .Where(u => !u.Supprime && (u.Externe || (u.CleSecteur.HasValue && u.CleSecteur.Value == cleSecteur) || (u.CleAgence.HasValue && u.CleAgence.Value == agence.CleAgence)));

            // Declare error and result
            Exception error = null;


            Entities = new ObservableCollection<UsrUtilisateur>(usrUtilisateur.ToList());

            // Invoke completion callback
            completed(error);
        }

        public void FindUsrUtilisateurbyInternalCriterias(bool externe, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            Exception error = null;
            IEnumerable<UsrUtilisateur> usrUtilisateur = this.domainContext.UsrUtilisateurs.Where(u => u.Supprime == false && u.Externe == externe).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);


            Entities = new ObservableCollection<UsrUtilisateur>(usrUtilisateur.ToList());

            // Invoke completion callback
            completed(error);
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
