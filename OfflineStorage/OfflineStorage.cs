using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;

namespace Offline
{
    /// <summary>
    /// Class to help with storing and restoring <see cref="DomainContext"/> from IsoStorage
    /// </summary>
    public static class OfflineStorage
    {
        #region Properties

        /// <summary>
        /// Default name of the cache file into IsoStore
        /// </summary>
        private readonly static string _fileName = "store.json";

        public static string FileName
        {
            get
            {
                return _fileName;
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// Serialize and save <see cref="DomainContext"/> into a file in the IsoStore
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> to be saved</param>
        /// <param name="fileName">Name of the file container</param>
        /// <returns>Content saved</returns>
        public static string SaveToIsoStore(this DomainContext domainContext, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || domainContext == null)
                throw new ArgumentNullException();

            string jsonData = SerializeContext(domainContext);
            return IsoStorageUtil.SaveData(fileName, jsonData);
        }

        /// <summary>
        /// Serialize and save <see cref="DomainContext"/> into the default file in the IsoStore
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> to be saved</param>
        /// <returns>Content saved</returns>
        public static string SaveToIsoStore(this DomainContext domainContext)
        {
            return SaveToIsoStore(domainContext, _fileName);
        }

        /// <summary>
        /// Save content into a file in the IsoStore
        /// </summary>
        /// <param name="jsonData">Content to be saved</param>
        /// <param name="fileName">Name of the file container</param>
        /// <returns>Content saved</returns>
        public static string SaveToIsoStore(string jsonData, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(jsonData))
                throw new ArgumentNullException();

            return IsoStorageUtil.SaveData(fileName, jsonData);
        }

        /// <summary>
        /// Save content into the default file in the IsoStore
        /// </summary>
        /// <param name="jsonData">Content to be saved</param>
        /// <returns>Content saved</returns>
        public static string SaveToIsoStore(string jsonData)
        {
            return SaveToIsoStore(jsonData, _fileName);
        }

        #endregion

        #region Load

        /// <summary>
        /// Takes existing instance of <see cref="DomainContext"/> and loads saved data into it in a file inside IsoStore
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> target for the load</param>
        /// <param name="fileName">Name of the target json file</param>
        /// <returns>Whether or not the load happen correctly</returns>
        public static bool LoadFromIsoStore(this DomainContext domainContext, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || domainContext == null)
                throw new ArgumentNullException();

