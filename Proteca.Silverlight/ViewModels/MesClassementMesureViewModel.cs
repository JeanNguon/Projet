using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using Jounce.Core.Application;
using Jounce.Core.Command;
using Jounce.Core.ViewModel;
using Jounce.Framework;
using Jounce.Framework.Command;
using System.Text;
using Proteca.Web.Models;
using Proteca.Silverlight.Services.Contracts;
using Proteca.Silverlight.Resources;
using Proteca.Silverlight.Services.EntityServices;
using Telerik.Windows.Controls.DragDrop;
using Proteca.Silverlight.Helpers;

namespace Proteca.Silverlight.ViewModels
{
    /// <summary>
    /// ViewModel for MesClassementMesure entity
    /// </summary>
    [ExportAsViewModel("MesClassementMesure")]
    public class MesClassementMesureViewModel : BaseProtecaEntityViewModel<MesClassementMesure>
    {
        #region Properties

        /// <summary>
        /// Déclaration de la variable FiltreCourantVagabond
        /// </summary>
        /// <remarks>
        /// Récupération de la zone de recherche d'une entité
        /// </remarks>
        public Boolean FiltreCourantVagabond { get; set; }

        /// <summary>
        /// Déclaration de la variable FiltreCourantAlternatif
        /// </summary>
        /// <remarks>
        /// Récupération de la zone de recherche d'une entité
        /// </remarks>
        public Boolean FiltreCourantAlternatif { get; set; }

        /// <summary>
        /// Déclaration de la variable FiltreElectrodeEnterre
        /// </summary>
        /// <remarks>
        /// Récupération de la zone de recherche d'une entité
        /// </remarks>
        public Boolean FiltreElectrodeEnterre { get; set; }

        /// <summary>
        /// Déclaration de la variable FiltreTemoinEnterre
        /// </summary>
        /// <remarks>
        /// Récupération de la zone de recherche d'une entité
        /// </remarks>
        public Boolean FiltreTemoinEnterre { get; set; }

        /// <summary>
        /// Déclaration de la variable FiltreTemoinSurface
        /// </summary>
        /// <remarks>
        /// Récupération de la zone de recherche d'une entité
        /// </remarks>
        public Boolean FiltreTemoinSurface { get; set; }

        /// <summary>
        /// Déclaration de la variable FiltreTelemesure
        /// </summary>
        /// <remarks>
        /// Récupération de la zone de recherche d'une entité
        /// </remarks>
        public Boolean FiltreTelemesure { get; set; }

