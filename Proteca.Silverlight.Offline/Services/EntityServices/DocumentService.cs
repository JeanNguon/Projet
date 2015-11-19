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
            completed(null);
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
            
            completed(null);
        }

        public Uri GetDocumentUrl(Document doc)
        {
            return new Uri(ContextClientSharePoint.Url + doc.ServerRelativeUrl + "/" + doc.Libelle, UriKind.Absolute);
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
