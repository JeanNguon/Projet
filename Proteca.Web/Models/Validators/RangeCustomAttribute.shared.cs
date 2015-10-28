using System.ComponentModel.DataAnnotations;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RangeCustomAttribute : ValidationAttribute
    {
        public String Maximum { get; set; }
        public String Minimum { get; set; }

		public RangeCustomAttribute()
        {
            this.ErrorMessage = ValidationErrorResources.DefaultValidationRangeFieldErrorMessage;
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
            
            Double dValue = 0;
            Double dMinimum = 0;
            Double dMaximum = 0;
            Boolean bHasValue = value != null && !String.IsNullOrEmpty(value.ToString()) && Double.TryParse(value.ToString(), out dValue);
            Boolean bHasMinimum = !String.IsNullOrEmpty(Minimum) && Double.TryParse(Minimum, NumberStyles.Any, new CultureInfo("en"), out dMinimum);
            Boolean bHasMaximum = !String.IsNullOrEmpty(Maximum) && Double.TryParse(Maximum, NumberStyles.Any, new CultureInfo("en"), out dMaximum);

            if (bHasValue && ((bHasMinimum && dValue >= dMinimum && bHasMaximum && dValue <= dMaximum)
                    || (!bHasMinimum && bHasMaximum && dValue <= dMaximum)
                    || (bHasMinimum && dValue >= dMinimum && !bHasMaximum)))
            {
                return ValidationResult.Success;
            }

            List<string> members = new List<string>() { validationContext.MemberName };
            List<string> linkedMembers = CustomValidators.GetLinkedMembers(validationContext);
            if (linkedMembers.Any())
            {
                members.AddRange(linkedMembers);
            }

            return new ValidationResult(String.Format(this.ErrorMessage, "", Minimum, Maximum), members);
        }
    }
}