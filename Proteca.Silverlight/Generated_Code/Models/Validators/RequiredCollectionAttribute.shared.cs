using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequiredCollectionAttribute : ValidationAttribute
    {
        public RequiredCollectionAttribute()
        {
            this.ErrorMessage = ValidationErrorResources.DefaultRequiredFieldErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CustomValidators.IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            var dynamicObject = CustomValidators.GetEntity(validationContext.ObjectInstance);
            if (dynamicObject.EntityState.ToString() == "Detached")
            {
                return ValidationResult.Success;
            }
            if (value is IEnumerable)
            {
                foreach (var element in (IEnumerable)value)
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

            return new ValidationResult(this.ErrorMessage , members);
        }
    }
}