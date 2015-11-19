using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Xml;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Helpers
{
    /// <summary>
    /// Classe statique pour les extensions liées à la classe Entity
    /// </summary>
    public static class EntityExtension
    {
        public static bool IsNew(this Entity entity)
        {
            bool result = false;
            if (entity != null && entity.GetIdentity() is int)
            {
                result = ((int) entity.GetIdentity()) == 0;
            }
            return result;
        }

        public static object GetCustomIdentity(this Entity entity)
        {
            object result = null;
            if (entity != null)
            {
                if (Application.Current.IsRunningOutOfBrowser && entity is Composition)
                {
                    result = (entity as Composition).EntityIndex;
                }
                else
                {
                    result = entity.GetIdentity();
                }
            }
            return result;
        }

        public static bool HasChildChanges(this Entity entity)
        {
            bool result = entity.GetHasChildChanges();

            if (!result)
            {
                PropertyInfo[] myPropertyInfo =
                    entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in myPropertyInfo.Where(p => p.PropertyType.BaseType == typeof (Entity)))
                {
                    var childEntity = prop.GetValue(entity, null) as Entity;
                    if (childEntity != null)
                    {
                        result = childEntity.HasChanges;

                        if (result)
                        {
                            break;
                        }
                    }
                }

                foreach (
                    var prop in
                        myPropertyInfo.Where(
                            p =>
                                p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof (EntityCollection<>)))
                {
                    var childEntities = prop.GetValue(entity, null) as IEnumerable;
                    if (childEntities != null)
                    {
                        foreach (var childEntity in childEntities)
                        {
                            if (childEntity.GetType().BaseType == typeof (Entity))
                            {
                                result = result || ((Entity) childEntity).HasChanges || ((Entity) childEntity).IsNew();

                                if (result)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Indique si les objets enfants ont des erreurs de validations
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool HasChildErrors(this Entity entity)
        {
            bool result = false;
            PropertyInfo[] myPropertyInfo = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in myPropertyInfo.Where(p => p.PropertyType.BaseType == typeof (Entity)))
            {
                var childEntity = prop.GetValue(entity, null) as Entity;
                if (childEntity != null)
                {
                    result = result || childEntity.HasValidationErrors;
                }
            }

            foreach (
                var prop in
                    myPropertyInfo.Where(
                        p =>
                            p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof (EntityCollection<>)))
            {
                var childEntities = prop.GetValue(entity, null) as IEnumerable;
                if (childEntities != null)
                {
                    foreach (var childEntity in childEntities)
                    {
                        if (childEntity.GetType().BaseType == typeof (Entity))
                        {
                            result = result || ((Entity) childEntity).HasValidationErrors;
                        }
                    }
                }
            }

            return result;
        }

        public static void ActivateChangePropagation(this Entity entity)
        {
            if (entity.HasRaiseAnyPropertyChangedMethod())
            {
                PropertyInfo[] myPropertyInfo =
                    entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (
                    var prop in
                        myPropertyInfo.Where(
                            p =>
                                p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof (EntityCollection<>)))
                {
                    var childEntities = prop.GetValue(entity, null) as IEnumerable;
                    if (childEntities != null)
                    {
                        foreach (var childEntity in childEntities)
                        {
                            if (childEntity.GetType().BaseType == typeof (Entity) &&
                                ((Entity) childEntity).HasGetParentWithPropNameMethod())
                            {
                                var theEntity = childEntity as Entity;

                                theEntity.PropertyChanged -= new PropertyChangedEventHandler(theEntity_PropertyChanged);
                                theEntity.PropertyChanged += new PropertyChangedEventHandler(theEntity_PropertyChanged);
                            }
                        }
                    }
                }
            }
            if (entity.HasRaiseAnyDataMemberChangedMethod())
            {
                PropertyInfo[] myPropertyInfo =
                    entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (
                    var prop in
                        myPropertyInfo.Where(
                            p =>
                                p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof (EntityCollection<>)))
                {
                    MethodInfo MI = typeof (EntityExtension).GetMethod("RegisterEntityEvent");

                    MethodInfo genericMethod =
                        MI.MakeGenericMethod(new[] {prop.PropertyType.GetGenericArguments().First()});
                    genericMethod.Invoke(null, new[] {prop.GetValue(entity, null), entity, prop.Name});
                }
            }
        }

        public static void RegisterEntityEvent<T>(EntityCollection<T> entityCol, Entity parent, String propName)
            where T : Entity
        {
            entityCol.EntityAdded += (o, e) => { parent.RaiseAnyDataMemberChanged(propName); };

            entityCol.EntityRemoved += (o, e) => { parent.RaiseAnyDataMemberChanged(propName); };
        }

        private static void theEntity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var changingPropery = sender.GetType().GetProperty(e.PropertyName);

            // EP : Activation de la propagation également lorsque la propriété n'est pas une entitée

            Dictionary<string, Entity> entityWithPropName = ((Entity) sender).GetParentWithPropName();
            foreach (String proName in entityWithPropName.Keys)
            {
                if (entityWithPropName[proName] != null)
                {
                    entityWithPropName[proName].RaiseAnyPropertyChanged(proName);
                }
            }
        }

        public static void RaiseAnyPropertyChanged(this Entity entity, String propName)
        {
            var raiseMethod = entity.GetType().GetMethod("RaiseAnyPropertyChanged");
            if (raiseMethod != null)
            {
                raiseMethod.Invoke(entity, new object[1] {propName});
            }
        }

        public static bool HasRaiseAnyPropertyChangedMethod(this Entity entity)
        {
            if (entity != null)
            {
                return entity.GetType().GetMethod("RaiseAnyPropertyChanged") != null;
            }
            return false;
        }

        public static void RaiseAnyDataMemberChanged(this Entity entity, String propName)
        {
            var raiseMethod = entity.GetType().GetMethod("RaiseAnyDataMemberChanged");
            if (raiseMethod != null)
            {
                raiseMethod.Invoke(entity, new object[1] {propName});
            }
        }

        public static bool HasRaiseAnyDataMemberChangedMethod(this Entity entity)
        {
            if (entity != null)
            {
                return entity.GetType().GetMethod("RaiseAnyDataMemberChanged") != null;
            }
            return false;
        }

        public static Dictionary<string, Entity> GetParentWithPropName(this Entity entity)
        {
            var raiseMethod = entity.GetType().GetMethod("GetParentWithPropName");
            if (raiseMethod != null)
            {
                return (Dictionary<string, Entity>) raiseMethod.Invoke(entity, null);
            }

            return new Dictionary<string, Entity>();
        }

        public static bool HasGetParentWithPropNameMethod(this Entity entity)
        {
            if (entity != null)
            {
                return entity.GetType().GetMethod("GetParentWithPropName") != null;
            }
            return false;
        }

        public static bool GetHasChildChanges(this Entity entity)
        {
            if (entity != null)
            {
                PropertyInfo pi = entity.GetType().GetProperty("HasChildChanges");
                return pi != null && (bool) pi.GetValue(entity, null);
            }
            return false;
        }

        public static List<String> GetChildChangesPropertyNames(this Entity entity)
        {
            if (entity != null)
            {
                PropertyInfo pi = entity.GetType().GetProperty("ChildChanges");
                if (pi != null && pi.PropertyType == typeof (List<String>))
                {
                    return (List<String>) pi.GetValue(entity, null);
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// Supprime les modifications apportées à l'entité
        /// </summary>
        /// <param name="param">Entité concernée</param>
        public static void RejectChanges(this Entity entity)
        {
            ((IRevertibleChangeTracking) entity).RejectChanges();
        }

        /// <summary>
        /// Supprime les modifications apportées à l'entité en cascade
        /// </summary>
        /// <param name="entity">Entité concernée</param>
        /// <param name="depthLevel">niveau de profondeur (-1 = infini)</param>
        public static void RejectCascadeChanges(this Entity entity, int depthLevel)
        {
            if (depthLevel != 0)
            {
                PropertyInfo[] myPropertyInfo =
                    entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in myPropertyInfo)
                {
                    var child = prop.GetValue(entity, null);

                    if (child != null && child.GetType().IsGenericType &&
                        child.GetType().GetGenericTypeDefinition() == typeof (EntityCollection<>))
                    {
                        var subEntities = child as IEnumerable;
                        foreach (var subEntity in subEntities)
                        {
                            if (subEntity is Entity)
                            {
                                if ((subEntity as Entity).IsNew())
                                {
                                    MethodInfo MI =
                                        typeof (EntityExtension).GetMethod("removeEntityFromEntityCollection");

                                    MethodInfo genericMethod = MI.MakeGenericMethod(new[] {subEntity.GetType()});
                                    genericMethod.Invoke(null, new[] {child, subEntity});
                                }
                                else
                                {
                                    (subEntity as Entity).RejectCascadeChanges(depthLevel - 1);
                                }
                            }
                        }
                    }
                }
            }

            entity.RejectChanges();
        }

        public static void removeEntityFromEntityCollection<T>(EntityCollection<T> coll, T entity) where T : Entity
        {
            coll.Remove(entity);
        }

        #region CLone et Serialisation

        public static T Clone<T>(this T source)
        {
            var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof (T));
            using (var ms = new System.IO.MemoryStream())
            {
                dcs.WriteObject(ms, source);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return (T) dcs.ReadObject(ms);
            }
        }

        public static String Serialize<T>(this T source)
        {
            String retourXML = String.Empty;
            DataContractSerializer serializer = new DataContractSerializer(typeof (T));

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            XmlWriter xmlWriter = XmlWriter.Create(sb);

            serializer.WriteObject(xmlWriter, source);

            retourXML = sb.ToString();

            xmlWriter.Close();

            return retourXML;
        }

        #endregion
    }
}