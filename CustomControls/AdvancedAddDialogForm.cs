using EasyWinFormLibrary.Data;
using EasyWinFormLibrary.Extension;
using EasyWinFormLibrary.Properties;
using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    public partial class AdvancedAddDialogForm : Form
    {
        private static SqlDatabaseActions actions = new SqlDatabaseActions(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig);
        private string _primaryID = "0";
        public string FormRole = "";
        private string _tableName;
        private string _primaryKeyColumn;
        private string _textColumn;
        private string _lblTitleText;
        private string _lblPlaceHolderText;
        private string _dgvColumnHeaderText;
        private string _whereCondition;

        public AdvancedAddDialogForm(string tableName, string primaryKeyColumn, string textColumn, string lblTitleText, string lblPlaceHolderText, string dgvColumnHeaderText, string whereCondition = "")
        {
            InitializeComponent();
            this._tableName = tableName;
            this._primaryKeyColumn = primaryKeyColumn;
            this._textColumn = textColumn;
            this._lblTitleText = lblTitleText;
            this._lblPlaceHolderText = lblPlaceHolderText;
            this._dgvColumnHeaderText = dgvColumnHeaderText;
            this._whereCondition = whereCondition;

            this.RightToLeft = LanguageManager.SelectedLanguage == FormLanguage.English ? RightToLeft.No : RightToLeft.Yes;
            this.RightToLeftLayout = LanguageManager.SelectedLanguage != FormLanguage.English;
        }


        private void AdvancedAddDialogForm_Load(object sender, EventArgs e)
        {
            formTopMenuBar1.FormTitlEnglish = formTopMenuBar1.FormTitlArabic = formTopMenuBar1.FormTitlKurdish = _lblTitleText;
            lbl.Text = _lblPlaceHolderText;
            RefreshDGV();
            txt.Focus();
        }
        private async void btnClick(object sender, EventArgs e)
        {
            if (_primaryID == "0")
            {
                if (FormRole != "" && !AuthUserInfo.HasPermission(FormRole, AuthUserInfo.PermissionType.Insert))
                    return;
                else if (!tableLayoutPanel1.CheckEmptyText())
                    return;

                if ((await actions.HasDataAsync($"SELECT * FROM {_tableName} WHERE {_textColumn}=N'{txt.Text.Trim()}'")).HasData)
                {
                    AdvancedAlert.ShowAlert("ببورە پێشتر زیادکراوە", "آسف، تمت إضافتها من قبل", "Sorry it's already been added", AdvancedAlert.AlertType.Warning, 3);
                    return;
                }

                await actions.InsertDataAsync(_tableName, new Dictionary<string, object>() { { _textColumn, txt.Text.Trim() }, { "archived", 0 } }, false);
            }
            else
            {
                if (FormRole != "" && !AuthUserInfo.HasPermission(FormRole, AuthUserInfo.PermissionType.Update))
                    return;

                if ((await actions.HasDataAsync($"SELECT * FROM {_tableName} WHERE {_textColumn}=N'{txt.Text.Trim()}' AND {_primaryKeyColumn}<>{_primaryID}")).HasData)
                {
                    AdvancedAlert.ShowAlert("ببورە پێشتر زیادکراوە", "آسف، تمت إضافتها من قبل", "Sorry it's already been added", AdvancedAlert.AlertType.Warning, 3);
                    return;
                }

                var update = await actions.UpdateDataAsync(_tableName, new Dictionary<string, object>() { { _textColumn, txt.Text.Trim() } }, new Dictionary<string, object>() { { _primaryKeyColumn, _primaryID } }, false);
                if(update.Success)
                {
                    AdvancedAlert.ShowAlert("سەرکەوتوو بوو", "تم التحديث بنجاح", "Updated successfully", AdvancedAlert.AlertType.Success, 3);
                }
                else
                {
                    AdvancedAlert.ShowAlert("هەڵەیەک ڕوویدا", "حدث خطأ", update.ErrorMessage, AdvancedAlert.AlertType.Error, 3);
                }
            }
            _primaryID = "0";
            txt.Focus();
            RefreshDGV();
        }

        private async void dgvCellClick(object sender, DataGridViewCellEventArgs e)
        {
            _primaryID = DGV.CurrentRow.Cells[_primaryKeyColumn].Value.ToString();
            string columnName = DGV.Columns[e.ColumnIndex].Name;

            if (columnName == "deleteBtn")
            {
                if (FormRole != "" && !AuthUserInfo.HasPermission(FormRole, AuthUserInfo.PermissionType.Delete))
                    return;
                else if (!AdvancedMessageBox.ShowMessageBox("دڵنیای لە ئەنجامدانی ئەم کردارە؟", "هل انت متأكد من أنك تريد أن تفعل هذا؟", "Are you sure ?", "سڕینەوە", "مسح", "Delete", AdvancedMessageBox.MessageBoxType.YesNo))
                    return;

                await actions.UpdateDataAsync(_tableName, new Dictionary<string, object>() { { "archived", 1 } }, new Dictionary<string, object>() { { _primaryKeyColumn, _primaryID } }, false);
                RefreshDGV();
                _primaryID = "0";
                txt.Focus();
            }
            else if (DGV.Columns[e.ColumnIndex].Name == "updateBtn")
            {
                SelectDataToTextBox();
                txt.Focus();
                txt.SelectAll();
            }
        }
        private void AdvancedAddDialogForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
        private void txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSave.PerformClick();
            }
        }

        private async void RefreshDGV()
        {
            DGV.Columns.Clear();
            DGV.DataSource = (await actions.GetDataAsync($"SELECT {_primaryKeyColumn},{_textColumn} FROM {_tableName} WHERE archived=0 {(_whereCondition.IsEmpty() ? "" : $" AND {_whereCondition}")} ORDER BY {_primaryKeyColumn} DESC")).Data;
            DGVColumnHeaderName();
            txt.Text = "";
        }
        private void DGVColumnHeaderName()
        {
            DGV.AddActionColumns(ButtonImage.Update, ButtonImage.Delete);

            DGV.Columns[_primaryKeyColumn].Width = 40;
            DGV.Columns["updateBtn"].Width = 30;
            DGV.Columns["deleteBtn"].Width = 30;

            DGV.Columns[_primaryKeyColumn].HeaderText = "#";
            DGV.Columns[_textColumn].HeaderText = _dgvColumnHeaderText;
        }
        private void SelectDataToTextBox()
        {
            if (DGV.Rows.Count <= 0) return;
            txt.Text = DGV.CurrentRow.Cells[_textColumn].Value.ToString();
        }

    }
}
