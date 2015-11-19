using System.Collections.Generic;

namespace Offline
{
    /// <summary>
    /// Class to store domain method invocations
    /// </summary>
    public class OfflinableEntityAction
    {
        public OfflinableEntityAction()
        {
        }

        public OfflinableEntityAction(string name, bool hasParams, object[] parameters)
        {
            this.Name = name;
            this.HasParameters = hasParams;
            this.Parameters = parameters;
        }

        public bool HasParameters { get; set; }

        public string Name { get; set; }

        public IEnumerable<object> Parameters { get; set; }
    }
}
