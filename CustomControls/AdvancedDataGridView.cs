using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using XlLineStyle = Microsoft.Office.Interop.Excel.XlLineStyle;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced DataGridView control with enhanced features including sorting, grouping, Excel exporting,
    /// column management, and multi-language support. Provides comprehensive data visualization capabilities
    /// with context menu operations, dynamic styling, and professional appearance customization.
    /// Optimized for .NET Framework 4.8 with improved performance, error handling, and extensibility.
    /// </summary>
    [ToolboxItem(true)]
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
    public class AdvancedDataGridView : DataGridView
    {
        #region Constants

        /// <summary>
        /// Default header background color
        /// </summary>
        private static readonly Color DEFAULT_HEADER_COLOR = Color.FromArgb(186, 198, 220);

        /// <summary>
        /// Default alternating row color
        /// </summary>
        private static readonly Color DEFAULT_ALTERNATING_COLOR = Color.FromArgb(240, 240, 240);

        /// <summary>
        /// Minimum color value for random group colors
        /// </summary>
        private const int MIN_GROUP_COLOR_VALUE = 210;

        /// <summary>
        /// Maximum color value for random group colors
        /// </summary>
        private const int MAX_GROUP_COLOR_VALUE = 256;

        /// <summary>
        /// Default header row height for Excel export
        /// </summary>
        private const double EXCEL_HEADER_ROW_HEIGHT = 25.0;

        #endregion

        #region Private Fields

        /// <summary>
        /// Dictionary mapping column names to their data types for search functionality
        /// </summary>
        private Dictionary<string, string> _columnsType = new Dictionary<string, string>();

        /// <summary>
        /// Menu strip for column selection in search operations
        /// </summary>
        private ToolStripMenuItem _columnsMenuStrip;

        /// <summary>
        /// Flow layout panel to display selected search columns
        /// </summary>
        private FlowLayoutPanel _flowSelectedColumn;

        /// <summary>
        /// Array of column names to skip from context menu operations
        /// </summary>
        private string[] _columnsToSkip;

        /// <summary>
        /// Array of column names that should be selected by default
        /// </summary>
        private string[] _defaultSelectedColumns;

        /// <summary>
        /// Flag indicating whether columns can be hidden
        /// </summary>
        private bool _allowHideColumns = true;

        /// <summary>
        /// Context menu for right-click operations
        /// </summary>
        private ContextMenuStrip _contextMenu = new ContextMenuStrip();

        /// <summary>
        /// Random number generator for group colors
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// Current column used for grouping
        /// </summary>
        private string _groupByColumn = "";

        /// <summary>
        /// Dictionary mapping group values to their assigned colors
        /// </summary>
        private Dictionary<object, Color> _groupColors = new Dictionary<object, Color>();

        /// <summary>
        /// Flag indicating whether the data source is empty
        /// </summary>
        private bool _isDataSourceEmpty = true;

        /// <summary>
        /// Default path for exported files
        /// </summary>
        private string _exportPath = "";

        /// <summary>
        /// Flag indicating whether grouping functionality is enabled
        /// </summary>
        private bool _enableGrouping = true;

        /// <summary>
        /// Flag indicating whether exporting functionality is enabled
        /// </summary>
        private bool _enableExporting = true;

        /// <summary>
        /// Flag indicating whether sorting functionality is enabled
        /// </summary>
        private bool _enableSorting = true;

        /// <summary>
        /// Flag indicating whether to auto-fit columns after export operations
        /// </summary>
        private bool _autoFitColumns = true;

        /// <summary>
        /// Background color for column headers
        /// </summary>
        private Color _headerBackColor = DEFAULT_HEADER_COLOR;

        /// <summary>
        /// Background color for alternating rows
        /// </summary>
        private Color _alternatingRowColor = DEFAULT_ALTERNATING_COLOR;

        /// <summary>
        /// Flag indicating whether alternating row colors are enabled
        /// </summary>
        private bool _enableAlternatingRowColors = false;

        #endregion

        #region Enums

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
        /// Event fired before Excel export begins
        /// </summary>
        public event EventHandler<ExportEventArgs> BeforeExport;

        /// <summary>
        /// Event fired after Excel export completes
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

        #region Event Arguments

        /// <summary>
        /// Event arguments for export operations
        /// </summary>
        public class ExportEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the file path for the export operation
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// Gets or sets whether the export operation should be canceled
            /// </summary>
            public bool Cancel { get; set; }

            /// <summary>
            /// Gets or sets the number of rows being exported
            /// </summary>
            public int RowCount { get; set; }

            /// <summary>
            /// Gets or sets the number of columns being exported
            /// </summary>
            public int ColumnCount { get; set; }
        }

        /// <summary>
        /// Event arguments for grouping operations
        /// </summary>
        public class GroupingEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the name of the column being grouped
            /// </summary>
            public string ColumnName { get; set; }

            /// <summary>
            /// Gets or sets whether the column is currently grouped
            /// </summary>
            public bool IsGrouped { get; set; }
        }

        /// <summary>
        /// Event arguments for column visibility changes
        /// </summary>
        public class ColumnVisibilityEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the name of the column whose visibility changed
            /// </summary>
            public string ColumnName { get; set; }

            /// <summary>
            /// Gets or sets whether the column is visible
            /// </summary>
            public bool IsVisible { get; set; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the header background color
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Background color for column headers")]
        [DefaultValue(typeof(Color), "186, 198, 220")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColumnsHeaderColor
        {
            get { return _headerBackColor; }
            set
            {
                if (_headerBackColor != value)
                {
                    _headerBackColor = value;
                    UpdateHeaderColors();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column types for search functionality
        /// </summary>
        [Category("Advanced Data")]
        [Description("Dictionary mapping column names to their data types")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, string> ColumnsType
        {
            get { return _columnsType; }
            set { _columnsType = value ?? new Dictionary<string, string>(); }
        }

        /// <summary>
        /// Gets or sets the menu strip for column selection
        /// </summary>
        [Category("Advanced UI")]
        [Description("Menu strip for column selection in search operations")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ToolStripMenuItem ColumnsMenuStrip
        {
            get { return _columnsMenuStrip; }
            set { _columnsMenuStrip = value; }
        }

        /// <summary>
        /// Gets or sets the flow panel for selected columns display
        /// </summary>
        [Category("Advanced UI")]
        [Description("Flow layout panel to display selected search columns")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string[] ColumnsToSkip
        {
            get { return _columnsToSkip ?? new string[0]; }
            set { _columnsToSkip = value; }
        }

        /// <summary>
        /// Gets or sets default selected columns
        /// </summary>
        [Category("Advanced Behavior")]
        [Description("Array of column names that should be selected by default for search")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableGrouping
        {
            get { return _enableGrouping; }
            set { _enableGrouping = value; }
        }

        /// <summary>
        /// Gets or sets whether exporting is enabled
        /// </summary>
        [Category("Advanced Behavior")]
        [Description("Enables or disables Excel export functionality")]
        [DefaultValue(true)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableSorting
        {
            get { return _enableSorting; }
            set { _enableSorting = value; }
        }

        /// <summary>
        /// Gets or sets the export file path
        /// </summary>
        [Category("Advanced Export")]
        [Description("Default path for exported Excel files")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ExportPath
        {
            get { return string.IsNullOrEmpty(_exportPath) ? System.Windows.Forms.Application.StartupPath : _exportPath; }
            set { _exportPath = value; }
        }

        /// <summary>
        /// Gets or sets whether to auto-fit columns after export operations
        /// </summary>
        [Category("Advanced Export")]
        [Description("Automatically fit columns in exported Excel files")]
        [DefaultValue(true)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableAlternatingRowColors
        {
            get { return _enableAlternatingRowColors; }
            set
            {
                if (_enableAlternatingRowColors != value)
                {
                    _enableAlternatingRowColors = value;
                    UpdateAlternatingRowColors();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alternating row color
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Background color for alternating rows")]
        [DefaultValue(typeof(Color), "240, 240, 240")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AlternatingRowColor
        {
            get { return _alternatingRowColor; }
            set
            {
                if (_alternatingRowColor != value)
                {
                    _alternatingRowColor = value;
                    if (_enableAlternatingRowColors)
                        UpdateAlternatingRowColors();
                }
            }
        }

        /// <summary>
        /// Gets whether the data source is empty
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDataSourceEmpty
        {
            get { return _isDataSourceEmpty; }
        }

        /// <summary>
        /// Gets the current grouping column
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Initializes the component with default settings
        /// </summary>
        private void InitializeComponent()
        {
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.MultiSelect = false;
            this.AutoGenerateColumns = true;
            this.EnableHeadersVisualStyles = false;
            this.RowHeadersVisible = false;
        }

        /// <summary>
        /// Sets default visual styles for the DataGridView
        /// </summary>
        private void SetDefaultStyles()
        {
            UpdateHeaderColors();

            this.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(this.Font, FontStyle.Bold);
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (_enableAlternatingRowColors)
                UpdateAlternatingRowColors();
        }

        /// <summary>
        /// Initializes the context menu for right-click operations
        /// </summary>
        private void InitializeContextMenu()
        {
            _contextMenu.Opening += OnContextMenuOpening;
        }

        /// <summary>
        /// Updates the header colors based on current settings
        /// </summary>
        private void UpdateHeaderColors()
        {
            this.ColumnHeadersDefaultCellStyle.BackColor = _headerBackColor;
            this.ColumnHeadersDefaultCellStyle.SelectionBackColor = _headerBackColor;
        }

        /// <summary>
        /// Updates the alternating row colors based on current settings
        /// </summary>
        private void UpdateAlternatingRowColors()
        {
            this.AlternatingRowsDefaultCellStyle.BackColor = _enableAlternatingRowColors ?
                _alternatingRowColor : this.DefaultCellStyle.BackColor;
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Handles mouse enter events for cells to change cursor for image columns
        /// </summary>
        /// <param name="e">Cell event arguments</param>
        protected override void OnCellMouseEnter(DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;

            base.OnCellMouseEnter(e);
            this.Cursor = this.Columns[e.ColumnIndex] is DataGridViewImageColumn ? Cursors.Hand : Cursors.Default;
        }

        /// <summary>
        /// Handles mouse click events for cells to show context menu on right-click
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
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
        /// Handles cell formatting for grouping visualization
        /// </summary>
        /// <param name="args">Cell formatting event arguments</param>
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs args)
        {
            base.OnCellFormatting(args);

            if (string.IsNullOrEmpty(_groupByColumn) || args.RowIndex < 0) return;

            ApplyGroupingFormat(args);
        }

        #endregion

        #region Context Menu Methods

        /// <summary>
        /// Handles context menu opening event to validate conditions
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Cancel event arguments</param>
        private void OnContextMenuOpening(object sender, CancelEventArgs e)
        {
            if (this.CurrentCell == null) return;

            if (!(this.Columns[this.CurrentCell.ColumnIndex] is DataGridViewTextBoxColumn))
            {
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Shows the context menu with appropriate options based on the clicked cell
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
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
        /// Builds the context menu with available options based on enabled features
        /// </summary>
        /// <param name="columnName">Name of the clicked column</param>
        /// <param name="columnHeaderText">Display text of the clicked column</param>
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
        /// Adds sorting menu items to the context menu
        /// </summary>
        /// <param name="columnName">Name of the column to sort</param>
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
        /// Adds column management menu items to the context menu
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="columnHeaderText">Display text of the column</param>
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
        /// Adds export menu items to the context menu
        /// </summary>
        private void AddExportMenuItems()
        {
            var exportToExcelItem = new ToolStripMenuItem("Export to Excel", null, async (s, e) => await ExportDataAsync())
            {
                Name = "exportExcel"
            };

            _contextMenu.Items.Add(exportToExcelItem);
        }

        /// <summary>
        /// Adds grouping menu items to the context menu
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="columnHeaderText">Display text of the column</param>
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
        /// Adds column sizing menu items to the context menu
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
        /// Creates a submenu for showing hidden columns
        /// </summary>
        /// <returns>Hidden columns menu item or null if no hidden columns exist</returns>
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
        /// <param name="direction">Sort direction (ascending or descending)</param>
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
        /// Hides the specified column from view
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
        /// Shows all hidden columns except those marked to be ignored
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
        /// Groups data by the specified column with visual formatting
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
        /// Clears current grouping and resets visual formatting
        /// </summary>
        public void ClearGrouping()
        {
            if (string.IsNullOrEmpty(_groupByColumn)) return;

            string previousColumn = _groupByColumn;
            _groupByColumn = string.Empty;
            _groupColors.Clear();
            this.Refresh();

            // Find first data-bound column
            var sortableColumn = this.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(col => !string.IsNullOrEmpty(col.DataPropertyName));

            if (sortableColumn != null)
            {
                this.Sort(sortableColumn, ListSortDirection.Ascending);
            }

            OnGroupingChanged(new GroupingEventArgs { ColumnName = previousColumn, IsGrouped = false });
        }

        /// <summary>
        /// Applies grouping format to cells for visual representation
        /// </summary>
        /// <param name="args">Cell formatting event arguments</param>
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
        /// Checks if a cell value is repeated from the previous row in the same column
        /// </summary>
        /// <param name="rowIndex">Index of the row to check</param>
        /// <returns>True if the cell value is repeated from the previous row</returns>
        private bool IsRepeatedCellValue(int rowIndex)
        {
            if (rowIndex <= 0 || string.IsNullOrEmpty(_groupByColumn)) return false;

            int groupingColumnIndex = Columns[_groupByColumn].Index;
            object currCellValue = Rows[rowIndex].Cells[groupingColumnIndex].Value;
            object prevCellValue = Rows[rowIndex - 1].Cells[groupingColumnIndex].Value;

            return object.Equals(currCellValue, prevCellValue);
        }

        /// <summary>
        /// Generates a random light color for grouping visualization
        /// </summary>
        /// <returns>A light color for group background</returns>
        private Color GetRandomColor()
        {
            return Color.FromArgb(_random.Next(MIN_GROUP_COLOR_VALUE, MAX_GROUP_COLOR_VALUE),
                                 _random.Next(MIN_GROUP_COLOR_VALUE, MAX_GROUP_COLOR_VALUE),
                                 _random.Next(MIN_GROUP_COLOR_VALUE, MAX_GROUP_COLOR_VALUE));
        }

        #endregion

        #region Export Methods

        /// <summary>
        /// Exports data asynchronously to Excel format
        /// </summary>
        public async Task ExportDataAsync()
        {
            if (!_enableExporting || this.Rows.Count == 0)
            {
                ShowWarningMessage("No data available for export");
                return;
            }

            try
            {
                var eventArgs = new ExportEventArgs
                {
                    FilePath = GetExportFilePath(),
                    RowCount = this.Rows.Count,
                    ColumnCount = this.Columns.Cast<DataGridViewColumn>().Count(col => col.Visible)
                };

                OnBeforeExport(eventArgs);

                if (eventArgs.Cancel) return;

                await Task.Run(() =>
                {
                    Thread staThread = new Thread(() =>
                    {
                        try
                        {
                            ExportToExcel(true);
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
        /// Exports data to Excel format with formatting and styling
        /// </summary>
        /// <param name="showExcel">Whether to show Excel application after export</param>
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
        /// Prepares the DataGridView for export operations by configuring visibility and clipboard
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
        /// Restores the DataGridView state after export operations
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
        /// Formats Excel worksheet with professional styling and borders
        /// </summary>
        /// <param name="worksheet">Excel worksheet to format</param>
        private void FormatExcelWorksheet(Worksheet worksheet)
        {
            try
            {
                // Set row height for header
                worksheet.Rows[1].RowHeight = EXCEL_HEADER_ROW_HEIGHT;

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
        /// Gets the export file path with timestamp for uniqueness
        /// </summary>
        /// <returns>Full path for the Excel export file</returns>
        private string GetExportFilePath()
        {
            return Path.Combine(ExportPath, $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        /// <summary>
        /// Cleans up Excel COM objects to prevent memory leaks
        /// </summary>
        /// <param name="worksheet">Excel worksheet to cleanup</param>
        /// <param name="workbook">Excel workbook to cleanup</param>
        /// <param name="application">Excel application to cleanup</param>
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
        /// Adds search columns to the menu strip for dynamic column selection
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
        /// <param name="column">Column to evaluate</param>
        /// <returns>True if column should be skipped</returns>
        private bool ShouldSkipColumn(DataGridViewColumn column)
        {
            return _columnsToSkip.Contains(column.Name) || column is DataGridViewImageColumn;
        }

        /// <summary>
        /// Gets the data type classification for a column
        /// </summary>
        /// <param name="column">Column to classify</param>
        /// <returns>Data type string (string or int)</returns>
        private string GetColumnDataType(DataGridViewColumn column)
        {
            if (column.ValueType == typeof(string) || column.ValueType == typeof(DateTime))
                return "string";
            return "int";
        }

        /// <summary>
        /// Updates the search columns selection based on menu choices
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
        /// Updates the flow panel with selected column labels
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
        /// Handles "All" item click in search menu to select/deselect all columns
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void SearchColumnItem_Click(object sender, EventArgs e)
        {
            UpdateSearchColumnsMenu();
        }

        #endregion

        #region Data Source Management

        /// <summary>
        /// Sets the data source with optional callback delegates for additional operations
        /// </summary>
        /// <param name="data">Data table to bind to the DataGridView</param>
        /// <param name="resizeHeaderCallback">Optional callback for resizing headers</param>
        /// <param name="addCaptionCallback">Optional callback for adding captions</param>
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
        /// Resets the data source and clears all associated data and grouping
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
        /// Checks if the DataGridView contains any data rows
        /// </summary>
        /// <returns>True if there are data rows available</returns>
        public bool HasRows()
        {
            return this.Rows.Count > 0 && !_isDataSourceEmpty;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Shows an error message dialog to the user
        /// </summary>
        /// <param name="message">Error message to display</param>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a warning message dialog to the user
        /// </summary>
        /// <param name="message">Warning message to display</param>
        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Shows an information message dialog to the user
        /// </summary>
        /// <param name="message">Information message to display</param>
        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the BeforeExport event to notify subscribers
        /// </summary>
        /// <param name="e">Export event arguments</param>
        protected virtual void OnBeforeExport(ExportEventArgs e)
        {
            BeforeExport?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the AfterExport event to notify subscribers
        /// </summary>
        /// <param name="e">Export event arguments</param>
        protected virtual void OnAfterExport(ExportEventArgs e)
        {
            AfterExport?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the GroupingChanged event to notify subscribers
        /// </summary>
        /// <param name="e">Grouping event arguments</param>
        protected virtual void OnGroupingChanged(GroupingEventArgs e)
        {
            GroupingChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the ColumnVisibilityChanged event to notify subscribers
        /// </summary>
        /// <param name="e">Column visibility event arguments</param>
        protected virtual void OnColumnVisibilityChanged(ColumnVisibilityEventArgs e)
        {
            ColumnVisibilityChanged?.Invoke(this, e);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes the entire DataGridView display and invalidates cached data
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
        /// Gets all currently visible column names
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
        /// Gets all currently hidden column names
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
        /// Sets the visibility of multiple columns at once
        /// </summary>
        /// <param name="columnVisibility">Dictionary mapping column names to their desired visibility</param>
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
        /// Applies a filter expression to the data source if it supports filtering
        /// </summary>
        /// <param name="filterExpression">Filter expression in DataTable syntax</param>
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
        /// Clears any applied data filters
        /// </summary>
        public void ClearFilter()
        {
            ApplyFilter(string.Empty);
        }

        /// <summary>
        /// Gets the count of currently visible rows
        /// </summary>
        /// <returns>Number of visible rows</returns>
        public int GetVisibleRowCount()
        {
            return this.Rows.Cast<DataGridViewRow>().Count(row => row.Visible);
        }

        /// <summary>
        /// Gets the current selection information
        /// </summary>
        /// <returns>Selection details including row and column counts</returns>
        public SelectionInfo GetSelectionInfo()
        {
            return new SelectionInfo
            {
                SelectedCellCount = this.SelectedCells.Count,
                SelectedRowCount = this.SelectedRows.Count,
                SelectedColumnCount = this.SelectedColumns.Count,
                HasSelection = this.SelectedCells.Count > 0
            };
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Information about current selection in the DataGridView
        /// </summary>
        public class SelectionInfo
        {
            /// <summary>Gets or sets the number of selected cells</summary>
            public int SelectedCellCount { get; set; }

            /// <summary>Gets or sets the number of selected rows</summary>
            public int SelectedRowCount { get; set; }

            /// <summary>Gets or sets the number of selected columns</summary>
            public int SelectedColumnCount { get; set; }

            /// <summary>Gets or sets whether there is any selection</summary>
            public bool HasSelection { get; set; }
        }

        #endregion

        #region Serialization Support

        /// <summary>
        /// Determines whether the ColumnsHeaderColor property should be serialized
        /// </summary>
        private bool ShouldSerializeColumnsHeaderColor()
        {
            return _headerBackColor != DEFAULT_HEADER_COLOR;
        }

        /// <summary>
        /// Resets the ColumnsHeaderColor property to its default value
        /// </summary>
        private void ResetColumnsHeaderColor()
        {
            ColumnsHeaderColor = DEFAULT_HEADER_COLOR;
        }

        /// <summary>
        /// Determines whether the AlternatingRowColor property should be serialized
        /// </summary>
        private bool ShouldSerializeAlternatingRowColor()
        {
            return _alternatingRowColor != DEFAULT_ALTERNATING_COLOR;
        }

        /// <summary>
        /// Resets the AlternatingRowColor property to its default value
        /// </summary>
        private void ResetAlternatingRowColor()
        {
            AlternatingRowColor = DEFAULT_ALTERNATING_COLOR;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Disposes the component and cleans up managed resources
        /// </summary>
        /// <param name="disposing">True if disposing managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _contextMenu?.Dispose();
                    _groupColors?.Clear();
                    _columnsType?.Clear();

                    // Clean up any remaining COM objects
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing AdvancedDataGridView: {ex.Message}");
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Designer Support

        /// <summary>
        /// Enables double buffering for smoother rendering
        /// </summary>
        protected override bool DoubleBuffered
        {
            get { return true; }
            set { base.DoubleBuffered = value; }
        }

        #endregion
    }
}