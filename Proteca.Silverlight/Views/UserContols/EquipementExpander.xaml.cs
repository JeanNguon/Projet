using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class EquipementExpander : UserControl, INotifyPropertyChanged
    {
        #region Public Properties

        /// <summary>
        /// Liste des regions.
        /// </summary>
        public ObservableCollection<GeoRegion> Regions
        {
            get { return (ObservableCollection<GeoRegion>)GetValue(RegionsProperty); }
            set { SetValue(RegionsProperty, value); }
        }

        public static readonly DependencyProperty RegionsProperty = DependencyProperty.Register("Regions", 
            typeof(ObservableCollection<GeoRegion>), typeof(EquipementExpander), new PropertyMetadata(null));

        /// <summary>
        /// Liste des Ensemble électrique.
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsemblesElectrique
        {
            get { return (List<GeoEnsElecPortion>)GetValue(GeoEnsemblesElectriqueProperty); }
            set { SetValue(GeoEnsemblesElectriqueProperty, value); }
        }

        public static readonly DependencyProperty GeoEnsemblesElectriqueProperty = DependencyProperty.Register("GeoEnsemblesElectrique",
            typeof(List<GeoEnsElecPortion>), typeof(EquipementExpander), new PropertyMetadata(null));

        /// <summary>
        /// Liste des Ensemble électrique.
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsElecPortions
        {
            get { return (List<GeoEnsElecPortion>)GetValue(GeoEnsElecPortionsProperty); }
            set { SetValue(GeoEnsElecPortionsProperty, value); }
        }

        public static readonly DependencyProperty GeoEnsElecPortionsProperty = DependencyProperty.Register("GeoEnsElecPortions",
            typeof(List<GeoEnsElecPortion>), typeof(EquipementExpander), new PropertyMetadata(null));

        /// <summary>
        /// Indique la clé de la région sélectionnée
        /// </summary>
        public int? FiltreCleRegion
        {
            get { return (int?)GetValue(FiltreCleRegionProperty); }
            set { SetValue(FiltreCleRegionProperty, value); }
        }

        public static readonly DependencyProperty FiltreCleRegionProperty = DependencyProperty.Register("FiltreCleRegion", typeof(int?),
                               typeof(EquipementExpander), new PropertyMetadata(null));

        /// <summary>
        /// Indique la clé de l'agence sélectionnée
        /// </summary>
        public int? FiltreCleAgence
        {
            get { return (int?)GetValue(FiltreCleAgenceProperty); }
            set { SetValue(FiltreCleAgenceProperty, value); }
        }

        public static readonly DependencyProperty FiltreCleAgenceProperty = DependencyProperty.Register("FiltreCleAgence", typeof(int?),
                               typeof(EquipementExpander), new PropertyMetadata(null));

        /// <summary>
        /// Indique la clé du secteur sélectionné
        /// </summary>
        public int? FiltreCleSecteur
        {
            get { return (int?)GetValue(FiltreCleSecteurProperty); }
            set { SetValue(FiltreCleSecteurProperty, value); }
        }

        public static readonly DependencyProperty FiltreCleSecteurProperty = DependencyProperty.Register("FiltreCleSecteur", typeof(int?),
                               typeof(EquipementExpander), new PropertyMetadata(null));
        
        /// <summary>
        /// Retourne la clé de l'ensemble électrique
        /// </summary>
        public int? FiltreCleEnsElec
        {
            get { return (int?)GetValue(FiltreCleEnsElecProperty); }
            set { SetValue(FiltreCleEnsElecProperty, value); }
        }

        public static readonly DependencyProperty FiltreCleEnsElecProperty = DependencyProperty.Register("FiltreCleEnsElec", typeof(int?),
                               typeof(EquipementExpander), new PropertyMetadata(null));

        /// <summary>
        /// Retourne la clé de la portion intégrité
        /// </summary>
        public int? FiltreClePortion 
        {
            get { return (int?)GetValue(FiltreClePortionProperty); }
            set { SetValue(FiltreClePortionProperty, value); }
        }

        public static readonly DependencyProperty FiltreClePortionProperty = DependencyProperty.Register("FiltreClePortion", typeof(int?),
                               typeof(EquipementExpander), new PropertyMetadata(null));

        /// <summary>
        /// Indique si l'on inclue les portions supprimées dans la recherche
        /// </summary>
        public bool DisplayEquipmentResult
        {
            get { return this.RadCbxResultEqEquipement.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                {
                    this.RadCbxResultPp.Visibility = Visibility.Collapsed;
                    this.RadCbxResultEqEquipement.Visibility = Visibility.Visible;
                }
                else
                {
                    this.RadCbxResultPp.Visibility = Visibility.Visible;
                    this.RadCbxResultEqEquipement.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Indique si l'on inclue les portions supprimées dans la recherche
        /// </summary>
        public bool IncludeDeletedEquipment
        {
            get { return (bool)GetValue(IncludeDeletedEquipmentProperty); }
            set { SetValue(IncludeDeletedEquipmentProperty, value); }
        }

        public static readonly DependencyProperty IncludeDeletedEquipmentProperty = DependencyProperty.Register("IncludeDeletedEquipment", typeof(bool),
                               typeof(EquipementExpander), new PropertyMetadata(null));

        /// <summary>
        /// Commande de recherche
        /// </summary>
        public ICommand FindCommand
        {
            get { return (ICommand)GetValue(FindCommandProperty); }
            set { SetValue(FindCommandProperty, value); }
        }

        public static readonly DependencyProperty FindCommandProperty = DependencyProperty.Register("FindCommand", typeof(ICommand),
                               typeof(EquipementExpander), new PropertyMetadata(null, null));

        #endregion Public Properties

        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public EquipementExpander()
        {
            InitializeComponent();
        }
        
        #endregion Constructor

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion Events
    }
}
