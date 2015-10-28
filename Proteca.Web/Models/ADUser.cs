using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.DomainServices.Server.ApplicationServices;
using System.Data;

namespace Proteca.Web.Models
{
    public class ADUser
    {
        public string SAMAccountName { get; set; }
        public string SN { get; set; }
        public string Givenname { get; set; }
        public string Email { get; set; }
        public bool IsError { get; set; }

        public ADUser()
        {

        }
    }
}