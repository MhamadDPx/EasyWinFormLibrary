using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Extension methods for DataGridView
    /// </summary>
    public static class DataGridViewExtentions
    {
        #region Enums
        /// <summary>
        /// Button types for DataGridView image columns
        /// </summary>
        public enum ButtonImage
        {
            /// <summary>Print button for printing records or reports</summary>
            Print = 0,

            /// <summary>Update button for editing or modifying existing records</summary>
            Update = 1,

            /// <summary>Delete button for removing records from the data source</summary>
            Delete = 2,

            /// <summary>Increment button for increasing a value in the record</summary>
            Increment = 3,

            /// <summary>Decrement button for decreasing a value in the record</summary>
            Decrement = 4
        }

        #endregion

        #region Public extension methods
        /// <summary>
        /// Checks if DataGridView has any rows
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>True if DataGridView has rows</returns>
        public static bool HasRow(this DataGridView dataGridView)
        {
            return dataGridView?.Rows?.Count > 0;
        }

        /// <summary>
        /// Checks if DataGridView is empty (no rows)
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>True if DataGridView is empty</returns>
        public static bool IsEmpty(this DataGridView dataGridView)
        {
            return dataGridView?.Rows?.Count == 0;
        }

        /// <summary>
        /// Gets the number of rows safely (excluding new row if present)
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>Number of rows, or 0 if null</returns>
        public static int RowCount(this DataGridView dataGridView)
        {
            if (dataGridView?.Rows == null) return 0;

            int count = dataGridView.Rows.Count;

            // Exclude the "new row" if AllowUserToAddRows is true
            if (dataGridView.AllowUserToAddRows && count > 0)
                count--;

            return count;
        }

        /// <summary>
        /// Checks if DataGridView has any selected rows
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>True if DataGridView has selected rows</returns>
        public static bool HasSelectedRow(this DataGridView dataGridView)
        {
            return dataGridView?.SelectedRows?.Count > 0;
        }

        /// <summary>
        /// Checks if DataGridView has more than specified number of rows
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <param name="count">Minimum row count</param>
        /// <returns>True if DataGridView has more rows than specified</returns>
        public static bool HasMoreThan(this DataGridView dataGridView, int count)
        {
            return dataGridView.RowCount() > count;
        }

        /// <summary>
        /// Gets the selected row safely
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>First selected row or null</returns>
        public static DataGridViewRow GetSelectedRow(this DataGridView dataGridView)
        {
            return dataGridView?.SelectedRows?.Count > 0 ? dataGridView.SelectedRows[0] : null;
        }

        /// <summary>
        /// Hides specified columns in DataGridView
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="hideIgnore">Whether to tag columns as "HideIgnore"</param>
        /// <param name="columns">Column names to hide</param>
        public static void HideColumns(this DataGridView dataGridView, bool hideIgnore, params string[] columns)
        {
            if (dataGridView?.Columns == null || columns == null) return;

            foreach (string columnName in columns)
            {
                if (string.IsNullOrEmpty(columnName)) continue;

                try
                {
                    if (dataGridView.Columns.Contains(columnName))
                    {
                        dataGridView.Columns[columnName].Visible = false;

                        if (hideIgnore)
                            dataGridView.Columns[columnName].Tag = "HideIgnore";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Column '{columnName}' not found in DataGridView");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error hiding column '{columnName}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Hides columns by index
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="hideIgnore">Whether to tag columns as "HideIgnore"</param>
        /// <param name="columnIndexes">Column indexes to hide</param>
        public static void HideColumns(this DataGridView dataGridView, bool hideIgnore, params int[] columnIndexes)
        {
            if (dataGridView?.Columns == null || columnIndexes == null) return;

            foreach (int index in columnIndexes)
            {
                try
                {
                    if (index >= 0 && index < dataGridView.Columns.Count)
                    {
                        dataGridView.Columns[index].Visible = false;

                        if (hideIgnore)
                            dataGridView.Columns[index].Tag = "HideIgnore";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Column index '{index}' is out of range");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error hiding column at index '{index}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Shows specified columns in DataGridView
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="columns">Column names to show</param>
        public static void ShowColumns(this DataGridView dataGridView, params string[] columns)
        {
            if (dataGridView?.Columns == null || columns == null) return;

            foreach (string columnName in columns)
            {
                if (string.IsNullOrEmpty(columnName)) continue;

                try
                {
                    if (dataGridView.Columns.Contains(columnName))
                    {
                        dataGridView.Columns[columnName].Visible = true;
                        dataGridView.Columns[columnName].Tag = null; // Clear tag
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error showing column '{columnName}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Shows all columns except those tagged as "HideIgnore"
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        public static void ShowAllColumns(this DataGridView dataGridView)
        {
            if (dataGridView?.Columns == null) return;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.Tag?.ToString() != "HideIgnore")
                {
                    column.Visible = true;
                }
            }
        }

        /// <summary>
        /// Hides all columns except the specified ones
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="visibleColumns">Column names to keep visible</param>
        public static void HideAllExcept(this DataGridView dataGridView, params string[] visibleColumns)
        {
            if (dataGridView?.Columns == null) return;

            var visibleSet = new HashSet<string>(visibleColumns ?? new string[0]);

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.Visible = visibleSet.Contains(column.Name);
            }
        }

        /// <summary>
        /// Gets all hidden column names
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>List of hidden column names</returns>
        public static List<string> GetHiddenColumns(this DataGridView dataGridView)
        {
            var hiddenColumns = new List<string>();

            if (dataGridView?.Columns == null) return hiddenColumns;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (!column.Visible)
                {
                    hiddenColumns.Add(column.Name);
                }
            }

            return hiddenColumns;
        }

        /// <summary>
        /// Gets all visible column names
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>List of visible column names</returns>
        public static List<string> GetVisibleColumns(this DataGridView dataGridView)
        {
            var visibleColumns = new List<string>();

            if (dataGridView?.Columns == null) return visibleColumns;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.Visible)
                {
                    visibleColumns.Add(column.Name);
                }
            }

            return visibleColumns;
        }

        /// <summary>
        /// Toggles visibility of specified columns
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="columns">Column names to toggle</param>
        public static void ToggleColumns(this DataGridView dataGridView, params string[] columns)
        {
            if (dataGridView?.Columns == null || columns == null) return;

            foreach (string columnName in columns)
            {
                if (string.IsNullOrEmpty(columnName)) continue;

                try
                {
                    if (dataGridView.Columns.Contains(columnName))
                    {
                        dataGridView.Columns[columnName].Visible = !dataGridView.Columns[columnName].Visible;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error toggling column '{columnName}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sets column widths for specified columns
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="columnWidths">Dictionary of column names and their widths</param>
        public static void SetColumnWidths(this DataGridView dataGridView, Dictionary<string, int> columnWidths)
        {
            if (dataGridView?.Columns == null || columnWidths == null) return;

            foreach (var kvp in columnWidths)
            {
                try
                {
                    if (dataGridView.Columns.Contains(kvp.Key))
                    {
                        dataGridView.Columns[kvp.Key].Width = kvp.Value;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error setting width for column '{kvp.Key}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Auto-sizes all visible columns
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="mode">The auto size mode</param>
        public static void AutoSizeVisibleColumns(this DataGridView dataGridView, DataGridViewAutoSizeColumnsMode mode = DataGridViewAutoSizeColumnsMode.AllCells)
        {
            if (dataGridView?.Columns == null) return;

            try
            {
                dataGridView.AutoSizeColumnsMode = mode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error auto-sizing columns: {ex.Message}");
            }
        }
        #endregion

        #region Column Creation Methods

        /// <summary>
        /// Creates a DataGridView text box column
        /// </summary>
        public static DataGridViewTextBoxColumn CreateTextColumn(
            string columnName = "txtColumn",
            string headerText = null,
            bool readOnly = false,
            Type valueType = null,
            DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.Fill)
        {
            return new DataGridViewTextBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                AutoSizeMode = autoSizeMode,
                ValueType = valueType ?? typeof(string)
            };
        }

        /// <summary>
        /// Creates a DataGridView checkbox column
        /// </summary>
        public static DataGridViewCheckBoxColumn CreateCheckBoxColumn(
            string columnName = "checkColumn",
            string headerText = null,
            bool readOnly = false)
        {
            return new DataGridViewCheckBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            };
        }

        /// <summary>
        /// Creates a DataGridView image button column
        /// </summary>
        public static DataGridViewImageColumn CreateButtonColumn(
            string columnName = "btn",
            Image backImage = null,
            int width = 30)
        {
            var column = new DataGridViewImageColumn()
            {
                Name = columnName,
                HeaderText = "",
                Image = backImage,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = width,
                ImageLayout = DataGridViewImageCellLayout.Normal
            };

            // Add padding to the cell style for spacing
            column.DefaultCellStyle.Padding = new Padding(2, 2, 2, 2);
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            return column;
        }

        /// <summary>
        /// Creates a DataGridView combobox column
        /// </summary>
        public static DataGridViewComboBoxColumn CreateComboBoxColumn(
            string columnName = "comboColumn",
            string headerText = null,
            DataTable dataSource = null,
            string displayMember = "",
            string valueMember = "",
            bool readOnly = false)
        {
            return new DataGridViewComboBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                FlatStyle = FlatStyle.Flat,
                DataSource = dataSource,
                DisplayMember = displayMember,
                ValueMember = valueMember,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
        }

        /// <summary>
        /// Creates a DataGridView number column with formatting
        /// </summary>
        public static DataGridViewTextBoxColumn CreateNumberColumn(
            string columnName = "numberColumn",
            string headerText = null,
            bool readOnly = false,
            int? roundNumber = null,
            bool isPercentage = false)
        {
            var column = new DataGridViewTextBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                ValueType = isPercentage ? typeof(decimal) : typeof(double),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            // Set number formatting
            int digits = roundNumber ?? LibrarySettings.NumberDefaultRound;
            column.DefaultCellStyle.Format = isPercentage ? $"p{digits}" : $"n{digits}";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            return column;
        }

        /// <summary>
        /// Creates a DataGridView date column with formatting
        /// </summary>
        public static DataGridViewTextBoxColumn CreateDateColumn(
            string columnName = "dateColumn",
            string headerText = null,
            bool readOnly = false,
            string dateFormat = "yyyy-MM-dd")
        {
            return new DataGridViewTextBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                ValueType = typeof(DateTime),
                DefaultCellStyle = { Format = dateFormat },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };
        }

        #endregion

        #region Extension Methods for Adding Columns

        /// <summary>
        /// Adds an edit button column to DataGridView
        /// </summary>
        public static void AddEditColumn(this DataGridView dgv, string columnName = "editBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Update), width));
        }

        /// <summary>
        /// Adds a delete button column to DataGridView
        /// </summary>
        public static void AddDeleteColumn(this DataGridView dgv, string columnName = "deleteBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Delete), width));
        }

        /// <summary>
        /// Adds a print button column to DataGridView
        /// </summary>
        public static void AddPrintColumn(this DataGridView dgv, string columnName = "printBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Print), width));
        }

        /// <summary>
        /// Adds a increment button column to DataGridView
        /// </summary>
        public static void AddIncerementColumn(this DataGridView dgv, string columnName = "incrementBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Print), width));
        }

        /// <summary>
        /// Adds a decrement button column to DataGridView
        /// </summary>
        public static void AddDecrementColumn(this DataGridView dgv, string columnName = "decrementBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Print), width));
        }

        /// <summary>
        /// Adds multiple action buttons at once
        /// </summary>
        public static void AddActionColumns(this DataGridView dgv, params ButtonImage[] buttons)
        {
            if (dgv == null || buttons == null) return;

            foreach (var button in buttons)
            {
                string columnName = $"{button.ToString().ToLower()}Btn";
                dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(button), 30));
            }
        }

        #endregion

        #region Formatting Methods

        /// <summary>
        /// Formats a column as number
        /// </summary>
        public static void FormatAsNumber(this DataGridView dgv, string columnName, int? roundNumber = null)
        {
            if (dgv?.Columns[columnName] == null) return;

            int digits = roundNumber ?? LibrarySettings.NumberDefaultRound;
            dgv.Columns[columnName].DefaultCellStyle.Format = $"n{digits}";
            dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// Formats a column as percentage
        /// </summary>
        public static void FormatAsPercentage(this DataGridView dgv, string columnName, int? roundNumber = null)
        {
            if (dgv?.Columns[columnName] == null) return;

            int digits = roundNumber ?? LibrarySettings.NumberDefaultRound;
            dgv.Columns[columnName].DefaultCellStyle.Format = $"p{digits}";
            dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// Formats a column as currency
        /// </summary>
        public static void FormatAsCurrency(this DataGridView dgv, string columnName, int? roundNumber = null)
        {
            if (dgv?.Columns[columnName] == null) return;

            int digits = roundNumber ?? LibrarySettings.NumberDefaultRound;
            dgv.Columns[columnName].DefaultCellStyle.Format = $"c{digits}";
            dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// Formats a column as date
        /// </summary>
        public static void FormatAsDate(this DataGridView dgv, string columnName, string dateFormat = "yyyy-MM-dd")
        {
            if (dgv?.Columns[columnName] == null) return;

            dgv.Columns[columnName].DefaultCellStyle.Format = dateFormat;
            dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        #endregion

        #region Calculation Methods

        /// <summary>
        /// Performs calculations on DataGridView data
        /// </summary>
        public static double Compute(this DataGridView dgv, string expression, string filter = "")
        {
            if (dgv?.DataSource == null || dgv.Rows.Count <= 0)
                return 0;

            try
            {
                var dataTable = dgv.DataSource as DataTable;
                if (dataTable == null) return 0;

                var result = dataTable.Compute(expression, filter);
                if (double.TryParse(result?.ToString(), out double value))
                    return value;

                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Compute: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Calculates sum of a column
        /// </summary>
        public static double Sum(this DataGridView dgv, string columnName, string filter = "")
        {
            return dgv.Compute($"SUM({columnName})", filter);
        }

        /// <summary>
        /// Calculates average of a column
        /// </summary>
        public static double Average(this DataGridView dgv, string columnName, string filter = "")
        {
            return dgv.Compute($"AVG({columnName})", filter);
        }

        /// <summary>
        /// Calculates count of rows
        /// </summary>
        public static int Count(this DataGridView dgv, string filter = "")
        {
            return (int)dgv.Compute("COUNT(*)", filter);
        }

        /// <summary>
        /// Gets minimum value of a column
        /// </summary>
        public static double Min(this DataGridView dgv, string columnName, string filter = "")
        {
            return dgv.Compute($"MIN({columnName})", filter);
        }

        /// <summary>
        /// Gets maximum value of a column
        /// </summary>
        public static double Max(this DataGridView dgv, string columnName, string filter = "")
        {
            return dgv.Compute($"MAX({columnName})", filter);
        }

        #endregion

        #region Styling Methods

        /// <summary>
        /// Sets alternating row colors
        /// </summary>
        public static void SetAlternatingRowColors(this DataGridView dgv,
            Color evenRowColor, Color oddRowColor)
        {
            if (dgv == null) return;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = oddRowColor;
            dgv.RowsDefaultCellStyle.BackColor = evenRowColor;
        }

        /// <summary>
        /// Applies a modern style to DataGridView
        /// </summary>
        public static void ApplyModernStyle(this DataGridView dgv)
        {
            if (dgv == null) return;

            // Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 35;

            // Row style
            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgv.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgv.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Alternating rows
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            // Grid appearance
            dgv.GridColor = Color.FromArgb(224, 224, 224);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowTemplate.Height = 30;

            // Enable double buffering for smoother scrolling
            dgv.DoubleBuffered(true);
        }

        /// <summary>
        /// Sets header text alignment for all columns
        /// </summary>
        public static void SetHeaderAlignment(this DataGridView dgv, DataGridViewContentAlignment alignment)
        {
            if (dgv == null) return;

            dgv.ColumnHeadersDefaultCellStyle.Alignment = alignment;
        }

        /// <summary>
        /// Sets font for all cells
        /// </summary>
        public static void SetFont(this DataGridView dgv, Font font)
        {
            if (dgv == null || font == null) return;

            dgv.Font = font;
            dgv.RowsDefaultCellStyle.Font = font;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(font, FontStyle.Bold);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets selected row data as dictionary
        /// </summary>
        public static Dictionary<string, object> GetSelectedRowData(this DataGridView dgv)
        {
            if (dgv?.SelectedRows.Count == 0) return null;

            try
            {
                var selectedRow = dgv.SelectedRows[0];
                var data = new Dictionary<string, object>();

                foreach (DataGridViewCell cell in selectedRow.Cells)
                {
                    data[cell.OwningColumn.Name] = cell.Value;
                }

                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting selected row data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Filters DataGridView based on column value
        /// </summary>
        public static void Filter(this DataGridView dgv, string columnName, object value, string operation = "=")
        {
            if (dgv?.DataSource == null) return;

            try
            {
                var dataTable = dgv.DataSource as DataTable;
                if (dataTable == null) return;

                string filter = value == null
                    ? $"{columnName} IS NULL"
                    : $"{columnName} {operation} '{value}'";

                dataTable.DefaultView.RowFilter = filter;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering DataGridView: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all filters from DataGridView
        /// </summary>
        public static void ClearFilter(this DataGridView dgv)
        {
            if (dgv?.DataSource == null) return;

            try
            {
                var dataTable = dgv.DataSource as DataTable;
                if (dataTable != null)
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing filter: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes DataGridView data
        /// </summary>
        public static void RefreshData(this DataGridView dgv)
        {
            if (dgv == null) return;

            try
            {
                dgv.Refresh();
                dgv.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing DataGridView: {ex.Message}");
            }
        }

        /// <summary>
        /// Selects row by column value
        /// </summary>
        public static bool SelectRowByValue(this DataGridView dgv, string columnName, object value)
        {
            if (dgv?.Rows == null) return false;

            try
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells[columnName]?.Value?.Equals(value) == true)
                    {
                        row.Selected = true;
                        dgv.FirstDisplayedScrollingRowIndex = row.Index;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting row: {ex.Message}");
            }

            return false;
        }

        #endregion

        #region Base64 Images

        private const string PrintImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABmJLR0QA/wD/AP+gvaeTAAACkElEQVRIDdWWS2hTQRSG/0luoQ+KdVlcSFHwCT7wUVfdduPCtNStJLoouMquIm5cCEI3IkWsScGNglofiCLduJBCISgoBrqSCio+FqWQdNHkjt/c5nVvU5piNob/3POfM2fOmXtmMomRwp9ZJY9YmVuSOSnZrvDoplaRkYWy/MtjmsnDazA1BnmuZG9JZhHaz0DeSqvwFmB7WNB+ApfKWjkwpke1eR7OGkrSaYx+Vj6TUDYJbxlPlHpI8HmjvmPoeSRALHhWHqx4h6NG5rvT2xHe+JuLN7J9TlclVKDqbKf+/wvQOmlWqWH6f5bWuA06A1+BO0G1BhK5/esl+h3y0Sr2YlTTbwy7fxHHNNJ2sNCU24OJNmX+Q7IrHPHr5CM33wxpwsMYQNqBO+eUueES0ZUE+hCyx7UoqIbxjzDvyyoPezL7uGrmSNaJyL2B020Qezyu2K/oakMFrGyBb3EPQWtGWttOVeZ0MKeDOQWkBwkQKkDyT3gH2aybCWWuwlsGR32SImmuCi5JdxOvT3UFfCg5eUbwWMkTMZmBiDtk+rJfRpXNhZx1o+wKuPv7cN1XZyQfZ1XJumcjIyaLN4U0w2ePtqTp/TNGu5EQOA2vOde/Q86IQUwu4qqaBV9Kewndm+PcHqR3Q6x2r2QGqxFGfje+nVW7Qf8cUfZag91AzUty3S7Jezumu189NzKizBL6Pj0fMZA6zBB8Q4uIWcTftIAvkxtV5hXjAYICAWvyKMmfjMs8iA6VFS9GfZvZoQIxxVbYD1lpl5tQ+QHPO76VVOewJ8uNsaECRa0udKnzBwEX2JchtEVaQZyg3ciS1fIHdA20s8YD8lSXjvrypzBOIW4iakuUiJjnb8t45a0x1/EXnUCnp/yWJpEAAAAASUVORK5CYII=";
        private const string EditImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABmJLR0QA/wD/AP+gvaeTAAACQElEQVRIDd1Uv2sUURD+ZvcOjCJopUgIVikMevo3iAE7IeySKIiVFv6KFhZJYyNqIUYUJYIREyHcXhpBLVQEe3/cLhaXTpFoaSV3OW53/GaNOVclub1Uyd58b9699+abebMzC6z3R9Z8gdkPu+G6g1D+kvglRg58wh/P2hwE4XlyXSM2ESYNDmfglx5Qp+KkYzdDORyj2QTxFNBexE4f56+J27BbcWLSnYNKVIbgChx5DMwPw9+/gJG9X+DiFEl70pRxYpLfgUWu6psxs/4dnpekcxuazq+Uiy6vFWy9Yxi5RS6MXPUzVMdRiTbjsp7EngrJk5vkqqOVvKJOpXMHGfLaCfh+jCA0knEMRAr09/BGQxA993cl2aGVUa5eJ5kSdxk1I106riooR3e4bnuKSji6tLOsVn8HRi5yiRb34O07DRFGy38m998V4OpOmxIz8EoT1DnEyIPQostGbhSTb4uMfI74b+R2ZGWsF/J/34G1v+XcShHzZzM5DwIX2woPeXVWC8Y6yXm7ImiFJ7WtWFz8BsgLoOalpWjrBiPX/kd0eIxdbORXbXk1ZPug2ThE8i38ttQz5PZCUZwlsUV+oZPIfzvOpiiRw9xoEEcRRDeoASPfTnIgN7nZZ28gGISDZ4jlKzvyIhtnF7uzyINGbmnJWedA28FcdQAJ+pDoe7iyA4k02LXDJK8To0zLLerc0nYQy0HmmARyhE4W+CmeRks/oilTOF76wY2upO1ACs8hLYepeYOhUpXVol0xbjijn4445rWEMCyKAAAAAElFTkSuQmCC";
        private const string DeleteImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABmJLR0QA/wD/AP+gvaeTAAACaklEQVRIDdVUv2sUURCeebs5wULFH1wsRAnRJijepbE8vIMkZRIuXdKIvZaCRRprEfQPUFLluE1lEriLgoLaeOlSREhhk5CIEpQ02X3j997tXrLunrsgFllm3ryZ+ebNe/NmH9FJ/zjPAZYqt8+Ro55o4gkmEWFaDogfz7Q/72fFqyzAYr3uiKNWhOgekbQg2yx03xVZfVupuFnxmQDnx9Y4FrlDTNPT7Y6HOTWrpRYTN767P8egvwb3pV6JmndLs8z8EkgG/wuhgjQ3udZZMIu4ZjDsav4YKHmEnc6S0EXYnoJzE7N6iNvZQxkXlOZPfQOXquUXXrUMYF9IqgMx38DP/3QmL5llG6ALi/WRAmQuWp4YPgXgeZx8GzJGyQRCBsSF/dPFCOnVSnPY3WakN2vlL8YW6b8Ozxosk7Kbi8xWJhJoVjvGI75cNtKwEA9BXgdbQpsOhzarFwKyWB04NtYaw0GF8khoewIS5dugI0f/mahuAnK6sceRyQQDIUhz7gToPIvVPpvyHl+fEgmCM0O7QASieBAyF4logw3o0rVE9yUSzDQaAVbdQ0fYXWGeTWJPuxvGxvCJBMaLX9kcNX8CtndgYkx4jFMTEP4FOP5fArzHO2hDU1e7GzzRW5hsgCPaCG1dXWgQ+ESLGmfvLTJKjxmdJFKcnycF1lPt9VfwGYYgmlrrjNgJBvgVvaci7ix/iXAHXxE7cPNd6QrkX6n84dZVAFxWYmIwjZMbV7vaIfGqS3KA59vzaqOtrjV99AMawwt6IL6/kobAZtPMRHh7xon4GYJvpCMiK2+y6AeTb9ZTE0Sokyt/A0xqufQlMXcxAAAAAElFTkSuQmCC";
        private const string IncrementImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABmJLR0QA/wD/AP+gvaeTAAABTklEQVRIDe1VO07EMBB9ToCl5wK0u0lDwQ1o+ZaAhDgBfZIt9wKcgE+DFk5BAUIgIaGVOAAdV4D18BJtdo3ljY2EBEWsGXvefKOxNQH+0+pmkvb6svaTb4pCnZNcNiOFJ6XxkBayFxoXVCApZIsJb8gd8qIIhkkm+5S95C3Qy2UbgmtmWiLXFEPhgraDWjHvbCzQLWRHAUMGm8kJK4ppO08zOazQnI0+bkuay64AV7S6klM9pTGTHI8G6nKqMQTaDDQRy5fCy7wn7JBD6EMDG68DdWs7L9iKEsefWNYKL+xzVGJe6opSWMX39Ub4TgbvSNMxrmRrUxZ2Qj7RIxrOYCwGnrAtp4bKKbKwU/9ryraAt5Vti9oWeTvgdQh7RcJpY6XirBJL5YRBBaIYz4wek2sSDsDHGjSdnFlN5pkt6cs6NKofPoNGHHR3M+sfSl/0jEmnpsiyJAAAAABJRU5ErkJggg==";
        private const string DecrementImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABmJLR0QA/wD/AP+gvaeTAAABVklEQVRIDe1UPUsDQRCd2QhikUr8Bzb5KlIEJKilIAiaf3AaW/9RkvsVfhUWggQLwcKc/goLUUhC9MY34oXj2NwuGLs73mNndt6+Yee4I/rnh339Jai1yMgusWGKacjhaOhz1quBBI0GmfgRhiVQIcjb3Hu51ySPJq84r5m4iTgxR0i4Bbc0cNGvAcMw6yQYVXbPkvs1sBz03SoaOCdVjKgYkXMCToH1ZyenlS36Mj38INZ+HcpYN8A0XpG8gYopsZxx//lGkzRX0sk8jkufOLCJfBVchHUUlFhoRsJTDbK0fgc8GD0Qx3sQf4AuTIjkkAfRnU1oHVEilKC6Q4bPkZdBGyYYY4f70ZWtqHu5DVQgJ7VtrBdgtsmYhDocRteoLYR1RGn1z9WZ9rH3DiYYE/GRy1zFzhuoSCnH9TZe/CViQ2IOOHy6RbxcSLdSl261uVzXP7p9A02wSis5DEoPAAAAAElFTkSuQmCC";

        /// <summary>
        /// List of base64 encoded images for buttons
        /// </summary>
        public static string[] ImagesList = {
            PrintImageBase64,
            EditImageBase64,
            DeleteImageBase64,
            IncrementImageBase64,
            DecrementImageBase64
        };

        #endregion

        #region Image Helper Methods

        private static Image GetIcon(string imageBase64)
        {
            try
            {
                if (string.IsNullOrEmpty(imageBase64)) return null;

                byte[] imageBytes = Convert.FromBase64String(imageBase64);
                using (var ms = new MemoryStream(imageBytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading icon: {ex.Message}");
                return null;
            }
        }

        private static Image GetButtonImage(ButtonImage buttonImage)
        {
            try
            {
                int index = (int)buttonImage;
                if (index >= 0 && index < ImagesList.Length)
                {
                    return GetIcon(ImagesList[index]);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting button image: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}
