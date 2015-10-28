using System.ComponentModel.DataAnnotations;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EntierPositif : ValidationAttribute
    {
        public Boolean AllowNull { get; private set; }

        public EntierPositif()
        {
            AllowNull = false;
            this.ErrorMessage = ValidationErrorResources.DefaultValidationPositivIntFieldErrorMessage;
        }

        public EntierPositif(Boolean allowNull)
        {
            AllowNull = allowNull;
            this.ErrorMessage = ValidationErrorResources.DefaultValidationPositivIntFieldErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CustomValidators.IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            if ((AllowNull && value == null) || (value != null && value is int && ((int)value) >= 0))
            {
                return ValidationResult.Success;
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