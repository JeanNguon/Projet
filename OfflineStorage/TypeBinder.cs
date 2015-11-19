using System;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;

namespace Offline
{
    public class TypeBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;
            AppDomain currentDomain = AppDomain.CurrentDomain;

            Assembly assem = currentDomain.GetAssemblies().Where(a => a.FullName.StartsWith(assemblyName)).FirstOrDefault();

            if (assem != null)
            {
                typeToDeserialize = assem.GetTypes().Where(t => t.FullName == typeName).FirstOrDefault();
            }

            return typeToDeserialize;
        }
    }
}