        /// <summary>
        /// Retourne l'ensemble des type mesure disponible
        /// </summary>
        public ObservableCollection<MesTypeMesure> TypeMesureDispo
        {
            get
            {
                if (serviceTypemesure.Entities != null && CurrentNavigation.Current.Filtre != null)
                {
                    // Application du filtre de type équipement sur le service
                    ObservableCollection<MesTypeMesure> MesTypesMesureFiltre = new ObservableCollection<MesTypeMesure>(serviceTypemesure.Entities.Where(r => r.MesModeleMesure.TypeEquipement.CodeEquipement == CurrentNavigation.Current.Filtre.ToString()));

                    // Application du second filtre si les type mesure obligatoire != vide
                    // Sinon on retourne le service
                    if (Typesmesure != null)
                    {
                        return new ObservableCollection<MesTypeMesure>(MesTypesMesureFiltre.Where(tm => !Typesmesure.Select(t => t.LibTypeMesure).Contains(tm.LibTypeMesure)).OrderBy(r => r.NumeroOrdre));
                    }
                    else
                    {
                        return new ObservableCollection<MesTypeMesure>(MesTypesMesureFiltre.OrderBy(r => r.NumeroOrdre));
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Déclaration de la variable des type de mesure obligatoire
        /// </summary>
        private ObservableCollection<MesTypeMesure> _typeMesure = null;

        /// <summary>
        /// Retourne l'ensemble des types mesures obligatoire
        /// </summary>
        public ObservableCollection<MesTypeMesure> Typesmesure
        {
            get
            {
                return _typeMesure;
            }
        }

        #endregion

        #region Constructor

        public MesClassementMesureViewModel()
            : base()
        {
            // Surcharge l'initialisation
            IsAutoNavigateToFirst = false;

            this.OnViewActivated += (o, e) =>
            {
                if (!e.ViewParameter.Any(p => p.Key == "IsExpanderLoaded"))
                {
                    EventAggregator.Publish("CustomExpander".AsViewNavigationArgs().AddNamedParameter("Title", " Recherche des types de mesures"));
                    EventAggregator.Publish("MesClassementMesure_Expander".AsViewNavigationArgs().AddNamedParameter("IsExpanderLoaded", true));
                }

                if (!e.ViewParameter.Any(r => r.Key == "IsTopContainerLoaded"))
                {
                    EventAggregator.Publish("CustomTopContainer".AsViewNavigationArgs().AddNamedParameter("HideContainer", false));
                    EventAggregator.Publish("TypeEquipement".AsViewNavigationArgs().AddNamedParameter("IsTopContainerLoaded", true));
                }
            };


            OnEntitiesLoaded += (oo, ee) =>
            {
                IsBusy = true;
                initGeoPreferences();
                Find();
            };

            OnViewModeChanged += (o, e) =>
            {
                if (IsEditMode)
                {
                    RaisePropertyChanged(() => this.TypeMesureDispo);
                }
            };

            this.OnFindLoaded += (o, e) =>
            {
                if (this.Entities != null)
                {
                    _typeMesure = new ObservableCollection<MesTypeMesure>(this.Entities.Select(c => c.MesTypeMesure));
                    _typeMesure.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Typesmesure_CollectionChanged);
                }

                RaisePropertyChanged(() => this.Typesmesure);
            };

            this.OnCanceled += (o, e) =>
            {
                // Rechargement des listes
                LoadEntities();

                // MAJ de la vue
                RaisePropertyChanged(() => this.Typesmesure);
                RaisePropertyChanged(() => this.TypeMesureDispo);
            };

        }

        public override void LoadEntities()
        {
            Find();
        }

        public override void LoadDetailEntity()
        {
            // ne rien faire car ne s'applique pas à cet écran
        }

        void Typesmesure_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MesClassementMesure mcm;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:

                    foreach (var item in e.NewItems)
                    {
                        MesTypeMesure ItemToModif = item as MesTypeMesure;

                        mcm = new MesClassementMesure()
                                {
                                    CleTypeMesure = ItemToModif.CleTypeMesure,
                                    CourantsVagabons = FiltreCourantVagabond,
                                    CourantsAlternatifsInduits = FiltreCourantAlternatif,
                                    ElectrodeEnterreeAmovible = FiltreElectrodeEnterre,
                                    TemoinEnterre = FiltreTemoinEnterre,
                                    TemoinDeSurface = FiltreTemoinSurface,
                                    Telemesure = FiltreTelemesure
                                };

                        service.Add(mcm);
                        Entities.Add(mcm);
                    }

                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:

                    MesTypeMesure ItemToRemove = e.OldItems[0] as MesTypeMesure;

                    // On supprime l'entité des classements mesure
                    mcm = Entities.Where(r => r.CleTypeMesure == ItemToRemove.CleTypeMesure).FirstOrDefault();
                    service.Delete(mcm);

                    break;
            }

            _typeMesure = new ObservableCollection<MesTypeMesure>(this.Entities.Select(c => c.MesTypeMesure).OrderBy(r => r.NumeroOrdre));
            _typeMesure.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Typesmesure_CollectionChanged);

            // MAJ de la vue
            RaisePropertyChanged(() => this.Typesmesure);
            RaisePropertyChanged(() => this.TypeMesureDispo);

            this.CanSave = true;
            SetCanProperties();
        }

        #endregion

        #region Services

        /// <summary>
        /// Service utilisé pour gérer l'entité de type Region
        /// </summary>
        [Import]
        public IEntityService<MesTypeMesure> serviceTypemesure { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Méthode de recherche appellé par la commande FindCommand
        /// cette méthode appelle la méthode Find du service
        /// pour remonter une liste d'entité en fonction de paramètres
        /// </summary>
        protected override void Find()
        {
            // Application du filtre
            this.Filtres = new List<System.Linq.Expressions.Expression<Func<MesClassementMesure, bool>>>();
            this.Filtres.Add(u => u.CourantsVagabons == this.FiltreCourantVagabond);
            this.Filtres.Add(u => u.CourantsAlternatifsInduits == this.FiltreCourantAlternatif);
            this.Filtres.Add(u => u.ElectrodeEnterreeAmovible == this.FiltreElectrodeEnterre);
            this.Filtres.Add(u => u.TemoinEnterre == this.FiltreTemoinEnterre);
            this.Filtres.Add(u => u.TemoinDeSurface == this.FiltreTemoinSurface);
            this.Filtres.Add(u => u.Telemesure == this.FiltreTelemesure);
            // remonter uniquement les mesures en service
            this.Filtres.Add(u => u.MesTypeMesure.MesureEnService == true);
            if (CurrentNavigation.Current.Filtre != null)
            {
                this.Filtres.Add(u => u.MesTypeMesure.MesModeleMesure.TypeEquipement.CodeEquipement == CurrentNavigation.Current.Filtre.ToString());
            }
            base.Find();

        }

        // EP (11/01/2012) : non utilisé
        ///// <summary>
        ///// Méthode utilisé pour charger l'entité de type Region
        ///// </summary>
        //private void TypesmesureLoaded(Exception error)
        //{
        //    Logger.Log(LogSeverity.Information, GetType().FullName, MethodBase.GetCurrentMethod().Name);

        //    if (error != null)
        //    {
        //        Logger.Log(LogSeverity.Error, this.GetType().FullName, error.ToString());
        //        ErrorWindow.CreateNew(string.Format(Resource.BaseProtecaEntityViewModel_LoadError, typeof(MesTypeMesure).Name));
        //    }
        //    else
        //    {
        //        RaisePropertyChanged(() => this.TypeMesureDispo);
        //    }

        //    // We're done
        //    IsBusy = false;
        //}

        #endregion

    }
}
