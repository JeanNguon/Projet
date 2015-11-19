using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DateTimeRequiredAttribute : ValidationAttribute
    {
        public DateTimeRequiredAttribute()
        {
            this.ErrorMessage = ValidationErrorResources.DefaultRequiredFieldErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CustomValidators.IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }

            DateTime date = DateTime.MinValue;
            if (value != null && DateTime.TryParse(value.ToString(), out date))
            {
                if (date > DateTime.MinValue)
                {
                    return ValidationResult.Success;
                }
            }
            
            List<string> members = new List<string>() { validationContext.MemberName };
            List<string> linkedMembers = CustomValidators.GetLinkedMembers(validationContext);
            if (linkedMembers.Any())
            {
                members.AddRange(linkedMembers);
            }

            return new ValidationResult(this.ErrorMessage, members);

        }
    }
}