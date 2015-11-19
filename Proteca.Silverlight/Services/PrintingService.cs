using System.Windows;
using Proteca.Silverlight.Services.Contracts;
using System.Windows.Printing;
using System.ComponentModel.Composition;
using Telerik.Windows.Controls;
using Proteca.Silverlight.Helpers;
using System.Windows.Controls;
using System;

namespace Proteca.Silverlight.Services
{
    [Export(typeof(IPrintingService))]
    public class PrintingService : IPrintingService
    {
        #region Methods
        
        public void Print(string documentName)
        {
            var printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDocPrintPage;
            printDoc.Print(documentName);
        }

        void PrintDocPrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = ((MainPage)Application.Current.RootVisual).ContentFrame;// Application.Current.RootVisual;
        }

        /// <summary>
        /// Lancement de l'impression
        /// </summary>
        /// <param name="parameter"></param>
        public void PrintGrid(object parameter)
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
                RadRichTextBox rtb = null;

                Grid parent = grid.ParentOfType<Grid>();

                if (parent != null)
                    rtb = (RadRichTextBox)ExportAndPrintHelper.GetParameterAtIndex(parameter, ExportAndPrintHelper.RichTextBoxIndex);

                if (rtb != null)
                {
                    rtb.Document = ExportAndPrintHelper.CreateDocument(grid);
                    rtb.PrintPreview();
                }
            }
            catch (OutOfMemoryException ex)
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

        #endregion
    }
}
