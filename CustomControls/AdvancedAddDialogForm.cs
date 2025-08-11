using EasyWinFormLibrary.Data;
using EasyWinFormLibrary.Extension;
using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced Add Dialog Form for generic CRUD operations with multi-language support.
    /// Provides a unified interface for adding, editing, and deleting records from any database table.
    /// Features automatic data binding, localized messages, and role-based permission checking.
    /// Optimized for .NET Framework 4.8 with comprehensive error handling and user feedback.
    /// </summary>
    public partial class AdvancedAddDialogForm : Form
    {
        #region Private Fields

        /// <summary>
        /// Primary ID of the currently selected record for update operations
        /// </summary>
        private string _primaryID = "0";

        /// <summary>
        /// The name of the database table to perform operations on
        /// </summary>
        private string _tableName;

        /// <summary>
        /// The name of the primary key column in the target table
        /// </summary>
        private string _primaryKeyColumn;

        /// <summary>
        /// The name of the text/content column in the target table
        /// </summary>
        private string _textColumn;

        /// <summary>
        /// The title text to display in the form header
        /// </summary>
        private string _lblTitleText;

        /// <summary>
        /// The placeholder text to display for the input field
        /// </summary>
        private string _lblPlaceHolderText;

        /// <summary>
        /// The header text for the data grid view column
        /// </summary>
        private string _dgvColumnHeaderText;

        /// <summary>
        /// Additional WHERE condition for filtering data
        /// </summary>
        private string _whereCondition;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the form role for permission checking
        /// </summary>
        public string FormRole { get; set; } = "";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedAddDialogForm class
        /// </summary>
        /// <param name="tableName">The name of the database table to perform operations on</param>
        /// <param name="primaryKeyColumn">The name of the primary key column</param>
        /// <param name="textColumn">The name of the text/content column</param>
        /// <param name="lblTitleText">The title text for the form header</param>
        /// <param name="lblPlaceHolderText">The placeholder text for the input field</param>
        /// <param name="dgvColumnHeaderText">The header text for the data grid view column</param>
        /// <param name="whereCondition">Optional WHERE condition for filtering data</param>
        public AdvancedAddDialogForm(string tableName, string primaryKeyColumn, string textColumn,
            string lblTitleText, string lblPlaceHolderText, string dgvColumnHeaderText, string whereCondition = "")
        {
            InitializeComponent();

            // Initialize private fields
            this._tableName = tableName;
            this._primaryKeyColumn = primaryKeyColumn;
            this._textColumn = textColumn;
            this._lblTitleText = lblTitleText;
            this._lblPlaceHolderText = lblPlaceHolderText;
            this._dgvColumnHeaderText = dgvColumnHeaderText;
            this._whereCondition = whereCondition;
            this.KeyPreview = true;

            // Set RTL layout based on selected language
            SetLanguageLayout();
        }

        #endregion

        #region Form Events

        /// <summary>
        /// Handles the form load event to initialize controls and load data
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Event arguments</param>
        private void AdvancedAddDialogForm_Load(object sender, EventArgs e)
        {
            InitializeFormControls();
            RefreshDGV();
            txt.Focus();
        }

        /// <summary>
        /// Handles form key down events for shortcuts
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Key event arguments</param>
        private void AdvancedAddDialogForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        #endregion

        #region Control Events

        /// <summary>
        /// Handles the save button click event for insert/update operations
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Event arguments</param>
        private async void btnClick(object sender, EventArgs e)
        {
            try
            {
                if (_primaryID == "0")
                {
                    await HandleInsertOperation();
                }
                else
                {
                    await HandleUpdateOperation();
                }

                // Reset form state after operation
                ResetFormState();
            }
            catch (Exception ex)
            {
                ShowErrorAlert($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles data grid view cell click events for update/delete operations
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Data grid view cell event arguments</param>
        private async void dgvCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _primaryID = DGV.CurrentRow.Cells[_primaryKeyColumn].Value.ToString();
            string columnName = DGV.Columns[e.ColumnIndex].Name;

            try
            {
                if (columnName == "deleteBtn")
                {
                    await HandleDeleteOperation();
                }
                else if (columnName == "updateBtn")
                {
                    HandleEditOperation();
                }
            }
            catch (Exception ex)
            {
                ShowErrorAlert($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles text box key up events for Enter key shortcut
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Key event arguments</param>
        private void txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSave.PerformClick();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the language layout based on the selected language
        /// </summary>
        private void SetLanguageLayout()
        {
            this.RightToLeft = LanguageManager.SelectedLanguage == FormLanguage.English ? RightToLeft.No : RightToLeft.Yes;
            this.RightToLeftLayout = LanguageManager.SelectedLanguage != FormLanguage.English;
            txt.TextAlign = LanguageManager.SelectedLanguage == FormLanguage.English ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        /// <summary>
        /// Initializes form controls with localized content
        /// </summary>
        private void InitializeFormControls()
        {
            formTopMenuBar1.FormTitlEnglish = _lblTitleText;
            formTopMenuBar1.FormTitlArabic = _lblTitleText;
            formTopMenuBar1.FormTitlKurdish = _lblTitleText;
            lbl.Text = _lblPlaceHolderText;
        }

        /// <summary>
        /// Handles the insert operation for new records
        /// </summary>
        private async Task HandleInsertOperation()
        {
            // Check permissions
            if (!string.IsNullOrEmpty(FormRole) && !AuthUserInfo.HasPermission(FormRole, AuthUserInfo.PermissionType.Insert))
                return;

            // Validate input
            if (!tableLayoutPanel1.CheckEmptyText())
                return;

            // Check for duplicates
            if (await CheckDuplicateRecord())
            {
                ShowDuplicateAlert();
                return;
            }

            // Insert new record
            var insertResult = await SqlDatabaseActions.InsertDataAsync(_tableName,
                new Dictionary<string, object>()
                {
                    { _textColumn, txt.Text.Trim() },
                    { "archived", 0 }
                }, false, false);

            if (insertResult.Success)
            {
                ShowInsertSuccessAlert();
            }
            else
            {
                ShowErrorAlert(insertResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Handles the update operation for existing records
        /// </summary>
        private async Task HandleUpdateOperation()
        {
            // Check permissions
            if (!string.IsNullOrEmpty(FormRole) && !AuthUserInfo.HasPermission(FormRole, AuthUserInfo.PermissionType.Update))
                return;

            // Check for duplicates excluding current record
            if (await CheckDuplicateRecordForUpdate())
            {
                ShowDuplicateAlert();
                return;
            }

            // Update existing record
            var updateResult = await SqlDatabaseActions.UpdateDataAsync(_tableName,
                new Dictionary<string, object>() { { _textColumn, txt.Text.Trim() } },
                new Dictionary<string, object>() { { _primaryKeyColumn, _primaryID } }, false, false);

            if (updateResult.Success)
            {
                ShowUpdateSuccessAlert();
            }
            else
            {
                ShowErrorAlert(updateResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Handles the delete operation for selected records
        /// </summary>
        private async Task HandleDeleteOperation()
        {
            // Check permissions
            if (!string.IsNullOrEmpty(FormRole) && !AuthUserInfo.HasPermission(FormRole, AuthUserInfo.PermissionType.Delete))
                return;

            // Confirm deletion
            if (!ShowDeleteConfirmation())
                return;

            // Mark record as archived (soft delete)
            await SqlDatabaseActions.UpdateDataAsync(_tableName,
                new Dictionary<string, object>() { { "archived", 1 } },
                new Dictionary<string, object>() { { _primaryKeyColumn, _primaryID } }, false, false);

            ResetFormState();
        }

        /// <summary>
        /// Handles the edit operation by selecting record data into the text box
        /// </summary>
        private void HandleEditOperation()
        {
            SelectDataToTextBox();
            txt.Focus();
            txt.SelectAll();
        }

        /// <summary>
        /// Checks if a duplicate record exists for insert operations
        /// </summary>
        /// <returns>True if duplicate exists, false otherwise</returns>
        private async Task<bool> CheckDuplicateRecord()
        {
            var result = await SqlDatabaseActions.HasDataAsync($"SELECT * FROM {_tableName} WHERE {_textColumn}=N'{txt.Text.Trim()}'");
            return result.HasData;
        }

        /// <summary>
        /// Checks if a duplicate record exists for update operations (excluding current record)
        /// </summary>
        /// <returns>True if duplicate exists, false otherwise</returns>
        private async Task<bool> CheckDuplicateRecordForUpdate()
        {
            var result = await SqlDatabaseActions.HasDataAsync($"SELECT * FROM {_tableName} WHERE {_textColumn}=N'{txt.Text.Trim()}' AND {_primaryKeyColumn}<>{_primaryID}");
            return result.HasData;
        }

        /// <summary>
        /// Refreshes the data grid view with current data from the database
        /// </summary>
        private async void RefreshDGV()
        {
            try
            {
                DGV.Columns.Clear();

                string whereClause = _whereCondition.IsEmpty() ? "" : $" AND {_whereCondition}";
                string query = $"SELECT {_primaryKeyColumn},{_textColumn} FROM {_tableName} WHERE archived=0{whereClause} ORDER BY {_primaryKeyColumn} DESC";

                var result = await SqlDatabaseActions.GetDataAsync(query);
                DGV.DataSource = result.Data;

                ConfigureDataGridViewColumns();
                txt.Text = "";
            }
            catch (Exception ex)
            {
                ShowErrorAlert($"Error refreshing data: {ex.Message}");
            }
        }

        /// <summary>
        /// Configures the data grid view column headers and properties
        /// </summary>
        private void ConfigureDataGridViewColumns()
        {
            // Add action columns for update and delete
            DGV.AddActionColumns(DataGridViewExtentions.ButtonImage.Update, DataGridViewExtentions.ButtonImage.Delete);

            // Set column widths
            DGV.Columns[_primaryKeyColumn].Width = 40;
            DGV.Columns["updateBtn"].Width = 30;
            DGV.Columns["deleteBtn"].Width = 30;

            // Set column headers
            DGV.Columns[_primaryKeyColumn].HeaderText = "#";
            DGV.Columns[_textColumn].HeaderText = _dgvColumnHeaderText;
        }

        /// <summary>
        /// Selects the current row data and populates the text box for editing
        /// </summary>
        private void SelectDataToTextBox()
        {
            if (DGV.Rows.Count <= 0) return;
            txt.Text = DGV.CurrentRow.Cells[_textColumn].Value.ToString();
        }

        /// <summary>
        /// Resets the form state after operations
        /// </summary>
        private void ResetFormState()
        {
            _primaryID = "0";
            txt.Focus();
            RefreshDGV();
        }

        #endregion

        #region Alert Methods

        /// <summary>
        /// Shows a duplicate record alert in multiple languages
        /// </summary>
        private void ShowDuplicateAlert()
        {
            AdvancedAlert.ShowAlert("ببورە پێشتر زیادکراوە", "آسف، تمت إضافتها من قبل", "Sorry it's already been added",
                AdvancedAlert.AlertType.Warning, 3);
        }

        /// <summary>
        /// Shows a success alert for insert operations
        /// </summary>
        private void ShowInsertSuccessAlert()
        {
            AdvancedAlert.ShowAlert("بە سەرکەوتووی زیادکرا", "تم الادخال بنجاح", "Inserted successfully",
                AdvancedAlert.AlertType.Success, 3);
        }

        /// <summary>
        /// Shows a success alert for update operations
        /// </summary>
        private void ShowUpdateSuccessAlert()
        {
            AdvancedAlert.ShowAlert("بەسەرکەوتووی نوێکرایەوە", "تم التحديث بنجاح", "Updated successfully",
                AdvancedAlert.AlertType.Success, 3);
        }

        /// <summary>
        /// Shows an error alert with the specified message
        /// </summary>
        /// <param name="message">The error message to display</param>
        private void ShowErrorAlert(string message)
        {
            AdvancedAlert.ShowAlert("هەڵەیەک ڕوویدا", "حدث خطأ", message,
                AdvancedAlert.AlertType.Error, 3);
        }

        /// <summary>
        /// Shows a confirmation dialog for delete operations
        /// </summary>
        /// <returns>True if user confirms deletion, false otherwise</returns>
        private bool ShowDeleteConfirmation()
        {
            return AdvancedMessageBox.ShowMessageBox(
                "دڵنیای لە ئەنجامدانی ئەم کردارە؟",
                "هل انت متأكد من أنك تريد أن تفعل هذا؟",
                "Are you sure ?",
                "سڕینەوە",
                "مسح",
                "Delete",
                AdvancedMessageBox.MessageBoxType.YesNo);
        }

        #endregion
    }
}