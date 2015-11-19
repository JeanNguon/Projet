using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;
using Jounce.Core.Application;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Web.Models;
using Proteca.Web.Services;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Enums.NavigationEnums;
using Offline;

namespace Proteca.Silverlight.Services.EntityServices
{
    /// <summary>
    /// A service class should be created for each RIA DomainContext in your project
    /// This class should expose Collections of Data.  Do not add properties, such as SelectedItems
    /// to your service.  Allow your ViewModel to handle these items.
    /// 
    /// Follow the TODO: items in the file to complete the implementation in your project
    /// 
    /// </summary>
    [Export(typeof(IEntityService<Composition>))]
    public class CompositionService : IEntityService<Composition>
    {
        #region Properties

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        private int? _filtreCleSecteurRecherche;
        public int? FiltreCleSecteurRecherche
        {
            get
            {
                return _filtreCleSecteurRecherche;
            }
            set
            {
                if (_filtreCleSecteurRecherche != value)
                {
                    _filtreCleSecteurRecherche = OriginEntities.Any(c => c.Ouvrage.PpAttachee.CleSecteur == value) ? value : null;
                    ReorderEntities();
                }
            }
        }

        private ObservableCollection<Composition> _entities;
        public ObservableCollection<Composition> Entities
        {
            get
            {
                if (this._entities == null)
                {
                    this._entities = new ObservableCollection<Composition>();
                }
                return this._entities;
            }
            set
            {
                this._entities = value;
            }
        }

        private ObservableCollection<Composition> _originEntities;
        public ObservableCollection<Composition> OriginEntities
        {
            get
            {
                if (this._originEntities == null)
                {
                    this._originEntities = new ObservableCollection<Composition>();
                }
                return this._originEntities;
            }
            set
            {
                this._originEntities = value;
            }
        }

        public ObservableCollection<SelectTourneeTableauBord_Result> TableauBordEntities { get; set; }
        public ObservableCollection<SelectPortionGraphique_Result> GraphiqueEntities { get; set; }

        public Composition DetailEntity { get; set; }
        #endregion

        #region Constructor
        public CompositionService()
        {
            this.GraphiqueEntities = new ObservableCollection<SelectPortionGraphique_Result>();
            this.TableauBordEntities = new ObservableCollection<SelectTourneeTableauBord_Result>();
            this.Entities = new ObservableCollection<Composition>();
        }
        #endregion

        #region Methods
        #region Standard Items in the Class - Do not change

        /// <summary>
        /// Adds a new Entity to the collection for submitting on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Composition entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            this.domainContext.Compositions.Add(entity);
            if (!this.OriginEntities.Contains(entity))
            {
                this.OriginEntities.Add(entity);
            }
            ReorderEntities();
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Composition entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (entity != null && entity.Ouvrage != null && entity.Ouvrage.LastVisite != null)
            {
                if (entity.Ouvrage is Pp)
                {
                    RejectChangesOnPp(entity.Pp);
                }
                RemoveVisite(entity.Ouvrage.LastVisite);

                SynchronizationService.RemoveTmpPPDuplications(domainContext);

                domainContext.SaveToIsoStore();
            }
        }

