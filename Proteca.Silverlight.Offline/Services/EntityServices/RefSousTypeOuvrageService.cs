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
    [Export(typeof(IEntityService<RefSousTypeOuvrage>))]
    public class RefSousTypeOuvrageService : IEntityService<RefSousTypeOuvrage>
    {
        #region Properties

        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        public ObservableCollection<RefSousTypeOuvrage> Entities { get; set; }

        public RefSousTypeOuvrage DetailEntity { get; set; }

        #endregion Properties

        #region Constructor

        [ImportingConstructor]
        public RefSousTypeOuvrageService([Import(AllowDefault = true)] IConfigurator configurator)
        {
            Entities = new ObservableCollection<RefSousTypeOuvrage>();
            domainContext = new ProtecaDomainContext(configurator);
        }

        #endregion

        #region Methods

        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(RefSousTypeOuvrage entity)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            EntitySet es = domainContext.EntityContainer.GetEntitySet(entity.GetType());
            es.Add(entity);
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(RefSousTypeOuvrage entity)
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
            //on ne met pas à jour la liste des enum est fixe
        }

        /// <summary>
        /// Save changes on the domain content if there are any
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if (domainContext.RefSousTypeOuvrages != null && domainContext.RefSousTypeOuvrages.HasChanges)
            {
                bool isValid = true;
                foreach (var obj in domainContext.RefSousTypeOuvrages.EntityContainer.GetChanges())
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
            if (domainContext.RefSousTypeOuvrages.Count > 0)
            {
                Entities = new ObservableCollection<RefSousTypeOuvrage>(domainContext.RefSousTypeOuvrages.OrderBy(r => r.NumeroOrdre).ThenBy(r => r.Libelle));
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
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.RefSousTypeOuvrages.FirstOrDefault(r => r.CleSousTypeOuvrage == cle);

            // Declare error and result
            Exception error = null;


            DetailEntity = query;
            if (Entities == null || Entities.Count == 0)
            {
                Entities = new ObservableCollection<RefSousTypeOuvrage>(new List<RefSousTypeOuvrage>() { DetailEntity });
            }

            // Invoke completion callback
            completed(error);

        }

        /// <summary>
        /// Récupère la liste des entités en fonction du filtre défini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<RefSousTypeOuvrage, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, this.GetType().FullName, MethodBase.GetCurrentMethod().Name);

            var query = domainContext.RefSousTypeOuvrages.AsQueryable();

            foreach (var filtre in filtres)
            {
                query = query.Where(filtre);
            }


            // Declare error and result
            Exception error = null;
            IEnumerable<RefSousTypeOuvrage> entities = null;


            entities = query;
            Entities = new ObservableCollection<RefSousTypeOuvrage>(entities.ToList());


            // Invoke completion callback
            completed(error);
        }

        /// <summary>
        /// Vérifie si le sous type d'ouvrage a supprimer n'est pas utilisé dans un équipement.
        /// Si pas utilisé alors on le supprime.
        /// </summary>
        public void CheckCanDeleteRefSousTypeOuvrage(string groupeName, int cle, Action<Exception, bool> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            throw new NotImplementedException();
        }

        #endregion
    }
}
