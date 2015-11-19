using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Framework.Command;
using Proteca.Silverlight.Enums;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Models;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using Telerik.Windows.Controls;
using Proteca.Silverlight.Services.EntityServices;
using System.Collections;
using Proteca.Web.Resources;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel de base pour gérer les entités ouvrage (equipement, ensemble electrique, portion intégrité, pp)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseOuvrageViewModel<T> : BaseProtecaEntityViewModel<T> where T : Entity
    {
        #region Constructor

        public BaseOuvrageViewModel()
            : base()
        {
            this.OnViewActivated += (o, e) =>
            {
                this.ListLogOuvrages = null;
            };

            this.OnDetailLoaded += (o, e) =>
            {
                ListLogOuvrages = null;
            };

            this.OnCanceled += (o, e) =>
            {
                ObjetToDuplicate = null;
            };

            this.OnSaveSuccess += (o, e) =>
            {
                ObjetToDuplicate = null;
                ListLogOuvrages = null;
            };
            
        }

        #endregion
        
        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type RefEnumValeur
        /// </summary>
        [Import]
        public IEntityService<RefEnumValeur> ServiceEnumValeur { get; set; }

        /// <summary>
        /// Service utilisé pour gérer l'entité de LogOuvrage
        /// </summary>
        [Import]
        public IEntityService<LogOuvrage> serviceLogOuvrage { get; set; }

        #endregion
        
        #region Properties

        /// <summary>
        /// Propriété privée permettant de géré l'entité avant modif
        /// </summary>
        private Entity _objetToDuplicate;

        /// <summary>
        /// Stocke l'entity avant modification
        /// </summary>
        public Entity ObjetToDuplicate
        {
            get
            {
                return _objetToDuplicate;
            }
            set
            {
                _objetToDuplicate = value;
            }
        }

        /// <summary>
        /// Liste des historiques de l'ouvrage sélectionné
        /// </summary>
        private List<LogOuvrage> _listLogOuvrages;
        public List<LogOuvrage> ListLogOuvrages
        {
            get { return _listLogOuvrages; }
            set
            {
                _listLogOuvrages = value;
                RaisePropertyChanged(() => this.ListLogOuvrages);
            }
        }

        #endregion
        
        #region Override Methods

        #endregion

        #region Historisation

        public void LogOuvrage(string operation)
        {
            this.LogOuvrage(operation, this.SelectedEntity);
        }

        /// <summary>
        /// Ajout d'un enregistrement dans logOuvrage
        /// </summary>
        public void LogOuvrage(string Operation, Entity MyEntity, string addToLog = null)
        {
            // Instanciation des propriétés
            EqEquipement currenteq = null;
            PortionIntegrite currentPortion = null;
            EnsembleElectrique currentEnsElect = null;
            Pp currentPp = null;
            EntityCollection<LogOuvrage> LogOuvrageList = null;
            LogOuvrage _logAajouter;

            // Instanciation du resource manager
            ResourceManager resourceManager = ResourceHisto.ResourceManager;

            // Détermination du type d'équipement
            if (MyEntity is EqEquipement)
            {
                currenteq = MyEntity as EqEquipement;
                LogOuvrageList = currenteq.LogOuvrage;
            }
            else if (MyEntity is PortionIntegrite)
            {
                currentPortion = MyEntity as PortionIntegrite;
                LogOuvrageList = currentPortion.LogOuvrage;
            }
            else if (MyEntity is EnsembleElectrique)
            {
                currentEnsElect = MyEntity as EnsembleElectrique;
                LogOuvrageList = currentEnsElect.LogOuvrage;
            }
            else if (MyEntity is Pp)
            {
                currentPp = MyEntity as Pp;
                LogOuvrageList = currentPp.LogOuvrage;
            }

            // Suppression des logs existant
            if (LogOuvrageList != null && LogOuvrageList.Any(lo => lo.IsNew()))
            {
                foreach (LogOuvrage log in LogOuvrageList.Where(lo => lo.IsNew()))
                {
                    LogOuvrageList.Remove(log);
                    serviceLogOuvrage.Delete(log);
                }
                _logAajouter = null;
            }

            RefEnumValeur typeLogOuvrage = ServiceEnumValeur.Entities.Where(r => r.CodeGroupe == RefEnumValeurCodeGroupeEnum.TYPE_LOG_OUVRAGE.GetStringValue() && r.Valeur == Operation).FirstOrDefault();
            
            if(typeLogOuvrage != null)
            {
                // Instanciation du log ouvrage
                _logAajouter = new LogOuvrage
                {
                    CleUtilisateur = this.CurrentUser.CleUtilisateur,
                    EnumTypeModification = typeLogOuvrage.CleEnumValeur,
                    DateHistorisation = DateTime.Now
                };

                // En cas de changement du sélected entity, on log l'enregistrement
                if (MyEntity.HasChanges || MyEntity.HasChildChanges() || Operation != null)
                {
                    string Modifiedproperties = null;

                    if ((MyEntity.HasChanges || MyEntity.HasChildChanges()) && !IsNewMode && Operation != "S")
                    {
                        Entity original = MyEntity.GetOriginal();
                        if (original == null)
                        {
                            original = MyEntity;
                        }

                        foreach (PropertyInfo p in MyEntity.GetType().GetProperties())
                        {
                            // Gestion des propriétés Nullable définies coté Silverlight
                            if (p.Name.EndsWith("Nullable"))
                            {
                                continue;
                            }

                            //récupération de la valeur à afficher. Si pas de valeurs on prend le nom de la propriété
                            string propertyName = resourceManager.GetString(p.Name) == null ? p.Name : resourceManager.GetString(p.Name);

                            if (String.IsNullOrEmpty(propertyName))
                            {
                                continue;
                            }

                            if (p.CanWrite && !(p.PropertyType.BaseType == typeof(Entity)))
                            {
                                Object originalValue = p.GetValue(original, null);
                                Object newValue = p.GetValue(MyEntity, null);
                                if ((originalValue == null && newValue == null) || (originalValue != null && originalValue.Equals(newValue)))
                                {
                                    continue;
                                }
                                else
                                {
                                    Modifiedproperties += Modifiedproperties == null ? propertyName : " / " + propertyName;
                                }
                            }
                            //else if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>))
                            //{
                            //    IEnumerable childEntities = p.GetValue(original, null) as IEnumerable;
                            //    IEnumerable newValue = p.GetValue(MyEntity, null) as IEnumerable;

                            //    if (p.Name != "LogOuvrage" && p.Name != "PPJumelee1" && childEntities != null && newValue != null)
                            //    {
                            //        foreach (var childEntity in newValue) // on regarde si il y a de nouveaux éléments ou des éléments modifiés
                            //        {
                            //            if (childEntity.GetType().BaseType == typeof(Entity) &&
                            //                ((((Entity)childEntity).EntityState == EntityState.New) || ((Entity)childEntity).EntityState == EntityState.Modified))
                            //            {
                            //                elements.Add(p.Name);
                            //                Modifiedproperties += Modifiedproperties == null ? propertyName : " / " + propertyName;
                            //                break;
                            //            }
                            //        }

                            //        if (!elements.Contains(p.Name))
                            //        {
                            //            foreach (var childEntity in childEntities) // on regarde si il y a des éléments supprimés
                            //            {
                            //                if (childEntity.GetType().BaseType == typeof(Entity) && ((Entity)childEntity).EntityState == EntityState.Deleted)
                            //                {
                            //                    Modifiedproperties += Modifiedproperties == null ? propertyName : " / " + propertyName;
                            //                    break;
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                        }

                        foreach (String propName in MyEntity.GetChildChangesPropertyNames())
                        {
                            //récupération de la valeur à afficher. Si pas de valeurs on prend le nom de la propriété
                            string childPropertyName = resourceManager.GetString(propName) == null ? propName : resourceManager.GetString(propName);
                            Modifiedproperties += Modifiedproperties == null ? childPropertyName : " / " + childPropertyName;
                        }
                    }

                    if (addToLog != null && !String.IsNullOrEmpty(addToLog) && (Modifiedproperties == null || !Modifiedproperties.Contains(addToLog)))
                    {
                        Modifiedproperties += Modifiedproperties == null ? addToLog : " / " + addToLog;
                    }

                    _logAajouter.ListeChamps = Modifiedproperties;

                    // On ajoute le log au contexte
                    LogOuvrageList.Add(_logAajouter);
                }
            }
        }

        #endregion
    }
}
