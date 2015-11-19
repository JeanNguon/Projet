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
using Microsoft.SharePoint.Client;
using Proteca.Silverlight.Models;
using System.Threading;
using Proteca.Silverlight.Resources;

namespace Proteca.Silverlight.Services
{
    /// <summary>
    /// Service d'intéraction avec Sharepoint
    /// </summary>
    [Export(typeof(IEntityService<TypeDocument>))]
    public class TypeDocumentService : SharepointService, IEntityService<TypeDocument>
    {
        #region Properties

        IEnumerable<Folder> foldersInfo;

        public ObservableCollection<TypeDocument> Entities { get; set; }

        #endregion

        #region Constructor
        public TypeDocumentService()
        {
            Entities = new ObservableCollection<TypeDocument>();
        }
        #endregion

        #region Methods
        public void Add(TypeDocument entity)
        {
            entity.IsNewEntity = true;
            if (String.IsNullOrEmpty(entity.ServerRelativeUrl))
            {
                entity.ServerRelativeUrl = "/" + this.ListName;
            }
        }

        public void Delete(TypeDocument entity)
        {
            entity.IsDeleted = true;
        }

        /// <summary>
        /// Nettoyage du service remise à 0 de certaines propriétés du domainContext
        /// </summary>
        public void Clear()
        {
            this.Entities.Clear();
        }

        private void EntitiesLoaded(Exception error)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (error != null)
            {
                Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
                ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(TypeDocument).Name));
            }
        }

        /// <summary>
        /// Récupère la liste de tous les dossiers de la liste sharepoint et les convertis en TypeDocument
        /// </summary>
        /// <param name="completed"></param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (Entities.Any())
            {
                completed(null);
            }
            else
            {
                ResetContext();
                ClientContext currentContext = ContextClientSharePoint;
                if (currentContext != null)
                {
                    Folder specificFolder = currentContext.Web.GetFolderByServerRelativeUrl(DefaultFolder);

                    // Construction de la query de récupération des dossiers
                    foldersInfo = currentContext.LoadQuery(specificFolder.Folders.Include(f => f.Name, f => f.ServerRelativeUrl, f => f.Folders.Include(f2 => f2.Name, f2 => f2.ServerRelativeUrl, f2 => f2.Folders.Include(f3 => f3.Name, f3 => f3.ServerRelativeUrl, f3 => f3.Folders))));

                    currentContext.ExecuteQueryAsync(
                           (o, e) =>
                           {
                               try
                               {
                                   index = 0;
                                   _syncCtxt.Post(unused => {
                                       Entities = new ObservableCollection<TypeDocument>(convertFolderToTypeDocument(foldersInfo.Where(f => f.ServerRelativeUrl != "/GED/Forms")));
                                       completed(null);
                                   }, null);
                               }
                               catch(Exception ex)
                               {
                                   _syncCtxt.Post(unused => completed(ex), null);
                               }
                           }
                           ,
                           (o, e) =>
                           {
                               _syncCtxt.Post(unused => completed(e.Exception), null);
                           }
                        );
                }
                else
                {
                    Logger.Log(LogSeverity.Warning, GetType().FullName, Resource.TypeDocument_SharepointContextError);
                    completed(null);
                }
            }
        }

        private int index = 0;
        private List<TypeDocument> convertFolderToTypeDocument(IEnumerable<Folder> folders, TypeDocument parent = null)
        {
            List<TypeDocument> results = new List<TypeDocument>();
            foreach (var folder in folders)
            {
                index++;
                TypeDocument result = new TypeDocument(index, folder.Name, folder.ServerRelativeUrl);
                result.Parent = parent;
                result.Entities = convertFolderToTypeDocument(folder.Folders, result);
                results.Add(result);
            }
            return results;
        }

        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            DetailEntity = getEntityRecursively(cle, this.Entities);
            completed(null);
        }

        private TypeDocument getEntityRecursively(int cle, IList<TypeDocument> entities)
        {
            if (entities.Where(e => e.Cle == cle).Any())
            {
                return entities.Where(f => f.Cle == cle).FirstOrDefault();
            }
            else
            {
                foreach (var entity in entities)
                {
                    var res = getEntityRecursively(cle, entity.Entities);
                    if (res != null)
                    {
                        return res;
                    }
                }
                return null;
            }
        }

        public void RejectChanges()
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            GetEntities((error) => EntitiesLoaded(error));
        }

        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            var entity = this.DetailEntity;
            entity.Libelle = entity.Libelle.Trim();
            Collection<ValidationResult> errors = new Collection<ValidationResult>();
            bool isValid = Validator.TryValidateObject(entity, new ValidationContext(entity, null, null), errors, true);
            if (!isValid)
            {
                foreach (var err in errors)
                {
                    entity.ValidationErrors.Add(err);
                }
            }

            if (isValid)
            {
                if (entity.IsDeleted)
                {
                    //suppression
                    this.DeleteFolder(entity.ServerRelativeUrl, entity.Libelle, completed);
                }
                else if (entity.IsNewEntity)
                {
                    //ajout
                    this.CreateFolder(entity.ServerRelativeUrl, entity.Libelle, completed);
                }
                else if (entity.IsModified)
                {
                    //Update
                    this.RenameFolder(entity.ServerRelativeUrl, entity.LibelleOriginal, entity.Libelle, completed);
                }
                else
                {
                    completed(null);
                }
            }
            else
            {
                completed(null);
            }
        }

        public TypeDocument DetailEntity
        {
            get;
            set;
        }

        public void FindEntities(List<System.Linq.Expressions.Expression<Func<TypeDocument, bool>>> filtres, Action<Exception> completed)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
