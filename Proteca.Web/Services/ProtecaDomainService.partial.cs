using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using Proteca.Web.Models;
using Proteca.Web.Resources;
using System.ServiceModel.DomainServices.Server;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml;
using log4net.Config;
using log4net;
using System.Data;
using System.Globalization;
using System.Data.SqlClient;


namespace Proteca.Web.Services
{
    public partial class ProtecaDomainService
    {
        private ILog logger;

        protected override void OnError(DomainServiceErrorInfo errorInfo)
        {
            base.OnError(errorInfo);
            logger.Error("ProtecaDomainService Error", errorInfo.Error);
        }

        protected override bool ValidateChangeSet()
        {
            foreach (var entry in this.ChangeSet.ChangeSetEntries.Where(e => e.Entity != null && e.Operation == DomainOperation.Update && e.OriginalEntity == null))
            {
                this.ObjectContext.AttachTo(GetEntitySetName(this.ObjectContext, entry.Entity), entry.Entity);
                this.ObjectContext.ObjectStateManager.ChangeObjectState(entry.Entity, System.Data.EntityState.Unchanged);
            }

            // Pour chaque entité modifiée ou ajoutée on vérifie s'il existe pour l'une de ces propriétés un attribut "Unique" qui impose que la valeur de cette propriété soit unique en base de données.
            IEnumerable<ChangeSetEntry> entries = this.ChangeSet.ChangeSetEntries.Where(e => e.Operation == DomainOperation.Update || e.Operation == DomainOperation.Insert);

            ChangeSetEntry logouvrageEntry = entries.FirstOrDefault(e => e.Operation == DomainOperation.Insert && e.Entity is LogOuvrage);
            int cleCurrentUser = 0;
            if (logouvrageEntry != null && logouvrageEntry.Entity != null)
            {
                cleCurrentUser = ((LogOuvrage)logouvrageEntry.Entity).CleUtilisateur;
            }

            foreach (var entry in entries)
            {
                if (entry.Entity != null)
                {
                    // désactive les changements pour la vue AlerteDetail
                    if (entry.Entity is AlerteDetail)
                    {
                        entry.Operation = DomainOperation.None;
                    }

                    var type = entry.Entity.GetType();

                    PropertyInfo prop = type.GetProperty("Supprime");

                    if (prop != null)
                    {
                        bool isDeleted = (bool)prop.GetValue(entry.Entity, null);


                        if (isDeleted)
                        {
                            //Dans le cas d'un equipement ou d'une PP, lors de la suppression, on supprime egalement les compositions associées
                            if (entry.Entity is EqEquipement)
                            {

                                EqEquipement eq = entry.Entity as EqEquipement;
                                //PropertyInfo prop2 = type.GetProperty("DateMajCommentaire");
                                //DateTime? isPhysicalDeleted = (DateTime?)prop2.GetValue(entry.Entity, null);
                                //if (isPhysicalDeleted == DateTime.MaxValue) // flag de suppression des tournés si deplacement ou suppression physique demandée
                                //{
                                // Dans le cas d'un déplacement d'équipement, on ajoute des compositions pour l'équipement d'arrivé
                                // pour les tournées par Eq (les autres seront ajoutées normalement plus bas)
                                if (eq.EqEquipementEqEquipement.Any() && isDeleted)
                                {
                                    foreach (Composition compo in eq.Compositions.Where(co => !co.Tournee.Compositions.Any(c => c.CleEnsElectrique.HasValue || c.ClePortion.HasValue)))
                                    {
                                        this.ObjectContext.AddToCompositions(new Composition
                                        {
                                            CleTournee = compo.CleTournee,
                                            EqEquipement = eq.EqEquipementEqEquipement.First(),
                                            NumeroOrdre = compo.NumeroOrdre,
                                            EnumTypeEval = compo.EnumTypeEval
                                        });
                                    }
                                }
                                ObjectContext.Compositions.Where(cp => cp.CleEquipement == ((EqEquipement)entry.Entity).CleEquipement).ToList()
                                    .ForEach(cp => this.ObjectContext.DeleteObject(cp));
                                addLogTourneeOnCompositionsForPpOrEq(cleCurrentUser, entry.Entity, true);
                                //}
                            }
                            else if (entry.Entity is Pp)
                            {
                                //PropertyInfo prop2 = type.GetProperty("DateMajCommentaire");
                                //DateTime? isPhysicalDeleted = (DateTime?)prop2.GetValue(entry.Entity, null);
                                //if (isPhysicalDeleted == DateTime.MaxValue) // flag de suppression des tournés si deplacement ou suppression physique demandée
                                //{
                                Pp pp = entry.Entity as Pp;
                                // Dans le cas d'un déplacement de PP, on ajoute des compositions pour la PP d'arrivée
                                // pour les tournées par Eq (les autres seront ajoutées normalement plus bas)
                                if (pp.Pp1.Any() && isDeleted)
                                {
                                    foreach (Composition compo in pp.Compositions.Where(co => !co.Tournee.Compositions.Any(c => c.CleEnsElectrique.HasValue || c.ClePortion.HasValue)))
                                    {
                                        this.ObjectContext.AddToCompositions(new Composition
                                        {
                                            CleTournee = compo.CleTournee,
                                            Pp = pp.Pp1.First(),
                                            NumeroOrdre = compo.NumeroOrdre,
                                            EnumTypeEval = compo.EnumTypeEval
                                        });
                                    }
                                }
                                ObjectContext.Compositions.Where(cp => cp.ClePp == ((Pp)entry.Entity).ClePp).ToList()
                                    .ForEach(cp => this.ObjectContext.DeleteObject(cp));
                                addLogTourneeOnCompositionsForPpOrEq(cleCurrentUser, entry.Entity, true);
                                //}
                            }
                            else if (entry.Entity is PortionIntegrite)
                            {
                                addLogTourneeOnCompositionsForPpOrEq(cleCurrentUser, entry.Entity, true);
                            }
                            continue;
                        }
                    }
                }

                // On récupère le type de la sous classe "Metadata" correspondant au type de l'entité en cours
                var ass = Assembly.GetAssembly(entry.Entity.GetType());
                var metadataType = ass.GetType(entry.Entity.GetType().FullName + "+" + entry.Entity.GetType().Name + "Metadata");
                if (metadataType != null)
                {
                    // On liste tous les membres de type "Property"
                    List<MemberInfo> members = new List<MemberInfo>(metadataType.GetProperties(
                                                            BindingFlags.Public |
                                                            BindingFlags.Instance |         //Get instance members
                                                            BindingFlags.DeclaredOnly));     //Get only members declared in classType

                    //Get inheritance members
                    Type entityBaseType = entry.Entity.GetType().BaseType;
                    if (entityBaseType != typeof(EntityObject))
                    {
                        var metadataBaseType = ass.GetType(entityBaseType.FullName + "+" + entityBaseType.Name + "Metadata");
                        members.AddRange(metadataBaseType.GetProperties(
                                            BindingFlags.Public |
                                            BindingFlags.Instance |         //Get instance members
                                            BindingFlags.DeclaredOnly));     //Get only members declared in classType
                    }
                    List<MemberInfo> uniqueMembers = members.Where(m => m.GetCustomAttributes(typeof(Unique), true).Any()).ToList();
                    foreach (var member in uniqueMembers)
                    {
                        // Using reflection.
                        object[] attrs = member.GetCustomAttributes(typeof(Unique), true);

                        // On vérifie si un attribut personnalisé de type Unique existe sur le membre.
                        if (attrs.Length > 0)
                        {
                            // Vérification d'un filtre sur le unique (attribut unique combiné à un autre membre)
                            Unique filterAttr = (Unique)attrs.FirstOrDefault(a => a is Unique);
                            List<String> FilterMemberName = new List<String>();
                            List<PropertyInfo> propFilter = new List<PropertyInfo>();
                            List<PropertyInfo> subPropFilter = new List<PropertyInfo>();
                            List<object> newValueFilter = new List<object>();
                            if (filterAttr != null && filterAttr.FilterMemberName.Count > 0)
                            {
                                int index = 0;
                                foreach (String filterMemberName in filterAttr.FilterMemberName)
                                {
                                    String[] filterMemberNames = filterMemberName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                                    if (filterMemberNames.Length > 0 && members.Any(m => m.Name == filterMemberNames[0]))
                                    {
                                        FilterMemberName.Add(filterMemberNames[0]);
                                        PropertyInfo filterProp = entry.Entity.GetType().GetProperty(filterMemberNames[0]);
                                        if (filterProp != null)
                                        {
                                            propFilter.Add(filterProp);

                                            if (filterMemberNames.Length > 1)
                                            {
                                                // On attache l'objet pour avoir accès aux objets liés
                                                this.ObjectContext.AttachTo(GetEntitySetName(this.ObjectContext, entry.Entity), entry.Entity);
                                            }
                                            // récupération des valeurs des filtres
                                            object filterNewValue = filterProp.GetValue(entry.Entity, null);

                                            if (filterMemberNames.Length > 1)
                                            {
                                                // récupération de la sous propriété liée
                                                PropertyInfo filtersubProp = filterNewValue.GetType().GetProperty(filterMemberNames[1]);
                                                subPropFilter.Add(filtersubProp);
                                                filterNewValue = filtersubProp.GetValue(filterNewValue, null);

                                                // On détache l'objet après le traitement
                                                this.ObjectContext.Detach(entry.Entity);
                                            }
                                            else
                                            {
                                                subPropFilter.Add(null);
                                            }
                                            newValueFilter.Add(filterNewValue);
                                        }
                                    }

                                    index++;
                                }
                            }

                            var key = this.ObjectContext.CreateEntityKey(GetEntitySetName(this.ObjectContext, entry.Entity), entry.Entity);
                            // Récupère la nouvelle valeur saisie par l'utilisateur pour le champ
                            var prop = entry.Entity.GetType().GetProperty(member.Name);
                            var newValue = prop.GetValue(entry.Entity, null);
                            if (newValue != null && (!(newValue is string) || !String.IsNullOrEmpty((string)newValue)))
                            {
                                Type objectType = entry.Entity.GetType();
                                if (objectType.BaseType != typeof(EntityObject))
                                {
                                    objectType = objectType.BaseType;
                                }

                                var property = this.ObjectContext.GetType().GetProperty(objectType.Name);
                                if (property == null)
                                {
                                    property = this.ObjectContext.GetType().GetProperty(objectType.Name + "s");
                                }

                                if (property != null)
                                {
                                    var ChangeSetObjectSet = this.ChangeSet.ChangeSetEntries
                                        .Where(ce => ce.Entity.GetType() == entry.Entity.GetType() && ce.Entity != entry.Entity);

                                    List<EntityKey> keys = new List<EntityKey>();

                                    foreach (ChangeSetEntry cEntry in ChangeSetObjectSet)
                                    {
                                        keys.Add(this.ObjectContext.CreateEntityKey(GetEntitySetName(this.ObjectContext, cEntry.Entity), cEntry.Entity));
                                    }

                                    // Pour chaque objet du même type dans le contexte, on vérifie si la valeur du champ n'est pas égale à celle nouvellement saisie.
                                    IEnumerable<object> objectSet = property.GetValue(this.ObjectContext, null) as IEnumerable<object>;
                                    objectSet = objectSet.Where(o => o.GetType() == entry.Entity.GetType() && !keys.Contains(((EntityObject)o).EntityKey));
                                    objectSet = objectSet.Union(ChangeSetObjectSet.Where(c => c.Operation != DomainOperation.Delete).Select(c => c.Entity));

                                    foreach (var obj in objectSet)
                                    {
                                        var objectKey = this.ObjectContext.CreateEntityKey(GetEntitySetName(this.ObjectContext, entry.Entity), obj);
                                        if (!objectKey.Equals(key) || objectKey.EntityKeyValues.Any(k => (int)k.Value == 0))
                                        {
                                            var value = prop.GetValue(obj, null);
                                            if (value != null)
                                            {
                                                // Vérification d'un filtre sur le unique (attribut unique combiné à un autre membre)
                                                Boolean IsSameContextFilter = true; // Par défaut, il n'y a pas de filtre particulier donc le context est le même

                                                for (int indexFilter = 0; indexFilter < propFilter.Count; indexFilter++)
                                                {
                                                    var valueFilter = propFilter[indexFilter].GetValue(obj, null);

                                                    // récupération de la sous propriété liée
                                                    if (subPropFilter[indexFilter] != null)
                                                    {
                                                        valueFilter = subPropFilter[indexFilter].GetValue(valueFilter, null);
                                                    }

                                                    if ((valueFilter == null && newValueFilter[indexFilter] == null)
                                                    || ((valueFilter != null && newValueFilter[indexFilter] != null)
                                                        && (valueFilter is string && newValueFilter[indexFilter] is string ? string.Equals(((string)valueFilter).Trim().ToLower(), ((string)newValueFilter[indexFilter]).Trim().ToLower(), StringComparison.InvariantCultureIgnoreCase) : valueFilter.Equals(newValueFilter[indexFilter]))))
                                                    {
                                                        // même contexte => pas de changement, poursuite de l'itération
                                                    }
                                                    else
                                                    {
                                                        // contexte différent => arrêt de l'itération
                                                        IsSameContextFilter = false;
                                                        break;
                                                    }
                                                }

                                                if (IsSameContextFilter
                                                && (value is string && newValue is string ? string.Equals(((string)value).Trim().ToLower(), ((string)newValue).Trim().ToLower(), StringComparison.InvariantCultureIgnoreCase) : value.Equals(newValue)))
                                                {
                                                    List<string> ErrorMembers = new List<string>() { member.Name };
                                                    var attributes = member.GetCustomAttributes(typeof(RequiredReferenceAttribute), true);
                                                    if (attributes.Any())
                                                    {
                                                        foreach (var attr in attributes)
                                                        {
                                                            ErrorMembers.Add(((RequiredReferenceAttribute)attr).MemberName);
                                                        }
                                                    }

                                                    List<ValidationResultInfo> errors = entry.ValidationErrors != null ? new List<ValidationResultInfo>(entry.ValidationErrors) : new List<ValidationResultInfo>();
                                                    errors.Add(new ValidationResultInfo(ValidationErrorResources.DefaultUniqueFieldErrorMessage, ErrorMembers));
                                                    entry.ValidationErrors = errors;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (entry.Entity is EqEquipement)
                {
                    EqEquipement eq = entry.Entity as EqEquipement;

                    int clePortion = -1;
                    int cleEE = -1;

                    Pp EqPP = this.ObjectContext.Pps.FirstOrDefault(pp => pp.ClePp == eq.ClePp);
                    if (EqPP != null)
                    {
                        clePortion = EqPP.ClePortion;
                        cleEE = EqPP.PortionIntegrite.CleEnsElectrique;
                    }
                    else if (eq.Pp != null)
                    {
                        clePortion = eq.Pp.ClePortion;
                        PortionIntegrite portion = this.ObjectContext.PortionIntegrite.FirstOrDefault(pi => pi.ClePortion == clePortion);
                        if (portion != null)
                        {
                            cleEE = portion.CleEnsElectrique;
                        }
                    }

                    //Si l'equipement est reintégré alors on le rajoute aux tournées qui composent sont ensembles electrique et sa portions
                    bool newvalue = false;
                    bool value = false;
                    if (entry.Operation != DomainOperation.Insert)
                    {
                        newvalue = (bool)entry.Entity.GetType().GetProperty("Supprime").GetValue(entry.Entity, null);
                        value = (bool)entry.Entity.GetType().GetProperty("Supprime").GetValue(this.ObjectContext.EqEquipement.Where(c => c.CleEquipement == eq.CleEquipement).FirstOrDefault(), null);
                    }
                    if ((value != newvalue && value) || entry.Operation == DomainOperation.Insert)
                    {
                        foreach (Tournee tournee in this.ObjectContext.Tournees.Where(t => (t.Compositions.Any(c => c.CleEnsElectrique == cleEE) || t.Compositions.Any(c => c.ClePortion == clePortion)) && !t.Compositions.Any(c => c.CleEquipement == eq.CleEquipement)))
                        {
                            this.ObjectContext.AddToCompositions(new Composition
                            {
                                CleTournee = tournee.CleTournee,
                                CleEquipement = eq.CleEquipement,
                                NumeroOrdre = tournee.Compositions.Max(c => c.NumeroOrdre) + 1,
                                EnumTypeEval = tournee.Compositions.Where(c => c.CleEnsElectrique != null || c.ClePortion != null).Select(c => c.EnumTypeEval).FirstOrDefault()
                            });
                            if (!this.ObjectContext.LogTournee.Any(lt => lt.CleTournee == tournee.CleTournee && lt.CleLogTournee == 0))
                            {
                                addLogTourneeOnComposition(cleCurrentUser, tournee.CleTournee);
                            }
                        }
                    }

                    if (EqPP != null && eq.Supprime == false &&
                        this.ObjectContext.EqEquipement.Any(e => e.CleEquipement != eq.CleEquipement
                            && e.Pp.ClePortion == EqPP.ClePortion
                            && e.Libelle.Trim().ToLower() == eq.Libelle.Trim().ToLower())
                        )
                    {
                        List<string> ErrorMembers = new List<string>();
                        if (eq is EqLiaisonInterne)
                            ErrorMembers.Add("LibellePrincipale");
                        else
                            ErrorMembers.Add("Libelle");

                        // rechercher s'il existe un membre lié
                        var type = ass.GetType(typeof(EqEquipement).FullName + "+" + typeof(EqEquipement).Name + "Metadata");
                        if (type == null)
                        {
                            type = typeof(EqEquipement);
                        }

                        var prop = type.GetProperty("Libelle");
                        var attributes = prop.GetCustomAttributes(typeof(RequiredReferenceAttribute), true);
                        if (attributes.Any())
                        {
                            foreach (var attr in attributes)
                            {
                                ErrorMembers.Add(((RequiredReferenceAttribute)attr).MemberName);
                            }
                        }

                        List<ValidationResultInfo> errors = entry.ValidationErrors != null ? new List<ValidationResultInfo>(entry.ValidationErrors) : new List<ValidationResultInfo>();
                        errors.Add(new ValidationResultInfo(ValidationErrorResources.DefaultUniqueFieldErrorMessage, ErrorMembers));
                        entry.ValidationErrors = errors;
                    }
                }
                else if (entry.Entity is Pp)
                {
                    Pp aPP = entry.Entity as Pp;

                    PortionIntegrite aPPPortion = this.ObjectContext.PortionIntegrite.FirstOrDefault(po => po.ClePortion == aPP.ClePortion);

                    //Si la PP est reintégré alors on le rajoute aux tournées qui composent sont ensembles electrique et sa portions
                    bool newvalue = false;
                    bool value = false;
                    if (entry.Operation != DomainOperation.Insert)
                    {
                        newvalue = (bool)entry.Entity.GetType().GetProperty("Supprime").GetValue(entry.Entity, null);
                        value = (bool)entry.Entity.GetType().GetProperty("Supprime").GetValue(this.ObjectContext.Pps.Where(c => c.ClePp == aPP.ClePp).FirstOrDefault(), null);
                    }
                    if (aPPPortion != null && ((value != newvalue && value) || entry.Operation == DomainOperation.Insert))
                    {
                        foreach (Tournee tournee in this.ObjectContext.Tournees.Where(t => (t.Compositions.Any(c => c.CleEnsElectrique == aPPPortion.CleEnsElectrique) || t.Compositions.Any(c => c.ClePortion == aPP.ClePortion)) && !t.Compositions.Any(c => c.ClePp == aPP.ClePp)))
                        {
                            this.ObjectContext.AddToCompositions(new Composition
                            {
                                CleTournee = tournee.CleTournee,
                                ClePp = aPP.ClePp,
                                NumeroOrdre = tournee.Compositions.Max(c => c.NumeroOrdre) + 1,
                                EnumTypeEval = tournee.Compositions.Where(c => c.CleEnsElectrique != null || c.ClePortion != null).Select(c => c.EnumTypeEval).FirstOrDefault()
                            });
                            if (!this.ObjectContext.LogTournee.Any(lt => lt.CleTournee == tournee.CleTournee && lt.CleLogTournee == 0))
                            {
                                addLogTourneeOnComposition(cleCurrentUser, tournee.CleTournee);
                            }
                        }
                    }

                    var listPpModifiees = this.ChangeSet.ChangeSetEntries.Where(ce => ce.Entity is Pp).Select(ce => ce.Entity as Pp).ToList();


                    if (aPP.Supprime == false && this.ObjectContext.Pps.Where(pp => pp.ClePp != aPP.ClePp && pp.ClePortion == aPP.ClePortion
                            && pp.Libelle.Trim().ToLower() == aPP.Libelle.Trim().ToLower()).ToList().Any(pp => !listPpModifiees.Any(ce => ce.ClePp == pp.ClePp)))
                    {
                        List<string> ErrorMembers = new List<string>() { "Libelle" };
                        // rechercher s'il existe un membre lié
                        var type = ass.GetType(typeof(Pp).FullName + "+" + typeof(Pp).Name + "Metadata");
                        if (type == null)
                        {
                            type = typeof(Pp);
                        }


                        var prop = type.GetProperty("Libelle");
                        var attributes = prop.GetCustomAttributes(typeof(RequiredReferenceAttribute), true);
                        if (attributes.Any())
                        {
                            foreach (var attr in attributes)
                            {
                                ErrorMembers.Add(((RequiredReferenceAttribute)attr).MemberName);
                            }
                        }

                        List<ValidationResultInfo> errors = entry.ValidationErrors != null ? new List<ValidationResultInfo>(entry.ValidationErrors) : new List<ValidationResultInfo>();
                        errors.Add(new ValidationResultInfo(ValidationErrorResources.DefaultUniqueFieldErrorMessage, ErrorMembers));
                        entry.ValidationErrors = errors;
                    }
                }
            }

            //Gestion des éléments à prendre en compte lors de la suppression des Eq ou Pp
            foreach (var entry in this.ChangeSet.ChangeSetEntries.Where(e => (e.Entity is Pp || e.Entity is EqEquipement || e.Entity is PortionIntegrite) && e.Operation == DomainOperation.Delete))
            {
                //Delete cascade sur les Pps
                if (entry.Entity is Pp)
                {
                    ObjectContext.EqEquipementTmp.Where(eq => eq.ClePp == ((Pp)entry.Entity).ClePp).ToList()
                        .ForEach(eq => this.ObjectContext.DeleteObject(eq));

                    ObjectContext.PpTmp.Where(pp => pp.ClePp == ((Pp)entry.Entity).ClePp).ToList()
                        .ForEach(pp => this.ObjectContext.DeleteObject(pp));

                    ObjectContext.Visites.Where(vi => vi.ClePp == ((Pp)entry.Entity).ClePp).ToList()
                        .ForEach(vi => this.ObjectContext.DeleteObject(vi));
                }
                else if (entry.Entity is EqEquipement)
                {
                    ObjectContext.Visites.Where(vi => vi.CleEquipement == ((EqEquipement)entry.Entity).CleEquipement).ToList()
                        .ForEach(vi => this.ObjectContext.DeleteObject(vi));
                }

                //ajout des logTournees en base sur les tournées
                addLogTourneeOnCompositionsForPpOrEq(cleCurrentUser, entry.Entity);
            }

            // detach
            foreach (var entry in this.ChangeSet.ChangeSetEntries)
            {
                var key = this.ObjectContext.CreateEntityKey(GetEntitySetName(this.ObjectContext, entry.Entity), entry.Entity);
                // On détache explicitement l'ancienne version de l'entité modifiée pour éviter toute erreure lors des AttachAsModified etc,..
                ObjectStateEntry currentEntry = null;
                if (this.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(key, out currentEntry))
                {
                    this.ObjectContext.Detach(currentEntry.Entity);
                }
            }

            return !base.ChangeSet.HasError && base.ValidateChangeSet();
        }

        #region LogTournee on ValidateChangeSet

        /// <summary>
        /// Ajout d'un logTournée sur les compositions de la tournée pour un eq
        /// </summary>
        /// <param name="cleCurrentUser">Clé de l'utilisateur sur lequel le log sera taggé</param>
        /// <param name="entry">Clé de la tournée à logger</param>
        private void addLogTourneeOnCompositionsForPpOrEq(int cleCurrentUser, object entity, bool isDeleting = false)
        {
            if (cleCurrentUser != 0)
            {
                if (entity is Pp)
                {
                    this.ObjectContext.Compositions.Where(c => c.ClePp == ((Pp)entity).ClePp).ToList().ForEach(c =>
                    {
                        addLogTourneeOnComposition(cleCurrentUser, c.CleTournee);
                        if (isDeleting)
                        {
                            this.ObjectContext.DeleteObject(c);
                        }
                    });
                }
                else if (entity is EqEquipement)
                {
                    this.ObjectContext.Compositions.Where(c => c.CleEquipement == ((EqEquipement)entity).CleEquipement).ToList().ForEach(c =>
                    {
                        addLogTourneeOnComposition(cleCurrentUser, c.CleTournee);
                        if (isDeleting)
                        {
                            this.ObjectContext.DeleteObject(c);
                        }
                    });
                }
                else if (entity is PortionIntegrite)
                {
                    this.ObjectContext.Compositions.Where(c => c.ClePortion == ((PortionIntegrite)entity).ClePortion).ToList().ForEach(c =>
                    {
                        addLogTourneeOnComposition(cleCurrentUser, c.CleTournee);
                        if (isDeleting)
                        {
                            this.ObjectContext.DeleteObject(c);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Ajout d'un logTournée sur les compositions de la tournée pour un utilisateur
        /// </summary>
        /// <param name="cleCurrentUser">Cle de l'utilisateur sur lequel le log sera taggé</param>
        /// <param name="cleTournee">Cle de la tournée à logger</param>
        private void addLogTourneeOnComposition(int cleCurrentUser, int cleTournee)
        {
            //if (!this.ObjectContext.LogTournee.Any(lt => lt.CleTournee == cleTournee && lt.CleLogTournee == 0) && cleCurrentUser != 0)
            if (!this.ObjectContext.ObjectStateManager
                                   .GetObjectStateEntries(EntityState.Added)
                                   .Select(e => e.Entity).OfType<LogTournee>()
                                   .Any(lt => lt.CleTournee == cleTournee)
                && cleCurrentUser != 0)
            {
                //ajout logTournée
                this.ObjectContext.AddToLogTournee(new LogTournee()
                {
                    CleUtilisateur = cleCurrentUser,
                    CleTournee = cleTournee,
                    ListeChamps = "Compositions",
                    DateHistorisation = DateTime.Now,
                    EnumTypeModification = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "M").CleEnumValeur
                });
            }
            else
            {

            }
        }

        #endregion LogTournee on ValidateChangeSet

        public static string GetEntitySetName(ObjectContext context, object entity)
        {
            Type entityType = entity.GetType().BaseType;

            if (entityType == typeof(EntityObject))
            {
                entityType = entity.GetType();
            }

            EntityContainer container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);
            var entitySetRes = (from entitySet in container.BaseEntitySets
                                where entitySet.ElementType.Name.Equals(entityType.Name)
                                select entitySet);
            if (!entitySetRes.Any())
            {
                entitySetRes = (from entitySet in container.BaseEntitySets
                                where entitySet.ElementType.Name.Equals(entityType.Name + "s")
                                select entitySet);
            }
            return entitySetRes.Select(e => e.Name).Single();
        }

        public static EntitySetBase GetEntitySet(ObjectContext context, object entity)
        {
            Type entityType = entity.GetType().BaseType;

            if (entityType == typeof(EntityObject))
            {
                entityType = entity.GetType();
            }

            EntityContainer container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);
            var entitySetRes = (from entitySet in container.BaseEntitySets
                                where entitySet.ElementType.Name.Equals(entityType.Name)
                                select entitySet);
            if (!entitySetRes.Any())
            {
                entitySetRes = (from entitySet in container.BaseEntitySets
                                where entitySet.ElementType.Name.Equals(entityType.Name + "s")
                                select entitySet);
            }
            return entitySetRes.Single();
        }

        public ProtecaDomainService()
            : base()
        {
            this.ObjectContext.CommandTimeout = 180;

            XmlConfigurator.Configure();
            logger = LogManager.GetLogger("ProtecaV4");
        }


        #region GeoRegion

        /// <summary>
        /// Récupère la région avec la clé passé en paramètre dans le domaine contexte
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public GeoRegion GetRegionWithInstrumentsByCle(int cle)
        {
            return this.ObjectContext.GeoRegion.Include("InsInstrument").Include("InsInstrument.InsInstrument").FirstOrDefault(r => r.CleRegion == cle);
        }

        /// <summary>
        /// Récupère la liste des régions en intégrant les Agence et les secteur
        /// </summary>
        /// <returns></returns>
        public IQueryable<GeoRegion> GetRegionsWithChildEntities()
        {
            return this.ObjectContext.GeoRegion.Include("GeoAgence").Include("GeoAgence.GeoSecteur").OrderBy(r => r.LibelleRegion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<GeoRegion> CheckAndRegroupeRegionByCle(int cleOfRegrouping, int cleToRegrouping, string libelle, string libelleAbrege)
        {
            GeoRegion regionOfRegrouping = this.ObjectContext.GeoRegion.Include("GeoAgence").Include("InsInstrument").FirstOrDefault(r => r.CleRegion == cleOfRegrouping);
            GeoRegion regionToRegrouping = this.ObjectContext.GeoRegion.Include("GeoAgence").Include("InsInstrument").FirstOrDefault(r => r.CleRegion == cleToRegrouping);

            if (regionOfRegrouping != null && regionToRegrouping != null)
            {
                int agenceCount = regionToRegrouping.GeoAgence.Count;

                //on met à jour le libelle et le libelle abrege
                if (!string.IsNullOrEmpty(libelle) && libelle != regionOfRegrouping.LibelleRegion)
                    regionOfRegrouping.LibelleRegion = libelle;

                if (!string.IsNullOrEmpty(libelleAbrege) && libelleAbrege != regionOfRegrouping.LibelleAbregeRegion)
                    regionOfRegrouping.LibelleAbregeRegion = libelleAbrege;

                //Attachement des agences de l'ancienne région sur la nouvelle
                for (int i = 0; i < agenceCount; i++)
                {
                    GeoAgence agence = regionToRegrouping.GeoAgence.FirstOrDefault();
                    if (agence != null)
                    {
                        regionOfRegrouping.GeoAgence.Add(agence);
                    }
                }

                int instrumentCount = regionToRegrouping.InsInstrument.Count;

                //Attachement des instruments de l'ancienne région sur la nouvelle
                for (int ii = 0; ii < instrumentCount; ii++)
                {
                    InsInstrument instrument = regionToRegrouping.InsInstrument.FirstOrDefault();
                    if (instrument != null)
                    {
                        if (regionOfRegrouping.InsInstrument.Any(i => i.Libelle.Equals(instrument.Libelle, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            regionOfRegrouping.InsInstrument.Add(instrument);
                        }
                        else
                        {
                            instrument.Libelle += " (" + regionToRegrouping.CodeRegion + ")";
                            regionOfRegrouping.InsInstrument.Add(instrument);
                        }
                    }
                }

                this.ObjectContext.GeoRegion.DeleteObject(regionToRegrouping);
            }

            this.ObjectContext.SaveChanges();

            return this.ObjectContext.GeoRegion.Include("GeoAgence").Include("GeoAgence.GeoSecteur").OrderBy(r => r.LibelleRegion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public string CheckAndDeleteRegionByCle(int cle)
        {
            GeoRegion region = this.ObjectContext.GeoRegion.FirstOrDefault(r => r.CleRegion == cle);

            bool hasInstrumentNoDeleted = false;
            if (region.InsInstrument.Count > 0)
            {
                foreach (InsInstrument instru in region.InsInstrument)
                {
                    if (!instru.Supprime)
                    {
                        hasInstrumentNoDeleted = true;
                        break;
                    }
                }
            }

            string message = string.Empty;
            //Si les conditions ne sont pas remplies on indique les conditions non respectées 
            if (region.GeoAgence.Count > 0 || hasInstrumentNoDeleted)
            {
                message = string.Format(Resources.Resource.DecoupageGeo_DeleteRegionMainMsgError, region.LibelleRegion);

                // des agences sont encore rattachées à la région
                if (region.GeoAgence.Count > 0)
                {
                    message += Environment.NewLine + Resources.Resource.DecoupageGeo_DeleteRegionAgenceMsgError;
                }

                // des instruments non supprimés logiquement sont encore attachés au secteur
                if (hasInstrumentNoDeleted)
                {
                    message += Environment.NewLine + Resources.Resource.DecoupageGeo_DeleteInstrumentMsgError;
                }
            }
            else // si les conditions sont respectées alors on supprime la region
            {
                if (region.InsInstrument.Count > 0)
                {
                    foreach (InsInstrument instrument in region.InsInstrument)
                    {
                        instrument.CleRegion = null;
                    }
                }
                this.ObjectContext.GeoRegion.DeleteObject(region);
                this.ObjectContext.SaveChanges();
            }

            return message;
        }

        #endregion GeoRegion

        #region GeoAgence

        /// <summary>
        /// On récupère les agences avec les regions
        /// </summary>
        /// <returns></returns>
        public IQueryable<GeoAgence> GetAgencesWithRegions()
        {
            return this.ObjectContext.GeoAgence.Include("GeoRegion").OrderBy(u => u.LibelleAgence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public GeoAgence GetAgenceWithInstrumentsByCle(int cle)
        {
            return this.ObjectContext.GeoAgence.Include("InsInstrument").FirstOrDefault(a => a.CleAgence == cle);
        }

        /// <summary>
        /// ON verifie si toutes les conditions sont remplies pour supprimer l'agence
        /// </summary>
        /// <param name="cle">Identifiant de l'agence</param>
        /// <returns>Un message</returns>
        public string CheckAndDeleteAgenceByCle(int cle)
        {
            GeoAgence agence = ObjectContext.GeoAgence.FirstOrDefault(a => a.CleAgence == cle);

            bool hasInstrumentNoDeleted = false;
            if (agence.InsInstrument.Count > 0)
            {
                foreach (InsInstrument instru in agence.InsInstrument)
                {
                    if (!instru.Supprime)
                    {
                        hasInstrumentNoDeleted = true;
                        break;
                    }
                }
            }

            string msgError = string.Empty;

            if (agence.GeoSecteur.Count > 0 || agence.UsrUtilisateur.Count > 0 || hasInstrumentNoDeleted)
            {
                msgError = string.Format(Resources.Resource.DecoupageGeo_DeleteAgenceMainMsgError, agence.LibelleAgence);

                // des secteurs sont encore rattachés à l'agence
                if (agence.GeoSecteur.Count > 0)
                {
                    msgError += Environment.NewLine + Resources.Resource.DecoupageGeo_DeleteAgenceSecteurMsgError;
                }

                // des utilisateurs sont encore rattachés à l'agence
                if (agence.UsrUtilisateur.Count > 0)
                {
                    msgError += Environment.NewLine + Resources.Resource.DecoupageGeo_DeleteAgenceUserMsgError;
                }

                // des instrument non supprimés logiquement sont encore rattachés à l'agence
                if (hasInstrumentNoDeleted)
                {
                    msgError += Environment.NewLine + Resources.Resource.DecoupageGeo_DeleteInstrumentMsgError;
                }
            }
            else
            {

                //On rattache les instrument à l'agence parente
                if (agence.InsInstrument.Count > 0)
                {
                    List<int> ClesInstru = agence.InsInstrument.Select(i => i.CleInstrument).ToList();
                    foreach (int cleInstru in ClesInstru)
                    {
                        InsInstrument instru = ObjectContext.InsInstrument.FirstOrDefault(i => i.CleInstrument == cleInstru);
                        instru.CleAgence = null;
                        instru.CleRegion = agence.CleRegion;
                    }
                }
                //On initialise le secteur de l'utilisateur à null
                if (agence.UsrUtilisateur.Count > 0)
                {
                    List<int> ClesUser = agence.UsrUtilisateur.Select(u => u.CleUtilisateur).ToList();
                    foreach (int cleUser in ClesUser)
                    {
                        ObjectContext.UsrUtilisateur.FirstOrDefault(u => u.CleUtilisateur == cleUser).CleAgence = null;
                    }
                }

                this.ObjectContext.GeoAgence.DeleteObject(agence);
                this.ObjectContext.SaveChanges();
            }
            return msgError;
        }

        #endregion GeoAgence

        #region GeoSecteur

        /// <summary>
        /// On récupère l'agence avec les utilisateurs associés
        /// </summary>
        /// <returns></returns>
        public GeoSecteur GetSecteurDetailsByCle(int cle)
        {
            return this.ObjectContext.GeoSecteur.Include("UsrUtilisateur")
                .Include("InsInstrument")
                .FirstOrDefault(s => s.CleSecteur == cle);
        }

        /// <summary>
        /// On récupère le secteur avec sa liste d'instruments
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public GeoSecteur GetSecteurWithInstrumentsByCle(int cle)
        {
            return this.ObjectContext.GeoSecteur.Include("InsInstrument").FirstOrDefault(s => s.CleSecteur == cle);
        }

        /// <summary>
        /// Vérifie si les conditions sont remplies pour supprimer le secteur 
        /// ayant comme identifiant la clé passé en paramètre
        /// </summary>
        /// <param name="cle">identifiant du secteur a supprimer</param>
        /// <returns>Retourne String.Empty si la suppression a été réalise 
        /// sinon les actions a réaliser pour supprimer le secteur</returns>
        public string CheckAndDeleteSecteurByCle(int cle)
        {
            GeoSecteur secteur = this.ObjectContext.GeoSecteur.FirstOrDefault(s => s.CleSecteur == cle);

            bool hasInstrumentNoDeleted = false;
            if (secteur.InsInstrument.Count > 0)
            {
                foreach (InsInstrument instru in secteur.InsInstrument)
                {
                    if (!instru.Supprime)
                    {
                        hasInstrumentNoDeleted = true;
                        break;
                    }
                }
            }

            bool hasUserNoDeleted = false;
            if (secteur.UsrUtilisateur.Count > 0)
            {
                foreach (UsrUtilisateur user in secteur.UsrUtilisateur)
                {
                    if (!user.Supprime)
                    {
                        hasUserNoDeleted = true;
                        break;
                    }
                }
            }

            string message = string.Empty;

            if (hasInstrumentNoDeleted || secteur.PiSecteurs.Any(PiSecteurs => !PiSecteurs.PortionIntegrite.Supprime) || secteur.Pps.Any(pp => !pp.Supprime) || hasUserNoDeleted)
            {
                message = string.Format(Resources.Resource.DecoupageGeo_DeleteSecteurMainMsgError, secteur.LibelleSecteur);

                // des utilisateurs non supprimés logiquement sont encore rattachés au secteur
                if (hasUserNoDeleted)
                {
                    message += Environment.NewLine + Resources.Resource.DecoupageGeo_DeleteSecteurUserMsgError;
                }
                // des instruments non supprimés logiquement sont encore attachés au secteur
                if (hasInstrumentNoDeleted)
                {
                    message += Environment.NewLine + Resources.Resource.DecoupageGeo_DeleteInstrumentMsgError;
                }
                // des Pi sont encore rattachés au secteur et que la portion concernée n'est pas supprimée
                if (secteur.PiSecteurs.Any(PiSecteurs => !PiSecteurs.PortionIntegrite.Supprime))
                {
                    message += Environment.NewLine + Resources.Resource.DecoupageGeo_DeletePiMsgError;
                }
                // des PP sont encore rattachés au secteur
                if (secteur.Pps.Any(pp => !pp.Supprime))
                {
                    message += Environment.NewLine + Resources.Resource.DecoupageGeo_DeletePpsMsgError;
                }
            }
            else
            {
                //On rattache les instrument à l'agence parente
                if (secteur.InsInstrument.Count > 0)
                {
                    List<int> ClesInstru = secteur.InsInstrument.Select(i => i.CleInstrument).ToList();
                    foreach (int cleInstru in ClesInstru)
                    {
                        InsInstrument instru = ObjectContext.InsInstrument.FirstOrDefault(i => i.CleInstrument == cleInstru);
                        instru.CleSecteur = null;
                        instru.CleAgence = secteur.CleAgence;
                    }
                }
                //On initialise le secteur de l'utilisateur à null
                if (secteur.UsrUtilisateur.Count > 0)
                {
                    List<int> ClesUser = secteur.UsrUtilisateur.Select(u => u.CleUtilisateur).ToList();
                    foreach (int cleUser in ClesUser)
                    {
                        ObjectContext.UsrUtilisateur.FirstOrDefault(u => u.CleUtilisateur == cleUser).CleSecteur = null;
                    }
                }

                // Suppression des PP, EQ, visites, etc...
                if (secteur.Pps.Count() > 0)
                {
                    foreach (Pp pp in secteur.Pps)
                    {
                        pp.Visites.ToList().ForEach(c => this.ObjectContext.DeleteObject(c));

                        foreach (EqEquipement eq in pp.EqEquipement)
                        {
                            eq.Visites.ToList().ForEach(c => this.ObjectContext.DeleteObject(c));
                        }

                        pp.EqEquipement.ToList().ForEach(c => this.ObjectContext.DeleteObject(c));
                    }

                    secteur.Pps.ToList().ForEach(c => this.ObjectContext.DeleteObject(c));
                }

                // Suppression des PISecteurs et Portions
                if (secteur.PiSecteurs.Count() > 0)
                {
                    secteur.PiSecteurs.ToList().ForEach(c => this.ObjectContext.DeleteObject(c.PortionIntegrite));
                    secteur.PiSecteurs.ToList().ForEach(c => this.ObjectContext.DeleteObject(c));
                }



                this.ObjectContext.GeoSecteur.DeleteObject(secteur);
                this.ObjectContext.SaveChanges();
            }

            return message;
        }

        #endregion GeoSecteur

        #region UsrUtilisateur

        static readonly Func<ProtecaEntities, int, UsrUtilisateur> getUsrUtilisateurByCleQuery = CompiledQuery.Compile<ProtecaEntities, int, UsrUtilisateur>(
            (ctx, p) => ctx.UsrUtilisateur.Include("RefUsrPortee").Include("GeoAgence").Include("GeoAgence.GeoRegion").Include("GeoSecteur").Include("UsrProfil").FirstOrDefault(u => u.CleUtilisateur == p));
        public UsrUtilisateur GetUsrUtilisateurByCle(int cle)
        {
            this.ObjectContext.UsrUtilisateur.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
            return getUsrUtilisateurByCleQuery.Invoke(this.ObjectContext, cle);
        }

        static readonly Func<ProtecaEntities, string, UsrUtilisateur> getUsrUtilisateurByIdentifiantQuery = CompiledQuery.Compile<ProtecaEntities, string, UsrUtilisateur>(
                 (ctx, p) => ctx.UsrUtilisateur.Include("GeoAgence").Include("RefUsrPortee").Include("UsrProfil").Include("UsrProfil.UsrRole").Include("UsrProfil.UsrRole.RefUsrPortee").Include("UsrProfil.UsrRole.RefUsrAutorisation").FirstOrDefault(u => u.Identifiant.ToUpper() == p));
        public UsrUtilisateur GetUsrUtilisateurByIdentifiant(string identifiant)
        {
            this.ObjectContext.UsrUtilisateur.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return getUsrUtilisateurByIdentifiantQuery.Invoke(this.ObjectContext, identifiant);
        }

        public IQueryable<UsrUtilisateur> GetUsrUtilisateurList()
        {
            this.ObjectContext.UsrUtilisateur.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return this.ObjectContext.UsrUtilisateur.OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
        }

        public IQueryable<UsrUtilisateur> FindUsrUtilisateurbyCriterias(int? cleAgence, int? cleSecteur)
        {
            if (cleSecteur.HasValue)
                return GetUsrUtilisateurList().Where(u => u.CleSecteur == cleSecteur.Value);
            else if (cleAgence.HasValue)
                return GetUsrUtilisateurList().Where(u => u.CleAgence == cleAgence.Value);
            else
                return GetUsrUtilisateurList();
        }

        public IQueryable<UsrUtilisateur> FindUsrUtilisateurbyGeoCriterias(int? cleRegion, int? cleAgence, int? cleSecteur)
        {
            this.ObjectContext.UsrUtilisateur.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            this.ObjectContext.GeoAgence.MergeOption = System.Data.Objects.MergeOption.NoTracking;

            if (cleSecteur.HasValue && cleAgence.HasValue)
                return this.ObjectContext.UsrUtilisateur.Where(u => u.CleSecteur == cleSecteur.Value || u.CleAgence == cleAgence.Value).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
            else if (cleAgence.HasValue)
                return this.ObjectContext.UsrUtilisateur.Where(u => u.CleAgence == cleAgence.Value).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
            else if (cleRegion.HasValue)
                return this.ObjectContext.UsrUtilisateur.Where(u => u.GeoAgence.CleRegion == cleRegion.Value).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
            else
                return this.ObjectContext.UsrUtilisateur.Where(u => u.Externe == false).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
        }

        public IQueryable<UsrUtilisateur> FindUsrUtilisateurByCleTournee(int cleTournee)
        {
            this.ObjectContext.UsrUtilisateur.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            ////this.ObjectContext.GeoAgence.MergeOption = System.Data.Objects.MergeOption.NoTracking;

            IEnumerable<GeoSecteur> secteurs = this.ObjectContext.Compositions.Where(co => co.CleTournee == cleTournee && co.ClePp.HasValue && !co.Pp.Supprime).Select(co => co.Pp.GeoSecteur)
                .Union(this.ObjectContext.Compositions.Where(co => co.CleTournee == cleTournee && co.CleEquipement.HasValue && !co.EqEquipement.Supprime).Select(co => co.EqEquipement.Pp.GeoSecteur)).Distinct();

            IEnumerable<int> cleSecteurs = secteurs.Select(s => s.CleSecteur);
            IEnumerable<int> cleAgences = secteurs.Select(a => a.CleAgence).Distinct();

            //return this.ObjectContext.UsrUtilisateur.Where(u => !u.Supprime && (u.Externe || (u.CleSecteur.HasValue && cleSecteurs.Contains(u.CleSecteur.Value)) || (!u.CleSecteur.HasValue && u.CleAgence.HasValue && cleAgences.Contains(u.CleAgence.Value)))).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);

            IEnumerable<int> cleRegions = this.ObjectContext.GeoAgence.Where(a => cleAgences.Contains(a.CleAgence)).Select(a => a.CleRegion).Distinct();

            IQueryable<UsrUtilisateur> usersExt = this.ObjectContext.UsrUtilisateur.Where(u => !u.Supprime
                && u.Externe);

            IQueryable<UsrUtilisateur> usersNat = this.ObjectContext.UsrUtilisateur.Where(u => !u.Supprime
                && !u.Externe
                && u.UsrProfil.UsrRole.Any(r => r.RefUsrAutorisation.CodeAutorisation == "CREA_VISITE_NIV" && r.RefUsrPortee.CodePortee == "01")
                );

            IQueryable<UsrUtilisateur> usersReg = this.ObjectContext.UsrUtilisateur.Where(u => !u.Supprime
                && !u.Externe
                && cleRegions.Contains(u.GeoAgence.CleRegion)
                && u.UsrProfil.UsrRole.Any(r => r.RefUsrAutorisation.CodeAutorisation == "CREA_VISITE_NIV" && r.RefUsrPortee.CodePortee == "02")
                );

            IQueryable<UsrUtilisateur> usersAg = this.ObjectContext.UsrUtilisateur.Where(u => !u.Supprime
                && !u.Externe
                && cleAgences.Contains(u.CleAgence.Value)
                && u.UsrProfil.UsrRole.Any(r => r.RefUsrAutorisation.CodeAutorisation == "CREA_VISITE_NIV" && r.RefUsrPortee.CodePortee == "03")
                );

            IQueryable<UsrUtilisateur> usersSec = this.ObjectContext.UsrUtilisateur.Where(u => !u.Supprime
                && !u.Externe
                && cleSecteurs.Contains(u.CleSecteur.Value)
                && u.UsrProfil.UsrRole.Any(r => r.RefUsrAutorisation.CodeAutorisation == "CREA_VISITE_NIV" && r.RefUsrPortee.CodePortee == "04")
                );

            IQueryable<UsrUtilisateur> users = usersExt.Union(usersNat)
                .Union(usersReg).Union(usersAg).Union(usersSec)
                .OrderBy(u => u.Nom).ThenBy(u => u.Prenom);

            return users;
        }

        /// <summary>
        /// Récupère la liste des instruments non supprimés filtrés par secteur
        /// </summary>
        /// <param name="cleSecteur">filtre sur le secteur</param>
        /// <returns>la liste des instruments filtrés</returns>
        public IQueryable<UsrUtilisateur> FindUsrUtilisateurByGeoSecteur(int cleSecteur)
        {
            this.ObjectContext.UsrUtilisateur.MergeOption = System.Data.Objects.MergeOption.NoTracking;

            GeoAgence agence = this.ObjectContext.GeoSecteur.Where(s => s.CleSecteur == cleSecteur).Select(s => s.GeoAgence).FirstOrDefault();

            return this.ObjectContext.UsrUtilisateur
                .Where(u => !u.Supprime && (u.Externe || (u.CleSecteur.HasValue && u.CleSecteur.Value == cleSecteur) || (u.CleAgence.HasValue && u.CleAgence.Value == agence.CleAgence)));
        }

        public IQueryable<UsrUtilisateur> FindUsrUtilisateurbyInternalCriterias(bool externe)
        {
            this.ObjectContext.UsrUtilisateur.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return this.ObjectContext.UsrUtilisateur.Include("GeoAgence").Where(u => u.Supprime == false && u.Externe == externe).OrderBy(u => u.Nom).ThenBy(u => u.Prenom);
        }

        public Boolean CheckDeleteUsrUtilisateurList(int cle)
        {
            var usrToDelete = this.ObjectContext.UsrUtilisateur.Where(u => u.CleUtilisateur == cle && (u.Pps.Any() || u.EqEquipement.Any() || u.LogOuvrage.Any() || u.Visites.Any() || u.Visites1.Any() || u.Visites2.Any() || u.Visites3.Any() || u.AnAction.Any() || u.AnAction1.Any() || u.AnAnalyse.Any() || u.Tournees.Any()));
            return usrToDelete.Any();
        }

        #endregion UsrUtilisateur

        #region UsrProfil

        public UsrProfil GetUsrProfilByCle(int cle)
        {
            return this.ObjectContext.UsrProfil.Include("UsrUtilisateur").Include("UsrRole").Include("UsrRole.RefUsrPortee").Include("UsrRole.RefUsrAutorisation").Include("UsrRole.RefUsrAutorisation.RefUsrGroupe").FirstOrDefault(p => p.CleProfil == cle);
        }

        public IQueryable<UsrProfil> GetUsrProfilWithRefUsrPortee()
        {
            return this.ObjectContext.UsrProfil.Include("RefUsrPortee");
        }

        #endregion UsrProfil

        #region UsrRole

        public UsrRole GetUsrRoleByCleProfil(int cle)
        {
            return this.ObjectContext.UsrRole.FirstOrDefault(u => u.CleProfil == cle);
        }

        #endregion

        #region InsInstrument

        /// <summary>
        /// Récupère l'instrument ayant comme identifiant la cle en paramètre
        /// </summary>
        /// <param name="cle">identifiant de l'instrument</param>
        /// <returns>Un instrument</returns>
        public InsInstrument GetInsInstrumentByCle(int cle)
        {
            return this.ObjectContext.InsInstrument.FirstOrDefault(u => u.CleInstrument == cle);
        }

        /// <summary>
        /// Récupère la liste des instruments filtre par region / agence / secteur
        /// </summary>
        /// <param name="cleRegion">filtre sur la region</param>
        /// <param name="cleAgence">filtre sur l'agence</param>
        /// <param name="cleSecteur">filtre sur le secteur</param>
        /// <param name="supprime">inclue ou non les instruments supprimés</param>
        /// <returns>la liste des instruments filtrés</returns>
        public IQueryable<InsInstrument> FindInsInstrumentByGeo(int cleRegion, int? cleAgence, int? cleSecteur, bool includeDelete)
        {
            if (includeDelete)
            {
                var instruments = this.ObjectContext.InsInstrument
                    .Where(i => i.CleRegion == cleRegion);
                if (cleAgence.HasValue)
                {
                    instruments = instruments.Union(this.ObjectContext.InsInstrument
                        .Where(i => i.CleAgence == cleAgence.Value));

                    if (cleSecteur.HasValue)
                    {
                        instruments = instruments.Union(this.ObjectContext.InsInstrument
                            .Where(i => i.CleSecteur == cleSecteur.Value));
                    }
                }
                return instruments;
            }
            else
            {
                var instruments = this.ObjectContext.InsInstrument
                   .Where(i => i.CleRegion == cleRegion && i.Supprime == false);
                if (cleAgence.HasValue)
                {
                    instruments = instruments.Union(this.ObjectContext.InsInstrument
                        .Where(i => i.CleAgence == cleAgence.Value && i.Supprime == false));

                    if (cleSecteur.HasValue)
                    {
                        instruments = instruments.Union(this.ObjectContext.InsInstrument
                            .Where(i => i.CleSecteur == cleSecteur.Value && i.Supprime == false));
                    }
                }
                return instruments;
            }
        }

        /// <summary>
        /// Revois la liste des equipement listés qui n'ont pas d'équipements
        /// </summary>
        /// <param name="clesInsInstrument">Liste de clés de InsInstrument</param>
        /// <returns>La liste triée</returns>
        //[Query(HasSideEffects = true)]
        public List<Int32> GetInsInstrumentUtilisesByListInsInstrument(ObservableCollection<int> clesInsInstrument)
        {
            return this.ObjectContext.InsInstrument.Where(i => clesInsInstrument.Contains(i.CleInstrument) && !i.InstrumentsUtilises.Any())
                                                   .Select(i => i.CleInstrument).ToList();

            //return this.ObjectContext.InsInstrument.Where(i => clesInsInstrument.Contains(i.CleInstrument)).Select(i=>i.CleInstrument).ToList();
        }

        /// <summary>
        /// Récupère la liste des instruments non supprimés filtrés par secteur
        /// </summary>
        /// <param name="cleSecteur">filtre sur le secteur</param>
        /// <returns>la liste des instruments filtrés</returns>
        public IQueryable<InsInstrument> FindInsInstrumentByGeoSecteur(int cleSecteur)
        {
            this.ObjectContext.InsInstrument.MergeOption = System.Data.Objects.MergeOption.NoTracking;

            GeoAgence agence = this.ObjectContext.GeoSecteur.Where(s => s.CleSecteur == cleSecteur).Select(s => s.GeoAgence).FirstOrDefault();

            return this.ObjectContext.InsInstrument
                .Where(i => !i.Supprime && ((i.CleSecteur.HasValue && i.CleSecteur.Value == cleSecteur) || (i.CleAgence.HasValue && i.CleAgence.Value == agence.CleAgence) || (i.CleRegion.HasValue && i.CleRegion.Value == agence.CleRegion)))
                .OrderBy(i => i.Libelle);
        }

        public IQueryable<InsInstrument> FindInsInstrumentByCleTournee(int cleTournee)
        {
            this.ObjectContext.InsInstrument.MergeOption = System.Data.Objects.MergeOption.NoTracking;

            IEnumerable<GeoSecteur> secteurs = this.ObjectContext.Compositions.Where(co => co.CleTournee == cleTournee && co.ClePp.HasValue && !co.Pp.Supprime).Select(co => co.Pp.GeoSecteur)
                .Union(this.ObjectContext.Compositions.Where(co => co.CleTournee == cleTournee && co.CleEquipement.HasValue && !co.EqEquipement.Supprime).Select(co => co.EqEquipement.Pp.GeoSecteur)).Distinct();

            IEnumerable<int> cleSecteurs = secteurs.Select(s => s.CleSecteur);
            IEnumerable<int> cleAgences = secteurs.Select(a => a.CleAgence).Distinct();
            IEnumerable<int> cleRegions = this.ObjectContext.GeoAgence.Where(a => cleAgences.Contains(a.CleAgence)).Select(a => a.CleRegion).Distinct();

            return this.ObjectContext.InsInstrument.Where(i => !i.Supprime && ((i.CleSecteur.HasValue && cleSecteurs.Contains(i.CleSecteur.Value)) || (i.CleAgence.HasValue && cleAgences.Contains(i.CleAgence.Value)) || (i.CleRegion.HasValue && cleRegions.Contains(i.CleRegion.Value)))).OrderBy(i => i.Libelle);
        }

        #endregion

        #region MesUnites

        /// <summary>
        /// On récupère l'unité ayant comme identifiant la clé passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public MesUnite GetMesUniteByCle(int cle)
        {
            return this.ObjectContext.MesUnite.Include("MesModeleMesure").FirstOrDefault(u => u.CleUnite == cle);
        }

        #endregion MesUnites

        #region MesModeleMesure

        /// <summary>
        /// Récupère le type de mesure ayant pour identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public MesModeleMesure GetMesModeleMesureByCle(int cle)
        {
            return this.ObjectContext.MesModeleMesure.Include("MesUnite").Include("MesTypeMesure").Include("MesTypeMesure.MesClassementMesure")
                .Include("MesNiveauProtection").FirstOrDefault(m => m.CleModeleMesure == cle);
        }

        /// <summary>
        /// Récupère le type de mesure ayant pour identifiant la cle de type équipement passé en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<MesModeleMesure> GetAllMesModeleMesure()
        {
            return this.ObjectContext.MesModeleMesure.Include("TypeEquipement").Include("MesUnite").Include("MesUnite.RefEnumValeur");
        }

        #endregion MesModeleMesure

        #region RefUsrGroupe

        public RefUsrGroupe GetRefUsrGroupeByCle(int cle)
        {
            return this.ObjectContext.RefUsrGroupe.FirstOrDefault(u => u.CleGroupe == cle);
        }

        public IQueryable<RefUsrGroupe> GetRefUsrGroupeByCleProfil(int cle)
        {
            return this.ObjectContext.RefUsrGroupe;
        }

        #endregion

        #region RefUsrPortee

        public RefUsrPortee GetRefUsrPorteeByCle(int cle)
        {
            return this.ObjectContext.RefUsrPortee.FirstOrDefault(u => u.ClePortee == cle);
        }

        #endregion

        #region RefEnumValeur

        /// <summary>
        /// Récupération d'une RefEnumValeur en fonction de son identifiant
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public RefEnumValeur GetRefEnumValeurByCle(int cle)
        {
            return this.ObjectContext.RefEnumValeur.FirstOrDefault(u => u.CleEnumValeur == cle);
        }

        /// <summary>
        /// Récupération d'une RefEnumValeur en fonction de son CodeGroup
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<RefEnumValeur> GetRefEnumValeursByCodeGroup(string codeGroupe)
        {
            return this.ObjectContext.RefEnumValeur.Where(u => u.CodeGroupe == codeGroupe);
        }


        /// <summary>
        /// Vérifie si le type d'action n'est pas associée à un paramètrage
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public Boolean CheckCanDeleteTypeAction(int cle)
        {
            return !this.ObjectContext.ParametreAction.Any(pa => pa.EnumTypeAction == cle);
        }

        /// <summary>
        /// Vérifie si la catégorie d'anomalie n'est pas associée à un paramètrage
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public Boolean CheckCanDeleteCategorieAnomalie(int cle)
        {
            return !this.ObjectContext.ParametreAction.Any(pa => pa.EnumCategorieAnomalie == cle);
        }

        /// <summary>
        /// Vérifie si le paramètre action n'est pas associé à une action
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public Boolean CheckCanDeleteParametreAction(int cle)
        {
            return !this.ObjectContext.AnAction.Any(a => a.CleParametreAction.HasValue && a.CleParametreAction.Value == cle);
        }

        #endregion RefEnumValeur

        #region RefUsrAutorisation

        public RefUsrAutorisation GetRefUsrAutorisationByCle(int cle)
        {
            return this.ObjectContext.RefUsrAutorisation.FirstOrDefault(u => u.CleAutorisation == cle);
        }

        public IQueryable<RefUsrAutorisation> GetRefUsrAutorisationWithGroupe()
        {
            return this.ObjectContext.RefUsrAutorisation.Include("RefUsrGroupe");
        }

        #endregion

        #region RefParamètre

        public RefParametre GetRefParametreByCle(int cle)
        {
            return this.ObjectContext.RefParametre.FirstOrDefault(u => u.CleParametre == cle);
        }

        #endregion

        #region MesClassementMesure

        public IQueryable<MesClassementMesure> FindMesClassementMesure()
        {
            return this.ObjectContext.MesClassementMesure.Include("MesTypeMesure");
        }

        public MesClassementMesure GetMesClassementMesureByCle(int cle)
        {
            return this.ObjectContext.MesClassementMesure.FirstOrDefault(u => u.CleClassementMesure == cle);
        }

        static readonly Func<ProtecaEntities, IQueryable<MesClassementMesure>> getMesClassementMesureWithMesNiveauProtectionQuery = CompiledQuery.Compile<ProtecaEntities, IQueryable<MesClassementMesure>>(
                      ctx => ctx.MesClassementMesure
                          //.Include("MesTypeMesure.RefEnumValeur")
                                            .Include("MesTypeMesure.MesModeleMesure.MesNiveauProtection")
                                            .Include("MesTypeMesure.MesModeleMesure.MesUnite")
                          //.Include("MesTypeMesure.MesModeleMesure.RefEnumValeur")
                          //.Include("MesTypeMesure.MesModeleMesure.TypeEquipement")
                                            .Where(m => m.MesTypeMesure.MesureEnService == true));

        public IQueryable<MesClassementMesure> GetMesClassementMesureWithMesNiveauProtection()
        {
            this.ObjectContext.MesNiveauProtection.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesTypeMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesModeleMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesUnite.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.RefEnumValeur.MergeOption = MergeOption.NoTracking;

            return getMesClassementMesureWithMesNiveauProtectionQuery.Invoke(this.ObjectContext);
        }

        #endregion

        #region MesTypeMesure

        /// <summary>
        /// Récupère le type de mesure ayant comme identifiant la clé passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public MesTypeMesure GetMesTypeMesureByCle(int cle)
        {
            return this.ObjectContext.MesTypeMesure.FirstOrDefault(u => u.CleTypeMesure == cle);
        }

        /// <summary>
        /// Retourne la liste des Type de mesure
        /// </summary>
        /// <returns></returns>
        public IQueryable<MesTypeMesure> ListMesTypeMesureEnService()
        {
            return this.ObjectContext.MesTypeMesure.Include("MesModeleMesure").Where(t => t.MesureEnService);
        }

        /// <summary>
        /// Vérifie les condition de suppression d'un type de mesure.
        /// </summary>
        /// <param name="cle">identifian du type de mesure à supprimer</param>
        public bool CheckMesTypeMesureToDeleteByCle(int cle)
        {
            // suppression possible uniquement si il n'y a pas de mesures associées
            return this.ObjectContext.MesMesure.Any(m => m.CleTypeMesure == cle);
        }

        /// <summary>
        /// Retourne la liste équipement correspondant à la clé du tableau de clé spécifié
        /// </summary>
        /// <param name="listCleEquipement">Liste de clé d'équipement</param>
        /// <returns>liste d'équipements</returns>
        [Query(HasSideEffects = true)]
        public IQueryable<MesTypeMesure> FindMesTypeMesuresByListCle(ObservableCollection<int> listCle)
        {
            IQueryable<MesTypeMesure> retour = this.ObjectContext.MesTypeMesure
                        .Include("MesModeleMesure")
                        .Include("MesModeleMesure.MesNiveauProtection")
                        .Include("MesModeleMesure.MesUnite")
                        .Where(a => listCle.Contains(a.CleTypeMesure));
            return retour;
        }

        #endregion

        #region TypeEquipement

        /// <summary>
        /// Retourne le type d'équipement ayant pour identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public TypeEquipement GetTypeEquipementByCle(int cle)
        {
            return this.ObjectContext.TypeEquipement.FirstOrDefault(u => u.CleTypeEq == cle);
        }

        #endregion

        #region CategoriePp

        /// <summary>
        /// Retourne le type de CategoriePp ayant pour identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public CategoriePp GetCategoriePpByCle(int cle)
        {
            return this.ObjectContext.CategoriePp.FirstOrDefault(u => u.CleCategoriePp == cle);
        }

        public IQueryable<CategoriePp> ListCategoriesPp()
        {
            return this.ObjectContext.CategoriePp;
        }

        /// <summary>
        /// Vérifie les condition de suppression d'une catégorie de PP.
        /// Si les conditions sont remplis on supprime l'objet.
        /// </summary>
        /// <param name="cle">identifian de la catégorie de PP à supprimer</param>
        /// <returns>True si la suppression est possible, False sinon</returns>
        public bool CheckCanDeleteCategoriesPpByCle(int cle)
        {
            return !this.ObjectContext.Pps.Any(pp => pp.CleCategoriePp == cle);
        }

        #endregion

        #region MesNiveauProtection

        /// <summary>
        /// Récupère le niveau de protection ayant comme identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public MesNiveauProtection GetMesNiveauProtectionByCle(int cle)
        {
            return this.ObjectContext.MesNiveauProtection.FirstOrDefault(n => n.CleNiveauProtection == cle);
        }

        #endregion MesNiveauProtection

        #region RefNiveauSensibilitePp

        /// <summary>
        /// Recupère le RefNiveauSensibilitePp ayant comme identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public RefNiveauSensibilitePp GetRefNiveauSensibilitePpByCle(int cle)
        {
            return this.ObjectContext.RefNiveauSensibilitePp.FirstOrDefault(r => r.CleNiveauSensibilite == cle);
        }

        #endregion RefNiveauSensibilitePp

        #region EnsembleElectrique

        static readonly Func<ProtecaEntities, int, EnsembleElectrique> getEnsembleElectriqueByCleQuery = CompiledQuery.Compile<ProtecaEntities, int, EnsembleElectrique>((ctx, cle) => ctx.EnsembleElectrique.
                Include("AnAnalyseEe").
                Include("AnAnalyseEe.RefEnumValeur").
                Include("PortionIntegrite.PiSecteurs").
                Include("LogOuvrage").Include("LogOuvrage.RefEnumValeur").Include("LogOuvrage.UsrUtilisateur").
                First(e => e.CleEnsElectrique == cle));
        /// <summary>
        /// Retourne l'ensemble électrique ayant comme identifiant la clé passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EnsembleElectrique GetEnsembleElectriqueByCle(int cle)
        {
            return getEnsembleElectriqueByCleQuery.Invoke(this.ObjectContext, cle);
            //return this.ObjectContext.EnsembleElectrique.
            //    Include("AnAnalyseEe").
            //    Include("AnAnalyseEe.RefEnumValeur").
            //    Include("LogOuvrage").Include("LogOuvrage.RefEnumValeur").Include("LogOuvrage.UsrUtilisateur").
            //    First(e => e.CleEnsElectrique == cle);
        }

        static readonly Func<ProtecaEntities, int, IQueryable<int>> getListPortionsPisQuery = CompiledQuery.Compile<ProtecaEntities, int, IQueryable<int>>((ctx, cle) => ctx.PortionIntegrite.Where(p => p.CleEnsElectrique == cle && p.Supprime == false).Select(p => p.ClePortion));
        /// <summary>
        /// Récupère la liste des Portions de l'ensemble électrique ayant comme identifiant la cle passé en paramètre avec les dates ECD / EG et CF
        /// </summary>
        /// <param name="cle">identifiant de l'ensemble électrique</param>
        /// <returns>la liste des portions de l'ensemble électrique</returns>
        public IQueryable<PortionDates> GetListPortions(int cle)
        {
            int[] pis = getListPortionsPisQuery.Invoke(this.ObjectContext, cle).ToArray();
            return this.ObjectContext.PortionDates.Where(p => pis.Contains(p.ClePortion));
        }

        /// <summary>
        /// Indique si il est possible de supprimer l'ensemble électrique physiquement
        /// </summary>
        /// <param name="cle"></param>
        /// <returns>un code pour la suppression 1=>suppression physique, 2=> suppression logique, 3=> suppression impossible</returns>
        public int GetDeleteCodeByEnsembleElectrique(int cle)
        {
            int code = 0;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;
            List<PortionIntegrite> tmp = this.ObjectContext.PortionIntegrite.Where(p => p.CleEnsElectrique == cle).ToList();

            if (tmp.Any())
            {
                if (tmp.Any(p => p.Supprime == false))
                    code = 3;
                else
                    code = 2;
            }
            else
            {
                code = 1;
            }

            return code;
        }

        /// <summary>
        /// Retourne la liste des ensembles d'instrument correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="includeWithoutPortion">inclue ou non les ensemble electrique sans portion</param>
        /// <param name="includeStation">inclue ou non les ensemble électrique de type station</param>
        /// <param name="includePosteGaz">inclue ou non les ensemble électrique de type poste gaz</param>
        /// <param name="libelleEnsElec">filtre sur le libelle de l'ensemble électrique</param>
        /// <returns>Une liste d'ensemble électrique</returns>
        public IQueryable<EnsembleElectrique> FindEnsembleElectriqueByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,
            bool includeWithoutPortion, bool includeStation, bool includePosteGaz, string libelleEnsElec)
        {

            IQueryable<EnsembleElectrique> query;
            if (cleSecteur.HasValue)
            {
                query = this.ObjectContext.EnsembleElectrique.Where(ee => ee.Supprime == false && (includeWithoutPortion && !ee.PortionIntegrite.Any())
                   || ee.PortionIntegrite.Any
                   (p => p.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value)));
            }
            else if (cleAgence.HasValue)
            {
                query = this.ObjectContext.EnsembleElectrique.Where(ee => ee.Supprime == false && (includeWithoutPortion && !ee.PortionIntegrite.Any())
                    || ee.PortionIntegrite.Any(p => p.PiSecteurs.Any(
                    pi => pi.GeoSecteur.GeoAgence.CleAgence == cleAgence.Value)));
            }
            else if (cleRegion.HasValue)
            {
                query = this.ObjectContext.EnsembleElectrique.Where(ee => ee.Supprime == false && (includeWithoutPortion && !ee.PortionIntegrite.Any())
                    || ee.PortionIntegrite.Any(p => p.PiSecteurs.Any(
                     pi => pi.GeoSecteur.GeoAgence.GeoRegion.CleRegion == cleRegion.Value)));
            }
            else
            {
                query = this.ObjectContext.EnsembleElectrique.Where(ee => (includeWithoutPortion || ee.PortionIntegrite.Any()) && ee.Supprime == false);
            }

            //Filtre sur le libellé
            if (!string.IsNullOrEmpty(libelleEnsElec))
            {
                query = query.Where(ee => ee.Libelle.ToLower().Contains(libelleEnsElec.ToLower()));
            }

            if (!includeStation || !includePosteGaz)
            {
                //Inclue les stations ou les poste Gaz ou non
                query = query.Where(ee => !ee.EnumStructureCplx.HasValue
                    || (includeStation && ee.RefEnumValeur1.Valeur == "1")
                    || (includePosteGaz && ee.RefEnumValeur1.Valeur == "2"));
            }

            return query.OrderBy(ee => ee.Libelle);
        }

        #endregion EnsembleElectrique

        #region EqEquipement

        static readonly Func<ProtecaEntities, int, EqEquipement> getEqEquipementByCle = CompiledQuery.Compile<ProtecaEntities, int, EqEquipement>((ctx, cle) => ctx.EqEquipement
                .Include("Pp").Include("Pp.PortionIntegrite").Include("Pp.PortionIntegrite.EnsembleElectrique")
                .Include("Images")
                .FirstOrDefault(e => e.CleEquipement == cle));
        /// <summary>
        /// Retourne l'équipement ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqEquipement GetEqEquipementByCle(int cle)
        {
            EqEquipement tmp = getEqEquipementByCle.Invoke(this.ObjectContext, cle);

            if (tmp != null)
            {
                tmp.EqEquipement2Reference.Load();
                // Chargement uniquement de l'élément déplacé
                tmp.EqEquipementEqEquipement.OfType<EqEquipement>().FirstOrDefault();
            }

            return tmp;
        }

        /// <summary>
        /// Retourne la liste des tournees d'un équipement
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<Tournee> GetListTourneesByEquipement(int cle)
        {
            return this.ObjectContext.Tournees.Where(t => !t.Supprime && t.Compositions.Any(c => c.CleEquipement == cle)).OrderBy(t => t.Libelle);
        }

        /// <summary>
        /// Retourne la liste de point commun des liaisons
        /// </summary>
        /// <returns></returns>
        public List<string> GetListPointCommun()
        {
            List<string> listPointCommun = new List<string>();

            listPointCommun.AddRange(this.ObjectContext.EqEquipement.OfType<EqLiaisonExterne>()
                .Where(e => !string.IsNullOrEmpty(e.LibellePointCommun) && e.Supprime != true)
                .Select(e => e.LibellePointCommun).Distinct().ToList());
            listPointCommun.AddRange(this.ObjectContext.EqEquipement.OfType<EqLiaisonInterne>()
                .Where(e => !string.IsNullOrEmpty(e.LibellePointCommun) && e.Supprime != true)
                .Select(e => e.LibellePointCommun).Distinct().ToList());

            return listPointCommun.Distinct().ToList();
        }

        /// <summary>
        /// Retourne la liste des liaison qui ont pour libelle de point commun le libelle passé en paramètre
        /// </summary>
        /// <returns></returns>
        public List<LiaisonCommunes> GetLiaisonsCommunes(string libelle)
        {
            return this.ObjectContext.EqEquipement.OfType<EqLiaisonExterne>()
                 .Where(e => e.LibellePointCommun == libelle && !e.Supprime)
                 .Select(e => new LiaisonCommunes
                     {
                         LibelleLiaison = e.Libelle,
                         LibellePortion = e.Pp.PortionIntegrite.Libelle,
                         CleEquipement = e.CleEquipement,
                         TypeEquipement = e.TypeEquipement.CodeEquipement,
                         ClePortion = e.Pp.ClePortion
                     })
                 .Union(
                     this.ObjectContext.EqEquipement.OfType<EqLiaisonInterne>()
                     .Where(e => e.LibellePointCommun == libelle && !e.Supprime)
                     .Select(e => new LiaisonCommunes
                     {
                         LibelleLiaison = e.Libelle,
                         LibellePortion = e.Pp.PortionIntegrite.Libelle,
                         CleEquipement = e.CleEquipement,
                         TypeEquipement = e.TypeEquipement.CodeEquipement,
                         ClePortion = e.Pp.ClePortion
                     })
                 ).ToList();
        }

        /// <summary>
        /// Retourne la Pp ayant comme identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle">identifiant de la Pp recherhcée</param>
        /// <returns></returns>
        public EqEquipement GetEqEquipementByCleAndDateAndTypeEval(int cle, DateTime? datedebut, DateTime? datefin, int enumTypeEval)
        {
            EqEquipement tmp;
            if (datedebut.HasValue && datefin.HasValue)
            {
                tmp = this.ObjectContext.EqEquipement
                    .Include("Pp").Include("Pp.PortionIntegrite.PiSecteurs").Include("Pp.PortionIntegrite.EnsembleElectrique")
                    .FirstOrDefault(e => e.CleEquipement == cle);

                DateTime LastVisiteDate = datefin.Value;
                var query = this.ObjectContext.Visites.Include("Alertes")
                    .Include("UsrUtilisateur").Include("UsrUtilisateur1").Include("UsrUtilisateur2").Include("UsrUtilisateur3")
                    .Include("MesMesure.Alertes")
                    .Include("MesMesure.MesTypeMesure.MesModeleMesure")
                    .Include("InstrumentsUtilises.InsInstrument")
                    .Include("AnAnalyseSerieMesure.RefEnumValeur").Include("AnAnalyseSerieMesure.Alertes")
                    .Where(v => v.CleEquipement == cle && v.EstValidee);

                var lastVisite = query
                   .Where(v => v.DateVisite.Value >= datedebut && v.DateVisite.Value <= datefin
                            && v.EnumTypeEvalComposition == enumTypeEval
                            )
                    .OrderByDescending(v => v.DateVisite).ThenByDescending(v => v.CleVisite)
                    .FirstOrDefault();
                if (lastVisite != null)
                {
                    tmp.Visites.Add(lastVisite);

                    var previousVisite = query
                       .Where(v => v.DateVisite.Value < lastVisite.DateVisite.Value
                                && (v.EnumTypeEval == enumTypeEval || (this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CleEnumValeur == enumTypeEval).Valeur == "1" && v.RefEnumValeur.Valeur == "2"))
                                && !v.Telemesure)
                        .OrderByDescending(v => v.DateVisite).ThenByDescending(v => v.CleVisite)
                        .FirstOrDefault();
                    if (previousVisite != null)
                        tmp.Visites.Add(previousVisite);
                }
            }
            else
                tmp = GetEqEquipementByCle(cle);
            return tmp;
        }

        /// <summary>
        /// Retourne la liste des liaison qui ont la clePP passé en paramètre
        /// </summary>
        /// <returns></returns>
        public List<LiaisonCommunes> GetLiaisonsByPP(int clePP)
        {
            return this.ObjectContext.EqEquipement.OfType<EqLiaisonExterne>()
                 .Where(e => e.ClePp == clePP && !e.Supprime)
                 .Select(e => new LiaisonCommunes
                 {
                     LibelleLiaison = e.Libelle,
                     CleEquipement = e.CleEquipement
                 })
                 .Union(
                     this.ObjectContext.EqEquipement.OfType<EqLiaisonInterne>()
                     .Where(e => (e.ClePp == clePP || e.ClePp2 == clePP) && !e.Supprime)
                     .Select(e => new LiaisonCommunes
                     {
                         LibelleLiaison = e.Libelle,
                         CleEquipement = e.CleEquipement
                     })
                 ).ToList();
        }

        /// <summary>
        /// Retourne la liste des Soutirage
        /// </summary>
        /// <returns></returns>
        public IQueryable<EqSoutirage> GetListSoutirageExt()
        {
            return this.ObjectContext.EqEquipement.OfType<EqSoutirage>().Where(eq => !eq.Supprime);
        }

        /// <summary>
        /// Retourne la liste des Drainage
        /// </summary>
        /// <returns></returns>
        public IQueryable<EqDrainage> GetListDrainageExt()
        {
            return this.ObjectContext.EqEquipement.OfType<EqDrainage>().Where(eq => !eq.Supprime);
        }

        /// <summary>
        /// Retourne l'équipement de type EqAnodeGalvanique ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqAnodeGalvanique GetEqAnodeGalvaniqueByCle(int cle)
        {
            EqAnodeGalvanique tmp = this.ObjectContext.EqEquipement.OfType<EqAnodeGalvanique>()
                .Include("Pp").Include("Pp.PortionIntegrite")
                .Include("RefSousTypeOuvrage")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqAnodeGalvanique>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqDispoEcoulementCourantsAlternatifs ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqDispoEcoulementCourantsAlternatifs GetEqDispoEcoulementCourantsAlternatifsByCle(int cle)
        {
            EqDispoEcoulementCourantsAlternatifs tmp = this.ObjectContext.EqEquipement.OfType<EqDispoEcoulementCourantsAlternatifs>()
                .Include("Pp").Include("Pp.PortionIntegrite")
                .Include("RefSousTypeOuvrage")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqDispoEcoulementCourantsAlternatifs>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqDrainage ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqDrainage GetEqDrainageByCle(int cle)
        {
            EqDrainage tmp = this.ObjectContext.EqEquipement.OfType<EqDrainage>()
                .Include("Pp").Include("Pp.PortionIntegrite")
                .Include("RefSousTypeOuvrage").Include("EqDrainageLiaisonsext.EqLiaisonExterne")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqDrainage>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqFourreauMetallique ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqFourreauMetallique GetEqFourreauMetalliqueByCle(int cle)
        {
            EqFourreauMetallique tmp = this.ObjectContext.EqEquipement.OfType<EqFourreauMetallique>()
                .Include("Pp").Include("Pp.PortionIntegrite").Include("Pp2")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqFourreauMetallique>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqPile ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqPile GetEqPileByCle(int cle)
        {
            EqPile tmp = this.ObjectContext.EqEquipement.OfType<EqPile>()
                .Include("Pp").Include("Pp.PortionIntegrite")
                .Include("RefEnumValeur").Include("RefSousTypeOuvrage")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqPile>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqPostegaz ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqPostegaz GetEqPostegazByCle(int cle)
        {
            EqPostegaz tmp = this.ObjectContext.EqEquipement.OfType<EqPostegaz>()
                .Include("Pp").Include("Pp.PortionIntegrite")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqPostegaz>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqRaccordIsolant ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqRaccordIsolant GetEqRaccordIsolantByCle(int cle)
        {
            var tmp = this.ObjectContext.EqEquipement.OfType<EqRaccordIsolant>()
                .Include("Pp").Include("Pp.PortionIntegrite").Include("Pp2")
                .Include("RefEnumValeur").Include("RefSousTypeOuvrage")
                .Include("RefEnumValeur1").Include("RefSousTypeOuvrage1")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqRaccordIsolant>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqTiersCroiseSansLiaison ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqTiersCroiseSansLiaison GetEqTiersCroiseSansLiaisonByCle(int cle)
        {
            EqTiersCroiseSansLiaison tmp = this.ObjectContext.EqEquipement.OfType<EqTiersCroiseSansLiaison>()
                .Include("Pp").Include("Pp.PortionIntegrite")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqTiersCroiseSansLiaison>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqSoutirage ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqSoutirage GetEqSoutirageByCle(int cle)
        {
            EqSoutirage tmp = this.ObjectContext.EqEquipement.OfType<EqSoutirage>()
                .Include("Pp").Include("Pp.PortionIntegrite")
                .Include("RefSousTypeOuvrage").Include("RefSousTypeOuvrage1").Include("EqSoutirageLiaisonsext.EqLiaisonExterne")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqSoutirage>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqLiaisonInterne ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqLiaisonInterne GetEqLiaisonInterneByCle(int cle)
        {
            EqLiaisonInterne tmp = this.ObjectContext.EqEquipement.OfType<EqLiaisonInterne>()
                .Include("Pp").Include("Pp.PortionIntegrite").Include("Pp2")
                .Include("RefSousTypeOuvrage").Include("EqRaccordIsolant1")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqLiaisonInterne>().FirstOrDefault();

            if (tmp.LiaisonInterEe)
            {
                var liaisonInterEE = this.ObjectContext.EqEquipement.OfType<EqLiaisonInterne>().Include("EqRaccordIsolant1").FirstOrDefault(e => e.CleEquipement == tmp.CleLiaisonInterEe);
                tmp.EqLiaisonInterne2 = liaisonInterEE;
            }

            return tmp;
        }

        /// <summary>
        /// Retourne l'équipement de type EqLiaisonExterne ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqLiaisonExterne GetEqLiaisonExterneByCle(int cle)
        {
            EqLiaisonExterne tmp = this.ObjectContext.EqEquipement.OfType<EqLiaisonExterne>()
                .Include("Pp").Include("Pp.PortionIntegrite")
                .Include("EqSoutirageLiaisonsext").Include("EqDrainageLiaisonsext")
                .Include("RefSousTypeOuvrage").Include("RefSousTypeOuvrage1").Include("EqRaccordIsolant1")
                .Include("Images")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.CleEquipement == cle);

            tmp.EqEquipement2Reference.Load();
            // Chargement uniquement de l'élément déplacé
            tmp.EqEquipementEqEquipement.OfType<EqLiaisonExterne>().FirstOrDefault();

            return tmp;
        }

        /// <summary>
        /// Retourne la liste des équipements correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="cleEnsElectrique">clé de l'ensemble électrique</param>
        /// <param name="clePortion">clé de la portion</param>
        /// <param name="includeDeletedEquipment"></param>
        /// <param name="codeEquipement"></param>
        /// <returns>Une liste d'équipement</returns>
        public IQueryable<EqEquipement> FindEqEquipementByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,
            int? cleEnsElectrique, int? clePortion, bool includeDeletedEquipment, string codeEquipement)
        {
            IQueryable<EqEquipement> query;

            if (cleSecteur.HasValue)
            {
                query = this.ObjectContext.EqEquipement.Include("Pp").Include("Pp.PortionIntegrite")
                    .Include("Pp.PortionIntegrite.EnsembleElectrique").Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value)
                    && (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment));
            }
            else if (cleAgence.HasValue)
            {
                query = this.ObjectContext.EqEquipement.Include("Pp").Include("Pp.PortionIntegrite")
                    .Include("Pp.PortionIntegrite.EnsembleElectrique").Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.CleAgence == cleAgence.Value)
                    && (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment));
            }
            else if (cleRegion.HasValue)
            {
                query = this.ObjectContext.EqEquipement.Include("Pp").Include("Pp.PortionIntegrite")
                    .Include("Pp.PortionIntegrite.EnsembleElectrique").Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value)
                    && (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment));
            }
            else
            {
                query = this.ObjectContext.EqEquipement.Include("Pp").Include("Pp.PortionIntegrite")
                    .Include("Pp.PortionIntegrite.EnsembleElectrique").Where(e => (codeEquipement == null || e.TypeEquipement.CodeEquipement == codeEquipement)
                    && (includeDeletedEquipment || e.Supprime == includeDeletedEquipment));
            }

            if (clePortion.HasValue)
            {
                query = query.Where(e => e.Pp.ClePortion == clePortion.Value);
            }
            else if (cleEnsElectrique.HasValue)
            {
                query = query.Where(e => e.Pp.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value);
            }

            return query.OrderBy(ee => ee.Libelle);
        }

        /// <summary>
        /// Récupère la liste des équipements à supprimer logiquement en cascade lié à une PP
        /// </summary>
        /// <param name="cle">identifiant de la PP</param>
        /// <returns>Liste d'équipement</returns>
        public bool CanPhysicalDeleteByEquipement(int cle)
        {
            return !this.ObjectContext.Visites.Any(v => v.CleEquipement == cle && v.EstValidee);
        }

        /// <summary>
        /// Récupère un équipement correspondant à la clé équipement
        /// </summary>
        /// <param name="cle">identifiant de la PP</param>
        /// <returns>Liste d'équipement</returns>
        public EqEquipement GetEqEquipementOnly(int cle)
        {
            return this.ObjectContext.EqEquipement.Where(v => v.CleEquipement == cle).FirstOrDefault();
        }

        /// <summary>
        /// Retourne la liste équipement correspondant à la clé du tableau de clé spécifié
        /// </summary>
        /// <param name="listCleEquipement">Liste de clé d'équipement</param>
        /// <returns>liste d'équipements</returns>
        [Query(HasSideEffects = true)]
        public IQueryable<EqEquipement> FindEqEquipementsByListCle(ObservableCollection<int> listCleEquipement)
        {
            int step = 100;

            if (listCleEquipement.Count > step)
            {
                List<EqEquipement> equipements = new List<EqEquipement>();
                int index = 0;
                while (index < listCleEquipement.Count)
                {
                    IEnumerable<int> list = listCleEquipement.Skip(index).Take(step);
                    if (index == 0)
                    {
                        equipements = this.ObjectContext.EqEquipement
                            .Include("TypeEquipement")
                            .Include("Pp")
                            .Where(a => list.Contains(a.CleEquipement)).ToList();
                    }
                    else
                    {
                        equipements.AddRange(this.ObjectContext.EqEquipement
                            .Include("TypeEquipement")
                            .Include("Pp")
                            .Where(a => list.Contains(a.CleEquipement)).ToList());
                    }
                    index += step;
                }

                return equipements.AsQueryable();
            }
            else
            {
                return this.ObjectContext.EqEquipement
                            .Include("TypeEquipement")
                            .Include("Pp")
                            .Where(a => listCleEquipement.Contains(a.CleEquipement));
            }
        }

        #endregion EqEquipement

        #region EqEquipementTmp

        static readonly Func<ProtecaEntities, int, EqEquipementTmp> getEqEquipementTmpByCle = CompiledQuery.Compile<ProtecaEntities, int, EqEquipementTmp>((ctx, cle) => ctx.EqEquipementTmp.Include("Pp2").Include("Pp2.PortionIntegrite")
                .Include("TypeEquipement").Include("Visites")
                .FirstOrDefault(e => e.CleEqTmp == cle));
        /// <summary>
        /// Retourne l'équipement ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqEquipementTmp GetEqEquipementTmpByCle(int cle)
        {
            return getEqEquipementTmpByCle.Invoke(this.ObjectContext, cle);
        }

        /// <summary>
        /// Retourne les Visites sur des critères Geo et de date
        /// </summary>
        /// <param name="filtreCleRegion"></param>
        /// <param name="filtreCleAgence"></param>
        /// <param name="filtreCleSecteur"></param>
        /// <param name="filtreCleEnsElec"></param>
        /// <param name="filtreClePortion"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <returns></returns>
        public IQueryable<EqEquipementTmp> FindEquipementsTmpByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                                                        int? filtreCleEnsElec, int? filtreClePortion,
                                                                        DateTime? dateMin, DateTime? dateMax, bool? filtreValider)
        {
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;

            ////Initialisation de la query sans resultats
            IQueryable<EqEquipementTmp> queryEqEquipementTmp = this.ObjectContext.EqEquipementTmp.Include("Pp2.PortionIntegrite")
                                                                                                 .Include("Visites.MesMesure.MesTypeMesure.MesModeleMesure.MesUnite")
                                                                                                 .Include("Visites.UsrUtilisateur").Include("Visites.UsrUtilisateur2")
                                                                                                 .Include("Visites.InstrumentsUtilises.InsInstrument")
                                                                                                 .Include("Visites.AnAnalyseSerieMesure");

            if (filtreCleSecteur.HasValue)
            {
                queryEqEquipementTmp = queryEqEquipementTmp.Where(e => e.Pp2.CleSecteur == filtreCleSecteur.Value);
            }
            else if (filtreCleAgence.HasValue)
            {
                queryEqEquipementTmp = queryEqEquipementTmp.Where(e => e.Pp2.GeoSecteur.CleAgence == filtreCleAgence.Value);
            }
            else if (filtreCleRegion.HasValue)
            {
                queryEqEquipementTmp = queryEqEquipementTmp.Where(e => e.Pp2.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
            }

            if (filtreClePortion.HasValue)
            {
                queryEqEquipementTmp = queryEqEquipementTmp.Where(e => e.Pp2.ClePortion == filtreClePortion.Value);
            }
            else if (filtreCleEnsElec.HasValue)
            {
                queryEqEquipementTmp = queryEqEquipementTmp.Where(e => e.Pp2.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
            }

            if (dateMin.HasValue)
            {
                dateMin = dateMin.Value.Date;
                queryEqEquipementTmp = queryEqEquipementTmp.Where(e => e.Visites.Any(v => v.DateImport.HasValue && v.DateImport.Value >= dateMin.Value));
            }
            if (dateMax.HasValue)
            {
                dateMax = dateMax.Value.AddDays(1).Date;
                queryEqEquipementTmp = queryEqEquipementTmp.Where(e => e.Visites.Any(v => v.DateImport.HasValue && v.DateImport.Value < dateMin.Value));
            }

            if (filtreValider.HasValue)
            {
                queryEqEquipementTmp = queryEqEquipementTmp.Where(e => e.EstValide == filtreValider.Value);
            }

            return queryEqEquipementTmp.OrderBy(e => e.Pp2.PortionIntegrite.Libelle).ThenBy(e => e.Pp2.Pk).ThenBy(e => e.TypeEquipement.NumeroOrdre).ThenBy(e => e.Libelle);

        }
        #endregion

        #region EqSoutirageLiaisonsext

        /// <summary>
        /// Retourne l'EqSoutirageLiaisonsext ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqSoutirageLiaisonsext GetEqSoutirageLiaisonsextByCle(int cle)
        {
            return this.ObjectContext.EqSoutirageLiaisonsext.FirstOrDefault(e => e.CleSoutirageLext == cle);
        }

        #endregion EqSoutirageLiaisonsext

        #region EqDrainageLiaisonsext

        /// <summary>
        /// Retourne l'EqDrainageLiaisonsext ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EqDrainageLiaisonsext GetEqDrainageLiaisonsextByCle(int cle)
        {
            return this.ObjectContext.EqDrainageLiaisonsext.FirstOrDefault(e => e.CleDrainageLext == cle);
        }

        #endregion EqSoutirageLiaisonsext

        #region Private Functions

        /// <summary>
        /// On attache les instrument à l'agence parente.
        /// </summary>
        private void AttachInstrumentToParent(object geo)
        {
            if (geo is GeoSecteur)
            {

            }
            else if (geo is GeoAgence)
            {
                foreach (InsInstrument instru in ((GeoAgence)geo).InsInstrument)
                {
                    instru.CleAgence = null;
                    instru.CleRegion = ((GeoAgence)geo).CleRegion;
                }
            }
        }

        #endregion Private Functions+++

        #region PortionIntegrite

        /// <summary>
        /// Retourne les portions de l'ensemble electrique
        /// </summary>
        /// <param name="cle"></param>
        /// <param name="cleRegion"></param>
        /// <returns></returns>
        public IQueryable<PortionIntegrite> GetPortionIntegriteByCleEnsElec(int cle, int cleRegion)
        {
            IQueryable<PortionIntegrite> query = from p in this.ObjectContext.PortionIntegrite
                                                 from pi in this.ObjectContext.PiSecteurs
                                                 where p.CleEnsElectrique == cle
                                                 && pi.ClePortion == p.ClePortion && pi.GeoSecteur != null && pi.GeoSecteur.GeoAgence != null
                                                 && pi.GeoSecteur.GeoAgence.CleRegion == cleRegion
                                                 select p;

            return query.Distinct();
        }

        /// <summary>
        /// Retourne la portion ayant comme identifiant la clé passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public PortionIntegrite GetPortionIntegriteByCle(int cle)
        {
            return this.ObjectContext.PortionIntegrite
                .Include("EnsembleElectrique")
                .Include("PiSecteurs")
                .Include("Pps")
                .Include("MesNiveauProtection")
                .FirstOrDefault(e => e.ClePortion == cle);
        }

        /// <summary>
        /// Retour la portion avec les équipement associé
        /// </summary>
        /// <param name="cle">clé de la portion</param>
        /// <returns></returns>
        public PortionIntegrite GetPortionIntegriteByCleWithEqEquipements(int cle)
        {
            return this.ObjectContext.PortionIntegrite
                .Include("EnsembleElectrique")
                .Include("PiSecteurs")
                .Include("Pps.Visites")
                .Include("Pps.PpJumelee.Pp1")
                .Include("Pps.PpJumelee1.Pp")
                .Include("Pps.EqEquipement.Visites")
                .Include("MesNiveauProtection")
                .Include("Compositions")
                .FirstOrDefault(e => e.ClePortion == cle);
        }


        /// <summary>
        /// Vérifie si la portion peut être supprimée Physiquement ou logiquement.
        /// Supprime les PP et equipements associés
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public int DeletePortionIntegriteAndCascade(int clePortion, int cleUtilisateur)
        {
            int sortie = 0;

            PortionIntegrite pi = this.ObjectContext.PortionIntegrite.FirstOrDefault(p => p.ClePortion == clePortion);
            pi.Supprime = true;

            DateTime dateLog = DateTime.Now;

            IEnumerable<Pp> PPs = pi.Pps.Where(p => !p.Supprime);
            int nbPP = PPs.Count();
            for (int i = nbPP - 1; i >= 0; i--)
            {
                Pp pp = PPs.ElementAt(i);
                pp.Supprime = true;
                pp.BypassCategoriePp = true;

                //Gestion des jumelages
                int nbJumelages = (pp.PpJumelee.Union(pp.PpJumelee1)).Count();
                for (int j = 0; j < nbJumelages; j++)
                {
                    //Récupération du jumelage
                    PpJumelee jumelage = (pp.PpJumelee.Union(pp.PpJumelee1)).FirstOrDefault();
                    //Récupération de la Pp jumelée
                    Pp ppJumelee = jumelage.Pp == pp ? jumelage.Pp1 : jumelage.Pp;
                    //Suppression du jumelage
                    this.ObjectContext.DeleteObject(jumelage);
                    //Si la pp jumelée n'est pas sur la portion on ajoute un log de modification sur celle-ci
                    if (!pi.Pps.Contains(ppJumelee))
                    {
                        ppJumelee.LogOuvrage.Add(new LogOuvrage
                        {
                            CleUtilisateur = cleUtilisateur,
                            DateHistorisation = dateLog,
                            ListeChamps = ResourceHisto.PpJumelee,
                            RefEnumValeur = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "M")
                        });
                    }
                }

                //Gestion de la suppression des clé pp secondaires sur les équipements ou celle-ci est facultative
                IEnumerable<EqFourreauMetallique> EqFM = pp.EqFourreauMetallique.Where(fm => fm.Pp.PortionIntegrite != pi);
                int nbEqFM = EqFM.Count();
                for (int j = nbEqFM - 1; j >= 0; j--)
                {
                    EqFourreauMetallique eqfm = EqFM.ElementAt(j);

                    eqfm.ClePp2 = null;

                    if (!eqfm.Supprime)
                    {
                        eqfm.LogOuvrage.Add(new LogOuvrage
                        {
                            CleUtilisateur = cleUtilisateur,
                            DateHistorisation = dateLog,
                            ListeChamps = ResourceHisto.ClePp2,
                            RefEnumValeur = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "M")
                        });
                    }
                }
                IEnumerable<EqRaccordIsolant> EqRI = pp.EqRaccordIsolant.Where(fm => fm.Pp.PortionIntegrite != pi);
                int nbEqRI = EqFM.Count();
                for (int j = nbEqRI - 1; j >= 0; j--)
                {
                    EqRaccordIsolant eqri = EqRI.ElementAt(j);

                    eqri.ClePp2 = null;

                    if (!eqri.Supprime)
                    {
                        eqri.LogOuvrage.Add(new LogOuvrage
                        {
                            CleUtilisateur = cleUtilisateur,
                            DateHistorisation = dateLog,
                            ListeChamps = ResourceHisto.ClePp2,
                            RefEnumValeur = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "M")
                        });
                    }
                }

                //Gestion de la suppression des visites non validées
                IEnumerable<Visite> Visites = pp.Visites.Where(v => !v.EstValidee);
                int nbVisites = Visites.Count();
                for (int j = nbEqFM - 1; j >= 0; j--)
                {
                    this.ObjectContext.DeleteObject(Visites.ElementAt(j));
                }

                //Gestion de la suppression des équipements avec lien obligatoire
                IEnumerable<EqEquipement> Eqs = (pp.EqEquipement.Where(p => !p.Supprime).Union(pp.EqLiaisonInterne.Where(p => !p.Supprime))).Distinct();
                int nbEq = Eqs.Count();
                for (int j = nbEq - 1; j >= 0; j--)
                {
                    EqEquipement eq = Eqs.ElementAt(j);
                    eq.Supprime = true;

                    //Gestion des soutirages et drainages liaisons ext
                    IEnumerable<EqLiaisonExterne> EqLElies =
                        (eq is EqSoutirage)
                        ? (eq as EqSoutirage).EqSoutirageLiaisonsext.Select(sl => sl.EqLiaisonExterne).Where(e => e.Pp.PortionIntegrite != pi)
                        : (eq is EqDrainage)
                            ? (eq as EqDrainage).EqDrainageLiaisonsext.Select(sl => sl.EqLiaisonExterne).Where(e => e.Pp.PortionIntegrite != pi)
                            : new List<EqLiaisonExterne>();
                    foreach (EqLiaisonExterne eqlelie in EqLElies.Where(e => !e.Supprime))
                    {
                        string listeChamps = (eq is EqSoutirage) ? ResourceHisto.EqSoutirageLiaisonsext : ResourceHisto.EqDrainageLiaisonsext;

                        eqlelie.LogOuvrage.Add(new LogOuvrage
                        {
                            CleUtilisateur = cleUtilisateur,
                            DateHistorisation = dateLog,
                            ListeChamps = listeChamps,
                            RefEnumValeur = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "M")
                        });
                    }

                    //Gestion des raccord isolants liés
                    IEnumerable<EqRaccordIsolant> EqRIlies = eq.EqRaccordIsolant1.Where(e => e.Pp.PortionIntegrite != pi);
                    foreach (EqRaccordIsolant eqrilie in EqRIlies.Where(e => !e.Supprime))
                    {
                        eqrilie.CleLiaison = null;
                        eqrilie.LogOuvrage.Add(new LogOuvrage
                        {
                            CleUtilisateur = cleUtilisateur,
                            DateHistorisation = dateLog,
                            ListeChamps = ResourceHisto.CleLiaison,
                            RefEnumValeur = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "M")
                        });
                    }

                    //Gestion de la suppression des visites non validées
                    IEnumerable<Visite> VisitesEq = eq.Visites.Where(v => !v.EstValidee);
                    int nbVisitesEq = Visites.Count();
                    for (int k = nbEqFM - 1; k >= 0; k--)
                    {
                        this.ObjectContext.DeleteObject(Visites.ElementAt(k));
                    }

                    bool physically = !eq.Visites.Any();
                    if (physically)
                    {
                        this.ObjectContext.DeleteObject(eq);
                    }
                    else
                    {
                        eq.LogOuvrage.Add(new LogOuvrage
                        {
                            CleUtilisateur = cleUtilisateur,
                            DateHistorisation = dateLog,
                            RefEnumValeur = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "S")
                        });
                    }
                    addLogTourneeOnCompositionsForPpOrEq(cleUtilisateur, eq, true);
                }

                if (!pp.EqEquipement.Any() && !pp.EqLiaisonInterne.Any() && !pp.Visites.Any())
                {
                    this.ObjectContext.DeleteObject(pp);
                }
                else
                {
                    pp.LogOuvrage.Add(new LogOuvrage
                    {
                        CleUtilisateur = cleUtilisateur,
                        DateHistorisation = dateLog,
                        RefEnumValeur = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "S")
                    });
                }

                addLogTourneeOnCompositionsForPpOrEq(cleUtilisateur, pp, true);
            }

            if (!pi.Pps.Any())
            {
                this.ObjectContext.DeleteObject(pi);
                sortie = 1;
            }
            else
            {
                pi.LogOuvrage.Add(new LogOuvrage
                {
                    CleUtilisateur = cleUtilisateur,
                    DateHistorisation = dateLog,
                    RefEnumValeur = this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CodeGroupe == "TYPE_LOG_OUVRAGE" && r.Valeur == "S")
                });
                sortie = 2;
            }
            addLogTourneeOnCompositionsForPpOrEq(cleUtilisateur, pi, true);

            this.ObjectContext.SaveChanges();

            return sortie;
        }

        /// <summary>
        /// Retourne les portions intégrités recherchés
        /// </summary>
        /// <param name="cleRegion"></param>
        /// <param name="cleAgence"></param>
        /// <param name="cleSecteur"></param>
        /// <param name="cleEnsElec"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public IQueryable<PortionIntegrite> FindPortionIntegritesByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,
           int? cleEnsElec, bool isDelete, bool IsPosteGaz, bool isStation)
        {
            IQueryable<PortionIntegrite> query;
            if (cleSecteur.HasValue)
            {
                query = this.ObjectContext.PortionIntegrite.Where(p => p.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value));
            }
            else if (cleAgence.HasValue)
            {
                query = this.ObjectContext.PortionIntegrite.Where(p => p.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleAgence == cleAgence.Value));
            }
            else if (cleRegion.HasValue)
            {
                query = this.ObjectContext.PortionIntegrite.Where(p => p.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.GeoRegion.CleRegion == cleRegion.Value));
            }
            else
            {
                query = this.ObjectContext.PortionIntegrite;
            }

            if (cleEnsElec.HasValue)
            {
                query = query.Where(r => r.CleEnsElectrique == cleEnsElec);
            }

            if (IsPosteGaz || isStation)
            {
                //Inclue les stations ou les poste Gaz ou non
                query = query.Where(ee =>
                    (isStation && ee.EnsembleElectrique.RefEnumValeur1.Valeur == "1")
                    || (IsPosteGaz && ee.EnsembleElectrique.RefEnumValeur1.Valeur == "2"));
            }

            return query.Where(i => i.Supprime == isDelete || isDelete).OrderBy(ee => ee.Libelle);
        }

        /// <summary>
        /// Retourne les données pour créer des graphes de suivi des mesures sur une portion
        /// </summary>
        /// <param name="clePortion"></param>
        /// <param name="dateDebut"></param>
        /// <param name="dateFin"></param>
        /// <returns></returns>
        public IQueryable<SelectPortionGraphique_Result> GetPortionGraphique(int clePortion, DateTime dateDebut, DateTime dateFin)
        {
            return this.ObjectContext.SelectPortionGraphique(clePortion, dateDebut, dateFin).AsQueryable();
        }

        #endregion

        #region GeoEnsembleElectrique

        /// <summary>
        /// Retourne la vue GeoEnsembleElectrique ayant comme identifiant la clé passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public GeoEnsembleElectrique GetGeoEnsembleElectriqueByCle(int cle)
        {
            return this.ObjectContext.GeoEnsembleElectrique.FirstOrDefault(e => e.CleEnsElectrique == cle);
        }

        #endregion

        #region GeoEnsElecPortion

        /// <summary>
        /// Retourne la vue GeoEnsElecPortion
        /// </summary>
        /// <returns></returns>
        public IQueryable<GeoEnsElecPortion> GetGeoEnsElecPortionNoTracking()
        {
            this.ObjectContext.GeoEnsElecPortion.MergeOption = MergeOption.NoTracking;
            return this.ObjectContext.GeoEnsElecPortion;
        }

        #endregion GeoEnsElecPortion

        #region GeoEnsElecPortionEqPp

        /// <summary>
        /// Retourne la vue GeoEnsElecPortion
        /// </summary>
        /// <returns></returns>
        public IQueryable<GeoEnsElecPortionEqPp> GetGeoEnsElecPortionEqPpNoTracking()
        {
            this.ObjectContext.GeoEnsElecPortionEqPp.MergeOption = MergeOption.NoTracking;
            return this.ObjectContext.GeoEnsElecPortionEqPp;
        }

        #endregion GeoEnsElecPortionEqPp

        #region PpEquipement

        /// <summary>
        /// Retourne la vue PpEquipement
        /// </summary>
        /// <returns></returns>
        public IQueryable<PpEquipement> GetPpEquipementNoTracking()
        {
            this.ObjectContext.PpEquipement.MergeOption = MergeOption.NoTracking;
            return this.ObjectContext.PpEquipement;
        }

        /// <summary>
        /// Retourne la vue équipement filtré par clé PP
        /// </summary>
        /// <param name="cle">clé PP</param>
        /// <returns></returns>
        public IQueryable<PpEquipement> GetPpEquipementNoTrackingByClePp(int cle)
        {
            this.ObjectContext.PpEquipement.MergeOption = MergeOption.NoTracking;
            return this.ObjectContext.PpEquipement.Where(r => r.ClePp == cle);
        }

        /// <summary>
        /// Retourne la vue équipement filtré par clé Portion
        /// </summary>
        /// <param name="cle">clé Portion</param>
        /// <returns></returns>
        public IQueryable<PpEquipement> GetPpEquipementNoTrackingByClePortion(int cle)
        {
            this.ObjectContext.PpEquipement.MergeOption = MergeOption.NoTracking;
            return this.ObjectContext.PpEquipement.Where(r => r.ClePortion == cle);
        }

        #endregion

        #region RefDiametre

        /// <summary>
        /// Retourne la vue RefDiametre ayant comme identifiant la clé passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public RefDiametre GetRefDiametreByCle(int cle)
        {
            return this.ObjectContext.RefDiametre.FirstOrDefault(e => e.CleDiametre == cle);
        }

        #endregion

        #region RefRevetement

        /// <summary>
        /// Retourne la vue RefRevetement ayant comme identifiant la clé passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public RefRevetement GetRefRevetementByCle(int cle)
        {
            return this.ObjectContext.RefRevetement.FirstOrDefault(e => e.CleRevetement == cle);
        }

        #endregion

        #region RefCommune

        /// <summary>
        /// Retourne la vue RefCommune ayant comme identifiant la clé passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public RefCommune GetRefCommuneByCle(int cle)
        {
            return this.ObjectContext.RefCommune.FirstOrDefault(e => e.CleCommune == cle);
        }

        /// <summary>
        /// Retourne la liste des communes
        /// </summary>
        /// <returns></returns>
        public IQueryable<RefCommune> GetRefCommuneNoTracking()
        {
            this.ObjectContext.RefCommune.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return this.ObjectContext.RefCommune;
        }

        #endregion

        #region HistoAdmin

        /// <summary>
        /// Retourne l'historique de l'utilisateur
        /// </summary>
        /// <param name="id">correspond à l'identifiant Gaïa</param>
        /// <returns></returns>
        public IQueryable<HistoAdmin> GetHistoAdminByUser(string id)
        {
            return this.ObjectContext.HistoAdmin.Where(e => e.IdUtilisateur == id);
        }

        /// <summary>
        /// Retourne l'historique sélectionné
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public HistoAdmin GetHistoAdminByCle(int cle)
        {
            return this.ObjectContext.HistoAdmin.FirstOrDefault(e => e.CleHistoAdmin == cle);
        }

        #endregion

        #region Pp



        static readonly Func<ProtecaEntities, int, Pp> getPpByClePpQuery = CompiledQuery.Compile<ProtecaEntities, int, Pp>((ctx, p) => ctx.Pps
                .Include("MesNiveauProtection")
                .Include("PpJumelee").Include("PpJumelee.Pp1")
                .Include("PpJumelee1")
                .Include("Pp1").Include("Pp2")
                .Include("PortionIntegrite").Include("PortionIntegrite.PiSecteurs").Include("PortionIntegrite.EnsembleElectrique")
                .Include("RefEnumValeur").Include("RefEnumValeur1")
                .Include("RefEnumValeur2").Include("RefEnumValeur2")
                .Include("RefEnumValeur3")
                .Include("RefNiveauSensibilitePp")
                .Include("CategoriePp")
                .Include("UsrUtilisateur")
                .Include("UsrUtilisateur1")
                .Include("RefCommune")
                .Include("Images")
                .FirstOrDefault(c => c.ClePp == p));

        static readonly Func<ProtecaEntities, int, IQueryable<Visite>> getPpByClePpVisitesQuery = CompiledQuery.Compile<ProtecaEntities, int, IQueryable<Visite>>((ctx, p) => ctx.Visites
                .Include("MesMesure").Include("MesMesure.MesTypeMesure").Include("MesMesure.MesTypeMesure.MesModeleMesure")
                .Include("InstrumentsUtilises")
                .Include("AnAnalyseSerieMesure").Include("AnAnalyseSerieMesure.RefEnumValeur")
                .Where(v => v.ClePp == p && v.RefEnumValeur.Valeur == "2"));


        /// <summary>
        /// Retourne la Pp ayant comme identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle">identifiant de la Pp recherhcée</param>
        /// <returns></returns>
        public Pp GetPpByCle(int cle)
        {
            Pp tmp = getPpByClePpQuery.Invoke(this.ObjectContext, cle);

            var visites = getPpByClePpVisitesQuery.Invoke(this.ObjectContext, cle);

            if (visites.Count() > 0)
            {
                tmp.Visites.Add(visites.OrderByDescending(v => v.DateVisite).FirstOrDefault());
            }

            return tmp;
        }

        static readonly Func<ProtecaEntities, int, Pp> getDeplacementPpByCleQuery = CompiledQuery.Compile<ProtecaEntities, int, Pp>((ctx, cle) =>
                  ctx.Pps.Include("MesNiveauProtection").Include("MesNiveauProtection.MesModeleMesure").Include("MesNiveauProtection.MesModeleMesure.MesUnite")
                 .Include("PpJumelee").Include("PpJumelee.Pp1")
                 .Include("PpJumelee1")
                 .Include("Pp1").Include("Pp2")
                 .Include("RefEnumValeur")
                 .Include("RefEnumValeur1")
                 .Include("RefEnumValeur2")
                 .Include("RefEnumValeur3")
                 .Include("RefNiveauSensibilitePp")
                 .Include("CategoriePp")
                 .Include("UsrUtilisateur")
                 .Include("RefCommune")
                 .Include("Compositions")
                 .Include("Visites")
                 .Include("EqRaccordIsolant")
                 .Include("EqFourreauMetallique")
                 .Include("EqLiaisonInterne")
                 .Where(p => p.ClePp == cle).FirstOrDefault());
        static readonly Func<ProtecaEntities, int, IQueryable<EqEquipement>> getDeplacementPpByCleEquipementsQuery = CompiledQuery.Compile<ProtecaEntities, int, IQueryable<EqEquipement>>((ctx, p) =>
            ctx.EqEquipement.Include("TypeEquipement").Where(e => e.ClePp == p));
        static readonly Func<ProtecaEntities, int, PortionIntegrite> getDeplacementPpByClePortionQuery = CompiledQuery.Compile<ProtecaEntities, int, PortionIntegrite>((ctx, p) => ctx.PortionIntegrite.Include("PiSecteurs").Include("EnsembleElectrique").Where(c => c.ClePortion == p).FirstOrDefault());
        /// <summary>
        /// Retourne la Pp ayant comme identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle">identifiant de la Pp recherhcée</param>
        /// <returns></returns>
        public Pp GetDeplacementPpByCle(int cle)
        {
            //Chargement "en plusieurs fois" de la pp pour éviter la génération d'une requête sql trop complexe
            //var equipements = this.ObjectContext.EqEquipement.Include("TypeEquipement").Where(e => e.ClePp == cle).ToList();
            var equipements = getDeplacementPpByCleEquipementsQuery.Invoke(this.ObjectContext, cle).ToList();
            // Utilisation d'une requête compilée pour optimiser l'exécution de la même requête avec différents paramètres.
            //var pp = this.ObjectContext.Pps
            //     .Include("MesNiveauProtection")
            //     .Include("PpJumelee").Include("PpJumelee.Pp1")
            //     .Include("PpJumelee1")
            //     .Include("Pp1").Include("Pp2")
            //     .Include("RefEnumValeur")
            //     .Include("RefEnumValeur1")
            //     .Include("RefEnumValeur2")
            //     .Include("RefEnumValeur3")
            //     .Include("RefNiveauSensibilitePp")
            //     .Include("CategoriePp")
            //     .Include("UsrUtilisateur")
            //     .Include("RefCommune")
            //     .Where(p => p.ClePp == cle).FirstOrDefault();
            var pp = getDeplacementPpByCleQuery.Invoke(this.ObjectContext, cle);
            if (pp != null)
            {
                //this.ObjectContext.PortionIntegrite.Include("PiSecteurs").Include("EnsembleElectrique").Where(c => c.ClePortion == pp.ClePortion).FirstOrDefault();
                var portion = getDeplacementPpByClePortionQuery.Invoke(this.ObjectContext, pp.ClePortion);
            }
            return pp;
        }

        /// <summary>
        /// Retourne la Pp ayant comme identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle">identifiant de la Pp recherhcée</param>
        /// <returns></returns>
        public Pp GetPpByCleAndDateAndTypeEval(int cle, DateTime? datedebut, DateTime? datefin, int enumTypeEval)
        {
            Pp tmp;
            if (datedebut.HasValue && datefin.HasValue)
            {
                tmp = this.ObjectContext.Pps
                  .Include("MesNiveauProtection").Include("PpJumelee").Include("PpJumelee.Pp1").Include("PpJumelee1")
                  .Include("PortionIntegrite").Include("PortionIntegrite.PiSecteurs").Include("PortionIntegrite.EnsembleElectrique")
                  .Include("RefEnumValeur").Include("RefEnumValeur1").Include("RefEnumValeur2").Include("RefEnumValeur3")
                  .Include("RefNiveauSensibilitePp").Include("CategoriePp")
                  .Include("PpJumelee").Include("PpJumelee1")
                  .FirstOrDefault(p => p.ClePp == cle);

                DateTime LastVisiteDate = datefin.Value;

                var query = this.ObjectContext.Visites.Include("Alertes")
                    .Include("UsrUtilisateur").Include("UsrUtilisateur1").Include("UsrUtilisateur2").Include("UsrUtilisateur3")
                    .Include("MesMesure.Alertes")
                    .Include("MesMesure.MesTypeMesure.MesModeleMesure")
                    .Include("InstrumentsUtilises.InsInstrument")
                    .Include("AnAnalyseSerieMesure.RefEnumValeur").Include("AnAnalyseSerieMesure.Alertes")
                    .Where(v => v.ClePp == cle && v.EstValidee);

                var visites = query
                    .Where(v => v.DateVisite.Value >= datedebut && v.DateVisite.Value <= datefin
                            && v.EnumTypeEvalComposition == enumTypeEval);

                var lastVisite = visites.OrderByDescending(v => v.DateVisite).ThenByDescending(v => v.CleVisite).FirstOrDefault();
                if (lastVisite != null)
                {
                    tmp.Visites.Add(lastVisite);

                    if (lastVisite != null)
                    {
                        var previousVisites = query
                            .Where(v => v.DateVisite.Value < lastVisite.DateVisite.Value
                            && (v.EnumTypeEval == enumTypeEval || (this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CleEnumValeur == enumTypeEval).Valeur == "1" && v.RefEnumValeur.Valeur == "2"))
                            && !v.Telemesure);
                        var previousVisite = previousVisites.OrderByDescending(v => v.DateVisite).ThenByDescending(v => v.CleVisite).FirstOrDefault();
                        if (previousVisite != null)
                            tmp.Visites.Add(previousVisite);
                    }
                }

            }
            else
                tmp = GetPpByCle(cle);
            return tmp;
        }
        /// <summary>
        /// Récupère la liste des équipements à supprimer logiquement en cascade lié à une PP
        /// </summary>
        /// <param name="cle">identifiant de la PP</param>
        /// <returns>un code pour la suppression 1=>suppression physique, 2=> suppression logique, 3=> suppression impossible</returns>
        public int GetDeleteCodeByPP(int cle)
        {
            int code = 0;

            IQueryable<EqEquipement> equipements = this.ObjectContext.EqEquipement.Where(e => e.ClePp == cle)
                                                   .Union(this.ObjectContext.EqEquipement.OfType<EqLiaisonInterne>().Where(e => e.ClePp2 == cle));

            bool equipements2 = this.ObjectContext.EqEquipement.OfType<EqFourreauMetallique>().Any(e => e.ClePp2.HasValue && e.ClePp2.Value == cle)
                                || this.ObjectContext.EqEquipement.OfType<EqRaccordIsolant>().Any(e => e.ClePp2.HasValue && e.ClePp2.Value == cle);

            if (equipements.Any(e => !e.Supprime))
            {
                code = 1;
            }
            else if (equipements.Any() || this.ObjectContext.Visites.Any(v => v.ClePp == cle && v.EstValidee))
            {
                code = 2;
            }
            else
                code = 3;

            return equipements2 ? code + 3 : code;
        }

        /// <summary>
        /// Retourne la liste des équipements rattachés à la pp ayant comme identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle">identifinat de la pp</param>
        /// <returns>Liste d'équipement lié à une pp</returns>
        public IQueryable<EqEquipement> GetListEquipementByPP(int cle)
        {
            IQueryable<EqEquipement> QueryToReturn = this.ObjectContext.EqEquipement.Where(e => e.ClePp == cle && e.Supprime == false);

            QueryToReturn = QueryToReturn.Union(this.ObjectContext.EqEquipement.OfType<EqLiaisonInterne>().Where(e => e.ClePp2 == cle && e.Supprime == false));
            QueryToReturn = QueryToReturn.Union(this.ObjectContext.EqEquipement.OfType<EqFourreauMetallique>().Where(e => e.ClePp2.HasValue && e.ClePp2.Value == cle && e.Supprime == false));
            QueryToReturn = QueryToReturn.Union(this.ObjectContext.EqEquipement.OfType<EqRaccordIsolant>().Where(e => e.ClePp2.HasValue && e.ClePp2.Value == cle && e.Supprime == false));
            QueryToReturn = QueryToReturn.OrderBy(e => e.Libelle);

            return QueryToReturn;
        }

        /// <summary>
        /// Retourne la liste des équipements rattachés comme secondaire et non obligatoire à la pp ayant comme identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle">identifiant de la pp</param>
        /// <returns>Liste d'équipement lié à une pp</returns>
        public IQueryable<EqEquipement> GetListEquipementByPP2(int cle)
        {
            IQueryable<EqEquipement> QueryToReturn = this.ObjectContext.EqEquipement.OfType<EqFourreauMetallique>().Where(e => e.ClePp2.HasValue && e.ClePp2.Value == cle);
            QueryToReturn = QueryToReturn.Union(this.ObjectContext.EqEquipement.OfType<EqRaccordIsolant>().Where(e => e.ClePp2.HasValue && e.ClePp2.Value == cle));
            QueryToReturn = QueryToReturn.OrderBy(e => e.Libelle);

            return QueryToReturn;
        }

        /// <summary>
        /// Retourne la liste des tournees d'un équipement
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<Tournee> GetListTourneesByPp(int cle)
        {
            return this.ObjectContext.Tournees.Where(t => !t.Supprime && t.Compositions.Any(c => c.ClePp == cle)).OrderBy(t => t.Libelle);
        }

        /// <summary>
        /// Retourne la liste des Pp correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="cleAgence">clé de l'ensemble électrique</param>
        /// <param name="cleSecteur">clé de la portion</param>
        /// <returns>Une liste de pp</returns>
        public IQueryable<Pp> FindPpByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,
            int? cleEnsElectrique, int? clePortion, bool includeDeletedEquipment)
        {
            IQueryable<Pp> query;

            if (cleSecteur.HasValue)
            {

                query = this.ObjectContext.Pps.Include("PortionIntegrite")
                    .Include("PortionIntegrite.EnsembleElectrique").Where(p => (p.PortionIntegrite.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value))
                    && (includeDeletedEquipment || p.Supprime == includeDeletedEquipment) && p.CleSecteur == cleSecteur.Value);
                // Correction mantis 19965 : restriction sur le secteur de la PP (et pas seulement sur les secteurs de la PI)
            }
            else if (cleAgence.HasValue)
            {
                query = this.ObjectContext.Pps.Include("PortionIntegrite")
                    .Include("PortionIntegrite.EnsembleElectrique").Where(p => (p.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.CleAgence == cleAgence.Value))
                   && (includeDeletedEquipment || p.Supprime == includeDeletedEquipment));
            }
            else if (cleRegion.HasValue)
            {
                query = this.ObjectContext.Pps.Include("PortionIntegrite")
                    .Include("PortionIntegrite.EnsembleElectrique").Where(p => (p.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value))
                    && (includeDeletedEquipment || p.Supprime == includeDeletedEquipment));
            }
            else
            {
                query = this.ObjectContext.Pps.Include("PortionIntegrite")
                    .Include("PortionIntegrite.EnsembleElectrique").Where(p => (includeDeletedEquipment || p.Supprime == includeDeletedEquipment));
            }


            if (clePortion.HasValue)
            {
                query = query.Where(p => (p.ClePortion == clePortion.Value));
            }
            else if (cleEnsElectrique.HasValue)
            {
                query = query.Where(p => (p.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value));
            }

            return query.OrderBy(p => p.Pk).ThenBy(p => p.Libelle);
        }

        /// <summary>
        /// Retourne la liste des Pps ayant comme portion de rattachement la portion ayant comme identifiant la clé passé en paramètre
        /// </summary>
        /// <param name="clePortion">identifiant de la portion </param>
        /// <returns>la liste des Pps</returns>
        public IQueryable<Pp> GetPpsByClePortion(int clePortion)
        {
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            return this.ObjectContext.Pps.Where(p => p.ClePortion == clePortion).OrderBy(p => p.Libelle);
        }

        /// <summary>
        /// Retourne la liste des Pps ayant comme portion de rattachement la portion ayant comme identifiant la clé passé en paramètre
        /// </summary>
        /// <param name="clePortion">identifiant de la portion </param>
        /// <returns>la liste des Pps</returns>
        public IQueryable<Pp> GetPpsAndPpJumeleeByClePortion(int clePortion)
        {
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            return this.ObjectContext.Pps.Include("PpJumelee").Include("PpJumelee1").Where(p => !p.Supprime && p.ClePortion == clePortion).OrderBy(p => p.Libelle);
        }

        /// <summary>
        /// Retourne la liste équipement correspondant à la clé du tableau de clé spécifié
        /// </summary>
        /// <param name="listCleEquipement">Liste de clé d'équipement</param>
        /// <returns>liste d'équipements</returns>
        [Query(HasSideEffects = true)]
        public IQueryable<Pp> FindPpsByListCle(ObservableCollection<int> listClePp)
        {
            int step = 100;

            if (listClePp.Count > step)
            {
                List<Pp> Pps = new List<Pp>();
                int index = 0;
                while (index < listClePp.Count)
                {
                    IEnumerable<int> list = listClePp.Skip(index).Take(step);
                    if (index == 0)
                    {
                        Pps = this.ObjectContext.Pps.Include("PpJumelee").Include("PpJumelee1").Where(a => list.Contains(a.ClePp)).ToList();
                    }
                    else
                    {
                        Pps.AddRange(this.ObjectContext.Pps.Include("PpJumelee").Include("PpJumelee1").Where(a => list.Contains(a.ClePp)).ToList());
                    }
                    index += step;
                }

                return Pps.AsQueryable();
            }
            else
            {
                return this.ObjectContext.Pps.Include("PpJumelee").Include("PpJumelee1")
                                             .Where(a => listClePp.Contains(a.ClePp));
            }
        }

        #endregion Pp

        #region PpTmp

        static readonly Func<ProtecaEntities, int, PpTmp> getPpTmpByCle = CompiledQuery.Compile<ProtecaEntities, int, PpTmp>((ctx, cle) => ctx.PpTmp
                .Include("Pp.PortionIntegrite")
                .Include("Pp.CategoriePp")
                .Include("Pp.RefEnumValeur")
                .Include("Pp.RefEnumValeur1")
                .Include("Pp.RefEnumValeur2")
                .Include("Pp.RefEnumValeur3")
                .Include("Pp.RefNiveauSensibilitePp")
                .Include("CategoriePp")
                .Include("RefEnumValeur")
                .Include("RefEnumValeur1")
                .Include("RefEnumValeur2")
                .Include("RefEnumValeur3")
                .Include("RefNiveauSensibilitePp")
                .Include("Visites")
                .FirstOrDefault(e => e.ClePpTmp == cle));
        /// <summary>
        /// Retourne l'équipement ayant pour identifiant la clé passée en paramètre.
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public PpTmp GetPpTmpByCle(int cle)
        {
            return getPpTmpByCle.Invoke(this.ObjectContext, cle);
        }

        /// <summary>
        /// Retourne les Pp tmp sur des critères Geo et de date
        /// </summary>
        /// <param name="filtreCleRegion"></param>
        /// <param name="filtreCleAgence"></param>
        /// <param name="filtreCleSecteur"></param>
        /// <param name="filtreCleEnsElec"></param>
        /// <param name="filtreClePortion"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <returns></returns>
        public IQueryable<PpTmp> FindPpTmpByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                                                        int? filtreCleEnsElec, int? filtreClePortion,
                                                                        DateTime? dateMin, DateTime? dateMax)
        {
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;

            ////Initialisation de la query sans resultats
            IQueryable<PpTmp> queryPpTmp = this.ObjectContext.PpTmp.Include("Pp.PortionIntegrite")
                                                    .Include("Visites.MesMesure.MesTypeMesure.MesModeleMesure.MesUnite")
                                                    .Include("Visites.UsrUtilisateur").Include("Visites.UsrUtilisateur2")
                                                    .Include("Visites.InstrumentsUtilises.InsInstrument")
                                                    .Include("Visites.AnAnalyseSerieMesure");

            if (filtreCleSecteur.HasValue)
            {
                queryPpTmp = queryPpTmp.Where(p => p.Pp.CleSecteur == filtreCleSecteur.Value);
            }
            else if (filtreCleAgence.HasValue)
            {
                queryPpTmp = queryPpTmp.Where(p => p.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
            }
            else if (filtreCleRegion.HasValue)
            {
                queryPpTmp = queryPpTmp.Where(p => p.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
            }

            if (filtreClePortion.HasValue)
            {
                queryPpTmp = queryPpTmp.Where(p => p.Pp.ClePortion == filtreClePortion.Value);
            }
            else if (filtreCleEnsElec.HasValue)
            {
                queryPpTmp = queryPpTmp.Where(p => p.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
            }

            if (dateMin.HasValue)
            {
                dateMin = dateMin.Value.Date;
                queryPpTmp = queryPpTmp.Where(p => p.Visites.Any(v => v.DateImport.HasValue && v.DateImport.Value >= dateMin.Value));
            }
            if (dateMax.HasValue)
            {
                dateMax = dateMax.Value.AddDays(1).Date;
                queryPpTmp = queryPpTmp.Where(p => p.Visites.Any(v => v.DateImport.HasValue && v.DateImport.Value < dateMin.Value));
            }

            return queryPpTmp.OrderBy(p => p.Pp.PortionIntegrite.Libelle).ThenBy(p => p.Pp.Pk).ThenBy(p => p.Pp.Libelle);

        }
        #endregion

        #region RefSousTypeOuvrage

        /// <summary>
        /// Retourne l'objet RefSousTypeOuvrage ayant pour identifiant la cle passé en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public RefSousTypeOuvrage GetRefSousTypeOuvrageByCle(int cle)
        {
            return this.ObjectContext.RefSousTypeOuvrage.FirstOrDefault(r => r.CleSousTypeOuvrage == cle);
        }

        /// <summary>
        /// Vérifie les condition de suppression d'un sous type d'ouvrage.
        /// Si les conditions sont remplis on supprime l'objet.
        /// </summary>
        /// <param name="cle">identifian du sous type d' ouvrage à supprimer</param>
        /// <returns>True si la suppression a été réalisée, False sinon</returns>
        public bool CheckCanDeleteRefSousTypeOuvrageByCle(string groupName, int cle)
        {
            bool result = false;
            switch (groupName)
            {
                case "TypeAnode":
                case "TypeAnomalie":
                case "TypeDeversoirPile":
                case "TypeDeversoirSoutirage":
                case "TypeDrainage":
                case "TypeLiaison":
                case "TypeNomTiers":
                    {
                        result = !this.ObjectContext.EqEquipement.OfType<EqLiaisonExterne>().Any(e => e.CleNomTiersAss == cle);
                        break;
                    }
                case "TypePriseTerre":
                case "TypeRaccord":
                case "TypeRedresseur":
                    {
                        result = !this.ObjectContext.EqEquipement.OfType<EqSoutirage>().Any(e => e.CleRedresseur == cle);
                        break;
                    }
                default:
                    return false;
            }

            return result;
        }

        #endregion RefSousTypeOuvrage

        #region MesCoutMesure

        /// <summary>
        /// Retourne les couts de mesures avec les refenumvaleur
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<MesCoutMesure> GetMesCoutMesureWithDependancy()
        {
            return this.ObjectContext.MesCoutMesure.Include("RefEnumValeur");
        }

        /// <summary>
        /// Retourne le cout de mesure sélectionné
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public MesCoutMesure GetMesCoutMesureByCle(int cle)
        {
            return this.ObjectContext.MesCoutMesure.FirstOrDefault(e => e.CleCoutMesure == cle);
        }

        #endregion

        #region ParametreAction

        /// <summary>
        /// Retourne les paramètres d'action avec les refenumvaleur
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<ParametreAction> GetParametreActionWithDependancy()
        {
            return this.ObjectContext.ParametreAction.Include("RefEnumValeur");
        }

        /// <summary>
        /// Retourne le paramètre d'action sélectionné
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public ParametreAction GetParametreActionByCle(int cle)
        {
            return this.ObjectContext.ParametreAction.FirstOrDefault(e => e.CleParametreAction == cle);
        }

        #endregion

        #region LogOuvrage

        /// <summary>
        /// Retourne le logsélectionné
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public LogOuvrage GetLogOuvrageByCle(int cle)
        {
            return this.ObjectContext.LogOuvrage.Include("HistoEquipement.RefSousTypeOuvrage").Include("HistoPp").FirstOrDefault(e => e.CleLogOuvrage == cle);
        }

        /// <summary>
        /// Retourne la liste des log ouvrage d'un ouvrage en fonction de son type et de sa cle
        /// </summary>
        /// <param name="typeOuvrage">type d'ouvrage</param>
        /// <param name="cleOuvrage">cle de l'ouvrage</param>
        /// <returns>Liste des logOuvrage</returns>
        public IQueryable<LogOuvrage> GetLogOuvrageByCleOuvrage(string typeOuvrage, int cleOuvrage)
        {
            this.ObjectContext.LogOuvrage.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.UsrUtilisateur.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.HistoPp.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.HistoEquipement.MergeOption = MergeOption.NoTracking;
            IQueryable<LogOuvrage> retour = null;
            switch (typeOuvrage)
            {
                case "ClePp":
                    retour = this.ObjectContext.LogOuvrage.Include("UsrUtilisateur").Include("HistoPp.CategoriePp").Where(l => l.ClePp == cleOuvrage);
                    break;
                case "CleEquipement":
                    retour = this.ObjectContext.LogOuvrage.Include("UsrUtilisateur").Include("HistoEquipement").Where(l => l.CleEquipement == cleOuvrage);
                    break;
                case "ClePortion":
                    retour = this.ObjectContext.LogOuvrage.Include("UsrUtilisateur").Where(l => l.ClePortion == cleOuvrage);
                    break;
                case "CleEnsElectrique":
                    retour = this.ObjectContext.LogOuvrage.Include("UsrUtilisateur").Where(l => l.CleEnsElectrique == cleOuvrage);
                    break;
            }

            return retour;
        }

        #endregion

        #region PpJumelee

        /// <summary>
        /// Retourne la PpJumelee ayant comme identifiant la cle passée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public PpJumelee GetPpJumeleeByCle(int cle)
        {
            return this.ObjectContext.PpJumelee.FirstOrDefault(p => p.ClePpJumelee == cle);
        }

        /// <summary>
        /// Retourne la liste des PP jumelée à la PP
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<PpJumelee> GetPpJumeleesByClePp(int cle)
        {
            IQueryable<PpJumelee> retour = this.ObjectContext.PpJumelee
                                    .Include("Pp")
                                    .Include("Pp1")
                                    .Where(c => c.PpClePp == cle || c.ClePp == cle).Distinct();

            ObservableCollection<PpJumelee> CollectionAretourner = new ObservableCollection<PpJumelee>();

            foreach (PpJumelee PpJumeleeNiveauZero in retour)
            {
                CollectionAretourner.Add(PpJumeleeNiveauZero);
            }

            foreach (Pp PpFirstNviveau in retour.Select(c => c.Pp))
            {
                IQueryable<PpJumelee> retourFirstNviveau = this.ObjectContext.PpJumelee
                                        .Include("Pp")
                                        .Include("Pp1")
                                        .Where(c => c.PpClePp == PpFirstNviveau.ClePp || c.ClePp == PpFirstNviveau.ClePp).Distinct();

                foreach (PpJumelee PpJumeleeFirstNiveau in retourFirstNviveau)
                {
                    if (!CollectionAretourner.Contains(PpJumeleeFirstNiveau))
                    {
                        CollectionAretourner.Add(PpJumeleeFirstNiveau);
                    }
                }
            }

            foreach (Pp PpFirstNviveau in retour.Select(c => c.Pp1))
            {
                IQueryable<PpJumelee> retourFirstNviveau = this.ObjectContext.PpJumelee
                                        .Include("Pp")
                                        .Include("Pp1")

                                        .Where(c => c.PpClePp == PpFirstNviveau.ClePp || c.ClePp == PpFirstNviveau.ClePp).Distinct();

                foreach (PpJumelee PpJumeleeFirstNiveau in retourFirstNviveau)
                {
                    if (!CollectionAretourner.Contains(PpJumeleeFirstNiveau))
                    {
                        CollectionAretourner.Add(PpJumeleeFirstNiveau);
                    }
                }
            }

            return CollectionAretourner.AsQueryable();
        }

        #endregion PpJumelee

        #region Tournee

        /// <summary>
        /// Retourne le tableau de bord d'une tournée pour une période donnée
        /// </summary>
        /// <param name="cleTournee"></param>
        /// <param name="dateDebut"></param>
        /// <param name="dateFin"></param>
        /// <returns></returns>
        public IQueryable<SelectTourneeTableauBord_Result> GetTourneeTableauBord(int cleTournee, DateTime dateDebut, DateTime dateFin)
        {
            return this.ObjectContext.SelectTourneeTableauBord(cleTournee, dateDebut, dateFin).AsQueryable();
        }

        static readonly Func<ProtecaEntities, int, Tournee> getTourneeByCleQuery = CompiledQuery.Compile<ProtecaEntities, int, Tournee>((ctx, p) =>
                ctx.Tournees
                .Include("LogTournee")
                .Include("LogTournee.UsrUtilisateur")
                .Include("Compositions")
                .Include("Compositions.Pp")
                .Include("Compositions.Pp.PortionIntegrite")
                .Include("Compositions.Pp.PortionIntegrite.EnsembleElectrique")
                .Include("Compositions.EnsembleElectrique")
                .Include("Compositions.PortionIntegrite")
                .Include("Compositions.PortionIntegrite.EnsembleElectrique")
                .Include("Compositions.EqEquipement")
                .Include("Compositions.EqEquipement.Pp")
                .Include("Compositions.EqEquipement.Pp.PortionIntegrite")
                .Include("Compositions.EqEquipement.Pp.PortionIntegrite.EnsembleElectrique")
                .FirstOrDefault(e => e.CleTournee == p));

        /// <summary>
        /// Retourne la tournée sélectionnée
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public Tournee GetTourneeByCle(int cle)
        {
            // Pour optimiser le chargement, on utilise une requête compilée.
            this.ObjectContext.EnsembleElectrique.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.EqEquipement.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;

            var tournee = getTourneeByCleQuery.Invoke(this.ObjectContext, cle);
            return tournee;
        }

        /// <summary>
        /// Retourne la tournée sélectionnée avec les dernières visites sur les équipements de ses compositions
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public Tournee GetTourneeCompleteByCle(int cle)
        {
            this.ObjectContext.EnsembleElectrique.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;

            // Pour optimiser le chargement, on utilise une requête compilée.
            Tournee tournee = this.ObjectContext.Tournees
                                    .Include("Compositions.Pp.PortionIntegrite.EnsembleElectrique")
                                    .Include("Compositions.EqEquipement.Pp.PortionIntegrite.EnsembleElectrique")
                                    .FirstOrDefault(e => e.CleTournee == cle);

            //On va parcourir les PP et equipements de la tournee pour ajouter la dernière Visite NON TELEMESUREE du type de celle-ci
            foreach (Composition c in tournee.Compositions.Where(co => co.CleEquipement.HasValue || co.ClePp.HasValue))
            {
                var query = this.ObjectContext
                    .Visites.Include("MesMesure")
                    .Where(v =>
                        v.EstValidee && v.DateVisite.HasValue
                        && !v.Telemesure
                        && (v.EnumTypeEval == c.EnumTypeEval
                            || (this.ObjectContext.RefEnumValeur.FirstOrDefault(r => r.CleEnumValeur == c.EnumTypeEval)).Valeur == "1" && v.RefEnumValeur.Valeur == "2"));

                if (c.ClePp.HasValue)
                    query = query.Where(v => v.ClePp.HasValue && v.ClePp.Value == c.ClePp.Value);
                else if (c.CleEquipement.HasValue)
                    query = query.Where(v => v.CleEquipement.HasValue && v.CleEquipement.Value == c.CleEquipement.Value);

                Visite previousVisite = query.OrderByDescending(v => v.DateVisite).FirstOrDefault();

                if (previousVisite != null)
                {
                    if (c.ClePp.HasValue)
                        c.Pp.Visites.Add(previousVisite);
                    else if (c.CleEquipement.HasValue)
                        c.EqEquipement.Visites.Add(previousVisite);
                }
            }

            return tournee;
        }

        /// <summary>
        /// Retourne la liste des équipements correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="cleEnsElectrique">clé de l'ensemble électrique</param>
        /// <param name="clePortion">clé de la portion</param>
        /// <param name="includeDeletedEquipment"></param>
        /// <param name="filtreEq"></param>
        /// <returns>Une liste d'équipement</returns>
        public IQueryable<EqEquipement> FindTourneeEquipementByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,
            int? cleEnsElectrique, int? clePortion, bool includeDeletedEquipment, List<string> filtreEq)
        {
            IQueryable<EqEquipement> query;

            if (cleSecteur.HasValue)
            {
                query = this.ObjectContext.EqEquipement.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.CleSecteur == cleSecteur.Value));
            }
            else if (cleAgence.HasValue)
            {
                query = this.ObjectContext.EqEquipement.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.CleAgence == cleAgence.Value));
            }
            else if (cleRegion.HasValue)
            {
                query = this.ObjectContext.EqEquipement.Where(e => e.Pp.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value));
            }
            else
            {
                query = this.ObjectContext.EqEquipement;
            }

