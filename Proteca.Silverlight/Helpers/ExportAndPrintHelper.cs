using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.FormatProviders.Html;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.TextBased.Csv;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace Proteca.Silverlight.Helpers
{
    /// <summary>
    /// Helper permettant de gérer l'export et l'impression d'une RadGridView Telerik
    /// </summary>
    public static class ExportAndPrintHelper
    {
        #region public Properties

        public static int RadGridViewIndex { get { return 0; } }
        public static int ColumnsToExportIndex { get { return 1; } }
        public static int RichTextBoxIndex { get { return 2; } }
        public static int RadDataPagerIndex { get { return 3; } }
        public static Color GroupHeaderBackground { get { return Colors.LightGray; } }
        public static Color HeaderBackground { get { return Color.FromArgb(255, 137, 183, 216); } }
        public static Color HeaderForeground { get { return Color.FromArgb(255, 11, 62, 151); } }
        public static Color RowForeground { get { return Colors.Black; } }
        public static Color RowBackground { get { return Colors.White; } }
        public static Color RowForegroundAlternate { get { return Colors.Black; } }
        public static Color RowBackgroundAlternate { get { return Color.FromArgb(255, 237, 237, 237); } }

        #endregion public Properties

        #region Public Functions

        /// <summary>
        /// Récupère le paramètre positionner à l'index dans la liste des parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object GetParameterAtIndex(object parameters, int index)
        {
            List<object> myParams = (List<object>)parameters;
            object ret = null;

            if (myParams != null && myParams.Count > index)
            {
                ret = myParams[index];
            }

            return ret;
        }

        /// <summary>
        /// On initialise le gridview afin de rendre seulement visible les colonnes 
        /// qu'on l'on souhaite visualiser dans l'export.
        /// Cette fonction permet d'inverser la visibilité des colonnes lors de l'export.
        /// Cette fonction doit être appelée avant et après l'export 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="columnIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public static RadGridView InverseVisibilityColumnsToExport(object parameter, int columnIndex, int gridIndex)
        {
            ObservableCollection<string> columnsHiddenToExport = (ObservableCollection<string>)ExportAndPrintHelper.GetParameterAtIndex(parameter, columnIndex);
            RadGridView grid = (RadGridView)ExportAndPrintHelper.GetParameterAtIndex(parameter, gridIndex);

            if (columnsHiddenToExport != null && columnsHiddenToExport.Count > 0)
            {
                foreach (GridViewColumn col in grid.Columns)
                {
                    if (columnsHiddenToExport.Contains(col.Header) || (col.Header != null && String.IsNullOrEmpty(col.Header.ToString())))
                    {
                        col.IsVisible = !col.IsVisible;
                    }
                }
            }

            return grid;
        }

        public static RadDocument CreateDocument(RadGridView grid)
        {
            RadDocument document = null;

            using (var stream = new MemoryStream())
            {
                grid.ElementExporting += elementExporting;

                grid.Export(stream, new GridViewExportOptions()
                {
                    Format = ExportFormat.Html,
                    ShowColumnFooters = grid.ShowColumnFooters,
                    ShowColumnHeaders = grid.ShowColumnHeaders,
                    ShowGroupFooters = grid.ShowGroupFooters,
                    Culture = new CultureInfo("fr-FR"),
                    Items = grid.Items
                });

                grid.ElementExporting -= elementExporting;

                stream.Position = 0;

                HtmlFormatProvider provider = new HtmlFormatProvider();
                document = new HtmlFormatProvider().Import(stream);
                document.SectionDefaultPageMargin = new Telerik.Windows.Documents.Layout.Padding(20);
                document.LayoutMode = DocumentLayoutMode.Paged;
                document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
            }

            return document;
        }



        public static Workbook CreateWorkBook(RadGridView grid)
        {
            Workbook book = null;

            using (var stream = new MemoryStream())
            {
                int index = 0;
                foreach (GroupDescriptor group in grid.GroupDescriptors)
                {
                    grid.Columns.Insert(index++, new GridViewDataColumn() { DataMemberBinding = new System.Windows.Data.Binding(group.Member) });
                }

                grid.ElementExporting += elementExporting;

                var exportOptions = new GridViewExportOptions()
                {
                    Format = ExportFormat.Csv,
                    ShowColumnFooters = grid.ShowColumnFooters,
                    ShowColumnHeaders = grid.ShowColumnHeaders,
                    ShowGroupFooters = grid.ShowGroupFooters,
                    Culture = new CultureInfo("fr-FR"),
                    Items = grid.Items
                };
                grid.Export(stream, exportOptions);

                grid.ElementExporting -= elementExporting;


                stream.Position = 0;

                var csvProvider = new CsvFormatProvider();
                book = csvProvider.Import(stream);


                // /!\ telerik date format workaround
                var pattern = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                var dateCellFormat = new CellValueFormat(pattern);

                var dateColsIdx = grid.Columns.Cast<GridViewColumn>().OfType<GridViewDataColumn>().Where(col => col.DataType == typeof(DateTime)).Select(col => col.DisplayIndex).ToList();

                if (dateColsIdx.Any())
                {
                    var skipFirstRow = true;
                    var rowCount = grid.Items.Count;
                    for (int i = skipFirstRow ? 1 : 0; i < rowCount; i++)
                    {
                        foreach (var colIdx in dateColsIdx)
                        {
                            book.Worksheets[0].Cells[i, colIdx].SetFormat(dateCellFormat);
                        }
                    }
                }

                foreach (GroupDescriptor group in grid.GroupDescriptors)
                {
                    grid.Columns.RemoveAt(0);
                }
            }

            return book;
        }

        #endregion Public Functions

        #region Private Functions

        private static int rowCount = 0;

        /// <summary>
        /// Application des couleurs sur les éléments du tableau
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void elementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            e.FontFamily = new FontFamily("Verdana");
            e.FontSize = 9;
            e.VerticalAlignment = VerticalAlignment.Center;

            if (e.Element == ExportElement.Table)
            {
                e.Attributes["border"] = "0";
            }
            else if (e.Element == ExportElement.HeaderRow)
            {
                rowCount = 0;

                e.Styles.Add("background-color", ExportAndPrintHelper.HeaderBackground.ToString().Remove(1, 2));

                e.Background = ExportAndPrintHelper.HeaderBackground;
                e.Foreground = ExportAndPrintHelper.HeaderForeground;
                e.FontWeight = FontWeights.Bold;
                e.FontSize = 10;
            }
            else if (e.Element == ExportElement.GroupHeaderRow)
            {
                rowCount = 0;

                e.Styles.Add("background-color", ExportAndPrintHelper.GroupHeaderBackground.ToString().Remove(1, 2));
                e.Background = ExportAndPrintHelper.GroupHeaderBackground;
                e.Foreground = ExportAndPrintHelper.HeaderForeground;
                e.FontWeight = FontWeights.Bold;
            }
            else if (e.Element == ExportElement.HeaderCell)
            {
                e.Styles.Add("white-space", "nowrap");

                if (e.Value is Image && (e.Context as GridViewDataColumn) != null)
                {
                    e.Value = (e.Context as GridViewDataColumn).DataMemberBinding.Path.Path;
                }
            }
            else if (e.Element == ExportElement.GroupHeaderCell && String.IsNullOrEmpty(e.Value.ToString()) && e.Context is QueryableCollectionViewGroup)
            {
                e.Value = (e.Context as QueryableCollectionViewGroup).Key;
            }
            else if (e.Element == ExportElement.Row)
            {
                rowCount++;

                if (rowCount % 2 == 0)
                {
                    e.Background = ExportAndPrintHelper.RowBackgroundAlternate;
                    e.Foreground = ExportAndPrintHelper.RowForegroundAlternate;
                }
                else
                {
                    e.Background = ExportAndPrintHelper.RowBackground;
                    e.Foreground = ExportAndPrintHelper.RowForeground;
                }
                e.Styles.Add("background-color", e.Background.ToString().Remove(1, 2));
            }
            else if (e.Element == ExportElement.Cell && (e.Context as GridViewDataColumn) != null)
            {
                if ((e.Context as GridViewDataColumn).DataType == typeof(DateTime)
                    || (e.Context as GridViewDataColumn).DataType == typeof(Decimal)
                    || (e.Context as GridViewDataColumn).DataType == typeof(Double)
                    || (e.Context as GridViewDataColumn).DataType == typeof(int)
                    )
                {
                    (e.Context as GridViewDataColumn).TextWrapping = TextWrapping.NoWrap;
                    e.Styles.Add("white-space", "nowrap");
                }

                if (e.Value is bool)
                {
                    e.Value = ((bool)e.Value) ? "oui" : "non";
                }
                if ((e.Context as GridViewDataColumn).DataType == typeof(DateTime))
                {
                    e.Width = 80;
                }
            }
        }

        #endregion Private Functions
    }
}