        /// <summary>
        /// Marks an Entity in the collection as deleted, which is submitted on the next save
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteEqTmp(EqEquipementTmp entity)
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            if (entity != null)
            {
                foreach (Composition c in entity.Compositions)
                {
                    if (domainContext.Compositions.Contains(c))
                    {
                        domainContext.Compositions.Remove(c);
                    }
                }
                if (domainContext.EqEquipementTmps.Contains(entity))
                {
                    domainContext.EqEquipementTmps.Remove(entity);
                }
                ReorderEntities();
            }
        }

        public void RejectChangesOnPp(Pp pp)
        {
            pp.RejectChanges();
            foreach (PpTmp ppTmp in pp.PpTmp.ToList())
            {
                domainContext.PpTmps.Remove(ppTmp);
            }
            pp.SavePropertiesInPpTmp();
        }

        /// <summary>
        /// Nettoyage du service remise a 0 de certaines proprietes du domainContext
        /// </summary>
        public void Clear()
        {
            ReorderEntities();
        }

        public void SaveChanges(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            // See if any products have changed
            if ((domainContext.Pps != null && domainContext.Pps.HasChanges)
               || (domainContext.Visites != null && domainContext.Visites.HasChanges)
               || (domainContext.InstrumentsUtilises != null && domainContext.InstrumentsUtilises.HasChanges)
               || (domainContext.MesMesures != null && domainContext.MesMesures.HasChanges)
               || (domainContext.EqEquipementTmps != null && domainContext.EqEquipementTmps.HasChanges)
               || (domainContext.Alertes != null && domainContext.Alertes.HasChanges)
               || (domainContext.AnAnalyses != null && domainContext.AnAnalyses.HasChanges))
            {
                bool isValid = true;

                SynchronizationService.RemoveTmpPPDuplications(domainContext);

                foreach (var obj in domainContext.Compositions.EntityContainer.GetChanges())
                {
                    obj.ValidationErrors.Clear();
                    Collection<ValidationResult> errors = new Collection<ValidationResult>();
                    bool isEntityValid = Validator.TryValidateObject(obj, new ValidationContext(obj, null, null), errors, true);
                    if (!isEntityValid)
                    {
                        foreach (var err in errors)
                        {
                            obj.ValidationErrors.Add(err);
                        }
                    }

                    //Cette partie permet de s'assurer qu'on ne valide que les éléments qui ne sont pas duplicated
                    Visite visiteValidating = obj is Visite ? obj as Visite : (obj is AnAnalyseSerieMesure ? (obj as AnAnalyseSerieMesure).Visite : (obj is MesMesure ? (obj as MesMesure).Visite : null));
                    bool isVisiteDuplicated = visiteValidating != null && visiteValidating.IsDuplicated;

                    isValid &= isEntityValid || isVisiteDuplicated;
                }

                if (isValid)
                {
                    //Suppression des visites en trop (les sauvegardes pour le cancel)
                    DeleteDuplicatedVisites();

                    //Commit des changements de la Pp dans les propriétés de la PpTmp
                    List<Pp> pps = this.OriginEntities.Where(c => c.Pp != null && c.Pp.LastVisite != null).Select(c => c.Pp).ToList();
                    foreach (Pp pp in pps)
                    {
                        pp.SavePropertiesInPpTmp();
                    }

                    domainContext.SaveToIsoStore();
                    // Invoke completion callback
                    completed(null);
                }
                else
                {
                    completed(new Exception("Des champs sont obligatoire"));
                }
            }
            else
            {
                completed(null);
            }
        }

        internal void DeleteDuplicatedVisites()
        {
            //Suppression des visites en trop (les sauvegardes pour le cancel)
            List<Visite> visitesToDelete = this.domainContext.Visites.Where(v => v.IsNew() && v.IsDuplicated).ToList();
            int counter = visitesToDelete.Count;
            for (int i = counter - 1; i > -1; i--)
            {
                RemoveVisite(visitesToDelete.ElementAt(i));
            }
            visitesToDelete.Clear();
        }


        /// <summary>
        /// Reverses all pending changes since the data was loaded
        /// </summary>
        public void RejectChanges()
        {
            Logger.Log(LogSeverity.Verbose, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            //Suppression des Visites non validées ou des doublon fait pour l'édition
            List<Visite> visites = this.domainContext.Visites.Where(v => v.IsNewInOfflineMode && !v.IsDuplicated).ToList();
            int counter = visites.Count;
            for (int i = counter - 1; i > -1; i--)
            {
                Visite vi = visites.ElementAt(i);
                visites.Remove(vi);
                RemoveVisite(vi);
            }

            //Annule les changements fait sur les Pps
            List<Pp> pps = this.OriginEntities.Where(c => c.Pp != null && c.Pp.PpTmp.Any()).Select(c => c.Pp).ToList();
            foreach (Pp pp in pps)
            {
                if (pp.LastVisite != null)
                {
                    pp.RevertPropertiesFromPpTmp();
                }
                else
                {
                    RejectChangesOnPp(pp);
                }
            }

            //this.domainContext.RestoreFromIsoStore();
            ReorderEntities();
        }

        /// <summary>
        /// Suppression d'une visite dans le domainContext proprement
        /// </summary>
        /// <param name="visite"></param>
        public void RemoveVisite(Visite visite)
        {
            foreach (Alerte ale in visite.Alertes)
            {
                this.domainContext.Alertes.Remove(ale);
            }

            foreach (MesMesure mes in visite.MesMesure)
            {
                this.domainContext.MesMesures.Remove(mes);
            }

            foreach (AnAnalyseSerieMesure ana in visite.AnAnalyseSerieMesure)
            {
                this.domainContext.AnAnalyses.Remove(ana);
            }

            foreach (InstrumentsUtilises ins in visite.InstrumentsUtilises)
            {
                this.domainContext.InstrumentsUtilises.Remove(ins);
            }

            this.domainContext.Visites.Remove(visite);

        }

        #endregion

        private void ReorderEntities()
        {
            OriginEntities = new ObservableCollection<Composition>(domainContext.Compositions.Where(c => !c.CleEnsElectrique.HasValue && !c.ClePortion.HasValue).OrderBy(c => c.CleEqTmp.HasValue).ThenBy(c => c.NumeroOrdre).ThenBy(c => c.Ouvrage.Libelle));

            Entities = new ObservableCollection<Composition>(OriginEntities.Where(c => !this.FiltreCleSecteurRecherche.HasValue || this.FiltreCleSecteurRecherche.Value == c.Ouvrage.PpAttachee.CleSecteur));

            foreach (Composition compo in Entities)
            {
                compo.EntityIndex = Entities.IndexOf(compo);
            }
        }

        /// <summary>
        /// Recupere une liste d'entite
        /// </summary>
        /// <param name="completed">callback fonction</param>
        public void GetEntities(Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            this.ReorderEntities();
            //Crée si besoin l'état de base des propriétés des Pps
            List<Pp> pps = this.OriginEntities.Where(c => c.Pp != null && !c.Pp.PpTmp.Any()).Select(c => c.Pp).ToList();
            foreach (Pp pp in pps)
            {
                pp.SavePropertiesInPpTmp();
            }
            completed(null);
        }

        /// <summary>
        /// Recupere une seule entite en fonction de sa cle
        /// </summary>
        /// <param name="cle">cle de l'entite</param>
        /// <param name="completed">callback fonction</param>
        public void GetEntityByCle(int cle, Action<Exception> completed)
        {

            // Declare error and result
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            this.DetailEntity = Entities.FirstOrDefault(c => c.EntityIndex == cle);
            // Invoke completion callback
            completed(null);

        }

        public void GetEntityByCleTournee(int? cleTournee, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            var query = domainContext.Compositions.Where(t => !t.CleEnsElectrique.HasValue && !t.ClePortion.HasValue && t.CleTournee == cleTournee).AsQueryable();

            // Declare error and result
            IEnumerable<Composition> tournees = null;


            tournees = query;
            Entities = new ObservableCollection<Composition>(tournees.ToList());


            // Invoke completion callback
            completed(null);

        }

        public void GetTourneeTableauBord(int cleTournee, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            this.TableauBordEntities.Clear();

            List<Composition> compos = domainContext.Compositions.Where(c => !c.CleEnsElectrique.HasValue && !c.ClePortion.HasValue && (c.CleTournee == cleTournee || c.IsNew())).ToList();

            int index = 0;
            foreach (Composition compo in compos)
            {
                this.TableauBordEntities.Add(new SelectTourneeTableauBord_Result()
                {
                    ID = index++,
                    CLE_TOURNEE = cleTournee,
                    Libelle_EE = compo.LibelleEe,
                    Libelle_PI = compo.LibellePortion,
                    LIBELLE_SECTEUR = compo.Ouvrage.PpAttachee.GeoSecteur.LibelleSecteur,
                    LIBELLE = compo.Ouvrage.Libelle,
                    CODE_EQUIPEMENT = compo.Ouvrage.CodeEquipement,
                    PK = compo.Ouvrage.PpAttachee.Pk,
                    DATE_VISITE = (compo.Ouvrage.LastVisite != null && !compo.Ouvrage.LastVisite.IsNewInOfflineMode) ? compo.Ouvrage.LastVisite.DateVisite : null,
                    DATE_SAISIE = (compo.Ouvrage.LastVisite != null && !compo.Ouvrage.LastVisite.IsNewInOfflineMode) ? compo.Ouvrage.LastVisite.DateSaisie : null,
                    DATE_IMPORT = (compo.Ouvrage.LastVisite != null && !compo.Ouvrage.LastVisite.IsNewInOfflineMode) ? compo.Ouvrage.LastVisite.DateImport : null,
                    DATE_VALIDATION = (compo.Ouvrage.LastVisite != null && !compo.Ouvrage.LastVisite.IsNewInOfflineMode) ? compo.Ouvrage.LastVisite.DateValidation : null
                });
            }

            completed(null);
        }

        public void GetPortionGraphique(int clePortion, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

            this.GraphiqueEntities.Clear();

            List<MesMesure> mesures = domainContext.MesMesures.Where(m => (m.Visite.ClePp.HasValue || m.Visite.ClePpTmp.HasValue) && m.Valeur.HasValue && m.Visite.PpReliee.ClePortion == clePortion && m.Visite.IsNew() && m.Visite.EstValidee && m.Visite == m.Visite.Ouvrage.LastVisite && m.MesTypeMesure.MesModeleMesure.EnumTypeGraphique.HasValue).ToList();

            int index = 0;
            foreach (MesMesure mesure in mesures)
            {
                this.GraphiqueEntities.Add(new SelectPortionGraphique_Result()
                {
                    ID = index++,
                    CLE_GRAPHIQUE = mesure.MesTypeMesure.MesModeleMesure.EnumTypeGraphique.Value,
                    CLE_PORTION = clePortion,
                    NIVEAU = mesure.MesTypeMesure.RefEnumValeur.Libelle,
                    PK = mesure.Visite.Pp.Pk,
                    VALEUR = mesure.Valeur.Value,
                    DATE_VISITE = mesure.Visite.DateVisite
                });
            }

            completed(null);
        }

        /// <summary>
        /// Recupere la liste des entites en fonction du filtre defini
        /// </summary>
        /// <param name="filtres">liste des filtres</param>
        /// <param name="completed">callback fonction</param>
        public void FindEntities(List<Expression<Func<Composition, bool>>> filtres, Action<Exception> completed)
        {
            Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);
            completed(null);

        }

        #region Load Data Examples
        ///// <summary>
        ///// Complex Load Example - creates a series of queries,
        ///// Loads all the data, and calls the callback Action
        ///// once all loads are complete, this is particularly helpful
        ///// when loading lots of data that will be used in static resultsets.
        ///// </summary>
        ///// <param name="jobID"></param>
        ///// <param name="callback">Action to perform after load</param>
        //public void ComplexLoadExample(Guid jobID, Action callback)
        //{
        //    if (domainContext == null)
        //    {
        //        callback();
        //        return;
        //    }

        //    // Batch these Queries together
        //    contextManager.StartBatch(callback);    // The callback occurs with the batch

        //    // No Callback Needed for LoadData operations, they are completed with Batch
        //    contextManager.LoadData(domainContext.GetJobsQuery(jobID));

        //    // Example of using a Process to Selectively delete items upon load
        //    // This is used to remove items which may have been deleted by another user
        //    // or within another Domain Context.  The DeleteFilter does not have to be
        //    // used.  If not used, everything in cache, not returned from the Query is deleted
        //    contextManager.LoadData(domainContext.GetResourcesQuery(),
        //    ProcessDeletes: true,
        //    DeleteFilter: (delArgs) =>
        //    {
        //        Resource r = delArgs.Item as Resource;
        //        return (r.JobID == jobID);
        //    });

        //    contextManager.EndBatch();              // Signifies the End of the Batching Process
        //}
        #endregion
        #endregion
    }
}