            if (clePortion.HasValue)
            {
                query = this.ObjectContext.EqEquipement.Where(e => e.Pp.ClePortion == clePortion.Value);
            }
            else if (cleEnsElectrique.HasValue)
            {
                query = this.ObjectContext.EqEquipement.Where(e => e.Pp.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value);
            }

            if (filtreEq.Count > 0)
            {
                query = query.Where(e => filtreEq.Contains(e.TypeEquipement.CodeEquipement));
            }
            if (!includeDeletedEquipment)
            {
                query = query.Where(e => e.Supprime == false);
            }

            return query.OrderBy(ee => ee.TypeEquipement.NumeroOrdre).ThenBy(ee => ee.Libelle);
        }

        /// <summary>
        /// Retourne la Portion ayant comme identifiant la clé passée en paramètre
        /// en incluant les Equipements et les PPs
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public PortionIntegrite GetTourneePortionIntegriteByCle(int cle)
        {
            this.ObjectContext.EnsembleElectrique.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.EqEquipement.MergeOption = MergeOption.NoTracking;

            return this.ObjectContext.PortionIntegrite
                .Include("EnsembleElectrique")
                .Include("Pps")
                .Include("Pps.EqEquipement")
                .Include("Pps.EqEquipement.TypeEquipement")
                .FirstOrDefault(e => e.ClePortion == cle);
        }

        /// <summary>
        /// Retourne uniquement la tournee correspondant à la cléTournée donnée en paramètre
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public Tournee GetTourneeOnlyByCle(int cle)
        {
            return this.ObjectContext.Tournees.FirstOrDefault(t => t.CleTournee == cle);
        }

        /// <summary>
        /// Retourne l'Ensemble électrique ayant comme identifiant la clé passée en paramètre
        /// en incluant les Portions, les Equipements et les PPs
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public EnsembleElectrique GetTourneeEnsElecByCle(int cle)
        {
            this.ObjectContext.EnsembleElectrique.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.EqEquipement.MergeOption = MergeOption.NoTracking;

            return this.ObjectContext.EnsembleElectrique
                .Include("PortionIntegrite")
                .Include("PortionIntegrite.Pps")
                .Include("PortionIntegrite.Pps.EqEquipement")
                .Include("PortionIntegrite.Pps.EqEquipement.TypeEquipement")
                .FirstOrDefault(e => e.CleEnsElectrique == cle);
        }

        /// <summary>
        /// Retourne la liste des tournées correspondant aux critères de recherche passés en paramètre
        /// </summary>
        /// <param name="cleRegion">clé de la région</param>
        /// <param name="cleAgence">clé de l'agence</param>
        /// <param name="cleSecteur">clé du secteur</param>
        /// <param name="cleEnsElectrique">clé de l'ensemble électrique</param>
        /// <param name="clePortion">clé de la portion</param>
        /// <param name="libelle">filtre sur le libelle de la tournée</param>
        /// <returns>Une liste d'ensemble électrique</returns>
        public IQueryable<Tournee> FindTourneesByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElectrique, int? clePortion,
            string libelle, bool IsDelete)
        {
            IQueryable<Tournee> query;

            if (cleRegion.HasValue || cleAgence.HasValue || cleSecteur.HasValue || cleEnsElectrique.HasValue || clePortion.HasValue)
            {
                IQueryable<Composition> queryPP = this.ObjectContext.Compositions.Where(c => c.ClePp.HasValue);
                IQueryable<Composition> queryEQ = this.ObjectContext.Compositions.Where(c => c.CleEquipement.HasValue);
                IQueryable<Composition> queryPI = this.ObjectContext.Compositions.Where(c => c.ClePortion.HasValue);
                IQueryable<Composition> queryEE = this.ObjectContext.Compositions.Where(c => c.CleEnsElectrique.HasValue);

                if (cleSecteur.HasValue)
                {
                    queryPP = queryPP.Where(c => c.Pp.CleSecteur == cleSecteur.Value);
                    queryEQ = queryEQ.Where(c => c.EqEquipement.Pp.CleSecteur == cleSecteur.Value);
                    queryPI = queryPI.Where(c => c.PortionIntegrite.PiSecteurs.Any(piS => piS.CleSecteur == cleSecteur.Value));
                    queryEE = queryEE.Where(c => c.EnsembleElectrique.PortionIntegrite.Any(pi => pi.PiSecteurs.Any(piS => piS.CleSecteur == cleSecteur.Value)));
                }
                else if (cleAgence.HasValue)
                {
                    queryPP = queryPP.Where(c => c.Pp.GeoSecteur.CleAgence == cleAgence.Value);
                    queryEQ = queryEQ.Where(c => c.EqEquipement.Pp.GeoSecteur.CleAgence == cleAgence.Value);
                    queryPI = queryPI.Where(c => c.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.CleAgence == cleAgence.Value));
                    queryEE = queryEE.Where(c => c.EnsembleElectrique.PortionIntegrite.Any(pi => pi.PiSecteurs.Any(piS => piS.GeoSecteur.CleAgence == cleAgence.Value)));
                }
                else if (cleRegion.HasValue)
                {
                    queryPP = queryPP.Where(c => c.Pp.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value);
                    queryEQ = queryEQ.Where(c => c.EqEquipement.Pp.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value);
                    queryPI = queryPI.Where(c => c.PortionIntegrite.PiSecteurs.Any(pi => pi.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value));
                    queryEE = queryEE.Where(c => c.EnsembleElectrique.PortionIntegrite.Any(pi => pi.PiSecteurs.Any(piS => piS.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value)));
                }

                if (clePortion.HasValue)
                {
                    queryPP = queryPP.Where(c => c.Pp.ClePortion == clePortion.Value);
                    queryEQ = queryEQ.Where(c => c.EqEquipement.Pp.ClePortion == clePortion.Value);
                    queryPI = queryPI.Where(c => c.ClePortion.Value == clePortion.Value);
                    queryEE = queryEE.Where(c => c.EnsembleElectrique.PortionIntegrite.Any(pi => pi.ClePortion == clePortion.Value));
                }
                else if (cleEnsElectrique.HasValue)
                {
                    queryPP = queryPP.Where(c => c.Pp.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value);
                    queryEQ = queryEQ.Where(c => c.EqEquipement.Pp.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value);
                    queryPI = queryPI.Where(c => c.PortionIntegrite.CleEnsElectrique == cleEnsElectrique.Value);
                    queryEE = queryEE.Where(c => c.CleEnsElectrique == cleEnsElectrique.Value);
                }

                query = queryPP.Select(c => c.Tournee);
                query = query.Union(queryEQ.Select(c => c.Tournee));
                query = query.Union(queryPI.Select(c => c.Tournee));
                query = query.Union(queryEE.Select(c => c.Tournee));
                query = query.Distinct();
            }
            else
            {
                query = this.ObjectContext.Tournees;
            }

            if (!String.IsNullOrEmpty(libelle))
            {
                query = query.Where(to => to.Libelle.Contains(libelle));
            }
            return query.Where(i => i.Supprime == IsDelete || IsDelete).OrderBy(ee => ee.Libelle);
        }

        #region Export Tournée

        /// <summary>
        /// Export la tournée en XML
        /// </summary>
        public List<string> GetTourneeExportXML(int cle)
        {
            Tournee t = this.ObjectContext.Tournees.Include("Compositions").FirstOrDefault(e => e.CleTournee == cle);

            XmlDocument XmlTournee = new XmlDocument();
            XmlDocument XmlClassementMesure = new XmlDocument();
            XmlDocument XmlDataRef = new XmlDocument();
            XmlDocument XmlEmptyTemplate = new XmlDocument();

            if (t != null && t.Compositions.Count > 0)
            {
                // Données de la tournée
                XmlElement princ = XmlTournee.CreateElement("ExportTournee");
                princ.SetAttribute("Version", "1.0");
                princ.AppendChild(GetTourneeToXml(t, XmlTournee));
                XmlTournee.AppendChild(princ);

                // Données de classement Mesure                 
                XmlElement elemMesClassementsMesures = XmlClassementMesure.CreateElement("MesClassementsMesures");
                IQueryable<MesClassementMesure> mes = this.ObjectContext.MesClassementMesure.Where(c => c.MesTypeMesure.MesureEnService).OrderBy(m => m.MesTypeMesure.MesModeleMesure.TypeEquipement.NumeroOrdre).ThenBy(m => m.MesTypeMesure.MesModeleMesure.NumeroOrdre).ThenBy(m => m.MesTypeMesure.NumeroOrdre);
                foreach (MesClassementMesure cm in mes)
                {
                    elemMesClassementsMesures.AppendChild(getMesClassementMesureToXML(cm, XmlClassementMesure));
                }
                XmlClassementMesure.AppendChild(elemMesClassementsMesures);

                // Données de référence de la tournée                
                XmlDataRef.AppendChild(GetTourneeRefDataToXml(t, XmlDataRef));

                // Templates Vides                
                XmlEmptyTemplate.AppendChild(GetTourneeEmptyTemplateToXml(XmlEmptyTemplate));
            }

            return new List<string>() { XmlTournee.OuterXml, XmlClassementMesure.OuterXml, XmlDataRef.OuterXml, XmlEmptyTemplate.OuterXml };
        }

        #region Export donnée de la tournée

        /// <summary>
        /// Classe tampon pour la génération du XML
        /// </summary>
        private class SortedPpAndTypeEval
        {
            public SortedPpAndTypeEval()
            {

            }

            public int NumeroOrdre { get; set; }
            public Pp Pp { get; set; }
            public RefEnumValeur TypeEval { get; set; }
        }

        /// <summary>
        /// Export d'une tournée au format XML
        /// </summary>
        /// <param name="To">la tournée a exporter</param>
        /// <param name="XmlDoc"></param>
        /// <returns></returns>
        public XmlElement GetTourneeToXml(Tournee To, XmlDocument XmlDoc)
        {
            XmlElement princ = XmlDoc.CreateElement("Tournee");

            XmlElement elem = XmlDoc.CreateElement("CleTournee");
            elem.InnerText = To.CleTournee.ToString();
            princ.AppendChild(elem);

            //elem = XmlDoc.CreateElement("CleUtilisateur");
            //elem.InnerText = To.CleUtilisateur.HasValue ? To.CleUtilisateur.Value.ToString() : string.Empty;
            //princ.AppendChild(elem);

            // Agent de mesure
            princ.AppendChild(XmlDoc.CreateElement("CleUtilisateur"));
            princ.AppendChild(XmlDoc.CreateElement("PrenomAgent"));
            princ.AppendChild(XmlDoc.CreateElement("NomAgent"));
            //Instruments
            princ.AppendChild(XmlDoc.CreateElement("Instruments"));

            elem = XmlDoc.CreateElement("Libelle");
            elem.InnerText = To.Libelle;
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("CodeTournee");
            elem.InnerText = To.CodeTournee;
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("Commentaire");
            elem.InnerText = To.Commentaire;
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("DateCreation");
            elem.InnerText = To.DateCreation.HasValue ? To.DateCreation.Value.ToString() : string.Empty;
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("Numero");
            elem.InnerText = To.Numero.ToString();
            princ.AppendChild(elem);

            XmlElement elemPPs = XmlDoc.CreateElement("PPs");
            XmlElement elemEQs = XmlDoc.CreateElement("Equipements");
            XmlElement elemPOs = XmlDoc.CreateElement("Portions");
            XmlElement elemEEs = XmlDoc.CreateElement("EnsemblesElectriques");

            // Composition par Equipement
            IEnumerable<Composition> EQs = To.Compositions.Where(co => co.CleEquipement.HasValue && !co.EqEquipement.Supprime).OrderBy(co => co.NumeroOrdre);
            foreach (var eq in EQs)
            {
                elemEQs.AppendChild(getEquipementToXML(eq, XmlDoc));
            }

            // Composition par PP
            IEnumerable<SortedPpAndTypeEval> PPs = To.Compositions.Where(co => co.ClePp.HasValue && !co.Pp.Supprime).Select(co => new SortedPpAndTypeEval() { NumeroOrdre = co.NumeroOrdre, Pp = co.Pp, TypeEval = co.RefEnumValeur });
            IEnumerable<int> clePPs = PPs.Select(p => p.Pp.ClePp);
            IEnumerable<SortedPpAndTypeEval> PPNonMesurees = EQs.Where(co => !clePPs.Contains(co.EqEquipement.ClePp)).GroupBy(co => new { co.EqEquipement.Pp, co.NumeroOrdre }).Select(c => new SortedPpAndTypeEval() { NumeroOrdre = c.Min(t => t.NumeroOrdre), Pp = c.Key.Pp }).Distinct();

            PPs = PPs.Union(PPNonMesurees).OrderBy(pp => pp.NumeroOrdre);
            foreach (var pp in PPs)
            {
                elemPPs.AppendChild(getPPToXML(pp.TypeEval, pp.Pp, XmlDoc));
            }


            //IEnumerable<Composition> PPs = To.Compositions.Where(co => co.ClePp.HasValue && !co.Pp.Supprime).OrderBy(co => co.NumeroOrdre);
            //foreach (var pp in PPs)
            //{
            //    elemPPs.AppendChild(getPPToXML(pp, null, XmlDoc));
            //}

            //IEnumerable<int> clePPs = PPs.Select(p => p.ClePp.Value);
            //IEnumerable<Pp> PPNonMesurees = EQs.Where(co => !clePPs.Contains(co.EqEquipement.ClePp)).Select(co => co.EqEquipement.Pp).Distinct();
            //foreach (var pp in PPNonMesurees)
            //{
            //    elemPPs.AppendChild(getPPToXML(null, pp, XmlDoc));
            //}

            // Composition par portion
            IEnumerable<PortionIntegrite> PIs = To.Compositions.Where(co => co.ClePortion.HasValue && !co.PortionIntegrite.Supprime).Select(co => co.PortionIntegrite).OrderBy(pi => pi.Libelle);
            if (!PIs.Any())
            {
                PIs = PPs.Select(co => co.Pp.PortionIntegrite);
                PIs = PIs.Union(EQs.Select(co => co.EqEquipement.Pp.PortionIntegrite)).Distinct().OrderBy(pi => pi.Libelle);
            }
            foreach (var pi in PIs)
            {
                elemPOs.AppendChild(getPortionIntegriteToXML(pi, XmlDoc));
            }

            // Composition par Ensemble électrique
            IEnumerable<EnsembleElectrique> EEs = To.Compositions.Where(co => co.CleEnsElectrique.HasValue && !co.EnsembleElectrique.Supprime).Select(co => co.EnsembleElectrique);
            EEs = EEs.Union(PIs.Select(pi => pi.EnsembleElectrique)).Distinct().OrderBy(ee => ee.Libelle);

            if (!EEs.Any())
            {
                EEs = PPs.Select(co => co.Pp.PortionIntegrite.EnsembleElectrique);
                EEs = EEs.Union(EQs.Select(co => co.EqEquipement.Pp.PortionIntegrite.EnsembleElectrique)).Distinct().OrderBy(ee => ee.Libelle);
            }
            foreach (var EE in EEs)
            {
                elemEEs.AppendChild(getEnsembleElectriqueToXML(EE, XmlDoc));
            }

            princ.AppendChild(elemEEs);
            princ.AppendChild(elemPOs);
            princ.AppendChild(elemPPs);
            princ.AppendChild(elemEQs);

            return princ;
        }

        /// <summary>
        /// Exporte l'objet PortionIntegrite passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="PI">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getPortionIntegriteToXML(PortionIntegrite PI, XmlDocument XmlDoc)
        {
            XmlElement elemPO = XmlDoc.CreateElement("PortionIntegrite");

            XmlElement elemPOdetails = XmlDoc.CreateElement("ClePortion");
            elemPOdetails.InnerText = PI.ClePortion > 0 ? PI.ClePortion.ToString() : string.Empty;
            elemPO.AppendChild(elemPOdetails);

            elemPOdetails = XmlDoc.CreateElement("Seuils");

            IEnumerable<MesNiveauProtection> seuils = PI.MesNiveauProtection.Where(s => !s.CleEquipement.HasValue && !s.ClePp.HasValue);
            foreach (MesNiveauProtection seuil in seuils)
            {
                XmlElement elemeSeuilDetail = XmlDoc.CreateElement("Seuil");
                elemeSeuilDetail.SetAttribute("Min", seuil != null && seuil.SeuilMini.HasValue ? seuil.SeuilMini.Value.ToString() : "");
                elemeSeuilDetail.SetAttribute("Max", seuil != null && seuil.SeuilMaxi.HasValue ? seuil.SeuilMaxi.Value.ToString() : "");
                elemeSeuilDetail.SetAttribute("CleModeleMesure", seuil != null ? seuil.CleModeleMesure.ToString() : "");
                elemPOdetails.AppendChild(elemeSeuilDetail);
            }

            elemPO.AppendChild(elemPOdetails);

            elemPOdetails = XmlDoc.CreateElement("CleEnsElec");
            elemPOdetails.InnerText = PI.CleEnsElectrique > 0 ? PI.CleEnsElectrique.ToString() : string.Empty;
            elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("CleRevetement");
            //elemPOdetails.InnerText = PI.CleRevetement != null ?PI.CleRevetement.ToString() : string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("CleDiametre");
            //elemPOdetails.InnerText = PI.CleDiametre != null ?PI.CleDiametre.ToString(): string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("CleCommuneDepart");
            //elemPOdetails.InnerText = PI.CleCommuneDepart.HasValue? PI.CleCommuneDepart.Value.ToString() : string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("CleCommuneArrivee");
            //elemPOdetails.InnerText = PI.CleCommuneArrivee.HasValue ? PI.CleCommuneArrivee.Value.ToString() : string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            elemPOdetails = XmlDoc.CreateElement("Code");
            elemPOdetails.InnerText = PI.Code != null ? PI.Code : string.Empty;
            elemPO.AppendChild(elemPOdetails);

            elemPOdetails = XmlDoc.CreateElement("Libelle");
            elemPOdetails.InnerText = PI.Libelle != null ? PI.Libelle : string.Empty;
            elemPO.AppendChild(elemPOdetails);

            elemPOdetails = XmlDoc.CreateElement("CodeGMAO");
            elemPOdetails.InnerText = PI.CodeGmao != null ? PI.CodeGmao : string.Empty;
            elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("DateMiseEnService");
            //elemPOdetails.InnerText = PI.DateMiseEnService.HasValue ? PI.DateMiseEnService.Value.ToString() : string.Empty;
            //elemPO.AppendChild(elemPOdetails);


            //elemPOdetails = XmlDoc.CreateElement("DatePose");
            //elemPOdetails.InnerText = PI.DatePose.HasValue ? PI.DatePose.Value.ToString() : string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("Commentaire");
            //elemPOdetails.InnerText = PI.Commentaire != null ?PI.Commentaire: string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("DateMAJCommentaire");
            //elemPOdetails.InnerText = PI.DateMajCommentaire.HasValue ? PI.DateMajCommentaire.Value.ToString() : string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            elemPOdetails = XmlDoc.CreateElement("Longueur");
            elemPOdetails.InnerText = PI.Longueur.HasValue ? PI.Longueur.Value.ToString() : string.Empty;
            elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("IDTroncon");
            //elemPOdetails.InnerText = PI.Idtroncon.HasValue ? PI.Idtroncon.Value.ToString() : string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("Supprime");
            //elemPOdetails.InnerText = PI.Supprime != null ?PI.Supprime.ToString(): string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            //elemPOdetails = XmlDoc.CreateElement("Branchement");
            //elemPOdetails.InnerText = PI.Branchement != null ? PI.Branchement.ToString() : string.Empty;
            //elemPO.AppendChild(elemPOdetails);

            return elemPO;
        }

        /// <summary>
        /// Exporte l'objet PP passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="PP">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        //private XmlElement getPPToXML(Composition co, Pp PP, XmlDocument XmlDoc)
        private XmlElement getPPToXML(RefEnumValeur typEval, Pp PP, XmlDocument XmlDoc)
        {
            XmlElement elemPP = XmlDoc.CreateElement("PP");

            if (PP != null)
            {
                XmlElement elemPPdetails = XmlDoc.CreateElement("PPMesuree");
                elemPPdetails.InnerText = typEval != null ? "1" : "0";
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("CleTypeEvaluation");
                elemPPdetails.InnerText = typEval != null ? typEval.CleEnumValeur.ToString() : String.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("TypeEvaluation");
                elemPPdetails.InnerText = typEval != null ? typEval.LibelleCourt : String.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("ClePP");
                elemPPdetails.InnerText = PP.ClePp.ToString();
                elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("CleCommune");
                //elemPPdetails.InnerText = PP.CleCommune != null ? PP.CleCommune.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("CleNiveauSensibilite");
                elemPPdetails.InnerText = PP.CleNiveauSensibilite.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("CleCategoriePP");
                elemPPdetails.InnerText = PP.CleCategoriePp.HasValue ? PP.CleCategoriePp.Value.ToString() : String.Empty;
                elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("ClePPOrigine");
                //elemPPdetails.InnerText = PP.ClePpOrigine.HasValue ? PP.ClePpOrigine.Value.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("EnumSurfaceTme");
                elemPPdetails.InnerText = PP.EnumSurfaceTme.HasValue ? PP.EnumSurfaceTme.Value.ToString() : string.Empty;
                elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("CleUtilisateur");
                //elemPPdetails.InnerText = PP.CleUtilisateur != null ? PP.CleUtilisateur.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("EnumSurfaceTms");
                elemPPdetails.InnerText = PP.EnumSurfaceTms.HasValue ? PP.EnumSurfaceTms.Value.ToString() : string.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("EnumDureeEnrg");
                elemPPdetails.InnerText = PP.EnumDureeEnrg.HasValue ? PP.EnumDureeEnrg.Value.ToString() : string.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("CleSecteur");
                elemPPdetails.InnerText = PP.CleSecteur.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("Secteur");
                elemPPdetails.InnerText = PP.GeoSecteur != null ? PP.GeoSecteur.LibelleSecteur : string.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("EnumPolarisation");
                elemPPdetails.InnerText = PP.EnumPolarisation.HasValue ? PP.EnumPolarisation.Value.ToString() : string.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("ClePortion");
                elemPPdetails.InnerText = PP.ClePortion.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("Libelle");
                elemPPdetails.InnerText = PP.Libelle != null ? PP.Libelle : string.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("Pk");
                elemPPdetails.InnerText = PP.Pk.ToString();
                elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("Commentaire");
                //elemPPdetails.InnerText = PP.Commentaire != null ? PP.Commentaire.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("DateMAJCommentaire");
                //elemPPdetails.InnerText = PP.DateMajCommentaire.HasValue ? PP.DateMajCommentaire.Value.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("PositionnementPostal");
                //elemPPdetails.InnerText = PP.PositionnementPostal != null ? PP.PositionnementPostal.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("CommentairePositionnement");
                //elemPPdetails.InnerText = PP.CommentairePositionnement != null ? PP.CommentairePositionnement : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("CourantsVagabonds");
                elemPPdetails.InnerText = PP.CourantsVagabonds.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("CourantsAlternatifsInduits");
                elemPPdetails.InnerText = PP.CourantsAlternatifsInduits.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("ElectrodeEnterreAmovible");
                elemPPdetails.InnerText = PP.ElectrodeEnterreeAmovible.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("TemoinEnterreAmovible");
                elemPPdetails.InnerText = PP.TemoinEnterreAmovible.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("TemoinMetalliqueDeSurface");
                elemPPdetails.InnerText = PP.TemoinMetalliqueDeSurface.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("Telemesure");
                elemPPdetails.InnerText = PP.PresenceDUneTelemesure.ToString();
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("PositionGPSLat");
                elemPPdetails.InnerText = PP.PositionGpsLat.HasValue ? PP.PositionGpsLat.Value.ToString() : string.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("PositionGPSLong");
                elemPPdetails.InnerText = PP.PositionGpsLong.HasValue ? PP.PositionGpsLong.Value.ToString() : string.Empty;
                elemPP.AppendChild(elemPPdetails);

                elemPPdetails = XmlDoc.CreateElement("CoordonneesGPSFiabilisee");
                elemPPdetails.InnerText = PP.CoordonneeGpsFiabilisee.ToString();
                elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("DateMiseEnService");
                //elemPPdetails.InnerText = PP.DateMiseEnService.HasValue ? PP.DateMiseEnService.Value.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("PPPoste");
                //elemPPdetails.InnerText = PP.PpPoste != null ? PP.PpPoste.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("Supprime");
                //elemPPdetails.InnerText = PP.Supprime != null ? PP.Supprime.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("DateMAJPP");
                //elemPPdetails.InnerText = PP.DateMajPp != null ? PP.DateMajPp.ToString() : string.Empty;
                //elemPP.AppendChild(elemPPdetails);

                if (typEval != null)
                {
                    // Dernière visite ayant le même type d'évaluation (Si EG, ECD convient également)
                    IEnumerable<Visite> visites = PP.Visites.Where(v => v.EnumTypeEval == typEval.CleEnumValeur || (typEval.Valeur == "1" && v.RefEnumValeur.Valeur == "2")).OrderByDescending(v => v.DateVisite);
                    elemPPdetails = XmlDoc.CreateElement("Visites");

                    if (visites.Count() > 0)
                    {
                        elemPPdetails.AppendChild(getVisiteToXML(visites.First(), XmlDoc));
                    }

                    // Génération d'une visite de saisie vide
                    XmlElement elemVisiteProteIN = XmlDoc.CreateElement("VisiteProteIN");

                    elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleUtilisateur"));

                    XmlElement elemVisiteProteINDetails = XmlDoc.CreateElement("EnumTypeEval");
                    elemVisiteProteINDetails.InnerText = typEval.CleEnumValeur.ToString();
                    elemVisiteProteIN.AppendChild(elemVisiteProteINDetails);

                    elemVisiteProteINDetails = XmlDoc.CreateElement("ClePP");
                    elemVisiteProteINDetails.InnerText = PP.ClePp.ToString();
                    elemVisiteProteIN.AppendChild(elemVisiteProteINDetails);

                    elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleEquipement"));

                    elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("DateVisite"));
                    elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("EstConfirmee"));
                    elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CommentaireVisite"));
                    elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CommentaireAnalyse"));
                    elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleAnalyse"));
                    elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("MesMesures"));

                    elemPPdetails.AppendChild(elemVisiteProteIN);

                    elemPP.AppendChild(elemPPdetails);
                }
            }

            return elemPP;
        }

        /// <summary>
        /// Exporte l'objet Visite passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="vi">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getVisiteToXML(Visite Vi, XmlDocument XmlDoc)
        {
            XmlElement elemVi = XmlDoc.CreateElement("Visite");

            XmlElement elemVidetails = XmlDoc.CreateElement("CleVisite");
            elemVidetails.InnerText = Vi.CleVisite.ToString();
            elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("CleUtilisateurCreation");
            //elemVidetails.InnerText = Vi.CleUtilisateurCreation.HasValue ? Vi.CleUtilisateurCreation.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("CleUtilisateurImport");
            //elemVidetails.InnerText = Vi.CleUtilisateurImport.HasValue ? Vi.CleUtilisateurImport.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("CleUtilisateurMesure");
            //elemVidetails.InnerText = Vi.CleUtilisateurMesure.HasValue ? Vi.CleUtilisateurMesure.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("CleUtilisateurValidation");
            //elemVidetails.InnerText = Vi.CleUtilisateurValidation.HasValue ? Vi.CleUtilisateurValidation.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("EnumTypeEval");
            //elemVidetails.InnerText = Vi.EnumTypeEval != null ? Vi.EnumTypeEval.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("ClePP");
            //elemVidetails.InnerText = Vi.ClePp.HasValue ? Vi.ClePp.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("CleEquipement");
            //elemVidetails.InnerText = Vi.CleEquipement.HasValue ? Vi.CleEquipement.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("CleAnalyse");
            //elemVidetails.InnerText = Vi.CleAnalyse.HasValue ? Vi.CleAnalyse.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            elemVidetails = XmlDoc.CreateElement("DateVisite");
            elemVidetails.InnerText = Vi.DateVisite.HasValue ? Vi.DateVisite.Value.ToString() : string.Empty;
            elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("DateSaisie");
            //elemVidetails.InnerText = Vi.DateSaisie.HasValue ? Vi.DateSaisie.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("DateImport");
            //elemVidetails.InnerText = Vi.DateImport.HasValue ? Vi.DateImport.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("DateValidation");
            //elemVidetails.InnerText = Vi.DateValidation.HasValue ? Vi.DateValidation.Value.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("EstValidee");
            //elemVidetails.InnerText = Vi.EstValidee != null ? Vi.EstValidee.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("Commentaire");
            //elemVidetails.InnerText = Vi.Commentaire != null ? Vi.Commentaire.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            //elemVidetails = XmlDoc.CreateElement("RelevePartiel");
            //elemVidetails.InnerText = Vi.RelevePartiel != null ? Vi.RelevePartiel.ToString() : string.Empty;
            //elemVi.AppendChild(elemVidetails);

            elemVidetails = XmlDoc.CreateElement("MesMesures");

            // récupération uniquement des mesures de type moyen ou autre
            IEnumerable<MesMesure> mes = Vi.MesMesure.Where(m => m.MesTypeMesure.RefEnumValeur.Valeur == "1" || m.MesTypeMesure.RefEnumValeur.Valeur == "4");
            foreach (MesMesure me in mes)
            {
                elemVidetails.AppendChild(getMesMesureToXML(me, XmlDoc));
            }
            elemVi.AppendChild(elemVidetails);

            return elemVi;
        }

        /// <summary>
        /// Exporte l'objet MesMesure passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="me">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getMesMesureToXML(MesMesure me, XmlDocument XmlDoc)
        {
            XmlElement elemMes = XmlDoc.CreateElement("MesMesure");

            XmlElement elemMesdetails = XmlDoc.CreateElement("ModeleMesureLib");
            elemMesdetails.InnerText = me.MesTypeMesure.MesModeleMesure != null ? me.MesTypeMesure.MesModeleMesure.LibGenerique : string.Empty;
            elemMes.AppendChild(elemMesdetails);

            //elemMesdetails = XmlDoc.CreateElement("CleMesure");
            //elemMesdetails.InnerText = me.CleMesure != null ? me.CleMesure.ToString() : string.Empty;
            //elemMes.AppendChild(elemMesdetails);

            //elemMesdetails = XmlDoc.CreateElement("CleTypeMesure");
            //elemMesdetails.InnerText = me.CleTypeMesure != null ? me.CleTypeMesure.ToString() : string.Empty;
            //elemMes.AppendChild(elemMesdetails);

            //elemMesdetails = XmlDoc.CreateElement("CleAlerteSeuil");
            //elemMesdetails.InnerText = me.CleAlerteSeuil != null ? me.CleAlerteSeuil.ToString() : string.Empty;
            //elemMes.AppendChild(elemMesdetails);

            //elemMesdetails = XmlDoc.CreateElement("CleVisite");
            //elemMesdetails.InnerText = me.CleVisite != null ? me.CleVisite.ToString() : string.Empty;
            //elemMes.AppendChild(elemMesdetails);

            elemMesdetails = XmlDoc.CreateElement("MesureComplementaire");
            elemMesdetails.InnerText = me.MesTypeMesure.MesureComplementaire.ToString();
            elemMes.AppendChild(elemMesdetails);

            elemMesdetails = XmlDoc.CreateElement("Valeur");
            elemMesdetails.InnerText = me.Valeur != null ? me.Valeur.ToString() : string.Empty;
            elemMes.AppendChild(elemMesdetails);

            return elemMes;
        }

        /// <summary>
        /// Exporte l'objet EnsembleElectrique passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="EE">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getEnsembleElectriqueToXML(EnsembleElectrique EE, XmlDocument XmlDoc)
        {
            XmlElement elemEE = XmlDoc.CreateElement("EnsElec");

            XmlElement elemEEdetails = XmlDoc.CreateElement("CleEnsElec");
            elemEEdetails.InnerText = EE.CleEnsElectrique.ToString();
            elemEE.AppendChild(elemEEdetails);

            //elemEEdetails = XmlDoc.CreateElement("EnumPeriodicite");
            //elemEEdetails.InnerText = EE.EnumPeriodicite != null ?EE.EnumPeriodicite.ToString(): string.Empty;
            //elemEE.AppendChild(elemEEdetails);

            //elemEEdetails = XmlDoc.CreateElement("EnumStructureCPLX");
            //elemEEdetails.InnerText = EE.EnumStructureCplx.HasValue ? EE.EnumStructureCplx.Value.ToString() : string.Empty;
            //elemEE.AppendChild(elemEEdetails);

            elemEEdetails = XmlDoc.CreateElement("Code");
            elemEEdetails.InnerText = EE.Code != null ? EE.Code : string.Empty;
            elemEE.AppendChild(elemEEdetails);

            elemEEdetails = XmlDoc.CreateElement("Libelle");
            elemEEdetails.InnerText = EE.Libelle != null ? EE.Libelle : string.Empty;
            elemEE.AppendChild(elemEEdetails);

            elemEEdetails = XmlDoc.CreateElement("LongueurReseau");
            elemEEdetails.InnerText = EE.LongueurReseau.ToString();
            elemEE.AppendChild(elemEEdetails);

            //elemEEdetails = XmlDoc.CreateElement("Commentaire");
            //elemEEdetails.InnerText = EE.Commentaire != null ?EE.Commentaire: string.Empty;
            //elemEE.AppendChild(elemEEdetails);

            //elemEEdetails = XmlDoc.CreateElement("DateMAJCommentaire");
            //elemEEdetails.InnerText = EE.DateMajCommentaire.HasValue ? EE.DateMajCommentaire.Value.ToString() : string.Empty;
            //elemEE.AppendChild(elemEEdetails);

            //elemEEdetails = XmlDoc.CreateElement("Supprime");
            //elemEEdetails.InnerText = EE.Supprime != null ? EE.Supprime.ToString() : string.Empty;
            //elemEE.AppendChild(elemEEdetails);

            return elemEE;
        }

        /// <summary>
        /// Exporte l'objet EqEquipement passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="EQ">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getEquipementToXML(Composition co, XmlDocument XmlDoc)
        {
            XmlElement elemEQ = XmlDoc.CreateElement("EQ");

            if (co.CleEquipement.HasValue)
            {
                EqEquipement EQ = co.EqEquipement;

                XmlElement elemEQdetails = XmlDoc.CreateElement("CleTypeEvaluation");
                elemEQdetails.InnerText = co.EnumTypeEval.ToString();
                elemEQ.AppendChild(elemEQdetails);

                elemEQdetails = XmlDoc.CreateElement("TypeEvaluation");
                elemEQdetails.InnerText = co.RefEnumValeur != null ? co.RefEnumValeur.LibelleCourt : string.Empty;
                elemEQ.AppendChild(elemEQdetails);

                elemEQdetails = XmlDoc.CreateElement("CleEquipement");
                elemEQdetails.InnerText = EQ.CleEquipement.ToString();
                elemEQ.AppendChild(elemEQdetails);

                //elemEQdetails = XmlDoc.CreateElement("CleEquipementOrigine");
                //elemEQdetails.InnerText = EQ.CleEquipementOrigine != null ? EQ.CleEquipementOrigine.ToString() : string.Empty;
                //elemEQ.AppendChild(elemEQdetails);

                elemEQdetails = XmlDoc.CreateElement("ClePp");
                elemEQdetails.InnerText = EQ.ClePp.ToString();
                elemEQ.AppendChild(elemEQdetails);

                //// Caractérisrique de la pp de ratachement
                //XmlElement elemEQPP = XmlDoc.CreateElement("PP");
                //XmlElement elemPPdetails = XmlDoc.CreateElement("CourantsVagabonds");
                //elemPPdetails.InnerText = EQ.Pp.CourantsVagabonds != null ? EQ.Pp.CourantsVagabonds.ToString() : string.Empty;
                //elemEQPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("CourantsAlternatifsInduits");
                //elemPPdetails.InnerText = EQ.Pp.CourantsAlternatifsInduits != null ? EQ.Pp.CourantsAlternatifsInduits.ToString() : string.Empty;
                //elemEQPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("ElectrodeEnterreAmovible");
                //elemPPdetails.InnerText = EQ.Pp.CourantsAlternatifsInduits != null ? EQ.Pp.CourantsAlternatifsInduits.ToString() : string.Empty;
                //elemEQPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("TemoinEnterreAmovible");
                //elemPPdetails.InnerText = EQ.Pp.TemoinEnterreAmovible != null ? EQ.Pp.TemoinEnterreAmovible.ToString() : string.Empty;
                //elemEQPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("TemoinMetalliqueDeSurface");
                //elemPPdetails.InnerText = EQ.Pp.TemoinMetalliqueDeSurface != null ? EQ.Pp.TemoinMetalliqueDeSurface.ToString() : string.Empty;
                //elemEQPP.AppendChild(elemPPdetails);

                //elemPPdetails = XmlDoc.CreateElement("Telemesure");
                //elemPPdetails.InnerText = EQ.Pp.PresenceDUneTelemesure != null ? EQ.Pp.PresenceDUneTelemesure.ToString() : string.Empty;
                //elemEQPP.AppendChild(elemPPdetails);

                //elemEQ.AppendChild(elemEQPP);

                elemEQdetails = XmlDoc.CreateElement("CleTypeEq");
                elemEQdetails.InnerText = EQ.CleTypeEq.ToString();
                elemEQ.AppendChild(elemEQdetails);

                elemEQdetails = XmlDoc.CreateElement("TypeEquipement");
                elemEQdetails.InnerText = EQ.TypeEquipement != null ? EQ.TypeEquipement.CodeEquipement : string.Empty;
                elemEQ.AppendChild(elemEQdetails);

                //elemEQdetails = XmlDoc.CreateElement("CleUtilisateur");
                //elemEQdetails.InnerText = EQ.CleUtilisateur.HasValue ? EQ.CleUtilisateur.Value.ToString() : string.Empty;
                //elemEQ.AppendChild(elemEQdetails);

                elemEQdetails = XmlDoc.CreateElement("Libelle");
                elemEQdetails.InnerText = EQ.Libelle != null ? EQ.Libelle : string.Empty;
                elemEQ.AppendChild(elemEQdetails);

                //elemEQdetails = XmlDoc.CreateElement("Commentaire");
                //elemEQdetails.InnerText = EQ.Commentaire != null ? EQ.Commentaire : string.Empty;
                //elemEQ.AppendChild(elemEQdetails);

                //elemEQdetails = XmlDoc.CreateElement("DateMAJCommentaire");
                //elemEQdetails.InnerText = EQ.DateMajCommentaire.HasValue ? EQ.DateMajCommentaire.Value.ToString() : string.Empty;
                //elemEQ.AppendChild(elemEQdetails);

                //elemEQdetails = XmlDoc.CreateElement("DateMAJEquipement");
                //elemEQdetails.InnerText = EQ.DateMajEquipement != null ? EQ.DateMajEquipement.ToString() : string.Empty;
                //elemEQ.AppendChild(elemEQdetails);

                //elemEQdetails = XmlDoc.CreateElement("Supprime");
                //elemEQdetails.InnerText = EQ.Supprime != null ? EQ.Supprime.ToString() : string.Empty;
                //elemEQ.AppendChild(elemEQdetails);

                // Dernière visite ayant le même type d'évaluation (Si EG, ECD convient également)
                IEnumerable<Visite> visites = EQ.Visites.Where(v => v.EnumTypeEval == co.EnumTypeEval || (co.RefEnumValeur.Valeur == "1" && v.RefEnumValeur.Valeur == "2")).OrderByDescending(v => v.DateVisite);

                elemEQdetails = XmlDoc.CreateElement("Visites");
                if (visites.Count() > 0)
                {
                    elemEQdetails.AppendChild(getVisiteToXML(visites.First(), XmlDoc));
                }

                // Génération d'une visite de saisie vide
                XmlElement elemVisiteProteIN = XmlDoc.CreateElement("VisiteProteIN");

                elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleUtilisateur"));

                XmlElement elemVisiteProteINDetails = XmlDoc.CreateElement("EnumTypeEval");
                elemVisiteProteINDetails.InnerText = co.EnumTypeEval.ToString();
                elemVisiteProteIN.AppendChild(elemVisiteProteINDetails);

                elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("ClePP"));

                elemVisiteProteINDetails = XmlDoc.CreateElement("CleEquipement");
                elemVisiteProteINDetails.InnerText = EQ.CleEquipement.ToString();
                elemVisiteProteIN.AppendChild(elemVisiteProteINDetails);

                elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("DateVisite"));
                elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("EstConfirmee"));
                elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CommentaireVisite"));
                elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CommentaireAnalyse"));
                elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleAnalyse"));
                elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("MesMesures"));

                elemEQdetails.AppendChild(elemVisiteProteIN);

                elemEQ.AppendChild(elemEQdetails);
            }

            return elemEQ;
        }

        /// <summary>
        /// Exporte l'objet MesClassementMesure passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="ClassMes">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getMesClassementMesureToXML(MesClassementMesure ClassMes, XmlDocument XmlDoc)
        {
            XmlElement elemClassMes = XmlDoc.CreateElement("MesClassementMesure");
            elemClassMes.SetAttribute("CV", ClassMes.CourantsVagabons ? "1" : "0");
            elemClassMes.SetAttribute("CA", ClassMes.CourantsAlternatifsInduits ? "1" : "0");
            elemClassMes.SetAttribute("EE", ClassMes.ElectrodeEnterreeAmovible ? "1" : "0");
            elemClassMes.SetAttribute("TME", ClassMes.TemoinEnterre ? "1" : "0");
            elemClassMes.SetAttribute("TMS", ClassMes.TemoinDeSurface ? "1" : "0");
            elemClassMes.SetAttribute("TM", ClassMes.Telemesure ? "1" : "0");

            XmlElement elemClassMesdetails = XmlDoc.CreateElement("CleClassementMesure");
            elemClassMesdetails.InnerText = ClassMes.CleClassementMesure.ToString();
            elemClassMes.AppendChild(elemClassMesdetails);

            //elemClassMesdetails = XmlDoc.CreateElement("CleTypeMesure");
            //elemClassMesdetails.InnerText = ClassMes.CleTypeMesure != null ? ClassMes.CleTypeMesure.ToString() : string.Empty;
            //elemClassMes.AppendChild(elemClassMesdetails);

            elemClassMes.AppendChild(getMesTypeMesureToXML(ClassMes.MesTypeMesure, XmlDoc));

            return elemClassMes;
        }

        /// <summary>
        /// Exporte l'objet MesTypeMesure passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="TypeMes">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getMesTypeMesureToXML(MesTypeMesure TypeMes, XmlDocument XmlDoc)
        {
            XmlElement elemTypeMes = XmlDoc.CreateElement("MesTypeMesure");

            int order = 0;
            //Moyen/Autre
            if (TypeMes.RefEnumValeur.Valeur == "1" || TypeMes.RefEnumValeur.Valeur == "4")
            {
                order = 1;
            }
            //Mini
            else if (TypeMes.RefEnumValeur.Valeur == "0")
            {
                order = 2;
            }

            elemTypeMes.SetAttribute("Niveau", TypeMes.RefEnumValeur.Libelle);
            elemTypeMes.SetAttribute("Ordre", order.ToString());

            XmlElement elemTypeMesdetails = XmlDoc.CreateElement("CleTypeMesure");
            elemTypeMesdetails.InnerText = TypeMes.CleTypeMesure.ToString();
            elemTypeMes.AppendChild(elemTypeMesdetails);

            elemTypeMesdetails = XmlDoc.CreateElement("TypeEvaluation");
            elemTypeMesdetails.InnerText = TypeMes.TypeEvaluation.ToString();
            elemTypeMes.AppendChild(elemTypeMesdetails);

            //elemTypeMesdetails = XmlDoc.CreateElement("CleModeleMesure");
            //elemTypeMesdetails.InnerText = TypeMes.CleModeleMesure != null ? TypeMes.CleModeleMesure.ToString() : string.Empty;
            //elemTypeMes.AppendChild(elemTypeMesdetails);

            //elemTypeMesdetails = XmlDoc.CreateElement("LibTypeMesure");
            //elemTypeMesdetails.InnerText = TypeMes.LibTypeMesure != null ? TypeMes.LibTypeMesure.ToString() : string.Empty;
            //elemTypeMes.AppendChild(elemTypeMesdetails);

            //elemTypeMesdetails = XmlDoc.CreateElement("LibNivAutre");
            //elemTypeMesdetails.InnerText = TypeMes.LibNivAutre != null ? TypeMes.LibNivAutre.ToString() : string.Empty;
            //elemTypeMes.AppendChild(elemTypeMesdetails);

            //elemTypeMesdetails = XmlDoc.CreateElement("MesureEnService");
            //elemTypeMesdetails.InnerText = TypeMes.MesureEnService != null ? TypeMes.MesureEnService.ToString() : string.Empty;
            //elemTypeMes.AppendChild(elemTypeMesdetails);

            elemTypeMesdetails = XmlDoc.CreateElement("NumeroOrdre");
            elemTypeMesdetails.InnerText = TypeMes.MesModeleMesure.NumeroOrdre.ToString();
            elemTypeMes.AppendChild(elemTypeMesdetails);

            elemTypeMesdetails = XmlDoc.CreateElement("MesureComplementaire");
            elemTypeMesdetails.InnerText = TypeMes.MesureComplementaire.ToString();
            elemTypeMes.AppendChild(elemTypeMesdetails);

            elemTypeMes.AppendChild(getMesModeleMesureToXML(TypeMes.MesModeleMesure, XmlDoc));



            return elemTypeMes;
        }

        /// <summary>
        /// Exporte l'objet MesModeleMesure passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="ModeleMes">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getMesModeleMesureToXML(MesModeleMesure ModeleMes, XmlDocument XmlDoc)
        {
            XmlElement elemModeleMes = XmlDoc.CreateElement("MesModeleMesure");

            XmlElement elemeModeleMesDetail = XmlDoc.CreateElement("CleModeleMesure");
            elemeModeleMesDetail.InnerText = ModeleMes.CleModeleMesure.ToString();
            elemModeleMes.AppendChild(elemeModeleMesDetail);

            MesNiveauProtection seuil = ModeleMes.MesNiveauProtection.FirstOrDefault(s => !s.CleEquipement.HasValue && !s.ClePortion.HasValue && !s.ClePp.HasValue);

            elemeModeleMesDetail = XmlDoc.CreateElement("Seuil");
            elemeModeleMesDetail.SetAttribute("Min", seuil != null && seuil.SeuilMini.HasValue ? seuil.SeuilMini.Value.ToString() : "");
            elemeModeleMesDetail.SetAttribute("Max", seuil != null && seuil.SeuilMaxi.HasValue ? seuil.SeuilMaxi.Value.ToString() : "");
            elemModeleMes.AppendChild(elemeModeleMesDetail);

            //elemeModeleMesDetail = XmlDoc.CreateElement("CleUnite");
            //elemeModeleMesDetail.InnerText = ModeleMes.CleUnite != null ? ModeleMes.CleUnite.ToString() : string.Empty;
            //elemModeleMes.AppendChild(elemeModeleMesDetail);

            elemeModeleMesDetail = XmlDoc.CreateElement("UniteSymbole");
            elemeModeleMesDetail.InnerText = ModeleMes.MesUnite != null ? ModeleMes.MesUnite.Symbole : string.Empty;
            elemModeleMes.AppendChild(elemeModeleMesDetail);

            //elemeModeleMesDetail = XmlDoc.CreateElement("UniteLibelle");
            //elemeModeleMesDetail.InnerText = ModeleMes.MesUnite != null ? ModeleMes.MesUnite.Libelle : string.Empty;
            //elemModeleMes.AppendChild(elemeModeleMesDetail);

            elemeModeleMesDetail = XmlDoc.CreateElement("UniteNombreDeDecimales");
            elemeModeleMesDetail.InnerText = ModeleMes.MesUnite != null && ModeleMes.MesUnite.NombreDeDecimales.HasValue ? ModeleMes.MesUnite.NombreDeDecimales.Value.ToString() : "0";
            elemModeleMes.AppendChild(elemeModeleMesDetail);

            //elemeModeleMesDetail = XmlDoc.CreateElement("CleTypeEq");
            //elemeModeleMesDetail.InnerText = ModeleMes.CleTypeEq != null ? ModeleMes.CleTypeEq.ToString() : string.Empty;
            //elemModeleMes.AppendChild(elemeModeleMesDetail);

            //elemeModeleMesDetail = XmlDoc.CreateElement("Libelle");
            //elemeModeleMesDetail.InnerText = ModeleMes.Libelle != null ? ModeleMes.Libelle.ToString() : string.Empty;
            //elemModeleMes.AppendChild(elemeModeleMesDetail);

            elemeModeleMesDetail = XmlDoc.CreateElement("LibGenerique");
            elemeModeleMesDetail.InnerText = ModeleMes.LibGenerique != null ? ModeleMes.LibGenerique.ToString() : string.Empty;
            elemModeleMes.AppendChild(elemeModeleMesDetail);

            elemeModeleMesDetail = XmlDoc.CreateElement("NumeroOrdre");
            elemeModeleMesDetail.InnerText = ModeleMes.NumeroOrdre.ToString();
            elemModeleMes.AppendChild(elemeModeleMesDetail);

            elemeModeleMesDetail = XmlDoc.CreateElement("CodeEquipement");
            elemeModeleMesDetail.InnerText = ModeleMes.TypeEquipement != null && ModeleMes.TypeEquipement.CodeEquipement != null ?
            ModeleMes.TypeEquipement.CodeEquipement.ToString() : string.Empty;
            elemModeleMes.AppendChild(elemeModeleMesDetail);

            elemeModeleMesDetail = XmlDoc.CreateElement("TypeGraphique");
            if (ModeleMes.RefEnumValeur != null)
            {
                elemeModeleMesDetail.InnerText = ModeleMes.RefEnumValeur.LibelleCourt;
            }

            elemModeleMes.AppendChild(elemeModeleMesDetail);

            if (ModeleMes.RefEnumValeur != null && ModeleMes.RefEnumValeur.Valeur == "Ucana ~")
            {
                elemeModeleMesDetail = XmlDoc.CreateElement("RegleDeGestion");

                XmlElement elemetRegle = XmlDoc.CreateElement("XPATH");
                elemetRegle.InnerText = "/ExportTournee/Tournee/PPs/PP[ClePP=[?]PP[/?]]/CourantsAlternatifsInduits";
                elemeModeleMesDetail.AppendChild(elemetRegle);

                elemetRegle = XmlDoc.CreateElement("NomRegle");
                elemetRegle.InnerText = "UAlt";
                elemeModeleMesDetail.AppendChild(elemetRegle);

                elemModeleMes.AppendChild(elemeModeleMesDetail);
            }

            return elemModeleMes;
        }

        #endregion

        #region Export donnée Référence

        /// <summary>
        /// Export des données de référence pour les tournées
        /// </summary>
        /// <param name="To">la tournée a exporter</param>
        /// <param name="XmlDoc"></param>
        /// <returns></returns>
        public XmlElement GetTourneeRefDataToXml(Tournee To, XmlDocument XmlDoc)
        {
            XmlElement princ = XmlDoc.CreateElement("ListeReference");

            XmlElement elem = XmlDoc.CreateElement("Parametres");
            RefParametre param = this.ObjectContext.RefParametre.FirstOrDefault(r => r.Libelle == "SEUIL_INFERIEUR_UCANA");
            if (param != null)
            {
                XmlElement elemDetail = XmlDoc.CreateElement("Parametre");
                elemDetail.SetAttribute("Name", param.Libelle);
                elemDetail.SetAttribute("Valeur", param.Valeur);
                elem.AppendChild(elemDetail);
            }
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("TypeEvaluations");
            IEnumerable<RefEnumValeur> vals = this.ObjectContext.RefEnumValeur.Where(r => r.CodeGroupe == "TYPE_EVAL").OrderBy(r => r.NumeroOrdre);
            foreach (RefEnumValeur item in vals)
            {
                elem.AppendChild(getRefEnumValeurToXML(item, "TypeEvaluation", XmlDoc));
            }
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("DureeEnregistrements");
            vals = this.ObjectContext.RefEnumValeur.Where(r => r.CodeGroupe == "PP_DUREE_ENRG").OrderBy(r => r.NumeroOrdre);
            foreach (RefEnumValeur item in vals)
            {
                elem.AppendChild(getRefEnumValeurToXML(item, "DureeEnregistrement", XmlDoc));
            }
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("Polarisations");
            vals = this.ObjectContext.RefEnumValeur.Where(r => r.CodeGroupe == "PP_POLARISATION").OrderBy(r => r.NumeroOrdre);
            foreach (RefEnumValeur item in vals)
            {
                elem.AppendChild(getRefEnumValeurToXML(item, "Polarisation", XmlDoc));
            }
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("SurfaceTMEs");
            vals = this.ObjectContext.RefEnumValeur.Where(r => r.CodeGroupe == "PP_SURFACE_TME").OrderBy(r => r.NumeroOrdre);
            foreach (RefEnumValeur item in vals)
            {
                elem.AppendChild(getRefEnumValeurToXML(item, "SurfaceTME", XmlDoc));
            }
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("SurfaceTMSs");
            vals = this.ObjectContext.RefEnumValeur.Where(r => r.CodeGroupe == "PP_SURFACE_TMS").OrderBy(r => r.NumeroOrdre);
            foreach (RefEnumValeur item in vals)
            {
                elem.AppendChild(getRefEnumValeurToXML(item, "SurfaceTMS", XmlDoc));
            }
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("AnalyseEtats");
            vals = this.ObjectContext.RefEnumValeur.Where(r => r.CodeGroupe == "AN_ETAT_PC").OrderBy(r => r.NumeroOrdre);
            foreach (RefEnumValeur item in vals)
            {
                elem.AppendChild(getRefEnumValeurToXML(item, "AnalyseEtat", XmlDoc));
            }
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("NiveauxSensibilites");
            foreach (RefNiveauSensibilitePp item in this.ObjectContext.RefNiveauSensibilitePp)
            {
                elem.AppendChild(getNiveauSensibiliteToXML(item, XmlDoc));
            }
            princ.AppendChild(elem);

            elem = XmlDoc.CreateElement("CategoriesPP");
            foreach (CategoriePp item in this.ObjectContext.CategoriePp)
            {
                elem.AppendChild(getCategoriePPToXML(item, XmlDoc));
            }
            princ.AppendChild(elem);

            // Données Utilisateurs
            XmlElement elemUtilisateur = XmlDoc.CreateElement("Utilisateurs");

            IQueryable<UsrUtilisateur> users = this.FindUsrUtilisateurByCleTournee(To.CleTournee);

            foreach (UsrUtilisateur user in users)
            {
                elemUtilisateur.AppendChild(getUsrUtilisateurToXML(user, XmlDoc));
            }
            princ.AppendChild(elemUtilisateur);

            // Données Instruments
            XmlElement elemInstrument = XmlDoc.CreateElement("Instruments");

            IQueryable<InsInstrument> instruments = FindInsInstrumentByCleTournee(To.CleTournee);
            foreach (InsInstrument instrument in instruments)
            {
                elemInstrument.AppendChild(getInsInstrumentToXML(instrument, XmlDoc));
            }
            princ.AppendChild(elemInstrument);

            return princ;
        }

        /// <summary>
        /// Exporte l'objet UsrUtilisateur passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="user">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getUsrUtilisateurToXML(UsrUtilisateur user, XmlDocument XmlDoc)
        {
            XmlElement elemUser = XmlDoc.CreateElement("Utilisateur");

            XmlElement elemUserDetail = XmlDoc.CreateElement("CleUtilisateur");
            elemUserDetail.InnerText = user.CleUtilisateur.ToString();
            elemUser.AppendChild(elemUserDetail);

            elemUserDetail = XmlDoc.CreateElement("Nom");
            elemUserDetail.InnerText = user.Nom;
            elemUser.AppendChild(elemUserDetail);

            elemUserDetail = XmlDoc.CreateElement("Prenom");
            elemUserDetail.InnerText = user.Prenom;
            elemUser.AppendChild(elemUserDetail);

            elemUser.AppendChild(elemUserDetail);

            return elemUser;
        }

        /// <summary>
        /// Exporte l'objet InsInstrument passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="instrument">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getInsInstrumentToXML(InsInstrument instrument, XmlDocument XmlDoc)
        {
            XmlElement elemInstrument = XmlDoc.CreateElement("Instrument");

            XmlElement elemInstrumentDetail = XmlDoc.CreateElement("CleInstrument");
            elemInstrumentDetail.InnerText = instrument.CleInstrument.ToString();
            elemInstrument.AppendChild(elemInstrumentDetail);

            elemInstrumentDetail = XmlDoc.CreateElement("Libelle");
            elemInstrumentDetail.InnerText = instrument.Libelle;
            elemInstrument.AppendChild(elemInstrumentDetail);

            return elemInstrument;
        }

        /// <summary>
        /// Exporte l'objet CatégoriePP passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="cat">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getCategoriePPToXML(CategoriePp cat, XmlDocument XmlDoc)
        {
            XmlElement elemCat = XmlDoc.CreateElement("CategoriePP");

            XmlElement elemCatDetail = XmlDoc.CreateElement("CleCategoriePP");
            elemCatDetail.InnerText = cat.CleCategoriePp.ToString();
            elemCat.AppendChild(elemCatDetail);

            elemCatDetail = XmlDoc.CreateElement("Libelle");
            elemCatDetail.InnerText = cat.Libelle;
            elemCat.AppendChild(elemCatDetail);

            elemCatDetail = XmlDoc.CreateElement("CleNiveauSensibilite");
            elemCatDetail.InnerText = cat.CleNiveauSensibilite.ToString();
            elemCat.AppendChild(elemCatDetail);

            return elemCat;
        }

        /// <summary>
        /// Exporte l'objet RefEnumValeur passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="EnumVal">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getRefEnumValeurToXML(RefEnumValeur EnumVal, String ExportName, XmlDocument XmlDoc)
        {
            XmlElement elemEnum = XmlDoc.CreateElement(ExportName);

            XmlElement elemEnumDetail = XmlDoc.CreateElement("CleEnumValeur");
            elemEnumDetail.InnerText = EnumVal.CleEnumValeur.ToString();
            elemEnum.AppendChild(elemEnumDetail);

            elemEnumDetail = XmlDoc.CreateElement("Libelle");
            elemEnumDetail.InnerText = EnumVal.Libelle;
            elemEnum.AppendChild(elemEnumDetail);

            elemEnumDetail = XmlDoc.CreateElement("LibelleCourt");
            elemEnumDetail.InnerText = EnumVal.LibelleCourt;
            elemEnum.AppendChild(elemEnumDetail);

            elemEnumDetail = XmlDoc.CreateElement("NumeroOrdre");
            elemEnumDetail.InnerText = EnumVal.NumeroOrdre.ToString();
            elemEnum.AppendChild(elemEnumDetail);

            // ajout d'une information d'obligation de commentaire en fonction de l'état de l'analyse
            if (ExportName == "AnalyseEtat")
            {
                elemEnumDetail = XmlDoc.CreateElement("CommAnalyseObligatoire");
                elemEnumDetail.InnerText = (EnumVal.Valeur == "02" || EnumVal.Valeur == "03") ? "1" : "0";
                elemEnum.AppendChild(elemEnumDetail);
            }

            return elemEnum;
        }

        /// <summary>
        /// Exporte l'objet RefEnumValeur passé en paramètre en un XmlElement
        /// </summary>
        /// <param name="EnumVal">Objet à transformer en XML</param>
        /// <param name="XmlDoc">XMLDocument racine</param>
        /// <returns></returns>
        private XmlElement getNiveauSensibiliteToXML(RefNiveauSensibilitePp NivSens, XmlDocument XmlDoc)
        {
            XmlElement elemNivSens = XmlDoc.CreateElement("NiveauSensibilite");

            XmlElement elemNivSensDetail = XmlDoc.CreateElement("CleNiveauSensibilite");
            elemNivSensDetail.InnerText = NivSens.CleNiveauSensibilite.ToString();
            elemNivSens.AppendChild(elemNivSensDetail);

            elemNivSensDetail = XmlDoc.CreateElement("Libelle");
            elemNivSensDetail.InnerText = NivSens.Libelle;
            elemNivSens.AppendChild(elemNivSensDetail);

            elemNivSensDetail = XmlDoc.CreateElement("TypeSensibilite");
            elemNivSensDetail.InnerText = NivSens.TypeSensibilite.ToString();
            elemNivSens.AppendChild(elemNivSensDetail);

            elemNivSensDetail = XmlDoc.CreateElement("CleTypeEvaluation");
            elemNivSensDetail.InnerText = NivSens.EnumTypeEval.ToString();
            elemNivSens.AppendChild(elemNivSensDetail);

            return elemNivSens;
        }

        #endregion

        #region Templates Vides

        /// <summary>
        /// Génération de templates de données vides
        /// </summary>
        /// <param name="XmlDoc"></param>
        /// <returns></returns>
        public XmlElement GetTourneeEmptyTemplateToXml(XmlDocument XmlDoc)
        {
            XmlElement princ = XmlDoc.CreateElement("EmptyTemplate");

            // Template de PP
            XmlElement elem = XmlDoc.CreateElement("PP");

            elem.AppendChild(XmlDoc.CreateElement("ClePP"));
            elem.AppendChild(XmlDoc.CreateElement("CleNiveauSensibilite"));
            elem.AppendChild(XmlDoc.CreateElement("CleCategoriePP"));
            elem.AppendChild(XmlDoc.CreateElement("EnumSurfaceTme"));
            elem.AppendChild(XmlDoc.CreateElement("EnumSurfaceTms"));
            elem.AppendChild(XmlDoc.CreateElement("EnumDureeEnrg"));
            elem.AppendChild(XmlDoc.CreateElement("CleSecteur"));
            elem.AppendChild(XmlDoc.CreateElement("EnumPolarisation"));
            elem.AppendChild(XmlDoc.CreateElement("ClePortion"));
            elem.AppendChild(XmlDoc.CreateElement("Libelle"));
            elem.AppendChild(XmlDoc.CreateElement("Pk"));
            elem.AppendChild(XmlDoc.CreateElement("CourantsVagabonds"));
            elem.AppendChild(XmlDoc.CreateElement("CourantsAlternatifsInduits"));
            elem.AppendChild(XmlDoc.CreateElement("ElectrodeEnterreAmovible"));
            elem.AppendChild(XmlDoc.CreateElement("TemoinEnterreAmovible"));
            elem.AppendChild(XmlDoc.CreateElement("TemoinMetalliqueDeSurface"));
            elem.AppendChild(XmlDoc.CreateElement("Telemesure"));
            elem.AppendChild(XmlDoc.CreateElement("PositionGPSLat"));
            elem.AppendChild(XmlDoc.CreateElement("PositionGPSLong"));
            elem.AppendChild(XmlDoc.CreateElement("CoordonneesGPSFiabilisee"));

            XmlElement elemVisite = XmlDoc.CreateElement("Visites");

            // Génération d'une visite de saisie vide
            XmlElement elemVisiteProteIN = XmlDoc.CreateElement("VisiteProteIN");
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleUtilisateur"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("EnumTypeEval"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("ClePP"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleEquipement"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("DateVisite"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("EstConfirmee"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CommentaireVisite"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CommentaireAnalyse"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleAnalyse"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("MesMesures"));

            elemVisite.AppendChild(elemVisiteProteIN);
            elem.AppendChild(elemVisite);
            princ.AppendChild(elem);


            // Template d'Equipement
            elem = XmlDoc.CreateElement("EQ");

            elem.AppendChild(XmlDoc.CreateElement("CleTypeEvaluation"));
            elem.AppendChild(XmlDoc.CreateElement("TypeEvaluation"));

            elem.AppendChild(XmlDoc.CreateElement("CleEquipement"));
            elem.AppendChild(XmlDoc.CreateElement("ClePp"));
            //elem.AppendChild(XmlDoc.CreateElement("CleTypeEq"));
            elem.AppendChild(XmlDoc.CreateElement("TypeEquipement"));
            elem.AppendChild(XmlDoc.CreateElement("Libelle"));

            elemVisite = XmlDoc.CreateElement("Visites");

            // Génération d'une visite de saisie vide
            elemVisiteProteIN = XmlDoc.CreateElement("VisiteProteIN");
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleUtilisateur"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("EnumTypeEval"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("ClePP"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleEquipement"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("DateVisite"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("EstConfirmee"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CommentaireVisite"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CommentaireAnalyse"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("CleAnalyse"));
            elemVisiteProteIN.AppendChild(XmlDoc.CreateElement("MesMesures"));

            elemVisite.AppendChild(elemVisiteProteIN);
            elem.AppendChild(elemVisite);
            princ.AppendChild(elem);


            // Template de Mesure
            elem = XmlDoc.CreateElement("MesMesure");

            elem.AppendChild(XmlDoc.CreateElement("ModeleMesureLib"));
            elem.AppendChild(XmlDoc.CreateElement("Valeur"));
            elem.AppendChild(XmlDoc.CreateElement("CleTypeMesure"));

            princ.AppendChild(elem);

            // Instruments
            elem = XmlDoc.CreateElement("Instrument");

            elem.AppendChild(XmlDoc.CreateElement("CleInstrument"));
            elem.AppendChild(XmlDoc.CreateElement("Libelle"));

            princ.AppendChild(elem);

            return princ;
        }

        #endregion

        #endregion

        #endregion

        #region TourneeEqPP
        /// <summary>
        /// Retourne les TourneePpEquipement de la clé tournee
        /// </summary>
        /// <param name="cle">Code equipement ou code PP</param>
        /// <param name="type">Pp ou Equipement</param>
        /// <returns></returns>
        public IQueryable<TourneePpEq> GetTourneePpEqByCleEquipment(int cle, string type)
        {
            if (type == "PP")
                return this.ObjectContext.TourneePpEq.Where(v => v.ClePp == cle);
            else
                return this.ObjectContext.TourneePpEq.Where(v => v.CleEquipement == cle);
        }

        /// <summary>
        /// Retourne les TourneePpEquipement de la clé tournee
        /// </summary>
        /// <param name="cle">Code tournee</param>
        /// <returns></returns>
        public IQueryable<TourneePpEq> GetTourneePpEqByCleTournee(int? cle)
        {
            return this.ObjectContext.TourneePpEq.Where(v => v.CleTournee == cle.Value);
        }

        /// <summary>
        /// Retourne les TourneePpEquipement en fonction d'une recherche Géo + date + mot clé
        /// </summary>
        /// <param name="cle">Code tournee</param>
        /// <returns></returns>
        public IQueryable<TourneePpEq> FindTourneePpEqByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur, int? filtreCleEnsElec, int? filtreClePortion, string filtreRechercheLibelle)
        {

            //IQueryable<int> query = this.ObjectContext.Tournees.Where(_ => false).Select(t => t.CleTournee);

            IQueryable<TourneePpEq> objQuery = this.ObjectContext.TourneePpEq;

            if (filtreCleSecteur.HasValue)
            {
                objQuery = objQuery.Where(t => t.CleSecteur == filtreCleSecteur.Value);
            }
            else if (filtreCleAgence.HasValue)
            {
                objQuery = objQuery.Where(t => t.CleAgence == filtreCleAgence.Value);
            }
            else if (filtreCleRegion.HasValue)
            {
                objQuery = objQuery.Where(t => t.CleRegion == filtreCleRegion.Value);
            }

            if (filtreClePortion.HasValue)
            {
                objQuery = objQuery.Where(t => t.ClePortion == filtreClePortion.Value);
            }
            else if (filtreCleEnsElec.HasValue)
            {
                objQuery = objQuery.Where(t => t.CleEnsElectrique == filtreCleEnsElec.Value);
            }

            if (!String.IsNullOrWhiteSpace(filtreRechercheLibelle))
            {
                objQuery = objQuery.Where(t => t.LibelleTournee.Contains(filtreRechercheLibelle));
            }

            IQueryable<int> query = objQuery.Select(t => t.CleTournee).Distinct();

            return this.ObjectContext.TourneePpEq.Where(v => query.Contains(v.CleTournee));
        }

        /// <summary>
        /// Retourne les TourneePpEquipement de la clé tournee
        /// </summary>
        /// <param name="cle">Code equipement ou code PP</param>
        /// <param name="type">Pp ou Equipement</param>
        /// <returns></returns>
        public TourneePpEq GetTourneePpEqByCle(int cle)
        {
            return this.GetTourneePpEq().FirstOrDefault(v => v.CleTournee == cle);
        }

        #endregion

        #region Visites

        public IQueryable<Visite> FindVisitesValideesByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                                                int? filtreCleEnsElec, int? filtreClePortion,
                                                                decimal? pkMin, decimal? pkMax, string typeEq, bool includeDeleted)
        {
            this.ObjectContext.EqEquipement.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Visites.MergeOption = MergeOption.NoTracking;

            ////Initialisation de la query sans resultats
            IQueryable<Visite> queryPp = this.ObjectContext.Visites.Include("Pp").Include("MesMesure") // Il est nécéssaire de récupérer les valeurs des mesures (mantis 19961)
                                                    .Where(v => v.EstValidee && v.ClePp.HasValue);

            IQueryable<Visite> queryEq = this.ObjectContext.Visites.Include("EqEquipement.Pp").Include("MesMesure") // Il est nécéssaire de récupérer les valeurs des mesures (mantis 19961)
                                                    .Where(v => v.EstValidee && v.CleEquipement.HasValue);

            if (filtreCleSecteur.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.CleSecteur == filtreCleSecteur.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.CleSecteur == filtreCleSecteur.Value);
            }
            else if (filtreCleAgence.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
            }
            else if (filtreCleRegion.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
            }

            if (filtreClePortion.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.ClePortion == filtreClePortion.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.PortionIntegrite.ClePortion == filtreClePortion.Value);
            }
            else if (filtreCleEnsElec.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
            }

            if (pkMin.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.Pk >= pkMin.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.Pk >= pkMin.Value);
            }
            if (pkMax.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.Pk <= pkMax.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.Pk <= pkMax.Value);
            }

            if (!includeDeleted)
            {
                queryPp = queryPp.Where(v => !v.Pp.Supprime);
                queryEq = queryEq.Where(v => !v.EqEquipement.Supprime);
            }


            if (!String.IsNullOrEmpty(typeEq))
            {
                if (typeEq != "PP")
                {
                    return queryEq.Where(v => v.EqEquipement.TypeEquipement.CodeEquipement == typeEq).OrderBy(v => v.EqEquipement.TypeEquipement.NumeroOrdre).ThenBy(v => v.EqEquipement.Libelle).ThenByDescending(v => v.DateVisite);
                }
                else
                {
                    return queryPp.OrderBy(v => v.Pp.Libelle).ThenByDescending(v => v.DateVisite);
                }
            }
            else
            {
                return (queryPp.OrderBy(v => v.Pp.Libelle).ThenByDescending(v => v.DateVisite))
                    .Union(queryEq.OrderBy(v => v.EqEquipement.TypeEquipement.NumeroOrdre).ThenBy(v => v.EqEquipement.Libelle).ThenByDescending(v => v.DateVisite));
            }
        }


        /// <summary>
        /// Retourne les visites de l'équipements selctionnées
        /// </summary>
        /// <param name="cleEquipement">Code equipement, null si recherche d'une PP</param>
        /// <param name="clePP">Code PP, null si recherche d'un equipement</param>
        /// <returns></returns>
        public IQueryable<Visite> GetVisitesByCleEquipement(int? cleEquipement, int? clePP)
        {
            if (cleEquipement.HasValue)
                return this.ObjectContext.Visites.Include("InstrumentsUtilises").Include("MesMesure").Where(v => v.CleEquipement == cleEquipement);
            else
                return this.ObjectContext.Visites.Include("InstrumentsUtilises").Include("MesMesure").Where(v => v.ClePp == clePP);
        }

        /// <summary>
        /// Retourne les visites de l'analyse entrée
        /// </summary>
        /// <param name="cleEnsembleElectrique"></param>
        /// <param name="dateDebut">Date début </param>
        /// <param name="dateFin"></param>
        /// <returns></returns>
        public IQueryable<Visite> FindVisitesByAnalyseEECriterias(int cleEnsembleElectrique, DateTime dateDebut, DateTime dateFin)
        {
            dateDebut = dateDebut.Date;
            dateFin = dateFin.Date.AddDays(1);

            return this.ObjectContext.Visites.Include("Alertes").Where(v => v.DateVisite.HasValue && v.DateVisite.Value >= dateDebut && v.DateVisite.Value < dateFin && ((v.ClePp.HasValue && v.Pp.PortionIntegrite.CleEnsElectrique == cleEnsembleElectrique)
                                                                          || v.EqEquipement.Pp.PortionIntegrite.CleEnsElectrique == cleEnsembleElectrique));
        }


        static readonly Func<ProtecaEntities, int, Visite> GetVisiteByCleQuery = CompiledQuery.Compile<ProtecaEntities, int, Visite>((ctx, p) => ctx.Visites
                .Include("MesMesure.Alertes")
                .Include("MesMesure.MesTypeMesure.MesModeleMesure.MesUnite")
                .Include("InstrumentsUtilises.InsInstrument")
                .Include("AnAnalyseSerieMesure.RefEnumValeur").Include("AnAnalyseSerieMesure.Alertes")
                .FirstOrDefault(v => v.CleVisite == p));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<Visite> GetVisiteByCle(int? cle)
        {
            this.ObjectContext.EqEquipement.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.EnsembleElectrique.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesTypeMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesModeleMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.InsInstrument.MergeOption = MergeOption.NoTracking;

            if (cle.HasValue)
            {
                // Appel de la requête compilée intégrant les sous éléments ne pouvant pas être chargé en LasyLoading
                Visite tmp = GetVisiteByCleQuery.Invoke(ObjectContext, cle.Value);

                if (tmp.ClePp.HasValue)
                {
                    // LasyLoading
                    tmp.PpReference.Load();
                    tmp.Pp.PortionIntegriteReference.Load();
                    tmp.Pp.PortionIntegrite.EnsembleElectriqueReference.Load();

                    tmp.Alertes.Load();
                    tmp.UsrUtilisateurReference.Load();
                    tmp.UsrUtilisateur1Reference.Load();
                    tmp.UsrUtilisateur2Reference.Load();
                    tmp.UsrUtilisateur3Reference.Load();


                    return new List<Visite>() { tmp }.AsQueryable();
                }
                else
                {
                    // LasyLoading
                    tmp.EqEquipementReference.Load();
                    tmp.EqEquipement.PpReference.Load();
                    tmp.EqEquipement.Pp.PortionIntegriteReference.Load();
                    tmp.EqEquipement.Pp.PortionIntegrite.EnsembleElectriqueReference.Load();

                    tmp.Alertes.Load();
                    tmp.UsrUtilisateurReference.Load();
                    tmp.UsrUtilisateur1Reference.Load();
                    tmp.UsrUtilisateur2Reference.Load();
                    tmp.UsrUtilisateur3Reference.Load();


                    return new List<Visite>() { tmp }.AsQueryable();
                }
            }

            return null;
        }


        private IQueryable<Visite> FilterVisitesEqNonValidees(IQueryable<Visite> eqQuery, int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                                         int? filtreCleEnsElec, int? filtreClePortion,
                                                         DateTime? dateMin, DateTime? dateMax)
        {
            eqQuery = eqQuery.Where(v => v.CleEquipement.HasValue && !v.EstValidee);

            if (dateMin.HasValue)
                eqQuery = eqQuery.Where(v => v.DateImport.HasValue && v.DateImport.Value >= dateMin.Value);

            if (dateMax.HasValue)
                eqQuery = eqQuery.Where(v => v.DateImport.HasValue && v.DateImport.Value < dateMax.Value);

            if (filtreCleSecteur.HasValue)
                eqQuery = eqQuery.Where(v => v.EqEquipement.Pp.CleSecteur == filtreCleSecteur.Value);
            else if (filtreCleAgence.HasValue)
                eqQuery = eqQuery.Where(v => v.EqEquipement.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
            else if (filtreCleRegion.HasValue)
                eqQuery = eqQuery.Where(v => v.EqEquipement.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);

            if (filtreClePortion.HasValue)
                eqQuery = eqQuery.Where(v => v.EqEquipement.Pp.ClePortion == filtreClePortion.Value);
            else if (filtreCleEnsElec.HasValue)
                eqQuery = eqQuery.Where(v => v.EqEquipement.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);

            return eqQuery;
        }

        private IQueryable<Visite> FilterVisitesPpNonValidees(IQueryable<Visite> ppQuery, int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                                         int? filtreCleEnsElec, int? filtreClePortion,
                                                         DateTime? dateMin, DateTime? dateMax)
        {
            ppQuery = ppQuery.Where(v => v.ClePp.HasValue && !v.EstValidee);

            if (dateMin.HasValue)
                ppQuery = ppQuery.Where(v => v.DateImport.HasValue && v.DateImport.Value >= dateMin.Value);

            if (dateMax.HasValue)
                ppQuery = ppQuery.Where(v => v.DateImport.HasValue && v.DateImport.Value < dateMax.Value);

            if (filtreCleSecteur.HasValue)
                ppQuery = ppQuery.Where(v => v.Pp.CleSecteur == filtreCleSecteur.Value);
            else if (filtreCleAgence.HasValue)
                ppQuery = ppQuery.Where(v => v.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
            else if (filtreCleRegion.HasValue)
                ppQuery = ppQuery.Where(v => v.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);

            if (filtreClePortion.HasValue)
                ppQuery = ppQuery.Where(v => v.Pp.ClePortion == filtreClePortion.Value);
            else if (filtreCleEnsElec.HasValue)
                ppQuery = ppQuery.Where(v => v.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);

            return ppQuery;
        }


        /// <summary>
        /// Retourne les Visites sur des critères Geo et de date
        /// </summary>
        /// <param name="filtreCleRegion"></param>
        /// <param name="filtreCleAgence"></param> 
        /// <param name="filtreCleSecteur"></param>
        /// <param name="filtreCleEnsElec"></param>
        /// <param name="filtreClePortion"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <returns></returns>
        public IEnumerable<Visite> FindVisitesNonValideesByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                                         int? filtreCleEnsElec, int? filtreClePortion,
                                                         DateTime? dateMin, DateTime? dateMax)
        {
            this.ObjectContext.MesTypeMesure.MergeOption = MergeOption.NoTracking;
            //this.ObjectContext.MesMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesModeleMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.RefEnumValeur.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesUnite.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesNiveauProtection.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.EqEquipement.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;

            if (dateMin.HasValue)
            {
                dateMin = dateMin.Value.Date;
            }
            if (dateMax.HasValue)
            {
                dateMax = dateMax.Value.AddDays(1).Date;
            }
            var visitesPPQuery = FilterVisitesPpNonValidees(this.ObjectContext.Visites, filtreCleRegion, filtreCleAgence, filtreCleSecteur, filtreCleEnsElec, filtreClePortion, dateMin, dateMax);
            var visitesEQQuery = FilterVisitesEqNonValidees(this.ObjectContext.Visites, filtreCleRegion, filtreCleAgence, filtreCleSecteur, filtreCleEnsElec, filtreClePortion, dateMin, dateMax);

            var visitePP = visitesPPQuery.ToList();
            visitesPPQuery.Select(v => v.Pp).ToList();
            visitesPPQuery.Select(v => v.Pp.PortionIntegrite).ToList();

            var visiteEQ = visitesEQQuery.ToList();
            visitesEQQuery.Select(v => v.EqEquipement).ToList();
            visitesEQQuery.Select(v => v.EqEquipement.Pp).ToList();
            visitesEQQuery.Select(v => v.EqEquipement.Pp.PortionIntegrite).ToList();

            visitesPPQuery.SelectMany(v => v.MesMesure).ToList();
            visitesEQQuery.SelectMany(v => v.MesMesure).ToList();

            visitesPPQuery.SelectMany(v => v.MesMesure.Select(vm => vm.MesTypeMesure)).ToList();
            visitesEQQuery.SelectMany(v => v.MesMesure.Select(vm => vm.MesTypeMesure)).ToList();

            visitesPPQuery.SelectMany(v => v.MesMesure.Select(vm => vm.MesTypeMesure.MesModeleMesure)).ToList();
            visitesEQQuery.SelectMany(v => v.MesMesure.Select(vm => vm.MesTypeMesure.MesModeleMesure)).ToList();

            visitesPPQuery.SelectMany(v => v.Alertes).ToList();
            visitesEQQuery.SelectMany(v => v.Alertes).ToList();

            visitesEQQuery.Select(v => v.UsrUtilisateur).ToList();
            visitesPPQuery.Select(v => v.UsrUtilisateur).ToList();

            visitesPPQuery.Select(v => v.UsrUtilisateur2).ToList();
            visitesEQQuery.Select(v => v.UsrUtilisateur2).ToList();

            visitesPPQuery.SelectMany(v => v.InstrumentsUtilises).ToList();
            visitesEQQuery.SelectMany(v => v.InstrumentsUtilises).ToList();

            visitesPPQuery.SelectMany(v => v.InstrumentsUtilises.Select(iu => iu.InsInstrument)).ToList();
            visitesEQQuery.SelectMany(v => v.InstrumentsUtilises.Select(iu => iu.InsInstrument)).ToList();

            visitesPPQuery.SelectMany(v => v.AnAnalyseSerieMesure).ToList();
            visitesEQQuery.SelectMany(v => v.AnAnalyseSerieMesure).ToList();

            visitesPPQuery.Select(v => v.RefEnumValeur).ToList();
            visitesEQQuery.Select(v => v.RefEnumValeur).ToList();


            // Chargement en mémoire de la dernière visite validée de même type pour chaque visite non validée.
            // le chargement en mémoire permet de transmettre au Silverlight les éléments chargés en plus de la requete principale.


            var queryOldVisitePp = from v in visitesPPQuery
                                   select this.ObjectContext.Visites
                                                 .Where(vp => vp.EstValidee
                                                             && vp.ClePp.HasValue
                                                             && vp.ClePp == v.ClePp
                                                             && (vp.EnumTypeEval == v.EnumTypeEval || (vp.RefEnumValeur.Valeur == "2" && v.RefEnumValeur.Valeur == "1")))
                                                 .OrderByDescending(vp => vp.DateVisite)
                                                 .Select(vp => vp.CleVisite)
                                                 .FirstOrDefault();

            var queryOldVisiteEq = from v in visitesEQQuery
                                   select this.ObjectContext.Visites
                                                 .Where(vp => vp.EstValidee
                                                             && vp.CleEquipement.HasValue
                                                             && vp.CleEquipement == v.CleEquipement
                                                             && (vp.EnumTypeEval == v.EnumTypeEval || (vp.RefEnumValeur.Valeur == "2" && v.RefEnumValeur.Valeur == "1")))
                                                 .OrderByDescending(vp => vp.DateVisite)
                                                 .Select(vp => vp.CleVisite)
                                                 .FirstOrDefault();

            var oldVisiteLoad = this.ObjectContext.Visites.Include("MesMesure")
                .Where(v => queryOldVisitePp.Concat(queryOldVisiteEq).Contains(v.CleVisite))
                .ToList();
            //var oldVisiteLoad = queryOldVisite.ToList();
            //var oldVisiteLoad = queryOldVisitePp.Union(queryOldVisiteEq).ToList();

            return (visitePP)
                .Union(visiteEQ)
                .ToList();
        }

        /*
        /// <summary>
        /// Retourne les Visites sur des critères Geo et de date
        /// </summary>
        /// <param name="filtreCleRegion"></param>
        /// <param name="filtreCleAgence"></param> 
        /// <param name="filtreCleSecteur"></param>
        /// <param name="filtreCleEnsElec"></param>
        /// <param name="filtreClePortion"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <returns></returns>
        public IQueryable<Visite> FindVisitesNonValideesByCriterias(int? filtreCleRegion, int? filtreCleAgence, int? filtreCleSecteur,
                                                         int? filtreCleEnsElec, int? filtreClePortion,
                                                         DateTime? dateMin, DateTime? dateMax)
        {
            this.ObjectContext.MesTypeMesure.MergeOption = MergeOption.NoTracking;
            //this.ObjectContext.MesMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesModeleMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesUnite.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesNiveauProtection.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.EqEquipement.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;

            ////Initialisation de la query sans resultats
            IQueryable<Visite> queryPp = this.ObjectContext.Visites
                                                    .Include("Pp.PortionIntegrite")
                                                    .Include("MesMesure")
                                                    .Include("UsrUtilisateur").Include("UsrUtilisateur2")
                                                    .Include("InstrumentsUtilises.InsInstrument")
                                                    .Include("AnAnalyseSerieMesure")
                                                    .Where(v => !v.EstValidee && v.ClePp.HasValue);

            IQueryable<Visite> queryEq = this.ObjectContext.Visites
                                                    .Include("EqEquipement.Pp.PortionIntegrite")
                                                    .Include("MesMesure")
                                                    .Include("UsrUtilisateur").Include("UsrUtilisateur2")
                                                    .Include("InstrumentsUtilises.InsInstrument")
                                                    .Include("AnAnalyseSerieMesure")
                                                    .Where(v => !v.EstValidee && v.CleEquipement.HasValue);

            if (filtreCleSecteur.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.CleSecteur == filtreCleSecteur.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.CleSecteur == filtreCleSecteur.Value);
            }
            else if (filtreCleAgence.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.GeoSecteur.CleAgence == filtreCleAgence.Value);
            }
            else if (filtreCleRegion.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.GeoSecteur.GeoAgence.CleRegion == filtreCleRegion.Value);
            }

            if (filtreClePortion.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.ClePortion == filtreClePortion.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.PortionIntegrite.ClePortion == filtreClePortion.Value);
            }
            else if (filtreCleEnsElec.HasValue)
            {
                queryPp = queryPp.Where(v => v.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
                queryEq = queryEq.Where(v => v.EqEquipement.Pp.PortionIntegrite.CleEnsElectrique == filtreCleEnsElec.Value);
            }

            if (dateMin.HasValue)
            {
                dateMin = dateMin.Value.Date;
                queryPp = queryPp.Where(v => v.DateImport.HasValue && v.DateImport.Value >= dateMin);
                queryEq = queryEq.Where(v => v.DateImport.HasValue && v.DateImport.Value >= dateMin);
            }
            if (dateMax.HasValue)
            {
                dateMax = dateMax.Value.AddDays(1).Date;
                queryPp = queryPp.Where(v => v.DateImport.HasValue && v.DateImport.Value < dateMax);
                queryEq = queryEq.Where(v => v.DateImport.HasValue && v.DateImport.Value < dateMax);
            }

            // Chargement en mémoire de la dernière visite validée de même type pour chaque visite non validée.
            // le chargement en mémoire permet de transmettre au Silverlight les éléments chargés en plus de la requete principale.

            IQueryable<Visite> queryOldVisitePp = from v in queryPp
                                                  select this.ObjectContext.Visites
                                                                .Where(vp => vp.EstValidee
                                                                            && vp.ClePp.HasValue
                                                                            && vp.ClePp == v.ClePp
                                                                            && (vp.EnumTypeEval == v.EnumTypeEval || (vp.RefEnumValeur.Valeur == "2" && v.RefEnumValeur.Valeur == "1")))
                                                                .OrderByDescending(vp => vp.DateVisite).FirstOrDefault();

            IQueryable<Visite> queryOldVisiteEq = from v in queryEq
                                                  select this.ObjectContext.Visites
                                                                .Where(vp => vp.EstValidee
                                                                            && vp.CleEquipement.HasValue
                                                                            && vp.CleEquipement == v.CleEquipement
                                                                            && (vp.EnumTypeEval == v.EnumTypeEval || (vp.RefEnumValeur.Valeur == "2" && v.RefEnumValeur.Valeur == "1")))
                                                                .OrderByDescending(vp => vp.DateVisite).FirstOrDefault();

            var oldVisite = queryOldVisitePp.Where(v => v != null).Select(v => v.CleVisite).ToList().Union(queryOldVisiteEq.Where(v => v != null).Select(v => v.CleVisite).ToList());

            IQueryable<Visite> queryOldVisite = this.ObjectContext.Visites.Include("MesMesure")
                                                                .Where(v => oldVisite.Contains(v.CleVisite));

            var oldVisiteLoad = queryOldVisite.ToList();

            return (queryPp.OrderBy(v => v.Pp.PortionIntegrite.Libelle).ThenBy(v => v.Pp.Pk).ThenBy(v => v.Pp.Libelle).ThenBy(v => v.DateVisite))
                .Union(queryEq.OrderBy(v => v.EqEquipement.Pp.PortionIntegrite.Libelle).ThenBy(v => v.EqEquipement.Pp.Pk).ThenBy(v => v.EqEquipement.Libelle).ThenBy(v => v.DateVisite));
        }
        */

        /// <summary>
        /// Retourne les visites de l'équipements selctionnées
        /// </summary>
        /// <param name="cleEquipement">Code equipement, null si recherche d'une PP</param>
        /// <param name="clePP">Code PP, null si recherche d'un equipement</param>
        /// <returns></returns>
        public IQueryable<Visite> GetVisiteByCleLight(int cle)
        {
            this.ObjectContext.MesMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Visites.MergeOption = MergeOption.NoTracking;

            return this.ObjectContext.Visites.Include("MesMesure.MesTypeMesure.MesModeleMesure.MesUnite").Where(v => v.CleVisite == cle);
        }

        #endregion

        #region Alerte

        public Alerte GetAlerteByCle(int cle)
        {
            return this.ObjectContext.Alertes
                    .Include("Visite")
                    .Include("Visite.Pp")
                    .Include("Visite.Pp.PortionIntegrite")
                    .Include("Visite.EqEquipement")
                    .Include("Visite.EqEquipement.Pp")
                    .Include("Visite.EqEquipement.Pp.PortionIntegrite")
                    .FirstOrDefault(v => v.CleAlerte == cle);
        }

        public IQueryable<Alerte> FindAlertes()
        {
            this.ObjectContext.Visites.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.AnAnalyse.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Pps.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.PortionIntegrite.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.EqEquipement.MergeOption = MergeOption.NoTracking;

            return this.ObjectContext.Alertes
                    .Include("Visite")
                    .Include("Visite.Pp")
                    .Include("Visite.Pp.PortionIntegrite")
                    .Include("Visite.EqEquipement")
                    .Include("Visite.EqEquipement.Pp")
                    .Include("Visite.EqEquipement.Pp.PortionIntegrite");
        }

        /// <summary>
        /// Recherche des Alertes par critères
        /// </summary>
        /// <param name="cleRegion"></param>
        /// <param name="cleAgence"></param>
        /// <param name="cleSecteur"></param>
        /// <param name="cleEnsElec"></param>
        /// <param name="clePortion"></param>
        /// <param name="pkMin"></param>
        /// <param name="pkMax"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <param name="includeDisabled"></param>
        /// <param name="listTypeAlerte"></param>
        /// <returns></returns>
        public IQueryable<Alerte> FindAlertesByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElec, int? clePortion,
            decimal? pkMin, decimal? pkMax, DateTime dateMin, DateTime dateMax, bool includeDisabled, ObservableCollection<String> listTypeAlerte)
        {
            //Mise en place des triggers de selection pour la date
            dateMin = dateMin.Date;
            dateMax = dateMax.Date.AddDays(1);

            //Initialisation de la query sans resultats
            IQueryable<Alerte> query = this.ObjectContext.Alertes.Where(_ => false);


            //Définition des includes
            ObjectQuery<Alerte> objQuery = this.ObjectContext.Alertes;

            if (clePortion.HasValue)
            {
                query = objQuery.Where(a => listTypeAlerte.Contains(a.RefEnumValeur.Valeur)
                                                && (includeDisabled || a.Supprime == includeDisabled)
                                                && a.Date >= dateMin
                                                && a.Date < dateMax
                                                && a.CleVisite.HasValue
                                                && a.Visite.EstValidee
                                                && a.Visite.Pp.ClePortion == clePortion.Value
                                                && (!pkMin.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk >= pkMin.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk >= pkMin.Value))
                                                && (!pkMax.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk <= pkMax.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk <= pkMax.Value))
                                            );
            }
            else if (cleEnsElec.HasValue)
            {
                query = objQuery.Where(a => listTypeAlerte.Contains(a.RefEnumValeur.Valeur)
                                                 && (includeDisabled || a.Supprime == includeDisabled)
                                                 && a.Date >= dateMin
                                                 && a.Date < dateMax
                                                 && a.CleVisite.HasValue
                                                 && a.Visite.EstValidee
                                                 && a.Visite.Pp.PortionIntegrite.CleEnsElectrique == cleEnsElec.Value
                                                 && (!pkMin.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk >= pkMin.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk >= pkMin.Value))
                                                 && (!pkMax.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk <= pkMax.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk <= pkMax.Value))
                                             );
            }
            else if (cleSecteur.HasValue)
            {
                query = objQuery.Where(a => listTypeAlerte.Contains(a.RefEnumValeur.Valeur)
                                                && (includeDisabled || a.Supprime == includeDisabled)
                                                && a.Date >= dateMin
                                                && a.Date < dateMax
                                                && a.CleVisite.HasValue
                                                && a.Visite.EstValidee
                                                && a.Visite.Pp.CleSecteur == cleSecteur.Value
                                                && (!pkMin.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk >= pkMin.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk >= pkMin.Value))
                                                && (!pkMax.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk <= pkMax.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk <= pkMax.Value))
                                            );
            }
            else if (cleAgence.HasValue)
            {
                query = objQuery.Where(a => listTypeAlerte.Contains(a.RefEnumValeur.Valeur)
                                                && (includeDisabled || a.Supprime == includeDisabled)
                                                && a.Date >= dateMin
                                                && a.Date < dateMax
                                                && a.CleVisite.HasValue
                                                && a.Visite.EstValidee
                                                && a.Visite.Pp.GeoSecteur.CleAgence == cleAgence.Value
                                                && (!pkMin.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk >= pkMin.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk >= pkMin.Value))
                                                && (!pkMax.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk <= pkMax.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk <= pkMax.Value))
                                            );
            }
            else if (cleRegion.HasValue)
            {
                query = objQuery.Where(a => listTypeAlerte.Contains(a.RefEnumValeur.Valeur)
                                                && (includeDisabled || a.Supprime == includeDisabled)
                                                && a.Date >= dateMin
                                                && a.Date < dateMax
                                                && a.CleVisite.HasValue
                                                && a.Visite.EstValidee
                                                && a.Visite.Pp.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value
                                                && (!pkMin.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk >= pkMin.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk >= pkMin.Value))
                                                && (!pkMax.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk <= pkMax.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk <= pkMax.Value))
                                            );
            }
            else
            {
                query = objQuery.Where(a => listTypeAlerte.Contains(a.RefEnumValeur.Valeur)
                                                && (includeDisabled || a.Supprime == includeDisabled)
                                                && a.Date >= dateMin
                                                && a.Date < dateMax
                                                && a.CleVisite.HasValue
                                                && a.Visite.EstValidee
                                                && (!pkMin.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk >= pkMin.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk >= pkMin.Value))
                                                && (!pkMax.HasValue || (a.Visite.ClePp.HasValue && a.Visite.Pp.Pk <= pkMax.Value) || (a.Visite.CleEquipement.HasValue && a.Visite.EqEquipement.Pp.Pk <= pkMax.Value))
                                            );
            }

            return query;
        }

        [Query(HasSideEffects = true)]
        public IQueryable<Alerte> FindAlertesByListCleAlerte(ObservableCollection<int> listCleAlerte)
        {
            int step = 2000;

            if (listCleAlerte.Count > step)
            {
                List<Alerte> alertes = new List<Alerte>();
                int index = 0;
                while (index < listCleAlerte.Count)
                {
                    IEnumerable<int> list = listCleAlerte.Skip(index).Take(step);
                    if (index == 0)
                    {
                        alertes = this.ObjectContext.Alertes.Where(a => list.Contains(a.CleAlerte)).ToList();
                    }
                    else
                    {
                        alertes.AddRange(this.ObjectContext.Alertes.Where(a => list.Contains(a.CleAlerte)).ToList());
                    }
                    index += step;
                }

                return alertes.AsQueryable();
            }
            else
            {
                return this.ObjectContext.Alertes.Where(a => listCleAlerte.Contains(a.CleAlerte));
            }
        }

        [Query(HasSideEffects = true)]
        public IQueryable<AlerteDetail> FindAlerteDetailByListCleAlerte(ObservableCollection<int> listCleAlerte)
        {
            int step = 2000;

            if (listCleAlerte.Count > step)
            {
                List<AlerteDetail> alertes = new List<AlerteDetail>();
                int index = 0;
                while (index < listCleAlerte.Count)
                {
                    IEnumerable<int> list = listCleAlerte.Skip(index).Take(step);
                    if (index == 0)
                    {
                        alertes = this.ObjectContext.AlerteDetail.Where(a => list.Contains(a.CleAlerte)).ToList();
                    }
                    else
                    {
                        alertes.AddRange(this.ObjectContext.AlerteDetail.Where(a => list.Contains(a.CleAlerte)).ToList());
                    }
                    index += step;
                }

                return alertes.AsQueryable();
            }
            else
            {
                return this.ObjectContext.AlerteDetail.Where(a => listCleAlerte.Contains(a.CleAlerte));
            }
        }

        public IQueryable<Alerte> FindAlerteByClePP(int clePp, bool includeDisable)
        {
            IQueryable<Alerte> query = this.ObjectContext.Alertes.Where(a => (includeDisable || !a.Supprime) && (a.Visite.ClePp == clePp || a.Visite.EqEquipement.ClePp == clePp));
            return query;
        }

        public IQueryable<AlerteDetail> FindAlerteDetailByClePP(int clePp, bool includeDisable)
        {
            IQueryable<int> ClesAlerte = this.ObjectContext.Alertes.Where(a => (includeDisable || !a.Supprime) && (a.Visite.ClePp == clePp || a.Visite.EqEquipement.ClePp == clePp)).Select(a => a.CleAlerte);
            IQueryable<AlerteDetail> queryDetail = this.ObjectContext.AlerteDetail.Where(a => ClesAlerte.Contains(a.CleAlerte)).Distinct();
            return queryDetail;
        }

        public IQueryable<AlerteDetail> FindAlerteDetailsByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElec, int? clePortion,
            decimal? pkMin, decimal? pkMax, DateTime dateMin, DateTime dateMax, bool includeDisabled, ObservableCollection<String> listTypeAlerte)
        {
            //Mise en place des triggers de selection pour la date
            dateMin = dateMin.Date;
            dateMax = dateMax.Date.AddDays(1);

            IQueryable<AlerteDetail> queryDetail = this.ObjectContext.AlerteDetail.Where(a => listTypeAlerte.Contains(a.Type));

            if (!includeDisabled)
            {
                queryDetail = queryDetail.Where(a => a.Supprime == false);
            }

            queryDetail = queryDetail.Where(a => a.Date >= dateMin);
            queryDetail = queryDetail.Where(a => a.Date < dateMax);

            if (pkMin.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.Pk >= pkMin.Value);
            }
            if (pkMax.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.Pk <= pkMax.Value);
            }

            if (cleSecteur.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.CleSecteur == cleSecteur.Value);
            }
            else if (cleAgence.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.CleAgence == cleAgence.Value);
            }
            else if (cleRegion.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.CleRegion == cleRegion.Value);
            }

            if (clePortion.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.ClePortion == clePortion.Value);
            }
            else if (cleEnsElec.HasValue)
            {
                queryDetail = queryDetail.Where(a => a.CleEnsElectrique == cleEnsElec.Value);
            }

            return queryDetail.OrderBy(ad => ad.LibellePortion).ThenBy(ad => ad.Pk).ThenBy(ad => ad.LibelleType).ThenBy(ad => ad.Date);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public Alerte GetAlerteWithVisiteByCle(int cle)
        {
            this.ObjectContext.Visites.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.MesMesure.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.Alertes.MergeOption = MergeOption.NoTracking;

            return this.ObjectContext.Alertes.Include("Visite").Include("Visite.MesMesure").FirstOrDefault(a => a.CleAlerte == cle);
        }

        #endregion Alerte

        #region AnAnalyse

        /// <summary>
        /// Retourne le cout de mesure sélectionné
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public AnAnalyse GetAnAnalyseByCle(int cle)
        {
            return this.ObjectContext.AnAnalyse.FirstOrDefault(e => e.CleAnalyse == cle);
        }

        #endregion

        #region AnAction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public AnAction GetAnActionByCle(int? cle)
        {
            this.ObjectContext.ParametreAction.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.RefEnumValeur.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.UsrUtilisateur.MergeOption = MergeOption.NoTracking;
            this.ObjectContext.AnAnalyse.MergeOption = MergeOption.NoTracking;

            if (cle.HasValue)
            {
                var act = this.ObjectContext.AnAction.Include("UsrUtilisateur")
                                                    .Include("UsrUtilisateur1")
                                                    .Include("ParametreAction")
                                                    .Include("ParametreAction.RefEnumValeur1")
                                                    .Include("ParametreAction.RefEnumValeur2")
                                                    .Include("AnAnalyse")
                                                    .Include("UsrUtilisateurResp")
                                                    .Include("UsrUtilisateurAgent")
                                                    .Include("PortionIntegriteAnAction")
                                                    .Include("PortionIntegriteAnAction.PortionIntegrite")
                                                    .Where(v => v.CleAction == cle).FirstOrDefault();
                if (act != null && act.CleAnalyse == null && act.PortionIntegriteAnAction.Any())
                {
                    var region = (from po in act.PortionIntegriteAnAction
                                  join pi in this.ObjectContext.PiSecteurs.Include("GeoSecteur.GeoAgence.GeoRegion") on po.ClePortion equals pi.ClePortion
                                  where pi.GeoSecteur.GeoAgence.GeoRegion.LibelleAbregeRegion == act.NumActionPc.Split(new char[] { (char)'-' })[1]
                                  select pi.GeoSecteur.GeoAgence.CleRegion).FirstOrDefault();
                    act.CleRegion = region;
                    var cleEnsElec = act.PortionIntegriteAnAction.First().PortionIntegrite.CleEnsElectrique;

                    act.CleEnsembleElec = cleEnsElec;
                }
                return act;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Supprime logiquement une action
        /// </summary>
        public void LogicalDeleteAction(int cleAction, int cleUtilisateur)
        {
            AnAction action = this.ObjectContext.AnAction.FirstOrDefault(p => p.CleAction == cleAction);
            action.Supprime = true;
            action.CleUtilisateurModification = cleUtilisateur;
            this.ObjectContext.SaveChanges();
        }

        /// <summary>
        /// Réintégration des actions supprimées
        /// </summary>
        public void ReintegrateAction(int cleAction, int cleUtilisateur)
        {
            AnAction action = this.ObjectContext.AnAction.FirstOrDefault(p => p.CleAction == cleAction);
            action.Supprime = false;
            action.CleUtilisateurModification = cleUtilisateur;
            this.ObjectContext.SaveChanges();
        }

        /// <summary>
        /// Update de la date d'édition d'un rapport sans passer par le validateur d'entity framework
        /// </summary>
        public void PatchUpdateDateRapportAnalyse(int cleAnalyse)
        {
            object[] obj = { new SqlParameter("@CLE", cleAnalyse) };
            this.ObjectContext.ExecuteStoreQuery<AnAnalyse>("UPDATE AN_ANALYSE set DATE_EDITION = CONVERT (date, GETDATE()) where CLE_ANALYSE = @CLE", obj);
        }

        /// <summary>
        /// Récupère la liste des Actions de l'ensemble électrique
        /// </summary>
        /// <param name="cle">identifiant de l'ensemble électrique</param>
        /// <returns>la liste des actions de l'ensemble électrique</returns>
        public IQueryable<AnAction> GetActionsByEnsembleElectrique(int cleEE)
        {
            IQueryable<AnAction> actionsQuery = null;

            IQueryable<AnAction> actionsAvecAnalyse = (from ac in this.ObjectContext.AnAction
                                                       join a in this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>() on ac.CleAnalyse equals a.CleAnalyse
                                                       where a.CleEnsElectrique == cleEE && ac.Supprime == false
                                                       select ac);

            IQueryable<AnAction> actionsHorsAnalyse = (from ac in this.ObjectContext.AnAction
                                                       from p in this.ObjectContext.PortionIntegrite
                                                       from pa in p.PortionIntegriteAnAction
                                                       where pa.AnAction.CleAnalyse == null && p.CleEnsElectrique == cleEE && pa.CleAction == ac.CleAction && p.ClePortion == pa.ClePortion && ac.Supprime == false
                                                       select ac);

            actionsQuery = actionsAvecAnalyse.Union(actionsHorsAnalyse);

            var parametres = this.ObjectContext.ParametreAction.Where(p => actionsQuery.Any(a => a.CleParametreAction == p.CleParametreAction)).ToList();

            return actionsQuery.OrderBy(a => a.DateCreation);
        }

        /// <summary>
        /// Récupère la liste des Actions d'une portion intégrité
        /// </summary>
        /// <param name="cle">identifiant d'une portion intégrité</param>
        /// <returns>la liste des actions d'une portion intégrité</returns>
        public IQueryable<AnAction> GetActionsByPortionIntegrite(int clePI)
        {
            IQueryable<AnAction> actionsQuery = (from ac in this.ObjectContext.AnAction
                                                 from p in this.ObjectContext.PortionIntegrite
                                                 from pa in p.PortionIntegriteAnAction
                                                 where pa.AnAction.CleAnalyse == null && p.ClePortion == clePI && pa.CleAction == ac.CleAction && p.ClePortion == pa.ClePortion && ac.Supprime == false
                                                 select ac);

            var parametres = this.ObjectContext.ParametreAction.Where(p => actionsQuery.Any(a => a.CleParametreAction == p.CleParametreAction)).ToList();

            return actionsQuery.OrderBy(a => a.DateCreation);
        }

        /// <summary>
        /// Récupère la liste des Actions d'un secteur
        /// </summary>
        /// <param name="cle">identifiant d'un secteur</param>
        /// <returns>la liste des actions d'un secteur</returns>
        public IQueryable<AnAction> GetActionsBySecteur(int cleSecteur)
        {
            IQueryable<AnAction> query = null;

            // actions par analyse
            query = this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>().Where
                (
                    a => a.EnsembleElectrique.PortionIntegrite.Any(p => p.PiSecteurs.Any(s => s.CleSecteur == cleSecteur))
                ).SelectMany(a => a.AnAction);

            query = query.Union(this.ObjectContext.AnAnalyse.OfType<AnAnalyseSerieMesure>().Where
                (
                    a => a.Visite.ClePp.HasValue && a.Visite.Pp.CleSecteur == cleSecteur
                ).SelectMany(a => a.AnAction));
            query = query.Union(this.ObjectContext.AnAnalyse.OfType<AnAnalyseSerieMesure>().Where
                (
                    a => a.Visite.CleEquipement.HasValue
                    && a.Visite.EqEquipement.Pp.CleSecteur == cleSecteur
                ).SelectMany(a => a.AnAction));

            // actions hors analyse
            query = query.Union(
                    from ac in this.ObjectContext.AnAction
                    from pia in this.ObjectContext.PortionIntegriteAnAction
                    from pis in this.ObjectContext.PiSecteurs
                    where ac.CleAction == pia.CleAction && pia.ClePortion == pis.ClePortion && pis.CleSecteur == cleSecteur
                    select ac
                );

            return query;
        }

        /// <summary>
        /// Récupère la liste des Actions d'une agence
        /// </summary>
        /// <param name="cle">identifiant d'une agence </param>
        /// <returns>la liste des actions d'une agence </returns>
        public IQueryable<AnAction> GetActionsByAgence(int cleAgence)
        {
            IQueryable<AnAction> query = null;

            // actions par analyse
            query = this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>().Where
                (
                    a => a.EnsembleElectrique.PortionIntegrite.Any(p => p.PiSecteurs.Any(s => s.GeoSecteur.CleAgence == cleAgence))
                ).SelectMany(a => a.AnAction);

            query = query.Union(this.ObjectContext.AnAnalyse.OfType<AnAnalyseSerieMesure>().Where
                (
                    a => a.Visite.ClePp.HasValue && a.Visite.Pp.GeoSecteur.CleAgence == cleAgence
                ).SelectMany(a => a.AnAction));
            query = query.Union(this.ObjectContext.AnAnalyse.OfType<AnAnalyseSerieMesure>().Where
                (
                    a => a.Visite.CleEquipement.HasValue
                    && a.Visite.EqEquipement.Pp.GeoSecteur.CleAgence == cleAgence
                ).SelectMany(a => a.AnAction));

            // actions hors analyse
            query = query.Union(
                    from ac in this.ObjectContext.AnAction
                    from pia in this.ObjectContext.PortionIntegriteAnAction
                    from pis in this.ObjectContext.PiSecteurs
                    from s in this.ObjectContext.GeoSecteur
                    where ac.CleAction == pia.CleAction && pia.ClePortion == pis.ClePortion && pis.CleSecteur == s.CleSecteur && s.CleAgence == cleAgence
                    select ac
                );

            return query;
        }

        /// <summary>
        /// Récupère la liste des Actions d'une region
        /// </summary>
        /// <param name="cle">identifiant d'une region </param>
        /// <returns>la liste des actions d'une region </returns>
        public IQueryable<AnAction> GetActionsByRegion(int cleRegion)
        {
            IQueryable<AnAction> query = null;

            // actions par analyse
            query = this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>().Where
                    (
                        a => a.EnsembleElectrique.PortionIntegrite.Any(p => p.PiSecteurs.Any(s => s.GeoSecteur.GeoAgence.CleRegion == cleRegion))
                    ).SelectMany(a => a.AnAction);

            query = query.Union(this.ObjectContext.AnAnalyse.OfType<AnAnalyseSerieMesure>().Where
                (
                    a => a.Visite.ClePp.HasValue && a.Visite.Pp.GeoSecteur.GeoAgence.CleRegion == cleRegion
                ).SelectMany(a => a.AnAction));
            query = query.Union(this.ObjectContext.AnAnalyse.OfType<AnAnalyseSerieMesure>().Where
                (
                    a => a.Visite.CleEquipement.HasValue
                    && a.Visite.EqEquipement.Pp.GeoSecteur.GeoAgence.CleRegion == cleRegion
                ).SelectMany(a => a.AnAction));

            // actions hors analyse
            query = query.Union(
                    from ac in this.ObjectContext.AnAction
                    from pia in this.ObjectContext.PortionIntegriteAnAction
                    from pis in this.ObjectContext.PiSecteurs
                    from s in this.ObjectContext.GeoSecteur
                    from ag in this.ObjectContext.GeoAgence
                    where ac.CleAction == pia.CleAction && pia.ClePortion == pis.ClePortion && pis.CleSecteur == s.CleSecteur && s.CleAgence == ag.CleAgence && ag.CleRegion == cleRegion
                    select ac
                );

            return query;
        }

        public IQueryable<AnAction> FindAnActions()
        {
            return this.ObjectContext.AnAction;
        }

        public IQueryable<AnAction> FindAnActionByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur, int? cleEnsElec, int? cleEtatAnalyse,
            int? clePriorite, int? cleStatut, int? cleAgent, DateTime? dateMin, DateTime? dateMax, bool? includeDeletedAction)
        {

            ////Initialisation de la query sans resultats
            IQueryable<AnAction> query = this.ObjectContext.AnAction;

            ////Création de la query selon le niveau de recherche
            if (cleEnsElec.HasValue)
            {
                query = GetActionsByEnsembleElectrique(cleEnsElec.Value);
            }
            else if (cleSecteur.HasValue)
            {
                query = GetActionsBySecteur(cleSecteur.Value);
            }
            else if (cleAgence.HasValue)
            {
                query = GetActionsByAgence(cleAgence.Value);
            }
            else if (cleRegion.HasValue)
            {
                query = GetActionsByRegion(cleRegion.Value);
            }

            ////Completion de la query selon les options de la recherche
            if (cleEtatAnalyse.HasValue)
            {
                query = query.Where(a => a.AnAnalyse.EnumEtatPc.HasValue && a.AnAnalyse.EnumEtatPc.Value == cleEtatAnalyse.Value);
            }

            if (clePriorite.HasValue)
            {
                query = query.Where(a => a.CleParametreAction.HasValue && a.ParametreAction.EnumPriorite == clePriorite.Value);
            }

            if (cleStatut.HasValue)
            {
                query = query.Where(a => a.EnumStatut == cleStatut.Value);
            }

            if (cleAgent.HasValue)
            {
                query = query.Where(a => a.CleUtilisateurAgent == cleAgent.Value);
            }

            if (dateMin.HasValue)
            {
                dateMin = dateMin.Value.Date;
                query = query.Where(a => a.DateCreation >= dateMin || (a.DateModification.HasValue && a.DateModification.Value >= dateMin));
            }
            if (dateMax.HasValue)
            {
                dateMax = dateMax.Value.AddDays(1).Date;
                query = query.Where(a => a.DateCreation < dateMax || (a.DateModification.HasValue && a.DateModification.Value < dateMax));
            }
            if (includeDeletedAction.HasValue)
            {
                if (includeDeletedAction == true)
                {
                    query = query.Where(a => a.Supprime == includeDeletedAction.Value || a.Supprime == !includeDeletedAction.Value);
                }
                else
                {
                    query = query.Where(a => a.Supprime == includeDeletedAction.Value);
                }
            }

            return query;
        }

        /// <summary>
        /// Calcul du numéro action pc en respectant le format ACT-[CODE_REGION]-[CLE_ACTION]
        /// </summary>
        /// <param name="anAction"></param>
        /// <returns></returns>
        private void CalculerNumActionPc(AnAction anAction)
        {
            string codeRegion = null;
            if (String.IsNullOrEmpty(anAction.CodeRegion))
            {
                if (anAction.CleAnalyse != null)
                {
                    //Action avec analyse
                    codeRegion = (from a in this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>()
                                  from po in a.EnsembleElectrique.PortionIntegrite
                                  from pi in po.PiSecteurs
                                  where a.CleAnalyse == anAction.CleAnalyse && pi.GeoSecteur != null && pi.GeoSecteur.GeoAgence != null && pi.GeoSecteur.GeoAgence.GeoRegion != null
                                  select pi.GeoSecteur.GeoAgence.GeoRegion.LibelleAbregeRegion).FirstOrDefault();
                }
                else if (anAction.PortionIntegriteAnAction != null && anAction.PortionIntegriteAnAction.Any())
                {
                    int firstClePortionIntegriteAnAction = anAction.PortionIntegriteAnAction.First().ClePortion;
                    //Action hors analyse donc liée à au moins une portion
                    codeRegion = (from pi in this.ObjectContext.PiSecteurs
                                  where pi.ClePortion == firstClePortionIntegriteAnAction && pi.GeoSecteur != null && pi.GeoSecteur.GeoAgence != null && pi.GeoSecteur.GeoAgence.GeoRegion != null
                                  select pi.GeoSecteur.GeoAgence.GeoRegion.LibelleAbregeRegion).FirstOrDefault();
                }
            }
            if (!String.IsNullOrEmpty(codeRegion))
            {
                anAction.NumActionPc = string.Format("ACT-{0}-{1}", codeRegion, anAction.CleAction.ToString().PadLeft(10, '0'));
            }
        }

        #endregion AnAction

        #region AnAnalyseEe

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public IQueryable<AnAnalyseEe> GetAnAnalyseEeByCle(int cle)
        {
            return this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>()
                .Include("EnsembleElectrique")
                .Include("AnAction.ParametreAction.RefEnumValeur1")
                .Include("AnAnalyseEeVisite.Visite.Alertes")
                .Where(v => v.CleAnalyse == cle);
        }

        /// <summary>
        /// Retourne les portions intégrités recherchés
        /// </summary>
        /// <param name="cleRegion"></param>
        /// <param name="cleAgence"></param>
        /// <param name="cleSecteur"></param>
        /// <param name="cleEnsElec"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public IQueryable<AnAnalyseEe> FindAnAnalyseEeByCriterias(int? cleRegion, int? cleAgence, int? cleSecteur,
           string LibelleEe, bool includeStation, bool includePosteGaz)
        {
            IQueryable<AnAnalyseEe> query;
            if (cleSecteur.HasValue)
            {
                query = this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>().Include("EnsembleElectrique").Where
                    (
                        a => a.EnsembleElectrique.PortionIntegrite.Any(p => p.PiSecteurs.Any(s => s.CleSecteur == cleSecteur.Value))
                    );

            }
            else if (cleAgence.HasValue)
            {
                query = this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>().Include("EnsembleElectrique").Where
                    (
                        a => a.EnsembleElectrique.PortionIntegrite.Any(p => p.PiSecteurs.Any(s => s.GeoSecteur.CleAgence == cleAgence.Value))
                    );
            }
            else if (cleRegion.HasValue)
            {
                query = this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>().Include("EnsembleElectrique").Where
                    (
                        a => a.EnsembleElectrique.PortionIntegrite.Any(p => p.PiSecteurs.Any(s => s.GeoSecteur.GeoAgence.CleRegion == cleRegion.Value))
                    );
            }
            else
            {
                query = this.ObjectContext.AnAnalyse.OfType<AnAnalyseEe>().Include("EnsembleElectrique");
            }
            if (!String.IsNullOrEmpty(LibelleEe))
            {
                query = query.Where(a => a.EnsembleElectrique.Libelle.Contains(LibelleEe));
            }
            if (includeStation || includePosteGaz)
            {
                //Inclue les stations ou les poste Gaz ou non
                if (includeStation && includePosteGaz)
                {
                    query = query.Where(a => a.EnsembleElectrique.RefEnumValeur1 == null || a.EnsembleElectrique.RefEnumValeur1.Valeur == "1" || a.EnsembleElectrique.RefEnumValeur1.Valeur == "2");
                }
                else if (includeStation)
                {
                    query = query.Where(a => a.EnsembleElectrique.RefEnumValeur1 == null || a.EnsembleElectrique.RefEnumValeur1.Valeur == "1");
                }
                else if (includePosteGaz)
                {
                    query = query.Where(a => a.EnsembleElectrique.RefEnumValeur1 == null || a.EnsembleElectrique.RefEnumValeur1.Valeur == "2");
                }
            }
            else
            {
                query = query.Where(a => a.EnsembleElectrique.RefEnumValeur1 == null);
            }

            return query.OrderBy(a => a.EnsembleElectrique.Libelle).ThenBy(a => a.DateAnalyse);
        }

        #endregion

        #region LogTournee

        /// <summary>
        /// Retourne le logsélectionné
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        public LogTournee GetLogTourneeByCle(int cle)
        {
            return this.ObjectContext.LogTournee
                .Include("RefEnumValeur")
                .Include("UsrUtilisateur")
                .FirstOrDefault(e => e.CleTournee == cle);
        }

        /// <summary>
        /// Retourne la liste des log ouvrage d'un ouvrage en fonction de son type et de sa cle
        /// </summary>
        /// <param name="typeOuvrage">type d'ouvrage</param>
        /// <param name="cleOuvrage">cle de l'ouvrage</param>
        /// <returns>Liste des logTournee</returns>
        public IQueryable<LogTournee> GetLogTourneeByCleTournee(int cleTournee)
        {
            return this.ObjectContext.LogTournee
                .Include("RefEnumValeur")
                .Include("UsrUtilisateur")
                .Where(l => l.CleTournee == cleTournee);
        }

        #endregion

        #region InsInstruments

        /// <summary>
        /// Retourne la liste d'instrument correspondant à la clé du tableau de clé spécifié
        /// </summary>
        /// <param name="listCleEquipement">Liste de clé d'équipement</param>
        /// <returns>liste d'équipements</returns>
        [Query(HasSideEffects = true)]
        public IQueryable<InsInstrument> FindInstrumentsByListCle(ObservableCollection<int> listCle)
        {
            if (listCle == null)
            {
                listCle = new ObservableCollection<int>();
            }

            IQueryable<InsInstrument> retour = this.ObjectContext.InsInstrument.Where(a => listCle.Contains(a.CleInstrument));

            return retour;
        }

        #endregion

        public override bool Submit(ChangeSet changeSet)
        {
            bool valretour = base.Submit(changeSet);

            IEnumerable<ChangeSetEntry> entries = this.ChangeSet.ChangeSetEntries.Where(e => e.Operation == DomainOperation.Insert && e.Entity != null && e.Entity is EqLiaisonInterne);
            foreach (var entry in entries)
            {
                EqLiaisonInterne eqLI = entry.Entity as EqLiaisonInterne;
                if (eqLI.LiaisonInterEe && eqLI.EqLiaisonInterne2.CleLiaisonInterEe != eqLI.CleEquipement)
                {
                    EqLiaisonInterne eqLiInter = this.ObjectContext.EqEquipement.OfType<EqLiaisonInterne>()
                        .Where(c => c.CleEquipement == eqLI.EqLiaisonInterne2.CleEquipement).FirstOrDefault();
                    eqLiInter.CleLiaisonInterEe = eqLI.CleEquipement;
                    try
                    {
                        this.ObjectContext.SaveChanges();
                    }
                    catch
                    {
                        valretour = false;
                    }
                }
            }

            // MANTIS-18602 - Calcul du numéro action pc
            IEnumerable<ChangeSetEntry> actions = this.ChangeSet.ChangeSetEntries.Where(e => e.Operation == DomainOperation.Insert && e.Entity != null && e.Entity is AnAction);
            foreach (var entry in actions)
            {
                if (entry.Entity.GetType() == typeof(AnAction))
                {
                    var act = entry.Entity as AnAction;
                    if (act != null && String.IsNullOrEmpty(act.NumActionPc))
                    {
                        CalculerNumActionPc(act);
                        try
                        {
                            this.ObjectContext.SaveChanges();
                        }
                        catch
                        {
                            valretour = false;
                        }
                    }
                }
            }

            return valretour;
        }

        public void SaveDomainContext(string jsonString)
        {
            File.WriteAllText(string.Format(@"c:\{0}_store.json", DateTime.Now.ToString("yyyyMMddhhmmss")), jsonString);
        }

        [Query(HasSideEffects = true)]
        public List<VisiteImportRapport> ImportTelemesures(int cleUtilisateur, ObservableCollection<String> lines)
        {
            return importerLesLignes(cleUtilisateur, lines).OrderByDescending(c => c.IsOnError).ThenByDescending(c => c.IsOnWarning).ThenByDescending(e => e.IsOnSuccess).Select(vi => vi.VisiteRapport).ToList();
        }

        #region Gestion de l'import des fichiers de Télémesures.

        private List<String[]> rowsToRowTable(ObservableCollection<String> lines)
        {
            List<String[]> result = new List<String[]>();
            String[] row;
            String temp;

            foreach (String line in lines)
            {
                row = line.Split(';');
                temp = line.Remove((line.Count() - 1) - (row.Any() ? row.Last().Count() : 0));
                //Ajout de la ligne aux éléments de la RowTable pour l'interface
                Array.Resize(ref row, row.Length + 1);
                row[row.Length - 1] = temp;
                result.Add(row);
            }

            return result;
        }

        private List<VisiteImport> importerLesLignes(int cleUtilisateur, ObservableCollection<String> lines)
        {
            List<VisiteImport> VisitesImport = new List<VisiteImport>();
            List<String[]> rowTable = rowsToRowTable(lines);

            // Chargement des données
            UsrUtilisateur userTelemesure = GetUsrUtilisateurByIdentifiant("USR_TLM");
            UsrUtilisateur userImport = GetUsrUtilisateurByCle(cleUtilisateur);

            List<RefEnumValeur> refEnumValeurs = this.ObjectContext.RefEnumValeur.ToList();
            List<MesClassementMesure> mesClassementMesures = GetMesClassementMesureWithMesNiveauProtection().ToList();

            int index = 0;
            string nomFichier = String.Empty;

            // 1ère itération pour valider le type d'équipement
            foreach (String[] row in rowTable)
            {
                VisiteImport MonImport = new VisiteImport() { };
                VisitesImport.Add(MonImport);
                MonImport.NomFichier = row.ElementAt(row.Count() - 2);
                if (MonImport.NomFichier != nomFichier)
                {
                    nomFichier = MonImport.NomFichier;
                    index = 0;
                }
                MonImport.IndexOnFile = ++index;
                MonImport.Line = row.LastOrDefault();
                MonImport.Row = row;

                try
                {
                    MonImport.TypeEquipementOrigine = row[1];

                    // Champs VISITES
                    MonImport.CleEquipement = Int32.Parse(row[0]);
                    MonImport.CleUtilisateur = userTelemesure.CleUtilisateur;
                    MonImport.DateImport = DateTime.Now;

                    MonImport.IsTelemesure = true;
                    MonImport.EstValide = true;
                    MonImport.CommentaireVisite = row[40];
                    MonImport.TypeEquipement = refEnumValeurs.Where(c => c.CodeGroupe == "IMPORT_TELEMESURE" && c.Libelle == "TypeEquipement" && c.Valeur.ToUpper() == row[1].ToUpper())
                        .Select(c => c.LibelleCourt).FirstOrDefault();
                    var firstOrDefault = refEnumValeurs.FirstOrDefault(c => c.CodeGroupe == "TYPE_EVAL" && c.LibelleCourt == "EG");
                    if (firstOrDefault != null)
                    {
                        MonImport.EnumTypeEval = firstOrDefault.CleEnumValeur;
                    }
                    MonImport.EnumTypeEvalComposition = MonImport.EnumTypeEval;
                }
                catch (IndexOutOfRangeException)
                {
                    MonImport.AddOnError(string.Format("Le format de la ligne {0} est incorrect.", index));
                }
                catch (Exception ex)
                {
                    MonImport.AddOnError(string.Format("Format de la ligne {0} incorrect : {1}", index, ex.Message));
                }

                try
                {
                    MonImport.DateVisite = DateTime.Parse(row[3], new CultureInfo("fr-FR"));
                    MonImport.DateSaisie = MonImport.DateVisite;
                }
                catch
                {
                    MonImport.AddOnError("Date de la mesure invalide.");
                }

                if (!MonImport.IsOnError && MonImport.TypeEquipement == null)
                {
                    MonImport.AddOnError("Le type d'équipement ne correspond à aucun type connu");
                }
            }

            List<EqEquipement> eqEquipements = FindEqEquipementsByListCle(new ObservableCollection<int>(VisitesImport.Where(vi => vi.TypeEquipement != "PP").Select(vi => vi.CleEquipement).Distinct())).ToList();
            List<Pp> pps = FindPpsByListCle(new ObservableCollection<int>((VisitesImport.Where(vi => vi.TypeEquipement == "PP").Select(vi => vi.CleEquipement).Union(eqEquipements.Select(e => e.ClePp))).Distinct())).ToList();

            foreach (VisiteImport vi in VisitesImport)
            {
                vi.VisiteRapport.NumLigne = vi.IndexOnFile;
                vi.VisiteRapport.NomFichier = vi.NomFichier;
                vi.VisiteRapport.TypeEquipement = vi.TypeEquipement;
                vi.VisiteRapport.CleEquipement = vi.CleEquipement;
                vi.VisiteRapport.DateVisite = vi.DateVisite;
                vi.VisiteRapport.TextImport = vi.TextImport;

                //Contrôle de l'intégrité des équipements
                Pp pp = pps.FirstOrDefault(p => p.ClePp == vi.CleEquipement);
                EqEquipement eq = eqEquipements.FirstOrDefault(e => e.CleEquipement == vi.CleEquipement);
                if (vi.TypeEquipement == "PP" && pp != null)
                {
                    vi.VisiteRapport.LibelleEq = pp.Libelle;
                }
                else if (eq != null)
                {
                    vi.VisiteRapport.LibelleEq = eq.Libelle;

                    if (eq.TypeEquipement.CodeEquipement != vi.TypeEquipement)
                    {
                        vi.AddOnError("la clé d'équipement et le type ne correspondent pas");
                    }
                }
                else
                {
                    vi.AddOnError("La clé équipement ne correspond à aucun équipement connu");
                }
            }

            foreach (VisiteImport vi in VisitesImport)
            {
                AjouterLigne(vi, cleUtilisateur, refEnumValeurs, mesClassementMesures, pps, eqEquipements);
            }

            return VisitesImport;
        }

        private void AjouterLigne(VisiteImport monImport, int cleUtilisateur, List<RefEnumValeur> refEnumValeurs, List<MesClassementMesure> mesClassementMesures, List<Pp> pps, List<EqEquipement> equipements)
        {
            if (!monImport.IsOnError)
            {
                // Création de la visite
                Visite nouvelleVisite = TryCreateVisite(monImport, cleUtilisateur, pps, equipements);
                // Remplissage de la visite avec les propriétés disponibles dans le VisiteImport
                if (nouvelleVisite != null)
                {
                    bool VisitIsValid = false;
                    // et traitement des mesures
                    if (TryCreateMesures(monImport, nouvelleVisite, refEnumValeurs))
                    {
                        // Création des alertes
                        if (TryCreateAlertes(monImport, nouvelleVisite, refEnumValeurs))
                        {
                            // On ajoute la visite avec ses mesures dans le contexte
                            this.ObjectContext.Visites.AddObject(nouvelleVisite);
                            this.CreateVisitesForPpJumelees(nouvelleVisite);

                            try
                            {

                                this.ObjectContext.SaveChanges();
                                VisitIsValid = true;
                                monImport.AddOnSucess();
                            }
                            catch
                            {
                                monImport.AddOnError("Erreur lors de l'enregistrement, réessayez ultérieurement ou contactez votre administrateur");
                            }
                        }
                    }
                    if (!VisitIsValid)//la visite doit être retiré du context
                    {
                        if (monImport.TypeEquipement == "PP")
                        {
                            var newpp = pps.Single(pp => pp.ClePp == monImport.CleEquipement);
                            newpp.Visites.Remove(nouvelleVisite);
                            this.ObjectContext.SaveChanges();
                        }
                        else
                        {
                            var newpp = equipements.Single(eq => eq.CleEquipement == monImport.CleEquipement);
                            newpp.Visites.Remove(nouvelleVisite);
                            this.ObjectContext.SaveChanges();
                        }

                    }
                }
                //la visite n'a pas pu etre créée
            }
        }

        private Visite TryCreateVisite(VisiteImport monImport, int cleUtilisateur, List<Pp> pps, List<EqEquipement> eqs)
        {
            try
            {
                Visite maVisite = new Visite();

                if (monImport.TypeEquipement == "PP" && !monImport.IsPPModifed)
                {
                    Pp newpp = pps.FirstOrDefault<Pp>(pp => pp.ClePp == monImport.CleEquipement);
                    newpp.Visites.Add(maVisite);
                }
                else if (!monImport.IsEquipementTempo && monImport.TypeEquipement != "PP")
                {
                    EqEquipement neweq = eqs.FirstOrDefault<EqEquipement>(eq => eq.CleEquipement == monImport.CleEquipement);
                    neweq.Visites.Add(maVisite);
                }

                // champs obligatoire
                if (monImport.Utilisateur != null)
                {
                    maVisite.UsrUtilisateur2 = monImport.Utilisateur;
                }
                else
                {
                    maVisite.CleUtilisateurMesure = monImport.CleUtilisateur;
                }

                maVisite.EstValidee = monImport.EstValide;
                maVisite.Telemesure = monImport.IsTelemesure;

                // Dates
                maVisite.DateVisite = monImport.DateVisite;
                maVisite.DateImport = monImport.DateImport;
                maVisite.DateSaisie = monImport.DateSaisie;

                // Spec import
                maVisite.CleUtilisateurImport = cleUtilisateur;
                maVisite.CleUtilisateurCreation = monImport.CleUtilisateur;
                maVisite.Commentaire = monImport.CommentaireVisite;

                var firstOrDefault = this.ObjectContext.RefEnumValeur.FirstOrDefault(c => c.CodeGroupe == "TYPE_EVAL" && c.LibelleCourt == "EG");
                if (firstOrDefault != null)
                {
                    maVisite.EnumTypeEval = firstOrDefault.CleEnumValeur;
                }
                maVisite.EnumTypeEvalComposition = monImport.EnumTypeEvalComposition;

                return maVisite;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de l'ajout de la visite : {0}", ex.Message));
                return null;
            }
        }
        private bool TryCreateMesures(VisiteImport monImport, Visite maVisite, List<RefEnumValeur> refEnumValeurs)
        {
            try
            {
                if (monImport.TypeEquipement == "PP")
                {
                    // Vérification que la Pp visitée dispose d'un temoin enterré amovible
                    if (maVisite.ClePp.HasValue && maVisite.Pp != null && maVisite.Pp.PresenceDUneTelemesure)
                    {
                        if (!TryCreateListMesures(monImport, maVisite, refEnumValeurs))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        monImport.AddOnError("Erreur lors de la création des mesures : type de PP non pris en charge par la télémesure");
                        return false;
                    }
                }
                else
                {
                    if (maVisite.CleEquipement.HasValue && maVisite.EqEquipement != null)
                    {
                        var prop = maVisite.EqEquipement.GetType().GetProperty("PresenceTelemesure");
                        if (prop != null && (bool)prop.GetValue(maVisite.EqEquipement, null))
                        {
                            if (!TryCreateListMesures(monImport, maVisite, refEnumValeurs))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            monImport.AddOnError("Erreur lors de la création des mesures : équipement non télémesuré");
                            return false;
                        }
                    }
                }
                maVisite.RelevePartiel = true;
                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de la création des mesures : {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// MANTIS 10882 FSI 23/06/2014 : Import télémesure V3
        /// Recherche les Enums d'attribution de CleTypeMesure en base et attribution de mesures en fonction 
        /// </summary>
        /// <param name="monImport">ImportVisite en cours d'import</param>
        /// <param name="maVisite">Visite en cours de création</param>
        /// <param name="refEnumValeurs">RefEnumValeurs chargés depuis la base</param>
        /// <returns>Si une erreur apparaît, arrêt du traitement et renvoit false sinon true</returns>
        private static bool TryCreateListMesures(VisiteImport monImport, Visite maVisite, List<RefEnumValeur> refEnumValeurs)
        {
            foreach (RefEnumValeur typeMesure in refEnumValeurs.Where(c => c.CodeGroupe == "IMPORT_TELEMESURE_TYPEMESURE" && c.Libelle == monImport.TypeEquipement))
            {
                if (!String.IsNullOrEmpty(monImport.Row[typeMesure.NumeroOrdre - 1]))
                {
                    decimal valeur;
                    if (decimal.TryParse(monImport.Row[typeMesure.NumeroOrdre - 1], out valeur))
                    {
                        int cleTypeMesure;
                        if (!String.IsNullOrEmpty(typeMesure.Valeur)
                            && int.TryParse(typeMesure.Valeur, out cleTypeMesure))
                        {
                            maVisite.MesMesure.Add(new MesMesure()
                            {
                                Valeur = valeur,
                                CleTypeMesure = cleTypeMesure
                            });
                        }
                    }
                    else
                    {
                        monImport.AddOnError("Erreur lors de la création des mesures : valeur de la mesure invalide");
                        return false;
                    }
                }
            }
            return true;
        }

        private bool TryCreateAlertes(VisiteImport monImport, Visite maVisite, List<RefEnumValeur> refEnumValeurs)
        {
            try
            {
                // On sélectionne uniquement les mesures qui sont en dépassement de seuil
                foreach (MesMesure mamesure in maVisite.MesMesure.Where(a => a.IsDepassementSeuil))
                {
                    // création de l'alerte de type seuil
                    mamesure.Alertes.Add(new Alerte()
                    {
                        RefEnumValeur = refEnumValeurs.FirstOrDefault(c => c.CodeGroupe == "ENUM_TYPE_ALERTE" && c.Valeur == "S"),
                        Date = monImport.DateSaisie,
                        Supprime = false
                    });

                    maVisite.Alertes.Add(mamesure.Alertes.FirstOrDefault());
                }

                return true;
            }
            catch (Exception ex)
            {
                monImport.AddOnError(string.Format("Erreur lors de la création des alertes : {0}", ex.Message));
                return false;
            }
        }

        private void CreateVisitesForPpJumelees(Visite maVisite)
        {
            if (maVisite.ClePp.HasValue)
            {
                //Récupération des Pp qui sont jumelees à cette Pp
                List<int> MesClesPpJumelees = maVisite.ClePp.HasValue ? maVisite.Pp.PpJumelee.Select(pj => pj.PpClePp).Union(maVisite.Pp.PpJumelee1.Select(pj => pj.ClePp)).ToList()
                    : maVisite.PpTmp.Pp.PpJumelee.Select(pj => pj.PpClePp).Union(maVisite.PpTmp.Pp.PpJumelee1.Select(pj => pj.ClePp)).ToList();

                foreach (int clePpJumelee in MesClesPpJumelees)
                {
                    //Création de ma copie de Visite
                    Visite visiteCopy = this.ObjectContext.Visites.CreateObject();

                    //{
                    visiteCopy.ClePp = clePpJumelee;
                    visiteCopy.CleUtilisateurValidation = maVisite.CleUtilisateurValidation;
                    visiteCopy.CleUtilisateurCreation = maVisite.CleUtilisateurCreation;
                    visiteCopy.CleUtilisateurMesure = maVisite.CleUtilisateurMesure;
                    visiteCopy.DateValidation = maVisite.DateValidation;
                    visiteCopy.DateSaisie = maVisite.DateSaisie;
                    visiteCopy.DateVisite = maVisite.DateVisite;
                    visiteCopy.RelevePartiel = maVisite.RelevePartiel;
                    visiteCopy.EnumTypeEval = maVisite.EnumTypeEval;
                    visiteCopy.EnumTypeEvalComposition = maVisite.EnumTypeEvalComposition;
                    visiteCopy.Commentaire = maVisite.Commentaire;
                    visiteCopy.EstValidee = maVisite.EstValidee;
                    visiteCopy.EnumConformiteTournee = maVisite.EnumConformiteTournee;
                    //};

                    //Copie des Mesures
                    foreach (MesMesure mesureOrigin in maVisite.MesMesure)
                    {
                        //Création de ma copie de Mesure
                        MesMesure mesureCopy = new MesMesure()
                        {
                            Valeur = mesureOrigin.Valeur,
                            CleTypeMesure = mesureOrigin.CleTypeMesure
                        };

                        if (mesureOrigin.Alertes != null && mesureOrigin.Alertes.Any())
                        {
                            Alerte alerte = new Alerte()
                            {
                                Supprime = false,
                                Date = mesureOrigin.Alertes.FirstOrDefault().Date,
                                EnumTypeAlerte = mesureOrigin.Alertes.FirstOrDefault().EnumTypeAlerte
                            };

                            visiteCopy.Alertes.Add(alerte);
                            mesureCopy.Alertes.Add(alerte);
                        }

                        visiteCopy.MesMesure.Add(mesureCopy);
                    }

                    this.ObjectContext.Visites.AddObject(visiteCopy);
                }
            }
        }

        #endregion
    }
}