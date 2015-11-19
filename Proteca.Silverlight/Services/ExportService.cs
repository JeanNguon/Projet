using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Proteca.Silverlight.Helpers;
using Proteca.Silverlight.Services.Contracts;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.FormatProviders.OpenXml;
using Telerik.Windows.Documents.Model;
using System.Globalization;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.UI;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.TextBased.Csv;
using Telerik.Windows.Documents.FormatProviders.Html;
using System;

namespace Proteca.Silverlight.Services
{
    /// <summary>
    /// Service d'export Excel et PDF
    /// </summary>
    [Export(typeof(IExportService))]
    public class ExportService : IExportService
    {
        #region Public Functions

        /// <summary>
        /// Lancement de l'export PDF
        /// </summary>
        /// <param name="parameter"></param>
        public void ExportPDF(object parameter)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.pdf";
            dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

            if (dialog.ShowDialog() == true)
            {
                //Récupération du DataPager pour imprimer toute la grid sans la pagination
                bool paged = ExportAndPrintHelper.GetParameterAtIndex(parameter, ExportAndPrintHelper.RadDataPagerIndex) != null && ExportAndPrintHelper.GetParameterAtIndex(parameter, ExportAndPrintHelper.RadDataPagerIndex) is RadDataPager;

                RadDataPager pager = paged ? (RadDataPager)ExportAndPrintHelper.GetParameterAtIndex(parameter, ExportAndPrintHelper.RadDataPagerIndex) : null;

                int originalPageSize = 0;
                int originalPageIndex = 0;

                if (paged)
                {
                    originalPageSize = pager.PageSize;
                    originalPageIndex = pager.PageIndex;

                    pager.PageSize = 0;
                }
                try
                {
                    RadGridView grid = ExportAndPrintHelper.InverseVisibilityColumnsToExport(parameter,
                        ExportAndPrintHelper.ColumnsToExportIndex, ExportAndPrintHelper.RadGridViewIndex);

                    RadDocument document = ExportAndPrintHelper.CreateDocument(grid);

                    PdfFormatProvider provider = new PdfFormatProvider();

                    using (Stream output = dialog.OpenFile())
                    {
                        provider.Export(document, output);
                    }
                }
                catch (OutOfMemoryException ex)
                {
                    throw ex;
                }
                catch (IOException ex)
                {
                    throw ex;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (paged)
                    {
                        pager.PageSize = originalPageSize;
                        pager.PageIndex = originalPageIndex;
                    }

                    ExportAndPrintHelper.InverseVisibilityColumnsToExport(parameter,
                        ExportAndPrintHelper.ColumnsToExportIndex, ExportAndPrintHelper.RadGridViewIndex);
                }
            }
        }

        /// <summary>
        /// Lancement de l'export Excel
        /// </summary>
        /// <param name="parameter"></param>
        public void ExportExcel(object parameter)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.xlsx";
            dialog.Filter = string.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", "xlsx", "Excel");

            if (dialog.ShowDialog() == true)
            {
                //Récupération du DataPager pour imprimer toute la grid sans la pagination
                bool paged = ExportAndPrintHelper.GetParameterAtIndex(parameter, ExportAndPrintHelper.RadDataPagerIndex) != null && ExportAndPrintHelper.GetParameterAtIndex(parameter, ExportAndPrintHelper.RadDataPagerIndex) is RadDataPager;

                RadDataPager pager = paged ? (RadDataPager)ExportAndPrintHelper.GetParameterAtIndex(parameter, ExportAndPrintHelper.RadDataPagerIndex) : null;

                int originalPageSize = 0;
                int originalPageIndex = 0;

                if (paged)
                {
                    originalPageSize = pager.PageSize;
                    originalPageIndex = pager.PageIndex;

                    pager.PageSize = 0;
                }

                try
                {

                    RadGridView grid = ExportAndPrintHelper.InverseVisibilityColumnsToExport(parameter,
                        ExportAndPrintHelper.ColumnsToExportIndex, ExportAndPrintHelper.RadGridViewIndex);

                    var workbook = ExportAndPrintHelper.CreateWorkBook(grid);

                    if (workbook != null)
                    {
                        XlsxFormatProvider provider = new XlsxFormatProvider();

                        using (Stream output = dialog.OpenFile())
                        {
                            provider.Export(workbook, output);
                        }
                    }
                }
                catch (OutOfMemoryException ex)
                {
                    throw ex;
                }
                catch (IOException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (paged)
                    {
                        pager.PageSize = originalPageSize;
                        pager.PageIndex = originalPageIndex;
                    }

                    ExportAndPrintHelper.InverseVisibilityColumnsToExport(parameter,
                        ExportAndPrintHelper.ColumnsToExportIndex, ExportAndPrintHelper.RadGridViewIndex);
                }
            }
        }

        #endregion Public Functions
    }
}
