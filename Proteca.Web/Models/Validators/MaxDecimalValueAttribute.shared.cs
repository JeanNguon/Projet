using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Resources;
using System.Globalization;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MaxDecimalValueAttribute : ValidationAttribute
    {
        public int MaxIntegerPartSize { get; set; }
        public int MaxDecimalPartSize { get; set; }
        public bool Positive { get; set; }
        public bool PositiveOrZero { get; set; }
        private static CultureInfo culture = CultureInfo.InvariantCulture;//new CultureInfo("en");

        public MaxDecimalValueAttribute()
        {
            this.ErrorMessage = ValidationErrorResources.DefaultDecimalFormatFieldErrorMessage;
        }

        public static decimal GetIntegerPart(decimal value)
        {
            return Decimal.Truncate(value);
        }
        public static int GetIntegerPartSize(decimal value)
        {
            return GetIntegerPart(Math.Abs(value)).ToString(culture).Length;
        }
        public static decimal GetDecimalPart(decimal value)
        {
            var positiveValue = Math.Abs(value);
            return positiveValue - GetIntegerPart(positiveValue);
        }
        public static int GetDecimalPartSize(decimal value)
        {
            var decimalPart = GetDecimalPart(value);
            if (decimalPart != 0m)
            {
                var decimalPartSize = decimalPart.ToString(culture).Length;
                // decimalPart => 0.xxx 
                // we remove '0.' from the length
                return Math.Max(0, decimalPartSize - 2);
            }
            else
                return 0;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            List<string> members = new List<string>() { validationContext.MemberName };
            List<string> linkedMembers = CustomValidators.GetLinkedMembers(validationContext);
            if (linkedMembers.Any())
                members.AddRange(linkedMembers);

            if (value is decimal)
            {
                var decimalValue = (decimal)value;
                if (!(
                    GetIntegerPartSize(decimalValue) <= MaxIntegerPartSize
                    && GetDecimalPartSize(decimalValue) <= MaxDecimalPartSize))
                    return new ValidationResult(String.Format(ValidationErrorResources.DefaultDecimalFormatFieldErrorMessage, MaxIntegerPartSize, MaxDecimalPartSize), members);
                if (PositiveOrZero && decimalValue < 0)
                    return new ValidationResult(ValidationErrorResources.DefaultValidationPositivFieldErrorMessage, members);
                if (Positive && decimalValue <= 0)
                    return new ValidationResult(ValidationErrorResources.DefaultValidationPositivFieldErrorMessage, members);
            }
            return ValidationResult.Success;
        }
    }
}