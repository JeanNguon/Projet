using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Proteca.Web.Resources;
using System.Linq;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequiredCustomAction : ValidationAttribute
    {
        public Boolean AllowZero { get; set; }

        public RequiredCustomAction()
            : base()
        {
            this.ErrorMessage = ValidationErrorResources.DefaultRequiredFieldErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var action = validationContext.ObjectInstance as AnAction;
            List<string> members = new List<string> { validationContext.MemberName };

            if (action != null)
            {
                if (CustomValidators.IsLogicalDelete(validationContext))
                {
                    return ValidationResult.Success;
                }

#if SILVERLIGHT
                 if (action.EntityState != System.ServiceModel.DomainServices.Client.EntityState.Unmodified && action.EntityState!= System.ServiceModel.DomainServices.Client.EntityState.Detached)
                {
                    dynamic actClient = action;

                    if (validationContext.MemberName == "CleUtilisateurAgent" && ((int)value == 0 || value == null))
                    {
                        return new ValidationResult(this.FormatErrorMessage(this.ErrorMessage), members);
                    }
                    if (action.IsRequiredField(validationContext.MemberName, AnAction.CurrentUser))
                    {
                        if (   (value is int && ((AllowZero && (int)value == 0) || (int)value != 0)) 
                            || (value is double && (double)value != 0)
                            || (value is string && !string.IsNullOrWhiteSpace((string)value))
                            || (value != null && !(value is string) && (!(value is IEnumerable<PortionIntegriteAnAction>) || ((IEnumerable<PortionIntegriteAnAction>)value).Any())))
                        {
                            return ValidationResult.Success;
                        }
                        else {
                            return new ValidationResult(this.FormatErrorMessage(this.ErrorMessage), members);
                        }
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
#else
                return ValidationResult.Success;
#endif
            }
            return new ValidationResult(this.FormatErrorMessage(this.ErrorMessage), members);
        }
    }
}