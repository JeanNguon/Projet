using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Navigation;
using Jounce.Core.View;
using Jounce.Core.ViewModel;
using Jounce.Regions.Core;
using Telerik.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using Proteca.Silverlight.Helpers;
using System.ComponentModel;

namespace Proteca.Silverlight.Views.UserContols
{
    /// <summary>
    /// User Control permettant de gérer l'export Excel, l'export PDF 
    /// ainsi que l'impression d'un tableau
    /// </summary>
    public partial class ExportAndPrint : UserControl, INotifyPropertyChanged
    {
        #region Public Properties

        /// <summary>
        /// GridView à exporter
        /// </summary>
        public RadGridView GridView
        {
            get { return (RadGridView)GetValue(GridViewProperty); }
            set { SetValue(GridViewProperty, value); }
        }

        public static readonly DependencyProperty GridViewProperty =
                               DependencyProperty.Register("GridView", typeof(RadGridView),
                               typeof(ExportAndPrint), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// DataPager associé au GridView à exporter
        /// </summary>
        public RadDataPager DataPager
        {
            get { return (RadDataPager)GetValue(DataPagerProperty); }
            set { SetValue(DataPagerProperty, value); }
        }

        public static readonly DependencyProperty DataPagerProperty =
                               DependencyProperty.Register("DataPager", typeof(RadDataPager),
                               typeof(ExportAndPrint), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Liste des noms de colonnes qui ne doivent pas apparaître dans l'exportation et l'impression
        /// </summary>
        public ObservableCollection<string> ColumnsHiddenToExport
        {
            get { return (ObservableCollection<string>)GetValue(ColumnsHiddenToExportProperty); }
            set { SetValue(ColumnsHiddenToExportProperty, value); }
        }

        public static readonly DependencyProperty ColumnsHiddenToExportProperty =
                               DependencyProperty.Register("ColumnsHiddenToExport", typeof(ObservableCollection<string>),
                               typeof(ExportAndPrint), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Liste des paramètres a passer aux bouttons pour qu'ils puissent réaliser
        /// les exports et l'impression
        /// </summary>
        public List<object> ExportAndPrintParameter { get; set; }
        
        /// <summary>
        /// Commande d'impression
        /// </summary>
        public ICommand PrintCommand
        {
            get { return (ICommand)GetValue(PrintCommandProperty); }
            set { SetValue(PrintCommandProperty, value); }
        }

        public static readonly DependencyProperty PrintCommandProperty =
                               DependencyProperty.Register("PrintCommand", typeof(ICommand), 
                               typeof(ExportAndPrint), new PropertyMetadata(null, null));

        /// <summary>
        /// Commande d'export PDF
        /// </summary>
        public ICommand ExportPDFCommand
        {
            get { return (ICommand)GetValue(ExportPDFCommandProperty); }
            set { SetValue(ExportPDFCommandProperty, value); }
        }

        public static readonly DependencyProperty ExportPDFCommandProperty =
                               DependencyProperty.Register("ExportPDFCommand", typeof(ICommand),
                               typeof(ExportAndPrint), new PropertyMetadata(null, null));

        /// <summary>
        /// Commande d'export Excel
        /// </summary>
        public ICommand ExportExcelCommand
        {
            get { return (ICommand)GetValue(ExportExcelCommandProperty); }
            set { SetValue(ExportExcelCommandProperty, value); }
        }

        public static readonly DependencyProperty ExportExcelCommandProperty =
                               DependencyProperty.Register("ExportExcelCommand", typeof(ICommand),
                               typeof(ExportAndPrint), new PropertyMetadata(null, null));

        #endregion Public Properties

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public ExportAndPrint()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ExportAndPrint_Loaded);
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// Une fois le control chargé on initialise les paramètres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExportAndPrint_Loaded(object sender, RoutedEventArgs e)
        {
            initComandParameter();
        }

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArg)
        {
            if (d is ExportAndPrint)
            {
                (d as ExportAndPrint).initComandParameter();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion Events

        #region Private Functions


        protected void initComandParameter()
        {
            CreateCommandParameter(ExportAndPrintHelper.RadGridViewIndex, GridView);
            CreateCommandParameter(ExportAndPrintHelper.ColumnsToExportIndex, ColumnsHiddenToExport);
            CreateCommandParameter(ExportAndPrintHelper.RichTextBoxIndex, this.rtbForPrint);
            CreateCommandParameter(ExportAndPrintHelper.RadDataPagerIndex, DataPager);
        }

        /// <summary>
        /// Créer le CommandParameter pour les boutons
        /// </summary>
        /// <param name="index">index ou doit être passer la valeur</param>
        /// <param name="val">la valeur</param>
        private void CreateCommandParameter(int index, object val)
        {
            List<object> _params = this.ExportAndPrintParameter;

            if (_params == null)
                _params = new List<object>();

            int count = _params.Count;
            if (count <= index)
                for (int i = count; i < index + 1; i++)
                    _params.Add(null);

            _params[index] = val;

            this.ExportAndPrintParameter = _params;

            OnPropertyChanged("ExportAndPrintParameter");
        }

        #endregion Private Functions
    }
}
