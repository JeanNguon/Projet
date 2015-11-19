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
    [Export(typeof(AllUsrUtilisateurService))]
    public class AllUsrUtilisateurService : IEntityService<UsrUtilisateur>
    {
        #region Properties

        /// <summary>
        /// Importation du DomainContext
        /// </summary>
        [Import]
        public ProtecaDomainContext domainContext { get; set; }

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

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur du service UsrUtilisateur
        /// </summary>
        public AllUsrUtilisateurService()
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
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(UsrUtilisateur entity)
        {
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
            completed(null);
        }

        /// <summary>
        /// Reverses all pending changes since the data was loaded
        /// </summary>
        public void RejectChanges()
        {
        }
        #endregion

        /// <summary>
        /// Récupère une liste d'entité 
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            EntityQuery<UsrUtilisateur> query = domainContext.FindUsrUtilisateurbyInternalCriteriasQuery(false);
            domainContext.Load(query, LoadBehavior.MergeIntoCurrent, loadOp =>
            {
                // Declare error and result
                Exception error = null;
                IEnumerable<UsrUtilisateur> usr = null;

                // Set error or result
                if (loadOp.HasError)
                {
                    error = loadOp.Error;
                    Logger.Log(LogSeverity.Error, GetType().FullName, error);
                    loadOp.MarkErrorAsHandled();
                }
                else
                {
                    usr = loadOp.Entities;
                    Entities = new ObservableCollection<UsrUtilisateur>(usr.ToList());
                }

                // Invoke completion callback
                completed(error);
            }, null);
        }

        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            completed(null);
        }

        public void FindEntities(List<Expression<Func<UsrUtilisateur, bool>>> filtres, Action<Exception> completed)
        {
            completed(null);
        }
        #endregion
    }
}