            if (IsoStorageUtil.FileExists(fileName))
            {
                RestoreInternal2(domainContext, fileName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Takes existing instance of <see cref="DomainContext"/> and loads saved data into it in the default file inside IsoStore
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> target for the load</param>
        /// <returns>Whether or not the load happen correctly</returns>
        public static bool LoadFromIsoStore(this DomainContext domainContext)
        {
            return LoadFromIsoStore(domainContext, _fileName);
        }

        /// <summary>
        /// Loads saved data from a file of the IsoStore
        /// </summary>
        /// <param name="fileName">Name of the target json file</param>
        /// <returns>Json formated <see cref="DomainContext"/></returns>
        public static string ExtractIsoStore(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException();

            return IsoStorageUtil.LoadData(fileName);
        }

        /// <summary>
        /// Loads saved data from the default file of the IsoStore
        /// </summary>
        /// <returns>Json formated <see cref="DomainContext"/></returns>
        public static string ExtractIsoStore()
        {
            return ExtractIsoStore(_fileName);
        }

        /// <summary>
        /// Takes existing instance of <see cref="DomainContext"/> and restores it to the state it was during Save
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> to be restored</param>
        /// <param name="fileName">Name of the target json file</param>
        /// <returns>Whether or not the restore happen correctly</returns>
        public static bool RestoreFromIsoStore(this DomainContext domainContext, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || domainContext == null)
                throw new ArgumentNullException();

            if (IsoStorageUtil.FileExists(fileName))
            {
                domainContext.EntityContainer.Clear();
                RestoreInternal2(domainContext, fileName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Takes existing instance of <see cref="DomainContext"/> and restores it to the state it was during Save
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> to be restored</param>
        /// <returns>Whether or not the restore happen correctly</returns>
        public static bool RestoreFromIsoStore(this DomainContext domainContext)
        {
            return RestoreFromIsoStore(domainContext, _fileName);
        }

        /// <summary>
        /// Restore <see cref="DomainContext"/> from a file in IsoStore, entitySet by entitySet
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> to be restored</param>
        /// <param name="fileName">Name of the target json file</param>
        private static void RestoreInternal(DomainContext domainContext, string fileName)
        {
            string data = IsoStorageUtil.LoadData(fileName);

            List<List<OfflinableEntity>> offlineEntityLists = JsonUtil.DeserializeList(data);

            //Add or Attach the entites back
            foreach (List<OfflinableEntity> list in offlineEntityLists)
            {
                OfflinableEntity oE = list.FirstOrDefault();
                if (oE == null)
                    continue;

                EntitySet entitySet = GetEntitySetForOfflinableEntity(domainContext, oE);

                foreach (OfflinableEntity offEntity in list)
                {
                    if (offEntity.EntityState == EntityState.New)
                    {
                        MethodInfo addMethod = entitySet.GetType().GetMethod("Add");
                        addMethod.Invoke(entitySet, new object[] { offEntity.CurrentEntity });
                    }
                    else
                    {
                        if (offEntity.OriginalEntity != null)
                            entitySet.Attach(offEntity.OriginalEntity);
                        else
                            entitySet.Attach(offEntity.CurrentEntity);
                    }
                }
            }

            //Playback updates, domain method invocations and deletes
            PlayBackChanges(domainContext, offlineEntityLists);
        }

        /// <summary>
        /// Restore <see cref="DomainContext"/> from a file in IsoStore, entitySet by entitySet
        /// Slight variation that merges changes with any entities that may already exist in the context
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> to be restored</param>
        /// <param name="fileName">Name of the target json file</param>
        private static void RestoreInternal2(DomainContext domainContext, string fileName)
        {
            string data = IsoStorageUtil.LoadData(fileName);

            RestoreContext(domainContext, data);
        }

        #endregion

        #region DomainContext handling

        /// <summary>
        /// Serialize <see cref="DomainContext"/> to a Json formatted string
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> content to be serialized</param>
        /// <returns>Json formatted <see cref="DomainContext"/></returns>
        public static string SerializeContext(this DomainContext domainContext)
        {
            IEnumerable<Type> entityTypes = GetEntityTypesInContext(domainContext.GetType());
            EntityChangeSet changeSet = domainContext.EntityContainer.GetChanges();

            List<List<OfflinableEntity>> offlineEntityLists = new List<List<OfflinableEntity>>();

            foreach (Type entityType in entityTypes)
            {
                EntitySet entitySet = domainContext.EntityContainer.GetEntitySet(entityType);

                List<OfflinableEntity> offlineEntityList = new List<OfflinableEntity>();
                foreach (Entity entity in entitySet)
                {
                    OfflinableEntity offEntity = new OfflinableEntity(entity, changeSet.AddedEntities.Contains(entity));
                    offlineEntityList.Add(offEntity);
                }

                //Get deleted entities from changeset
                IEnumerable<Entity> removedEntites = changeSet.RemovedEntities.Where(x => x.GetType().Equals(entityType));

                //Need to save deleted entities as well
                foreach (Entity entity in removedEntites)
                {
                    OfflinableEntity offEntity = new OfflinableEntity(entity);
                    offlineEntityList.Add(offEntity);
                }

                offlineEntityLists.Add(offlineEntityList);
            }

            string jsonData = JsonUtil.SerializeList(offlineEntityLists);
            return jsonData;
        }

        ///// <summary>
        ///// Restore <see cref="DomainContext"/> from a Json formatted string
        ///// </summary>
        ///// <param name="domainContext"><see cref="DomainContext"/> to be restored</param>
        ///// <param name="data">Json formatted content</param>
        //public static void RestoreContext(this DomainContext domainContext, string data)
        //{
        //    List<List<OfflinableEntity>> offlineEntityLists = JsonUtil.DeserializeList(data);

        //    //Add or Attach the entites back
        //    foreach (List<OfflinableEntity> list in offlineEntityLists)
        //    {
        //        OfflinableEntity oE = list.FirstOrDefault();
        //        if (oE == null)
        //            continue;

        //        EntitySet entitySet = GetEntitySetForOfflinableEntity(domainContext, oE);

        //        foreach (OfflinableEntity offEntity in list)
        //        {
        //            if (offEntity.EntityState == EntityState.New)
        //            {
        //                MethodInfo addMethod;
        //                if (offEntity.EntityRefs != null)
        //                {
        //                    foreach (var entities in offEntity.EntityRefs)
        //                    {
        //                        if (entities.Value != null && entities.Value.Any())
        //                        {
        //                            addMethod = typeof(EntityCollection<>).MakeGenericType(entities.Value.First().GetType()).GetMethod("Add");
        //                            if (addMethod != null)
        //                            {
        //                                var prop = offEntity.CurrentEntity.GetType().GetProperty(entities.Key);
        //                                if (prop != null)
        //                                {
        //                                    var collection = prop.GetValue(offEntity.CurrentEntity, null);
        //                                    if (collection != null)
        //                                    {
        //                                        foreach (var entity in entities.Value)
        //                                        {
        //                                            addMethod.Invoke(collection, new object[] { entity });
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                foreach (var entity in offEntity.CurrentEntity.GetType().GetProperties().Where(p => p.CanWrite && p.PropertyType.BaseType == typeof(Entity)))
        //                {
        //                    object[] attrs = entity.GetCustomAttributes(typeof(AssociationAttribute), true);
        //                    if (attrs.Length > 0)
        //                    {
        //                        AssociationAttribute AssocAttr = (AssociationAttribute)attrs.FirstOrDefault(a => a is AssociationAttribute);

        //                        var entityVal = entity.GetValue(offEntity.CurrentEntity, null);
        //                        // Correctif pour ajouter les nouveaux objets à des entités déjà existantes
        //                        if (entityVal == null)
        //                        {
        //                            // Récupération de la clé correspondante
        //                            var requiredReferenceType = Assembly.GetCallingAssembly().GetType("Proteca.Web.Models.RequiredReferenceAttribute");
        //                            var referenceAttributesProperties = offEntity.CurrentEntity.GetType().GetProperties()
        //                                                                      .Select(p => new { Property = p, ReferenceAttribute = p.GetCustomAttributes(requiredReferenceType, false).FirstOrDefault() }).ToList();
        //                            var cle = referenceAttributesProperties.FirstOrDefault(p => p.ReferenceAttribute != null && p.ReferenceAttribute.GetType().GetProperty("MemberName").GetValue(p.ReferenceAttribute, null).ToString() == entity.Name);
        //                            var es = domainContext.EntityContainer.GetEntitySet(entity.PropertyType);
        //                           //Récupération de l'entity dont la clé est "cle" dans le domaincontext.
        //                            entityVal = es.Cast<Entity>().Where(e => cle != null && e.GetIdentity().ToString() == "" + cle.Property.GetValue(offEntity.CurrentEntity, null)).FirstOrDefault();
        //                        }
        //                        if (entityVal != null)
        //                        {
        //                            foreach (var collection in entityVal.GetType().GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>) && p.PropertyType.GetGenericArguments()[0] == offEntity.CurrentEntity.GetType()))
        //                            {
        //                                attrs = collection.GetCustomAttributes(typeof(AssociationAttribute), true);
        //                                if (attrs.Length > 0)
        //                                {
        //                                    AssociationAttribute ColAssocAttr = (AssociationAttribute)attrs.FirstOrDefault(a => a is AssociationAttribute);
        //                                    if (ColAssocAttr.Name == AssocAttr.Name)
        //                                    {
        //                                        var collectionVal = collection.GetValue(entityVal, null);
        //                                        addMethod = typeof(EntityCollection<>).MakeGenericType(offEntity.CurrentEntity.GetType()).GetMethod("Add");
        //                                        if (addMethod != null)
        //                                        {
        //                                            addMethod.Invoke(collectionVal, new object[] { offEntity.CurrentEntity });
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                entitySet.Add(offEntity.CurrentEntity);
        //                //addMethod = entitySet.GetType().GetMethods().Where(m => m.Name == "Add").First();
        //                //addMethod.Invoke(entitySet, new object[] { offEntity.CurrentEntity });
        //            }
        //            else
        //            {
        //                if (offEntity.OriginalEntity != null)
        //                    entitySet.Attach(offEntity.OriginalEntity);
        //                else
        //                    entitySet.Attach(offEntity.CurrentEntity);
        //            }
        //        }
        //    }
        //    //Playback updates, domain method invocations and deletes
        //    PlayBackChanges(domainContext, offlineEntityLists);
        //}

        /// <summary>
        /// Restore <see cref="DomainContext"/> from a Json formatted string
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> to be restored</param>
        /// <param name="data">Json formatted content</param>
        public static void RestoreContext(this DomainContext domainContext, string data)
        {
            List<List<OfflinableEntity>> offlineEntityLists = JsonUtil.DeserializeList(data);

            //Add or Attach the entites back
            foreach (List<OfflinableEntity> list in offlineEntityLists)
            {
                OfflinableEntity oE = list.FirstOrDefault();
                if (oE == null)
                    continue;

                EntitySet entitySet = GetEntitySetForOfflinableEntity(domainContext, oE);

                foreach (OfflinableEntity offEntity in list)
                {
                    if (offEntity.EntityState == EntityState.New)
                        entitySet.Add(offEntity.CurrentEntity);
                    else if (offEntity.OriginalEntity != null)
                        entitySet.Attach(offEntity.OriginalEntity);
                    else
                        entitySet.Attach(offEntity.CurrentEntity);
                }
            }
            //link added entities back
            foreach (List<OfflinableEntity> list in offlineEntityLists)
            {
                OfflinableEntity oE = list.FirstOrDefault();
                if (oE == null)
                    continue;

                foreach (OfflinableEntity offEntity in list)
                {
                    if (offEntity.EntityState == EntityState.New)
                    {
                        MethodInfo addMethod;
                        if (offEntity.EntityRefs != null)
                        {
                            foreach (var entities in offEntity.EntityRefs)
                            {
                                if (entities.Value != null && entities.Value.Any())
                                {
                                    addMethod = typeof(EntityCollection<>).MakeGenericType(entities.Value.First().GetType()).GetMethod("Add");
                                    if (addMethod != null)
                                    {
                                        var prop = offEntity.CurrentEntity.GetType().GetProperty(entities.Key);
                                        if (prop != null)
                                        {
                                            var collection = prop.GetValue(offEntity.CurrentEntity, null);
                                            if (collection != null)
                                            {
                                                if (entities.Value != null && entities.Value.Any())
                                                {
                                                    //MANTIS-21144 - Correction pour éviter le plantage lorsqu'une entité est déjà présente dans le contexte
                                                    // On récupère l'entitySet correspondant à l'entité à ajouter à la collection
                                                    var entitySet = GetEntitySetForOfflinableEntity(domainContext, entities.Value.First().GetType());

                                                    foreach (var entity in entities.Value)
                                                    {
                                                        // Si l'entité n'est pas nouvelle et qu'elle existe déjà dans le contexte on utilise cette référence
                                                        var existingEntity = entity.GetIdentity().Equals(0) ? null :
                                                            entitySet.Cast<Entity>()
                                                                .FirstOrDefault(
                                                                    en => en.GetIdentity().Equals(entity.GetIdentity()));
                                                        if (existingEntity != null)
                                                        {
                                                            addMethod.Invoke(collection, new object[] { existingEntity });
                                                        }
                                                        else
                                                        {
                                                            addMethod.Invoke(collection, new object[] { entity });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        foreach (var entity in offEntity.CurrentEntity.GetType().GetProperties().Where(p => p.CanWrite && p.PropertyType.BaseType == typeof(Entity)))
                        {
                            object[] attrs = entity.GetCustomAttributes(typeof(AssociationAttribute), true);
                            if (attrs.Length > 0)
                            {
                                AssociationAttribute AssocAttr = (AssociationAttribute)attrs.FirstOrDefault(a => a is AssociationAttribute);

                                var entityVal = entity.GetValue(offEntity.CurrentEntity, null);
                                // Correctif pour ajouter les nouveaux objets à des entités déjà existantes
                                if (entityVal == null)
                                {
                                    // Récupération de la clé correspondante
                                    var requiredReferenceType = Assembly.GetCallingAssembly().GetType("Proteca.Web.Models.RequiredReferenceAttribute");
                                    var referenceAttributesProperties = offEntity.CurrentEntity.GetType().GetProperties()
                                                                              .Select(p => new { Property = p, ReferenceAttribute = p.GetCustomAttributes(requiredReferenceType, false).FirstOrDefault() }).ToList();
                                    var cle = referenceAttributesProperties.FirstOrDefault(p => p.ReferenceAttribute != null && p.ReferenceAttribute.GetType().GetProperty("MemberName").GetValue(p.ReferenceAttribute, null).ToString() == entity.Name);
                                    var es = domainContext.EntityContainer.GetEntitySet(entity.PropertyType);
                                    //Récupération de l'entity dont la clé est "cle" dans le domaincontext.
                                    entityVal = es.Cast<Entity>().FirstOrDefault(e => cle != null && e.GetIdentity().ToString() == "" + cle.Property.GetValue(offEntity.CurrentEntity, null));
                                }
                                if (entityVal != null)
                                {
                                    foreach (var collection in entityVal.GetType().GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>) && p.PropertyType.GetGenericArguments()[0] == offEntity.CurrentEntity.GetType()))
                                    {
                                        attrs = collection.GetCustomAttributes(typeof(AssociationAttribute), true);
                                        if (attrs.Length > 0)
                                        {
                                            AssociationAttribute ColAssocAttr = (AssociationAttribute)attrs.FirstOrDefault(a => a is AssociationAttribute);
                                            if (ColAssocAttr.Name == AssocAttr.Name)
                                            {
                                                var collectionVal = collection.GetValue(entityVal, null);
                                                addMethod = typeof(EntityCollection<>).MakeGenericType(offEntity.CurrentEntity.GetType()).GetMethod("Add");
                                                if (addMethod != null)
                                                {
                                                    addMethod.Invoke(collectionVal, new object[] { offEntity.CurrentEntity });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            //Playback updates, domain method invocations and deletes
            PlayBackChanges(domainContext, offlineEntityLists);
        }

        /// <summary>
        /// Reinitialize <see cref="DomainContext"/> to the state it was at load
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> to be reinitialized</param>
        /// <param name="entities"><see cref="DomainContext"/> as list of entitySets</param>
        private static void PlayBackChanges(DomainContext domainContext, List<List<OfflinableEntity>> entities)
        {
            foreach (List<OfflinableEntity> oList in entities)
            {
                OfflinableEntity oE = oList.FirstOrDefault();
                if (oE == null)
                    continue;

                EntitySet entitySet = GetEntitySetForOfflinableEntity(domainContext, oE);

                foreach (OfflinableEntity offEntity in oList)
                {
                    switch (offEntity.EntityState)
                    {
                        case EntityState.Deleted:
                            //play back entity deletes
                            Entity delEntity = entitySet.Cast<Entity>().Single(x => x.GetIdentity().Equals(offEntity.CurrentEntity.GetIdentity()));
                            MethodInfo removeMethod = entitySet.GetType().GetMethods().FirstOrDefault(m => m.Name == "Remove");
                            removeMethod.Invoke(entitySet, new object[] { delEntity });

                            break;
                        case EntityState.Modified:

                            Entity activeEntity = offEntity.CurrentEntity;
                            //play back entity updates if any
                            if (offEntity.OriginalEntity != null)
                            {
                                UpdateEntity(offEntity.CurrentEntity, offEntity.OriginalEntity);
                                activeEntity = offEntity.OriginalEntity;
                            }

                            //if there are domain method invocations, need to play that back too
                            if (offEntity.EntityActions != null && offEntity.EntityActions.Any())
                            {
                                foreach (OfflinableEntityAction eAction in offEntity.EntityActions)
                                {
                                    MethodInfo domainMethod = activeEntity.GetType().GetMethod(eAction.Name);
                                    var dmParams = eAction.HasParameters == true ? eAction.Parameters.ToArray() : null;
                                    domainMethod.Invoke(activeEntity, dmParams);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        #region Entity handling

        /// <summary>
        /// Check if the <see cref="Entity"/> has been changed and update it if it is the case
        /// </summary>
        /// <param name="current"><see cref="Entity"/> currently in use</param>
        /// <param name="original"><see cref="Entity"/> orriginally loaded</param>
        private static void UpdateEntity(Entity current, Entity original)
        {
            Type entityType = current.GetType();
            StreamingContext context = new StreamingContext(StreamingContextStates.Persistence);
            original.OnDeserializing(context);

            try
            {
                PropertyInfo[] props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo prop in props)
                {
                    Object[] readOnly = prop.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
                    Object[] dataMember = prop.GetCustomAttributes(typeof(DataMemberAttribute), true);
                    if (readOnly.Length == 0 && prop.CanWrite && dataMember.Length > 0)
                    {
                        object newValue = prop.GetValue(current, null);
                        prop.SetValue(original, newValue, null);
                    }
                }
            }
            finally
            {
                original.OnDeserialized(context);
            }
        }

        /// <summary>
        /// Get <see cref="EntitySet"/> of <see cref="Entity"/> for offline use
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> source</param>
        /// <param name="offEntity"><see cref="OfflinableEntity"/> to select the type of <see cref="EntitySet"/></param>
        /// <returns><see cref="EntitySet"/> claimed</returns>
        private static EntitySet GetEntitySetForOfflinableEntity(DomainContext domainContext, OfflinableEntity offEntity)
        {
            Type entityType = offEntity.CurrentEntity.GetType();
            return GetEntitySetForOfflinableEntity(domainContext, entityType);
        }

        /// <summary>
        /// Get <see cref="EntitySet"/> of <see cref="Entity"/> for offline use
        /// </summary>
        /// <param name="domainContext"><see cref="DomainContext"/> source</param>
        /// <param name="entityType">the type of <see cref="EntitySet"/></param>
        /// <returns><see cref="EntitySet"/> claimed</returns>
        private static EntitySet GetEntitySetForOfflinableEntity(DomainContext domainContext, Type entityType)
        {

            while (entityType.BaseType.Name != "Entity")
            {
                entityType = entityType.BaseType;
            }

            //Get entitylist for entityType
            MethodInfo methodInfo = domainContext.EntityContainer.GetType().GetMethod("GetEntitySet", Type.EmptyTypes);
            Type[] genericArguments = new Type[] { entityType };
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
            EntitySet entitySet = (EntitySet)genericMethodInfo.Invoke(domainContext.EntityContainer, null);
            return entitySet;
        }

        /// <summary>
        /// Gets list of entity types exposed in the <see cref="DomainContext"/>
        /// </summary>
        /// <param name="domainContextType"><see cref="DomainContext"/> derived Type</param>
        /// <returns><see cref="IEnumerable"/> of <see cref="Type"/> contained in the <see cref="DomainContext"/></returns>
        private static IEnumerable<Type> GetEntityTypesInContext(Type domainContextType)
        {
            IEnumerable<PropertyInfo> entityListProps = domainContextType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => typeof(EntitySet).IsAssignableFrom(p.PropertyType));
            IEnumerable<Type> entityTypes = entityListProps.Select(e => e.PropertyType.GetGenericArguments()[0]);

            return entityTypes;
        }
        #endregion
    }
}
