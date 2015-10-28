using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Resources;
using System.Reflection;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DeleteStateValueAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public DeleteStateValueAttribute()
        {
            this.ErrorMessage = Resources.ValidationErrorResources.DeleteStateError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            List<string> members = new List<string>() { validationContext.MemberName };

            if (value != null)
            {
                var type = value.GetType();

                List<string> linkedMembers = CustomValidators.GetLinkedMembers(validationContext);
                if (linkedMembers.Any())
                {
                    members.AddRange(linkedMembers);
                }

                PropertyInfo prop = type.GetProperty("Supprime");
                if (prop != null)
                {
                    bool Result = (bool)prop.GetValue(value, null);
                    switch (Result)
                    {
                        case true:
                            return new ValidationResult(ValidationErrorResources.DeleteStateError, members);
                        case false:
                            return ValidationResult.Success;
                    }
                }
                
                return new ValidationResult(ValidationErrorResources.DeleteStateError, members);
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static Boolean IsLogicalDelete(ValidationContext validationContext)
        {
            var dynamicObject = CustomValidators.GetEntity(validationContext.ObjectInstance);
            if (dynamicObject.EntityState.ToString() == "Deleted")
            {
                return true;
            }

            if (validationContext.ObjectInstance != null)
            {
                var type = validationContext.ObjectInstance.GetType();

                PropertyInfo prop = type.GetProperty("Supprime");
                if (prop != null)
                {
                    bool Result = (bool)prop.GetValue(validationContext.ObjectInstance, null);
                    return Result;
                }
            }
            return false;
        }
    }
}