using System.ComponentModel.DataAnnotations;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class StringLengthCustom : StringLengthAttribute
    {
        public StringLengthCustom(int maximumLength)
            : base(maximumLength)
        {
            this.ErrorMessage = ValidationErrorResources.DefaultStringLengthFieldErrorMessage;
        }

        public StringLengthCustom(int maximumLength, int minimumLenght)
            : base(maximumLength)
        {
            MinimumLength = minimumLenght;

            this.ErrorMessage = ValidationErrorResources.StringLengthFieldErrorMessage;
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CustomValidators.IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            ValidationResult vr = base.IsValid(value, validationContext);

            if (vr == null)
            {
                return ValidationResult.Success;
            }
            
            List<string> members = new List<string>(vr.MemberNames);
            List<string> linkedMembers = CustomValidators.GetLinkedMembers(validationContext);
            if (linkedMembers.Any())
            {
                members.AddRange(linkedMembers);
            }

            return new ValidationResult(vr.ErrorMessage, members);
        }
    }
}