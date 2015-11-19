using System.ComponentModel.DataAnnotations;
using Proteca.Web.Resources;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Proteca.Web.Models
{
    public static class CustomValidators
    {
        #region Validateurs Equipements



#if SILVERLIGHT
        public static System.ServiceModel.DomainServices.Client.Entity GetEntity(object instance)
        {
            return instance as System.ServiceModel.DomainServices.Client.Entity;
        }
#else
        public static System.Data.Objects.DataClasses.EntityObject GetEntity(object instance)
        {
            return instance as System.Data.Objects.DataClasses.EntityObject;
        }
#endif



        /// <summary>
        /// Vérifie que le champ PK inférieur à la longueur de la Portion
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckPkPP(decimal pk, ValidationContext validationContext)
        {
            if (IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            var dynamicObject = GetEntity(validationContext.ObjectInstance);

            if (dynamicObject.EntityState.ToString() == "Detached")
            {
                return ValidationResult.Success;
            }

            Pp pp = (Pp)validationContext.ObjectInstance;
            if (pp != null && pp.PortionIntegrite != null && !pp.BypassPkLimitation && (pp.PortionIntegrite.Longueur < pk || pk < 0))
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);

                if (linkedMembers.Any())
                    members.AddRange(linkedMembers);

                return new ValidationResult(String.Format(ValidationErrorResources.ValidationPkFieldErrorMessage, pp.PortionIntegrite.Longueur)
                    , members);
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Vérifie que le champ CleCategoriePp est renseigné quand le niveau de sensibilité est différent de "PP non Mesurée"
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckClassification(int? cle, ValidationContext validationContext)
        {
            if (IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            Pp pp = (Pp)validationContext.ObjectInstance;

            if (!pp.BypassCategoriePp && pp.CleNiveauSensibilite > 0 && pp.CleNiveauSensibilite != 4 && (!cle.HasValue || cle.Value < 1))
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);

                if (linkedMembers.Any())
                    members.AddRange(linkedMembers);

                return new ValidationResult(
                    ValidationErrorResources.DefaultRequiredFieldErrorMessage
                    , members);
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Vérifie que le champ SurfaceTME est renseigné quand le Témoin enterré est coché
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckTMERaccorde(int? cle, ValidationContext validationContext)
        {
            if (IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            Pp pp = (Pp)validationContext.ObjectInstance;
            if (pp.TemoinEnterreAmovible && (!cle.HasValue || cle.Value < 1))
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);

                if (linkedMembers.Any())
                    members.AddRange(linkedMembers);

                return new ValidationResult(
                    ValidationErrorResources.DefaultRequiredFieldErrorMessage
                    , members);
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Vérifie que le champ SurfaceTMS est renseigné quand le Témoin metallique de surface est coché
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckTMSRaccorde(int? cle, ValidationContext validationContext)
        {
            if (IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            Pp pp = (Pp)validationContext.ObjectInstance;
            if (pp.TemoinMetalliqueDeSurface && (!cle.HasValue || cle.Value < 1))
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);

                if (linkedMembers.Any())
                    members.AddRange(linkedMembers);

                return new ValidationResult(
                    ValidationErrorResources.DefaultRequiredFieldErrorMessage
                    , members);
            }
            return ValidationResult.Success;
        }

        #endregion Validateurs Equipements

        /// <summary>
        /// Validation de l'agence ou du profil en fonction du type d'utilisateur (externe ou non) et si il est supprimé ou non
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckUsrIfNotExterneAndNotSupprime(
            Nullable<int> cle,
            ValidationContext validationContext)
        {
            UsrUtilisateur user = (UsrUtilisateur)validationContext.ObjectInstance;
            if (!user.Externe && !user.Supprime && (!cle.HasValue || cle.Value <= 0))
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);
                if (linkedMembers.Any())
                {
                    members.AddRange(linkedMembers);
                }

                return new ValidationResult(
                    ValidationErrorResources.DefaultRequiredFieldErrorMessage
                    , members);
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validation du mail ou de l'identifiant en fonction du type d'utilisateur (externe ou non)
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckUsrIfNotExterne(
            String prop,
            ValidationContext validationContext)
        {
            UsrUtilisateur user = (UsrUtilisateur)validationContext.ObjectInstance;
            if (!user.Externe && String.IsNullOrEmpty(prop))
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);
                if (linkedMembers.Any())
                {
                    members.AddRange(linkedMembers);
                }

                return new ValidationResult(
                    ValidationErrorResources.DefaultRequiredFieldErrorMessage
                    , members);
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validation de l'agence ou du profil en fonction du type d'utilisateur (externe ou non) et si il est supprimé ou non
        /// </summary>
        /// <param name="societe"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckUsrIfExterne(
            String societe,
            ValidationContext validationContext)
        {
            UsrUtilisateur user = (UsrUtilisateur)validationContext.ObjectInstance;
            if (user.Externe && String.IsNullOrEmpty(societe))
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);
                if (linkedMembers.Any())
                {
                    members.AddRange(linkedMembers);
                }

                return new ValidationResult(
                    ValidationErrorResources.DefaultRequiredFieldErrorMessage
                    , members);
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validation du libelle autre type de mesure
        /// </summary>
        /// <param name="libNiveauAutre"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckLibelleAutre(string libNiveauAutre, ValidationContext validationContext)
        {
            MesTypeMesure mesTypeMesure = (MesTypeMesure)validationContext.ObjectInstance;
            if (mesTypeMesure.RefEnumValeur != null && mesTypeMesure.RefEnumValeur.Libelle == "Autre" && string.IsNullOrEmpty(libNiveauAutre))
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);
                if (linkedMembers.Any())
                {
                    members.AddRange(linkedMembers);
                }

                return new ValidationResult(ValidationErrorResources.DefaultRequiredFieldErrorMessage, members);
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validation du libelle autre type de mesure
        /// </summary>
        /// <param name="seuilMini"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckSeuilMini(decimal? seuilMini, ValidationContext validationContext)
        {
            MesNiveauProtection mesNiveauProtection = (MesNiveauProtection)validationContext.ObjectInstance;
            if (mesNiveauProtection != null && seuilMini >= mesNiveauProtection.SeuilMaxi)
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);
                if (linkedMembers.Any())
                {
                    members.AddRange(linkedMembers);
                }

                return new ValidationResult(ValidationErrorResources.NiveauProtectionCompareSeuilErrorMessage, members);
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckIsUnsignedInt(string text, ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(text) && !Regex.Match(text, @"(^\d*$)").Success)
            {
                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);
                if (linkedMembers.Any())
                {
                    members.AddRange(linkedMembers);
                }

                return new ValidationResult(ValidationErrorResources.NiveauProtectionCompareSeuilErrorMessage, members);
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
        public static List<string> GetLinkedMembers(ValidationContext validationContext)
        {
            List<string> members = new List<string>();
            // rechercher s'il existe un membre lié
            var ass = Assembly.GetExecutingAssembly();
            var type = ass.GetType(validationContext.ObjectInstance.GetType().FullName + "+" + validationContext.ObjectInstance.GetType().Name + "Metadata");
            if (type == null)
            {
                type = validationContext.ObjectInstance.GetType();
            }

            var prop = type.GetProperty(validationContext.MemberName);
            var attributes = prop.GetCustomAttributes(typeof(RequiredReferenceAttribute), true);
            if (attributes.Any())
            {
                foreach (var attr in attributes)
                {
                    members.Add(((RequiredReferenceAttribute)attr).MemberName);
                }
            }
            return members;
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

        /// <summary>
        /// Vérifie que le champ SurfaceTME est renseigné quand le Témoin enterré est coché
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult CheckNbDecimalUnit(int? cle, ValidationContext validationContext)
        {
            if (IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }

            MesUnite monUnite = (MesUnite)validationContext.ObjectInstance;
            if (monUnite.TypeDonnee == 10)
            {

                List<string> members = new List<string>() { validationContext.MemberName };
                List<string> linkedMembers = GetLinkedMembers(validationContext);

                if (cle.HasValue)
                {

                    if (cle <= 0)
                    {
                        if (linkedMembers.Any())
                            members.AddRange(linkedMembers);

                        return new ValidationResult(
                            ValidationErrorResources.DefaultPositiveInt
                            , members);
                    }
                }
                else
                {
                    if (linkedMembers.Any())
                        members.AddRange(linkedMembers);

                    return new ValidationResult(
                        ValidationErrorResources.DefaultRequiredFieldErrorMessage
                        , members);
                }
            }

            return ValidationResult.Success;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="valeur"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult ValidateRefParametreValeur(
           string valeur,
           ValidationContext validationContext)
        {
            var obj = validationContext.ObjectInstance as RefParametre;
            if (validationContext.ObjectInstance is RefParametre)
            {
                switch (obj.TypeDeDonnee)
                {
                    case ("INT"):
                        int intVal;
                        if (obj.Libelle.StartsWith("DELAI_VALIDATION_VISITE") && (!Int32.TryParse(valeur, out intVal) || Convert.ToInt32(valeur) < 0 || Convert.ToInt32(valeur) > 365))
                        {
                            return new ValidationResult(String.Format(ValidationErrorResources.ValidationErrorDateIntervalle, "0", "365"), new[] { validationContext.MemberName, "ObjValue" });
                        }
                        if (!Int32.TryParse(valeur, out intVal) || Convert.ToInt32(valeur) < 0 || Convert.ToInt32(valeur) > 9999)
                        {
                            return new ValidationResult(ValidationErrorResources.ParamGenValidationPositiveIntFieldErrorMessage, new[] { validationContext.MemberName, "ObjValue" });
                        }

                        break;
                    case ("BOOLEAN"):
                        if (valeur != "0" && valeur != "1")
                        {
                            return new ValidationResult(ValidationErrorResources.DefaultValidationBoolFieldErrorMessage, new[] { validationContext.MemberName, "ObjValue" });
                        }
                        break;
                    case ("DOUBLE"):
                        Double doubleVal;
                        String tmpValeur = valeur.Replace('.', ',');
                        // Les paramètres de types seuil peuvent être négatif.
                        if (obj.Libelle.StartsWith("SEUIL_PROTECTION_CONTINU"))
                        {
                            if (!Double.TryParse(tmpValeur, out doubleVal)
                          || (Convert.ToDouble(tmpValeur) < -9999.999 || Convert.ToDouble(tmpValeur) > 9999.999
                          || (tmpValeur.IndexOf(',') > 0 && tmpValeur.Substring(tmpValeur.IndexOf(',')).Length > 4)
                          ))
                            {
                                return new ValidationResult(ValidationErrorResources.ParamGenValidationDoubleFieldErrorMessage, new[] { validationContext.MemberName, "ObjValue" });
                            }
                        }
                        else
                        {
                            if (!Double.TryParse(tmpValeur, out doubleVal)
                                || (Convert.ToDouble(tmpValeur) < 0 || Convert.ToDouble(tmpValeur) > 9999.999
                                || (tmpValeur.IndexOf(',') > 0 && tmpValeur.Substring(tmpValeur.IndexOf(',')).Length > 4)
                                ))
                            {

                                return new ValidationResult(ValidationErrorResources.ParamGenValidationPositiveDoubleFieldErrorMessage, new[] { validationContext.MemberName, "ObjValue" });
                            }

                        }
                        break;
                }
            }
            return ValidationResult.Success;
        }

        #region "Validation Fiche action"

         public static ValidationResult CheckDateRealisationSupDateDebut(
           string propDateRef,
           ValidationContext validationContext)
        {
            if (IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }
             var obj = validationContext.ObjectInstance as AnAction;
             if(obj != null)
             {
                 if(obj.DateDebut.HasValue && obj.DateDebut > DateTime.MinValue)
                 {
                     if (propDateRef != null && Convert.ToDateTime(propDateRef) <= obj.DateDebut)
                     {
                         return new ValidationResult(string.Format(ValidationErrorResources.ValidationErrorDateSup, obj.DateDebut.Value.ToString("dd/MM/yyyy")), new[] { validationContext.MemberName, "ObjValue" });
                     }
                 }
             }
             return ValidationResult.Success;
        }

        public static ValidationResult CheckDateClotureSupDateDebut(
          string propDateRef,
          ValidationContext validationContext)
        {
            if (IsLogicalDelete(validationContext))
            {
                return ValidationResult.Success;
            }
            var obj = validationContext.ObjectInstance as AnAction;
            if (obj != null)
            {
                if (obj.DateDebut.HasValue && obj.DateDebut > DateTime.MinValue)
                {
                    if (propDateRef != null && Convert.ToDateTime(propDateRef) < obj.DateDebut)
                    {
                        return new ValidationResult(string.Format(ValidationErrorResources.ValidationErrorDateSupOrEquals, obj.DateDebut.Value.ToString("dd/MM/yyyy")), new[] { validationContext.MemberName, "ObjValue" });
                    }
                }
            }
            return ValidationResult.Success;
        }

        
        #endregion
    }
}