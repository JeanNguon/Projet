
namespace Proteca.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using Proteca.Web.Models;


    // Implements application logic using the ProtecaEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public partial class ProtecaDomainService : LinqToEntitiesDomainService<ProtecaEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Alertes' query.
        public IQueryable<Alerte> GetAlertes()
        {
            return this.ObjectContext.Alertes;
        }

        public void InsertAlerte(Alerte alerte)
        {
            if ((alerte.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(alerte, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Alertes.AddObject(alerte);
            }
        }

        public void UpdateAlerte(Alerte currentAlerte)
        {
            this.ObjectContext.Alertes.AttachAsModified(currentAlerte, this.ChangeSet.GetOriginal(currentAlerte));
        }

        public void DeleteAlerte(Alerte alerte)
        {
            if ((alerte.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(alerte, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Alertes.Attach(alerte);
                this.ObjectContext.Alertes.DeleteObject(alerte);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'AlerteDetail' query.
        public IQueryable<AlerteDetail> GetAlerteDetail()
        {
            return this.ObjectContext.AlerteDetail;
        }

        public void InsertAlerteDetail(AlerteDetail alerteDetail)
        {
            if ((alerteDetail.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(alerteDetail, EntityState.Added);
            }
            else
            {
                this.ObjectContext.AlerteDetail.AddObject(alerteDetail);
            }
        }

        public void UpdateAlerteDetail(AlerteDetail currentAlerteDetail)
        {
            this.ObjectContext.AlerteDetail.AttachAsModified(currentAlerteDetail, this.ChangeSet.GetOriginal(currentAlerteDetail));
        }

        public void DeleteAlerteDetail(AlerteDetail alerteDetail)
        {
            if ((alerteDetail.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(alerteDetail, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.AlerteDetail.Attach(alerteDetail);
                this.ObjectContext.AlerteDetail.DeleteObject(alerteDetail);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'AnAction' query.
        public IQueryable<AnAction> GetAnAction()
        {
            return this.ObjectContext.AnAction;
        }

        public void InsertAnAction(AnAction anAction)
        {
            if ((anAction.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(anAction, EntityState.Added);
            }
            else
            {
                this.ObjectContext.AnAction.AddObject(anAction);
            }
        }

        public void UpdateAnAction(AnAction currentAnAction)
        {
            this.ObjectContext.AnAction.AttachAsModified(currentAnAction, this.ChangeSet.GetOriginal(currentAnAction));
        }

        public void DeleteAnAction(AnAction anAction)
        {
            if ((anAction.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(anAction, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.AnAction.Attach(anAction);
                this.ObjectContext.AnAction.DeleteObject(anAction);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'AnAnalyse' query.
        public IQueryable<AnAnalyse> GetAnAnalyse()
        {
            return this.ObjectContext.AnAnalyse;
        }

        public void InsertAnAnalyse(AnAnalyse anAnalyse)
        {
            if ((anAnalyse.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(anAnalyse, EntityState.Added);
            }
            else
            {
                this.ObjectContext.AnAnalyse.AddObject(anAnalyse);
            }
        }

        public void UpdateAnAnalyse(AnAnalyse currentAnAnalyse)
        {
            this.ObjectContext.AnAnalyse.AttachAsModified(currentAnAnalyse, this.ChangeSet.GetOriginal(currentAnAnalyse));
        }

        public void DeleteAnAnalyse(AnAnalyse anAnalyse)
        {
            if ((anAnalyse.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(anAnalyse, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.AnAnalyse.Attach(anAnalyse);
                this.ObjectContext.AnAnalyse.DeleteObject(anAnalyse);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'AnAnalyseEeVisite' query.
        public IQueryable<AnAnalyseEeVisite> GetAnAnalyseEeVisite()
        {
            return this.ObjectContext.AnAnalyseEeVisite;
        }

        public void InsertAnAnalyseEeVisite(AnAnalyseEeVisite anAnalyseEeVisite)
        {
            if ((anAnalyseEeVisite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(anAnalyseEeVisite, EntityState.Added);
            }
            else
            {
                this.ObjectContext.AnAnalyseEeVisite.AddObject(anAnalyseEeVisite);
            }
        }

        public void UpdateAnAnalyseEeVisite(AnAnalyseEeVisite currentAnAnalyseEeVisite)
        {
            this.ObjectContext.AnAnalyseEeVisite.AttachAsModified(currentAnAnalyseEeVisite, this.ChangeSet.GetOriginal(currentAnAnalyseEeVisite));
        }

        public void DeleteAnAnalyseEeVisite(AnAnalyseEeVisite anAnalyseEeVisite)
        {
            if ((anAnalyseEeVisite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(anAnalyseEeVisite, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.AnAnalyseEeVisite.Attach(anAnalyseEeVisite);
                this.ObjectContext.AnAnalyseEeVisite.DeleteObject(anAnalyseEeVisite);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'CategoriePp' query.
        public IQueryable<CategoriePp> GetCategoriePp()
        {
            return this.ObjectContext.CategoriePp;
        }

        public void InsertCategoriePp(CategoriePp categoriePp)
        {
            if ((categoriePp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(categoriePp, EntityState.Added);
            }
            else
            {
                this.ObjectContext.CategoriePp.AddObject(categoriePp);
            }
        }

        public void UpdateCategoriePp(CategoriePp currentCategoriePp)
        {
            this.ObjectContext.CategoriePp.AttachAsModified(currentCategoriePp, this.ChangeSet.GetOriginal(currentCategoriePp));
        }

        public void DeleteCategoriePp(CategoriePp categoriePp)
        {
            if ((categoriePp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(categoriePp, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.CategoriePp.Attach(categoriePp);
                this.ObjectContext.CategoriePp.DeleteObject(categoriePp);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Compositions' query.
        public IQueryable<Composition> GetCompositions()
        {
            return this.ObjectContext.Compositions;
        }

        public void InsertComposition(Composition composition)
        {
            if ((composition.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(composition, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Compositions.AddObject(composition);
            }
        }

        public void UpdateComposition(Composition currentComposition)
        {
            this.ObjectContext.Compositions.AttachAsModified(currentComposition, this.ChangeSet.GetOriginal(currentComposition));
        }

        public void DeleteComposition(Composition composition)
        {
            if ((composition.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(composition, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Compositions.Attach(composition);
                this.ObjectContext.Compositions.DeleteObject(composition);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'EnsembleElectrique' query.
        public IQueryable<EnsembleElectrique> GetEnsembleElectrique()
        {
            return this.ObjectContext.EnsembleElectrique;
        }

        public void InsertEnsembleElectrique(EnsembleElectrique ensembleElectrique)
        {
            if ((ensembleElectrique.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ensembleElectrique, EntityState.Added);
            }
            else
            {
                this.ObjectContext.EnsembleElectrique.AddObject(ensembleElectrique);
            }
        }

        public void UpdateEnsembleElectrique(EnsembleElectrique currentEnsembleElectrique)
        {
            this.ObjectContext.EnsembleElectrique.AttachAsModified(currentEnsembleElectrique, this.ChangeSet.GetOriginal(currentEnsembleElectrique));
        }

        public void DeleteEnsembleElectrique(EnsembleElectrique ensembleElectrique)
        {
            if ((ensembleElectrique.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ensembleElectrique, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.EnsembleElectrique.Attach(ensembleElectrique);
                this.ObjectContext.EnsembleElectrique.DeleteObject(ensembleElectrique);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'EqDrainageLiaisonsext' query.
        public IQueryable<EqDrainageLiaisonsext> GetEqDrainageLiaisonsext()
        {
            return this.ObjectContext.EqDrainageLiaisonsext;
        }

        public void InsertEqDrainageLiaisonsext(EqDrainageLiaisonsext eqDrainageLiaisonsext)
        {
            if ((eqDrainageLiaisonsext.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(eqDrainageLiaisonsext, EntityState.Added);
            }
            else
            {
                this.ObjectContext.EqDrainageLiaisonsext.AddObject(eqDrainageLiaisonsext);
            }
        }

        public void UpdateEqDrainageLiaisonsext(EqDrainageLiaisonsext currentEqDrainageLiaisonsext)
        {
            this.ObjectContext.EqDrainageLiaisonsext.AttachAsModified(currentEqDrainageLiaisonsext, this.ChangeSet.GetOriginal(currentEqDrainageLiaisonsext));
        }

        public void DeleteEqDrainageLiaisonsext(EqDrainageLiaisonsext eqDrainageLiaisonsext)
        {
            if ((eqDrainageLiaisonsext.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(eqDrainageLiaisonsext, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.EqDrainageLiaisonsext.Attach(eqDrainageLiaisonsext);
                this.ObjectContext.EqDrainageLiaisonsext.DeleteObject(eqDrainageLiaisonsext);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'EqEquipement' query.
        public IQueryable<EqEquipement> GetEqEquipement()
        {
            return this.ObjectContext.EqEquipement;
        }

        public void InsertEqEquipement(EqEquipement eqEquipement)
        {
            if ((eqEquipement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(eqEquipement, EntityState.Added);
            }
            else
            {
                this.ObjectContext.EqEquipement.AddObject(eqEquipement);
            }
        }

        public void UpdateEqEquipement(EqEquipement currentEqEquipement)
        {
            this.ObjectContext.EqEquipement.AttachAsModified(currentEqEquipement, this.ChangeSet.GetOriginal(currentEqEquipement));
        }

        public void DeleteEqEquipement(EqEquipement eqEquipement)
        {
            if ((eqEquipement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(eqEquipement, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.EqEquipement.Attach(eqEquipement);
                this.ObjectContext.EqEquipement.DeleteObject(eqEquipement);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'EqEquipementTmp' query.
        public IQueryable<EqEquipementTmp> GetEqEquipementTmp()
        {
            return this.ObjectContext.EqEquipementTmp;
        }

        public void InsertEqEquipementTmp(EqEquipementTmp eqEquipementTmp)
        {
            if ((eqEquipementTmp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(eqEquipementTmp, EntityState.Added);
            }
            else
            {
                this.ObjectContext.EqEquipementTmp.AddObject(eqEquipementTmp);
            }
        }

        public void UpdateEqEquipementTmp(EqEquipementTmp currentEqEquipementTmp)
        {
            this.ObjectContext.EqEquipementTmp.AttachAsModified(currentEqEquipementTmp, this.ChangeSet.GetOriginal(currentEqEquipementTmp));
        }

        public void DeleteEqEquipementTmp(EqEquipementTmp eqEquipementTmp)
        {
            if ((eqEquipementTmp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(eqEquipementTmp, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.EqEquipementTmp.Attach(eqEquipementTmp);
                this.ObjectContext.EqEquipementTmp.DeleteObject(eqEquipementTmp);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'EqSoutirageLiaisonsext' query.
        public IQueryable<EqSoutirageLiaisonsext> GetEqSoutirageLiaisonsext()
        {
            return this.ObjectContext.EqSoutirageLiaisonsext;
        }

        public void InsertEqSoutirageLiaisonsext(EqSoutirageLiaisonsext eqSoutirageLiaisonsext)
        {
            if ((eqSoutirageLiaisonsext.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(eqSoutirageLiaisonsext, EntityState.Added);
            }
            else
            {
                this.ObjectContext.EqSoutirageLiaisonsext.AddObject(eqSoutirageLiaisonsext);
            }
        }

        public void UpdateEqSoutirageLiaisonsext(EqSoutirageLiaisonsext currentEqSoutirageLiaisonsext)
        {
            this.ObjectContext.EqSoutirageLiaisonsext.AttachAsModified(currentEqSoutirageLiaisonsext, this.ChangeSet.GetOriginal(currentEqSoutirageLiaisonsext));
        }

        public void DeleteEqSoutirageLiaisonsext(EqSoutirageLiaisonsext eqSoutirageLiaisonsext)
        {
            if ((eqSoutirageLiaisonsext.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(eqSoutirageLiaisonsext, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.EqSoutirageLiaisonsext.Attach(eqSoutirageLiaisonsext);
                this.ObjectContext.EqSoutirageLiaisonsext.DeleteObject(eqSoutirageLiaisonsext);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'GeoAgence' query.
        public IQueryable<GeoAgence> GetGeoAgence()
        {
            return this.ObjectContext.GeoAgence;
        }

        public void InsertGeoAgence(GeoAgence geoAgence)
        {
            if ((geoAgence.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoAgence, EntityState.Added);
            }
            else
            {
                this.ObjectContext.GeoAgence.AddObject(geoAgence);
            }
        }

        public void UpdateGeoAgence(GeoAgence currentGeoAgence)
        {
            this.ObjectContext.GeoAgence.AttachAsModified(currentGeoAgence, this.ChangeSet.GetOriginal(currentGeoAgence));
        }

        public void DeleteGeoAgence(GeoAgence geoAgence)
        {
            if ((geoAgence.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoAgence, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.GeoAgence.Attach(geoAgence);
                this.ObjectContext.GeoAgence.DeleteObject(geoAgence);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'GeoEnsElecPortion' query.
        public IQueryable<GeoEnsElecPortion> GetGeoEnsElecPortion()
        {
            return this.ObjectContext.GeoEnsElecPortion;
        }

        public void InsertGeoEnsElecPortion(GeoEnsElecPortion geoEnsElecPortion)
        {
            if ((geoEnsElecPortion.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoEnsElecPortion, EntityState.Added);
            }
            else
            {
                this.ObjectContext.GeoEnsElecPortion.AddObject(geoEnsElecPortion);
            }
        }

        public void UpdateGeoEnsElecPortion(GeoEnsElecPortion currentGeoEnsElecPortion)
        {
            this.ObjectContext.GeoEnsElecPortion.AttachAsModified(currentGeoEnsElecPortion, this.ChangeSet.GetOriginal(currentGeoEnsElecPortion));
        }

        public void DeleteGeoEnsElecPortion(GeoEnsElecPortion geoEnsElecPortion)
        {
            if ((geoEnsElecPortion.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoEnsElecPortion, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.GeoEnsElecPortion.Attach(geoEnsElecPortion);
                this.ObjectContext.GeoEnsElecPortion.DeleteObject(geoEnsElecPortion);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'GeoEnsElecPortionEqPp' query.
        public IQueryable<GeoEnsElecPortionEqPp> GetGeoEnsElecPortionEqPp()
        {
            return this.ObjectContext.GeoEnsElecPortionEqPp;
        }

        public void InsertGeoEnsElecPortionEqPp(GeoEnsElecPortionEqPp geoEnsElecPortionEqPp)
        {
            if ((geoEnsElecPortionEqPp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoEnsElecPortionEqPp, EntityState.Added);
            }
            else
            {
                this.ObjectContext.GeoEnsElecPortionEqPp.AddObject(geoEnsElecPortionEqPp);
            }
        }

        public void UpdateGeoEnsElecPortionEqPp(GeoEnsElecPortionEqPp currentGeoEnsElecPortionEqPp)
        {
            this.ObjectContext.GeoEnsElecPortionEqPp.AttachAsModified(currentGeoEnsElecPortionEqPp, this.ChangeSet.GetOriginal(currentGeoEnsElecPortionEqPp));
        }

        public void DeleteGeoEnsElecPortionEqPp(GeoEnsElecPortionEqPp geoEnsElecPortionEqPp)
        {
            if ((geoEnsElecPortionEqPp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoEnsElecPortionEqPp, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.GeoEnsElecPortionEqPp.Attach(geoEnsElecPortionEqPp);
                this.ObjectContext.GeoEnsElecPortionEqPp.DeleteObject(geoEnsElecPortionEqPp);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'GeoEnsembleElectrique' query.
        public IQueryable<GeoEnsembleElectrique> GetGeoEnsembleElectrique()
        {
            return this.ObjectContext.GeoEnsembleElectrique;
        }

        public void InsertGeoEnsembleElectrique(GeoEnsembleElectrique geoEnsembleElectrique)
        {
            if ((geoEnsembleElectrique.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoEnsembleElectrique, EntityState.Added);
            }
            else
            {
                this.ObjectContext.GeoEnsembleElectrique.AddObject(geoEnsembleElectrique);
            }
        }

        public void UpdateGeoEnsembleElectrique(GeoEnsembleElectrique currentGeoEnsembleElectrique)
        {
            this.ObjectContext.GeoEnsembleElectrique.AttachAsModified(currentGeoEnsembleElectrique, this.ChangeSet.GetOriginal(currentGeoEnsembleElectrique));
        }

        public void DeleteGeoEnsembleElectrique(GeoEnsembleElectrique geoEnsembleElectrique)
        {
            if ((geoEnsembleElectrique.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoEnsembleElectrique, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.GeoEnsembleElectrique.Attach(geoEnsembleElectrique);
                this.ObjectContext.GeoEnsembleElectrique.DeleteObject(geoEnsembleElectrique);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'GeoRegion' query.
        public IQueryable<GeoRegion> GetGeoRegion()
        {
            return this.ObjectContext.GeoRegion;
        }

        public void InsertGeoRegion(GeoRegion geoRegion)
        {
            if ((geoRegion.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoRegion, EntityState.Added);
            }
            else
            {
                this.ObjectContext.GeoRegion.AddObject(geoRegion);
            }
        }

        public void UpdateGeoRegion(GeoRegion currentGeoRegion)
        {
            this.ObjectContext.GeoRegion.AttachAsModified(currentGeoRegion, this.ChangeSet.GetOriginal(currentGeoRegion));
        }

        public void DeleteGeoRegion(GeoRegion geoRegion)
        {
            if ((geoRegion.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoRegion, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.GeoRegion.Attach(geoRegion);
                this.ObjectContext.GeoRegion.DeleteObject(geoRegion);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'GeoSecteur' query.
        public IQueryable<GeoSecteur> GetGeoSecteur()
        {
            return this.ObjectContext.GeoSecteur;
        }

        public void InsertGeoSecteur(GeoSecteur geoSecteur)
        {
            if ((geoSecteur.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoSecteur, EntityState.Added);
            }
            else
            {
                this.ObjectContext.GeoSecteur.AddObject(geoSecteur);
            }
        }

        public void UpdateGeoSecteur(GeoSecteur currentGeoSecteur)
        {
            this.ObjectContext.GeoSecteur.AttachAsModified(currentGeoSecteur, this.ChangeSet.GetOriginal(currentGeoSecteur));
        }

        public void DeleteGeoSecteur(GeoSecteur geoSecteur)
        {
            if ((geoSecteur.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(geoSecteur, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.GeoSecteur.Attach(geoSecteur);
                this.ObjectContext.GeoSecteur.DeleteObject(geoSecteur);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'HistoAdmin' query.
        public IQueryable<HistoAdmin> GetHistoAdmin()
        {
            return this.ObjectContext.HistoAdmin;
        }

        public void InsertHistoAdmin(HistoAdmin histoAdmin)
        {
            if ((histoAdmin.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(histoAdmin, EntityState.Added);
            }
            else
            {
                this.ObjectContext.HistoAdmin.AddObject(histoAdmin);
            }
        }

        public void UpdateHistoAdmin(HistoAdmin currentHistoAdmin)
        {
            this.ObjectContext.HistoAdmin.AttachAsModified(currentHistoAdmin, this.ChangeSet.GetOriginal(currentHistoAdmin));
        }

        public void DeleteHistoAdmin(HistoAdmin histoAdmin)
        {
            if ((histoAdmin.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(histoAdmin, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.HistoAdmin.Attach(histoAdmin);
                this.ObjectContext.HistoAdmin.DeleteObject(histoAdmin);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'HistoEquipement' query.
        public IQueryable<HistoEquipement> GetHistoEquipement()
        {
            return this.ObjectContext.HistoEquipement;
        }

        public void InsertHistoEquipement(HistoEquipement histoEquipement)
        {
            if ((histoEquipement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(histoEquipement, EntityState.Added);
            }
            else
            {
                this.ObjectContext.HistoEquipement.AddObject(histoEquipement);
            }
        }

        public void UpdateHistoEquipement(HistoEquipement currentHistoEquipement)
        {
            this.ObjectContext.HistoEquipement.AttachAsModified(currentHistoEquipement, this.ChangeSet.GetOriginal(currentHistoEquipement));
        }

        public void DeleteHistoEquipement(HistoEquipement histoEquipement)
        {
            if ((histoEquipement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(histoEquipement, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.HistoEquipement.Attach(histoEquipement);
                this.ObjectContext.HistoEquipement.DeleteObject(histoEquipement);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'HistoPp' query.
        public IQueryable<HistoPp> GetHistoPp()
        {
            return this.ObjectContext.HistoPp;
        }

        public void InsertHistoPp(HistoPp histoPp)
        {
            if ((histoPp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(histoPp, EntityState.Added);
            }
            else
            {
                this.ObjectContext.HistoPp.AddObject(histoPp);
            }
        }

        public void UpdateHistoPp(HistoPp currentHistoPp)
        {
            this.ObjectContext.HistoPp.AttachAsModified(currentHistoPp, this.ChangeSet.GetOriginal(currentHistoPp));
        }

        public void DeleteHistoPp(HistoPp histoPp)
        {
            if ((histoPp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(histoPp, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.HistoPp.Attach(histoPp);
                this.ObjectContext.HistoPp.DeleteObject(histoPp);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Images' query.
        public IQueryable<Image> GetImages()
        {
            return this.ObjectContext.Images;
        }

        public void InsertImage(Image image)
        {
            if ((image.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(image, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Images.AddObject(image);
            }
        }

        public void UpdateImage(Image currentImage)
        {
            this.ObjectContext.Images.AttachAsModified(currentImage, this.ChangeSet.GetOriginal(currentImage));
        }

        public void DeleteImage(Image image)
        {
            if ((image.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(image, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Images.Attach(image);
                this.ObjectContext.Images.DeleteObject(image);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'InsInstrument' query.
        public IQueryable<InsInstrument> GetInsInstrument()
        {
            return this.ObjectContext.InsInstrument;
        }

        public void InsertInsInstrument(InsInstrument insInstrument)
        {
            if ((insInstrument.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(insInstrument, EntityState.Added);
            }
            else
            {
                this.ObjectContext.InsInstrument.AddObject(insInstrument);
            }
        }

        public void UpdateInsInstrument(InsInstrument currentInsInstrument)
        {
            this.ObjectContext.InsInstrument.AttachAsModified(currentInsInstrument, this.ChangeSet.GetOriginal(currentInsInstrument));
        }

        public void DeleteInsInstrument(InsInstrument insInstrument)
        {
            if ((insInstrument.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(insInstrument, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.InsInstrument.Attach(insInstrument);
                this.ObjectContext.InsInstrument.DeleteObject(insInstrument);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'InstrumentsUtilises' query.
        public IQueryable<InstrumentsUtilises> GetInstrumentsUtilises()
        {
            return this.ObjectContext.InstrumentsUtilises;
        }

        public void InsertInstrumentsUtilises(InstrumentsUtilises instrumentsUtilises)
        {
            if ((instrumentsUtilises.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(instrumentsUtilises, EntityState.Added);
            }
            else
            {
                this.ObjectContext.InstrumentsUtilises.AddObject(instrumentsUtilises);
            }
        }

        public void UpdateInstrumentsUtilises(InstrumentsUtilises currentInstrumentsUtilises)
        {
            this.ObjectContext.InstrumentsUtilises.AttachAsModified(currentInstrumentsUtilises, this.ChangeSet.GetOriginal(currentInstrumentsUtilises));
        }

        public void DeleteInstrumentsUtilises(InstrumentsUtilises instrumentsUtilises)
        {
            if ((instrumentsUtilises.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(instrumentsUtilises, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.InstrumentsUtilises.Attach(instrumentsUtilises);
                this.ObjectContext.InstrumentsUtilises.DeleteObject(instrumentsUtilises);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'LogOuvrage' query.
        public IQueryable<LogOuvrage> GetLogOuvrage()
        {
            return this.ObjectContext.LogOuvrage;
        }

        public void InsertLogOuvrage(LogOuvrage logOuvrage)
        {
            if ((logOuvrage.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(logOuvrage, EntityState.Added);
            }
            else
            {
                this.ObjectContext.LogOuvrage.AddObject(logOuvrage);
            }
        }

        public void UpdateLogOuvrage(LogOuvrage currentLogOuvrage)
        {
            this.ObjectContext.LogOuvrage.AttachAsModified(currentLogOuvrage, this.ChangeSet.GetOriginal(currentLogOuvrage));
        }

        public void DeleteLogOuvrage(LogOuvrage logOuvrage)
        {
            if ((logOuvrage.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(logOuvrage, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.LogOuvrage.Attach(logOuvrage);
                this.ObjectContext.LogOuvrage.DeleteObject(logOuvrage);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'LogTournee' query.
        public IQueryable<LogTournee> GetLogTournee()
        {
            return this.ObjectContext.LogTournee;
        }

        public void InsertLogTournee(LogTournee logTournee)
        {
            if ((logTournee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(logTournee, EntityState.Added);
            }
            else
            {
                this.ObjectContext.LogTournee.AddObject(logTournee);
            }
        }

        public void UpdateLogTournee(LogTournee currentLogTournee)
        {
            this.ObjectContext.LogTournee.AttachAsModified(currentLogTournee, this.ChangeSet.GetOriginal(currentLogTournee));
        }

        public void DeleteLogTournee(LogTournee logTournee)
        {
            if ((logTournee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(logTournee, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.LogTournee.Attach(logTournee);
                this.ObjectContext.LogTournee.DeleteObject(logTournee);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'MesClassementMesure' query.
        public IQueryable<MesClassementMesure> GetMesClassementMesure()
        {
            return this.ObjectContext.MesClassementMesure;
        }

        public void InsertMesClassementMesure(MesClassementMesure mesClassementMesure)
        {
            if ((mesClassementMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesClassementMesure, EntityState.Added);
            }
            else
            {
                this.ObjectContext.MesClassementMesure.AddObject(mesClassementMesure);
            }
        }

        public void UpdateMesClassementMesure(MesClassementMesure currentMesClassementMesure)
        {
            this.ObjectContext.MesClassementMesure.AttachAsModified(currentMesClassementMesure, this.ChangeSet.GetOriginal(currentMesClassementMesure));
        }

        public void DeleteMesClassementMesure(MesClassementMesure mesClassementMesure)
        {
            if ((mesClassementMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesClassementMesure, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.MesClassementMesure.Attach(mesClassementMesure);
                this.ObjectContext.MesClassementMesure.DeleteObject(mesClassementMesure);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'MesCoutMesure' query.
        public IQueryable<MesCoutMesure> GetMesCoutMesure()
        {
            return this.ObjectContext.MesCoutMesure;
        }

        public void InsertMesCoutMesure(MesCoutMesure mesCoutMesure)
        {
            if ((mesCoutMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesCoutMesure, EntityState.Added);
            }
            else
            {
                this.ObjectContext.MesCoutMesure.AddObject(mesCoutMesure);
            }
        }

        public void UpdateMesCoutMesure(MesCoutMesure currentMesCoutMesure)
        {
            this.ObjectContext.MesCoutMesure.AttachAsModified(currentMesCoutMesure, this.ChangeSet.GetOriginal(currentMesCoutMesure));
        }

        public void DeleteMesCoutMesure(MesCoutMesure mesCoutMesure)
        {
            if ((mesCoutMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesCoutMesure, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.MesCoutMesure.Attach(mesCoutMesure);
                this.ObjectContext.MesCoutMesure.DeleteObject(mesCoutMesure);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'MesMesure' query.
        public IQueryable<MesMesure> GetMesMesure()
        {
            return this.ObjectContext.MesMesure;
        }

        public void InsertMesMesure(MesMesure mesMesure)
        {
            if ((mesMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesMesure, EntityState.Added);
            }
            else
            {
                this.ObjectContext.MesMesure.AddObject(mesMesure);
            }
        }

        public void UpdateMesMesure(MesMesure currentMesMesure)
        {
            this.ObjectContext.MesMesure.AttachAsModified(currentMesMesure, this.ChangeSet.GetOriginal(currentMesMesure));
        }

        public void DeleteMesMesure(MesMesure mesMesure)
        {
            if ((mesMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesMesure, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.MesMesure.Attach(mesMesure);
                this.ObjectContext.MesMesure.DeleteObject(mesMesure);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'MesModeleMesure' query.
        public IQueryable<MesModeleMesure> GetMesModeleMesure()
        {
            return this.ObjectContext.MesModeleMesure;
        }

        public void InsertMesModeleMesure(MesModeleMesure mesModeleMesure)
        {
            if ((mesModeleMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesModeleMesure, EntityState.Added);
            }
            else
            {
                this.ObjectContext.MesModeleMesure.AddObject(mesModeleMesure);
            }
        }

        public void UpdateMesModeleMesure(MesModeleMesure currentMesModeleMesure)
        {
            this.ObjectContext.MesModeleMesure.AttachAsModified(currentMesModeleMesure, this.ChangeSet.GetOriginal(currentMesModeleMesure));
        }

        public void DeleteMesModeleMesure(MesModeleMesure mesModeleMesure)
        {
            if ((mesModeleMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesModeleMesure, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.MesModeleMesure.Attach(mesModeleMesure);
                this.ObjectContext.MesModeleMesure.DeleteObject(mesModeleMesure);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'MesNiveauProtection' query.
        public IQueryable<MesNiveauProtection> GetMesNiveauProtection()
        {
            return this.ObjectContext.MesNiveauProtection;
        }

        public void InsertMesNiveauProtection(MesNiveauProtection mesNiveauProtection)
        {
            if ((mesNiveauProtection.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesNiveauProtection, EntityState.Added);
            }
            else
            {
                this.ObjectContext.MesNiveauProtection.AddObject(mesNiveauProtection);
            }
        }

        public void UpdateMesNiveauProtection(MesNiveauProtection currentMesNiveauProtection)
        {
            this.ObjectContext.MesNiveauProtection.AttachAsModified(currentMesNiveauProtection, this.ChangeSet.GetOriginal(currentMesNiveauProtection));
        }

        public void DeleteMesNiveauProtection(MesNiveauProtection mesNiveauProtection)
        {
            if ((mesNiveauProtection.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesNiveauProtection, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.MesNiveauProtection.Attach(mesNiveauProtection);
                this.ObjectContext.MesNiveauProtection.DeleteObject(mesNiveauProtection);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'MesTypeMesure' query.
        public IQueryable<MesTypeMesure> GetMesTypeMesure()
        {
            return this.ObjectContext.MesTypeMesure;
        }

        public void InsertMesTypeMesure(MesTypeMesure mesTypeMesure)
        {
            if ((mesTypeMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesTypeMesure, EntityState.Added);
            }
            else
            {
                this.ObjectContext.MesTypeMesure.AddObject(mesTypeMesure);
            }
        }

        public void UpdateMesTypeMesure(MesTypeMesure currentMesTypeMesure)
        {
            this.ObjectContext.MesTypeMesure.AttachAsModified(currentMesTypeMesure, this.ChangeSet.GetOriginal(currentMesTypeMesure));
        }

        public void DeleteMesTypeMesure(MesTypeMesure mesTypeMesure)
        {
            if ((mesTypeMesure.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesTypeMesure, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.MesTypeMesure.Attach(mesTypeMesure);
                this.ObjectContext.MesTypeMesure.DeleteObject(mesTypeMesure);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'MesUnite' query.
        public IQueryable<MesUnite> GetMesUnite()
        {
            return this.ObjectContext.MesUnite;
        }

        public void InsertMesUnite(MesUnite mesUnite)
        {
            if ((mesUnite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesUnite, EntityState.Added);
            }
            else
            {
                this.ObjectContext.MesUnite.AddObject(mesUnite);
            }
        }

        public void UpdateMesUnite(MesUnite currentMesUnite)
        {
            this.ObjectContext.MesUnite.AttachAsModified(currentMesUnite, this.ChangeSet.GetOriginal(currentMesUnite));
        }

        public void DeleteMesUnite(MesUnite mesUnite)
        {
            if ((mesUnite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(mesUnite, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.MesUnite.Attach(mesUnite);
                this.ObjectContext.MesUnite.DeleteObject(mesUnite);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ParametreAction' query.
        public IQueryable<ParametreAction> GetParametreAction()
        {
            return this.ObjectContext.ParametreAction;
        }

        public void InsertParametreAction(ParametreAction parametreAction)
        {
            if ((parametreAction.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(parametreAction, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ParametreAction.AddObject(parametreAction);
            }
        }

        public void UpdateParametreAction(ParametreAction currentParametreAction)
        {
            this.ObjectContext.ParametreAction.AttachAsModified(currentParametreAction, this.ChangeSet.GetOriginal(currentParametreAction));
        }

        public void DeleteParametreAction(ParametreAction parametreAction)
        {
            if ((parametreAction.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(parametreAction, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.ParametreAction.Attach(parametreAction);
                this.ObjectContext.ParametreAction.DeleteObject(parametreAction);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'PiSecteurs' query.
        public IQueryable<PiSecteurs> GetPiSecteurs()
        {
            return this.ObjectContext.PiSecteurs;
        }

        public void InsertPiSecteurs(PiSecteurs piSecteurs)
        {
            if ((piSecteurs.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(piSecteurs, EntityState.Added);
            }
            else
            {
                this.ObjectContext.PiSecteurs.AddObject(piSecteurs);
            }
        }

        public void UpdatePiSecteurs(PiSecteurs currentPiSecteurs)
        {
            this.ObjectContext.PiSecteurs.AttachAsModified(currentPiSecteurs, this.ChangeSet.GetOriginal(currentPiSecteurs));
        }

        public void DeletePiSecteurs(PiSecteurs piSecteurs)
        {
            if ((piSecteurs.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(piSecteurs, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.PiSecteurs.Attach(piSecteurs);
                this.ObjectContext.PiSecteurs.DeleteObject(piSecteurs);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'PortionDates' query.
        public IQueryable<PortionDates> GetPortionDates()
        {
            return this.ObjectContext.PortionDates;
        }

        public void InsertPortionDates(PortionDates portionDates)
        {
            if ((portionDates.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(portionDates, EntityState.Added);
            }
            else
            {
                this.ObjectContext.PortionDates.AddObject(portionDates);
            }
        }

        public void UpdatePortionDates(PortionDates currentPortionDates)
        {
            this.ObjectContext.PortionDates.AttachAsModified(currentPortionDates, this.ChangeSet.GetOriginal(currentPortionDates));
        }

        public void DeletePortionDates(PortionDates portionDates)
        {
            if ((portionDates.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(portionDates, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.PortionDates.Attach(portionDates);
                this.ObjectContext.PortionDates.DeleteObject(portionDates);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'PortionIntegrite' query.
        public IQueryable<PortionIntegrite> GetPortionIntegrite()
        {
            return this.ObjectContext.PortionIntegrite;
        }

        public void InsertPortionIntegrite(PortionIntegrite portionIntegrite)
        {
            if ((portionIntegrite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(portionIntegrite, EntityState.Added);
            }
            else
            {
                this.ObjectContext.PortionIntegrite.AddObject(portionIntegrite);
            }
        }

        public void UpdatePortionIntegrite(PortionIntegrite currentPortionIntegrite)
        {
            this.ObjectContext.PortionIntegrite.AttachAsModified(currentPortionIntegrite, this.ChangeSet.GetOriginal(currentPortionIntegrite));
        }

        public void DeletePortionIntegrite(PortionIntegrite portionIntegrite)
        {
            if ((portionIntegrite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(portionIntegrite, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.PortionIntegrite.Attach(portionIntegrite);
                this.ObjectContext.PortionIntegrite.DeleteObject(portionIntegrite);
            }
        }


        public void InsertPortionIntegriteAnAction(PortionIntegriteAnAction portionIntegrite)
        {
            if ((portionIntegrite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(portionIntegrite, EntityState.Added);
            }
            else
            {
                this.ObjectContext.PortionIntegriteAnAction.AddObject(portionIntegrite);
            }
        }

        public void UpdatePortionIntegrite(PortionIntegriteAnAction currentPortionIntegrite)
        {
            this.ObjectContext.PortionIntegriteAnAction.AttachAsModified(currentPortionIntegrite, this.ChangeSet.GetOriginal(currentPortionIntegrite));
        }

        public void DeletePortionIntegrite(PortionIntegriteAnAction portionIntegrite)
        {
            if ((portionIntegrite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(portionIntegrite, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.PortionIntegriteAnAction.Attach(portionIntegrite);
                this.ObjectContext.PortionIntegriteAnAction.DeleteObject(portionIntegrite);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Pps' query.
        public IQueryable<Pp> GetPps()
        {
            return this.ObjectContext.Pps;
        }

        public void InsertPp(Pp pp)
        {
            if ((pp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(pp, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Pps.AddObject(pp);
            }
        }

        public void UpdatePp(Pp currentPp)
        {
            this.ObjectContext.Pps.AttachAsModified(currentPp, this.ChangeSet.GetOriginal(currentPp));
        }

        public void DeletePp(Pp pp)
        {
            if ((pp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(pp, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Pps.Attach(pp);
                this.ObjectContext.Pps.DeleteObject(pp);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'PpEquipement' query.
        public IQueryable<PpEquipement> GetPpEquipement()
        {
            return this.ObjectContext.PpEquipement;
        }

        public void InsertPpEquipement(PpEquipement ppEquipement)
        {
            if ((ppEquipement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ppEquipement, EntityState.Added);
            }
            else
            {
                this.ObjectContext.PpEquipement.AddObject(ppEquipement);
            }
        }

        public void UpdatePpEquipement(PpEquipement currentPpEquipement)
        {
            this.ObjectContext.PpEquipement.AttachAsModified(currentPpEquipement, this.ChangeSet.GetOriginal(currentPpEquipement));
        }

        public void DeletePpEquipement(PpEquipement ppEquipement)
        {
            if ((ppEquipement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ppEquipement, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.PpEquipement.Attach(ppEquipement);
                this.ObjectContext.PpEquipement.DeleteObject(ppEquipement);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'PpJumelee' query.
        public IQueryable<PpJumelee> GetPpJumelee()
        {
            return this.ObjectContext.PpJumelee;
        }

        public void InsertPpJumelee(PpJumelee ppJumelee)
        {
            if ((ppJumelee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ppJumelee, EntityState.Added);
            }
            else
            {
                this.ObjectContext.PpJumelee.AddObject(ppJumelee);
            }
        }

        public void UpdatePpJumelee(PpJumelee currentPpJumelee)
        {
            this.ObjectContext.PpJumelee.AttachAsModified(currentPpJumelee, this.ChangeSet.GetOriginal(currentPpJumelee));
        }

        public void DeletePpJumelee(PpJumelee ppJumelee)
        {
            if ((ppJumelee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ppJumelee, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.PpJumelee.Attach(ppJumelee);
                this.ObjectContext.PpJumelee.DeleteObject(ppJumelee);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'PpTmp' query.
        public IQueryable<PpTmp> GetPpTmp()
        {
            return this.ObjectContext.PpTmp;
        }

        public void InsertPpTmp(PpTmp ppTmp)
        {
            if ((ppTmp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ppTmp, EntityState.Added);
            }
            else
            {
                this.ObjectContext.PpTmp.AddObject(ppTmp);
            }
        }

        public void UpdatePpTmp(PpTmp currentPpTmp)
        {
            this.ObjectContext.PpTmp.AttachAsModified(currentPpTmp, this.ChangeSet.GetOriginal(currentPpTmp));
        }

        public void DeletePpTmp(PpTmp ppTmp)
        {
            if ((ppTmp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ppTmp, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.PpTmp.Attach(ppTmp);
                this.ObjectContext.PpTmp.DeleteObject(ppTmp);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefCommune' query.
        public IQueryable<RefCommune> GetRefCommune()
        {
            return this.ObjectContext.RefCommune;
        }

        public void InsertRefCommune(RefCommune refCommune)
        {
            if ((refCommune.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refCommune, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefCommune.AddObject(refCommune);
            }
        }

        public void UpdateRefCommune(RefCommune currentRefCommune)
        {
            this.ObjectContext.RefCommune.AttachAsModified(currentRefCommune, this.ChangeSet.GetOriginal(currentRefCommune));
        }

        public void DeleteRefCommune(RefCommune refCommune)
        {
            if ((refCommune.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refCommune, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefCommune.Attach(refCommune);
                this.ObjectContext.RefCommune.DeleteObject(refCommune);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefDiametre' query.
        public IQueryable<RefDiametre> GetRefDiametre()
        {
            return this.ObjectContext.RefDiametre;
        }

        public void InsertRefDiametre(RefDiametre refDiametre)
        {
            if ((refDiametre.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refDiametre, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefDiametre.AddObject(refDiametre);
            }
        }

        public void UpdateRefDiametre(RefDiametre currentRefDiametre)
        {
            this.ObjectContext.RefDiametre.AttachAsModified(currentRefDiametre, this.ChangeSet.GetOriginal(currentRefDiametre));
        }

        public void DeleteRefDiametre(RefDiametre refDiametre)
        {
            if ((refDiametre.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refDiametre, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefDiametre.Attach(refDiametre);
                this.ObjectContext.RefDiametre.DeleteObject(refDiametre);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefEnumValeur' query.
        public IQueryable<RefEnumValeur> GetRefEnumValeur()
        {
            return this.ObjectContext.RefEnumValeur;
        }

        public void InsertRefEnumValeur(RefEnumValeur refEnumValeur)
        {
            if ((refEnumValeur.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refEnumValeur, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefEnumValeur.AddObject(refEnumValeur);
            }
        }

        public void UpdateRefEnumValeur(RefEnumValeur currentRefEnumValeur)
        {
            this.ObjectContext.RefEnumValeur.AttachAsModified(currentRefEnumValeur, this.ChangeSet.GetOriginal(currentRefEnumValeur));
        }

        public void DeleteRefEnumValeur(RefEnumValeur refEnumValeur)
        {
            if ((refEnumValeur.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refEnumValeur, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefEnumValeur.Attach(refEnumValeur);
                this.ObjectContext.RefEnumValeur.DeleteObject(refEnumValeur);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefNiveauSensibilitePp' query.
        public IQueryable<RefNiveauSensibilitePp> GetRefNiveauSensibilitePp()
        {
            return this.ObjectContext.RefNiveauSensibilitePp;
        }

        public void InsertRefNiveauSensibilitePp(RefNiveauSensibilitePp refNiveauSensibilitePp)
        {
            if ((refNiveauSensibilitePp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refNiveauSensibilitePp, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefNiveauSensibilitePp.AddObject(refNiveauSensibilitePp);
            }
        }

        public void UpdateRefNiveauSensibilitePp(RefNiveauSensibilitePp currentRefNiveauSensibilitePp)
        {
            this.ObjectContext.RefNiveauSensibilitePp.AttachAsModified(currentRefNiveauSensibilitePp, this.ChangeSet.GetOriginal(currentRefNiveauSensibilitePp));
        }

        public void DeleteRefNiveauSensibilitePp(RefNiveauSensibilitePp refNiveauSensibilitePp)
        {
            if ((refNiveauSensibilitePp.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refNiveauSensibilitePp, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefNiveauSensibilitePp.Attach(refNiveauSensibilitePp);
                this.ObjectContext.RefNiveauSensibilitePp.DeleteObject(refNiveauSensibilitePp);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefParametre' query.
        public IQueryable<RefParametre> GetRefParametre()
        {
            return this.ObjectContext.RefParametre;
        }

        public void InsertRefParametre(RefParametre refParametre)
        {
            if ((refParametre.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refParametre, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefParametre.AddObject(refParametre);
            }
        }

        public void UpdateRefParametre(RefParametre currentRefParametre)
        {
            this.ObjectContext.RefParametre.AttachAsModified(currentRefParametre, this.ChangeSet.GetOriginal(currentRefParametre));
        }

        public void DeleteRefParametre(RefParametre refParametre)
        {
            if ((refParametre.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refParametre, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefParametre.Attach(refParametre);
                this.ObjectContext.RefParametre.DeleteObject(refParametre);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefRevetement' query.
        public IQueryable<RefRevetement> GetRefRevetement()
        {
            return this.ObjectContext.RefRevetement;
        }

        public void InsertRefRevetement(RefRevetement refRevetement)
        {
            if ((refRevetement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refRevetement, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefRevetement.AddObject(refRevetement);
            }
        }

        public void UpdateRefRevetement(RefRevetement currentRefRevetement)
        {
            this.ObjectContext.RefRevetement.AttachAsModified(currentRefRevetement, this.ChangeSet.GetOriginal(currentRefRevetement));
        }

        public void DeleteRefRevetement(RefRevetement refRevetement)
        {
            if ((refRevetement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refRevetement, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefRevetement.Attach(refRevetement);
                this.ObjectContext.RefRevetement.DeleteObject(refRevetement);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefSousTypeOuvrage' query.
        public IQueryable<RefSousTypeOuvrage> GetRefSousTypeOuvrage()
        {
            return this.ObjectContext.RefSousTypeOuvrage;
        }

        public void InsertRefSousTypeOuvrage(RefSousTypeOuvrage refSousTypeOuvrage)
        {
            if ((refSousTypeOuvrage.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refSousTypeOuvrage, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefSousTypeOuvrage.AddObject(refSousTypeOuvrage);
            }
        }

        public void UpdateRefSousTypeOuvrage(RefSousTypeOuvrage currentRefSousTypeOuvrage)
        {
            this.ObjectContext.RefSousTypeOuvrage.AttachAsModified(currentRefSousTypeOuvrage, this.ChangeSet.GetOriginal(currentRefSousTypeOuvrage));
        }

        public void DeleteRefSousTypeOuvrage(RefSousTypeOuvrage refSousTypeOuvrage)
        {
            if ((refSousTypeOuvrage.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refSousTypeOuvrage, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefSousTypeOuvrage.Attach(refSousTypeOuvrage);
                this.ObjectContext.RefSousTypeOuvrage.DeleteObject(refSousTypeOuvrage);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefUsrAutorisation' query.
        public IQueryable<RefUsrAutorisation> GetRefUsrAutorisation()
        {
            return this.ObjectContext.RefUsrAutorisation;
        }

        public void InsertRefUsrAutorisation(RefUsrAutorisation refUsrAutorisation)
        {
            if ((refUsrAutorisation.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refUsrAutorisation, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefUsrAutorisation.AddObject(refUsrAutorisation);
            }
        }

        public void UpdateRefUsrAutorisation(RefUsrAutorisation currentRefUsrAutorisation)
        {
            this.ObjectContext.RefUsrAutorisation.AttachAsModified(currentRefUsrAutorisation, this.ChangeSet.GetOriginal(currentRefUsrAutorisation));
        }

        public void DeleteRefUsrAutorisation(RefUsrAutorisation refUsrAutorisation)
        {
            if ((refUsrAutorisation.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refUsrAutorisation, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefUsrAutorisation.Attach(refUsrAutorisation);
                this.ObjectContext.RefUsrAutorisation.DeleteObject(refUsrAutorisation);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefUsrGroupe' query.
        public IQueryable<RefUsrGroupe> GetRefUsrGroupe()
        {
            return this.ObjectContext.RefUsrGroupe;
        }

        public void InsertRefUsrGroupe(RefUsrGroupe refUsrGroupe)
        {
            if ((refUsrGroupe.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refUsrGroupe, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefUsrGroupe.AddObject(refUsrGroupe);
            }
        }

        public void UpdateRefUsrGroupe(RefUsrGroupe currentRefUsrGroupe)
        {
            this.ObjectContext.RefUsrGroupe.AttachAsModified(currentRefUsrGroupe, this.ChangeSet.GetOriginal(currentRefUsrGroupe));
        }

        public void DeleteRefUsrGroupe(RefUsrGroupe refUsrGroupe)
        {
            if ((refUsrGroupe.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refUsrGroupe, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefUsrGroupe.Attach(refUsrGroupe);
                this.ObjectContext.RefUsrGroupe.DeleteObject(refUsrGroupe);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'RefUsrPortee' query.
        public IQueryable<RefUsrPortee> GetRefUsrPortee()
        {
            return this.ObjectContext.RefUsrPortee;
        }

        public void InsertRefUsrPortee(RefUsrPortee refUsrPortee)
        {
            if ((refUsrPortee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refUsrPortee, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RefUsrPortee.AddObject(refUsrPortee);
            }
        }

        public void UpdateRefUsrPortee(RefUsrPortee currentRefUsrPortee)
        {
            this.ObjectContext.RefUsrPortee.AttachAsModified(currentRefUsrPortee, this.ChangeSet.GetOriginal(currentRefUsrPortee));
        }

        public void DeleteRefUsrPortee(RefUsrPortee refUsrPortee)
        {
            if ((refUsrPortee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(refUsrPortee, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RefUsrPortee.Attach(refUsrPortee);
                this.ObjectContext.RefUsrPortee.DeleteObject(refUsrPortee);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Tournees' query.
        public IQueryable<Tournee> GetTournees()
        {
            return this.ObjectContext.Tournees;
        }

        public void InsertTournee(Tournee tournee)
        {
            if ((tournee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tournee, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Tournees.AddObject(tournee);
            }
        }

        public void UpdateTournee(Tournee currentTournee)
        {
            this.ObjectContext.Tournees.AttachAsModified(currentTournee, this.ChangeSet.GetOriginal(currentTournee));
        }

        public void DeleteTournee(Tournee tournee)
        {
            if ((tournee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tournee, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Tournees.Attach(tournee);
                this.ObjectContext.Tournees.DeleteObject(tournee);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'TourneePpEq' query.
        public IQueryable<TourneePpEq> GetTourneePpEq()
        {
            return this.ObjectContext.TourneePpEq;
        }

        public void InsertTourneePpEq(TourneePpEq tourneePpEq)
        {
            if ((tourneePpEq.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tourneePpEq, EntityState.Added);
            }
            else
            {
                this.ObjectContext.TourneePpEq.AddObject(tourneePpEq);
            }
        }

        public void UpdateTourneePpEq(TourneePpEq currentTourneePpEq)
        {
            this.ObjectContext.TourneePpEq.AttachAsModified(currentTourneePpEq, this.ChangeSet.GetOriginal(currentTourneePpEq));
        }

        public void DeleteTourneePpEq(TourneePpEq tourneePpEq)
        {
            if ((tourneePpEq.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tourneePpEq, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.TourneePpEq.Attach(tourneePpEq);
                this.ObjectContext.TourneePpEq.DeleteObject(tourneePpEq);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'TypeEquipement' query.
        public IQueryable<TypeEquipement> GetTypeEquipement()
        {
            return this.ObjectContext.TypeEquipement;
        }

        public void InsertTypeEquipement(TypeEquipement typeEquipement)
        {
            if ((typeEquipement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(typeEquipement, EntityState.Added);
            }
            else
            {
                this.ObjectContext.TypeEquipement.AddObject(typeEquipement);
            }
        }

        public void UpdateTypeEquipement(TypeEquipement currentTypeEquipement)
        {
            this.ObjectContext.TypeEquipement.AttachAsModified(currentTypeEquipement, this.ChangeSet.GetOriginal(currentTypeEquipement));
        }

        public void DeleteTypeEquipement(TypeEquipement typeEquipement)
        {
            if ((typeEquipement.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(typeEquipement, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.TypeEquipement.Attach(typeEquipement);
                this.ObjectContext.TypeEquipement.DeleteObject(typeEquipement);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'UsrProfil' query.
        public IQueryable<UsrProfil> GetUsrProfil()
        {
            return this.ObjectContext.UsrProfil;
        }

        public void InsertUsrProfil(UsrProfil usrProfil)
        {
            if ((usrProfil.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(usrProfil, EntityState.Added);
            }
            else
            {
                this.ObjectContext.UsrProfil.AddObject(usrProfil);
            }
        }

        public void UpdateUsrProfil(UsrProfil currentUsrProfil)
        {
            this.ObjectContext.UsrProfil.AttachAsModified(currentUsrProfil, this.ChangeSet.GetOriginal(currentUsrProfil));
        }

        public void DeleteUsrProfil(UsrProfil usrProfil)
        {
            if ((usrProfil.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(usrProfil, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.UsrProfil.Attach(usrProfil);
                this.ObjectContext.UsrProfil.DeleteObject(usrProfil);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'UsrRole' query.
        public IQueryable<UsrRole> GetUsrRole()
        {
            return this.ObjectContext.UsrRole;
        }

        public void InsertUsrRole(UsrRole usrRole)
        {
            if ((usrRole.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(usrRole, EntityState.Added);
            }
            else
            {
                this.ObjectContext.UsrRole.AddObject(usrRole);
            }
        }

        public void UpdateUsrRole(UsrRole currentUsrRole)
        {
            this.ObjectContext.UsrRole.AttachAsModified(currentUsrRole, this.ChangeSet.GetOriginal(currentUsrRole));
        }

        public void DeleteUsrRole(UsrRole usrRole)
        {
            if ((usrRole.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(usrRole, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.UsrRole.Attach(usrRole);
                this.ObjectContext.UsrRole.DeleteObject(usrRole);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'UsrUtilisateur' query.
        public IQueryable<UsrUtilisateur> GetUsrUtilisateur()
        {
            return this.ObjectContext.UsrUtilisateur;
        }

        public void InsertUsrUtilisateur(UsrUtilisateur usrUtilisateur)
        {
            if ((usrUtilisateur.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(usrUtilisateur, EntityState.Added);
            }
            else
            {
                this.ObjectContext.UsrUtilisateur.AddObject(usrUtilisateur);
            }
        }

        public void UpdateUsrUtilisateur(UsrUtilisateur currentUsrUtilisateur)
        {
            this.ObjectContext.UsrUtilisateur.AttachAsModified(currentUsrUtilisateur, this.ChangeSet.GetOriginal(currentUsrUtilisateur));
        }

        public void DeleteUsrUtilisateur(UsrUtilisateur usrUtilisateur)
        {
            if ((usrUtilisateur.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(usrUtilisateur, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.UsrUtilisateur.Attach(usrUtilisateur);
                this.ObjectContext.UsrUtilisateur.DeleteObject(usrUtilisateur);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Visites' query.
        public IQueryable<Visite> GetVisites()
        {
            return this.ObjectContext.Visites;
        }

        public void InsertVisite(Visite visite)
        {
            if ((visite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(visite, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Visites.AddObject(visite);
            }
        }

        public void UpdateVisite(Visite currentVisite)
        {
            this.ObjectContext.Visites.AttachAsModified(currentVisite, this.ChangeSet.GetOriginal(currentVisite));
        }

        public void DeleteVisite(Visite visite)
        {
            if ((visite.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(visite, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Visites.Attach(visite);
                this.ObjectContext.Visites.DeleteObject(visite);
            }
        }
    }
}


