using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Proteca.Web.Models;
using System.ComponentModel;
using Telerik.Windows.Controls;
using System;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class Equipement_Tournees : UserControl, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Listes des tournées
        /// </summary>
        public List<Tournee> Tournees
        {
            get { return (List<Tournee>)GetValue(TourneesProperty); }
            set { SetValue(TourneesProperty, value); }
        }

        public static readonly DependencyProperty TourneesProperty = DependencyProperty.Register("Tournees",
            typeof(List<Tournee>), typeof(Equipement_Tournees), new PropertyMetadata(null));

        /// <summary>
        /// Liste des nom de colonne a cacher lors des exports
        /// </summary>
        public List<string> ColumnsHiddenToExport
        {
            get { return (List<string>)GetValue(ColumnsHiddenToExportProperty); }
            set { SetValue(ColumnsHiddenToExportProperty, value); }
        }

        public static readonly DependencyProperty ColumnsHiddenToExportProperty = DependencyProperty.Register("ColumnsHiddenToExport",
            typeof(List<string>), typeof(Equipement_Tournees), new PropertyMetadata(null));

        /// <summary>
        /// Commande de l'export PDF
        /// </summary>
        public ICommand ExportPDFCommand
        {
            get { return (ICommand)GetValue(ExportPDFCommandProperty); }
            set { SetValue(ExportPDFCommandProperty, value); }
        }

        public static readonly DependencyProperty ExportPDFCommandProperty = DependencyProperty.Register("ExportPDFCommand", typeof(ICommand),
                               typeof(Equipement_Tournees), new PropertyMetadata(null, null));

        /// <summary>
        /// Commande de l'impression
        /// </summary>
        public ICommand PrintCommand
        {
            get { return (ICommand)GetValue(PrintCommandProperty); }
            set { SetValue(PrintCommandProperty, value); }
        }

        public static readonly DependencyProperty PrintCommandProperty = DependencyProperty.Register("PrintCommand", typeof(ICommand),
                               typeof(Equipement_Tournees), new PropertyMetadata(null, null));

        /// <summary>
        /// Commande de l'export Excel
        /// </summary>
        public ICommand ExportExcelCommand
        {
            get { return (ICommand)GetValue(ExportExcelCommandProperty); }
            set { SetValue(ExportExcelCommandProperty, value); }
        }

        public static readonly DependencyProperty ExportExcelCommandProperty = DependencyProperty.Register("ExportExcelCommand", typeof(ICommand),
                               typeof(Equipement_Tournees), new PropertyMetadata(null, null));

        #endregion Properties

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        public Equipement_Tournees()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Equipement_Tournees_Loaded);
        }

        #endregion Construecteur

        #region Events

        /// <summary>
        /// Onn chargement du user control on attache à la grid au user control d'impression 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Equipement_Tournees_Loaded(object sender, EventArgs e)
        {
            Grid myGrid = this.Content as Grid;
            ((ExportAndPrint)myGrid.Children[0]).GridView = myGrid.Children[1] as RadGridView;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion Events
    }
}
