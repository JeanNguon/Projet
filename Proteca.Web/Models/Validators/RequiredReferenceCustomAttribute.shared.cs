using System;
using System.Collections.Generic;
namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=true)]
    public class RequiredReferenceAttribute : System.Attribute
    {
        private string _memberName;
        public string MemberName
        {
            get { return _memberName; }
            set { _memberName=value; }
        }

        public RequiredReferenceAttribute(string member)
        {
            _memberName = member;
        }
    }
}