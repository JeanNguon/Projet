using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Proteca.Web.Models;
using System.ServiceModel.DomainServices.Client;
using System;
using Telerik.Windows.Controls;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class Historique : UserControl, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Liste des nom de colonne a cacher lors des exports
        /// </summary>
        public List<LogOuvrage> MesLogsOuvrages
        {
            get { return (List<LogOuvrage>)GetValue(LogOuvrageProperty); }
            set { SetValue(LogOuvrageProperty, value); }
        }

        /// <summary>
        /// Liste des nom de colonne a cacher lors des exports
        /// </summary>
        public List<string> ColumnsHiddenToExport
        {
            get { return (List<string>)GetValue(ColumnsHiddenToExportProperty); }
            set { SetValue(ColumnsHiddenToExportProperty, value); }
        }

        /// <summary>
        /// Commande de l'export PDF
        /// </summary>
        public ICommand ExportPDFCommand
        {
            get { return (ICommand)GetValue(ExportPDFCommandProperty); }
            set { SetValue(ExportPDFCommandProperty, value); }
        }

        /// <summary>
        /// Commande de l'impression
        /// </summary>
        public ICommand PrintCommand
        {
            get { return (ICommand)GetValue(PrintCommandProperty); }
            set { SetValue(PrintCommandProperty, value); }
        }

        /// <summary>
        /// Commande de l'export Excel
        /// </summary>
        public ICommand ExportExcelCommand
        {
            get { return (ICommand)GetValue(ExportExcelCommandProperty); }
            set { SetValue(ExportExcelCommandProperty, value); }
        }

        /// <summary>
        /// Commande de la visualisation de l'historique
        /// </summary>
        public ICommand HistoViewCommand
        {
            get { return (ICommand)GetValue(HistoViewCommandProperty); }
            set 
            {
                SetValue(HistoViewCommandProperty, value);
            }
        }
        
        #endregion

        #region DependancyProperty

        public static readonly DependencyProperty LogOuvrageProperty = DependencyProperty.Register("MesLogsOuvrages",
                                typeof(List<LogOuvrage>), typeof(Historique), new PropertyMetadata(null));

        public static readonly DependencyProperty ColumnsHiddenToExportProperty = DependencyProperty.Register("ColumnsHiddenToExport",
                                typeof(List<string>), typeof(Historique), new PropertyMetadata(null));

        public static readonly DependencyProperty ExportPDFCommandProperty = DependencyProperty.Register("ExportPDFCommand", typeof(ICommand),
                               typeof(Historique), new PropertyMetadata(null, null));

        public static readonly DependencyProperty PrintCommandProperty = DependencyProperty.Register("PrintCommand", typeof(ICommand),
                               typeof(Historique), new PropertyMetadata(null, null));

        public static readonly DependencyProperty ExportExcelCommandProperty = DependencyProperty.Register("ExportExcelCommand", typeof(ICommand),
                               typeof(Historique), new PropertyMetadata(null, null));

        public static readonly DependencyProperty HistoViewCommandProperty = DependencyProperty.Register("HistoViewCommand", typeof(ICommand),
                       typeof(Historique), new PropertyMetadata(null, null));

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        public Historique()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Historique_Loaded);
        }

        #endregion Constructeur

        #region Events

        /// <summary>
        /// Onn chargement du user control on attache à la grid au user control d'impression 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Historique_Loaded(object sender, EventArgs e)
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
