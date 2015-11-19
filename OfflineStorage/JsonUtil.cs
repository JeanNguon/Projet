using System.Collections.Generic;
using Newtonsoft.Json;

namespace Offline
{
    /// <summary>
    /// Class to help with Json serialization/deserialization of EntityLists.
    /// </summary>
    internal class JsonUtil
    {
        /// <summary>
        /// Serialize EntityLists to Json
        /// </summary>
        /// <param name="list">Entity lists offlinable</param>
        /// <returns>Json-serialized lists of entities</returns>
        internal static string SerializeList(List<List<OfflinableEntity>> list)
        {
            return JsonConvert.SerializeObject(list, Formatting.None, new JsonSerializerSettings
             {
                 PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                 TypeNameHandling = TypeNameHandling.Auto
             });
        }

        /// <summary>
        /// Deserialize Json to EntityLists
        /// </summary>
        /// <param name="json">Json-serialized lists of entities</param>
        /// <returns>Entity lists offlinable</returns>
        internal static List<List<OfflinableEntity>> DeserializeList(string json)
        {
            return JsonConvert.DeserializeObject<List<List<OfflinableEntity>>>(json, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                Binder = new TypeBinder()
            });
        }
    }
}