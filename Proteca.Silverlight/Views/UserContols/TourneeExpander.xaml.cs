using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Proteca.Web.Models;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class TourneeExpander : UserControl
    {
        public TourneeExpander()
        {
            InitializeComponent();
        }

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
            typeof(ObservableCollection<GeoRegion>), typeof(TourneeExpander), new PropertyMetadata(null));

        /// <summary>
        /// Liste des Ensemble électrique.
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsemblesElectrique
        {
            get { return (List<GeoEnsElecPortion>)GetValue(GeoEnsemblesElectriqueProperty); }
            set { SetValue(GeoEnsemblesElectriqueProperty, value); }
        }

        public static readonly DependencyProperty GeoEnsemblesElectriqueProperty = DependencyProperty.Register("GeoEnsemblesElectrique",
            typeof(List<GeoEnsElecPortion>), typeof(TourneeExpander), new PropertyMetadata(null));

        /// <summary>
        /// Liste des Ensemble électrique.
        /// </summary>
        public List<GeoEnsElecPortion> GeoEnsElecPortions
        {
            get { return (List<GeoEnsElecPortion>)GetValue(GeoEnsElecPortionsProperty); }
            set { SetValue(GeoEnsElecPortionsProperty, value); }
        }

        public static readonly DependencyProperty GeoEnsElecPortionsProperty = DependencyProperty.Register("GeoEnsElecPortions",
            typeof(List<GeoEnsElecPortion>), typeof(TourneeExpander), new PropertyMetadata(null));

        /// <summary>
        /// Indique la clé de la région sélectionnée
        /// </summary>
        public int? FiltreCleRegion
        {
            get { return (int?)GetValue(FiltreCleRegionProperty); }
            set { SetValue(FiltreCleRegionProperty, value); }
        }

        public static readonly DependencyProperty FiltreCleRegionProperty = DependencyProperty.Register("FiltreCleRegion", typeof(int?),
                               typeof(TourneeExpander), new PropertyMetadata(null));

        /// <summary>
        /// Indique la clé de l'agence sélectionnée
        /// </summary>
        public int? FiltreCleAgence
        {
            get { return (int?)GetValue(FiltreCleAgenceProperty); }
            set { SetValue(FiltreCleAgenceProperty, value); }
        }

        public static readonly DependencyProperty FiltreCleAgenceProperty = DependencyProperty.Register("FiltreCleAgence", typeof(int?),
                               typeof(TourneeExpander), new PropertyMetadata(null));


        /// <summary>
        /// Indique la clé du secteur sélectionné
        /// </summary>
        public int? FiltreCleSecteur
        {
            get { return (int?)GetValue(FiltreCleSecteurProperty); }
            set { SetValue(FiltreCleSecteurProperty, value); }
        }

        public static readonly DependencyProperty FiltreCleSecteurProperty = DependencyProperty.Register("FiltreCleSecteur", typeof(int?),
                               typeof(TourneeExpander), new PropertyMetadata(null));

        /// <summary>
        /// Retourne la clé de l'ensemble électrique
        /// </summary>
        public int? FiltreCleEnsElec
        {
            get { return (int?)GetValue(FiltreCleEnsElecProperty); }
            set { SetValue(FiltreCleEnsElecProperty, value); }
        }

        public static readonly DependencyProperty FiltreCleEnsElecProperty = DependencyProperty.Register("FiltreCleEnsElec", typeof(int?),
                               typeof(TourneeExpander), new PropertyMetadata(null));

        /// <summary>
        /// Retourne la clé de la portion intégrité
        /// </summary>
        public int? FiltreClePortion
        {
            get { return (int?)GetValue(FiltreClePortionProperty); }
            set { SetValue(FiltreClePortionProperty, value); }
        }

        public static readonly DependencyProperty FiltreClePortionProperty = DependencyProperty.Register("FiltreClePortion", typeof(int?),
                               typeof(TourneeExpander), new PropertyMetadata(null));

        
        public static readonly DependencyProperty IncludeDeletedEquipmentProperty = DependencyProperty.Register("IncludeDeletedEquipment", typeof(bool),
                               typeof(TourneeExpander), new PropertyMetadata(null));


        #endregion Public Properties
        
        public bool DisplayTourneeResult
        {
            get { return this.RadCbxResultTournee.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                {
                    this.RadCbxResultTournee.Visibility = Visibility.Visible;
                }
                else
                {
                    this.RadCbxResultTournee.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Commande de recherche
        /// </summary>
        public ICommand FindCommand
        {
            get { return (ICommand)GetValue(FindCommandProperty); }
            set { SetValue(FindCommandProperty, value); }
        }

        public static readonly DependencyProperty FindCommandProperty = DependencyProperty.Register("FindCommand", typeof(ICommand),
                               typeof(TourneeExpander), new PropertyMetadata(null, null));

       


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
