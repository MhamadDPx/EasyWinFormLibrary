using EasyWinFormLibrary.Data;
using EasyWinFormLibrary.Extension;
using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    public class AdvancedComboBox : ComboBox
    {
        private string _tableName = string.Empty;
        private string _tableColumns = string.Empty;
        private string[] _columnFilterNames;
        private string[] _columnFilterTypes;
        private string _displayMember = string.Empty;
        private string _valueMember = string.Empty;
        private string _whereCondition = string.Empty;
        private bool _enableMouseScroll = false;
        private bool _autoFillOnDropDown = false;
        private bool _removeDuplicates = false;
        private bool _isDefaultComboBox = false;
        private bool _isReadOnly = false;
        private bool _resetWhenValueNotFound = true;
        private Color _borderColor = Color.FromArgb(213, 218, 223);
        private int _searchDelayMs = 300;
        private int _maxResults = 200;
        private SqlDatabaseActions SqlActions;

        /// <summary>
        /// Border color of the ComboBox
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Appearance")]
        [DefaultValue(typeof(Color), "213, 218, 223")]
        [Description("The border color of the ComboBox")]
        public Color BorderColor { get { return _borderColor; } set { _borderColor = value; Invalidate(); } }

        /// <summary>
        /// Database table name for data source
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Appearance")]
        [DefaultValue("")]
        [Description("Database table name for data source")]
        public string TableName { get { return _tableName; } set { _tableName = value; } }

        [Category("Advanced Appearance")]
        public string TableColumns { get { return _tableColumns; } set { _tableColumns = value; } }
        [Category("Advanced Appearance")]
        public string[] ColumnFilterName { get { return _columnFilterNames; } set { _columnFilterNames = value; } }
        [Category("Advanced Appearance")]
        public string[] ColumnFilterType { get { return _columnFilterTypes; } set { _columnFilterTypes = value; } }
        [Category("Advanced Appearance")]
        public string CmbDisplayMember { get { return _displayMember; } set { _displayMember = value; } }
        [Category("Advanced Appearance")]
        public string CmbValueMember { get { return _valueMember; } set { _valueMember = value; } }
        [Category("Advanced Appearance")]
        public string WhereCondition { get { return _whereCondition; } set { _whereCondition = value; } }
        [Category("Advanced Appearance")]
        public bool EnableMouseScroll { get { return _enableMouseScroll; } set { _enableMouseScroll = value; } }
        [Category("Advanced Appearance")]
        public bool AutoFillOnDropDown { get { return _autoFillOnDropDown; } set { _autoFillOnDropDown = value; } }
        [Category("Advanced Appearance")]
        public bool RemoveDuplicates { get { return _removeDuplicates; } set { _removeDuplicates = value; } }
        [Category("Advanced Appearance")]
        public bool IsDefaultComboBox { get { return _isDefaultComboBox; } set { _isDefaultComboBox = value; } }
        [Category("Advanced Appearance")]
        public bool IsReadOnly { get { return _isReadOnly; } set { _isReadOnly = value; } }
        [Category("Advanced Appearance")]
        public bool ResetWhenValueNotFound { get { return _resetWhenValueNotFound; } set { _resetWhenValueNotFound = value; } }
        /// <summary>
        /// Gets the selected ID value safely
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
        /// Delay in milliseconds before performing search
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Appearance")]
        [DefaultValue(300)]
        [Description("Delay in milliseconds before performing search")]
        public int SearchDelayMs
        {
            get => _searchDelayMs;
            set { _searchDelayMs = Math.Max(50, Math.Min(2000, value)); }
        }
        /// <summary>
        /// Maximum number of results to retrieve from database
        /// </summary>
        [Browsable(true)]
        [Category("Advanced Appearance")]
        [DefaultValue(200)]
        [Description("Maximum number of results to retrieve from database")]
        public int MaxResults
        {
            get => _maxResults;
            set { _maxResults = Math.Max(1, Math.Min(1000, value)); }
        }

        private const int WM_PAINT = 0xF;
        private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT && DropDownStyle != ComboBoxStyle.Simple)
            {
                using (var g = Graphics.FromHwnd(Handle))
                {
                    using (var p = new Pen(_borderColor))
                    {
                        g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);

                        var d = FlatStyle == FlatStyle.Popup ? 1 : 0;
                        g.DrawLine(p, Width - buttonWidth - d,
                            0, Width - buttonWidth - d, Height);
                    }
                }
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate(); // Redraw the control when resized
        }
        public AdvancedComboBox()
        {
            this.SuspendLayout();
            this.Name = "AdvancedComboBox";
            SqlActions = new SqlDatabaseActions(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig);
            this.DropDownHeight = 200;
            this.DropDown += AdvancedComboBox_DropDown;
            this.KeyUp += new KeyEventHandler(AdvancedComboBox_KeyUp);
            this.KeyDown += new KeyEventHandler(AdvancedComboBox_KeyDown);
            this.KeyPress += new KeyPressEventHandler(AdvancedComboBox_KeyPress);
            this.MouseWheel += new MouseEventHandler(AdvancedComboBox_MouseWheel);
            this.Leave += AdvancedComboBox_Leave;
            this.DrawMode = DrawMode.Normal;
            this.DoubleBuffered(true);
            this.ResumeLayout(true);
        }


        private void AdvancedComboBox_Leave(object sender, EventArgs e)
        {
            if ((!_resetWhenValueNotFound && this.DataSource != null) || _isDefaultComboBox) return;

            if (!this.Text.IsEmpty() && this.SelectedValue is null)
            {
                this.ResetText();
            }
        }

        private void AdvancedComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_isReadOnly)
                e.Handled = true;
        }
        private async void AdvancedComboBox_DropDown(object sender, EventArgs e)
        {
            if (_isDefaultComboBox) return;

            if (_autoFillOnDropDown && this.Text.IsEmpty())
            {
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            string Query = $"SELECT {(_removeDuplicates ? "" : "DISTINCT")} TOP {_maxResults} {TableColumns} FROM {_tableName} {(!_whereCondition.IsEmpty() ? $" WHERE {_whereCondition}" : "")}";
            var SqlData = await SqlActions.GetDataAsync(Query);

            this.DataSource = SqlData.Data;
            this.DisplayMember = _displayMember;
            this.ValueMember = _valueMember;
            this.SelectedIndex = -1;
            Cursor.Current = Cursors.Default;

        }

        private void AdvancedComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.Text.Contains("'"))
                this.ResetText();

            if (_isDefaultComboBox || _isReadOnly) return;

            if (e.KeyCode != Keys.Up
                && e.KeyCode != Keys.Down
                && e.KeyCode != Keys.Left
                && e.KeyCode != Keys.Right
                && e.KeyCode != Keys.Enter)
            {
                TaskDelayUtils.TaskDelay(async () => { await SearchItem(); }, _searchDelayMs);
            }
        }
        private void AdvancedComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!TabStop) return;

                e.SuppressKeyPress = true;
                var nextControl = FindForm().GetNextControl(sender as Control, true);
                while (nextControl != null && !nextControl.TabStop)
                {
                    nextControl = FindForm().GetNextControl(nextControl, true);
                }

                nextControl?.Focus();
            }

            if (_enableMouseScroll == false)
                if (!DroppedDown)
                    if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                        e.Handled = true;
        }

        private async Task SearchItem()
        {
            string BeforeSeachText = this.Text.Trim();
            string query = $"SELECT {(!_removeDuplicates ? "" : "DISTINCT")} TOP {_maxResults} {_tableColumns} FROM {_tableName} WHERE {MakeSearchColumn(_columnFilterNames, _columnFilterTypes, this.Text.Trim().Split())} {(!_whereCondition.IsEmpty() ? $"AND {_whereCondition}" : "")}";
            var SqlData = await SqlActions.GetDataAsync(query);

            if (SqlData.Data.Rows.Count > 0)
            {
                this.DataSource = SqlData.Data;
                this.DisplayMember = _displayMember;
                this.ValueMember = _valueMember;
                this.DroppedDown = true;

                Cursor.Current = Cursors.Default;

                if (this.Text.IsEmpty())
                {
                    this.DropDownStyle = ComboBoxStyle.Simple;
                    this.DropDownStyle = ComboBoxStyle.DropDown;
                }
            }
            else
            {
                this.DropDownStyle = ComboBoxStyle.Simple;
                this.DropDownStyle = ComboBoxStyle.DropDown;
            }

            this.Text = BeforeSeachText;
            this.SelectionStart = this.Text.Length;
        }
        public static string MakeSearchColumn(string[] cName, string[] cType, string[] values)
        {
            string query = "";

            foreach (string item in values)
            {
                string column = "(";
                for (int i = 0; i < cName.Length; i++)
                {
                    if (cType[i] == "string")
                        column += $"ISNULL({cName[i]},'')+' '+";
                    else if (cType[i] == "int")
                        column += $"CONVERT(nvarchar,ISNULL({cName[i]},''))+' '+";
                }

                column += $"'') LIKE N'%{item}%' AND ";
                query += column;
            }

            return query.Substring(0, query.Length - 4);
        }
        private void AdvancedComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = !_enableMouseScroll;
        }

    }
}
