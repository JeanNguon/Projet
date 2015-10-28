using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CheckCommentaireVisiteAttribute : ValidationAttribute
    {
        public CheckCommentaireVisiteAttribute()
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
            if (validationContext.ObjectInstance is Visite)
            {
                var niveau = validationContext.ObjectInstance as Visite;
                if(niveau.Alertes != null && 
                    (!niveau.Alertes.Any(a => a.RefEnumValeur != null && a.RefEnumValeur.Valeur == "U") || !String.IsNullOrEmpty(niveau.Commentaire)))
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