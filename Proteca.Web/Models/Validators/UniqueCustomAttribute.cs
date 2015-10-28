using System.Collections.Generic;
namespace Proteca.Web.Models
{
    [System.AttributeUsage(System.AttributeTargets.Property)  // multiuse attribute
    ]
    public class Unique : System.Attribute
    {
        public List<string> FilterMemberName { get; private set; }

        public Unique(string filterMemberName)
        {
            FilterMemberName = new List<string>();
            FilterMemberName.Add(filterMemberName);
        }

        public Unique(params string[] filterMemberName)
        {
            FilterMemberName = new List<string>(filterMemberName);
        }

        public Unique()
        {
            FilterMemberName = new List<string>();
        }
    }
}