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
using System.ComponentModel;

namespace Proteca.Silverlight.Services
{
    [Export(typeof(SynchronizationService))]
    public class SynchronizationService
    {
        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        [Import]
        public ProtecaDomainContext domainContext { get; set; }

        [Import(AllowDefault = true)]
        public Proteca.Silverlight.Services.Contracts.IConfigurator configurator;

        public void ExporterContext()
        {
            RemoveTmpPPDuplications(this.domainContext);
            string json = domainContext.SerializeContext();
            var operation = domainContext.SaveDomainContext(json);
        }

        public void ImporterContext(string content)
        {
            this.domainContext.EntityContainer.Clear();
            this.domainContext.RestoreContext(content);
            ImportedContextChanged = RemoveTmpPPDuplications(this.domainContext);
        }
        public bool ImportedContextChanged { get; private set; }

        public string LibelleTournee
        {
            get
            {
                if (this.domainContext != null
                    && this.domainContext.Tournees.Count > 0)
                {
                    return this.domainContext.Tournees.FirstOrDefault().Libelle;
                }
                else
                {
                    return String.Empty;
                }
            }
        }


        public static bool RemoveTmpPPDuplications(ProtecaDomainContext domainContext, Action<int> foundItemsToDelete = null, Action itemDeleted = null)
        {
            var ppTmpGrps = CreateIntegrityData(domainContext);
            var firstItemsToDelete = ppTmpGrps.SelectMany(ppTmpGrp => ppTmpGrp.GetObsoleteItemsToDeleteDirect()).ToList();
            var conflictsAutoResolved = ppTmpGrps.Where(ppTmpGrp => ppTmpGrp.NeedChoice).SelectMany(ppTmpGrp => ppTmpGrp.GetObsoleteItemsToDeleteAutoChoice()).ToList();
            var conflictedItems = ppTmpGrps.Where(ppTmpGrp => ppTmpGrp.NeedChoice && !ppTmpGrp.AutoChoiceFound).ToList();
            var itemsToDelete = firstItemsToDelete.Concat(conflictsAutoResolved);
            if (foundItemsToDelete != null)
                foundItemsToDelete(itemsToDelete.Count());
            foreach (var ppTmpToDelete in itemsToDelete)
            {
                domainContext.PpTmps.Remove(ppTmpToDelete);
                if (itemDeleted != null)
                    itemDeleted();
            }
            return itemsToDelete.Any();
        }
        private static List<PpDataCheckInfo> CreateIntegrityData(ProtecaDomainContext domainContext)
        {
            return domainContext.EntityContainer.GetChanges().OfType<PpTmp>()
                .GroupBy(
                    pp => pp.Pp,
                    (pp, grp) => new PpDataCheckInfo
                    {
                        PP = pp,
                        Groups = grp
                            .GroupBy(pptmp => GetValuesAsString(pptmp), (title, pptmps) => new PpTmpGroupInfo { Values = pptmps.ToList() })
                            .ToList()
                    })
                .ToList();
        }

        private static string GetValuesAsString(PpTmp ppTmp)
        {
            return string.Join(",",
                ppTmp.CleCategoriePp,
                ppTmp.CleNiveauSensibilite,
                ppTmp.EnumDureeEnrg,
                ppTmp.EnumPolarisation,
                ppTmp.EnumSurfaceTme,
                ppTmp.EnumSurfaceTms,
                ppTmp.CourantsAlternatifsInduits,
                ppTmp.CourantsVagabonds,
                ppTmp.ElectrodeEnterreeAmovible,
                ppTmp.TemoinEnterreAmovible,
                ppTmp.TemoinMetalliqueDeSurface,
                ppTmp.PresenceDUneTelemesure,
                ppTmp.PositionGpsLat,
                ppTmp.PositionGpsLong,
                ppTmp.CoordonneeGpsFiabilisee);
        }



        public class SimpleViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void RaisePropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class PpDataCheckInfo : SimpleViewModel
        {
            public Pp PP { get; set; }
            public List<PpTmpGroupInfo> Groups { get; set; }

            private PpTmpGroupInfo _SelectedGroup;
            public PpTmpGroupInfo SelectedGroup
            {
                get { return _SelectedGroup; }
                set
                {
                    _SelectedGroup = value;
                    RaisePropertyChanged("SelectedGroup");
                }
            }

            public IEnumerable<PpTmp> GetObsoleteItemsToDeleteDirect()
            {
                return Groups.SelectMany(grp => grp.PpTmpToDeleteDirect());
            }

            public bool NeedChoice { get { return Groups.Skip(1).Any(); } }
            public bool AutoChoiceFound { get; private set; }
            public IEnumerable<PpTmp> GetObsoleteItemsToDeleteAutoChoice()
            {
                var items = GetObsoleteItemsToDisplayForManualChoice().ToList();
                if (items.Skip(1).Any())
                {
                    var changedItems = items.Where(item => item.HasAnyChangesFromOriginalPP).ToList();
                    if (changedItems.Count == 1)
                    {
                        items.Remove(changedItems.First());
                        AutoChoiceFound = true;
                        return items;
                    }
                }
                return Enumerable.Empty<PpTmp>();


            }
            public IEnumerable<PpTmp> GetObsoleteItemsToDisplayForManualChoice()
            {
                return GroupsToDelete().SelectMany(grp => grp.PpTmpToDisplayForManualChoice());
            }
            private IEnumerable<PpTmpGroupInfo> GroupsToDelete()
            {
                foreach (var grp in Groups)
                    if (grp != SelectedGroup)
                        yield return grp;
            }
        }
        public class PpTmpGroupInfo
        {
            public string DisplayText { get; set; }
            public List<PpTmp> Values { get; set; }
            public IEnumerable<PpTmp> PpTmpToDeleteDirect()
            {
                return Values.Skip(1);
            }
            public IEnumerable<PpTmp> PpTmpToDisplayForManualChoice()
            {
                yield return Values.First();
            }
        }



    }
}
