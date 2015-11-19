using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Proteca.Web.Resources;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CheckMesureAttribute : ValidationAttribute
    {
        public CheckMesureAttribute()
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

            if (validationContext.ObjectInstance is MesMesure)
            {
                var niveau = validationContext.ObjectInstance as MesMesure;

                if (value == null && niveau.Visite != null)
                {
                    this.ErrorMessage = ValidationErrorResources.DefaultRequiredFieldErrorMessage;
                    if (niveau.Visite.RelevePartiel || niveau.Visite.DateImport != null)
                    {
                        return ValidationResult.Success;
                    }
                }
                else if (value != null
                    && niveau.MesTypeMesure != null
                    && niveau.MesTypeMesure.MesModeleMesure != null)
                {
                    if (niveau.MesTypeMesure.MesModeleMesure.MesurePositive && ((decimal)value) < 0m)
                    {
                        this.ErrorMessage = ValidationErrorResources.DefaultPositiveValue;
                    }
                    else if (niveau.MesTypeMesure.MesModeleMesure.MesUnite != null
                        && niveau.MesTypeMesure.MesModeleMesure.MesUnite.RefEnumValeur != null)
                    {
                        var decimalPartSize = MaxDecimalValueAttribute.GetDecimalPartSize((Decimal)value);
                        switch (niveau.MesTypeMesure.MesModeleMesure.MesUnite.RefEnumValeur.Valeur)
                        {
                            case ("10"):
                                this.ErrorMessage = ValidationErrorResources.DefaultValidationIntFieldErrorMessage;
                                if (decimalPartSize <= 0)
                                {
                                    return ValidationResult.Success;
                                }
                                break;

                            case ("20"):
                                int nbDec = niveau.MesTypeMesure.MesModeleMesure.MesUnite.NombreDeDecimales.GetValueOrDefault(0);
                                this.ErrorMessage = String.Format(ValidationErrorResources.DefaultValidationDecimalFieldErrorMessage, nbDec, nbDec > 0 ? "s" : "");
                                if (decimalPartSize <= 0 || decimalPartSize <= nbDec)
                                {
                                    return ValidationResult.Success;
                                }
                                break;
                        }

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