using System.ComponentModel.DataAnnotations;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Proteca.Web.Resources;
using System.Text.RegularExpressions;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EmailCustomAttribute : ValidationAttribute
    {
        public EmailCustomAttribute()
        {
            this.ErrorMessage = ValidationErrorResources.DefaultEmailFieldErrorMessage;
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CustomValidators.IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            //création de la regex (cf: http://regexhero.net)
            string strRegex = @"^((?("""")("""".+?""""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))|)$";
            Regex regex = new Regex(strRegex, RegexOptions.None);

            if (value is String && regex.IsMatch(value.ToString()))
            {
                return ValidationResult.Success;
            }
            
            List<string> members = new List<string>();
            members.Add(validationContext.MemberName);
            List<string> linkedMembers = CustomValidators.GetLinkedMembers(validationContext);
            if (linkedMembers.Any())
            {
                members.AddRange(linkedMembers);
            }

            return new ValidationResult(this.ErrorMessage, members);
        }
    }
}