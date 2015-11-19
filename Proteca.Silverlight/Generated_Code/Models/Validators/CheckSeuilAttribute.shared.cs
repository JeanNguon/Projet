using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CheckSeuilAttribute : ValidationAttribute
    {
        public CheckSeuilAttribute()
        {
            this.ErrorMessage = ValidationErrorResources.DefaultValidationNoUnitFieldErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dynamicObject = CustomValidators.GetEntity(validationContext.ObjectInstance);
            if (dynamicObject.EntityState.ToString() == "Detached")
            {
                return ValidationResult.Success;
            }

            if (CustomValidators.IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (validationContext.ObjectInstance is MesNiveauProtection)
            {
                var niveau = validationContext.ObjectInstance as MesNiveauProtection;
                if (niveau.MesModeleMesure != null && niveau.MesModeleMesure.MesUnite != null && niveau.MesModeleMesure.MesUnite.RefEnumValeur != null)
                {
                    String str = ((Decimal)value).ToString("#.####################");
                    int index = str.IndexOf(",");
                    if (index <= 0)
                    {
                        index = str.IndexOf(".");
                    }
                    switch (niveau.MesModeleMesure.MesUnite.RefEnumValeur.Valeur)
                    {
                        case ("10"):
                            this.ErrorMessage = ValidationErrorResources.DefaultValidationIntFieldErrorMessage;
                            if (index <= 0)
                            {
                                return ValidationResult.Success;
                            }
                            break;

                        case ("20"):
                            int nbDec = niveau.MesModeleMesure.MesUnite.NombreDeDecimales.HasValue ? niveau.MesModeleMesure.MesUnite.NombreDeDecimales.Value : 0;
                            this.ErrorMessage = String.Format(ValidationErrorResources.DefaultValidationDecimalFieldErrorMessage, nbDec, nbDec > 0 ? "s" : "");
                            if (index <= 0 || ((str.Length) - (index + 1) <= nbDec))
                            {
                                return ValidationResult.Success;
                            }
                            break;
                    }
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