using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Proteca.Silverlight.Models;
using Proteca.Silverlight.Services.Contracts;
using System.ComponentModel.Composition;
using Jounce.Core.Application;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Proteca.Silverlight.Services
{
    [Export(typeof(IEntityService<Document>))]
    public class DocumentService : SharepointService, IEntityService<Document>
    {
        #region Constructor
        public DocumentService()
        {
            Entities = new ObservableCollection<Document>();
        }
        #endregion

        public void Add(Document entity)
        {
            Entities.Add(entity);
            entity.IsNewEntity = true;
        }

        public void Delete(Document entity)
        {
            entity.IsDeleted = true;
        }

        public void Clear()
        {
            if (this.Entities != null)
            {
                this.Entities.Clear();
            }
        }

        public void GetEntities(Action<Exception> completed)
        {
            completed(null);
        }

        /// <summary>
        /// Récupère la liste des documents liés à l'ouvrage
        /// </summary>
        /// <param name="completed"></param>
        /// <param name="typeOuvrage"></param>
        /// <param name="cleOuvrage"></param>
        /// <param name="relativePath"></param>
        public void GetEntitiesByCleOuvrage(Action<Exception> completed, CleOuvrage? typeOuvrage, int? cleOuvrage, string relativePath = null)
        {
            string type = typeOuvrage != null ? Enum.GetName(typeof(CleOuvrage), typeOuvrage) : null;
            string cle = cleOuvrage != null ? cleOuvrage.ToString() : null;
            this.GetFiles(cle, type,
            (ex, entities) =>
            {
                if (ex == null)
                {
                    this.Entities = new System.Collections.ObjectModel.ObservableCollection<Document>(entities);
                    completed(null);
                }
                else
                {
                    Logger.Log(LogSeverity.Error, GetType().FullName, ex.ToString());
                    completed(ex);
                }
            }, relativePath);
        }

        public void GetEntityByCle(int cle, Action<Exception> completed)
        {
            throw new NotImplementedException();
        }

        public void RejectChanges()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sauvegarde les documents dans sharepoint
        /// </summary>
        /// <param name="completed"></param>
        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            bool isValid = false;

            foreach (var entity in Entities)
            {
                Collection<ValidationResult> errors = new Collection<ValidationResult>();
                isValid = Validator.TryValidateObject(entity, new ValidationContext(entity, null, null), errors, true);
                if (!isValid)
                {
                    foreach (var err in errors)
                    {
                        entity.ValidationErrors.Add(err);
                    }
                }
            }

            if (isValid)
            {

                var modifiedEntities = Entities.Where(e => e.IsDeleted || e.IsMoved || e.IsModified || e.IsNewEntity);
                if (modifiedEntities.Any())
                {
                    try
                    {
                        foreach (var entity in modifiedEntities)
                        {
                            if (entity.IsDeleted || (entity.IsNewEntity && entity.ItemId > 0))
                            {
                                //suppression
                                this.DeleteFile(entity.ServerRelativeUrl, entity.PrefixeFileName + entity.LibelleOriginal, completed);
                            }

                            if (entity.IsNewEntity)
                            {
                                //ajout
                                var metadatas = new Dictionary<string, object>();
                                metadatas.Add(Enum.GetName(typeof(CleOuvrage), entity.TypeOuvrage), entity.CleOuvrage.ToString());
                                metadatas.Add("Archive", entity.Archive);
                                metadatas.Add("NumEnregistrement", entity.NumeroVersion);
                                metadatas.Add("Libell_x00e9_", entity.Libelle);
                                this.CreateFile(entity.ServerRelativeUrl, entity.PrefixeFileName + entity.Libelle, entity.Content, metadatas, completed);
                                entity.Cle = Entities.Max(e => e.Cle) + 1;
                                entity.IsNewEntity = false;
                                entity.DocumentUrl = GetDocumentUrl(entity);
                            }
                            else if (entity.IsModified)
                            {
                                // mise à jour
                                var metadatas = new Dictionary<string, object>();
                                metadatas.Add("Archive", entity.Archive);
                                metadatas.Add("NumEnregistrement", entity.NumeroVersion);
                                metadatas.Add("Libell_x00e9_", entity.Libelle);
                                this.UpdateFile(entity.ServerRelativeUrl, entity.PrefixeFileName + entity.Libelle, metadatas, (ex) =>
                                {
                                    entity.IsModified = false;
                                    if (entity.IsMoved)
                                    {
                                        // Déplacement d'un document
                                        this.MoveFile(entity.OriginalServerRelativeUrl, entity.ServerRelativeUrl, entity.PrefixeFileName + entity.Libelle, completed);
                                        entity.DocumentUrl = GetDocumentUrl(entity);
                                        entity.IsMoved = false;
                                    }
                                    else
                                    {
                                        completed(ex);
                                    }
                                });
                            }
                            else if (entity.IsMoved)
                            {
                                // Déplacement d'un document
                                this.MoveFile(entity.OriginalServerRelativeUrl, entity.ServerRelativeUrl, entity.PrefixeFileName + entity.Libelle, completed);
                                entity.DocumentUrl = GetDocumentUrl(entity);
                                entity.IsMoved = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogSeverity.Error, GetType().FullName, ex.ToString());
                        completed(ex);
                    }
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

        public Uri GetDocumentUrl(Document doc)
        {
            return new Uri(ContextClientSharePoint.Url + doc.ServerRelativeUrl + "/" + doc.PrefixeFileName + doc.Libelle, UriKind.Absolute);
        }


        public System.Collections.ObjectModel.ObservableCollection<Document> Entities
        {
            get;
            set;
        }

        public Document DetailEntity
        {
            get;
            set;
        }

        public void FindEntities(System.Collections.Generic.List<System.Linq.Expressions.Expression<Func<Document, bool>>> filtres, Action<Exception> completed)
        {
            throw new NotImplementedException();
        }
    }
}
