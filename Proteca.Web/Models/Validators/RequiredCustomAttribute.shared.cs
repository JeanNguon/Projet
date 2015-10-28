using System.ComponentModel.DataAnnotations;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Proteca.Web.Resources;
using System.Reflection;

namespace Proteca.Web.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequiredCustom : RequiredAttribute
    {
        public Boolean AllowZero { get; set; }

        public RequiredCustom()
            : base()
        {
            this.ErrorMessage = ValidationErrorResources.DefaultRequiredFieldErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CustomValidators.IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            ValidationResult vr = base.IsValid(value, validationContext);

            if (vr == null && (AllowZero || !(value is int) || ((int)value) != 0))
            {
                return ValidationResult.Success;
            }

            // EP : Suite à la modification du RequiredReference, cette validation peut également s'effectuer coté Client
            //
            //// Dans le cas d'une clé étrangère obligatoire, la validation doit se faire coté Serveur pour vérifier
            //// si la clé ne fait pas référence à un objet en création
            //if ((value is int) && ((int)value) == 0 && validationContext.ObjectType.Module.Assembly.FullName.Contains("Silverlight"))
            //{
            //    return ValidationResult.Success;
            //}

            List<string> members = new List<string>() { validationContext.MemberName };
            List<string> linkedMembers = CustomValidators.GetLinkedMembers(validationContext);
            if (linkedMembers.Any())
            {
                members.AddRange(linkedMembers);

                // Si l'erreur c'est une Cle = 0
                if (vr == null)
                {
                    var objectType = validationContext.ObjectInstance.GetType();
                    foreach (string propertyName in linkedMembers)
                    {
                        PropertyInfo pi = objectType.GetProperty(propertyName);
                        if (pi != null)
                        {
                            var dynamicObject = CustomValidators.GetEntity(pi.GetValue(validationContext.ObjectInstance, null));
                            if (dynamicObject != null)
                            {
                                if (dynamicObject.EntityState.ToString() == "New" || dynamicObject.EntityState.ToString() == "Detached")
                                {
                                    return ValidationResult.Success;
                                }
                            }
                        }
                        // la propriété n'existe pas dans le modèle
                        // ne rien faire
                    }
                }
            }

            return new ValidationResult(this.FormatErrorMessage(this.ErrorMessage), members);
        }
    }
}