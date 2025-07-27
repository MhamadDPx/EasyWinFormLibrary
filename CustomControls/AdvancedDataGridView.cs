using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using XlBorderWeight = Microsoft.Office.Interop.Excel.XlBorderWeight;
using XlFixedFormatType = Microsoft.Office.Interop.Excel.XlFixedFormatType;
using XlLineStyle = Microsoft.Office.Interop.Excel.XlLineStyle;
using XlPageOrientation = Microsoft.Office.Interop.Excel.XlPageOrientation;
using XlPaperSize = Microsoft.Office.Interop.Excel.XlPaperSize;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced DataGridView control with enhanced features including sorting, grouping, exporting,
    /// column management, and multi-language support. Optimized for .NET Framework 4.8 with
    /// improved performance, error handling, and extensibility.
    /// </summary>
    [ToolboxItem(true)]
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
    public class AdvancedDataGridView : DataGridView
    {
        #region Private Fields

        private Dictionary<string, string> _columnsType = new Dictionary<string, string>();
        private ToolStripMenuItem _columnsMenuStrip;
        private FlowLayoutPanel _flowSelectedColumn;
        private string[] _columnsToSkip;
        private string[] _defaultSelectedColumns;
        private bool _allowHideColumns = true;
        private ContextMenuStrip _contextMenu = new ContextMenuStrip();
        private Random _random = new Random();
        private string _groupByColumn = "";
        private Dictionary<object, Color> _groupColors = new Dictionary<object, Color>();
        private bool _isDataSourceEmpty = true;
        private string _exportPath = "";
        private ExportFormat _defaultExportFormat = ExportFormat.Excel;
        private bool _enableGrouping = true;
        private bool _enableExporting = true;
        private bool _enableSorting = true;
        private bool _autoFitColumns = true;
        private Color _headerBackColor = Color.FromArgb(186, 198, 220);
        private Color _alternatingRowColor = Color.FromArgb(240, 240, 240);
        private bool _enableAlternatingRowColors = false;

        #endregion

        #region Enums

        /// <summary>
        /// Supported export formats
        /// </summary>
        public enum ExportFormat
        {
            /// <summary>Microsoft Excel format</summary>
            Excel,
            /// <summary>PDF format</summary>
            PDF,
            /// <summary>CSV format</summary>
            CSV
        }

        /// <summary>
        /// Supported languages for UI elements
        /// </summary>
        public enum SupportedLanguage
        {
            /// <summary>English language</summary>
            English,
            /// <summary>Arabic language</summary>
            Arabic,
            /// <summary>Kurdish language</summary>
            Kurdish
        }

        #endregion

        #region Events

        /// <summary>
        /// Event fired before data export begins
        /// </summary>
        public event EventHandler<ExportEventArgs> BeforeExport;

        /// <summary>
        /// Event fired after data export completes
        /// </summary>
        public event EventHandler<ExportEventArgs> AfterExport;

        /// <summary>
        /// Event fired when grouping changes
        /// </summary>
        public event EventHandler<GroupingEventArgs> GroupingChanged;

        /// <summary>
        /// Event fired when column visibility changes
        /// </summary>
        public event EventHandler<ColumnVisibilityEventArgs> ColumnVisibilityChanged;

        #endregion

        #region Event Args

        /// <summary>
        /// Event arguments for export operations
        /// </summary>
        public class ExportEventArgs : EventArgs
        {
            public ExportFormat Format { get; set; }
            public string FilePath { get; set; }
            public bool Cancel { get; set; }
        }

        /// <summary>
        /// Event arguments for grouping operations
        /// </summary>
        public class GroupingEventArgs : EventArgs
        {
            public string ColumnName { get; set; }
            public bool IsGrouped { get; set; }
        }

        /// <summary>
        /// Event arguments for column visibility changes
        /// </summary>
        public class ColumnVisibilityEventArgs : EventArgs
        {
            public string ColumnName { get; set; }
            public bool IsVisible { get; set; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the header background color
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Background color for column headers")]
        public Color ColumnsHeaderColor
        {
            get { return _headerBackColor; }
            set
            {
                _headerBackColor = value;
                this.ColumnHeadersDefaultCellStyle.BackColor = value;
                this.ColumnHeadersDefaultCellStyle.SelectionBackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the column types for search functionality
        /// </summary>
        [Category("Advanced Data")]
        [Description("Dictionary mapping column names to their data types")]
        [Browsable(false)]
        public Dictionary<string, string> ColumnsType
        {
            get { return _columnsType; }
            set { _columnsType = value ?? new Dictionary<string, string>(); }
        }

        /// <summary>
        /// Gets or sets the menu strip for column selection
        /// </summary>
        [Category("Advanced UI")]
        [Description("Menu strip for column selection")]
        public ToolStripMenuItem ColumnsMenuStrip
        {
            get { return _columnsMenuStrip; }
            set { _columnsMenuStrip = value; }
        }

        /// <summary>
        /// Gets or sets the flow panel for selected columns display
        /// </summary>
        [Category("Advanced UI")]
        [Description("Flow layout panel to display selected columns")]
        public FlowLayoutPanel FlowSelectedColumn
        {
            get { return _flowSelectedColumn; }
            set { _flowSelectedColumn = value; }
        }

        /// <summary>
        /// Gets or sets columns to skip from context menu operations
        /// </summary>
        [Category("Advanced Behavior")]
        [Description("Array of column names to skip from context menu operations")]
        public string[] ColumnsToSkip
        {
            get { return _columnsToSkip ?? new string[0]; }
            set { _columnsToSkip = value; }
        }

        /// <summary>
        /// Gets or sets default selected columns
        /// </summary>
        [Category("Advanced Behavior")]
        [Description("Array of column names that should be selected by default")]
        public string[] DefaultSelectedColumns
        {
            get { return _defaultSelectedColumns ?? new string[0]; }
            set { _defaultSelectedColumns = value; }
        }

        /// <summary>
        /// Gets or sets whether columns can be hidden
        /// </summary>
        [Category("Advanced Behavior")]
        [Description("Enables or disables column hiding functionality")]
        [DefaultValue(true)]
        public bool AllowHideColumns
        {
            get { return _allowHideColumns; }
            set { _allowHideColumns = value; }
        }

        /// <summary>
        /// Gets or sets whether grouping is enabled
        /// </summary>
        [Category("Advanced Behavior")]
        [Description("Enables or disables grouping functionality")]
        [DefaultValue(true)]
        public bool EnableGrouping
        {
            get { return _enableGrouping; }
            set { _enableGrouping = value; }
        }

        /// <summary>
        /// Gets or sets whether exporting is enabled
        /// </summary>
        [Category("Advanced Behavior")]
        [Description("Enables or disables export functionality")]
        [DefaultValue(true)]
        public bool EnableExporting
        {
            get { return _enableExporting; }
            set { _enableExporting = value; }
        }

        /// <summary>
        /// Gets or sets whether sorting is enabled
        /// </summary>
        [Category("Advanced Behavior")]
        [Description("Enables or disables sorting functionality")]
        [DefaultValue(true)]
        public bool EnableSorting
        {
            get { return _enableSorting; }
            set { _enableSorting = value; }
        }

        /// <summary>
        /// Gets or sets the default export format
        /// </summary>
        [Category("Advanced Export")]
        [Description("Default format for export operations")]
        [DefaultValue(ExportFormat.Excel)]
        public ExportFormat DefaultExportFormat
        {
            get { return _defaultExportFormat; }
            set { _defaultExportFormat = value; }
        }

        /// <summary>
        /// Gets or sets the export file path
        /// </summary>
        [Category("Advanced Export")]
        [Description("Default path for exported files")]
        public string ExportPath
        {
            get { return string.IsNullOrEmpty(_exportPath) ? System.Windows.Forms.Application.StartupPath : _exportPath; }
            set { _exportPath = value; }
        }

        /// <summary>
        /// Gets or sets whether to auto-fit columns after export operations
        /// </summary>
        [Category("Advanced Export")]
        [Description("Automatically fit columns in exported files")]
        [DefaultValue(true)]
        public bool AutoFitColumns
        {
            get { return _autoFitColumns; }
            set { _autoFitColumns = value; }
        }

        /// <summary>
        /// Gets or sets whether to enable alternating row colors
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Enable alternating row background colors")]
        [DefaultValue(false)]
        public bool EnableAlternatingRowColors
        {
            get { return _enableAlternatingRowColors; }
            set
            {
                _enableAlternatingRowColors = value;
                this.AlternatingRowsDefaultCellStyle.BackColor = value ? _alternatingRowColor : this.DefaultCellStyle.BackColor;
            }
        }

        /// <summary>
        /// Gets or sets the alternating row color
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Background color for alternating rows")]
        public Color AlternatingRowColor
        {
            get { return _alternatingRowColor; }
            set
            {
                _alternatingRowColor = value;
                if (_enableAlternatingRowColors)
                    this.AlternatingRowsDefaultCellStyle.BackColor = value;
            }
        }

        /// <summary>
        /// Gets whether the data source is empty
        /// </summary>
        [Browsable(false)]
        public bool IsDataSourceEmpty
        {
            get { return _isDataSourceEmpty; }
        }

        /// <summary>
        /// Gets the current grouping column
        /// </summary>
        [Browsable(false)]
        public string GroupByColumn
        {
            get { return _groupByColumn; }
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Delegate for resizing DataGridView headers
        /// </summary>
        public delegate void ResizeDGVHeaderDelegate();

        /// <summary>
        /// Delegate for adding captions to DataGridView
        /// </summary>
        public delegate void AddCaptionToDGVDelegate();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedDataGridView class
        /// </summary>
        public AdvancedDataGridView()
        {
            InitializeComponent();
            InitializeContextMenu();
            SetDefaultStyles();
        }

        /// <summary>
        /// Initializes the component with default settings
        /// </summary>
        private void InitializeComponent()
        {
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.ReadOnly = true;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.MultiSelect = false;
            this.AutoGenerateColumns = true;
            this.EnableHeadersVisualStyles = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RowHeadersVisible = false;
        }

        /// <summary>
        /// Sets default visual styles
        /// </summary>
        private void SetDefaultStyles()
        {
            this.ColumnHeadersDefaultCellStyle.BackColor = _headerBackColor;
            this.ColumnHeadersDefaultCellStyle.SelectionBackColor = _headerBackColor;
            this.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(this.Font, FontStyle.Bold);
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (_enableAlternatingRowColors)
                this.AlternatingRowsDefaultCellStyle.BackColor = _alternatingRowColor;
        }

        /// <summary>
        /// Initializes the context menu
        /// </summary>
        private void InitializeContextMenu()
        {
            _contextMenu.Opening += OnContextMenuOpening;
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Handles mouse enter events for cells
        /// </summary>
        protected override void OnCellMouseEnter(DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;

            base.OnCellMouseEnter(e);
            this.Cursor = this.Columns[e.ColumnIndex] is DataGridViewImageColumn ? Cursors.Hand : Cursors.Default;
        }

        /// <summary>
        /// Handles mouse click events for cells
        /// </summary>
        protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseClick(e);

            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                this.CurrentCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                ShowContextMenu(e);
            }
        }

        /// <summary>
        /// Handles cell formatting for grouping
        /// </summary>
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs args)
        {
            base.OnCellFormatting(args);

            if (string.IsNullOrEmpty(_groupByColumn) || args.RowIndex < 0) return;

            ApplyGroupingFormat(args);
        }

        #endregion

        #region Context Menu Methods

        /// <summary>
        /// Handles context menu opening event
        /// </summary>
        private void OnContextMenuOpening(object sender, CancelEventArgs e)
        {
            if (this.CurrentCell == null) return;

            string columnName = this.Columns[this.CurrentCell.ColumnIndex].Name;
            if (!(this.Columns[this.CurrentCell.ColumnIndex] is DataGridViewTextBoxColumn))
            {
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Shows the context menu with appropriate options
        /// </summary>
        private void ShowContextMenu(DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) return;

            string columnName = this.Columns[e.ColumnIndex].Name;
            string columnHeaderText = this.Columns[e.ColumnIndex].HeaderText;

            if (!(this.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn)) return;

            BuildContextMenu(columnName, columnHeaderText);
            _contextMenu.Show(MousePosition);
        }

        /// <summary>
        /// Builds the context menu with available options
        /// </summary>
        private void BuildContextMenu(string columnName, string columnHeaderText)
        {
            _contextMenu.Items.Clear();

            // Sorting options
            if (_enableSorting)
            {
                AddSortingMenuItems(columnName);
                _contextMenu.Items.Add(new ToolStripSeparator());
            }

            // Column management options
            if (_allowHideColumns)
            {
                AddColumnManagementMenuItems(columnName, columnHeaderText);
                _contextMenu.Items.Add(new ToolStripSeparator());
            }

            // Export options
            if (_enableExporting)
            {
                AddExportMenuItems();
                _contextMenu.Items.Add(new ToolStripSeparator());
            }

            // Grouping options
            if (_enableGrouping)
            {
                AddGroupingMenuItems(columnName, columnHeaderText);
                _contextMenu.Items.Add(new ToolStripSeparator());
            }

            // Column sizing options
            AddColumnSizingMenuItems();
        }

        /// <summary>
        /// Adds sorting menu items
        /// </summary>
        private void AddSortingMenuItems(string columnName)
        {
            var sortAscItem = new ToolStripMenuItem("Sort A to Z", null, (s, e) => SortColumn(columnName, ListSortDirection.Ascending))
            {
                Name = "sortAsc",
                Tag = columnName
            };

            var sortDescItem = new ToolStripMenuItem("Sort Z to A", null, (s, e) => SortColumn(columnName, ListSortDirection.Descending))
            {
                Name = "sortDesc",
                Tag = columnName
            };

            _contextMenu.Items.AddRange(new ToolStripItem[] { sortAscItem, sortDescItem });
        }

        /// <summary>
        /// Adds column management menu items
        /// </summary>
        private void AddColumnManagementMenuItems(string columnName, string columnHeaderText)
        {
            var hideColumnItem = new ToolStripMenuItem($"Hide ({columnHeaderText}) Column", null, (s, e) => HideColumn(columnName))
            {
                Name = "hideColumn",
                Tag = columnName
            };

            _contextMenu.Items.Add(hideColumnItem);

            // Add hidden columns submenu if there are hidden columns
            var hiddenColumnsItem = CreateHiddenColumnsMenu();
            if (hiddenColumnsItem != null)
            {
                _contextMenu.Items.Add(hiddenColumnsItem);
            }
        }

        /// <summary>
        /// Adds export menu items
        /// </summary>
        private void AddExportMenuItems()
        {
            var exportToExcelItem = new ToolStripMenuItem("Export to Excel", null, async (s, e) => await ExportDataAsync(ExportFormat.Excel))
            {
                Name = "exportExcel"
            };

            var exportToPdfItem = new ToolStripMenuItem("Export to PDF", null, async (s, e) => await ExportDataAsync(ExportFormat.PDF))
            {
                Name = "exportPdf"
            };

            var exportToCsvItem = new ToolStripMenuItem("Export to CSV", null, async (s, e) => await ExportDataAsync(ExportFormat.CSV))
            {
                Name = "exportCsv"
            };

            _contextMenu.Items.AddRange(new ToolStripItem[] { exportToExcelItem, exportToPdfItem, exportToCsvItem });
        }

        /// <summary>
        /// Adds grouping menu items
        /// </summary>
        private void AddGroupingMenuItems(string columnName, string columnHeaderText)
        {
            bool isCurrentlyGrouped = _groupByColumn == columnName && !string.IsNullOrEmpty(_groupByColumn);
            string groupText = isCurrentlyGrouped ? "Cancel group by" : $"Group by {columnHeaderText}";

            var groupByItem = new ToolStripMenuItem(groupText, null, (s, e) => ToggleGroupBy(columnName))
            {
                Name = "groupBy",
                Tag = columnName
            };

            _contextMenu.Items.Add(groupByItem);
        }

        /// <summary>
        /// Adds column sizing menu items
        /// </summary>
        private void AddColumnSizingMenuItems()
        {
            var columnModeItem = new ToolStripMenuItem("Column Mode");

            var fillModeItem = new ToolStripMenuItem("Fill Column", null, (s, e) => this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill);
            var allCellsModeItem = new ToolStripMenuItem("All Cells", null, (s, e) => this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells);

            columnModeItem.DropDownItems.AddRange(new ToolStripItem[] { fillModeItem, allCellsModeItem });
            _contextMenu.Items.Add(columnModeItem);
        }

        /// <summary>
        /// Creates hidden columns submenu
        /// </summary>
        private ToolStripMenuItem CreateHiddenColumnsMenu()
        {
            var hiddenColumns = this.Columns.Cast<DataGridViewColumn>()
                .Where(col => !col.Visible && (col.Tag == null || col.Tag.ToString() != "HideIgnore"))
                .ToList();

            if (!hiddenColumns.Any()) return null;

            var hiddenColumnsMenu = new ToolStripMenuItem("Hidden Columns");

            // Show all item
            var showAllItem = new ToolStripMenuItem("Show All", null, (s, e) => ShowAllColumns())
            {
                Tag = "ShowAll"
            };
            hiddenColumnsMenu.DropDownItems.Add(showAllItem);

            // Individual column items
            foreach (var column in hiddenColumns)
            {
                var showColumnItem = new ToolStripMenuItem(column.HeaderText, null, (s, e) => ShowColumn(column.Name))
                {
                    Tag = column.Name
                };
                hiddenColumnsMenu.DropDownItems.Add(showColumnItem);
            }

            return hiddenColumnsMenu;
        }

        #endregion

        #region Sorting Methods

        /// <summary>
        /// Sorts a column in the specified direction
        /// </summary>
        /// <param name="columnName">Name of the column to sort</param>
        /// <param name="direction">Sort direction</param>
        public void SortColumn(string columnName, ListSortDirection direction)
        {
            if (!_enableSorting || !this.Columns.Contains(columnName)) return;

            try
            {
                this.Sort(this.Columns[columnName], direction);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error sorting column: {ex.Message}");
            }
        }

        #endregion

        #region Column Management Methods

        /// <summary>
        /// Hides the specified column
        /// </summary>
        /// <param name="columnName">Name of the column to hide</param>
        public void HideColumn(string columnName)
        {
            if (!_allowHideColumns || !this.Columns.Contains(columnName)) return;

            this.Columns[columnName].Visible = false;
            OnColumnVisibilityChanged(new ColumnVisibilityEventArgs { ColumnName = columnName, IsVisible = false });
        }

        /// <summary>
        /// Shows the specified column
        /// </summary>
        /// <param name="columnName">Name of the column to show</param>
        public void ShowColumn(string columnName)
        {
            if (!this.Columns.Contains(columnName)) return;

            this.Columns[columnName].Visible = true;
            OnColumnVisibilityChanged(new ColumnVisibilityEventArgs { ColumnName = columnName, IsVisible = true });
        }

        /// <summary>
        /// Shows all hidden columns
        /// </summary>
        public void ShowAllColumns()
        {
            foreach (DataGridViewColumn column in this.Columns)
            {
                if (!column.Visible && (column.Tag == null || column.Tag.ToString() != "HideIgnore"))
                {
                    column.Visible = true;
                    OnColumnVisibilityChanged(new ColumnVisibilityEventArgs { ColumnName = column.Name, IsVisible = true });
                }
            }
        }

        #endregion

        #region Grouping Methods

        /// <summary>
        /// Toggles grouping by the specified column
        /// </summary>
        /// <param name="columnName">Name of the column to group by</param>
        public void ToggleGroupBy(string columnName)
        {
            if (!_enableGrouping) return;

            if (string.IsNullOrEmpty(_groupByColumn) || _groupByColumn != columnName)
            {
                GroupBy(columnName);
            }
            else
            {
                ClearGrouping();
            }
        }

        /// <summary>
        /// Groups data by the specified column
        /// </summary>
        /// <param name="columnName">Name of the column to group by</param>
        public void GroupBy(string columnName)
        {
            if (!_enableGrouping || !HasRows() || !this.Columns.Contains(columnName)) return;

            try
            {
                _groupByColumn = columnName;
                _groupColors.Clear();
                this.Sort(this.Columns[columnName], ListSortDirection.Ascending);
                this.Refresh();
                OnGroupingChanged(new GroupingEventArgs { ColumnName = columnName, IsGrouped = true });
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error grouping data: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears current grouping
        /// </summary>
        public void ClearGrouping()
        {
            if (string.IsNullOrEmpty(_groupByColumn)) return;

            string previousColumn = _groupByColumn;
            _groupByColumn = string.Empty;
            _groupColors.Clear();
            this.Refresh();

            if (this.Columns.Count > 0)
            {
                this.Sort(this.Columns[0], ListSortDirection.Ascending);
            }

            OnGroupingChanged(new GroupingEventArgs { ColumnName = previousColumn, IsGrouped = false });
        }

        /// <summary>
        /// Applies grouping format to cells
        /// </summary>
        private void ApplyGroupingFormat(DataGridViewCellFormattingEventArgs args)
        {
            if (args.ColumnIndex != this.Columns[_groupByColumn].Index) return;

            if (IsRepeatedCellValue(args.RowIndex))
            {
                args.Value = string.Empty;
            }
            else
            {
                object cellValue = args.Value ?? string.Empty;
                if (!_groupColors.ContainsKey(cellValue))
                {
                    _groupColors[cellValue] = GetRandomColor();
                }
                this.Rows[args.RowIndex].DefaultCellStyle.BackColor = _groupColors[cellValue];
            }
        }

        /// <summary>
        /// Checks if a cell value is repeated from the previous row
        /// </summary>
        private bool IsRepeatedCellValue(int rowIndex)
        {
            if (rowIndex <= 0 || string.IsNullOrEmpty(_groupByColumn)) return false;

            int groupingColumnIndex = Columns[_groupByColumn].Index;
            object currCellValue = Rows[rowIndex].Cells[groupingColumnIndex].Value;
            object prevCellValue = Rows[rowIndex - 1].Cells[groupingColumnIndex].Value;

            return object.Equals(currCellValue, prevCellValue);
        }

        /// <summary>
        /// Generates a random light color for grouping
        /// </summary>
        private Color GetRandomColor()
        {
            return Color.FromArgb(_random.Next(210, 256), _random.Next(210, 256), _random.Next(210, 256));
        }

        #endregion

        #region Export Methods

        /// <summary>
        /// Exports data asynchronously in the specified format
        /// </summary>
        /// <param name="format">Export format</param>
        public async Task ExportDataAsync(ExportFormat format)
        {
            if (!_enableExporting || this.Rows.Count == 0)
            {
                ShowWarningMessage("No data available for export");
                return;
            }

            try
            {
                var eventArgs = new ExportEventArgs { Format = format, FilePath = GetExportFilePath(format) };
                OnBeforeExport(eventArgs);

                if (eventArgs.Cancel) return;

                await Task.Run(() =>
                {
                    Thread staThread = new Thread(() =>
                    {
                        try
                        {
                            switch (format)
                            {
                                case ExportFormat.Excel:
                                    ExportToExcel(true);
                                    break;
                                case ExportFormat.PDF:
                                    ExportToPdf();
                                    break;
                                case ExportFormat.CSV:
                                    ExportToCsv(eventArgs.FilePath);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Invoke((MethodInvoker)(() => ShowErrorMessage($"Export failed: {ex.Message}")));
                        }
                    });
                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    staThread.Join();
                });

                OnAfterExport(eventArgs);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Export operation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Exports data to Excel format
        /// </summary>
        private void ExportToExcel(bool showExcel = true)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Workbook excelWorkBook = null;
            Worksheet excelWorkSheet = null;

            try
            {
                PrepareForExport();

                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = showExcel;
                excelWorkBook = excelApp.Workbooks.Add(Missing.Value);
                excelWorkSheet = (Worksheet)excelWorkBook.Worksheets.get_Item(1);

                // Paste data
                excelWorkSheet.PasteSpecial(excelWorkSheet.Range["A1"], Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

                // Format the worksheet
                FormatExcelWorksheet(excelWorkSheet);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Excel export failed: {ex.Message}", ex);
            }
            finally
            {
                RestoreAfterExport();
                if (!showExcel)
                {
                    CleanupExcelObjects(excelWorkSheet, excelWorkBook, excelApp);
                }
            }
        }

        /// <summary>
        /// Exports data to PDF format
        /// </summary>
        private void ExportToPdf()
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Workbook excelWorkBook = null;
            Worksheet excelWorkSheet = null;
            string pdfFilePath = Path.Combine(ExportPath, $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            try
            {
                PrepareForExport();

                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                excelWorkBook = excelApp.Workbooks.Add(Missing.Value);
                excelWorkSheet = (Worksheet)excelWorkBook.Worksheets.get_Item(1);

                // Paste data
                excelWorkSheet.PasteSpecial(excelWorkSheet.Range["A1"], Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

                // Format the worksheet
                FormatExcelWorksheet(excelWorkSheet);

                // Configure page setup for PDF
                ConfigurePdfPageSetup(excelWorkSheet);

                // Export to PDF
                excelWorkSheet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, pdfFilePath);

                // Open the PDF file
                if (File.Exists(pdfFilePath))
                {
                    Process.Start(pdfFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"PDF export failed: {ex.Message}", ex);
            }
            finally
            {
                RestoreAfterExport();
                CleanupExcelObjects(excelWorkSheet, excelWorkBook, excelApp);
            }
        }

        /// <summary>
        /// Exports data to CSV format
        /// </summary>
        private void ExportToCsv(string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // Write headers
                    var headers = this.Columns.Cast<DataGridViewColumn>()
                        .Where(col => col.Visible)
                        .Select(col => EscapeCsvField(col.HeaderText));
                    writer.WriteLine(string.Join(",", headers));

                    // Write data rows
                    foreach (DataGridViewRow row in this.Rows)
                    {
                        if (row.IsNewRow) continue;

                        var values = this.Columns.Cast<DataGridViewColumn>()
                            .Where(col => col.Visible)
                            .Select(col => EscapeCsvField(row.Cells[col.Index].Value?.ToString() ?? ""));
                        writer.WriteLine(string.Join(",", values));
                    }
                }

                if (File.Exists(filePath))
                {
                    Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"CSV export failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Escapes CSV field values
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }

        /// <summary>
        /// Prepares the DataGridView for export operations
        /// </summary>
        private void PrepareForExport()
        {
            this.Invoke((MethodInvoker)delegate
            {
                // Hide non-text columns temporarily
                foreach (DataGridViewColumn column in this.Columns)
                {
                    if (!(column is DataGridViewTextBoxColumn))
                    {
                        column.Tag = column.Visible ? "WasVisible" : "WasHidden";
                        column.Visible = false;
                    }
                }

                // Configure clipboard settings
                this.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                this.MultiSelect = true;
                this.SelectAll();

                // Copy to clipboard
                DataObject dataObj = this.GetClipboardContent();
                if (dataObj != null)
                    Clipboard.SetDataObject(dataObj);
            });
        }

        /// <summary>
        /// Restores the DataGridView after export operations
        /// </summary>
        private void RestoreAfterExport()
        {
            this.Invoke((MethodInvoker)delegate
            {
                // Restore column visibility
                foreach (DataGridViewColumn column in this.Columns)
                {
                    if (!(column is DataGridViewTextBoxColumn))
                    {
                        column.Visible = column.Tag?.ToString() == "WasVisible";
                        column.Tag = null;
                    }
                }

                this.ClearSelection();
                this.MultiSelect = false;
            });
        }

        /// <summary>
        /// Formats Excel worksheet with styling
        /// </summary>
        private void FormatExcelWorksheet(Worksheet worksheet)
        {
            try
            {
                // Set row height for header
                worksheet.Rows[1].RowHeight = 25;

                // Format header row
                Range headerRange = worksheet.Range["A1", worksheet.Cells[1, worksheet.UsedRange.Columns.Count]];
                headerRange.Interior.Color = ColorTranslator.ToOle(_headerBackColor);
                headerRange.Font.Bold = true;
                headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                // Add borders to all data
                Range dataRange = worksheet.Range["A1"].Resize[worksheet.UsedRange.Rows.Count, worksheet.UsedRange.Columns.Count];
                dataRange.Borders.LineStyle = XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = XlBorderWeight.xlThin;

                // Auto-fit columns if enabled
                if (_autoFitColumns)
                {
                    for (int i = 1; i <= worksheet.UsedRange.Columns.Count; i++)
                    {
                        worksheet.Columns[i].AutoFit();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log formatting error but don't fail the export
                System.Diagnostics.Debug.WriteLine($"Excel formatting error: {ex.Message}");
            }
        }

        /// <summary>
        /// Configures page setup for PDF export
        /// </summary>
        private void ConfigurePdfPageSetup(Worksheet worksheet)
        {
            try
            {
                worksheet.PageSetup.Orientation = XlPageOrientation.xlPortrait;
                worksheet.PageSetup.PaperSize = XlPaperSize.xlPaperA4;
                worksheet.PageSetup.Zoom = false;
                worksheet.PageSetup.FitToPagesWide = 1;
                worksheet.PageSetup.FitToPagesTall = false;
                worksheet.PageSetup.PrintTitleRows = "$1:$1"; // Repeat header row on each page
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Page setup error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the export file path for the specified format
        /// </summary>
        private string GetExportFilePath(ExportFormat format)
        {
            string extension;

            switch (format)
            {
                case ExportFormat.Excel:
                    extension = ".xlsx";
                    break;
                case ExportFormat.PDF:
                    extension = ".pdf";
                    break;
                case ExportFormat.CSV:
                    extension = ".csv";
                    break;
                default:
                    extension = ".xlsx";
                    break;
            }

            return Path.Combine(ExportPath, $"Export_{DateTime.Now:yyyyMMdd_HHmmss}{extension}");
        }

        /// <summary>
        /// Cleans up Excel COM objects
        /// </summary>
        private void CleanupExcelObjects(Worksheet worksheet, Workbook workbook, Microsoft.Office.Interop.Excel.Application application)
        {
            try
            {
                if (worksheet != null)
                {
                    Marshal.ReleaseComObject(worksheet);
                    worksheet = null;
                }

                if (workbook != null)
                {
                    workbook.Close(false);
                    Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }

                if (application != null)
                {
                    application.Quit();
                    Marshal.ReleaseComObject(application);
                    application = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"COM cleanup error: {ex.Message}");
            }
        }

        #endregion

        #region Search Column Management

        /// <summary>
        /// Adds search columns to the menu strip
        /// </summary>
        public void AddSearchColumnsMenu()
        {
            if (_columnsMenuStrip == null) return;

            _columnsMenuStrip.DropDownItems.Clear();

            // Add "All" item
            var allItem = new ToolStripMenuItem
            {
                Name = "All",
                Text = "All",
                Checked = false,
                CheckOnClick = true
            };
            allItem.Click += AllItem_Click;
            _columnsMenuStrip.DropDownItems.Add(allItem);

            // Add separator
            _columnsMenuStrip.DropDownItems.Add(new ToolStripSeparator());

            // Add individual column items
            var newColumnsType = new Dictionary<string, string>();

            foreach (DataGridViewColumn column in this.Columns)
            {
                if (ShouldSkipColumn(column)) continue;

                bool isDefault = _defaultSelectedColumns.Contains(column.Name);
                var item = new ToolStripMenuItem
                {
                    Name = column.Name,
                    Text = column.HeaderText,
                    Checked = isDefault,
                    CheckOnClick = true
                };

                item.Click += SearchColumnItem_Click;
                _columnsMenuStrip.DropDownItems.Add(item);

                if (isDefault)
                {
                    string dataType = GetColumnDataType(column);
                    newColumnsType.Add(item.Name, dataType);
                }
            }

            _columnsType = newColumnsType;
            UpdateFlowPanel();
        }

        /// <summary>
        /// Determines if a column should be skipped from search menu
        /// </summary>
        private bool ShouldSkipColumn(DataGridViewColumn column)
        {
            return _columnsToSkip.Contains(column.Name) || column is DataGridViewImageColumn;
        }

        /// <summary>
        /// Gets the data type for a column
        /// </summary>
        private string GetColumnDataType(DataGridViewColumn column)
        {
            if (column.ValueType == typeof(string) || column.ValueType == typeof(DateTime))
                return "string";
            return "int";
        }

        /// <summary>
        /// Updates the search columns selection
        /// </summary>
        private void UpdateSearchColumnsMenu()
        {
            var newColumnsType = new Dictionary<string, string>();

            foreach (ToolStripMenuItem item in _columnsMenuStrip.DropDownItems)
            {
                if (!item.Checked || item.Name == "All") continue;

                if (this.Columns.Contains(item.Name))
                {
                    string dataType = GetColumnDataType(this.Columns[item.Name]);
                    newColumnsType.Add(item.Name, dataType);
                }
            }

            _columnsType = newColumnsType;
            UpdateFlowPanel();
        }

        /// <summary>
        /// Updates the flow panel with selected columns
        /// </summary>
        private void UpdateFlowPanel()
        {
            if (_flowSelectedColumn == null) return;

            _flowSelectedColumn.SuspendLayout();
            _flowSelectedColumn.Controls.Clear();

            foreach (var columnType in _columnsType)
            {
                if (!this.Columns.Contains(columnType.Key)) continue;

                var label = new System.Windows.Forms.Label
                {
                    Name = columnType.Key,
                    Text = this.Columns[columnType.Key].HeaderText,
                    BackColor = Color.FromArgb(223, 230, 236),
                    ForeColor = Color.FromArgb(81, 80, 93),
                    Height = 30,
                    AutoSize = true,
                    Padding = new Padding(5),
                    Margin = new Padding(2)
                };

                _flowSelectedColumn.Controls.Add(label);
            }

            _flowSelectedColumn.ResumeLayout();
        }

        /// <summary>
        /// Handles "All" item click in search menu
        /// </summary>
        private void AllItem_Click(object sender, EventArgs e)
        {
            var allItem = sender as ToolStripMenuItem;
            if (allItem == null) return;

            foreach (ToolStripMenuItem item in _columnsMenuStrip.DropDownItems)
            {
                if (item != allItem)
                    item.Checked = allItem.Checked;
            }

            UpdateSearchColumnsMenu();
        }

        /// <summary>
        /// Handles individual search column item click
        /// </summary>
        private void SearchColumnItem_Click(object sender, EventArgs e)
        {
            UpdateSearchColumnsMenu();
        }

        #endregion

        #region Data Source Management

        /// <summary>
        /// Sets the data source with optional callbacks
        /// </summary>
        /// <param name="data">Data table to bind</param>
        /// <param name="resizeHeaderCallback">Callback for resizing headers</param>
        /// <param name="addCaptionCallback">Callback for adding captions</param>
        public void SetDataSource(System.Data.DataTable data, ResizeDGVHeaderDelegate resizeHeaderCallback = null, AddCaptionToDGVDelegate addCaptionCallback = null)
        {
            try
            {
                this.DataSource = data;
                this.ClearSelection();

                if (_isDataSourceEmpty && data != null && data.Rows.Count > 0)
                {
                    _isDataSourceEmpty = false;
                    resizeHeaderCallback?.Invoke();
                    addCaptionCallback?.Invoke();
                }
                else if (data == null || data.Rows.Count == 0)
                {
                    _isDataSourceEmpty = true;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error setting data source: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the data source and clears all data
        /// </summary>
        public void ResetDataSource()
        {
            try
            {
                _isDataSourceEmpty = true;
                this.DataSource = null;
                this.Columns.Clear();
                _groupByColumn = string.Empty;
                _groupColors.Clear();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error resetting data source: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if the DataGridView has any rows
        /// </summary>
        /// <returns>True if there are rows, false otherwise</returns>
        public bool HasRows()
        {
            return this.Rows.Count > 0 && !_isDataSourceEmpty;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Shows an error message to the user
        /// </summary>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a warning message to the user
        /// </summary>
        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Shows an information message to the user
        /// </summary>
        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the BeforeExport event
        /// </summary>
        protected virtual void OnBeforeExport(ExportEventArgs e)
        {
            BeforeExport?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the AfterExport event
        /// </summary>
        protected virtual void OnAfterExport(ExportEventArgs e)
        {
            AfterExport?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the GroupingChanged event
        /// </summary>
        protected virtual void OnGroupingChanged(GroupingEventArgs e)
        {
            GroupingChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the ColumnVisibilityChanged event
        /// </summary>
        protected virtual void OnColumnVisibilityChanged(ColumnVisibilityEventArgs e)
        {
            ColumnVisibilityChanged?.Invoke(this, e);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes the entire DataGridView
        /// </summary>
        public void RefreshGrid()
        {
            try
            {
                this.Refresh();
                this.Invalidate();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error refreshing grid: {ex.Message}");
            }
        }

        /// <summary>
        /// Exports data using the default format
        /// </summary>
        public async Task ExportDataAsync()
        {
            await ExportDataAsync(_defaultExportFormat);
        }

        /// <summary>
        /// Gets all visible columns
        /// </summary>
        /// <returns>Array of visible column names</returns>
        public string[] GetVisibleColumns()
        {
            return this.Columns.Cast<DataGridViewColumn>()
                .Where(col => col.Visible)
                .Select(col => col.Name)
                .ToArray();
        }

        /// <summary>
        /// Gets all hidden columns
        /// </summary>
        /// <returns>Array of hidden column names</returns>
        public string[] GetHiddenColumns()
        {
            return this.Columns.Cast<DataGridViewColumn>()
                .Where(col => !col.Visible)
                .Select(col => col.Name)
                .ToArray();
        }

        /// <summary>
        /// Sets the visibility of multiple columns
        /// </summary>
        /// <param name="columnVisibility">Dictionary mapping column names to visibility</param>
        public void SetColumnVisibility(Dictionary<string, bool> columnVisibility)
        {
            if (columnVisibility == null) return;

            foreach (var kvp in columnVisibility)
            {
                if (this.Columns.Contains(kvp.Key))
                {
                    this.Columns[kvp.Key].Visible = kvp.Value;
                }
            }
        }

        /// <summary>
        /// Applies a filter to the data source (if it's a DataTable)
        /// </summary>
        /// <param name="filterExpression">Filter expression</param>
        public void ApplyFilter(string filterExpression)
        {
            try
            {
                if (this.DataSource is System.Data.DataTable dataTable)
                {
                    dataTable.DefaultView.RowFilter = filterExpression;
                }
                else if (this.DataSource is BindingSource bindingSource && bindingSource.DataSource is System.Data.DataTable table)
                {
                    table.DefaultView.RowFilter = filterExpression;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error applying filter: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears any applied filters
        /// </summary>
        public void ClearFilter()
        {
            ApplyFilter(string.Empty);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Disposes the component and cleans up resources
        /// </summary>
        /// <param name="disposing">True if disposing managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _contextMenu?.Dispose();
                _groupColors?.Clear();
                _columnsType?.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Designer Support

        /// <summary>
        /// Provides design-time support for the control
        /// </summary>
        protected override bool DoubleBuffered
        {
            get { return true; }
            set { base.DoubleBuffered = value; }
        }

        #endregion
    }
}