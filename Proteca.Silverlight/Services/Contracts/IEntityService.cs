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
using System.ServiceModel.DomainServices.Client;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Proteca.Silverlight.Services.Contracts
{
    public interface IEntityService<T> : IEntityService where T : Entity
    {
        void Add(T entity);
        void Delete(T entity);
        void Clear();
        void GetEntityByCle(int cle, Action<Exception> completed);
        void RejectChanges();
        void SaveChanges(Action<Exception> completed);
        ObservableCollection<T> Entities { get; set; }
        T DetailEntity { get; set; }
        void FindEntities(List<Expression<Func<T, bool>>> filtres, Action<Exception> completed);

    }
    public interface IEntityService
    {
        void GetEntities(Action<Exception> completed);
    }
    public static class EntityServiceHelper
    {
        #region DomainContext
        public static Task<IEnumerable<Exception>> LoadManyAsync<TDomain>(this TDomain domain, LoadBehavior loadBehavior, params Func<TDomain, EntityQuery>[] getQueries)
             where TDomain : DomainContext
        {
            var loadTask = getQueries.Select(getQuery => domain.LoadAsync(getQuery, loadBehavior).HandleError()).ToArray();
            return Task.Factory.ContinueWhenAll(
                loadTask,
                ts => ts.Where(t => t.Result != null).Select(t => t.Result),
                new System.Threading.CancellationToken(),
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

        }

        public static Task ContinueWhenAll(Task[] tasks, Action<Task[]> act)
        {
            return Task.Factory.ContinueWhenAll(tasks, act, new System.Threading.CancellationToken(), TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Task ContinueWhenAll<TResult>(Task<TResult>[] tasks, Action<Task<TResult>[]> act)
        {
            return Task.Factory.ContinueWhenAll<TResult>(tasks, act, new System.Threading.CancellationToken(), TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Task<Exception> HandleError<TEntity>(this Task<LoadOperation<TEntity>> loadTask)
            where TEntity : Entity
        {
            return loadTask.ContinueWith<Exception>(tLo =>
            {
                if (tLo.Result.HasError)
                {
                    var error = tLo.Result.Error;
                    tLo.Result.MarkErrorAsHandled();
                    return error;
                }
                else
                    return null;
            });
        }
        public static Task<Exception> HandleError(this Task<LoadOperation> loadTask)
        {
            return loadTask.ContinueWith<Exception>(tLo =>
            {
                if (tLo.Result.HasError)
                {
                    var error = tLo.Result.Error;
                    tLo.Result.MarkErrorAsHandled();
                    return error;
                }
                else
                    return null;
            });
        }

        public static Task<LoadOperation<TEntity>> LoadAsync<TDomain, TEntity>(this TDomain domain, Func<TDomain, EntityQuery<TEntity>> getQuery)
            where TDomain : DomainContext
            where TEntity : Entity
        {
            var completion = new TaskCompletionSource<LoadOperation<TEntity>>();
            domain.Load(getQuery(domain), op => { completion.SetResult(op); }, null);
            return completion.Task;
        }
        public static Task<LoadOperation<TEntity>> LoadAsync<TDomain, TEntity>(this TDomain domain, Func<TDomain, EntityQuery<TEntity>> getQuery, LoadBehavior loadBehavior)
            where TDomain : DomainContext
            where TEntity : Entity
        {
            var completion = new TaskCompletionSource<LoadOperation<TEntity>>();
            domain.Load(getQuery(domain), loadBehavior, op => { completion.SetResult(op); }, null);
            return completion.Task;
        }

        public static Task<LoadOperation> LoadAsync<TDomain>(this TDomain domain, Func<TDomain, EntityQuery> getQuery, LoadBehavior loadBehavior)
            where TDomain : DomainContext
        {
            var completion = new TaskCompletionSource<LoadOperation>();
            domain.Load(getQuery(domain), loadBehavior, op => { completion.SetResult(op); }, null);
            return completion.Task;
        }
        #endregion

        #region EntityServices

        private static IEntityService[] GetServicesToLoad(object container, Func<IEntityService, bool> serviceFilter)
        {
            var servicesQuery = container.GetType().GetProperties()
                   .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(IEntityService)))
                   .Select(p => (IEntityService)p.GetValue(container, null));
            if (serviceFilter != null)
                servicesQuery = servicesQuery.Where(serviceFilter);
            return servicesQuery.ToArray();

        }

        private static Task<Exception>[] LoadAllServices(object container, Func<IEntityService, bool> serviceFilter, Action<IEntityService, Exception> anyLoadedAction)
        {
            var services = GetServicesToLoad(container, serviceFilter);
            return services.Select(svc => GetEntitiesTask(svc, anyLoadedAction)).ToArray();
        }

        private static Task<Exception> GetEntitiesTask(IEntityService service, Action<IEntityService, Exception> loadedAction)
        {
            var completion = new TaskCompletionSource<Exception>();
            service.GetEntities(err =>
            {
                try
                {
                    loadedAction(service, err);
                    completion.SetResult(err);
                }
                catch (Exception ex)
                {
                    completion.SetException(ex);
                }
            });
            return completion.Task;
        }

        //public static void LoadAllServicesSync(object container, Func<IEntityService, bool> serviceFilter, Action<IEntityService, Exception> anyLoadedAction)
        //{
        //    var tasks = LoadAllServices(container, serviceFilter, anyLoadedAction);
        //    Task.WaitAll(tasks);
        //}

        public static void LoadAllServicesAsync(object container, Action<IEntityService, Exception> anyLoadedAction, Action allLoadedAction)
        {
            LoadAllServicesAsync(container, null, anyLoadedAction, allLoadedAction);
        }
        public static void LoadAllServicesAsync(object container, Func<IEntityService, bool> serviceFilter, Action<IEntityService, Exception> anyLoadedAction, Action allLoadedAction)
        {
            var tasks = LoadAllServices(container, serviceFilter, anyLoadedAction);
            if (Application.Current.IsRunningOutOfBrowser)
            {
                Task.WaitAll(tasks);
                allLoadedAction();
            }
            else
            {
                ContinueWhenAll(tasks, ts => allLoadedAction());
            }
        }

        #endregion
    }

}
