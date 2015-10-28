using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Helpers;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CheckCommentaireAnalyseAttribute : ValidationAttribute
    {
        public CheckCommentaireAnalyseAttribute()
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
            if (validationContext.ObjectInstance is AnAnalyse)
            {
                var niveau = validationContext.ObjectInstance as AnAnalyse;

                List<String> tags = new List<string>() { "style", "script" };
                String valueStripped = HTMLHelper.StripHtml((value != null && value.ToString() != " ") ? value.ToString() : String.Empty, tags);
                if (niveau.RefEnumValeur != null)
                {
                    if (niveau.RefEnumValeur.Valeur == "01" || !String.IsNullOrEmpty(valueStripped) && valueStripped != "&nbsp;")
                    {
                        return ValidationResult.Success;
                    }
                }
                else
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