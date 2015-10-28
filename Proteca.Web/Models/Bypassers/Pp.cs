using System;
using System.Runtime.Serialization;

namespace Proteca.Web.Models
{
    public partial class Pp
    {
        [DataMember]
        public Boolean BypassCategoriePp { get; set; }

        [DataMember]
        public Boolean BypassPkLimitation { get; set; }
    }
}