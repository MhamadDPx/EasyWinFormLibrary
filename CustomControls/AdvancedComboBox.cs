using EasyWinFormLibrary.Data;
using EasyWinFormLibrary.Extension;
using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced ComboBox control with database integration, custom search functionality, and enhanced styling.
    /// Provides real-time search capabilities, configurable data filtering, and comprehensive customization options.
    /// Features include automatic data loading, duplicate removal, read-only mode, and custom border styling.
    /// Optimized for .NET Framework 4.8 with async database operations and robust error handling.
    /// </summary>
    public class AdvancedComboBox : ComboBox
    {
        #region Constants

        /// <summary>
        /// Windows message constant for paint operations
        /// </summary>
        private const int WM_PAINT = 0xF;

        /// <summary>
        /// Default search delay in milliseconds
        /// </summary>
        private const int DEFAULT_SEARCH_DELAY = 300;

        /// <summary>
        /// Default maximum results limit
        /// </summary>
        private const int DEFAULT_MAX_RESULTS = 200;

        /// <summary>
        /// Default dropdown height
        /// </summary>
        private const int DEFAULT_DROPDOWN_HEIGHT = 200;

        /// <summary>
        /// Minimum search delay value
        /// </summary>
        private const int MIN_SEARCH_DELAY = 50;

        /// <summary>
        /// Maximum search delay value
        /// </summary>
        private const int MAX_SEARCH_DELAY = 2000;

        /// <summary>
        /// Minimum max results value
        /// </summary>
        private const int MIN_MAX_RESULTS = 1;

        /// <summary>
        /// Maximum max results value
        /// </summary>
        private const int MAX_MAX_RESULTS = 1000;

        #endregion

        #region Private Fields

        /// <summary>
        /// Database table name for data source
        /// </summary>
        private string _tableName = string.Empty;

        /// <summary>
        /// Database table columns to retrieve
        /// </summary>
        private string _tableColumns = string.Empty;

        /// <summary>
        /// Column names for filtering
        /// </summary>
        private string[] _columnFilterNames;

        /// <summary>
        /// Column types for filtering (string, int, etc.)
        /// </summary>
        private string[] _columnFilterTypes;

        /// <summary>
        /// Display member property for data binding
        /// </summary>
        private string _displayMember = string.Empty;

        /// <summary>
        /// Value member property for data binding
        /// </summary>
        private string _valueMember = string.Empty;

        /// <summary>
        /// Additional WHERE condition for database queries
        /// </summary>
        private string _whereCondition = string.Empty;

        /// <summary>
        /// Flag to enable or disable mouse wheel scrolling
        /// </summary>
        private bool _enableMouseScroll = false;

        /// <summary>
        /// Flag to automatically fill data when dropdown opens
        /// </summary>
        private bool _autoFillOnDropDown = false;

        /// <summary>
        /// Flag to remove duplicate entries from results
        /// </summary>
        private bool _removeDuplicates = false;

        /// <summary>
        /// Flag to indicate if this is a default ComboBox without database functionality
        /// </summary>
        private bool _isDefaultComboBox = false;

        /// <summary>
        /// Flag to make the ComboBox read-only
        /// </summary>
        private bool _isReadOnly = false;

        /// <summary>
        /// Flag to reset text when selected value is not found
        /// </summary>
        private bool _resetWhenValueNotFound = true;

        /// <summary>
        /// Custom border color
        /// </summary>
        private Color _borderColor = Color.FromArgb(213, 218, 223);

        /// <summary>
        /// Search delay in milliseconds
        /// </summary>
        private int _searchDelayMs = DEFAULT_SEARCH_DELAY;

        /// <summary>
        /// Maximum number of results to retrieve
        /// </summary>
        private int _maxResults = DEFAULT_MAX_RESULTS;

        /// <summary>
        /// Width of the dropdown button
        /// </summary>
        private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the border color of the ComboBox
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Appearance")]
        [DefaultValue(typeof(Color), "213, 218, 223")]
        [Description("The border color of the ComboBox")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        /// <summary>
        /// Gets or sets the database table name for data source
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Data")]
        [DefaultValue("")]
        [Description("Database table name for data source")]
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// Gets or sets the database table columns to retrieve
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Data")]
        [DefaultValue("")]
        [Description("Database table columns to retrieve (e.g., 'ID, Name, Description')")]
        public string TableColumns
        {
            get { return _tableColumns; }
            set { _tableColumns = value; }
        }

        /// <summary>
        /// Gets or sets the column names used for filtering during search
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Data")]
        [Description("Column names used for filtering during search")]
        public string[] ColumnFilterName
        {
            get { return _columnFilterNames; }
            set { _columnFilterNames = value; }
        }

        /// <summary>
        /// Gets or sets the column types for filtering (string, int, etc.)
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Data")]
        [Description("Column types for filtering (string, int, etc.)")]
        public string[] ColumnFilterType
        {
            get { return _columnFilterTypes; }
            set { _columnFilterTypes = value; }
        }

        /// <summary>
        /// Gets or sets the display member property for data binding
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Data")]
        [DefaultValue("")]
        [Description("Display member property for data binding")]
        public string CmbDisplayMember
        {
            get { return _displayMember; }
            set { _displayMember = value; }
        }

        /// <summary>
        /// Gets or sets the value member property for data binding
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Data")]
        [DefaultValue("")]
        [Description("Value member property for data binding")]
        public string CmbValueMember
        {
            get { return _valueMember; }
            set { _valueMember = value; }
        }

        /// <summary>
        /// Gets or sets additional WHERE condition for database queries
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Data")]
        [DefaultValue("")]
        [Description("Additional WHERE condition for database queries")]
        public string WhereCondition
        {
            get { return _whereCondition; }
            set { _whereCondition = value; }
        }

        /// <summary>
        /// Gets or sets whether mouse wheel scrolling is enabled
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Behavior")]
        [DefaultValue(false)]
        [Description("Enables or disables mouse wheel scrolling")]
        public bool EnableMouseScroll
        {
            get { return _enableMouseScroll; }
            set { _enableMouseScroll = value; }
        }

        /// <summary>
        /// Gets or sets whether to automatically fill data when dropdown opens
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Behavior")]
        [DefaultValue(false)]
        [Description("Automatically fills data when dropdown opens")]
        public bool AutoFillOnDropDown
        {
            get { return _autoFillOnDropDown; }
            set { _autoFillOnDropDown = value; }
        }

        /// <summary>
        /// Gets or sets whether to remove duplicate entries from results
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Behavior")]
        [DefaultValue(false)]
        [Description("Removes duplicate entries from search results")]
        public bool RemoveDuplicates
        {
            get { return _removeDuplicates; }
            set { _removeDuplicates = value; }
        }

        /// <summary>
        /// Gets or sets whether this is a default ComboBox without database functionality
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Behavior")]
        [DefaultValue(false)]
        [Description("Indicates if this is a default ComboBox without database functionality")]
        public bool IsDefaultComboBox
        {
            get { return _isDefaultComboBox; }
            set { _isDefaultComboBox = value; }
        }

        /// <summary>
        /// Gets or sets whether the ComboBox is read-only
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Behavior")]
        [DefaultValue(false)]
        [Description("Makes the ComboBox read-only")]
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }

        /// <summary>
        /// Gets or sets whether to reset text when selected value is not found
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Behavior")]
        [DefaultValue(true)]
        [Description("Resets text when selected value is not found")]
        public bool ResetWhenValueNotFound
        {
            get { return _resetWhenValueNotFound; }
            set { _resetWhenValueNotFound = value; }
        }

        /// <summary>
        /// Gets the selected ID value safely, returns "1" if no valid selection
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedID
        {
            get
            {
                return SelectedValue is null || Text.IsEmpty() || this.DataSource is null ? "1" : SelectedValue.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the delay in milliseconds before performing search
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Performance")]
        [DefaultValue(DEFAULT_SEARCH_DELAY)]
        [Description("Delay in milliseconds before performing search")]
        public int SearchDelayMs
        {
            get => _searchDelayMs;
            set { _searchDelayMs = Math.Max(MIN_SEARCH_DELAY, Math.Min(MAX_SEARCH_DELAY, value)); }
        }

        /// <summary>
        /// Gets or sets the maximum number of results to retrieve from database
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Performance")]
        [DefaultValue(DEFAULT_MAX_RESULTS)]
        [Description("Maximum number of results to retrieve from database")]
        public int MaxResults
        {
            get => _maxResults;
            set { _maxResults = Math.Max(MIN_MAX_RESULTS, Math.Min(MAX_MAX_RESULTS, value)); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedComboBox class
        /// </summary>
        public AdvancedComboBox()
        {
            InitializeComponent();
            InitializeEventHandlers();
            SetDefaultProperties();
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Initializes the component properties and SQL actions
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "AdvancedComboBox";
            //SqlActions = new SqlDatabaseActions(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig);
            this.ResumeLayout(true);
            this.FlatStyle = FlatStyle.Flat;
        }

        /// <summary>
        /// Initializes event handlers for the control
        /// </summary>
        private void InitializeEventHandlers()
        {
            this.DropDown += AdvancedComboBox_DropDown;
            this.KeyUp += AdvancedComboBox_KeyUp;
            this.KeyDown += AdvancedComboBox_KeyDown;
            this.KeyPress += AdvancedComboBox_KeyPress;
            this.MouseWheel += AdvancedComboBox_MouseWheel;
            this.Leave += AdvancedComboBox_Leave;
        }

        /// <summary>
        /// Sets default properties for the control
        /// </summary>
        private void SetDefaultProperties()
        {
            this.DropDownHeight = DEFAULT_DROPDOWN_HEIGHT;
            this.DrawMode = DrawMode.Normal;
            this.DoubleBuffered(true);
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Processes Windows messages to handle custom painting
        /// </summary>
        /// <param name="m">Windows message</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && DropDownStyle != ComboBoxStyle.Simple)
            {
                DrawCustomBorder();
            }
        }

        /// <summary>
        /// Handles the resize event to trigger repainting
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate(); // Redraw the control when resized
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the dropdown event to auto-fill data if enabled
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Event arguments</param>
        private async void AdvancedComboBox_DropDown(object sender, EventArgs e)
        {
            if (_isDefaultComboBox) return;

            if (_autoFillOnDropDown && this.Text.IsEmpty())
            {
                await LoadData();
            }
        }

        /// <summary>
        /// Handles key up events to trigger search functionality
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Key event arguments</param>
        private void AdvancedComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Prevent SQL injection by removing single quotes
            if (this.Text.Contains("'"))
            {
                this.ResetText();
                return;
            }

            if (_isDefaultComboBox || _isReadOnly) return;

            // Trigger search for non-navigation keys
            if (IsSearchTriggerKey(e.KeyCode))
            {
                TaskDelayUtils.TaskDelay(async () => { await SearchItem(); }, _searchDelayMs);
            }
        }

        /// <summary>
        /// Handles key down events for navigation and Enter key behavior
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Key event arguments</param>
        private void AdvancedComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HandleEnterKey(e, sender as Control);
            }

            if (!_enableMouseScroll && !DroppedDown && IsNavigationKey(e.KeyCode))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles key press events for read-only mode
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Key press event arguments</param>
        private void AdvancedComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_isReadOnly)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles mouse wheel events based on scroll settings
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Mouse event arguments</param>
        private void AdvancedComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = !_enableMouseScroll;
        }

        /// <summary>
        /// Handles leave event to reset text if value not found
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Event arguments</param>
        private void AdvancedComboBox_Leave(object sender, EventArgs e)
        {
            if ((!_resetWhenValueNotFound && this.DataSource != null) || _isDefaultComboBox)
                return;

            if (!this.Text.IsEmpty() && this.SelectedValue is null)
            {
                this.ResetText();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Draws custom border for the ComboBox
        /// </summary>
        private void DrawCustomBorder()
        {
            try
            {
                using (var graphics = Graphics.FromHwnd(Handle))
                using (var pen = new Pen(_borderColor))
                {
                    // Draw outer border
                    graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

                    // Draw separator line for dropdown button
                    var offset = FlatStyle == FlatStyle.Popup ? 1 : 0;
                    graphics.DrawLine(pen, Width - buttonWidth - offset, 0,
                                    Width - buttonWidth - offset, Height);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error drawing custom border: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads data from the database asynchronously
        /// </summary>
        private async Task LoadData()
        {
            try
            {
                string distinctClause = _removeDuplicates ? "" : "DISTINCT";
                string whereClause = !_whereCondition.IsEmpty() ? $" WHERE {_whereCondition}" : "";
                string query = $"SELECT {distinctClause} TOP {_maxResults} {TableColumns} FROM {_tableName}{whereClause}";

                var sqlData = await SqlDatabaseActions.GetDataAsync(query);

                this.DataSource = sqlData.Data;
                this.DisplayMember = _displayMember;
                this.ValueMember = _valueMember;
                this.SelectedIndex = -1;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Searches for items based on the current text input
        /// </summary>
        private async Task SearchItem()
        {
            try
            {
                string beforeSearchText = this.Text.Trim();
                string searchCondition = MakeSearchColumn(_columnFilterNames, _columnFilterTypes,
                                                        beforeSearchText.Split());

                string distinctClause = !_removeDuplicates ? "" : "DISTINCT";
                string whereClause = !_whereCondition.IsEmpty() ? $"AND {_whereCondition}" : "";
                string query = $"SELECT {distinctClause} TOP {_maxResults} {_tableColumns} FROM {_tableName} " +
                             $"WHERE {searchCondition} {whereClause}";

                var sqlData = await SqlDatabaseActions.GetDataAsync(query);

                if (sqlData.Data.Rows.Count > 0)
                {
                    UpdateDataSourceAndShowDropdown(sqlData.Data);
                }
                else
                {
                    RefreshDropdownStyle();
                }

                RestoreTextAndCursor(beforeSearchText);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching items: {ex.Message}");
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Updates the data source and shows the dropdown
        /// </summary>
        /// <param name="data">Data to bind to the ComboBox</param>
        private void UpdateDataSourceAndShowDropdown(System.Data.DataTable data)
        {
            this.DataSource = data;
            this.DisplayMember = _displayMember;
            this.ValueMember = _valueMember;
            this.DroppedDown = true;
            Cursor.Current = Cursors.Default;

            if (this.Text.IsEmpty())
            {
                RefreshDropdownStyle();
            }
        }

        /// <summary>
        /// Refreshes the dropdown style to trigger visual update
        /// </summary>
        private void RefreshDropdownStyle()
        {
            this.DropDownStyle = ComboBoxStyle.Simple;
            this.DropDownStyle = ComboBoxStyle.DropDown;
        }

        /// <summary>
        /// Restores text and cursor position after search
        /// </summary>
        /// <param name="text">Text to restore</param>
        private void RestoreTextAndCursor(string text)
        {
            this.Text = text;
            this.SelectionStart = this.Text.Length;
        }

        /// <summary>
        /// Handles Enter key press for tab navigation
        /// </summary>
        /// <param name="e">Key event arguments</param>
        /// <param name="currentControl">Current control</param>
        private void HandleEnterKey(KeyEventArgs e, Control currentControl)
        {
            if (!TabStop) return;

            e.SuppressKeyPress = true;
            var nextControl = FindForm().GetNextControl(currentControl, true);

            while (nextControl != null && !nextControl.TabStop)
            {
                nextControl = FindForm().GetNextControl(nextControl, true);
            }

            nextControl?.Focus();
        }

        /// <summary>
        /// Checks if the key code should trigger a search
        /// </summary>
        /// <param name="keyCode">Key code to check</param>
        /// <returns>True if key should trigger search</returns>
        private bool IsSearchTriggerKey(Keys keyCode)
        {
            return keyCode != Keys.Up && keyCode != Keys.Down &&
                   keyCode != Keys.Left && keyCode != Keys.Right &&
                   keyCode != Keys.Enter;
        }

        /// <summary>
        /// Checks if the key code is a navigation key
        /// </summary>
        /// <param name="keyCode">Key code to check</param>
        /// <returns>True if key is a navigation key</returns>
        private bool IsNavigationKey(Keys keyCode)
        {
            return keyCode == Keys.Up || keyCode == Keys.Down;
        }

        /// <summary>
        /// Creates a search condition string for multiple columns and values
        /// </summary>
        /// <param name="columnNames">Array of column names to search</param>
        /// <param name="columnTypes">Array of column types (string, int, etc.)</param>
        /// <param name="searchValues">Array of values to search for</param>
        /// <returns>SQL WHERE condition string</returns>
        private string MakeSearchColumn(string[] columnNames, string[] columnTypes, string[] searchValues)
        {
            if (columnNames == null || columnTypes == null || searchValues == null)
                return "1=1"; // Return safe condition if parameters are null

            string query = "";

            foreach (string searchValue in searchValues)
            {
                if (string.IsNullOrWhiteSpace(searchValue)) continue;

                string columnCondition = "(";

                for (int i = 0; i < columnNames.Length && i < columnTypes.Length; i++)
                {
                    if (columnTypes[i] == "string")
                    {
                        columnCondition += $"ISNULL({columnNames[i]},'')+' '+";
                    }
                    else if (columnTypes[i] == "int")
                    {
                        columnCondition += $"CONVERT(nvarchar,ISNULL({columnNames[i]},''))+' '+";
                    }
                }

                columnCondition += $"'') LIKE N'%{searchValue.Replace("'", "''")}%' AND ";
                query += columnCondition;
            }

            return query.Length > 4 ? query.Substring(0, query.Length - 4) : "1=1";
        }
        #endregion
    }
}