using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EasyWinFormLibrary.CustomControls;
using Excel = Microsoft.Office.Interop.Excel;

namespace EasyWinFormLibrary.Excel
{
    /// <summary>
    /// Excel export provider for <see cref="AdvancedDataGridView"/>, backed by Office Interop.
    /// Requires Microsoft Excel to be installed on the machine running the app.
    /// </summary>
    /// <example>
    /// <code>
    /// // Once at app startup:
    /// AdvancedDataGridView.ExportProvider = new ExcelGridExportProvider();
    /// </code>
    /// </example>
    public class ExcelGridExportProvider : IGridExportProvider
    {
        private const double HeaderRowHeight = 25.0;

        public void Export(AdvancedDataGridView grid, string filePath, bool showResult)
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                object[,] data = BuildDataArray(grid, out int visibleColumnCount);

                excelApp = new Excel.Application { Visible = showResult };
                workbook = excelApp.Workbooks.Add(Type.Missing);
                worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);

                Excel.Range target = worksheet.Range["A1"].Resize[data.GetLength(0), visibleColumnCount];
                target.Value2 = data;

                FormatWorksheet(worksheet, grid, data.GetLength(0), visibleColumnCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Excel export failed: {ex.Message}", ex);
            }
            finally
            {
                if (!showResult)
                {
                    Cleanup(worksheet, workbook, excelApp);
                }
            }
        }

        private static object[,] BuildDataArray(AdvancedDataGridView grid, out int visibleColumnCount)
        {
            var visibleColumns = new System.Collections.Generic.List<DataGridViewColumn>();
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column.Visible) visibleColumns.Add(column);
            }
            visibleColumnCount = visibleColumns.Count;

            int rowCount = grid.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow);
            var data = new object[rowCount + 1, visibleColumnCount];

            for (int c = 0; c < visibleColumnCount; c++)
            {
                data[0, c] = visibleColumns[c].HeaderText;
            }

            int r2 = 1;
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                for (int c = 0; c < visibleColumnCount; c++)
                {
                    data[r2, c] = row.Cells[visibleColumns[c].Index].Value?.ToString() ?? string.Empty;
                }
                r2++;
            }

            return data;
        }

        private static void FormatWorksheet(Excel.Worksheet worksheet, AdvancedDataGridView grid, int rowCount, int columnCount)
        {
            try
            {
                worksheet.Rows[1].RowHeight = HeaderRowHeight;

                Excel.Range headerRange = worksheet.Range["A1", worksheet.Cells[1, columnCount]];
                headerRange.Interior.Color = ColorTranslator.ToOle(grid.ColumnsHeaderColor);
                headerRange.Font.Bold = true;
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                Excel.Range dataRange = worksheet.Range["A1"].Resize[rowCount, columnCount];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                if (grid.AutoFitColumns)
                {
                    for (int i = 1; i <= columnCount; i++)
                    {
                        worksheet.Columns[i].AutoFit();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excel formatting error: {ex.Message}");
            }
        }

        private static void Cleanup(Excel.Worksheet worksheet, Excel.Workbook workbook, Excel.Application application)
        {
            try
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null)
                {
                    workbook.Close(false);
                    Marshal.ReleaseComObject(workbook);
                }
                if (application != null)
                {
                    application.Quit();
                    Marshal.ReleaseComObject(application);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"COM cleanup error: {ex.Message}");
            }
        }
    }
}
