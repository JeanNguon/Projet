using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Client;

namespace Offline
{
    /// <summary>
    /// Class that enables taking entities offline
    /// </summary>
    public class OfflinableEntity
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public OfflinableEntity()
        {
        }

        /// <summary>
        /// Constructor referencing an <see cref="Entity"/>
        /// </summary>
        /// <param name="entity"><see cref="Entity"/> reference to be offlined</param>
        /// <param name="addedEntity">Conditional <see cref="Boolean"/> to specify whether or not the <see cref="Entity"/> is Added</param>
        public OfflinableEntity(Entity entity, bool addedEntity = false)
        {
            this.OriginalEntity = entity.GetOriginal();
            this.CurrentEntity = entity;
 
            if (addedEntity)
            {
                foreach (var collection in entity.GetType().GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>)))
                {
                    List<Entity> entities = new List<Entity>();
                    foreach (var entityRef in collection.GetValue(entity, null) as IEnumerable<Entity>)
                    {
                       entities.Add(entityRef);
                    }
                    if (entities.Any())
                    {
                        if (EntityRefs == null)
                        {
                            EntityRefs = new Dictionary<string, List<Entity>>();
                        }
                        this.EntityRefs.Add(collection.Name, entities);
                    }
                }
            }
            this.EntityState = entity.EntityState;
            this.EntityActions = ConvertToOfflinableEntityActions(entity.EntityActions);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Original <see cref="Entity"/> set at load
        /// </summary>
        public Entity OriginalEntity { get; set; }

        /// <summary>
        /// Current <see cref="Entity"/> in use
        /// </summary>
        public Entity CurrentEntity { get; set; }

        /// <summary>
        /// <see cref="Dictionary"/> of references to <see cref="Entity"/> related
        /// </summary>
        public Dictionary<string, List<Entity>> EntityRefs { get; set; }

        /// <summary>
        /// Get the current state of this entity
        /// </summary>
        public EntityState EntityState { get; set; }

        /// <summary>
        /// Get the list of custom method invocations pending for this entity
        /// </summary>
        public IEnumerable<OfflinableEntityAction> EntityActions { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Convert <see cref="EntityAction"/> list to <see cref="OfflinableEntityAction"/> list
        /// </summary>
        /// <param name="entityActions"><see cref="EntityAction"/> list</param>
        /// <returns><see cref="OfflinableEntityAction"/> list</returns>
        private IEnumerable<OfflinableEntityAction> ConvertToOfflinableEntityActions(IEnumerable<EntityAction> entityActions)
        {
            List<OfflinableEntityAction> entityActions2 = new List<OfflinableEntityAction>();
            foreach (EntityAction action in entityActions)
            {
                entityActions2.Add(new OfflinableEntityAction(action.Name, action.HasParameters, action.Parameters.ToArray()));
            }
            return entityActions2;
        }

        #endregion
    }
}
