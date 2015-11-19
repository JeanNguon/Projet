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
using Proteca.Silverlight.Models;

namespace Proteca.Silverlight.Views.UserContols
{
    public partial class Rejets : UserControl
    {
        #region Properties

        /// <summary>
        /// Liste des nom de colonne a cacher lors des exports
        /// </summary>
        public List<Rejet> ListRejets
        {
            get { return (List<Rejet>)GetValue(ListRejetsProperty); }
            set { SetValue(ListRejetsProperty, value); }
        }

        /// <summary>
        /// Liste des nom de colonne a cacher lors des exports
        /// </summary>
        public ObservableCollection<string> ColumnsHiddenToExport
        {
            get { return (ObservableCollection<string>)GetValue(ColumnsHiddenToExportProperty); }
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

        #endregion Properties

        #region DependancyProperty

        public static readonly DependencyProperty ListRejetsProperty = DependencyProperty.Register("ListRejets",
                                typeof(List<Rejet>), typeof(Rejets), new PropertyMetadata(null));

        public static readonly DependencyProperty ColumnsHiddenToExportProperty = DependencyProperty.Register("ColumnsHiddenToExport",
                                typeof(ObservableCollection<string>), typeof(Rejets), new PropertyMetadata(null));

        public static readonly DependencyProperty ExportPDFCommandProperty = DependencyProperty.Register("ExportPDFCommand", typeof(ICommand),
                               typeof(Rejets), new PropertyMetadata(null, null));

        public static readonly DependencyProperty PrintCommandProperty = DependencyProperty.Register("PrintCommand", typeof(ICommand),
                               typeof(Rejets), new PropertyMetadata(null, null));

        public static readonly DependencyProperty ExportExcelCommandProperty = DependencyProperty.Register("ExportExcelCommand", typeof(ICommand),
                               typeof(Rejets), new PropertyMetadata(null, null));

        #endregion DependancyProperty
        
        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        public Rejets()
        {
            InitializeComponent();
        }

        #endregion Constructeur

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
